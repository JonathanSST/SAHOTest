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
using Sa.DB;

namespace SahoAcs
{
    public partial class ClassData : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;  //宣告Ajax元件
        private int _pagesize = 100;                             //DataGridView每頁顯示的列數
        private static string MsgDuplicate = "";
        #endregion

        #region 網頁前置作業
        #region LoadProcess
        private void LoadProcess()
        {
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            ////oScriptManager.RegisterAsyncPostBackControl(QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("ClassData", "ClassData.js");  //加入同一頁面所需的JavaScript檔案
            ////ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "YYData();", true);
        }
        #endregion

        #region RegisterObj
        private void RegisterObj()
        {
            #region 設定彈跳視窗
            Pub.SetModalPopup(ModalPopupExtender1, 1);
            Pub.SetModalPopup(ModalPopupExtender2, 2);
            #endregion

            #region 註冊主頁Button動作
            AddButton.Attributes["onClick"] = "CallAdd('" + this.GetLocalResourceObject("CallAdd_Title") + "'); return false;";
            EditButton.Attributes["onClick"] = "CallEdit('" + this.GetLocalResourceObject("CallEdit_Title") + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";
            DeleteButton.Attributes["onClick"] = "CallDelete('" + this.GetLocalResourceObject("CallDelete_Title") + "', '" +
                this.GetLocalResourceObject("CallDelete_DelLabel") + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForDelete") + "'); return false;";
            #endregion

            #region 註冊pop1頁Button動作
            ImgCloseButton1.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            popB_Add.Attributes["onClick"] = "AddExcute('" + hUserId.Value + "'); return false;";
            popB_Edit.Attributes["onClick"] = "EditExcute('" + hUserId.Value + "'); return false;";
            popB_Cancel.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            #endregion

            #region 註冊pop2頁Button動作
            ImgCloseButton2.Attributes["onClick"] = "SetMode(''); CancelTrigger2.click(); return false;";
            popB_Delete.Attributes["onClick"] = "DeleteExcute('" + hUserId.Value + "'); return false;";
            popB_Cancel2.Attributes["onClick"] = "SetMode(''); CancelTrigger2.click(); return false;";
            #endregion

            //設定DataGridView每頁的顯示列數
            this.MainGridView.PageSize = _pagesize;
        }
        #endregion

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            MsgDuplicate = this.GetLocalResourceObject("MsgDouble").ToString();

            try
            {
                hUserId.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                //hMenuNo.Value = Sa.Web.Fun.GetSessionStr(this.Page, "MenuNo");
                //hMenuName.Value = Sa.Web.Fun.GetSessionStr(this.Page, "MenuName");

                LoadProcess();
                RegisterObj();

                if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
                {
                    ViewState["SortExpression"] = "CNo";
                    ViewState["SortDire"] = "ASC";
                    hSelectState.Value = "true";
                    Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                }
                else
                {
                    string sFormTarget = Request.Form["__EVENTTARGET"];
                    string sFormArg = Request.Form["__EVENTARGUMENT"];

                    if (!string.IsNullOrEmpty(sFormArg))
                    {
                        if (sFormArg == "popPagePost")  //進行因應新增或編輯後所需的換頁動作
                        {
                            //int find = Query("popPagePost", ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                            Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                            Sa.Web.Fun.RunJavaScript(this, "GridSelect();");
                        }
                        else if (sFormArg == "NewQuery")
                        {
                            this.MainGridView.PageIndex = 0;
                            hSelectState.Value = "true";
                            Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        }
                    }
                    else
                    {
                        hSelectState.Value = "false";
                        Query(false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    }
                }
            }
            catch (Exception ee)
            {
                Response.Redirect("~/Web/MessagePage/LoginSorry.aspx");
            }
        }
        #endregion

        #region VerifyRenderingInServerForm
        public override void VerifyRenderingInServerForm(Control control)
        {
            //如有使用UpdatePanel配合GridVew才需要這個方法
            //修正'XX'型別必須置於有runat=server的表單標記之中Override此Methods
        }
        #endregion
        #endregion

