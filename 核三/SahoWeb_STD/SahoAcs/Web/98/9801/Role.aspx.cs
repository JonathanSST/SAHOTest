using Sa.DB;
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
using SahoAcs.DBClass;
using System.Web.UI.WebControls;

namespace SahoAcs
{
    public partial class Role : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 20;
        Hashtable TableInfo;
        private static string MyUserID = "";
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js += "\nSetMode('');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("Role", "Role.js?"+Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案

            #region 註冊主頁Button動作
            QueryButton.Attributes["onClick"] = "SelectState();return false;";
            AddButton.Attributes["onClick"] = "CallAdd('" +this.GetLocalResourceObject("ttRoleAdd").ToString() + "'); return false;";
            EditButton.Attributes["onClick"] = "CallEdit('" + this.GetLocalResourceObject("ttRoleEdit").ToString() + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";
            DeleteButton.Attributes["onClick"] = "CallDelete('" + this.GetLocalResourceObject("ttRoleDel").ToString() + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForDelete") +"','"+
                this.GetLocalResourceObject("msgDelete") + "'); return false;";
            AuthButton.Attributes["onClick"] = "CallAuth('" + this.GetLocalResourceObject("ttRoleAuth").ToString() + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";
            #endregion

            #region 註冊pop1頁Button動作
            ImgCloseButton1.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            popB_Add.Attributes["onClick"] = "AddExcute(); return false;";
            popB_Edit.Attributes["onClick"] = "EditExcute(); return false;";
            popB_Delete.Attributes["onClick"] = "DeleteExcute(); return false;";
            popB_Cancel.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            #endregion

            #region 註冊pop2頁Button動作
            ImgCloseButton2.Attributes["onClick"] = "ReSetAuth(); CancelTrigger2.click(); return false;";
            popB_AuthAdd.Attributes["onClick"] = "AuthSaveExcute(); return false;";
            popB_AuthCancel.Attributes["onClick"] = "ReSetAuth(); CancelTrigger2.click(); return false;";
            #endregion

            Pub.SetModalPopup(ModalPopupExtender1, 1);
            Pub.SetModalPopup(ModalPopupExtender2, 2);

            this.MainGridView.PageSize = _pagesize;

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

            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                MyUserID = hideUserID.Value;
                #endregion

