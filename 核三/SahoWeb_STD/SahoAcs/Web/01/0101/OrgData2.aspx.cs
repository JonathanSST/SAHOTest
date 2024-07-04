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
using DapperDataObjectLib;


namespace SahoAcs
{
    public partial class OrgData2 : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;     //宣告Ajax元件
        private int _pagesize = 20;        //DataGridView每頁顯示的列數
        Hashtable hOrgName = null;
        public DataTable OrgClassData = new DataTable();
        #endregion

        #region 網頁前置作業

        #region LoadProcess
        private void LoadProcess()
        {
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(QueryButton);
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            oAcsDB.GetDataTable("OrgClass", "SELECT * FROM B00_ItemList WHERE ItemClass='OrgClass' AND ItemOrder>0", out this.OrgClassData);
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";            
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("OrgData", "OrgData.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "OrgClassData();", true);
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
            //AddButton.Attributes["onClick"] = "CallAdd('" + this.GetLocalResourceObject("CallAdd_Title") + "'); return false;";
            AddButton.Attributes["onClick"] = "SetAddRow();return false";
            EditButton.Attributes["onClick"] = "CallEdit('" + this.GetLocalResourceObject("CallEdit_Title") + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";
            DeleteButton.Attributes["onClick"] = "CallDelete('" + this.GetLocalResourceObject("CallDelete_Title") + "', '" +
                this.GetLocalResourceObject("CallDelete_DelLabel") + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForDelete") + "'); return false;";
            QueryButton.Attributes["onClick"] = "SelectState(); return false;";
            this.SaveButton.Attributes["onClick"] = "SetSave();return false";
            #endregion

            #region 註冊pop1頁Button動作
            ImgCloseButton1.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            popB_Add.Attributes["onClick"] = "AddExcute('" + hUserId.Value + "'); return false;";
            popB_Edit.Attributes["onClick"] = "EditExcute('" + hUserId.Value + "'); return false;";
            popB_Cancel.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            popInput_Class.Attributes["onchange"] = "ChangeTitle();";
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
            try
            {
                hUserId.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                //hMenuNo.Value = Sa.Web.Fun.GetSessionStr(this.Page, "MenuNo");
                //hMenuName.Value = Sa.Web.Fun.GetSessionStr(this.Page, "MenuName");
                LoadProcess();
                RegisterObj();
                GetOrgName();

                if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
                {
                    if (Request["PageEvent"] != null && Request["PageEvent"] == "SaveOrgData")
                    {
                        this.SetBatchInsert();
                    }
                    else
                    {
                        ViewState["SortExpression"] = "OrgNo";
                        ViewState["SortDire"] = "ASC";
                        hSelectState.Value = "true";
                        Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());                    
                    }                    
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
            catch(Exception ex)
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

        #region 取得各類別名稱
        private void GetOrgName()
        {
            hOrgName = new Hashtable();
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            String sql = @" SELECT ItemNo,ItemName FROM B00_ItemList WHERE ItemClass = 'OrgClass' AND ItemOrder > 0 ORDER BY ItemOrder ";
            oAcsDB.GetDataReader(sql, out dr);
            if (dr.HasRows)
            {
                while (dr.Read())
                    hOrgName.Add(dr.DataReader[0].ToString(), dr.DataReader[1].ToString());
            }
        }
        #endregion

        #endregion

        #region 批次新增組織資料
        private void SetBatchInsert()
        {
            //string[] checkOrg = Request.Form.GetValues("ChkOrg");
            if (Request["ChkOrg"] != null)
            {

                List<string> checkOrg = Request.Form.GetValues("ChkOrg").ToList();
                List<string> org_names = Request.Form.GetValues("NewOrgName").ToList();
                List<string> org_nos = Request.Form.GetValues("NewOrgNo").ToList();
                List<string> org_class_s = Request.Form.GetValues("NewOrgClass").ToList();
                foreach (var s in checkOrg)
                {
                    int key_index = int.Parse(s);
                    string name = org_names[key_index];
                    string no = org_nos[key_index];
                    string org_class = org_class_s[key_index];
                    OrmDataObject orm_data_obj = new OrmDataObject("MsSql"
                        , string.Format(Pub.db_connection_template, Pub.db_source_ip, Pub.db_data_name, Pub.db_user_id, Pub.db_pwd));
                    var query_orgs = orm_data_obj.GetQueryResult("SELECT * FROM B01_OrgData WHERE OrgNo=@OrgNo ",
                        new { OrgNo = string.Concat(org_class.Substring(0, 1), no) });
                    if (query_orgs.Count() == 0)
                    {
                        orm_data_obj.Execute(@"INSERT INTO B01_OrgData (OrgNo,OrgName,OrgClass,CreateUserID,CreateTime) 
                        VALUES (@OrgNo,@OrgName,@OrgClass,@CreateUser,GETDATE())",
                            new { OrgNo = string.Concat(org_class.Substring(0, 1), no), OrgName = name, OrgClass = org_class, CreateUser = this.hUserId.Value });
                    }
                }                
            }//end check request
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new { message = "Ok",success=true }));
            Response.End();
        }//end insert method

        #endregion


        #region 查詢
        private void Query(bool select_state, string SortExpression, string SortDire)
        {
            String sClass = SelectClassNo.Value;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<String> CheckData = new List<String>();
            List<string> liSqlPara = new List<string>();
            String Nowno = "", Nowname = "", NowClass = "";

            if (select_state)
            {
                CheckData.Add(this.Input_No.Text.Trim());
                CheckData.Add(this.Input_Name.Text.Trim());
                CheckData.Add(sClass);
                CatchSession(CheckData);
                Nowno = this.Input_No.Text.Trim();
                Nowname = this.Input_Name.Text.Trim();
                NowClass = sClass;
            }
            else
            {
                if (Session["OldSearchList"].ToString() != "" && Session["OldSearchList"].ToString() != null)
                {
                    String[] mgalist = Session["OldSearchList"].ToString().Split('|');
                    Nowno = mgalist[0];
                    Nowname = mgalist[1];
                    NowClass = mgalist[2];
                }
            }

            #region Process String

            sql = " SELECT * FROM B01_OrgData ";
            //wheresql += " (OwnerList LIKE '%\\" + hUserId.Value + "\\%') ";

            if (!string.IsNullOrEmpty(Nowno))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (OrgNo LIKE ? ) ";
                liSqlPara.Add("S:" + '%' + Nowno + '%');
            }

            if (!string.IsNullOrEmpty(Nowname))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (OrgName LIKE ? ) ";
                liSqlPara.Add("S:" + '%' + Nowname + '%');
            }

