using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sa.DB;
using OfficeOpenXml;

namespace SahoAcs
{
    public partial class QuerySysLog : System.Web.UI.Page
    {
        #region 一.宣告
        private int _pagesize = 100, _datacount = 1, _pageindex = 0, _pagecount = 0;
        string _sqlcommand;
        AjaxControlToolkit.ToolkitScriptManager m_ToolKitScriptManager;
        DataTable SysLogTable = null;
        #endregion

        #region 二.網頁

        #region 2-1A.網頁：事件方法
        /// <summary>
        /// 初始化網頁相關的動作
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterComponeAndScript();
            RegisterWindowsAndButton();
            ChangeLanguage();
            popInput0_STime.SetWidth(192);
            popInput0_ETime.SetWidth(192);
            this.MainGridView.PageSize = _pagesize;

            if (!IsPostBack && !m_ToolKitScriptManager.IsInAsyncPostBack)  //網頁初載入時相關的動作
            {
                QueryInput_KeyWord.Text = "";
                hSaveComplexQueryData.Value = "";

                hUserID.Value = Session["UserID"].ToString();
                hOwnerList.Value = Session["OwnerList"].ToString();
                ViewState["query_SysLogTable"] = "";
                ViewState["SortExpression"] = "LogTime";
                ViewState["SortDire"] = "DESC";
                //Query(false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
            }
            else                                                           //網頁PostBack時相關的動作
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];    //取得網頁目標
                string sFormArg = Request.Form["__EVENTARGUMENT"];  //取得網頁參數

                //處理按下查詢或複合查詢按鈕後的相關動作
                if ((sFormTarget == this.QueryButton.ClientID) || (sFormTarget == this.ComplexQueryButton.ClientID))
                {
                    if (sFormTarget == this.QueryButton.ClientID)
                        this.QueryInput_KeyWord.Text = this.QueryInput_KeyWord.Text.Trim();
                    else if (sFormTarget == this.ComplexQueryButton.ClientID)
                        this.QueryInput_KeyWord.Text = "";

                    Query(false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                }
                else
                {
                    string[] arrControl = sFormTarget.Split('$');

                    if (arrControl.Length == 5)
                    {
                        _pageindex = Convert.ToInt32(arrControl[4].Split('_')[1]);
                        Query(false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    }
                }
            }
        }

        protected void ExcelButton_Click(object sender, EventArgs e)
        {
            Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }
        #endregion

        #region 2-1B.網頁：自訂方法
        /// <summary>
        /// *覆寫此方法用來修正'XX'型別必須置於含有runat=server的標記屬性
        /// </summary>
        /// <param name="control">伺服器控制項</param>
        public override void VerifyRenderingInServerForm(Control control)
        {
            //覆寫此方法用來修正'XX'型別必須置於含有runat=server的標記屬性
        }

        /// <summary>
        /// *開啟對話視窗的JavaScript語法
        /// </summary>
        private void OpenDialog_Js()
        {
            string jstr = "";

            jstr = @"
                    function OpenDialogAdd(theURL,win_width,win_height) { 
                        var PosX = (screen.width-win_width)/2; 
                        var PosY = (screen.height-win_height)/2; 
                        features = 'dialogWidth='+win_width+',dialogHeight='+win_height+',dialogTop='+PosY+',dialogLeft='+PosX+';' 
                        window.showModalDialog(theURL, '', features);
                    }

                    function OpenDialogEdit(theURL,key,win_width,win_height) { 
                        var PosX = (screen.width-win_width)/2; 
                        var PosY = (screen.height-win_height)/2; 
                        features = 'dialogWidth='+win_width+',dialogHeight='+win_height+',dialogTop='+PosY+',dialogLeft='+PosX+';' 
                        window.showModalDialog(theURL+key, '', features);
                    }";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "OpenDialog_Js", jstr, true);
        }

