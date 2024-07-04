using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sa.DB;
using DapperDataObjectLib;

namespace SahoAcs
{
    public partial class TempCardCreateWithArea : Page
    {
        #region 一.宣告
        private int _pagesize = 10;             //設定GridView控制項每頁可顯示的資料列數
        private static string UserID = "";      //儲存目前使用者的UserID
        private static string UserName = "";    //儲存目前使用者的UserName
        public static string AreaListStr = "SELECT * FROM B00_ItemList WHERE ItemClass='AreaType' ORDER BY ItemOrder ";
        private DataTable tmpDT = new DataTable();
        private OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        //Hashtable TableInfo;
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;  //宣告Ajax元件
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
            this.MainGridView.PageSize = _pagesize;

            UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            UserName = Sa.Web.Fun.GetSessionStr(this.Page, "UserName");

            if (Session["tmpDatatable"] == null) Session["tmpDatatable"] = new DataTable();

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                ViewState["query_CardNo"] = "";
                ViewState["SortExpression"] = "CardNo";
                ViewState["SortDire"] = "ASC";

                Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg    = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == this.QueryButton.ClientID)
                {
                    ViewState["query_CardNo"] = this.QueryInput_CardNo.Text.Trim();
                }

                if (!string.IsNullOrEmpty(sFormArg))
                {
                    if (sFormArg == "popPagePost")                 //進行因應新增或編輯後所需的換頁動作
                    {
                        tmpDT.Clear();
                        int find = Query("popPagePost", ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());

                        if (find > 0) find = find - 1;
                        MainGridView.PageIndex = find;
                        GirdViewDataBind(MainGridView, Session["tmpDatatable"] as DataTable);
                        Sa.Web.Fun.RunJavaScript(this, "GridSelect();");
                    }
                }
                else
                {
                    Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    Sa.Web.Fun.RunJavaScript(this, "GridSelect();");
                }

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI", "$.unblockUI();", true);
            }

            var result = this.odo.GetQueryResult(AreaListStr);
            this.ddlCardArea.Items.Add(new ListItem()
            {
                Text = "無",
                Value = ""
            });
            foreach (var o in result)
            {
                this.ddlCardArea.Items.Add(new ListItem()
                {
                    Text = Convert.ToString(o.ItemName),
                    Value = Convert.ToString(o.ItemNo)
                });
            }

        }//end page_load
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
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";
            js  = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("TempCardRecord", "TempCardCreateWithArea.js");//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>
        /// 註冊網頁的視窗與按鈕動作
        /// </summary>
        private void RegisterWindowsAndButton()
        {
            //註冊主作業畫面的按鈕動作
            QueryButton.Attributes["onClick"]  = "Block(); SelectState();  return false;";
            AddButton.Attributes["onClick"] = "CallAdd('" +
                this.GetLocalResourceObject("CallAdd_Title") + "'); return false;";
            EditButton.Attributes["onClick"] = "CallEdit('" +
                this.GetLocalResourceObject("CallEdit_Title") + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";
            DeleteButton.Attributes["onClick"] = "CallDelete('" +
                this.GetLocalResourceObject("CallDelete_Title") + "', '" +
                this.GetLocalResourceObject("CallDelete_DelLabel") + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForDelete") + "'); return false;";

            //註冊次作業畫面的按鈕動作
            ImgCloseButton1.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            popB_Add.Attributes["onClick"]    = "Block(); AddExcute(); return false;";
            popB_Edit.Attributes["onClick"]   = "Block(); EditExcute(); return false;";
            popB_Delete.Attributes["onClick"] = "Block(); DeleteExcute(); return false;";
            popB_Cancel.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";

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
                    e.Row.Cells[0].Width = 100;     //卡片號碼
                    e.Row.Cells[1].Width = 120;     //使用者
                    e.Row.Cells[2].Width = 80;      //卡片權限
                    e.Row.Cells[3].Width = 80;
                    //e.Row.Cells[3].Width = 300;  //卡片描述(此部份不用設定)

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
                    GrRow.ID = "GV_Row" + oRow.Row["CardNo"].ToString().Trim();
                    #endregion

                    #region B-2.設定欄位隱藏與寬度
                    e.Row.Cells[0].Width = 104;  //卡片號碼
                    e.Row.Cells[1].Width = 124;  //使用者
                    e.Row.Cells[2].Width = 84;   //卡片權限
                    e.Row.Cells[3].Width = 84;  //卡片廠區
                    //e.Row.Cells[3].Width = 304;  //卡片描述(此部份不用設定)

                    #endregion

                    #region B-3.處理欄位資料的格式
                    //0.卡片號碼
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;

                    //1.使用者
                    e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Center;

                    //2.卡片權限
                    e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Center;

                    //3.卡片描述
                    e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Left;

                    #endregion

                    #region B-4.限制欄位資料的長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }

                    int intLastCount = e.Row.Cells.Count - 1;
                    e.Row.Cells[intLastCount].Text = LimitText(e.Row.Cells[intLastCount].Text, 50, true);  //卡片描述
                    #endregion

                    #region B-5.設定表格事件的方法
                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, hSelectValue,'" + oRow.Row["CardNo"].ToString().Trim() + "', '', '')");
                    e.Row.Attributes.Add("OnDblclick", "CallEdit('" + "編輯臨時卡建檔作業資料" + "')");
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
            DataTable dt = new DataTable();
            List<string> liSqlPara = new List<string>();

            string sql = "";

            #region Process String - B01_Card

            //設定臨時卡查詢條件 C.CardType = 'R'
            sql = @"
                SELECT 
                    C.CardNo, 
                    C.CardDesc, I.ItemName AS Rev02,
                    CASE 
                        WHEN C.CardAuthAllow = 1 THEN '有效'
                        WHEN C.CardAuthAllow = 0 THEN '無效'
                    END AS CardAuthAllow, 
                    (CONVERT(VARCHAR, C.PsnID) + ' / ' + P.PsnName) AS PersonData     
                FROM B01_Card C 
                LEFT JOIN B01_Person P ON P.PsnID = C.PsnID  
                LEFT JOIN B00_ItemList I ON C.Rev02=I.ItemNo AND I.ItemClass='AreaType'
                WHERE C.CardType = 'R' ";

            //設定查詢條件
            if (!string.IsNullOrEmpty(ViewState["query_CardNo"].ToString().Trim()))
            {
                sql += " AND C.CardNo LIKE ? ";
                liSqlPara.Add("A:" + "%" + ViewState["query_CardNo"].ToString().Trim() + "%");
            }

            sql += " ORDER BY " + SortExpression + " " + SortDire;

            //取得查詢資料
            oAcsDB.GetDataTable("CardTable", sql, liSqlPara, out dt);
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

            string sql = "";

            #region Process String - Get B01_Card
            sql = @"
                SELECT 
                    C.CardID, 
                    C.CardNo, 
                    C.CardDesc, I.ItemName AS Rev02,
                    CASE 
                        WHEN C.CardAuthAllow = 1 THEN '有效'
                        WHEN C.CardAuthAllow = 0 THEN '無效'
                    END AS CardAuthAllow, 
                    (CONVERT(VARCHAR, C.PsnID) + ' / ' + P.PsnName) AS PersonData  
                FROM B01_Card C 
                LEFT JOIN B00_ItemList L ON L.ItemClass = 'CardType' AND L.ItemNo = C.CardType  
                LEFT JOIN B01_Person P ON P.PsnID = C.PsnID 
                LEFT JOIN B00_ItemList I ON C.Rev02=I.ItemNo AND I.ItemClass='AreaType'
                WHERE C.CardID IS NOT NULL AND C.CardType = 'R' ";

            if (!string.IsNullOrEmpty(ViewState["query_CardNo"].ToString().Trim()))
            {
                sql += " AND C.CardNo LIKE ? ";
                liSqlPara.Add("S:" + "%" + ViewState["query_CardNo"].ToString().Trim() + "%");
            }

            //設定查詢排序
            sql += " ORDER BY " + SortExpression + " " + SortDire;

            //取得查詢資料
            oAcsDB.GetDataTable("CardTable", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            Session["tmpDatatable"] = dt;
            #endregion

            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();

            //完成查詢後並取得資料目前的表格頁數
            int find = 0;
            DataRow dr = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dr = dt.Rows[i];

                if (hSelectValue.Value == dr["CardNo"].ToString().Trim())
                {
                    find = i;
                    break;
                }
            }

            return (find / _pagesize) + 1;
        }
        #endregion

        #region 五.載入、新增、編輯、刪除
        /// <summary>
        /// 載入臨時卡建檔作業視窗相關的欄位資料
        /// </summary>
        /// <param name="sCardID">卡片識別碼</param>
        /// <returns>string[] EditData</returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadData(string sCardNo)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            List<string> liSqlPara = new List<string>();
            DBReader dr = new DBReader();

            bool IsOk = true;
            string sSql = "";
            string[] EditData = null;

            if (sCardNo.Trim() == "")
            {
                EditData = new string[2];

                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "傳入LoadData方法的卡片識別碼為空字串！";

                return EditData;
            }

            #region 取得載入畫面相關的欄位資料
            sSql = @"SELECT CardNo, CardDesc, Rev02 FROM B01_Card WHERE CardNo = ?";

            liSqlPara.Clear();
            liSqlPara.Add("A:" + sCardNo.Trim());

            dr = new DBReader();
            IsOk = oAcsDB.GetDataReader(sSql, liSqlPara, out dr);

            if (IsOk)
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        EditData = new string[dr.DataReader.FieldCount];
                        for (int i = 0; i < dr.DataReader.FieldCount; i++)
                            EditData[i] = dr.DataReader[i].ToString();
                    }
                }
                else
                {
                    EditData = new string[2];
                    EditData[0] = "Saho_SysErrorMassage";
                    EditData[1] = "系統中無此資料！";
                }
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "讀取資料失敗！";
            }

            #endregion

            return EditData;
        }

        /// <summary>
        /// 新增臨時卡建檔作業視窗相關的欄位資料
        /// </summary>
        /// <param name="sCardNo">卡片號碼</param>
        /// <param name="sCardDesc">卡片描述</param>
        /// <returns>Pub.MessageObject</returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Insert(string sCardNo, string sCardDesc, string sRev02)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            List<string> liSqlPara = new List<string>();
            DataTable dt  = new DataTable();
            DateTime Time = DateTime.Now;

            int iResult = -1;
            string sSql = "";

            objRet.act   = "Add";
            objRet.result = true;

            #region 輸入條件判斷

            #region 是否輸入卡片號碼
            if (sCardNo.Trim() == "")
            {
                objRet.result = false;
                objRet.message = "清輸入卡片號碼！";

                return objRet;
            }
            #endregion

            #region 判斷新增的卡片號碼是否已存在

            sSql = " SELECT CardNo FROM B01_Card WHERE CardNo = ? ";

            liSqlPara.Clear();
            liSqlPara.Add("A:" + sCardNo.Trim());

            dt = new DataTable();
            oAcsDB.GetDataTable("CardNoCountTable", sSql, liSqlPara, out dt);

            if ((dt != null) && (dt.Rows.Count > 0))
            {
                objRet.result = false;
                objRet.message = "資料庫已經有該卡片號碼：" + sCardNo.Trim() + "！";

                return objRet;
            }
            #endregion

            #region 判斷卡片號碼長度是否跟 B00_SysParameter 裡面的設定一致
            // wei 20170207
            sSql = " SELECT TOP 1 [ParaValue] FROM [B00_SysParameter] WHERE ParaClass = 'CardID' AND ParaNo = 'CardLen' ";

            string strCardLen = oAcsDB.GetStrScalar(sSql);
            int intCardNoLen = Encoding.Default.GetBytes(sCardNo.Trim()).Length;

            if (strCardLen != null && strCardLen != "")
            {
                if (Int32.Parse(strCardLen) != intCardNoLen)
                {
                    objRet.result = false;
                    objRet.message = string.Format("卡片長度需為 {0} 碼", strCardLen);
                }
            }
            else
            {
                if (sCardNo.Trim().ToString().Length != 10)
                {
                    objRet.result = false;
                    objRet.message = string.Format("卡片長度需為 {0} 碼", "10");
                }
            }

            if (!objRet.result) return objRet;

            #endregion

            #region 判斷卡號是否為數字
            int i = 0;
            //bool bolRlt = int.TryParse(sCardNo.Trim(), out i);

            bool bolRlt = Sa.Check.IsHex(sCardNo.Trim());

            if (!bolRlt)
            {
                objRet.result = false;
                objRet.message = "卡片號碼必須是A~F的英文字母、0~9阿拉伯數字！";

                return objRet;
            }
            #endregion

            #endregion

            #region 新增欄位資料至資料庫
            sSql = @"
                INSERT INTO B01_Card 
                (
                    CardNo, CardVer, CardSerialNo, CardType, 
                    CardAuthAllow, CardSTime, CardETime, CardDesc, Rev02,
                    CreateUserID, CreateTime 
                ) 
                VALUES 
                (?,?,?,?,?,?,?,?,?,?,?)";
            liSqlPara = new List<string>();
            string strTime = Time.ToString("yyyy-MM-dd HH:mm:ss.fff");

            liSqlPara.Add("A:" + sCardNo.Trim());
            liSqlPara.Add("A:");
            liSqlPara.Add("A:");
            liSqlPara.Add("A:R");
            liSqlPara.Add("I:0");
            liSqlPara.Add("D:" + strTime);
            liSqlPara.Add("D:2099-12-31 23:59:59.999");
            liSqlPara.Add("S:" + sCardDesc.Trim());
            liSqlPara.Add("S:" + sRev02);
            liSqlPara.Add("A" + UserID.Trim());
            liSqlPara.Add("D:" + strTime);

            iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);

            if (iResult > 0)
            {
                objRet.result = true;
                objRet.message = sCardNo.Trim();

                oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, UserID, UserName, "0106", "", "", string.Format("新增卡片號碼：[{0}] 的資料成功", sCardNo.Trim()), "B01_Card");
            }
            else
            {
                objRet.result = false;
                objRet.message = "新增臨時卡作業失敗！";
            }
            #endregion

            return objRet;
        }

        /// <summary>
        /// 更新臨時卡建檔作業視窗相關的欄位資料
        /// </summary>
        /// <param name="SelectValue">選擇列的卡片識別碼</param>
        /// <param name="sCardDesc">卡片描述</param>
        /// <returns>Pub.MessageObject</returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Update(string SelectValue, string sCardDesc, string Rev02)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();

            DateTime Time = DateTime.Now;

            int iResult = -1;
            string sSql = "";

            objRet.act    = "Edit";
            objRet.result = true;

            #region 輸入條件判斷
            if (SelectValue.Trim() == "")
            {
                objRet.result = false;
                objRet.message = "更新臨時卡片的資料失敗，因為沒有卡片識別碼！";

                return objRet;
            }
            #endregion

            #region 取出舊資料

            sSql = @"
                SELECT CardNo,CardDesc 
                FROM B01_Card WHERE CardNo = ? ";
            List<string> liSqlPara = new List<string>();
            liSqlPara.Add("S:" + SelectValue.Trim());

            DBReader dr = new DBReader();
            oAcsDB.GetDataReader(sSql, liSqlPara, out dr);

            string strCardNo = "";
            string strCardDesc = "";

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    strCardNo = dr.ToString("CardNo");
                    strCardDesc = dr.ToString("CardDesc");
                }
            }

            #endregion

            #region 更新資料至資料庫
            sSql = @" 
                UPDATE B01_Card SET 
                    CardDesc = ?,Rev02 = ?,
                    UpdateUserID = ?,
                    UpdateTime = ? 
                WHERE CardNo = ? ";

            liSqlPara = new List<string>();
            liSqlPara.Add("S:" + sCardDesc.Trim());
            liSqlPara.Add("S:" + Rev02);
            liSqlPara.Add("A:" + UserID.Trim());
            liSqlPara.Add("D:" + Time.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            liSqlPara.Add("A:" + SelectValue.Trim());

            iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
            
            if (iResult > 0)
            {
                objRet.result = true;
                objRet.message = "";

                #region 比對資料，產生LOG，回存syslog

                string strLog = string.Format("更新卡片號碼：[{0}] 的資料成功", strCardNo);

                if (strCardDesc != sCardDesc.Trim())
                {
                    strLog += string.Format("，CardDesc：[{0}]->[{1}]", strCardDesc, sCardDesc.Trim());
                }

                oAcsDB.WriteLog(DB_Acs.Logtype.資料修改, UserID, UserName, "0106", "", "", strLog, "B01_Card");
                #endregion
            }
            else
            {
                objRet.result = false;
                objRet.message = "更新臨時卡片識別碼：" + SelectValue.Trim() + "的資料失敗！";
            }
            #endregion

            return objRet;
        }

        /// <summary>
        /// 刪除臨時卡建檔作業視窗相關的欄位資料
        /// </summary>
        /// <param name="SelectValue">選擇列的卡片識別碼</param>
        /// <returns>Pub.MessageObject</returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Delete(string SelectValue, string sCardNo)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            DataTable dt = new DataTable();
            List<string> liSqlPara = new List<string>();

            int iResult = -1;
            bool IsOK = true;
            string sSql = "";

            objRet.act    = "Delete";
            objRet.result = true;

            #region 輸入條件判斷
            if (sCardNo.Trim() == "")
            {
                objRet.result = false;
                objRet.message = "傳入Delete方法的卡片號碼為空字串！";
            }
            else if (SelectValue.Trim() == "")
            {
                objRet.result = false;
                objRet.message = "傳入Delete方法的卡片識別碼為空字串！";
            }

            if (!objRet.result) return objRet;
            #endregion

            #region 判斷要刪除的臨時卡號是否被借出

            sSql = @" 
                SELECT 
                    RecordID, PsnNo, OrigCardNo, CardNo, BorrowTime, ReturnTime, TempDesc 
                FROM B01_TempCardRecord 
                WHERE BorrowTime IS NOT NULL AND ReturnTime IS NULL 
                AND CardNo = ? ";

            liSqlPara.Clear();
            liSqlPara.Add("S:" + sCardNo.Trim());

            dt = new DataTable();
            IsOK = oAcsDB.GetDataTable("TemopCardRecordTable", sSql, liSqlPara, out dt);
           
            if (IsOK)
            {
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    objRet.result = false;
                    objRet.message = "臨時卡號：" + sCardNo.Trim() + "被借出尚未歸還不能被刪除！";
                }
            }
            else
            {
                objRet.result = false;
                objRet.message = "讀取臨時卡號：" + sCardNo.Trim() + "失敗！";
            }

            if (!objRet.result) return objRet;

            #endregion

            #region 刪除指定的資料 

            sSql = " DELETE FROM B01_Card WHERE CardNo = ? ";

            liSqlPara.Clear();
            liSqlPara.Add("S:" + SelectValue.Trim());
            iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);

            if (iResult != -1)
            {
                objRet.result  = true;
                objRet.message = "";

                oAcsDB.WriteLog(DB_Acs.Logtype.資料刪除, UserID, UserName, "0106", "", "", string.Format("刪除卡片號碼：[{0}] 的資料成功", sCardNo.Trim()), "B01_Card");
            }
            else
            {
                objRet.result  = false;
                objRet.message = string.Format("刪除卡片號碼：{0}失敗！", sCardNo.Trim());
            }

            #endregion

            return objRet;
        }
        #endregion
    }
}