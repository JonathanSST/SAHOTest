using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SahoAcs.DB.DBInfo;

namespace SahoAcs
{
    public partial class EquMapSetting2 : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 20;
        Hashtable TableInfo;
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
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

            RegisterTreeViewJS();
            ClientScript.RegisterClientScriptInclude("EquMapSetting", "EquMapSetting2.js");//加入同一頁面所需的JavaScript檔案

            #region 設定彈跳視窗
            Pub.SetModalPopup(ModalPopupExtender1, 1);
            #endregion

            #region 註冊主頁Button動作
            Master_Input_Type_TCPIP.Attributes.Add("Onclick", "SetParamDiv('IPParam');");
            Master_Input_Type_COMPort.Attributes.Add("Onclick", "SetParamDiv('ComPortParam');");
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
            btnFilter.Attributes["onClick"] = "CallSearch('" +
                this.GetLocalResourceObject("CallSearch_Title") + "'); return false;";
            this.btnAddEquData.Attributes["onClick"] = "AddEquData(); return false;";
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

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion

                LoadEquipment("");
                Master_CreateCtrlModel();

            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == this.TreeView_UpdatePanel.ClientID)
                {
                    if (sFormArg == "Refalsh")
                    {
                        txt_NodeTypeList.Value = "";
                        txt_NodeIDList.Value = "";
                        LoadEquipment("");
                        //TreeView_UpdatePanel.Update();
                        EquOrg_TreeView.ExpandAll();
                    }
                }
            }
        }
        #endregion

        #endregion

        #region Method

        #region LoadEquipment
        private void LoadEquipment(string strFilter)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            EquOrg_TreeView.Nodes.Clear();

            EquOrg_TreeView.Attributes.Add("Onclick", "OpenWin();");

            TreeNode RootNode = new TreeNode();
            RootNode.Text = "門禁管理系統";
            txt_NodeTypeList.Value += "SMS,";
            txt_NodeIDList.Value += "0,";
            RootNode.SelectAction = TreeNodeSelectAction.None;

            List<DciInfo> DciTree = new List<DciInfo>();

            #region Process String
            sql = " SELECT * FROM B01_DeviceConnInfo ";
            #endregion

            oAcsDB.GetDataTable("DciTable", sql, liSqlPara, out dt);

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
            TreeView_UpdatePanel.Update();
        }
        #endregion

        #region LoadDCiTree
        private void LoadDCiTree(TreeNode PNode, List<DciInfo> objDciTree)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
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
                SubNode.Text += "[" + objDciTree[i].DciNo + "] 連線 - (" + objDciTree[i].DciName + ")";
                txt_NodeTypeList.Value += "DCI,";
                txt_NodeIDList.Value += objDciTree[i].DciID.ToString() + ",";
                SubNode.NavigateUrl = "#";
                #region Process String
                sql = " SELECT * FROM B01_Master WHERE DciID = ? ";
                #endregion
                liSqlPara.Add("S:" + objDciTree[i].DciID);

                oAcsDB.GetDataTable("MasterTable", sql, liSqlPara, out dt);

                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    MasterInfo MasterNode = new MasterInfo();
                    MasterNode.MstID = int.Parse(dt.Rows[k]["MstID"].ToString());
                    MasterNode.MstNo = dt.Rows[k]["MstNo"].ToString();
                    MasterNode.MstDesc = dt.Rows[k]["MstDesc"].ToString();
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
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
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

                SubNode.Text += "[" + objMasterTree[i].MstNo + "] 連線裝置 - (" + objMasterTree[i].MstDesc + ")";
                txt_NodeTypeList.Value += "MASTER,";
                txt_NodeIDList.Value += objMasterTree[i].MstID.ToString() + ",";
                SubNode.NavigateUrl = "#";
                #region Process String
                sql = " SELECT * FROM B01_Controller WHERE MstID = ? ";
                #endregion
                liSqlPara.Add("S:" + objMasterTree[i].MstID);

                oAcsDB.GetDataTable("ControllerTable", sql, liSqlPara, out dt);

                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    ControllerInfo ControllerNode = new ControllerInfo();
                    ControllerNode.CtrlID = int.Parse(dt.Rows[k]["CtrlID"].ToString());
                    ControllerNode.CtrlNo = dt.Rows[k]["CtrlNo"].ToString();
                    ControllerNode.CtrlName = dt.Rows[k]["CtrlName"].ToString();
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
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            for (int i = 0; i < objControllerTree.Count; i++)
            {
                sql = "";
                liSqlPara.Clear();
                TreeNode SubNode = new TreeNode();
                SubNode.Text = "[" + objControllerTree[i].CtrlNo + "] " + objControllerTree[i].CtrlName;
                txt_NodeTypeList.Value += "CONTROLLER,";
                txt_NodeIDList.Value += objControllerTree[i].CtrlID.ToString() + ",";
                SubNode.NavigateUrl = "#";

                #region Process String
                sql = " SELECT * FROM B01_Reader WHERE CtrlID = ? ";
                #endregion

                liSqlPara.Add("S:" + objControllerTree[i].CtrlID);

                oAcsDB.GetDataTable("ReaderTable", sql, liSqlPara, out dt);

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

        #region Master 相關

        #region Master_CreateCtrlModel
        private void Master_CreateCtrlModel()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt;

            this.Master_Input_CtrlModel.Items.Clear();

            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = "- 請選擇 -";
            Item.Value = "";
            this.Master_Input_CtrlModel.Items.Add(Item);

            #region Process String
            sql = @" SELECT * FROM B00_ItemList WHERE ItemClass = 'EquModel' ";
            //liSqlPara.Add("S:" + SelectValue);
            #endregion

            oAcsDB.GetDataTable("EquNoTable", sql, liSqlPara, out dt);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new System.Web.UI.WebControls.ListItem();
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
            Sa.DB.DBReader dr;

            #region Process String
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
                EditData[1] = "系統中無此資料！";
            }
            #endregion

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
            Sa.DB.DBReader dr;

            #region Process String
            sql = @" SELECT 
                     Mst.MstNo, Mst.MstDesc, Dci.DciName, 
                     Mst.MstType, Mst.MstConnParam, 
                     Mst.MstModel, Mst.MstStatus, Mst.CtrlModel,
                     Mst.LinkMode, Mst.AutoReturn, Dci.DciID
                     FROM B01_Master AS Mst
                     LEFT JOIN B01_DeviceConnInfo AS Dci ON Dci.DciID = Mst.DciID
                     WHERE Mst.MstID = ? ";
            liSqlPara.Add("I:" + MstID.Trim());
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
                EditData[1] = "系統中無此資料！";
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
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            objRet = Check_Input_DB_Master(DataArray, "", "Insert");
            #region 新增設備參數定義資料
            if (objRet.result)
            {
                #region Process String
                sql = @" INSERT INTO B01_Master (MstNo, MstDesc, MstType, MstConnParam, CtrlModel, LinkMode, AutoReturn, MstModel, MstStatus, DciID, CreateUserID, CreateTime, UpdateUserID, UpdateTime)
                         VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?) ";
                liSqlPara.Add("S:" + DataArray[0].Trim());
                liSqlPara.Add("S:" + DataArray[1].Trim());
                liSqlPara.Add("S:" + DataArray[3].Trim());
                liSqlPara.Add("S:" + DataArray[4].Trim());
                liSqlPara.Add("S:" + DataArray[7].Trim());
                liSqlPara.Add("I:" + DataArray[8].Trim());
                liSqlPara.Add("I:" + DataArray[9].Trim());
                liSqlPara.Add("S:" + DataArray[5].Trim());
                liSqlPara.Add("I:" + DataArray[6].Trim());
                liSqlPara.Add("I:" + DataArray[2].Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                #endregion
                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            #endregion
            objRet.act = "InsertMaster";
            return objRet;
        }
        #endregion

        #region UpdateMaster
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object UpdateMaster(string UserID, string ClickItem, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            objRet = Check_Input_DB_Master(DataArray, ClickItem, "Update");
            #region 新增設備參數定義資料
            if (objRet.result)
            {
                #region Process String
                sql = @" UPDATE B01_Master
                         SET MstNo = ?, MstDesc = ?, MstType = ?, 
                         MstConnParam = ?, CtrlModel = ?, 
                         LinkMode = ?, AutoReturn = ?, 
                         MstModel = ?, MstStatus = ?, 
                         UpdateUserID = ?, UpdateTime = ?
                         WHERE MstID = ? ";
                liSqlPara.Add("S:" + DataArray[0].Trim());
                liSqlPara.Add("S:" + DataArray[1].Trim());
                liSqlPara.Add("S:" + DataArray[3].Trim());
                liSqlPara.Add("S:" + DataArray[4].Trim());
                liSqlPara.Add("S:" + DataArray[7].Trim());
                liSqlPara.Add("I:" + DataArray[8].Trim());
                liSqlPara.Add("I:" + DataArray[9].Trim());
                liSqlPara.Add("S:" + DataArray[5].Trim());
                liSqlPara.Add("I:" + DataArray[6].Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("I:" + ClickItem.Trim());
                #endregion
                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            #endregion
            objRet.act = "UpdateMaster";
            return objRet;
        }
        #endregion

        #region DeleteMaster
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object DeleteMaster(string UserID, string MstID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            int iRet = 0;
            List<string> liSqlPara = new List<string>();

            oAcsDB.BeginTransaction();

            #region 新增設備參數定義資料
            if (objRet.result)
            {
                if (iRet > -1)
                {
                    #region Process String - Delete RoleRoleMenus
                    sql = @" DELETE B01_Reader 
                             WHERE ReaderID IN
                                ( SELECT Reader.ReaderID FROM B01_Reader AS Reader
                                  LEFT JOIN B01_Controller AS Controller ON Controller.CtrlID = reader.CtrlID
                                  LEFT JOIN B01_Master AS Master ON Master.MstID = Controller.MstID
                                  WHERE Master.MstID = ? ) ";
                    liSqlPara.Add("I:" + MstID);
                    #endregion
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }

                liSqlPara.Clear();

                if (iRet > -1)
                {
                    #region Process String
                    sql = @" DELETE FROM B01_Controller WHERE MstID = ? ";
                    liSqlPara.Add("I:" + MstID.Trim());
                    #endregion
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }

                liSqlPara.Clear();

                if (iRet > -1)
                {
                    #region Process String
                    sql = @" DELETE FROM B01_Master WHERE MstID = ? ";
                    liSqlPara.Add("I:" + MstID.Trim());
                    #endregion
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }

                if (iRet > -1)
                    oAcsDB.Commit();
                else
                    oAcsDB.Rollback();
            }
            #endregion
            objRet.act = "DeleteMaster";
            return objRet;
        }
        #endregion

        #region Check_Input_DB_Master
        protected static Pub.MessageObject Check_Input_DB_Master(string[] DataArray, string ClickItem, string Mode)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            int tempint;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Input
            if (string.IsNullOrEmpty(DataArray[0].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "連線裝置編號 必須輸入";
            }
            else if (DataArray[0].Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "連線裝置編號 字數超過上限";
            }

            if (DataArray[1].Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "連線裝置說明 字數超過上限";
            }

            if (string.IsNullOrEmpty(DataArray[2].Trim()) && Mode == "Insert")
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "使用連線 必須指定";
            }

            if (string.IsNullOrEmpty(DataArray[3].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "連線類型 必須指定";
            }

            #region 參數
            if (objRet.result)
            {
                string[] ParaArray = DataArray[4].Split(':');

                if (DataArray[3] == "T")
                {
                    if (string.IsNullOrEmpty(ParaArray[0].Trim()))
                    {
                        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                        objRet.result = false;
                        objRet.message += "IP 必須輸入";
                    }

                    if (string.IsNullOrEmpty(ParaArray[1].Trim()))
                    {
                        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                        objRet.result = false;
                        objRet.message += "Port 必須輸入";
                    }

                    if (objRet.result && (!CheckIP(ParaArray[0]) || !CheckPort(ParaArray[1])))
                    {
                        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                        objRet.result = false;
                        objRet.message += "IP位置不合法";
                    }
                }
                else if (DataArray[3] == "C")
                {
                    string[] Paras = ParaArray[1].Split(',');

                    if (string.IsNullOrEmpty(ParaArray[0].Trim()))
                    {
                        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                        objRet.result = false;
                        objRet.message += "ComPort 必須指定";
                    }

                    if (string.IsNullOrEmpty(Paras[0].Trim()))
                    {
                        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                        objRet.result = false;
                        objRet.message += "BaudRate 必須指定";
                    }

                    if (string.IsNullOrEmpty(Paras[1].Trim()))
                    {
                        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                        objRet.result = false;
                        objRet.message += "Parity 必須指定";
                    }

                    if (string.IsNullOrEmpty(Paras[2].Trim()))
                    {
                        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                        objRet.result = false;
                        objRet.message += "DataBits 必須指定";
                    }

                    if (string.IsNullOrEmpty(Paras[3].Trim()))
                    {
                        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                        objRet.result = false;
                        objRet.message += "StopBits 必須指定";
                    }
                }



            }
            #endregion

            if (DataArray[5].Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "連線裝置機型 字數超過上限";
            }

            if (string.IsNullOrEmpty(DataArray[6].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "狀態 必須指定";
            }

            if (string.IsNullOrEmpty(DataArray[7].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "控制器機型 必須指定";
            }

            if (string.IsNullOrEmpty(DataArray[8].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "連線模式 必須指定";
            }

            if (string.IsNullOrEmpty(DataArray[9].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "自動回傳 必須指定";
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
                        liSqlPara.Add("S:" + ClickItem.Trim());
                        break;
                }

                oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                if (dr.Read())
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "此連線裝置編號已存在於系統中";
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
            Sa.DB.DBReader dr;

            #region Process String
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
                EditData[1] = "系統中無此資料！";
            }
            #endregion

            return EditData;
        }
        #endregion

        #region LoadController
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadController(string CtrlID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Process String
            sql = @" SELECT 
                     Controller.CtrlNo, Controller.CtrlName, Controller.CtrlDesc,
                     Master.MstDesc, Controller.CtrlModel, Controller.CtrlStatus,
                     Controller.CtrlAddr, Controller.CtrlFwVer, Master.MstID
                     FROM B01_Controller AS Controller
                     LEFT JOIN B01_Master AS Master ON Master.MstID = Controller.MstID
                     WHERE Controller.CtrlID = ? ";
            liSqlPara.Add("I:" + CtrlID.Trim());
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
                EditData[1] = "系統中無此資料！";
            }
            #endregion

            return EditData;
        }
        #endregion

        #region InsertController
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object InsertController(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "", ctrlid = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            objRet = Check_Input_DB_Controller(DataArray, "", "Insert");
            #region 新增控制器資料
            if (objRet.result)
            {
                #region Process String
                sql = @" INSERT INTO B01_Controller (CtrlNo, CtrlName, CtrlDesc, CtrlAddr, CtrlModel, CtrlStatus, MstID, CreateUserID, CreateTime, UpdateUserID, UpdateTime)
                         VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?) ";
                liSqlPara.Add("S:" + DataArray[0].Trim());
                liSqlPara.Add("S:" + DataArray[1].Trim());
                liSqlPara.Add("S:" + DataArray[2].Trim());
                liSqlPara.Add("I:" + DataArray[6].Trim());
                liSqlPara.Add("S:" + DataArray[4].Trim());
                liSqlPara.Add("I:" + DataArray[5].Trim());
                liSqlPara.Add("I:" + DataArray[3].Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                #endregion
                oAcsDB.SqlCommandExecute(sql, liSqlPara);

                sql = "";
                liSqlPara.Clear();

                sql = @" SELECT MAX(CtrlID) AS CtrlID FROM B01_Controller WHERE CreateTime = ? ";
                liSqlPara.Add("D:" + Time);
                oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                if (dr.Read())
                    ctrlid = dr.DataReader["CtrlID"].ToString();

                sql = "";
                liSqlPara.Clear();

                if (int.Parse(DataArray[7]) > 0)
                {
                    for (int i = 1; i <= int.Parse(DataArray[7]); i++)
                    {
                        sql += @" INSERT INTO B01_Reader (ReaderNo,ReaderName,CtrlID,CreateUserID,CreateTime,UpdateUserID,UpdateTime)
                                  VALUES (?, ?, ?, ?, ?, ?, ?)";
                        liSqlPara.Add("I:" + i.ToString());
                        liSqlPara.Add("S:" + "讀卡機 - " + i.ToString());
                        liSqlPara.Add("I:" + ctrlid.ToString());
                        liSqlPara.Add("S:" + UserID.ToString());
                        liSqlPara.Add("D:" + Time);
                        liSqlPara.Add("S:" + UserID.ToString());
                        liSqlPara.Add("D:" + Time);
                    }
                    oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }

            }
            #endregion
            objRet.act = "InsertController";
            return objRet;
        }
        #endregion

        #region UpdateController
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object UpdateController(string UserID, string ClickItem, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            objRet = Check_Input_DB_Controller(DataArray, ClickItem, "Update");
            #region 編輯控制器資料
            if (objRet.result)
            {
                #region Process String
                sql = @" UPDATE B01_Controller
                         SET CtrlNo = ?, CtrlName = ?, 
                         CtrlDesc = ?, CtrlAddr = ?, 
                         CtrlModel = ?, CtrlStatus = ?,
                         UpdateUserID = ?, UpdateTime = ?
                         WHERE CtrlID = ? ";
                liSqlPara.Add("S:" + DataArray[0].Trim());
                liSqlPara.Add("S:" + DataArray[1].Trim());
                liSqlPara.Add("S:" + DataArray[2].Trim());
                liSqlPara.Add("I:" + DataArray[6].Trim());
                liSqlPara.Add("S:" + DataArray[4].Trim());
                liSqlPara.Add("I:" + DataArray[5].Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("I:" + ClickItem.Trim());
                #endregion
                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            #endregion
            objRet.act = "UpdateController";
            return objRet;
        }
        #endregion

        #region DeleteController
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object DeleteController(string UserID, string CtrlID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            int iRet = 0;
            List<string> liSqlPara = new List<string>();

            oAcsDB.BeginTransaction();

            #region 刪除控制器資料
            if (objRet.result)
            {
                if (iRet > -1)
                {
                    #region Process String - Delete RoleRoleMenus
                    sql = @" DELETE FROM B01_Reader WHERE CtrlID = ? ";
                    liSqlPara.Add("I:" + CtrlID.Trim());
                    #endregion
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }

                liSqlPara.Clear();

                if (iRet > -1)
                {
                    #region Process String
                    sql = @" DELETE FROM B01_Controller WHERE CtrlID = ? ";
                    liSqlPara.Add("I:" + CtrlID.Trim());
                    #endregion
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                if (iRet > -1)
                    oAcsDB.Commit();
                else
                    oAcsDB.Rollback();
            }
            #endregion
            objRet.act = "DeleteController";
            return objRet;
        }
        #endregion

        #region Check_Input_DB_Controller
        protected static Pub.MessageObject Check_Input_DB_Controller(string[] DataArray, string ClickItem, string Mode)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            int tempint;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Input
            if (string.IsNullOrEmpty(DataArray[0].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "控制器編號 必須輸入";
            }
            else if (DataArray[0].Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "控制器編號 字數超過上限";
            }

            if (DataArray[1].Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "控制器編號 字數超過上限";
            }

            if (DataArray[2].Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "控制器說明 字數超過上限";
            }

            if (string.IsNullOrEmpty(DataArray[3].Trim()) && Mode == "Insert")
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "連線裝罝 必須指定";
            }

            if (string.IsNullOrEmpty(DataArray[4].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "機型 必須指定";
            }

            if (string.IsNullOrEmpty(DataArray[5].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "控制器狀態 必須指定";
            }

            if (string.IsNullOrEmpty(DataArray[6].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "機號 必須指定";
            }
            else if (!int.TryParse(DataArray[6].Trim(), out tempint))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "機號 必須為數值";
            }
            else if (DataArray[6].Trim().Count() > 3)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "機號 字數超過上限";
            }
            if (objRet.result)
            {
                int.TryParse(DataArray[6].Trim(), out tempint);
                if (tempint < 1 || tempint > 255)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "機號 需介於1與255之間";
                }
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
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
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
                        liSqlPara.Add("S:" + DataArray[3].Trim());
                        break;
                    case "Update":
                        sql = @" SELECT * FROM B01_Controller WHERE CtrlAddr = ? AND MstID = ? AND CtrlID != ? ";
                        liSqlPara.Add("I:" + DataArray[6].Trim());
                        liSqlPara.Add("S:" + DataArray[3].Trim());
                        liSqlPara.Add("I:" + ClickItem.Trim());
                        break;
                }
                oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                if (dr.Read())
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
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
            Sa.DB.DBReader dr;

            #region Process String
            sql = @" SELECT CtrlID, CtrlName, CtrlModel FROM B01_Controller 
                     WHERE CtrlID = ? ";
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
                EditData[1] = "系統中無此資料！";
            }
            #endregion

            return EditData;
        }
        #endregion

        #region LoadReader
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadReader(string ReaderID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Process String
            sql = @" SELECT 
                     Reader.ReaderNo, Reader.ReaderName,
                     Reader.ReaderDesc, Reader.EquNo, Reader.Dir,
                     Controller.CtrlName,Controller.CtrlModel, Controller.CtrlID
                     FROM B01_Reader AS Reader
                     LEFT JOIN B01_Controller AS Controller ON Controller.CtrlID = Reader.CtrlID
                     WHERE Reader.ReaderID = ? ";
            liSqlPara.Add("I:" + ReaderID.Trim());
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
                EditData[1] = "系統中無此資料！";
            }
            #endregion

            return EditData;
        }
        #endregion

        #region InsertReader
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object InsertReader(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            objRet = Check_Input_DB_Reader(DataArray, "", "Insert");
            #region 新增讀卡機設備資料
            if (objRet.result)
            {
                #region Process String
                sql = @" INSERT INTO B01_Reader (ReaderNo, ReaderName, ReaderDesc, CtrlID, EquNo, Dir, CreateUserID, CreateTime, UpdateUserID, UpdateTime)
                         VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?) ";
                liSqlPara.Add("I:" + DataArray[0].Trim());
                liSqlPara.Add("S:" + DataArray[1].Trim());
                liSqlPara.Add("S:" + DataArray[2].Trim());
                liSqlPara.Add("I:" + DataArray[5].Trim());
                liSqlPara.Add("S:" + DataArray[3].Trim());
                liSqlPara.Add("S:" + DataArray[4].Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                #endregion
                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            #endregion
            objRet.act = "InsertReader";
            return objRet;
        }
        #endregion

        #region UpdateReader
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object UpdateReader(string UserID, string ClickItem, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            objRet = Check_Input_DB_Reader(DataArray, ClickItem, "Update");
            #region 編輯讀卡機設備資料
            if (objRet.result)
            {
                #region Process String
                sql = @" UPDATE B01_Reader
                         SET ReaderNo = ?, ReaderName = ?, ReaderDesc = ?,
                         EquNo = ?, Dir = ?,
                         UpdateUserID = ?, UpdateTime = ?
                         WHERE ReaderID = ? ";
                liSqlPara.Add("I:" + DataArray[0].Trim());
                liSqlPara.Add("S:" + DataArray[1].Trim());
                liSqlPara.Add("S:" + DataArray[2].Trim());
                liSqlPara.Add("S:" + DataArray[3].Trim());
                liSqlPara.Add("S:" + DataArray[4].Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("I:" + ClickItem.Trim());
                #endregion
                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            #endregion
            objRet.act = "UpdateReader";
            return objRet;
        }
        #endregion

        #region DeleteReader
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object DeleteReader(string UserID, string ReaderID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            #region 刪除讀卡機設備資料
            if (objRet.result)
            {
                #region Process String
                sql = @" DELETE FROM B01_Reader WHERE ReaderID = ? ";
                liSqlPara.Add("I:" + ReaderID.Trim());
                #endregion
                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            #endregion
            objRet.act = "DeleteReader";
            return objRet;
        }
        #endregion

        #region Check_Input_DB_Reader
        protected static Pub.MessageObject Check_Input_DB_Reader(string[] DataArray, string ClickItem, string Mode)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            int tempint;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Input
            if (string.IsNullOrEmpty(DataArray[0].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "讀卡機編號 必須輸入";
            }
            else if (!int.TryParse(DataArray[0].Trim(), out tempint))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "讀卡機編號 必須為數值";
            }
            else if (DataArray[0].Trim().Count() > 3)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "讀卡機編號 字數超過上限";
            }
            if (objRet.result)
            {
                int.TryParse(DataArray[0].Trim(), out tempint);
                if (tempint < 1 || tempint > 128)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "讀卡機編號 需介於1與128之間";
                }
            }

            if (DataArray[1].Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "讀卡機名稱 字數超過上限";
            }

            if (DataArray[2].Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "讀卡機說明 字數超過上限";
            }

            if (string.IsNullOrEmpty(DataArray[3].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "對應設備 必須指定";
            }

            if (string.IsNullOrEmpty(DataArray[4].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "方向控制 必須指定";
            }

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
                        liSqlPara.Add("S:" + ClickItem.Trim());
                        break;
                }

                oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                if (dr.Read())
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "此讀卡機編號已存在於系統中";
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
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt;

            //this.Reader_Input_EquNo.Items.Clear();

            //Item = new System.Web.UI.WebControls.ListItem();
            //Item.Text = "- 請選擇 -";
            //Item.Value = "";
            //this.Reader_Input_EquNo.Items.Add(Item);

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
                //for (int i = 0; i < dr.DataReader.FieldCount; i++)
                EditData[i] = tempstr;

                //Item = new System.Web.UI.WebControls.ListItem();
                //Item.Text = dr["EquName"].ToString();
                //Item.Value = dr["EquNo"].ToString();
                //this.Reader_Input_EquNo.Items.Add(Item);
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
            System.Web.UI.WebControls.ListItem Item;
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

            jstr = @"EquOrg_TreeView.oncontextmenu = showMenu;
                document.body.onclick = hideMenu;
                SetDivMode('');";

            ScriptManager.RegisterStartupScript(TreeView_UpdatePanel, TreeView_UpdatePanel.GetType(), "TreeViewJS", jstr, true);
        }
        #endregion

        #endregion
    }
}