        /// <summary>
        /// 註冊公用的元件與JavaScript檔案
        /// </summary>
        private void RegisterComponeAndScript()
        {
            m_ToolKitScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            m_ToolKitScriptManager.EnablePageMethods = true;
            m_ToolKitScriptManager.RegisterAsyncPostBackControl(QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetViewWindowMode('');";
            js += "\nSetComplexQueryWindowMode('');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("QueryCardLogScript", "QuerySysLog.js");//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>
        /// 註冊網頁的視窗與按鈕動作
        /// </summary>
        private void RegisterWindowsAndButton()
        {
            //註冊主要作業畫面的按鈕動作：網頁畫面設計一
            QueryButton.Attributes["onClick"] = "SelectState(); return false;";
            ComplexQueryButton.Attributes["onClick"] = "CallComplexQueryWindow('" + this.GetLocalResourceObject("lblShowAdv").ToString() + "'); return false;";
            ViewButton.Attributes["onClick"] = "CallViewWindow('" + this.GetLocalResourceObject("lblViewSysLog").ToString() + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";

            //註冊複合查詢畫面的按鈕動作：網頁畫面設計二
            ImgCloseButton0.Attributes["onClick"] = "CancelTrigger0.click(); return false;";
            popBtn0_Query.Attributes["onClick"] = "SaveComplexQueryData(); SetComplexQueryWheresql(); return false;";
            popBtn0_Cancel.Attributes["onClick"] = "CancelTrigger0.click(); return false;";
            popBtn0_ClearQueryParam.Attributes["onClick"] = "SetComplexQueryWindowMode(''); return false;";

            //註冊彈出作業畫面的按鈕動作：網頁畫面設計三
            ImgCloseButton1.Attributes["onClick"] = "SetViewWindowMode(''); CancelTrigger1.click(); return false;";
            popBtn1_Exit.Attributes["onClick"] = "SetViewWindowMode(''); CancelTrigger1.click(); return false;";

            Pub.SetModalPopup(ModalPopupExtender0, 0);
            Pub.SetModalPopup(ModalPopupExtender1, 1);
        }

        /// <summary>
        /// 切換網頁的顯示語系
        /// </summary>
        private void ChangeLanguage()
        {
            //等待新系統整個完成後再一併處理
        }
        #endregion

        #endregion

        #region 三.元件
        #region 3-1A.元件-動態：事件方法
        #endregion

        #region 3-1B.元件-動態：自訂方法
        /// <summary>
        /// 建立「查詢 系統事件」網頁相關的元件
        /// </summary>
        //public void CreateQueryCardLogPageComponent()

        #endregion

        #region 3-2A.元件-表格：事件方法
        /// <summary>
        /// *處理GridView控制項在變更分頁索引時的相關動作
        /// </summary>
        protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            MainGridView.PageIndex = e.NewPageIndex;

            if (MainGridView.Rows[0].Cells.Count == MainGridView.HeaderRow.Cells.Count)
                MainGridView.DataBind();
        }

        /// <summary>
        /// 處理GridView控制項在完成資料繫結後的相關動作
        /// </summary>
        protected void GridView_Data_DataBound(object sender, EventArgs e)
        {
            td_showGridView.Attributes["colspan"] = MainGridView.Columns.Count.ToString();
        }

        /// <summary>
        /// 處理GridView控制項在排序的相關動作
        /// </summary>
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
            Query(false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }

        /// <summary>
        /// 處理資料列繫結至GridView控制項資料時的相關動作
        /// </summary>
        public void GridView_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region A.設定表格的標題部份
                case DataControlRowType.Header:
                    #region A-1.設定欄位隱藏與寬度
                    e.Row.Cells[0].Visible = false;

                    e.Row.Cells[0].Width = 50;      //設備識別碼
                    e.Row.Cells[1].Width = 180;     //記錄時間
                    e.Row.Cells[2].Width = 120;     //記錄類型
                    e.Row.Cells[3].Width = 90;      //使用者帳號
                    e.Row.Cells[4].Width = 90;      //使用者名稱
                    e.Row.Cells[5].Width = 100;     //程式名稱
                    e.Row.Cells[6].Width = 120;     //來源IP
                    e.Row.Cells[7].Width = 90;      //設備編號
                    e.Row.Cells[8].Width = 90;      //設備名稱
                    e.Row.Cells[9].Width = 90;      //記錄資訊
                    //e.Row.Cells[10].Width = 180;    //記錄說明    //不要設定
                    #endregion

                    #region A-2.*排序條件Header加工
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

                    #region A-3.*寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);

                    e.Row.CssClass = "GVStyle";
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    li_header.Text = Header_sw.ToString();
                    #endregion
                    break;
                #endregion

                #region B.設定表格的資料部份
                case DataControlRowType.DataRow:
                    #region B-1.指定資料列的代碼
                    DataRowView drvRow = (DataRowView)e.Row.DataItem;
                    GridViewRow gvrRow = e.Row;

                    gvrRow.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    gvrRow.ID = "GVRow_" + drvRow.Row["RecordID"].ToString().Trim();  //設定資料列代碼
                    #endregion

                    #region B-2.設定欄位隱藏與寬度
                    e.Row.Cells[0].Visible = false;

                    e.Row.Cells[0].Width = 53;      //設備識別碼
                    e.Row.Cells[1].Width = 183;     //記錄時間
                    e.Row.Cells[2].Width = 124;     //記錄類型
                    e.Row.Cells[3].Width = 94;      //使用者帳號
                    e.Row.Cells[4].Width = 94;      //使用者名稱
                    e.Row.Cells[5].Width = 104;     //程式名稱
                    e.Row.Cells[6].Width = 124;     //來源IP
                    e.Row.Cells[7].Width = 94;      //設備編號
                    e.Row.Cells[8].Width = 94;      //設備名稱
                    e.Row.Cells[9].Width = 94;      //記錄資訊
                    //e.Row.Cells[10].Width = 196;    //記錄說明    //不要設定
                    #endregion

                    #region B-3.處理欄位資料的格式
                    //0.設備識別碼
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;

                    //1.記錄時間
                    e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Left;

                    //2.記錄類型
                    e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Left;

                    //3.使用者帳號
                    e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Left;

                    //4.使用者名稱
                    e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Left;

                    //5.程式名稱
                    e.Row.Cells[5].HorizontalAlign = HorizontalAlign.Left;

                    //6.來源IP
                    e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Left;

                    //7.設備編號
                    e.Row.Cells[7].HorizontalAlign = HorizontalAlign.Left;

                    //8.設備名稱
                    e.Row.Cells[8].HorizontalAlign = HorizontalAlign.Left;

                    //9.記錄資訊
                    e.Row.Cells[9].HorizontalAlign = HorizontalAlign.Left;

                    //10.記錄說明
                    e.Row.Cells[10].HorizontalAlign = HorizontalAlign.Left;

                    #endregion

                    #region B-4.限制欄位資料的長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }

                    e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text, 12, true);  //記錄類型
                    e.Row.Cells[5].Text = LimitText(e.Row.Cells[5].Text, 12, true);  //程式名稱
                    e.Row.Cells[9].Text = LimitText(e.Row.Cells[9].Text, 12, true);  //記錄資訊
                    #endregion