                ViewState["query_no"] = "";
                ViewState["query_name"] = "";
                ViewState["query_states"] = "";
                ViewState["SortExpression"] = "RoleNo";
                ViewState["SortDire"] = "ASC";
                Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == this.QueryButton.ClientID)
                {
                    ViewState["query_no"] = this.Input_No.Text.Trim();
                    ViewState["query_name"] = this.Input_Name.Text.Trim();
                    ViewState["query_states"] = this.Input_States.SelectedValue.ToString();
                }

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
                }
                else
                {
                    Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                }
            }

            CreateMenuAuthTable();
        }
        #endregion

        #region VerifyRenderingInServerForm
        public override void VerifyRenderingInServerForm(Control control)
        {
            // 修正 'XX'型別 必須置於有 runat=server 的表單標記之中 Override此Methods
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
            Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
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
                    int[] HeaderWidth = { 120, 120, 180, 230, 50 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    //#region 編號
                    //e.Row.Cells[0].Text = TableInfo["RoleNo"].ToString();
                    //#endregion
                    //#region 名稱
                    //e.Row.Cells[1].Text = TableInfo["RoleName"].ToString();
                    //#endregion
                    //#region 英文名稱
                    //e.Row.Cells[2].Text = TableInfo["RoleEName"].ToString();
                    //#endregion
                    //#region 描述
                    //e.Row.Cells[3].Text = TableInfo["RoleDesc"].ToString();
                    //#endregion
                    //#region 狀態
                    //e.Row.Cells[4].Text = TableInfo["RoleState"].ToString();
                    //#endregion
                    //#region 備註
                    //e.Row.Cells[5].Text = TableInfo["Remark"].ToString();
                    //#endregion

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
                    GrRow.ID = "GV_Row" + oRow.Row["RoleNo"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 123, 124, 184, 234, 54 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 編號
                    #endregion
                    #region 名稱
                    #endregion
                    #region 英文名稱
                    #endregion
                    #region 描述
                    #endregion
                    #region 狀態
                    if (oRow.Row["RoleState"].ToString() == "1")
                        e.Row.Cells[4].Text = this.GetLocalResourceObject("ttStateOpen").ToString();
                    else
                        e.Row.Cells[4].Text = this.GetLocalResourceObject("ttStateClose").ToString();
                    e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Center;
                    #endregion
                    #region 備註
                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }
                    e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 16, true);
                    e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text, 25, true);
                    e.Row.Cells[3].Text = LimitText(e.Row.Cells[3].Text, 31, true);
                    e.Row.Cells[5].Text = LimitText(e.Row.Cells[5].Text, 30, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["RoleNo"].ToString() + "', '', '');SetNowName('" + oRow.Row["RoleName"].ToString() + "')");
                    e.Row.Attributes.Add("OnDblclick", "CallEdit('" + this.GetLocalResourceObject("ttRoleEdit").ToString() + "')");
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
                    int startIndex = (pageIndex + 1 < showRange) ?
                        0 : (pageIndex + 1 + showRange / 2 >= pageCount) ? pageCount - showRange : pageIndex - showRange / 2;
                    int endIndex = (startIndex >= pageCount - showRange) ? pageCount : startIndex + showRange;
                    #region 頁數列
                    phdPageNumber.Controls.Add(new LiteralControl("  "));
                    #region 指定頁數及改變文字風格
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
                    #endregion

                    #region 上下頁
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

                    #region 首末頁
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

        #endregion

        #region Method

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

        #region Query
        public void Query(string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = " SELECT * FROM B00_SysRole  ";
            if (!string.IsNullOrEmpty(ViewState["query_no"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (RoleNo LIKE ?) ";
                liSqlPara.Add("S:" + "%" + ViewState["query_no"].ToString().Trim() + "%");
            }

            if (!string.IsNullOrEmpty(ViewState["query_name"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " ( RoleName LIKE ? OR RoleEName LIKE ? ) ";
                liSqlPara.Add("S:" + "%" + ViewState["query_name"].ToString().Trim() + "%");
                liSqlPara.Add("S:" + "%" + ViewState["query_name"].ToString().Trim() + "%");
            }

            if (!string.IsNullOrEmpty(ViewState["query_states"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (RoleState = ?) ";
                liSqlPara.Add("I:" + ViewState["query_states"].ToString().Trim());
            }

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("RoleTable", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();
        }
        #endregion

        #region Query(string mode)
        public int Query(string mode, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = " SELECT * FROM B00_SysRole ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("RoleTable", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();

            int find = 0;

            #region 取得RoleTable後的動作
            DataRow dr = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dr = dt.Rows[i];

                if (SelectValue.Value == dr["RoleNo"].ToString())
                {
                    find = i;
                    break;
                }
            }
            #endregion

            return (find / _pagesize) + 1;
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
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Process String
            sql = @" SELECT * FROM B00_SysRole WHERE RoleNo = ? ";
            liSqlPara.Add("S:" + SelectValue.Trim());
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

        #region Insert
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Insert(string UserID, string popInput_No, string popInput_Name, string popInput_EName, string popInput_Desc, string popInput_States, string popInput_Remark)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            string[] NoArray = new string[2];
            NoArray[0] = popInput_No;

            objRet = Check_Input_DB(NoArray, popInput_Name, popInput_EName, popInput_Desc, popInput_Remark, "Insert");
            if (objRet.result)
            {
                #region Process String
                sql = @" INSERT INTO B00_SysRole (RoleNo, RoleName, RoleEName, RoleDesc, RoleState, Remark, CreateUserID, UpdateUserID, UpdateTime) 
                         VALUES (?, ?, ?, ?, ?, ?, ?, ?, GETDATE())";
                liSqlPara.Add("S:" + popInput_No.Trim());
                liSqlPara.Add("S:" + popInput_Name.Trim());
                liSqlPara.Add("S:" + popInput_EName.Trim());
                liSqlPara.Add("S:" + popInput_Desc.Trim());
                liSqlPara.Add("S:" + popInput_States.Trim());
                liSqlPara.Add("S:" + popInput_Remark.Trim());
                liSqlPara.Add("S:" + UserID.ToString().Trim());
                liSqlPara.Add("S:" + UserID.ToString().Trim());
                //liSqlPara.Add("D:" + Time);
                #endregion
                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            objRet.act = "Add";
            return objRet;
        }
        #endregion

        #region Update
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Update(string UserID, string SelectValue, string popInput_No, string popInput_Name, string popInput_EName, string popInput_Desc, string popInput_States, string popInput_Remark)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            string[] NoArray = new string[2];
            NoArray[0] = popInput_No;
            NoArray[1] = SelectValue;

            objRet = Check_Input_DB(NoArray, popInput_Name, popInput_EName, popInput_Desc, popInput_Remark, "Update");
            #region 編輯角色資料
            if (objRet.result)
            {
                #region Process String - Updata Role
                sql = @" UPDATE B00_SysRole SET
                         RoleNo = ? , RoleName = ? ,
                         RoleEName = ? , RoleDesc = ? ,
                         RoleState = ? , Remark = ? ,
                         UpdateUserID = ? , UpdateTime = GETDATE()
                         WHERE RoleNo = ? ";
                liSqlPara.Add("S:" + popInput_No.Trim());
                liSqlPara.Add("S:" + popInput_Name.Trim());
                liSqlPara.Add("S:" + popInput_EName.Trim());
                liSqlPara.Add("S:" + popInput_Desc.Trim());
                liSqlPara.Add("S:" + popInput_States.Trim());
                liSqlPara.Add("S:" + popInput_Remark.Trim());
                liSqlPara.Add("S:" + UserID.ToString().Trim());
                //liSqlPara.Add("D:" + Time);
                liSqlPara.Add("S:" + SelectValue.Trim());
                #endregion

                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            #endregion

            objRet.act = "Edit";
            return objRet;
        }
        #endregion

        #region Delete
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Delete(string UserID, string SelectValue)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "", rid = "";
            DateTime Time = DateTime.Now;
            int iRet = 0;
            List<string> liSqlPara = new List<string>();

            oAcsDB.BeginTransaction();

            #region 取得RID
            sql = "SELECT RoleID FROM B00_SysRole WHERE RoleNo = ? ";
            liSqlPara.Add("S:" + SelectValue.Trim());
            Sa.DB.DBReader dr;
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (dr.Read())
            {
                rid = dr.DataReader["RoleID"].ToString();
            }
            sql = "";
            liSqlPara.Clear();
            #endregion

            #region 刪除角色及權限選單資料
            if (objRet.result)
            {
                if (iRet > -1)
                {
                    iRet = oAcsDB.SaveModifyBackupInfo(Time, UserID.ToString().Trim(), "B00_SysRoleMenus", "RoleID", rid.ToString(), false);
                }

                if (iRet > -1)
                {
                    #region Process String - Delete RoleRoleMenus
                    sql = @" DELETE FROM B00_SysRoleMenus 
                             WHERE RoleID = (SELECT RoleID FROM B00_SysRole WHERE RoleNo = ?) ";
                    liSqlPara.Add("S:" + SelectValue);
                    #endregion
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }

                liSqlPara.Clear();

                if (iRet > -1)
                {
                    #region Process String - Delete Role
                    sql = " DELETE FROM B00_SysRole WHERE RoleNo = ? ";
                    liSqlPara.Add("S:" + SelectValue);
                    #endregion
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }

                if (iRet > -1)
                    oAcsDB.Commit();
                else
                    oAcsDB.Rollback();
            }
            #endregion

            objRet.act = "Delete";
            return objRet;
        }
        #endregion

        #region Check_Input_DB
        protected static Pub.MessageObject Check_Input_DB(string[] NoArray, string Name, string EName, string Desc, string Remark, string mode)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Input
            if (string.IsNullOrEmpty(NoArray[0].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "代碼 必須輸入";
            }
            else if (NoArray[0].Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "代碼 字數超過上限";
            }

            if (Name.Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "名稱 字數超過上限";
            }

            if (EName.Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "英文名稱 字數超過上限";
            }

            if (Desc.Trim().Count() > 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "描述 字數超過上限";
            }

            if (Remark.Trim().Count() > 200)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "備註 字數超過上限";
            }

            #endregion
            #region DB
            switch (mode)
            {
                case "Insert":
                    sql = @" SELECT * FROM B00_SysRole WHERE RoleNo = ? ";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    break;
                case "Update":
                    sql = @" SELECT * FROM B00_SysRole WHERE RoleNo = ? AND RoleNo != ? ";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    liSqlPara.Add("S:" + NoArray[1].Trim());
                    break;
            }


            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "此代碼已存在於系統中";
            }
            #endregion

            return objRet;
        }
        #endregion

        #region CreateMenuAuthTable
        public void CreateMenuAuthTable()
        {
            TableRow tr;
            TableCell td;
            DataTable dt_Authtype;
            DataTable dt_SysMenu;
            int Authitemcount = 0;

            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();

            #region Process String - Get AuthType Count
            sql = @" SELECT ItemNo, ItemName 
                     FROM B00_ItemList 
                     WHERE ItemClass = 'MenuAuth'
                     ORDER BY ItemOrder ";
            #endregion

            oAcsDB.GetDataTable("AuthTypeContent", sql, out dt_Authtype);
            Authitemcount = dt_Authtype.Rows.Count;

            #region Header
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
            td.Text = this.GetLocalResourceObject("ttAuthItem").ToString();
            td.Attributes.Add("colspan", Authitemcount.ToString());
            td.Style.Add("background-color", "#24AC2F");
            td.Style.Add("color", "#FBFBFB");
            td.Style.Add("border-color", "#FBFBFB");
            td.HorizontalAlign = HorizontalAlign.Center;
            tr.Controls.Add(td);

            popTableHeader.Controls.Add(tr);

            tr = new TableRow();
            string i18n = Request.Cookies["i18n"].Value;
            for (int i = 0; i < dt_Authtype.Rows.Count; i++)
            {
                td = new TableCell();
                td.Text = i18n == "zh-TW" ? dt_Authtype.Rows[i]["ItemName"].ToString() : dt_Authtype.Rows[i]["ItemNo"].ToString();
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

            popTableHeader.Controls.Add(tr);

            popTableHeader.Attributes.Add("border", "1");
            #endregion

            #region Datarow

            #region Process String - Get SysMenu
            sql = @" SELECT MenuNo, MenuName, FunAuthDef
                     FROM B00_SysMenu 
                     WHERE IsAuthCtrl = '1' AND MenuIsUse='1'
                     ORDER BY MenuNo ";
            #endregion
            oAcsDB.GetDataTable("SysMenuInfo", sql, out dt_SysMenu);
            List<string> menu_list = this.GetMenuList();//.GetMenuList();
            foreach (DataRow dr_SysMenu in dt_SysMenu.Rows)
            {                
                if(menu_list.Count>0 && !menu_list.Contains(dr_SysMenu["MenuNo"].ToString()))
                {
                    continue;
                }
                tr = new TableRow();

                #region Title
                td = new TableCell();
                td.Text = dr_SysMenu["MenuName"].ToString();
                td.Width = 150;
                tr.Controls.Add(td);
                #endregion

                #region Control
                foreach (DataRow dr_Authtype in dt_Authtype.Rows)
                {                    
                    td = new TableCell();
                    td.HorizontalAlign = HorizontalAlign.Center;
                    System.Web.UI.WebControls.CheckBox popCheckBox = new System.Web.UI.WebControls.CheckBox();
                    popCheckBox.ID = dr_SysMenu["MenuNo"].ToString() + "_" + dr_Authtype["ItemNo"].ToString();
                    popCheckBox.Width = 25;
                    popCheckBox.Height = 25;
                    if (dr_SysMenu["FunAuthDef"].ToString().ToLower().Contains(dr_Authtype["ItemNo"].ToString().ToLower()))
                    {
                        popCheckBox.Enabled = true;
                        popCheckBox.Attributes.Add("OnClick", " CheckBoxSelected('" + popCheckBox.ClientID.ToString() + "');");
                        td.Attributes.Add("OnClick", " CheckBoxSelected('" + popCheckBox.ClientID.ToString() + "');");
                    }
                    else
                    {
                        popCheckBox.BackColor = System.Drawing.Color.Gray;
                        popCheckBox.Enabled = false;
                        popCheckBox.ToolTip = this.GetLocalResourceObject("ttNonAuth").ToString();
                        td.BackColor = System.Drawing.Color.Gray;
                    }

                    td.Controls.Add(popCheckBox);

                    td.Width = 80;
                    td.HorizontalAlign = HorizontalAlign.Center;
                    tr.Controls.Add(td);
                }

                #endregion

                tr.Height = 30;
                popAuthTable.Controls.Add(tr);
            }
            #endregion

            popAuthTable.Style.Add("word-break", "break-all");
            popAuthTable.Attributes.Add("border", "1");
        }
        #endregion

        #region LoadMenuAuthTable
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[,] LoadMenuAuthTable(string popInput_No)
        {
            string[,] AuthData = null;
            DataTable dt_RoleMenu;

            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();

            #region Process String - RoleMenus Info
            sql = @" SELECT RoleMenus.*,Role.RoleName FROM 
                     B00_SysRole AS Role 
                     INNER JOIN B00_SysRoleMenus AS RoleMenus ON RoleMenus.RoleID = Role.RoleID
                     INNER JOIN B00_SysMenu AS SysMenu ON RoleMenus.MenuNo=SysMenu.MenuNo AND IsAuthCtrl='1' AND MenuIsUse='1'
                     WHERE Role.RoleNo = ? ";
            liSqlPara.Add("S:" + popInput_No.Trim());
            oAcsDB.GetDataTable("RoleMenus", sql, liSqlPara, out dt_RoleMenu);
            List<string> menu_list = MyUserID.GetMenuList();
            AuthData = new string[dt_RoleMenu.Rows.Count, 2];                       
            foreach (DataRow dr in dt_RoleMenu.Rows)
            {
                if(menu_list.Count>0 && !menu_list.Contains(dr["MenuNo"].ToString()))
                {
                    AuthData[dt_RoleMenu.Rows.IndexOf(dr), 0] = "None";
                    AuthData[dt_RoleMenu.Rows.IndexOf(dr), 1] = "None";
                }
                else
                {
                    AuthData[dt_RoleMenu.Rows.IndexOf(dr), 0] = dr["MenuNo"].ToString();
                    AuthData[dt_RoleMenu.Rows.IndexOf(dr), 1] = dr["FunAuthSet"].ToString();
                }
                
            }
            if (dt_RoleMenu.Rows.Count == 0)
            {
                AuthData = new string[2, 2];
                AuthData[0, 0] = "Saho_SysErrorMassage";
                AuthData[0, 1] = "系統中無此資料！";                
            }
            #endregion

            return AuthData;
        }
        #endregion

        #region SaveMenuAuth
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object SaveMenuAuth(string UserID, string SelectValue, string[] Authid, string[] Authact, string[] Authchecked)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "", querysql = "", insertsql = "", updatesql = "", deletesql = "", rid = "";
            DateTime Time = DateTime.UtcNow;
            int iRet = 0;
            List<string> liSqlPara = new List<string>();
            List<string> liSqlPara2 = new List<string>();
            DataTable dt_SysMenu;
            DBReader DBdr;

            oAcsDB.BeginTransaction();

            #region 取得SysMenuInfo
            sql = @" SELECT MenuNo, MenuName, FunAuthDef
                     FROM B00_SysMenu 
                     WHERE IsAuthCtrl = '1' AND MenuIsUse='1'
                     ORDER BY MenuNo ";
            //oAcsDB.GetDataTable("SysMenuInfo", sql, out dt_SysMenu);
            DapperDataObjectLib.OrmDataObject odo = new DapperDataObjectLib.OrmDataObject("MsSql", Pub.GetDapperConnString());
            List<string> MenuList = UserID.GetMenuList();
            var result = odo.GetQueryResult(sql, new { Menus = MenuList });
            dt_SysMenu = new DataTable();
            dt_SysMenu.Columns.Add(new DataColumn("MenuNo"));
            dt_SysMenu.Columns.Add(new DataColumn("MenuName"));
            dt_SysMenu.Columns.Add(new DataColumn("FunAuthDef"));
            foreach (var o in result)
            {
                DataRow r = dt_SysMenu.NewRow();
                r["MenuNo"] = Convert.ToString(o.MenuNo);
                r["MenuName"] = Convert.ToString(o.MenuName);
                r["FunAuthDef"] = Convert.ToString(o.FunAuthDef);
                if (MenuList.Count == 0 || MenuList.Contains(Convert.ToString(o.MenuNo)))
                {
                    dt_SysMenu.Rows.Add(r);
                }                
            }
            #endregion
            
            string[] actqueue = null;
            string[] checkqueue = null;
            string sqlact = "";

            #region 取得RID
            sql = "SELECT RoleID FROM B00_SysRole WHERE RoleNo = ? ";
            liSqlPara.Add("S:" + SelectValue.Trim());
            Sa.DB.DBReader dr;
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (dr.Read())
            {
                rid = dr.DataReader["RoleID"].ToString();
            }
            sql = "";
            liSqlPara.Clear();
            #endregion           
            foreach (DataRow dr_SysMenu in dt_SysMenu.Rows)
            {

                if(MenuList.Count>0 && !MenuList.Contains(dr_SysMenu["MenuNo"].ToString()))
                {
                    continue;
                }
                querysql = @" SELECT RoleMenus.* FROM 
                              B00_SysRole AS Role 
                              Left Join B00_SysRoleMenus AS RoleMenus ON RoleMenus.RoleID = Role.RoleID 
                              WHERE Role.RoleNo = ? AND RoleMenus.MenuNo = ? ";
                liSqlPara.Add("S:" + SelectValue.Trim());
                liSqlPara.Add("S:" + dr_SysMenu["MenuNo"].ToString());
                
                oAcsDB.GetDataReader(querysql, liSqlPara, out DBdr);                

                if (Authchecked[dt_SysMenu.Rows.IndexOf(dr_SysMenu)].ToLower().Contains("true"))//如果該行有勾選任一權限,則進行新增或編輯
                {
                    actqueue = Authact[dt_SysMenu.Rows.IndexOf(dr_SysMenu)].Split(',');
                    checkqueue = Authchecked[dt_SysMenu.Rows.IndexOf(dr_SysMenu)].Split(',');
                    for (int i = 0; i < checkqueue.Length; i++)
                    {
                        if (checkqueue[i].ToString() == "true" && dr_SysMenu["FunAuthDef"].ToString().ToLower().Contains(actqueue[i].ToString().ToLower()))  //SysMenu中有該預設權限,且UI中有選取,則取出字串
                        {
                            if (sqlact == "") sqlact = actqueue[i].ToString();
                            else sqlact += "," + actqueue[i].ToString();
                        }
                    }
                    if (!DBdr.HasRows)  //尋找資料庫,若無則新增,有則編輯
                    {
                        #region Process String - INSERT RoleMenu
                        insertsql = @" INSERT INTO B00_SysRoleMenus (RoleID, MenuNo, FunAuthSet, CreateUserID, UpdateUserID, UpdateTime)
                                       VALUES (?, ?, ?, ?, ?, ?)";
                        liSqlPara2.Add("S:" + rid.ToString());
                        liSqlPara2.Add("S:" + dr_SysMenu["MenuNo"].ToString());
                        liSqlPara2.Add("S:" + sqlact);
                        liSqlPara2.Add("S:" + UserID.ToString().Trim());
                        liSqlPara2.Add("S:" + UserID.ToString().Trim());
                        liSqlPara2.Add("D:" + Time);
                        #endregion
                        sql += insertsql;
                    }
                    else
                    {
                        iRet = oAcsDB.SaveModifyBackupInfo(Time, UserID.ToString().Trim(), "B00_SysRoleMenus", "RoleID", rid.ToString(), "MenuNo", dr_SysMenu["MenuNo"].ToString(), true);
                        #region Process String - Update RoleMenu
                        updatesql = @" UPDATE B00_SysRoleMenus SET
                                       FunAuthSet = ? , 
                                       UpdateUserID = ? , UpdateTime = ?
                                       WHERE RoleID = ? AND MenuNo = ? ";
                        liSqlPara2.Add("S:" + sqlact);
                        liSqlPara2.Add("S:" + UserID.ToString().Trim());
                        liSqlPara2.Add("D:" + Time);
                        liSqlPara2.Add("S:" + rid.ToString());
                        liSqlPara2.Add("S:" + dr_SysMenu["MenuNo"].ToString());
                        #endregion
                        sql += updatesql;
                    }
                }
                else  //如果不勾選任一權限,則刪除該筆資料
                {
                    if (DBdr.HasRows)
                    {
                        iRet = oAcsDB.SaveModifyBackupInfo(Time, UserID.ToString().Trim(), "B00_SysRoleMenus", "RoleID", rid.ToString(), "MenuNo", dr_SysMenu["MenuNo"].ToString(), false);
                        #region Process String - Delete RoleMenu
                        deletesql = @" Delete FROM B00_SysRoleMenus WHERE RoleID = ? AND MenuNo = ? ";

                        liSqlPara2.Add("S:" + rid.ToString());
                        liSqlPara2.Add("S:" + dr_SysMenu["MenuNo"].ToString());
                        #endregion
                        sql += deletesql;
                    }
                }
                liSqlPara.Clear();
                sqlact = "";
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
                ProcessGridView.Rows[0].Cells[0].Text = "查無資料";
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

        #region RegisterStartupScript

        //        #region OpenDialog_Js
        //        private void OpenDialog_Js()
        //        {
        //            string jstr = "";

        //            jstr = @" 
        //                    function OpenDialogAdd(theURL,win_width,win_height) { 
        //                        var PosX = (screen.width-win_width)/2; 
        //                        var PosY = (screen.height-win_height)/2; 
        //                        features = 'dialogWidth='+win_width+',dialogHeight='+win_height+',dialogTop='+PosY+',dialogLeft='+PosX+';' 
        //                        window.showModalDialog(theURL, '', features);
        //                    }
        //
        //                    function OpenDialogEdit(theURL,key,win_width,win_height) { 
        //                        var PosX = (screen.width-win_width)/2; 
        //                        var PosY = (screen.height-win_height)/2; 
        //                        features = 'dialogWidth='+win_width+',dialogHeight='+win_height+',dialogTop='+PosY+',dialogLeft='+PosX+';' 
        //                        window.showModalDialog(theURL+key, '', features);
        //                    }";

        //            Page.ClientScript.RegisterStartupScript(this.GetType(), "OpenDialog_Js", jstr, true);
        //        }
        //        #endregion

        #endregion
    }
}