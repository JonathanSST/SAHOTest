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
    public partial class EquMapSetting : Page
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
        private static string[] strMaster = new string[8];
        private static string[] strCtrl = new string[5];
        private static string[] strReader = new string[4];
        private static string[] strEqu = new string[11];

        private static string[] string_Dci = new string[14];
        private static string[] string_ConnectDevice = new string[11];
        private static string[] string_Para_T = new string[3];
        private static string[] string_Para_C = new string[5];
        private static string[] string_Message = new string[2];
        private static string[] string_Controller = new string[11];
        private static string[] string_Reader = new string[19];
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

            string jsFileEnd = "<script src=\"EquMapSetting_End.js\" Type=\"text/javascript\"></script>";
            ClientScript.RegisterStartupScript(typeof(string), "EquMapSetting_End", jsFileEnd, false);

            ScriptManager.RegisterStartupScript(TreeView_UpdatePanel, TreeView_UpdatePanel.GetType(), "OnPageLoad", js, false);

            RegisterTreeViewJS();           // 處理樹狀結構
            ClientScript.RegisterClientScriptInclude("EquMapSetting", "EquMapSetting.js?" + Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案

            ClientScript.RegisterClientScriptInclude("jqueryMin", Pub.JqueyNowVer);
            ClientScript.RegisterClientScriptInclude("jqueryUI", "/Scripts/jquery-ui.js");

            #region 設定彈跳視窗
            Pub.SetModalPopup(ModalPopupExtender1, 1);
            #endregion

            #region 註冊主頁Button動作
            //Master_Input_Type_TCPIP.Attributes.Add("Onclick", "SetParamDiv('IPParam');");
            //Master_Input_Type_COMPort.Attributes.Add("Onclick", "SetParamDiv('ComPortParam');");

            Dci_B_Add.Attributes["onClick"] = "InsertDciExcute(); return false;";
            Dci_B_Edit.Attributes["onClick"] = "UpdateDciExcute(); return false;";
            Dci_B_Delete.Attributes["onClick"] = "DeleteDciExcute(); return false;";
            Dci_B_Cancel.Attributes["onClick"] = "SetDivMode(''); return false;";

            Master_B_Add.Attributes["onClick"] = "InsertMasterExcute(); return false;";
            Master_B_Edit.Attributes["onClick"] = "UpdateMasterExcute(); return false;";
            Master_B_Delete.Attributes["onClick"] = "DeleteMasterExcute(); return false;";
            Master_B_Cancel.Attributes["onClick"] = "SetDivMode(''); return false;";
            Controller_B_Add.Attributes["onClick"] = "InsertControllerExcute(); return false;";
            Controller_B_Edit.Attributes["onClick"] = "UpdateControllerExcute(); return false;";
            Controller_B_Delete.Attributes["onClick"] = "DeleteControllerExcute(); return false;";
            Controller_B_Cancel.Attributes["onClick"] = "SetDivMode(''); return false;";
            Reader_B_Add.Attributes["onClick"] = "InsertReaderExcute(); return false;";
            Reader_B_Edit.Attributes["onClick"] = "UpdateReaderExcute(); return false;";
            Reader_B_Delete.Attributes["onClick"] = "DeleteReaderExcute(); return false;";
            Reader_B_Cancel.Attributes["onClick"] = "SetDivMode(''); return false;";

            //this.btnAddEquData.Attributes["onClick"] = "AddEquData(); return false;";
            //btnFilter.Attributes["onClick"] = "CallSearch('" + this.GetLocalResourceObject("CallSearch_Title") + "'); return false;";

            ddlEquClass.Attributes["onChange"] = "SetControlStatus(); return false;";

            //Master_Input_CtrlModel.Attributes["onload"] = "GetEquClassListItem(); return false;";
            //Master_Input_CtrlModel.Attributes["onChange"] = "GetEquClassListItem(); return false;";

            // 參數設定
            ParaButton.Attributes["onClick"] = "CallParaSetting();return false;";
            CtrlParaButton.Attributes["onClick"] = "CallCtrlParaSetting();return false;";

            #endregion

            #region 註冊pop1頁Button動作
            ImgCloseButton1.Attributes["onClick"] = "CancelTrigger1.click(); return false;";
            btnSearch.Attributes["onClick"] = "SearchEquData('" + txtKeyWord.Text.Trim() + "'); return false;";
            #endregion

            #region 語系切換
            //DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            //oAcsDB.GetTableHash("B01_DeviceConnInfo", "zhtw", out TableInfo);
            //Label_No.Text = TableInfo["DciNo"].ToString();
            //Label_Name.Text = TableInfo["DciName"].ToString();
            //Label_Ip.Text = TableInfo["IpAddress"].ToString();
            //Label_IsAssign.Text = TableInfo["IsAssignIP"].ToString();
            //popLabel_No.Text = TableInfo["DciNo"].ToString();
            //popLabel_Name.Text = TableInfo["DciName"].ToString();
            //popLabel_PassWD.Text = TableInfo["DciPassWD"].ToString();
            //popLabel_Ip.Text = TableInfo["IpAddress"].ToString();
            //popLabel_Port.Text = TableInfo["TcpPort"].ToString();
            //popLabel_IsAssign.Text = TableInfo["IsAssignIP"].ToString();
            #endregion

            #endregion

            #region 取 resource 裡面的值填 屬性

            strDci[0] = GetLocalResourceObject("ttConnNo").ToString();
            strDci[1] = GetLocalResourceObject("ttConnName").ToString();
            strDci[2] = GetLocalResourceObject("ttLockedIP").ToString();
            strDci[3] = GetLocalResourceObject("ttIP").ToString();
            strDci[4] = GetLocalResourceObject("ttPort").ToString();
            strDci[5] = GetLocalResourceObject("ttConnPW").ToString();

            strMaster[0] = GetGlobalResourceObject("Resource", "DeviceNo").ToString();      // 連線裝置編號 MstNo
            strMaster[1] = GetGlobalResourceObject("Resource", "DeviceInfo").ToString();    // 連線裝置說明 MstDesc
            strMaster[2] = "IP PORT";                                                       // IP + PORT MstConnParam
            strMaster[3] = GetGlobalResourceObject("Resource", "DeviceModel").ToString();   // 連線裝置機型 MstModel
            strMaster[4] = GetGlobalResourceObject("Resource", "Status").ToString();        // 狀態 MstStatus
            strMaster[5] = GetGlobalResourceObject("Resource", "CtrlModel").ToString();     // 控制器機型 CtrlModel
            strMaster[6] = GetGlobalResourceObject("Resource", "AutoReturn").ToString();    // 自動回傳 AutoReturn
            strMaster[7] = GetGlobalResourceObject("Resource", "FirmVer").ToString();       // 韌體版本 MstFwVer

            strCtrl[0] = GetGlobalResourceObject("Resource", "CtrlNo").ToString();
            strCtrl[1] = GetGlobalResourceObject("Resource", "CtrlName").ToString();
            strCtrl[2] = GetGlobalResourceObject("Resource", "CtrlInfo").ToString();
            strCtrl[3] = GetGlobalResourceObject("Resource", "MacNo").ToString();
            strCtrl[4] = GetGlobalResourceObject("Resource", "CtrlStatus").ToString();

            strReader[0] = GetGlobalResourceObject("Resource", "ReaderNo").ToString();
            strReader[1] = GetGlobalResourceObject("Resource", "ReaderName").ToString();
            strReader[2] = GetGlobalResourceObject("Resource", "ReaderInfo").ToString();
            strReader[3] = GetGlobalResourceObject("Resource", "InOut").ToString();

            strEqu[0] = GetLocalResourceObject("labEquClass").ToString();           //  7. EquClass
            strEqu[1] = GetLocalResourceObject("labEquModel").ToString();           //  8. EquModel
            strEqu[2] = GetGlobalResourceObject("Resource", "EquNo").ToString();    //  9. EquNo 
            strEqu[3] = GetLocalResourceObject("labBuilding").ToString();           // 12. Building
            strEqu[4] = GetGlobalResourceObject("Resource", "Floor").ToString();    // 13. Floor 
            strEqu[5] = GetGlobalResourceObject("Resource", "EquName").ToString();  // 14. EquName 
            strEqu[6] = GetLocalResourceObject("labEquEName").ToString();           // 15. EquEName
            strEqu[7] = GetLocalResourceObject("labCardNoLen").ToString();          // 17. CardNoLen
            strEqu[8] = GetGlobalResourceObject("Resource", "InMgn").ToString();    // 18. InToCtrlAreaID
            strEqu[9] = GetGlobalResourceObject("Resource", "OutMgn").ToString();   // 19. OutToCtrlAreaID
            strEqu[10] = GetLocalResourceObject("labInput_Trt").ToString();         // 20. IsAndTrt

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
            //string_Dci[14] = GetLocalResourceObject("string_Dci_14").ToString();

            string_ConnectDevice[0] = GetLocalResourceObject("string_ConnectDevice_00").ToString();
            string_ConnectDevice[1] = GetLocalResourceObject("string_ConnectDevice_01").ToString();
            string_ConnectDevice[2] = GetLocalResourceObject("string_ConnectDevice_02").ToString();
            string_ConnectDevice[3] = GetLocalResourceObject("string_ConnectDevice_03").ToString();
            string_ConnectDevice[4] = GetLocalResourceObject("string_ConnectDevice_04").ToString();
            string_ConnectDevice[5] = GetLocalResourceObject("string_ConnectDevice_05").ToString();
            string_ConnectDevice[6] = GetLocalResourceObject("string_ConnectDevice_06").ToString();
            string_ConnectDevice[7] = GetLocalResourceObject("string_ConnectDevice_07").ToString();
            string_ConnectDevice[8] = GetLocalResourceObject("string_ConnectDevice_08").ToString();
            string_ConnectDevice[9] = GetLocalResourceObject("string_ConnectDevice_09").ToString();
            string_ConnectDevice[10] = GetLocalResourceObject("string_ConnectDevice_10").ToString();

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

            string_Controller[0] = GetLocalResourceObject("string_Controller_00").ToString();
            string_Controller[1] = GetLocalResourceObject("string_Controller_01").ToString();
            string_Controller[2] = GetLocalResourceObject("string_Controller_02").ToString();
            string_Controller[3] = GetLocalResourceObject("string_Controller_03").ToString();
            string_Controller[4] = GetLocalResourceObject("string_Controller_04").ToString();
            string_Controller[5] = GetLocalResourceObject("string_Controller_05").ToString();
            string_Controller[6] = GetLocalResourceObject("string_Controller_06").ToString();
            string_Controller[7] = GetLocalResourceObject("string_Controller_07").ToString();
            string_Controller[8] = GetLocalResourceObject("string_Controller_08").ToString();
            string_Controller[9] = GetLocalResourceObject("string_Controller_09").ToString();
            string_Controller[10] = GetLocalResourceObject("string_Controller_10").ToString();

            string_Reader[0] = GetLocalResourceObject("string_Reader_00").ToString();
            string_Reader[1] = GetLocalResourceObject("string_Reader_01").ToString();
            string_Reader[2] = GetLocalResourceObject("string_Reader_02").ToString();
            string_Reader[3] = GetLocalResourceObject("string_Reader_03").ToString();
            string_Reader[4] = GetLocalResourceObject("string_Reader_04").ToString();
            string_Reader[5] = GetLocalResourceObject("string_Reader_05").ToString();
            string_Reader[6] = GetLocalResourceObject("string_Reader_06").ToString();
            string_Reader[7] = GetLocalResourceObject("string_Reader_07").ToString();
            string_Reader[8] = GetLocalResourceObject("string_Reader_08").ToString();
            string_Reader[9] = GetLocalResourceObject("string_Reader_09").ToString();
            string_Reader[10] = GetLocalResourceObject("string_Reader_10").ToString();
            string_Reader[11] = GetLocalResourceObject("string_Reader_11").ToString();
            string_Reader[12] = GetLocalResourceObject("string_Reader_12").ToString();
            string_Reader[13] = GetLocalResourceObject("string_Reader_13").ToString();
            string_Reader[14] = GetLocalResourceObject("string_Reader_14").ToString();
            string_Reader[15] = GetLocalResourceObject("string_Reader_15").ToString();
            string_Reader[16] = GetLocalResourceObject("string_Reader_16").ToString();
            string_Reader[17] = GetLocalResourceObject("string_Reader_17").ToString();
            string_Reader[18] = GetLocalResourceObject("string_Reader_18").ToString();

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

                LoadEquipment("");
                TreeView_UpdatePanel.Update();
                Master_CreateCtrlModel();
                GetLocationListItem();

                Div_Dci.Attributes["style"] = "display:none";
                Div_Master.Attributes["style"] = "display:none";
                Div_Controller.Attributes["style"] = "display:none";
                Div_Reader.Attributes["style"] = "display:none";

                //Div_Master.Attributes["style"] = "visibility:hidden";
                //Div_Controller.Attributes["style"] = "visibility:hidden";
                //Div_Reader.Attributes["style"] = "visibility:hidden";



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
                            LoadEquipment("");
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
                            LoadEquipment("");
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

        #region LoadEquipment
        private void LoadEquipment(string strFilter)
        {
            //DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
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

            List<DciInfo> DciTree = new List<DciInfo>();

            sql = " SELECT DciID, DciNo, DciName FROM B01_DeviceConnInfo ORDER BY DciNo ";

            this.MyoAcsDB.GetDataTable("DciTable", sql, liSqlPara, out dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DciInfo DciNode = new DciInfo();
                DciNode.DciID = int.Parse(dt.Rows[i]["DciID"].ToString());
                DciNode.DciNo = dt.Rows[i]["DciNo"].ToString();
                DciNode.DciName = dt.Rows[i]["DciName"].ToString();
                DciTree.Add(DciNode);
            }

            LoadDCiTree(RootNode, DciTree);
            EquOrg_TreeView.Nodes.Add(RootNode);
            EquOrg_TreeView.ShowLines = true;
            //EquOrg_TreeView.ShowExpandCollapse = true;
            //設定管制區進出的欄位設定
            this.MyoAcsDB.GetDataTable("CtrlAreaTable", "SELECT CtrlAreaName, CtrlAreaID, CtrlAreaNo FROM B01_CtrlArea", liSqlPara, out dt);
            foreach (DataRow dr in dt.Rows)
            {
                ListItem Item = new ListItem();
                Item.Text =  "[" + dr["CtrlAreaNo"] + "]" + dr["CtrlAreaName"].ToString();
                Item.Value = dr["CtrlAreaID"].ToString();

                this.ddlInToCtrlAreaID.Items.Add(Item);
                this.ddlOutToCtrlAreaID.Items.Add(Item);
            }

            //設定連動設定資料
            //this.MyoAcsDB.GetDataTable("EquDataTable", "SELECT * FROM B01_EquData", liSqlPara, out dt);
            dt = this.Myodo.GetDataTableBySql("SELECT * FROM B01_EquData");
            foreach (DataRow dr in dt.Rows)
            {
                ListItem Item = new ListItem();
                Item.Text = dr["EquName"].ToString() + " [" + dr["EquNo"].ToString() + "]";
                Item.Value = dr["EquNo"].ToString();
                this.ddlLinkEquNoList.Items.Add(Item);
            }

            ////區域、建築物、樓層
            //dt.Clear();
            //dt = this.Myodo.GetDataTableBySql("SELECT * FROM B01_Location ");
            //foreach (DataRow dr in dt.Rows)
            //{
            //    LocInfo locInfo = new LocInfo();
            //    locInfo.LocID = int.Parse(dr["LocID"].ToString());
            //    locInfo.LocNo = dr["LocNo"].ToString();
            //    locInfo.LocName = dr["LocName"].ToString();
            //    locInfo.LocPID = int.Parse(dr["LocPID"].ToString());
            //    locInfo.LocType = dr["LocType"].ToString();

            //    locInfos.Add(locInfo);
            //}

            //ddlLocationArea.Items.Clear();
            //ddlLocationBuilding.Items.Clear();
            //ddlLocationFloor.Items.Clear();
            //if (locInfos != null)
            //{
            //    var result = locInfos.Where(i => i.LocType == "AREA");
            //    if (result.Count() > 0)
            //    {
            //        foreach (var r in result)
            //        {
            //            ListItem Item = new ListItem();
            //            Item.Text = " [" + r.LocNo.ToString() + "]" + r.LocName;
            //            Item.Value = r.LocID.ToString();
            //            this.ddlLocationArea.Items.Add(Item);
            //        }
            //    }

            //    result = locInfos.Where(i => i.LocType == "BUILDING");
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

            //    result = locInfos.Where(i => i.LocType == "FLOOR");
            //    if (result.Count() > 0)
            //    {
            //        foreach (var r in result)
            //        {
            //            ListItem Item = new ListItem();
            //            Item.Text = " [" + r.LocNo.ToString() + "]" + r.LocName;
            //            Item.Value = r.LocID.ToString();
            //            this.ddlLocationFloor.Items.Add(Item);
            //        }
            //    }

            //}

        }
        #endregion

        #region LoadDCiTree
        private void LoadDCiTree(TreeNode PNode, List<DciInfo> objDciTree)
        {
            //DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            List<MasterInfo> MasterTree;

            for (int i = 0; i < objDciTree.Count; i++)
            {
                MasterTree = new List<MasterInfo>();
                sql = "";
                liSqlPara.Clear();
                TreeNode SubNode = new TreeNode();

                // 語系切換：string_Connection = 連線 / Connection
                string string_Connection = GetLocalResourceObject("string_Connection").ToString();

                SubNode.Text += "[" + objDciTree[i].DciNo + "] " + string_Connection + " - (" + objDciTree[i].DciName + ")";
                txt_NodeTypeList.Value += "DCI,";
                txt_NodeIDList.Value += objDciTree[i].DciID.ToString() + ",";
                SubNode.NavigateUrl = "#";

                sql = " SELECT MstID, MstNo, MstDesc, MstStatus FROM B01_Master WHERE DciID = ? ORDER BY MstNo ";
                liSqlPara.Add("S:" + objDciTree[i].DciID);

                this.MyoAcsDB.GetDataTable("MasterTable", sql, liSqlPara, out dt);

                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    MasterInfo MasterNode = new MasterInfo();
                    MasterNode.MstID = int.Parse(dt.Rows[k]["MstID"].ToString());
                    MasterNode.MstNo = dt.Rows[k]["MstNo"].ToString();
                    MasterNode.MstDesc = dt.Rows[k]["MstDesc"].ToString();
                    MasterNode.MstStatus = dt.Rows[k]["MstStatus"].ToString();

                    MasterTree.Add(MasterNode);
                }
                objDciTree[i].MasterList = MasterTree;
                LoadMasterTree(SubNode, objDciTree[i].MasterList);
                PNode.ChildNodes.Add(SubNode);
            }
        }
        #endregion

        #region LoadMasterTree
        private void LoadMasterTree(TreeNode PNode, List<MasterInfo> objMasterTree)
        {
            //DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            List<ControllerInfo> ControllerTree;

            for (int i = 0; i < objMasterTree.Count; i++)
            {
                ControllerTree = new List<ControllerInfo>();
                sql = "";
                liSqlPara.Clear();
                TreeNode SubNode = new TreeNode();

                // 語系切換：string_Connection = 連線 / Connection
                string string_Connenect_Device = GetLocalResourceObject("string_Connenect_Device").ToString();

                SubNode.Text += "[" + objMasterTree[i].MstNo + "] " + string_Connenect_Device + " - (" + objMasterTree[i].MstDesc + ")";

                // wei 2017/5/8 加入圖片判斷
                if (objMasterTree[i].MstStatus == "1")
                {
                    SubNode.ImageUrl = "~/Img/22.png";
                }
                else if (objMasterTree[i].MstStatus == "2")
                {
                    SubNode.ImageUrl = "~/Img/42.png";
                }
                else
                {
                    SubNode.ImageUrl = "~/Img/43.png";
                }

                txt_NodeTypeList.Value += "MASTER,";
                txt_NodeIDList.Value += objMasterTree[i].MstID.ToString() + ",";
                SubNode.NavigateUrl = "#";

                sql = " SELECT CtrlID, CtrlNo, CtrlName, CtrlStatus FROM B01_Controller WHERE MstID = ? ORDER BY CtrlNo ";
                liSqlPara.Add("S:" + objMasterTree[i].MstID);
                this.MyoAcsDB.GetDataTable("ControllerTable", sql, liSqlPara, out dt);

                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    this.CurrentCtrls++;
                    ControllerInfo ControllerNode = new ControllerInfo();
                    ControllerNode.CtrlID = int.Parse(dt.Rows[k]["CtrlID"].ToString());
                    ControllerNode.CtrlNo = dt.Rows[k]["CtrlNo"].ToString();
                    ControllerNode.CtrlName = dt.Rows[k]["CtrlName"].ToString();
                    ControllerNode.CtrlStatus = dt.Rows[k]["CtrlStatus"].ToString();

                    ControllerTree.Add(ControllerNode);
                }

                objMasterTree[i].ControllerList = ControllerTree;
                LoadCtrlTree(SubNode, objMasterTree[i].ControllerList);
                PNode.ChildNodes.Add(SubNode);
            }
        }
        #endregion

        #region LoadCtrlTree
        private void LoadCtrlTree(TreeNode PNode, List<ControllerInfo> objControllerTree)
        {
            //DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            for (int i = 0; i < objControllerTree.Count; i++)
            {
                sql = "";
                liSqlPara.Clear();
                TreeNode SubNode = new TreeNode();
                SubNode.Text = "[" + objControllerTree[i].CtrlNo + "] " + objControllerTree[i].CtrlName;

                // wei 2017/5/8 加入圖片判斷
                if (objControllerTree[i].CtrlStatus == "1")
                {
                    SubNode.ImageUrl = "~/Img/22.png";
                }
                else if (objControllerTree[i].CtrlStatus == "2")
                {
                    SubNode.ImageUrl = "~/Img/42.png";
                }
                else
                {
                    SubNode.ImageUrl = "~/Img/43.png";
                }

                txt_NodeTypeList.Value += "CONTROLLER,";
                txt_NodeIDList.Value += objControllerTree[i].CtrlID.ToString() + ",";
                SubNode.NavigateUrl = "#";

                #region Process String
                sql = " SELECT ReaderID, ReaderNo, ReaderName FROM B01_Reader WHERE CtrlID = ? ORDER BY ReaderNo ";
                #endregion

                liSqlPara.Add("S:" + objControllerTree[i].CtrlID);

                this.MyoAcsDB.GetDataTable("ReaderTable", sql, liSqlPara, out dt);

                ReaderInfo ReaderNode;
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    ReaderNode = new ReaderInfo();
                    ReaderNode.ReaderID = int.Parse(dt.Rows[k]["ReaderID"].ToString());
                    ReaderNode.ReaderNo = dt.Rows[k]["ReaderNo"].ToString();
                    ReaderNode.ReaderName = dt.Rows[k]["ReaderName"].ToString();
                    BuildReaderTree(SubNode, ReaderNode);
                }

                PNode.ChildNodes.Add(SubNode);
            }
        }
        #endregion

        #region BuildReaderTree
        private void BuildReaderTree(TreeNode PNode, ReaderInfo objReaderList)
        {
            TreeNode SubNode = new TreeNode();
            SubNode.Text = "[" + objReaderList.ReaderNo + "] " + objReaderList.ReaderName;
            txt_NodeTypeList.Value += "READER,";
            txt_NodeIDList.Value += objReaderList.ReaderID.ToString() + ",";
            SubNode.NavigateUrl = "#";
            PNode.ChildNodes.Add(SubNode);
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

        #region Master 相關

        #region Master_CreateCtrlModel
        private void Master_CreateCtrlModel()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt = new DataTable();

            this.Master_Input_CtrlModel.Items.Clear();

            // 選取資料 / Select Data
            ListItem Item = new ListItem();
            Item.Text = GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString();
            Item.Value = "";
            this.Master_Input_CtrlModel.Items.Add(Item);

            sql = @" SELECT DISTINCT ItemName, ItemNo FROM B00_ItemList WHERE ItemClass = 'EquModel' ";
            oAcsDB.GetDataTable("EquNoTable", sql, liSqlPara, out dt);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new ListItem();
                Item.Text = dr["ItemName"].ToString();
                Item.Value = dr["ItemNo"].ToString();
                this.Master_Input_CtrlModel.Items.Add(Item);
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

        #region LoadMaster
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadMaster(string MstID)
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
                    Mst.MstNo, 
                    Mst.MstDesc, 
                    Dci.DciName, 
                    Mst.MstType, 
                    Mst.MstConnParam, 
                    Mst.MstModel, 
                    Mst.MstStatus, 
                    Mst.CtrlModel,
                    Mst.LinkMode, 
                    Mst.AutoReturn, 
                    Dci.DciID, 
                    Mst.MstID, 
                    Mst.MstFwVer, 
                    (SELECT COUNT(*) FROM [B01_Controller] WHERE MstID = Mst.MstID) AS CtrlCount 
                FROM B01_Master AS Mst
                LEFT JOIN B01_DeviceConnInfo AS Dci ON Dci.DciID = Mst.DciID
                WHERE Mst.MstID = ? ";

            liSqlPara.Add("I:" + MstID.Trim());
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

        #region InsertMaster
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object InsertMaster(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            MessageObject1 objRet = new MessageObject1();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            // 驗證輸入的值
            objRet = Check_Input_DB_Master(DataArray, "Insert");

            if (objRet.result)
            {
                string[] strTrmp = DataArray[4].Split('_');

                if (strTrmp[0] != "")
                {
                    IPAddress address = null;
                    IPAddress.TryParse(strTrmp[0], out address);

                    if (address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        DataArray[4] = DataArray[4].Replace("_", ":");
                    }
                    else if (address.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        DataArray[4] = DataArray[4].Replace("_", ",");
                    }
                }

                #region Process String
                sql = @" 
                    INSERT INTO B01_Master 
                    (
                        MstNo, 
                        MstDesc, 
                        MstType, 
                        MstConnParam, 
                        MstModel, 
                        MstStatus, 
                        CtrlModel, 
                        LinkMode, 
                        AutoReturn, 
                        DciID, 
                        MstFwVer, 
                        CreateUserID, 
                        CreateTime, 
                        UpdateUserID, 
                        UpdateTime 
                    ) 
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?) ";

                liSqlPara.Add("A:" + DataArray[0].Trim());      // 連線裝置編號
                liSqlPara.Add("S:" + DataArray[1].Trim());      // 連線裝置說明
                liSqlPara.Add("S:" + DataArray[3].Trim());      // 連線類型 
                liSqlPara.Add("S:" + DataArray[4].Trim());      // IP + PORT
                liSqlPara.Add("A:" + DataArray[5].Trim());      // 連線裝置機型
                liSqlPara.Add("I:" + DataArray[6].Trim());      // 狀態
                liSqlPara.Add("A:" + DataArray[7].Trim());      // 控制器機型
                liSqlPara.Add("I:" + DataArray[8].Trim());      // 連線模式
                liSqlPara.Add("I:" + DataArray[9].Trim());      // 自動回傳    
                liSqlPara.Add("I:" + DataArray[10].Trim());     // DciID
                liSqlPara.Add("S:" + DataArray[12].Trim());     // 韌體版本
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);

                try
                {
                    int intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);

                    if (intResult > -1)
                    {
                        #region 寫入B00_SysLog
                        int[] intAry = new int[] { 0, 1, 4, 5, 6, 7, 9, 12 };
                        string[] strAry = new string[] { "MstNo", "MstDesc", "MstConnParam", "MstModel", "MstStatus", "CtrlModel", "AutoReturn", "MstFwVer" };

                        List<Machine> liData = new List<Machine>();
                        int r = 0;
                        foreach (int g in intAry)
                        {
                            //string strOldValue = drTemp.DataReader.GetValue(g).ToString();
                            string strNewValue = DataArray[g].Trim();

                            liData.Add(new Machine(strAry[r], strMaster[r], "", strNewValue));
                            r++;
                        }

                        oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, sUserID, sUserName, "020102",
                            DataArray[0].Trim(),
                            DataArray[1].Trim(),
                            string.Format("{0}", CompareVaule("Insert", liData)), "新增連線裝置");
                        #endregion

                        #region 取得剛剛新增的MstNo的MstID回傳使用
                        sql = @" 
                            SELECT TOP 1 MstID FROM B01_Master 
                            WHERE MstNo = ? AND DciID = ? ";

                        liSqlPara.Clear();
                        liSqlPara.Add("A:" + DataArray[0].Trim());
                        liSqlPara.Add("I:" + DataArray[10].Trim());

                        objRet.message = oAcsDB.GetStrScalar(sql, liSqlPara);
                        #endregion

                        #region 取得這個 DciNO 和 MstNo 的順位，依此計算出新增的Master，在整體中的順位
                        int intUp = 0;

                        string strDciNo = oAcsDB.GetStrScalar(string.Format(@"SELECT DciNO FROM B01_DeviceConnInfo WHERE DciID = {0}", DataArray[10].Trim()));

                        if (strDciNo == null) strDciNo = "";

                        /*
                            1. 取小於等於 DciNO 的 B01_DeviceConnInfo 的數量
                            2. 取小於 DciNO 的 B01_Master 的數量
                            3. 取等於 DciNO 和 小於 MstNo 的 B01_Master 的數量
                            4. 取小於 DciNO 的 B01_Controller 的數量
                            5. 取等於 DciNO 和 小於 MstNo 的 B01_Controller 的數量
                            6. 取小於 DciNO 的 B01_Reader 的數量
                            7. 取等於 DciNO 和 小於 MstNo 的 B01_Reader 的數量

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
		                            SELECT COUNT(*) FROM B01_Master M 
		                            INNER JOIN B01_DeviceConnInfo D ON D.DciID = M.DciID 
		                            WHERE D.DciNO = '{0}' AND M.MstNo < '{1}' 
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
		                            SELECT COUNT(*) FROM B01_Controller WHERE MstID IN 
		                            (
			                            SELECT MstID FROM B01_Master M
			                            INNER JOIN B01_DeviceConnInfo D ON D.DciID = M.DciID 
			                            WHERE D.DciNO = '{0}' AND M.MstNo < '{1}'
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
	                            ) + 
	                            (
		                            SELECT COUNT(*) FROM B01_Reader WHERE CtrlID IN 
		                            (
			                            SELECT CtrlID FROM B01_Controller WHERE MstID IN 
			                            (
				                            SELECT MstID FROM B01_Master M
				                            INNER JOIN B01_DeviceConnInfo D ON D.DciID = M.DciID 
				                            WHERE D.DciNO = '{0}' AND M.MstNo < '{1}'
			                            )
		                            )
	                            )  ", strDciNo, DataArray[0].Trim());

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
                        objRet.message = "新增連線裝置失敗。";
                        objRet.result = false;
                    }
                }
                catch (Exception ex)
                {
                    objRet.message = string.Format("[InsertMaster] {0}", ex.Message);
                    objRet.result = false;
                }

                #endregion
            }

            objRet.act = "InsertMaster";
            return objRet;
        }
        #endregion

        #region UpdateMaster
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object UpdateMaster(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            objRet = Check_Input_DB_Master(DataArray, "Update");

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
                            DataArray[4] = DataArray[4].Replace("_", ":");
                        }
                        else if (address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            DataArray[4] = DataArray[4].Replace("_", ",");
                        }
                    }
                }

                sql = @" 
                    UPDATE B01_Master
                    SET 
                        MstNo = ?, 
                        MstDesc = ?, 
                        MstType = ?, 
                        MstConnParam = ?, 
                        MstModel = ?, 
                        MstStatus = ?, 
                        CtrlModel = ?, 
                        LinkMode = ?, 
                        AutoReturn = ?, 
                        MstFwVer = ?, 
                        UpdateUserID = ?, 
                        UpdateTime = ? 
                    WHERE MstID = ? ";

                liSqlPara.Add("A:" + DataArray[0].Trim());
                liSqlPara.Add("S:" + DataArray[1].Trim());
                liSqlPara.Add("S:" + DataArray[3].Trim());
                liSqlPara.Add("S:" + DataArray[4].Trim());
                liSqlPara.Add("A:" + DataArray[5].Trim());
                liSqlPara.Add("I:" + DataArray[6].Trim());
                liSqlPara.Add("A:" + DataArray[7].Trim());
                liSqlPara.Add("I:" + DataArray[8].Trim());
                liSqlPara.Add("I:" + DataArray[9].Trim());
                liSqlPara.Add("S:" + DataArray[12].Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("I:" + DataArray[11].Trim());

                try
                {
                    int intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);

                    if (intResult > -1)
                    {
                        #region 寫入B00_SysLog
                        int[] intAry = new int[] { 0, 1, 4, 5, 6, 7, 9, 12 };
                        string[] strAry = new string[] { "MstNo", "MstDesc", "MstConnParam", "MstModel", "MstStatus", "CtrlModel", "AutoReturn", "MstFwVer" };

                        List<Machine> liData = new List<Machine>();
                        int r = 0;
                        foreach (int g in intAry)
                        {
                            string strOldValue = drTemp.DataReader.GetValue(g).ToString();
                            string strNewValue = DataArray[g].Trim();

                            liData.Add(new Machine(strAry[r], strMaster[r], strOldValue, strNewValue));
                            r++;
                        }

                        oAcsDB.WriteLog(DB_Acs.Logtype.資料修改, sUserID, sUserName, "020102",
                            drTemp.DataReader.GetValue(0).ToString(),
                            drTemp.DataReader.GetValue(1).ToString(),
                            string.Format("連線裝置編碼：{0}，{1}", DataArray[0].Trim(), CompareVaule("Update", liData)),
                            "修改連線裝置");
                        #endregion

                        objRet.message = DataArray[11].Trim();  // 取得MstID回傳使用
                    }
                    else
                    {
                        objRet.result = false;
                        objRet.message = "修改連線裝置資料失敗。";
                    }
                }
                catch (Exception ex)
                {
                    objRet.result = false;
                    objRet.message = string.Format("[UpdateMaster] {0}", ex.Message);
                }
            }

            objRet.act = "UpdateMaster";
            return objRet;
        }
        #endregion

        #region DeleteMaster
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object DeleteMaster(string UserID, string[] DataArray)
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
                #region 暫不用
                /*
                #region 刪除連線裝置所屬的所有B01_EquParaData資料
                if (iRet != -1)
                {
                    sql = @" 
                        DELETE FROM [B01_EquParaData] 
                        WHERE EquID IN 
                        (
                            SELECT EquID FROM B01_EquData WHERE EquNo IN
	                        (
		                        SELECT EquNo FROM B01_Reader WHERE ReaderID IN  
		                        (
			                        SELECT rdr.ReaderID FROM B01_Reader AS rdr
			                        LEFT JOIN B01_Controller AS clr ON clr.CtrlID = rdr.CtrlID
			                        LEFT JOIN B01_Master AS mtr ON mtr.MstID = clr.MstID
			                        WHERE mtr.MstID = ? 
		                        ) 
	                        )
                        ) ";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[11].Trim());
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                } 
                #endregion

                #region 刪除連線裝置所屬的所有設備
                if (iRet != -1)
                {
                    sql = @" 
                        DELETE FROM [B01_EquData] 
                        WHERE EquNo IN 
	                    (
                            SELECT EquNo FROM B01_Reader WHERE ReaderID IN  
		                    (
                                SELECT rdr.ReaderID FROM B01_Reader AS rdr
		                        LEFT JOIN B01_Controller AS clr ON clr.CtrlID = rdr.CtrlID
		                        LEFT JOIN B01_Master AS mtr ON mtr.MstID = clr.MstID
		                        WHERE mtr.MstID = ? 
                            ) 
                        )";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[11].Trim());
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #region 刪除連線裝置所屬的讀卡機
                if (iRet != -1)
                {
                    sql = @" 
                        DELETE FROM B01_Reader 
                        WHERE ReaderID IN
                        ( 
                            SELECT rdr.ReaderID FROM B01_Reader AS rdr
                            LEFT JOIN B01_Controller AS clr ON clr.CtrlID = rdr.CtrlID
                            LEFT JOIN B01_Master AS mtr ON mtr.MstID = clr.MstID
                            WHERE mtr.MstID = ? 
                        ) ";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[11].Trim());
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #region 刪除連線裝置所屬的控制器
                if (iRet != -1)
                {
                    sql = @" DELETE FROM B01_Controller WHERE MstID = ? ";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[11].Trim());
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion
                */
                #endregion

                sql = @"
                    SELECT 1 FROM [B01_Controller] Ctr 
                    LEFT JOIN dbo.B01_Master Mst ON Mst.MstID = Ctr.MstID 
                    WHERE Ctr.MstID = ? ";
                liSqlPara.Clear();
                liSqlPara.Add("I:" + DataArray[11].Trim());

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
                        else
                        {
                            #region 刪除連線裝置
                            if (iRet > -1)
                            {
                                sql = @" DELETE FROM B01_Master WHERE MstID = ? ";
                                liSqlPara.Clear();
                                liSqlPara.Add("I:" + DataArray[11].Trim());
                                iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                            }
                            #endregion

                            if (iRet > -1)
                            {
                                #region 寫入B00_SysLog
                                oAcsDB.WriteLog(DB_Acs.Logtype.資料刪除, sUserID, sUserName, "020102",
                                    drTemp.DataReader.GetValue(0).ToString(),
                                    drTemp.DataReader.GetValue(1).ToString(),
                                    string.Format("設備連線編碼：{0}", DataArray[0]),
                                    "刪除連線裝置");
                                #endregion

                                #region 取得MstID回傳使用
                                objRet.message = DataArray[11].Trim();
                                #endregion
                            }
                            else
                            {
                                objRet.message = "刪除連線裝置失敗。";
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
                    objRet.message = string.Format("[DeleteMaster] {0}", ex.Message);
                    objRet.result = false;
                }
            }
            #endregion

            objRet.act = "DeleteMaster";
            return objRet;
        }
        #endregion

        #region Check_Input_DB_Master
        protected static MessageObject1 Check_Input_DB_Master(string[] DataArray, string Mode)
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
                objRet.message += string_ConnectDevice[0];
            }
            else if (DataArray[0].Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：連線裝置編號 字數超過上限 
                objRet.message += string_ConnectDevice[1];
            }

            if (DataArray[1].Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：連線裝置說明 字數超過上限 
                objRet.message += string_ConnectDevice[2];
            }

            if (string.IsNullOrEmpty(DataArray[2].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：使用連線 必須指定 
                objRet.message += string_ConnectDevice[3];
            }

            #region 參數
            if (objRet.result)
            {
                string[] ParaArray = DataArray[4].Split('_');

                if (string.IsNullOrEmpty(ParaArray[0].Trim()))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：IP 必須輸入
                    objRet.message += string_Para_T[0];
                }

                if (string.IsNullOrEmpty(ParaArray[1].Trim()))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：Port 必須輸入
                    objRet.message += string_Para_T[1];
                }

                if (objRet.result && (!CheckIP(ParaArray[0]) || !CheckPort(ParaArray[1])))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：IP位置不合法
                    objRet.message += string_Para_T[2];
                }

                //if (DataArray[3] == "T")
                //{
                //    if (string.IsNullOrEmpty(ParaArray[0].Trim()))
                //    {
                //        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                //        objRet.result = false;

                //        // 訊息：IP 必須輸入
                //        objRet.message += string_Para_T[0];
                //    }

                //    if (string.IsNullOrEmpty(ParaArray[1].Trim()))
                //    {
                //        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                //        objRet.result = false;

                //        // 訊息：Port 必須輸入
                //        objRet.message += string_Para_T[1];
                //    }

                //    if (objRet.result && (!CheckIP(ParaArray[0]) || !CheckPort(ParaArray[1])))
                //    {
                //        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                //        objRet.result = false;

                //        // 訊息：IP位置不合法
                //        objRet.message += string_Para_T[2];
                //    }
                //}
                //else if (DataArray[3] == "C")
                //{
                //    string[] Paras = ParaArray[1].Split(',');

                //    if (string.IsNullOrEmpty(ParaArray[0].Trim()))
                //    {
                //        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                //        objRet.result = false;

                //        // 訊息：ComPort 必須指定
                //        objRet.message += string_Para_C[0];
                //    }

                //    if (string.IsNullOrEmpty(Paras[0].Trim()))
                //    {
                //        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                //        objRet.result = false;

                //        // 訊息：BaudRate 必須指定
                //        objRet.message += string_Para_C[1];
                //    }

                //    if (string.IsNullOrEmpty(Paras[1].Trim()))
                //    {
                //        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                //        objRet.result = false;

                //        // 訊息：Parity 必須指定
                //        objRet.message += string_Para_C[2];
                //    }

                //    if (string.IsNullOrEmpty(Paras[2].Trim()))
                //    {
                //        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                //        objRet.result = false;

                //        // 訊息：DataBits 必須指定
                //        objRet.message += string_Para_C[3];
                //    }

                //    if (string.IsNullOrEmpty(Paras[3].Trim()))
                //    {
                //        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                //        objRet.result = false;

                //        // 訊息：StopBits 必須指定
                //        objRet.message += string_Para_C[4];
                //    }
                //}
            }
            #endregion

            if (DataArray[5].Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：連線裝置機型 字數超過上限
                objRet.message += string_ConnectDevice[5];
            }

            if (string.IsNullOrEmpty(DataArray[6].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：狀態 必須指定
                objRet.message += string_ConnectDevice[6];
            }

            if (string.IsNullOrEmpty(DataArray[7].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：控制器機型 必須指定
                objRet.message += string_ConnectDevice[7];
            }

            //if (string.IsNullOrEmpty(DataArray[8].Trim()))
            //{
            //    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
            //    objRet.result = false;

            //    // 訊息：連線模式 必須指定
            //    objRet.message += string_ConnectDevice[8];
            //}

            if (string.IsNullOrEmpty(DataArray[9].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：自動回傳 必須指定
                objRet.message += string_ConnectDevice[9];
            }

            if (Encoding.Default.GetBytes(DataArray[12].Trim()).Length > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：韌體版本 字數超過上限
                objRet.message += string_ConnectDevice[10];
            }

            #endregion

            #region DB
            if (objRet.result)
            {
                switch (Mode)
                {
                    case "Insert":
                        sql = @" SELECT * FROM B01_Master WHERE MstNo = ? ";
                        liSqlPara.Add("S:" + DataArray[0].Trim());
                        break;
                    case "Update":
                        sql = @" SELECT * FROM B01_Master WHERE MstNo = ? AND MstID != ? ";
                        liSqlPara.Add("S:" + DataArray[0].Trim());
                        liSqlPara.Add("S:" + DataArray[11].Trim());
                        break;
                }

                oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                if (dr.Read())
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：此連線裝置編號已存在於系統中
                    objRet.message += string_Message[0];
                }
            }
            #endregion

            return objRet;
        }
        #endregion

        #endregion

        #region Controller 相關

        #region GetMstInfo
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] GetMstInfo(string FromMstID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            DBReader dr = new DBReader();

            sql = @" SELECT MstID, MstDesc, CtrlModel FROM B01_Master WHERE MstID = ? ";
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

        #region LoadController
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadController(string strUserID, string strCtrlID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            DBReader dr = null;

            #region Process String
            sql = @" 
                SELECT 
                    Controller.CtrlNo, 
                    Controller.CtrlName, 
                    Controller.CtrlDesc,
                    Master.MstDesc, 
                    Controller.CtrlModel, 
                    Controller.CtrlStatus,
                    Controller.CtrlAddr, 
                    Controller.CtrlFwVer, 
                    Master.MstID, 
                    (
                        SELECT TOP 1 ED.EquClass FROM [B01_EquData] ED 
                        LEFT JOIN B01_READER RD ON RD.EquNo = ED.EquNo 
                        LEFT JOIN B01_Controller CR ON CR.CtrlID = RD.CtrlID 
                        WHERE CR.CtrlID = Controller.CtrlID 
                    ) AS EquClass, 
                    Master.DciID, 
                    Controller.CtrlID,
                    dbo.Get_LocParentID(Controller.LocID, 'AREA') AS 'AREA', 
                    dbo.Get_LocParentID(Controller.LocID, 'BUILDING') AS 'BUILDING', 
                    dbo.Get_LocParentID(Controller.LocID, 'FLOOR') AS 'FLOOR'
                FROM B01_Controller AS Controller
                LEFT JOIN B01_Master AS Master ON Master.MstID = Controller.MstID
                WHERE Controller.CtrlID = ? ";

            liSqlPara.Add("I:" + strCtrlID.Trim());
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

                // wei 20170327 補足缺的設備參數 hideUserID.value、CtrlID
                AddParaMeterForControl(strUserID, strCtrlID.Trim());
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


        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object NewInsertController(string UserID, string[] DataArray)
        {

            DapperDataObjectLib.OrmDataObject odo = new DapperDataObjectLib.OrmDataObject("MsSql", Pub.GetDapperConnString());
            Pub.MessageObject objRet = CheckNewController(DataArray, "", "Insert", ref odo);
            string sql = "";
            if (objRet.result)
            {
                #region 新增控制器
                if (DataArray[1].Trim().Equals(""))
                {
                    DataArray[1] = DataArray[0].Trim();
                }
                sql = @"INSERT INTO B01_Controller (CtrlNo,CtrlName,CtrlDesc,CtrlAddr,CtrlModel,CtrlStatus,MstID,CreateUserID,CreateTime,UpdateuserID,UpdateTime) 
                                VALUES (@CtrlNo,@CtrlName,@CtrlDesc,@CtrlAddr,@CtrlModel,@CtrlStatus,@MstID,@User,GETDATE(),@User,GETDATE())";
                int intResult = odo.Execute(sql, new { CtrlNo = DataArray[0], CtrlName = DataArray[1], CtrlDesc = DataArray[2], CtrlAddr = DataArray[6], CtrlModel = DataArray[4], CtrlStatus = DataArray[5], MstID = DataArray[8], User = UserID });
                #endregion

                //取得目前的連線設備編號
                int DciID = odo.GetIntScalar("SELECT DciID FROM B01_master WHERE MstID=@MstID", new { MstID = DataArray[8] });
                int CtrlID = -1;
                if (intResult > -1)
                {
                    //取得新的控制器編號
                    CtrlID = odo.GetIntScalar("SELECT IDENT_CURRENT('B01_Controller')");
                    if (CtrlID == 0 || DciID == 0)
                    {
                        objRet.message = "新增控制器失敗。";
                        objRet.result = false;
                        objRet.act = "InsertController";
                        return objRet;
                    }
                    else
                    {
                        objRet.message = CtrlID.ToString();
                    }

                }
                else
                {
                    objRet.message = "新增控制器失敗。";
                    objRet.result = false;
                    objRet.act = "InsertController";
                    return objRet;
                }
                if (!GetInsertReadWithEquData(DataArray, CtrlID, DciID, UserID, ref odo))
                {
                    odo.Execute("DELETE B01_Controller WHERE CtrlID=@CtrlID", new { CtrlID = CtrlID });
                    objRet.message = "新增控制器失敗。";
                    objRet.result = false;
                }
            }
            objRet.act = "InsertController";
            return objRet;
        }

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


        public static Pub.MessageObject CheckNewController(string[] DataArray, string ItemID, string Mode, ref OrmDataObject odo)
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            int tempint;
            string sql = "";
            #region Input
            if (string.IsNullOrEmpty(DataArray[0].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：控制器編號 必須輸入
                objRet.message += string_Controller[0];
            }
            else if (DataArray[0].Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：控制器編號 字數超過上限
                objRet.message += string_Controller[1];
            }

            if (DataArray[1].Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：控制器名稱 字數超過上限
                objRet.message += string_Controller[2];
            }

            if (DataArray[2].Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：控制器說明 字數超過上限
                objRet.message += string_Controller[3];
            }

            //if (string.IsNullOrEmpty(DataArray[8].Trim()) && Mode == "Insert")
            //{
            //    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
            //    objRet.result = false;

            //    // 訊息：連線裝罝 必須指定
            //    objRet.message += string_Controller[4];
            //}

            if (string.IsNullOrEmpty(DataArray[4].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：機型 必須指定
                objRet.message += string_Controller[5];
            }

            if (string.IsNullOrEmpty(DataArray[5].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：控制器狀態 必須指定
                objRet.message += string_Controller[6];
            }

            if (string.IsNullOrEmpty(DataArray[6].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：機號 必須指定
                objRet.message += string_Controller[7];
            }
            else if (!int.TryParse(DataArray[6].Trim(), out tempint))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：機號 必須為數值
                objRet.message += string_Controller[8];
            }


            //else if (DataArray[6].Trim().Count() > 3)
            //{
            //    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
            //    objRet.result = false;

            //    // 訊息：機號 字數超過上限
            //    objRet.message += string_Controller[9];
            //}

            // wei 20170207 
            // 如果機型是NanoEye(虹膜設定)，字數上限則為4碼，一般則為3碼。
            if (DataArray[4].Trim().Equals("NanoEye"))
            {
                if (DataArray[6].Trim().Count() > 4)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：機號 字數超過上限
                    objRet.message += string_Controller[9];
                }
            }
            else
            {
                if (DataArray[6].Trim().Count() > 3)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：機號 字數超過上限
                    objRet.message += string_Controller[9];
                }

                int.TryParse(DataArray[6].Trim(), out tempint);
                if (tempint < 1 || tempint > 255)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：機號 需介於1與255之間
                    objRet.message += string_Controller[10];
                }
            }


            #endregion

            #region DB

            if (objRet.result)
            {
                #region check controller
                switch (Mode)
                {
                    case "Insert":
                        sql = "SELECT * FROM B01_Controller WHERE CtrlNo=@Ctrl ";
                        break;
                    case "Update":
                        sql = "SELECT * FROM B01_Controller WHERE CtrlNo=@Ctrl AND CtrlID!=@ID";
                        break;
                }
                if (odo.GetQueryResult(sql, new { Ctrl = DataArray[0].Trim(), ID = ItemID }).Count() > 0)
                {
                    if (!string.IsNullOrEmpty(objRet.message))
                        objRet.message += "<br />";
                    objRet.result = false;
                    objRet.message += "此控制器編號已存在於系統中";
                }
                #endregion

                #region check reader
                switch (Mode)
                {
                    case "Insert":
                        sql = @" SELECT * FROM B01_Controller WHERE CtrlAddr = @Addr AND MstID = @Mst";
                        break;
                    case "Update":
                        sql = @" SELECT * FROM B01_Controller WHERE CtrlAddr = @Addr AND MstID = @Mst AND CtrlID != @ID ";
                        break;
                }
                if (odo.GetQueryResult(sql, new { Addr = DataArray[6], Mst = DataArray[8], ID = ItemID }).Count() > 0)
                {
                    if (!string.IsNullOrEmpty(objRet.message))
                        objRet.message += "<br />";
                    objRet.result = false;
                    objRet.message += "此機號已存在於系統中";
                }
                #endregion
            }

            #endregion



            return objRet;
        }


        #region InsertController
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object InsertController(string UserID, string[] DataArray)
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

            objRet = Check_Input_DB_Controller(DataArray, "", "Insert", ref oAcsDB);
            #region 新增控制器資料
            if (objRet.result)
            {
                #region 新增控制器
                // 1. 控制器名稱是空白，就帶入控制器編號
                if (DataArray[1].Trim().Equals(""))
                {
                    DataArray[1] = DataArray[0].Trim();
                }

                string sLocID = DataArray[15].Trim();
                if (string.IsNullOrEmpty(sLocID)) sLocID = DataArray[14].Trim();
                if (string.IsNullOrEmpty(sLocID)) sLocID = DataArray[13].Trim();

                sql = @" 
                    INSERT INTO B01_Controller 
                    (
                        CtrlNo, 
                        CtrlName, 
                        CtrlDesc, 
                        CtrlAddr, 
                        CtrlModel, 
                        CtrlStatus, 
                        MstID, 
                        CreateUserID, CreateTime, UpdateUserID, UpdateTime, 
                        LocID
                    )  
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?) ";

                liSqlPara.Clear();
                liSqlPara.Add("S:" + DataArray[0].Trim());
                liSqlPara.Add("S:" + DataArray[1].Trim());
                liSqlPara.Add("S:" + DataArray[2].Trim());
                liSqlPara.Add("I:" + DataArray[6].Trim());      // CtrlAddr
                liSqlPara.Add("S:" + DataArray[4].Trim());
                liSqlPara.Add("I:" + DataArray[5].Trim());
                liSqlPara.Add("I:" + DataArray[8].Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("I:" + sLocID);

                oAcsDB.BeginTransaction();

                int intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);

                int intCtrlID = -1;
                int intDciID = -1;

                if (intResult > -1)
                {
                    sql = " SELECT IDENT_CURRENT('B01_Controller') ";
                    intCtrlID = oAcsDB.GetIntScalar(sql);

                    // 取得 Default DciID
                    sql = " SELECT DciID FROM B01_master WHERE MstID = ? ";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[8].Trim());
                    intDciID = oAcsDB.GetIntScalar(sql, liSqlPara);

                    if (intCtrlID != -1 && intDciID != -1)
                    {
                        oAcsDB.Commit();
                    }
                    else
                    {
                        oAcsDB.Rollback();

                        objRet.message = "新增控制器失敗。";
                        objRet.result = false;
                        objRet.act = "InsertController";
                        return objRet;
                    }
                }
                else
                {
                    oAcsDB.Rollback();

                    objRet.message = "新增控制器失敗。";
                    objRet.result = false;
                    objRet.act = "InsertController";
                    return objRet;
                }

                #endregion

                #region 新增暫存的讀卡機和暫存的設備

                string strReaderNo = "";
                string strReaderName = "";
                string strEquNo = "";
                string strEquName = "";
                string strEquEName = "";

                List<string[]> liLog = new List<string[]>();

                if (int.Parse(DataArray[12]) > 0)
                {
                    #region 讀取預設的卡號長度
                    string ss = " SELECT [ParaValue] FROM [B00_SysParameter] WHERE  ParaNo='CardDefaultLength' ";
                    string strCardlength = oAcsDB.GetStrScalar(ss);
                    if (strCardlength == null)
                    {
                        strCardlength = "10";
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

                        #region [no use]驗證
                        //string[] tmpAry = new string[22];
                        //tmpAry[0] = i.ToString();           // ReaderNo
                        //tmpAry[1] = strReaderName;          // ReaderName
                        //tmpAry[2] = "";                     // ReaderDesc
                        //tmpAry[3] = strEquNo;               // EquNo
                        //tmpAry[4] = "進";                   // Dir
                        //tmpAry[5] = ctrlid.ToString();      // CtrlID
                        //tmpAry[6] = "";
                        //tmpAry[7] = "";
                        //tmpAry[8] = DataArray[4].Trim();    // EquModel
                        //tmpAry[9] = strEquNo;               // EquNo
                        //tmpAry[10] = "";          
                        //tmpAry[11] = DataArray[8].Trim();   // EquClass
                        //tmpAry[12] = "1";                   // Building
                        //tmpAry[13] = "1";                   // Floor
                        //tmpAry[14] = strEquNo;              // EquName
                        //tmpAry[15] = strEquNo;              // EquEName
                        //tmpAry[16] = DataArray[9].Trim();   // DciID
                        //tmpAry[17] = "8";                   // CardNoLen
                        //tmpAry[18] = "";//
                        //tmpAry[19] = "";//
                        //tmpAry[20] = "";//
                        //tmpAry[21] = DataArray[11].Trim();  // EquID

                        //objRet = Check_Input_DB_Reader(tmpAry, "", "Insert");

                        #endregion

                        sql1 += @" 
                            INSERT INTO B01_Reader 
                            (
                                ReaderNo,ReaderName,CtrlID,CreateUserID,CreateTime,
                                UpdateUserID,UpdateTime,ReaderDesc,EquNo,[Dir]  
                            ) 
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                        liSqlPara1.Add("I:" + strReaderNo);                 // 讀卡機編號
                        liSqlPara1.Add("S:" + strReaderName);               // 
                        liSqlPara1.Add("I:" + intCtrlID.ToString());        // CtrlID
                        liSqlPara1.Add("S:" + UserID.ToString());
                        liSqlPara1.Add("D:" + Time);
                        liSqlPara1.Add("S:" + UserID.ToString());
                        liSqlPara1.Add("D:" + Time);
                        liSqlPara1.Add("S:" + strReaderName);
                        liSqlPara1.Add("S:" + strEquNo);
                        liSqlPara1.Add("S:進");
                        #endregion

                        #region 新增預設設備
                        sql2 += @" 
	                        INSERT INTO B01_EquData 
	                        (
		                        EquClass,EquModel,EquNo,Building,[Floor],
		                        EquName,EquEName,DciID,CardNoLen,InToCtrlAreaID,
		                        OutToCtrlAreaID,IsAndTrt,  
		                        CreateUserID,CreateTime,UpdateUserID, UpdateTime, LocID , Area
	                        ) 
	                        VALUES  
	                        (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?) ";

                        liSqlPara2.Add("S:" + DataArray[9].Trim());     // EquClass
                        liSqlPara2.Add("S:" + DataArray[4].Trim());     // EquModel
                        liSqlPara2.Add("S:" + strEquNo);                // EquNo
                        liSqlPara2.Add("S:" + DataArray[14].Trim());    // Building
                        liSqlPara2.Add("S:" + DataArray[15].Trim());                             // Floor
                        liSqlPara2.Add("S:" + strEquName);              // EquName
                        liSqlPara2.Add("S:" + strEquEName);             // EquEName
                        liSqlPara2.Add("I:" + intDciID.ToString());     // DciID
                        liSqlPara2.Add("I:" + strCardlength);           // CardNoLen 
                        liSqlPara2.Add("I:0");                          // InToCtrlAreaID
                        liSqlPara2.Add("I:0");                          // OutToCtrlAreaID
                        liSqlPara2.Add("I:0");                          // IsAndTrt
                        liSqlPara2.Add("S:" + UserID.ToString());
                        liSqlPara2.Add("D:" + Time);
                        liSqlPara2.Add("S:" + UserID.ToString());
                        liSqlPara2.Add("D:" + Time);
                        liSqlPara2.Add("I:" + sLocID);
                        liSqlPara2.Add("S:" + DataArray[13].Trim());
                        #endregion

                        #region 取得新增LOG用的資料
                        string[] strLog = new string[4];

                        strLog[0] = strReaderNo;    // ReaderNo
                        strLog[1] = strReaderName;  // ReaderName
                        strLog[2] = strEquNo;       // EquNo
                        strLog[3] = strEquName;     // EquName

                        liLog.Add(strLog);
                        #endregion
                    }

                    oAcsDB.BeginTransaction();

                    int intResult1 = oAcsDB.SqlCommandExecute(sql1, liSqlPara1);
                    int intResult2 = oAcsDB.SqlCommandExecute(sql2, liSqlPara2);

                    if (intResult1 > 0 && intResult2 > 0)
                    {
                        oAcsDB.Commit();

                        #region 寫入B00_SysLog
                        #region 新增控制器
                        int[] intAry = new int[] { 0, 1, 2, 6, 5 };
                        string[] strAry = new string[] { "CtrlNo", "CtrlName", "CtrlDesc", "CtrlAddr", "CtrlStatus" };

                        List<Machine> liData = new List<Machine>();
                        int r = 0;
                        foreach (int g in intAry)
                        {
                            //string strOldValue = drTemp.DataReader.GetValue(g).ToString();
                            string strNewValue = DataArray[g].Trim();

                            liData.Add(new Machine(strAry[r], strCtrl[r], "", strNewValue));
                            r++;
                        }

                        oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, sUserID, sUserName, "020102",
                            DataArray[0].Trim(),
                            DataArray[1].Trim(),
                            string.Format("{0}", CompareVaule("Insert", liData)), "新增控制器");
                        #endregion

                        #region 新增讀卡機&設備
                        foreach (string[] sArray in liLog)
                        {
                            oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, sUserID, sUserName, "020102",
                                sArray[0], sArray[1],
                                string.Format("讀卡機編號：{0}，讀卡機名稱：{1}", sArray[0], sArray[1]),
                                "新增讀卡機(預設值)");

                            oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, sUserID, sUserName, "020102",
                                sArray[0], sArray[1],
                                string.Format("設備編號：{0}，設備名稱：{1}", sArray[2], sArray[3]),
                                "新增設備(預設值)");
                        }
                        #endregion
                        #endregion

                        #region 取得剛剛新增的CtrlNo的CtrlID回傳使用
                        objRet.message = intCtrlID.ToString();
                        #endregion

                        // 新增設備後的處理(新增設備(EquID)到全群組(EG999)、給新增的設備，設定設備參數預設值)
                        string strCtrlID = intCtrlID.ToString();
                        AfterAddReaderHealWith(UserID, strCtrlID);
                    }
                    else
                    {
                        oAcsDB.Rollback();

                        #region 交易失敗，刪掉剛剛新增的控制器
                        string strDel = " DELETE FROM B01_Controller WHERE CtrlID = ?";

                        liSqlPara.Clear();
                        liSqlPara.Add("I:" + intCtrlID);
                        oAcsDB.SqlCommandExecute(strDel, liSqlPara);
                        #endregion

                        objRet.message = "新增控制器失敗。";
                        objRet.result = false;
                    }
                }
                #endregion
            }
            #endregion

            objRet.act = "InsertController";
            return objRet;
        }
        #endregion

        #region UpdateController
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object UpdateController(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            objRet = Check_Input_DB_Controller(DataArray, DataArray[11], "Update", ref oAcsDB);
            #region 編輯控制器資料
            if (objRet.result)
            {
                string sLocID = DataArray[15].Trim();
                if (string.IsNullOrEmpty(sLocID)) sLocID = DataArray[14].Trim();
                if (string.IsNullOrEmpty(sLocID)) sLocID = DataArray[13].Trim();

                sql = @" 
                    UPDATE B01_Controller SET 
                        CtrlNo = ?, 
                        CtrlName = ?, 
                        CtrlDesc = ?, 
                        CtrlAddr = ?, 
                        CtrlModel = ?, 
                        CtrlStatus = ?,
                        UpdateUserID = ?, UpdateTime = ?, LocID = ?
                    WHERE CtrlID = ? ";
                liSqlPara.Add("S:" + DataArray[0].Trim());
                liSqlPara.Add("S:" + DataArray[1].Trim());
                liSqlPara.Add("S:" + DataArray[2].Trim());
                liSqlPara.Add("I:" + DataArray[6].Trim());
                liSqlPara.Add("S:" + DataArray[4].Trim());
                liSqlPara.Add("I:" + DataArray[5].Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("I:" + sLocID);
                liSqlPara.Add("I:" + DataArray[11].Trim());
               

                // 更新設備類別(EquClass)
                sql += @" 
                    UPDATE B01_EquData SET EquClass = ? 
                    WHERE EquNo IN ( SELECT EquNo FROM B01_Reader WHERE CtrlID = ? ) ";
                liSqlPara.Add("S:" + DataArray[9].Trim());
                liSqlPara.Add("I:" + DataArray[11].Trim());

                int intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);

                #region syslog:update
                if (intResult > -1)
                {
                    #region 寫入B00_SysLog
                    int[] intAry = new int[] { 0, 1, 2, 6, 5 };
                    string[] strAry = new string[] { "CtrlNo", "CtrlName", "CtrlDesc", "CtrlAddr", "CtrlStatus" };

                    List<Machine> liData = new List<Machine>();
                    int r = 0;
                    foreach (int g in intAry)
                    {
                        string strOldValue = drTemp.DataReader.GetValue(g).ToString();
                        string strNewValue = DataArray[g].Trim();

                        liData.Add(new Machine(strAry[r], strCtrl[r], strOldValue, strNewValue));
                        r++;
                    }

                    oAcsDB.WriteLog(DB_Acs.Logtype.資料修改, sUserID, sUserName, "020102",
                        drTemp.DataReader.GetValue(0).ToString(),
                        drTemp.DataReader.GetValue(1).ToString(),
                        string.Format("{0}", CompareVaule("Update", liData)),
                        "修改控制器");
                    #endregion

                    objRet.message = DataArray[11].Trim();
                }
                else
                {
                    objRet.result = false;
                    objRet.message = "修改控制器資料失敗。";
                }
                #endregion

            }
            #endregion
            objRet.act = "UpdateController";
            return objRet;
        }
        #endregion

        #region DeleteController
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object DeleteController(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            int iRet = 0;
            List<string> liSqlPara = new List<string>();

            #region 刪除控制器資料
            if (objRet.result)
            {
                oAcsDB.BeginTransaction();

                #region 刪除控制器所屬的所有設備相關資料

                #region 刪除 B01_EquParaData 裡面的資料(控制器所屬全部)
                if (iRet > -1)
                {
                    sql = @" 
                        DELETE FROM [B01_EquParaData] 
                        WHERE EquID IN 
                        (
                            SELECT EquID FROM B01_EquData WHERE EquNo IN
	                        (
		                        SELECT EquNo FROM B01_Reader WHERE CtrlID = ?   
	                        )
                        )";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[11].Trim());
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #region 刪除 B01_EquGroupData 裡面的資料(控制器所屬全部)
                if (iRet > -1)
                {
                    sql = @" 
                        DELETE FROM [B01_EquGroupData] 
                        WHERE EquID IN 
	                    (
                            SELECT EquID FROM B01_EquData WHERE EquNo IN
	                        (
		                        SELECT EquNo FROM B01_Reader WHERE CtrlID = ?   
	                        )
                        )";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[11].Trim());
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #region 刪除 B01_EquDataExt 裡面的資料(控制器所屬全部)
                if (iRet > -1)
                {
                    sql = @" 
                        DELETE FROM B01_EquDataExt 
                        WHERE EquID IN 
	                    (
                            SELECT EquID FROM B01_EquData WHERE EquNo IN
	                        (
		                        SELECT EquNo FROM B01_Reader WHERE CtrlID = ?   
	                        )
                        )";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[11].Trim());
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #region 刪除 B01_ElevatorFloor 裡面的資料(控制器所屬全部)
                if (iRet > -1)
                {
                    sql = @" 
                        DELETE FROM B01_ElevatorFloor 
                        WHERE EquID IN 
                        (
                            SELECT EquID FROM B01_EquData WHERE EquNo IN
	                        (
		                        SELECT EquNo FROM B01_Reader WHERE CtrlID = ?   
	                        )
                        )";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[11].Trim());
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #region 刪除 B01_CardAuth 裡面的資料(控制器所屬全部)
                if (iRet > -1)
                {
                    sql = @" 
                        DELETE FROM B01_CardAuth 
                        WHERE EquID IN 
                        (
                            SELECT EquID FROM B01_EquData WHERE EquNo IN
	                        (
		                        SELECT EquNo FROM B01_Reader WHERE CtrlID = ?   
	                        )
                        )";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[11].Trim());
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #region 刪除 B01_CardEquAdj 裡面的資料(控制器所屬全部)
                if (iRet > -1)
                {
                    sql = @" 
                        DELETE FROM B01_CardEquAdj 
                        WHERE EquID IN 
                        (
                            SELECT EquID FROM B01_EquData WHERE EquNo IN
	                        (
		                        SELECT EquNo FROM B01_Reader WHERE CtrlID = ?   
	                        )
                        )";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[11].Trim());
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #region 刪除 B01_EquData 裡面的資料(控制器所屬全部)
                if (iRet > -1)
                {
                    sql = @" 
                        DELETE FROM [B01_EquData] 
                        WHERE EquNo IN 
	                    (
                            SELECT EquNo FROM B01_Reader WHERE CtrlID = ?  
                        )";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[11].Trim());
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #region 刪除 B01_Reader 裡面的資料(控制器所屬全部)
                if (iRet > -1)
                {
                    sql = @" DELETE FROM B01_Reader WHERE CtrlID = ? ";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[11].Trim());
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #endregion

                #region 刪除控制器
                if (iRet > -1)
                {
                    sql = @" DELETE FROM B01_Controller WHERE CtrlID = ? ";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[11].Trim());
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                if (iRet > -1)
                {
                    oAcsDB.Commit();

                    #region 寫入B00_SysLog
                    oAcsDB.WriteLog(DB_Acs.Logtype.資料刪除, sUserID, sUserName, "020102",
                        drTemp.DataReader.GetValue(0).ToString(),
                        drTemp.DataReader.GetValue(1).ToString(),
                        string.Format("控制器編號：{0}", DataArray[0]), "刪除控制器");
                    #endregion

                    objRet.message = DataArray[11].Trim();
                }
                else
                {
                    oAcsDB.Rollback();

                    objRet.message = "刪除控制器及其所屬的讀卡機、設備失敗。";
                    objRet.result = false;
                }

            }
            #endregion
            objRet.act = "DeleteController";
            return objRet;
        }
        #endregion

        #region Check_Input_DB_Controller
        protected static Pub.MessageObject Check_Input_DB_Controller(string[] DataArray, string ClickItem, string Mode, ref DB_Acs oAcsDB)
        {
            //DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
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

                // 訊息：控制器編號 必須輸入
                objRet.message += string_Controller[0];
            }
            else if (DataArray[0].Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：控制器編號 字數超過上限
                objRet.message += string_Controller[1];
            }

            if (DataArray[1].Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：控制器名稱 字數超過上限
                objRet.message += string_Controller[2];
            }

            if (DataArray[2].Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：控制器說明 字數超過上限
                objRet.message += string_Controller[3];
            }

            //if (string.IsNullOrEmpty(DataArray[8].Trim()) && Mode == "Insert")
            //{
            //    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
            //    objRet.result = false;

            //    // 訊息：連線裝罝 必須指定
            //    objRet.message += string_Controller[4];
            //}

            if (string.IsNullOrEmpty(DataArray[4].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：機型 必須指定
                objRet.message += string_Controller[5];
            }

            if (string.IsNullOrEmpty(DataArray[5].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：控制器狀態 必須指定
                objRet.message += string_Controller[6];
            }

            if (string.IsNullOrEmpty(DataArray[6].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：機號 必須指定
                objRet.message += string_Controller[7];
            }
            else if (!int.TryParse(DataArray[6].Trim(), out tempint))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：機號 必須為數值
                objRet.message += string_Controller[8];
            }


            //else if (DataArray[6].Trim().Count() > 3)
            //{
            //    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
            //    objRet.result = false;

            //    // 訊息：機號 字數超過上限
            //    objRet.message += string_Controller[9];
            //}

            // wei 20170207 
            // 如果機型是NanoEye(虹膜設定)，字數上限則為4碼，一般則為3碼。
            if (DataArray[4].Trim().Equals("NanoEye"))
            {
                if (DataArray[6].Trim().Count() > 4)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：機號 字數超過上限
                    objRet.message += string_Controller[9];
                }
            }
            else
            {
                if (DataArray[6].Trim().Count() > 3)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：機號 字數超過上限
                    objRet.message += string_Controller[9];
                }

                int.TryParse(DataArray[6].Trim(), out tempint);
                if (tempint < 1 || tempint > 255)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：機號 需介於1與255之間
                    objRet.message += string_Controller[10];
                }
            }

            if (string.IsNullOrEmpty(DataArray[13].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "區域 必須指定";
            }

            if (string.IsNullOrEmpty(DataArray[14].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                objRet.message += "棟別 必須指定";
            }


            #endregion

            #region DB
            if (objRet.result)
            {
                #region Check No
                switch (Mode)
                {
                    case "Insert":
                        sql = @" SELECT * FROM B01_Controller WHERE CtrlNo = ? ";
                        liSqlPara.Add("S:" + DataArray[0].Trim());
                        break;
                    case "Update":
                        sql = @" SELECT * FROM B01_Controller WHERE CtrlNo = ? AND CtrlID != ? ";
                        liSqlPara.Add("S:" + DataArray[0].Trim());
                        liSqlPara.Add("I:" + ClickItem.Trim());
                        break;
                }

                oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                if (dr.Read())
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;
                    objRet.message += "此控制器編號已存在於系統中";
                }
                #endregion

                sql = "";
                liSqlPara.Clear();

                #region Check Add
                switch (Mode)
                {
                    case "Insert":
                        sql = @" SELECT * FROM B01_Controller WHERE CtrlAddr = ? AND MstID = ?";
                        liSqlPara.Add("I:" + DataArray[6].Trim());
                        liSqlPara.Add("S:" + DataArray[8].Trim());
                        break;
                    case "Update":
                        sql = @" SELECT * FROM B01_Controller WHERE CtrlAddr = ? AND MstID = ? AND CtrlID != ? ";
                        liSqlPara.Add("I:" + DataArray[6].Trim());
                        liSqlPara.Add("S:" + DataArray[8].Trim());
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

        #region Reader 相關

        #region GetCtrlInfo
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] GetCtrlInfo(string FromCtrlID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            DBReader dr = new DBReader();

            sql = @" 
                SELECT TOP 1 
	                CR.CtrlID, CR.CtrlName, CR.CtrlModel, MR.DciID, 
                    (SELECT TOP 1 [ParaValue] FROM [B00_SysParameter] WHERE ParaNo='CardDefaultLength') CardNoLen,
                    (
                        SELECT TOP 1 ED1.EquClass FROM [B01_EquData] ED1 
                        LEFT JOIN B01_READER RD1 ON RD1.EquNo = ED1.EquNo 
                        LEFT JOIN B01_Controller CR1 ON CR1.CtrlID = RD1.CtrlID 
                        WHERE CR1.CtrlID = CR.CtrlID 
                    ) AS EquClass,
                    (SELECT (COUNT(*) + 1) FROM B01_READER WHERE CtrlID = ?) ReaderCount, 
                    CR.CtrlNo 
                FROM B01_Controller CR 
                LEFT JOIN [B01_Master] MR ON MR.MstID = CR.MstID 
                WHERE CR.CtrlID = ? ";

            liSqlPara.Clear();
            liSqlPara.Add("I:" + FromCtrlID.Trim());
            liSqlPara.Add("I:" + FromCtrlID.Trim());
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

        #region LoadReader
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadReader(string UserID, string ReaderID)
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
                SELECT 
                    RD.ReaderNo, RD.ReaderName, RD.ReaderDesc, RD.EquNo AS RdEquNo, RD.Dir,
                    CR.CtrlName, CR.CtrlModel, CR.CtrlID, 
                    ED.EquModel,
                    ED.EquNo, 
                    CASE 
                        WHEN ED.EquClass = 'Door Access' THEN '門禁設備'
                        WHEN ED.EquClass = 'TRT' THEN '考勤設備'
                        WHEN ED.EquClass = 'Elevator' THEN '電梯設備'
                        ELSE '' 
                    END AS ChtEquClass, 
                    ED.EquClass,
                    ED.Building, 
                    ED.[Floor],
                    ED.EquName,
                    ED.EquEName,
                    ED.DciID, 
                    ED.CardNoLen, 
	                ED.InToCtrlAreaID, 
	                ED.OutToCtrlAreaID, 
                    ED.IsAndTrt, 
                    ED.EquID, 
                    ED.IsShowName, 
                    RD.ReaderID,ED.EquNoList,
                    ED.Area
                FROM B01_Reader AS RD
                LEFT JOIN B01_Controller AS CR ON CR.CtrlID IS NOT NULL AND CR.CtrlID = RD.CtrlID
                LEFT JOIN B01_EquData AS ED ON ED.EquNo IS NOT NULL AND ED.EquNo = RD.EquNo 
                WHERE RD.ReaderID = @ReaderID ";

            //liSqlPara.Add("I:" + ReaderID.Trim());
            //oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            var result = odo.GetQueryResult(sql, new { ReaderID = ReaderID });
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            DataTable dt = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(DataTable));
            if (dt != null && dt.Rows.Count > 0)
            {
                EditData = new string[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                    EditData[i] = dt.Rows[0][i].ToString();

                dtTemp.Dispose();
                dtTemp = dt;

                // wei 20170327 補足缺的設備參數 hideUserID.value、EquID (index=21)、EquModel (index=8)
                AddParaMeterForReader(UserID, EditData[21], EditData[8]);
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

        #region InsertReader
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object InsertReader(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "", sql1 = "", sql2 = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            List<string> liSqlPara1 = new List<string>();
            List<string> liSqlPara2 = new List<string>();

            // 驗證資料
            objRet = Check_Input_DB_Reader(DataArray, "Insert");

            #region 新增讀卡機設備資料

            if (objRet.result)
            {
                #region 新增讀卡機
                sql1 = @" 
                    INSERT INTO B01_Reader 
                    (
                        ReaderNo, ReaderName, ReaderDesc, CtrlID, EquNo, Dir, 
                        CreateUserID, CreateTime, UpdateUserID, UpdateTime 
                    ) 
                    VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?) ";

                liSqlPara1.Add("I:" + DataArray[0].Trim());
                liSqlPara1.Add("S:" + DataArray[1].Trim());
                liSqlPara1.Add("S:" + DataArray[2].Trim());
                liSqlPara1.Add("I:" + DataArray[5].Trim());
                liSqlPara1.Add("S:" + DataArray[3].Trim());          // EquNo
                liSqlPara1.Add("S:" + DataArray[4].Trim());
                liSqlPara1.Add("S:" + UserID.ToString());
                liSqlPara1.Add("D:" + Time.ToString());
                liSqlPara1.Add("S:" + UserID.ToString());
                liSqlPara1.Add("D:" + Time.ToString());
                #endregion

                #region 新增設備

                string sLocID = DataArray[13].Trim();
                if (string.IsNullOrEmpty(sLocID)) sLocID = DataArray[12].Trim();
                if (string.IsNullOrEmpty(sLocID)) sLocID = DataArray[25].Trim();

                sql2 = @" 
                    INSERT INTO B01_EquData 
                    (
                        EquClass,
                        EquModel,
                        EquNo,
                        Building,
                        [Floor],
                        EquName,
                        EquEName,
                        DciID,
                        CardNoLen,
                        InToCtrlAreaID,
                        OutToCtrlAreaID,
                        IsAndTrt,  
                        CreateUserID,CreateTime,UpdateUserID,UpdateTime,
                        Area, LocID 
                    ) 
                    VALUES  
                    (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?) ";

                liSqlPara2.Add("S:" + DataArray[7].Trim());      // EquClass
                liSqlPara2.Add("S:" + DataArray[8].Trim());      // EquModel
                liSqlPara2.Add("S:" + DataArray[9].Trim());      // EquNo
                liSqlPara2.Add("S:" + DataArray[12].Trim());     // Building
                liSqlPara2.Add("S:" + DataArray[13].Trim());     // Floor
                liSqlPara2.Add("S:" + DataArray[14].Trim());     // EquName
                liSqlPara2.Add("S:" + DataArray[15].Trim());     // EquEName
                liSqlPara2.Add("I:" + DataArray[16].Trim());     // DciID
                liSqlPara2.Add("I:" + DataArray[17].Trim());     // CardNoLen 
                liSqlPara2.Add("I:" + DataArray[18].Trim());     // InToCtrlAreaID
                liSqlPara2.Add("I:" + DataArray[19].Trim());     // OutToCtrlAreaID
                liSqlPara2.Add("I:" + DataArray[20].Trim());     // IsAndTrt
                liSqlPara2.Add("S:" + UserID.ToString());
                liSqlPara2.Add("D:" + Time.ToString());
                liSqlPara2.Add("S:" + UserID.ToString());
                liSqlPara2.Add("D:" + Time.ToString());
                liSqlPara2.Add("S:" + DataArray[25].Trim());     // Area
                liSqlPara2.Add("S:" + sLocID);     // LocID
                #endregion

                #region 開始交易
                oAcsDB.BeginTransaction();

                int intResult1 = oAcsDB.SqlCommandExecute(sql1, liSqlPara1);
                int intResult2 = oAcsDB.SqlCommandExecute(sql2, liSqlPara2);

                if (intResult1 != -1 && intResult2 != -1)
                {
                    oAcsDB.Commit();

                    #region 寫入B00_SysLog
                    #region 新增讀卡機
                    int[] intAry = new int[] { 0, 1, 2, 4 };
                    string[] strAry = new string[] { "ReaderNo", "ReaderName", "ReaderDesc", "Dir" };

                    List<Machine> liData = new List<Machine>();
                    int r = 0;
                    foreach (int g in intAry)
                    {
                        //string strOldValue = drTemp.DataReader.GetValue(g).ToString();
                        string strNewValue = DataArray[g].Trim();

                        liData.Add(new Machine(strAry[r], strReader[r], "", strNewValue));
                        r++;
                    }

                    oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, sUserID, sUserName, "020102",
                        DataArray[0].Trim(),
                        DataArray[1].Trim(),
                        string.Format("{0}", CompareVaule("Insert", liData)),
                        "新增讀卡機");
                    #endregion

                    #region 新增設備
                    intAry = new int[] { 7, 8, 9, 12, 13, 14, 15, 17, 18, 19, 20 };
                    strAry = new string[] { "EquClass", "EquModel", "EquNo", "Building", "Floor", "EquName", "EquEName", "CardNoLen", "InToCtrlAreaID", "OutToCtrlAreaID", "IsAndTrt" };

                    liData = new List<Machine>();
                    r = 0;
                    foreach (int g1 in intAry)
                    {
                        //string strOldValue = drTemp.DataReader.GetValue(g).ToString();
                        string strNewValue = DataArray[g1].Trim();

                        liData.Add(new Machine(strAry[r], strEqu[r], "", strNewValue));
                        r++;
                    }

                    oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, sUserID, sUserName, "020102",
                        DataArray[9].Trim(),
                        DataArray[14].Trim(),
                        string.Format("{0}", CompareVaule("Insert", liData)),
                        "新增設備");
                    #endregion
                    #endregion

                    #region 取得剛剛新增的ReaderNo的ReaderID回傳使用
                    sql = @" 
                        SELECT TOP 1 ReaderID FROM B01_Reader 
                        WHERE ReaderNo = ? AND EquNo = ? AND CtrlID = ? ";

                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[0].Trim());
                    liSqlPara.Add("A:" + DataArray[3].Trim());
                    liSqlPara.Add("I:" + DataArray[5].Trim());
                    objRet.message = oAcsDB.GetStrScalar(sql, liSqlPara);
                    #endregion

                    #region 新增設備(EquID)到全群組(EG999)
                    // 取得剛剛新增設備的EquID
                    string strEquNo = DataArray[9].Trim();   // EquNo
                    string strEquID
                        = oAcsDB.GetStrScalar(string.Format("SELECT TOP 1 EquID FROM B01_EquData WHERE EquNo='{0}'", strEquNo));

                    AddEquIdToEquGroupData(strEquID);
                    #endregion

                    #region 新增該設備的設備參數預設值
                    string strEquModel = DataArray[8].Trim();   // EquModel
                    AddDefaultEquParaData(UserID, strEquID, strEquModel);
                    #endregion
                }
                else
                {
                    oAcsDB.Rollback();

                    objRet.result = false;
                    objRet.message = "新增讀卡機和設備失敗。";
                }
                #endregion

                #region 同步控制器參數
                // 僅用於控制器參數的部份
                // CtrlNo所屬的讀卡機 & 設備若有多筆，取最舊那筆，
                // 將其參數值複製給其他所屬的讀卡機，使其他讀卡機無需重送
                if (!string.IsNullOrEmpty(DataArray[5].Trim()))
                {
                    CopyParaToOtherReader(DataArray[5].Trim());
                }
                #endregion
            }

            #endregion

            objRet.act = "InsertReader";
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

        #region UpdateReader
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object UpdateReader(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql1 = "", sql2 = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara1 = new List<string>();
            List<string> liSqlPara2 = new List<string>();

            // Check_Input_DB_Reader 同時處理 READER 和 EQUIPMENT 的資料例外狀況
            objRet = Check_Input_DB_Reader(DataArray, "Update");

            #region 編輯讀卡機和設備資料

            if (objRet.result)
            {
                #region 修改讀卡機資料
                sql1 = @" 
                    UPDATE B01_Reader SET 
                        ReaderNo = ?, 
                        ReaderName = ?, 
                        ReaderDesc = ?,
                        EquNo = ?, 
                        Dir = ?,
                        UpdateUserID = ?, 
                        UpdateTime = ? 
                    WHERE ReaderID = ? ";
                liSqlPara1.Add("I:" + DataArray[0].Trim());
                liSqlPara1.Add("S:" + DataArray[1].Trim());
                liSqlPara1.Add("S:" + DataArray[2].Trim());
                liSqlPara1.Add("S:" + DataArray[3].Trim());
                liSqlPara1.Add("S:" + DataArray[4].Trim());
                liSqlPara1.Add("S:" + UserID.ToString());
                liSqlPara1.Add("D:" + Time);
                liSqlPara1.Add("I:" + DataArray[23].Trim());

                oAcsDB.SqlCommandExecute(sql1, liSqlPara1);
                #endregion

                #region 修改設備資料

                string sLocID = DataArray[13].Trim();
                if (string.IsNullOrEmpty(sLocID)) DataArray[12].Trim();
                if (string.IsNullOrEmpty(sLocID)) DataArray[25].Trim();

                sql2 = @" 
                    UPDATE B01_EquData SET 
                        EquModel = ?,                        
                        EquNo = ?,
                        EquClass = ?,
                        Building = ?, 
                        Floor = ?, 
                        EquName = ?, 
                        EquEName = ?,
                        DciID = ?,
                        CardNoLen = ?,
                        InToCtrlAreaID = ?, 
                        OutToCtrlAreaID = ?,
                        IsAndTrt = ?, 
                        IsShowName=?,
                        UpdateUserID = ?, 
                        UpdateTime = GETDATE(),
                        EquNoList = ? , 
                        Area = ? ,
                        LocID = ?
                    WHERE EquID = ? ";

                liSqlPara2.Add("S:" + DataArray[8].Trim());      // EquModel
                liSqlPara2.Add("S:" + DataArray[9].Trim());      // EquNo
                liSqlPara2.Add("S:" + DataArray[11].Trim());     // EquClass
                liSqlPara2.Add("S:" + DataArray[12].Trim());     // Building
                liSqlPara2.Add("S:" + DataArray[13].Trim());     // Floor
                liSqlPara2.Add("S:" + DataArray[14].Trim());     // EquName
                liSqlPara2.Add("S:" + DataArray[15].Trim());     // EquEName
                liSqlPara2.Add("I:" + DataArray[16].Trim());     // DciID
                liSqlPara2.Add("I:" + DataArray[17].Trim());     // CardNoLen 
                liSqlPara2.Add("I:" + DataArray[18].Trim());     // InToCtrlAreaID
                liSqlPara2.Add("I:" + DataArray[19].Trim());     // OutToCtrlAreaID
                liSqlPara2.Add("I:" + DataArray[20].Trim());     // IsAndTrt
                liSqlPara2.Add("I:" + DataArray[22].Trim());     // IsShowName
                liSqlPara2.Add("S:" + UserID.ToString());
                liSqlPara2.Add("S:" + DataArray[24]);           //EquNoList
                liSqlPara2.Add("S:" + DataArray[25].Trim());           //Area
                liSqlPara2.Add("S:" + sLocID);                  //LocID  
                liSqlPara2.Add("I:" + DataArray[21].Trim());     // EquID


                #endregion

                #region 開始交易
                oAcsDB.BeginTransaction();

                int intResult1 = oAcsDB.SqlCommandExecute(sql1, liSqlPara1);
                int intResult2 = oAcsDB.SqlCommandExecute(sql2, liSqlPara2);

                if (intResult1 > -1 && intResult2 > -1)
                {
                    oAcsDB.Commit();

                    #region 寫入B00_SysLog
                    #region 新增讀卡機
                    int[] intAry = new int[] { 0, 1, 2, 4 };
                    string[] strAry = new string[] { "ReaderNo", "ReaderName", "ReaderDesc", "Dir" };

                    List<Machine> liData = new List<Machine>();
                    int r = 0;
                    foreach (int g in intAry)
                    {
                        //string strOldValue = drTemp.DataReader.GetValue(g).ToString();
                        string strOldValue = dtTemp.Rows[0][g].ToString();
                        string strNewValue = DataArray[g].Trim();

                        liData.Add(new Machine(strAry[r], strReader[r], strOldValue, strNewValue));
                        r++;
                    }

                    oAcsDB.WriteLog(DB_Acs.Logtype.資料修改, sUserID, sUserName, "020102",
                        dtTemp.Rows[0][0].ToString(),
                        dtTemp.Rows[0][1].ToString(),
                        string.Format("{0}", CompareVaule("Update", liData)),
                        "修改讀卡機");
                    #endregion

                    #region 新增設備
                    intAry = new int[] { 7, 8, 9, 12, 13, 14, 15, 17, 18, 19, 20 };
                    strAry = new string[] { "EquClass", "EquModel", "EquNo", "Building", "Floor", "EquName", "EquEName", "CardNoLen", "InToCtrlAreaID", "OutToCtrlAreaID", "IsAndTrt" };

                    liData = new List<Machine>();
                    r = 0;
                    foreach (int g1 in intAry)
                    {
                        string strOldValue = dtTemp.Rows[0][g1].ToString();
                        string strNewValue = DataArray[g1].Trim();

                        liData.Add(new Machine(strAry[r], strEqu[r], strOldValue, strNewValue));
                        r++;
                    }

                    oAcsDB.WriteLog(DB_Acs.Logtype.資料修改, sUserID, sUserName, "020102",
                        dtTemp.Rows[0][9].ToString(),
                        dtTemp.Rows[0][14].ToString(),
                        string.Format("{0}", CompareVaule("Update", liData)),
                        "修改設備");
                    #endregion
                    #endregion

                    objRet.message = DataArray[23].Trim();
                }
                else
                {
                    oAcsDB.Rollback();

                    objRet.result = false;
                    objRet.message = "修改讀卡機和設備資料失敗。";
                }
                #endregion
            }
            #endregion

            objRet.act = "UpdateReader";
            //Object oo = SendAppCmdStrList("");
            return objRet;
        }
        #endregion

        #region DeleteReader
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object DeleteReader(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            int intResult = 0;

            #region 刪除讀卡機和設備資料

            #region 如果這個控制器只有一個讀卡機，則不可以刪除，除非是連控制器一起刪除

            sql = @" SELECT COUNT([ReaderID]) FROM [B01_Reader] WHERE CtrlID = ? ";
            liSqlPara.Clear();
            liSqlPara.Add("S:" + DataArray[5]);

            int intRt = oAcsDB.GetIntScalar(sql, liSqlPara);

            if (intRt < 2)
            {
                objRet.result = false;
                objRet.message = "一個控制器至少需要搭配一個讀卡機和設備，故無法刪除。若真要刪除，請使用控制器刪除功能連同控制器一起刪除。";
            }

            #endregion

            if (objRet.result)
            {
                oAcsDB.BeginTransaction();

                #region 刪除設備相關資料

                #region 刪除 B01_EquGroupData 裡面的資料
                if (intResult > -1)
                {
                    sql = @" DELETE FROM B01_EquGroupData WHERE EquID = ? ";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[21].Trim());
                    intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #region 刪除 B01_EquDataExt 裡面的資料
                if (intResult > -1)
                {
                    sql = @" DELETE FROM B01_EquDataExt WHERE EquID = ? ";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[21].Trim());
                    intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #region 刪除 B01_EquParaData 裡面的資料
                if (intResult > -1)
                {
                    sql = @" DELETE FROM B01_EquParaData WHERE EquID = ? ";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[21].Trim());
                    intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #region 刪除 B01_ElevatorFloor 裡面的資料
                if (intResult > -1)
                {
                    sql = @" DELETE FROM B01_ElevatorFloor WHERE EquID = ? ";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[21].Trim());
                    intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #region 刪除 B01_CardAuth 裡面的資料
                if (intResult > -1)
                {
                    sql = @" DELETE FROM B01_CardAuth WHERE EquID = ? ";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[21].Trim());
                    intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #region 刪除 B01_CardEquAdj 裡面的資料
                if (intResult > -1)
                {
                    sql = @" DELETE FROM B01_CardEquAdj WHERE EquID = ? ";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[21].Trim());
                    intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #region 刪除 B01_EquData 裡面的資料
                if (intResult > -1)
                {
                    sql = @" DELETE FROM B01_EquData WHERE EquID = ? ";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[21].Trim());
                    intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #region 刪除 B01_Reader 裡面的資料
                if (intResult > -1)
                {
                    sql = @" DELETE FROM B01_Reader WHERE ReaderID = ? ";
                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + DataArray[23].Trim());
                    intResult = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                #endregion

                if (intResult > -1)
                {
                    oAcsDB.Commit();

                    oAcsDB.WriteLog(DB_Acs.Logtype.資料刪除, sUserID, sUserName, "020102",
                        DataArray[0].Trim(),
                        DataArray[1].Trim(),
                        string.Format("讀卡機編號：{0}", DataArray[0].Trim()),
                        "刪除讀卡機");

                    oAcsDB.WriteLog(DB_Acs.Logtype.資料刪除, sUserID, sUserName, "020102",
                        DataArray[9].Trim(),
                        DataArray[14].Trim(),
                        string.Format("設備編號：{0}", DataArray[9].Trim()),
                        "刪除設備");

                    objRet.message = DataArray[23].Trim();
                }
                else
                {
                    oAcsDB.Rollback();

                    objRet.message = "刪除讀卡機和設備資料失敗。";
                    objRet.result = false;
                }
            }

            #endregion

            objRet.act = "DeleteReader";
            return objRet;
        }
        #endregion

        #region Check_Input_DB_Reader
        protected static Pub.MessageObject Check_Input_DB_Reader(string[] DataArray, string Mode)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            int tempint;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr = new Sa.DB.DBReader();

            #region Input

            #region 0. ReaderNo
            if (string.IsNullOrEmpty(DataArray[0].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：讀卡機編號 必須輸入
                objRet.message += string_Reader[0];
            }
            else if (!int.TryParse(DataArray[0].Trim(), out tempint))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：讀卡機編號 必須為數值
                objRet.message += string_Reader[1];
            }
            else if (DataArray[0].Trim().Count() > 3)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：讀卡機編號 字數超過上限
                objRet.message += string_Reader[2];
            }

            if (objRet.result)
            {
                int.TryParse(DataArray[0].Trim(), out tempint);
                if (tempint < 1 || tempint > 128)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;

                    // 訊息：讀卡機編號 需介於1與128之間
                    objRet.message += string_Reader[3];
                }
            }
            #endregion

            #region 1. ReaderName
            if (DataArray[1].Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：讀卡機名稱 字數超過上限
                objRet.message += string_Reader[4];
            }
            #endregion

            #region 2. ReaderDesc
            if (DataArray[2].Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：讀卡機說明 字數超過上限
                objRet.message += string_Reader[5];
            }
            #endregion

            #region 3. EquNo
            if (string.IsNullOrEmpty(DataArray[3].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：對應設備 必須指定
                objRet.message += string_Reader[6];
            }
            #endregion

            #region 4. Dir
            if (string.IsNullOrEmpty(DataArray[4].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：方向控制 必須指定
                objRet.message += string_Reader[7];
            }
            #endregion

            // 從 index=8 開始，就是用在 設備(Equipment) 的資料處理了

            #region  8. 設備型號 - EquModel

            if (Encoding.Default.GetByteCount(DataArray[8].Trim()) > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：設備型號 字數超過上限
                objRet.message += string_Reader[9];
            }
            #endregion

            #region  9. 設備編號 - EquNo

            if (Encoding.Default.GetByteCount(DataArray[9].Trim()) > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：設備編號 字數超過上限
                objRet.message += string_Reader[10];
            }

            #endregion

            #region 11. 設備類別 - EquClass

            if (Encoding.Default.GetByteCount(DataArray[11].Trim()) > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：設備類別 字數超過上限
                objRet.message += string_Reader[8];
            }

            #endregion

            #region 12. 建築物名稱 - Building

            if (string.IsNullOrEmpty(DataArray[12].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;
                objRet.message += string_Reader[11];
            }

            #endregion

            #region 13. 樓層 - Floor

            if (string.IsNullOrEmpty(DataArray[13].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;
                objRet.message += string_Reader[12];
            }

            #endregion

            #region 14. 設備名稱 - EquName

            if (Encoding.Default.GetByteCount(DataArray[14].Trim()) > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：設備名稱 字數超過上限
                objRet.message += string_Reader[13];
            }

            #endregion

            #region 15. 設備英文名稱 - EquEName

            if (Encoding.Default.GetByteCount(DataArray[15].Trim()) > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：設備英文名稱 字數超過上限
                objRet.message += string_Reader[14];
            }

            #endregion

            #region 16. 設備連線 - DciID

            if (Encoding.Default.GetByteCount(DataArray[16].Trim()) > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：設備連線 字數超過上限
                objRet.message += string_Reader[15];
            }

            #endregion

            #region 17. 卡號長度 - CardNoLen (0-255)

            tempint = 0;

            try
            {
                tempint = Int32.Parse(DataArray[17].Trim());
            }
            catch
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：卡號長度 不是整數
                objRet.message += string_Reader[16];
            }

            if (tempint < 0 || tempint > 255)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;

                // 訊息：卡號長度 其值不在0-255之內
                objRet.message += string_Reader[17];
            }

            #endregion

            // 5.6.7.18.19.20.21.22 目前用不到

            #region 25. 區域 - Area

            if (string.IsNullOrEmpty(DataArray[25].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;
                objRet.message += string_Reader[18];
            }

            #endregion

            #endregion

            #region DB
            if (objRet.result)
            {
                switch (Mode)
                {
                    case "Insert":
                        sql = @" SELECT * FROM B01_Reader WHERE CtrlID = ? AND ReaderNo = ? ";
                        liSqlPara.Add("S:" + DataArray[5].Trim());
                        liSqlPara.Add("S:" + DataArray[0].Trim());

                        break;
                    case "Update":
                        sql = @" SELECT * FROM B01_Reader WHERE CtrlID = ? AND ReaderNo = ? AND ReaderID != ? ";

                        liSqlPara.Add("S:" + DataArray[5].Trim());
                        liSqlPara.Add("S:" + DataArray[0].Trim());
                        liSqlPara.Add("S:" + DataArray[23].Trim());
                        break;
                }

                dr = new DBReader();
                oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                if (dr.HasRows)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;
                    objRet.message += "此讀卡機編號已存在於系統中。";
                }

            }

            // 從這邊開始，就是用在 設備(Equipment) 的資料處理了
            #region 設備方面的資料處理

            if (objRet.result)
            {
                switch (Mode)
                {
                    case "Insert":
                        // 更新時，須注意要更新的 EquNo 是否存在資料表
                        sql = @" SELECT 1 FROM B01_EquData WHERE EquNo = ? ";

                        liSqlPara.Clear();
                        liSqlPara.Add("S:" + DataArray[9].Trim());

                        break;
                    case "Update":
                        sql = @" SELECT 1 FROM B01_EquData WHERE EquNo = ? AND EquID != ? ";
                        liSqlPara.Clear();
                        liSqlPara.Add("S:" + DataArray[9].Trim());
                        liSqlPara.Add("S:" + DataArray[21].Trim());
                        break;
                }

                dr = new DBReader();

                oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                if (dr.HasRows)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                    objRet.result = false;
                    objRet.message += "此設備編號已存在於系統中。";
                }
            }

            #endregion

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
            // 從控制器編號(Controller_Input_No)，得到其中的READER，再依此得到相對應的設備，擇一發送則可，若無READER則不做
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
            liSqlPara.Add("S:" + Controller_Input_No.Text);
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

        protected void ddl_Building_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlLocationFloor.Items.Clear();
            if (locInfos != null)
            {
                var result = locInfos.Where(i => i.LocType == "FLOOR" && i.LocPID == int.Parse(ddlLocationArea.SelectedItem.Value)); 
                if (result.Count() > 0)
                {
                    foreach (var r in result)
                    {
                        ListItem Item = new ListItem();
                        Item.Text = " [" + r.LocNo.ToString() + "]" + r.LocName;
                        Item.Value = r.LocID.ToString();
                        this.ddlLocationFloor.Items.Add(Item);
                    }
                }
            }
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
    }

}