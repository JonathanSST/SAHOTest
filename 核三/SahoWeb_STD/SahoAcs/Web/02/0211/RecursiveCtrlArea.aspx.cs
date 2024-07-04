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
using AjaxControlToolkit.Config;

namespace SahoAcs
{
    public partial class RecursiveCtrlArea : Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        #endregion

        DB_Acs MyoAcsDB = null;

        public static Dictionary<string, string> objItemList = new Dictionary<string, string>();
        private static DataTable objDt = new DataTable();

        #region 設定靜態屬性，方便 Webthod 使用
        private static string[] strArea = new string[5];
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

            string jsFileEnd = "<script src=\"RecursiveCtrlArea_End.js?" + DateTime.Now.ToString("yyyyMMddHHmmss") + "\" Type =\"text/javascript\"></script>";
            ClientScript.RegisterStartupScript(typeof(string), "RecursiveCtrlArea_End", jsFileEnd, false);

            ScriptManager.RegisterStartupScript(TreeView_UpdatePanel, TreeView_UpdatePanel.GetType(), "OnPageLoad", js, false);

            RegisterTreeViewJS();           // 處理樹狀結構
            ClientScript.RegisterClientScriptInclude("RecursiveCtrlArea", "RecursiveCtrlArea.js?" + Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案

            ClientScript.RegisterClientScriptInclude("jqueryMin", Pub.JqueyNowVer);
            ClientScript.RegisterClientScriptInclude("jqueryUI", "/Scripts/jquery-ui.js");

            #region 設定彈跳視窗
            Pub.SetModalPopup(ModalPopupExtender1, 1);
            #endregion

            #region 註冊主頁Button動作

            Area_B_Add.Attributes["onClick"] = "InsertAreaExcute(); return false;";
            Area_B_Edit.Attributes["onClick"] = "UpdateAreaExcute(); return false;";
            Area_B_Delete.Attributes["onClick"] = "DeleteAreaExcute(); return false;";
            Area_B_Cancel.Attributes["onClick"] = "SetDivMode(''); return false;";

            #endregion

            strArea[0] = "編號";
            strArea[1] = "名稱";
            strArea[2] = "上層ID";
            strArea[3] = "是否管制";
            strArea[4] = "說明";

            //#region 預先載入資料
            //LocTypeModel();
            //#endregion

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

                LoadCtrlArea("");
                TreeView_UpdatePanel.Update();
                GetAreaInfoList();

                Div_CtrlArea.Attributes["style"] = "display:none";

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
                            LoadCtrlArea("");
                            GetAreaInfoList();
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
                            LoadCtrlArea("");
                            GetAreaInfoList();
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
        private void GetAreaInfoList()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            List<string> liSqlPara = new List<string>();

            objDt.Clear();
            this.Area_Input_PList.Items.Clear();

            string strSql = @"SELECT CtrlAreaID, CtrlAreaNo, CtrlAreaName, CtrlAreaPID, IsControl, CtrlAreaDesc, CtrlAreaLevel, 
                                dbo.Get_AreaLongName(CtrlAreaID, ' / ') AS LongName
                                FROM B01_CtrlArea WHERE CtrlAreaLevel <> 3 ORDER BY CtrlAreaLevel, CtrlAreaNo ";
            oAcsDB.GetDataTable("AreacListTable", strSql, liSqlPara, out objDt);

            if (objDt.Rows.Count > 0)
            {
                foreach (DataRow dr in objDt.Rows)
                {
                    string strPName = "";

                    if (dr["CtrlAreaLevel"].ToString() == "1")
                    {
                        strPName = "[" + dr["CtrlAreaNo"].ToString() + "] " + dr["LongName"].ToString();
                    } else if (dr["CtrlAreaLevel"].ToString() == "2")
                    {
                        strPName = "[" + dr["CtrlAreaNo"].ToString() + "] " + dr["LongName"].ToString();
                    }

                    this.Area_Input_PList.Items.Add(new ListItem(strPName, dr["CtrlAreaID"].ToString()));
                }
            }

            this.Area_Input_PList.Items.Insert(0, new ListItem("===最上層===", "0"));
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

            sql = @"SELECT CtrlAreaID, CtrlAreaNo, CtrlAreaName, CtrlAreaPID, IsControl, CtrlAreaDesc, CtrlAreaLevel, 
                                dbo.Get_AreaLongName(CtrlAreaID, ' / ') AS LongName
                                FROM B01_CtrlArea WHERE CtrlAreaLevel <> 3 ORDER BY CtrlAreaLevel, CtrlAreaNo ";
            oAcsDB.GetDataTable("AreacListTable", sql, liSqlPara, out dt);

            string[] EditData = null;
            EditData = new string[dt.Rows.Count];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                EditData[i] = dt.Rows[i]["CtrlAreaID"].ToString() + "|" + "[" + dt.Rows[i]["CtrlAreaNo"].ToString().ToString() + "] " + dt.Rows[i]["LongName"].ToString().ToString();
            }