        #region 其他方法
        #region 記載查詢條件的紀錄，防止頁數按鈕切換時查詢錯誤
        private void CatchSession(List<String> Data)
        {
            String datalist = "";

            for (int i = 0; i < Data.Count; i++)
                datalist += Data[i] + "|";

            Session["OldSearchList"] = datalist;
        }
        #endregion

        #region LimitText
        public string LimitText(string str, int len, bool ellipsis)
        {
            Encoding oEnBig5 = Encoding.GetEncoding("big5");
            byte[] byteAY = oEnBig5.GetBytes(str);

            if (byteAY.Length <= len)
                return str;
            else
            {
                if (ellipsis) len -= 3;

                string res = oEnBig5.GetString(byteAY, 0, len);
                if (!oEnBig5.GetString(byteAY).StartsWith(res))
                    res = oEnBig5.GetString(byteAY, 0, len - 1);

                return res + (ellipsis ? "..." : "");
            }
        }
        #endregion
        #endregion

        #region 查詢
        private void Query(bool select_state, string SortExpression, string SortDire)
        {
            String sYear = SelectYear.Value;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt = new DataTable();
            List<String> CheckData = new List<String>();
            List<string> liSqlPara = new List<string>();
            String NowCondition = "", NowYear = "", wheresql = "";

            if (select_state)
            {
                //CheckData.Add(this.Input_Condition.Text.Trim());
                CheckData.Add(sYear);
                CatchSession(CheckData);
                //NowCondition = this.Input_Condition.Text.Trim();
                NowYear = sYear;
            }
            else
            {
                //if (Session["OldSearchList"].ToString() != "" && Session["OldSearchList"].ToString() != null)
                //{
                //    String[] mgalist = Session["OldSearchList"].ToString().Split('|');
                //    NowCondition = mgalist[0];
                //    NowYear = mgalist[1];
                //}
                NowYear = sYear;
            }

            #region Process String
            sql = " SELECT * FROM B00_ClassData ";

            /*
            //設定查詢條件
            if (!string.IsNullOrEmpty(NowCondition))
            {
                string[] strCondiition = new string[] { "\t", " ", "　", ",", "，", "、", "。", "\n" };
                string[] strArray = NowCondition.Split(strCondiition, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i <= strArray.Length - 1; i++)
                {
                    if (strArray[i].Trim() != "")
                    {
                        wheresql += " AND (OrgNo + '__' + ISNULL(OrgName,'')) LIKE ? ";
                        liSqlPara.Add("S:" + "%" + strArray[i].Trim() + "%");
                    }
                }
            }
            */

            /*
            if (NowYear != "")
            {
                wheresql += " WHERE HEDate >= ? AND HEDate <= ? ";
                liSqlPara.Add("S:" + NowYear + "-01-01");
                liSqlPara.Add("S:" + NowYear + "-12-31");
            }
            else
            {
                wheresql += " WHERE HEDate >= ? AND HEDate <= ? ";
                liSqlPara.Add("S:" + "1911-01-01");
                liSqlPara.Add("S:" + "1911-12-31");
            }
            */

            sql += wheresql + " ORDER BY CNo ";
            #endregion

            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            GirdViewDataBind(this.MainGridView, dt);

            UpdatePanel1.Update();
            hSelectState.Value = "false";
        }

        protected static Pub.MessageObject Check_Input_DB(string[] sNoAY, string sMode)
        {
            string sSql = "";
            DB_Acs oAcsDB = null;
            DBReader oDbRdr = null;
            List<string> sSqlParaLT = new List<string>();
            Pub.MessageObject sRetMsg = new Pub.MessageObject();

            #region Input
            if (sMode == "Insert")
            {
                #region 檢查新增欄位
                if (string.IsNullOrEmpty(sNoAY[0].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRetMsg.message)) { sRetMsg.message += "\n"; }
                    sRetMsg.result = false;
                    sRetMsg.message += "班別代號 必須輸入";
                }

                if (sNoAY[0].Contains(" "))
                {
                    sRetMsg.result = false;
                    sRetMsg.message += "班別代號字串中不可有空白符號";
                }

                if (string.IsNullOrEmpty(sNoAY[1].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRetMsg.message)) { sRetMsg.message += "\n"; }
                    sRetMsg.result = false;
                    sRetMsg.message += "班別名稱 必須輸入";
                }

