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
using OfficeOpenXml;
using PagedList;


namespace SahoAcs
{
    public partial class QueryGroupPsn : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 100;
        Hashtable TableInfo;
        string EquGrpName = "";
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.Input_EquGroup);

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

                CreateEquGroupDropItem();
                ViewState["SortExpression"] = "PsnNo";
                ViewState["SortDire"] = "ASC";
            }

            Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
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
                    int[] HeaderWidth = { 100, 120, 100, 230, 100 };
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
                    GrRow.ID = "GV_Row" + oRow.Row["PsnNo"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 103, 124, 104, 233, 104 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 參數名稱
                    #endregion
                    #region 參數描述
                    #endregion
                    #region 輸入方式
                    e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Center;
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
                    e.Row.Cells[3].Text = LimitText(e.Row.Cells[3].Text, 43, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
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

        #region Input_EquGroup_SelectedIndexChanged
        protected void Input_EquGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }
        #endregion

        #region Excel output
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

        #region CreateEquGroupDropItem
        private void CreateEquGroupDropItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Give Empty Item
            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = this.GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString();
            Item.Value = "";
            this.Input_EquGroup.Items.Add(Item);
            #endregion

            #region Process String
            sql = @" SELECT EquGroup.EquGrpNo, EquGroup.EquGrpName 
                     FROM dbo.B01_EquGroup AS EquGroup
					 INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroup.EquGrpID
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID
                     INNER JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID ";
            #endregion

            #region DataAuth
            if (wheresql != "") wheresql += " AND ";
            wheresql += " (SysUser.UserID = ? )";
            liSqlPara.Add("S:" + Session["UserID"].ToString());
            #endregion

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY EquGroup.EquGrpNo ";

            oAcsDB.GetDataTable("DropListItem", sql, liSqlPara, out dt);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new System.Web.UI.WebControls.ListItem();
                Item.Text = dr["EquGrpName"].ToString();
                Item.Value = dr["EquGrpNo"].ToString();
                this.Input_EquGroup.Items.Add(Item);
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
        public void Query(string SortExpression, string SortDire,bool _mode=false)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();
            liSqlPara.Add("S:"+ Input_EquGroup.SelectedValue);
            this.EquGrpName = oAcsDB.GetStrScalar("SELECT EquGrpName FROM B01_EquGroup WHERE EquGrpNo=?", liSqlPara);
            liSqlPara.Clear();
            #region Process String
            sql = @" SELECT
                     Person.PsnNo, Person.PsnName,
                     ItemList.ItemName AS PsnTypeName,
                     OrgStrucAllData.OrgNameList,
                     Card.CardNo, Card.CardVer
                     FROM dbo.B01_Person AS Person
                     LEFT JOIN dbo.B00_ItemList AS ItemList ON ItemList.ItemNo = Person.PsnType AND ItemList.ItemClass = 'PsnType'
                     LEFT JOIN dbo.B01_OrgStruc AS OrgStruc ON OrgStruc.OrgStrucID = Person.OrgStrucID
                     INNER JOIN OrgStrucAllData('') AS OrgStrucAllData ON OrgStrucAllData.OrgStrucID = OrgStruc.OrgStrucID
                     INNER JOIN B01_Card AS Card ON Card.PsnID = Person.PsnID
                     LEFT JOIN dbo.B01_CardEquGroup AS CardEquGroup ON CardEquGroup.CardID = Card.CardID
                     INNER JOIN dbo.B01_EquGroup AS EquGroup ON EquGroup.EquGrpID = CardEquGroup.EquGrpID 
					 INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroup.EquGrpID
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID
                     INNER JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID";
            
            #region DataAuth
            if (wheresql != "") wheresql += " AND ";
            wheresql += " (SysUser.UserID = ? )";
            liSqlPara.Add("S:" + Session["UserID"].ToString());
            #endregion

            if (!string.IsNullOrEmpty(Input_EquGroup.SelectedValue))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (EquGroup.EquGrpNo = ?) ";
                liSqlPara.Add("S:" + Input_EquGroup.SelectedValue);
            }
            else
                wheresql = " 1 = 0 ";

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("QueryGroupPsn", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            if (_mode)
            {
                if (dt.Rows.Count != 0)
                {
                    ExportExcel(dt);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('無資料可匯出!!');location.href='QueryGroupPsn.aspx';", true);
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


            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("EATCardLog");
            DataTable equData = ProcDt;

            //Title
            ws.Cells[1, 1].Value = this.GetLocalResourceObject("ttPsnNo").ToString();
            ws.Cells[1, 2].Value = this.GetLocalResourceObject("ttPsnName").ToString();
            ws.Cells[1, 3].Value = this.GetLocalResourceObject("ttPsnType").ToString();
            ws.Cells[1, 4].Value = this.GetLocalResourceObject("ttDeptName").ToString();
            ws.Cells[1, 5].Value = this.GetLocalResourceObject("ttCardNo").ToString();
            ws.Cells[1, 6].Value = this.GetLocalResourceObject("ttVersion").ToString();
            //Content
            for (int i = 0, iCount = equData.Rows.Count; i < iCount; i++)
            {
                //for (int j = 2, jCount = equData.Rows[i].ItemArray.Length; j < jCount; j++)
                //{
                //    ws.Cells[i + 2, j - 1].Value = equData.Rows[i].ItemArray[j].ToString().Replace("&nbsp;", "").Trim();
                //}
                ws.Cells[i + 2, 1].Value = equData.Rows[i]["PsnNo"].ToString().Replace("&nbsp;", "").Trim();
                ws.Cells[i + 2, 2].Value = equData.Rows[i]["PsnName"].ToString().Replace("&nbsp;", "").Trim();
                ws.Cells[i + 2, 3].Value = equData.Rows[i]["PsnTypeName"].ToString().Replace("&nbsp;", "").Trim();
                ws.Cells[i + 2, 4].Value = equData.Rows[i]["OrgNameList"].ToString().Replace("&nbsp;", "").Trim();
                ws.Cells[i + 2, 5].Value = equData.Rows[i]["CardNo"].ToString().Replace("&nbsp;", "").Trim();
                ws.Cells[i + 2, 6].Value = equData.Rows[i]["CardVer"].ToString().Replace("&nbsp;", "").Trim();                
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
            StringBuilder sb = new StringBuilder(@"<html><style> .DataBar
                {
                    border-bottom-style:solid; border-bottom-width:1px; border-bottom-color:Black;width:70px;font-size:12pt
                }
                span {
                    background-color:#ffcc00;
                    display:-moz-inline-box;
                    display:inline-block;
                    width:150px;
                }
                </style>
                <body style='font-size:12pt;margin-left:0px;margin-right:0px'>");
            int PageSize = 32;
            if (ProcDt.Rows.Count > 0)
            {
                var PageData = ProcDt.AsEnumerable().ToPagedList(1, PageSize);
                for (int p = 1; p <= PageData.PageCount; p++)
                {
                    sb.Append("<div style='width:100%;text-align:center;font-size:14pt'>設備群組人員名單</div>");
                    sb.Append(string.Format("<table style='width:100%;font-size:12pt'><tr><td style='width:50%'>製表者：{0}/{1}</td></tr></table>", Sa.Web.Fun.GetSessionStr(this, "UserID"), Sa.Web.Fun.GetSessionStr(this, "UserName")));
                    sb.Append(string.Format("<table style='width:100%;font-size:12pt'><tr><td style='width:50%'>製表時間：{0:yyyy/MM/dd HH:mm:ss}</td></tr></table>", DateTime.Now));
                    sb.Append(string.Format("<table style='width:100%;font-size:12pt'><tr><td style='width:75%'>頁數：{0}/{1}</td><td style='width:25%'>設備群組：{2}</td></tr></table>", p, PageData.PageCount, this.EquGrpName));
                    sb.Append("<p style='height:2px;font-size:2pt'>&nbsp;</p>");
                    sb.Append(@"<table style='width:100%; border-collapse:collapse;border-spacing:0px 0px;'>");
                    sb.Append("<tr><td class='DataBar' style='width:10%'>人員編號</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>人員姓名</td>");
                    sb.Append("<td class='DataBar' style='width:10%'>身份類型</td>");
                    sb.Append("<td class='DataBar'  style='width:45%'>所在部門</td>");
                    sb.Append("<td class='DataBar'  style='width:15%'>卡號</td>");
                    sb.Append("<td class='DataBar' style='width:15%'>版次</td>");
                    sb.Append("</tr>");
                    foreach (var r in ProcDt.AsEnumerable().ToPagedList(p, PageSize))
                    {
                        sb.Append("<tr><td>" + r["PsnNo"].ToString() + "</td>");
                        sb.Append(string.Format("<td>{0}</td>", r["PsnName"]));
                        sb.Append(string.Format("<td>{0}</td>", r["PsnTypeName"]));
                        sb.Append(string.Format("<td>{0}</td>", r["OrgNameList"]));
                        sb.Append(string.Format("<td>{0}</td>", r["CardNo"]));
                        sb.Append(string.Format("<td>{0}</td>", r["CardVer"]));                        
                        sb.Append("</tr>");
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
            iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 5, 5, 5, 5);
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
            Response.AddHeader("content-disposition", "attachment;filename=QueryGroupPsn.pdf");
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