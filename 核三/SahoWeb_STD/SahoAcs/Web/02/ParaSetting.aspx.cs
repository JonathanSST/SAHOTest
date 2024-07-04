using SahoAcs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs
{
    public partial class ParaSetting : Sa.BasePage
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        DataTable ParaTable = new DataTable();
        DataTable ProcessTable = new DataTable();
        #endregion

        #region Events

        #region Page_Load
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Page.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(popB_Save);
            oScriptManager.RegisterAsyncPostBackControl(popB_Cancel);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("ParaSetting", "ParaSetting.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");

            ClientScript.RegisterClientScriptInclude("jqueryMin",Pub.JqueyNowVer);
            ClientScript.RegisterClientScriptInclude("jqueryUI", "/Scripts/jquery-ui.js");

            #region 註冊Button動作

            #endregion

            #endregion

            string strTitle = Request.QueryString["ParaType"] as string;

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                #region Give hideValue
                DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
                string sql = "";
                List<string> liSqlPara = new List<string>();
                Sa.DB.DBReader dr;

                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");

                // wei 20170323 區分成控制器參數或讀卡機參數
                #region 取得 EquID
                if (Request.QueryString["EquNo"] != null)
                {
                    labTilte.Text = "讀卡機參數列表";

                    sql = @" SELECT EquID FROM B01_EquData WHERE EquNo = ? ";
                    liSqlPara.Add("S:" + Request.QueryString["EquNo"]);
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                    if (dr.Read())
                        hideEquID.Value = dr.DataReader["EquID"].ToString();

                    ViewState["ParaTable"] = LoadData();
                }
                #endregion

                #region 取得 CtrlID
                if (Request.QueryString["CtrlNo"] != null)
                {
                    labTilte.Text = "控制器參數列表";

                    sql = @" SELECT CtrlID FROM B01_Controller WHERE CtrlNo = ? ";
                    liSqlPara.Add("S:" + Request.QueryString["CtrlNo"]);
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                    if (dr.Read())
                        hideCtrlID.Value = dr.DataReader["CtrlID"].ToString();

                    ViewState["ParaTable"] = LoadDataForControl();
                }
                #endregion

                #region 設定web title
                if (strTitle != null)
                {
                    if (strTitle == "Controller")
                    {
                        string strCtrlNo = Request.QueryString["CtrlNo"] as string;
                        this.Title = "控制器參數設定 - ";
                        if (strCtrlNo != null)
                        {
                            this.Title += oAcsDB.GetStrScalar(string.Format(@"
                                SELECT TOP 1 CtrlName FROM B01_Controller WHERE CtrlNo = '{0}'", 
                            strCtrlNo));
                        }
                    }
                    else if (strTitle == "Reader")
                    {
                        string strEquNo = Request.QueryString["EquNo"] as string;
                        this.Title = "讀卡機參數設定 - ";
                        if (strEquNo != null)
                        {
                            this.Title += oAcsDB.GetStrScalar(string.Format(@"
                                SELECT TOP 1 ReaderName FROM B01_Reader WHERE EquNo = '{0}'",
                            strEquNo));
                        }
                    }
                }
                #endregion
                #endregion

                ParaGridView.DataSource = (DataTable)ViewState["ParaTable"];
                ParaGridView.DataBind();
                ParaUpdataPanel.Update();
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];
                string[] strCheck;

                #region SettingCardExtData
                if (sFormTarget.IndexOf("ParaGridView_URLLink", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    KeepUIDataToTable();
                    ProcessTable = (DataTable)ViewState["ParaTable"];
                    string[] Arg = sFormArg.Split('/');
                    
                    #region FloorNameList
                    if (string.Compare(Arg[0], "FloorNameList") == 0)
                    {
                        strCheck = Arg[2].Split(',');

                        Arg[1] = Server.UrlDecode(Arg[1]);
                        Sa.Fun.GetStrL(ref Arg[1], "&", true);
                        for (int i = 0; i < ProcessTable.Rows.Count; i++)
                        {
                            string[] FloorNameList = Arg[1].Split(',');
                            if (string.Compare(ProcessTable.Rows[i]["EquParaID"].ToString(), Arg[0].ToString()) == 0)
                            {
                                ProcessTable.Rows[i]["ParaValue"] = FloorNameList.Length;
                            }
                            if (string.Compare(ProcessTable.Rows[i]["ParaName"].ToString(), "ElevCount") == 0
                             || string.Compare(ProcessTable.Rows[i]["ParaName"].ToString(), "ElevAlwysOpen") == 0
                             || string.Compare(ProcessTable.Rows[i]["ParaName"].ToString(), "ElevCtrlOnOpen") == 0)
                            {
                                ProcessTable.Rows[i]["FloorName"] = Arg[1].ToString();
                            }
                        }

                    }
                    #endregion
                    #region Normal Act
                    else
                    {
                        Arg = sFormArg.Split('&');
                        strCheck = Arg[2].Split(',');

                        for (int i = 0; i < ProcessTable.Rows.Count; i++)
                        {
                            if (string.Compare(ProcessTable.Rows[i]["EquParaID"].ToString(), Arg[0].ToString()) == 0)
                                ProcessTable.Rows[i]["ParaValue"] = Arg[1];
                        }
                    }
                    #endregion

                    ViewState["ParaTable"] = ProcessTable;

                    ParaGridView.DataSource = (DataTable)ViewState["ParaTable"];
                    ParaGridView.DataBind();
                    ParaUpdataPanel.Update();

                    #region 還原ParaGridView打勾的項目
                    if (strCheck[0] == "0")
                    {
                        Sa.Web.Fun.RunJavaScript(this, "document.getElementById('cbxHeader').checked = false;");
                    }
                    else
                    {
                        Sa.Web.Fun.RunJavaScript(this, "document.getElementById('cbxHeader').checked = true;");
                    }

                    foreach (GridViewRow gr in ParaGridView.Rows)
                    {
                        CheckBox cbx = gr.FindControl("cbx") as CheckBox;
                        cbx.Checked = (strCheck[gr.RowIndex + 1] == "0") ? false : true;
                    }
                    #endregion
                }

                // 清除 __EVENTTARGET、__EVENTARGUMENT 的值
                Sa.Web.Fun.RunJavaScript(this,
                @" theForm.__EVENTTARGET.value   = '' ;
                   theForm.__EVENTARGUMENT.value = '' ; ");
                #endregion
            }
        }
        #endregion

        #region ParaGridView_RowDataBound
        protected void ParaGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    int[] HeaderWidth = { 39, 279, 270 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    li_header.Text = Header_sw.ToString();
                    #endregion

                    break;
                #endregion

                #region DataRow
                case DataControlRowType.DataRow:
                    DataRowView oRow = (DataRowView)e.Row.DataItem;

                    #region 設定欄位寛度
                    int[] DataWidth = { 41, 263, 263 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 參數數值
                    Button ParaButton = (Button)e.Row.FindControl("B_ParaItem");
                    TextBox ParaTextBox = (TextBox)e.Row.FindControl("T_ParaItem");
                    DropDownList ParaDropDown = (DropDownList)e.Row.FindControl("D_ParaItem");
                    string Arguments = "";

                    switch (oRow["ParaUI"].ToString())
                    {
                        case "0":
                            #region 文字欄位
                            TextBox InputText = new TextBox();
                            ParaTextBox.Width = 230;
                            ParaButton.Visible = false;
                            ParaDropDown.Visible = false;
                            if (string.Compare(oRow.Row["ParaValue"].ToString(), "") != 0)
                                ParaTextBox.Text = oRow.Row["ParaValue"].ToString();
                            #endregion
                            break;
                        case "1":
                            #region 數字數值
                            TextBox InputInt = new TextBox();
                            ParaTextBox.Width = 230;
                            ParaButton.Visible = false;
                            ParaDropDown.Visible = false;
                            if (string.Compare(oRow.Row["ParaValue"].ToString(), "") != 0)
                                ParaTextBox.Text = oRow.Row["ParaValue"].ToString();
                            #endregion
                            break;
                        case "2":
                            #region 清單選項
                            if (string.Compare(oRow.Row["ValueOptions"].ToString(), "") != 0)
                            {
                                ParaDropDown.Width = 230;
                                ListItem Items;
                                string[] Obj = oRow.Row["ValueOptions"].ToString().Split('/');
                                for (int i = 0; i < Obj.Length; i++)
                                {
                                    Items = new ListItem();
                                    string[] Itemstr = Obj[i].ToString().Split(':');
                                    Items.Text = Itemstr[0];
                                    Items.Value = Itemstr[1];
                                    ParaDropDown.Items.Add(Items);
                                }
                                ParaButton.Visible = false;
                                ParaTextBox.Visible = false;
                                if (!oRow.Row["ParaValue"].ToString().Equals("") && string.Compare(oRow.Row["ParaValue"].ToString(), "") != 0)
                                {
                                    ParaDropDown.SelectedValue = oRow.Row["ParaValue"].ToString();
                                }
                                else
                                {
                                    ParaDropDown.SelectedValue = oRow.Row["DefaultValue"].ToString();
                                }
                            }

                            #endregion
                            break;
                        case "3":
                            #region 連結參考
                            if (string.Compare(oRow.Row["EditFormURL"].ToString(), "") != 0)
                            {
                                ParaButton.ID = "URLLink";
                                ParaButton.Width = 230;                               
                                ParaButton.Text = "設　　定";
                                ParaButton.Font.Size = 10;
                                ParaButton.Style.Add("margin", "2px 2px 2px 2px");
                                ParaButton.Style.Add("padding", "2px 10px");
                                Arguments = oRow.Row["EquID"].ToString() + "&" + oRow.Row["EquParaID"].ToString() + "&" + oRow.Row["ParaValue"].ToString();

                                if (string.IsNullOrEmpty(oRow.Row["FloorName"].ToString()))
                                    ParaButton.Attributes["onClick"] = "CallURL('" + oRow.Row["EditFormURL"].ToString() + "','" + Arguments + "','" + oRow.Row["FormSize"].ToString() + "','" + ParaButton.ClientID + "'); return false;";
                                else
                                    ParaButton.Attributes["onClick"] = " CallURLExtData('" + oRow.Row["EditFormURL"].ToString() + "','" + Arguments + "','" + oRow.Row["FloorName"].ToString() + "','" + oRow.Row["FormSize"].ToString() + "','" + ParaButton.ClientID + "'); return false;";

                                ParaTextBox.Visible = false;
                                ParaDropDown.Visible = false;
                                oScriptManager.RegisterAsyncPostBackControl(ParaButton);
                            }
                            else
                            {
                                e.Row.Cells[2].Text = "無資料";
                            }
                            break;
                            #endregion
                        default:
                            e.Row.Cells[2].Text = "無資料";
                            break;
                    }
                    e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Center;
                    #endregion

                    #endregion

                    break;
                #endregion
            }
        }
        #endregion

        #region popB_Refresh_Click
        protected void popB_Refresh_Click(object sender,EventArgs e)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "", wheresql = "", actsql = "";
            int Decide;
            bool Floorflag = false;
            List<string> liSqlPara = new List<string>();
            List<string> ActliSqlPara = new List<string>();
            DateTime Time = DateTime.Now;
            DataTable CheckDb = new DataTable();
            KeepUIDataToTable();

            ProcessTable = null;
            ProcessTable = (DataTable)ViewState["ParaTable"];

            List<string> liSetPara = new List<string>();

            try
            {

                #region 取得有打勾的項目
                foreach (GridViewRow gr in ParaGridView.Rows)
                {
                    CheckBox cbx = gr.FindControl("cbx") as CheckBox;

                    if (cbx.Checked == true)
                    {
                        // 取得需要更新的項目，去掉 'ParaGridView_cbx_' 得到的數字加 1 就得到需要更新的Seq
                        string strIndex = cbx.ClientID.ToString().Replace("ParaGridView_cbx_", "");
                        strIndex = (int.Parse(strIndex) + 1).ToString();

                        liSetPara.Add(strIndex);
                    }
                }
                #endregion

                if (Session["UserID"] != null)
                {
                    if (liSetPara.Count == 0)
                    {
                        objRet.result = false;
                        objRet.message = "沒有勾選任何要重送的項目。";
                    }

                    if (objRet.result) objRet = CheckData(ProcessTable);

                    if (objRet.result)
                    {
                        for (int i = 0; i < ProcessTable.Rows.Count; i++)
                        {
                            string strSeq = ProcessTable.Rows[i]["Seq"].ToString();

                            if (liSetPara.Contains(strSeq))
                            {
                                #region 判斷動作
                                string strEquID = ProcessTable.Rows[i]["EquID"].ToString();
                                string strEquParaID = ProcessTable.Rows[i]["EquParaID"].ToString();

                                liSqlPara.Clear();
                                wheresql = "";
                                sql = @" SELECT * FROM B01_EquParaData ";

                                if (!string.IsNullOrEmpty(hideEquID.Value))
                                {
                                    if (wheresql != "") wheresql += " AND ";
                                    wheresql += " EquID = ? ";
                                    liSqlPara.Add("S:" + strEquID);
                                }

                                if (!string.IsNullOrEmpty(ProcessTable.Rows[i]["EquParaID"].ToString()))
                                {
                                    if (wheresql != "") wheresql += " AND ";
                                    wheresql += " EquParaID = ? ";
                                    liSqlPara.Add("S:" + ProcessTable.Rows[i]["EquParaID"].ToString());
                                }
                                if (wheresql != "")
                                    sql += " WHERE ";
                                sql += wheresql;
                                Decide = oAcsDB.GetIntScalar(sql, liSqlPara);
                                //oAcsDB.GetDataTable("CheckParaTable", sql, liSqlPara, out CheckDb);
                                #endregion

                                #region Process ParaData
                                if (Decide <= 0)
                                {
                                    #region Insert
                                    actsql += @" 
                                        INSERT INTO B01_EquParaData 
                                        (EquID,EquParaID,ParaValue,UpdateUserID,UpdateTime) 
                                        VALUES (?, ?, ?, ?, ?) ";
                                    ActliSqlPara.Add("I:" + strEquID);
                                    ActliSqlPara.Add("I:" + strEquParaID);
                                    ActliSqlPara.Add("S:" + ProcessTable.Rows[i]["ParaValue"].ToString());
                                    ActliSqlPara.Add("S:" + hideUserID.Value);
                                    ActliSqlPara.Add("D:" + Time.ToString());
                                    #endregion
                                }
                                else
                                {
                                    #region Update

                                    actsql += @" UPDATE B01_EquParaData 
                                         SET ParaValue = ?, IsReSend = '1', ErrCnt = '0', OpStatus='',
                                         UpdateUserID = ?, UpdateTime = ?
                                         WHERE EquID = ? AND EquParaID = ? ";
                                    ActliSqlPara.Add("S:" + ProcessTable.Rows[i]["ParaValue"].ToString());
                                    ActliSqlPara.Add("S:" + hideUserID.Value);
                                    ActliSqlPara.Add("D:" + Time.ToString());
                                    ActliSqlPara.Add("I:" + strEquID);
                                    ActliSqlPara.Add("I:" + strEquParaID);

                                    #endregion
                                }
                                #endregion

                                #region Process FloorName

                                string strFloorName = ProcessTable.Rows[i]["FloorName"].ToString();
                                if (!Floorflag && !string.IsNullOrEmpty(ProcessTable.Rows[i]["FloorName"].ToString()))
                                {
                                    Floorflag = true;
                                    string[] FloorNameList = ProcessTable.Rows[i]["FloorName"].ToString().Split(',');
                                    actsql += @" DELETE FROM B01_ElevatorFloor WHERE EquID = ? ";
                                    ActliSqlPara.Add("I:" + strEquID);

                                    for (int k = 0; k < FloorNameList.Length; k++)
                                    {
                                        string[] ItemArray = FloorNameList[k].ToString().Split(':');
                                        actsql += @" INSERT INTO B01_ElevatorFloor(EquID,IOIndex,FloorName)
                                         VALUES (?, ?, ?) ";
                                        ActliSqlPara.Add("I:" + strEquID);
                                        ActliSqlPara.Add("I:" + ItemArray[0].ToString());
                                        ActliSqlPara.Add("S:" + ItemArray[1].ToString());
                                    }
                                }
                                #endregion
                            }
                        }

                        int intRet = -1;
                        intRet = oAcsDB.SqlCommandExecute(actsql, ActliSqlPara);

                        // 僅用於控制器參數的部份
                        // CtrlNo所屬的讀卡機 & 設備若有多筆，取最舊那筆，
                        // 將其參數值複製給其他所屬的讀卡機，使其他讀卡機無需重送
                        if (!string.IsNullOrEmpty(hideCtrlID.Value))
                        {
                            CopyParaToOtherReader(hideCtrlID.Value);
                        }

                        if (intRet > -1)
                        {
                            Sa.Web.Fun.RunJavaScript(this, "ShowDialog('message', 'General', '重新傳送完成！'); ");
                        }
                        else
                        {
                            Sa.Web.Fun.RunJavaScript(this, "ShowDialog('alert', 'Error', '重新傳送失敗！'); ");
                        }

                        //Sa.Web.Fun.RunJavaScript(this, "alert('重新傳送完成。'); window.close();");
                    }
                    else
                    {
                        Sa.Web.Fun.RunJavaScript(this, "ShowDialog('alert', 'Error', '" + objRet.message + "'); ");
                        //Sa.Web.Fun.RunJavaScript(this, "alert('" + objRet.message + "')");
                    }
                }
                else
                {
                    Sa.Web.Fun.RunJavaScript(this, "ShowDialog('alert', 'Failed', '更新參數失敗，請再來一次。'); ");
                    //Sa.Web.Fun.RunJavaScript(this, "alert('更新參數失敗，請再來一次。')");
                }
            }
            catch (Exception ex)
            {
                Sa.Web.Fun.RunJavaScript(this, "ShowDialog('alert', 'Failed', '更新參數失敗。" + ex.Message.ToString() + "'); ");
                //Sa.Web.Fun.RunJavaScript(this, "alert('更新參數失敗。'" + ex.Message.ToString() + ")");
            }
        }
        #endregion

        #region popB_Save_Click
        protected void popB_Save_Click(object sender, EventArgs e)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "", wheresql = "", actsql = "";
            int Decide;
            bool Floorflag = false;
            List<string> liSqlPara = new List<string>();
            List<string> ActliSqlPara = new List<string>();
            DateTime Time = DateTime.Now;
            DataTable CheckDb = new DataTable();
            KeepUIDataToTable();

            ProcessTable = null;
            ProcessTable = (DataTable)ViewState["ParaTable"];

            objRet = CheckData(ProcessTable);

            try
            {
                if (Session["UserID"] != null)
                {
                    if (objRet.result)
                    {
                        for (int i = 0; i < ProcessTable.Rows.Count; i++)
                        {
                            #region 判斷動作
                            liSqlPara.Clear();
                            wheresql = "";
                            sql = @" SELECT DISTINCT EquID FROM B01_EquParaData ";

                            // 判斷是讀卡機參數或是控制器參數
                            if (!string.IsNullOrEmpty(hideEquID.Value))
                            {
                                if (wheresql != "") wheresql += " AND ";
                                wheresql += " EquID = ? ";
                                liSqlPara.Add("S:" + hideEquID.Value);
                            }
                            else if (!string.IsNullOrEmpty(hideCtrlID.Value))
                            {
                                if (wheresql != "") wheresql += " AND ";
                                wheresql += @" EquID = 
                            (
                                SELECT TOP 1 ED.EquID FROM B01_EquData ED 
                                INNER JOIN B01_Reader RD ON RD.EquNo = ED.EquNo 
                                INNER JOIN B01_Controller CR ON CR.CtrlID = RD.CtrlID 
                                WHERE CR.CtrlID = ? 
                                ORDER BY CR.CreateTime 
                            ) ";
                                liSqlPara.Add("S:" + hideCtrlID.Value);
                            }

                            if (!string.IsNullOrEmpty(ProcessTable.Rows[i]["EquParaID"].ToString()))
                            {
                                if (wheresql != "") wheresql += " AND ";
                                wheresql += " EquParaID = ? ";
                                liSqlPara.Add("S:" + ProcessTable.Rows[i]["EquParaID"].ToString());
                            }

                            if (wheresql != "") sql += " WHERE ";
                            sql += wheresql;

                            Decide = oAcsDB.GetIntScalar(sql, liSqlPara);
                            //oAcsDB.GetDataTable("CheckParaTable", sql, liSqlPara, out CheckDb);
                            #endregion

                            #region Process ParaData
                            if (Decide <= 0)
                            {
                                #region Insert[舊的暫不用]
                                //actsql += @" INSERT INTO B01_EquParaData(EquID,EquParaID,ParaValue,UpdateUserID,UpdateTime)
                                //             VALUES (?, ?, ?, ?, ?) ";
                                //ActliSqlPara.Add("I:" + hideEquID.Value);
                                //ActliSqlPara.Add("I:" + ProcessTable.Rows[i]["EquParaID"].ToString());
                                //ActliSqlPara.Add("S:" + ProcessTable.Rows[i]["ParaValue"].ToString());
                                //ActliSqlPara.Add("S:" + hideUserID.Value);
                                //ActliSqlPara.Add("D:" + Time.ToString());
                                #endregion

                                #region Insert
                                if (Request.QueryString["EquNo"] != null)
                                {
                                    actsql += @" 
                                        INSERT INTO B01_EquParaData 
                                            (EquID,EquParaID,ParaValue,UpdateUserID,UpdateTime) 
                                        VALUES (?, ?, ?, ?, ?) ";
                                    ActliSqlPara.Add("I:" + hideEquID.Value);
                                    ActliSqlPara.Add("I:" + ProcessTable.Rows[i]["EquParaID"].ToString());
                                    ActliSqlPara.Add("S:" + ProcessTable.Rows[i]["ParaValue"].ToString());
                                    ActliSqlPara.Add("S:" + hideUserID.Value);
                                    ActliSqlPara.Add("D:" + Time.ToString());
                                }
                                else if (Request.QueryString["CtrlNo"] != null)
                                {
                                    #region 新增控制器參數
                                    string strSQL = string.Format(@"
                                        SELECT ED.EquID FROM B01_EquData ED 
		                                INNER JOIN B01_Reader RD ON RD.EquNo = ED.EquNo 
		                                INNER JOIN B01_Controller CR ON CR.CtrlID = RD.CtrlID 
		                                WHERE CR.CtrlID={0}", hideCtrlID.Value);

                                    Sa.DB.DBReader dr = null;
                                    oAcsDB.GetDataReader(strSQL, out dr);

                                    if (dr != null)
                                    {
                                        if (dr.HasRows)
                                        {
                                            while (dr.Read())
                                            {
                                                actsql += @" 
                                            INSERT INTO B01_EquParaData 
                                                (EquID,EquParaID,ParaValue,UpdateUserID,UpdateTime) 
                                            VALUES (?, ?, ?, ?, ?) ";
                                                ActliSqlPara.Add("I:" + dr.ToInt32("EquID"));
                                                ActliSqlPara.Add("I:" + ProcessTable.Rows[i]["EquParaID"].ToString());
                                                ActliSqlPara.Add("S:" + ProcessTable.Rows[i]["ParaValue"].ToString());
                                                ActliSqlPara.Add("S:" + hideUserID.Value);
                                                ActliSqlPara.Add("D:" + Time.ToString());
                                            }
                                        }
                                    }

                                    #endregion
                                }
                                #endregion
                            }
                            else
                            {
                                #region Update
                                if (Request.QueryString["EquNo"] != null)
                                {
                                    if (ProcessTable.Rows[i]["ParaValue"].ToString() !=
                                        ProcessTable.Rows[i]["ParaValueO"].ToString())
                                    {
                                        actsql += @" 
                                            UPDATE B01_EquParaData 
                                                SET ParaValue = ?, IsReSend = '1', ErrCnt = '0', OpStatus='',
                                                UpdateUserID = ?, UpdateTime = ?
                                            WHERE EquParaID = ? AND EquID = ? ";
                                        ActliSqlPara.Add("S:" + ProcessTable.Rows[i]["ParaValue"].ToString());
                                        ActliSqlPara.Add("S:" + hideUserID.Value);
                                        ActliSqlPara.Add("D:" + Time.ToString());
                                        ActliSqlPara.Add("I:" + ProcessTable.Rows[i]["EquParaID"].ToString());
                                        ActliSqlPara.Add("I:" + hideEquID.Value);
                                    }
                                }
                                else if (Request.QueryString["CtrlNo"] != null)
                                {
                                    if (ProcessTable.Rows[i]["ParaValue"].ToString() !=
                                        ProcessTable.Rows[i]["ParaValueO"].ToString())
                                    {
                                        actsql += @" 
                                            UPDATE B01_EquParaData 
                                                SET ParaValue = ?, IsReSend = '1', ErrCnt = '0', OpStatus='',
                                                UpdateUserID = ?, UpdateTime = ?
                                            WHERE EquParaID = ? 
                                            AND EquID IN 
                                            (
		                                        SELECT ED.EquID FROM B01_EquData ED 
		                                        INNER JOIN B01_Reader RD ON RD.EquNo = ED.EquNo 
		                                        INNER JOIN B01_Controller CR ON CR.CtrlID = RD.CtrlID 
		                                        WHERE CR.CtrlID = ? 
                                            )";
                                        ActliSqlPara.Add("S:" + ProcessTable.Rows[i]["ParaValue"].ToString());
                                        ActliSqlPara.Add("S:" + hideUserID.Value);
                                        ActliSqlPara.Add("D:" + Time.ToString());
                                        ActliSqlPara.Add("I:" + ProcessTable.Rows[i]["EquParaID"].ToString());
                                        ActliSqlPara.Add("I:" + hideCtrlID.Value);
                                    }
                                }
                                #endregion
                            }

                            #endregion

                            #region Process FloorName 
                            // 樓層數量及名稱這個參數只為控制器參數

                            if (!Floorflag && !string.IsNullOrEmpty(ProcessTable.Rows[i]["FloorName"].ToString()))
                            {
                                Floorflag = true;
                                string[] FloorNameList = ProcessTable.Rows[i]["FloorName"].ToString().Split(',');
                                actsql += @" 
                                    DELETE FROM B01_ElevatorFloor 
                                    WHERE EquID IN 
                                    (
		                                SELECT ED.EquID FROM B01_EquData ED 
		                                INNER JOIN B01_Reader RD ON RD.EquNo = ED.EquNo 
		                                INNER JOIN B01_Controller CR ON CR.CtrlID = RD.CtrlID 
		                                WHERE CR.CtrlID = ?  
                                    )";
                                ActliSqlPara.Add("I:" + hideCtrlID.Value);

                                for (int k = 0; k < FloorNameList.Length; k++)
                                {
                                    string[] ItemArray = FloorNameList[k].ToString().Split(':');
                                    actsql += string.Format(@" 
                                        INSERT INTO B01_ElevatorFloor(EquID,IOIndex,FloorName)
                                        SELECT ED.EquID, '{0}', '{1}'  
                                        FROM B01_EquData ED 
		                                INNER JOIN B01_Reader RD ON RD.EquNo = ED.EquNo 
		                                INNER JOIN B01_Controller CR ON CR.CtrlID = RD.CtrlID 
		                                WHERE CR.CtrlID = ? ",
                                        ItemArray[0].ToString(), ItemArray[1].ToString());
                                    ActliSqlPara.Add("I:" + hideCtrlID.Value);

                                }
                            }
                            #endregion
                        }

                        int intRet = oAcsDB.SqlCommandExecute(actsql, ActliSqlPara);

                        if (intRet > -1)
                        {
                            // 僅用於控制器參數的部份
                            // CtrlNo所屬的讀卡機 & 設備若有多筆，取最舊那筆，
                            // 將其參數值複製給其他所屬的讀卡機，使其他讀卡機無需重送
                            if (!string.IsNullOrEmpty(hideCtrlID.Value))
                            {
                                CopyParaToOtherReader(hideCtrlID.Value);
                            }
                        }

                        Sa.Web.Fun.RunJavaScript(this, "ShowDialog('message', 'General', '更新參數完成！'); ");
                        //Sa.Web.Fun.RunJavaScript(this, "alert('更新參數完成。'); window.close();");
                    }
                    else
                    {
                        Sa.Web.Fun.RunJavaScript(this, "ShowDialog('alert', 'Error', '" + objRet.message + "'); ");
                        //Sa.Web.Fun.RunJavaScript(this, "alert('" + objRet.message + "')");
                    }
                }
                else
                {
                    Sa.Web.Fun.RunJavaScript(this, "ShowDialog('alert', 'Failed', '更新參數失敗，請再來一次。'); ");
                    //Sa.Web.Fun.RunJavaScript(this, "alert('更新參數失敗，請再來一次。')");
                }
            }
            catch (Exception ex)
            {
                Sa.Web.Fun.RunJavaScript(this, "ShowDialog('alert', 'Failed', '更新參數失敗。" + ex.Message.ToString() + "'); ");

               // Sa.Web.Fun.RunJavaScript(this, "alert('更新參數失敗。'" + ex.Message.ToString() + ")");
            }

            

            //if (oAcsDB.SqlCommandExecute(sql, liSqlPara) != -1)
            //Response.Write("<script>window.close();</script>");
        }
        #endregion

        #region popB_Cancel_Click
        protected void popB_Cancel_Click(object sender, EventArgs e)
        {
            Sa.Web.Fun.RunJavaScript(this, "window.close();");
        }
        #endregion

        #endregion

        #region Method

        #region LoadData
        public DataTable LoadData()
        {
            DataTable Process = new DataTable();
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = @" SELECT 
                     ROW_NUMBER()OVER(ORDER BY ParaDef.EquParaID) AS Seq,
                     EquData.EquID, ParaDef.EquParaID,
                     ParaDef.ParaName, ParaDef.ParaDesc, ParaDef.InputType AS ParaUI,
                     ParaDef.ValueOptions, ParaDef.EditFormURL, ParaDef.FormSize,
                     ParaDef.MinValue, ParaDef.MaxValue, ParaDef.DefaultValue,
                     ISNULL(CASE WHEN ParaName = 'ElevCtrlOnOpen' AND ParaValue IS NULL THEN 
						'FFFFFFFFFFFF'
					 WHEN ParaName = 'CardTransferMode' AND EquData.EquModel='SCM320.Elev' AND ParaValue IS NULL THEN 
						'000108'
					WHEN ParaName = 'CardTransferMode' AND EquData.EquModel='SCM320' AND ParaValue IS NULL THEN 
						'000008'
                     ELSE 
						ParaData.ParaValue END,'') AS ParaValue,
                    CASE WHEN ParaName = 'ElevCtrlOnOpen' AND ParaValue IS NULL THEN 
						'FFFFFFFFFFFF'
					 WHEN ParaName = 'CardTransferMode' AND EquData.EquModel='SCM320.Elev' AND ParaValue IS NULL THEN 
						'000108'
					WHEN ParaName = 'CardTransferMode' AND EquData.EquModel='SCM320' AND ParaValue IS NULL THEN 
						'000008'
                     ELSE 
						ParaData.ParaValue End as ParaValueO, 
                     ParaData.M_ParaValue,
                     ParaData.UpdateTime, ParaData.SendTime, ParaData.CompleteTime,
                     '' AS ParaStatus,
                     Replace(STUFF( (SELECT ',' + CAST( IOIndex as NVARCHAR ) + ':' + CAST( FloorName as NVARCHAR ) 
                             FROM B01_ElevatorFloor 
                             WHERE EquID = EquData.EquID AND (ParaDef.ParaName = 'ElevCount' OR ParaDef.ParaName = 'ElevAlwysOpen' OR ParaDef.ParaName = 'ElevCtrlOnOpen') FOR XML PATH(''))
                     ,1,1,''),'&amp;','&') AS FloorName 
                     FROM B01_EquData AS EquData 
                     LEFT JOIN B01_EquParaDef AS ParaDef ON ParaDef.EquModel = EquData.EquModel
                     LEFT JOIN B01_EquParaData AS ParaData ON ParaData.EquID = EquData.EquID AND ParaData.EquParaID = ParaDef.EquParaID
                     WHERE ParaDef.ParaType = ? AND EquData.EquNo = ?  
                     ORDER BY ParaDef.EquParaID ";

            liSqlPara.Add("S:" + Request.QueryString["ParaType"]);
            liSqlPara.Add("S:" + Request.QueryString["EquNo"]);
            oAcsDB.GetDataTable("EquData", sql, liSqlPara, out Process);

            #endregion

            return Process;
        }
        #endregion

        #region LoadDataForControl
        public DataTable LoadDataForControl()
        {
            DataTable Process = new DataTable();
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();

            #region Process String

            // 1. 加入控制器參數的判斷 (ParaDef.ParaType = ? )
            // 2. 從 CtrlID 取得 EquNo 其一做為修改的標準，用 CreateTime 最舊的那筆。

            sql = @" 
                SELECT 
                    ROW_NUMBER() OVER(ORDER BY ParaDef.EquParaID) AS Seq,
                    EquData.EquID, ParaDef.EquParaID,
                    ParaDef.ParaName, ParaDef.ParaDesc, ParaDef.InputType AS ParaUI,
                    ParaDef.ValueOptions, ParaDef.EditFormURL, ParaDef.FormSize,
                    ParaDef.MinValue, ParaDef.MaxValue, ParaDef.DefaultValue,
                    ISNULL(
                        CASE 
                        WHEN ParaName='ElevCtrlOnOpen' AND ParaValue IS NULL THEN 'FFFFFFFFFFFF'
					    WHEN ParaName='CardTransferMode' AND EquData.EquModel='SCM320.Elev' 
                            AND ParaValue IS NULL THEN '000108'
				        WHEN ParaName='CardTransferMode' AND EquData.EquModel='SCM320' 
                            AND ParaValue IS NULL THEN '000008'
                        ELSE 
					        ParaData.ParaValue END, ''
                    ) AS ParaValue,
                    CASE 
                    WHEN ParaName='ElevCtrlOnOpen' AND ParaValue IS NULL THEN 'FFFFFFFFFFFF'
					WHEN ParaName='CardTransferMode' AND EquData.EquModel='SCM320.Elev' 
                        AND ParaValue IS NULL THEN '000108'
				    WHEN ParaName='CardTransferMode' AND EquData.EquModel='SCM320' 
                        AND ParaValue IS NULL THEN '000008'
                    ELSE 
					ParaData.ParaValue End AS ParaValueO, 
                    ParaData.M_ParaValue,
                    ParaData.UpdateTime, ParaData.SendTime, ParaData.CompleteTime,
                    '' AS ParaStatus,
                    Replace(STUFF(
                        (SELECT ',' + CAST( IOIndex as NVARCHAR ) + ':' + CAST( FloorName as NVARCHAR ) 
                            FROM B01_ElevatorFloor 
                            WHERE EquID = EquData.EquID AND (ParaDef.ParaName = 'ElevCount' OR ParaDef.ParaName = 'ElevAlwysOpen' OR ParaDef.ParaName = 'ElevCtrlOnOpen') FOR XML PATH(''))
                    ,1,1,''),'&amp;','&') AS FloorName 
                FROM B01_EquData AS EquData 
                LEFT JOIN B01_EquParaDef AS ParaDef ON ParaDef.EquModel = EquData.EquModel
                LEFT JOIN B01_EquParaData AS ParaData ON ParaData.EquID = EquData.EquID AND ParaData.EquParaID = ParaDef.EquParaID
                WHERE ParaDef.ParaType = ? 
                AND EquData.EquNo = 
                (
	                SELECT TOP 1 ED.EquNo FROM B01_EquData ED 
	                INNER JOIN B01_Reader RD ON RD.EquNo = ED.EquNo 
	                INNER JOIN B01_Controller CR ON CR.CtrlID = RD.CtrlID 
                    WHERE CR.CtrlNo = ? 
                    ORDER BY CR.CreateTime 
                )
                ORDER BY ParaDef.EquParaID ";

            liSqlPara.Add("S:" + Request.QueryString["ParaType"]);
            liSqlPara.Add("S:" + Request.QueryString["CtrlNo"]);
            oAcsDB.GetDataTable("EquData", sql, liSqlPara, out Process);

            #endregion

            return Process;
        }
        #endregion

        #region CheckData
        protected static Pub.MessageObject CheckData(DataTable ParaTable)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();
            int tempint;
            DataTable ParaInfo = new DataTable();
            Sa.DB.DBReader dr;

            for (int i = 0; i < ParaTable.Rows.Count; i++)
            {
                liSqlPara.Clear();
                #region Get Para Info
                sql = " SELECT * FROM B01_EquParaDef WHERE EquParaID = ? ";
                liSqlPara.Add("S:" + ParaTable.Rows[i]["EquParaID"].ToString());
                oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                if (dr.Read())
                {
                    switch (dr.DataReader["InputType"].ToString())
                    {
                        case "0":
                            #region 文字欄位
                            if (ParaTable.Rows[i]["ParaValue"].ToString().Length > 1024)
                            {
                                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                                objRet.result = false;
                                objRet.message += dr.DataReader["ParaDesc"].ToString() + " 字數超過上限";
                            }
                            #endregion
                            break;
                        case "1":
                            #region 數字數值
                            if (!string.IsNullOrEmpty(ParaTable.Rows[i]["ParaValue"].ToString()))
                            {
                                if (!int.TryParse(ParaTable.Rows[i]["ParaValue"].ToString(), out tempint))
                                {
                                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                                    objRet.result = false;
                                    objRet.message += dr.DataReader["ParaDesc"].ToString() + " 必需為數字";
                                }
                                if (ParaTable.Rows[i]["ParaValue"].ToString().Length > 1024)
                                {
                                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "<br />";
                                    objRet.result = false;
                                    objRet.message += dr.DataReader["ParaDesc"].ToString() + " 字數超過上限";
                                }
                                if (int.TryParse(ParaTable.Rows[i]["ParaValue"].ToString(), out tempint))
                                {
                                    int minvalue = int.Parse(dr.DataReader["MinValue"].ToString());
                                    int maxvalue = int.Parse(dr.DataReader["MaxValue"].ToString());
                                    if (tempint < minvalue || tempint > maxvalue)
                                    {
                                        objRet.result = false;
                                        objRet.message += dr.DataReader["ParaDesc"].ToString() + " 需介於 " + minvalue + " ~ " + maxvalue + " 之間。";
                                    }
                                }
                            }
                            #endregion
                            break;
                    }
                }
                else
                {
                    objRet.result = false;
                    objRet.message = "發生錯誤，系統中查無參數。";
                    break;
                }
                #endregion
            }
            objRet.act = "CheckData";
            return objRet;
        }
        #endregion

        #region KeepUIDataToTable
        public void KeepUIDataToTable()
        {
            TextBox TBOX;
            DropDownList DDL;
            ProcessTable = (DataTable)ViewState["ParaTable"];

            for (int i = 0; i < ParaGridView.Rows.Count; i++)
            {
                if (ProcessTable.Rows[i]["ParaUI"].ToString() == "0" || ProcessTable.Rows[i]["ParaUI"].ToString() == "1")
                {
                    TBOX = (TextBox)ParaGridView.Rows[i].Cells[2].FindControl("T_ParaItem");
                    ProcessTable.Rows[i]["ParaValue"] = TBOX.Text.ToString();
                }
                else if (ProcessTable.Rows[i]["ParaUI"].ToString() == "2")
                {
                    DDL = (DropDownList)ParaGridView.Rows[i].Cells[2].FindControl("D_ParaItem");
                    ProcessTable.Rows[i]["ParaValue"] = DDL.SelectedValue;
                }
            }
            ViewState["ParaTable"] = ProcessTable;
        }
        #endregion

        private void CopyParaToOtherReader(string strCtrlID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            #region 更新控制器參數，若該控制器多於二台讀卡機，於更新時，自動將其他所屬讀卡機&設備更新到不需要重送，只送一台的參數即可
            string strSQL = string.Format(@"
                UPDATE B01_EquParaData 
                SET paraValue=SV.P1, M_ParaValue=SV.P1, isReSend=0, OpStatus='Setted', ParaValueOther=''  
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
                AND B01_EquParaData.EquID IN
                (
                    SELECT EquID FROM B01_EquData WHERE EquID != SV.FirstEquID 
                    AND EquNo IN (SELECT EquNo FROM B01_Reader WHERE CtrlID = {0})
                ) ", strCtrlID);

            oAcsDB.SqlCommandExecute(strSQL);
            #endregion
        }
        #endregion
    }
}