using Sa.DB;
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

namespace SahoAcs
{
    public partial class OrgStruc2 : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;     //宣告Ajax元件
        private int _pagesize = 20;        //DataGridView每頁顯示的列數
        #endregion

        #region 網頁前置作業

        #region LoadProcess
        private void LoadProcess()
        {
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";
            js += "\nChangeTextSet('" + this.GetLocalResourceObject("DeleteExcute_Msg") + "');";
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("OrgStruc", "OrgStruc.js");//加入同一頁面所需的JavaScript檔案
        }
        #endregion

        #region RegisterObj
        private void RegisterObj()
        {
            #region 設定彈跳視窗
            Pub.SetModalPopup(ModalPopupExtender1, 1);
            Pub.SetModalPopup(ModalPopupExtender2, 2);
            Pub.SetModalPopup(ModalPopupExtender3, 3);
            Pub.SetModalPopup(ModalPopupExtender4, 4);
            #endregion

            #region 註冊主頁Button動作
            AddButton.Attributes["onClick"] = "CallAdd('" + this.GetLocalResourceObject("CallAdd_Title") + "'); return false;";
            EditButton.Attributes["onClick"] = "CallEdit('" + this.GetLocalResourceObject("CallEdit_Title") + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";
            DeleteButton.Attributes["onClick"] = "CallDelete('" + this.GetLocalResourceObject("CallDelete_Title") + "', '" +
                this.GetLocalResourceObject("CallDelete_DelLabel") + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForDelete") + "'); return false;";
            QueryButton.Attributes["onClick"] = "SelectState(); return false;";
            AuthButton.Attributes["onClick"] = "CallAuth('" + this.GetLocalResourceObject("CallAuth_Title") + "', '" +
                this.GetLocalResourceObject("CallAuth_DelLabel") + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "','Mgn'); return false;";
            AuthEquGrButton.Attributes["onClick"] = "CallAuthEquGr('" + this.GetLocalResourceObject("CallAuthEquGr_Title") + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";
            EquListButton.Attributes["onClick"] = "CallAuth('" + this.GetLocalResourceObject("CallEqu_Title") + "', '" +
               this.GetLocalResourceObject("CallAuth_DelLabel") + "', '" +
               this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "','Equ'); return false;";
            #endregion

            #region 註冊pop1頁Button動作
            ImgCloseButton1.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            popB_Add.Attributes["onClick"] = "AddExcute('" + hUserId.Value + "'); return false;";
            popB_Edit.Attributes["onClick"] = "EditExcute('" + hUserId.Value + "'); return false;";
            popB_Cancel.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            popB_ddlClass.Attributes["onchange"] = "ShowOrgList();";
            popB_Enter.Attributes["onClick"] = "DataEnterRemove('Add'); return false;";
            popB_Remove.Attributes["onClick"] = "DataEnterRemove('Del'); return false;";
            #endregion

            #region 註冊pop2頁Button動作
            ImgCloseButton2.Attributes["onClick"] = "SetMode(''); CancelTrigger2.click(); return false;";
            popB_Delete.Attributes["onClick"] = "DeleteExcute('" + hUserId.Value + "', '" +
                this.GetLocalResourceObject("DeleteExcute_Msg") + "'); return false;";
            popB_Cancel2.Attributes["onClick"] = "SetMode(''); CancelTrigger2.click(); return false;";
            #endregion

            #region 註冊pop3頁Button動作
            ImgCloseButton3.Attributes["onClick"] = "SetMode(''); CancelTrigger3.click(); return false;";
            #endregion

            #region 註冊pop4頁Button動作
            ImgCloseButton4.Attributes["onClick"] = "SetMode(''); CancelTrigger4.click(); return false;";
            popB_Auth2.Attributes["onClick"] = "AuthExcute('" + hUserId.Value + "'); return false;";
            popB_Cancel4.Attributes["onClick"] = "SetMode(''); CancelTrigger4.click(); return false;";
            popB_Enter2.Attributes["onClick"] = "DataEnterRemove2('Add'); return false;";
            popB_Remove2.Attributes["onClick"] = "DataEnterRemove2('Del'); return false;";
            #endregion

            //設定DataGridView每頁顯示的列數
            this.MainGridView.PageSize = _pagesize;
        }
        #endregion

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                hUserId.Value = Session["UserID"].ToString();
                LoadProcess();
                RegisterObj();

