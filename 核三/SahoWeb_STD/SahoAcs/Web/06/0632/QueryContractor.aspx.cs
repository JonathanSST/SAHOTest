using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using System.Threading;

namespace SahoAcs
{
    public partial class QueryContractor : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 30;
        string sPsnID;
        private string strLoadCompareTime = "";     // 報到比對時間
        private string strLoadRegEquGroup = "";     // 報到用設備群組
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(QueryButton);
            oScriptManager.RegisterAsyncPostBackControl(btnSetup);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("QueryContractor", "QueryContractor.js");//加入同一頁面所需的JavaScript檔案

            #region 註冊主頁Button動作
            QueryButton.Attributes["onClick"] = "SelectState(); return false;";
            btnSetup.Attributes["onClick"] = "SetValue(); return false;";
            ExportButton.Attributes["onClick"] = "ExportQuery(); return false;";
            #endregion

            this.MainGridView.PageSize = _pagesize;
            #endregion

            #region Check Person Data
            this.sPsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
            if (!this.sPsnID.Equals(""))
            {
                this.ShowPsnInfo1.Visible = false;
                this.ShowPsnInfo2.Visible = false;
            }
            #endregion

            // 讀取比對時間
            strLoadCompareTime = LoadCompareTime();

            // 讀取報到用設備群組
            strLoadRegEquGroup = LoadRegEquGroup();

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion

                #region Give viewstate value
                ViewState["query_CardTimeSDate"] = "";
                ViewState["query_CardTimeEDate"] = "";
                ViewState["query_CardNo_PsnName"] = "";
                ViewState["dropCompany"] = "";
                ViewState["ddlStatus"] = "";
                ViewState["ddlDetail"] = "";
                ViewState["SortExpression"] = "CardTime";
                ViewState["SortDire"] = "DESC";
                ViewState["query_sMode"] = "Normal";
                ViewState["FlagExport"] = "";
                #endregion