            if (NowClass != "")
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (OrgClass = ? ) ";
                liSqlPara.Add("S:" + NowClass);
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
            String Nowno = "", Nowname = "", NowClass = "";
            if (Session["OldSearchList"].ToString() != "" && Session["OldSearchList"].ToString() != null)
            {
                String[] mgalist = Session["OldSearchList"].ToString().Split('|');
                Nowno = mgalist[0];
                Nowname = mgalist[1];
                NowClass = mgalist[2];
            }

            #region Process String
            sql = " SELECT * FROM B01_OrgData ";
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
            MainGridView.DataSource = dt;
            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();

            int find = 0;

            #region 取得RoleTable後的動作
            DataRow dr = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dr = dt.Rows[i];

                if (SelectValue.Value == dr["OrgID"].ToString())
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
                if (string.IsNullOrEmpty(NoArray[1].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += "名稱 必須輸入";
                }
                if (string.IsNullOrEmpty(NoArray[2].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += "類別 必須選擇";
                }
            }
            if (mode == "Update")
            {
                if (string.IsNullOrEmpty(NoArray[0].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += "類別 必須選擇";
                }
                if (string.IsNullOrEmpty(NoArray[1].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += "編號 必須輸入";
                }
                if (NoArray[1].Contains(" "))
                {
                    sRet.result = false;
                    sRet.message += "編號字串中不可有空白符號";
                }
                if (string.IsNullOrEmpty(NoArray[2].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += "名稱 必須輸入";
                }
            }
            #endregion

            #region DB
            switch (mode)
            {
                case "Insert":
                    sql = @" SELECT * FROM B01_OrgData WHERE OrgNo = ? AND OrgClass = ? ";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    liSqlPara.Add("S:" + NoArray[2].Trim());
                    break;
                case "Update":
                    sql = @" SELECT * FROM B01_OrgData WHERE OrgNo = ? AND OrgNo <> ? AND OrgClass = ? ";
                    liSqlPara.Add("S:" + NoArray[0].Trim() + NoArray[1].Trim());
                    liSqlPara.Add("S:" + NoArray[2].Trim());
                    liSqlPara.Add("S:" + NoArray[3].Trim());
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
            Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }

        protected void GridView_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    e.Row.Cells[0].Width = 20;
                    e.Row.Cells[1].Width = 100;
                    e.Row.Cells[2].Width = 200;
                    e.Row.Cells[3].Width = 150;
                    #endregion

                    #region 設定表頭欄位Visible狀態
                    //不想顯示的欄位，可在此處關掉
                    //e.Row.Cells[0].Visible = true;
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
                    GrRow.ID = "GV_Row" + oRow.Row["OrgID"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    e.Row.Cells[0].Width = 23;
                    e.Row.Cells[1].Width = 103;
                    e.Row.Cells[2].Width = 204;
                    e.Row.Cells[3].Width = 154;
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 組織ID
                    //e.Row.Cells[0].Visible = false;
                    #endregion

                    #region 編號
                    #endregion

                    #region 名稱
                    #endregion

                    #region 類別
                    if (e.Row.Cells[3].Text != "&nbsp;")
                    {
                        e.Row.Cells[3].Text = hOrgName[e.Row.Cells[3].Text].ToString();
                        e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Center;
                    }
                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }

                    //e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 20, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["OrgID"].ToString() + "', '', '');");
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
        public static String[] DDLData(String CtrlID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            String sData = "";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            String sql = @" SELECT ItemNo,ItemName FROM B00_ItemList WHERE ItemClass = 'OrgClass' AND ItemOrder > 0 ORDER BY ItemOrder ";
            oAcsDB.GetDataReader(sql, out dr);
            if (dr.HasRows)
            {
                EditData = new string[dr.DataReader.FieldCount + 1];
                sData += CtrlID + "|";
                while (dr.Read())
                    sData += dr.DataReader[0].ToString() + "|" + dr.DataReader[1].ToString() + "|";
                EditData[EditData.Length - 1] = sData.Substring(0, sData.Length - 1);
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "系統中無此資料！";
            }

            return EditData;
        }
        #endregion

        #region 取得代碼 for 後台
        private static String GetOrgTitle(String sClass)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            String sql = @" SELECT ItemInfo2 FROM B00_ItemList WHERE ItemClass = 'OrgClass' AND ItemNo = '" + sClass + "' AND ItemOrder > 0 ORDER BY ItemOrder ";
            oAcsDB.GetDataReader(sql, out dr);
            sql = "";
            if (dr.HasRows)
            {
                while (dr.Read())
                    sql = dr.DataReader[0].ToString();
            }
            return sql;
        }
        #endregion

        #region 取得代碼 for 前台
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static String[] GetOrgTitle2(String sClass)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            String sql = @" SELECT ItemInfo2 FROM B00_ItemList WHERE ItemClass = 'OrgClass' AND ItemNo = '" + sClass + "' AND ItemOrder > 0 ORDER BY ItemOrder ";
            oAcsDB.GetDataReader(sql, out dr);
            if (dr.HasRows)
            {
                EditData = new string[dr.DataReader.FieldCount];
                while (dr.Read())
                    EditData[0] = dr.DataReader[0].ToString();
            }
            else
            {
                EditData = new string[1];
                EditData[0] = "";
            }
            return EditData;
        }
        #endregion

        #region Insert 寫入新資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Insert(string Org_No, string Org_Name, string Org_Class, String UserID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();
            String sOrgNo = GetOrgTitle(Org_Class.Trim()) + Org_No.Trim();
            string[] NoArray = new string[3];
            NoArray[0] = sOrgNo;
            NoArray[1] = Org_Name;
            NoArray[2] = Org_Class;
            sRet = Check_Input_DB(NoArray, "Insert");

            if (sRet.result)
            {
                #region Process String
                sql = @" INSERT INTO B01_OrgData(OrgClass ,OrgNo ,OrgName ,CreateUserID ,UpdateUserID ,UpdateTime) 
                         VALUES (?,?,?,?,?,GETDATE()) ";
                liSqlPara.Add("S:" + Org_Class.Trim());
                liSqlPara.Add("S:" + sOrgNo);
                liSqlPara.Add("S:" + Org_Name.Trim());
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
                        sql = " SELECT OrgID FROM B01_OrgData WHERE OrgNo = ? ";
                        liSqlPara.Clear();
                        liSqlPara.Add("S:" + sOrgNo);
                        #endregion

                        Sa.DB.DBReader oReader = null;
                        if (oAcsDB.GetDataReader(sql, liSqlPara, out oReader))
                        {
                            if (oReader.HasRows)
                            {
                                oReader.Read();
                                sRet.message = sOrgNo + "|" + oReader.DataReader["OrgID"].ToString();
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
        public static object Update(String Org_ID, String Old_Org_No, string New_Org_Title, string New_Org_No, string Org_Name, string Org_Class, String UserID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            int istat = 0;
            List<string> liSqlPara = new List<string>();

            string[] NoArray = new string[4];
            NoArray[0] = New_Org_Title;
            NoArray[1] = New_Org_No;
            NoArray[2] = Old_Org_No;
            NoArray[3] = Org_Class;

            sRet = Check_Input_DB(NoArray, "Update");
            if (sRet.result)
            {
                if (istat > -1)
                {
                    #region Process String
                    sql = @" UPDATE B01_OrgData SET OrgClass = ? ,OrgNo = ? ,OrgName = ? ,UpdateUserID = ? ,UpdateTime = GETDATE() WHERE OrgID = ? ; ";
                    liSqlPara.Add("S:" + Org_Class);
                    liSqlPara.Add("S:" + New_Org_Title.Trim() + New_Org_No.Trim());
                    liSqlPara.Add("S:" + Org_Name.Trim());
                    liSqlPara.Add("S:" + UserID);
                    liSqlPara.Add("S:" + Org_ID);

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
        public static object Delete(string Org_ID, string ChangeOrgID, String UserID)
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
                    sql = @"DELETE FROM B01_OrgData WHERE OrgID = ? ;";
                    sql += @"UPDATE B01_OrgStruc SET OrgIDList = REPLACE(OrgIDList,?,?), UpdateUserID = ?, UpdateTime = GETDATE();";
                    sql += @"UPDATE B01_OrgStruc SET OrgIDList = '', UpdateUserID = ?, UpdateTime = GETDATE() WHERE OrgIDList = '\' ;";
                    liSqlPara.Add("S:" + Org_ID.Trim());
                    liSqlPara.Add("S:\\" + Org_ID.Trim() + "\\");

                    if (ChangeOrgID != "")
                    {
                        liSqlPara.Add("S:\\" + ChangeOrgID.Trim() + "\\");
                    }
                    else
                    {
                        liSqlPara.Add("S:\\");
                    }

                    liSqlPara.Add("S:" + UserID.Trim());
                    liSqlPara.Add("S:" + UserID.Trim());
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
        public static string[] LoadData(String CtrlID, string Org_ID, String UserID, String mode)
        {
            Hashtable hOrgAllName = null;
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            String sql = @"SELECT ItemNo,ItemName FROM B00_ItemList WHERE ItemClass = 'OrgClass' AND ItemOrder > 0 ORDER BY ItemOrder";
            oAcsDB.GetDataReader(sql, out dr);
            if (dr.HasRows)
            {
                hOrgAllName = new Hashtable();
                while (dr.Read())
                    hOrgAllName.Add(dr.DataReader[0].ToString(), dr.DataReader[1].ToString());
            }

            Pub.MessageObject sRet = new Pub.MessageObject();
            sql = "";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            dr = null;
            bool OnceState = true;

            #region Process String
            sql = @" SELECT * FROM B01_OrgData WHERE OrgID = ? ";
            liSqlPara.Add("S:" + Org_ID.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                EditData = (mode == "Edit") ? new string[dr.DataReader.FieldCount] : new string[dr.DataReader.FieldCount + 1];
                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                    EditData[i] = dr.DataReader[i].ToString();
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
                sql = @" SELECT * FROM B01_OrgData WHERE OrgID <> ? AND OrgClass = ? ";
                liSqlPara.Add("S:" + Org_ID.Trim());
                liSqlPara.Add("S:" + EditData[1]);
                oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                String OrgDataList = CtrlID + "|";
                EditData[1] = hOrgAllName[EditData[1].ToString()].ToString();
                EditData[EditData.Length - 1] = OrgDataList.Substring(0, OrgDataList.Length - 1);
                if (dr.HasRows)
                {
                    while (dr.Read())
                        OrgDataList += dr.DataReader[0].ToString() + "|" + hOrgAllName[dr.DataReader[1].ToString()].ToString() + "|" + dr.DataReader[2].ToString() + "|";
                    EditData[EditData.Length - 1] = OrgDataList.Substring(0, OrgDataList.Length - 1);
                }
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
    }
}