                if (string.IsNullOrEmpty(sNoAY[2].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRetMsg.message)) { sRetMsg.message += "\n"; }
                    sRetMsg.result = false;
                    sRetMsg.message += "上班時間 必須輸入";
                }

                if (string.IsNullOrEmpty(sNoAY[3].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRetMsg.message)) { sRetMsg.message += "\n"; }
                    sRetMsg.result = false;
                    sRetMsg.message += "下班時間 必須輸入";
                }

                if (string.IsNullOrEmpty(sNoAY[4].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRetMsg.message)) { sRetMsg.message += "\n"; }
                    sRetMsg.result = false;
                    sRetMsg.message += "彈性時間 必須輸入";
                }

                if (string.IsNullOrEmpty(sNoAY[5].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRetMsg.message)) { sRetMsg.message += "\n"; }
                    sRetMsg.result = false;
                    sRetMsg.message += "自動轉換加班 必須輸入";
                }

                if (string.IsNullOrEmpty(sNoAY[6].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRetMsg.message)) { sRetMsg.message += "\n"; }
                    sRetMsg.result = false;
                    sRetMsg.message += "工作時間 必須輸入";
                }

                if (string.IsNullOrEmpty(sNoAY[7].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRetMsg.message)) { sRetMsg.message += "\n"; }
                    sRetMsg.result = false;
                    sRetMsg.message += "彈性時間型別 必須輸入";
                }
                #endregion
            }
            else if (sMode == "Update")
            {
                #region 檢查更新欄位
                if (string.IsNullOrEmpty(sNoAY[0].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRetMsg.message)) { sRetMsg.message += "\n"; }
                    sRetMsg.result = false;
                    sRetMsg.message += "班別代號 必須輸入";
                }

                if (sNoAY[0].Contains(" "))
                {
                    sRetMsg.result = false;
                    sRetMsg.message += "班別代號字串中不可有空白符號";
                }

                if (string.IsNullOrEmpty(sNoAY[1].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRetMsg.message)) { sRetMsg.message += "\n"; }
                    sRetMsg.result = false;
                    sRetMsg.message += "班別名稱 必須輸入";
                }

                if (string.IsNullOrEmpty(sNoAY[2].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRetMsg.message)) { sRetMsg.message += "\n"; }
                    sRetMsg.result = false;
                    sRetMsg.message += "上班時間 必須輸入";
                }

                if (string.IsNullOrEmpty(sNoAY[3].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRetMsg.message)) { sRetMsg.message += "\n"; }
                    sRetMsg.result = false;
                    sRetMsg.message += "下班時間 必須輸入";
                }

                if (string.IsNullOrEmpty(sNoAY[4].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRetMsg.message)) { sRetMsg.message += "\n"; }
                    sRetMsg.result = false;
                    sRetMsg.message += "彈性時間 必須輸入";
                }

                if (string.IsNullOrEmpty(sNoAY[5].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRetMsg.message)) { sRetMsg.message += "\n"; }
                    sRetMsg.result = false;
                    sRetMsg.message += "自動轉換加班 必須輸入";
                }

                if (string.IsNullOrEmpty(sNoAY[6].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRetMsg.message)) { sRetMsg.message += "\n"; }
                    sRetMsg.result = false;
                    sRetMsg.message += "工作時間 必須輸入";
                }

                if (string.IsNullOrEmpty(sNoAY[7].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRetMsg.message)) { sRetMsg.message += "\n"; }
                    sRetMsg.result = false;
                    sRetMsg.message += "彈性時間型別 必須輸入";
                }
                #endregion
            }
            #endregion

            #region DB
            switch (sMode)
            {
                case "Insert":
                    sSql = @" SELECT * FROM B00_ClassData WHERE CNo = ?; ";
                    sSqlParaLT.Add("S:" + sNoAY[0].Trim());
                    break;
                case "Update":
                    sSql = @" SELECT * FROM B00_ClassData WHERE CNo = ? AND CID <> ?; ";
                    sSqlParaLT.Add("S:" + sNoAY[0].Trim());
                    sSqlParaLT.Add("S:" + sNoAY[8]);
                    break;
            }

            oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            oAcsDB.GetDataReader(sSql, sSqlParaLT, out oDbRdr);

            if (oDbRdr.Read())
            {
                if (!string.IsNullOrEmpty(sRetMsg.message)) { sRetMsg.message += "\n"; }
                sRetMsg.result = false;
                sRetMsg.message += MsgDuplicate;
            }
            #endregion

            return sRetMsg;
        }
        #endregion

        #region 處理GridView
        protected void GridView_Data_DataBound(object sender, EventArgs e)
        {
            td_showGridView.Attributes["colspan"] = MainGridView.Columns.Count.ToString();
        }

        protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            MainGridView.PageIndex = e.NewPageIndex;
            if (MainGridView.Rows[0].Cells.Count == MainGridView.HeaderRow.Cells.Count) { MainGridView.DataBind(); }
        }

        protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            string SortField = e.SortExpression, SortDire = "DESC";

            if (ViewState["SortExpression"] != null)
            {
                if (ViewState["SortExpression"].ToString().Equals(SortField))
                {
                    if (!ViewState["SortDire"].Equals("ASC")) { SortDire = "ASC"; } else { SortDire = "DESC"; }
                }
            }

            hSelectState.Value = "true";
            ViewState["SortDire"] = SortDire;
            ViewState["SortExpression"] = SortField;
            Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }

        protected void GridView_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:
                    #region 設定欄位寛度
                    e.Row.Cells[0].Width  = 50;
                    e.Row.Cells[1].Width  = 50;
                    e.Row.Cells[2].Width  = 110;
                    e.Row.Cells[3].Width  = 50;
                    e.Row.Cells[4].Width  = 50;
                    e.Row.Cells[5].Width  = 50;
                    //e.Row.Cells[5].Width  = 60;
                    e.Row.Cells[6].Width  = 50;
                    //e.Row.Cells[7].Width  = 60;
                    e.Row.Cells[7].Width  = 110;
                    e.Row.Cells[8].Width = 180;
                    e.Row.Cells[9].Width = 110;
                    //e.Row.Cells[10].Width = 190;
                    #endregion

                    #region 設定表頭欄位Visible狀態
                    //不想顯示的欄位，可在此處關掉
                    e.Row.Cells[0].Visible = false;
                    #endregion

                    #region 排序條件Header加工
                    //foreach (DataControlField dataControlField in MainGridView.Columns)
                    //{
                    //    if (dataControlField.SortExpression.Equals(this.SortExpression))
                    //    {
                    //        Label label = new Label();
                    //        label.Text = this.SortDire.Equals("ASC") ? "▲" : "▼";
                    //        e.Row.Cells[MainGridView.Columns.IndexOf(dataControlField)].Controls.Add(label);
                    //    }
                    //}
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
                    GrRow.ID = "GV_Row" + oRow.Row["CID"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    e.Row.Cells[0].Width  = 50;
                    e.Row.Cells[1].Width  = 54;
                    e.Row.Cells[2].Width  = 114;
                    e.Row.Cells[3].Width  = 54;
                    e.Row.Cells[4].Width  = 54;
                    e.Row.Cells[5].Width  = 54;
                    //e.Row.Cells[5].Width  = 64;
                    e.Row.Cells[6].Width = 54;
                    //e.Row.Cells[7].Width  = 63;
                    e.Row.Cells[7].Width  = 114;
                    e.Row.Cells[8].Width = 184;
                    e.Row.Cells[9].Width = 114;
                    //e.Row.Cells[10].Width = 165;
                    #endregion

                    #region 設定欄位排版
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Left;
                    e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[5].HorizontalAlign = HorizontalAlign.Center;
                    //e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Center;
                    //e.Row.Cells[7].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[7].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[8].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[9].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[10].HorizontalAlign = HorizontalAlign.Center;
                    #endregion

                    #region 針對各欄位做所需處理
                    #region 組織ID
                    e.Row.Cells[0].Visible = false;
                    #endregion
                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }

                    e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text, 45, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["CID"].ToString() + "', '', '');");
                    e.Row.Attributes.Add("OnDblclick", "CallEdit('" + this.GetLocalResourceObject("CallEdit_Title") + "', '" +
                        this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "')");
                    break;
                #endregion

                #region Pager
                case DataControlRowType.Pager:
                    #region 取得控制項
                    GridView gv = sender as GridView;
                    PlaceHolder phdPageNumber = e.Row.FindControl("phdPageNumber") as PlaceHolder;
                    LinkButton lbtnFirst = e.Row.FindControl("lbtnFirst") as LinkButton;
                    LinkButton lbtnLast = e.Row.FindControl("lbtnLast") as LinkButton;
                    LinkButton lbtnPrev = e.Row.FindControl("lbtnPrev") as LinkButton;
                    LinkButton lbtnNext = e.Row.FindControl("lbtnNext") as LinkButton;
                    LinkButton lbtnPage;
                    #endregion

                    #region 決定顯示頁數及上下頁處理
                    int showRange = 5; //顯示快捷頁數
                    int pageCount = gv.PageCount;
                    int pageIndex = gv.PageIndex;
                    int startIndex = (pageIndex + 1 < showRange) ?
                        0 : (pageIndex + 1 + showRange / 2 >= pageCount) ? pageCount - showRange : pageIndex - showRange / 2;
                    int endIndex = (startIndex >= pageCount - showRange) ? pageCount : startIndex + showRange;

                    #region 頁數列
                    phdPageNumber.Controls.Add(new LiteralControl("  "));
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
                            lbtnPage.OnClientClick = "return false;";
                        }
                        else
                            lbtnPage.Font.Bold = false;

                        phdPageNumber.Controls.Add(lbtnPage);
                        phdPageNumber.Controls.Add(new LiteralControl(" "));
                    }
                    #endregion

                    #region 上下頁
                    lbtnPrev.Click += delegate (object obj, EventArgs args)
                    {
                        if (gv.PageIndex > 0)
                        {
                            gv.PageIndex = gv.PageIndex - 1;

                            if (hSelectState.Value == "true")
                                Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                            else
                                Query(false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        }
                    };

                    lbtnNext.Click += delegate (object obj, EventArgs args)
                    {
                        if (gv.PageIndex < gv.PageCount)
                        {
                            gv.PageIndex = gv.PageIndex + 1;
                            if (hSelectState.Value == "true")
                                Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                            else
                                Query(false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        }
                    };
                    #endregion

                    #region 首末頁
                    lbtnFirst.Click += delegate (object obj, EventArgs args)
                    {
                        gv.PageIndex = 0;

                        if (hSelectState.Value == "true")
                            Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        else
                            Query(false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    };

                    lbtnLast.Click += delegate (object obj, EventArgs args)
                    {
                        gv.PageIndex = MainGridView.PageCount;

                        if (hSelectState.Value == "true")
                            Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        else
                            Query(false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    };
                    #endregion
                    #endregion

                    #region 顯示總頁數及目前頁數
                    phdPageNumber.Controls.Add(
                        new LiteralControl(string.Format("　{0} / {1}　", pageIndex + 1, pageCount)));
                    phdPageNumber.Controls.Add(
                        new LiteralControl("  "));
                    #endregion

                    #region 顯示總筆數
                    phdPageNumber.Controls.Add(
                        new LiteralControl(string.Format(this.GetGlobalResourceObject("Resource", "lblGvCounts").ToString(), hDataRowCount.Value)));
                    phdPageNumber.Controls.Add(
                        new LiteralControl("  "));
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

        #region 若無資料GridView將顯示查無資料資訊
        public void GirdViewDataBind(GridView ProcessGridView, DataTable dt)
        {
            if (dt.Rows.Count != 0)  //Gridview含有資料
            {
                ProcessGridView.DataSource = dt;
                ProcessGridView.DataBind();
            }
            else                     //Gridview沒有資料
            {
                dt.Rows.Add(dt.NewRow());
                ProcessGridView.DataSource = dt;
                ProcessGridView.DataBind();

                int columnCount = ProcessGridView.Rows[0].Cells.Count;
                ProcessGridView.Rows[0].Cells.Clear();
                ProcessGridView.Rows[0].Cells.Add(new TableCell());
                ProcessGridView.Rows[0].Cells[0].ColumnSpan = columnCount;
                ProcessGridView.Rows[0].Cells[0].Text = "查無資料";
                ProcessGridView.RowStyle.HorizontalAlign = HorizontalAlign.Center;
            }
        }
        #endregion
        #endregion

        #region aspx及JavaScript共用方法
        #region 載入單筆資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadData(string sCID, string sUserID, string sMode)
        {
            string sSql = "";
            DB_Acs oAcsDB = null;
            DBReader oDbRdr = null;
            string[] sEditDataAY = null;
            List<string> sSqlParaLT = new List<string>();
            Pub.MessageObject oRetMsg = new Pub.MessageObject();

            #region Process String
            oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

            sSql = @" SELECT * FROM B00_ClassData WHERE CID = ? ";
            sSqlParaLT.Add("S:" + sCID.Trim());

            oAcsDB.GetDataReader(sSql, sSqlParaLT, out oDbRdr);

            if (oDbRdr.Read())
            {
                sEditDataAY = new string[oDbRdr.DataReader.FieldCount];
                for (int i = 0; i < oDbRdr.DataReader.FieldCount; i++) { sEditDataAY[i] = oDbRdr.DataReader[i].ToString(); }
            }
            else
            {
                sEditDataAY = new string[2] { "Saho_SysErrorMassage", "系統中無此資料！" };
            }
            #endregion

            return sEditDataAY;
        }
        #endregion

        #region Update 更新資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Update(string sCID, string sCNo, string sCName, string sWBTime, string sWETime, string sBERange, string sIsCOW, string sWTMin, string sRTType, string sUserID)
        {
            string sSql = "";
            DB_Acs oAcsDB = null;
            string[] sNoAY = new string[9];
            List<string> sSqlParaLT = new List<string>();
            Pub.MessageObject sRetMsg = new Pub.MessageObject();

            sNoAY[0] = sCNo;
            sNoAY[1] = sCName;
            sNoAY[2] = sWBTime;
            sNoAY[3] = sWETime;
            sNoAY[4] = sBERange;
            sNoAY[5] = sIsCOW;
            sNoAY[6] = sWTMin;
            sNoAY[7] = sRTType;
            sNoAY[8] = sCID;

            sRetMsg = Check_Input_DB(sNoAY, "Update");
            if (sRetMsg.result)
            {
                oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

                sSql = @" UPDATE B00_ClassData SET CNo = ?, CName = ?, WBTime = ?, WETime = ?, BERange = ?, IsCOW = ?, WTMin = ?, RTType = ?, UpdateUserID = ?, UpdateTime = GETDATE() WHERE CID = ?; ";
                sSqlParaLT.Add("S:" + sCNo.Trim());
                sSqlParaLT.Add("S:" + sCName.Trim());
                sSqlParaLT.Add("S:" + sWBTime.Trim());
                sSqlParaLT.Add("S:" + sWETime.Trim());
                sSqlParaLT.Add("S:" + sBERange.Trim());
                sSqlParaLT.Add("S:" + sIsCOW.Trim());
                sSqlParaLT.Add("S:" + sWTMin.Trim());
                sSqlParaLT.Add("S:" + sRTType.Trim());
                sSqlParaLT.Add("S:" + sUserID);
                sSqlParaLT.Add("S:" + sCID);
                #endregion

                oAcsDB.BeginTransaction();
                if (oAcsDB.SqlCommandExecute(sSql, sSqlParaLT) > -1)
                {
                    oAcsDB.Commit();
                }
                else
                {
                    oAcsDB.Rollback();
                }
            }
            #region Process String


            sRetMsg.act = "Edit";
            return sRetMsg;
        }
        #endregion

        #region Insert 新增資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Insert(string sCNo, string sCName, string sWBTime, string sWETime, string sBERange, string sIsCOW, string sWTMin, string sRTType, string sUserID)
        {
            string sSql = "";
            DB_Acs oAcsDB = null;
            DBReader oDbRdr = null;
            string[] NoAY = new string[8];
            List<string> sSqlParaLT = new List<string>();
            Pub.MessageObject oRetMsg = new Pub.MessageObject();

            NoAY[0] = sCNo;
            NoAY[1] = sCName;
            NoAY[2] = sWBTime;
            NoAY[3] = sWETime;
            NoAY[4] = sBERange;
            NoAY[5] = sIsCOW;
            NoAY[6] = sWTMin;
            NoAY[7] = sRTType;

            oRetMsg = Check_Input_DB(NoAY, "Insert");

            if (oRetMsg.result)
            {
                #region Process String
                oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

                sSql = @" INSERT INTO B00_ClassData(CNo, CName, WBTime, WETime, BERange, IsCOW, WTMin, RTType, CreateUserID) 
                         VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?) ";
                sSqlParaLT.Add("S:" + sCNo.Trim());
                sSqlParaLT.Add("S:" + sCName.Trim());
                sSqlParaLT.Add("S:" + sWBTime.Trim());
                sSqlParaLT.Add("S:" + sWETime.Trim());
                sSqlParaLT.Add("S:" + sBERange.Trim());
                sSqlParaLT.Add("S:" + sIsCOW.Trim());
                sSqlParaLT.Add("S:" + sWTMin.Trim());
                sSqlParaLT.Add("S:" + sRTType.Trim());
                sSqlParaLT.Add("S:" + sUserID);

                oAcsDB.BeginTransaction();

                if (oAcsDB.SqlCommandExecute(sSql, sSqlParaLT) > -1)
                {
                    if (oAcsDB.Commit())
                    {
                        #region 取得新的資料ＩＤ
                        sSql = @" SELECT CID FROM B00_ClassData WHERE CNo = ?; ";
                        sSqlParaLT.Clear();
                        sSqlParaLT.Add("S:" + sCNo);

                        if (oAcsDB.GetDataReader(sSql, sSqlParaLT, out oDbRdr))
                        {
                            if (oDbRdr.HasRows)
                            {
                                oDbRdr.Read();
                                oRetMsg.message = sCNo + "|" + oDbRdr.DataReader["CID"].ToString();
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    oAcsDB.Rollback();
                }
                #endregion
            }

            oRetMsg.act = "Add";
            return oRetMsg;
        }
        #endregion

        #region Delete 刪除資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Delete(string sCID, string sUserID)
        {
            string sSql = "";
            DB_Acs oAcsDB = null; ;
            List<string> sSqlParaLT = new List<string>();
            Pub.MessageObject oRetMsg = new Pub.MessageObject();

            #region Process String
            oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

            sSql = @" DELETE FROM B00_ClassData WHERE CID = ? ; ";
            sSqlParaLT.Add("S:" + sCID.Trim());

            oAcsDB.BeginTransaction();
            if (oAcsDB.SqlCommandExecute(sSql, sSqlParaLT) > -1) { oAcsDB.Commit(); } else { oAcsDB.Rollback(); }
            #endregion

            oRetMsg.act = "Delete";
            return oRetMsg;
        }
        #endregion
        #endregion
    }
}