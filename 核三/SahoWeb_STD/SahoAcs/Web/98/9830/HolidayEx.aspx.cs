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
    public partial class HolidayEx : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;     //宣告Ajax元件
        private int _pagesize = 200;        //DataGridView每頁顯示的列數
        private static string MsgDuplicate = "";
        #endregion

        #region 網頁前置作業

        #region LoadProcess
        private void LoadProcess()
        {
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(QueryButton);
            oScriptManager.RegisterAsyncPostBackControl(BulidButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("HolidayEx", "HolidayEx.js");//加入同一頁面所需的JavaScript檔案
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "YYData();", true);
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
            QueryButton.Attributes["onClick"] = "SelectState(); return false;";
            BulidButton.Attributes["onClick"] = "SelectState2(); return false;";
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

            //設定DataGridView每頁顯示的列數
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
                    ViewState["SortExpression"] = "HEDate";
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
                        else if (sFormArg == "Buliding")
                        {
                            this.MainGridView.PageIndex = 0;
                            hSelectState.Value = "true";
                            BulidHolidayEx();
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

        #region 產生假日資料
        private void BulidHolidayEx()
        {
            if (SelectYear.Value == "")
                return;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            List<string> liSqlPara = new List<string>();
            List<string> lis = new List<string>();
            String sYear = SelectYear.Value;
            String sYear2 = (int.Parse(sYear) + 1).ToString();
            DateTime startDate = DateTime.Parse(sYear + "-01-01 00:00:00");
            DateTime endDate = DateTime.Parse(sYear2 + "-01-01 00:00:00");

            string sql = "";
            int istat = 0;
            oAcsDB.BeginTransaction();
            while (startDate < endDate)
            {
                if ((int)startDate.DayOfWeek == 0)
                {
                    sql = " if not exists (select HEID from B00_HolidayEx where HEDate = ?) begin insert into B00_HolidayEx(HEDate,HEDesc,CreateUserID) values(?,?,?) end ";
                    liSqlPara.Add("S:" + startDate.ToString("yyyy-MM-dd"));
                    liSqlPara.Add("S:" + startDate.ToString("yyyy-MM-dd"));
                    liSqlPara.Add("S:" + "星期日");
                    liSqlPara.Add("S:" + hUserId.Value);
                }
                else if ((int)startDate.DayOfWeek == 6)
                {
                    sql = " if not exists (select HEID from B00_HolidayEx where HEDate = ?) begin insert into B00_HolidayEx(HEDate,HEDesc,CreateUserID) values(?,?,?) end ";
                    liSqlPara.Add("S:" + startDate.ToString("yyyy-MM-dd"));
                    liSqlPara.Add("S:" + startDate.ToString("yyyy-MM-dd"));
                    liSqlPara.Add("S:" + "星期六");
                    liSqlPara.Add("S:" + hUserId.Value);
                }
                else
                    sql = "";
                if (sql != "")
                {
                    istat += oAcsDB.SqlCommandExecute(sql, liSqlPara);
                    liSqlPara.Clear();
                }
                startDate = startDate.AddDays(1);
            }

            if (istat > -1)
                oAcsDB.Commit();
            else
                oAcsDB.Rollback();

            this.MainGridView.PageIndex = 0;
            hSelectState.Value = "true";
            Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
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
            sql = " SELECT * FROM B00_HolidayEx ";

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

            sql += wheresql + " ORDER BY HEDate ";
            #endregion

            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();
            hSelectState.Value = "false";
        }
        #endregion

        #region Insert 寫入新資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Insert(string sHEDate, string sHEDesc, String UserID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = " insert into B00_HolidayEx(HEDate,HEDesc,HEIsCus,CreateUserID) values(?,?,?,?) ";
            liSqlPara.Add("S:" + sHEDate);
            liSqlPara.Add("S:" + sHEDesc);
            liSqlPara.Add("S:" + "1");
            liSqlPara.Add("S:" + UserID);
            #endregion

            oAcsDB.BeginTransaction();
            int istat = oAcsDB.SqlCommandExecute(sql, liSqlPara);

            if (istat > -1)
            {
                if (!oAcsDB.Commit())
                {
                    sRet.result = false;
                    sRet.message = "新增失敗，資料庫異常！";
                }
                else
                {
                    sRet.message = sHEDate + "|" + sHEDate.Split('-')[0];
                }
            }
            else
            {
                oAcsDB.Rollback();
                sRet.result = false;
                sRet.message = "新增失敗，日期可能已存在！";
            }

            liSqlPara.Clear();
            sRet.act = "Add";
            return sRet;
        }
        #endregion

        #region Update 更新資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Update(String HEID, string HEDesc, String UserID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            int istat = 0;
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = @" UPDATE B00_HolidayEx SET HEDesc = ? ,UpdateUserID = ? ,UpdateTime = GETDATE() WHERE HEID = ? ; ";
            liSqlPara.Add("S:" + HEDesc);
            liSqlPara.Add("S:" + UserID);
            liSqlPara.Add("S:" + HEID);

            #endregion
            oAcsDB.BeginTransaction();
            istat = oAcsDB.SqlCommandExecute(sql, liSqlPara);

            if (istat > -1)
            {
                if (!oAcsDB.Commit())
                {
                    sRet.result = false;
                    sRet.message = "更新失敗，資料庫異常！";
                }
            }
            else
            {
                oAcsDB.Rollback();
                sRet.result = false;
                sRet.message = "更新失敗，資料異常！";
            }

            liSqlPara.Clear();
            sRet.act = "Edit";
            return sRet;
        }
        #endregion

        #region Delete 刪除資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Delete(string HEID, String UserID)
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
                    sql = @"DELETE FROM B00_HolidayEx WHERE HEID = ? ;";
                    liSqlPara.Add("S:" + HEID);
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

        #region 載入單筆資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadData(string HEID, String UserID, String mode)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            String sql = "";
            
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            dr = null;

            #region Process String
            sql = @" SELECT * FROM B00_HolidayEx WHERE HEID = ? ";
            liSqlPara.Add("S:" + HEID.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                EditData = new string[dr.DataReader.FieldCount];
                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                    EditData[i] = dr.DataReader[i].ToString();
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "系統中無此資料！";
            }

            #endregion

            return EditData;
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
            Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }

        protected void GridView_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    e.Row.Cells[0].Width = 100;
                    e.Row.Cells[1].Width = 100;
                    e.Row.Cells[2].Width = 300;
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
                    GrRow.ID = "GV_Row" + oRow.Row["HEID"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    e.Row.Cells[0].Width = 103;
                    e.Row.Cells[1].Width = 104;
                    e.Row.Cells[2].Width = 304;
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 例假日ID
                    e.Row.Cells[0].Visible = false;
                    #endregion

                    #region 編號
                    #endregion

                    #region 名稱
                    #endregion

                    #region 類別
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
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["HEID"].ToString() + "', '', '');");
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

                    //型別 'LinkButton' 的控制項 'ContentPlaceHolder1_MainGridView_lbtnFirst' 必須置於有 runat=server 的表單標記之中
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

        #region YYData
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static String[] YYData(String CtrlID)
        {
            //DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            //DBReader dr = null;
            //String sData = "";
            //string[] EditData = null;
            //List<string> liSqlPara = new List<string>();
            //String sql = @" SELECT ItemNo,ItemName FROM B00_ItemList WHERE ItemClass = 'OrgClass' AND ItemOrder > 0 ORDER BY ItemOrder ";
            //oAcsDB.GetDataReader(sql, out dr);
            //if (dr.HasRows)
            //{
            //    EditData = new string[dr.DataReader.FieldCount + 1];
            //    sData += CtrlID + "|";
            //    while (dr.Read())
            //        sData += dr.DataReader[0].ToString() + "|" + dr.DataReader[1].ToString() + "|";
            //    EditData[EditData.Length - 1] = sData.Substring(0, sData.Length - 1);
            //}
            //else
            //{
            //    EditData = new string[2];
            //    EditData[0] = "Saho_SysErrorMassage";
            //    EditData[1] = "系統中無此資料！";
            //}

            string[] EditData = null;
            String sData = "";
            EditData = new string[3];
            int iYear = int.Parse(DateTime.Now.ToString("yyyy"));
            sData += CtrlID + "|";
            for (int i = 0; i < 3; i++)
            {
                sData += (iYear).ToString() + "|" + (iYear).ToString() + "|";
                iYear++;
            }
            EditData[EditData.Length - 1] = sData.Substring(0, sData.Length - 1);
            return EditData;
        }
        #endregion

        #region MMData
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static String[] MMData(String CtrlID)
        {
            string[] EditData = null;
            String sData = "";
            EditData = new string[3];
            sData += CtrlID + "|";
            for (int i = 0; i < 12; i++)
            {
                sData += (i + 1).ToString().PadLeft(2, '0') + "|" + (i + 1).ToString().PadLeft(2, '0') + "|";
            }
            EditData[EditData.Length - 1] = sData.Substring(0, sData.Length - 1);
            return EditData;
        }
        #endregion

        #region DDData
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static String[] DDData(String CtrlID)
        {
            string[] EditData = null;
            String sData = "";
            EditData = new string[3];
            sData += CtrlID + "|";
            for (int i = 0; i < 31; i++)
            {
                sData += (i + 1).ToString().PadLeft(2, '0') + "|" + (i + 1).ToString().PadLeft(2, '0') + "|";
            }
            EditData[EditData.Length - 1] = sData.Substring(0, sData.Length - 1);
            return EditData;
        }
        #endregion

        #endregion


    }
}