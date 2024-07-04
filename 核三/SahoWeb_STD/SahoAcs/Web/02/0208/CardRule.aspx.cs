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
    public partial class CardRule : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 20;
        Hashtable TableInfo;
        static int DayCount = 8;
        static int RowCount = 5;
        public string MsgSelect = "";

        public string UpdateCardRuleDef = @"UPDATE B01_EquParaDef 
            SET DefaultValue='0:'+ISNULL((SELECT TOP 1 CONVERT(varchar,RuleID) FROM B01_CardRuleDef WHERE EquModel=@EquModel ORDER BY RuleID),'0')
            WHERE EquModel=@EquModel AND ParaName='CardRule'";

        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            //this.MsgSelect = GetGlobalResourceObject("Resource", "NotSelectForEdit").ToString().Replace("\n","\\n");
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("CardRule", "CardRule.js");//加入同一頁面所需的JavaScript檔案
            
            #region 註冊主頁Button動作
            string titleName = GetLocalResourceObject("ttRuleData").ToString();
            string strDelete = GetGlobalResourceObject("Resource", "btnDelete").ToString();
            string strAdd = GetGlobalResourceObject("Resource", "btnAdd").ToString();
            string strEdit = GetGlobalResourceObject("Resource", "btnEdit").ToString();
            QueryButton.Attributes["onClick"] = "SelectState(); return false;";
            AddButton.Attributes["onClick"] = "CallAdd('" + string.Format("{0}「{1}」", titleName, strAdd) + "'); return false;";
            EditButton.Attributes["onClick"] = "CallEdit('" + string.Format("{0}「{1}」",titleName,strEdit) + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";
            DeleteButton.Attributes["onClick"] = "CallDelete('" + string.Format("{0}「{1}」", titleName, strEdit) + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForDelete") + "'); return false;";
            #endregion

            #region 註冊pop1頁Button動作
            ImgCloseButton1.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            popB_Add.Attributes["onClick"] = "AddExcute(); return false;";
            popB_Edit.Attributes["onClick"] = "EditExcute(); return false;";
            popB_Delete.Attributes["onClick"] = "DeleteExcute(); return false;";
            popB_Cancel.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            #endregion

            Pub.SetModalPopup(ModalPopupExtender1, 1);
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
                #endregion

                hideEquModel.Value = "";
                ViewState["query_no"] = "";
                ViewState["query_name"] = "";
                ViewState["SortExpression"] = "RuleNo";
                ViewState["SortDire"] = "ASC";
                CreateDropItem();
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
                    int[] HeaderWidth = { 180 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 讀卡規則編號
                    #endregion
                    #region 讀卡規則名稱
                    #endregion
                    #region 讀卡規則資訊
                    #endregion

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
                    GrRow.ID = "GV_Row" + oRow.Row["RuleNo"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 183 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 讀卡規則編號
                    #endregion
                    #region 讀卡規則名稱
                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    //for (int x = 0; x < e.Row.Cells.Count; x++)
                    //{
                    //    if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                    //        e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    //}
                    //e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 13, true);
                    //e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text, 25, true);
                    //e.Row.Cells[3].Text = LimitText(e.Row.Cells[3].Text, 31, true);
                    //e.Row.Cells[5].Text = LimitText(e.Row.Cells[5].Text, 22, true);
                    #endregion
                    string titleName = GetLocalResourceObject("ttRuleData").ToString();        
                    string strEdit = GetGlobalResourceObject("Resource", "btnEdit").ToString();
                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["RuleNo"].ToString() + "', '', '')");
                    e.Row.Attributes.Add("OnDblclick", "CallEdit('" + string.Format("{0}「{1}」", titleName, strEdit) +"')");
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

        #endregion

        #region Method

        #region CreateDropItem
        private void CreateDropItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt;

            this.Input_EquModel.Items.Clear();

            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString();
            Item.Value = "";
            this.Input_EquModel.Items.Add(Item);


            #region Process String
            sql = @" SELECT A.* FROM B00_ItemList A INNER JOIN B01_EquParaDef B ON A.ItemNo=B.EquModel AND B.ParaName='CardRule'
                     WHERE ItemClass = 'EquModel' ";
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
            sql = " SELECT * FROM B01_CardRuleDef ";

            if (!string.IsNullOrEmpty(hideEquModel.Value.Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (EquModel = ?) ";
                liSqlPara.Add("S:" + hideEquModel.Value.Trim());
            }
            else
                wheresql += " 1 = 0 ";

            if (!string.IsNullOrEmpty(ViewState["query_no"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (RuleNo LIKE ?) ";
                liSqlPara.Add("S:" + "%" + ViewState["query_no"].ToString().Trim() + "%");
            }

            if (!string.IsNullOrEmpty(ViewState["query_name"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (RuleName LIKE ?) ";
                liSqlPara.Add("S:" + "%" + ViewState["query_name"].ToString().Trim() + "%");
            }

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("RuleTable", sql, liSqlPara, out dt);
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
            sql = " SELECT * FROM B01_CardRuleDef ";

            if (!string.IsNullOrEmpty(hideEquModel.Value.Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (EquModel = ?) ";
                liSqlPara.Add("S:" + hideEquModel.Value.Trim());
            }
            else
                wheresql += " 1 = 0 ";

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("RuleTable", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();

            int find = 0;

            #region 取得RoleTable後的動作
            DataRow dr = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dr = dt.Rows[i];

                if (SelectValue.Value == dr["RuleNo"].ToString())
                {
                    find = i;
                    break;
                }
            }
            #endregion

            return (find / _pagesize) + 1;
        }
        #endregion

        #region CheckAdd
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object CheckAdd(string EquModel)
        {
            Pub.MessageObject objRet = new Pub.MessageObject();

            if (string.IsNullOrEmpty(EquModel))
            {
                objRet.result = false;
                objRet.message = "請選擇設備型號";
            }
            objRet.act = "CheckAdd";
            return objRet;
        }
        #endregion

        #region LoadData
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadData(string SelectValue, string hideEquModel)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Process String
            sql = @" SELECT * FROM B01_CardRuleDef WHERE RuleNo = ? AND EquModel = ? ";
            liSqlPara.Add("S:" + SelectValue.Trim());
            liSqlPara.Add("S:" + hideEquModel.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                EditData = new string[dr.DataReader.FieldCount + 2];
                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                {
                    if (dr.DataReader[i].ToString() == dr.DataReader["RuleInfo"].ToString())
                    {
                        EditData[i] = RlueInfoReturn(dr.DataReader["RuleInfo"].ToString());
                        for (int x = 0; x < DayCount * RowCount; x++)
                        {
                            EditData[dr.DataReader.FieldCount] += (string.IsNullOrEmpty(EditData[dr.DataReader.FieldCount])) ? EditData[i].Substring(x * 6 + 4, 2) : "&" + EditData[i].Substring(x * 6 + 4, 2);
                            EditData[dr.DataReader.FieldCount + 1] += (string.IsNullOrEmpty(EditData[dr.DataReader.FieldCount + 1])) ? EditData[i].Substring(x * 6, 4) : "&" + EditData[i].Substring(x * 6, 4);
                        }
                    }
                    else
                        EditData[i] = dr.DataReader[i].ToString();
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

        #region Insert
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Insert(string UserID, string EquModel, string popInput_No, string popInput_Name, string[] CheckItem, string[] TextItem)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            string[] TimeBlock;
            string Infostr = "";

            TimeBlock = ProcessUIData(CheckItem, TextItem);

            string[] NoArray = new string[2];
            NoArray[0] = popInput_No;

            objRet = Check_Input_DB(EquModel, NoArray, popInput_Name, TimeBlock, "Insert", out TimeBlock);
            if (objRet.result)
            {
                for (int x = 0; x < DayCount; x++)
                    Infostr += TimeBlock[x];
                #region Process String - Insert Rule
                sql = @" INSERT INTO B01_CardRuleDef(EquModel, RuleNo, RuleName, RuleInfo, CreateUserID, CreateTime, UpdateUserID, UpdateTime)
                         VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
                liSqlPara.Add("S:" + EquModel.Trim());
                liSqlPara.Add("S:" + popInput_No.Trim());
                liSqlPara.Add("S:" + popInput_Name.Trim());
                liSqlPara.Add("S:" + Infostr.Trim());
                liSqlPara.Add("S:" + UserID.ToString().Trim());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("S:" + UserID.ToString().Trim());
                liSqlPara.Add("D:" + Time);
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
        public static object Update(string UserID, string EquModel, string SelectValue, string popInput_No, string popInput_Name, string[] CheckItem, string[] TextItem)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DB_Acs oAcsDB1 = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            string[] TimeBlock, ParaValueArray;//, ParaValueOtherArray;
            List<string> ParaValueOtherArray = new List<string>();
            string Infostr = "", rid = "", ParaValueStr = "", CompareStr = "";
            int iRet = 0, markLen, ridLen;
            DataTable dt;
            Sa.DB.DBReader dr;

            oAcsDB.BeginTransaction();

            TimeBlock = ProcessUIData(CheckItem, TextItem);

            string[] NoArray = new string[2];
            NoArray[0] = popInput_No;
            NoArray[1] = SelectValue;

            objRet = Check_Input_DB(EquModel, NoArray, popInput_Name, TimeBlock, "Update", out TimeBlock);
            #region 編輯卡片規則資料
            if (objRet.result)
            {
                #region 取得 RuleID
                sql = "SELECT RuleID FROM B01_CardRuleDef WHERE RuleNo = ? AND EquModel = ? ";
                liSqlPara.Add("S:" + SelectValue.Trim());
                liSqlPara.Add("S:" + EquModel.Trim());

                oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                if (dr.Read())
                {
                    rid = dr.DataReader["RuleID"].ToString();
                }
                sql = "";
                liSqlPara.Clear();
                #endregion

                #region 處理已定義的設備參數資料
                if (iRet > -1)
                {

                    sql = @" SELECT EquID, EquParaID, ParaValue, ParaValueOther FROM B01_EquParaData
                             WHERE EquParaID in ( SELECT EquParaID FROM B01_EquParaDef WHERE ParaName = 'CardRule' )
                             AND ParaValue + ',' LIKE '%:" + rid + @",%'";
                    oAcsDB.GetDataTable("EquParaData", sql, out dt);

                    sql = "";

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow Rowdr in dt.Rows)
                        {
                            ParaValueStr = "";
                            ParaValueOtherArray.Clear();
                            ParaValueOtherArray.AddRange(Rowdr["ParaValueOther"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                            ParaValueArray = Rowdr["ParaValue"].ToString().Split(',');

                            #region 移除rid項目
                            for (int k = ParaValueOtherArray.Count; 0 < k; k--)
                            {
                                markLen = ParaValueOtherArray[k - 1].ToString().IndexOf(":");
                                ridLen = (ParaValueOtherArray[k - 1].Length - 1) - ParaValueOtherArray[k - 1].ToString().IndexOf(":");
                                CompareStr = ParaValueOtherArray[k - 1].Substring(markLen + 1, ridLen);
                                if (string.Compare(CompareStr, rid) == 0)
                                {
                                    ParaValueOtherArray.RemoveAt(k - 1);
                                }
                            }
                            #endregion

                            #region 組成異動rid所影響到的字串
                            for (int i = 0; i < ParaValueArray.Length; i++)
                            {
                                markLen = ParaValueArray[i].ToString().IndexOf(":");
                                ridLen = (ParaValueArray[i].Length - 1) - ParaValueArray[i].ToString().IndexOf(":");
                                CompareStr = ParaValueArray[i].Substring(markLen + 1, ridLen);
                                if (string.Compare(CompareStr, rid) == 0)
                                {
                                    if (ParaValueStr != "") ParaValueStr += ",";
                                    ParaValueStr += ParaValueArray[i].ToString();
                                }
                            }
                            #endregion

                            #region 將ParaValueOtherArray的內容補上以供更新
                            for (int k = 0; k < ParaValueOtherArray.Count; k++)
                            {
                                if (ParaValueStr != "")
                                    ParaValueStr += ",";
                                ParaValueStr += ParaValueOtherArray[k].ToString();
                            }
                            #endregion

                            sql += @" UPDATE B01_EquParaData
                                  SET ParaValueOther = ?, IsReSend = '1', UpdateUserID = ?, UpdateTime = ?,OpStatus=''
                                  WHERE EquID = ? AND EquParaID = ? ";
                            liSqlPara.Add("S:" + ParaValueStr);
                            liSqlPara.Add("S:" + UserID.Trim());
                            liSqlPara.Add("D:" + Time);
                            liSqlPara.Add("S:" + Rowdr["EquID"].ToString());
                            liSqlPara.Add("S:" + Rowdr["EquParaID"].ToString());
                        }
                        iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                    }
                }
                sql = "";
                liSqlPara.Clear();
                #endregion

                for (int x = 0; x < DayCount; x++)
                    Infostr += TimeBlock[x];
                #region Process String - Update CardRule
                if (iRet > -1)
                {
                    sql = @" UPDATE B01_CardRuleDef SET 
                             RuleNo = ?, RuleName = ?, RuleInfo = ?,
                             UpdateUserID = ?,
                             UpdateTime = ?
                             WHERE RuleNo = ? AND EquModel = ? ";

                    liSqlPara.Add("S:" + popInput_No.Trim());
                    liSqlPara.Add("S:" + popInput_Name.Trim());
                    liSqlPara.Add("S:" + Infostr.Trim());
                    liSqlPara.Add("S:" + UserID.ToString().Trim());
                    liSqlPara.Add("D:" + Time);
                    liSqlPara.Add("S:" + SelectValue.Trim());
                    liSqlPara.Add("S:" + EquModel.Trim());
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

                if (iRet > -1)
                {
                    oAcsDB.Commit();
                }
                else
                {
                    objRet.result = false;
                    objRet.message = "資料庫語法問題!";
                    oAcsDB.Rollback();
                }
            }
            #endregion

            #region 更新控制器參數

            if (objRet.message == null)
            {
                #region 01. 得到需要更新資料的 CtrlID
                string strSQL1 = string.Format(@"
                    SELECT DISTINCT CtrlID FROM B01_Reader WHERE EquNo IN
                    (
	                    SELECT EquNo FROM B01_EquData WHERE EquID IN
	                    (
		                    SELECT EquID FROM B01_EquParaData
		                    WHERE EquParaID IN ( SELECT EquParaID FROM B01_EquParaDef WHERE ParaName = 'CardRule' )
		                    AND ParaValue + ',' LIKE '%:{0},%' 
	                    )
                    )", rid);

                Sa.DB.DBReader dr2 = new Sa.DB.DBReader();
                bool IsOK_1 = oAcsDB1.GetDataReader(strSQL1, out dr2);
                #endregion

                #region 02. 更新控制器參數
                // 若該控制器多於二台讀卡機，於更新時，自動將其他所屬讀卡機&設備更新到不需要重送，只送一台的參數即可
                if (IsOK_1)
                {
                    if (dr2.HasRows)
                    {
                        while (dr2.Read())
                        {
                            string strCtrlID = dr2.ToInt32("CtrlID").ToString();

                            strSQL1 = string.Format(@"
                                UPDATE B01_EquParaData 
                                SET paraValue=SV.P1, M_ParaValue=SV.P1, 
                                    isReSend=0, OpStatus='Setted', ParaValueOther='' 
                                FROM 
                                (
	                                SELECT 
                                        EPD.paraValue P1, EPD.M_ParaValue P2, 
                                        EPD.EquParaID, EPD.EquID FirstEquID 
                                    FROM B01_EquParaData EPD 
                                    INNER JOIN B01_EquParaDef EPF ON EPF.EquParaID = EPD.EquParaID 
                                    WHERE 
                                    EPD.EquID IN 
                                    (
	                                    SELECT TOP 1 EquID FROM B01_EquData ED 
	                                    INNER JOIN B01_Reader RD ON RD.EquNo = ED.EquNo 
	                                    INNER JOIN B01_Controller CR ON CR.CtrlID = RD.CtrlID 
	                                    WHERE CR.CtrlID = {0} 
	                                    ORDER BY CR.CreateTime
                                    ) 
                                    AND EPF.ParaType = 'Controller' 
                                    AND EPF.EquParaID = 
                                    (
                                        SELECT TOP 1 EquParaID FROM B01_EquParaDef 
                                        WHERE ParaName = 'CardRule' AND EquModel = 
                                            (SELECT CtrlModel FROM B01_Controller WHERE CtrlID = {0})
                                    ) 
                                ) SV 
                                WHERE B01_EquParaData.EquParaID = SV.EquParaID 
                                AND B01_EquParaData.EquID IN
                                (
                                    SELECT EquID FROM B01_EquData WHERE EquID != SV.FirstEquID 
                                    AND EquNo IN (SELECT EquNo FROM B01_Reader WHERE CtrlID = {0})
                                ) ", strCtrlID);

                            int iRlt = oAcsDB1.SqlCommandExecute(strSQL1);
                            ; ; ;
                        }
                    }
                }
                #endregion
            }

            #endregion

            objRet.act = "Edit";
            return objRet;
        }
        #endregion

        #region Delete
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Delete(string EquModel, string SelectValue, string UserID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            string[] ParaValueArray;
            string rid = "", ParaValueStr = "";
            int iRet = 0;
            DataTable dt;
            Sa.DB.DBReader dr;

            oAcsDB.BeginTransaction();

            #region 刪除卡片規則資料
            if (objRet.result)
            {
                #region 取得RID
                sql = "SELECT RuleID FROM B01_CardRuleDef WHERE RuleNo = ? AND EquModel = ? ";
                liSqlPara.Add("S:" + SelectValue.Trim());
                liSqlPara.Add("S:" + EquModel.Trim());

                oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                if (dr.Read())
                {
                    rid = dr.DataReader["RuleID"].ToString();
                }
                sql = "";
                liSqlPara.Clear();
                #endregion

                #region 處理已定義的設備參數資料
                if (iRet > -1)
                {

                    sql = @" SELECT EquID, EquParaID, ParaValue FROM B01_EquParaData
                             WHERE EquParaID in ( SELECT EquParaID FROM B01_EquParaDef WHERE ParaName = 'CardRule' )
                             AND ParaValue + ',' LIKE '%:" + rid + @",%'";
                    oAcsDB.GetDataTable("EquParaData", sql, out dt);

                    foreach (DataRow Rowdr in dt.Rows)
                    {
                        ParaValueStr = "";
                        ParaValueArray = Rowdr["ParaValue"].ToString().Split(',');
                        for (int i = 0; i < ParaValueArray.Length; i++)
                        {
                            if (ParaValueArray[i].ToString().IndexOf(rid) >= 0)
                                continue;
                            else
                            {
                                if (ParaValueStr != "") ParaValueStr += ",";
                                ParaValueStr += ParaValueArray[i].ToString();
                            }
                        }
                        sql += @" UPDATE B01_EquParaData
                                  SET ParaValue = ?, UpdateUserID = ?, UpdateTime = ?
                                  WHERE EquID = ? AND EquParaID = ? ";
                        liSqlPara.Add("S:" + ParaValueStr);
                        liSqlPara.Add("S:" + UserID.Trim());
                        liSqlPara.Add("D:" + Time);
                        liSqlPara.Add("S:" + Rowdr["EquID"].ToString());
                        liSqlPara.Add("S:" + Rowdr["EquParaID"].ToString());
                    }
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                sql = "";
                liSqlPara.Clear();
                #endregion

                #region Process String - Updata Role
                if (iRet > -1)
                {
                    sql = @" DELETE FROM B01_CardRuleDef
                             WHERE RuleNo = ? AND EquModel = ? ";
                    liSqlPara.Add("S:" + SelectValue.Trim());
                    liSqlPara.Add("S:" + EquModel.Trim());
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                #endregion

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
        protected static Pub.MessageObject Check_Input_DB(string EquModel, string[] NoArray, string Name, string[] TimeBlock, string mode, out string[] varObj)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();
            string[] ProcessStr = new string[RowCount];
            Sa.DB.DBReader dr;
            bool CheckTimeflag = true;

            #region Input
            if (string.IsNullOrEmpty(NoArray[0].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "讀卡規則編號 必須輸入";
            }
            else if (NoArray[0].Trim().Count() > 40)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "讀卡規則編號 字數超過上限";
            }

            if (Name.Trim().Count() > 40)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "讀卡規則名稱 字數超過上限";
            }
            #endregion

            #region CheckTime
            for (int i = 0; i < DayCount; i++)
            {
                #region 依照每天中的資料進行判斷
                for (int x = 0; x < RowCount; x++)
                {
                    //if (string.Compare(TimeBlock[i].Substring(x * 6, 4), "FFFF") == 0 && string.Compare(TimeBlock[i].Substring(x * 6 + 4, 2), "01") == 0)
                    //{
                    //    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    //    CheckTimeflag = false;
                    //    break;
                    //}
                    ProcessStr[x] = TimeBlock[i].Substring(x * 6, 6);
                }
                #endregion
                #region 進行交換排序並丟回TimeBlock
                //if (CheckTimeflag)
                //{
                    ProcessStr = ExchangeSort(ProcessStr, true);
                    TimeBlock[i] = null;
                    for (int x = 0; x < RowCount; x++)
                        TimeBlock[i] += ProcessStr[x];
                //}
                //else
                //{
                //    objRet.result = false;
                //    objRet.message += "時間未輸入,不能進行讀卡控制";
                //    break;
                //}
                #endregion
            }
            varObj = TimeBlock;
            #endregion

            #region DB
            switch (mode)
            {
                case "Insert":
                    sql = @" SELECT * FROM B01_CardRuleDef WHERE RuleNo = ? AND EquModel = ?";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    liSqlPara.Add("S:" + EquModel);
                    break;
                case "Update":
                    sql = @" SELECT * FROM B01_CardRuleDef WHERE (RuleNo = ? AND RuleNo != ?) AND EquModel = ? ";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    liSqlPara.Add("S:" + NoArray[1].Trim());
                    liSqlPara.Add("S:" + EquModel);
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

        #region RlueInfoReturn
        public static string RlueInfoReturn(string RlueInfo)
        {
            string[] TimeBlock = new string[DayCount];
            string ProcessStr = "";
            for (int i = 0; i < TimeBlock.Length; i++)
            {
                TimeBlock[i] = RlueInfo.Substring(i * RowCount * 6, RowCount * 6);
            }
            for (int x = 0; x < RowCount; x++)
            {
                for (int i = 0; i < TimeBlock.Length; i++)
                    ProcessStr += TimeBlock[i].Substring(x * 6, 6);
            }
            return ProcessStr;
        }
        #endregion

        #region ProcessUIData
        public static string[] ProcessUIData(string[] CheckItem, string[] TextItem)
        {
            string boolstr = "", timestr = "";
            string[] ReturnArray = new string[DayCount];
            for (int i = 0; i < DayCount * RowCount; i++)
            {
                boolstr = (bool.Parse(CheckItem[i])) ? "01" : "00";
                timestr = (string.IsNullOrEmpty(TextItem[i])) ? "FFFF" : TextItem[i];
                ReturnArray[i % 8] += timestr + boolstr;
            }
            return ReturnArray;
        }
        #endregion

        #region ExchangeSort
        /// <summary>
        /// 交換排序（Exchange sort）
        /// </summary>
        /// <param name="OrgArr">傳入所需處理陣列</param>
        /// <param name="isIncrease">升降冪控制[True:升冪 False:降冪]</param>
        /// <returns></returns>
        public static string[] ExchangeSort(string[] sOrgArr, bool isIncrease)
        {
            int buffer;
            int[] OrgArr = new int[sOrgArr.Length];
            #region 文字陣列 轉 數字陣列
            for (int i = 0; i < sOrgArr.Length; i++)
            {
                if (int.TryParse(sOrgArr[i], out buffer))
                    OrgArr[i] = buffer;
                else
                    OrgArr[i] = int.Parse(sOrgArr[i].Replace("FFFF", "9999"));
            }
            #endregion

            #region 交換排序
            for (int i = 0; i < OrgArr.Length - 1; i++)
            {
                for (int k = i + 1; k < OrgArr.Length; k++)
                {
                    if ((isIncrease && OrgArr[i] > OrgArr[k]) || (!isIncrease && OrgArr[i] < OrgArr[k]))  //遞增遞減判斷
                    {
                        buffer = OrgArr[i];
                        OrgArr[i] = OrgArr[k];
                        OrgArr[k] = buffer;
                    }
                }
            }
            #endregion

            #region 數字陣列 轉 文字陣列
            for (int i = 0; i < sOrgArr.Length; i++)
            {
                if (OrgArr[i] >= 999900)
                    sOrgArr[i] = OrgArr[i].ToString().Replace("9999", "FFFF");
                else
                    sOrgArr[i] = OrgArr[i].ToString().PadLeft(6, '0');
            }
            #endregion
            return sOrgArr;
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
                ProcessGridView.Rows[0].Cells[0].Text = GetGlobalResourceObject("Resource","NonData").ToString();
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

