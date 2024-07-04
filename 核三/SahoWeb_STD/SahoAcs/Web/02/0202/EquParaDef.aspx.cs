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
    public partial class EquParaDef : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 20;
        Hashtable TableInfo;
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.Input_EquType);
            oScriptManager.RegisterAsyncPostBackControl(this.Input_EquModel);
            

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("EquParaDef", "EquParaDef.js");//加入同一頁面所需的JavaScript檔案

            #region 註冊主頁Button動作
            Input_EquModel.Attributes["onChange"] = "SelectState('" + this.Input_EquModel .ClientID+ "');";
            AddButton.Attributes["onClick"] = "CallAdd('" + GetGlobalResourceObject("ResourceEquData", "EquParaDefineAdd").ToString() +
                "','設備型號 必須指定'); return false;";
            EditButton.Attributes["onClick"] = "CallEdit('" + GetGlobalResourceObject("ResourceEquData", "EquParaDefineEdit").ToString() + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";
            DeleteButton.Attributes["onClick"] = "CallDelete('" + "設備參數定義資料刪除" + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForDelete") + "'); return false;";
            #endregion

            #region 註冊pop1頁Button動作
            ImgCloseButton1.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            popInput_InputType.Attributes["onchange"] = "InputTypeChange(popInput_InputType);";
            popB_Add.Attributes["onClick"] = "AddExcute(); return false;";
            popB_Edit.Attributes["onClick"] = "EditExcute(); return false;";
            popB_Delete.Attributes["onClick"] = "DeleteExcute(); return false;";
            popB_Cancel.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            #endregion

            div_Value.Style.Add("display", "none");
            div_Option.Style.Add("display", "none");
            div_URL.Style.Add("display", "none");
            Pub.SetModalPopup(ModalPopupExtender1, 1);
            this.MainGridView.PageSize = _pagesize;

            #region 語系切換
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            oAcsDB.GetTableHash("B01_EquParaDef", "zhtw", out TableInfo);
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
                #endregion

                CreateParentDropItem();
                ViewState["query_EquModel"] = this.Input_EquModel.SelectedValue;
                ViewState["SortExpression"] = "ParaName";
                ViewState["SortDire"] = "ASC";
                Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == this.Input_EquModel.ClientID)
                {
                    ViewState["query_EquModel"] = this.Input_EquModel.SelectedValue;
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
                    int[] HeaderWidth = { 150, 200, 100 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 參數名稱
                    #endregion
                    #region 參數描述
                    #endregion
                    #region 輸入方式
                    #endregion
                    #region 參數定義
                    #endregion

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
                    GrRow.ID = "GV_Row" + oRow.Row["ParaName"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 153, 204, 104 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 參數名稱
                    #endregion
                    #region 參數描述
                    #endregion
                    #region 輸入方式
                    switch (oRow.Row["InputType"].ToString())
                    {
                        case "0":
                            e.Row.Cells[2].Text =this.GetGlobalResourceObject("ResourceEquData","ParaTextField").ToString();// "文字欄位";
                            break;
                        case "1":
                            e.Row.Cells[2].Text = this.GetGlobalResourceObject("ResourceEquData","ParaNumField").ToString();//"數值範圍";
                            break;
                        case "2":
                            e.Row.Cells[2].Text = this.GetGlobalResourceObject("ResourceEquData","ParaItemSelect").ToString();//"清單選項";
                            break;
                        case "3":
                            e.Row.Cells[2].Text = this.GetGlobalResourceObject("ResourceEquData","ParaRefLink").ToString();//"參考連結";
                            break;
                    }
                    //e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Center;
                    #endregion
                    #region 參數定義
                    switch (oRow.Row["InputType"].ToString())
                    {
                        case "0":
                            break;
                        case "1":
                            e.Row.Cells[3].Text = "最小數值：" + oRow.Row["MinValue"].ToString() + " ; 最大數值：" + oRow.Row["MaxValue"].ToString();
                            break;
                        case "2":
                            e.Row.Cells[3].Text = oRow.Row["ValueOptions"].ToString();
                            break;
                        case "3":
                            e.Row.Cells[3].Text = oRow.Row["EditFormURL"].ToString();
                            break;
                    }
                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }
                    e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text.Trim(), 27, true);
                    e.Row.Cells[3].Text = LimitText(e.Row.Cells[3].Text.Trim(), 43, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["ParaName"].ToString() + "', '', '');");
                    e.Row.Attributes.Add("OnDblclick", "CallEdit('" + "角色資料編輯" + "')");
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

        #region Input_EquType_SelectedIndexChanged
        protected void Input_EquType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateDropItem(this.Input_EquType.SelectedValue);
            UpdatePanel2.Update();
            ViewState["query_EquModel"] = "";
            Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }
        #endregion

        #endregion

        #region Method

        #region CreateParentDropItem
        private void CreateParentDropItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;

            #region Give Empty Item
            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = "- "+this.GetGlobalResourceObject("Resource","ddlSelectDefault").ToString()+" -";
            Item.Value = "";
            this.Input_EquType.Items.Add(Item);

            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = "- " + this.GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString() + " -";
            Item.Value = "";
            this.Input_EquModel.Items.Add(Item);
            #endregion

            #region Process String
            sql = @" SELECT DISTINCT ItemInfo1 FROM B00_ItemList
                     WHERE ItemClass = 'EquModel' ";
            #endregion

            oAcsDB.GetDataTable("DropListItem", sql, out dt);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new System.Web.UI.WebControls.ListItem();
                switch (dr["ItemInfo1"].ToString())
                {
                    case "Door Access":
                        Item.Text = GetGlobalResourceObject("ResourceEquData", "EquTypeDA").ToString();
                        break;
                    case "Elevator":
                        Item.Text = GetGlobalResourceObject("ResourceEquData", "EquTypeElev").ToString();
                        break;
                    case "TRT":
                        Item.Text = GetGlobalResourceObject("ResourceEquData", "EquTypeTRT").ToString();
                        break;
                    case "Meal":
                        Item.Text = GetGlobalResourceObject("ResourceEquData", "EquTypeMeal").ToString();
                        break;
                }
                Item.Value = dr["ItemInfo1"].ToString();
                this.Input_EquType.Items.Add(Item);
            }
        }
        #endregion

        #region CreateDropItem
        private void CreateDropItem(string SelectValue)
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt;

            this.Input_EquModel.Items.Clear();

            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = "- " + this.GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString() + " -";
            Item.Value = "";
            this.Input_EquModel.Items.Add(Item);

            #region Process String
            sql = @" SELECT ItemNo, ItemName FROM B00_ItemList
                     WHERE ItemClass = 'EquModel' AND ItemInfo1 = ? ";
            liSqlPara.Add("S:" + SelectValue);
            #endregion

            oAcsDB.GetDataTable("DropListItem", sql, liSqlPara, out dt);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new System.Web.UI.WebControls.ListItem();
                Item.Text = dr["ItemName"].ToString();
                Item.Value = dr["ItemNo"].ToString();
                this.Input_EquModel.Items.Add(Item);
            }
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

        #region Query
        public void Query(string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = " SELECT * FROM B01_EquParaDef ";
            if (!string.IsNullOrEmpty(ViewState["query_EquModel"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (EquModel = ?) ";
                liSqlPara.Add("S:" + ViewState["query_EquModel"].ToString().Trim());
            }
            else
                wheresql = " 1 = 0 ";

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("EquParaDef", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();
        }
        #endregion

        #region Query(string mode)
        public int Query(string mode, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = " SELECT * FROM B01_EquParaDef ";
            if (!string.IsNullOrEmpty(ViewState["query_EquModel"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (EquModel = ?) ";
                liSqlPara.Add("S:" + ViewState["query_EquModel"].ToString().Trim());
            }

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("EquParaDef", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();

            int find = 0;

            #region 取得RoleTable後的動作
            DataRow dr = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dr = dt.Rows[i];

                if (SelectValue.Value == dr["ParaName"].ToString())
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
        public static string[] LoadData(string EquModel, string SelectValue)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Process String
            sql = @" SELECT * FROM B01_EquParaDef WHERE EquModel = ? AND ParaName = ? ";
            liSqlPara.Add("S:" + EquModel.Trim());
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
        public static object Insert(string UserID, string popInput_InputType, string Input_EquModel, string popInput_ParaName, string popInput_ParaDesc, string popInput_MinValue, string popInput_MaxValue, string popInput_ValueOptions, string popInput_EditFormURL, string popInput_Height, string popInput_Width, string popInput_DefaultValue)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            string[] NoArray = new string[2];
            NoArray[0] = popInput_ParaName;

            objRet = Check_Input_DB(NoArray, Input_EquModel, popInput_InputType, popInput_ParaDesc, popInput_MinValue, popInput_MaxValue, popInput_ValueOptions, popInput_EditFormURL, popInput_Height, popInput_Width, popInput_DefaultValue, "Insert");
            #region 新增設備參數定義資料
            if (objRet.result)
            {
                #region Process String
                sql = @" INSERT INTO B01_EquParaDef(EquModel, ParaName, ParaDesc, InputType, ValueOptions, MinValue,
                    MaxValue, EditFormURL, FormSize, DefaultValue, CreateUserID, CreateTime, UpdateUserID, UpdateTime)
                    VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
                liSqlPara.Add("S:" + Input_EquModel.Trim());
                liSqlPara.Add("S:" + popInput_ParaName.Trim());
                liSqlPara.Add("S:" + popInput_ParaDesc.Trim());
                liSqlPara.Add("I:" + popInput_InputType.Trim());
                liSqlPara.Add("S:" + popInput_ValueOptions.Trim());
                liSqlPara.Add("I:" + popInput_MinValue.Trim());
                liSqlPara.Add("I:" + popInput_MaxValue.Trim());
                liSqlPara.Add("S:" + popInput_EditFormURL.Trim());
                if (!string.IsNullOrEmpty(popInput_Height.Trim()) && !string.IsNullOrEmpty(popInput_Width.Trim()))
                    liSqlPara.Add("S:" + "dialogHeight:" + popInput_Height.Trim() + "px;dialogWidth:" + popInput_Width.Trim() + "px");
                else
                    liSqlPara.Add("S:");
                liSqlPara.Add("S:" + popInput_DefaultValue.Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                #endregion
                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            #endregion
            objRet.act = "Add";
            return objRet;
        }
        #endregion

        #region Update
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Update(string UserID, string SelectValue, string popInput_InputType, string Input_EquModel, string popInput_ParaName, string popInput_ParaDesc, string popInput_MinValue, string popInput_MaxValue, string popInput_ValueOptions, string popInput_EditFormURL, string popInput_Height, string popInput_Width, string popInput_DefaultValue)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            string[] NoArray = new string[2];
            NoArray[0] = popInput_ParaName;
            NoArray[1] = SelectValue;

            objRet = Check_Input_DB(NoArray, Input_EquModel, popInput_InputType, popInput_ParaDesc, popInput_MinValue, popInput_MaxValue, popInput_ValueOptions, popInput_EditFormURL, popInput_Height, popInput_Width, popInput_DefaultValue, "Update");

            #region 編輯設備參數定義資料
            if (objRet.result)
            {
                #region Process String - Updata Role
                sql = @" UPDATE B01_EquParaDef
                         SET InputType = ?, 
                         ParaName = ?, ParaDesc = ?, 
                         MinValue = ?, MaxValue = ?,
                         ValueOptions = ?, EditFormURL = ?,
                         FormSize = ?, DefaultValue = ?,
                         UpdateUserID = ?, UpdateTime = ?
                         WHERE EquModel = ? AND ParaName = ? ";

                liSqlPara.Add("I:" + popInput_InputType.Trim());
                liSqlPara.Add("S:" + popInput_ParaName.Trim());
                liSqlPara.Add("S:" + popInput_ParaDesc.Trim());
                liSqlPara.Add("I:" + popInput_MinValue.Trim());
                liSqlPara.Add("I:" + popInput_MaxValue.Trim());
                liSqlPara.Add("S:" + popInput_ValueOptions.Trim());
                liSqlPara.Add("S:" + popInput_EditFormURL.Trim());
                if (!string.IsNullOrEmpty(popInput_Height.Trim()) && !string.IsNullOrEmpty(popInput_Width.Trim()))
                    liSqlPara.Add("S:" + "dialogHeight:" + popInput_Height.Trim() + "px;dialogWidth:" + popInput_Width.Trim() + "px");
                else
                    liSqlPara.Add("S:");
                liSqlPara.Add("S:" + popInput_DefaultValue.Trim());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("S:" + Input_EquModel.Trim());
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
        public static object Delete(string SelectValue, string Input_EquModel)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            int iRet = 0;

            #region 刪除設備參數定義資料
            if (objRet.result)
            {
                oAcsDB.BeginTransaction();

                #region Process String - Delete EquParaDef
                sql = @" DELETE FROM B01_EquParaData
                         WHERE EquParaID  = ( SELECT EquParaID FROM B01_EquParaDef 
				     	                      WHERE EquModel = ? AND ParaName = ?) ";
                liSqlPara.Add("S:" + Input_EquModel.Trim());
                liSqlPara.Add("S:" + SelectValue.Trim());
                #endregion

                iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);

                liSqlPara.Clear();

                if (iRet > -1)
                {
                    #region Process String - Delete EquParaDef
                    sql = @" DELETE FROM B01_EquParaDef
                             WHERE EquModel = ? AND ParaName = ? ";
                    liSqlPara.Add("S:" + Input_EquModel.Trim());
                    liSqlPara.Add("S:" + SelectValue.Trim());
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
        protected static Pub.MessageObject Check_Input_DB(string[] NoArray, string EquModel, string InputType, string ParaDesc, string MinValue, string MaxValue, string ValueOptions, string EditFormURL, string Height, string Width, string DefaultValue, string mode)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            int result;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Input
            if (string.IsNullOrEmpty(EquModel.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "設備型號 必須指定";
            }
            else if (EquModel.Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "設備型號 字數超過上限";
            }

            if (string.IsNullOrEmpty(NoArray[0].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "參數名稱 必須輸入";
            }
            else if (NoArray[0].Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "參數名稱 字數超過上限";
            }

            if (string.IsNullOrEmpty(InputType.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "輸入方式 必須指定";
            }
            else if (!int.TryParse(InputType.Trim(), out result))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "輸入方式 設定錯誤";
            }

            if (string.IsNullOrEmpty(ParaDesc.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "參數描述 必須輸入";
            }
            else if (ParaDesc.Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "參數描述 字數超過上限";
            }

            if (string.IsNullOrEmpty(DefaultValue.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "預設值 必須輸入";
            }
            else if (DefaultValue.Trim().Count() > 1024)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "預設值 字數超過上限";
            }

            if (int.TryParse(InputType.Trim(), out result))
            {
                switch (result)
                {
                    case 1:
                        #region 數值範圍
                        int min, max, def;
                        if (string.IsNullOrEmpty(MinValue.Trim()))
                        {
                            if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                            objRet.result = false;
                            objRet.message += "最小數值 必須輸入";
                        }
                        else if (!int.TryParse(MinValue.Trim(), out min))
                        {
                            if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                            objRet.result = false;
                            objRet.message += "最小數值 必須為數值";
                        }

                        if (string.IsNullOrEmpty(MaxValue.Trim()))
                        {
                            if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                            objRet.result = false;
                            objRet.message += "最大數值 必須輸入";
                        }
                        else if (!int.TryParse(MaxValue.Trim(), out max))
                        {
                            if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                            objRet.result = false;
                            objRet.message += "最大數值 必須為數值";
                        }

                        if (!int.TryParse(DefaultValue.Trim(), out def))
                        {
                            if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                            objRet.result = false;
                            objRet.message += "選項為數值範圍，因此 預設值 必須為數值";
                        }
                        if (objRet.result)
                        {
                            if ((!string.IsNullOrEmpty(MinValue.Trim())) && (!string.IsNullOrEmpty(MaxValue.Trim())))
                            {
                                int.TryParse(MinValue.Trim(), out min);
                                int.TryParse(MaxValue.Trim(), out max);
                                int.TryParse(DefaultValue.Trim(), out def);

                                if (max < min)
                                {
                                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                                    objRet.result = false;
                                    objRet.message += "最大數值 不可小於 最小數值";
                                }
                                if (def < min || def > max)
                                {
                                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                                    objRet.result = false;
                                    objRet.message += "預設值 需介於最大數值與最小數值之間";
                                }
                            }
                        }
                        #endregion
                        break;
                    case 2:
                        #region 清單選項
                        if (string.IsNullOrEmpty(ValueOptions.Trim()))
                        {
                            if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                            objRet.result = false;
                            objRet.message += "選項參數設定 必須輸入";
                        }
                        else if (ValueOptions.Trim().Count() > 50)
                        {
                            if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                            objRet.result = false;
                            objRet.message += "選項參數設定 字數超過上限";
                        }
                        #endregion
                        break;
                    case 3:
                        #region 參考連結
                        int tempint;
                        if (string.IsNullOrEmpty(EditFormURL.Trim()))
                        {
                            if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                            objRet.result = false;
                            objRet.message += "編輯視窗URL 必須輸入";
                        }
                        else if (EditFormURL.Trim().Count() > 200)
                        {
                            if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                            objRet.result = false;
                            objRet.message += "編輯視窗URL 字數超過上限";
                        }

                        if (string.IsNullOrEmpty(Height.Trim()))
                        {
                            if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                            objRet.result = false;
                            objRet.message += "視窗高度 必須輸入";
                        }
                        else if (!int.TryParse(Height.Trim(), out tempint))
                        {
                            if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                            objRet.result = false;
                            objRet.message += "視窗高度 必須為數值";
                        }

                        if (string.IsNullOrEmpty(Width.Trim()))
                        {
                            if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                            objRet.result = false;
                            objRet.message += "視窗寬度 必須輸入";
                        }
                        else if (!int.TryParse(Width.Trim(), out tempint))
                        {
                            if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                            objRet.result = false;
                            objRet.message += "視窗寬度 必須為數值";
                        }
                        #endregion
                        break;
                }
            }
            #endregion

            #region DB
            switch (mode)
            {
                case "Insert":
                    sql = @" SELECT * FROM B01_EquParaDef WHERE EquModel = ? AND ParaName = ? ";
                    liSqlPara.Add("S:" + EquModel.Trim());
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    break;
                case "Update":
                    sql = @" SELECT * FROM B01_EquParaDef WHERE EquModel = ? AND ParaName = ? AND ParaName != ? ";
                    liSqlPara.Add("S:" + EquModel.Trim());
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    liSqlPara.Add("S:" + NoArray[1].Trim());
                    break;
            }


            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "此參數名稱已存在於系統中";
            }
            #endregion

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
                ProcessGridView.Rows[0].Cells[0].Text = this.GetGlobalResourceObject("Resource", "NonData").ToString();
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
    }
}