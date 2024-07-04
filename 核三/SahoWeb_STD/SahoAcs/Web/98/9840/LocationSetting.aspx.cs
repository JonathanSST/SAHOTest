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
using iTextSharp.tool.xml.html;
using Microsoft.Ajax.Utilities;
using SahoAcs.DBModel;

namespace SahoAcs
{
    public partial class LocationSetting : Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        #endregion

        DB_Acs MyoAcsDB = null;

        public static Dictionary<string, string> objItemList = new Dictionary<string, string>();
        private static DataTable objLocTypeDt = new DataTable();

        #region 設定靜態屬性，方便 Webthod 使用
        private static string[] strLoc = new string[5];
        //private static string[] string_Loc = new string[5];

        private static string sUserID = "";      //儲存目前使用者的UserID
        private static string sUserName = "";    //儲存目前使用者的UserName

        private static DBReader drTemp = new DBReader();
        private static DataTable dtTemp = new DataTable();


        #endregion

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MyoAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;

            #region ControlToJavaScript
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";
            #endregion

            string jsFileEnd = "<script src=\"LocationSetting_End.js?" + DateTime.Now.ToString("yyyyMMddHHmmss") + "\" Type =\"text/javascript\"></script>";
            ClientScript.RegisterStartupScript(typeof(string), "LocationSetting_End", jsFileEnd, false);

            ScriptManager.RegisterStartupScript(TreeView_UpdatePanel, TreeView_UpdatePanel.GetType(), "OnPageLoad", js, false);

            RegisterTreeViewJS();           // 處理樹狀結構
            ClientScript.RegisterClientScriptInclude("LocationSetting", "LocationSetting.js?" + Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案

            ClientScript.RegisterClientScriptInclude("jqueryMin", Pub.JqueyNowVer);
            ClientScript.RegisterClientScriptInclude("jqueryUI", "/Scripts/jquery-ui.js");

            #region 設定彈跳視窗
            Pub.SetModalPopup(ModalPopupExtender1, 1);
            #endregion

            #region 註冊主頁Button動作

            Loc_B_Add.Attributes["onClick"] = "InsertLocExcute(); return false;";
            Loc_B_Edit.Attributes["onClick"] = "UpdateLocExcute(); return false;";
            Loc_B_Delete.Attributes["onClick"] = "DeleteLocExcute(); return false;";
            Loc_B_Cancel.Attributes["onClick"] = "SetDivMode(''); return false;";

            #endregion


            strLoc[0] = "編號";
            strLoc[1] = "名稱";
            strLoc[2] = "Map";
            strLoc[3] = "說明";
            strLoc[4] = "上層ID";

            #region 預先載入資料
            LocTypeModel();
            #endregion

            #region 註冊pop1頁Button動作
            ImgCloseButton1.Attributes["onClick"] = "CancelTrigger1.click(); return false;";
            //btnSearch.Attributes["onClick"] = "SearchEquData('" + txtKeyWord.Text.Trim() + "'); return false;";
            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                // Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");

                sUserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                sUserName = Sa.Web.Fun.GetSessionStr(this.Page, "UserName");

                LoadAREA("");
                TreeView_UpdatePanel.Update();
                GetLocationInfoList();

                Div_Loc.Attributes["style"] = "display:none";

                if (Session["SahoWebSocket"] != null)
                {
                    ((SahoWebSocket)Session["SahoWebSocket"]).ClearCmdResult();
                }
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];

                string[] sFormArg = Request.Form["__EVENTARGUMENT"].ToString().Split('_');