                #region 給開始、結束時間預設值
                DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
                DateTime dtLastCardTime = oAcsDB.GetDateTimeScalar(@"
                    SELECT MAX(CardTime) FROM B01_CardLog WHERE CardTime <= GETDATE()");

                if (dtLastCardTime == DateTime.MinValue)
                {
                    Calendar_CardTimeSDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 00:00:00";
                    Calendar_CardTimeEDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 23:59:59";
                }
                else
                {
                    Calendar_CardTimeSDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 00:00:00";
                    Calendar_CardTimeEDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 23:59:59";
                }

                ViewState["query_CardTimeSDate"] = Calendar_CardTimeSDate.DateValue;
                ViewState["query_CardTimeEDate"] = Calendar_CardTimeEDate.DateValue;
                #endregion

                #region 處理各個DropDownList
                // 給 dropCompany 派值
                DropBind(this.dropCompany, "OrgName", "OrgName", @"SELECT OrgName FROM B01_OrgData WHERE OrgClass = 'Company' ORDER BY OrgName");

                #region 給 ddlHour 、 ddlMin 派值
                ddlHour.Items.Clear();
                ddlMin.Items.Clear();

                for (int i = 0; i < 24; i++)
                {
                    ListItem liItem1 = new ListItem();

                    liItem1.Text = i.ToString().PadLeft(2, '0');
                    liItem1.Value = liItem1.Text;

                    this.ddlHour.Items.Add(liItem1);

                    liItem1 = null;
                }

                for (int i = 0; i < 60; i++)
                {
                    ListItem liItem2 = new ListItem();

                    liItem2.Text = i.ToString().PadLeft(2, '0');
                    liItem2.Value = liItem2.Text;

                    this.ddlMin.Items.Add(liItem2);

                    liItem2 = null;
                }
                #endregion

                // 給 指定報到群組 labRegEquGroup 派值
                DropBind(this.ddlRegEquGroup, "EquGrpID", "EquGrpName", 
                    @"SELECT EquGrpID, EquGrpName FROM B01_EquGroup WHERE EquGrpName <> '' ORDER BY EquGrpID"); 
                #endregion

                #region 依比對時間設定 ddlHour、ddlMin
                string[] strCompateTime = strLoadCompareTime.Split(':');
                ddlHour.Items.FindByValue(strCompateTime[0]).Selected = true;
                ddlMin.Items.FindByValue(strCompateTime[1]).Selected = true;
                #endregion

                // 依 指定報到群組 設定 ddlRegEquGroup 
                ddlRegEquGroup.Items.FindByValue(strLoadRegEquGroup).Selected = true;

                // EmptyCondition();       // 做一個空的datatable給gridview秀查無資料

                GiveConditionToQuery();     // 設定條件後查詢資料
            }
            else
            {
                bool IsOK = true;

                #region 判斷輸入欄位條件
                if (!IsDate(Calendar_CardTimeSDate.DateValue.Trim()))
                {
                    Sa.Web.Fun.RunJavaScript(this.Page, "alert('Start Time Format is error!!!');");
                    IsOK = false;
                }

                if (!IsDate(Calendar_CardTimeEDate.DateValue.Trim()))
                {
                    Sa.Web.Fun.RunJavaScript(this.Page, "alert('End Time Format is error!!!');");
                    IsOK = false;
                }
                #endregion

                if (IsOK)
                {
                    string sFormTarget = Request.Form["__EVENTTARGET"];
                    string sFormArg = Request.Form["__EVENTARGUMENT"];

                    if (sFormTarget == this.QueryButton.ClientID)
                    {
                        // 設定條件後查詢資料
                        GiveConditionToQuery();
                    }
                    else if (sFormTarget == this.ExportButton.ClientID)
                    {
                        Query(ViewState["query_sMode"].ToString(), true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    }
                    else if (sFormTarget == btnSetup.ClientID)
                    {
                        // 更新報到時間和報到設備群組的設備參數資料
                        UpdateRegTimeAndEquGroup();

                        Query(ViewState["query_sMode"].ToString(), false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    }
                    else
                    {
                        // 不是按下 MainGridView 上方 Title 做排序時
                        if (!sFormArg.Contains("Sort"))
                        {
                            Query(ViewState["query_sMode"].ToString(), false,
                                ViewState["SortExpression"].ToString(),
                                ViewState["SortDire"].ToString());
                        }
                    }

                    // 清除 __EVENTTARGET、__EVENTARGUMENT 的值
                    Sa.Web.Fun.RunJavaScript(this.Page,
                        @" theForm.__EVENTTARGET.value   = '' ;
                       theForm.__EVENTARGUMENT.value = '' ; ");
                }

                Sa.Web.Fun.RunJavaScript(this.Page, "$.unblockUI();");
            }
        }

        // 設定條件後查詢資料
        private void GiveConditionToQuery()
        {
            ViewState["query_CardTimeSDate"] = Calendar_CardTimeSDate.DateValue.Trim();
            ViewState["query_CardTimeEDate"] = Calendar_CardTimeEDate.DateValue.Trim();
            ViewState["query_CardNo_PsnName"] = TextBox_CardNo_PsnName.Text.Trim();
            ViewState["dropCompany"] = dropCompany.SelectedValue.Trim();
            ViewState["ddlStatus"] = ddlStatus.SelectedValue.Trim();
            ViewState["ddlDetail"] = ddlDetail.SelectedValue.Trim();
            ViewState["query_sMode"] = "Normal";

            Query(ViewState["query_sMode"].ToString(), false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }
        #endregion

        #region GridView_Data_RowDataBound
        public void GridView_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    int[] HeaderWidth = { 130, 130, 120, 80, 100, 80 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 排序條件Header加工
                    foreach (DataControlField dataControlField in MainGridView.Columns)
                    {
                        if (dataControlField.SortExpression.Equals(this.SortExpression))
                        {
                            Label label = new Label();
                            label.Text = this.SortDire.Equals("ASC") ? "▲" : "▼";
                            e.Row.Cells[MainGridView.Columns.IndexOf(dataControlField)].Controls.Add(label);
                        }
                    }
                    #endregion

                    #region 寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);
                    e.Row.CssClass = "GVStyle";
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    li_header.Text = Header_sw.ToString();
                    #endregion

                    break;
                #endregion

                #region DataRow
                case DataControlRowType.DataRow:
                    DataRowView oRow = (DataRowView)e.Row.DataItem;

                    #region 指定Row的ID
                    GridViewRow GrRow = e.Row;
                    GrRow.ClientIDMode = ClientIDMode.Static;
                    GrRow.ID = "GV_Row" + oRow.Row["NewIDNum"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 134, 134, 124, 84, 104, 84 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理
                    #region 讀卡時間
                    #endregion
                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["NewIDNum"].ToString() + "', '', '');");
                    break;
                #endregion

                #region Pager
                case DataControlRowType.Pager:
                    #region 取得控制項
                    GridView gv = sender as GridView;
                    LinkButton lbtnFirst = e.Row.FindControl("lbtnFirst") as LinkButton;
                    LinkButton lbtnLast = e.Row.FindControl("lbtnLast") as LinkButton;
                    PlaceHolder phdPageNumber = e.Row.FindControl("phdPageNumber") as PlaceHolder;
                    LinkButton lbtnPrev = e.Row.FindControl("lbtnPrev") as LinkButton;
                    LinkButton lbtnNext = e.Row.FindControl("lbtnNext") as LinkButton;
                    LinkButton lbtnPage;
                    #endregion

                    #region 顯示頁數及[上下頁、首頁、末頁]處理
                    int showRange = 5; //顯示快捷頁數
                    int pageCount = gv.PageCount;
                    int pageIndex = gv.PageIndex;

                    //_pagecount = (_datacount % _pagesize) == 0 ? (_datacount / _pagesize) : (_datacount / _pagesize) + 1;
                    //int pageCount = _pagecount;
                    //int pageIndex = _pageindex;

                    int startIndex = (pageIndex + 1 < showRange) ?
                        0 : (pageIndex + 1 + showRange / 2 >= pageCount) ? pageCount - showRange : pageIndex - showRange / 2;
                    int endIndex = (startIndex >= pageCount - showRange) ? pageCount : startIndex + showRange;
                    #region 頁數列
                    phdPageNumber.Controls.Add(new LiteralControl("  "));

                    #region 指定頁數及改變文字風格
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        lbtnPage = new LinkButton();
                        lbtnPage.ID = "Pages_" + (i).ToString();
                        lbtnPage.Text = (i + 1).ToString();
                        //lbtnPage.CommandName = "Pages";
                        lbtnPage.CommandName = "Page";
                        lbtnPage.CommandArgument = (i + 1).ToString();
                        lbtnPage.Font.Overline = false;

                        if (i == pageIndex)
                        {
                            lbtnPage.Font.Bold = true;
                            lbtnPage.ForeColor = System.Drawing.Color.White;
                            lbtnPage.OnClientClick = "return false;";
                        }
                        else
                            lbtnPage.Font.Bold = false;

                        phdPageNumber.Controls.Add(lbtnPage);
                        phdPageNumber.Controls.Add(new LiteralControl(" "));
                    }
                    #endregion

                    #endregion

                    #region 上下頁

                    lbtnPrev.OnClientClick = " Block(); ";
                    lbtnPrev.Click += delegate (object obj, EventArgs args)
                    {
                        if (gv.PageIndex > 0)
                        {
                            gv.PageIndex = gv.PageIndex - 1;
                            MainGridView.DataBind();
                        }

                        Sa.Web.Fun.RunJavaScript(this.Page, "$.unblockUI();");
                    };

                    lbtnNext.OnClientClick = " Block(); ";
                    lbtnNext.Click += delegate (object obj, EventArgs args)
                    {
                        if (gv.PageIndex < gv.PageCount)
                        {
                            gv.PageIndex = gv.PageIndex + 1;
                            MainGridView.DataBind();
                        }

                        Thread.Sleep(500);
                        Sa.Web.Fun.RunJavaScript(this.Page, "$.unblockUI();");
                    };
                    #endregion

                    #region 首末頁

                    lbtnFirst.Click += delegate (object obj, EventArgs args)
                    {
                        gv.PageIndex = 0;
                        MainGridView.DataBind();
                    };

                    lbtnLast.Click += delegate (object obj, EventArgs args)
                    {
                        gv.PageIndex = MainGridView.PageCount - 1;
                        MainGridView.DataBind();
                    };
                    #endregion

                    #endregion

                    #region 顯示總頁數及目前頁數
                    phdPageNumber.Controls.Add(
                        new LiteralControl(string.Format("　{0} / {1}　", pageIndex + 1, pageCount == 0 ? 1 : pageCount)));
                    phdPageNumber.Controls.Add(new LiteralControl("  "));
                    #endregion

                    #region 顯示總筆數
                    phdPageNumber.Controls.Add(
                        new LiteralControl(string.Format(this.GetGlobalResourceObject("Resource", "lblGvCounts").ToString(), hDataRowCount.Value)));
                    phdPageNumber.Controls.Add(new LiteralControl("  "));
                    #endregion

                    #region 寫入Literal_Pager
                    StringWriter Pager_sw = new StringWriter();
                    HtmlTextWriter Pager_writer = new HtmlTextWriter(Pager_sw);
                    e.Row.CssClass = "GVStylePgr";

                    e.Row.RenderControl(Pager_writer);
                    e.Row.Visible = false;
                    li_Pager.Text = Pager_sw.ToString();
                    #endregion
                    break;
                    #endregion
            }
        }
        #endregion

        #region GridView_Data_DataBound
        protected void GridView_Data_DataBound(object sender, EventArgs e)
        {
            td_showGridView.Attributes["colspan"] = MainGridView.Columns.Count.ToString();
        }
        #endregion

        #region GridView_PageIndexChanging
        protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            MainGridView.PageIndex = e.NewPageIndex;
            if (MainGridView.Rows[0].Cells.Count == MainGridView.HeaderRow.Cells.Count)
                MainGridView.DataBind();
        }
        #endregion

