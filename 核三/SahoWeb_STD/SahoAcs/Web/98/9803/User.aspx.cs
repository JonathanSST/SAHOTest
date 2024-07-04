using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sa.DB;
using SahoAcs.DBClass;

namespace SahoAcs
{
    public partial class User : System.Web.UI.Page
    {
        #region 1.Main Description - r
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;   //宣告Ajax元件

        private int _pagesize = 20;   //DataGridView每頁顯示的資料列數

        private static string UserID = "";
        private static string OwnerList = "";
        private static string query_id = "", query_name = "", query_states = "";
        #endregion

        #region 2.RegisterStartupScript

        #region 2-1.OpenDialog_Js - r
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
        #endregion

        #endregion

        #region 3.Events

        #region 3-1.Page_Load - r
        protected void Page_Load(object sender, EventArgs e)
        {
            //取得登入者本身的OwnerList
            OwnerList = Session["OwnerList"].ToString().Trim();

            #region 3-1-1.LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js += "\nSetMode('');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("User", "User.js?"+Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案

            #region 註冊各頁Button動作
            //註冊主頁Button動作 - 主作業畫面
            QueryButton.Attributes["onClick"] = "SelectState(); return false;";
            AddButton.Attributes["onClick"] = "CallAdd('" + this.GetLocalResourceObject("ttUserAdd") + "'); return false;";
            EditButton.Attributes["onClick"] = "CallEdit('" + this.GetLocalResourceObject("ttUserEdit") + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";
            DeleteButton.Attributes["onClick"] = "CallDelete('" + this.GetLocalResourceObject("ttUserDel") + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForDelete") +"','"+
                this.GetLocalResourceObject("msgDelete") +
                "'); return false;";
            MenuButton.Attributes["onClick"] = "CallUserMenusAuth('" + this.GetLocalResourceObject("ttUserAuth") + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";
            RoleButton.Attributes["onClick"] = "CallUserRolesAuth('" + this.GetLocalResourceObject("lblUserRole") + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";
            MgnaButton.Attributes["onClick"] = "CallUserMgnsAuth('" + this.GetLocalResourceObject("lblUserMgn") + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";

            //註冊pop1頁Button動作 - 次作業畫面一：使用者資料新增與編輯
            ImgCloseButton1.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            popB_Add.Attributes["onClick"] = "AddExcute(); return false;";
            popB_Edit.Attributes["onClick"] = "EditExcute(); return false;";
            popB_Delete.Attributes["onClick"] = "DeleteExcute(); return false;";
            popB_Cancel.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";

            //註冊pop2頁Button動作 - 次作業畫面二：使用者功能清單權限設定
            ImgCloseButton2.Attributes["onClick"] = "ReSetUserMenusAuth(); CancelTrigger2.click(); return false;";
            popB_UserMenusAuthAdd.Attributes["onClick"] = "UserMenusAuthSaveExcute(); return false;";
            popB_UserMenusAuthCancel.Attributes["onClick"] = "ReSetUserMenusAuth(); CancelTrigger2.click(); return false;";

            //註冊pop3頁Button動作 - 次作業畫面三：使用者角色清單權限設定
            ImgCloseButton3.Attributes["onClick"] = "ReSetUserRolesAuth(); CancelTrigger3.click(); return false;";
            popB_UserRolesAuthAdd.Attributes["onClick"] = "UserRolesAuthSaveExcute(); return false;";
            popB_UserRolesAuthCancel.Attributes["onClick"] = "ReSetUserRolesAuth(); CancelTrigger3.click(); return false;";

            //註冊pop4頁Button動作 - 次作業畫面四：使用者管理區清單權限設定
            ImgCloseButton4.Attributes["onClick"] = "ReSetUserMgnsAuth(); CancelTrigger4.click(); return false;";
            popB_UserMgnsAuthAdd.Attributes["onClick"] = "UserMgnsAuthSaveExcute(); return false;";
            popB_UserMgnsAuthCancel.Attributes["onClick"] = "ReSetUserMgnsAuth(); CancelTrigger4.click(); return false;";

            Pub.SetModalPopup(ModalPopupExtender1, 1);   //次作業畫面一：使用者資料新增與編輯
            Pub.SetModalPopup(ModalPopupExtender2, 2);   //次作業畫面二：使用者功能清單設定
            Pub.SetModalPopup(ModalPopupExtender3, 3);   //次作業畫面三：使用者角色清單權限設定
            Pub.SetModalPopup(ModalPopupExtender4, 4);   //次作業畫面四：使用者管理區清單權限設定
            #endregion

            #region 語系切換
            //DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            //oAcsDB.GetTableHash("B00_SysRole", "zhtw", out TableInfo);
            //Label_No.Text = TableInfo["RoleNo"].ToString();
            //Label_Name.Text = TableInfo["RoleName"].ToString();
            //Label_States.Text = TableInfo["RoleState"].ToString();
            //popLabel_No.Text = TableInfo["RoleNo"].ToString();
            //popLabel_Name.Text = TableInfo["RoleName"].ToString();
            //popLabel_EName.Text = TableInfo["RoleEName"].ToString();
            //popLabel_States.Text = TableInfo["RoleState"].ToString();
            //popLabel_Desc.Text = TableInfo["RoleDesc"].ToString();
            //popLabel_Remark.Text = TableInfo["Remark"].ToString();
            #endregion

            this.MainGridView.PageSize = _pagesize;
            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                query_id = "";
                query_name = "";
                query_states = "";
                UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                ViewState["SortExpression"] = "UserID";
                ViewState["SortDire"] = "ASC";
                Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                this.popInput_STime.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd HH:mm:ss");
                this.popInput_ETime.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd HH:mm:ss");
                //this.popin
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == this.QueryButton.ClientID)
                {
                    query_id = this.Input_ID.Text.Trim();
                    query_name = this.Input_Name.Text.Trim();
                    query_states = this.Input_IsOptAllow.SelectedValue.ToString();
                }

                if (!string.IsNullOrEmpty(sFormArg))
                {
                    if (sFormArg == "popPagePost")                  //進行因應新增或編輯後所需的換頁動作
                    {
                        int find = Query("popPagePost", ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        Sa.Web.Fun.RunJavaScript(this, "__doPostBack('ctl00$ContentPlaceHolder1$MainGridView', 'Page$" + find + "');");
                    }
                    else if (sFormArg.Substring(0, 5) == "Page$")   //換頁完成後進行GridViewRow的移動
                    {
                        Query("popPagePost", ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        Sa.Web.Fun.RunJavaScript(this, "GridSelect();");
                    }
                }
                else
                {
                    Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                }
            }

            CreateUserMenusAuthTable();
            CreateUserRolesAuthTable();
            CreateUserMgnsAuthTable();
        }
        #endregion

        #region 3-2.VerifyRenderingInServerForm
        public override void VerifyRenderingInServerForm(Control control)
        {
            //修正'XX'型別必須置於含有runat=server的表單標記之中所以Override該Methods
        }
        #endregion

        #region 3-3.GridView_Data_DataBound - r
        protected void GridView_Data_DataBound(object sender, EventArgs e)
        {
            td_showGridView.Attributes["colspan"] = MainGridView.Columns.Count.ToString();
        }
        #endregion

        #region 3-4.GridView_PageIndexChanging - r
        protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            MainGridView.PageIndex = e.NewPageIndex;

            if (MainGridView.Rows[0].Cells.Count == MainGridView.HeaderRow.Cells.Count)
                MainGridView.DataBind();
        }
        #endregion

