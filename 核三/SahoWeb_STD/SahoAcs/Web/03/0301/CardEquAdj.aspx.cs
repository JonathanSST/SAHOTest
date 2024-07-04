using Sa.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web
{
    public partial class CardEquAdj : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;     //宣告Ajax元件
        private int _pagesize = 20;      //DataGridView每頁顯示的列數
        #endregion

        #region 網頁前置作業

        #region LoadProcess
        private void LoadProcess()
        {
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            //oScriptManager.RegisterAsyncPostBackControl(QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            //js += "\nSetMode('');";
            //js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("CardEquAdj", "CardEquAdj.js");//加入同一頁面所需的JavaScript檔案
        }
        #endregion

        #region RegisterObj
        private void RegisterObj()
        {
            #region 設定彈跳視窗
            //Pub.SetModalPopup(ModalPopupExtender1, 1);
            #endregion

            #region 註冊主頁Button動作
            //Input_OrgStrucID.Attributes["onchange"] = "OrgStrucData();";
            //Input_PsnPicSource.Attributes["onkeyup"] = "InputPicText();";
            //btSave.Attributes["onClick"] = "SaveData('" + hUserId.Value + "'); return false;";
            //btDelete.Attributes["onClick"] = "DeleteData('" + hUserId.Value + "'); return false;";
            //btCardInfo.Attributes["onClick"] = "CallCardEdit(); return false;";
            //btCardAdd.Attributes["onClick"] = "CallCardAdd(); return false;";
            QueryButton.Attributes["onClick"] = "SelectState(); return false;";
            #endregion

            #region 註冊pop1頁Button動作
            //ImgCloseButton1.Attributes["onClick"] = "CancelTrigger1.click();UnSelectItem(); return false;";
            //popB_Add.Attributes["onClick"] = "SaveCardData('" + hUserId.Value + "'); return false;";
            //popB_Edit.Attributes["onClick"] = "SaveCardData('" + hUserId.Value + "'); return false;";
            //popB_Cancel.Attributes["onClick"] = "CancelTrigger1.click();UnSelectItem(); return false;";
            //popB_Delete.Attributes["onClick"] = "DeleteCardData('" + hUserId.Value + "'); return false;";
            #endregion

            //設定DataGridView每頁顯示的列數
            this.MainGridView.PageSize = _pagesize;
            this.MainGridView2.PageSize = _pagesize;
        }
        #endregion

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            hUserId.Value = Session["UserID"].ToString();
            LoadProcess();
            RegisterObj();
            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                hSelectState.Value = "true";
                Query(true);
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];
                if (!string.IsNullOrEmpty(sFormArg))
                {
                    if (sFormArg == "popPagePost") //進行因應新增或編輯後所需的換頁動作
                    {
                        int find = Query("popPagePost");
                        Sa.Web.Fun.RunJavaScript(this, "__doPostBack('ctl00$ContentPlaceHolder1$MainGridView', 'Page$" + find + "');");
                    }
                    else if (sFormArg.Substring(0, 5) == "Page$") //換頁完成後進行GridViewRow的移動
                    {
                        Query("popPagePost");
                        Sa.Web.Fun.RunJavaScript(this, "GridSelect();");
                    }
                    else if (sFormArg == "NewQuery")
                    {
                        hSelectState.Value = "true";
                        Query(true);
                    }
                }
                else
                {
                    hSelectState.Value = "false";
                    Query(false);
                }
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

        private void Query(bool select_state)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            DBReader dr = null;
            List<String> CheckData = new List<String>();
            List<string> liSqlPara = new List<string>();

            String NowData = "";
            if (select_state)
            {
                CheckData.Add(this.InputWord.Text.Trim());
                CatchSession(CheckData);
                NowData = this.InputWord.Text.Trim();
            }
            else
            {
                if (Session["OldSearchList"].ToString() != "" && Session["OldSearchList"].ToString() != null)
                {
                    String[] mgalist = Session["OldSearchList"].ToString().Split('|');
                    NowData = mgalist[0];
                }
            }
            #region Process String
            sql = " SELECT DISTINCT(OrgStrucID),OrgStrucNo FROM ";
            sql += " (SELECT B00_SysUserMgns.UserID, B01_MgnOrgStrucs.MgaID, B01_OrgStruc.OrgStrucID, B01_OrgStruc.OrgStrucNo, B01_OrgStruc.OrgIDList ";
            sql += " FROM B01_MgnOrgStrucs INNER JOIN B01_OrgStruc ON B01_MgnOrgStrucs.OrgStrucID = B01_OrgStruc.OrgStrucID INNER JOIN B00_SysUserMgns ON B01_MgnOrgStrucs.MgaID = B00_SysUserMgns.MgaID)a ";
            sql += " WHERE a.UserID = '" + hUserId.Value + "' ";
            oAcsDB.GetDataReader(sql, out dr);
            if (dr.HasRows)
            {
                wheresql = " (";
                while (dr.Read())
                    wheresql += "B01_Person.OrgStrucID = " + dr.DataReader["OrgStrucID"].ToString() + " OR ";
                wheresql = wheresql.Substring(0, wheresql.Length - 4);
                wheresql += ") ";
            }

            sql = " SELECT DISTINCT(B01_Person.PsnID), B01_Person.PsnNo, B01_Person.PsnName, B01_Person.PsnEName, B01_Person.PsnType,  ";
            sql += " B01_Person.IDNum, B01_Person.Birthday, B01_Person.OrgStrucID, B01_Person.PsnAccount, B01_Person.PsnPW,  ";
            sql += " B01_Person.PsnAuthAllow, B01_Person.PsnSTime, B01_Person.PsnETime, B01_Person.PsnPicSource, B01_Person.Remark,  ";
            sql += " B01_Person.CreateUserID, B01_Person.CreateTime, B01_Person.UpdateUserID, B01_Person.UpdateTime, B01_Person.Rev01, ";
            sql += " B01_Person.Rev02, OrgStrucAllData_1.OrgStrucNo, OrgStrucAllData_1.OrgNameList, OrgStrucAllData_1.OrgNoList, '' AS VCard ";
            sql += " FROM B01_Person INNER JOIN ";
            sql += " OrgStrucAllData('') AS OrgStrucAllData_1 ON B01_Person.OrgStrucID = OrgStrucAllData_1.OrgStrucID LEFT OUTER JOIN ";
            sql += " B01_Card ON B01_Person.PsnID = B01_Card.PsnID ";

            if (!string.IsNullOrEmpty(NowData))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " ( PsnNo LIKE ? OR PsnName LIKE ? OR PsnEName LIKE ? OR CardNo LIKE ? OR OrgNameList LIKE ? OR OrgNoList LIKE ? ) ";
                liSqlPara.Add("S:" + '%' + NowData + '%');
                liSqlPara.Add("S:" + '%' + NowData + '%');
                liSqlPara.Add("S:" + '%' + NowData + '%');
                liSqlPara.Add("S:" + '%' + NowData + '%');
                liSqlPara.Add("S:" + '%' + NowData + '%');
                liSqlPara.Add("S:" + '%' + NowData + '%');
            }

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY PsnNo ";
            #endregion

            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();
            oAcsDB.GetDataTable("DataTable", "SELECT B01_Card.*, B00_ItemList.ItemName FROM B01_Card INNER JOIN B00_ItemList ON B01_Card.CardType = B00_ItemList.ItemNo WHERE (B00_ItemList.ItemClass = 'CardType') AND PsnID = 0", out dt);
            GirdViewDataBind(this.MainGridView2, dt);
            hSelectState.Value = "false";
            Label_PsnNo.Text = "";
        }

        private int Query(string mode)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            DBReader drd = null;
            List<string> liSqlPara = new List<string>();
            //String Nowno = "", Nowname = "", NowClass = "";
            //if (Session["OldSearchList"].ToString() != "" && Session["OldSearchList"].ToString() != null)
            //{
            //    String[] mgalist = Session["OldSearchList"].ToString().Split('|');
            //    Nowno = mgalist[0];
            //    Nowname = mgalist[1];
            //    NowClass = mgalist[2];
            //}

            #region Process String
            sql = " SELECT DISTINCT(OrgStrucID),OrgStrucNo FROM ";
            sql += " (SELECT B00_SysUserMgns.UserID, B01_MgnOrgStrucs.MgaID, B01_OrgStruc.OrgStrucID, B01_OrgStruc.OrgStrucNo, B01_OrgStruc.OrgIDList ";
            sql += " FROM B01_MgnOrgStrucs INNER JOIN B01_OrgStruc ON B01_MgnOrgStrucs.OrgStrucID = B01_OrgStruc.OrgStrucID INNER JOIN B00_SysUserMgns ON B01_MgnOrgStrucs.MgaID = B00_SysUserMgns.MgaID)a ";
            sql += " WHERE a.UserID = '" + hUserId.Value + "' ";
            oAcsDB.GetDataReader(sql, out drd);
            if (drd.HasRows)
            {
                wheresql = " (";
                while (drd.Read())
                    wheresql += "B01_Person.OrgStrucID = " + drd.DataReader["OrgStrucID"].ToString() + " OR ";
                wheresql = wheresql.Substring(0, wheresql.Length - 4);
                wheresql += ") ";
            }

            sql = " SELECT DISTINCT(B01_Person.PsnID), B01_Person.PsnNo, B01_Person.PsnName, B01_Person.PsnEName, B01_Person.PsnType,  ";
            sql += " B01_Person.IDNum, B01_Person.Birthday, B01_Person.OrgStrucID, B01_Person.PsnAccount, B01_Person.PsnPW,  ";
            sql += " B01_Person.PsnAuthAllow, B01_Person.PsnSTime, B01_Person.PsnETime, B01_Person.PsnPicSource, B01_Person.Remark,  ";
            sql += " B01_Person.CreateUserID, B01_Person.CreateTime, B01_Person.UpdateUserID, B01_Person.UpdateTime, B01_Person.Rev01, ";
            sql += " B01_Person.Rev02, OrgStrucAllData_1.OrgStrucNo, OrgStrucAllData_1.OrgNameList, OrgStrucAllData_1.OrgNoList ";
            sql += " FROM B01_Person INNER JOIN ";
            sql += " OrgStrucAllData('') AS OrgStrucAllData_1 ON B01_Person.OrgStrucID = OrgStrucAllData_1.OrgStrucID LEFT OUTER JOIN ";
            sql += " B01_Card ON B01_Person.PsnID = B01_Card.PsnID ";

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY PsnNo ";
            #endregion

            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);

            MainGridView.DataSource = dt;
            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();

            int find = 0;

            #region 取得RoleTable後的動作
            DataRow dr = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dr = dt.Rows[i];

                if (SelectPsnID.Value == dr["PsnID"].ToString())
                {
                    find = i;
                    break;
                }
            }
            #endregion

            return (find / _pagesize) + 1;
        }

        private void Query2()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = " SELECT B01_Card.*, B00_ItemList.ItemName FROM B01_Card INNER JOIN B00_ItemList ON B01_Card.CardType = B00_ItemList.ItemNo WHERE (B00_ItemList.ItemClass = 'CardType') AND PsnID = ? ;";
            liSqlPara.Add("I:" + SelectPsnID.Value);
            #endregion

            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            GirdViewDataBind(this.MainGridView2, dt);
            Label_PsnNo.Text = SelectPsnNo.Value;
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

        protected void GridView_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:
                    #region 設定欄位寛度
                    e.Row.Cells[1].Width = 125;
                    e.Row.Cells[2].Width = 125;
                    #endregion

                    #region 設定表頭欄位Visible狀態
                    //不想顯示的欄位，可在此處關掉

                    e.Row.Cells[0].Visible = false;

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
                    GrRow.ID = "GV_Row" + oRow.Row["PsnID"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    e.Row.Cells[1].Width = 129;
                    e.Row.Cells[2].Width = 129;
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 員工ID
                    e.Row.Cells[0].Visible = false;
                    #endregion
                    #region 編號
                    #endregion
                    #region 名稱
                    #endregion
                    #region 搜尋卡片
                    Button Bt_VCard = new Button();
                    Bt_VCard.Text = "搜尋";
                    Bt_VCard.CommandArgument = oRow.Row["PsnID"].ToString();
                    Bt_VCard.CommandName = e.Row.Cells[1].Text;
                    e.Row.Cells[3].Controls.Add(Bt_VCard);
                    e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Center;
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
                    e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text, 10, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectPsnNo,'" + oRow.Row["PsnNo"].ToString() + "', '', '');");
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
                                Query(true);
                            else
                                Query(false);
                        }
                    };

                    lbtnNext.Click += delegate(object obj, EventArgs args)
                    {
                        if (gv.PageIndex < gv.PageCount)
                        {
                            gv.PageIndex = gv.PageIndex + 1;
                            if (hSelectState.Value == "true")
                                Query(true);
                            else
                                Query(false);
                        }
                    };
                    #endregion
                    #region 首末頁
                    lbtnFirst.Click += delegate(object obj, EventArgs args)
                    {
                        gv.PageIndex = 0;
                        if (hSelectState.Value == "true")
                            Query(true);
                        else
                            Query(false);
                    };

                    lbtnLast.Click += delegate(object obj, EventArgs args)
                    {
                        gv.PageIndex = MainGridView.PageCount;
                        if (hSelectState.Value == "true")
                            Query(true);
                        else
                            Query(false);
                    };
                    #endregion
                    #endregion

                    #region 顯示總頁數及目前頁數
                    phdPageNumber.Controls.Add(
                        new LiteralControl(string.Format("　{0} / {1}　", pageIndex + 1, pageCount)));
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

        protected void GridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            SelectPsnID.Value = e.CommandArgument.ToString();
            Sa.Web.Fun.RunJavaScript(this, "GridSelect();");
            Query2();
        }
        
        protected void GridView_Data_DataBound2(object sender, EventArgs e)
        {
            td_showGridView2.Attributes["colspan"] = MainGridView2.Columns.Count.ToString();
        }

        protected void GridView_PageIndexChanging2(object sender, GridViewPageEventArgs e)
        {
            MainGridView2.PageIndex = e.NewPageIndex;
            if (MainGridView2.Rows[0].Cells.Count == MainGridView2.HeaderRow.Cells.Count)
                MainGridView2.DataBind();
        }

        protected void GridView_Data_RowDataBound2(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:
                    #region 設定欄位寛度
                    e.Row.Cells[1].Width = 125;
                    #endregion

                    #region 設定表頭欄位Visible狀態
                    //不想顯示的欄位，可在此處關掉

                    e.Row.Cells[0].Visible = false;

                    #endregion

                    #region 寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);
                    e.Row.CssClass = "GVStyle";
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    li_header2.Text = Header_sw.ToString();
                    #endregion
                    break;
                #endregion
                #region DataRow
                case DataControlRowType.DataRow:
                    DataRowView oRow = (DataRowView)e.Row.DataItem;

                    #region 指定Row的ID
                    GridViewRow GrRow = e.Row;
                    GrRow.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    GrRow.ID = "GV_Row" + oRow.Row["CardID"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    e.Row.Cells[1].Width = 129;
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 卡片ID
                    e.Row.Cells[0].Visible = false;
                    #endregion
                    #region 卡片類別
                    e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Center;
                    #endregion
                    #region 卡片號碼
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
                    e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text, 12, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectCardNo,'" + oRow.Row["CardNo"].ToString() + "', '', '');");
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

    }
}