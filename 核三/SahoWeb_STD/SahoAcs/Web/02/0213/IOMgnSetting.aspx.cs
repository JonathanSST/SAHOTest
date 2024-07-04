using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using SahoAcs.DB.DBInfo;
using SMS_Communication;
using Sa.DB;
using System.Net;
using System.Net.Sockets;
using DapperDataObjectLib;
using Newtonsoft;


namespace SahoAcs
{
    public partial class IOMgnSetting : Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        //private int _pagesize = 20;
        Hashtable TableInfo = new Hashtable();
        #endregion

        DB_Acs MyoAcsDB = null;
        OrmDataObject Myodo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        #region 設定靜態屬性，方便 Webthod 使用

        private static string[] strDci = new string[6];
        private static string[] strIOMaster = new string[10];
        private static string[] strIom = new string[11];
        private static string[] strSensor = new string[12];
      
        private static string[] string_Dci = new string[14];
        private static string[] string_Para_T = new string[3];
        private static string[] string_Para_C = new string[5];
        private static string[] string_Message = new string[2];

        public int MaxCtrls = 100;
        public int CurrentCtrls = 0;
        private static string sUserID = "";      //儲存目前使用者的UserID
        private static string sUserName = "";    //儲存目前使用者的UserName

        //儲存目前 Dci、Master、Controller、Reader、Equ 的值，以便更新LOG使用
        private static DBReader drTemp = new DBReader();
        private static DataTable dtTemp = new DataTable();
        private static string string_DoorAccess = "";
        private static string string_Elevator = "";
        private static string string_TRT = "";

        private static string string_Dci_Insert_Failed = "";
        private static string string_Dci_Update_Failed = "";
        private static string string_Dci_Delete_Failed = "";

        private static string string_Data_Used = "";

        List<LocInfo> locInfos = new List<LocInfo>();

        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MyoAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            this.MaxCtrls = DBClass.DongleVaries.GetMaxCtrls();

            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;

            #region ControlToJavaScript
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";
            #endregion

            string jsFileEnd = "<script src=\"IOMgnSetting_End.js?" + DateTime.Now.ToString("yyyyMMddHHmmss") + "\" Type =\"text/javascript\"></script>";
            ClientScript.RegisterStartupScript(typeof(string), "IOMgnSetting_End", jsFileEnd, false);

            ScriptManager.RegisterStartupScript(TreeView_UpdatePanel, TreeView_UpdatePanel.GetType(), "OnPageLoad", js, false);

            RegisterTreeViewJS();           // 處理樹狀結構
            ClientScript.RegisterClientScriptInclude("IOMgnSetting", "IOMgnSetting.js?" + Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案

            ClientScript.RegisterClientScriptInclude("jqueryMin", Pub.JqueyNowVer);
            ClientScript.RegisterClientScriptInclude("jqueryUI", "/Scripts/jquery-ui.js");

            //#region 設定彈跳視窗
            //Pub.SetModalPopup(ModalPopupExtender1, 1);
            //#endregion

            #region 註冊主頁Button動作
            //Master_Input_Type_TCPIP.Attributes.Add("Onclick", "SetParamDiv('IPParam');");
            //Master_Input_Type_COMPort.Attributes.Add("Onclick", "SetParamDiv('ComPortParam');");

            Dci_B_Add.Attributes["onClick"] = "InsertDciExcute(); return false;";
            Dci_B_Edit.Attributes["onClick"] = "UpdateDciExcute(); return false;";
            Dci_B_Delete.Attributes["onClick"] = "DeleteDciExcute(); return false;";
            Dci_B_Cancel.Attributes["onClick"] = "SetDivMode(''); return false;";

            IOMaster_B_Add.Attributes["onClick"] = "InsertIOMasterExcute(); return false;";
            IOMaster_B_Edit.Attributes["onClick"] = "UpdateIOMasterExcute(); return false;";
            IOMaster_B_Delete.Attributes["onClick"] = "DeleteIOMasterExcute(); return false;";
            IOMaster_B_Cancel.Attributes["onClick"] = "SetDivMode(''); return false;";

            Iom_B_Add.Attributes["onClick"] = "InsertIOModuleExcute(); return false;";
            Iom_B_Edit.Attributes["onClick"] = "UpdateIOModuleExcute(); return false;";
            Iom_B_Delete.Attributes["onClick"] = "DeleteIOModuleExcute(); return false;";
            Iom_B_Cancel.Attributes["onClick"] = "SetDivMode(''); return false;";

            Sen_B_Add.Attributes["onClick"] = "InsertSensorExcute(); return false;";
            Sen_B_Edit.Attributes["onClick"] = "UpdateSensorExcute(); return false;";
            Sen_B_Delete.Attributes["onClick"] = "DeleteSensorExcute(); return false;";
            Sen_B_Cancel.Attributes["onClick"] = "SetDivMode(''); return false;";

            //this.btnAddEquData.Attributes["onClick"] = "AddEquData(); return false;";
            //btnFilter.Attributes["onClick"] = "CallSearch('" + this.GetLocalResourceObject("CallSearch_Title") + "'); return false;";

            //ddlEquClass.Attributes["onChange"] = "SetControlStatus(); return false;";

            //Master_Input_CtrlModel.Attributes["onload"] = "GetEquClassListItem(); return false;";
            //Master_Input_CtrlModel.Attributes["onChange"] = "GetEquClassListItem(); return false;";

            // 參數設定
            ParaButton.Attributes["onClick"] = "CallParaSetting();return false;";
            CtrlParaButton.Attributes["onClick"] = "CallCtrlParaSetting();return false;";

            #endregion

            //#region 註冊pop1頁Button動作
            //ImgCloseButton1.Attributes["onClick"] = "CancelTrigger1.click(); return false;";
            //btnSearch.Attributes["onClick"] = "SearchEquData('" + txtKeyWord.Text.Trim() + "'); return false;";
            //#endregion


            #endregion

            #region 取 resource 裡面的值填 屬性

            strDci[0] = GetLocalResourceObject("ttConnNo").ToString();
            strDci[1] = GetLocalResourceObject("ttConnName").ToString();
            strDci[2] = GetLocalResourceObject("ttLockedIP").ToString();
            strDci[3] = GetLocalResourceObject("ttIP").ToString();
            strDci[4] = GetLocalResourceObject("ttPort").ToString();
            strDci[5] = GetLocalResourceObject("ttConnPW").ToString();

            strIOMaster[0] = "連線裝置編號";      // 連線裝置編號 IOMstNo
            strIOMaster[1] = "連線裝置名稱";      // 連線裝置說明 IOMstName
            strIOMaster[2] = "IP PORT";           // IP + PORT IOMstConnParam
            strIOMaster[3] = "自動回傳";          // 自動回傳  IOAuthReturn
            strIOMaster[4] = "區域";              // 區域
            strIOMaster[5] = "棟別";              // 棟別
            strIOMaster[6] = "樓層";              // 樓層
            strIOMaster[7] = "連線裝置機型";      // 連線裝置機型  IOMstModel
            strIOMaster[8] = "控制器機型";        // 控制器機型   CtrlModel  
            strIOMaster[9] = "狀態";

            strIom[0] = "IO模組編號" ;
            strIom[1] = "IO模組名稱";
            strIom[2] = "機號";
            strIom[3] = "用途";
            strIom[4] = "對應控制器";
            strIom[5] = "接點數";
            strIom[6] = "區域";
            strIom[7] = "棟別";
            strIom[8] = "樓層";
            strIom[9] = "狀態";
            strIom[10] = "說明";

            strSensor[0] = "偵測器編號";
            strSensor[1] = "偵測器名稱";
            strSensor[2] = "IO模組ID";
            strSensor[3] = "Bit數";
            strSensor[4] = "觸發訊號";
            strSensor[5] = "區域";
            strSensor[6] = "棟別";
            strSensor[7] = "樓層";
            strSensor[8] = "狀態";
            strSensor[9] = "警報類型";
            strSensor[10] = "說明";
            strSensor[11] = "警報秒數";

            string_Data_Used = GetLocalResourceObject("string_Data_Used").ToString();

            string_Dci_Insert_Failed = GetLocalResourceObject("string_Dci_Insert_Failed").ToString();
            string_Dci_Update_Failed = GetLocalResourceObject("string_Dci_Update_Failed").ToString();
            string_Dci_Delete_Failed = GetLocalResourceObject("string_Dci_Delete_Failed").ToString();

            string_Dci[0] = GetLocalResourceObject("string_Dci_00").ToString();
            string_Dci[1] = GetLocalResourceObject("string_Dci_01").ToString();
            string_Dci[2] = GetLocalResourceObject("string_Dci_02").ToString();
            string_Dci[3] = GetLocalResourceObject("string_Dci_03").ToString();
            string_Dci[4] = GetLocalResourceObject("string_Dci_04").ToString();
            string_Dci[5] = GetLocalResourceObject("string_Dci_05").ToString();
            string_Dci[6] = GetLocalResourceObject("string_Dci_06").ToString();
            string_Dci[7] = GetLocalResourceObject("string_Dci_07").ToString();
            string_Dci[8] = GetLocalResourceObject("string_Dci_08").ToString();
            string_Dci[9] = GetLocalResourceObject("string_Dci_09").ToString();
            string_Dci[10] = GetLocalResourceObject("string_Dci_10").ToString();
            string_Dci[11] = GetLocalResourceObject("string_Dci_11").ToString();
            string_Dci[12] = GetLocalResourceObject("string_Dci_12").ToString();
            string_Dci[13] = GetLocalResourceObject("string_Dci_13").ToString();
           
            string_Para_T[0] = GetLocalResourceObject("string_Para_T_0").ToString();
            string_Para_T[1] = GetLocalResourceObject("string_Para_T_1").ToString();
            string_Para_T[2] = GetLocalResourceObject("string_Para_T_2").ToString();

            string_Para_C[0] = GetLocalResourceObject("string_Para_C_0").ToString();
            string_Para_C[1] = GetLocalResourceObject("string_Para_C_1").ToString();
            string_Para_C[2] = GetLocalResourceObject("string_Para_C_2").ToString();
            string_Para_C[3] = GetLocalResourceObject("string_Para_C_3").ToString();
            string_Para_C[4] = GetLocalResourceObject("string_Para_C_4").ToString();

            string_Message[0] = GetLocalResourceObject("string_Message_0").ToString();
            string_Message[1] = GetLocalResourceObject("string_Message_1").ToString();
          
            string_DoorAccess = GetLocalResourceObject("ddlEquClass_DoorAccess").ToString();
            string_Elevator = GetLocalResourceObject("ddlEquClass_Elevator").ToString();
            string_TRT = GetLocalResourceObject("ddlEquClass_TRT").ToString();

            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                // Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");

                sUserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                sUserName = Sa.Web.Fun.GetSessionStr(this.Page, "UserName");

                LoadIOment("");
                TreeView_UpdatePanel.Update();
                IOMaster_CreateCtrlModel();
                GetLocationListItem();

                Div_Dci.Attributes["style"] = "display:none";
                Div_IOMaster.Attributes["style"] = "display:none";
                Div_IOModule.Attributes["style"] = "display:none";
                Div_Sensor.Attributes["style"] = "display:none";

                if (Session["SahoWebSocket"] != null)
                {
                    ((SahoWebSocket)Session["SahoWebSocket"]).ClearCmdResult();
                }
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                //string sFormArg = Request.Form["__EVENTARGUMENT"];

                string[] sFormArg = Request.Form["__EVENTARGUMENT"].ToString().Split('_');

                if (sFormTarget == this.TreeView_UpdatePanel.ClientID)
                {
                    if (sFormArg.GetUpperBound(0) == 0)
                    {
                        if (sFormArg[0] == "Refalsh")
                        {
                            txt_NodeTypeList.Value = "";
                            txt_NodeIDList.Value = "";
                            LoadIOment("");
                            TreeView_UpdatePanel.Update();
                            EquOrg_TreeView.ExpandAll();
                            GetLocationListItem();
                        }
                    }
                    else
                    {
                        if (sFormArg[0] == "Refalsh")
                        {
                            txt_NodeTypeList.Value = "";
                            txt_NodeIDList.Value = "";
                            LoadIOment("");
                            TreeView_UpdatePanel.Update();
                            EquOrg_TreeView.ExpandAll();
                            GetLocationListItem();

                            string strClass = sFormArg[1].ToString().Trim();    // sClass
                            string strAct = sFormArg[2].ToString().Trim();      // sAct
                            string strID = sFormArg[3].ToString().Trim();       // sID
                            string sUp = "";

                            if (sFormArg.Length > 4)
                            {
                                sUp = sFormArg[4].ToString().Trim();       // sUp
                            }

                            // 移動 TreeView_Panel 的 SCROLLBAR，和傳送需展開的資料
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "LoadAffterAction", string.Format("LoadAffterAction('{0}','{1}','{2}','{3}');", strClass, strAct, strID, sUp), true);
                        }
                    }
                }