        #region 3-5.GridView_Sorting
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
            Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }
        #endregion

        #region 3-6.GridView_Data_RowDataBound
        public void GridView_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region 3-5-1.Header
                case DataControlRowType.Header:
                    #region 3-5-1-1.設定欄位寛度
                    e.Row.Cells[0].Width = 80;
                    e.Row.Cells[1].Width = 80;
                    e.Row.Cells[2].Width = 120;
                    e.Row.Cells[3].Width = 160;
                    e.Row.Cells[4].Width = 90;
                    e.Row.Cells[5].Width = 90;
                    e.Row.Cells[6].Width = 90;
                    e.Row.Cells[7].Width = 100;
                    e.Row.Cells[8].Width = 140;
                    #endregion

                    #region 3-5-1-2.排序條件Header加工
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

                    #region 3-5-1-3.寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);

                    e.Row.CssClass = "GVStyle";
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible  = false;

                    li_header.Text = Header_sw.ToString();
                    #endregion
                    break;
                #endregion

                #region 3-5-2.DataRow
                case DataControlRowType.DataRow:
                    DataRowView oRow = (DataRowView)e.Row.DataItem;

                    #region 3-5-2-1.指定Row的ID
                    GridViewRow GrRow = e.Row;

                    GrRow.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    GrRow.ID = "GV_Row" + oRow.Row["UserID"].ToString();
                    #endregion

                    #region 3-5-2-2.設定欄位寛度
                    e.Row.Cells[0].Width = 83;
                    e.Row.Cells[1].Width = 84;
                    e.Row.Cells[2].Width = 124;
                    e.Row.Cells[3].Width = 164;
                    e.Row.Cells[4].Width = 94;
                    e.Row.Cells[5].Width = 94;
                    e.Row.Cells[6].Width = 94;
                    e.Row.Cells[7].Width = 104;
                    e.Row.Cells[8].Width = 144;
                    #endregion

                    #region 3-5-2-3.針對各欄位做所需處理
                    #region 3-5-2-3-1.帳號
                    #endregion

                    #region 3-5-2-3-2.姓名
                    #endregion

                    #region 3-5-2-3-3.英文姓名
                    #endregion

                    #region 3-5-2-3-4.電子郵件信箱
                    #endregion

                    #region 3-5-2-3-5.操作權限
                    if (oRow.Row["IsOptAllow"].ToString() == "1")
                        e.Row.Cells[4].Text = "啟用";
                    else
                        e.Row.Cells[4].Text = "停用";

                    e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Center;
                    #endregion

                    #region 3-5-2-3-6.啟用時間
                    if (!String.IsNullOrEmpty(e.Row.Cells[5].Text.Replace("&nbsp;", "").Trim()))
                        e.Row.Cells[5].Text = DateTime.Parse(e.Row.Cells[5].Text).GetZoneTime(this).ToString("yyyy/MM/dd");
                    #endregion

                    #region 3-5-2-3-7.停用時間
                    if (!String.IsNullOrEmpty(e.Row.Cells[6].Text.Replace("&nbsp;", "").Trim()))
                        e.Row.Cells[6].Text = DateTime.Parse(e.Row.Cells[6].Text).GetZoneTime(this).ToString("yyyy/MM/dd");
                    #endregion

                    #region 3-5-2-3-8.密碼啟用時間
                    if (!String.IsNullOrEmpty(e.Row.Cells[7].Text.Replace("&nbsp;", "").Trim()))
                        e.Row.Cells[7].Text = DateTime.Parse(e.Row.Cells[7].Text).GetZoneTime(this).ToString("yyyy/MM/dd");
                    #endregion

                    #region 3-5-2-3-9.密碼控制
                    #endregion

                    #region 3-5-2-3-10.備註
                    #endregion
                    #endregion

                    #region 3-5-2-4.檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }

                    e.Row.Cells[0].Text = LimitText(e.Row.Cells[0].Text, 20);   //帳號
                    e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 14);   //姓名
                    e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text, 20);   //英文姓名
                    e.Row.Cells[3].Text = LimitText(e.Row.Cells[3].Text, 40);   //電子郵件信箱
                    e.Row.Cells[4].Text = LimitText(e.Row.Cells[4].Text, 2);    //操作權限
                    e.Row.Cells[5].Text = LimitText(e.Row.Cells[5].Text, 20);   //啟用時間
                    e.Row.Cells[6].Text = LimitText(e.Row.Cells[6].Text, 20);   //停用時間
                    e.Row.Cells[7].Text = LimitText(e.Row.Cells[7].Text, 20);   //密碼啟用時間
                    e.Row.Cells[8].Text = LimitText(e.Row.Cells[8].Text, 20);   //密碼控制
                    e.Row.Cells[9].Text = LimitText(e.Row.Cells[9].Text, 40);   //備註
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["UserID"].ToString() + "', '', '');SetNowName('" + oRow.Row["UserName"].ToString() + "')");
                    e.Row.Attributes.Add("OnDblclick", "CallEdit('" + this.GetLocalResourceObject("ttUserEdit") + "')");
                    break;
                #endregion

                #region 3-5-3.Pager
                case DataControlRowType.Pager:
                    #region 3-5-3-1.取得控制項
                    GridView gv = sender as GridView;

                    LinkButton lbtnFirst = e.Row.FindControl("lbtnFirst") as LinkButton;
                    LinkButton lbtnLast = e.Row.FindControl("lbtnLast") as LinkButton;
                    PlaceHolder phdPageNumber = e.Row.FindControl("phdPageNumber") as PlaceHolder;
                    LinkButton lbtnPrev = e.Row.FindControl("lbtnPrev") as LinkButton;
                    LinkButton lbtnNext = e.Row.FindControl("lbtnNext") as LinkButton;
                    LinkButton lbtnPage;
                    #endregion

                    #region 3-5-3-2.顯示頁數及[上下頁、首頁、末頁]處理
                    int showRange = 5;   //顯示快捷頁數
                    int pageCount = gv.PageCount;
                    int pageIndex = gv.PageIndex;
                    int startIndex = (pageIndex + 1 < showRange) ?
                        0 : (pageIndex + 1 + showRange / 2 >= pageCount) ? pageCount - showRange : pageIndex - showRange / 2;
                    int endIndex = (startIndex >= pageCount - showRange) ? pageCount : startIndex + showRange;

                    #region 3-5-3-2-1.頁數列
                    phdPageNumber.Controls.Add(new LiteralControl("  "));

                    #region 3-5-3-2-1-1.指定頁數及改變文字風格
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
                    #endregion

                    #region 3-5-3-2-2.上下頁
                    lbtnPrev.Click += delegate(object obj, EventArgs args)
                    {
                        if (gv.PageIndex > 0)
                        {
                            gv.PageIndex = gv.PageIndex - 1;
                            Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        }
                    };

                    lbtnNext.Click += delegate(object obj, EventArgs args)
                    {
                        if (gv.PageIndex < gv.PageCount)
                        {
                            gv.PageIndex = gv.PageIndex + 1;
                            Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        }
                    };
                    #endregion

                    #region 3-5-3-2-3.首末頁
                    lbtnFirst.Click += delegate(object obj, EventArgs args)
                    {
                        gv.PageIndex = 0;
                        Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    };

                    lbtnLast.Click += delegate(object obj, EventArgs args)
                    {
                        gv.PageIndex = MainGridView.PageCount;
                        Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    };
                    #endregion
                    #endregion

                    #region 3-5-3-3.顯示總頁數及目前頁數
                    phdPageNumber.Controls.Add(new LiteralControl(string.Format("　{0} / {1}　", pageIndex + 1, pageCount)));
                    phdPageNumber.Controls.Add(new LiteralControl("  "));
                    #endregion

                    #region 3-5-3-4.顯示總筆數
                    phdPageNumber.Controls.Add(
                        new LiteralControl(string.Format(this.GetGlobalResourceObject("Resource","lblGvCounts").ToString(), hDataRowCount.Value)));
                    phdPageNumber.Controls.Add(
                        new LiteralControl("  "));
                    #endregion

                    #region 3-5-3-5.寫入Literal_Pager
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

        #endregion

        #region 4.Method
        #region 4-1.LimitText - r
        public string LimitText(string str, int len)
        {
            if (str.Length > len)
            {
                return str.Substring(0, len) + "...";
            }
            else
            {
                return str;
            }
        }
        #endregion

        #region 4-2.GirdViewDataBind - r
        public void GirdViewDataBind(GridView ProcessGridView, DataTable dt)
        {
            if (dt.Rows.Count != 0)   //Gridview中有資料
            {
                ProcessGridView.DataSource = dt;
                ProcessGridView.DataBind();
            }
            else                      //Gridview中沒有資料
            {
                dt.Rows.Add(dt.NewRow());
                ProcessGridView.DataSource = dt;
                ProcessGridView.DataBind();

                int columnCount = ProcessGridView.Rows[0].Cells.Count;
                ProcessGridView.Rows[0].Cells.Clear();
                ProcessGridView.Rows[0].Cells.Add(new TableCell());
                ProcessGridView.Rows[0].Cells[0].ColumnSpan = columnCount;
                ProcessGridView.Rows[0].Cells[0].Text = "查無資料";
            }
        }
        #endregion

        #region 4-3.LoadData
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadData(string popInput_ID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();

            Sa.DB.DBReader dr;
            List<string> liSqlPara = new List<string>();

            string sql = "";
            string[] EditData = null;

            #region 4-3-1.Process String
            sql = @" SELECT RecordID, UserID, UserPW, UserName, UserEName, UserPWCtrl, " +
                "UserSTime,  UserETime, PWChgTime, IsOptAllow, EMail FROM B00_SysUser WHERE UserID = ? ";

            liSqlPara.Add("S:" + popInput_ID.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (dr.Read())
            {
                EditData = new string[dr.DataReader.FieldCount];
                var page = HttpContext.Current.Handler as Page;
                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                {
                    if (i == 7 || i == 6 || i==8)
                    {
                        EditData[i] = Convert.ToDateTime(dr.DataReader[i]).GetZoneTime(page).ToString("yyyy/MM/dd HH:mm:ss");
                    }
                    else
                    {
                        EditData[i] = dr.DataReader[i].ToString();
                    }                    
                }
                    
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

        #region 4-4.Check_Input_DB
        protected static Pub.MessageObject Check_Input_DB(string Mode, string[] NoArray, string PW, string Name, string EName, string STime, string ETime, string EMail, string Remark)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Sa.DB.DBReader dr;
            List<string> liSqlPara = new List<string>();
            Pub.MessageObject objRet = new Pub.MessageObject();
            //System.Text.RegularExpressions.Regex rexPwd = new System.Text.RegularExpressions.Regex(@"^(?=.*\d)(?=.*[a-zA-Z]).{6,18}$"); 
            string sql = "";

            #region 4-4-1.Input
            //1.檢查輸入帳號
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
            }

            //2.檢查輸入密碼
            if (string.IsNullOrEmpty(PW.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "密碼 必須輸入";
            }
            else if (PW.Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "密碼 字數超過上限";
            }
            //else if (!rexPwd.Match(PW.Trim()).Success)
            //{
            //    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
            //    objRet.result = false;
            //    objRet.message += "密碼 不符合密碼原則";
            //}

            //3.檢查輸入姓名
            if (Name.Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "姓名 字數超過上限";
            }

            //4.檢查輸入英文姓名
            if (EName.Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "英文姓名 字數超過上限";
            }

            //5.檢查輸入啟始時間
            if (string.IsNullOrEmpty(STime.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "啟始時間 必須輸入";
            }

            //6.檢查輸入結束時間
            if (string.IsNullOrEmpty(ETime.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "結束時間 必須輸入";
            }

            //7.檢查輸入電子郵件信箱
            if (EMail.Trim().Count() > 100)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "電子郵件信箱 字數超過上限";
            }

            //8.檢查輸入備註
            if (Remark.Trim().Count() > 200)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "備註 字數超過上限";
            }
            #endregion

            #region 4-4-2.DB
            //查詢只為驗證是否帳號存在盡量避免用*，以免浪費無謂的效能
            switch (Mode)
            {
                case "Insert":
                    sql = @" SELECT UserID FROM B00_SysUser WHERE UserID = ? 
                                     UNION  SELECT PsnAccount FROM B01_Person WHERE PsnAccount=? ";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    break;
                case "Update":
                    sql = @" SELECT UserID FROM B00_SysUser WHERE UserID = ? AND UserID <> ?
                                    UNION  SELECT PsnAccount FROM B01_Person WHERE PsnAccount=? ";
                    liSqlPara.Add("S:" + NoArray[0].Trim());        //新
                    liSqlPara.Add("S:" + NoArray[1].Trim());        //舊
                    liSqlPara.Add("S:" + NoArray[0].Trim());        //新
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
        #endregion

        #region 4-5.Query - r
        public void Query(string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            DataTable dt;
            List<string> liSqlPara = new List<string>();

            string sql = "", wheresql = "";

            #region 4-5-1.Process String
            sql = @" SELECT * 
                     FROM B00_SysUser ";

            if (!string.IsNullOrEmpty(OwnerList))
            {
                string qryOwnerList = OwnerList + UserID.Trim() + @"\";

                wheresql = " ( OwnerList LIKE ? ) ";
                liSqlPara.Add("S:" + "%" + qryOwnerList.Trim() + "%");
            }

            if (!string.IsNullOrEmpty(query_id))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " ( UserID LIKE ? ) ";
                liSqlPara.Add("S:" + "%" + query_id + "%");
            }

            if (!string.IsNullOrEmpty(query_name))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " ( UserName LIKE ? OR UserEName LIKE ? ) ";
                liSqlPara.Add("S:" + "%" + query_name + "%");
                liSqlPara.Add("S:" + "%" + query_name + "%");
            }

            if (!string.IsNullOrEmpty(query_states))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " ( IsOptAllow = ? ) ";
                liSqlPara.Add("I:" + query_states);
            }

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("UserTable", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            GirdViewDataBind(this.MainGridView, dt);
            this.MaxUser.Value = SahoAcs.DBClass.DongleVaries.GetMaxUser().ToString();
            this.CurrentUser.Value = SahoAcs.DBClass.DongleVaries.GetCurrentUser().ToString();            
            UpdatePanel1.Update();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "SetUserLevel", "SetUserLevel('" + Session["FunAuthSet"].ToString() + "')", true);
        }
        #endregion

        #region 4-6.Query(string mode)
        public int Query(string mode, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

            DataTable dt;
            List<string> liSqlPara = new List<string>();

            string sql = "", wheresql = "";
            this.MaxUser.Value = SahoAcs.DBClass.DongleVaries.GetMaxUser().ToString();
            this.CurrentUser.Value = SahoAcs.DBClass.DongleVaries.GetCurrentUser().ToString();
            #region 4-6-1.Process String
            sql = @" SELECT * 
                     FROM B00_SysUser ";

            if (!string.IsNullOrEmpty(OwnerList))
            {
                string qryOwnerList = OwnerList + UserID.Trim() + @"\";
                wheresql = " ( OwnerList LIKE ? ) ";
                liSqlPara.Add("S:" + "%" + qryOwnerList.Trim() + "%");
            }
            if (wheresql.Trim() != "")
            {
                sql += "WHERE ";
            }
            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;

            oAcsDB.GetDataTable("UserTable", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            #endregion

            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();

            int find = 0;

            #region 4-6-2.取得UserTable後的動作
            DataRow dr = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dr = dt.Rows[i];

                if (SelectValue.Value == dr["UserID"].ToString())
                {
                    find = i;
                    break;
                }
            }
            #endregion
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "SetUserLevel", "SetUserLevel('"+Session["FunAuthSet"].ToString()+"')", true);
            return (find / _pagesize) + 1;
        }
        #endregion

        

        #region 4-7.Insert
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Insert(string popInput_ID, string popInput_PW, string popInput_Name, string popInput_EName, string popInput_STime, string popInput_ETime, string popInput_PWChgTime, string popInput_IsOptAllow, string popInput_EMail, string popInput_Remark)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            List<string> liSqlPara = new List<string>();
            string sql = "";
            string[] NoArray = new string[2];

            NoArray[0] = popInput_ID;
            objRet = Check_Input_DB("Insert", NoArray, popInput_PW, popInput_Name, popInput_EName, popInput_STime, popInput_ETime, popInput_EMail, popInput_Remark);

            if (objRet.result)
            {
                //處理新增時資料表欄位OwnerID與OwnerList的關係
                string insOwnerID = "", insOwnerList = "";

                insOwnerID = UserID;
                if (string.IsNullOrEmpty(OwnerList))  //登入使用者帳號OwnerID="Admin"、OwnerList=""
                    insOwnerList = @"\" + UserID + @"\";
                else                                  //登入使用者帳號OwnerID!="Admin"
                    insOwnerList = OwnerList + UserID + @"\";

                #region 4-7-1.Process String
                sql = @" INSERT INTO B00_SysUser (UserID, UserPW, UserName, UserEName, UserSTime, UserETime, IsOptAllow,
                    EMail, Remark, OwnerID, OwnerList, CreateUserID) 
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                liSqlPara.Add("S:" + popInput_ID.Trim());
                liSqlPara.Add("S:" + popInput_PW.Trim());
                liSqlPara.Add("S:" + popInput_Name.Trim());
                liSqlPara.Add("S:" + popInput_EName.Trim());
                //UserPWCtrl欄位新增不用寫入
                var page = HttpContext.Current.Handler as Page;
                liSqlPara.Add("D:" + DateTime.Parse(popInput_STime.Trim()).GetUtcTime(page).ToString());
                liSqlPara.Add("D:" + DateTime.Parse(popInput_ETime.Trim()).GetUtcTime(page).ToString());
                //liSqlPara.Add("D:" + popInput_PWChgTime.Trim());
                liSqlPara.Add("S:" + popInput_IsOptAllow.Trim());
                liSqlPara.Add("S:" + popInput_EMail.Trim());
                liSqlPara.Add("S:" + popInput_Remark.Trim());
                liSqlPara.Add("S:" + insOwnerID);//OwnerID欄位
                liSqlPara.Add("S:" + insOwnerList);//OwnerList欄位
                liSqlPara.Add("S:" + UserID.Trim());//CreateUserID欄位
                //UpdateUserID欄位：新增不用寫入
                //UpdateTime欄位：新增不用寫入
                //Rev01欄位：保留不用寫入
                //Rev02欄位：保留不用寫入
                #endregion

                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }

            objRet.act = "Add";
            return objRet;
        }
        #endregion

        #region 4-8.Update
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Update(string SelectValue, string popInput_ID, string popInput_PW, string popInput_Name, string popInput_EName, string popInput_PWCtrl, string popInput_STime, string popInput_ETime, string popInput_PWChgTime, string popInput_IsOptAllow, string popInput_EMail, string popInput_Remark, string UpdateUserID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            List<string> liSqlPara = new List<string>();
            string sql = "";
            string[] NoArray = new string[2];
            NoArray[0] = popInput_ID;
            NoArray[1] = SelectValue;
            objRet = Check_Input_DB("Update", NoArray, popInput_PW, popInput_Name, popInput_EName, popInput_STime, popInput_ETime, popInput_EMail, popInput_Remark);

            #region 4-8-1.編輯使用者資料
            if (objRet.result)
            {
                #region Process String - Updata User
                sql = @" UPDATE B00_SysUser SET UserID=?, UserPW = ?, UserName = ?, UserEName = ? , UserPWCtrl = ? ,
                    UserSTime = ?, UserETime = ?, PWChgTime = ?, IsOptAllow = ? , EMail = ?, Remark = ?, UpdateUserID = ?,
                    UpdateTime = GETDATE() WHERE UserID = ? AND UserID<>'User' ";
                var page = HttpContext.Current.Handler as Page;
                liSqlPara.Add("S:" + popInput_ID.Trim());
                liSqlPara.Add("S:" + popInput_PW.Trim());
                liSqlPara.Add("S:" + popInput_Name.Trim());
                liSqlPara.Add("S:" + popInput_EName.Trim());
                liSqlPara.Add("S:" + popInput_PWCtrl.Trim());
                liSqlPara.Add("D:" + DateTime.Parse(popInput_STime.Trim()).GetUtcTime(page).ToString());
                liSqlPara.Add("D:" + DateTime.Parse(popInput_ETime.Trim()).GetUtcTime(page).ToString());
                liSqlPara.Add("D:" + DateTime.Parse(popInput_PWChgTime.Trim()).GetUtcTime(page).ToString());
                liSqlPara.Add("S:" + popInput_IsOptAllow.Trim());
                liSqlPara.Add("S:" + popInput_EMail.Trim());
                liSqlPara.Add("S:" + popInput_Remark.Trim());
                //GID欄位更新不用寫入
                //OwnerID欄位更新不用寫入
                //OwnerList欄位更新不用寫入
                //CreateUserID欄位更新不用寫入
                //CreateTime欄位更新不用寫入
                liSqlPara.Add("S:" + UserID.ToString());
                //CreateTime欄位系統自動寫入
                liSqlPara.Add("S:" + SelectValue.Trim());
                #endregion

                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            #endregion

            #region 4-8-2.編輯使用者功能選單資料
            liSqlPara.Clear();

            if (!SelectValue.Trim().Equals(popInput_ID.Trim()))
            {
                #region Process String - Updata UserMenus
                sql = @" UPDATE B00_SysUserMenus SET UserID = ?, UpdateUserID = ?, UpdateTime = GETDATE()
                         WHERE UserID = ? ";

                liSqlPara.Add("S:" + popInput_ID.Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("S:" + SelectValue.Trim());
                #endregion

                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            #endregion

            #region 4-8-3.編輯使用者管理區資料
            liSqlPara.Clear();

            if (!SelectValue.Trim().Equals(popInput_ID.Trim()))
            {
                #region Process String - Updata UserMgns
                sql = @" UPDATE B00_SysUserMgns SET UserID = ? WHERE UserID = ? ";

                liSqlPara.Add("S:" + popInput_ID.Trim());
                liSqlPara.Add("S:" + SelectValue.Trim());
                #endregion

                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            #endregion

            #region 4-8-4.編輯使用者角色資料
            liSqlPara.Clear();

            if (!SelectValue.Trim().Equals(popInput_ID.Trim()))
            {
                #region Process String - Updata UserRoles
                sql = @" UPDATE B00_SysUserRoles SET UserID = ? WHERE UserID = ? ";

                liSqlPara.Add("S:" + popInput_ID.Trim());
                liSqlPara.Add("S:" + SelectValue.Trim());
                #endregion

                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            #endregion

            objRet.act = "Edit";
            return objRet;
        }
        #endregion

        #region 4-9.Delete
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Delete(string SelectValue)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            List<string> liSqlPara = new List<string>();
            string sql = "";

            oAcsDB.BeginTransaction();

            #region 4-9-1.刪除使用者資料
            if (objRet.result)
            {
                #region Process String - Delete User
                sql = " DELETE FROM B00_SysUser WHERE UserID = ? AND UserID<>'User' ";

                liSqlPara.Add("S:" + SelectValue);
                #endregion

                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            #endregion

            liSqlPara.Clear();

            #region 4-9-2.刪除使用者選單資料
            liSqlPara.Clear();

            if (objRet.result)
            {
                #region Process String - Delete UserMemus
                sql = " DELETE FROM B00_SysUserMenus WHERE UserID = ? ";

                liSqlPara.Add("S:" + SelectValue);
                #endregion

                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            #endregion

            #region 4-9-3.刪除使用者管理區資料
            liSqlPara.Clear();

            if (objRet.result)
            {
                #region Process String - Delete UserMgns
                sql = " DELETE FROM B00_SysUserMgns WHERE UserID = ? ";

                liSqlPara.Add("S:" + SelectValue);
                #endregion

                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            #endregion

            #region 4-9-4.刪除使用者角色資料
            liSqlPara.Clear();

            if (objRet.result)
            {
                #region Process String - Delete UserRoles
                sql = " DELETE FROM B00_SysUserRoles WHERE UserID = ? ";

                liSqlPara.Add("S:" + SelectValue);
                #endregion

                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            #endregion

            oAcsDB.Commit();
            objRet.act = "Delete";
            return objRet;
        }
        #endregion

        #region 4-10.次作業畫面二：使用者功能清單權限設定視窗相關的方法
        /// <summary>
        /// 建立使用者功能清單權限設定表 - Rex
        /// </summary>
        public void CreateUserMenusAuthTable()
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();

            TableRow tr;
            TableCell td;
            DataTable dt_SysMenu;
            DataTable dt_Authtype;
            List<string> liSqlPara = new List<string>();

            int Authitemcount = 0;
            string sql = "";

            #region Process String - Get B00_ItemList AuthType Count
            sql = @" SELECT ItemNo, ItemName 
                     FROM B00_ItemList 
                     WHERE ItemClass = 'MenuAuth' 
                     ORDER BY ItemOrder ";

            oAcsDB.GetDataTable("AuthTypeTable", sql, out dt_Authtype);
            #endregion

            Authitemcount = dt_Authtype.Rows.Count;

            #region A.建立表格標頭
            //第一列部份
            tr = new TableRow();

            td = new TableCell();
            td.Text = this.GetLocalResourceObject("ttFuncName").ToString();
            td.Width = 160;
            td.Style.Add("white-space", "nowrap");
            td.Attributes.Add("rowspan", "2");
            td.Style.Add("background-color", "#016B0A");
            td.Style.Add("color", "#FBFBFB");
            td.Style.Add("border-color", "#FBFBFB");
            td.VerticalAlign = VerticalAlign.Middle;
            td.HorizontalAlign = HorizontalAlign.Center;

            tr.Controls.Add(td);

            td = new TableCell();
            td.Text = this.GetLocalResourceObject("popAdj").ToString();
            td.Width = 90;
            td.Style.Add("white-space", "nowrap");
            td.Attributes.Add("rowspan", "2");
            td.Style.Add("background-color", "#016B0A");
            td.Style.Add("color", "#FBFBFB");
            td.Style.Add("border-color", "#FBFBFB");
            td.VerticalAlign = VerticalAlign.Middle;
            td.HorizontalAlign = HorizontalAlign.Center;

            tr.Controls.Add(td);

            td = new TableCell();
            td.Text = this.GetLocalResourceObject("ttAuthItem").ToString();
            td.Attributes.Add("colspan", Authitemcount.ToString());
            td.Style.Add("background-color", "#24AC2F");
            td.Style.Add("color", "#FBFBFB");
            td.Style.Add("border-color", "#FBFBFB");
            td.HorizontalAlign = HorizontalAlign.Center;

            tr.Controls.Add(td);
            popUserMenusTableHeader.Controls.Add(tr);

            //第二列部份
            tr = new TableRow();

            for (int i = 0; i < dt_Authtype.Rows.Count; i++)
            {
                td = new TableCell();
                td.Text = Request.Cookies["i18n"].Value == "zh-TW" 
                    ? dt_Authtype.Rows[i]["ItemName"].ToString() : dt_Authtype.Rows[i]["ItemNo"].ToString();
                td.Style.Add("background-color", "#24AC2F");
                td.Style.Add("color", "#FBFBFB");
                td.Style.Add("border-color", "#FBFBFB");
                td.HorizontalAlign = HorizontalAlign.Center;

                if (i != dt_Authtype.Rows.Count - 1)
                {
                    td.Width = 90;
                }

                tr.Controls.Add(td);
            }

            popUserMenusTableHeader.Controls.Add(tr);
            popUserMenusTableHeader.Attributes.Add("border", "1");
            #endregion

            #region B.建立表格內容
            #region Process String - Get B00_SysMenu
            sql = @" SELECT MenuNo, MenuName, FunAuthDef
                     FROM B00_SysMenu 
                     WHERE IsAuthCtrl = '1' AND MenuIsUse='1'
                     ORDER BY MenuNo ";

            oAcsDB.GetDataTable("SysMenuTable", sql, out dt_SysMenu);
            #endregion
            List<string> MenuList = this.GetMenuList();
            foreach (DataRow dr_SysMenu in dt_SysMenu.Rows)
            {
                if(MenuList.Count>0 && !MenuList.Contains(dr_SysMenu["MenuNo"].ToString()))
                {
                    continue;
                }
                tr = new TableRow();

                #region B1.建立表格的選項名稱
                td = new TableCell();
                td.Width = 150;
                td.Text = dr_SysMenu["MenuName"].ToString();

                tr.Controls.Add(td);
                #endregion

                #region B2.建立表格的自訂選項
                td = new TableCell();
                td.Width = 80;
                td.HorizontalAlign = HorizontalAlign.Center;

                //建立權限的CheckBox元件，並設定代碼及勾選狀態
                System.Web.UI.WebControls.DropDownList popDropDownList = new System.Web.UI.WebControls.DropDownList();

                popDropDownList.ID = dr_SysMenu["MenuNo"].ToString() + "_" + "UserMenusOPMode";

                popDropDownList.Enabled = true;
                popDropDownList.Items.Add(this.GetLocalResourceObject("popAuthRed").ToString());
                popDropDownList.Items.Add(this.GetLocalResourceObject("popAuthAdd").ToString());
                popDropDownList.Items[0].Value = "-";
                popDropDownList.Items[1].Value = "+";
                popDropDownList.Attributes.Add("OnClick", "DropDownListSelected('" + popDropDownList.ClientID.ToString() + "', '*');");

                td.Attributes.Add("OnClick", "DropDownListSelected('" + popDropDownList.ClientID.ToString() + "', '*');");

                td.Controls.Add(popDropDownList);
                tr.Controls.Add(td);
                #endregion

                #region B3.建立表格的權項類別
                foreach (DataRow dr_Authtype in dt_Authtype.Rows)
                {
                    td = new TableCell();
                    td.Width = 80;
                    td.HorizontalAlign = HorizontalAlign.Center;

                    //建立權限的CheckBox元件，並設定代碼及勾選狀態
                    System.Web.UI.WebControls.CheckBox popCheckBox = new System.Web.UI.WebControls.CheckBox();

                    popCheckBox.ID = dr_SysMenu["MenuNo"].ToString() + "_" + dr_Authtype["ItemNo"].ToString();

                    if (dr_SysMenu["FunAuthDef"].ToString().ToLower().Contains(dr_Authtype["ItemNo"].ToString().ToLower()))
                    {
                        popCheckBox.Enabled = true;
                        popCheckBox.Attributes.Add("OnClick", "CheckBoxSelected('" + popCheckBox.ClientID.ToString() + "');");

                        td.Attributes.Add("OnClick", "CheckBoxSelected('" + popCheckBox.ClientID.ToString() + "');");
                    }
                    else
                    {
                        popCheckBox.Enabled = false;
                        popCheckBox.BackColor = System.Drawing.Color.Gray;                   
                        popCheckBox.ToolTip = "此項目無權限設定";
                        td.BackColor = System.Drawing.Color.Gray;
                    }

                    td.Controls.Add(popCheckBox);
                    tr.Controls.Add(td);
                }
                #endregion

                tr.Height = 30;
                popUserMenusAuthTable.Controls.Add(tr);
            }
            #endregion

            popUserMenusAuthTable.Style.Add("word-break", "break-all");
            popUserMenusAuthTable.Attributes.Add("border", "1");
        }

        /// <summary>
        /// 載入使用者功能清單權限設定表 - Rex2
        /// </summary>
        /// <param name="popInput_ID"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[,] LoadUserMenusAuthTable(string popInput_ID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            DataTable dt_UserMenus;
            List<string> liSqlPara = new List<string>();
            string sql = "";
            string[,] AuthData = null;

            #region Process String - Get B00_SysUserMenus
            sql = @" SELECT SysUserMenus.*,UserName FROM 
                     B00_SysUserMenus AS SysUserMenus 
                     INNER JOIN B00_SysMenu SysMenu ON SysMenu.MenuNo=SysUserMenus.MenuNo AND MenuIsUse='1' AND IsAuthCtrl='1'
                     LEFT JOIN B00_SysUser AS SysUser  ON SysUser.UserID = SysUserMenus.UserID                    
                     WHERE SysUser.UserID = ?";

            liSqlPara.Add("S:" + popInput_ID.Trim());

            oAcsDB.GetDataTable("UserMenusTable", sql, liSqlPara, out dt_UserMenus);
            #endregion

//            sql = @" SELECT SysRoleMenus.* , SysUserMenus.OPMode FROM 
//                     B00_SysRoleMenus AS SysRoleMenus 
//                     LEFT JOIN B00_SysUserRoles AS SysUserRoles ON SysUserRoles.RoleID = SysRoleMenus.RoleID
//                     LEFT JOIN B00_SysUserRoles AS SysUserRoles ON SysUserRoles.RoleID = SysRoleMenus.RoleID
//                     WHERE SysUserRoles.UserID = ? ";

            //oAcsDB.GetDataTable("RoleMenusTable", sql, liSqlPara, out dt_RoleMenus);

            if (dt_UserMenus.Rows.Count > 0)
            {
                AuthData = new string[dt_UserMenus.Rows.Count, 3];
                var MenuList = UserID.GetMenuList();
                foreach (DataRow dr_UserMenus in dt_UserMenus.Rows)
                {
                    if (MenuList.Count>0 && !MenuList.Contains(dr_UserMenus["MenuNo"].ToString()))
                    {
                        AuthData[dt_UserMenus.Rows.IndexOf(dr_UserMenus), 0] = "None";
                        AuthData[dt_UserMenus.Rows.IndexOf(dr_UserMenus), 1] = "None";
                        AuthData[dt_UserMenus.Rows.IndexOf(dr_UserMenus), 2] = "None";
                    }
                    else
                    {
                        AuthData[dt_UserMenus.Rows.IndexOf(dr_UserMenus), 0] = dr_UserMenus["MenuNo"].ToString();
                        AuthData[dt_UserMenus.Rows.IndexOf(dr_UserMenus), 1] = dr_UserMenus["FunAuthSet"].ToString();
                        AuthData[dt_UserMenus.Rows.IndexOf(dr_UserMenus), 2] = dr_UserMenus["OPMode"].ToString();                        
                    }
                    
                }
            }
            
            return AuthData;
        }

        /// <summary>
        /// 儲存使用者功能清單權限設定表 - Rex
        /// </summary>
        /// <param name="SelectValue"></param>
        /// <param name="Authid"></param>
        /// <param name="Authact"></param>
        /// <param name="Authchecked"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object SaveUserMenusAuth(string SelectValue, string[] Authid, string[] Authop, string[] Authact, string[] Authchecked)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string MyUserID = UserID;
            DBReader dbr_UserMenus;
            DataTable dt_SysMenu;
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            List<string> liSqlPara2 = new List<string>();

            int iRet = 0;
            string sql = "", querysql = "", insertsql = "", updatesql = "", deletesql = "", rid = "";

            oAcsDB.BeginTransaction();

            #region  Process String - Get B00_SysMenu
            sql = @" SELECT MenuNo , MenuName , FunAuthDef 
                     FROM B00_SysMenu 
                     WHERE IsAuthCtrl = '1' AND MenuIsUse='1'
                     ORDER BY MenuNo ";
            DapperDataObjectLib.OrmDataObject odo = new DapperDataObjectLib.OrmDataObject("MsSql", Pub.GetDapperConnString());
            var result = odo.GetQueryResult(sql, new {});
            var Menus = UserID.GetMenuList();
            //oAcsDB.GetDataTable("SysMenuTable", sql, out dt_SysMenu);
            dt_SysMenu = new DataTable();
            dt_SysMenu.Columns.Add(new DataColumn("MenuNo"));
            dt_SysMenu.Columns.Add(new DataColumn("MenuName"));
            dt_SysMenu.Columns.Add(new DataColumn("FunAuthDef"));
            foreach(var o in result)
            {
                DataRow dr = dt_SysMenu.NewRow();
                dr["MenuNo"] = Convert.ToString(o.MenuNo);
                dr["MenuName"] = Convert.ToString(o.MenuName);
                dr["FunAuthDef"] = Convert.ToString(o.FunAuthDef);
                if (Menus.Count == 0 || Menus.Contains(dr["MenuNo"].ToString()))
                {
                    dt_SysMenu.Rows.Add(dr);
                }
                
            }
            #endregion

            rid = SelectValue;

            int index = 0;
            string sqlact = "", opmode = "";
            string[] actqueue = null;
            string[] checkqueue = null;

            sql = "";
            liSqlPara.Clear();

            foreach (DataRow dr_SysMenu in dt_SysMenu.Rows)
            {
                
                #region Process String - Get B00_SysUserMenus
                querysql = @" SELECT SysUserMenus.* FROM 
                              B00_SysUserMenus AS SysUserMenus 
                              LEFT JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMenus.UserID 
                              WHERE SysUserMenus.UserID = ? AND SysUserMenus.MenuNo = ? ";

                liSqlPara.Add("S:" + SelectValue.Trim());
                liSqlPara.Add("S:" + dr_SysMenu["MenuNo"].ToString());

                oAcsDB.GetDataReader(querysql, liSqlPara, out dbr_UserMenus);
                #endregion

                index = dt_SysMenu.Rows.IndexOf(dr_SysMenu);  //取得指定資料列的索引值

                if (Authchecked[index].ToLower().Contains("true"))   //如果指定的資料列有勾選任一個權限，接著將進行該資料列的新增或編輯動作
                {
                    opmode = Authop[index];
                    actqueue = Authact[index].Split(',');
                    checkqueue = Authchecked[index].Split(',');

                    for (int i = 0; i < checkqueue.Length; i++)
                    {
                        //在B00_SysMenu資料表指定的dr_SysMenu資料列有該預設權限，並且在權限設定視窗有被選取時，將取得該資料列相關被勾選的權限設定字串，例如sqlact="Add,Edit,Del"
                        if (checkqueue[i].ToString() == "true" && dr_SysMenu["FunAuthDef"].ToString().ToLower().Contains(actqueue[i].ToString().ToLower()))
                        {
                            if (sqlact == "")
                                sqlact = actqueue[i].ToString();
                            else 
                                sqlact += "," + actqueue[i].ToString();
                        }
                    }

                    //在B00_SysMenu資料表找尋指定UserID與MenuNo條件的資料列，若無指定的資料列將進行新增動作、反之將進行更新動作
                    if (!dbr_UserMenus.HasRows)
                    {
                        //新增動作
                        #region Process String - INSERT B00_SysUserMenus
                        insertsql = @" INSERT INTO B00_SysUserMenus ( UserID , MenuNo , FunAuthSet , OPMode , CreateUserID , CreateTime , UpdateUserID , UpdateTime ) 
                                       VALUES ( ? , ? , ? , ? , ? , ? , ? , ? ) ";

                        liSqlPara2.Add("S:" + rid.ToString());
                        liSqlPara2.Add("S:" + dr_SysMenu["MenuNo"].ToString());
                        liSqlPara2.Add("S:" + sqlact);
                        liSqlPara2.Add("S:" + opmode);
                        liSqlPara2.Add("S:" + UserID.ToString());
                        liSqlPara2.Add("D:" + Time);
                        liSqlPara2.Add("S:" + UserID.ToString());
                        liSqlPara2.Add("D:" + Time);

                        sql += insertsql;
                        #endregion
                    }
                    else
                    {
                        //更新動作
                        //該資料列在更新之前將先儲存異動備份資料
                        iRet = oAcsDB.SaveModifyBackupInfo(Time, UserID.ToString(), "B00_SysUserMenus", "UserID", rid.ToString(), "MenuNo", dr_SysMenu["MenuNo"].ToString(), true);

                        #region Process String - Update B00_UserMenus
                        updatesql = @" UPDATE B00_SysUserMenus SET 
                                       FunAuthSet = ? , 
                                       OPMode = ? , 
                                       UpdateUserID = ? , 
                                       UpdateTime = ? 
                                       WHERE UserID = ? AND MenuNo = ? ";

                        liSqlPara2.Add("S:" + sqlact);
                        liSqlPara2.Add("S:" + opmode);
                        liSqlPara2.Add("S:" + UserID.ToString());
                        liSqlPara2.Add("D:" + Time);
                        liSqlPara2.Add("S:" + rid.ToString());
                        liSqlPara2.Add("S:" + dr_SysMenu["MenuNo"].ToString());

                        sql += updatesql;
                        #endregion
                    }
                }
                else                                                                               //如果指定的資料列未勾選任一個權限，接著將進行該資料列的刪除動作
                {
                    if (dbr_UserMenus.HasRows)
                    {
                        //刪除動作
                        //該資料列在刪除之前將先儲存異動備份資料
                        iRet = oAcsDB.SaveModifyBackupInfo(Time, UserID.ToString(), "B00_SysUserMenus", "UserID", rid.ToString(), "MenuNo", dr_SysMenu["MenuNo"].ToString(), false);

                        #region Process String - Delete RoleMenu
                        deletesql = @" DELETE FROM B00_SysUserMenus WHERE UserID = ? AND MenuNo = ? ";

                        liSqlPara2.Add("S:" + rid.ToString());
                        liSqlPara2.Add("S:" + dr_SysMenu["MenuNo"].ToString());
                        #endregion

                        sql += deletesql;
                    }
                }

                sqlact = "";
                liSqlPara.Clear();

                if (iRet <= -1)
                    break;
            }

            if (iRet > -1)
                iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara2);

            if (iRet > -1)
                oAcsDB.Commit();
            else
                oAcsDB.Rollback();

            objRet.act = "Auth";
            return objRet;
        }
        #endregion

        #region 4-11.次作業畫面三：使用者角色清單權限設定視窗相關的方法
        /// <summary>
        /// 建立使用者角色清單權限設定表 - Rex3
        /// </summary>
        public void CreateUserRolesAuthTable()
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();

            TableRow tr;
            TableCell td;
            DataTable dt_SysRole;
            List<string> liSqlPara = new List<string>();

            int Authitemcount = 1;
            string sql = "", roleName = "";

            #region A.建立表格標頭
            //第一列部份
            tr = new TableRow();

            td = new TableCell();
            td.Text = this.GetLocalResourceObject("popRoleName").ToString();
            td.Width = 160;
            td.Style.Add("white-space", "nowrap");
            td.Attributes.Add("rowspan", "2");
            td.Style.Add("background-color", "#016B0A");
            td.Style.Add("color", "#FBFBFB");
            td.Style.Add("border-color", "#FBFBFB");
            td.VerticalAlign = VerticalAlign.Middle;
            td.HorizontalAlign = HorizontalAlign.Center;

            tr.Controls.Add(td);

            td = new TableCell();
            td.Text = this.GetLocalResourceObject("popAuth").ToString();
            td.Attributes.Add("colspan", Authitemcount.ToString());
            td.Style.Add("background-color", "#24AC2F");
            td.Style.Add("color", "#FBFBFB");
            td.Style.Add("border-color", "#FBFBFB");
            td.HorizontalAlign = HorizontalAlign.Center;

            tr.Controls.Add(td);
            popUserRolesTableHeader.Controls.Add(tr);

            //第二列部份
            tr = new TableRow();

            td = new TableCell();
            td.Text = this.GetLocalResourceObject("popSelect").ToString();
            td.Width = 90;
            td.Style.Add("background-color", "#24AC2F");
            td.Style.Add("color", "#FBFBFB");
            td.Style.Add("border-color", "#FBFBFB");
            td.HorizontalAlign = HorizontalAlign.Center;

            tr.Controls.Add(td);
            popUserRolesTableHeader.Controls.Add(tr);
            popUserRolesTableHeader.Attributes.Add("border", "1");
            #endregion

            #region B.建立表格內容
            #region Process String - Get B00_SysRole
            sql = @" SELECT RoleID, RoleNo, RoleName 
                     FROM B00_SysRole 
                     WHERE RoleState = '1' 
                     ORDER BY RoleNo ";

            oAcsDB.GetDataTable("SysRoleTable", sql, out dt_SysRole);
            #endregion

            foreach (DataRow dr_SysRole in dt_SysRole.Rows)
            {
                tr = new TableRow();

                #region B1.建立表格的選項名稱
                td = new TableCell();
                td.Width = 150;
                td.Text = dr_SysRole["RoleNo"].ToString();

                roleName = dr_SysRole["RoleName"].ToString();
                if (!string.IsNullOrEmpty(roleName))
                    td.Text += "(" + roleName + ")";
                #endregion

                tr.Controls.Add(td);

                #region B2.建立表格的權限類別
                td = new TableCell();
                td.HorizontalAlign = HorizontalAlign.Center;

                //建立權限的CheckBox元件，並設定代碼及勾選狀態
                System.Web.UI.WebControls.CheckBox popCheckBox = new System.Web.UI.WebControls.CheckBox();

                popCheckBox.ID = dr_SysRole["RoleID"].ToString() + "_" + "UserRolesAuth";
                popCheckBox.Enabled = true;
                popCheckBox.Attributes.Add("OnClick", "CheckBoxSelected('" + popCheckBox.ClientID.ToString() + "');");

                td.Attributes.Add("OnClick", " CheckBoxSelected('" + popCheckBox.ClientID.ToString() + "');");
                td.Controls.Add(popCheckBox);
                #endregion

                tr.Height = 30;
                tr.Controls.Add(td);
                popUserRolesAuthTable.Controls.Add(tr);
            }
            #endregion

            popUserRolesAuthTable.Style.Add("word-break", "break-all");
            popUserRolesAuthTable.Attributes.Add("border", "1");
        }

        /// <summary>
        /// 載入使用者角色清單權限設定表 - Rex3
        /// </summary>
        /// <param name="popInput_ID"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[,] LoadUserRolesAuthTable(string popInput_ID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();

            DataTable dt_UserRoles;
            List<string> liSqlPara = new List<string>();

            string sql = "";
            string[,] AuthData = null;

            #region Process String - Get B00_UserRoles
            sql = @" SELECT SysUserRoles.*,UserName FROM 
                     B00_SysUserRoles AS SysUserRoles
                     LEFT JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserRoles.UserID 
                     WHERE SysUser.UserID = ? ";

            liSqlPara.Add("S:" + popInput_ID.Trim());

            oAcsDB.GetDataTable("UserRolesTable", sql, liSqlPara, out dt_UserRoles);
            #endregion

            if (dt_UserRoles.Rows.Count > 0)
            {
                AuthData = new string[dt_UserRoles.Rows.Count, 2];

                foreach (DataRow dr_UserRoles in dt_UserRoles.Rows)
                {
                    AuthData[dt_UserRoles.Rows.IndexOf(dr_UserRoles), 0] = dr_UserRoles["RoleID"].ToString();
                    AuthData[dt_UserRoles.Rows.IndexOf(dr_UserRoles), 1] = "UserRolesAuth";
                }
            }

            return AuthData;
        }

        /// <summary>
        /// 儲存使用者角色清單權限設定表 - Rex3
        /// </summary>
        /// <param name="SelectValue"></param>
        /// <param name="Authid"></param>
        /// <param name="Authact"></param>
        /// <param name="Authchecked"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object SaveUserRolesAuth(string SelectValue, string[] Authid, string[] Authact, string[] Authchecked)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();

            DBReader dbr_UserRoles;
            DataTable dt_Role;
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            List<string> liSqlPara2 = new List<string>();

            int iRet = 0;
            string sql = "", querysql = "", insertsql = "", deletesql = "", rid = "";

            oAcsDB.BeginTransaction();

            #region  Process String - Get B00_SysRole
            sql = @" SELECT RoleID, RoleNo, RoleName 
                     FROM B00_SysRole 
                     WHERE RoleState = '1' 
                     ORDER BY RoleNo ";

            oAcsDB.GetDataTable("SysRoleTable", sql, out dt_Role);
            #endregion

            rid = SelectValue;
            foreach (DataRow dr_Role in dt_Role.Rows)
            {
                #region Process String - Get B00_SysUserRoles
                querysql = @" SELECT SysUserRoles.* FROM 
                              B00_SysUserRoles AS SysUserRoles 
                              LEFT JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserRoles.UserID 
                              WHERE SysUserRoles.UserID = ? AND SysUserRoles.RoleID = ? ";

                liSqlPara.Add("S:" + SelectValue.Trim());
                liSqlPara.Add("S:" + dr_Role["RoleID"].ToString());

                oAcsDB.GetDataReader(querysql, liSqlPara, out dbr_UserRoles);
                #endregion

                if (Authchecked[dt_Role.Rows.IndexOf(dr_Role)].ToLower().Contains("true"))   //如果指定的資料列有勾選任一個權限，接著將進行該資料列的新增動作
                {
                    //在B00_SysUserRoles資料表找尋指定UserID與RoleID條件的資料列，若無指定的資料列將進行新增動作
                    if (!dbr_UserRoles.HasRows)
                    {
                        //新增動作
                        #region Process String - INSERT B00_SysUserRoles
                        insertsql = @" INSERT INTO B00_SysUserRoles ( UserID , RoleID , CreateUserID) 
                                       VALUES ( ? , ? , ? ) ";

                        liSqlPara2.Add("S:" + rid.ToString());
                        liSqlPara2.Add("S:" + dr_Role["RoleID"].ToString());
                        liSqlPara2.Add("S:" + UserID.ToString());

                        sql += insertsql;
                        #endregion
                    }
                }
                else                                                                               //如果指定的資料列未勾選任一個權限，接著將進行該資料列的刪除動作
                {
                    if (dbr_UserRoles.HasRows)
                    {
                        //刪除動作
                        //該資料列在刪除之前將先儲存異動備份資料
                        iRet = oAcsDB.SaveModifyBackupInfo(Time, UserID.ToString(), "B00_SysUserRoles", "UserID", rid.ToString(), "RoleID", dr_Role["RoleID"].ToString(), false);

                        #region Process String - Delete RoleMenu
                        deletesql = @" DELETE FROM B00_SysUserRoles WHERE UserID = ? AND RoleID = ? ";

                        liSqlPara2.Add("S:" + rid.ToString());
                        liSqlPara2.Add("S:" + dr_Role["RoleID"].ToString());

                        sql += deletesql;
                        #endregion
                    }
                }

                liSqlPara.Clear();

                if (iRet <= -1)
                    break;
            }

            if (iRet > -1)
                iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara2);

            if (iRet > -1)
                oAcsDB.Commit();
            else
                oAcsDB.Rollback();

            objRet.act = "Auth";
            return objRet;
        }
        #endregion

        #region 4-12.次作業畫面四：使用者管理區清單權限設定視窗相關的方法
        /// <summary>
        /// 建立使用者管理區清單權限設定表 - Rex4
        /// </summary>
        public void CreateUserMgnsAuthTable()
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();

            TableRow tr;
            TableCell td;
            DataTable dt_ManageArea;
            List<string> liSqlPara = new List<string>();

            int Authitemcount = 1;
            string sql = "", mgaName = "";

            #region A.建立表格標頭
            //第一列部份
            tr = new TableRow();

            td = new TableCell();
            td.Text = this.GetLocalResourceObject("popMgnName").ToString();
            td.Width = 160;
            td.Style.Add("white-space", "nowrap");
            td.Attributes.Add("rowspan", "2");
            td.Style.Add("background-color", "#016B0A");
            td.Style.Add("color", "#FBFBFB");
            td.Style.Add("border-color", "#FBFBFB");
            td.VerticalAlign = VerticalAlign.Middle;
            td.HorizontalAlign = HorizontalAlign.Center;

            tr.Controls.Add(td);

            td = new TableCell();
            td.Text = this.GetLocalResourceObject("popAuth").ToString();
            td.Attributes.Add("colspan", Authitemcount.ToString());
            td.Style.Add("background-color", "#24AC2F");
            td.Style.Add("color", "#FBFBFB");
            td.Style.Add("border-color", "#FBFBFB");
            td.HorizontalAlign = HorizontalAlign.Center;

            tr.Controls.Add(td);
            popUserMgnsTableHeader.Controls.Add(tr);

            //第二列部份
            tr = new TableRow();

            td = new TableCell();
            td.Text = this.GetLocalResourceObject("popSelect").ToString();
            td.Width = 90;
            td.Style.Add("background-color", "#24AC2F");
            td.Style.Add("color", "#FBFBFB");
            td.Style.Add("border-color", "#FBFBFB");
            td.HorizontalAlign = HorizontalAlign.Center;

            tr.Controls.Add(td);
            popUserMgnsTableHeader.Controls.Add(tr);
            popUserMgnsTableHeader.Attributes.Add("border", "1");
            #endregion

            #region B.建立表格內容
            #region Process String - Get B00_ManageArea
            sql = @" SELECT MgaID, MgaNo, MgaName 
                     FROM B00_ManageArea 
                     ORDER BY MgaNo ";

            oAcsDB.GetDataTable("ManageAreaTable", sql, out dt_ManageArea);
            #endregion

            foreach (DataRow dr_ManageArea in dt_ManageArea.Rows)
            {
                tr = new TableRow();

                #region B1.建立表格的選項名稱
                td = new TableCell();
                td.Width = 150;
                td.Text = dr_ManageArea["MgaNo"].ToString();

                mgaName = dr_ManageArea["MgaName"].ToString();
                if (!string.IsNullOrEmpty(mgaName))
                    td.Text += "(" + mgaName + ")";
                #endregion

                tr.Controls.Add(td);

                #region B2.建立表格的權限類別
                td = new TableCell();
                td.HorizontalAlign = HorizontalAlign.Center;

                //建立權限的CheckBox元件，並設定代碼及勾選狀態
                System.Web.UI.WebControls.CheckBox popCheckBox = new System.Web.UI.WebControls.CheckBox();

                popCheckBox.ID = dr_ManageArea["MgaID"].ToString() + "_" + "UserMgnsAuth";
                popCheckBox.Enabled = true;
                popCheckBox.Attributes.Add("OnClick", "CheckBoxSelected('" + popCheckBox.ClientID.ToString() + "');");

                td.Attributes.Add("OnClick", " CheckBoxSelected('" + popCheckBox.ClientID.ToString() + "');");
                td.Controls.Add(popCheckBox);
                #endregion

                tr.Height = 30;
                tr.Controls.Add(td);
                popUserMgnsAuthTable.Controls.Add(tr);
            }
            #endregion

            popUserMgnsAuthTable.Style.Add("word-break", "break-all");
            popUserMgnsAuthTable.Attributes.Add("border", "1");
        }

        /// <summary>
        /// 載入使用者管理區清單權限設定表 - Rex4
        /// </summary>
        /// <param name="popInput_ID"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[,] LoadUserMgnsAuthTable(string popInput_ID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();

            DataTable dt_UserMgns;
            List<string> liSqlPara = new List<string>();

            string sql = "";
            string[,] AuthData = null;

            #region Process String - Get B00_UserMgns
            sql = @" SELECT SysUserMgns.*,UserName FROM 
                     B00_SysUserMgns AS SysUserMgns
                     LEFT JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID 
                     WHERE SysUser.UserID = ? ";

            liSqlPara.Add("S:" + popInput_ID.Trim());

            oAcsDB.GetDataTable("UserMgnsTable", sql, liSqlPara, out dt_UserMgns);
            #endregion

            if (dt_UserMgns.Rows.Count > 0)
            {
                AuthData = new string[dt_UserMgns.Rows.Count, 2];

                foreach (DataRow dr_UserMgns in dt_UserMgns.Rows)
                {
                    AuthData[dt_UserMgns.Rows.IndexOf(dr_UserMgns), 0] = dr_UserMgns["MgaID"].ToString();
                    AuthData[dt_UserMgns.Rows.IndexOf(dr_UserMgns), 1] = "UserMgnsAuth";
                }
            }

            return AuthData;
        }

        /// <summary>
        /// 儲存使用者管理區清單權限設定表 - Rex4
        /// </summary>
        /// <param name="SelectValue"></param>
        /// <param name="Authid"></param>
        /// <param name="Authact"></param>
        /// <param name="Authchecked"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object SaveUserMgnsAuth(string SelectValue, string[] Authid, string[] Authact, string[] Authchecked)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();

            DBReader dbr_UserMgns;
            DataTable dt_ManageArea;
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            List<string> liSqlPara2 = new List<string>();

            int iRet = 0;
            string sql = "", querysql = "", insertsql = "", deletesql = "", rid = "";

            oAcsDB.BeginTransaction();

            #region  Process String - Get B00_ManageArea
            sql = @" SELECT MgaID, MgaNo, MgaName 
                     FROM B00_ManageArea 
                     ORDER BY MgaNo ";

            oAcsDB.GetDataTable("ManageAreaTable", sql, out dt_ManageArea);
            #endregion

            rid = SelectValue;
            foreach (DataRow dr_ManageArea in dt_ManageArea.Rows)
            {
                #region Process String - Get B00_SysUserMgns
                querysql = @" SELECT SysUserMgns.* FROM 
                              B00_SysUserMgns AS SysUserMgns 
                              LEFT JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID 
                              WHERE SysUserMgns.UserID = ? AND SysUserMgns.MgaID = ? ";

                liSqlPara.Add("S:" + SelectValue.Trim());
                liSqlPara.Add("S:" + dr_ManageArea["MgaID"].ToString());

                oAcsDB.GetDataReader(querysql, liSqlPara, out dbr_UserMgns);
                #endregion

                if (Authchecked[dt_ManageArea.Rows.IndexOf(dr_ManageArea)].ToLower().Contains("true"))   //如果指定的資料列有勾選任一個權限，接著將進行該資料列的新增動作
                {
                    //在B00_SysUserMgns資料表找尋指定UserID與MgaID條件的資料列，若無指定的資料列將進行新增動作
                    if (!dbr_UserMgns.HasRows)
                    {
                        //新增動作
                        #region Process String - INSERT B00_SysUserMgns
                        insertsql = @" INSERT INTO B00_SysUserMgns (UserID , MgaID , CreateUserID) 
                                       VALUES (?, ?, ?) ";

                        liSqlPara2.Add("S:" + rid.ToString());
                        liSqlPara2.Add("S:" + dr_ManageArea["MgaID"].ToString());
                        liSqlPara2.Add("S:" + UserID.ToString());

                        sql += insertsql;
                        #endregion
                    }
                }
                else                                                                               //如果指定的資料列未勾選任一個權限，接著將進行該資料列的刪除動作
                {
                    if (dbr_UserMgns.HasRows)
                    {
                        //刪除動作
                        //該資料列在刪除之前將先儲存異動備份資料
                        iRet = oAcsDB.SaveModifyBackupInfo(Time, UserID.ToString(), "B00_SysUserMgns", "UserID", rid.ToString(), "MgaID", dr_ManageArea["MgaID"].ToString(), false);

                        #region Process String - Delete RoleMenu
                        deletesql = @" DELETE FROM B00_SysUserMgns WHERE UserID = ? AND MgaID = ? ";

                        liSqlPara2.Add("S:" + rid.ToString());
                        liSqlPara2.Add("S:" + dr_ManageArea["MgaID"].ToString());

                        sql += deletesql;
                        #endregion
                    }
                }

                liSqlPara.Clear();

                if (iRet <= -1)
                    break;
            }

            if (iRet > -1)
                iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara2);

            if (iRet > -1)
                oAcsDB.Commit();
            else
                oAcsDB.Rollback();

            objRet.act = "Auth";
            return objRet;
        }
        #endregion

        #region 4-13.排序欄位及條件
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
    }
}