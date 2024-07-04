using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sa.DB;
using System.Net.Sockets;
using AjaxControlToolkit;
using SahoAcs.DBClass;


namespace SahoAcs
{
    public partial class TempCardRecordCloud : System.Web.UI.Page
    {
        #region 一.宣告
        private int _pagesize = 20;         //設定GridView控制項每頁可顯示的資料列數
        private static string UserID = "";  //儲存目前使用者的UserID
        ToolkitScriptManager oScriptManager;  //宣告Ajax元件

        private DataTable tmpDT = new DataTable();

        #region 宣告靜態屬性，方便 Webthod 使用

        private static string stringDefaultListItem = "";
        private static string strUserId = "";
        private static string strUserName = "";

        #endregion

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
            popInput_BorrowTime.SetWidth(180);
            popInput_ReturnTime.SetWidth(180);
            popInput_BorrowTime.SetRequired();
            this.MainGridView.PageSize = _pagesize;

            GetddlCardNoList();         // 得到 ddlCardNo 的選項
            strUserId = Session["UserId"].ToString();
            strUserName = Session["UserName"].ToString();
            UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");

            // 將所有 CardType = 'T' 的資料更新成 CardType = 'R'
            //UpdateCardType();

            // 20170310 暫用 將臨時卡的CardVer相關的資料清除
            //UpdateTempCardVer();

            if (Session["tmpDatatable"] == null) Session["tmpDatatable"] = new DataTable();

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                ViewState["query_no"]   = "";
                ViewState["SortExpression"] = "BorrowTime";
                ViewState["SortDire"] = "DESC";
                Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                //this.popInput_BorrowTime.DateValue = DateTime.Now.GetZoneTime(this).ToString("yyyy/MM/dd HH:mm:ss");
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg    = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == this.QueryButton.ClientID)
                {
                    ViewState["query_no"]   = this.Input_No.Text.Trim();
                }