                    #region B-5.設定表格資料列的事件方法
                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, hSelectValue,'" + drvRow.Row["RecordID"].ToString().Trim() + "', '', '')");  //設定資料列代碼
                    e.Row.Attributes.Add("OnDblclick", "CallViewWindow('" + "檢視「系統事件」記錄資料" + "')");
                    #endregion
                    break;
                #endregion

                #region C.*設定表格的換頁部份
                case DataControlRowType.Pager:

                    #region C-1.*建立相關的換頁控制項
                    GridView gv = sender as GridView;
                    LinkButton lbtnFirst = e.Row.FindControl("lbtnFirst") as LinkButton;
                    LinkButton lbtnLast = e.Row.FindControl("lbtnLast") as LinkButton;
                    PlaceHolder phdPageNumber = e.Row.FindControl("phdPageNumber") as PlaceHolder;
                    LinkButton lbtnPrev = e.Row.FindControl("lbtnPrev") as LinkButton;
                    LinkButton lbtnNext = e.Row.FindControl("lbtnNext") as LinkButton;
                    LinkButton lbtnPage;
                    #endregion

                    #region C-2.*顯示頁數列及處理上下頁、首頁、末頁動作
                    int showRange = 5;  //顯示快捷頁數
                    //int pageCount = gv.PageCount;
                    //int pageIndex = gv.PageIndex;
                    _pagecount = (_datacount % _pagesize) == 0 ? (_datacount / _pagesize) : (_datacount / _pagesize) + 1;
                    int pageCount = _pagecount;
                    int pageIndex = _pageindex;
                    int startIndex = (pageIndex + 1 < showRange) ?
                        0 : (pageIndex + 1 + showRange / 2 >= pageCount) ? pageCount - showRange : pageIndex - showRange / 2;
                    int endIndex = (startIndex >= pageCount - showRange) ? pageCount : startIndex + showRange;

                    #region C-2-1.*頁數列
                    phdPageNumber.Controls.Add(new LiteralControl("  "));

                    //指定頁數及改變文字風格
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        lbtnPage = new LinkButton();
                        lbtnPage.ID = "Pages_" + (i).ToString();
                        lbtnPage.Text = (i + 1).ToString();
                        lbtnPage.CommandName = "Pages";
                        lbtnPage.CommandArgument = (i + 1).ToString();
                        lbtnPage.Font.Overline = false;

                        if (i == pageIndex)
                        {
                            lbtnPage.Font.Bold = true;
                            lbtnPage.ForeColor = System.Drawing.Color.White;
                            lbtnPage.CommandArgument = "";
                        }
                        else
                            lbtnPage.Font.Bold = false;

                        phdPageNumber.Controls.Add(lbtnPage);
                        phdPageNumber.Controls.Add(new LiteralControl(" "));
                    }
                    #endregion

                    #region C-2-2.*上下頁
                    //lbtnPrev.Click += delegate(object obj, EventArgs args)
                    //{
                    //    if (gv.PageIndex > 0)
                    //    {
                    //        gv.PageIndex = gv.PageIndex - 1;
                    //        Query("");
                    //    }
                    //};
                    if (_pageindex == 0)
                    {
                        lbtnPrev.Enabled = false;
                    }
                    else
                    {
                        lbtnPrev.Enabled = true;
                        lbtnPrev.ID = "lbtnPrev_" + (_pageindex - 1);
                    }

                    //lbtnNext.Click += delegate(object obj, EventArgs args)
                    //{
                    //    if (gv.PageIndex < gv.PageCount)
                    //    {
                    //        gv.PageIndex = gv.PageIndex + 1;
                    //        Query("");
                    //    }
                    //};
                    if (_pageindex == _pagecount - 1 || _pagecount == 0)
                    {
                        lbtnNext.Enabled = false;
                    }
                    else
                    {
                        lbtnNext.Enabled = true;
                        lbtnNext.ID = "lbtnNext_" + (_pageindex + 1);
                    }
                    #endregion

                    #region C-2-3.*首末頁
                    //lbtnFirst.Click += delegate(object obj, EventArgs args)
                    //{
                    //    gv.PageIndex = 0;
                    //    Query("");
                    //};
                    if (_pageindex == 0 || _pagecount == 0)
                    {
                        lbtnFirst.Enabled = false;
                    }
                    else
                    {
                        lbtnFirst.Enabled = true;
                        lbtnFirst.ID = "lbtnFirst_" + 0;
                    }

                    //lbtnLast.Click += delegate(object obj, EventArgs args)
                    //{
                    //    gv.PageIndex = MainGridView.PageCount;
                    //    Query("");
                    //};
                    if (_pageindex == _pagecount - 1 || _pagecount == 0)
                    {
                        lbtnLast.Enabled = false;
                    }
                    else
                    {
                        lbtnLast.Enabled = true;
                        lbtnLast.ID = "lbtnLast_" + (_pagecount - 1);
                    }
                    #endregion

                    #endregion

                    #region C-3.*顯示總頁數及目前頁數
                    phdPageNumber.Controls.Add(
                        new LiteralControl(string.Format("　{0} / {1}　", pageIndex + 1, pageCount == 0 ? 1 : pageCount)));
                    phdPageNumber.Controls.Add(
                        new LiteralControl("  "));
                    #endregion

                    #region C-4.*顯示總筆數
                    phdPageNumber.Controls.Add(
                        new LiteralControl(string.Format(this.GetGlobalResourceObject("Resource", "lblGvCounts").ToString(), hDataRowCount.Value)));
                    phdPageNumber.Controls.Add(
                        new LiteralControl("  "));
                    #endregion

                    #region C-5.*寫入Literal_Pager
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

        #region 3-2B.*元件-表格：自訂方法
        /// <summary>
        /// *限制來源字串的顯示長度
        /// </summary>
        /// <param name="str">來源字串</param>
        /// <param name="len">顯示長度</param>
        /// <param name="ellipsis">是否省略</param>
        /// <returns>string</returns>
        public string LimitText(string str, int len, bool ellipsis)
        {
            Encoding big5 = Encoding.GetEncoding("big5");
            byte[] b = big5.GetBytes(str);

            if (b.Length <= len)
                return str;
            else
            {
                if (ellipsis)
                    len -= 3;

                string res = big5.GetString(b, 0, len);

                if (!big5.GetString(b).StartsWith(res))
                    res = big5.GetString(b, 0, len - 1);

                return res + (ellipsis ? "..." : "");
            }
        }

        /// <summary>
        /// *處理資料列繫結至GridView控制項有無資料時的動作
        /// </summary>
        /// <param name="ProcessGridView">表格</param>
        /// <param name="dt">資料表</param>
        public void GirdViewDataBind(GridView ProcessGridView, DataTable dt)
        {
            if (dt.Rows.Count != 0)  //處理Gridview控制項含有資料內容的情況
            {
                ProcessGridView.DataSource = dt;
                ProcessGridView.DataBind();
            }
            else                     //處理Gridview控制項沒有含有資料內容的情況
            {
                dt.Rows.Add(dt.NewRow());
                ProcessGridView.DataSource = dt;
                ProcessGridView.DataBind();

                int columnCount = ProcessGridView.Rows[0].Cells.Count;
                ProcessGridView.Rows[0].Cells.Clear();
                ProcessGridView.Rows[0].Cells.Add(new TableCell());
                ProcessGridView.Rows[0].Cells[0].ColumnSpan = columnCount;
                ProcessGridView.Rows[0].Cells[0].Text = this.GetGlobalResourceObject("Resource", "NonData").ToString();
            }
        }

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

        public void ExportExcel(DataTable ProcDt)
        {
            //for (int x = 0; x < MainGridView.Rows.Count; x++)
            //{
            //    for (int i = 0; i < MainGridView.Rows[x].Cells.Count; i++)
            //    {
            //        MainGridView.Rows[x].Cells[i].Text = MainGridView.Rows[x].Cells[i].ToolTip;
            //    }
            //}

            //Response.ClearContent();
            //string excelFileName = "ToExcel.xls";
            //Response.AddHeader("content-disposition", "attachment;filename=" + Server.UrlEncode(excelFileName));
            //Response.ContentType = "application/excel";
            //System.IO.StringWriter stringWrite = new System.IO.StringWriter();
            //System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
            //li_header.Text = "<table border='1'>" + li_header.Text.ToString().Replace("th", "td");
            //li_header.RenderControl(htmlWrite);
            //MainGridView.Attributes.Clear();
            //for (int i = 0; i < MainGridView.Rows.Count; i++)
            //    MainGridView.Rows[i].Attributes.Clear();
            //MainGridView.RenderControl(htmlWrite);
            //Response.Write(stringWrite.ToString());
            //Response.End();

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("SysLog");
            DataTable dtCardLog = ProcDt;

            //Title
            ws.Cells[1, 1].Value = "記錄時間";
            ws.Cells[1, 2].Value = "記錄類型";
            ws.Cells[1, 3].Value = "使用者帳號";
            ws.Cells[1, 4].Value = "使用者名稱";
            ws.Cells[1, 5].Value = "程式名稱";
            ws.Cells[1, 6].Value = "來源IP";
            ws.Cells[1, 7].Value = "設備編號";
            ws.Cells[1, 8].Value = "設備名稱";
            ws.Cells[1, 9].Value = "記錄資訊";
            ws.Cells[1, 10].Value = "記錄說明";
            //Content
            for (int i = 0, iCount = dtCardLog.Rows.Count; i < iCount; i++)
            {
                for (int j = 2, jCount = dtCardLog.Rows[i].ItemArray.Length; j < jCount; j++)
                {
                    ws.Cells[i + 2, j - 1].Value = dtCardLog.Rows[i].ItemArray[j].ToString().Replace("&nbsp;", "").Trim();
                }
            }

            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=SysLog.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }
        #endregion

        #endregion

        #region 四.查詢、複合查詢語法
        /// <summary>
        /// 依據指定的模式查詢資料並更新顯示於表格
        /// </summary>
        /// <param name="bMode">是否匯出</param>
        public void Query(bool bMode, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            List<string> liSqlPara = new List<string>();
            string sql = "", wheresql = "";

            #region Process String - B00_SysLog
            sql = @" SELECT ROW_NUMBER() OVER(ORDER BY " + SortExpression + " " + SortDire + @") AS NewIDNum,
                SysLog.RecordID, SysLog.LogTime, SysLog.LogType, SysLog.UserID,
                SysLog.UserName, CASE ISNULL(MenuName,'') WHEN '' THEN LogFrom ELSE MenuName END AS LogFrom, 
                SysLog.LogIP, SysLog.EquNo, SysLog.EquName,
                SysLog.LogInfo, SysLog.LogDesc FROM B00_SysLog AS SysLog LEFT JOIN B00_SysMenu SysMenu ON SysLog.LogFrom=SysMenu.MenuNo ";
            //sql += "INNER JOIN dbo.B01_EquData AS EquData ON EquData.EquNo = SysLog.EquNo ";
            //sql += "INNER JOIN B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID ";
            //sql += "INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID ";
            //sql += "INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID ";
            //sql += "INNER JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID ";

            #region DataAuth
            if (wheresql != "")
                wheresql += " AND ";
            wheresql += " (SysLog.UserID = ? ) ";
            liSqlPara.Add("S:" + Session["UserID"].ToString());
            #endregion

            //設定查詢條件
            if (!string.IsNullOrEmpty(hComplexQueryWheresql.Value))  //複合查詢
                wheresql = hComplexQueryWheresql.Value;
            else                                                     //關鍵字查詢
            {
                string QueryKeyWord = QueryInput_KeyWord.Text.Trim();

                if (!string.IsNullOrEmpty(QueryKeyWord.Trim()))
                {
                    //記錄時間：不做關鍵字查詢
                    wheresql = "(SysLog.LogType LIKE '%" + QueryKeyWord.Trim() + "%') OR ";
                    wheresql += "(SysLog.UserID LIKE '%" + QueryKeyWord.Trim() + "%') OR ";
                    wheresql += "(SysLog.UserName LIKE '%" + QueryKeyWord.Trim() + "%') OR ";
                    wheresql += "(SysLog.LogFrom LIKE '%" + QueryKeyWord.Trim() + "%') OR ";
                    wheresql += "(SysLog.LogIP LIKE '%" + QueryKeyWord.Trim() + "%') OR ";
                    wheresql += "(SysLog.EquNo LIKE '%" + QueryKeyWord.Trim() + "%') OR ";
                    wheresql += "(SysLog.EquName LIKE '%" + QueryKeyWord.Trim() + "%') OR ";
                    wheresql += "(SysLog.LogInfo LIKE '%" + QueryKeyWord.Trim() + "%') OR ";
                    wheresql += "(SysLog.LogDesc LIKE '%" + QueryKeyWord.Trim() + "%') ";
                }
            }

            //設定查詢排序
            if (wheresql != "")
                sql += " WHERE ";

            _sqlcommand = sql += wheresql;
            _datacount = oAcsDB.DataCount(_sqlcommand, liSqlPara);
            hDataRowCount.Value = _datacount.ToString();

            if (bMode == true)
            {
                oAcsDB.GetDataTable("DeviceOPLog", _sqlcommand, liSqlPara, out SysLogTable);
            }
            else
            {
                SysLogTable = oAcsDB.PageData(_sqlcommand, liSqlPara, "NewIDNum", _pageindex + 1, _pagesize);
            }
            //取得查詢資料
            //oAcsDB.GetDataTable("CardLogTable", sql, liSqlPara, out dt);
            if (bMode == true)
            {
                if (SysLogTable.Rows.Count != 0)
                {
                    ExportExcel(SysLogTable);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('無資料可匯出!!');location.href='QueryCardLog.aspx';", true);
                }
            }
            else
            {
                GirdViewDataBind(this.MainGridView, SysLogTable);
                UpdatePanel1.Update();
            }
            #endregion
        }

        /// <summary>
        /// 設定複合查詢的SQL條件語法
        /// </summary>
        /// <param name="qryParam">查詢參數資料</param>
        /// <returns>Pub.MessageObject</returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object SetComplexQueryWheresql(string[] qryParam)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();

            //設定複合查詢相關的欄位條件
            string wheresql = "";

            if ((qryParam != null) && (qryParam.Length > 0))
            {
                //0~1.時間欄位(起始時間與結束時間)   //查詢記錄時間欄位
                if (!string.IsNullOrEmpty(qryParam[0].Trim()) || !string.IsNullOrEmpty(qryParam[1].Trim()))
                {
                    if (wheresql != "")
                        wheresql += " AND ";

                    if (!string.IsNullOrEmpty(qryParam[0].Trim()) && !string.IsNullOrEmpty(qryParam[1].Trim()))  //查詢起迄時間範圍的資料內容
                        wheresql += "(SysLog.LogTime >= '" + qryParam[0].Trim() + "' AND SysLog.LogTime <= '" + qryParam[1].Trim() + "')";
                    else if (!string.IsNullOrEmpty(qryParam[0].Trim()))                                          //查詢起始時間之後的資料內容
                        wheresql += "(SysLog.LogTime >= '" + qryParam[0].Trim() + "')";
                    else if (!string.IsNullOrEmpty(qryParam[1].Trim()))                                          //查詢結束時間之前的資料內容  
                        wheresql += "(SysLog.LogTime <= '" + qryParam[1].Trim() + "')";
                }

                //2.使用者帳號
                if (!string.IsNullOrEmpty(qryParam[2].Trim()))
                {
                    if (wheresql != "")
                        wheresql += " AND ";

                    wheresql += "(SysLog.UserID LIKE '%" + qryParam[2].Trim() + "%')";
                }

                //3.使用者名稱
                if (!string.IsNullOrEmpty(qryParam[3].Trim()))
                {
                    if (wheresql != "")
                        wheresql += " AND ";

                    wheresql += "(SysLog.UserName LIKE '%" + qryParam[3].Trim() + "%')";
                }

                //4.程式名稱
                if (!string.IsNullOrEmpty(qryParam[4].Trim()))
                {
                    if (wheresql != "")
                        wheresql += " AND ";

                    wheresql += "(SysLog.LogFrom LIKE '%" + qryParam[4].Trim() + "%')";
                }

                //5.來源IP
                if (!string.IsNullOrEmpty(qryParam[5].Trim()))
                {
                    if (wheresql != "")
                        wheresql += " AND ";

                    wheresql += "(SysLog.LogIP LIKE '%" + qryParam[5].Trim() + "%')";
                }

                //6.設備編號
                if (!string.IsNullOrEmpty(qryParam[6].Trim()))
                {
                    if (wheresql != "")
                        wheresql += " AND ";

                    wheresql += "(SysLog.EquNo LIKE '%" + qryParam[6].Trim() + "%')";
                }

                //7.設備名稱
                if (!string.IsNullOrEmpty(qryParam[7].Trim()))
                {
                    if (wheresql != "")
                        wheresql += " AND ";

                    wheresql += "(SysLog.EquName LIKE '%" + qryParam[7].Trim() + "%')";
                }

                //8.記錄資訊
                if (!string.IsNullOrEmpty(qryParam[8].Trim()))
                {
                    if (wheresql != "")
                        wheresql += " AND ";

                    wheresql += "(SysLog.LogInfo LIKE '%" + qryParam[8].Trim() + "%')";
                }

                //9.記錄說明
                if (!string.IsNullOrEmpty(qryParam[9].Trim()))
                {
                    if (wheresql != "")
                        wheresql += " AND ";

                    wheresql += "(SysLog.LogDesc LIKE '%" + qryParam[9].Trim() + "%')";
                }
            }

            objRet.message = wheresql;
            objRet.act = "ComplexQuery";
            return objRet;
        }
        #endregion

        #region 五.載入
        /// <summary>
        /// 載入「系統事件 記錄資料」視窗相關的欄位資料
        /// </summary>
        /// <param name="RecordID">資料列識別碼</param>
        /// <returns>string[] LoadViewWindosData</returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadViewWindowData(string RecordID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();

            Sa.DB.DBReader dr;
            List<string> liSqlPara = new List<string>();

            string[] LoadViewWindosData = null;
            string sql = "", NoAndYesListSql = "", VerifyModeListSql = "";

            //取得載入畫面相關的欄位資料


            #region Process String - B00_SysLog
            sql = "SELECT RecordID, LogTime, LogType, UserID, UserName, LogFrom, LogIP, EquNo, EquName, LogInfo, LogDesc ";
            sql += "FROM B00_SysLog AS SysLog";

            sql += " WHERE SysLog.RecordID = ? ";

            liSqlPara.Add("S:" + RecordID.Trim());

            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            #endregion

            if (dr.Read())
            {
                LoadViewWindosData = new string[dr.DataReader.FieldCount];

                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                    LoadViewWindosData[i] = dr.DataReader[i].ToString();
            }
            else
            {
                LoadViewWindosData = new string[2];
                LoadViewWindosData[0] = "Saho_SysErrorMassage";
                LoadViewWindosData[1] = "系統中無此資料！";
            }

            return LoadViewWindosData;
        }

        #endregion
    }
}