using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using System.Data.SqlClient;
using System.Data.Common;

namespace SahoAcs
{
    public partial class QueryCardLog_old20180109 : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 30, _datacount = 1, _pageindex = 0, _pagecount = 0;
        string _sqlcommand, sPsnID;
        DataTable CardLogTable = new DataTable();
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.QueryButton);
            //oScriptManager.RegisterAsyncPostBackControl(this.ADVQueryShowButton);
            //oScriptManager.RegisterAsyncPostBackControl(this.ViewButton);
            //oScriptManager.RegisterAsyncPostBackControl(this.ADVQueryButton);
            //oScriptManager.RegisterAsyncPostBackControl(this.ADVCloseButton);
            //oScriptManager.RegisterAsyncPostBackControl(this.ExportButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("QueryCardLog", "QueryCardLog.js");//加入同一頁面所需的JavaScript檔案

            #region 註冊主頁Button動作
            QueryButton.Attributes["onClick"] = "SelectState(); return false;";
            ADVQueryShowButton.Attributes["onClick"] = "CallAdvancedQuery(); return false;";
            //ExportButton.Attributes["onClick"] = "ExportQuery(); return false;";
            ViewButton.Attributes["onClick"] = "CallShowLogDetail(); return false;";
            #endregion

            #region 註冊pop1頁Button動作
            //ImgCloseButton1.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            //popB_Cancel.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            #endregion

            #region 註冊pop2頁Button動作
            ImgCloseButton2.Attributes["onClick"] = "SetMode(''); CancelTrigger2.click(); return false;";
            ADVQueryButton.Attributes["onClick"] = "AVDQuery(); CancelTrigger2.click(); return false;";
            ADVCloseButton.Attributes["onClick"] = "SetMode(''); CancelTrigger2.click(); return false;";
            #endregion

            Pub.SetModalPopup(ModalPopupExtender1, 1);
            Pub.SetModalPopup(ModalPopupExtender2, 2);
            this.MainGridView.PageSize = _pagesize;
            #endregion

            #region Check Person Data
            this.sPsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
            if (!this.sPsnID.Equals(""))
            {
                this.ShowPsnInfo1.Visible = false;
                this.ShowPsnInfo2.Visible = false;
                this.ShowPsnInfo3.Visible = false;
            }
            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                Session["tmpList"] = new List<string>();
                Session["tmpDatatable"] = new DataTable();

                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion

                #region 給下拉式選單派值
                CreateDropDownList_DepItem();
                CreateDropDownList_EquItem();

                //CreateDropDownList_LogStatusItem();
                //CreateDropDownList_ADVLogStatusItem();
                CreateDropDownList_StatusItem();    // 一般和進階的都在這邊完成

                #endregion

                #region Give viewstate value
                ViewState["query_CardTimeSDate"] = "";
                ViewState["query_CardTimeEDate"] = "";
                ViewState["query_CardNo_PsnName"] = "";
                ViewState["query_LogStatus"] = "";
                ViewState["query_ADVCardTimeSDate"] = "";
                ViewState["query_ADVCardTimeEDate"] = "";
                ViewState["query_ADVLogTimeSDate"] = "";
                ViewState["query_ADVLogTimeEDate"] = "";
                ViewState["query_ADVDepNoDepName"] = "";
                ViewState["query_ADVDep"] = "";
                ViewState["query_ADVEquNoEquName"] = "";
                ViewState["query_ADVEqu"] = "";
                ViewState["query_ADVPsnNameCardNo"] = "";
                ViewState["query_ADVLogStatus"] = "";
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

                    ADVCalendar_CardTimeSDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 00:00:00";
                    ADVCalendar_CardTimeEDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 23:59:59";
                }
                else
                {
                    Calendar_CardTimeSDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 00:00:00";
                    Calendar_CardTimeEDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 23:59:59";

                    ADVCalendar_CardTimeSDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 00:00:00";
                    ADVCalendar_CardTimeEDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 23:59:59";
                }

                //Calendar_CardTimeSDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 00:00:00";
                //Calendar_CardTimeEDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 23:59:59";

                //ADVCalendar_CardTimeSDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 00:00:00";
                //ADVCalendar_CardTimeEDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 23:59:59";

                ViewState["query_CardTimeSDate"] = Calendar_CardTimeSDate.DateValue;
                ViewState["query_CardTimeEDate"] = Calendar_CardTimeEDate.DateValue;

                ViewState["query_ADVCardTimeSDate"] = ADVCalendar_CardTimeSDate.DateValue;
                ViewState["query_ADVCardTimeEDate"] = ADVCalendar_CardTimeEDate.DateValue;
                #endregion

                EmptyCondition();       // 做一個空的datatable給gridview秀查無資料
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
                        ViewState["query_CardTimeSDate"] = Calendar_CardTimeSDate.DateValue.Trim();
                        ViewState["query_CardTimeEDate"] = Calendar_CardTimeEDate.DateValue.Trim();
                        ViewState["query_CardNo_PsnName"] = TextBox_CardNo_PsnName.Text.Trim();
                        ViewState["query_LogStatus"] = DropDownList_LogStatus.SelectedTextCSV;

                        ViewState["query_sMode"] = "Normal";

                        //if (this.sPsnID == "")
                        //    Query(ViewState["query_sMode"].ToString(), false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        //else
                        //    QueryByPerson(ViewState["query_sMode"].ToString(), false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());

                        Query(ViewState["query_sMode"].ToString(), false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    }
                    else if (sFormTarget == this.ADVQueryButton.ClientID)
                    {
                        ViewState["query_ADVCardTimeSDate"] = this.ADVCalendar_CardTimeSDate.DateValue;
                        ViewState["query_ADVCardTimeEDate"] = this.ADVCalendar_CardTimeEDate.DateValue;
                        ViewState["query_ADVLogTimeSDate"] = this.ADVCalendar_LogTimeSDate.DateValue;
                        ViewState["query_ADVLogTimeEDate"] = this.ADVCalendar_LogTimeEDate.DateValue;
                        ViewState["query_ADVDepNoDepName"] = this.ADVTextBox_DepNoDepName.Text.Trim();
                        ViewState["query_ADVDep"] = this.ADVDropDownList_Dep.SelectedValuesCSV;
                        ViewState["query_ADVEquNoEquName"] = this.ADVTextBox_EquNoEquName.Text.Trim();
                        ViewState["query_ADVEqu"] = this.ADVDropDownList_Equ.SelectedTextCSV;
                        ViewState["query_ADVPsnNameCardNo"] = this.ADVTextBox_PsnNameCardNo.Text.Trim();
                        ViewState["query_ADVLogStatus"] = this.ADVDropDownList_LogStatus.SelectedTextCSV;

                        ViewState["query_sMode"] = "ADV";
                        Query(ViewState["query_sMode"].ToString(), false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    }
                    else if (sFormTarget == this.ExportButton.ClientID)
                    {
                        //Query(ViewState["query_sMode"].ToString(), true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    }
                    else
                    {
                        Query(ViewState["query_sMode"].ToString(), false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    }

                    // 清除 __EVENTTARGET、__EVENTARGUMENT 的值
                    Sa.Web.Fun.RunJavaScript(this.Page,
                        @" theForm.__EVENTTARGET.value   = '' ;
                       theForm.__EVENTARGUMENT.value = '' ; ");
                }

                Sa.Web.Fun.RunJavaScript(this.Page, "$.unblockUI();");
            }
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
                    int[] HeaderWidth = { 120, 140, 80, 100, 70,70, 60, 80, 100, 100, 120 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

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
                    GrRow.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    GrRow.ID = "GV_Row" + oRow.Row["RecordID"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 123, 144, 84, 104, 74,74, 64, 84, 104, 104, 124 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 讀卡時間
                    if (!string.IsNullOrEmpty(oRow.Row["CardTime"].ToString()))
                    {
                        e.Row.Cells[0].Text = DateTime.Parse(oRow.Row["CardTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                    }
                    #endregion
                    #region 部門代碼
                    #endregion
                    #region 部門名稱
                    #endregion
                    #region 人員編號
                    #endregion
                    #region 姓名
                    #endregion
                    #region 卡號
                    #endregion
                    #region 版次
                    #endregion                    
                    #region 設備編號
                    //LinkButton btn = (LinkButton)e.Row.Cells[7].FindControl("BtnVideo");
                    //btn.Attributes["href"] = "#";
                    //if ((e.Row.RowIndex % 2) == 0)
                    //{
                    //    btn.Attributes.Add("onclick", "OpenVideo(1)");
                    //}
                    //else
                    //{
                    //    btn.Attributes.Add("onclick", "OpenVideo(2)");
                    //}
                    #endregion
                    #region 設備名稱
                    #endregion
                    #region 讀卡結果
                    #endregion
                    #region 讀卡時間
                    if (!string.IsNullOrEmpty(oRow.Row["LogTime"].ToString()))
                    {
                        e.Row.Cells[e.Row.Cells.Count - 1].Text = DateTime.Parse(oRow.Row["LogTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                    }
                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    //for (int x = 0; x < e.Row.Cells.Count; x++)
                    //{
                    //    if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                    //        e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    //}
                    //e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 20, true);
                    //e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text, 32, true);
                    //e.Row.Cells[3].Text = LimitText(e.Row.Cells[3].Text, 11, true);
                    //e.Row.Cells[5].Text = LimitText(e.Row.Cells[5].Text, 20, true);
                    //e.Row.Cells[6].Text = LimitText(e.Row.Cells[6].Text, 20, true);
                    //e.Row.Cells[7].Text = LimitText(e.Row.Cells[7].Text, 32, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["RecordId"].ToString() + "', '', '');");
                    e.Row.Attributes.Add("OnDblclick", "CallShowLogDetail()");
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

                        //lbtnPage = new LinkButton();
                        //lbtnPage.Text = (i + 1).ToString();
                        //lbtnPage.CommandName = "Page";
                        //lbtnPage.CommandArgument = (i + 1).ToString();
                        //lbtnPage.Font.Overline = false;
                        //if (i == pageIndex)
                        //{
                        //    lbtnPage.Font.Bold = true;
                        //    lbtnPage.ForeColor = System.Drawing.Color.White;
                        //    lbtnPage.OnClientClick = "return false;";
                        //}
                        //else
                        //    lbtnPage.Font.Bold = false;

                        //phdPageNumber.Controls.Add(lbtnPage);
                        //phdPageNumber.Controls.Add(new LiteralControl(" "));
                    }
                    #endregion

                    #endregion

                    #region 上下頁
                    lbtnPrev.Click += delegate (object obj, EventArgs args)
                    {
                        if (gv.PageIndex > 0)
                        {
                            gv.PageIndex = gv.PageIndex - 1;
                            MainGridView.DataBind();
                            //Query(ViewState["query_sMode"].ToString());
                        }
                    };
                    //if (_pageindex == 0)
                    //{
                    //    lbtnPrev.Enabled = false;
                    //}
                    //else
                    //{
                    //    lbtnPrev.Enabled = true;
                    //    lbtnPrev.ID = "lbtnPrev_" + (_pageindex - 1);
                    //}

                    lbtnNext.Click += delegate (object obj, EventArgs args)
                    {
                        if (gv.PageIndex < gv.PageCount)
                        {
                            gv.PageIndex = gv.PageIndex + 1;
                            MainGridView.DataBind();
                            //Query(ViewState["query_sMode"].ToString());
                        }
                    };
                    //if (_pageindex == _pagecount - 1 || _pagecount == 0)
                    //{
                    //    lbtnNext.Enabled = false;
                    //}
                    //else
                    //{
                    //    lbtnNext.Enabled = true;
                    //    lbtnNext.ID = "lbtnNext_" + (_pageindex + 1);
                    //}
                    #endregion

                    #region 首末頁
                    lbtnFirst.Click += delegate (object obj, EventArgs args)
                    {
                        gv.PageIndex = 0;
                        MainGridView.DataBind();
                        //Query(ViewState["query_sMode"].ToString());
                    };

                    //if (_pageindex == 0 || _pagecount == 0)
                    //{
                    //    lbtnFirst.Enabled = false;
                    //}
                    //else
                    //{
                    //    lbtnFirst.Enabled = true;
                    //    lbtnFirst.ID = "lbtnFirst_" + 0;
                    //}

                    lbtnLast.Click += delegate (object obj, EventArgs args)
                    {
                        gv.PageIndex = MainGridView.PageCount;
                        MainGridView.DataBind();

                        //Query(ViewState["query_sMode"].ToString());
                    };
                    //if (_pageindex == _pagecount - 1 || _pagecount == 0)
                    //{
                    //    lbtnLast.Enabled = false;
                    //}
                    //else
                    //{
                    //    lbtnLast.Enabled = true;
                    //    lbtnLast.ID = "lbtnLast_" + (_pagecount - 1);
                    //}
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

        #region ExportButton_Click
        protected void ExportButton_Click(object sender, EventArgs e)
        {
            Query(ViewState["query_sMode"].ToString(), true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());            
        }
        #endregion

        #endregion

        #region Method

        #region Query
        public void Query(string sMode, bool bMode, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "", LogStatusSql = "", DepSql = "", EquSql = "";
            string[] ArrayLogStatus, ArrayDep, ArrayEqu;
            int intRet = -1;
            List<string> liSqlPara = new List<string>();
            bool isOK = true;
            ViewState["query_sMode"] = sMode;

            if (Session["tmpList"] == null) Session["tmpList"] = new List<string>();
            if (Session["tmpDatatable"] == null) Session["tmpDatatable"] = new DataTable();

            if ((string)ViewState["FlagExport"] != "OK" && bMode)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('無資料可匯出!!');", true);

                EmptyCondition();       // 做一個空的datatable給gridview秀查無資料

                return;
            }

            // 產生[全域的暫存##TABLE]，避免重複。
            string strTmpDT1 = "[##TEMP_0601_1_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "]";
            string strOSA = "[##TEMP_0601_OSA_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "]";

            try
            {
                #region 得到第一階段結果 (strTmpDT1)

                string sql2 = string.Format(@"
                SELECT * INTO {0} 
                FROM OrgStrucAllData('') ", strOSA);
                oAcsDB.SqlCommandExecute(sql2);

                #region 取出符合 其管理區的設備群組 條件的資料
                string sql_e = string.Format(@"
                SELECT DISTINCT 
                    CL.RecordID, CL.LogTime, CL.CardTime, CL.CardNo, 
                    CL.CardVer, CL.CtrlAddr, CL.ReaderNo, CL.PsnNo,
                    CL.PsnName, CL.OrgStruc, CL.EquNo, CL.EquName, CL.TempCardNo, CL.DepName, CL.DepID,
                    CL.LogStatus, OS.OrgNoList
                FROM B01_Cardlog CL 
                LEFT JOIN {0} OS ON OS.OrgIDList = CL.OrgStruc
                WHERE CL.EquNo IN 
                    (
	                    SELECT DISTINCT ED.EquNo 
	                    FROM B01_EquData ED
	                    INNER JOIN B01_EquGroupData EGD ON EGD.EquID = ED.EquID
	                    INNER JOIN B01_MgnEquGroup MEG ON MEG.EquGrpID = EGD.EquGrpID
	                    INNER JOIN B00_SysUserMgns SUMS ON SUMS.MgaID = MEG.MgaID
	                    WHERE SUMS.UserID = '{1}'
                    ) ", strOSA, this.hideUserID.Value);
                #endregion

                #region 取出符合 其管理區的組織架構 條件的資料
                string sql_o = string.Format(@"
                SELECT DISTINCT 
                    CL.RecordID, CL.LogTime, CL.CardTime, CL.CardNo, 
                    CL.CardVer, CL.CtrlAddr, CL.ReaderNo, CL.PsnNo,
                    CL.PsnName, CL.OrgStruc, CL.EquNo, CL.EquName, CL.TempCardNo, CL.DepName, CL.DepID,
                    CL.LogStatus, OS.OrgNoList 
                FROM B01_Cardlog CL 
                LEFT JOIN {0} OS ON OS.OrgIDList = CL.OrgStruc
                WHERE CL.OrgStruc IN 
                    (
                        SELECT DISTINCT OS.OrgIDList  
                        FROM B01_OrgStruc OS 
                        INNER JOIN B01_MgnOrgStrucs MOS ON MOS.OrgStrucID = OS.OrgStrucID 
                        INNER JOIN B00_SysUserMgns SUMS ON SUMS.MgaID = MOS.MgaID 
                        WHERE SUMS.UserID = '{1}' AND OS.OrgStrucNo != '' AND OS.OrgIDList != ''
                    ) ", strOSA, this.hideUserID.Value);
                #endregion

                liSqlPara.Clear();

                // 因應人員登入需求使用
                if (this.sPsnID != "")
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " PsnNo = (SELECT TOP 1 PsnNo FROM B01_Person WHERE PsnID = ?) ";
                    liSqlPara.Add("A:" + this.sPsnID);
                }

                if (sMode == "Normal")
                {
                    #region [一般查詢] 將查詢結果insert到暫存TABLE

                    if (!string.IsNullOrEmpty(ViewState["query_CardTimeSDate"].ToString()))
                    {
                        if (wheresql != "") wheresql += " AND ";
                        wheresql += " CardTime >= ? ";
                        liSqlPara.Add("D:" + ViewState["query_CardTimeSDate"].ToString());
                    }

                    if (!string.IsNullOrEmpty(ViewState["query_CardTimeEDate"].ToString()))
                    {
                        if (wheresql != "") wheresql += " AND ";
                        wheresql += " CardTime <= ? ";
                        liSqlPara.Add("D:" + ViewState["query_CardTimeEDate"].ToString());
                    }

                    if (!string.IsNullOrEmpty(ViewState["query_CardNo_PsnName"].ToString().Trim()))
                    {
                        if (wheresql != "") wheresql += " AND ";
                        wheresql += " (PsnNo LIKE ? OR PsnName LIKE ? OR CardNo LIKE ?) ";
                        liSqlPara.Add("S:" + "%" + ViewState["query_CardNo_PsnName"].ToString().Trim() + "%");
                        liSqlPara.Add("S:" + "%" + ViewState["query_CardNo_PsnName"].ToString().Trim() + "%");
                        liSqlPara.Add("S:" + "%" + ViewState["query_CardNo_PsnName"].ToString().Trim() + "%");
                    }
                    #endregion
                }
                else if (sMode == "ADV")
                {
                    #region [進階查詢] 將查詢結果insert到暫存TABLE

                    // 讀卡時間 開始
                    if (!string.IsNullOrEmpty(ViewState["query_ADVCardTimeSDate"].ToString()))
                    {
                        if (wheresql != "") wheresql += " AND ";
                        wheresql += " CardTime >= ? ";
                        liSqlPara.Add("D:" + ViewState["query_ADVCardTimeSDate"].ToString());
                    }

                    // 讀卡時間 結束
                    if (!string.IsNullOrEmpty(ViewState["query_ADVCardTimeEDate"].ToString()))
                    {
                        if (wheresql != "") wheresql += " AND ";
                        wheresql += " CardTime <= ? ";
                        liSqlPara.Add("D:" + ViewState["query_ADVCardTimeEDate"].ToString());
                    }

                    // 記錄時間 開始
                    if (!string.IsNullOrEmpty(ViewState["query_ADVLogTimeSDate"].ToString()))
                    {
                        if (wheresql != "") wheresql += " AND ";
                        wheresql += " LogTime >= ? ";
                        liSqlPara.Add("D:" + ViewState["query_ADVLogTimeSDate"].ToString());
                    }

                    // 記錄時間 結束
                    if (!string.IsNullOrEmpty(ViewState["query_ADVLogTimeEDate"].ToString()))
                    {
                        if (wheresql != "") wheresql += " AND ";
                        wheresql += " LogTime <= ? ";
                        liSqlPara.Add("D:" + ViewState["query_ADVLogTimeEDate"].ToString());
                    }

                    // 設備代碼、設備名稱
                    if (!string.IsNullOrEmpty(ViewState["query_ADVEquNoEquName"].ToString()))
                    {
                        if (wheresql != "") wheresql += " AND ";
                        wheresql += " ( EquNo LIKE ? OR EquName LIKE ? ) ";
                        liSqlPara.Add("A:" + "%" + ViewState["query_ADVEquNoEquName"].ToString() + "%");
                        liSqlPara.Add("S:" + "%" + ViewState["query_ADVEquNoEquName"].ToString() + "%");
                    }

                    // 進階設備名稱
                    if (!string.IsNullOrEmpty(ViewState["query_ADVEqu"].ToString().Trim()))
                    {
                        ArrayEqu = ViewState["query_ADVEqu"].ToString().Split(',');
                        for (int i = 0; i < ArrayEqu.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(EquSql)) EquSql += ",";
                            EquSql += "?";
                            liSqlPara.Add("S:" + ArrayEqu[i].ToString());
                        }
                        if (wheresql != "") wheresql += " AND ";
                        wheresql += " EquName IN ( " + EquSql + " ) ";
                    }

                    // 人員編號、人員名稱、卡號
                    if (!string.IsNullOrEmpty(ViewState["query_ADVPsnNameCardNo"].ToString()))
                    {
                        if (wheresql != "") wheresql += " AND ";
                        wheresql += " ( PsnNo LIKE ? OR PsnName LIKE ? OR CardNo LIKE ? ) ";
                        liSqlPara.Add("A:" + "%" + ViewState["query_ADVPsnNameCardNo"].ToString() + "%");
                        liSqlPara.Add("S:" + "%" + ViewState["query_ADVPsnNameCardNo"].ToString() + "%");
                        liSqlPara.Add("A:" + "%" + ViewState["query_ADVPsnNameCardNo"].ToString() + "%");
                    }

                    #endregion
                }

                if (wheresql != "")
                {
                    sql_e += " AND ";
                    sql_o += " AND ";
                }

                sql_e += wheresql;
                sql_o += wheresql;

                #region 將符合 該管理區的設備群組 條件的資料和符合 該管理區的組織架構 條件的資料 UNION 起來
                sql = sql_e + " UNION " + sql_o;
                sql = string.Format(@"SELECT * INTO {0} FROM ({1}) RLT", strTmpDT1, sql);
                List<string> liPara = new List<string>();

                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < liSqlPara.Count; j++)
                    {
                        liPara.Add(liSqlPara[j]);
                    }
                }

                intRet = oAcsDB.SqlCommandExecute(sql, liPara);
                #endregion

                #endregion

                #region 從 strTmpDT1 的結果再得到最終結果
                if (intRet > -1)
                {
                    #region 組成SQL
                    sql = string.Format(@"
                        SELECT DISTINCT 
                            CL.RecordID, 
                            CL.CardTime,
                            DepName,
                            CL.PsnNo, 
                            CL.PsnName,
                            CL.CardNo, 
                            CL.CardVer,
                            CL.CtrlAddr,
                            CL.ReaderNo,
                            CL.EquNo, 
                            CL.EquName,
                            CLS.StateDesc LogStatus, 
                            CL.LogTime, 
                            '' DepNo,DepID,
                            CL.OrgNoList,
                            CL.TempCardNo     
                        FROM {0} AS CL 
                        LEFT JOIN B00_CardLogState CLS ON CLS.Code = CL.LogStatus 
                        LEFT JOIN B01_Card CD ON CD.CardNo = CL.CardNo 
                            AND (CD.CardVer = CL.CardVer OR 
                                ((CD.CardVer IS NULL OR CD.CardVer ='') AND 
                                    (CD.CardVer IS NULL OR CL.CardVer = ''))) ", strTmpDT1);

                    liSqlPara.Clear();
                    wheresql = "";

                    #region 區別 查詢 或 進階查詢
                    if (sMode == "Normal")
                    {
                        // 讀卡結果
                        if (!string.IsNullOrEmpty(ViewState["query_LogStatus"].ToString().Trim()))
                        {
                            ArrayLogStatus = ViewState["query_LogStatus"].ToString().Split(',');

                            for (int i = 0; i < ArrayLogStatus.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(LogStatusSql)) LogStatusSql += ",";
                                LogStatusSql += "?";
                                liSqlPara.Add("S:" + ArrayLogStatus[i].ToString());
                            }

                            if (wheresql != "") wheresql += " AND ";
                            wheresql += " CLS.StateDesc IN ( " + LogStatusSql + " ) ";
                        }
                    }
                    else if (sMode == "ADV")
                    {
                        // 進階讀卡結果 
                        if (!string.IsNullOrEmpty(ViewState["query_ADVLogStatus"].ToString().Trim()))
                        {
                            ArrayLogStatus = ViewState["query_ADVLogStatus"].ToString().Split(',');

                            for (int i = 0; i < ArrayLogStatus.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(LogStatusSql)) LogStatusSql += ",";
                                LogStatusSql += "?";
                                liSqlPara.Add("S:" + ArrayLogStatus[i].ToString());
                            }

                            if (wheresql != "") wheresql += " AND ";
                            wheresql += " CLS.StateDesc IN ( " + LogStatusSql + " ) ";
                        }
                    }
                    #endregion

                    if (wheresql != "") sql += " WHERE ";
                    sql += wheresql;

                    #endregion

                    _sqlcommand = sql;
                    DataTable dtTmp = new DataTable();

                    try
                    {
                        isOK = oAcsDB.GetDataTable("Result", sql, liSqlPara, "QueryCardLog.aspx", out dtTmp);

                        sql = string.Format("DROP TABLE {0}", strTmpDT1);
                        oAcsDB.SqlCommandExecute(sql);
                        sql = string.Format("DROP TABLE {0}", strOSA);
                        oAcsDB.SqlCommandExecute(sql);
                    }
                    catch
                    {
                        sql = string.Format("DROP TABLE {0}", strTmpDT1);
                        oAcsDB.SqlCommandExecute(sql);
                        sql = string.Format("DROP TABLE {0}", strOSA);
                        oAcsDB.SqlCommandExecute(sql);
                    }

                    #region 處理單位代碼、名稱

                    sql = @"
                        SELECT DISTINCT OrgID, OrgNo, OrgName, OrgClass  
                        FROM [B01_OrgData] WHERE OrgClass IN ( 'Unit', 'Department')";
                    DataTable tmpOrg = new DataTable();

                    bool isOk = oAcsDB.GetDataTable("tmpOrgData", sql, out tmpOrg);

                    /*
                    if (isOk)
                    {
                        #region 在 gridview 處理單位名稱、單位代碼

                        // dtTmp.Columns.Add("DepNo");
                        // dtTmp.Columns.Add("DepName");

                        #region 判斷以UNIT還是DEPARMENT做標準
                        int intUnit = (int)tmpOrg.Compute("Count(OrgID)", "OrgClass='Unit'");
                        int intDepartment = (int)tmpOrg.Compute("Count(OrgID)", "OrgClass='Department'");

                        string strFlag = "U";
                        if (intUnit > intDepartment)
                        {
                            strFlag = "U";
                        }
                        else
                        {
                            strFlag = "D";
                        }
                        #endregion

                        foreach (DataRow dr in dtTmp.Rows)
                        {
                            if (dr["OrgNoList"].ToString() != "")
                            {
                                // 用反斜線區分開 \u005C = 反斜線
                                string[] tmp_OrgNoList = dr["OrgNoList"].ToString().Split('\u005C');
                                string strOrgNo = "";

                                for (int j = 0; j <= tmp_OrgNoList.GetUpperBound(0); j++)
                                {
                                    if (!tmp_OrgNoList[j].Equals(""))
                                    {
                                        if (tmp_OrgNoList[j].Substring(0, 1).ToUpper().Equals(strFlag))
                                        {
                                            strOrgNo = tmp_OrgNoList[j].Trim();
                                            DataRow[] drs = tmpOrg.Select("OrgNo = '" + strOrgNo + "'");

                                            if (drs.GetUpperBound(0) != -1)
                                            {
                                                foreach (DataRow dr1 in drs)
                                                {
                                                    dr["DepNo"] = dr1["OrgNo"];
                                                    dr["DepName"] = dr1["OrgName"];
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    */
                    #endregion
                    

                    if (isOK)
                    {
                        if (dtTmp.Rows.Count > 0)
                        {
                            #region 處理進階查詢的條件(二)
                            string strCondition = "";

                            if (sMode == "Normal")
                            {

                            }
                            else if (sMode == "ADV")
                            {
                                // 部門代碼、部門名稱
                                if (!string.IsNullOrEmpty(ViewState["query_ADVDepNoDepName"].ToString()))
                                {
                                    if (strCondition != "") strCondition += " AND ";
                                    strCondition += string.Format(@"
                                        (DepNo LIKE '%{0}%' OR DepName LIKE '%{0}%') ",
                                        ViewState["query_ADVDepNoDepName"].ToString());
                                }

                                // 進階的部門名稱(可多選那個)
                                if (!string.IsNullOrEmpty(ViewState["query_ADVDep"].ToString().Trim()))
                                {
                                    ArrayDep = ViewState["query_ADVDep"].ToString().Split(',');

                                    for (int i = 0; i < ArrayDep.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(DepSql)) DepSql += ",";
                                        DepSql += "'" + ArrayDep[i].ToString() + "'";
                                    }

                                    if (strCondition != "") strCondition += " AND ";
                                    strCondition += " DepID IN ( " + DepSql + " ) ";
                                }
                            }

                            dtTmp.DefaultView.RowFilter = strCondition;
                            #endregion

                            // 處理排序
                            dtTmp.DefaultView.Sort = SortExpression + " " + SortDire;
                            CardLogTable = new DataTable();
                            CardLogTable = dtTmp.DefaultView.ToTable();
                            //Session["CardLogTable"] = CardLogTable;
                            _datacount = CardLogTable.Rows.Count;
                            hDataRowCount.Value = _datacount.ToString();
                            ViewState["FlagExport"] = "OK";

                            if (bMode)
                            {
                                ExportExcel(CardLogTable);
                            }
                            else
                            {
                                GirdViewDataBind(this.MainGridView, CardLogTable);
                                UpdatePanel1.Update();
                            }
                        }
                        else
                        {
                            ViewState["FlagExport"] = "";
                            EmptyCondition();
                        }
                    }
                }
                else
                {
                    ViewState["FlagExport"] = "";
                    EmptyCondition();
                }
                #endregion
            }
            catch(Exception ex)
            {
                Sa.Fun.SaveExceptionLog(ex.GetBaseException());
                sql = string.Format("DROP TABLE {0}", strTmpDT1);
                oAcsDB.SqlCommandExecute(sql);
                sql = string.Format("DROP TABLE {0}", strOSA);
                oAcsDB.SqlCommandExecute(sql);                
            }
            finally
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI", "$.unblockUI();", true);
            }
        }//end Query

        public void QueryByPerson(string sMode, bool bMode, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "", LogStatusSql = "", DepSql = "", EquSql = "";
            string[] ArrayLogStatus, ArrayDep, ArrayEqu;
            bool bQueryCondition = false;
            List<string> liSqlPara = new List<string>();
            ViewState["query_sMode"] = sMode;

            string strUnionTB = Pub.ReturnNewNestedSerachSQL("0601", ViewState["query_CardTimeSDate"].ToString(), ViewState["query_CardTimeEDate"].ToString());

            #region Process String
            if (SortExpression == "EquNo" || SortExpression == "EquName")
                SortExpression = "B01_EquData." + SortExpression;
            sql = @" SELECT ROW_NUMBER() OVER(ORDER BY " + SortExpression + " " + SortDire + @") AS NewIDNum,
                B01_CardLog.RecordID, B01_CardLog.CardTime,B01_CardLog.TempCardNo,
                B01_CardLog.DepName, B01_CardLog.PsnNo, B01_CardLog.PsnName,
                B01_CardLog.CardNo, B01_CardLog.CardVer, B01_CardLog.ReaderNo,
                B01_EquData.EquNo, B01_EquData.EquName,
                B00_CardLogState.StateDesc AS LogStatus, B01_CardLog.LogTime
                FROM " + strUnionTB + " AS B01_CardLog " +
                @"INNER JOIN B00_CardLogState ON B00_CardLogState.Code = B01_CardLog.LogStatus

                INNER JOIN (SELECT B01_EquData.EquNo, B01_EquData.EquName FROM B01_EquData
                INNER JOIN B01_EquGroupData ON B01_EquGroupData.EquID = B01_EquData.EquID
                INNER JOIN B01_MgnEquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroupData.EquGrpID
                INNER JOIN B00_SysUserMgns ON B00_SysUserMgns.MgaID = B01_MgnEquGroup.MgaID
                GROUP BY B01_EquData.EquNo, B01_EquData.EquName) AS B01_EquData ON B01_EquData.EquNo = B01_CardLog.EquNo 

                INNER JOIN (SELECT OrgData.OrgID FROM OrgStrucAllData('Department') AS OrgData
                INNER JOIN B01_MgnOrgStrucs ON B01_MgnOrgStrucs.OrgStrucID = OrgData.OrgStrucID
                INNER JOIN B00_SysUserMgns ON B00_SysUserMgns.MgaID = B01_MgnOrgStrucs.MgaID                  
                GROUP BY OrgData.OrgID) AS B01_OrgData ON B01_OrgData.OrgID = B01_CardLog.DepID  
                AND PsnNo=(SELECT TOP 1 PsnNo FROM B01_Person WHERE PsnID=?)";

            #region DataAuth
            liSqlPara.Add("S:" + this.sPsnID);
            #endregion

            if (sMode == "Normal")
            {
                #region 一般查詢

                if (!string.IsNullOrEmpty(ViewState["query_CardTimeSDate"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_CardLog.CardTime >= ? ) ";
                    liSqlPara.Add("D:" + ViewState["query_CardTimeSDate"].ToString());
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_CardTimeEDate"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_CardLog.CardTime <= ? ) ";
                    liSqlPara.Add("D:" + ViewState["query_CardTimeEDate"].ToString());
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_LogStatus"].ToString().Trim()))
                {
                    ArrayLogStatus = ViewState["query_LogStatus"].ToString().Split(',');
                    for (int i = 0; i < ArrayLogStatus.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(LogStatusSql)) LogStatusSql += ",";
                        LogStatusSql += "?";
                        liSqlPara.Add("S:" + ArrayLogStatus[i].ToString());
                        bQueryCondition = true;
                    }
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B00_CardLogState.StateDesc IN ( " + LogStatusSql + " ) ) ";
                    bQueryCondition = true;
                }
                #endregion
            }
            else if (sMode == "ADV")
            {
                #region 進階查詢

                if (!string.IsNullOrEmpty(ViewState["query_ADVCardTimeSDate"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_CardLog.CardTime >= ? ) ";
                    liSqlPara.Add("D:" + ViewState["query_ADVCardTimeSDate"].ToString());
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVCardTimeEDate"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_CardLog.CardTime <= ? ) ";
                    liSqlPara.Add("D:" + ViewState["query_ADVCardTimeEDate"].ToString());
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVLogTimeSDate"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_CardLog.LogTime >= ? ) ";
                    liSqlPara.Add("S:" + ViewState["query_ADVLogTimeSDate"].ToString());
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVLogTimeEDate"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_CardLog.LogTime <= ? ) ";
                    liSqlPara.Add("S:" + ViewState["query_ADVLogTimeEDate"].ToString());
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVDepNoDepName"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_CardLog.DepID LIKE ? OR B01_CardLog.DepName LIKE ? ) ";
                    liSqlPara.Add("S:" + "%" + ViewState["query_ADVDepNoDepName"].ToString() + "%");
                    liSqlPara.Add("S:" + "%" + ViewState["query_ADVDepNoDepName"].ToString() + "%");
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVDep"].ToString().Trim()))
                {
                    ArrayDep = ViewState["query_ADVDep"].ToString().Split(',');
                    for (int i = 0; i < ArrayDep.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(DepSql)) DepSql += ",";
                        DepSql += "?";
                        liSqlPara.Add("S:" + ArrayDep[i].ToString());
                    }
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_CardLog.DepID IN ( " + DepSql + " ) )";
                    bQueryCondition = true;
                }


                if (!string.IsNullOrEmpty(ViewState["query_ADVEquNoEquName"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_EquData.EquNo LIKE ? OR B01_EquData.EquName LIKE ? ) ";
                    liSqlPara.Add("S:" + "%" + ViewState["query_ADVEquNoEquName"].ToString() + "%");
                    liSqlPara.Add("S:" + "%" + ViewState["query_ADVEquNoEquName"].ToString() + "%");
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVEqu"].ToString().Trim()))
                {
                    ArrayEqu = ViewState["query_ADVEqu"].ToString().Split(',');
                    for (int i = 0; i < ArrayEqu.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(EquSql)) EquSql += ",";
                        EquSql += "?";
                        liSqlPara.Add("S:" + ArrayEqu[i].ToString());
                    }
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_EquData.EquName IN ( " + EquSql + " ) )";
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVPsnNameCardNo"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_CardLog.PsnNo LIKE ? OR B01_CardLog.PsnName LIKE ? OR B01_CardLog.CardNo LIKE ?) ";
                    liSqlPara.Add("S:" + "%" + ViewState["query_ADVPsnNameCardNo"].ToString() + "%");
                    liSqlPara.Add("S:" + "%" + ViewState["query_ADVPsnNameCardNo"].ToString() + "%");
                    liSqlPara.Add("S:" + "%" + ViewState["query_ADVPsnNameCardNo"].ToString() + "%");
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVLogStatus"].ToString().Trim()))
                {
                    ArrayLogStatus = ViewState["query_ADVLogStatus"].ToString().Split(',');
                    for (int i = 0; i < ArrayLogStatus.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(LogStatusSql)) LogStatusSql += ",";
                        LogStatusSql += "?";
                        liSqlPara.Add("S:" + ArrayLogStatus[i].ToString());
                    }
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B00_CardLogState.StateDesc IN ( " + LogStatusSql + " ) )";
                    bQueryCondition = true;
                }

                #endregion
            }

            if (bQueryCondition)
            {
                if (wheresql != "")
                    sql += " WHERE ";
            }
            else
            {
                if (wheresql != "")
                {
                    sql += " WHERE 1 = 0 AND ";
                }
                else
                {
                    sql += " WHERE 1 = 0 ";
                }
            }

            _sqlcommand = sql += wheresql;
            //_datacount = oAcsDB.DataCount(_sqlcommand, liSqlPara);
            //hDataRowCount.Value = _datacount.ToString();

            // (一)先用 _sqlcommand 得到 dtTmp，然後得到總筆數
            DataTable dtTmp = new DataTable();

            oAcsDB.GetDataTable("ALL", _sqlcommand, liSqlPara, out dtTmp);

            _datacount = dtTmp.Rows.Count;
            hDataRowCount.Value = _datacount.ToString();
            #endregion

            if (bMode == true)
            {
                if (dtTmp.Rows.Count != 0)
                {
                    ExportExcel(dtTmp);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('無資料可匯出!!');location.href='QueryCardLog.aspx';", true);
                }
            }
            else
            {
                string strCondition =
                    @" NewIDNum >= " + Convert.ToString((_pagesize * _pageindex + 1)) +
                    " AND NewIDNum <= " + Convert.ToString((_pagesize * (_pageindex + 1)));

                dtTmp.DefaultView.RowFilter = strCondition;
                CardLogTable = dtTmp.DefaultView.ToTable();

                GirdViewDataBind(this.MainGridView, CardLogTable);
                UpdatePanel1.Update();
            }
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI", "$.unblockUI();", true);
        }

        #endregion

        #region LoadData
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadData(string SelectValue)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            string[] TableData = null;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            string strUnionTB = Pub.ReturnNewNestedSerachSQL("0601_LoadData", "", "");

            #region Process String
            //sql = @" SELECT 
            //         B01_CardLog.CardTime,CardLogState.StateDesc AS LogStatus,
            //         OrgStrucAllData.OrgStrucID AS DepID, OrgStrucAllData.OrgName AS DepName,
            //         Person.PsnNo, Person.PsnName,
            //         Card.CardNo, Card.CardVer,
            //         EquData.EquNo, EquData.EquName,
            //         B01_CardLog.LogTime 
            //         FROM " + strUnionTB + " AS B01_CardLog " +
            //         @"INNER JOIN dbo.B01_Card AS Card ON Card.CardNo = B01_CardLog.CardNo AND (Card.CardVer = B01_CardLog.CardVer OR (Card.CardVer IS NULL AND B01_CardLog.CardVer IS NULL ))
            //         INNER JOIN dbo.B01_Person AS Person ON Person.PsnID = Card.PsnID
            //         INNER JOIN dbo.B01_EquData AS EquData ON EquData.EquNo = B01_CardLog.EquNo
            //         INNER JOIN OrgStrucAllData('Department') AS OrgStrucAllData ON OrgStrucAllData.OrgStrucID = Person.OrgStrucID
            //         INNER JOIN B00_CardLogState AS CardLogState ON CardLogState.Code = B01_CardLog.LogStatus 
            //         INNER JOIN B01_MgnOrgStrucs AS MgnOrgStrucs ON MgnOrgStrucs.OrgStrucID = OrgStrucAllData.OrgStrucID
            //         INNER JOIN B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID
            //         INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID
            //         INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnOrgStrucs.MgaID AND SysUserMgns.MgaID = MgnEquGroup.MgaID
            //         WHERE B01_CardLog.RecordID = ? ";

            ////ADV_TextBox_CardTime.value = DataArray[0];
            ////ADV_TextBox_LogStatus.value = DataArray[1];
            ////ADV_TextBox_DepNo.value = DataArray[2];
            ////ADV_TextBox_DepName.value = DataArray[3];
            ////ADV_TextBox_PsnNo.value = DataArray[4];
            ////ADV_TextBox_PsnName.value = DataArray[5];
            ////ADV_TextBox_CardNo.value = DataArray[6];
            ////ADV_TextBox_CardVer.value = DataArray[7];
            ////ADV_TextBox_EquNo.value = DataArray[8];
            ////ADV_TextBox_EquName.value = DataArray[9];
            ////ADV_TextBox_LogTime.value = DataArray[10];

            sql = @"
                SELECT 
                    CL.RecordID,
	                CL.CardTime,
                    PN.PsnName,
	                CLS.StateDesc AS LogStatus, 
	                PN.PsnNo, 
	                CD.CardNo, 
	                CD.CardVer,
	                ED.EquNo, 
	                ED.EquName,
                    CL.LogTime, 
	                OS.OrgStrucNo  
                FROM B01_CardLog CL 
                LEFT JOIN B00_CardLogState CLS ON CLS.Code = CL.LogStatus 
                LEFT JOIN B01_Card CD ON CD.CardNo = CL.CardNo 
	                AND (CD.CardVer = CL.CardVer OR (CD.CardVer IS NULL AND CL.CardVer IS NULL )) 
                LEFT JOIN B01_Person AS PN ON PN.PsnID = CD.PsnID 
                LEFT JOIN B01_EquData AS ED ON ED.EquNo = CL.EquNo 
                LEFT JOIN B01_OrgStruc AS OS ON	OS.OrgStrucID = PN.OrgStrucID 
                WHERE CL.RecordID = ? ";

            liSqlPara.Add("S:" + SelectValue.Trim());
            bool isOk = oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (isOk)
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        TableData = new string[11];
                        TableData[0] = DateTime.Parse(dr.ToString("CardTime")).ToString("yyyy/MM/dd HH:mm:ss");
                        TableData[1] = DateTime.Parse(dr.ToString("LogTime")).ToString("yyyy/MM/dd HH:mm:ss");
                        TableData[2] = dr.ToString("LogStatus");
                        TableData[3] = dr.ToString("PsnNo");
                        TableData[4] = dr.ToString("PsnName");
                        TableData[5] = dr.ToString("CardNo");
                        TableData[6] = dr.ToString("CardVer");
                        TableData[7] = dr.ToString("EquNo");
                        TableData[8] = dr.ToString("EquName");
                        TableData[9] = "";      // OrgNo
                        TableData[10] = "";     // OrgName

                        string strOrgStrucNo = dr.ToString("OrgStrucNo");
                        if (strOrgStrucNo != "")
                        {
                            string[] strTemp = strOrgStrucNo.Split('_');

                            for (int j = 0; j <= strTemp.GetUpperBound(0); j++)
                            {
                                if (!strTemp[j].Equals("") && strTemp[j].Substring(0, 1).ToUpper().Equals("U"))
                                {
                                    TableData[9] = strTemp[j].Trim();   // OrgNo


                                    string strSQL = "SELECT OrgName FROM B01_OrgData WHERE OrgNo='{0}'";
                                    TableData[10] = oAcsDB.GetStrScalar(string.Format(strSQL, TableData[9]));
                                }
                            }
                        }
                    }
                }
                else
                {
                    TableData = new string[2];
                    TableData[0] = "Saho_SysErrorMassage";
                    TableData[1] = "系統中無此資料！";
                }
            }
            else
            {
                TableData = new string[2];
                TableData[0] = "Saho_SysErrorMassage";
                TableData[1] = "讀取資料庫失敗！";
            }


            //if (dr.Read())
            //{
            //    TableData = new string[dr.DataReader.FieldCount];
            //    for (int i = 0; i < dr.DataReader.FieldCount; i++)
            //    {
            //        switch (i)
            //        {
            //            case 0:
            //            case 1:
            //                TableData[i] = DateTime.Parse(dr.DataReader[i].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
            //                break;
            //            default:
            //                TableData[i] = dr.DataReader[i].ToString();
            //                break;
            //        }
            //    }
            //}
            //else
            //{

            //}
            #endregion

            return TableData;
        }
        #endregion

        #region GirdViewDataBind
        public void GirdViewDataBind(GridView ProcessGridView, DataTable dt)
        {
            if (dt.Rows.Count != 0)//Gridview中有資料
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
        #endregion

        #region Create DropDownList_LogStatusItem 和 DropDownList_ADVLogStatusItem
        private void CreateDropDownList_StatusItem()
        {
            ListItem Item = new ListItem();
            ListItem Item2 = new ListItem();
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt = new DataTable();
            List<string> liSqlPara = new List<string>();

            DropDownList_LogStatus.Items.Clear();
            ADVDropDownList_LogStatus.Items.Clear();

            sql = @" SELECT DISTINCT Code, StateDesc FROM B00_CardLogState AS CardLogState ";

            oAcsDB.GetDataTable("StatusDropItem", sql, liSqlPara, out dt);

            if (dt.Rows.Count > 0)
            {
                // 將正常開門和使用未授權卡排在上面
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Code"].ToString().Equals("0") || dr["Code"].ToString().Equals("160"))
                    {
                        Item = new ListItem();
                        Item.Text = dr["StateDesc"].ToString();
                        Item.Value = dr["Code"].ToString();
                        DropDownList_LogStatus.Items.Add(Item);

                        Item2 = new ListItem();
                        Item2.Text = dr["StateDesc"].ToString();
                        Item2.Value = dr["Code"].ToString();
                        ADVDropDownList_LogStatus.Items.Add(Item2);
                    }

                }

                // 其餘的排在下面
                foreach (DataRow dr in dt.Rows)
                {
                    if (!(dr["Code"].ToString().Equals("0") || dr["Code"].ToString().Equals("160")))
                    {
                        Item = new ListItem();
                        Item.Text = dr["StateDesc"].ToString();
                        Item.Value = dr["Code"].ToString();
                        DropDownList_LogStatus.Items.Add(Item);

                        Item2 = new ListItem();
                        Item2.Text = dr["StateDesc"].ToString();
                        Item2.Value = dr["Code"].ToString();
                        ADVDropDownList_LogStatus.Items.Add(Item2);
                    }
                }
            }
            else
            {
                DropDownList_LogStatus.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
                ADVDropDownList_LogStatus.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
            }
        }
        #endregion

        #region CreateDropDownList_LogStatusItem
        private void CreateDropDownList_LogStatusItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            DropDownList_LogStatus.Items.Clear();

            #region Process String
            sql = @" SELECT Code, StateDesc FROM B00_CardLogState AS CardLogState ";
            #endregion

            oAcsDB.GetDataTable("LogStatusDropItem", sql, liSqlPara, out dt);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Item = new ListItem();
                    Item.Text = dr["StateDesc"].ToString();
                    Item.Value = dr["Code"].ToString();
                    DropDownList_LogStatus.Items.Add(Item);
                }
            }
            else
            {
                DropDownList_LogStatus.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
            }
        }
        #endregion

        #region CreateDropDownList_ADVLogStatusItem
        private void CreateDropDownList_ADVLogStatusItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            ADVDropDownList_LogStatus.Items.Clear();

            #region Process String
            sql = @" SELECT Code, StateDesc FROM B00_CardLogState AS CardLogState ";
            #endregion

            oAcsDB.GetDataTable("ADVLogStatusDropItem", sql, liSqlPara, out dt);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Item = new System.Web.UI.WebControls.ListItem();
                    Item.Text = dr["StateDesc"].ToString();
                    Item.Value = dr["Code"].ToString();
                    ADVDropDownList_LogStatus.Items.Add(Item);
                }
            }
            else
            {
                ADVDropDownList_LogStatus.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
            }
        }
        #endregion

        #region CreateDropDownList_DepItem
        private void CreateDropDownList_DepItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            ADVDropDownList_Dep.Items.Clear();

            #region Process String
            string sqlouttable = "";
            sql = @" SELECT DISTINCT
                     OrgStrucAllData.OrgID AS DepID, OrgStrucAllData.OrgName AS DepName,UserID,OrgStrucAllData.OrgNo AS DepNo
                     FROM  OrgStrucAllData('@Type') AS OrgStrucAllData
                     INNER JOIN B01_MgnOrgStrucs AS MgnOrgStrucs ON MgnOrgStrucs.OrgStrucID = OrgStrucAllData.OrgStrucID
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnOrgStrucs.MgaID  ";
            sqlouttable += "SELECT * FROM (";
            sqlouttable += sql.Replace("@Type", "Department");
            sqlouttable += " UNION ";
            sqlouttable += sql.Replace("@Type", "Unit");
            sqlouttable += " UNION ";
            sqlouttable += sql.Replace("@Type", "Title");
            sqlouttable += ") AS ResultTable ";
            #region DataAuth
            if (wheresql != "") wheresql += " AND ";
            wheresql += " (UserID = ? ) ";
            wheresql += " AND DepID<>'' ";
            liSqlPara.Add("S:" + this.hideUserID.Value);
            #endregion

            #endregion

            sqlouttable += " WHERE " + wheresql + " ORDER BY DepName ";

            oAcsDB.GetDataTable("DepDropItem", sqlouttable, liSqlPara, out dt);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Item = new System.Web.UI.WebControls.ListItem();
                    Item.Text = dr["DepName"].ToString()+" [" + dr["DepNo"].ToString() + "]";
                    Item.Value = dr["DepID"].ToString();
                    ADVDropDownList_Dep.Items.Add(Item);
                }
            }
            else
            {
                ADVDropDownList_Dep.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
            }
        }
        #endregion

        #region CreateDropDownList_EquItem
        private void CreateDropDownList_EquItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            ADVDropDownList_Equ.Items.Clear();

            #region Process String
            sql = @" SELECT DISTINCT
                     EquData.EquNo, EquData.EquName
                     FROM B01_EquData AS EquData
                     INNER JOIN B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID
                     INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID ";

            #region DataAuth
            if (wheresql != "") wheresql += " AND ";
            wheresql += " (SysUserMgns.UserID = ? ) ";
            liSqlPara.Add("S:" + this.hideUserID.Value);
            #endregion

            #endregion

            sql += " WHERE " + wheresql + " ORDER BY EquData.EquNo ";

            oAcsDB.GetDataTable("EquDropItem", sql, liSqlPara, out dt);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Item = new ListItem();
                    Item.Text = dr["EquName"].ToString();
                    Item.Value = dr["EquNo"].ToString();
                    ADVDropDownList_Equ.Items.Add(Item);
                }
            }
            else
            {
                ADVDropDownList_Equ.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
            }
        }
        #endregion

        #region ExportExcel
        public void ExportExcel(DataTable ProcDt)
        {
            try
            {
                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("CardLog");
                DataTable dtCardLog = ProcDt;

                #region 去掉不必要的欄位
                string strDels = "";
                foreach (DataColumn dc in dtCardLog.Columns)
                {
                    if (dc.ColumnName.Equals("DepNo") || dc.ColumnName.Equals("OrgNoList"))
                    {
                        strDels += dc.ColumnName + "|";
                    }
                }

                if (strDels != "")
                {
                    string[] strDel = strDels.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string str in strDel)
                    {
                        dtCardLog.Columns.Remove(str);
                    }
                }
                #endregion

                //Title
                ws.Cells[1, 1].Value = "讀卡時間";
                ws.Cells[1, 2].Value = "部門名稱";
                ws.Cells[1, 3].Value = "人員編號";
                ws.Cells[1, 4].Value = "人員姓名";
                ws.Cells[1, 5].Value = "卡號";
                ws.Cells[1, 6].Value = "臨時卡號";
                ws.Cells[1, 7].Value = "版次";
                ws.Cells[1, 8].Value = "控制器編號";
                ws.Cells[1, 9].Value = "讀卡機編號";
                ws.Cells[1, 10].Value = "設備編號";
                ws.Cells[1, 11].Value = "設備名稱";
                ws.Cells[1, 12].Value = "讀卡結果";
                ws.Cells[1, 13].Value = "記錄時間";

                //Content
                for (int i = 0, iCount = dtCardLog.Rows.Count; i < iCount; i++)
                {
                    for (int j = 1, jCount = dtCardLog.Rows[i].ItemArray.Length; j < jCount; j++)
                    {
                        ws.Cells[i + 2, j].Value = dtCardLog.Rows[i].ItemArray[j].ToString().Replace("&nbsp;", "").Trim();
                    }
                }

                ws.Cells.AutoFitColumns(); //自動欄寬
                Response.Clear();
                Response.AddHeader("content-disposition", "attachment; filename=CardLog.xlsx");
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.BinaryWrite(pck.GetAsByteArray());
                //Response.End();
                //System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch(System.Threading.ThreadAbortException ce)
            {
                Sa.Fun.SaveExceptionLog(ce.GetBaseException());
            }
            finally
            {
                Response.End();
            }
        }
        #endregion

        #region 排序欄位及條件
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
        #endregion

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
            dtEmpty.Columns.Add("RecordID");
            dtEmpty.Columns.Add("CardTime");
            dtEmpty.Columns.Add("LogTime");
            dtEmpty.Columns.Add("LogStatus");
            dtEmpty.Columns.Add("PsnNo");
            dtEmpty.Columns.Add("PsnName");
            dtEmpty.Columns.Add("CardNo");
            dtEmpty.Columns.Add("CardVer");
            dtEmpty.Columns.Add("EquNo");
            dtEmpty.Columns.Add("EquName");
            dtEmpty.Columns.Add("OrgStrucNo");
            dtEmpty.Columns.Add("DepName");
            dtEmpty.Columns.Add("DepNo");
            dtEmpty.Columns.Add("TempCardNo");

            GirdViewDataBind(this.MainGridView, dtEmpty);
            UpdatePanel1.Update();
            #endregion
        }

        #endregion
    }
}