                if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
                {
                    ViewState["SortExpression"] = "OrgStrucNo";
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
                        if (sFormArg == "popPagePost") //進行因應新增或編輯後所需的換頁動作
                        {
                            int find = Query("popPagePost", ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                            Sa.Web.Fun.RunJavaScript(this, "__doPostBack('ctl00$ContentPlaceHolder1$MainGridView', 'Page$" + find + "');");
                        }
                        else if (sFormArg.Substring(0, 5) == "Page$") //換頁完成後進行GridViewRow的移動
                        {
                            Query("popPagePost", ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                            Sa.Web.Fun.RunJavaScript(this, "GridSelect();");
                        }
                        else if (sFormArg == "NewQuery")
                        {
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
            catch
            {
                Response.Redirect("~/Web/MessagePage/LoginSorry.aspx");
            }
        }
        #endregion

        #region VerifyRenderingInServerForm
        public override void VerifyRenderingInServerForm(Control control)
        {
            //如有使用UpdatePanel配合GridVew才需要這個方法
            // 修正 'XX'型別 必須置於有 runat=server 的表單標記之中 Override此Methods
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
            Encoding big5 = Encoding.GetEncoding("big5");
            byte[] b = big5.GetBytes(str);
            if (b.Length <= len)
                return str;
            else
            {
                if (ellipsis) len -= 3;

                string res = big5.GetString(b, 0, len);
                if (!big5.GetString(b).StartsWith(res))
                    res = big5.GetString(b, 0, len - 1);
                return res + (ellipsis ? "..." : "");
            }
        }
        #endregion

        #endregion

        #region 查詢

        private void Query(bool select_state, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<String> CheckData = new List<String>();
            List<string> liSqlPara = new List<string>();
            String Nowno = "", NowOrgNo = "", NowOrgName = "";
            if (select_state)
            {
                CheckData.Add(this.Input_No.Text.Trim());
                CheckData.Add(this.Input_OrgNo.Text.Trim());
                CheckData.Add(this.Input_OrgName.Text.Trim());
                CatchSession(CheckData);
                Nowno = this.Input_No.Text.Trim();
                NowOrgNo = this.Input_OrgNo.Text.Trim();
                NowOrgName = this.Input_OrgName.Text.Trim();
            }
            else
            {
                if (Session["OldSearchList"].ToString() != "" && Session["OldSearchList"] != null)
                {
                    String[] mgalist = Session["OldSearchList"].ToString().Split('|');
                    Nowno = mgalist[0];
                    NowOrgNo = mgalist[1];
                    NowOrgName = mgalist[2];
                }
            }

            #region Process String

            sql = " SELECT * FROM OrgStrucAllData('') ";

            if (!string.IsNullOrEmpty(Nowno))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (OrgStrucNo LIKE ? ) ";
                liSqlPara.Add("S:" + '%' + Nowno + '%');
            }
            if (!string.IsNullOrEmpty(NowOrgNo))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (OrgNoList LIKE ? ) ";
                liSqlPara.Add("S:" + '%' + NowOrgNo + '%');
            }
            if (!string.IsNullOrEmpty(NowOrgName))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (OrgNameList LIKE ? ) ";
                liSqlPara.Add("S:" + '%' + NowOrgName + '%');
            }

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();
            hSelectState.Value = "false";
        }