            return EditData;
        }
        #endregion

        #region LoadCtrlArea
        private void LoadCtrlArea(string strFilter)
        {
            string sql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            Location_TreeView.Nodes.Clear();

            Location_TreeView.Attributes.Add("Onclick", "OpenWin();");

            TreeNode RootNode = new TreeNode();

            RootNode.Text = "遞迴區域列表";

            txt_NodeTypeList.Value += "SMS,";
            txt_NodeIDList.Value += "0,";
            RootNode.SelectAction = TreeNodeSelectAction.Select;

            List<CtrlAreaInfo> AreaTree = new List<CtrlAreaInfo>();

            sql = "select CtrlAreaID, CtrlAreaNo, CtrlAreaName, CtrlAreaPID, IsControl, CtrlAreaDesc, CtrlAreaLevel from B01_CtrlArea where CtrlAreaLevel=1 ORDER BY CtrlAreaNo;";

            this.MyoAcsDB.GetDataTable("CtrlAreaTable", sql, liSqlPara, out dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                CtrlAreaInfo ctrlInfo = new CtrlAreaInfo();
                ctrlInfo.CtrlAreaID = int.Parse(dt.Rows[i]["CtrlAreaID"].ToString());
                ctrlInfo.CtrlAreaNo = dt.Rows[i]["CtrlAreaNo"].ToString();
                ctrlInfo.CtrlAreaName = dt.Rows[i]["CtrlAreaName"].ToString();
                ctrlInfo.CtrlAreaPID = int.Parse(dt.Rows[i]["CtrlAreaPID"].ToString());
                ctrlInfo.Level = int.Parse(dt.Rows[i]["CtrlAreaLevel"].ToString());

                AreaTree.Add(ctrlInfo);
            }

            LoadFirstTree(RootNode, AreaTree);

            Location_TreeView.Nodes.Add(RootNode);
            Location_TreeView.ShowLines = true;
        }
        #endregion

        #region LoadLocTree
        private void LoadFirstTree(TreeNode PNode, List<CtrlAreaInfo> objFirstTree)
        {
            string sql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            List<CtrlAreaInfo> SecondTree;

            for (int i = 0; i < objFirstTree.Count; i++)
            {
                SecondTree = new List<CtrlAreaInfo>();
                sql = "";
                liSqlPara.Clear();
                TreeNode SubNode = new TreeNode();

                SubNode.Text += "[" + objFirstTree[i].CtrlAreaNo + "] " + " - " + objFirstTree[i].CtrlAreaName;
                txt_NodeTypeList.Value += "FIRST,";
                txt_NodeIDList.Value += objFirstTree[i].CtrlAreaID.ToString() + ",";
                SubNode.NavigateUrl = "#";

                sql = " select CtrlAreaID, CtrlAreaNo, CtrlAreaName, CtrlAreaPID, IsControl, CtrlAreaDesc, CtrlAreaLevel from B01_CtrlArea WHERE CtrlAreaPID= ? ORDER BY CtrlAreaNo; ";
                liSqlPara.Add("S:" + objFirstTree[i].CtrlAreaID);

                this.MyoAcsDB.GetDataTable("SecondTable", sql, liSqlPara, out dt);

                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    CtrlAreaInfo areaSecond = new CtrlAreaInfo();
                    areaSecond.CtrlAreaID = int.Parse(dt.Rows[k]["CtrlAreaID"].ToString());
                    areaSecond.CtrlAreaNo = dt.Rows[k]["CtrlAreaNo"].ToString();
                    areaSecond.CtrlAreaName = dt.Rows[k]["CtrlAreaName"].ToString();
                    areaSecond.CtrlAreaPID = int.Parse(dt.Rows[k]["CtrlAreaPID"].ToString());
                    areaSecond.Level = int.Parse(dt.Rows[k]["CtrlAreaLevel"].ToString());

                    SecondTree.Add(areaSecond);
                }
                objFirstTree[i].SubMgrAreas = SecondTree;
                LoadSecondTree(SubNode, objFirstTree[i].SubMgrAreas);
                PNode.ChildNodes.Add(SubNode);
            }
        }
        #endregion

        #region LoadMasterTree
        private void LoadSecondTree(TreeNode PNode, List<CtrlAreaInfo> objSecondTree)
        {
            string sql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            List<CtrlAreaInfo> ThredTree;

            for (int i = 0; i < objSecondTree.Count; i++)
            {
                ThredTree = new List<CtrlAreaInfo>();
                sql = "";
                liSqlPara.Clear();
                TreeNode SubNode = new TreeNode();

                SubNode.Text += "[" + objSecondTree[i].CtrlAreaNo + "] " + " - " + objSecondTree[i].CtrlAreaName;


                txt_NodeTypeList.Value += "SECOND,";
                txt_NodeIDList.Value += objSecondTree[i].CtrlAreaID.ToString() + ",";
                SubNode.NavigateUrl = "#";

                sql = " select CtrlAreaID, CtrlAreaNo, CtrlAreaName, CtrlAreaPID, IsControl, CtrlAreaDesc, CtrlAreaLevel from B01_CtrlArea WHERE CtrlAreaPID= ? ORDER BY CtrlAreaNo; ";
                liSqlPara.Add("S:" + objSecondTree[i].CtrlAreaID);
                this.MyoAcsDB.GetDataTable("ThredTable", sql, liSqlPara, out dt);

                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    CtrlAreaInfo areaThred = new CtrlAreaInfo();
                    areaThred.CtrlAreaID = int.Parse(dt.Rows[k]["CtrlAreaID"].ToString());
                    areaThred.CtrlAreaNo = dt.Rows[k]["CtrlAreaNo"].ToString();
                    areaThred.CtrlAreaName = dt.Rows[k]["CtrlAreaName"].ToString();
                    areaThred.CtrlAreaPID = int.Parse(dt.Rows[k]["CtrlAreaPID"].ToString());
                    areaThred.Level = int.Parse(dt.Rows[k]["CtrlAreaLevel"].ToString());

                    BuildThredTree(SubNode, areaThred);
                }


                PNode.ChildNodes.Add(SubNode);
            }
        }
        #endregion

        #region BuildThredTree
        private void BuildThredTree(TreeNode PNode, CtrlAreaInfo objThredList)
        {
            TreeNode SubNode = new TreeNode();
            SubNode.Text = "[" + objThredList.CtrlAreaNo + "] " + objThredList.CtrlAreaName;
            txt_NodeTypeList.Value += "THRID,";
            txt_NodeIDList.Value += objThredList.CtrlAreaID.ToString() + ",";
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

        #region CtrlArea相關

        #region  LoadAreaInfo
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadAreaInfo(string AreaID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            string[] AreaInfo = null;
            List<string> liSqlPara = new List<string>();
            DBReader dr = new DBReader();
            bool isSuccess = true;

            try
            {
                sql = @" 
                       SELECT CtrlAreaID, CtrlAreaNo, CtrlAreaName, CtrlAreaPID, IsControl, CtrlAreaDesc, CtrlAreaLevel,
                       dbo.Get_AreaLongName(CtrlAreaID, ' / ') AS LongName 
                       FROM B01_CtrlArea WHERE CtrlAreaID= ? ";
                liSqlPara.Add("I:" + AreaID.Trim());

                isSuccess = oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                if (isSuccess)
                {
                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {
                            AreaInfo = new string[dr.DataReader.FieldCount + 1];
                            for (int i = 0; i < dr.DataReader.FieldCount; i++)
                            {
                                AreaInfo[i] = dr.DataReader[i].ToString();
                            }

                            drTemp.Free();
                            drTemp = dr;
                        }
                        else
                        {
                            AreaInfo = new string[2];
                            AreaInfo[0] = "Saho_SysErrorMassage";
                            AreaInfo[1] = "Can Not Read DataReader！";
                        }
                    }
                    else
                    {
                        AreaInfo = new string[2];
                        AreaInfo[0] = "Saho_SysErrorMassage";
                        AreaInfo[1] = "No Data！";
                    }
                }
                else
                {
                    AreaInfo = new string[2];
                    AreaInfo[0] = "Saho_SysErrorMassage";
                    AreaInfo[1] = "Load Database failure！";
                }
            }
            catch (Exception ex)
            {
                AreaInfo = new string[2];
                AreaInfo[0] = "Saho_SysErrorMassage";
                AreaInfo[1] = string.Format("[LoadAreaInfo Exception] {0}", ex.Message);
            }

            return AreaInfo;
        }

        #endregion

        #region InsertAreaInfo
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object InsertAreaInfo(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            objRet = Check_Input_DB_AreaInfo(DataArray, "Insert");
            string strAreaID = DataArray[0].Trim();

            if (objRet.result)
            {
                sql = @"INSERT INTO B01_CtrlArea (CtrlAreaNO, CtrlAreaName, CreateUserID, CreateTime, CtrlAreaPID, IsControl, CtrlAreaDesc, CtrlAreaLevel) 
                        VALUES (?,?,?,?,?,?,?,?);
                ";

                liSqlPara.Add("S:" + DataArray[1].Trim());
                liSqlPara.Add("S:" + DataArray[2].Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("S:" + DataArray[7].Trim());
                liSqlPara.Add("S:" + DataArray[4].Trim());
                liSqlPara.Add("S:" + DataArray[5].Trim());
                liSqlPara.Add("S:" + DataArray[6].Trim());

                try
                {
                    int intRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);

                    if (intRet > 0)
                    {
                        #region 寫入B00_SysLog
                        int[] intAry = new int[] { 1, 2, 3, 4, 5 };
                        string[] strAry = new string[] { "CtrlAreaNo", "CtrlAreaName", "CtrlAreaPID", "IsControl", "CtrlAreaDesc" };

                        List<Machine> liData = new List<Machine>();
                        int r = 0;
                        foreach (int g in intAry)
                        {

                            string strNewValue = DataArray[g].Trim();

                            liData.Add(new Machine(strAry[r], strArea[r], "", strNewValue));
                            r++;
                        }

                        oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, sUserID, sUserName, "0211", "", "",
                            string.Format("遞迴管制區管理：{0}", CompareVaule("Insert", liData)),
                            "新增第" + DataArray[6].ToString() + "層");
                        #endregion

                        #region 取得剛剛新增的LocNo的LocID回傳使用
                        sql = @" 
                            SELECT TOP 1 CtrlAreaID FROM B01_CtrlArea 
                            WHERE CtrlAreaNO = ? AND CtrlAreaLevel = ? AND CtrlAreaPID = ? AND CreateTime = ?";

                        liSqlPara.Clear();
                        liSqlPara.Add("S:" + DataArray[1].Trim());
                        liSqlPara.Add("S:" + DataArray[6].Trim());
                        liSqlPara.Add("S:" + DataArray[7].Trim());
                        liSqlPara.Add("D:" + Time);

                        strAreaID = oAcsDB.GetStrScalar(sql, liSqlPara);

                        objRet.message = strAreaID;  // 取得CtrlAreaID回傳使用

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
                    objRet.message = string.Format("[InsertAreaInfo] {0}", ex.Message);
                    objRet.result = false;
                }
            }

            objRet.act = "InsertAreaInfo";
            return objRet;

        }


        #endregion

        #region UpdateAreaInfo
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object UpdateAreaInfo(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            objRet = Check_Input_DB_AreaInfo(DataArray, "Update");
            string strAreaID = DataArray[0].Trim();

            if (objRet.result)
            {
                sql = @"UPDATE B01_CtrlArea Set 
                            CtrlAreaNo = ?, CtrlAreaName = ?, CtrlAreaPID = ?, IsControl = ?, 
                            CtrlAreaDesc = ?, UpdateUserID= ?, UpdateTime = getdate() 
                        WHERE CtrlAreaID = ? ;
                ";

                liSqlPara.Add("S:" + DataArray[1].Trim());
                liSqlPara.Add("S:" + DataArray[2].Trim());
                liSqlPara.Add("I:" + DataArray[7].Trim());
                liSqlPara.Add("I:" + DataArray[4].Trim());
                liSqlPara.Add("S:" + DataArray[5].Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("I:" + DataArray[0].Trim());

                try
                {
                    int intRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);

                    if (intRet > 0)
                    {
                        #region 寫入B00_SysLog
                        int[] intAry = new int[] { 1, 2, 3, 4, 5 };
                        string[] strAry = new string[] { "CtrlAreaNo", "CtrlAreaName", "CtrlAreaPID", "IsControl", "CtrlAreaDesc" };

                        List<Machine> liData = new List<Machine>();
                        int r = 0;
                        foreach (int g in intAry)
                        {
                            string strOldValue = drTemp.DataReader.GetValue(g).ToString();
                            string strNewValue = DataArray[g].Trim();

                            liData.Add(new Machine(strAry[r], strArea[r], strOldValue, strNewValue));
                            r++;
                        }

                        oAcsDB.WriteLog(DB_Acs.Logtype.資料修改, sUserID, sUserName, "0211", "", "",
                            string.Format("遞迴管制區管理：{0}，{1}", strAreaID, CompareVaule("Update", liData)),
                            "修改遞迴管制區管理");
                        #endregion

                        objRet.message = strAreaID;  // 取得DctID回傳使用
                    }
                    else
                    {
                        objRet.message = "更新失敗";
                        objRet.result = false;
                    }
                }
                catch (Exception ex)
                {
                    objRet.message = string.Format("[UpdateAreaInfo] {0}", ex.Message);
                    objRet.result = false;
                }
            }

            objRet.act = "UpdateAreaInfo";
            return objRet;
        }
        #endregion

        #region DeleteAreaInfo
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object DeleteAreaInfo(string UserID, string[] DataArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            DBReader dr = new DBReader();

            sql = @"SELECT 1 from B01_CtrlArea WHERE CtrlAreaPID = ? ;";
            liSqlPara.Add("S:" + DataArray[0].Trim());

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
                        sql = " DELETE FROM B01_CtrlArea WHERE CtrlAreaID = ? ";
                        liSqlPara.Clear();
                        liSqlPara.Add("S:" + DataArray[0].Trim());

                        int intRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                        if (intRet > 0)
                        {
                            #region 寫入B00_SysLog
                            oAcsDB.WriteLog(DB_Acs.Logtype.資料刪除, sUserID, sUserName, "0211","","",
                                string.Format("遞迴管制區管理：{0}", DataArray[0].Trim()),
                                "刪除資料");
                            #endregion

                            objRet.message = DataArray[0].Trim();  // 取得CtrlAreaID回傳使用
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
                objRet.message = string.Format("[DeleteAreaInfo] {0}", ex.Message);
                objRet.result = false;
            }

            objRet.act = "DeleteAreaInfo";
            return objRet;
        }
        #endregion

        #endregion

        #region DeviceConnInfo 相關


        #region Check_Input_DB_LocInfo
        protected static MessageObject1 Check_Input_DB_AreaInfo(string[] DataArray, string Mode)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            MessageObject1 objRet = new MessageObject1();
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DBReader dr = new DBReader();

            if (string.IsNullOrEmpty(DataArray[1].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;
                objRet.message += "編號必須輸入";
            }

            if (string.IsNullOrEmpty(DataArray[2].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;
                objRet.message += "名稱必須輸入";
            }
            else if (Encoding.Default.GetBytes(DataArray[2].Trim()).Length >= 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                objRet.result = false;
                objRet.message += "名稱字數超過上限";
            }

            //判斷上層位置是否正確
            DataRow[] drow = null;
            bool tmpFlag = false;
            if (DataArray[6].Trim() == "2")
            {
                drow = objDt.Select("CtrlAreaLevel='1'");
            }
            else if (DataArray[6].Trim() == "3")
            {
                drow = objDt.Select("CtrlAreaLevel='2'");
            }
            else if (DataArray[6].Trim() == "1")
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
                    sql = @" SELECT * FROM B01_CtrlArea WHERE CtrlAreaNo = ? AND CtrlAreaLevel = ? AND CtrlAreaPID = ? ;";
                    liSqlPara.Add("S:" + DataArray[1].Trim());
                    liSqlPara.Add("S:" + DataArray[6].Trim());
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