                if (sFormTarget == this.TreeView_UpdatePanel.ClientID)
                {
                    if (sFormArg.GetUpperBound(0) == 0)
                    {
                        if (sFormArg[0] == "Refalsh")
                        {
                            txt_NodeTypeList.Value = "";
                            txt_NodeIDList.Value = "";
                            LoadAREA("");
                            GetLocationInfoList();
                            TreeView_UpdatePanel.Update();
                            Location_TreeView.ExpandAll();
                            
                        }
                    }
                    else
                    {
                        if (sFormArg[0] == "Refalsh")
                        {
                            txt_NodeTypeList.Value = "";
                            txt_NodeIDList.Value = "";
                            LoadAREA("");
                            GetLocationInfoList();
                            TreeView_UpdatePanel.Update();
                            Location_TreeView.ExpandAll();
                           

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

        #region GetLocationInfoList
        private void GetLocationInfoList()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            List<string> liSqlPara = new List<string>();

            objLocTypeDt.Clear();
            this.Loc_Input_PList.Items.Clear();

            string strSql = @"SELECT LocID, LocType, LocNo, LocName, LocMap, LocDesc , dbo.Get_LocLongName(LocID,' / ') LocPList,
                              LocPID FROM B01_Location WHERE LocType <> 'FLOOR'";
            oAcsDB.GetDataTable("LocListTable", strSql, liSqlPara, out objLocTypeDt);

            if (objLocTypeDt.Rows.Count > 0)
            {
                foreach (DataRow dr in objLocTypeDt.Rows)
                {
                    string strLocPName = "[" + dr["LocNo"].ToString().ToString() + "] " + dr["LocPList"].ToString().ToString();
                    this.Loc_Input_PList.Items.Add(new ListItem(strLocPName, dr["LocID"].ToString()));
                }
            }

            this.Loc_Input_PList.Items.Insert(0, new ListItem("===最上層===", "0"));
        }
        #endregion

        #region LoadNewPList
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadNewPList()
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt;

            #region Process String
            sql = @" SELECT LocID, LocType, LocNo, LocName, LocMap, LocDesc , dbo.Get_LocLongName(LocID,' / ') LocPList,
                     LocPID FROM B01_Location WHERE LocType <> 'FLOOR' ";
            #endregion

            oAcsDB.GetDataTable("EquNoTable", sql, liSqlPara, out dt);

            string[] EditData = null;
            EditData = new string[dt.Rows.Count];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                EditData[i] = dt.Rows[i]["LocID"].ToString() + "," + "[" + dt.Rows[i]["LocNo"].ToString().ToString() + "] " + dt.Rows[i]["LocPList"].ToString().ToString();
            }

            return EditData;
        }
        #endregion

        #region LoadAREA
        private void LoadAREA(string strFilter)
        {
            string sql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            Location_TreeView.Nodes.Clear();

            Location_TreeView.Attributes.Add("Onclick", "OpenWin();");

            TreeNode RootNode = new TreeNode();

            RootNode.Text = "門禁管理系統";

            txt_NodeTypeList.Value += "SMS,";
            txt_NodeIDList.Value += "0,";
            RootNode.SelectAction = TreeNodeSelectAction.Select;

            List<LocInfo> LocTree = new List<LocInfo>();

            sql = "SELECT LocID, LocPID, LocType, LocNo, LocName, dbo.Get_LocLongName(LocID,' / ') LocPList FROM B01_Location WHERE LocPID=0 ;";

            this.MyoAcsDB.GetDataTable("LocTable", sql, liSqlPara, out dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                LocInfo locInfo = new LocInfo();
                locInfo.LocID = int.Parse(dt.Rows[i]["LocID"].ToString());
                locInfo.LocPID = int.Parse(dt.Rows[i]["LocPID"].ToString());
                locInfo.LocType = dt.Rows[i]["LocType"].ToString().Trim();
                locInfo.LocNo = dt.Rows[i]["LocNo"].ToString().Trim();
                locInfo.LocName = dt.Rows[i]["LocName"].ToString().Trim();
                locInfo.LocPList = dt.Rows[i]["LocPList"].ToString().Trim();

                LocTree.Add(locInfo);
            }

            LoadLocTree(RootNode, LocTree);

            Location_TreeView.Nodes.Add(RootNode);
            Location_TreeView.ShowLines = true;
        }
        #endregion