        private int Query(string mode, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();
            String Nowno = "", NowOrgNo = "", NowOrgName = "";
            if (Session["OldSearchList"].ToString() != "" && Session["OldSearchList"].ToString() != null)
            {
                String[] mgalist = Session["OldSearchList"].ToString().Split('|');
                Nowno = mgalist[0];
                NowOrgNo = mgalist[1];
                NowOrgName = mgalist[2];
            }

            #region Process String
            sql = " SELECT * FROM OrgStrucAllData('') ";
            //wheresql += " (OwnerList LIKE '%\\" + hUserId.Value + "\\%') ";

            //視情況調整
            //if (!string.IsNullOrEmpty(mgano))
            //{
            //    if (wheresql != "") wheresql += " AND ";
            //    wheresql += " (MgaNo LIKE '%' + ? +'%') ";
            //    liSqlPara.Add("S:" + '%' + mgano + '%');
            //}

            //if (!string.IsNullOrEmpty(mganame))
            //{
            //    if (wheresql != "") wheresql += " AND ";
            //    wheresql += " (MgaName LIKE '%' + ? +'%' OR MgaEName LIKE '%' + ? +'%') ";
            //    liSqlPara.Add("S:" + '%' + mganame + '%');
            //    liSqlPara.Add("S:" + '%' + mganame + '%');
            //}

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();

            int find = 0;

            #region 取得RoleTable後的動作
            DataRow dr = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dr = dt.Rows[i];

                if (SelectValue.Value == dr["OrgStrucID"].ToString())
                {
                    find = i;
                    break;
                }
            }
            #endregion