        #region GridView_Sorting
        protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            String SortField = e.SortExpression, SortDire = "DESC";

            if (ViewState["SortExpression"] != null)
            {
                if (ViewState["SortExpression"].ToString().Equals(SortField))
                {
                    if (!ViewState["SortDire"].Equals("ASC"))
                    {
                        SortDire = "ASC";
                    }
                    else
                    {
                        SortDire = "DESC";
                    }
                }
            }

            ViewState["SortExpression"] = SortField;
            ViewState["SortDire"] = SortDire;
            Query(ViewState["query_sMode"].ToString(), false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }
        #endregion

        #region VerifyRenderingInServerForm
        public override void VerifyRenderingInServerForm(Control control)
        {
            // 修正 'XX'型別 必須置於有 runat=server 的表單標記之中 Override此Methods
        }
        #endregion

        #endregion

        #region Method

        #region Query
        public void Query(string sMode, bool bMode, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            List<string> liSqlPara = new List<string>();
            bool isOK = true;
            ViewState["query_sMode"] = sMode;

            if ((string)ViewState["FlagExport"] != "OK" && bMode)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('無資料可匯出!!');", true);

                EmptyCondition();       // 做一個空的datatable給gridview秀查無資料
                return;
            }

            string strTmpDT1 = "[##TEMP_0632_1_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "]";
            string strTmpDT2 = "[##TEMP_0632_2_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "]";
            string strOrgData = "[##TEMP_0632_3_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "]";

            try
            {
                #region 產生資料

                #region 測試 SQL CODE
                //string test = @"
                //    DECLARE @dd DATETIME
                //    DECLARE @d1 DATETIME
                //    DECLARE @d2 DATETIME
                //    SET @dd = '2016/09/02'
                //    SET @d1 = @dd + ' 00:00:00'
                //    SET @d2 = @dd + ' 23:59:59'
                //    SELECT
                //        TE.FirstTime, TE.OrgName, TE.PsnNo, TE.PsnName,  
	               //     DE.FirstTime 'FirstTime2', DE.OrgName 'OrgName2', DE.PsnNo 'PsnNo2', DE.PsnName 'PsnName2'
                //    FROM
                //    (
                //        SELECT * FROM
                //        (
                //            SELECT
                //                MIN(CL.CardTime) AS 'FirstTime', OS.OrgName, CL.PsnNo, CL.PsnName,
                //                ROW_NUMBER() OVER(PARTITION BY CONVERT(VARCHAR, CL.CardTime, 111), OS.OrgName, CL.PsnNo, CL.PsnName ORDER BY MIN(CL.CardTime) ASC) AS RN
                //            FROM
                //            (
                //                SELECT DISTINCT rlt.CardTime, rlt.PsnNo, rlt.PsnName, rlt.OrgStruc FROM
                //                (
                //                    SELECT CardTime, PsnNo, PsnName, EquNo, OrgStruc FROM B01_CardLog
                //                    WHERE CardTime BETWEEN @d1 AND @d2
                //                    UNION
                //                    SELECT CardTime, PsnNo, PsnName, EquNo, OrgStruc FROM B01_CardLogFill
                //                    WHERE CardTime BETWEEN @d1 AND @d2
                //                ) rlt WHERE rlt.EquNo IN
                //                (
                //                    SELECT ED.EquNo FROM B01_EquData ED
                //                    INNER JOIN B01_Reader RD ON RD.EquNo = ED.EquNo
                //                    WHERE ED.EquID IN
                //                    (
                //                        SELECT EquID FROM B01_EquGroupData WHERE EquGrpID = 5
                //                    )
                //                )
                //            ) CL
                //            INNER JOIN
                //            (
                //                SELECT DISTINCT OrgNo, OrgIDList, OrgName FROM OrgStrucAllData('Company')
                //                WHERE OrgNo <> '' AND OrgNo <> 'C001'
                //            ) OS ON OS.OrgIDList = CL.OrgStruc
                //            GROUP BY CONVERT(VARCHAR, CL.CardTime, 111), OS.OrgName, CL.PsnNo, CL.PsnName
                //        ) RLT1 WHERE RN = 1
                //    ) TE
                //    FULL JOIN
                //    (
                //        SELECT * FROM
                //        (
                //            SELECT
                //                MIN(CL.CardTime) AS 'FirstTime', OS.OrgName, CL.PsnNo, CL.PsnName,
                //                ROW_NUMBER() OVER(PARTITION BY CONVERT(VARCHAR, CL.CardTime, 111), OS.OrgName, CL.PsnNo, CL.PsnName ORDER BY MIN(CL.CardTime) ASC) AS RN
                //            FROM
                //            (
                //                SELECT DISTINCT rlt.CardTime, rlt.PsnNo, rlt.PsnName, rlt.OrgStruc FROM
                //                (
                //                    SELECT CardTime, PsnNo, PsnName, EquNo, OrgStruc FROM B01_CardLog
                //                    WHERE CardTime BETWEEN @d1 AND @d2
                //                    UNION
                //                    SELECT CardTime, PsnNo, PsnName, EquNo, OrgStruc FROM B01_CardLogFill
                //                    WHERE CardTime BETWEEN @d1 AND @d2
                //                ) rlt WHERE rlt.EquNo IN
                //                (
                //                    SELECT ED.EquNo FROM B01_EquData ED
                //                    INNER JOIN B01_Reader RD ON RD.EquNo = ED.EquNo
                //                    WHERE RD.Dir = '進' AND
                //                    ED.EquID IN
                //                    (
                //                        SELECT EquID FROM B01_EquGroupData WHERE EquGrpID = 5
                //                    )
                //                )
                //            ) CL
                //            INNER JOIN
                //            (
                //                SELECT DISTINCT OrgNo, OrgIDList, OrgName FROM OrgStrucAllData('Company')
                //                WHERE OrgNo <> '' AND OrgNo <> 'C001'
                //            ) OS ON OS.OrgIDList = CL.OrgStruc
                //            GROUP BY CONVERT(VARCHAR, CL.CardTime, 111), OS.OrgName, CL.PsnNo, CL.PsnName
                //        ) RLT2 WHERE RN = 1
                //    ) DE ON TE.PsnNo = DE.PsnNo AND CONVERT(VARCHAR, TE.FirstTime, 111) = CONVERT(VARCHAR, DE.FirstTime, 111) ";
                #endregion

                #region 將目標設備的讀卡資料放到暫存資料表 strTmpDT1
                sql = string.Format(@"
                    SELECT DISTINCT rlt.CardTime, rlt.PsnNo, rlt.PsnName, rlt.OrgStruc INTO {0} FROM 
                    (
	                    SELECT CardTime, PsnNo, PsnName, EquNo, OrgStruc FROM B01_CardLog 
	                    WHERE CardTime BETWEEN ? AND ? 
	                    UNION
	                    SELECT CardTime, PsnNo, PsnName, EquNo, OrgStruc FROM B01_CardLogFill 
	                    WHERE CardTime BETWEEN ? AND ?
                    ) rlt WHERE rlt.EquNo IN 
                    (
                        SELECT ED.EquNo FROM B01_EquData ED  
                        INNER JOIN B01_Reader RD ON RD.EquNo = ED.EquNo
                        WHERE ED.EquID IN 
                        (
	                        SELECT EquID FROM B01_EquGroupData WHERE EquGrpID = {1}
                        )
                    ) ", strTmpDT1, strLoadRegEquGroup);

                liSqlPara.Clear();
                liSqlPara.Add("D:" + ViewState["query_CardTimeSDate"]);
                liSqlPara.Add("D:" + ViewState["query_CardTimeEDate"]);
                liSqlPara.Add("D:" + ViewState["query_CardTimeSDate"]);
                liSqlPara.Add("D:" + ViewState["query_CardTimeEDate"]);
                oAcsDB.SqlCommandExecute(sql, liSqlPara);
                #endregion

                #region 將三號門相關設備的讀卡資料放到暫存資料表 strTmpDT2
                sql = string.Format(@"
                    SELECT DISTINCT rlt.CardTime, rlt.PsnNo, rlt.PsnName, rlt.OrgStruc INTO {0} FROM 
                    (
	                    SELECT CardTime, PsnNo, PsnName, EquNo, OrgStruc FROM B01_CardLog 
	                    WHERE CardTime BETWEEN ? AND ? 
	                    UNION
	                    SELECT CardTime, PsnNo, PsnName, EquNo, OrgStruc FROM B01_CardLogFill 
	                    WHERE CardTime BETWEEN ? AND ?
                    ) rlt WHERE rlt.EquNo IN 
                    (
                        SELECT ED.EquNo FROM B01_EquData ED  
                        INNER JOIN B01_Reader RD ON RD.EquNo = ED.EquNo
                        WHERE RD.Dir = '進' AND 
                        ED.EquID IN 
                        (
	                        SELECT EquID FROM B01_EquGroupData WHERE EquGrpID = 5 
                        )
                    ) ", strTmpDT2);

                liSqlPara.Clear();
                liSqlPara.Add("D:" + ViewState["query_CardTimeSDate"]);
                liSqlPara.Add("D:" + ViewState["query_CardTimeEDate"]);
                liSqlPara.Add("D:" + ViewState["query_CardTimeSDate"]);
                liSqlPara.Add("D:" + ViewState["query_CardTimeEDate"]);
                oAcsDB.SqlCommandExecute(sql, liSqlPara);
                #endregion

                #region 將 OrgStrucAllData('Company') 放到暫存資料表 strOrgData
                sql = string.Format(@"
                    SELECT DISTINCT OrgNo, OrgIDList, OrgName INTO {0} FROM OrgStrucAllData('Company') 
			        WHERE OrgNo <> '' AND OrgNo <> 'C001'", strOrgData);

                oAcsDB.SqlCommandExecute(sql);
                #endregion

                #region 組成SQL
                sql = string.Format(@"
                    SELECT 
	                    TE.FirstTime, TE.OrgName, TE.PsnNo, TE.PsnName,  
	                    DE.FirstTime 'FirstTime2', DE.OrgName 'OrgName2', DE.PsnNo 'PsnNo2', DE.PsnName 'PsnName2' 
                    FROM 
                    (
	                    SELECT * FROM 
	                    (
		                    SELECT 
			                    MIN(CL.CardTime) AS 'FirstTime', OS.OrgName, CL.PsnNo, CL.PsnName, 
			                    ROW_NUMBER() OVER (PARTITION BY CONVERT(VARCHAR, CL.CardTime, 111), OS.OrgName, CL.PsnNo, CL.PsnName ORDER BY MIN(CL.CardTime) ASC) AS RN  
		                    FROM {0} CL 
		                    INNER JOIN {2} OS ON OS.OrgIDList = CL.OrgStruc 
		                    GROUP BY CONVERT(VARCHAR, CL.CardTime, 111), OS.OrgName, CL.PsnNo, CL.PsnName 
	                    ) RLT1 WHERE RN = 1
                    ) TE 
                    FULL JOIN 
                    (
	                    SELECT * FROM 
	                    (
		                    SELECT 
			                    MIN(CL.CardTime) AS 'FirstTime', OS.OrgName, CL.PsnNo, CL.PsnName,  
			                    ROW_NUMBER() OVER (PARTITION BY CONVERT(VARCHAR, CL.CardTime, 111), OS.OrgName, CL.PsnNo, CL.PsnName ORDER BY MIN(CL.CardTime) ASC) AS RN  
		                    FROM {1} CL 
		                    INNER JOIN {2} OS ON OS.OrgIDList = CL.OrgStruc 
		                    GROUP BY CONVERT(VARCHAR, CL.CardTime, 111), OS.OrgName, CL.PsnNo, CL.PsnName  
	                    ) RLT2 WHERE RN = 1
                    ) DE ON TE.PsnNo = DE.PsnNo AND CONVERT(VARCHAR, TE.FirstTime, 111) = CONVERT(VARCHAR, DE.FirstTime, 111) ; 

                    DROP TABLE {0} ; 
                    DROP TABLE {1} ;
                    DROP TABLE {2} ; ", 
                    strTmpDT1, strTmpDT2, strOrgData);

                #endregion

                DataTable dtTmp = new DataTable();
                isOK = oAcsDB.GetDataTable("Result", sql, out dtTmp);
                #endregion

                #region 整理 dtTmp dtGV
                if (isOK)
                {
                    // 將處理後的資料丟到GRIDVIEW顯示
                    HandleGridView(dtTmp, SortExpression, SortDire, bMode);
                }
                #endregion
            }
            catch
            {
                #region drop tmp table
                sql = string.Format(@"
                        DROP TABLE {0} ; 
                        DROP TABLE {1} ;
                        DROP TABLE {2} ;",
                    strTmpDT1, strTmpDT2, strOrgData);
                oAcsDB.SqlCommandExecute(sql);
                #endregion
            }
            finally
            {
                Sa.Web.Fun.RunJavaScript(this.Page, "$.unblockUI();");
            }
        }

        #endregion

        private void HandleGridView(DataTable dtTmp, string SortExpression, string SortDire, bool bMode)
        {
            string strCurrentDate = "";     // 每筆紀錄的時間 (yyyy/MM/dd) + 比對時間

            DataTable dtGV = new DataTable();
            dtGV.Columns.Add("NewIDNum");
            dtGV.Columns.Add("CardTime");
            dtGV.Columns.Add("CardTime2");
            dtGV.Columns.Add("OrgName");
            dtGV.Columns.Add("PsnNo");
            dtGV.Columns.Add("PsnName");
            dtGV.Columns.Add("Status");
            dtGV.Columns.Add("Detail");

            int i = 0;

            foreach (DataRow dr in dtTmp.Rows)
            {
                DataRow dr_new = dtGV.NewRow();
                Random rnd = new Random(Guid.NewGuid().GetHashCode());

                dr_new["NewIDNum"] = i.ToString() + rnd.Next().ToString();
                i++;

                #region FirstTime
                DateTime dtime;
                if (DateTime.TryParse(dr["FirstTime"].ToString(), out dtime))
                {
                    dr_new["CardTime"] = dtime.ToString("yyyy/MM/dd HH:mm:ss");
                    strCurrentDate = string.Format("{0} {1}:00",
                        dtime.ToString("yyyy/MM/dd"), strLoadCompareTime);
                }
                else
                {
                    dr_new["CardTime"] = "";
                }
                #endregion

                #region FirstTime2
                DateTime dtime2;
                if (DateTime.TryParse(dr["FirstTime2"].ToString(), out dtime2))
                {
                    dr_new["CardTime2"] = dtime2.ToString("yyyy/MM/dd HH:mm:ss");
                    strCurrentDate = string.Format("{0} {1}:00",
                        dtime2.ToString("yyyy/MM/dd"), strLoadCompareTime);
                }
                else
                {
                    dr_new["CardTime2"] = "";
                }
                #endregion

                #region OrgName
                if (dr["OrgName"].ToString() != "")
                {
                    dr_new["OrgName"] = dr["OrgName"].ToString();
                }

                if (dr["OrgName2"].ToString() != "")
                {
                    dr_new["OrgName"] = dr["OrgName2"].ToString();
                }
                #endregion

                #region PsnNo
                if (dr["PsnNo"].ToString() != "")
                {
                    dr_new["PsnNo"] = dr["PsnNo"].ToString();
                }

                if (dr["PsnNo2"].ToString() != "")
                {
                    dr_new["PsnNo"] = dr["PsnNo2"].ToString();
                }
                #endregion

                #region PsnName
                if (dr["PsnName"].ToString() != "")
                {
                    dr_new["PsnName"] = dr["PsnName"].ToString();
                }

                if (dr["PsnName2"].ToString() != "")
                {
                    dr_new["PsnName"] = dr["PsnName2"].ToString();
                }
                #endregion

                #region Status、Detail
                string strDatail = "";
                
                if (IsStatus(dr_new["CardTime"].ToString(), dr_new["CardTime2"].ToString(), strCurrentDate, out strDatail))
                {
                    dr_new["Status"] = "正常";
                }
                else
                {
                    dr_new["Status"] = "異常";
                }

                dr_new["Detail"] = strDatail;
                #endregion

                dtGV.Rows.Add(dr_new);
            }

            string strCondition = "";

            #region 加入 人員編號、姓名 的條件    
            if (!TextBox_CardNo_PsnName.Text.Trim().Equals(""))
            {
                if (strCondition == "")
                {
                    strCondition = string.Format(" PsnNo LIKE '%{0}%' OR PsnName LIKE '%{0}%' ",
                        ViewState["query_CardNo_PsnName"]);
                }
                else
                {
                    strCondition += string.Format(" AND PsnNo LIKE '%{0}%' OR PsnName LIKE '%{0}%' ",
                        ViewState["query_CardNo_PsnName"]);
                }
            }
            #endregion

            #region 加入 公司 的條件
            if (!dropCompany.SelectedValue.Equals(""))
            {
                if (strCondition == "")
                {
                    strCondition = string.Format(" OrgName = '{0}' ", ViewState["dropCompany"]);
                }
                else
                {
                    strCondition += string.Format(" AND OrgName = '{0}' ", ViewState["dropCompany"]);
                }
            }
            #endregion

            #region 加入 狀態 的條件
            if (!ddlStatus.SelectedValue.Equals(""))
            {
                if (strCondition == "")
                {
                    strCondition = string.Format(" Status = '{0}' ", ViewState["ddlStatus"]);
                }
                else
                {
                    strCondition += string.Format(" AND Status = '{0}' ", ViewState["ddlStatus"]);
                }
            }
            #endregion

            #region 加入 壯態說明 的條件 
            if (!ddlDetail.SelectedValue.Equals(""))
            {
                if (strCondition == "")
                {
                    strCondition = string.Format(" Detail = '{0}' ", ViewState["ddlDetail"]);
                }
                else
                {
                    strCondition += string.Format(" AND Detail = '{0}' ", ViewState["ddlDetail"]);
                }
            }
            #endregion

            dtGV.DefaultView.RowFilter = strCondition;
            dtGV.DefaultView.Sort = SortExpression + " " + SortDire;
            
            DataTable dtRT = new DataTable();
            dtRT = dtGV.DefaultView.ToTable();
            hDataRowCount.Value = dtRT.Rows.Count.ToString();   // 顯示總筆數

            if (dtRT.Rows.Count > 0)
            {
                ViewState["FlagExport"] = "OK";
            }
            else
            {
                ViewState["FlagExport"] = "";
            }

            if (bMode)
            {
                ExportExcel(dtRT);  // 列印模式
            }
            else
            {
                GirdViewDataBind(MainGridView, dtRT);
                UpdatePanel1.Update();
            }  
        }

        // 讀取比對時間
        private string LoadCompareTime()
        {
            string strReturnValue = "";
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            try
            {
                string strSQL = @"
                    SELECT ParaValue FROM B00_SysParameter 
                    WHERE ParaClass = 'Contractor' AND ParaNo = 'cttime'";

                strReturnValue = oAcsDB.GetStrScalar(strSQL);

                // 如果沒有這個設備參數，則補上該設備參數
                if (strReturnValue == null)
                {
                    strSQL = @"
                        INSERT INTO B00_SysParameter 
		                    (ParaClass, ParaNo, ParaName, ParaValue, ParaType, ParaDesc, CreateUserID)
	                    VALUES
		                    ('Contractor','cttime', N'廠商報到時間', '08:15', 'String', N'廠商報到時間使用，輸入格式 HH:MM', 'Saho')";

                    oAcsDB.SqlCommandExecute(strSQL);
                    strReturnValue = "08:15";
                }
            }
            catch {}

            return strReturnValue;
        }

        // 讀取 報到用設備群組
        private string LoadRegEquGroup()
        {
            string strReturnValue = "";
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            try
            {
                string strSQL = @"
                    SELECT ParaValue FROM B00_SysParameter 
                    WHERE ParaClass = 'Contractor' AND ParaNo = 'RegEquGroup'";

                strReturnValue = oAcsDB.GetStrScalar(strSQL);

                // 如果沒有這個設備參數，則補上該設備參數
                if (strReturnValue == null)
                {
                    strSQL = @"
                        INSERT INTO B00_SysParameter 
		                    (ParaClass, ParaNo, ParaName, ParaValue, ParaType, ParaDesc, CreateUserID)
	                    VALUES
		                    ('Contractor','RegEquGroup', N'報到用設備群組', '1', 'String', N'廠商報到用設備群組，取EquGrpID', 'Saho')";

                    oAcsDB.SqlCommandExecute(strSQL);
                    strReturnValue = "1";
                }
            }
            catch { }

            return strReturnValue;
        }

        // 更新報到時間和報到設備群組
        private void UpdateRegTimeAndEquGroup()
        {
            try
            {
                string strTime = this.ddlHour.SelectedValue + ":" + this.ddlMin.SelectedValue;
                DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

                string strSQL = @"
                    UPDATE B00_SysParameter SET ParaValue = ? 
                    WHERE ParaClass = 'Contractor' AND ParaNo = 'cttime' ; 
                    UPDATE B00_SysParameter SET ParaValue = ? 
                    WHERE ParaClass = 'Contractor' AND ParaNo = 'RegEquGroup' ";

                List<string> liPara = new List<string>();
                liPara.Add("S:" + strTime);
                liPara.Add("S:" + this.ddlRegEquGroup.SelectedValue);

                int i = oAcsDB.SqlCommandExecute(strSQL, liPara);

                if (i > -1)
                {
                    strLoadCompareTime = strTime;
                    strLoadRegEquGroup = this.ddlRegEquGroup.SelectedValue;
                }
            }
            catch { }
        }

        // 判斷 Status 正、異常
        private bool IsStatus(string ct1, string ct2, string cd, out string strDetail)
        {
            bool IsOk = false;
            bool IsOk1 = true;
            bool IsOk2 = true;
            bool IsOk3 = true;
            strDetail = "其他異常";

            DateTime dt1 = new DateTime();
            DateTime dt2 = new DateTime();
            DateTime dd = new DateTime();

            try
            {
                // 處理比對時間
                // ct1 - 報到時間，ct2 - 入廠時間，cd - 比對時間
                IsOk1 = DateTime.TryParse(ct1, out dt1);
                IsOk2 = DateTime.TryParse(ct2, out dt2);
                IsOk3 = DateTime.TryParse(cd, out dd);

                if (IsOk3)
                {
                    if (IsOk1 && IsOk2)
                    {
                        if (dd > dt1 && dd > dt2 && dt1 >= dt2)
                        {
                            IsOk = true;
                            strDetail = "正常報到";
                        }
                        else if (dd < dt1 && dd > dt2 && dt1 >= dt2)
                        {
                            strDetail = "延遲報到";
                        }
                        else if (dd < dt2 || dt1 < dt2)
                        {
                            strDetail = "入廠有刷卡，其時間晚於比對或報到時間";
                        }
                        
                    }
                    else if (!IsOk1 && IsOk2)
                    {
                        strDetail = "入廠有刷卡，沒有報到刷卡";
                    }
                    else if (IsOk1 && !IsOk2)
                    {
                        if (dd > dt1)
                        {
                            strDetail = "入廠沒刷卡，有報到刷卡";
                        }
                        else
                        {
                            strDetail = "入廠沒刷卡，延遲報到刷卡";
                        }
                    }
                }

                //// 正常報到
                //if (IsOk1 && IsOk2 && IsOk3)
                //{
                //    if (dd > dt1 && dd > dt2 && dt1 >= dt2)
                //    {
                //        IsOk = true;
                //        strDetail = "正常報到";
                //    } 
                //}

                //// 延遲報到
                //if (IsOk1 && IsOk2 && IsOk3)
                //{
                //    if (dd < dt1)
                //    {
                //        strDetail = "延遲報到";
                //    }
                //}

                //// 入廠有刷卡，沒有報到刷卡
                //if (!IsOk1 && IsOk2 && IsOk3)
                //{
                //    strDetail = "入廠有刷卡，沒有報到刷卡";
                //}

                //// 入廠沒刷卡，有報到刷卡 || 入廠沒刷卡，延遲報到刷卡
                //if (IsOk1 && !IsOk2 && IsOk3)
                //{
                //    if (dd > dt1)
                //    {
                //        strDetail = "入廠沒刷卡，有報到刷卡";
                //    }
                //    else
                //    {
                //        strDetail = "入廠沒刷卡，延遲報到刷卡";
                //    }
                //}

                //// 入廠有刷卡，入廠時間晚於比對時間或報到時間
                //if (IsOk1 && IsOk2 && IsOk3)
                //{
                //    if (dd < dt2 || dt1 < dt2)
                //    {
                //        strDetail = "入廠有刷卡，其時間晚於比對或報到時間";
                //    }
                //}

            }
            catch { }

            return IsOk;
        }

        public void GirdViewDataBind(GridView ProcessGridView, DataTable dt)
        {
            if (dt.Rows.Count > 0)//Gridview中有資料
            {
                ProcessGridView.DataSource = dt;
                ProcessGridView.DataBind();
            }
            else//Gridview中沒有資料
            {
                dt.Rows.Add(dt.NewRow());
                ProcessGridView.DataSource = dt;
                ProcessGridView.DataBind();

                int columnCount = ProcessGridView.Rows[0].Cells.Count;
                ProcessGridView.Rows[0].Cells.Clear();
                ProcessGridView.Rows[0].Cells.Add(new TableCell());
                ProcessGridView.Rows[0].Cells[0].ColumnSpan = columnCount;
                ProcessGridView.Rows[0].Cells[0].Text = GetGlobalResourceObject("Resource", "NonData").ToString();
            }
        }

        public void ExportExcel(DataTable ProcDt)
        {
            try
            {
                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("承攬商報到紀錄");
                DataTable dt = ProcDt;

                #region 去掉不必要的欄位
                string strDels = "";
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.ColumnName.Equals("NewIDNum"))
                    {
                        strDels += dc.ColumnName + "|";
                    }
                }

                if (strDels != "")
                {
                    string[] strDel = strDels.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string str in strDel)
                    {
                        dt.Columns.Remove(str);
                    }
                }
                #endregion

                //Title
                ws.Cells[1, 1].Value = "報到時間";
                ws.Cells[1, 2].Value = "入廠時間";
                ws.Cells[1, 3].Value = "公司";
                ws.Cells[1, 4].Value = "人員編號";
                ws.Cells[1, 5].Value = "人員姓名";
                ws.Cells[1, 6].Value = "狀態";
                ws.Cells[1, 7].Value = "狀態說明";

                //Content
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        ws.Cells[i + 2, j + 1].Value = dt.Rows[i].ItemArray[j].ToString().Replace("&nbsp;", "").Trim();
                    }
                }

                string strHeaderValue = string.Format("attachment; filename=Report_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmssfff"));

                ws.Cells.AutoFitColumns(); //自動欄寬
                Response.Clear();
                Response.AddHeader("content-disposition", strHeaderValue);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
            }
            catch {}
        }

        public string SortExpression
        {
            get
            {
                if (ViewState["SortExpression"] == null)
                {
                    ViewState["SortExpression"] = "SortCode";
                }
                return ViewState["SortExpression"].ToString();
            }
            set
            {
                ViewState["SortExpression"] = value;
            }
        }

        public string SortDire
        {
            get
            {
                if (ViewState["SortDire"] == null)
                {
                    ViewState["SortDire"] = " DESC ";
                }
                return ViewState["SortDire"].ToString();
            }
            set
            {
                ViewState["SortDire"] = value;
            }
        }

        public bool IsDate(string strDate)
        {
            try
            {
                DateTime.Parse(strDate);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void EmptyCondition()
        {
            #region 做一個空的datatable給gridview秀查無資料
            DataTable dtEmpty = new DataTable();
            dtEmpty.Columns.Add("NewIDNum");
            dtEmpty.Columns.Add("CardTime");
            dtEmpty.Columns.Add("CardTime2");
            dtEmpty.Columns.Add("OrgName");
            dtEmpty.Columns.Add("PsnNo");
            dtEmpty.Columns.Add("PsnName");
            dtEmpty.Columns.Add("Status");
            dtEmpty.Columns.Add("Detail");

            GirdViewDataBind(this.MainGridView, dtEmpty);
            UpdatePanel1.Update();
            #endregion
        }

        private void DropBind(DropDownList dropData, string strValue, string strText, string strCmd)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            DataTable oDataTable = null;
            ListItem oListItem = new ListItem();

            oAcsDB.GetDataTable("DropData", strCmd, out oDataTable);

            dropData.Items.Clear();

            #region Give Empty Item
            if (dropData == ddlRegEquGroup)
            {
                oListItem.Value = "0";
            }
            else
            {
                oListItem.Value = "";
            }
            oListItem.Text = "- 請選擇 -";

            dropData.Items.Add(oListItem);
            #endregion

            foreach (DataRow dr in oDataTable.Rows)
            {
                oListItem = new ListItem();

                oListItem.Text = dr[strText].ToString();
                oListItem.Value = dr[strValue].ToString();

                dropData.Items.Add(oListItem);
            }
        }
        #endregion
    }
}