        #region LoadLocTree
        private void LoadLocTree(TreeNode PNode, List<LocInfo> objLocTree)
        {
            string sql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            List<SubLocInfo> LocSecondTree;

            for (int i = 0; i < objLocTree.Count; i++)
            {
                LocSecondTree = new List<SubLocInfo>();
                sql = "";
                liSqlPara.Clear();
                TreeNode SubNode = new TreeNode();

                SubNode.Text += "[" + objLocTree[i].LocNo + "] " + " - " + objLocTree[i].LocName;
                txt_NodeTypeList.Value += "AREA,";
                txt_NodeIDList.Value += objLocTree[i].LocID.ToString() + ",";
                SubNode.NavigateUrl = "#";

                sql = " SELECT LocID, LocPID, LocType, LocNo, LocName, dbo.Get_LocLongName(LocID,' / ') LocPList FROM B01_Location WHERE LocPID= ? ORDER BY LocNo; ";
                liSqlPara.Add("S:" + objLocTree[i].LocID);

                this.MyoAcsDB.GetDataTable("LocSecondTable", sql, liSqlPara, out dt);

                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    SubLocInfo locSecond = new SubLocInfo();
                    locSecond.LocID = int.Parse(dt.Rows[k]["LocID"].ToString());
                    locSecond.LocPID = int.Parse(dt.Rows[k]["LocPID"].ToString());
                    locSecond.LocType = dt.Rows[k]["LocType"].ToString().Trim();
                    locSecond.LocNo = dt.Rows[k]["LocNo"].ToString().Trim();
                    locSecond.LocName = dt.Rows[k]["LocName"].ToString().Trim();
                    locSecond.LocPList = dt.Rows[k]["LocPList"].ToString().Trim();

                    LocSecondTree.Add(locSecond);
                }
                objLocTree[i].SubLocInfo = LocSecondTree;
                LoadSecondTree(SubNode, objLocTree[i].SubLocInfo);
                PNode.ChildNodes.Add(SubNode);
            }
        }
        #endregion

        #region LoadMasterTree
        private void LoadSecondTree(TreeNode PNode, List<SubLocInfo> objSecondTree)
        {
            string sql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            List<SubLocInfo> LocThredTree;

            for (int i = 0; i < objSecondTree.Count; i++)
            {
                LocThredTree = new List<SubLocInfo>();
                sql = "";
                liSqlPara.Clear();
                TreeNode SubNode = new TreeNode();

                SubNode.Text += "[" + objSecondTree[i].LocNo + "] " + " - " + objSecondTree[i].LocName;


                txt_NodeTypeList.Value += "BUILDING,";
                txt_NodeIDList.Value += objSecondTree[i].LocID.ToString() + ",";
                SubNode.NavigateUrl = "#";

                sql = " SELECT LocID, LocPID, LocType, LocNo, LocName FROM B01_Location WHERE LocPID= ? ORDER BY LocNo; ";
                liSqlPara.Add("S:" + objSecondTree[i].LocID);
                this.MyoAcsDB.GetDataTable("LocThred", sql, liSqlPara, out dt);

                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    SubLocInfo locThred = new SubLocInfo();

                    locThred.LocID = int.Parse(dt.Rows[k]["LocID"].ToString());
                    locThred.LocPID = int.Parse(dt.Rows[k]["LocPID"].ToString());
                    locThred.LocType = dt.Rows[k]["LocType"].ToString().Trim();
                    locThred.LocNo = dt.Rows[k]["LocNo"].ToString().Trim();
                    locThred.LocName = dt.Rows[k]["LocName"].ToString().Trim();

                    BuildThredTree(SubNode, locThred);
                }


                PNode.ChildNodes.Add(SubNode);
            }
        }
        #endregion

        #region BuildThredTree
        private void BuildThredTree(TreeNode PNode, SubLocInfo objThredList)
        {
            TreeNode SubNode = new TreeNode();
            SubNode.Text = "[" + objThredList.LocNo + "] " + objThredList.LocName;
            txt_NodeTypeList.Value += "FLOOR,";
            txt_NodeIDList.Value += objThredList.LocID.ToString() + ",";
            SubNode.NavigateUrl = "#";
            PNode.ChildNodes.Add(SubNode);
        }
        #endregion

        #region LocTypeModel
        private void LocTypeModel()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt = new DataTable();

            sql = @" SELECT DISTINCT ItemName, ItemNo FROM B00_ItemList WHERE ItemClass = 'LocationType' ";
            oAcsDB.GetDataTable("LocTypeTable", sql, liSqlPara, out dt);

            objItemList.Clear();

            foreach (DataRow dr in dt.Rows)
            {
                objItemList.Add(dr["ItemNo"].ToString(), dr["ItemName"].ToString());
            }
        }
        #endregion

        #region Location相關

        #region  LoadLocInfo
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadLocInfo(string LocID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            string[] LocInfo = null;
            List<string> liSqlPara = new List<string>();
            DBReader dr = new DBReader();
            bool isSuccess = true;

            try
            {
                sql = @" 
                       SELECT LocID, LocType, LocNo, LocName, LocMap, LocDesc , dbo.Get_LocLongName(LocID,' / ') LocList,LocPID 
                       from B01_Location where LocID= ? ";
                liSqlPara.Add("I:" + LocID.Trim());

                isSuccess = oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                if (isSuccess)
                {
                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {
                            LocInfo = new string[dr.DataReader.FieldCount + 1];
                            for (int i = 0; i < dr.DataReader.FieldCount; i++)
                            {
                                LocInfo[i] = dr.DataReader[i].ToString();
                            }

                            if (objItemList.ContainsKey(LocInfo[1].ToString()))
                            {
                                LocInfo[LocInfo.Length - 1] = objItemList[LocInfo[1].ToString()];
                            }
                            else
                            {
                                LocInfo[LocInfo.Length - 1] = LocInfo[1].ToString();
                            }


                            drTemp.Free();
                            drTemp = dr;
                        }
                        else
                        {
                            LocInfo = new string[2];
                            LocInfo[0] = "Saho_SysErrorMassage";
                            LocInfo[1] = "Can Not Read DataReader！";
                        }
                    }
                    else
                    {
                        LocInfo = new string[2];
                        LocInfo[0] = "Saho_SysErrorMassage";
                        LocInfo[1] = "No Data！";
                    }
                }
                else
                {
                    LocInfo = new string[2];
                    LocInfo[0] = "Saho_SysErrorMassage";
                    LocInfo[1] = "Load Database failure！";
                }
            }
            catch (Exception ex)
            {
                LocInfo = new string[2];
                LocInfo[0] = "Saho_SysErrorMassage";
                LocInfo[1] = string.Format("[LoadLocInfo Exception] {0}", ex.Message);
            }

            return LocInfo;
        }

        #endregion

        #region InsertLocInfo
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object InsertLocInfo(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            objRet = Check_Input_DB_LocInfo(DataArray, "Insert");
            string strLocID = DataArray[6].Trim();

            if (objRet.result)
            {
                sql = @"INSERT INTO B01_Location (LocType, LocNo, LocName, LocPID, LocMap, LocDesc, CreateUserID, CreateTime) 
                        VALUES (?,?,?,?,?,?,?,?);
                ";

                liSqlPara.Add("S:" + DataArray[0].Trim());
                liSqlPara.Add("S:" + DataArray[2].Trim());
                liSqlPara.Add("S:" + DataArray[3].Trim());
                liSqlPara.Add("I:" + DataArray[7].Trim());
                liSqlPara.Add("S:" + DataArray[4].Trim());
                liSqlPara.Add("S:" + DataArray[5].Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);

                try
                {
                    int intRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);

                    if (intRet > 0)
                    {
                        #region 寫入B00_SysLog
                        int[] intAry = new int[] { 2, 3, 4, 5, 7 };
                        string[] strAry = new string[] { "LocNo", "LocName", "LocMap", "LocDesc", "LocPID" };

                        List<Machine> liData = new List<Machine>();
                        int r = 0;
                        foreach (int g in intAry)
                        {

                            string strNewValue = DataArray[g].Trim();

                            liData.Add(new Machine(strAry[r], strLoc[r], "", strNewValue));
                            r++;
                        }

                        oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, sUserID, sUserName, "9840", "", "",
                            string.Format("區域/棟別/樓層設定：{0}，{1}", strLocID, CompareVaule("Insert", liData)),
                            "新增" + DataArray[1].ToString());
                        #endregion

                        #region 取得剛剛新增的LocNo的LocID回傳使用
                        sql = @" 
                            SELECT TOP 1 LocID FROM B01_Location 
                            WHERE LocNo = ? AND LocType = ? AND LocPID = ? AND CreateTime = ?";

                        liSqlPara.Clear();
                        liSqlPara.Add("S:" + DataArray[2].Trim());
                        liSqlPara.Add("S:" + DataArray[0].Trim());
                        liSqlPara.Add("S:" + DataArray[7].Trim());
                        liSqlPara.Add("D:" + Time);

                        strLocID = oAcsDB.GetStrScalar(sql, liSqlPara);

                        objRet.message = strLocID;  // 取得LocID回傳使用

                        #endregion
                    }
                    else
                    {
                        objRet.message = "新增失敗";
                        objRet.result = false;
                    }
                }
                catch (Exception ex)
                {
                    objRet.message = string.Format("[InsertLocInfo] {0}", ex.Message);
                    objRet.result = false;
                }
            }

            objRet.act = "InsertLocInfo";
            return objRet;

        }


        #endregion

        #region UpdateLocInfo
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object UpdateLocInfo(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            objRet = Check_Input_DB_LocInfo(DataArray, "Update");
            string strLocID = DataArray[6].Trim();

            if (objRet.result)
            {
                sql = @"UPDATE B01_Location Set 
                            LocType = ?, LocNo = ?, LocName = ?, LocPID = ?, 
                            LocMap = ?, LocDesc = ?, UpdateUserID= ?, UpdateTime = getdate() 
                        WHERE LocID = ? ;
                ";

                liSqlPara.Add("S:" + DataArray[0].Trim());
                liSqlPara.Add("S:" + DataArray[2].Trim());
                liSqlPara.Add("S:" + DataArray[3].Trim());
                liSqlPara.Add("I:" + DataArray[7].Trim());
                liSqlPara.Add("S:" + DataArray[4].Trim());
                liSqlPara.Add("S:" + DataArray[5].Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("I:" + DataArray[6].Trim());

                try
                {
                    int intRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);

                    if (intRet > 0)
                    {
                        #region 寫入B00_SysLog
                        int[] intAry = new int[] { 2, 3, 4, 5, 7 };
                        string[] strAry = new string[] { "LocNo", "LocName", "LocMap", "LocDesc", "LocPID" };

                        List<Machine> liData = new List<Machine>();
                        int r = 0;
                        foreach (int g in intAry)
                        {
                            string strOldValue = drTemp.DataReader.GetValue(g).ToString();
                            string strNewValue = DataArray[g].Trim();

                            liData.Add(new Machine(strAry[r], strLoc[r], strOldValue, strNewValue));
                            r++;
                        }

                        oAcsDB.WriteLog(DB_Acs.Logtype.資料修改, sUserID, sUserName, "9840", "", "",
                            string.Format("區域/棟別/樓層設定：{0}，{1}", strLocID, CompareVaule("Update", liData)),
                            "修改區域/棟別/樓層設定");
                        #endregion

                        objRet.message = strLocID;  // 取得DctID回傳使用
                    }
                    else
                    {
                        objRet.message = "更新失敗";
                        objRet.result = false;
                    }
                }
                catch (Exception ex)
                {
                    objRet.message = string.Format("[UpdateLocInfo] {0}", ex.Message);
                    objRet.result = false;
                }
            }

            objRet.act = "UpdateLocInfo";
            return objRet;
        }
        #endregion

        #region DeleteLocInfo
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object DeleteLocInfo(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            DBReader dr = new DBReader();

            sql = @"SELECT 1 from B01_Location WHERE LocPID = ? ;";
            liSqlPara.Add("S:" + DataArray[6].Trim());

            try
            {
                bool isSuccess = oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                if (isSuccess)
                {
                    if (dr.HasRows)
                    {
                        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                        objRet.result = false;
                        objRet.message += "此項目已被使用，無法刪除";
                    }

                    if (objRet.result)
                    {
                        sql = " DELETE FROM B01_Location WHERE LocID = ? ";
                        liSqlPara.Clear();
                        liSqlPara.Add("S:" + DataArray[6].Trim());

                        int intRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                        if (intRet > 0)
                        {
                            #region 寫入B00_SysLog
                            oAcsDB.WriteLog(DB_Acs.Logtype.資料刪除, sUserID, sUserName, "9840","","",
                                string.Format("區域/棟別/樓層設定：{0}", DataArray[6].Trim()),
                                "刪除資料");
                            #endregion

                            objRet.message = DataArray[6].Trim();  // 取得LocID回傳使用
                        }
                        else
                        {
                            objRet.message = "刪除資料失敗";  // 刪除連線失敗
                            objRet.result = false;
                        }
                    }
                }
                else
                {
                    objRet.message = "資料庫程式處理失敗，請再試一次";  
                    objRet.result = false;
                }
            }
            catch (Exception ex)
            {
                objRet.message = string.Format("[DeleteLocInfo] {0}", ex.Message);
                objRet.result = false;
            }

            objRet.act = "DeleteLocInfo";
            return objRet;
        }
        #endregion

        #endregion

        #region DeviceConnInfo 相關


        #region Check_Input_DB_LocInfo
        protected static MessageObject1 Check_Input_DB_LocInfo(string[] DataArray, string Mode)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            MessageObject1 objRet = new MessageObject1();
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DBReader dr = new DBReader();

            if (string.IsNullOrEmpty(DataArray[2].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;
                objRet.message += "編號必須輸入";
            }

            if (string.IsNullOrEmpty(DataArray[3].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;
                objRet.message += "名稱必須輸入";
            }
            else if (Encoding.Default.GetBytes(DataArray[3].Trim()).Length >= 60)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;
                objRet.message += "名稱字數超過上限";
            }

            //判斷上層位置是否正確
            DataRow[] drow = null;
            bool tmpFlag = false;
            if (DataArray[0].Trim() == "BUILDING")
            {
                drow = objLocTypeDt.Select("LocType='AREA'");
            }
            else if (DataArray[0].Trim() == "FLOOR")
            {
                drow = objLocTypeDt.Select("LocType='BUILDING'");
            }
            else if (DataArray[0].Trim() == "AREA")
            {
                if (int.Parse(DataArray[7].Trim()) == 0)
                {
                    tmpFlag = true;
                }
            }
            if (drow != null)
            {
                if (drow.Count() > 0)
                {
                    foreach (var g in drow)
                    {
                        if (int.Parse(g.ItemArray[0].ToString()) == int.Parse(DataArray[7].Trim()))
                        {
                            tmpFlag = true;
                            break;
                        }
                    }
                }
            }

            if (!tmpFlag)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;
                objRet.message += "上層選單有錯";
            }


            //DB
            if (objRet.result)
            {
                if (Mode == "Insert")
                {
                    sql = @" SELECT * FROM B01_Location WHERE LocNo = ? AND LocType = ? AND LocPID = ? ;";
                    liSqlPara.Add("S:" + DataArray[2].Trim());
                    liSqlPara.Add("S:" + DataArray[0].Trim());
                    liSqlPara.Add("S:" + DataArray[7].Trim());

                    dr = new DBReader();
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                    if (dr.HasRows)
                    {
                        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                        objRet.result = false;
                        objRet.message += "此編號已存在。";
                    }
                }
            }

            return objRet;
        }


        #endregion


        #endregion


        #endregion

        #region RegisterStartupScript

        #region RegisterTreeViewJS
        private void RegisterTreeViewJS()
        {
            string jstr = "";
            jstr = @"Location_TreeView.oncontextmenu = showMenu;
                document.body.onclick = hideMenu;";

            ScriptManager.RegisterStartupScript(TreeView_UpdatePanel, TreeView_UpdatePanel.GetType(), "TreeViewJS", jstr, true);
        }

        #endregion

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

    }

}