            return (find / _pagesize) + 1;
        }

        protected static Pub.MessageObject Check_Input_DB(string[] NoArray, string mode)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Input
            if (mode == "Insert")
            {
                if (string.IsNullOrEmpty(NoArray[0].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += "編號 必須輸入";
                }
                if (NoArray[0].Contains(" "))
                {
                    sRet.result = false;
                    sRet.message += "編號字串中不可有空白符號";
                }
            }
            if (mode == "Update")
            {
                if (string.IsNullOrEmpty(NoArray[0].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += "編號 必須輸入";
                }
                if (NoArray[0].Contains(" "))
                {
                    sRet.result = false;
                    sRet.message += "編號字串中不可有空白符號";
                }
            }
            #endregion
            #region DB
            switch (mode)
            {
                case "Insert":
                    sql = @" SELECT * FROM B01_OrgStruc WHERE OrgStrucNo = ? ";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    break;
                case "Update":
                    sql = @" SELECT * FROM B01_OrgStruc WHERE OrgStrucNo = ? AND OrgStrucNo <> ? ";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    liSqlPara.Add("S:" + NoArray[1].Trim());
                    break;
            }

            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                sRet.result = false;
                sRet.message += "此編號已存在於系統中";
            }
            #endregion

            return sRet;
        }

        #endregion

        #region GridView處理
        protected void GridView_Data_DataBound(object sender, EventArgs e)
        {
            td_showGridView.Attributes["colspan"] = MainGridView.Columns.Count.ToString();
        }

        protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            MainGridView.PageIndex = e.NewPageIndex;
            if (MainGridView.Rows[0].Cells.Count == MainGridView.HeaderRow.Cells.Count)
                MainGridView.DataBind();
        }

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
            hSelectState.Value = "true";
            Query(true, SortField, SortDire);
        }

        protected void GridView_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    e.Row.Cells[1].Width = 100;
                    e.Row.Cells[2].Width = 250;
                    e.Row.Cells[3].Width = 250;
                    #endregion

                    #region 設定表頭欄位Visible狀態
                    //不想顯示的欄位，可在此處關掉

                    e.Row.Cells[0].Visible = false;

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
                    GrRow.ID = "GV_Row" + oRow.Row["OrgStrucID"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    e.Row.Cells[1].Width = 104;
                    e.Row.Cells[2].Width = 254;
                    e.Row.Cells[3].Width = 254;
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 組織架構ID
                    e.Row.Cells[0].Visible = false;
                    #endregion
                    #region 編號
                    #endregion
                    #region 名稱
                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }
                    //e.Row.Cells[0].Text = LimitText(e.Row.Cells[0].Text, 10);
                    e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 10, true);
                    e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text, 30, true);
                    e.Row.Cells[3].Text = LimitText(e.Row.Cells[3].Text, 30, true);
                    e.Row.Cells[4].Text = LimitText(e.Row.Cells[4].Text, 10, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["OrgStrucID"].ToString() + "', '', '');");
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
                    lbtnPrev.Click += delegate(object obj, EventArgs args)
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

                    lbtnNext.Click += delegate(object obj, EventArgs args)
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
                    lbtnFirst.Click += delegate(object obj, EventArgs args)
                    {
                        gv.PageIndex = 0;
                        if (hSelectState.Value == "true")
                            Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        else
                            Query(false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    };

                    lbtnLast.Click += delegate(object obj, EventArgs args)
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
                        new LiteralControl(string.Format("　總共 {0} 筆　", hDataRowCount.Value)));
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

        #region 查無資料時，GridView顯示查無資料資訊
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
                ProcessGridView.Rows[0].Cells[0].Text = "查無資料";
                //ProcessGridView.RowStyle.HorizontalAlign = HorizontalAlign.Center;
            }
        }
        #endregion

        #endregion

        #region JavaScript及aspx共用方法

        #region DDLData
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static String[] DDLData(String Ctrl_ddl, String Ctrl_List, String sList)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Hashtable hList = null;
            DBReader dr = null;
            String sData = Ctrl_ddl + "|";
            String sData2 = Ctrl_List + "|";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            String sql = @" SELECT ItemNo,ItemName FROM B00_ItemList WHERE ItemClass = 'OrgClass' AND ItemOrder > 0 ";
            if (sList != "")
            {
                hList = new Hashtable();
                string[] sdata = sList.Split('|');
                for (int i = 0; i < sdata.Length - 1; i += 4)
                {
                    sql += " AND ItemOrder <> " + sdata[i + 2] + " ";
                    hList.Add(sdata[i + 2], sdata[i] + "|" + sdata[i + 1] + "|" + sdata[i + 2] + "|" + sdata[i + 3]);
                }
            }
            sql += " ORDER BY ItemOrder ";
            oAcsDB.GetDataReader(sql, out dr);
            if (dr.HasRows)
            {
                EditData = new string[dr.DataReader.FieldCount + 2];
                while (dr.Read())
                    sData += dr.DataReader[0].ToString() + "|" + dr.DataReader[1].ToString() + "|";
                EditData[EditData.Length - 2] = sData.Substring(0, sData.Length - 1);
                EditData[EditData.Length - 1] = sData2;
            }
            else
            {
                EditData = new string[3];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = sData;
                EditData[2] = sData2;
            }
            if (hList != null)
            {
                if (hList.Count > 0)
                {
                    ArrayList mArr = new ArrayList();
                    foreach (DictionaryEntry obj in hList)
                        mArr.Add(int.Parse(obj.Key.ToString()));
                    mArr.Sort();
                    for (int i = 0; i < mArr.Count; i++)
                        sData2 += hList[mArr[i].ToString()] + "|";
                    EditData[EditData.Length - 1] = sData2.Substring(0, sData2.Length - 1);
                }
            }
            return EditData;
        }
        #endregion

        #region Insert 寫入新資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Insert(string sOrgStrucNo, string sOrgIDList, String UserID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();

            string[] NoArray = new string[1];
            NoArray[0] = sOrgStrucNo;

            sRet = Check_Input_DB(NoArray, "Insert");
            if (sRet.result)
            {
                #region Process String
                sql = @" INSERT INTO B01_OrgStruc( OrgStrucNo ,OrgIDList ,CreateUserID ,UpdateUserID ,UpdateTime)  
                         VALUES ( ? ,? ,? ,? ,GETDATE()) ";
                liSqlPara.Add("S:" + sOrgStrucNo.Trim());
                liSqlPara.Add("S:" + sOrgIDList);
                liSqlPara.Add("S:" + UserID);
                liSqlPara.Add("S:" + UserID);
                #endregion
                oAcsDB.BeginTransaction();
                int istat = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                if (istat > -1)
                {
                    if (oAcsDB.Commit())
                    {
                        #region 取得新資料ID
                        #region Process String
                        sql = " SELECT OrgStrucID FROM B01_OrgStruc WHERE OrgStrucNo = ? ";
                        liSqlPara.Clear();
                        liSqlPara.Add("S:" + sOrgStrucNo.Trim());
                        #endregion
                        Sa.DB.DBReader oReader = null;
                        if (oAcsDB.GetDataReader(sql, liSqlPara, out oReader))
                        {
                            if (oReader.HasRows)
                            {
                                oReader.Read();
                                sRet.message = sOrgStrucNo.Trim() + "|" + oReader.DataReader["OrgStrucID"].ToString();

                                #region 將新增組織架構納入全區的管理區下
                                liSqlPara.Clear();
                                sql = @"INSERT INTO B01_MgnOrgStrucs(MgaID, OrgStrucID, CreateUserID) VALUES(?, ?, ?)";
                                liSqlPara.Add("S:" + "1");
                                liSqlPara.Add("S:" + oReader.DataReader["OrgStrucID"].ToString());
                                liSqlPara.Add("S:" + UserID.ToString());
                                oAcsDB.SqlCommandExecute(sql, liSqlPara);
                                #endregion
                            }
                        }
                        #endregion
                    }
                }
                else
                    oAcsDB.Rollback();
            }
            sRet.act = "Add";
            return sRet;
        }
        #endregion

        #region Update 更新資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Update(String OrgStruc_ID, String Old_OrgStruc_No, string New_OrgStruc_No, String Org_IDList, String UserID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            int istat = 0;
            List<string> liSqlPara = new List<string>();

            string[] NoArray = new string[2];
            NoArray[0] = New_OrgStruc_No.Trim();
            NoArray[1] = Old_OrgStruc_No.Trim();

            sRet = Check_Input_DB(NoArray, "Update");
            if (sRet.result)
            {
                if (istat > -1)
                {
                    #region Process String
                    sql = @" UPDATE B01_OrgStruc SET OrgStrucNo = ? ,OrgIDList = ? ,UpdateUserID = ? ,UpdateTime = GETDATE() WHERE OrgStrucID = ? ; ";
                    liSqlPara.Add("S:" + New_OrgStruc_No.Trim());
                    liSqlPara.Add("S:" + Org_IDList);
                    liSqlPara.Add("S:" + UserID);
                    liSqlPara.Add("S:" + OrgStruc_ID);

                    #endregion
                    oAcsDB.BeginTransaction();
                    istat = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                if (istat > -1)
                    oAcsDB.Commit();
                else
                    oAcsDB.Rollback();
            }
            sRet.act = "Edit";
            return sRet;
        }
        #endregion

        #region Delete 刪除資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Delete(string OrgStruc_ID, string ChangeOrgStrucID, String UserID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            int istat = 0;
            List<string> liSqlPara = new List<string>();

            if (sRet.result)
            {
                if (istat > -1)
                {
                    #region Process String
                    sql = @" DELETE FROM B01_OrgStruc WHERE OrgStrucID = ? ; ";
                    sql += @" DELETE FROM B01_MgnOrgStrucs WHERE OrgStrucID = ? ; ";
                    liSqlPara.Add("S:" + OrgStruc_ID.Trim());
                    liSqlPara.Add("S:" + OrgStruc_ID.Trim());
                    #endregion
                    oAcsDB.BeginTransaction();
                    istat = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                if (istat > -1)
                    oAcsDB.Commit();
                else
                    oAcsDB.Rollback();
            }
            sRet.act = "Delete";
            return sRet;
        }
        #endregion

        #region 取得選取的Class的組織資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static String[] ShowOrgData(String CtrlID, String sClass)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            string[] EditData = null;
            String sData = "";
            List<string> liSqlPara = new List<string>();
            String sql = @" SELECT B01_OrgData.*, B00_ItemList.ItemOrder FROM B01_OrgData INNER JOIN B00_ItemList ON B01_OrgData.OrgClass = B00_ItemList.ItemNo WHERE OrgClass = '" + sClass + "' ORDER BY OrgNo ";
            oAcsDB.GetDataReader(sql, out dr);
            EditData = new string[1];
            sData += CtrlID + "|";
            if (dr.HasRows)
            {
                while (dr.Read())
                    sData += dr.DataReader[2].ToString() + "|" + dr.DataReader[3].ToString() + "|" + dr.DataReader[0].ToString() + "|" + dr.DataReader[10].ToString() + "|" + dr.DataReader[1].ToString() + "|";
                EditData[0] = sData.Substring(0, sData.Length - 1);
            }
            else
            {
                EditData[0] = sData;
            }
            return EditData;
        }
        #endregion

        #region 載入單筆資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadData(String Ctrl_ddl, String Ctrl_List, string OrgStruc_ID, String UserID, String mode)
        {
            Hashtable hOrgData = null;
            List<HItemList> HILList = null;
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            String sql = "";

            #region 取得所有組織資料
            sql = @" SELECT B01_OrgData.OrgID,
                     '['+B01_OrgData.OrgNo+']'+ B01_OrgData.OrgName+'|'+CONVERT(VARCHAR(100),B01_OrgData.OrgID) +'|'+CONVERT(VARCHAR(100),B00_ItemList.ItemOrder)+'|'+ B00_ItemList.ItemNo AS OrgVal 
                     FROM B01_OrgData 
                     INNER JOIN B00_ItemList ON B01_OrgData.OrgClass = B00_ItemList.ItemNo AND B00_ItemList.ItemClass = 'OrgClass' 
                     ORDER BY OrgID  ";
            oAcsDB.GetDataReader(sql, out dr);
            if (dr.HasRows)
            {
                hOrgData = new Hashtable();
                while (dr.Read())
                    hOrgData.Add(dr.DataReader[0].ToString(), dr.DataReader[1].ToString());
            }
            #endregion

            #region 取得組織類型資料
            dr = null;
            sql = @" SELECT ItemOrder,ItemNo,ItemName FROM B00_ItemList WHERE ItemClass = 'OrgClass' AND ItemOrder > 0 ORDER BY ItemOrder ";
            oAcsDB.GetDataReader(sql, out dr);
            if (dr.HasRows)
            {
                HILList = new List<HItemList>();
                while (dr.Read())
                {
                    HItemList CIL = new HItemList();
                    CIL.ItemOrder = dr.DataReader[0].ToString();
                    CIL.ItemNo = dr.DataReader[1].ToString();
                    CIL.ItemName = dr.DataReader[2].ToString();
                    HILList.Add(CIL);
                }
            }
            #endregion

            Pub.MessageObject sRet = new Pub.MessageObject();
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            dr = null;
            bool OnceState = true;

            #region Process String
            sql = @" SELECT * FROM B01_OrgStruc WHERE OrgStrucID = ? ";
            liSqlPara.Add("S:" + OrgStruc_ID.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                EditData = new string[dr.DataReader.FieldCount + 2];
                EditData[EditData.Length - 2] = Ctrl_ddl + "|";
                EditData[EditData.Length - 1] = Ctrl_List + "|";
                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                {
                    EditData[i] = dr.DataReader[i].ToString();
                    if (i == 2 && dr.DataReader[i].ToString() != "")
                    {
                        String[] sdata = dr.DataReader[i].ToString().Split('\\');
                        for (int j = 1; j < sdata.Length - 1; j++)
                        {
                            EditData[EditData.Length - 1] += hOrgData[sdata[j]] + "|";
                            String[] sItem = hOrgData[sdata[j]].ToString().Split('|');
                            for (int k = 0; k < HILList.Count; k++)
                            {
                                HItemList mHIL = (HItemList)HILList[k];
                                if (mHIL.ItemOrder == sItem[2])
                                    HILList.Remove(mHIL);
                                mHIL = null;
                            }
                            sItem = null;
                        }
                        sdata = null;

                        EditData[EditData.Length - 1] = EditData[EditData.Length - 1].Substring(0, EditData[EditData.Length - 1].Length - 1);
                    }
                }
                if (mode == "Edit" && HILList.Count > 0)
                {
                    for (int i = 0; i < HILList.Count; i++)
                    {
                        HItemList mHIL = (HItemList)HILList[i];
                        EditData[EditData.Length - 2] += mHIL.ItemNo + "|" + mHIL.ItemName + "|";
                        mHIL = null;
                    }
                    EditData[EditData.Length - 2] = EditData[EditData.Length - 2].Substring(0, EditData[EditData.Length - 2].Length - 1);
                    HILList = null;
                }
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "系統中無此資料！";
                OnceState = false;
            }

            if (mode == "Delete" && OnceState)
            {
                liSqlPara.Clear();
                dr = null;
                sql = @" SELECT * FROM B01_OrgStruc WHERE OrgStrucID <> ? ";
                liSqlPara.Add("S:" + EditData[0]);
                oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                if (dr.HasRows)
                {
                    while (dr.Read())
                        EditData[EditData.Length - 2] += dr.DataReader[0].ToString() + "|" + dr.DataReader[1].ToString() + "|";
                    EditData[EditData.Length - 2] = EditData[EditData.Length - 2].Substring(0, EditData[EditData.Length - 2].Length - 1);
                }
            }
            #endregion

            return EditData;
        }
        #endregion

        #region 載入管理區資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadMgaData(String OrgStruc_ID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            String sql = "";
            List<string> liSqlPara = new List<string>();
            string[] EditData = new string[2];
            EditData[1] = "";

            #region 取得編號
            sql = " SELECT * FROM B01_OrgStruc WHERE OrgStrucID = ? ";
            liSqlPara.Add("S:" + OrgStruc_ID.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.HasRows)
            {
                while (dr.Read())
                    EditData[0] = dr.DataReader[1].ToString();
            }
            else
            {
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "系統中無此資料！";
            }
            #endregion

            #region 取得管理區資料
            liSqlPara = new List<string>();
            dr = null;
            sql = @" SELECT B00_ManageArea.MgaNo, B00_ManageArea.MgaName, B00_ManageArea.MgaEName ";
            sql += @" FROM B01_MgnOrgStrucs INNER JOIN B00_ManageArea ON B01_MgnOrgStrucs.MgaID = B00_ManageArea.MgaID ";
            sql += @" WHERE OrgStrucID = ? ";
            liSqlPara.Add("S:" + OrgStruc_ID.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    EditData[1] += dr.DataReader[0].ToString() + "|" + dr.DataReader[1].ToString() + "|" + dr.DataReader[2].ToString() + "|";
                }
                EditData[1] = EditData[1].Substring(0, EditData[1].Length - 1);
            }
            #endregion

            return EditData;
        }
        #endregion

        #region 載入設備資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadEquData(String OrgStruc_ID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            String sql = "";
            List<string> liSqlPara = new List<string>();
            string[] EditData = new string[2];
            EditData[1] = "";

            #region 取得編號
            sql = " SELECT * FROM B01_OrgStruc WHERE OrgStrucID = ? ";
            liSqlPara.Add("S:" + OrgStruc_ID.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.HasRows)
            {
                while (dr.Read())
                    EditData[0] = dr.DataReader[1].ToString();
            }
            else
            {
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "系統中無此資料！";
            }
            #endregion

            #region 取得設備資料
            //sql = @" SELECT EquID, EquNo, EquName FROM dbo.OrgStrucEquData(?) ORDER BY EquID ";
            sql = @" SELECT dbo.B01_EquData.EquID, dbo.B01_EquData.EquNo, dbo.B01_EquData.EquName ";
            sql += @" FROM dbo.B01_EquData INNER JOIN ";
            sql += @" dbo.B01_EquGroupData ON dbo.B01_EquData.EquID = dbo.B01_EquGroupData.EquID INNER JOIN ";
            sql += @" dbo.B01_OrgEquGroup ON dbo.B01_EquGroupData.EquGrpID = dbo.B01_OrgEquGroup.EquGrpID ";
            sql += @" WHERE dbo.B01_OrgEquGroup.OrgStrucID = ? ";
            sql += @" ORDER BY EquID  ";
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    EditData[1] += dr.DataReader[0].ToString() + "|" + dr.DataReader[1].ToString() + "|" + dr.DataReader[2].ToString() + "|";
                }

                EditData[1] = EditData[1].Substring(0, EditData[1].Length - 1);
            }
            else
            {
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "系統中無此資料！";
            }
            #endregion

            return EditData;
        }
        #endregion

        #region AuthUpdate 權限資料編輯
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object AuthUpdate(String OrgStruc_ID, String EquGrList, String UserID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            int istat = 0;
            List<string> liSqlPara = new List<string>();

            if (sRet.result)
            {
                if (istat > -1)
                {
                    #region Process String
                    sql = @" DELETE FROM B01_OrgEquGroup WHERE OrgStrucID = ? ; ";
                    liSqlPara.Add("S:" + OrgStruc_ID);
                    if (EquGrList.Length > 0)
                    {
                        String[] OSL = EquGrList.Split('|');
                        for (int i = 0; i < OSL.Length - 1; i++)
                        {
                            sql += @" INSERT INTO B01_OrgEquGroup( OrgStrucID ,EquGrpID ,CreateUserID) ";
                            sql += @" VALUES ( ? ,? ,?) ; ";
                            liSqlPara.Add("S:" + OrgStruc_ID);
                            liSqlPara.Add("S:" + OSL[i]);
                            liSqlPara.Add("S:" + UserID);
                        }
                    }

                    #endregion
                    oAcsDB.BeginTransaction();
                    istat = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                if (istat > -1)
                    oAcsDB.Commit();
                else
                    oAcsDB.Rollback();
            }
            sRet.act = "AuthUpdate";
            return sRet;
        }
        #endregion

        #region 載入設備群組資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static String[] LoadEquGrList(string OrgStruc_ID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            String[] EditData = null;
            List<String> EquGrList = null;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Process String
            sql = @" SELECT * FROM B01_OrgStruc WHERE OrgStrucID = ? ";
            liSqlPara.Add("S:" + OrgStruc_ID.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            EditData = new String[3];
            if (dr.HasRows)
            {
                if (dr.Read())
                    EditData[0] = dr.DataReader[1].ToString();
            }
            else
            {
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "系統中無此資料！";
            }

            #endregion

            #region Process String2
            if (EditData[0] != "Saho_SysErrorMassage")
            {
                dr = null;
                liSqlPara = new List<string>();
                sql = " SELECT B01_OrgStruc.OrgStrucID, B01_EquGroup.EquGrpID, B01_EquGroup.EquGrpNo, B01_EquGroup.EquGrpName ";
                sql += " FROM B01_OrgStruc INNER JOIN ";
                sql += " B01_OrgEquGroup ON B01_OrgStruc.OrgStrucID = B01_OrgEquGroup.OrgStrucID INNER JOIN ";
                sql += " B01_EquGroup ON B01_OrgEquGroup.EquGrpID = B01_EquGroup.EquGrpID ";
                sql += " WHERE B01_OrgStruc.OrgStrucID = ? ";
                liSqlPara.Add("S:" + OrgStruc_ID.Trim());
                oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                EquGrList = new List<string>();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        EditData[2] += dr.DataReader[1].ToString() + "|" + dr.DataReader[2].ToString() + "|" + dr.DataReader[3].ToString() + "|";
                        EquGrList.Add(dr.DataReader[1].ToString());
                    }
                    EditData[2] = EditData[2].Substring(0, EditData[2].Length - 1);
                }
                else
                    EditData[2] = "";
            }
            #endregion

            #region Process String3
            if (EditData[0] != "Saho_SysErrorMassage")
            {
                dr = null;
                liSqlPara = new List<string>();
                sql = @" SELECT * FROM B01_EquGroup ";
                if (EquGrList.Count > 0)
                {
                    sql += " WHERE EquGrpID NOT IN ( ";
                    for (int i = 0; i < EquGrList.Count; i++)
                        sql += "'" + EquGrList[i] + "',";
                    sql = sql.Substring(0, sql.Length - 1) + ") ";
                }
                oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        EditData[1] += dr.DataReader[0].ToString() + "|" + dr.DataReader[1].ToString() + "|" + dr.DataReader[2].ToString() + "|";
                    }
                    EditData[1] = EditData[1].Substring(0, EditData[1].Length - 1);
                }
                else
                    EditData[1] = "";
            }
            #endregion

            return EditData;
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

        #region  物件
        public class HItemList
        {
            public String ItemOrder { get; set; }
            public String ItemNo { get; set; }
            public String ItemName { get; set; }
        }
        #endregion
    }
}