                // 清除 __EVENTTARGET、__EVENTARGUMENT 的值
                Sa.Web.Fun.RunJavaScript(this,
                @" theForm.__EVENTTARGET.value   = '' ;
                   theForm.__EVENTARGUMENT.value = '' ; ");
            }
        }
        #endregion

        #endregion

        #region Method

        #region LoadIOment
        private void LoadIOment(string strFilter)
        {
            string sql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            EquOrg_TreeView.Nodes.Clear();

            EquOrg_TreeView.Attributes.Add("Onclick", "OpenWin();");

            TreeNode RootNode = new TreeNode();

            // 語系切換：RootNode.Text = "門禁管理系統";
            RootNode.Text = this.GetLocalResourceObject("RootNode").ToString();

            txt_NodeTypeList.Value += "SMS,";
            txt_NodeIDList.Value += "0,";
            RootNode.SelectAction = TreeNodeSelectAction.Select;

            List<IODciInfo> DciTree = new List<IODciInfo>();

            sql = " SELECT DciID, DciNo, DciName FROM B01_DeviceConnInfo ORDER BY DciNo ";

            this.MyoAcsDB.GetDataTable("DciTable", sql, liSqlPara, out dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IODciInfo DciNode = new IODciInfo();
                DciNode.DciID = int.Parse(dt.Rows[i]["DciID"].ToString());
                DciNode.DciNo = dt.Rows[i]["DciNo"].ToString();
                DciNode.DciName = dt.Rows[i]["DciName"].ToString();
                DciTree.Add(DciNode);
            }

            LoadDCiTree(RootNode, DciTree);
            EquOrg_TreeView.Nodes.Add(RootNode);
            EquOrg_TreeView.ShowLines = true;

     
            //取得控制器資料
            dt.Clear();
            this.IomDDLCtrlK3.Items.Clear();
            dt = this.Myodo.GetDataTableBySql("select CtrlID, CtrlNo, CtrlName from B01_Controller where CtrlModel IN ('SC300','SCM320') ORDER BY CtrlNo ");
            foreach (DataRow dr in dt.Rows)
            {
                ListItem Item = new ListItem();
                Item.Text = "[" + dr["CtrlNo"].ToString() + "]" + dr["CtrlName"].ToString();
                Item.Value = dr["CtrlID"].ToString();
                this.IomDDLCtrlK3.Items.Add(Item);
            }
            this.IomDDLCtrlK3.Items.Insert(0, new ListItem("選取資料", ""));

            //取得警報類型
            dt.Clear();
            this.SenDDLAlmType.Items.Clear();
            dt = this.Myodo.GetDataTableBySql("SELECT Code, StateDesc FROM B00_CardLogState ");
            foreach (DataRow dr in dt.Rows)
            {
                ListItem Item = new ListItem();
                Item.Text =  dr["StateDesc"].ToString();
                Item.Value = dr["Code"].ToString();
                this.SenDDLAlmType.Items.Add(Item);
            }
        }
        #endregion

        #region LoadDCiTree
        private void LoadDCiTree(TreeNode PNode, List<IODciInfo> objDciTree)
        {
            string sql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            List<IOMasterInfo> IOMasterTree;

            for (int i = 0; i < objDciTree.Count; i++)
            {
                IOMasterTree = new List<IOMasterInfo>();
                sql = "";
                liSqlPara.Clear();
                TreeNode SubNode = new TreeNode();

                // 語系切換：string_Connection = 連線 / Connection
                string string_Connection = GetLocalResourceObject("string_Connection").ToString();

                SubNode.Text += "[" + objDciTree[i].DciNo + "] " + string_Connection + " - (" + objDciTree[i].DciName + ")";
                txt_NodeTypeList.Value += "IODCI,";
                txt_NodeIDList.Value += objDciTree[i].DciID.ToString() + ",";
                SubNode.NavigateUrl = "#";

                sql = " SELECT IOMstID, IOMstNo, IOMstName, IOMstStatus from B01_IOMaster WHERE DciID = ? ORDER BY IOMstNo ";
                liSqlPara.Add("S:" + objDciTree[i].DciID);

                this.MyoAcsDB.GetDataTable("IOMasterTable", sql, liSqlPara, out dt);

                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    IOMasterInfo IOMasterNode = new IOMasterInfo();
                    IOMasterNode.IOMstID = int.Parse(dt.Rows[k]["IOMstID"].ToString());
                    IOMasterNode.IOMstNo = dt.Rows[k]["IOMstNo"].ToString();
                    IOMasterNode.IOMstName = dt.Rows[k]["IOMstName"].ToString();
                    IOMasterNode.IOMstStatus = dt.Rows[k]["IOMstStatus"].ToString();

                    IOMasterTree.Add(IOMasterNode);
                }
                objDciTree[i].IOMasterList = IOMasterTree;
                LoadIOMasterTree(SubNode, objDciTree[i].IOMasterList);
                PNode.ChildNodes.Add(SubNode);
            }
        }
        #endregion

        #region LoadIOMasterTree
        private void LoadIOMasterTree(TreeNode PNode, List<IOMasterInfo> objIOMasterTree)
        {
            string sql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            List<IOModuleInfo> IOModuleTree;

            for (int i = 0; i < objIOMasterTree.Count; i++)
            {
                IOModuleTree = new List<IOModuleInfo>();
                sql = "";
                liSqlPara.Clear();
                TreeNode SubNode = new TreeNode();

                // 語系切換：string_Connection = 連線 / Connection
                string string_Connenect_Device = GetLocalResourceObject("string_Connenect_Device").ToString();

                SubNode.Text += "[" + objIOMasterTree[i].IOMstNo + "] I/O" + string_Connenect_Device + " - (" + objIOMasterTree[i].IOMstName + ")";

                // wei 2017/5/8 加入圖片判斷
                if (objIOMasterTree[i].IOMstStatus == "1")
                {
                    SubNode.ImageUrl = "~/Img/22.png";
                }
                else if (objIOMasterTree[i].IOMstStatus == "2")
                {
                    SubNode.ImageUrl = "~/Img/42.png";
                }
                else
                {
                    SubNode.ImageUrl = "~/Img/43.png";
                }

                txt_NodeTypeList.Value += "IOMASTER,";
                txt_NodeIDList.Value += objIOMasterTree[i].IOMstID.ToString() + ",";
                SubNode.NavigateUrl = "#";

                sql = " SELECT IOMID, IOMNo, IOMName, IOMStatus FROM B01_IOModule WHERE IOMstID = ? ORDER BY IOMNo ";
                liSqlPara.Add("S:" + objIOMasterTree[i].IOMstID);
                this.MyoAcsDB.GetDataTable("IOModuleTable", sql, liSqlPara, out dt);

                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    this.CurrentCtrls++;
                    IOModuleInfo IomModuleNode = new IOModuleInfo();
                    IomModuleNode.IOMID = int.Parse(dt.Rows[k]["IOMID"].ToString());
                    IomModuleNode.IOMNo = dt.Rows[k]["IOMNo"].ToString();
                    IomModuleNode.IOMName = dt.Rows[k]["IOMName"].ToString();
                    IomModuleNode.IOMStatus = dt.Rows[k]["IOMStatus"].ToString();

                    IOModuleTree.Add(IomModuleNode);
                }

                objIOMasterTree[i].IOModuleList = IOModuleTree;
                LoadIOModuleTree(SubNode, objIOMasterTree[i].IOModuleList);
                PNode.ChildNodes.Add(SubNode);
            }
        }
        #endregion

        #region LoadIOModuleTree
        private void LoadIOModuleTree(TreeNode PNode, List<IOModuleInfo> objIOModuleTree)
        {
            string sql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            for (int i = 0; i < objIOModuleTree.Count; i++)
            {
                sql = "";
                liSqlPara.Clear();
                TreeNode SubNode = new TreeNode();
                SubNode.Text = "[" + objIOModuleTree[i].IOMNo + "] " + objIOModuleTree[i].IOMName;

                // wei 2017/5/8 加入圖片判斷
                if (objIOModuleTree[i].IOMStatus == "1")
                {
                    SubNode.ImageUrl = "~/Img/22.png";
                }
                else if (objIOModuleTree[i].IOMStatus == "2")
                {
                    SubNode.ImageUrl = "~/Img/42.png";
                }
                else
                {
                    SubNode.ImageUrl = "~/Img/43.png";
                }

                txt_NodeTypeList.Value += "IOMODULE,";
                txt_NodeIDList.Value += objIOModuleTree[i].IOMID.ToString() + ",";
                SubNode.NavigateUrl = "#";

                #region Process String
                sql = " SELECT SenID, SenNo, SenName, SenStatus FROM B01_Sensor WHERE IOMID = ?  ORDER BY SenNo";
                #endregion

                liSqlPara.Add("S:" + objIOModuleTree[i].IOMID);

                this.MyoAcsDB.GetDataTable("SersorTable", sql, liSqlPara, out dt);

                SensorInfo SersorNode;
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    SersorNode = new SensorInfo();
                    SersorNode.SenID = int.Parse(dt.Rows[k]["SenID"].ToString());
                    SersorNode.SenNo = dt.Rows[k]["SenNo"].ToString();
                    SersorNode.SenName = dt.Rows[k]["SenName"].ToString();
                    SersorNode.SenStatus = dt.Rows[k]["SenStatus"].ToString();

                    BuildSensorTree(SubNode, SersorNode);
                }

                PNode.ChildNodes.Add(SubNode);
            }
        }
        #endregion

        #region BuildSensorTree
        private void BuildSensorTree(TreeNode PNode, SensorInfo objSensorList)
        {
            TreeNode SubNode = new TreeNode();
            if (objSensorList.SenStatus == "1")
            {
                SubNode.ImageUrl = "~/Img/22.png";
            }
            else if (objSensorList.SenStatus == "2")
            {
                SubNode.ImageUrl = "~/Img/42.png";
            }
            else
            {
                SubNode.ImageUrl = "~/Img/43.png";
            }


            SubNode.Text = "[" + objSensorList.SenNo + "] " + objSensorList.SenName;

            txt_NodeTypeList.Value += "SENSOR,";
            txt_NodeIDList.Value += objSensorList.SenID.ToString() + ",";
            SubNode.NavigateUrl = "#";
            PNode.ChildNodes.Add(SubNode);
            PNode.Collapse();
        }
        #endregion

        #region DeviceConnInfo 相關

        #region LoadDeviceConnInfo

        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadDeviceConnInfo(string DciID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            DBReader dr = new DBReader();
            bool isSuccess = true;

            sql = @" 
                SELECT 
                    DciNo,
                    DciName,
                    IsAssignIP,
                    IpAddress,
                    TcpPort,
                    DciPassWD, 
                    DciID 
                FROM B01_DeviceConnInfo
                WHERE DciID = ? ";
            liSqlPara.Add("I:" + DciID.Trim());

            try
            {
                isSuccess = oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                if (isSuccess)
                {
                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {
                            EditData = new string[dr.DataReader.FieldCount];
                            for (int i = 0; i < dr.DataReader.FieldCount; i++)
                            {
                                EditData[i] = dr.DataReader[i].ToString();
                            }

                            drTemp.Free();
                            drTemp = dr;
                        }
                        else
                        {
                            EditData = new string[2];
                            EditData[0] = "Saho_SysErrorMassage";
                            EditData[1] = "Can Not Read DataReader！";
                        }
                    }
                    else
                    {
                        EditData = new string[2];
                        EditData[0] = "Saho_SysErrorMassage";
                        EditData[1] = "No Data！";
                    }
                }
                else
                {
                    EditData = new string[2];
                    EditData[0] = "Saho_SysErrorMassage";
                    EditData[1] = "Load Database failure！";
                }
            }
            catch (Exception ex)
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = string.Format("[LoadDeviceConnInfo Exception] {0}", ex.Message);
            }

            return EditData;
        }

        #endregion

        #region InsertDeviceConnInfo
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object InsertDeviceConnInfo(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            MessageObject1 objRet = new MessageObject1();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            // 驗證輸入的值
            objRet = Check_Input_DB_DeviceConnInfo(DataArray, "Insert");

            if (objRet.result)
            {
                sql = @" 
                    INSERT INTO B01_DeviceConnInfo 
                    (
                        [DciNo],
                        [DciName],
                        [IsAssignIP],
                        [IpAddress],
                        [TcpPort],
                        [DciPassWD], 
                        CreateUserID, CreateTime, UpdateUserID, UpdateTime
                    )  
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?) ";

                string strDciNo = DataArray[0].Trim();          // 設備連線編碼　　DciNo
                string strDciName = DataArray[1].Trim();        // 設備連線名稱　　DciName
                string strIsAssignIP = DataArray[2].Trim();     // 限制IP位置　　　IsAssignIP
                string strIpAddress = DataArray[3].Trim();      // IP位置　　　　　IpAddress
                string strTcpPort = DataArray[4].Trim();        // 主動回傳端口　　TcpPort 
                string strDciPassWD = DataArray[5].Trim();      // 連線密碼　　　　DciPassWD

                liSqlPara.Add("A:" + strDciNo);
                liSqlPara.Add("S:" + strDciName);
                liSqlPara.Add("I:" + strIsAssignIP);
                liSqlPara.Add("A:" + strIpAddress);
                liSqlPara.Add("I:" + strTcpPort);
                liSqlPara.Add("A:" + strDciPassWD);
                liSqlPara.Add("A:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("A:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);

                try
                {
                    int intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);

                    if (intResult > -1)
                    {
                        #region 寫入B00_SysLog
                        int[] intAry = new int[] { 0, 1, 2, 3, 4, 5 };
                        string[] strAry = new string[] { "DciNo", "DciName", "IsAssignIP", "IpAddress", "TcpPort", "DciPassWD" };

                        List<Machine> liData = new List<Machine>();
                        int r = 0;
                        foreach (int g in intAry)
                        {
                            //string strOldValue = drTemp.DataReader.GetValue(g).ToString();
                            string strNewValue = DataArray[g].Trim();

                            liData.Add(new Machine(strAry[r], strDci[r], "", strNewValue));
                            r++;
                        }

                        oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, sUserID, sUserName, "020102",
                            DataArray[0].Trim(),
                            DataArray[1].Trim(),
                            string.Format("{0}", CompareVaule("Insert", liData)),
                            "新增連線");
                        #endregion

                        #region 取得剛剛新增的DciNo的DciID回傳使用
                        sql = @" 
                            SELECT TOP 1 DciID FROM B01_DeviceConnInfo 
                            WHERE DciNo = ? AND DciName = ? AND CreateTime = ?";

                        liSqlPara.Clear();
                        liSqlPara.Add("S:" + strDciNo);
                        liSqlPara.Add("S:" + strDciName);
                        liSqlPara.Add("D:" + Time);

                        objRet.message = oAcsDB.GetStrScalar(sql, liSqlPara);
                        #endregion

                        #region 取得這個 DciNO 和 MstNo 的順位，依此計算出新增的DeviceConnInfo，在整體中的順位
                        int intUp = 0;

                        if (strDciNo == null) strDciNo = "";

                        /*
                            1. 取小於等於 DciNO 的 B01_DeviceConnInfo 的數量
                            2. 取小於 DciNO 的 B01_Master 的數量
                            3. 取小於 DciNO 的 B01_Controller 的數量
                            4. 取小於 DciNO 的 B01_Reader 的數量

                            依此計算出TREEVIEW在資料新增後，需要移動位置的數值
                        */
                        sql = string.Format(@"                
                            SELECT 
	                            (SELECT COUNT(*) FROM B01_DeviceConnInfo WHERE DciNO <= '{0}') +
	                            (
		                            SELECT COUNT(*) FROM B01_Master M 
		                            INNER JOIN B01_DeviceConnInfo D ON D.DciID = M.DciID 
		                            WHERE D.DciNO < '{0}'
	                            ) +  
	                            (
		                            SELECT COUNT(*) FROM B01_Controller WHERE MstID IN 
		                            (
			                            SELECT MstID FROM B01_Master M 
			                            INNER JOIN B01_DeviceConnInfo D ON D.DciID = M.DciID 
			                            WHERE D.DciNO < '{0}'
		                            )
	                            ) +
	                            (
		                            SELECT COUNT(*) FROM B01_Reader WHERE CtrlID IN 
		                            (
			                            SELECT CtrlID FROM B01_Controller WHERE MstID IN 
			                            (
				                            SELECT MstID FROM B01_Master M
				                            INNER JOIN B01_DeviceConnInfo D ON D.DciID = M.DciID 
				                            WHERE D.DciNO < '{0}'
			                            )
		                            )
	                            ) ", strDciNo);

                        try
                        {
                            intUp = oAcsDB.GetIntScalar(sql);
                            if (intUp < 0) intUp = 0;
                        }
                        catch
                        {
                            intUp = 1;
                        }

                        objRet.sUp = intUp.ToString();
                        #endregion
                    }
                    else
                    {
                        objRet.message = string_Dci_Insert_Failed;  // 新增連線失敗
                        objRet.result = false;
                    }
                }
                catch (Exception ex)
                {
                    objRet.message = string.Format("[InsertDeviceConnInfo] {0}", ex.Message);
                    objRet.result = false;
                }
            }

            objRet.act = "InsertDeviceConnInfo";
            return objRet;
        }
        #endregion

        #region UpdateDeviceConnInfo
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object UpdateDeviceConnInfo(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            objRet = Check_Input_DB_DeviceConnInfo(DataArray, "Update");

            if (objRet.result)
            {
                string strDciNo = DataArray[0].Trim();          // 設備連線編碼　　DciNo
                string strDciName = DataArray[1].Trim();        // 設備連線名稱　　DciName
                string strIsAssignIP = DataArray[2].Trim();     // 限制IP位置　　　IsAssignIP
                string strIpAddress = DataArray[3].Trim();      // IP位置　　　　　IpAddress
                string strTcpPort = DataArray[4].Trim();        // 主動回傳端口　　TcpPort 
                string strDciPassWD = DataArray[5].Trim();      // 連線密碼　　　　DciPassWD
                string strDciID = DataArray[6].Trim();          // DciID

                sql = @" 
                    UPDATE B01_DeviceConnInfo SET
                        DciNo = ?,
                        DciName = ?,
                        IsAssignIP = ?,
                        IpAddress = ?,
                        TcpPort = ?,
                        DciPassWD = ?, 
                        UpdateUserID = ?, 
                        UpdateTime = ? 
                    WHERE DciID = ? ";

                liSqlPara.Add("S:" + strDciNo);
                liSqlPara.Add("S:" + strDciName);
                liSqlPara.Add("I:" + strIsAssignIP);
                liSqlPara.Add("S:" + strIpAddress);
                liSqlPara.Add("I:" + strTcpPort);
                liSqlPara.Add("S:" + strDciPassWD);
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("I:" + strDciID);

                try
                {
                    int intRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);

                    if (intRet > 0)
                    {
                        #region 寫入B00_SysLog
                        int[] intAry = new int[] { 0, 1, 2, 3, 4, 5 };
                        string[] strAry = new string[] { "DciNo", "DciName", "IsAssignIP", "IpAddress", "TcpPort", "DciPassWD" };

                        List<Machine> liData = new List<Machine>();
                        int r = 0;
                        foreach (int g in intAry)
                        {
                            string strOldValue = drTemp.DataReader.GetValue(g).ToString();
                            string strNewValue = DataArray[g].Trim();

                            liData.Add(new Machine(strAry[r], strDci[r], strOldValue, strNewValue));
                            r++;
                        }

                        oAcsDB.WriteLog(DB_Acs.Logtype.資料修改, sUserID, sUserName, "020102",
                            drTemp.DataReader.GetValue(0).ToString(),
                            drTemp.DataReader.GetValue(1).ToString(),
                            string.Format("設備連線編碼：{0}，{1}", strDciNo, CompareVaule("Update", liData)),
                            "修改連線");
                        #endregion

                        objRet.message = strDciID;  // 取得DctID回傳使用
                    }
                    else
                    {
                        objRet.message = string_Dci_Update_Failed;  // 更新連線失敗;
                        objRet.result = false;
                    }
                }
                catch (Exception ex)
                {
                    objRet.message = string.Format("[UpdateDeviceConnInfo] {0}", ex.Message);
                    objRet.result = false;
                }
            }

            objRet.act = "UpdateDeviceConnInfo";
            return objRet;
        }
        #endregion

        #region DeleteDeviceConnInfo
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object DeleteDeviceConnInfo(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DBReader dr = new DBReader();

            string strDciNo = DataArray[0].Trim();          // 設備連線編碼　　DciNo
            string strDciName = DataArray[1].Trim();        // 設備連線名稱　　DciName
            string strIsAssignIP = DataArray[2].Trim();     // 限制IP位置　　　IsAssignIP
            string strIpAddress = DataArray[3].Trim();      // IP位置　　　　　IpAddress
            string strTcpPort = DataArray[4].Trim();        // 主動回傳端口　　TcpPort 
            string strDciPassWD = DataArray[5].Trim();      // 連線密碼　　　　DciPassWD
            string strDciID = DataArray[6].Trim();          // DciID

            sql = @" 
                SELECT 1 FROM B01_Master AS Mtr
                LEFT JOIN B01_DeviceConnInfo AS Dci ON Dci.DciID = Mtr.DciID
                WHERE Dci.DciID = ? ";
            liSqlPara.Add("I:" + strDciID);

            try
            {
                bool isSuccess = oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                if (isSuccess)
                {
                    if (dr.HasRows)
                    {
                        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                        objRet.result = false;
                        objRet.message += string_Data_Used;     //此項目已被使用，無法刪除
                    }

                    if (objRet.result)
                    {
                        sql = " DELETE FROM B01_DeviceConnInfo WHERE DciID = ? ";
                        liSqlPara.Clear();
                        liSqlPara.Add("I:" + strDciID);

                        int intRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);

                        if (intRet > 0)
                        {
                            #region 寫入B00_SysLog
                            oAcsDB.WriteLog(DB_Acs.Logtype.資料刪除, sUserID, sUserName, "020102",
                                drTemp.DataReader.GetValue(0).ToString(),
                                drTemp.DataReader.GetValue(1).ToString(),
                                string.Format("設備連線編碼：{0}", strDciNo),
                                "刪除連線");
                            #endregion

                            objRet.message = strDciID;  // 取得DctID回傳使用
                        }
                        else
                        {
                            objRet.message = string_Dci_Delete_Failed;  // 刪除連線失敗
                            objRet.result = false;
                        }
                    }
                }
                else
                {
                    objRet.message = string_Dci[13];  // 資料庫程式處理失敗，請再試一次
                    objRet.result = false;
                }
            }
            catch (Exception ex)
            {
                objRet.message = string.Format("[DeleteDeviceConnInfo] {0}", ex.Message);
                objRet.result = false;
            }

            objRet.act = "DeleteDeviceConnInfo";
            return objRet;
        }
        #endregion

        #region Check_Input_DB_DeviceConnInfo
        protected static MessageObject1 Check_Input_DB_DeviceConnInfo(string[] DataArray, string Mode)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            MessageObject1 objRet = new MessageObject1();
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DBReader dr = new DBReader();

            string strDciNo = DataArray[0].Trim();          // 設備連線編碼　　DciNo
            string strDciName = DataArray[1].Trim();        // 設備連線名稱　　DciName
            string strIsAssignIP = DataArray[2].Trim();     // 限制IP位置　　　IsAssignIP
            string strIpAddress = DataArray[3].Trim();      // IP位置　　　　　IpAddress
            string strTcpPort = DataArray[4].Trim();        // 主動回傳端口　　TcpPort 
            string strDciPassWD = DataArray[5].Trim();      // 連線密碼　　　　DciPassWD
            string strDciID = DataArray[6].Trim();          // DciID

            #region Input

            // 設備連線編碼
            if (string.IsNullOrEmpty(DataArray[0].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：設備連線編碼 必須輸入 
                objRet.message += string_Dci[0];
            }
            else if (Encoding.Default.GetBytes(DataArray[0].Trim()).Length > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：設備連線編碼 字數超過上限 
                objRet.message += string_Dci[1];
            }

            // 設備連線名稱
            if (string.IsNullOrEmpty(DataArray[1].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：設備連線名稱 必須輸入 
                objRet.message += string_Dci[2];
            }
            else if (Encoding.Default.GetBytes(DataArray[1].Trim()).Length > 60)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：設備連線名稱 字數超過上限 
                objRet.message += string_Dci[3];
            }

            // IP位置
            if (string.IsNullOrEmpty(DataArray[3].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：IP位置 必須輸入 
                objRet.message += string_Dci[4];
            }
            else if (Encoding.Default.GetBytes(DataArray[3].Trim()).Length > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：IP位置 字數超過上限 
                objRet.message += string_Dci[5];
            }

            if (objRet.result && !CheckIP(DataArray[3].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：IP位置不合法
                objRet.message += string_Dci[6];
            }

            // 主動回傳端口
            if (string.IsNullOrEmpty(DataArray[4].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：主動回傳端口 必須輸入 
                objRet.message += string_Dci[7];
            }
            else if (Encoding.Default.GetBytes(DataArray[4].Trim()).Length > 5)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：主動回傳端口 字數超過上限 
                objRet.message += string_Dci[8];
            }

            if (objRet.result && !CheckPort(DataArray[4].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：主動回傳端口不符規格，設定值應為1~65535
                objRet.message += string_Dci[9];
            }

            // 連線密碼
            if (string.IsNullOrEmpty(DataArray[5].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：連線密碼 必須輸入 
                objRet.message += string_Dci[10];
            }
            else if (Encoding.Default.GetBytes(DataArray[5].Trim()).Length > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：連線密碼 字數超過上限 
                objRet.message += string_Dci[11];
            }

            #endregion

            #region DB
            if (objRet.result)
            {
                switch (Mode)
                {
                    case "Insert":
                        sql = @" SELECT DciNo FROM B01_DeviceConnInfo WHERE DciNo = ? ";
                        liSqlPara.Add("S:" + strDciNo);
                        break;
                    case "Update":
                        sql = @" SELECT DciNo FROM B01_DeviceConnInfo WHERE DciNo = ? AND DciID != ? ";
                        liSqlPara.Add("S:" + strDciNo);
                        liSqlPara.Add("S:" + strDciID);
                        break;
                }

                bool isSuccess = oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                if (isSuccess)
                {
                    if (dr.Read())
                    {
                        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                        objRet.result = false;

                        // 訊息：此設備連線編碼已存在於系統中
                        objRet.message += string_Dci[12];
                    }
                }
                else
                {
                    objRet.result = false;

                    // 訊息：資料庫程式處理失敗，請再試一次
                    objRet.message += string_Dci[13];
                }
            }
            #endregion

            return objRet;
        }
        #endregion

        #endregion

        #region 產生B01_Location 下拉選單
        private void GetLocationListItem()
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt = new DataTable();

            sql = @"
                select * from (
                select LocType, LocID, LocNo, LocName, LocPID from B01_Location where LocType = 'AREA'
                UNION
                select LocType, LocID, LocNo, LocName, LocPID from B01_Location where LocType = 'BUILDING' 
                UNION 
                select LocType, LocID, LocNo, LocName, LocPID from B01_Location where LocType = 'FLOOR' 
                ) as R
                ORDER BY R.LocType, LocID
            ";

            this.txtAreaList.Value = "";
            this.txtBuildingList.Value = "";
            this.txtFloorList.Value = "";

            oAcsDB.GetDataTable("LocInfo", sql, liSqlPara, out dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["LocType"].ToString() == "AREA")
                {
                    this.txtAreaList.Value += $"{dt.Rows[i]["LocID"].ToString()}|{dt.Rows[i]["LocNo"].ToString()}|{dt.Rows[i]["LocName"].ToString()}|{dt.Rows[i]["LocPID"].ToString()},";
                }
                if (dt.Rows[i]["LocType"].ToString() == "BUILDING")
                {
                    this.txtBuildingList.Value += $"{dt.Rows[i]["LocID"].ToString()}|{dt.Rows[i]["LocNo"].ToString()}|{dt.Rows[i]["LocName"].ToString()}|{dt.Rows[i]["LocPID"].ToString()},";
                }
                if (dt.Rows[i]["LocType"].ToString() == "FLOOR")
                {
                    this.txtFloorList.Value += $"{dt.Rows[i]["LocID"].ToString()}|{dt.Rows[i]["LocNo"].ToString()}|{dt.Rows[i]["LocName"].ToString()}|{dt.Rows[i]["LocPID"].ToString()},";
                }
            }

            this.txtAreaList.Value = this.txtAreaList.Value.TrimEnd(',');
            this.txtBuildingList.Value = this.txtBuildingList.Value.TrimEnd(',');
            this.txtFloorList.Value = this.txtFloorList.Value.TrimEnd(',');
        }

        #endregion

        #region Master 相關

        #region IOMaster_CreateCtrlModel
        private void IOMaster_CreateCtrlModel()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt = new DataTable();

            this.IOMaster_Input_CtrlModel.Items.Clear();

            // 選取資料 / Select Data
            ListItem Item = new ListItem();
            Item.Text = GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString();
            Item.Value = "";
            this.IOMaster_Input_CtrlModel.Items.Add(Item);

            sql = @" SELECT DISTINCT ItemName, ItemNo FROM B00_ItemList WHERE ItemClass = 'EquModel' ";
            oAcsDB.GetDataTable("EquNoTable", sql, liSqlPara, out dt);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new ListItem();
                Item.Text = dr["ItemName"].ToString();
                Item.Value = dr["ItemNo"].ToString();
                this.IOMaster_Input_CtrlModel.Items.Add(Item);
            }
        }
        #endregion

        #region GetDciInfo
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] GetDciInfo(string FromDciID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            DBReader dr = new DBReader();

            sql = @" SELECT DciID, DciName FROM B01_DeviceConnInfo WHERE DciID = ? ";
            liSqlPara.Add("I:" + FromDciID.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                EditData = new string[dr.DataReader.FieldCount];
                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                    EditData[i] = dr.DataReader[i].ToString();
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "No Data！";
            }

            return EditData;
        }
        #endregion

        #region LoadIOMaster
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadIOMaster(string IOMstID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            DBReader dr = new DBReader();

            #region Process String
            sql = @" 
                SELECT 
                  Mst.IOMstNO, Mst.IOMstName, Dci.DciName, Mst.IOMstConnParam, IOAutoReturn, 
                  dbo.Get_LocParentID(Mst.LocID, 'AREA') AS 'AREA', dbo.Get_LocParentID(Mst.LocID, 'BUILDING') AS 'BUILDING', 
                  dbo.Get_LocParentID(Mst.LocID, 'FLOOR') AS 'FLOOR', Mst.IOMstModel, Mst.IOMstStatus , Mst.CtrlModel, Dci.DciID , Mst.IOMstID
                FROM B01_IOMaster AS Mst
                LEFT JOIN B01_DeviceConnInfo AS Dci ON Dci.DciID = Mst.DciID
                WHERE Mst.IOMstID = ? ";

            liSqlPara.Add("I:" + IOMstID.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                EditData = new string[dr.DataReader.FieldCount];
                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                {
                    EditData[i] = dr.DataReader[i].ToString();
                }

                drTemp.Free();
                drTemp = dr;
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "No Data！";
            }
            #endregion

            return EditData;
        }
        #endregion

        #region InsertIOMaster
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object InsertIOMaster(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            MessageObject1 objRet = new MessageObject1();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            // 驗證輸入的值
            objRet = Check_Input_DB_IOMaster(DataArray, "Insert");

            if (objRet.result)
            {
                string[] strTrmp = DataArray[3].Split('_');

                if (strTrmp[0] != "")
                {
                    IPAddress address = null;
                    IPAddress.TryParse(strTrmp[0], out address);

                    if (address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        DataArray[3] = DataArray[3].Replace("_", ":");
                    }
                    else if (address.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        DataArray[3] = DataArray[3].Replace("_", ",");
                    }
                }

                string sLocID = DataArray[7]; //預設為樓層
                if (string.IsNullOrEmpty(sLocID))
                {
                    sLocID = DataArray[6];
                }
                if (string.IsNullOrEmpty(sLocID))
                {
                    sLocID = DataArray[5];
                }

                #region Process String
                sql = @" 
                    INSERT INTO B01_IOMaster 
                    (
                        IOMstNo, IOMstName, DciID, IOMstConnParam, IOAutoReturn, IOMstModel, CtrlModel, 
                        LocID, IOMstStatus, CreateUserID, CreateTime
                    ) 
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?) ";

                liSqlPara.Add("S:" + DataArray[0].Trim());      // 連線裝置編號
                liSqlPara.Add("S:" + DataArray[1].Trim());      // 連線裝置說明
                liSqlPara.Add("I:" + DataArray[11].Trim());      // DciID
                liSqlPara.Add("S:" + DataArray[3].Trim());      // IP + PORT
                liSqlPara.Add("I:" + DataArray[4].Trim());      // 自動回傳
                liSqlPara.Add("S:" + DataArray[8].Trim());      // 連線裝置機型
                liSqlPara.Add("S:" + DataArray[10].Trim());     // 設備機型
                liSqlPara.Add("I:" + sLocID);                   // LocID
                liSqlPara.Add("I:" + DataArray[9].Trim());      // 狀態
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);

                try
                {
                    int intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);

                    if (intResult > -1)
                    {
                        #region 寫入B00_SysLog
                        int[] intAry = new int[] { 0, 1, 3, 4, 5, 6, 7, 8, 10, 9 };
                        string[] strAry = new string[] { "IOMstNo", "IOMstName", "IOMstConnParam", "IOAutoReturn", "Area", "Building", "Floor", "IOMstModel", "CtrlModel", "IOMstStatus" };

                        List<Machine> liData = new List<Machine>();
                        int r = 0;
                        foreach (int g in intAry)
                        {
                            //string strOldValue = drTemp.DataReader.GetValue(g).ToString();
                            string strNewValue = DataArray[g].Trim();

                            liData.Add(new Machine(strAry[r], strIOMaster[r], "", strNewValue));
                            r++;
                        }

                        oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, sUserID, sUserName, "0213",
                            DataArray[0].Trim(),
                            DataArray[1].Trim(),
                            string.Format("{0}", CompareVaule("Insert", liData)), "新增I/O連線裝置");
                        #endregion

                        #region 取得剛剛新增的IOMstNo的IOMstID回傳使用
                        sql = @" 
                            SELECT TOP 1 IOMstID FROM B01_IOMaster 
                            WHERE IOMstNo = ? AND DciID = ? ";

                        liSqlPara.Clear();
                        liSqlPara.Add("S:" + DataArray[0].Trim());
                        liSqlPara.Add("I:" + DataArray[11].Trim());

                        objRet.message = oAcsDB.GetStrScalar(sql, liSqlPara);
                        #endregion

                        //#region 取得這個 DciNO 和 MstNo 的順位，依此計算出新增的Master，在整體中的順位
                        //int intUp = 0;

                        //string strDciNo = oAcsDB.GetStrScalar(string.Format(@"SELECT DciNO FROM B01_DeviceConnInfo WHERE DciID = {0}", DataArray[10].Trim()));

                        //if (strDciNo == null) strDciNo = "";

                        ///*
                        //    1. 取小於等於 DciNO 的 B01_DeviceConnInfo 的數量
                        //    2. 取小於 DciNO 的 B01_Master 的數量
                        //    3. 取等於 DciNO 和 小於 MstNo 的 B01_Master 的數量
                        //    4. 取小於 DciNO 的 B01_Controller 的數量
                        //    5. 取等於 DciNO 和 小於 MstNo 的 B01_Controller 的數量
                        //    6. 取小於 DciNO 的 B01_Reader 的數量
                        //    7. 取等於 DciNO 和 小於 MstNo 的 B01_Reader 的數量

                        //    依此計算出TREEVIEW在資料新增後，需要移動位置的數值
                        //*/
                        //sql = string.Format(@"                
                        //    SELECT 
                        //     (SELECT COUNT(*) FROM B01_DeviceConnInfo WHERE DciNO <= '{0}') +
                        //     (
                        //      SELECT COUNT(*) FROM B01_Master M 
                        //      INNER JOIN B01_DeviceConnInfo D ON D.DciID = M.DciID 
                        //      WHERE D.DciNO < '{0}'
                        //     ) + 
                        //     (
                        //      SELECT COUNT(*) FROM B01_Master M 
                        //      INNER JOIN B01_DeviceConnInfo D ON D.DciID = M.DciID 
                        //      WHERE D.DciNO = '{0}' AND M.MstNo < '{1}' 
                        //     ) + 
                        //     (
                        //      SELECT COUNT(*) FROM B01_Controller WHERE MstID IN 
                        //      (
                        //       SELECT MstID FROM B01_Master M 
                        //       INNER JOIN B01_DeviceConnInfo D ON D.DciID = M.DciID 
                        //       WHERE D.DciNO < '{0}'
                        //      )
                        //     ) +
                        //     (
                        //      SELECT COUNT(*) FROM B01_Controller WHERE MstID IN 
                        //      (
                        //       SELECT MstID FROM B01_Master M
                        //       INNER JOIN B01_DeviceConnInfo D ON D.DciID = M.DciID 
                        //       WHERE D.DciNO = '{0}' AND M.MstNo < '{1}'
                        //      )
                        //     ) +
                        //     (
                        //      SELECT COUNT(*) FROM B01_Reader WHERE CtrlID IN 
                        //      (
                        //       SELECT CtrlID FROM B01_Controller WHERE MstID IN 
                        //       (
                        //        SELECT MstID FROM B01_Master M
                        //        INNER JOIN B01_DeviceConnInfo D ON D.DciID = M.DciID 
                        //        WHERE D.DciNO < '{0}'
                        //       )
                        //      )
                        //     ) + 
                        //     (
                        //      SELECT COUNT(*) FROM B01_Reader WHERE CtrlID IN 
                        //      (
                        //       SELECT CtrlID FROM B01_Controller WHERE MstID IN 
                        //       (
                        //        SELECT MstID FROM B01_Master M
                        //        INNER JOIN B01_DeviceConnInfo D ON D.DciID = M.DciID 
                        //        WHERE D.DciNO = '{0}' AND M.MstNo < '{1}'
                        //       )
                        //      )
                        //     )  ", strDciNo, DataArray[0].Trim());

                        //try
                        //{
                        //    intUp = oAcsDB.GetIntScalar(sql);
                        //    if (intUp < 0) intUp = 0;
                        //}
                        //catch
                        //{
                        //    intUp = 1;
                        //}

                        //objRet.sUp = intUp.ToString();
                        //#endregion
                    }
                    else
                    {
                        objRet.message = "新增I/O連線裝置失敗。";
                        objRet.result = false;
                    }
                }
                catch (Exception ex)
                {
                    objRet.message = string.Format("[InserIOtMaster] {0}", ex.Message);
                    objRet.result = false;
                }

                #endregion
            }

            objRet.act = "InsertIOMaster";
            return objRet;
        }
        #endregion

        #region UpdateIOMaster
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object UpdateIOMaster(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            objRet = Check_Input_DB_IOMaster(DataArray, "Update");

            if (objRet.result)
            {
                string[] strTrmp = DataArray[4].Split('_');

                if (strTrmp[0] != "")
                {
                    IPAddress address = null;
                    IPAddress.TryParse(strTrmp[0], out address);

                    if (address != null)
                    {
                        if (address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            DataArray[3] = DataArray[3].Replace("_", ":");
                        }
                        else if (address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            DataArray[3] = DataArray[3].Replace("_", ",");
                        }
                    }
                }

                string sLocID = DataArray[7]; //預設為樓層
                if (string.IsNullOrEmpty(sLocID))
                {
                    sLocID = DataArray[6];
                }
                if (string.IsNullOrEmpty(sLocID))
                {
                    sLocID = DataArray[5];
                }

                sql = @" 
                    UPDATE B01_IOMaster
                    SET 
                        IOMstNO = ?, IOMstName = ?, IOMstConnParam = ?, IOAutoReturn = ?, IOMstModel = ?, 
                        LocID = ?, IOMstStatus = ?, UpdateUserID = ?, UpdateTime = ?
                    WHERE IOMstID = ? ";

                liSqlPara.Add("S:" + DataArray[0].Trim());
                liSqlPara.Add("S:" + DataArray[1].Trim());
                liSqlPara.Add("S:" + DataArray[3].Trim());
                liSqlPara.Add("I:" + DataArray[4].Trim());
                liSqlPara.Add("S:" + DataArray[8].Trim());
                liSqlPara.Add("I:" + sLocID);
                liSqlPara.Add("I:" + DataArray[9].Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("I:" + DataArray[12].Trim());

                try
                {
                    int intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);

                    if (intResult > -1)
                    {
                        #region 寫入B00_SysLog
                        int[] intAry = new int[] { 0, 1, 3, 4, 5, 6, 7, 8, 10, 9 };
                        string[] strAry = new string[] { "IOMstNo", "IOMstName", "IOMstConnParam", "IOAutoReturn", "Area", "Building", "Floor", "IOMstModel", "CtrlModel", "IOMstStatus" };

                        List<Machine> liData = new List<Machine>();
                        int r = 0;
                        bool isUpdate = false;
                        foreach (int g in intAry)
                        {
                            string strOldValue = drTemp.DataReader.GetValue(g).ToString();
                            string strNewValue = DataArray[g].Trim();

                            if (g == 3 || g == 4 || g == 9)
                            {
                                if (strOldValue != strNewValue)
                                {
                                    isUpdate = true;
                                }
                            }

                            liData.Add(new Machine(strAry[r], strIOMaster[r], strOldValue, strNewValue));
                            r++;
                        }

                        if (isUpdate)
                        {
                            sql = "UPDATE B01_IOMaster SET isUpdate = 1 WHERE IOMstID = ? ";

                            liSqlPara.Clear();
                            liSqlPara.Add("I:" + DataArray[12].Trim());
                            oAcsDB.SqlCommandExecute(sql, liSqlPara);
                        }

                        oAcsDB.WriteLog(DB_Acs.Logtype.資料修改, sUserID, sUserName, "0213",
                            drTemp.DataReader.GetValue(0).ToString(),
                            drTemp.DataReader.GetValue(1).ToString(),
                            string.Format("I/O連線裝置編號：{0}，{1}", DataArray[0].Trim(), CompareVaule("Update", liData)),
                            "修改I/O連線裝置");
                        #endregion

                        objRet.message = DataArray[12].Trim();  // 取得IOMstID回傳使用
                    }
                    else
                    {
                        objRet.result = false;
                        objRet.message = "修改I/O連線裝置資料失敗。";
                    }
                }
                catch (Exception ex)
                {
                    objRet.result = false;
                    objRet.message = string.Format("[UpdateIOMaster] {0}", ex.Message);
                }
            }

            objRet.act = "UpdateIOMaster";
            return objRet;
        }
        #endregion

        #region DeleteIOMaster
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object DeleteIOMaster(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            int iRet = 0;
            List<string> liSqlPara = new List<string>();
            DBReader dr = new DBReader();

            #region 刪除 Master 資料
            if (objRet.result)
            {
                sql = @"
                    SELECT 1 FROM B01_IOModule Iom 
                    LEFT JOIN B01_IOMaster Mst ON Mst.IOMstID = Iom.IOMstID 
                    WHERE Iom.IOMstID = ? ";
                liSqlPara.Clear();
                liSqlPara.Add("I:" + DataArray[12].Trim());

                try
                {
                    bool isSuccess = oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                    if (isSuccess)
                    {
                        if (dr.HasRows)
                        {
                            if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                            objRet.result = false;
                            objRet.message += "此項目已被使用，無法刪除";     //此項目已被使用，無法刪除
                        }
                        else
                        {
                            #region 刪除連線裝置
                            if (iRet > -1)
                            {
                                sql = @" DELETE FROM B01_IOMaster WHERE IOMstID = ? ";
                                liSqlPara.Clear();
                                liSqlPara.Add("I:" + DataArray[12].Trim());
                                iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                            }
                            #endregion

                            if (iRet > -1)
                            {
                                #region 寫入B00_SysLog
                                oAcsDB.WriteLog(DB_Acs.Logtype.資料刪除, sUserID, sUserName, "0213",
                                    drTemp.DataReader.GetValue(0).ToString(),
                                    drTemp.DataReader.GetValue(1).ToString(),
                                    string.Format("連線裝置編號：{0}", DataArray[0]),
                                    "刪除I/O連線裝置");
                                #endregion

                                #region 取得MstID回傳使用
                                objRet.message = DataArray[12].Trim();
                                #endregion
                            }
                            else
                            {
                                objRet.message = "刪除I/O連線裝置失敗。";
                                objRet.result = false;
                            }
                        }
                    }
                    else
                    {
                        objRet.message = "資料庫程式處理失敗，請再試一次";  // 資料庫程式處理失敗，請再試一次
                        objRet.result = false;
                    }
                }
                catch (Exception ex)
                {
                    objRet.message = string.Format("[DeleteIOMaster] {0}", ex.Message);
                    objRet.result = false;
                }
            }
            #endregion

            objRet.act = "DeleteIOMaster";
            return objRet;
        }
        #endregion

        #region Check_Input_DB_IOMaster
        protected static MessageObject1 Check_Input_DB_IOMaster(string[] DataArray, string Mode)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            MessageObject1 objRet = new MessageObject1();
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DBReader dr = new DBReader();

            #region Input
            if (string.IsNullOrEmpty(DataArray[0].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：連線裝置編號 必須輸入 
                objRet.message += "連線裝置編號 必須輸入";
            }
            else if (DataArray[0].Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：連線裝置編號 字數超過上限 
                objRet.message += "連線裝置編號 字數超過上限 ";
            }

            if (DataArray[1].Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：連線裝置名稱 字數超過上限 
                objRet.message += "連線裝置名稱 字數超過上限";
            }

            if (string.IsNullOrEmpty(DataArray[2].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：使用連線 必須指定 
                objRet.message += "使用連線 必須指定";
            }

            #region 參數
            if (objRet.result)
            {
                string[] ParaArray = DataArray[3].Split('_');

                if (string.IsNullOrEmpty(ParaArray[0].Trim()))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：IP 必須輸入
                    objRet.message += "IP 必須輸入";
                }

                if (string.IsNullOrEmpty(ParaArray[1].Trim()))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：Port 必須輸入
                    objRet.message += "Port 必須輸入";
                }

                if (objRet.result && (!CheckIP(ParaArray[0]) || !CheckPort(ParaArray[1])))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：IP位置不合法
                    objRet.message += "IP位置不合法";
                }

                if (string.IsNullOrEmpty(DataArray[4].Trim()))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：自動回傳 必須指定
                    objRet.message += "自動回傳 必須指定";
                }

                if (string.IsNullOrEmpty(DataArray[5].Trim()))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：區域 必須指定
                    objRet.message += "區域 必須指定";
                }

                //if (string.IsNullOrEmpty(DataArray[6].Trim()))
                //{
                //    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                //    objRet.result = false;

                //    // 訊息：棟別 必須指定
                //    objRet.message += "棟別 必須指定";
                //}

                if (DataArray[8].Trim().Count() > 20)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：連線裝置機型 字數超過上限
                    objRet.message += "連線裝置機型 字數超過上限";
                }

                if (string.IsNullOrEmpty(DataArray[9].Trim()))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：狀態 必須指定
                    objRet.message += "狀態 必須指定";
                }

                if (string.IsNullOrEmpty(DataArray[10].Trim()))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：設備機型 必須指定
                    objRet.message += "設備機型 必須指定";
                }
            }
            #endregion


            #endregion

            #region DB
            if (objRet.result)
            {
                switch (Mode)
                {
                    case "Insert":
                        sql = @" SELECT * FROM B01_IOMaster WHERE IOMstNo = ? ";
                        liSqlPara.Add("S:" + DataArray[0].Trim());
                        break;
                    case "Update":
                        sql = @" SELECT * FROM B01_IOMaster WHERE IOMstNo = ? AND IOMstID != ? ";
                        liSqlPara.Add("S:" + DataArray[0].Trim());
                        liSqlPara.Add("S:" + DataArray[12].Trim());
                        break;
                }

                oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                if (dr.Read())
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：此連線裝置編號已存在於系統中
                    objRet.message += "此I/O連線裝置編號已存在於系統中";
                }
            }
            #endregion

            return objRet;
        }
        #endregion

        #endregion

        #region IOModule 相關

        #region GetMstInfo
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] GetIOMstInfo(string FromMstID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            DBReader dr = new DBReader();

            //sql = @" SELECT IOMstID, IOMstName, CtrlModel FROM B01_IOMaster WHERE IOMstID = ? ";
            sql = @" SELECT B01_IOMaster.DciID, DciName, IOMstID, IOMstName, CtrlModel FROM B01_IOMaster
                     LEFT JOIN B01_DeviceConnInfo ON B01_DeviceConnInfo.DciID = B01_IOMaster.DciID WHERE IOMstID = ? ";
            liSqlPara.Add("I:" + FromMstID.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (dr.Read())
            {
                EditData = new string[dr.DataReader.FieldCount];
                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                    EditData[i] = dr.DataReader[i].ToString();
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";

                // 訊息：系統中無此資料！
                EditData[1] = string_Message[1];
            }

            drTemp.Free();

            return EditData;
        }
        #endregion

        #region LoadIOModule
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadIOModule(string strUserID, string strIOMID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            DBReader dr = null;

            #region Process String
            sql = @" 
                    SELECT Iom.IOMNo, Iom.IOMName, Iom.IOMAddr, Iom.CtrlModel, IOMUsage, (Dci.DciName + ' / ' +  Mst.IOMstName) AS MstName, Iom.CtrlIDK3, Iom.IOMBits, 
                    dbo.Get_LocParentID(Iom.LocID, 'AREA') AS 'AREA', dbo.Get_LocParentID(Iom.LocID, 'BUILDING') AS 'BUILDING', 
                    dbo.Get_LocParentID(Iom.LocID, 'FLOOR') AS 'FLOOR', Iom.IOMStatus, Iom.IOMDesc, Mst.DciID, Mst.IOMstID, Iom.IOMID
                    FROM B01_IOModule Iom
                    LEFT JOIN B01_IOMaster Mst ON Mst.IOMstID = Iom.IOMstID
                    LEFT JOIN B01_DeviceConnInfo Dci ON Dci.DciID = Mst.DciID
                    WHERE Iom.IOMID = ? ";

            liSqlPara.Add("I:" + strIOMID.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (dr.Read())
            {
                EditData = new string[dr.DataReader.FieldCount];
                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                {
                    EditData[i] = dr.DataReader[i].ToString();
                }

                drTemp.Free();
                drTemp = dr;

            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";

                // 訊息：系統中無此資料！
                EditData[1] = string_Message[1];
            }
            #endregion

            return EditData;
        }

        public static void AddParaMeterForControl(string strUserID, string strCtrlID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            string strTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string strSQL = string.Format(@"
                 SELECT ED.EquID, ED.EquModel FROM B01_EquData ED 
                 INNER JOIN B01_Reader RD ON RD.EquNo=ED.EquNo 
                 INNER JOIN B01_Controller CR ON CR.CtrlID=RD.CtrlID 
                 WHERE CR.CtrlID={0}", strCtrlID);
            oAcsDB.GetDataReader(strSQL, out dr);

            if (dr != null)
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        // wei 20170327 補足缺的設備參數 hideUserID.value、EquID、EquModel
                        AddParaMeterForReader(strUserID, dr.ToInt32("EquID").ToString(), dr.ToString("EquModel"));
                    }
                }
            }
        }
        #endregion


        //[System.Web.Services.WebMethod()]
        //[System.Web.Script.Services.ScriptMethod()]
        //public static object NewInsertController(string UserID, string[] DataArray)
        //{

        //    DapperDataObjectLib.OrmDataObject odo = new DapperDataObjectLib.OrmDataObject("MsSql", Pub.GetDapperConnString());
        //    Pub.MessageObject objRet = CheckNewController(DataArray, "", "Insert", ref odo);
        //    string sql = "";
        //    if (objRet.result)
        //    {
        //        #region 新增控制器
        //        if (DataArray[1].Trim().Equals(""))
        //        {
        //            DataArray[1] = DataArray[0].Trim();
        //        }
        //        sql = @"INSERT INTO B01_Controller (CtrlNo,CtrlName,CtrlDesc,CtrlAddr,CtrlModel,CtrlStatus,MstID,CreateUserID,CreateTime,UpdateuserID,UpdateTime) 
        //                        VALUES (@CtrlNo,@CtrlName,@CtrlDesc,@CtrlAddr,@CtrlModel,@CtrlStatus,@MstID,@User,GETDATE(),@User,GETDATE())";
        //        int intResult = odo.Execute(sql, new { CtrlNo = DataArray[0], CtrlName = DataArray[1], CtrlDesc = DataArray[2], CtrlAddr = DataArray[6], CtrlModel = DataArray[4], CtrlStatus = DataArray[5], MstID = DataArray[8], User = UserID });
        //        #endregion

        //        //取得目前的連線設備編號
        //        int DciID = odo.GetIntScalar("SELECT DciID FROM B01_master WHERE MstID=@MstID", new { MstID = DataArray[8] });
        //        int CtrlID = -1;
        //        if (intResult > -1)
        //        {
        //            //取得新的控制器編號
        //            CtrlID = odo.GetIntScalar("SELECT IDENT_CURRENT('B01_Controller')");
        //            if (CtrlID == 0 || DciID == 0)
        //            {
        //                objRet.message = "新增控制器失敗。";
        //                objRet.result = false;
        //                objRet.act = "InsertController";
        //                return objRet;
        //            }
        //            else
        //            {
        //                objRet.message = CtrlID.ToString();
        //            }

        //        }
        //        else
        //        {
        //            objRet.message = "新增控制器失敗。";
        //            objRet.result = false;
        //            objRet.act = "InsertController";
        //            return objRet;
        //        }
        //        if (!GetInsertReadWithEquData(DataArray, CtrlID, DciID, UserID, ref odo))
        //        {
        //            odo.Execute("DELETE B01_Controller WHERE CtrlID=@CtrlID", new { CtrlID = CtrlID });
        //            objRet.message = "新增控制器失敗。";
        //            objRet.result = false;
        //        }
        //    }
        //    objRet.act = "InsertController";
        //    return objRet;
        //}

        public static bool GetInsertReadWithEquData(string[] DataArray, int CtrlID, int DciID, string UserID, ref OrmDataObject odo)
        {
            #region 新增暫存的讀卡機和暫存的設備
            string strReaderNo = "";
            string strReaderName = "";
            string strEquNo = "";
            string strEquName = "";
            string strEquEName = "";
            if (int.Parse(DataArray[12]) > 0)
            {
                #region 取得預設卡號長度
                string ss = "SELECT ParaValue FROM B00_SysParameter WHERE ParaNo='CardDefaultLength' ";
                string strCardLength = odo.GetStrScalar(ss);
                if (strCardLength == null || strCardLength == "")
                {
                    strCardLength = "10";
                }
                #endregion

                for (int i = 1; i <= int.Parse(DataArray[12]); i++)
                {
                    #region 新增預設讀卡機

                    #region 給預設值

                    strReaderNo = i.ToString();      // 讀卡機編號

                    // 1. 讀卡機名稱 = 控制器名稱_讀卡機編號
                    strReaderName = DataArray[1].Trim() + "_" + strReaderNo;

                    // 2. 設備編號 = 控制器編號_讀卡機編號
                    strEquNo = DataArray[0].Trim() + "_" + strReaderNo;

                    // 3. 設備名稱 = 控制器名稱_讀卡機編號 
                    strEquName = DataArray[1].Trim() + "_" + strReaderNo;

                    // 4. 設備英語名稱 = 設備名稱
                    strEquEName = strEquName;
                    #endregion


                    string sql1 = @" 
                            INSERT INTO B01_Reader 
                            (
                                ReaderNo,ReaderName,CtrlID,CreateUserID,CreateTime,
                                UpdateUserID,UpdateTime,ReaderDesc,EquNo,[Dir]  
                            ) VALUES (@ReaderNo,@ReaderName,@CtrlID,@UserID,GETDATE(),@UserID,GETDATE(),@ReaderDesc,@EquNo,@Dir)";

                    #endregion

                    #region 新增預設設備
                    string sql2 = @" 
	                        INSERT INTO B01_EquData 
	                        (
		                        EquClass,EquModel,EquNo,Building,[Floor],
		                        EquName,EquEName,DciID,CardNoLen,InToCtrlAreaID,
		                        OutToCtrlAreaID,IsAndTrt,  
		                        CreateUserID,CreateTime,UpdateUserID, UpdateTime 
	                        )  VALUES  (@EquClass,@EquModel,@EquNo,@Building,@Floor,@EquName,@EquEName,@DciID,@CardNoLen,@InToCtrlAreaID,@OutToCtrlAreaID,@IsAndTrt,@UserID,GETDATE(),@UserID, GETDATE()) ";

                    #endregion
                    var para = new
                    {
                        ReaderNo = strReaderNo,
                        Readername = strReaderName,
                        CtrlID = CtrlID,
                        UserID = UserID,
                        ReaderDesc = strReaderName,
                        EquEName = strEquEName,
                        EquNo = strEquNo,
                        Dir = "進",
                        EquClass = DataArray[9],
                        EquModel = DataArray[4],
                        Building = "",
                        Floor = "",
                        EquName = strEquName,
                        DciID = DciID,
                        CardNoLen = strCardLength,
                        InToCtrlAreaID = 0,
                        OutToCtrlAreaID = 0,
                        IsAndTrt = 0
                    };
                    int intResult1 = odo.Execute(sql1, para);
                    int intResult2 = odo.Execute(sql2, para);
                    if (intResult1 > 0 && intResult2 > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }




            }



            #endregion

            return false;
        }


        #region InsertIOModule
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object InsertIOModule(string UserID, string[] DataArray)
        {
            DapperDataObjectLib.OrmDataObject odo = new DapperDataObjectLib.OrmDataObject();
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "", sql1 = "", sql2 = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            List<string> liSqlPara1 = new List<string>();
            List<string> liSqlPara2 = new List<string>();
            DBReader dr = new DBReader();

            objRet = Check_Input_DB_IOModule(DataArray, "", "Insert", ref oAcsDB);

            #region 新增偵測器資料
            if (objRet.result)
            {
                #region 新增IO
                // 1. 偵測器名稱是空白，就帶入偵測器編號
                if (DataArray[1].Trim().Equals(""))
                {
                    DataArray[1] = DataArray[0].Trim();
                }

                if (DataArray[4] == "1")
                {
                    DataArray[7] = "3";
                }

                string sLocID = DataArray[10].Trim();
                if (string.IsNullOrEmpty(sLocID)) sLocID = DataArray[9].Trim();
                if (string.IsNullOrEmpty(sLocID)) sLocID = DataArray[8].Trim();



                sql = @" 
                    INSERT INTO B01_IOModule 
                    (
                        IOMNo, IOMName, IOMAddr, CtrlModel, IOMstID, IOMUsage, CtrlIDK3, 
                        IOMBits, LocID, IOMDesc, IOMStatus, CreateUserID, CreateTime 
                    )  
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?) ";

                liSqlPara.Clear();
                liSqlPara.Add("S:" + DataArray[0].Trim());
                liSqlPara.Add("S:" + DataArray[1].Trim());
                liSqlPara.Add("S:" + DataArray[2].Trim());
                liSqlPara.Add("S:" + DataArray[3].Trim());
                liSqlPara.Add("I:" + DataArray[5].Trim());
                liSqlPara.Add("I:" + DataArray[4].Trim());
                liSqlPara.Add("I:" + DataArray[6].Trim());
                liSqlPara.Add("S:" + DataArray[7].Trim());
                liSqlPara.Add("S:" + sLocID);
                liSqlPara.Add("S:" + DataArray[12].Trim());
                liSqlPara.Add("S:" + DataArray[11].Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);

                oAcsDB.BeginTransaction();

                int intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);

                int intIOMID = -1;
                int intDciID = -1;

                if (intResult > -1)
                {
                    sql = " SELECT IDENT_CURRENT('B01_IOModule') ";
                    intIOMID = oAcsDB.GetIntScalar(sql);

                    // 取得 Default DciID
                    sql = " SELECT DciID FROM B01_IOMaster WHERE IOMstID = ? ";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[5].Trim());
                    intDciID = oAcsDB.GetIntScalar(sql, liSqlPara);

                    if (intIOMID != -1 && intDciID != -1)
                    {
                        oAcsDB.Commit();
                    }
                    else
                    {
                        oAcsDB.Rollback();

                        objRet.message = "新增IO模組失敗。";
                        objRet.result = false;
                        objRet.act = "InsertController";
                        return objRet;
                    }
                }
                else
                {
                    oAcsDB.Rollback();

                    objRet.message = "新增IO模組失敗。";
                    objRet.result = false;
                    objRet.act = "InsertIOModule";
                    return objRet;
                }

                #endregion

                #region 新增暫存的偵測器

                string strSenNo = "";
                string strSenName = "";
                string strAlmType = "";
                int IoBit = 1;

                List<string[]> liLog = new List<string[]>();

                if (int.Parse(DataArray[7]) > 0)
                {
                    //SCM320 FOR SC300
                    if (DataArray[4] == "1")
                    {
                        for (int i = 1; i <= 3; i++)
                        {
                            if (i == 1)
                            {
                                strSenNo = DataArray[0].Trim() + "_KEY";
                                strSenName = DataArray[0].Trim() + "_鑰匙狀態";
                                strAlmType = "7";
                                IoBit = 1;
                            }
                            if (i == 2)
                            {
                                strSenNo = DataArray[0].Trim() + "_PUSH";
                                strSenName = DataArray[0].Trim() + "_按鈕狀態";
                                strAlmType = "1";
                                IoBit = 5;
                            }
                            if (i == 3)
                            {
                                strSenNo = DataArray[0].Trim() + "_DOOR";
                                strSenName = DataArray[0].Trim() + "_門位狀態";
                                strAlmType = "163";
                                IoBit = 9;
                            }

                            sql1 += @" 
                            INSERT INTO B01_Sensor 
                            (
                                SenNo, SenName, IOMID, IoBit, ActiveSignal, LocID, 
                                SenStatus, AlmType, CreateUserID, CreateTime
                            ) 
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                            liSqlPara1.Add("S:" + strSenNo);
                            liSqlPara1.Add("S:" + strSenName);
                            liSqlPara1.Add("I:" + intIOMID.ToString());
                            liSqlPara1.Add("I:" + IoBit);
                            liSqlPara1.Add("I:" + 1);
                            liSqlPara1.Add("S:" + sLocID);
                            liSqlPara1.Add("S:" + DataArray[11]);
                            liSqlPara1.Add("I:" + strAlmType);
                            liSqlPara1.Add("S:" + UserID.ToString());
                            liSqlPara1.Add("D:" + Time);

                            #region 取得新增LOG用的資料
                            string[] strLog = new string[2];

                            strLog[0] = strSenNo;
                            strLog[1] = strSenName;

                            liLog.Add(strLog);
                            #endregion
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= int.Parse(DataArray[7]); i++)
                        {
                            strSenNo = DataArray[0].Trim() + "_" + i.ToString("d2");
                            strSenName = strSenNo;

                            sql1 += @" 
                            INSERT INTO B01_Sensor 
                            (
                                SenNo, SenName, IOMID, IoBit, ActiveSignal, LocID, 
                                SenStatus, AlmType, CreateUserID, CreateTime
                            ) 
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                            liSqlPara1.Add("S:" + strSenNo);
                            liSqlPara1.Add("S:" + strSenName);
                            liSqlPara1.Add("I:" + intIOMID.ToString());
                            liSqlPara1.Add("I:" + i);
                            liSqlPara1.Add("I:" + 1);
                            liSqlPara1.Add("S:" + sLocID);
                            liSqlPara1.Add("S:" + DataArray[11]);
                            liSqlPara1.Add("I:" + 25);
                            liSqlPara1.Add("S:" + UserID.ToString());
                            liSqlPara1.Add("D:" + Time);

                            #region 取得新增LOG用的資料
                            string[] strLog = new string[2];

                            strLog[0] = strSenNo;
                            strLog[1] = strSenName;

                            liLog.Add(strLog);
                            #endregion
                        }
                    }

                    oAcsDB.BeginTransaction();

                    int intResult1 = oAcsDB.SqlCommandExecute(sql1, liSqlPara1);

                    if (intResult1 > 0)
                    {
                        oAcsDB.Commit();

                        #region 寫入B00_SysLog
                        #region 新增IO模組
                        int[] intAry = new int[] { 0, 1, 2, 4, 6, 7, 8, 9, 10, 11, 12 };
                        string[] strAry = new string[] { "IOMNo", "IOMName", "IOMAddr", "IOMUsage", "CtrlIDK3", "IOMBits", "Area", "Building", "Floor", "IOMStatus", "IOMDesc" };

                        List<Machine> liData = new List<Machine>();
                        int r = 0;
                        foreach (int g in intAry)
                        {
                            string strNewValue = DataArray[g].Trim();

                            liData.Add(new Machine(strAry[r], strIom[r], "", strNewValue));
                            r++;
                        }

                        oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, sUserID, sUserName, "0213",
                            DataArray[0].Trim(),
                            DataArray[1].Trim(),
                            string.Format("{0}", CompareVaule("Insert", liData)), "IO模組");
                        #endregion

                        #region 新增偵測器
                        foreach (string[] sArray in liLog)
                        {
                            oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, sUserID, sUserName, "0213",
                                sArray[0], sArray[1],
                                string.Format("偵測器編號：{0}，偵測器名稱：{1}", sArray[0], sArray[1]),
                                "新增偵測器(預設值)");
                        }
                        #endregion
                        #endregion

                        #region 取得剛剛新增的IOMID回傳使用
                        objRet.message = intIOMID.ToString();
                        #endregion

                        // 新增設備後的處理(新增設備(EquID)到全群組(EG999)、給新增的設備，設定設備參數預設值)
                        //string strIOMID = intIOMID.ToString();
                        //AfterAddReaderHealWith(UserID, strIOMID);
                    }
                    else
                    {
                        oAcsDB.Rollback();

                        #region 交易失敗，刪掉剛剛新增的IO模組
                        string strDel = " DELETE FROM B01_IOModule WHERE IOMID = ?";

                        liSqlPara.Clear();
                        liSqlPara.Add("I:" + intIOMID);
                        oAcsDB.SqlCommandExecute(strDel, liSqlPara);
                        #endregion

                        objRet.message = "新增IO模組失敗。";
                        objRet.result = false;
                    }
                }
                #endregion
            }
            #endregion

            objRet.act = "InsertIOModule";
            return objRet;
        }
        #endregion

        #region UpdateIOModule
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object UpdateIOModule(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            objRet = Check_Input_DB_IOModule(DataArray, DataArray[15], "Update", ref oAcsDB);
            #region 編輯IO模組資料
            if (objRet.result)
            {
                string sLocID = DataArray[10].Trim();
                if (string.IsNullOrEmpty(sLocID)) sLocID = DataArray[9].Trim();
                if (string.IsNullOrEmpty(sLocID)) sLocID = DataArray[8].Trim();

                sql = @"
                        UPDATE B01_IOModule SET 
                          IOMNo = ?, IOMName = ?, IOMAddr = ?, CtrlIDK3 = ?, LocID = ?, 
                          IOMStatus = ?, IOMDesc = ? , UpdateUserID = ?, UpdateTime = ? 
                        WHERE IOMID = ? ";

                liSqlPara.Add("S:" + DataArray[0].Trim());
                liSqlPara.Add("S:" + DataArray[1].Trim());
                liSqlPara.Add("S:" + DataArray[2].Trim());
                liSqlPara.Add("S:" + DataArray[6].Trim());
                liSqlPara.Add("I:" + sLocID);
                liSqlPara.Add("I:" + DataArray[11].Trim());
                liSqlPara.Add("S:" + DataArray[12].Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("I:" + DataArray[15].Trim());

                int intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);

                #region syslog:update
                if (intResult > -1)
                {
                    #region 寫入B00_SysLog
                    int[] intAry = new int[] { 0, 1, 2, 4, 6, 7, 8, 9, 10, 11, 12 };
                    string[] strAry = new string[] { "IOMNo", "IOMName", "IOMAddr", "IOMUsage", "CtrlIDK3", "IOMBits", "Area", "Building", "Floor", "IOMStatus", "IOMDesc" };

                    List<Machine> liData = new List<Machine>();
                    int r = 0;
                    bool isUpdate = false;
                    foreach (int g in intAry)
                    {
                        string strOldValue = drTemp.DataReader.GetValue(g).ToString();
                        string strNewValue = DataArray[g].Trim();

                        if (g == 11)
                        {
                            if (strOldValue != strNewValue)
                            {
                                isUpdate = true;
                            }
                        }

                        liData.Add(new Machine(strAry[r], strIom[r], strOldValue, strNewValue));
                        r++;
                    }

                    if (isUpdate)
                    {
                        sql = "UPDATE B01_IOModule SET isUpdate = 1 WHERE IOMID = ? ";

                        liSqlPara.Clear();
                        liSqlPara.Add("I:" + DataArray[15].Trim());
                        oAcsDB.SqlCommandExecute(sql, liSqlPara);
                    }

                    oAcsDB.WriteLog(DB_Acs.Logtype.資料修改, sUserID, sUserName, "0213",
                        drTemp.DataReader.GetValue(0).ToString(),
                        drTemp.DataReader.GetValue(1).ToString(),
                        string.Format("{0}", CompareVaule("Update", liData)),
                        "修改IO模組");
                    #endregion

                    objRet.message = DataArray[15].Trim();
                }
                else
                {
                    objRet.result = false;
                    objRet.message = "修改IO模組資料失敗。";
                }
                #endregion

            }
            #endregion
            objRet.act = "UpdateIOModule";
            return objRet;
        }
        #endregion

        #region DeleteIOModule
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object DeleteIOModule(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            int iRet = 0;
            List<string> liSqlPara = new List<string>();

            #region 刪除IO資料
            if (objRet.result)
            {
                oAcsDB.BeginTransaction();

                #region 刪除控制器所屬的所有設備相關資料

                //#region 刪除 B01_EquParaData 裡面的資料(控制器所屬全部)
                //if (iRet > -1)
                //{
                //    sql = @" 
                //        DELETE FROM [B01_EquParaData] 
                //        WHERE EquID IN 
                //        (
                //            SELECT EquID FROM B01_EquData WHERE EquNo IN
                //         (
                //          SELECT EquNo FROM B01_Reader WHERE CtrlID = ?   
                //         )
                //        )";
                //    liSqlPara.Clear();
                //    liSqlPara.Add("I:" + DataArray[11].Trim());
                //    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                //}
                //#endregion

                //#region 刪除 B01_EquGroupData 裡面的資料(控制器所屬全部)
                //if (iRet > -1)
                //{
                //    sql = @" 
                //        DELETE FROM [B01_EquGroupData] 
                //        WHERE EquID IN 
                //     (
                //            SELECT EquID FROM B01_EquData WHERE EquNo IN
                //         (
                //          SELECT EquNo FROM B01_Reader WHERE CtrlID = ?   
                //         )
                //        )";
                //    liSqlPara.Clear();
                //    liSqlPara.Add("I:" + DataArray[11].Trim());
                //    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                //}
                //#endregion

                //#region 刪除 B01_EquDataExt 裡面的資料(控制器所屬全部)
                //if (iRet > -1)
                //{
                //    sql = @" 
                //        DELETE FROM B01_EquDataExt 
                //        WHERE EquID IN 
                //     (
                //            SELECT EquID FROM B01_EquData WHERE EquNo IN
                //         (
                //          SELECT EquNo FROM B01_Reader WHERE CtrlID = ?   
                //         )
                //        )";
                //    liSqlPara.Clear();
                //    liSqlPara.Add("I:" + DataArray[11].Trim());
                //    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                //}
                //#endregion

                //#region 刪除 B01_ElevatorFloor 裡面的資料(控制器所屬全部)
                //if (iRet > -1)
                //{
                //    sql = @" 
                //        DELETE FROM B01_ElevatorFloor 
                //        WHERE EquID IN 
                //        (
                //            SELECT EquID FROM B01_EquData WHERE EquNo IN
                //         (
                //          SELECT EquNo FROM B01_Reader WHERE CtrlID = ?   
                //         )
                //        )";
                //    liSqlPara.Clear();
                //    liSqlPara.Add("I:" + DataArray[11].Trim());
                //    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                //}
                //#endregion

                //#region 刪除 B01_CardAuth 裡面的資料(控制器所屬全部)
                //if (iRet > -1)
                //{
                //    sql = @" 
                //        DELETE FROM B01_CardAuth 
                //        WHERE EquID IN 
                //        (
                //            SELECT EquID FROM B01_EquData WHERE EquNo IN
                //         (
                //          SELECT EquNo FROM B01_Reader WHERE CtrlID = ?   
                //         )
                //        )";
                //    liSqlPara.Clear();
                //    liSqlPara.Add("I:" + DataArray[11].Trim());
                //    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                //}
                //#endregion

                //#region 刪除 B01_CardEquAdj 裡面的資料(控制器所屬全部)
                //if (iRet > -1)
                //{
                //    sql = @" 
                //        DELETE FROM B01_CardEquAdj 
                //        WHERE EquID IN 
                //        (
                //            SELECT EquID FROM B01_EquData WHERE EquNo IN
                //         (
                //          SELECT EquNo FROM B01_Reader WHERE CtrlID = ?   
                //         )
                //        )";
                //    liSqlPara.Clear();
                //    liSqlPara.Add("I:" + DataArray[11].Trim());
                //    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                //}
                //#endregion

                //#region 刪除 B01_EquData 裡面的資料(控制器所屬全部)
                //if (iRet > -1)
                //{
                //    sql = @" 
                //        DELETE FROM [B01_EquData] 
                //        WHERE EquNo IN 
                //     (
                //            SELECT EquNo FROM B01_Reader WHERE CtrlID = ?  
                //        )";
                //    liSqlPara.Clear();
                //    liSqlPara.Add("I:" + DataArray[11].Trim());
                //    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                //}
                //#endregion

                #region 刪除 B01_Sensor 裡面的資料(IO所屬全部)
                if (iRet > -1)
                {
                    sql = @" DELETE FROM B01_Sensor WHERE IOMID = ? ";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[15].Trim());
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #endregion

                #region 刪除IO模組
                if (iRet > -1)
                {
                    sql = @" DELETE FROM B01_IOModule WHERE IOMID = ? ";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[15].Trim());
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                if (iRet > -1)
                {
                    oAcsDB.Commit();

                    #region 寫入B00_SysLog
                    oAcsDB.WriteLog(DB_Acs.Logtype.資料刪除, sUserID, sUserName, "0213",
                        drTemp.DataReader.GetValue(0).ToString(),
                        drTemp.DataReader.GetValue(1).ToString(),
                        string.Format("IO模組編號：{0}", DataArray[0]), "刪除IO模組");
                    #endregion

                    objRet.message = DataArray[15].Trim();
                }
                else
                {
                    oAcsDB.Rollback();

                    objRet.message = "刪除IO模組及其所屬的偵測器失敗。";
                    objRet.result = false;
                }

            }
            #endregion
            objRet.act = "DeleteIOModule";
            return objRet;
        }
        #endregion

        #region Check_Input_DB_Controller
        protected static Pub.MessageObject Check_Input_DB_IOModule(string[] DataArray, string ClickItem, string Mode, ref DB_Acs oAcsDB)
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            int tempint;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Input
            if (string.IsNullOrEmpty(DataArray[0].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "IO編號 必須輸入";
            }
            else if (DataArray[0].Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "IO編號 字數超過上限";
            }

            if (DataArray[1].Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "IO名稱 字數超過上限";
            }

            if (string.IsNullOrEmpty(DataArray[2].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "機號 必須指定";
            }
            else if (!int.TryParse(DataArray[2].Trim(), out tempint))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "機號 必須為數值";
            }

            if (string.IsNullOrEmpty(DataArray[3].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "設備機型 必須指定";
            }

            if (string.IsNullOrEmpty(DataArray[4].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "IO用途 必須指定";
            }

            if (DataArray[4].Trim().Equals("1"))
            {
                if (string.IsNullOrEmpty(DataArray[6].Trim()))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    objRet.message += "對應控制器 必須指定";
                }
            }

            if (string.IsNullOrEmpty(DataArray[7].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "接點數量 必須輸入";
            }
            else if (!int.TryParse(DataArray[7].Trim(), out tempint))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "接點數量 必須為數值";
            }

            if (string.IsNullOrEmpty(DataArray[8].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "區域 必須輸入";
            }

            if (string.IsNullOrEmpty(DataArray[11].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "狀態 必須指定";
            }

            if (DataArray[12].Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "說明 字數超過上限";
            }
            #endregion

            #region DB
            if (objRet.result)
            {
                #region Check No
                switch (Mode)
                {
                    case "Insert":
                        sql = @" SELECT * FROM B01_IOModule WHERE IOMNo = ? ";
                        liSqlPara.Add("S:" + DataArray[0].Trim());
                        break;
                    case "Update":
                        sql = @" SELECT * FROM B01_IOModule WHERE IOMNo = ? AND IOMID != ? ";
                        liSqlPara.Add("S:" + DataArray[0].Trim());
                        liSqlPara.Add("I:" + ClickItem.Trim());
                        break;
                }

                oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                if (dr.Read())
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;
                    objRet.message += "此IO編號已存在於系統中";
                }
                #endregion

                sql = "";
                liSqlPara.Clear();

                #region Check Add
                switch (Mode)
                {
                    case "Insert":
                        sql = @" SELECT * FROM B01_IOModule WHERE IOMAddr = ? AND IOMstID = ?";
                        liSqlPara.Add("I:" + DataArray[2].Trim());
                        liSqlPara.Add("S:" + DataArray[5].Trim());
                        break;
                    case "Update":
                        sql = @" SELECT * FROM B01_IOModule WHERE IOMAddr = ? AND IOMstID = ? AND IOMID != ? ";
                        liSqlPara.Add("I:" + DataArray[2].Trim());
                        liSqlPara.Add("S:" + DataArray[5].Trim());
                        liSqlPara.Add("I:" + ClickItem.Trim());
                        break;
                }

                oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                if (dr.Read())
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;
                    objRet.message += "此機號已存在於系統中";
                }
                #endregion
            }
            #endregion

            return objRet;
        }
        #endregion

        #endregion

        #region Sensor 相關

        #region GetIOMInfo
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] GetIOMInfo(string FromIomID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            DBReader dr = new DBReader();

            sql = @" 
                    SELECT Top 1 Mst.DciID, Mst.IOMstID, Iom.IOMID, Iom.IOMNo, Sen.AlmType,
                    (SELECT (COUNT(*) + 1) FROM B01_Sensor WHERE IOMID = ? ) SenCount,
                    dbo.Get_LocParentID(Sen.LocID, 'AREA') AS 'AREA', dbo.Get_LocParentID(Sen.LocID, 'BUILDING') AS 'BUILDING', 
                    dbo.Get_LocParentID(Sen.LocID, 'FLOOR') AS 'FLOOR', Sen.SenStatus
                    FROM B01_Sensor Sen
                    LEFT JOIN B01_IOModule Iom ON Iom.IOMID = Sen.IOMID 
                    LEFT JOIN B01_IOMaster Mst ON Mst.IOMstID = Iom.IOMstID
                    WHERE Sen.IOMID = ? ";

            liSqlPara.Clear();
            liSqlPara.Add("I:" + FromIomID.Trim());
            liSqlPara.Add("I:" + FromIomID.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                EditData = new string[dr.DataReader.FieldCount];
                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                {
                    EditData[i] = dr.DataReader[i].ToString();
                }
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "No Data！";
            }

            return EditData;
        }
        #endregion

        #region LoadSensor
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadSensor(string UserID, string SenID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            DBReader dr = new DBReader();

            #region Process String
            sql = @"
                SELECT Sen.SenNo, Sen.SenName, Sen.IoBit, Sen.ActiveSignal, Sen.AlmType, 
                dbo.Get_LocParentID(Sen.LocID, 'AREA') AS 'AREA', dbo.Get_LocParentID(Sen.LocID, 'BUILDING') AS 'BUILDING', 
                dbo.Get_LocParentID(Sen.LocID, 'FLOOR') AS 'FLOOR', Sen.SenStatus, Sen.SenDesc, Mst.DciID, Mst.IOMstID, Iom.IOMID, Sen.SenID, Sen.AlmSeconds
                FROM B01_Sensor Sen
                LEFT JOIN B01_IOModule Iom ON Iom.IOMID = Sen.IOMID 
                LEFT JOIN B01_IOMaster Mst ON Mst.IOMstID = Iom.IOMstID
                WHERE Sen.SenID = @SenID ";

            var result = odo.GetQueryResult(sql, new { SenID = SenID });
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            DataTable dt = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(DataTable));
            if (dt != null && dt.Rows.Count > 0)
            {
                EditData = new string[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                    EditData[i] = dt.Rows[0][i].ToString();

                dtTemp.Dispose();
                dtTemp = dt;
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "No Data！";
            }
            #endregion

            return EditData;
        }

        public static void AddParaMeterForReader(string strUserID, string strEquID, string strEquModel)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string strTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            string strSQL = string.Format(@"
                INSERT INTO B01_EquParaData 
                (
                    EquID, EquParaID, ParaValue, M_ParaValue, OpStatus, 
                    UpdateUserID, UpdateTime, SendTime, CompleteTime
                ) 
                SELECT 
                    '{0}', EquParaID, DefaultValue, DefaultValue, 'Setted', 
                    '{1}', '{2}', '{2}', '{2}'   
                FROM B01_EquParaDef 
                WHERE EquParaID IS NOT NULL AND EquModel='{3}' 
                AND EquParaID NOT IN 
                (SELECT EquParaID FROM B01_EquParaData WHERE EquID={0})",
                strEquID,
                strUserID,
                strTime,
                strEquModel);

            oAcsDB.SqlCommandExecute(strSQL);
        }
        #endregion

        #region InsertSensor
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object InsertSensor(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "", sql1 = "", sql2 = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            List<string> liSqlPara1 = new List<string>();
            List<string> liSqlPara2 = new List<string>();

            // 驗證資料
            objRet = Check_Input_DB_Sensor(DataArray, "Insert");

            #region 新增偵測器資料

            if (objRet.result)
            {
                string sLocID = DataArray[7];
                if (string.IsNullOrEmpty(sLocID)) sLocID = DataArray[6];
                if (string.IsNullOrEmpty(sLocID)) sLocID = DataArray[5];

                #region 新增偵測器
                sql1 += @" 
                            INSERT INTO B01_Sensor 
                            (
                                SenNo, SenName, IOMID, IoBit, ActiveSignal, LocID, 
                                SenStatus, AlmType, SenDesc, AlmSeconds, CreateUserID, CreateTime
                            ) 
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                liSqlPara1.Add("S:" + DataArray[0].Trim());
                liSqlPara1.Add("S:" + DataArray[1].Trim());
                liSqlPara1.Add("I:" + DataArray[12].Trim());
                liSqlPara1.Add("I:" + DataArray[2].Trim());
                liSqlPara1.Add("I:" + DataArray[3].Trim());
                liSqlPara1.Add("S:" + sLocID);
                liSqlPara1.Add("S:" + DataArray[8].Trim());
                liSqlPara1.Add("I:" + DataArray[4].Trim());
                liSqlPara1.Add("S:" + DataArray[9].Trim());
                liSqlPara1.Add("S:" + DataArray[14].Trim());
                liSqlPara1.Add("S:" + UserID.ToString());
                liSqlPara1.Add("D:" + Time);

                #endregion

                #region 開始交易
                oAcsDB.BeginTransaction();

                int intResult1 = oAcsDB.SqlCommandExecute(sql1, liSqlPara1);

                if (intResult1 != -1)
                {
                    oAcsDB.Commit();

                    #region 寫入B00_SysLog
                    #region 新增偵測器
                    int[] intAry = new int[] { 0, 1, 12, 2, 3, 5, 6, 7, 8, 4, 9, 14 };
                    string[] strAry = new string[] { "SenNo", "SenName", "IOMID", "IoBit", "ActiveSignal", "Area", "Building", "Floor", "SenStatus", "AlmType", "SenDesc", "AlmSeconds" };

                    List<Machine> liData = new List<Machine>();
                    int r = 0;
                    foreach (int g in intAry)
                    {
                        //string strOldValue = drTemp.DataReader.GetValue(g).ToString();
                        string strNewValue = DataArray[g].Trim();

                        liData.Add(new Machine(strAry[r], strSensor[r], "", strNewValue));
                        r++;
                    }

                    oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, sUserID, sUserName, "0213",
                        DataArray[0].Trim(),
                        DataArray[1].Trim(),
                        string.Format("{0}", CompareVaule("Insert", liData)),
                        "新增偵測器");
                    #endregion

                    #endregion

                    #region 取得剛剛新增的SenID回傳使用
                    sql = @" 
                        SELECT TOP 1 SenID FROM B01_Sensor 
                        WHERE SenNo = ? AND IoBit = ? AND IOMID = ? ";

                    liSqlPara.Clear();
                    liSqlPara.Add("S:" + DataArray[0].Trim());
                    liSqlPara.Add("I:" + DataArray[2].Trim());
                    liSqlPara.Add("I:" + DataArray[12].Trim());
                    objRet.message = oAcsDB.GetStrScalar(sql, liSqlPara);
                    #endregion

                }
                else
                {
                    oAcsDB.Rollback();

                    objRet.result = false;
                    objRet.message = "新增偵測器失敗。";
                }
                #endregion

            }

            #endregion

            objRet.act = "InsertSensor";
            return objRet;
        }

        private static void CopyParaToOtherReader(string strCtrlID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

            string strSQL = string.Format(@"
                UPDATE B01_EquParaData 
                SET paraValue=SV.P1, M_ParaValue=SV.P1, isReSend=0, OpStatus='Setted' 
                FROM 
                (
	                SELECT EPD.paraValue P1, EPD.M_ParaValue P2, EPD.EquParaID, EPD.EquID FirstEquID 
                    FROM B01_EquParaData EPD 
                    INNER JOIN B01_EquParaDef EPF ON EPF.EquParaID = EPD.EquParaID 
                    WHERE 
                    EPD.EquID IN 
                    (
	                    SELECT TOP 1 EquID FROM B01_EquData ED 
	                    INNER JOIN B01_Reader RD ON RD.EquNo = ED.EquNo 
	                    INNER JOIN B01_Controller CR ON CR.CtrlID = RD.CtrlID 
	                    WHERE CR.CtrlID = {0} 
	                    ORDER BY CR.CreateTime
                    ) 
                    AND EPF.ParaType = 'Controller' 
                ) SV 
                WHERE B01_EquParaData.EquParaID = SV.EquParaID 
                AND B01_EquParaData.EquID != SV.FirstEquID ", strCtrlID);

            oAcsDB.SqlCommandExecute(strSQL);
        }
        #endregion

        #region 新增設備後的處理(新增設備(EquID)到全群組(EG999)、給新增的設備，設定設備參數預設值)
        private static void AfterAddReaderHealWith(string strsUserID, string strCtrlID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;

            string strSQL = string.Format(@"
                SELECT ED.EquID EquID, ED.EquModel EquModel
                FROM B01_Reader Rdr
                LEFT JOIN B01_EquData ED ON ED.EquNo = Rdr.EquNo
                WHERE CtrlID = {0}", strCtrlID);

            bool isSuccess = oAcsDB.GetDataReader(strSQL, out dr);

            if (isSuccess)
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        string strEquID = dr.ToInt32("EquID").ToString();
                        string strEquModel = dr.ToString("EquModel");

                        AddEquIdToEquGroupData(strEquID);
                        AddDefaultEquParaData(strsUserID, strEquID, strEquModel);
                    }
                }
            }
        }
        #endregion

        #region 新增設備(EquID)到全群組(EG999)
        private static void AddEquIdToEquGroupData(string strEquID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            List<string> liSqlPara = new List<string>();

            // 取得設備群組EquGrpNO='EG999'的EquGrpID
            string strEquGrpID
                = oAcsDB.GetStrScalar(string.Format("SELECT TOP 1 EquGrpID FROM B01_EquGroup WHERE EquGrpNo='{0}'", "EG999"));

            if (strEquGrpID == null)
            {
                oAcsDB.SqlCommandExecute(@"
                    INSERT INTO [B01_EquGroup] ([EquGrpNo],[EquGrpName],[OwnerID],[OwnerList],[CreateUserID],[CreateTime]) 
                    VALUES ('EG999','全群組','Saho','\Saho\','Saho',GETDATE())");
            }

            // 新增這個EquID到全群組
            if (strEquGrpID != null)
            {
                string strSQL = @"
                    INSERT INTO B01_EquGroupData 
                        (EquGrpID,EquID,CreateUserID,CreateTime,UpdateUserID,UpdateTime) 
                    VALUES  
                        (?,?,'saho',GETDATE(),'saho',GETDATE()) ";

                liSqlPara.Clear();
                liSqlPara.Add("A:" + strEquGrpID);      // EquGrpID
                liSqlPara.Add("A:" + strEquID);         // EquID

                oAcsDB.SqlCommandExecute(strSQL, liSqlPara);
            }
        }

        #endregion

        #region 給新增的設備，設定設備參數預設值
        private static void AddDefaultEquParaData(string strsUserID, string strEquID, string strEquModel)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string strTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            string strSQL = string.Format(@"
                INSERT INTO B01_EquParaData 
                (
                    EquID, 
                    EquParaID, 
                    ParaValue, 
                    M_ParaValue, 
                    IsReSend, 
                    OpStatus, 
                    UpdateUserID, UpdateTime, SendTime, CompleteTime  
                )
                SELECT 
                    '{0}', EquParaID, DefaultValue, DefaultValue, '0', 'Setted', 
                    '{1}', '{2}', '{2}', '{2}' 
                FROM B01_EquParaDef WHERE EquModel= '{3}'",
                strEquID, strsUserID, strTime, strEquModel);

            oAcsDB.SqlCommandExecute(strSQL);
        }
        #endregion

        #region UpdateSensor
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object UpdateSensor(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql1 = "", sql2 = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara1 = new List<string>();
            List<string> liSqlPara2 = new List<string>();

            // Check_Input_DB_Sensor
            objRet = Check_Input_DB_Sensor(DataArray, "Update");

            #region 編輯偵測器

            if (objRet.result)
            {
                string sLocID = DataArray[7].Trim();
                if (string.IsNullOrEmpty(sLocID)) sLocID = DataArray[6].Trim();
                if (string.IsNullOrEmpty(sLocID)) sLocID = DataArray[5].Trim();

                #region 修改偵測器資料
                sql1 = @" 
                    UPDATE B01_Sensor SET 
                        SenNo = ?, SenName = ?, IoBit = ?, ActiveSignal = ?, LocID = ?, 
                        SenStatus = ?, AlmType = ?, SenDesc = ?, AlmSeconds = ? , UpdateUserID = ?, UpdateTime = ?
                    WHERE SenID = ? ";

                liSqlPara1.Add("S:" + DataArray[0].Trim());
                liSqlPara1.Add("S:" + DataArray[1].Trim());
                liSqlPara1.Add("S:" + DataArray[2].Trim());
                liSqlPara1.Add("I:" + DataArray[3].Trim());
                liSqlPara1.Add("I:" + sLocID);
                liSqlPara1.Add("I:" + DataArray[8].Trim());
                liSqlPara1.Add("I:" + DataArray[4].Trim());
                liSqlPara1.Add("S:" + DataArray[9].Trim());
                liSqlPara1.Add("S:" + DataArray[14].Trim());
                liSqlPara1.Add("S:" + UserID.ToString());
                liSqlPara1.Add("D:" + Time);
                liSqlPara1.Add("I:" + DataArray[13].Trim());

                oAcsDB.SqlCommandExecute(sql1, liSqlPara1);
                #endregion

                #region 開始交易
                oAcsDB.BeginTransaction();

                int intResult1 = oAcsDB.SqlCommandExecute(sql1, liSqlPara1);

                if (intResult1 > -1)
                {
                    oAcsDB.Commit();

                    #region 寫入B00_SysLog
                    #region 修改偵測器
                    int[] intAry = new int[] { 0, 1, 12, 2, 3, 5, 6, 7, 8, 4, 9, 14 };
                    string[] strAry = new string[] { "SenNo", "SenName", "IOMID", "IoBit", "ActiveSignal", "Area", "Building", "Floor", "SenStatus", "AlmType", "SenDesc", "AlmSeconds" };

                    List<Machine> liData = new List<Machine>();
                    int r = 0;
                    bool isUpdate = false; 
                    foreach (int g in intAry)
                    {

                        string strOldValue = dtTemp.Rows[0][g].ToString();
                        string strNewValue = DataArray[g].Trim();

                        if (g == 3 || g == 8 )
                        {
                            if (strOldValue != strNewValue)
                            {
                                isUpdate = true;
                            }
                        }

                        liData.Add(new Machine(strAry[r], strSensor[r], strOldValue, strNewValue));
                        r++;
                    }

                    if (isUpdate)
                    {
                        sql1 = "UPDATE B01_Sensor SET isUpdate = 1 WHERE SenID = ? ";

                        liSqlPara1.Clear();
                        liSqlPara1.Add("I:" + DataArray[13].Trim());
                        oAcsDB.SqlCommandExecute(sql1, liSqlPara1);
                    }

                    oAcsDB.WriteLog(DB_Acs.Logtype.資料修改, sUserID, sUserName, "0213",
                        dtTemp.Rows[0][0].ToString(),
                        dtTemp.Rows[0][1].ToString(),
                        string.Format("{0}", CompareVaule("Update", liData)),
                        "修改偵測器");
                    #endregion

                   
                    #endregion

                    objRet.message = DataArray[13].Trim();
                }
                else
                {
                    oAcsDB.Rollback();

                    objRet.result = false;
                    objRet.message = "修改偵測器。";
                }
                #endregion
            }
            #endregion

            objRet.act = "UpdateSensor";
            //Object oo = SendAppCmdStrList("");
            return objRet;
        }
        #endregion

        #region DeleteSensor
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object DeleteSensor(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            int intResult = 0;

            #region 刪除偵測器

            if (objRet.result)
            {
                oAcsDB.BeginTransaction();

                #region 刪除偵測器資料

                //#region 刪除 B01_EquGroupData 裡面的資料
                //if (intResult > -1)
                //{
                //    sql = @" DELETE FROM B01_EquGroupData WHERE EquID = ? ";
                //    liSqlPara.Clear();
                //    liSqlPara.Add("I:" + DataArray[21].Trim());
                //    intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                //}
                //#endregion

                //#region 刪除 B01_EquDataExt 裡面的資料
                //if (intResult > -1)
                //{
                //    sql = @" DELETE FROM B01_EquDataExt WHERE EquID = ? ";
                //    liSqlPara.Clear();
                //    liSqlPara.Add("I:" + DataArray[21].Trim());
                //    intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                //}
                //#endregion

                //#region 刪除 B01_EquParaData 裡面的資料
                //if (intResult > -1)
                //{
                //    sql = @" DELETE FROM B01_EquParaData WHERE EquID = ? ";
                //    liSqlPara.Clear();
                //    liSqlPara.Add("I:" + DataArray[21].Trim());
                //    intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                //}
                //#endregion

                //#region 刪除 B01_ElevatorFloor 裡面的資料
                //if (intResult > -1)
                //{
                //    sql = @" DELETE FROM B01_ElevatorFloor WHERE EquID = ? ";
                //    liSqlPara.Clear();
                //    liSqlPara.Add("I:" + DataArray[21].Trim());
                //    intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                //}
                //#endregion

                //#region 刪除 B01_CardAuth 裡面的資料
                //if (intResult > -1)
                //{
                //    sql = @" DELETE FROM B01_CardAuth WHERE EquID = ? ";
                //    liSqlPara.Clear();
                //    liSqlPara.Add("I:" + DataArray[21].Trim());
                //    intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                //}
                //#endregion

                //#region 刪除 B01_CardEquAdj 裡面的資料
                //if (intResult > -1)
                //{
                //    sql = @" DELETE FROM B01_CardEquAdj WHERE EquID = ? ";
                //    liSqlPara.Clear();
                //    liSqlPara.Add("I:" + DataArray[21].Trim());
                //    intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                //}
                //#endregion

                //#region 刪除 B01_EquData 裡面的資料
                //if (intResult > -1)
                //{
                //    sql = @" DELETE FROM B01_EquData WHERE EquID = ? ";
                //    liSqlPara.Clear();
                //    liSqlPara.Add("I:" + DataArray[21].Trim());
                //    intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                //}
                //#endregion

                #region 刪除 B01_Sensor 裡面的資料
                if (intResult > -1)
                {
                    sql = @" DELETE FROM B01_Sensor WHERE SenID = ? ";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[13].Trim());
                    intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #endregion

                if (intResult > -1)
                {
                    oAcsDB.Commit();

                    oAcsDB.WriteLog(DB_Acs.Logtype.資料刪除, sUserID, sUserName, "0213",
                        DataArray[0].Trim(),
                        DataArray[1].Trim(),
                        string.Format("偵測器編號：{0}", DataArray[0].Trim()),
                        "刪除偵測器");

                    objRet.message = DataArray[13].Trim();
                }
                else
                {
                    oAcsDB.Rollback();

                    objRet.message = "刪除偵測器資料失敗。";
                    objRet.result = false;
                }
            }

            #endregion

            objRet.act = "DeleteSensor";
            return objRet;
        }
        #endregion

        #region Check_Input_DB_Sensor
        protected static Pub.MessageObject Check_Input_DB_Sensor(string[] DataArray, string Mode)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            int tempint;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr = new Sa.DB.DBReader();

            #region Input

            #region 0. SenNo
            if (string.IsNullOrEmpty(DataArray[0].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "偵測器編號 必須輸入";
            }
            #endregion

            #region 1. SenName
            if (DataArray[1].Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "偵測器名稱 字數超過上限";
            }
            #endregion

            #region 2. IoBit
            if (string.IsNullOrEmpty(DataArray[3].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "Bit數 必須輸入";
            }
            #endregion

            #region 3. ActiveSignal
            if (string.IsNullOrEmpty(DataArray[3].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "觸發訊號 必須指定";
            }
            #endregion

            #region 5. SenDDLArea
            if (string.IsNullOrEmpty(DataArray[5].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "區域 必須指定";
            }
            #endregion

            #region 9. SenDDLBuilding
            if (DataArray[9].Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "說明 字數超過上限";
            }
            #endregion

            #endregion

            #region DB
            if (objRet.result)
            {
                switch (Mode)
                {
                    case "Insert":
                        sql = @" SELECT * FROM B01_Sensor WHERE IOMID = ? AND SenNo = ? ";
                        liSqlPara.Add("S:" + DataArray[12].Trim());
                        liSqlPara.Add("S:" + DataArray[0].Trim());

                        break;
                    case "Update":
                        sql = @" SELECT * FROM B01_Sensor WHERE IOMID = ? AND SenNo = ? AND SenID != ? ";

                        liSqlPara.Add("S:" + DataArray[12].Trim());
                        liSqlPara.Add("S:" + DataArray[0].Trim());
                        liSqlPara.Add("S:" + DataArray[13].Trim());
                        break;
                }

                dr = new DBReader();
                oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                if (dr.HasRows)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;
                    objRet.message += "此偵測器編號已存在於系統中。";
                }
            }

            if (objRet.result)
            {
                liSqlPara.Clear();
                switch (Mode)
                {
                    case "Insert":
                        sql = @" SELECT * FROM B01_Sensor WHERE IOMID = ? AND IoBit = ? ";

                        liSqlPara.Add("S:" + DataArray[12].Trim());
                        liSqlPara.Add("S:" + DataArray[2].Trim());

                        break;
                    case "Update":
                        sql = @" SELECT * FROM B01_Sensor WHERE IOMID = ? AND IoBit = ? AND SenID != ? ";

                        liSqlPara.Add("S:" + DataArray[12].Trim());
                        liSqlPara.Add("I:" + DataArray[2].Trim());
                        liSqlPara.Add("S:" + DataArray[13].Trim());
                        break;
                }

                dr = new DBReader();
                oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                if (dr.HasRows)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;
                    objRet.message += "Bit數不可重複";
                }
            }

            #endregion

            return objRet;
        }
        #endregion

        #endregion

        #region Reader_GetDropDownList
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] Reader_GetDropDownList(string ReaderID)
        {
            ListItem Item = new ListItem();
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt;

            #region Process String
            sql = @" SELECT 
                     EquData.*
                     FROM B01_EquData AS EquData
                     LEFT JOIN  B01_DeviceConnInfo AS  Dev ON dev.DciID = EquData.DciID
                     LEFT JOIN B01_Master AS Mas ON Mas.DciID = dev.DciID
                     LEFT JOIN B01_Controller AS Ctrl ON Ctrl.MstID = Mas.MstID
                     LEFT JOIN B01_Reader AS Reader ON Reader.CtrlID = ctrl.CtrlID
                     WHERE Reader.ReaderID = ? ";
            liSqlPara.Add("I:" + ReaderID);
            #endregion

            oAcsDB.GetDataTable("EquNoTable", sql, liSqlPara, out dt);

            string[] EditData = null;
            string tempstr = "";
            EditData = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                tempstr = "";
                tempstr += dt.Rows[i]["EquName"].ToString() + "/" + dt.Rows[i]["EquNo"].ToString();
                EditData[i] = tempstr;
            }

            return EditData;
        }
        #endregion

        #region CheckIP
        public static bool CheckIP(string IP)
        {
            bool flag = true;
            string[] IParray;
            int tempint;

            #region ipv4
            IP = IP.Trim();
            string pattern = @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$";
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
            if (!regex.Match(IP).Success)
                flag = false;
            if (flag)
            {
                IParray = IP.Split('.');
                for (int i = 0; i < IParray.Length; i++)
                {
                    int.TryParse(IParray[i].Trim(), out tempint);
                    if (tempint < 0 || tempint > 255)
                    {
                        flag = false;
                        break;
                    }
                }
            }
            #endregion

            #region ipv6
            if (!flag)
            {
                IPAddress address = null;

                flag = IPAddress.TryParse(IP, out address);

                if (address != null)
                {
                    if (address.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }

            }
            #endregion

            return flag;
        }
        #endregion

        #region CheckPort
        public static bool CheckPort(string Port)
        {
            int tempint;
            bool flag = true;

            if (!int.TryParse(Port.Trim(), out tempint))
                flag = false;
            else
            {
                int.TryParse(Port.Trim(), out tempint);
                if (tempint < 1 || tempint > 65535)
                    flag = false;
            }
            return flag;
        }
        #endregion

        #region SearchEquData
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] SearchEquData(string strKeyWord)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt;

            #region Process String
            sql = @" SELECT EquNo, EquName FROM B01_EquData
                WHERE EquNo LIKE ? OR EquName LIKE ? ";
            liSqlPara.Add("S:" + '%' + strKeyWord + '%');
            liSqlPara.Add("S:" + '%' + strKeyWord + '%');
            #endregion

            oAcsDB.GetDataTable("EquNoTable", sql, liSqlPara, out dt);

            string[] EditData = null;
            EditData = new string[dt.Rows.Count];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                EditData[i] = dt.Rows[i]["EquName"].ToString() + "/" + dt.Rows[i]["EquNo"].ToString();
            }

            return EditData;
        }
        #endregion

        #endregion

        #region RegisterStartupScript

        #region RegisterTreeViewJS
        private void RegisterTreeViewJS()
        {
            string jstr = "";

            //jstr = @"EquOrg_TreeView.oncontextmenu = showMenu;
            //    document.body.onclick = hideMenu;
            //    SetDivMode('');";

            jstr = @"EquOrg_TreeView.oncontextmenu = showMenu;
                document.body.onclick = hideMenu;";

            ScriptManager.RegisterStartupScript(TreeView_UpdatePanel, TreeView_UpdatePanel.GetType(), "TreeViewJS", jstr, true);
        }

        #endregion

        #endregion

        #region SetButton_Click
        protected void SetButton_Click(object sender, EventArgs e)
        {
            // 從控制器編號(Ctrl_Input_No)，得到其中的READER，再依此得到相對應的設備，擇一發送則可，若無READER則不做
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt = new DataTable();
            List<string> liSqlPara = new List<string>();
            DBReader dr = new Sa.DB.DBReader();

            #region Process String
            sql = @" 
                SELECT ED.EquID, RD.EquNo FROM B01_Controller CR 
                INNER JOIN B01_Reader RD ON RD.CtrlID = CR.CtrlID 
                INNER JOIN B01_EquData ED ON ED.EquNo = RD.EquNo  
                WHERE CR.CtrlNo = ? ";
            liSqlPara.Add("S:" + Iom_Input_No.Text);
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (dr.HasRows)
            {
                string strCmd = "";

                while (dr.Read())
                {
                    string strEquID = dr.DataReader[0].ToString();
                    string strEquNo = dr.DataReader[1].ToString();

                    if (strCmd != "") { strCmd += ";"; }
                    strCmd = string.Format("{0}@{1}@SetAllCardDataStart@", strEquID, strEquNo);
                }

                SendAppCmdStrList(strCmd);

                // 訊息：設碼動作已處理! / Set Code action has been processed!
                //Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('" + GetLocalResourceObject("string_SetCodeMsg_0").ToString() + "');");

                Sa.Web.Fun.RunJavaScript(this, "ShowDialog('message','General','" + GetLocalResourceObject("string_SetCodeMsg_0").ToString() + "');");

            }
            else
            {
                // 訊息：控制器沒有相對應的設備! / The Controller does not have a corresponding device
                //Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('" + GetLocalResourceObject("string_ControllerMsg_0") + "!');");

                Sa.Web.Fun.RunJavaScript(this, "ShowDialog('alert','alert','" + GetLocalResourceObject("string_SetCodeMsg_0").ToString() + "');");
            }
            #endregion

        }

        #endregion

        #region SendAppCmdStrList
        /// <summary>
        /// 傳送APP指令字串清單至SahoWebSocket
        /// </summary>
        /// <param name="sCmdStrList">指令字串清單</param>
        private void SendAppCmdStrList(string sCmdStrList)
        {
            string[] sIPArray = null, sAppCmdStrArray = null;
            SahoWebSocket oSWSocket = null;

            try
            {
                if (!string.IsNullOrEmpty(sCmdStrList))
                {
                    #region 建立與設定SahoWebSocket
                    if (Session["SahoWebSocket"] != null)
                    {
                        if ((!((SahoWebSocket)Session["SahoWebSocket"]).IsWorking) || ((SahoWebSocket)Session["SahoWebSocket"]).IsGameOver)
                        {
                            ((SahoWebSocket)Session["SahoWebSocket"]).Stop();
                            ((SahoWebSocket)Session["SahoWebSocket"]).Start();
                        }
                    }
                    else
                    {
                        #region 取得APP的IP位址
                        string sIPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                        if (!string.IsNullOrEmpty(sIPAddress))
                        {
                            sIPArray = sIPAddress.Split(new char[] { ',' });
                        }
                        else
                        {
                            sIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        }
                        if (sIPAddress == "::1") { sIPAddress = "127.0.0.1"; }
                        #endregion

                        #region 建立與設定SahoWebSocket物件及基本資料
                        Session["SahoWebSocket"] = new SahoWebSocket();

                        ((SahoWebSocket)Session["SahoWebSocket"]).UserID = hideUserID.Value;
                        ((SahoWebSocket)Session["SahoWebSocket"]).SourceIP = sIPAddress;
                        ((SahoWebSocket)Session["SahoWebSocket"]).SCListenIPPort = System.Web.Configuration.WebConfigurationManager.AppSettings["SCListenIPPort"].ToString();
                        ((SahoWebSocket)Session["SahoWebSocket"]).DbConnectionString = Pub.GetConnectionString(Pub.sConnName);

                        //((SahoWebSocket)Session["SahoWebSocket"]).SelectRecordIDAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
                        //((SahoWebSocket)Session["SahoWebSocket"]).InsertAppCmdObjAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
                        //((SahoWebSocket)Session["SahoWebSocket"]).UpdateDriverCmdResultStrAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
                        ((SahoWebSocket)Session["SahoWebSocket"]).Start();
                        #endregion
                    }
                    #endregion

                    #region 傳送APP指令字串
                    oSWSocket = (SahoWebSocket)Session["SahoWebSocket"];
                    string BeType = "0";
                    if (ConfigurationManager.AppSettings["LanguageOption"] != null && new string[] { "1", "0" }.Contains(ConfigurationManager.AppSettings["LanguageOption"]))
                    {
                        BeType = System.Configuration.ConfigurationManager.AppSettings["LanguageOption"];
                    }
                    oSWSocket.SetBECommTag(BeType);
                    oSWSocket.ClearCmdResult();
                    sAppCmdStrArray = sCmdStrList.Split(';');
                    for (int i = 0; i < sAppCmdStrArray.Length; i++)
                    {
                        oSWSocket.SendAppCmdStr(sAppCmdStrArray[i]);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Response.Write("ex" + ex.Message.ToString());

            }
            finally
            {
                sIPArray = null;
                oSWSocket = null;
                sAppCmdStrArray = null;
            }
        }
        #endregion

        #region GetEquClassListItem
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] GetEquClassListItem(string Master_Input_CtrlModel)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt = new DataTable();

            sql = @" 
                SELECT [ItemInfo1] FROM [B00_ItemList]
                WHERE [ItemClass] = 'EquModel' AND [ItemNo] = ? 
                ORDER BY [ItemInfo1] ASC ";
            liSqlPara.Add("S:" + Master_Input_CtrlModel);

            oAcsDB.GetDataTable("ItemInfo1", sql, liSqlPara, out dt);

            string[] EditData = null;
            EditData = new string[dt.Rows.Count];

            string str = "";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                switch (dt.Rows[i]["ItemInfo1"].ToString())
                {
                    case "Door Access":
                        str = string_DoorAccess;
                        break;
                    case "Elevator":
                        str = string_Elevator;
                        break;
                    case "TRT":
                        str = string_TRT;
                        break;
                }

                EditData[i] = str + "/" + dt.Rows[i]["ItemInfo1"].ToString();
            }

            return EditData;
        }
        #endregion

        private static string CompareVaule(string strCmd, List<Machine> liData)
        {
            string str = "";
            new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            if (strCmd == "Insert")
            {
                foreach (Machine machine in liData)
                {
                    switch (machine.Code)
                    {
                        case "IsAssignIP":
                        case "MstStatus":
                        case "AutoReturn":
                        case "CtrlStatus":
                        case "IsAndTrt":
                        case "EquClass":
                            machine.NewV = ChangeValue(machine.Code, machine.NewV);
                            break;
                    }

                    if (!machine.NewV.Equals(""))
                    {
                        if (str != "")
                        {
                            str = str + "，";
                        }
                        str = str + string.Format("{0}：{1}", machine.Name, machine.NewV);
                    }
                }
                return str;
            }
            if (strCmd == "Update")
            {
                foreach (Machine machine2 in liData)
                {
                    switch (machine2.Code)
                    {
                        case "IsAssignIP":
                        case "MstStatus":
                        case "AutoReturn":
                        case "CtrlStatus":
                        case "IsAndTrt":
                        case "EquClass":
                            machine2.OldV = ChangeValue("AutoReturn", machine2.OldV);
                            machine2.NewV = ChangeValue("AutoReturn", machine2.NewV);
                            break;
                    }

                    if (!machine2.OldV.Equals(machine2.NewV))
                    {
                        if (str != "")
                        {
                            str = str + "，";
                        }
                        str = str + string.Format("{0}：{1}\x00bb{2}", machine2.Name, machine2.OldV, machine2.NewV);
                    }
                }
                if (str == "")
                {
                    str = "Not Modified";
                }
            }
            return str;
        }

        private static string ChangeValue(string strCode, string strValue)
        {
            string strResult = "";

            switch (strCode)
            {
                case "IsAssignIP":
                    strResult = (strValue == "0") ? "關閉" : "開啟";
                    break;
                case "MstStatus":
                    strResult = (strValue == "0") ? "是" : "否";
                    break;
                case "AutoReturn":
                case "IsAndTrt":
                    strResult = (strValue == "0") ? "否" : "是";
                    break;
                case "CtrlStatus":
                    switch (strValue)
                    {
                        case "1":
                            strResult = "啟用";
                            break;
                        case "0":
                            strResult = "未啟用";
                            break;
                        case "2":
                            strResult = "限制";
                            break;
                        default:
                            strResult = "選取資料";
                            break;
                    }
                    break;
                case "EquClass":
                    switch (strValue)
                    {
                        case "Door Access":
                            strResult = "門禁設備";
                            break;
                        case "Elevator":
                            strResult = "電梯設備";
                            break;
                        case "TRT":
                            strResult = "考勤設備";
                            break;
                        default:
                            strResult = "";
                            break;
                    }
                    break;
                default:
                    strResult = "";
                    break;
            }

            return strResult;
        }

        private class Machine
        {
            public Machine(string strCode, string strName, string strOld, string strNew)
            {
                this.Code = strCode;        // 欄位的名稱
                this.Name = strName;        // 中、英語名稱
                this.OldV = strOld;         // 舊值
                this.NewV = strNew;         // 新值
            }

            public string Code { get; set; }
            public string Name { get; set; }
            public string NewV { get; set; }
            public string OldV { get; set; }
        }

        public static string CompareVaule(DBReader dr, string[] DataArray, int[] intAry)
        {
            string strResult = "";

            //0,1,2,3,4,8,9,10,11,12,13,14,15,16,17,18,19,20

            foreach (int i in intAry)
            {
                if (!dr.DataReader.GetValue(i).ToString().Equals(DataArray[i].ToString()))
                {
                    if (strResult != "") strResult += "，";

                    strResult += string.Format("{0}：{1}>>{2}", dr.DataReader.GetName(i), dr.DataReader.GetValue(i).ToString(), DataArray[i].ToString());
                    //$"{dr.DataReader.GetName(i)}：{dr.DataReader.GetValue(i).ToString()}>>{DataArray[i].ToString()}";
                }
            }

            if (strResult == "") strResult = "Not Modified";

            return strResult;
        }

        public class MessageObject1 : Pub.MessageObject
        {
            public string sUp;
        }

        protected void ddl_Area_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ddlLocationBuilding.Items.Clear();
            //if (locInfos != null)
            //{
            //    var result = locInfos.Where(i => i.LocType == "BUILDING" && i.LocPID == int.Parse(ddlLocationArea.SelectedItem.Value));
            //    if (result.Count() > 0)
            //    {
            //        foreach (var r in result)
            //        {
            //            ListItem Item = new ListItem();
            //            Item.Text = " [" + r.LocNo.ToString() + "]" + r.LocName;
            //            Item.Value = r.LocID.ToString();
            //            this.ddlLocationBuilding.Items.Add(Item);
            //        }
            //    }
            //}

            //ddlLocationFloor.Items.Clear();
            //UpdatePanel1.Update();
        }

       

        //#region GetLocationListItem
        //[System.Web.Services.WebMethod()]
        //[System.Web.Script.Services.ScriptMethod()]
        //public static string[] GetLocationListItem()
        //{
        //    DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
        //    string sql = "";
        //    List<string> liSqlPara = new List<string>();
        //    DataTable dt = new DataTable();

        //    sql = @"
        //        select * from (
        //        select LocType, LocID, LocNo, LocName, LocPID from B01_Location where LocType = 'AREA'
        //        UNION
        //        select LocType, LocID, LocNo, LocName, LocPID from B01_Location where LocType = 'BUILDING' 
        //        UNION 
        //        select LocType, LocID, LocNo, LocName, LocPID from B01_Location where LocType = 'FLOOR' 
        //        ) as R
        //        ORDER BY R.LocType, LocID
        //    ";

        //    oAcsDB.GetDataTable("ItemInfo1", sql, liSqlPara, out dt);

        //    string[] EditData = null;
        //    EditData = new string[dt.Rows.Count];

        //    string str = "";

        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    { 
        //        EditData[i] = dt.Rows[i]["LocType"].ToString() + "|" + dt.Rows[i]["LocID"].ToString() + "|" + dt.Rows[i]["LocNo"].ToString() + "|" + dt.Rows[i]["LocName"].ToString() + "|" + dt.Rows[i]["LocPID"].ToString();
        //    }

        //    return EditData;
        //}
        //#endregion
    }

}