                if (!string.IsNullOrEmpty(sFormArg))
                {
                    //進行因應新增或編輯後所需的換頁動作
                    if (sFormArg == "popPagePost")                 
                    {
                        int find = Query("popPagePost", ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());

                        if (find > 0) find = find - 1;
                        MainGridView.PageIndex = find;
                        GirdViewDataBind(MainGridView, Session["tmpDatatable"] as DataTable);
                        Sa.Web.Fun.RunJavaScript(this, "GridSelect();");

                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI", "$.unblockUI();", true);
                    }
                }
                else
                {
                    Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    Sa.Web.Fun.RunJavaScript(this, "GridSelect();");
                }
            }
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
            oScriptManager = this.Master.FindControl("ToolkitScriptManager1") as ToolkitScriptManager;
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 100);";
            js += "\nSetMode('');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);

            //加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("TempCardRecord", "TempCardRecordCloud.js");
        }

        /// <summary>
        /// 註冊網頁的視窗與按鈕動作
        /// </summary>
        private void RegisterWindowsAndButton()
        {
            //註冊主作業畫面的按鈕動作
            QueryButton.Attributes["onClick"]  = "SelectState(); return false;";
            AddButton.Attributes["onClick"] = "CallAdd('" +
                this.GetLocalResourceObject("CallAdd_Title") + "'); return false;";
            EditButton.Attributes["onClick"] = "CallEdit('" +
                this.GetLocalResourceObject("CallEdit_Title") + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") +"'); return false;";
            DeleteButton.Attributes["onClick"] = "CallDelete('" +
                this.GetLocalResourceObject("CallDelete_Title") + "', '" +
                this.GetLocalResourceObject("CallDelete_DelLabel") + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForDelete") + "'); return false;";

            //註冊次作業畫面的按鈕動作
            ImgCloseButton1.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            popB_Add.Attributes["onClick"]    = "AddExcute(); return false;";
            popB_Edit.Attributes["onClick"]   = "EditExcute(); return false;";
            popB_Delete.Attributes["onClick"] = "DeleteExcute(); return false;";
            popB_Cancel.Attributes["onClick"] = "CancelExcute(); return false;";

            // popInput_PsnNo OnBlur
            popInput_PsnNo.Attributes["OnBlur"] = "GetCardData(); return false;";

            Pub.SetModalPopup(ModalPopupExtender1, 1);
        }

        /// <summary>
        /// 切換網頁的顯示語系########################################
        /// </summary>
        private void ChangeLanguage()
        {
             // 內容：選取資料
             stringDefaultListItem = GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString();
            }
        #endregion

        #endregion

        #region 三.元件
        #region 3-1A.元件-表格：事件方法
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
        /// *處理GridView控制項在完成資料繫結後的相關動作
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

            if (Session["tmpDatatable"] != null)
            {
                tmpDT.Clear();
                tmpDT = Session["tmpDatatable"] as DataTable;
                tmpDT.DefaultView.Sort = SortExpression + " " + SortDire;
                MainGridView.DataSource = tmpDT;
                MainGridView.DataBind();
            }
            else
            {
                Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
            }

            //Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
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
                    //e.Row.Cells[0].Visible = false;  //資料列識別碼

                    e.Row.Cells[0].Width = 80;     //資料列識別碼
                    e.Row.Cells[1].Width = 100;      //人員編號 
                    e.Row.Cells[2].Width = 100;      //人員姓名
                    e.Row.Cells[3].Width = 120;      //原來卡片號碼
                    e.Row.Cells[4].Width = 120;      //借出卡片號碼
                    e.Row.Cells[5].Width = 150;      //借出時間
                    e.Row.Cells[6].Width = 150;      //歸還時間
                    //e.Row.Cells[6].Width = 300;     //說明(此部份不用設定)
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
                    DataRowView oRow = (DataRowView)e.Row.DataItem;
                    GridViewRow GrRow = e.Row;

                    GrRow.ClientIDMode = ClientIDMode.Static;
                    GrRow.ID = "GV_Row" + oRow.Row["RecordID"].ToString().Trim();
                    #endregion

                    #region B-2.設定欄位隱藏與寬度
                    //e.Row.Cells[0].Visible = false;  //資料列識別碼

                    e.Row.Cells[0].Width = 84;      //資料列識別碼
                    e.Row.Cells[1].Width = 104;      //人員編號
                    e.Row.Cells[2].Width = 104;      //人員名稱
                    e.Row.Cells[3].Width = 124;      //原來卡片號碼
                    e.Row.Cells[4].Width = 124;      //借出卡片號碼
                    e.Row.Cells[5].Width = 154;      //借出時間
                    e.Row.Cells[6].Width = 154;      //歸還時間

                    //e.Row.Cells[7].Width = 304;     //說明(此部份不用設定)
                    #endregion

                    #region B-3.處理欄位資料的格式
                    //0.資料列識別碼
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;

                    //1.人員編號
                    e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Center;

                    //2.人員姓名
                    e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Center;

                    //3.原來卡片號碼
                    e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Center;

                    //4.借出卡片號碼
                    e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Center;

                    //5.借出時間
                    e.Row.Cells[5].HorizontalAlign = HorizontalAlign.Center;

                    if (!String.IsNullOrEmpty(e.Row.Cells[5].Text.Replace("&nbsp;", "").Trim()))
                        e.Row.Cells[5].Text = DateTime.Parse(e.Row.Cells[5].Text).ToString("yyyy/MM/dd HH:mm:ss");

                    //6.歸還時間
                    e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Center;

                    if (!String.IsNullOrEmpty(e.Row.Cells[6].Text.Replace("&nbsp;", "").Trim()))
                        e.Row.Cells[6].Text = DateTime.Parse(e.Row.Cells[6].Text).ToString("yyyy/MM/dd HH:mm:ss");

                    //7.說明
                    #endregion

                    #region B-4.限制欄位資料的長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }

                    e.Row.Cells[7].Text = LimitText(e.Row.Cells[7].Text, 50, true);  //說明
                    #endregion

                    #region B-5.設定表格事件的方法

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["RecordID"].ToString().Trim() + "', '', '')");
                    e.Row.Attributes.Add("OnDblclick", "CallEdit('" + "編輯臨時卡借還作業資料" + "')");

                    #endregion
                    break;
                #endregion

                #region C.*設定表格的控管部份
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
                    int pageCount = gv.PageCount;
                    int pageIndex = gv.PageIndex;
                    int startIndex = (pageIndex + 1 < showRange) ?
                        0 : (pageIndex + 1 + showRange / 2 >= pageCount) ? pageCount - showRange : pageIndex - showRange / 2;
                    int endIndex = (startIndex >= pageCount - showRange) ? pageCount : startIndex + showRange;

                    #region C-2-1.*頁數列
                    phdPageNumber.Controls.Add(new LiteralControl("  "));

                    //指定頁數及改變文字風格
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        lbtnPage = new LinkButton();
                        lbtnPage.Text = (i + 1).ToString();
                        lbtnPage.CommandName = "Page";
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
                    lbtnPrev.Click += delegate(object obj, EventArgs args)
                    {
                        if (gv.PageIndex > 0)
                        {
                            gv.PageIndex = gv.PageIndex - 1;
                            MainGridView.DataBind();

                            //Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        }
                    };

                    lbtnNext.Click += delegate(object obj, EventArgs args)
                    {
                        if (gv.PageIndex < gv.PageCount)
                        {
                            gv.PageIndex = gv.PageIndex + 1;
                            MainGridView.DataBind();

                            //Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        }
                    };
                    #endregion

                    #region C-2-3.*首末頁
                    lbtnFirst.Click += delegate(object obj, EventArgs args)
                    {
                        gv.PageIndex = 0;
                        MainGridView.DataBind();

                        //Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    };

                    lbtnLast.Click += delegate(object obj, EventArgs args)
                    {
                        gv.PageIndex = MainGridView.PageCount;
                        MainGridView.DataBind();

                        //Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    };
                    #endregion
                    #endregion

                    #region C-3.*顯示總頁數及目前頁數
                    phdPageNumber.Controls.Add(new LiteralControl(string.Format("　{0} / {1}　", pageIndex + 1, pageCount)));
                    phdPageNumber.Controls.Add(new LiteralControl("  "));
                    #endregion

                    #region C-4.*顯示顯示總筆數
                    phdPageNumber.Controls.Add(
                        new LiteralControl(string.Format("　總共 {0} 筆　", hDataRowCount.Value)));
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

        #region 3-1B.*元件-表格：自訂方法
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
                ProcessGridView.Rows[0].Cells[0].Text = GetGlobalResourceObject("Resource", "NonData").ToString();

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

        #region 設定下拉式選單[ddlCardNo]的項目

        private void GetddlCardNoList()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt = new DataTable();

            this.ddlCardNo.Items.Clear();
            
            // 選取資料 / Select Data
            this.ddlCardNo.Items.Add(new ListItem(GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString(), ""));

            sql = @" 
                SELECT 
                    C.CardNo, C.CardID  
                FROM B01_Card C 
                LEFT JOIN B00_ItemList L ON L.ItemClass = 'CardType' AND L.ItemNo = C.CardType 
                LEFT JOIN dbo.B01_Person P ON P.PsnID = C.PsnID   
                WHERE C.CardType = 'R' AND C.CardAuthAllow = 0 
                ORDER BY C.CardID ";

            oAcsDB.GetDataTable("CardNoTable", sql, out dt);

            foreach (DataRow dr in dt.Rows)
            {
                this.ddlCardNo.Items.Add(new ListItem(dr["CardNo"].ToString(), dr["CardNo"].ToString()));
            }
        }

        #endregion
        
        #endregion

        #region 四.查詢
        /// <summary>
        /// 依據指定的條件內容查詢資料並更新顯示於表格
        /// </summary>
        /// <param name="SortExpression">排序欄位</param>
        /// <param name="SortDire">排序定序</param>
        public void Query(string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            DataTable dt;
            List<string> liSqlPara = new List<string>();

            string sql = "", wheresql = "";

            #region Process String - B01_TempCardRecord
            sql = @" 
                SELECT TCR.*, P.PsnName 
                FROM B01_TempCardRecord AS TCR 
                LEFT JOIN B01_Person AS P ON P.PsnNo = TCR.PsnNo ";

            // 設定查詢條件
            if (!string.IsNullOrEmpty(ViewState["query_no"].ToString().Trim()))
            {
                string tmp = ViewState["query_no"].ToString().Trim();
                string[] strCondiition = new string[] { "\t", " ", "　", ",", "，", "、", "。", "\n" };
                string[] strArray = tmp.Split(strCondiition, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i <= strArray.Length - 1; i++)
                {
                    if (strArray[i].Trim() != "")
                    {
                        if (wheresql != "")
                        {
                            wheresql += " OR (( TCR.PsnNo + '__' + TCR.OrigCardNo + '__' + TCR.CardNo + '__' + P.PsnName + '__' + ISNULL(P.IDNum,'') ) LIKE ? ) ";

                            liSqlPara.Add("S:" + "%" + strArray[i].Trim() + "%");
                        }
                        else
                        {
                            wheresql += " (( TCR.PsnNo + '__' + TCR.OrigCardNo + '__' + TCR.CardNo + '__' + P.PsnName + '__' + ISNULL(P.IDNum,'') ) LIKE ? ) ";

                            liSqlPara.Add("S:" + "%" + strArray[i].Trim() + "%");
                        }
                        
                    }
                }
            }

            if (wheresql != "") wheresql = " WHERE " + wheresql;

            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;            
            //取得查詢資料
            oAcsDB.GetDataTable("TempCardRecordTable", sql, liSqlPara, out dt);
            foreach (DataRow r in dt.Rows)
            {
                r["BorrowTime"] = Convert.ToDateTime(r["BorrowTime"]).GetZoneTime(this);
                if (r["ReturnTime"] != DBNull.Value)
                {
                    r["ReturnTime"] = Convert.ToDateTime(r["ReturnTime"]).GetZoneTime(this);
                }
            }
            hDataRowCount.Value = dt.Rows.Count.ToString();

            Session["tmpDatatable"] = dt;
            #endregion

            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();
        }

        /// <summary>
        /// 重新查詢資料並更新顯示於表格，以及取得資料目前的表格頁數
        /// </summary>
        /// <param name="mode">查詢模式</param>
        /// <param name="SortExpression">排序欄位</param>
        /// <param name="SortDire">排序定序</param>
        /// <returns>資料目前的表格頁數</returns>
        public int Query(string mode, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            DataTable dt = new DataTable();
            List<string> liSqlPara = new List<string>();

            string sql = "", wheresql = "";

            #region Process String - Get B01_TempCardRecord
            sql = @" 
                SELECT 
                    TCR.*, P.PsnName 
                FROM B01_TempCardRecord AS TCR 
                LEFT JOIN B01_Person AS P ON P.PsnNo = TCR.PsnNo ";

            //設定查詢排序
            //sql += wheresql + " ORDER BY TempCardRecord.ReturnTime , TempCardRecord.BorrowTime DESC, TempCardRecord.PsnNo ";

            //設定查詢條件
            if (!string.IsNullOrEmpty(ViewState["query_no"].ToString().Trim()))
            {
                string tmp = ViewState["query_no"].ToString().Trim().Replace(",", " ");
                tmp = tmp.Replace("_", " ");

                string[] strArray = tmp.Split(' ');

                for (int i = 0; i <= strArray.Length - 1; i++)
                {
                    if (strArray[i].Trim() != "")
                    {
                        if (wheresql != "")
                        {
                            wheresql += " OR (( TCR.PsnNo + '__' + TCR.OrigCardNo + '__' + TCR.CardNo + '__' + P.PsnName + '__' + ISNULL(P.IDNum,'') ) LIKE ? ) ";

                            liSqlPara.Add("S:" + "%" + strArray[i].Trim() + "%");
                        }
                        else
                        {
                            wheresql += " (( TCR.PsnNo + '__' + TCR.OrigCardNo + '__' + TCR.CardNo + '__' + P.PsnName + '__' + ISNULL(P.IDNum,'') ) LIKE ? ) ";

                            liSqlPara.Add("S:" + "%" + strArray[i].Trim() + "%");
                        }

                    }
                }
            }

            if (wheresql != "") wheresql = " WHERE " + wheresql;

            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;

            oAcsDB.GetDataTable("TempCardRecordTable", sql, liSqlPara, out dt);
            foreach (DataRow r in dt.Rows)
            {
                r["BorrowTime"] = Convert.ToDateTime(r["BorrowTime"]).GetZoneTime(this);
                if (r["ReturnTime"] != DBNull.Value)
                {
                    r["ReturnTime"] = Convert.ToDateTime(r["ReturnTime"]).GetZoneTime(this);
                }
            }
            hDataRowCount.Value = dt.Rows.Count.ToString();

            Session["tmpDatatable"] = dt;
            #endregion

            //完成查詢後並取得資料目前的表格頁數
            int find = 0;
            DataRow dr = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dr = dt.Rows[i];

                if (SelectValue.Value == dr["RecordID"].ToString().Trim())
                {
                    find = i;
                    break;
                }
            }

            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();

            return (find / _pagesize) + 1;
        }
        #endregion

        #region 五.檢查、載入、新增、編輯、刪除
        /// <summary>
        /// 檢查臨時卡借還作業視窗相關的輸入欄位與資料庫資料
        /// </summary>
        /// <param name="Mode">作業模式</param>
        /// <param name="NoArray">編號陣列</param>
        /// <param name="PsnNo">人員編號</param>
        /// <param name="OrigCardNo">原來卡片號碼</param>
        /// <param name="CardNo">借出卡片號碼</param>
        /// <param name="BorrowTime">借出時間</param>
        /// <param name="ReturnTime">歸還時間</param>
        /// <param name="TempDesc">說明</param>
        /// <returns>Pub.MessageObject</returns>
        protected static Pub.MessageObject Check_Input_DB(string Mode, string[] NoArray, string PsnNo, string OrigCardNo, string CardNo, string BorrowTime, string ReturnTime, string TempDesc)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

            Sa.DB.DBReader dr;
            List<string> liSqlPara = new List<string>();
            Pub.MessageObject objRet = new Pub.MessageObject();

            string sql = "";

            #region A.檢查輸入欄位
            //1.人員編號
            if (string.IsNullOrEmpty(NoArray[0].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "帳號 必須輸入";
            }
            else if (NoArray[0].Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "帳號 字數超過上限";
                string ss = "1234";

                //ss.ToString().coun

                //NoArray[0].Trim().
            }

            //2.原來卡片號碼
            if (string.IsNullOrEmpty(PsnNo.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "密碼 必須輸入";
            }
            else if (PsnNo.Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "借出卡片號碼 長度超過上限";
            }

            //3.借出卡片號碼
            if (OrigCardNo.Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "借出卡片號碼 長度超過上限";
            }

            //4.借出時間
            if (string.IsNullOrEmpty(BorrowTime.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "歸還時間 必須輸入";
            }

            //5.歸還時間
            if (string.IsNullOrEmpty(BorrowTime.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "歸還時間 必須輸入";
            }

            //6.說明
            if (TempDesc.Trim().Count() > 200)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "說明 字數超過上限";
            }
            #endregion

            #region B.檢查資料庫資料
            switch (Mode)
            {
                case "Insert":
                    sql = @" SELECT * FROM B00_SysUser WHERE UserID = ? ";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    break;
                case "Update":
                    sql = @" SELECT * FROM B00_SysUser WHERE UserID = ? AND UserID != ? ";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    liSqlPara.Add("S:" + NoArray[1].Trim());
                    break;
            }

            oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (dr.Read())
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "此帳號已存在於系統中";
            }
            #endregion

            return objRet;
        }

        /// <summary>
        /// 載入臨時卡借還作業視窗相關的欄位資料
        /// </summary>
        /// <param name="sRecordID">資料列識別碼</param>
        /// <returns>string[] EditData</returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadData(string sRecordID, string sActFunc)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();

            Sa.DB.DBReader dr = null;

            string sql = "";
            string[] EditData = null;

            if (sRecordID.Trim() == "")
            {
                EditData = new string[2];

                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "資料識別碼為空值！";

                return EditData;
            }

            //取得載入畫面相關的欄位資料
            #region Process String - Get B01_TempCardRecord
            sql  = @" 
                SELECT 
                    TR.RecordID, 
                    TR.PsnNo, 
                    TR.OrigCardNo, 
                    TR.CardNo, 
                    TR.BorrowTime, 
                    TR.ReturnTime, 
                    TR.TempDesc, 
                    PN.PsnName 
                FROM B01_TempCardRecord AS TR 
                LEFT JOIN B01_Person AS PN ON PN.PsnNo = TR.PsnNo 
                WHERE TR.RecordID = ? ";

            List<string> liSqlPara = new List<string>();
            liSqlPara.Add("I:" + sRecordID.Trim());

            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            #endregion

            EditData = new string[2];

            EditData[0] = "Saho_SysErrorMassage";
            EditData[1] = "系統中無此資料！";

            if (dr == null)
                return EditData;
            else if (dr.Read())
            {
                EditData = new string[dr.DataReader.FieldCount + 1];

                EditData[0] = sActFunc;

                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                {
                    if ((i != 4) && (i != 5))
                        EditData[i + 1] = dr.DataReader[i].ToString();
                    else
                    {
                        EditData[i + 1] = dr.DataReader[i].ToString();

                        //將BorrowTime及ReturnTime欄位資料內容設定為指定的資料格式
                        if (EditData[i + 1] != "")
                        {
                            var page = System.Web.HttpContext.Current.Handler as Page;
                            if (page != null)
                            {
                                EditData[i + 1] = DateTime.Parse(dr.DataReader[i].ToString()).GetZoneTime(page).ToString("yyyy/MM/dd HH:mm:ss");
                            }
                            else
                            {
                                EditData[i + 1] = DateTime.Parse(dr.DataReader[i].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                            }                            
                        }                            
                    }
                }
            }

            dr = null;

            return EditData;
        }

        /// <summary>
        /// 新增臨時卡借還作業視窗相關的欄位資料
        /// </summary>
        /// <param name="sPsnNo">人員編號</param>
        /// <param name="sOrigCardNo">原來卡片號碼</param>
        /// <param name="sCardNo">借出卡片號碼</param>
        /// <param name="sBorrowTime">借出時間</param>
        /// <param name="sReturnTime">歸還時間</param>
        /// <param name="sTempDesc">說明</param>
        /// <returns>Pub.MessageObject</returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Insert(string sPsnNo, string sOrigCardNo, string sCardNo, string sBorrowTime, string sTempDesc)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            List<string> liSqlPara = new List<string>();
            DataTable dt = new DataTable();
            DBReader dr = new DBReader();

            int iResult = -1;
            string sSql = "";
            string sNowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            objRet.act   = "Add";
            objRet.result = true;

            #region 確認輸入的相關欄位是否為空字串
            if (sPsnNo.Trim() == "")
            {
                objRet.result = false;
                objRet.message = "人員編號不可為空值！";
            }
            else if (sOrigCardNo.Trim() == "")
            {
                objRet.result = false;
                objRet.message = "原來卡片號碼不可為空值！";
            }
            else if (sCardNo.Trim() == "")
            {
                objRet.result = false;
                objRet.message = "借出卡片號碼必需選取一個臨時卡或不可為空值！";
            }
            else if (sBorrowTime == "")
            {
                objRet.result = false;
                objRet.message = "借出時間不可為空值！";
            }
            else if (sOrigCardNo == sCardNo)
            {
                objRet.result = false;
                objRet.message = "來源卡片號碼與借出卡片號碼不能相同！";
            }

            if (!objRet.result) return objRet;
            #endregion

            #region [暫不用]確認輸入的人員編號是否存在與權限是否為有效
            
            //string sPsnID = "";

            //sSql = " SELECT PsnID, PsnAuthAllow";
            //sSql += " FROM B01_Person";
            //sSql += " WHERE PsnNo = '" + sPsnNo.Trim() + "' ";

            //dt = new DataTable();
            //oAcsDB.GetDataTable("PsnNoCountTable", sSql, out dt);

            //if ((dt == null) || (dt.Rows.Count <= 0))
            //{
            //    objRet.result = false;
            //    objRet.message = "輸入的人員編號：" + sPsnNo.Trim() + "不存在！";
            //}
            //else
            //{
            //    string sPsnAuthAllow = dt.Rows[0]["PsnAuthAllow"].ToString();

            //    if (sPsnAuthAllow == "1")
            //        sPsnID = dt.Rows[0]["PsnID"].ToString();
            //    else
            //    {
            //        objRet.result = false;
            //        objRet.message = "輸入的人員編號：" + sPsnNo.Trim() + "該權限為無效！";
            //    }
            //}

            //if (!objRet.result) return objRet;
            #endregion

            #region [暫不用]確認輸入的原來卡片號碼是否存在、是否不是臨時卡、權限是否為該人員編號
            //sSql = " SELECT CardType, PsnID";
            //sSql += " FROM B01_Card";
            //sSql += " WHERE CardNo = '" + sOrigCardNo.Trim() + "' ";

            //dt = new DataTable();
            //oAcsDB.GetDataTable("CardNoCountTable", sSql, out dt);

            //if ((dt == null) || (dt.Rows.Count <= 0))
            //{
            //    objRet.result = false;
            //    objRet.message = "輸入的原來卡片號碼：" + sOrigCardNo.Trim() + "不存在！";
            //}
            //else
            //{
            //    string sTestPsnID = dt.Rows[0]["PsnID"].ToString();
            //    string sTestCardType = dt.Rows[0]["CardType"].ToString();

            //    if (sTestCardType == "R")
            //    {
            //        objRet.result = false;
            //        objRet.message = "輸入的原來卡片號碼：" + sOrigCardNo.Trim() + "不能為臨時卡！";
            //    }
            //    else if (sPsnID != sTestPsnID)
            //    {
            //        objRet.result = false;
            //        objRet.message = "輸入的原來卡片號碼：" + sOrigCardNo.Trim() + "不是該人員編號：" + sPsnNo.Trim() + "所持有！";
            //    }
            //}

            //if (!objRet.result) return objRet;
            #endregion

            #region [暫不用]確認是否已拿來借用臨時卡
            //sSql = " SELECT RecordID";
            //sSql += " FROM B01_TempCardRecord";
            //sSql += " WHERE (OrigCardNo = '" + sOrigCardNo.Trim() + "') AND (BorrowTime IS NOT NULL) AND (ReturnTime IS NULL) ";

            //dt = new DataTable();
            //oAcsDB.GetDataTable("OrigCardNoCountTable", sSql, out dt);

            //if ((dt != null) && (dt.Rows.Count > 0))
            //{
            //    objRet.result = false;
            //    objRet.message = "輸入的原來卡片號碼：" + sOrigCardNo.Trim() + "已經用來借用臨時卡！";
            //}

            //if (!objRet.result) return objRet;
            #endregion

            #region [暫不用]確認輸入的借出卡片號碼是否存在、是否為臨時卡與權限是否該臨時卡已借出
            //sSql = " SELECT CardNo, CardType";
            //sSql += " FROM B01_Card";
            //sSql += " WHERE CardNo = '" + sCardNo.Trim() + "' ";

            //dt = new DataTable();
            //oAcsDB.GetDataTable("CardNoCountTable", sSql, out dt);

            //if ((dt == null) || (dt.Rows.Count <= 0))
            //{
            //    objRet.result = false;
            //    objRet.message = "輸入的借出卡片號碼：" + sCardNo.Trim() + "不存在！";
            //}
            //else
            //{
            //    string sTestCardNo = dt.Rows[0]["CardNo"].ToString();
            //    string sTestCardType = dt.Rows[0]["CardType"].ToString();

            //    if (sTestCardType != "R")
            //    {
            //        objRet.result = false;
            //        objRet.message = "輸入的借出卡片號碼：" + sCardNo.Trim() + "不是臨時卡！";
            //    }
            //    else
            //    {
            //        if (dt != null)
            //        {
            //            dt.Clear();
            //            dt = null;
            //        }

            //        #region GET B01_TempCardRecord - CardNo
            //        sSql = " SELECT RecordID";
            //        sSql += " FROM B01_TempCardRecord";
            //        sSql += " WHERE (CardNo = '" + sCardNo.Trim() + "') AND (BorrowTime IS NOT NULL) AND (ReturnTime IS NULL) ";

            //        oAcsDB.GetDataTable("CardNoCountTable", sSql, out dt);
            //        #endregion

            //        if ((dt != null) && (dt.Rows.Count > 0))
            //        {
            //            objRet.result = false;
            //            objRet.message = "輸入的借出卡片號碼：" + sCardNo.Trim() + "目前被借出尚未歸還！";
            //        }
            //    }
            //}

            //if (!objRet.result) return objRet;
            #endregion

            #region 確認借出時間是否大過於目前的時間

            /*
            int y1 = int.Parse(sBorrowTime.Substring(0, 4));
            int m1 = int.Parse(sBorrowTime.Substring(5, 2));
            int d1 = int.Parse(sBorrowTime.Substring(8, 2));
            int y2 = int.Parse(sNowTime.Substring(0, 4));
            int m2 = int.Parse(sNowTime.Substring(5, 2));
            int d2 = int.Parse(sNowTime.Substring(8, 2));
            */

            DateTime t1 = DateTime.Parse(sBorrowTime);            
            var page = System.Web.HttpContext.Current.Handler as System.Web.UI.Page;
            DateTime t2 = DateTime.Now;            
            if (page!=null)
            {
                //t1 = t1.GetUtcTime(page);
                t2 = t2.GetZoneTime(page);
            }
            int iCompare = DateTime.Compare(t1, t2);

            if (iCompare > 0)
            {
                objRet.result = false;
                objRet.message = "輸入的借出日期時間不能大過於目前的日期時間！";
            }

            if (!objRet.result) return objRet;

            #endregion

            #region 處理卡片權限複製步驟
            //以下步驟要先做卡片權限複製、接著將來源卡片號碼的CardAuthAllow(1->0)，最後再做來源卡片號碼的卡片權限重整
            //處理卡片權限複製步驟
            string[] AuthResetCardNoArray = new string[1];

            AuthResetCardNoArray[0] = sCardNo;

            Pub.MessageObject objRetAuthCopy = (Pub.MessageObject)ExcuteCardAuthCopy(UserID, sOrigCardNo, AuthResetCardNoArray);

            if (!objRetAuthCopy.result)
            {
                objRet.result = false;
                objRet.message = objRetAuthCopy.message;
            }

            if (objRetAuthCopy != null) objRetAuthCopy = null;

            if (!objRet.result) return objRet;

            #endregion

            #region [暫不用]設定臨時卡為有效

            //sSql = @" 
            //    UPDATE B01_Card SET CardAuthAllow = 1 ,
            //        PsnID=(SELECT TOP 1 PsnID FROM B01_Card WHERE CardNo=?), 
            //        CardPW=(SELECT TOP 1 CardPW FROM B01_Card WHERE CardNo=?)      
            //    WHERE CardNo=? ";

            //liSqlPara.Clear();
            //liSqlPara.Add("S:" + sOrigCardNo.Trim());
            //liSqlPara.Add("S:" + sOrigCardNo.Trim());
            //liSqlPara.Add("S:" + sCardNo.Trim());

            //iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);

            //if (iResult == -1)
            //{
            //    objRet.result = false;
            //    objRet.message = "設定臨時卡片號碼：" + sCardNo + "的權限設為「有效」失敗！！";
            //}

            //if (!objRet.result) return objRet;

            #endregion

            #region 處理來源卡片號碼的CardAuthAllow設為「無效」

            sSql = " UPDATE B01_Card SET CardAuthAllow = 0 WHERE CardNo = ? ";

            liSqlPara.Clear();
            liSqlPara.Add("S:" + sOrigCardNo);
            iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);

            if (iResult < 0)
            {
                objRet.result = false;
                objRet.message = "設定來源卡片號碼：" + sOrigCardNo + "的權限設為「無效」失敗！";

                return objRet;
            }
            else
            {
                oAcsDB.WriteLog(DB_Acs.Logtype.資料修改, strUserId, strUserName, "0107", "", "", string.Format("SET CardAuthAllow=0 WHERE CardNo={0}", sOrigCardNo), "B01_Card");
            }

            if (!objRet.result)
                return objRet;

            #endregion

            #region 處理來源卡片號碼的卡片權限重整
            string[] AuthCardNoArray = new string[2];

            AuthCardNoArray[0] = sOrigCardNo;
            AuthCardNoArray[1] = sCardNo;
            liSqlPara.Clear();
            liSqlPara.Add("S:" + sCardNo);
            oAcsDB.SqlCommandExecute("DELETE B02_FaceData2 WHERE CardNo=?", liSqlPara);
            oAcsDB.SqlCommandExecute("DELETE B02_FaceImageData2 WHERE CardNo=?", liSqlPara);
            liSqlPara.Add("S:" + sOrigCardNo);
            oAcsDB.SqlCommandExecute(@"INSERT INTO B02_FaceData2 
                    (CardNo ,FaceAmount ,FaceKey ,NumOfTemplate ,Flag ,ImageLen ,ImageData ,TemplateDAtaLen ,TemplateData ,SecurityLevel ,FaceType ,CardType ,IDType ,AuthType ,UserType) 
                    SELECT ? ,FaceAmount ,FaceKey ,NumOfTemplate ,Flag ,ImageLen ,ImageData ,TemplateDAtaLen 
                        ,TemplateData ,SecurityLevel ,FaceType , CardType ,IDType ,AuthType ,UserType FROM B02_FaceData2 WHERE CardNo=?", liSqlPara);
            oAcsDB.SqlCommandExecute(@"INSERT INTO B02_FaceImageData2 (CardNo ,ImageData ,ImageLen) 
                    SELECT ? ,ImageData ,ImageLen FROM B02_FaceImageData2 WHERE CardNo=? ", liSqlPara);
            Pub.MessageObject objRetAuthReset = (Pub.MessageObject)ExcuteCardAuthReset(UserID, AuthCardNoArray);

            if (!objRetAuthReset.result)
            {
                objRet.result = false;
                objRet.message = objRetAuthReset.message;
            }

            if (objRetAuthReset != null) objRetAuthReset = null;

            if (!objRet.result) return objRet;
            #endregion

            #region 新增臨時卡作業資料至資料庫
            sSql = @" 
                INSERT INTO B01_TempCardRecord 
                    (PsnNo, OrigCardNo, CardNo, BorrowTime, TempDesc, CreateUserID, CreateTime) 
                VALUES (?,?,?,?,?,?,?) ";

            liSqlPara.Clear();
            liSqlPara.Add("S:" + sPsnNo.Trim());
            liSqlPara.Add("S:" + sOrigCardNo.Trim());
            liSqlPara.Add("S:" + sCardNo.Trim());
            liSqlPara.Add("S:" + t1.ToString("yyyy/MM/dd HH:mm:ss"));
            liSqlPara.Add("S:" + sTempDesc.Trim());
            liSqlPara.Add("S:" + UserID.Trim());
            liSqlPara.Add("S:" + t2.ToString("yyyy/MM/dd HH:mm:ss"));

            iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);

            string sRecordID = "";

            if (iResult < 0)
            {
                objRet.result = false;
                objRet.message = "新增臨時卡作業失敗！";
            }
            else
            {
                #region 取得新增臨時卡片借還的資料識別碼
                sSql = @" 
                    SELECT 
                        RecordID FROM B01_TempCardRecord 
                    WHERE PsnNo = ? 
                    AND OrigCardNo = ? 
                    AND CardNo = ? 
                    AND BorrowTime = ? 
                    AND TempDesc = ? 
                    AND CreateUserID = ? 
                    AND CreateTime = ? ";

                liSqlPara.Clear();
                liSqlPara.Add("S:" + sPsnNo.Trim());
                liSqlPara.Add("S:" + sOrigCardNo.Trim());
                liSqlPara.Add("S:" + sCardNo.Trim());
                liSqlPara.Add("S:" + t1.ToString("yyyy/MM/dd HH:mm:ss"));
                liSqlPara.Add("S:" + sTempDesc.Trim());
                liSqlPara.Add("S:" + UserID.Trim());
                liSqlPara.Add("S:" + t2.ToString("yyyy/MM/dd HH:mm:ss"));

                dt = new DataTable();
                oAcsDB.GetDataTable("RecordIDCountTable", sSql, liSqlPara, out dt);

                if ((dt == null) || (dt.Rows.Count <= 0))
                {
                    objRet.result = false;
                    objRet.message = "新增臨時卡片借還紀錄之後在取得資料識別碼失敗！";
                }
                else
                {
                    sRecordID = dt.Rows[0]["RecordID"].ToString();

                    objRet.result = true;
                    objRet.message = sRecordID;
                }
                #endregion
            }

            if (!objRet.result) return objRet;

            #endregion

            #region 將LOG寫入B00_SysLog

            oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, strUserId, strUserName, "0107", "", "", string.Format("借出臨時卡：{0}成功，OrigCardNo={1}，RecordID={2}", sCardNo, sOrigCardNo, sRecordID), "B01_TempCardRecord");

            oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, strUserId, strUserName, "0107", "", "", string.Format("RecordID={0} PsnNo={1} OrigCardNo={2} CardNo={3} BorrowTime={4} TempDesc={5} CreateUserID={6} CreateTime={7}", 
                sRecordID, sPsnNo.Trim(), sOrigCardNo.Trim(), sCardNo.Trim(), t1.ToString("yyyy/MM/dd HH:mm:ss"), sTempDesc.Trim(), UserID.Trim(), DateTime.Now), "B01_TempCardRecord");
            #endregion

            // 完成新增臨時卡借用作業
            return objRet;
        }

        /// <summary>
        /// 更新臨時卡借還作業視窗相關的欄位資料
        /// </summary>
        /// <param name="SelectValue">選擇列的資料列識別碼</param>
        /// <param name="sReturnTime">歸還時間</param>
        /// <param name="sTempDesc">說明</param>
        /// <returns>Pub.MessageObject</returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Update(string SelectValue, string sReturnTime, string sTempDesc)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            List<string> liSqlPara = new List<string>();

            int iResult = -1;
            string sSql = "";
            string sNowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");            
            objRet.act    = "Edit";
            objRet.result = true;

            //確認輸入的相關欄位是否為空字串
            if (SelectValue.Trim() == "")
            {
                objRet.result  = false;
                objRet.message = "沒有選擇任何項目！";
            }
            else if (sReturnTime.Trim() == "")
            {
                objRet.result  = false;
                objRet.message = "歸還時間不可以為空值！";
            }

            if (!objRet.result)
                return objRet;

            //以下步驟要先做臨時卡片號碼的卡片權限(CardAuthAllow)設為無效(0)，接著做來源卡片號碼的卡片權限(CardAuthAllow)設為有效(1)，最再做來源卡片號碼與臨時卡片號碼的卡片權限重整
            #region 取得指定資料列的來源卡片號碼與借出卡片號碼
            //取得指定資料列的來源卡片號碼與借出卡片號碼

            #region sql command
            sSql = "SELECT RecordID, OrigCardNo, CardNo FROM B01_TempCardRecord WHERE RecordID = ?";

            liSqlPara.Clear();
            liSqlPara.Add("I:" + SelectValue.Trim());

            DataTable dt = new DataTable();
            oAcsDB.GetDataTable("TempCardRecordTable", sSql, liSqlPara, out dt);
            #endregion

            if ((dt == null) || (dt.Rows.Count <= 0))
            {
                objRet.result = false;
                objRet.message = "在B01_TempCardRecord資料表找不到相關的資料內容！";

                if (dt != null)
                {
                    dt.Clear();
                    dt = null;
                }

                return objRet;
            }
            string sCardPwd = "0000";
            string sCardNo = "";
            string sOrigCardNo = dt.Rows[0]["OrigCardNo"].ToString();

            if (dt.Rows[0]["CardNo"] != DBNull.Value)
                sCardNo = dt.Rows[0]["CardNo"].ToString();
            else
            {
                objRet.result = false;
                objRet.message = "臨時卡片號碼的歸還作業裡借出卡片號碼不能空字串！";
            }

            if (dt != null)
            {
                dt.Clear();
                dt = null;
            }

            if (!objRet.result)
                return objRet;
            #endregion

            #region 處理來源卡片號碼的卡片權限(CardAuthAllow)設為有效(1)
            
            #region UPD - B01_Card - CardNo
            sSql = " UPDATE B01_Card SET CardAuthAllow = 1 WHERE CardNo = ? ";

            liSqlPara.Clear();
            liSqlPara.Add("S:" + sOrigCardNo);

            iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
            #endregion

            if (iResult < 0)
            {
                objRet.result = false;
                objRet.message = "設定來源卡片號碼：" + sOrigCardNo + "的權限設為「有效」失敗！";

                return objRet;
            }
            else
            {
                oAcsDB.WriteLog(DB_Acs.Logtype.資料修改, strUserId, strUserName, "0107", "", "", string.Format("SET CardAuthAllow=1 WHERE CardNo={0}", sOrigCardNo), "B01_Card");
            }
            #endregion

            #region 處理借出卡片號碼的卡片權限(CardAuthAllow)設為無效(0)
            //處理借出卡片號碼的卡片權限(CardAuthAllow)設為無效(0)
            #region UPD - B01_Card - CardNo
            sSql = @" 
                UPDATE B01_Card SET 
                    CardAuthAllow = 0, PsnID = null, CardPW = '0000' 
                WHERE CardNo = ? ";

            liSqlPara.Clear();
            liSqlPara.Add("S:" + sCardNo);
            iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
            #endregion

            if (iResult < 0)
            {
                objRet.result = false;
                objRet.message = "設定借出卡片號碼：" + sCardNo + "的權限設為「無效」失敗！";

                return objRet;
            }
            else
            {
                oAcsDB.WriteLog(DB_Acs.Logtype.資料修改, strUserId, strUserName, "0107", "", "", string.Format("SET CardAuthAllow=0,PsnID=null,CardPW='0000' WHERE CardNo={0}", sCardNo), "B01_Card");
            }
            #endregion

            #region 刪除臨時卡在 B01_CardEquAdj、B01_CardEquGroup、B01_CardExt 的資料

            #region (一)刪除臨時卡在 B01_CardEquAdj 的資料
            sSql = @"
                DELETE FROM [B01_CardEquAdj] 
                WHERE CardID = (SELECT CardID FROM B01_Card WHERE CardNo=?)";

            liSqlPara = new List<string>();
            liSqlPara.Add("S:" + sCardNo);

            iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);

            if (iResult < 0)
            {
                objRet.result = false;
                objRet.message = "刪除臨時卡：" + sCardNo + "的權限(一)失敗！";

                return objRet;
            }
            else
            {
                // syslog
                oAcsDB.WriteLog(DB_Acs.Logtype.資料刪除, strUserId, strUserName, "0107", "", "", string.Format("CardNo：{0}，delete all EquId", sCardNo), "B01_CardEquAdj");
            }
            #endregion

            #region (二)刪除臨時卡在 B01_CardEquGroup 的資料
            sSql = @"
                DELETE FROM [B01_CardEquGroup] 
                WHERE CardID = (SELECT CardID FROM B01_Card WHERE CardNo=?)";

            liSqlPara = new List<string>();
            liSqlPara.Add("S:" + sCardNo);

            iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);

            if (iResult < 0)
            {
                objRet.result = false;
                objRet.message = "刪除臨時卡：" + sCardNo + "的權限(二)失敗！";

                return objRet;
            }
            else
            {
                // syslog
                oAcsDB.WriteLog(DB_Acs.Logtype.資料刪除, strUserId, strUserName, "0107", "", "", string.Format("CardNo：{0}，delete all EquGrpID", sCardNo), "B01_CardEquGroup");
            }
            #endregion

            #region (三)刪除臨時卡在 B01_CardExt 的資料
            sSql = @"
                DELETE FROM [B01_CardExt] 
                WHERE CardID = (SELECT CardID FROM B01_Card WHERE CardNo = ?)";

            liSqlPara = new List<string>();
            liSqlPara.Add("S:" + sCardNo);

            iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);

            if (iResult < 0)
            {
                objRet.result = false;
                objRet.message = "刪除臨時卡：" + sCardNo + "的權限(三)失敗！";

                return objRet;
            }
            else
            {
                oAcsDB.WriteLog(DB_Acs.Logtype.資料刪除, strUserId, strUserName, "0107", "", "", string.Format("CardNo：{0}，delete all EquGrpID", sCardNo), "B01_CardExt");
            }
            #endregion

            #region (四)處理 B02_B02_FPData 資料表 (指紋辨識)
            // 刪掉臨時卡(sCardNo)的指紋資料
            sSql = string.Format("DELETE FROM B02_FPData WHERE CardNo = '{0}'", sCardNo);
           
            int intRlt = oAcsDB.SqlCommandExecute(sSql);
            #endregion

            #endregion

            #region 處理來源卡片號碼的卡片權限重整
            string[] AuthResetCardNoArray = new string[2];

            // 20170728 修改，針對 ADM100 系列，臨時卡作業歸還，必要先對臨時卡消碼，才對正式卡設碼
            AuthResetCardNoArray[0] = sCardNo;
            AuthResetCardNoArray[1] = sOrigCardNo;
            liSqlPara.Clear();
            liSqlPara.Add("S:" + sCardNo);
            oAcsDB.SqlCommandExecute("DELETE B02_FaceData2 WHERE CardNo=? ", liSqlPara);
            oAcsDB.SqlCommandExecute("DELETE B02_FaceImageData2 WHERE CardNo=? ", liSqlPara);
            Pub.MessageObject objRetAuthReset = (Pub.MessageObject)ExcuteCardAuthReset(UserID, AuthResetCardNoArray);

            if (!objRetAuthReset.result)
            {
                objRet.result = false;
                objRet.message = objRetAuthReset.message;
            }

            if (objRetAuthReset != null)
                objRetAuthReset = null;

            if (!objRet.result) return objRet;
            #endregion

            #region 更新 B01_TempCardRecord 的資料

            var page = System.Web.HttpContext.Current.Handler as Page;
            if (page != null)
            {
                sNowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                sReturnTime = DateTime.Parse(sReturnTime).GetUtcTime(page).ToString("yyyy-MM-dd HH:mm:ss.fff");
            }

            #region Process String - Updata B01_TempCardRecord
            sSql = @" 
                UPDATE B01_TempCardRecord SET 
                    ReturnTime = ?,
                    TempDesc = ?,
                    UpdateUserID = ?,
                    UpdateTime = ? 
                WHERE RecordID = ? ";

            liSqlPara.Clear();
            liSqlPara.Add("S:" + sReturnTime);
            liSqlPara.Add("S:" + sTempDesc.Trim());
            liSqlPara.Add("S:" + UserID);
            liSqlPara.Add("S:" + sNowTime);
            liSqlPara.Add("S:" + SelectValue.Trim());

            iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
            #endregion

            if (iResult <= 0)
            {
                objRet.result = false;
                objRet.message = "更新臨時卡片借還紀錄失敗！";
            }

            #endregion

            #region 將LOG寫入B00_SysLog
            oAcsDB.WriteLog(DB_Acs.Logtype.卡片權限調整, strUserId, strUserName, "0107", "", "", string.Format("歸還臨時卡{0}成功，來源卡片為{1}", sCardNo, sOrigCardNo), "B01_TempCardRecord");
            #endregion

            return objRet;
        }

        /// <summary>
        /// 刪除臨時卡借還作業視窗相關的欄位資料
        /// </summary>
        /// <param name="SelectValue">選擇列的資料列識別碼</param>
        /// <returns>Pub.MessageObject</returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Delete(string SelectValue)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            List<string> liSqlPara = new List<string>();

            int iResult = -1;
            string sSql = "";
            objRet.act = "Delete";

            //確認輸入的相關欄位是否為空字串
            if (SelectValue.Trim() == "")
            {
                objRet.result = false;
                objRet.message = "傳入Delete方法的資料識別碼為空字串！";
            }

            #region 如果該筆資料所屬的臨時卡還存在，則不可刪除該筆紀錄
            sSql = @" 
                SELECT CardNo FROM B01_Card WHERE CardNo = 
                (SELECT TOP 1 CardNo FROM B01_TempCardRecord WHERE RecordID = ?)";

            liSqlPara = new List<string>();
            liSqlPara.Add("I:" + SelectValue.Trim());

            string strCardNo = oAcsDB.GetStrScalar(sSql, liSqlPara);
            if (strCardNo != null)
            {
                objRet.result = false;
                objRet.message += "該筆紀錄所屬的臨時卡還存在資料庫，不可以刪除該筆紀錄。";
            }
            #endregion

            #region 如果該筆資料的使用者(PsnNo)還存在，則不可刪除該筆紀錄
            sSql = @" 
                SELECT PsnNo FROM B01_Person WHERE PsnNo = 
                (SELECT TOP 1 PsnNo FROM B01_TempCardRecord WHERE RecordID = ?)";

            liSqlPara = new List<string>();
            liSqlPara.Add("I:" + SelectValue.Trim());

            string strPsnNo = oAcsDB.GetStrScalar(sSql, liSqlPara);
            if (strPsnNo != null)
            {
                objRet.result = false;
                objRet.message += "該筆紀錄所屬的臨時卡還存在資料庫，不可以刪除該筆紀錄。";
            }
            #endregion

            if (!objRet.result) return objRet;

            #region 刪除資料庫指定的資料
            sSql = " DELETE FROM B01_TempCardRecord WHERE RecordID = ? ";

            liSqlPara = new List<string>();
            liSqlPara.Add("I:" + SelectValue.Trim());

            iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
            #endregion

            if (iResult <= 0)
            {
                objRet.result = false;
                objRet.message = "刪除臨時卡片借還紀錄失敗！";
            }
            else
            {
                oAcsDB.WriteLog(DB_Acs.Logtype.資料刪除, strUserId, strUserName, "0107", "", "", string.Format("RecordID={0}", SelectValue.Trim()), "B01_CardEquGroup");
            }

            return objRet;
        }

        /// <summary>
        /// 取消臨時卡借還作業視窗相關的欄位資料
        /// </summary>
        /// <param name="SelectValue">選擇列的資料列識別碼</param>
        /// <returns>Pub.MessageObject</returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object CancelAction(string SelectValue)
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            objRet.act = "Cancel";

            return objRet;
        }
        #endregion

        #region 讀取卡號相關  

        // 從人員編號對應到卡號
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] GetCardData(string strPsnID)
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            bool IsSuccess = true;
            DBReader dr = new DBReader();
            List<string> liSqlPara = new List<string>();
            string[] EditData = new string[2];

            string sql = @"
                SELECT 
                    C.CardNo, C.CardType, P.PsnName, P.PsnAuthAllow  
                FROM [B01_Person] P 
                LEFT JOIN [B01_Card] C ON C.PsnID = P.PsnID 
                WHERE C.CardNo IS NOT NULL AND C.CardNo <> '' 
                AND P.PsnNo = ? ";

            liSqlPara.Clear();
            liSqlPara.Add("S:" + strPsnID);

            IsSuccess = oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (IsSuccess)
            {
                if (dr.HasRows)
                {
                    string strTempCardNo = "";      // 臨時卡的資料
                    string strCardNo = "";          // 一般卡的資料
                    string strPsnAuthAllow = "";    // 人員權限
                    string strCardType = "";        // 卡片類型
                    string strPsnName = "";         // 姓名

                    while (dr.Read())
                    {
                        strCardType = dr.DataReader[1].ToString();
                        strPsnName = dr.DataReader[2].ToString();
                        strPsnAuthAllow = dr.DataReader[3].ToString();

                        // 卡片類型CardType(E:員工卡;F:廠商卡;R:臨時卡;C:停車卡;G:警巡卡;M:母卡)，參考 ItemList.CardType
                        if (strCardType == "R")
                        {
                            strTempCardNo = dr.DataReader[0].ToString();
                        }
                        else
                        {
                            strCardNo = dr.DataReader[0].ToString();
                        }
                    }

                    // 人員權限(0:無效;1:有效)
                    if (strPsnAuthAllow != "1")
                    {
                        EditData[0] = "Saho_SysErrorMassage";
                        EditData[1] = "輸入的人員編號：" + strPsnID + "該權限為無效！";
                    }
                    else
                    {
                        // 如果臨時卡的部份有資料，則不可再申請臨時卡
                        if (strTempCardNo != "")
                        {
                            EditData[0] = "Saho_SysErrorMassage";
                            EditData[1] = "這個使用者已經申請臨時卡！";
                        }
                        else
                        {
                            EditData[0] = "GetCardNo";
                            EditData[1] = strCardNo + "___" + strPsnName;
                        }
                    }
                }

                if (!dr.HasRows)
                {
                    EditData[0] = "Saho_SysErrorMassage";
                    EditData[1] = "資料庫內無相對應的人員和卡片資料！";
                }
            }
            else
            {
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "讀取人員和卡片資料失敗！";
            }

            return EditData;
        }

        // 讀取目前可用的臨時卡的卡號
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] GetTempCardNo()
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            bool IsSuccess = true;
            DBReader dr = new DBReader();
            List<string> liSqlPara = new List<string>();
            string[] EditData = new string[2];

            string sql = @"
                SELECT 
                    C.CardNo, C.CardId  
                FROM B01_Card C 
                LEFT JOIN B00_ItemList L ON L.ItemClass = 'CardType' AND L.ItemNo = C.CardType 
                LEFT JOIN dbo.B01_Person P ON P.PsnID = C.PsnID   
                WHERE C.CardType = 'R' AND C.CardAuthAllow = 0 
                ORDER BY C.CardNo ";

            IsSuccess = oAcsDB.GetDataReader(sql, out dr);

            if (IsSuccess)
            {
                if (dr.HasRows)
                {
                    int i = 1;

                    EditData[0] = "GetTempCardNo";
                    EditData[1] = stringDefaultListItem;        // 選取資料

                    while (dr.Read())
                    {
                        i += 1;

                        if (EditData.Length < i + 1)
                        {
                            Array.Resize(ref EditData, EditData.Length + 1);
                        }

                        EditData[i] = dr.DataReader[0].ToString();
                    }
                }
                else
                {
                    EditData[0] = "Saho_DbHaveNoData";
                    EditData[1] = "資料庫內無相對應的臨時卡資料！";
                }
            }
            else
            {
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "讀取臨時卡資料失敗！";
            }

            return EditData;
        }

        #endregion

        #region 六.權限複製
        /// <summary>
        /// 執行卡片權限複製
        /// </summary>
        /// <param name="sUserID"></param>
        /// <param name="sSourceCardNo"></param>
        /// <param name="sObjectCardNoArray"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object ExcuteCardAuthCopy(string sUserID, string sSourceCardNo, string[] sObjectCardNoArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();

            List<string> liSqlPara = new List<string>();
            DataTable dt = new DataTable();
            DBReader dr = new DBReader();
            Hashtable htCardIDList = new Hashtable();
            bool IsSuccess = true;

            int iResult = -1;
            string sSql = "", sDelWhereSql = "", sSourceCardID = "", sObjectCardID = "";
            string sNowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            objRet.act    = "ExcuteCardAuthCopy";
            objRet.result = true;

            #region 確認輸入的相關欄位是否為空字串

            if (sUserID.Trim() == "")
            {
                objRet.result = false;
                objRet.message = "傳入ExcuteCardAuthCopy方法的使用者編號之參數值內容為空字串！";
            }
            else if (sSourceCardNo.Trim() == "")
            {
                objRet.result = false;
                objRet.message = "傳入ExcuteCardAuthCopy方法的複製來源卡號之參數值內容為空字串！";
            }
            else if (sObjectCardNoArray == null)
            {
                objRet.result = false;
                objRet.message = "傳入ExcuteCardAuthCopy方法的複製目標卡號之參數值陣列為空值！";
            }
            else if (sObjectCardNoArray.Length <= 0)
            {
                objRet.result = false;
                objRet.message = "傳入ExcuteCardAuthCopy方法的複製目標卡號之參數值長度為零！";
            }

            if (!objRet.result) return objRet;

            #endregion

            #region [暫不用]取得資料庫所有CardNo的CardID清單

            //sSql = " SELECT CardID, CardNo";
            //sSql += " FROM B01_Card ";

            //oAcsDB.GetDataTable("CardIDListTable", sSql, out dt);

            //if ((dt == null) || (dt.Rows.Count <= 0))
            //{
            //    objRet.result = false;
            //    objRet.message = "沒有找到相關的CardID清單！";

            //    return objRet;
            //}

            #endregion

            #region [暫不用]取得來源卡片號碼與目的卡片號碼相關的卡片識別碼清單

            //htCardIDList = new Hashtable();

            //foreach (DataRow dataRow in dt.Rows)
            //{
            //    string sCardID1 = dataRow["CardID"].ToString();
            //    string sCardNo1 = dataRow["CardNo"].ToString();

            //    if (sCardNo1 == sSourceCardNo.Trim())
            //        htCardIDList.Add(sCardNo1, sCardID1);
            //    else
            //    {
            //        foreach (string sCardNo2 in sObjectCardNoArray)
            //        {
            //            if (sCardNo1 == sCardNo2)
            //            {
            //                htCardIDList.Add(sCardNo1, sCardID1);
            //                break;
            //            }
            //        }
            //    }
            //}

            #endregion

            #region [暫不用]確認htCardIDList的CardID數量與輸入來源與目標卡片號碼總和是否相同

            //objRet.message = "";

            //if (htCardIDList.Count != (sObjectCardNoArray.Length + 1))
            //{
            //    if (!htCardIDList.ContainsKey(sSourceCardNo))
            //        objRet.message = sSourceCardNo;

            //    foreach (string sObjCardNo in sObjectCardNoArray)
            //    {
            //        if (!htCardIDList.ContainsKey(sObjCardNo))
            //        {
            //            if (objRet.message != "")
            //                objRet.message += ",";

            //            objRet.message += sObjCardNo;
            //        }
            //    }

            //    if (objRet.message != "")
            //    {
            //        objRet.result = false;
            //        objRet.message = "目前在卡片資料的資料表裡，沒有下列的卡片號碼：" + objRet.message + "。";

            //        if (htCardIDList != null)
            //        {
            //            htCardIDList.Clear();
            //            htCardIDList = null;
            //        }

            //        return objRet;
            //    }
            //}

            #endregion

            #region 取得卡片和臨時卡的CardNo和CardID

            string strWhereInCondition = "";

            foreach (string str in sObjectCardNoArray)
            {
                if (strWhereInCondition == "")
                {
                    strWhereInCondition = "?, ?";
                }
                else
                {
                    strWhereInCondition += ", ?";
                }
            }

            sSql = "SELECT CardID, CardNo FROM B01_Card WHERE CardNo IN (" + strWhereInCondition + ") ";

            liSqlPara.Clear();
            liSqlPara.Add("S:" + sSourceCardNo);

            int intIndex = 0;
            foreach (string str in sObjectCardNoArray)
            {
                liSqlPara.Add("S:" + sObjectCardNoArray[intIndex]);
                intIndex += 1;
            }

            dr = new DBReader();
            IsSuccess = oAcsDB.GetDataReader(sSql, liSqlPara, out dr);

            if (IsSuccess)
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        string strCardID = dr.DataReader[0].ToString();     // CardID
                        string strCardNo = dr.DataReader[1].ToString();     // CardNo

                        htCardIDList.Add(strCardNo, strCardID);
                    }
                }
                else
                {
                    objRet.result = false;
                    objRet.message = "沒有找到相關的CardID清單！";

                    return objRet;
                }
            }
            else
            {
                objRet.result = false;
                objRet.message = "讀取資料庫失敗！";

                return objRet;
            }

            #endregion

            #region 一、處理B01_Card資料表

            //取得來源卡片號碼相關的卡片額外資料，並刪除目的卡片號碼相關的卡片額外資料
            sSourceCardID = htCardIDList[sSourceCardNo.Trim()].ToString();

            #region GET - B01_CardExt - CardID
            sSql = @" 
                SELECT 
                    CardID, CardNo, CardAuthAllow, CardSTime, CardETime, PsnID, CardPW
                FROM B01_Card 
                WHERE CardID = ? ";

            liSqlPara.Clear();
            liSqlPara.Add("I:" + sSourceCardID);

            dt = new DataTable();
            IsSuccess = oAcsDB.GetDataTable("CardTable", sSql, liSqlPara, out dt);
            #endregion

            if (!IsSuccess)
            {
                objRet.result = false;
                objRet.message = "目前沒有來源卡片號碼相關的卡片資料！";

                if (htCardIDList != null)
                {
                    htCardIDList.Clear();
                    htCardIDList = null;
                }

                return objRet;
            }
            else
            {
                string sCardIDValue1 = "", sCardNoValue1 = "", sCardAuthAllow1 = "", sCardSTime1 = "", sCardETime1 = "", sPsnID = "", sCardPW="";

                //取得來源卡片號碼相關的欄位資料
                DataRow dataRow = dt.Rows[0];

                sCardIDValue1 = dataRow["CardID"].ToString();
                sCardPW = dataRow["CardPW"].ToString();

                if (dataRow["CardNo"] == DBNull.Value)
                    sCardNoValue1 = "NULL";
                else
                    sCardNoValue1 = dataRow["CardNo"].ToString();

                sCardAuthAllow1 = dataRow["CardAuthAllow"].ToString();
                sCardSTime1 = dataRow["CardSTime"].ToString();

                if (dataRow["CardETime"] == DBNull.Value)
                    sCardETime1 = "NULL";
                else
                    sCardETime1 = dataRow["CardETime"].ToString();

                sPsnID = dataRow["PsnID"].ToString();

                //更新來源卡片號碼相關的欄位資料至所有目標卡片號碼的相關的欄位資料
                objRet.message = "";

                string sCardSTime = DateTime.Parse(sCardSTime1).ToString("yyyy-MM-dd HH:mm:ss.fff");
                string sCardETime = "";

                foreach (string sObjectCardNo in sObjectCardNoArray)
                {
                    sObjectCardID = htCardIDList[sObjectCardNo].ToString();

                    #region UPD - B01_Card - CardID

                    sSql = @" 
                        UPDATE B01_Card SET 
                            CardAuthAllow = ?, 
                            PsnID = ?, 
                            CardSTime = ?, 
                            CardETime = ?,
                            UpdateUserID = ?, 
                            UpdateTime = ?,
                            CardPW = ?
                        WHERE CardID = ? ";

                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + sCardAuthAllow1);
                    liSqlPara.Add("I:" + sPsnID);
                    liSqlPara.Add("D:" + sCardSTime);
                    if (sCardETime1 == "NULL")
                    {
                        sCardETime = sCardETime1;
                        liSqlPara.Add("");
                    }
                    else
                    {
                        sCardETime = DateTime.Parse(sCardETime1).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        liSqlPara.Add("D:" + sCardETime);
                    }
                    liSqlPara.Add("S:" + sUserID.Trim());
                    liSqlPara.Add("D:" + sNowTime);
                    liSqlPara.Add("S:" + sCardPW);
                    liSqlPara.Add("I:" + sObjectCardID);

                    iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
                    #endregion

                    if (iResult <= 0)
                    {
                        if (objRet.message != "")
                            objRet.message += ",";

                        objRet.message += sObjectCardID;
                    }
                }

                if (objRet.message != "")
                {
                    objRet.result = false;
                    objRet.message = "卡片權限複製更新B01_Card失敗的卡片號碼有：" + objRet.message + "。";

                    if (htCardIDList != null)
                    {
                        htCardIDList.Clear();
                        htCardIDList = null;
                    }

                    return objRet;
                }
                else
                {
                    // syslog
                    foreach (string sObjectCardNo in sObjectCardNoArray)
                    {
                        oAcsDB.WriteLog(DB_Acs.Logtype.卡片權限調整, strUserId, strUserName, "0107", "", "", string.Format("將CardNo：{0}的資料寫入CardNo：{1}，Update CardAuthAllow={2} PsnID={3} CardSTime = {4} CardETime={5}", sSourceCardNo, sObjectCardNo, sCardAuthAllow1,sPsnID, sCardSTime, sCardETime), "B01_Card");
                    }
                }
            }
            #endregion

            #region 二、處理B01_CardEquAdj資料表

            //二、處理B01_CardEquAdj資料表

            //取得來源卡片號碼相關的卡片設備權限調整資料，並刪除目的卡片號碼相關的片設備權限調整資料
            sSourceCardID = htCardIDList[sSourceCardNo.Trim()].ToString();

            #region GET - B01_CardEquAdj - CardID
            sSql = @" 
                SELECT CardID, EquID, OpMode, CardRule, CardExtData, BeginTime, EndTime 
                FROM B01_CardEquAdj 
                WHERE CardID = ? ";

            liSqlPara.Clear();
            liSqlPara.Add("I:" + sSourceCardID.Trim());

            dt = new DataTable();
            IsSuccess = oAcsDB.GetDataTable("CardEquAdjTable", sSql, liSqlPara, out dt);
            #endregion

            if (IsSuccess)
            {
                #region 刪除來源卡片的B01_CardEquAdj資料
                #region DEL - B01_CardEquAdj - CardID
                sSql = " DELETE FROM B01_CardEquAdj WHERE ";

                liSqlPara.Clear();
                foreach (string sObjectCardNo in sObjectCardNoArray)
                {
                    sObjectCardID = htCardIDList[sObjectCardNo].ToString();

                    if (sDelWhereSql != "") sDelWhereSql += "OR";
                    sDelWhereSql += " CardID = ? ";

                    liSqlPara.Add("S:" + sObjectCardID);
                }

                sSql += sDelWhereSql;

                iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
                #endregion

                if (iResult < 0)
                {
                    objRet.result = false;
                    objRet.message = "刪除B01_CardEquAdj資料失敗！";

                    return objRet;
                }
                else
                {
                    // syslog
                    foreach (string sObjectCardNo in sObjectCardNoArray)
                    {
                        oAcsDB.WriteLog(DB_Acs.Logtype.卡片權限調整, strUserId, strUserName, "0107", "", "", string.Format("CardNo：{0}，delete all EquId for addnew", sObjectCardNo), "B01_CardEquAdj");
                    } 
                }
                #endregion

                #region 複製來源卡片號碼相關的卡片設備權限資料到每一個目標卡片號碼

                //複製來源卡片號碼相關的卡片設備權限資料到每一個目標卡片號碼
                string sCardIDValue1 = "", sEquIDValue2 = "", sOpModeValue2 = "";
                //string sCardRuleValue2 = "", sCardExtDataValue2 = "", sBeginTimeValue2 = "", sEndTimeValue2 = "";

                objRet.message = "";

                foreach (string sObjectCardNo in sObjectCardNoArray)
                {
                    sObjectCardID = htCardIDList[sObjectCardNo].ToString();

                    foreach (DataRow dataRow in dt.Rows)
                    {
                        #region //comment out old code//
                        //sCardIDValue1 = sObjectCardID.Trim();
                        //sEquIDValue2 = dataRow["EquID"].ToString();
                        //sOpModeValue2 = dataRow["OpMode"].ToString();

                        //if (dataRow["CardRule"] == DBNull.Value)
                        //    sCardRuleValue2 = "NULL";
                        //else
                        //    sCardRuleValue2 = dataRow["CardRule"].ToString();

                        //if (dataRow["CardExtData"] == DBNull.Value)
                        //    sCardExtDataValue2 = "NULL";
                        //else
                        //    sCardExtDataValue2 = dataRow["CardExtData"].ToString();

                        //if (dataRow["BeginTime"] == DBNull.Value)
                        //    sBeginTimeValue2 = "NULL";
                        //else
                        //    sBeginTimeValue2 = dataRow["BeginTime"].ToString();

                        //if (dataRow["EndTime"] == DBNull.Value)
                        //    sEndTimeValue2 = "NULL";
                        //else
                        //    sEndTimeValue2 = dataRow["EndTime"].ToString();

                        //#region INS - B01_CardEquAdj - CardID, EquID
                        //sSql = " INSERT INTO B01_CardEquAdj";
                        //sSql += " (CardID, EquID, OpMode, CardRule, CardExtData, BeginTime, EndTime, CreateUserID, CreateTime)";
                        //sSql += " VALUES";
                        //sSql += "('" + sCardIDValue1.Trim() + "',";
                        //sSql += " '" + sEquIDValue2.Trim() + "',";
                        //sSql += " '" + sOpModeValue2.Trim() + "',";

                        //if (sCardRuleValue2.Trim() == "NULL")
                        //    sSql += " " + sCardRuleValue2.Trim() + ",";
                        //else
                        //    sSql += " '" + sCardRuleValue2.Trim() + "',";

                        //if (sCardExtDataValue2.Trim() == "NULL")
                        //    sSql += " " + sCardExtDataValue2.Trim() + ",";
                        //else
                        //    sSql += " '" + sCardExtDataValue2.Trim() + "',";

                        //if (sBeginTimeValue2.Trim() == "NULL")
                        //    sSql += " " + sBeginTimeValue2.Trim() + ",";
                        //else
                        //    sSql += " '" + DateTime.Parse(sBeginTimeValue2.Trim()).ToString("yyyy-MM-dd HH:mm:ss.fff") + "',";

                        //if (sEndTimeValue2.Trim() == "NULL")
                        //    sSql += " " + sEndTimeValue2.Trim() + ",";
                        //else
                        //    sSql += " '" + DateTime.Parse(sEndTimeValue2.Trim()).ToString("yyyy-MM-dd HH:mm:ss.fff") + "',";

                        //sSql += " '" + sUserID.Trim() + "',";
                        //sSql += " '" + sNowTime.Trim() + "') ";

                        //iResult = oAcsDB.SqlCommandExecute(sSql);
                        #endregion

                        sSql = @"
                            INSERT INTO B01_CardEquAdj 
                            (
                                CardID, EquID, OpMode, CardRule, CardExtData, 
                                BeginTime, EndTime, CreateUserID, CreateTime 
                            ) 
                            VALUES (?,?,?,?,?,?,?,?,?) ";

                        liSqlPara.Clear();
                        liSqlPara.Add("I:" + sObjectCardID.Trim());
                        liSqlPara.Add("I:" + dataRow["EquID"].ToString().Trim());
                        liSqlPara.Add("S:" + dataRow["OpMode"].ToString().Trim());

                        if (dataRow["CardRule"] == DBNull.Value)
                            liSqlPara.Add("");
                        else
                            liSqlPara.Add("A:" + dataRow["CardRule"].ToString().Trim());

                        if (dataRow["CardExtData"] == DBNull.Value)
                            liSqlPara.Add("");
                        else
                            liSqlPara.Add("A:" + dataRow["CardExtData"].ToString().Trim());

                        if (dataRow["BeginTime"] == DBNull.Value)
                            liSqlPara.Add("");
                        else
                            liSqlPara.Add("D:" + DateTime.Parse(dataRow["BeginTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss.fff").Trim());

                        if (dataRow["EndTime"] == DBNull.Value)
                            liSqlPara.Add("");
                        else
                            liSqlPara.Add("D:" + DateTime.Parse(dataRow["EndTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss.fff").Trim());

                        liSqlPara.Add("I:" + sUserID.Trim());
                        liSqlPara.Add("S:" + sNowTime.Trim());

                        iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);

                        if (iResult < 0)
                        {
                            if (objRet.message != "")
                                objRet.message += ",";

                            objRet.message += sObjectCardNo;
                        }
                    }
                }

                if (objRet.message != "")
                {
                    objRet.result = true;
                    objRet.message = "複製來源卡片號碼相關的卡片設備權限資料失敗有：" + objRet.message + "。";

                    if (htCardIDList != null)
                    {
                        htCardIDList.Clear();
                        htCardIDList = null;
                    }

                    return objRet;
                }
                else
                {
                    // 全部無誤，才將 LOG 寫入 B00_SysLog
                    foreach (string sObjectCardNo in sObjectCardNoArray)
                    {
                        sObjectCardID = htCardIDList[sObjectCardNo].ToString();

                        foreach (DataRow dataRow in dt.Rows)
                        {
                            sCardIDValue1 = sObjectCardID.Trim();
                            sEquIDValue2 = dataRow["EquID"].ToString();
                            sOpModeValue2 = dataRow["OpMode"].ToString();

                            oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, strUserId, strUserName, "0107", "", "", string.Format("CardNo：{0}，add EquId {1} set OpMode={2}", sObjectCardNo, sEquIDValue2, sOpModeValue2), "B01_CardEquAdj");
                        }
                    }
                }
                #endregion
            }

            #endregion

            #region 三、處理B01_CardEquGroup資料表

            //三、處理B01_CardEquGroup資料表

            //取得來源卡片號碼相關的卡片設備群組資料，並刪除目的卡片號碼相關的卡片設備群組資料
            sSourceCardID = htCardIDList[sSourceCardNo.Trim()].ToString();

            #region GET - B01_CardEquGroup - CardID
            sSql = " SELECT CardID, EquGrpID FROM B01_CardEquGroup WHERE CardID = ? ";

            liSqlPara.Clear();
            liSqlPara.Add("I:" + sSourceCardID.Trim());

            dt = new DataTable();
            oAcsDB.GetDataTable("CardEquGroupTable", sSql, liSqlPara, out dt);
            #endregion

            if ((dt == null) || (dt.Rows.Count < 0))
            {
                objRet.result = false;
                objRet.message = "目前沒有來源卡號碼相關的卡片設備群組資料！";

                if (htCardIDList != null)
                {
                    htCardIDList.Clear();
                    htCardIDList = null;
                }
            }

            #region DEL - B01_CardEquGroup - CardID
            sSql = " DELETE FROM B01_CardEquGroup WHERE ";

            sDelWhereSql = "";
            liSqlPara.Clear();

            foreach (string sObjectCardNo in sObjectCardNoArray)
            {
                sObjectCardID = htCardIDList[sObjectCardNo].ToString();

                if (sDelWhereSql != "") sDelWhereSql += "OR";
                sDelWhereSql += " CardID = ? ";

                liSqlPara.Add("I:" + sObjectCardID );
            }

            sSql += sDelWhereSql;

            iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);

            #endregion

            if (iResult < 0)
            {
                objRet.result = false;
                objRet.message = "刪除B01_CardEquGroup資料失敗！";

                return objRet;
            }
            else
            {
                // syslog
                foreach (string sObjectCardNo in sObjectCardNoArray)
                {
                    oAcsDB.WriteLog(DB_Acs.Logtype.資料刪除, strUserId, strUserName, "0107", "", "", string.Format("CardNo：{0}，delete all EquGrpID for addnew", sObjectCardNo), "B01_CardEquGroup");
                }
            }

            //複製來源卡片號碼相關的卡片設備群組資料到每一個目標卡片號碼
            objRet.message = "";

            foreach (string sObjectCardNo in sObjectCardNoArray)
            {
                sObjectCardID = htCardIDList[sObjectCardNo].ToString();

                foreach (DataRow dataRow in dt.Rows)
                {
                    string sEquGrpID = dataRow["EquGrpID"].ToString();

                    #region //comment out old code//
                    //sSql = " INSERT INTO B01_CardEquGroup";
                    //sSql += " (CardID, EquGrpID, CreateUserID, CreateTime)";
                    //sSql += " VALUES";
                    //sSql += "('" + sObjectCardID.Trim() + "',";
                    //sSql += " '" + sEquGrpID.Trim() + "',";
                    //sSql += " '" + sUserID.Trim() + "',";
                    //sSql += " '" + sNowTime.Trim() + "') ";

                    //iResult = oAcsDB.SqlCommandExecute(sSql);
                    #endregion

                    sSql = @" 
                        INSERT INTO B01_CardEquGroup 
                            (CardID, EquGrpID, CreateUserID, CreateTime) 
                        VALUES 
                            (?, ?, ?, ?) ";

                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + sObjectCardID.Trim());
                    liSqlPara.Add("I:" + sEquGrpID.Trim());
                    liSqlPara.Add("S:" + sUserID.Trim());
                    liSqlPara.Add("D:" + sNowTime.Trim());

                    iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
                }

                if (iResult < 0)
                {
                    if (objRet.message != "")
                        objRet.message += ",";

                    objRet.message += sObjectCardNo;
                }
            }

            if (objRet.message != "")
            {
                objRet.result = false;
                objRet.message = "複製來源卡片號碼相關的卡片設備群組失敗有：" + objRet.message + "。";

                if (htCardIDList != null)
                {
                    htCardIDList.Clear();
                    htCardIDList = null;
                }

                return objRet;
            }
            else
            {
                // 全部無誤，才將 LOG 寫入 B00_SysLog
                foreach (string sObjectCardNo in sObjectCardNoArray)
                {
                    sObjectCardID = htCardIDList[sObjectCardNo].ToString();

                    foreach (DataRow dataRow in dt.Rows)
                    {
                        string sEquGrpID = dataRow["EquGrpID"].ToString();
                        oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, strUserId, strUserName, "0107", "", "", string.Format("CardNo：{0}，add EquGrpID {1}", sObjectCardNo, sEquGrpID), "B01_CardEquGroup");
                    }
                }
            }

            #endregion

            #region 四、處理B01_CardExt資料表
            //取得來源卡片號碼相關的卡片額外資料，並刪除目的卡片號碼相關的卡片額外資料
            sSourceCardID = htCardIDList[sSourceCardNo.Trim()].ToString();

            #region GET - B01_CardExt - CardID
            sSql =  @" 
                SELECT 
                    CardID, CardLock, LockTime, LockReason, CardBorrow, 
                    LastTime, LastDoorNo, CtrlAreaNo 
                FROM B01_CardExt 
                WHERE CardID = ? ";

            liSqlPara.Clear();
            liSqlPara.Add("I:" + sSourceCardID.Trim());

            dt = new DataTable();
            oAcsDB.GetDataTable("CardExtTable", sSql, liSqlPara, out dt);
            #endregion

            if ((dt == null) || (dt.Rows.Count < 0))
            {
                objRet.result = false;
                objRet.message = "目前沒有來源卡號碼相關的卡片額外資料！";

                if (dt != null)
                {
                    dt.Clear();
                    dt = null;
                }

                if (htCardIDList != null)
                {
                    htCardIDList.Clear();
                    htCardIDList = null;
                }
            }

            #region DEL - B01_CardExt - CardID
            sSql = " DELETE FROM B01_CardExt WHERE ";

            sDelWhereSql = "";

            liSqlPara.Clear();

            foreach (string sObjectCardNo in sObjectCardNoArray)
            {
                sObjectCardID = htCardIDList[sObjectCardNo].ToString();

                if (sDelWhereSql != "")
                    sDelWhereSql += "OR";

                sDelWhereSql += " CardID = ? ";

                liSqlPara.Add("I:" + sObjectCardID);
            }

            sSql += sDelWhereSql;

            iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
            #endregion

            if (iResult < 0)
            {
                objRet.result = false;
                objRet.message = "刪除B01_CardExt資料夾失敗！";

                if (dt != null)
                {
                    dt.Clear();
                    dt = null;
                }

                return objRet;
            }

            //複製來源卡片號碼相關的卡片額外資料到每一個目標卡片號碼
            if (dt.Rows.Count != 0)
            {
                DataRow drSource = dt.Rows[0];

                //string sCardIDValue4 = "", sCardLockValue4 = "", sLockTimeValue4 = "", sLockReasonValue4 = "", sCardBorrowValue4 = "", sLastTimeValue4 = "", sLastDoorNoValue4 = "", sCtrlAreaNoValue4 = "";

                objRet.message = "";

                foreach (string sObjectCardNo in sObjectCardNoArray)
                {
                    #region //comment out old code//
                    //sObjectCardID = htCardIDList[sObjectCardNo].ToString();

                    //sCardIDValue4 = sObjectCardID.Trim();
                    //sCardLockValue4 = drSource["CardLock"].ToString();

                    //if (drSource["LockTime"] == DBNull.Value)
                    //    sLockTimeValue4 = "NULL";
                    //else
                    //    sLockTimeValue4 = drSource["LockTime"].ToString();

                    //if (drSource["LockReason"] == DBNull.Value)
                    //    sLockReasonValue4 = "NULL";
                    //else
                    //    sLockReasonValue4 = drSource["LockReason"].ToString();

                    //sCardBorrowValue4 = drSource["CardBorrow"].ToString();

                    //if (drSource["LastTime"] == DBNull.Value)
                    //    sLastTimeValue4 = "NULL";
                    //else
                    //    sLastTimeValue4 = drSource["LastTime"].ToString();

                    //if (drSource["LastDoorNo"] == DBNull.Value)
                    //    sLastDoorNoValue4 = "NULL";
                    //else
                    //    sLastDoorNoValue4 = drSource["LastDoorNo"].ToString();

                    //if (drSource["CtrlAreaNo"] == DBNull.Value)
                    //    sCtrlAreaNoValue4 = "NULL";
                    //else
                    //    sCtrlAreaNoValue4 = drSource["CtrlAreaNo"].ToString();

                    //sSql = " INSERT INTO B01_CardExt";
                    //sSql += " (CardID, CardLock, LockTime, LockReason, CardBorrow, LastTime, LastDoorNo, CtrlAreaNo)";
                    //sSql += " VALUES";
                    //sSql += "('" + sCardIDValue4.Trim() + "',";
                    //sSql += " " + sCardLockValue4.Trim() + ",";

                    //if (sLockTimeValue4.Trim() == "NULL")
                    //    sSql += " " + sLockTimeValue4.Trim() + ",";
                    //else
                    //    sSql += " '" + DateTime.Parse(sLockTimeValue4.Trim()).ToString("yyyy-MM-dd HH:mm:ss.fff") + "',";

                    //if (sLockReasonValue4.Trim() == "NULL")
                    //    sSql += " " + sLockReasonValue4.Trim() + ",";
                    //else
                    //    sSql += " '" + sLockReasonValue4.Trim() + "',";

                    //sSql += " " + sCardBorrowValue4.Trim() + ",";

                    //if (sLastTimeValue4.Trim() == "NULL")
                    //    sSql += " " + sLastTimeValue4.Trim() + ",";
                    //else
                    //    sSql += " '" + DateTime.Parse(sLastTimeValue4.Trim()).ToString("yyyy-MM-dd HH:mm:ss.fff") + "',";

                    //if (sLastDoorNoValue4.Trim() == "NULL")
                    //    sSql += " " + sLastDoorNoValue4.Trim() + ",";
                    //else
                    //    sSql += " '" + sLastDoorNoValue4.Trim() + "',";

                    //if (sLastDoorNoValue4.Trim() == "NULL")
                    //    sSql += " " + sLastDoorNoValue4.Trim() + ") ";
                    //else
                    //    sSql += " '" + sLastDoorNoValue4.Trim() + "') ";

                    //iResult = oAcsDB.SqlCommandExecute(sSql);
                    #endregion

                    #region INS - B01_CardExt - CardID

                    sObjectCardID = htCardIDList[sObjectCardNo].ToString();
                    
                    sSql = @" 
                        INSERT INTO B01_CardExt 
                        (
                            CardID, 
                            CardLock, 
                            LockTime, 
                            LockReason, 
                            CardBorrow, 
                            LastTime, 
                            LastDoorNo, 
                            CtrlAreaNo 
                        ) 
                        VALUES (?, ?, ?, ?, ?, ?, ?, ?) ";

                    liSqlPara.Clear();
                    liSqlPara.Add("I:" + sObjectCardID.Trim());
                    liSqlPara.Add("I:" + drSource["CardLock"].ToString());

                    if (drSource["LockTime"] == DBNull.Value)
                        liSqlPara.Add("");
                    else
                        liSqlPara.Add("D:" + DateTime.Parse(drSource["LockTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss.fff").Trim());

                    if (drSource["LockReason"] == DBNull.Value)
                        liSqlPara.Add("");
                    else
                        liSqlPara.Add("S:" + drSource["LockReason"].ToString().Trim());

                    if (drSource["CardBorrow"] == DBNull.Value)
                        liSqlPara.Add("");
                    else
                        liSqlPara.Add("I:" + drSource["CardBorrow"].ToString().Trim());

                    if (drSource["LastTime"] == DBNull.Value)
                        liSqlPara.Add("");
                    else
                        liSqlPara.Add("D:" + DateTime.Parse(drSource["LastTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss.fff").Trim());

                    if (drSource["LastDoorNo"] == DBNull.Value)
                        liSqlPara.Add("");
                    else
                        liSqlPara.Add("A:" + drSource["LastDoorNo"].ToString().Trim());

                    if (drSource["CtrlAreaNo"] == DBNull.Value)
                        liSqlPara.Add("");
                    else
                        liSqlPara.Add("A:" + drSource["CtrlAreaNo"].ToString().Trim());

                    iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
                    #endregion

                    if (iResult < 0)
                    {
                        if (objRet.message != "")
                            objRet.message += ",";

                        objRet.message += sObjectCardNo;
                    }
                }
            }

            if (dt != null)
            {
                dt.Clear();
                dt = null;
            }

            if (objRet.message != "")
            {
                objRet.result = false;
                objRet.message = "複製來源卡片號碼相關的卡片額外資料失敗有：" + objRet.message + "。";

                //if (drSource != null)
                //    drSource = null;

                if (htCardIDList != null)
                {
                    htCardIDList.Clear();
                    htCardIDList = null;
                }

                return objRet;
            }

            #endregion

            #region 五、處理 B02_B02_FPData 資料表 (指紋辨識)
            // 使用來源卡片(sSourceCardNo)以新增臨時卡(sObjectCardNoArray[0])的指紋資料
            sSql = string.Format(@"
                INSERT INTO B02_FPData 
                SELECT '{0}', FPAmount, FPData, FPScore, FPTemplateType FROM B02_FPData WHERE CardNo = '{1}'", 
                sObjectCardNoArray[0], sSourceCardNo);
                
            int intRlt = oAcsDB.SqlCommandExecute(sSql);
            #endregion

            #region [暫不用]複製目標卡號做卡片權限重整
            ////五、複製目標卡號做卡片權限重整
            //Pub.MessageObject objRetReset = (Pub.MessageObject)ExcuteCardAuthReset(sUserID, sObjectCardNoArray);

            //if (objRetReset.message == "")
            //{
            //    objRet.message = "執行卡片權限複製成功！";
            //} 
            //else
            //{
            //    objRet.result = false;
            //    objRet.message += "下列卡片號碼做卡片重整失敗，分別有" + objRetReset.message + "。";
            //}
            #endregion

            return objRet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sUserID"></param>
        /// <param name="sCardNoArray"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object ExcuteCardAuthReset(string sUserID, string[] sCardNoArray)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            IPHostEntry IHEY = Dns.GetHostEntry(Dns.GetHostName());
            string sMyLocalIp = "";

            foreach (IPAddress ipaddress in IHEY.AddressList)
            {
                if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    sMyLocalIp = ipaddress.ToString();
                    break;
                }
            }

            int iResult = -1;
            string sSql = "", sErrorCardNoList = "";

            objRet.act    = "ExcuteCardAuthReset";
            objRet.result = true;

            if (sCardNoArray.Length > 0)
            {
                for (int i = 0; i < sCardNoArray.Length; i++)
                {
                    iResult = -1;
                    List<string> liPara = new List<string>();                    
                    sSql = "CardAuth_Update";
                    liPara.Clear();
                    liPara.Add("S:" + sCardNoArray[i]);
                    liPara.Add("S:" + sUserID);
                    liPara.Add("S:CopyCardAuth:ExcuteCardAuthCopy");
                    liPara.Add("S:" + sMyLocalIp);
                    liPara.Add("S:卡片權限重整");
                    
                    iResult = oAcsDB.SqlProcedureExecute(sSql, liPara);

                    if (iResult < 0)
                    {
                        // 如果失敗，再做一次
                        iResult = oAcsDB.SqlProcedureExecute(sSql, liPara);

                        if (iResult < 0)
                        {
                            if (sErrorCardNoList != "")
                                sErrorCardNoList += ",";

                            sErrorCardNoList += sCardNoArray[i];
                        }
                    }

                    if (iResult != -1)
                    {
                        oAcsDB.WriteLog(DB_Acs.Logtype.卡片權限調整, strUserId, strUserName, "0107", "", "", string.Format("卡片「{0}」卡片權限重整成功", sCardNoArray[i]), "卡片權限重整");
                    }
                }
            }

            objRet.message = sErrorCardNoList;

            return objRet;
        }
        #endregion

        private void UpdateCardType()
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string strSQL = "UPDATE B01_Card Set CardType='R' Where CardType='T' ";
            oAcsDB.SqlCommandExecute(strSQL);
        }

        private void UpdateTempCardVer()
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string strSQL = @"UPDATE B01_Card Set CardVer='', CardSerialNo='' 
                Where CardType='R' AND (CardVer <>'' OR CardSerialNo <> '') ;
                UPDATE B01_CardAuth SET CardVer='' WHERE CardNo IN 
                ( SELECT CardNo FROM B01_Card WHERE CardType='R' AND CardVer <>'') ";

            oAcsDB.SqlCommandExecute(strSQL);
        }
    }
}