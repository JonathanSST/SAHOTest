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
    public partial class CtrlArea : System.Web.UI.Page
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
            oScriptManager.RegisterAsyncPostBackControl(this.QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("CtrlArea", "CtrlArea.js?jsdate=" + Pub.GetNowTime );//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");

            #region 註冊主頁Button動作
            string ttName = GetLocalResourceObject("ttRuleData").ToString();
            string strEdit = GetGlobalResourceObject("Resource", "btnEdit").ToString();
            string strAdd = GetGlobalResourceObject("Resource", "btnAdd").ToString();
            string strDelete = GetGlobalResourceObject("Resource", "btnDelete").ToString();

            QueryButton.Attributes["onClick"] = "SelectState(); return false;";
            AddButton.Attributes["onClick"] = "CallAdd('" + string.Format("{0}「{1}」",ttName,strAdd) + "'); return false;";
            EditButton.Attributes["onClick"] = "CallEdit('" + string.Format("{0}「{1}」", ttName, strEdit) + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";
            DeleteButton.Attributes["onClick"] = "CallDelete('" + string.Format("{0}「{1}」", ttName, strDelete) + "', '" +
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
            //oAcsDB.GetTableHash("B01_DeviceConnInfo", "zhtw", out TableInfo);
            //Label_No.Text = TableInfo["DciNo"].ToString();
            //Label_Name.Text = TableInfo["DciName"].ToString();
            //Label_Ip.Text = TableInfo["IpAddress"].ToString();
            //Label_IsAssign.Text = TableInfo["IsAssignIP"].ToString();
            //popLabel_No.Text = TableInfo["DciNo"].ToString();
            //popLabel_Name.Text = TableInfo["DciName"].ToString();
            //popLabel_PassWD.Text = TableInfo["DciPassWD"].ToString();
            //popLabel_Ip.Text = TableInfo["IpAddress"].ToString();
            //popLabel_Port.Text = TableInfo["TcpPort"].ToString();
            //popLabel_IsAssign.Text = TableInfo["IsAssignIP"].ToString();
            #endregion

            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion

                ViewState["query_no"] = "";
                ViewState["query_name"] = "";
                ViewState["SortExpression"] = "CtrlAreaNo";
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
                }

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
                    int[] HeaderWidth = { 150,150 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    //#region 管制區編號
                    //e.Row.Cells[0].Text = TableInfo["DciNo"].ToString();
                    //#endregion
                    //#region 管制區名稱
                    //e.Row.Cells[1].Text = TableInfo["DciName"].ToString();
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
                    GrRow.ID = "GV_Row" + oRow.Row["CtrlAreaNo"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 153, 153 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 管制區編號
                    #endregion
                    #region 管制區名稱
                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }
                    e.Row.Cells[0].Text = LimitText(e.Row.Cells[0].Text, 20, true);
                    e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 45, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["CtrlAreaNo"].ToString() + "', '', '')");
                    e.Row.Attributes.Add("OnDblclick", "CallEdit('" + "管制區資料編輯" + "')");

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
                        new LiteralControl(string.Format(this.GetGlobalResourceObject("Resource","lblGvCounts").ToString(), hDataRowCount.Value)));
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
            sql = " SELECT * FROM B01_CtrlArea  ";

            if (!string.IsNullOrEmpty(ViewState["query_no"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (CtrlAreaNo LIKE ?) ";
                liSqlPara.Add("S:" + "%" + ViewState["query_no"].ToString().Trim() + "%");
            }

            if (!string.IsNullOrEmpty(ViewState["query_name"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (CtrlAreaName LIKE ?) ";
                liSqlPara.Add("S:" + "%" + ViewState["query_name"].ToString().Trim() + "%");
            }

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("CtrlAreaTable", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();
        }
        #endregion

        #region Query(string mode)
        public int Query(string mode)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = " SELECT * FROM B01_CtrlArea ";
            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("DeviceTable", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();

            int find = 0;

            #region 取得RoleTable後的動作
            DataRow dr = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dr = dt.Rows[i];

                if (SelectValue.Value == dr["CtrlAreaNo"].ToString())
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
            string[] TableData = null;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Process String
            sql = @" SELECT CtrlAreaNo ,CtrlAreaName, ISNULL(CtrlAreaPsnCount,0) AS CtrlAreaPsnCount
                     FROM B01_CtrlArea WHERE CtrlAreaNo = ? ";
            liSqlPara.Add("S:" + SelectValue.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                TableData = new string[dr.DataReader.FieldCount];
                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                    TableData[i] = dr.DataReader[i].ToString();
            }
            else
            {
                TableData = new string[2];
                TableData[0] = "Saho_SysErrorMassage";
                TableData[1] = "系統中無此資料！";
            }
            #endregion

            return TableData;
        }
        #endregion

        #region Insert
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Insert(string UserID, string popInput_No, string popInput_Name, string popInput_PsnCount)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            string[] NoArray = new string[2];
            NoArray[0] = popInput_No;

            objRet = Check_Input_DB(NoArray, popInput_Name, "Insert");
            if (objRet.result)
            {
                #region Process String - Add DeviceConnInfo
                sql = @" INSERT INTO B01_CtrlArea(CtrlAreaNo, CtrlAreaName, CtrlAreaPsnCount, CreateUserID, CreateTime, UpdateUserID, UpdateTime)
                         VALUES(?, ?, ?, ?, ?, ?, ?) ";
                liSqlPara.Add("S:" + popInput_No.Trim());
                liSqlPara.Add("S:" + popInput_Name.Trim());
                //liSqlPara.Add("I:" + popInput_PsnCount.Trim());
                liSqlPara.Add("I:" + 0);
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("S:" + UserID.ToString());
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
        public static object Update(string UserID, string SelectValue, string popInput_No, string popInput_Name, string popInput_PsnCount)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            string[] NoArray = new string[2];
            NoArray[0] = popInput_No;
            NoArray[1] = SelectValue;

            objRet = Check_Input_DB(NoArray, popInput_Name, "Update");
            if (objRet.result)
            {
                #region Process String - Updata DeviceConnInfo
                sql = @" UPDATE B01_CtrlArea SET
                         CtrlAreaNo = ? , CtrlAreaName = ? , CtrlAreaPsnCount=?,
                         UpdateUserID = ? , UpdateTime = ?
                         WHERE CtrlAreaNo = ? ";

                liSqlPara.Add("S:" + popInput_No.Trim());
                liSqlPara.Add("S:" + popInput_Name.Trim());
                //liSqlPara.Add("I:" + popInput_PsnCount.Trim());
                liSqlPara.Add("I:" + 0);
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("S:" + SelectValue.Trim());
                #endregion
                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }

            objRet.act = "Edit";
            return objRet;
        }
        #endregion

        #region Delete
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Delete(string SelectValue)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();

            string[] NoArray = new string[2];
            NoArray[0] = SelectValue;

            objRet = Check_Input_DB(NoArray, "Delete", "Delete");
            if (objRet.result)
            {
                #region Process String - Delete DeviceConnInfo
                sql = " DELETE FROM B01_CtrlArea WHERE CtrlAreaNo = ? ";
                liSqlPara.Add("S:" + SelectValue);
                #endregion
                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }

            objRet.act = "Delete";
            return objRet;
        }
        #endregion

        #region Check_Input_DB
        protected static Pub.MessageObject Check_Input_DB(string[] NoArray, string Name, string mode)
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
                objRet.message += "管制區編號 必須輸入";
            }
            else if (NoArray[0].Trim().Count() > 20)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "管制區編號 字數超過上限";
            }

            if (string.IsNullOrEmpty(Name.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "管制區名稱 必須輸入";
            }
            else if (Name.Trim().Count() > 30)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "管制區名稱 字數超過上限";
            }

            #endregion

            #region DB
            switch (mode)
            {
                case "Insert":
                    sql = @" SELECT * FROM B01_CtrlArea WHERE CtrlAreaNo = ? ";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    break;
                case "Update":
                    sql = @" SELECT * FROM B01_CtrlArea WHERE CtrlAreaNo = ? AND CtrlAreaNo != ? ";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    liSqlPara.Add("S:" + NoArray[1].Trim());
                    break;
                case "Delete":
                    sql = @" SELECT * FROM B01_EquData AS EquData
                             LEFT JOIN B01_CtrlArea AS InToCtrlArea ON InToCtrlArea.CtrlAreaID = EquData.InToCtrlAreaID
                             LEFT JOIN B01_CtrlArea AS OutToCtrlArea ON OutToCtrlArea.CtrlAreaID = EquData.OutToCtrlAreaID
                             WHERE InToCtrlArea.CtrlAreaNo = ? OR OutToCtrlArea.CtrlAreaNo = ? ";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    break;
            }

            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                if (string.Compare(mode, "Delete") != 0)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "此代碼已存在於系統中";
                }
                else
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "此項目已被使用，無法刪除";
                }
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