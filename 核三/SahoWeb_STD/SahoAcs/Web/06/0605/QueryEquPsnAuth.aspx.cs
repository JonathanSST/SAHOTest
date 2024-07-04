using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using Sa.DB;
using PagedList;


namespace SahoAcs
{
    public partial class QueryEquPsnAuth : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 20;
        Hashtable TableInfo = new Hashtable();
        string OrgTitleName = "單位名稱";
        string OrgTitleNo = "單位代碼";
        string OrgClass = "Unit";
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            this.OrgTitleName = this.GetLocalResourceObject("ttUnitName").ToString();
            this.OrgTitleNo = this.GetLocalResourceObject("ttUnitNo").ToString();

            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);

            #region 註冊主頁Button動作
            //Input_EquModel.Attributes["onChange"] = "SelectState('" + this.Input_EquModel .ClientID+ "');";
            //AddButton.Attributes["onClick"] = "CallAdd('" + "設備參數定義資料新增" + "','設備型號 必須指定'); return false;";
            //EditButton.Attributes["onClick"] = "CallEdit('" + "設備參數定義資料編輯" + "'); return false;";
            //DeleteButton.Attributes["onClick"] = "CallDelete('" + "設備參數定義資料刪除" + "'); return false;";

            #endregion

            #region 註冊pop1頁Button動作
            //ImgCloseButton1.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            //popInput_InputType.Attributes["onchange"] = "InputTypeChange(popInput_InputType);";
            //popB_Add.Attributes["onClick"] = "AddExcute(); return false;";
            //popB_Edit.Attributes["onClick"] = "EditExcute(); return false;";
            //popB_Delete.Attributes["onClick"] = "DeleteExcute(); return false;";
            //popB_Cancel.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            #endregion

            this.MainGridView.PageSize = _pagesize;

            #region 語系切換
            //DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            //oAcsDB.GetTableHash("B01_EquParaDef", "zhtw", out TableInfo);
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
                Session["tmpList"] = new List<string>();
                Session["tmpDatatable"] = new DataTable();

                // Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                ViewState["SortExpression"] = "Building";
                ViewState["SortDire"] = "ASC";
                ViewState["hFlagRowCount"] = "";        

                CreateBuildingDropItem();

                EmptyCondition();
            }
            else
            {
                #region 暫不用
                //string sFormTarget = Request.Form["__EVENTTARGET"];
                //string sFormArg = Request.Form["__EVENTARGUMENT"];

                //if (sFormArg == "popPagePost") //進行因應新增或編輯後所需的換頁動作
                //{
                //    //ViewState["query_Building"] = this.BuildingValue.Value;
                //    //int find = Query("popPagePost");
                //    Sa.Web.Fun.RunJavaScript(this, "__doPostBack('ctl00$ContentPlaceHolder1$MainGridView', 'Page$" + find + "');");
                //}
                //else if (sFormArg.Substring(0, 5) == "Page$") //換頁完成後進行GridViewRow的移動
                //{
                //Query("popPagePost");
                //Sa.Web.Fun.RunJavaScript(this, "GridSelect();");
                //}
                #endregion

                if (!this.Input_Condition.Text.Trim().Equals(""))
                {
                    Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString(), false);
                }
                else
                {
                    EmptyCondition();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert1", "alert('" + this.GetLocalResourceObject("alertInput") + "!!!')", true);
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

            Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString(), false);
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
                    int[] HeaderWidth = { 100, 60, 120, 200, 100, 80, 80, 80 };
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
                    e.Row.Cells[7].Text = this.OrgTitleNo;
                    e.Row.Cells[8].Text = this.OrgTitleName;
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
                    //GridViewRow GrRow = e.Row;
                    //GrRow.ClientIDMode = ClientIDMode.Static;
                    //GrRow.ID = "GV_Row" + oRow.Row["PsnNo"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 103, 64, 124, 204, 104, 85, 83, 85 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 參數名稱
                    #endregion
                    #region 參數描述
                    #endregion
                    #region 輸入方式
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[5].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[7].HorizontalAlign = HorizontalAlign.Center;
                    #endregion
                    #region 參數定義
                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }
                    e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 27, true);
                    e.Row.Cells[3].Text = LimitText(e.Row.Cells[3].Text, 29, true);
                    e.Row.Cells[6].Text = LimitText(e.Row.Cells[6].Text, 11, true);
                    e.Row.Cells[8].Text = LimitText(e.Row.Cells[8].Text, 29, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["PsnNo"].ToString().Trim() + "', '', '')");

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
                    lbtnPrev.Click += delegate (object obj, EventArgs args)
                    {
                        if (gv.PageIndex > 0)
                        {
                            gv.PageIndex = gv.PageIndex - 1;
                            MainGridView.DataBind();

                            //Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        }
                    };

                    lbtnNext.Click += delegate (object obj, EventArgs args)
                    {
                        if (gv.PageIndex < gv.PageCount)
                        {
                            gv.PageIndex = gv.PageIndex + 1;
                            MainGridView.DataBind();

                            //Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        }
                    };
                    #endregion

                    #region 首末頁
                    lbtnFirst.Click += delegate (object obj, EventArgs args)
                    {
                        gv.PageIndex = 0;
                        MainGridView.DataBind();

                        //Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    };

                    lbtnLast.Click += delegate (object obj, EventArgs args)
                    {
                        gv.PageIndex = MainGridView.PageCount;
                        MainGridView.DataBind();

                        //Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
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

        #region QueryButton_Click
        protected void QueryButton_Click(object sender, EventArgs e)
        {
            //Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString(), false);
        }

        protected void ExportButton_Click(object sender, EventArgs e)
        {
            Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString(), true);
        }

        #endregion


        #region PdfButton_Click
        protected void PdfButton_Click(object sender, EventArgs e)
        {
            Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString(), true);
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
        public void Query(string SortExpression, string SortDire, bool _mode = false)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt = new DataTable();
            List<string> liSqlPara = new List<string>();
            String NowCondition = Input_Condition.Text.Trim();

            oAcsDB.GetDataTable("OrgData", "SELECT OrgClass,COUNT(*) AS COUNTS FROM B01_OrgData where OrgClass IN ('Department','Unit') GROUP BY OrgClass ORDER BY COUNTS DESC", out dt);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["OrgClass"].ToString() != "Unit")
                {
                    this.OrgTitleName = this.GetLocalResourceObject("ttDeptName").ToString();
                    this.OrgTitleNo = this.GetLocalResourceObject("ttDeptNo").ToString();
                    this.OrgClass = "Department";
                }
            }

            #region Process String

            sql = @" 
                SELECT DISTINCT
                    EquData.Building, EquData.Floor, EquData.EquNo, 
                    EquData.EquName, EquData.EquClass,
                    Person.PsnNo, Person.PsnName,
                    Org.OrgNoList,OrgName,OrgNo
                FROM B01_EquData AS EquData
                INNER JOIN B01_CardAuth CA ON CA.EquID = EquData.EquID AND CA.OpMode<>'Del'
                INNER JOIN B01_Card AS [Card] ON [Card].CardNo = CA.CardNo 
                INNER JOIN B01_Person AS Person ON Person.PsnID = [Card].PsnID
                INNER JOIN B01_EquGroupData AS EG ON EG.EquID=EquData.EquID
                INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EG.EquGrpID
                INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID
                INNER JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID 
                LEFT JOIN OrgStrucAllData(?) AS Org ON Org.OrgStrucID = Person.OrgStrucID 
                WHERE SysUser.UserID = ? ";

            /*
            @" 
            SELECT DISTINCT
                EquData.Building, EquData.Floor, EquData.EquNo, 
                EquData.EquName, EquData.EquClass,
                Person.PsnNo, Person.PsnName,
                Org.OrgNoList,OrgName,OrgNo
            FROM B01_EquData AS EquData
            INNER JOIN B01_CardAuth CA ON CA.EquID = EquData.EquID AND CA.OpMode<>'Del'
            INNER JOIN B01_Card AS [Card] ON [Card].CardNo = CA.CardNo 
            INNER JOIN B01_Person AS Person ON Person.PsnID = [Card].PsnID
            INNER JOIN B01_OrgStruc AS OrgStruc ON OrgStruc.OrgStrucID = Person.OrgStrucID
            INNER JOIN B01_MgnOrgStrucs AS MgnOrgStrucs ON MgnOrgStrucs.OrgStrucID = OrgStruc.OrgStrucID
            INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnOrgStrucs.MgaID
            INNER JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID 
            LEFT JOIN OrgStrucAllData(?) AS Org ON Org.OrgIDList = OrgStruc.OrgIDList 
            WHERE SysUser.UserID = ? ";
            */
            liSqlPara.Add("S:" + this.OrgClass);
            liSqlPara.Add("S:" + Session["UserID"].ToString());

            if (Input_Building.SelectedValue != 
                    GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString())
            {
                sql += " AND EquData.Building = ? ";
                liSqlPara.Add("S:" + Input_Building.SelectedValue);
            }            

            if (!string.IsNullOrEmpty(NowCondition))
            {
                string[] strCondiition = new string[] { "\t", " ", "　", ",", "，", "、", "。", "\n" };
                string[] strArray = NowCondition.Split(strCondiition, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i <= strArray.Length - 1; i++)
                {
                    if (strArray[i].Trim() != "")
                    {
                        sql += "AND (EquData.EquNo + '__' + ISNULL(EquData.EquName,'') + '__' + ISNULL(Person.PsnNo,'') + '__' + ISNULL(Person.PsnName,'')) LIKE ? ";
                        liSqlPara.Add("S:" + "%" + strArray[i].Trim() + "%");
                    }
                }
            }
            if (!this.ChkOutDate.Checked)
            {
                sql += " AND GETDATE() BETWEEN CA.BeginTime AND CA.EndTime ";
            }

            sql += " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            #region 讀取資料庫資料並寫入datatable
            /* 
                建立是否讀取資料庫比較標準，使用比照標準的資料表做是否RUN SQL的標準，
                以免資料量大時反復讀取速度過慢
                這個網頁採用 B01_EquData、B01_Person 的'總數字串'相加作為標準
                當 B01_EquData、B01_Person 的總數發生異動時，才重新讀取資料庫
            */

            if (Session["tmpList"] == null)
                Session["tmpList"] = new List<string>();
            if (Session["tmpDatatable"] == null)
                Session["tmpDatatable"] = new DataTable();
            if (Session["ChkState"] == null)
            {
                Session["ChkState"] = this.ChkOutDate.Checked;
            }
            

            string strC1 = oAcsDB.GetStrScalar("SELECT COUNT(*) FROM B01_EquData");
            string strC2 = oAcsDB.GetStrScalar("SELECT COUNT(*) FROM B01_Person");
            string strCount = "";

            if (strC1 != null && strC2 != null) strCount = strC1 + strC2;

            if (liSqlPara.SequenceEqual(Session["tmpList"] as List<string>)
                && ViewState["hFlagRowCount"].ToString() == strCount && ((bool)Session["ChkState"])==this.ChkOutDate.Checked)
            {
                dt = Session["tmpDatatable"] as DataTable;
                dt.DefaultView.Sort = SortExpression + " " + SortDire;
            }
            else
            {
                bool isOk = true;

                isOk = oAcsDB.GetDataTable("CardTable", sql, liSqlPara, out dt);

                if (isOk)
                {
                    dt.DefaultView.Sort = SortExpression + " " + SortDire;

                    sql = @"
                        SELECT DISTINCT [OrgID],[OrgNo],[OrgName] 
                        FROM [B01_OrgData] WHERE OrgClass = 'Unit' ";
                    DataTable tmpDT = new DataTable();

                    isOk = oAcsDB.GetDataTable("tmpOrgData", sql, out tmpDT);
                    /*
                    if (isOk)
                    {
                        #region 在 gridview 增加單位名稱、單位代碼

                        dt.Columns.Add("OrgNo");
                        dt.Columns.Add("OrgName");

                        foreach (DataRow dr in dt.Rows)
                        {
                            // 用反斜線區分開 \u005C = 反斜線
                            string[] tmp_OrgNoList = dr["OrgNoList"].ToString().Split('\u005C');
                            string strOrgNo = "";

                            for (int j = 0; j <= tmp_OrgNoList.GetUpperBound(0); j++)
                            {
                                if (!tmp_OrgNoList[j].Equals(""))
                                {
                                    if (tmp_OrgNoList[j].Substring(0, 1).ToUpper().Equals("U"))
                                    {
                                        strOrgNo = tmp_OrgNoList[j].Trim();
                                        DataRow[] drs = tmpDT.Select("OrgNo = '" + strOrgNo + "'");

                                        if (drs.GetUpperBound(0) != -1)
                                        {
                                            foreach (DataRow dr1 in drs)
                                            {
                                                dr["OrgNo"] = dr1["OrgNo"];
                                                dr["OrgName"] = dr1["OrgName"];
                                            }
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                        dt.Columns.Remove("OrgNoList");

                        #endregion
                    }
                    */

                    ViewState["hFlagRowCount"] = strCount;
                    Session["tmpList"] = liSqlPara;
                    Session["ChkState"] = this.ChkOutDate.Checked;
                    Session["tmpDatatable"] = dt;
                }
            }

            hDataRowCount.Value = dt.Rows.Count.ToString();
            #endregion

            if (_mode)
            {
                if (dt.Rows.Count != 0)
                {
                    ExportExcel(dt);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('無資料可匯出!!');location.href='QueryEquPsnAuth.aspx';", true);
                }
            }
            else
            {
                GirdViewDataBind(this.MainGridView, dt);
                UpdatePanel1.Update();
            }
        }
        #endregion

        #region ExportExcel add by Sam 20160624
        public void ExportExcel(DataTable ProcDt)
        {

            if (Request.Form.AllKeys.Where(i => i.Contains("PdfButton")).Count() > 0)
            {
                this.ExportPdf(ProcDt);
            }


            // syslog
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            oAcsDB.WriteLog(DB_Acs.Logtype.資料查詢, Session["UserID"].ToString(), Session["UserName"].ToString(), "0605", "", "", 
                string.Format("匯出資料，條件：[{0}]]", Input_Condition.Text.Trim()),
                "EXCEL資料匯出"
            );

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("EATCardLog");
            DataTable equData = ProcDt;

            //Title
            ws.Cells[1, 1].Value = "建築物名稱";
            ws.Cells[1, 2].Value = "樓層";
            ws.Cells[1, 3].Value = "設備編號";
            ws.Cells[1, 4].Value = "設備名稱";
            ws.Cells[1, 5].Value = "設備型號";
            ws.Cells[1, 6].Value = "人員編號";
            ws.Cells[1, 7].Value = "人員姓名";            
            ws.Cells[1, 8].Value = this.OrgTitleName;
            ws.Cells[1, 9].Value = this.OrgTitleNo;

            //Content
            for (int i = 0, iCount = equData.Rows.Count; i < iCount; i++)
            {
                //for (int j = 2, jCount = equData.Rows[i].ItemArray.Length; j < jCount; j++)
                //{
                //    ws.Cells[i + 2, j - 1].Value = equData.Rows[i].ItemArray[j].ToString().Replace("&nbsp;", "").Trim();
                //}
                ws.Cells[i + 2, 1].Value = equData.Rows[i]["Building"].ToString().Replace("&nbsp;", "").Trim();
                ws.Cells[i + 2, 2].Value = equData.Rows[i]["Floor"].ToString().Replace("&nbsp;", "").Trim();
                ws.Cells[i + 2, 3].Value = equData.Rows[i]["EquNo"].ToString().Replace("&nbsp;", "").Trim();
                ws.Cells[i + 2, 4].Value = equData.Rows[i]["EquName"].ToString().Replace("&nbsp;", "").Trim();
                ws.Cells[i + 2, 5].Value = equData.Rows[i]["EquClass"].ToString().Replace("&nbsp;", "").Trim();
                ws.Cells[i + 2, 6].Value = equData.Rows[i]["PsnNo"].ToString().Replace("&nbsp;", "").Trim();
                ws.Cells[i + 2, 7].Value = equData.Rows[i]["PsnName"].ToString().Replace("&nbsp;", "").Trim();
                ws.Cells[i + 2, 8].Value = equData.Rows[i]["OrgName"].ToString().Replace("&nbsp;", "").Trim();
                ws.Cells[i + 2, 9].Value = equData.Rows[i]["OrgNo"].ToString().Replace("&nbsp;", "").Trim();
            }

            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=EATCardLog.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }
        #endregion


        #region 匯出pdf檔
        public void ExportPdf(DataTable ProcDt)
        {
            StringBuilder sb = new StringBuilder(@"<html><style> td
                {
                    border-style:solid; border-width:1px; border-color:Black;width:70px;
                } </style><body>");
            if (ProcDt.Rows.Count > 0)
            {
                var PageData = ProcDt.AsEnumerable().ToPagedList(1, 18);
                for (int p = 1; p <= PageData.PageCount; p++)
                {
                    sb.Append(@"<table style='width:100%; border-collapse:collapse;border-spacing:0px 0px;'>");
                    sb.Append("<tr><td>建築物名稱</td>");
                    sb.Append("<td>樓層</td>");
                    sb.Append("<td>設備編號</td>");
                    sb.Append("<td>設備名稱</td>");
                    sb.Append("<td>設備型號</td>");
                    sb.Append("<td>人員編號</td>");
                    sb.Append("<td>人員姓名</td>");
                    sb.Append("<td>"+this.OrgTitleNo+"</td>");
                    sb.Append("<td>"+this.OrgTitleName+"</td>");                    
                    sb.Append("</tr>");
                    foreach (var r in ProcDt.AsEnumerable().ToPagedList(p, 18))
                    {
                        sb.Append("<tr><td>" + r["Building"].ToString() + "</td>");
                        sb.Append(string.Format("<td>{0}</td>", r["Floor"]));
                        sb.Append(string.Format("<td>{0}</td>", r["EquNo"]));
                        sb.Append(string.Format("<td style='width:12%'>{0}</td>", r["EquName"]));
                        sb.Append(string.Format("<td>{0}</td>", r["EquClass"]));
                        sb.Append(string.Format("<td>{0}</td>", r["PsnNo"]));                        
                        sb.Append(string.Format("<td>{0}</td>", r["PsnName"]));
                        sb.Append(string.Format("<td>{0}</td>", r["OrgNo"]));
                        sb.Append(string.Format("<td>{0}</td></tr>", r["OrgName"]));
                    }
                    sb.Append("</table>");
                    if (p < PageData.PageCount)
                    {
                        sb.Append("<p style='page-break-after:always'>&nbsp;</p>");
                    }
                }
            }
            else
            {
                sb.Append("查無資料");
            }
            sb.Append("</body></html>");
            Response.Clear();
            byte[] data = Encoding.UTF8.GetBytes(sb.ToString());
            MemoryStream msInput = new MemoryStream(data);
            iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 3, 3, 3, 3);
            iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, Response.OutputStream);
            iTextSharp.text.pdf.PdfDestination pdfDest = new iTextSharp.text.pdf.PdfDestination(iTextSharp.text.pdf.PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
            doc.Open();
            iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.UTF8, new SahoAcs.UnicodeFontFactory());
            //將pdfDest設定的資料寫到PDF檔
            iTextSharp.text.pdf.PdfAction action = iTextSharp.text.pdf.PdfAction.GotoLocalPage(1, pdfDest, writer);
            writer.SetOpenAction(action);
            doc.Close();
            msInput.Close();
            //這裡控制Response回應的格式內容

            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=ReportData.pdf");
            Response.End();
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

        #region CreateBuildingDropItem
        private void CreateBuildingDropItem()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt = new DataTable();

            oAcsDB.GetDataTable("OrgData", "SELECT OrgClass,COUNT(*) AS COUNTS FROM B01_OrgData WHERE OrgClass IN ('Department','Unit') GROUP BY OrgClass ORDER BY COUNTS DESC", out dt);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["OrgClass"].ToString() != "Unit")
                {
                    this.OrgTitleName = this.GetLocalResourceObject("ttDeptName").ToString();
                    this.OrgTitleNo = this.GetLocalResourceObject("ttDeptNo").ToString();
                    this.OrgClass = "Department";
                }
            }

            ListItem Item = new ListItem();
            Item.Text = this.GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString();
            Item.Value = this.GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString();
            this.Input_Building.Items.Add(Item);

            sql = @" SELECT DISTINCT Building FROM B01_EquData 
                     WHERE [Building] > '' AND [Building] IS NOT NULL ";
            oAcsDB.GetDataTable("DropListItem", sql, out dt);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new ListItem();
                Item.Text = dr["Building"].ToString();
                Item.Value = dr["Building"].ToString();
                this.Input_Building.Items.Add(Item);
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

        private void EmptyCondition()
        {
            #region 做一個空的datatable給gridview秀查無資料
            DataTable dtEmpty = new DataTable();
            dtEmpty.Columns.Add("Building");
            dtEmpty.Columns.Add("Floor");
            dtEmpty.Columns.Add("EquNo");
            dtEmpty.Columns.Add("EquName");
            dtEmpty.Columns.Add("EquClass");
            dtEmpty.Columns.Add("PsnNo");
            dtEmpty.Columns.Add("PsnName");
            dtEmpty.Columns.Add("OrgStrucNo");
            dtEmpty.Columns.Add("OrgNo");
            dtEmpty.Columns.Add("OrgName");

            GirdViewDataBind(this.MainGridView, dtEmpty);
            UpdatePanel1.Update();
            #endregion
        }
        #endregion
    }
}