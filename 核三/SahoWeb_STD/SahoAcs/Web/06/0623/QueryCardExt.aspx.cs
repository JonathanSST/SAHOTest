using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web._06._0623
{
    public partial class QueryCardExt : System.Web.UI.Page
    {
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;

        protected void Page_Load(object sender, EventArgs e)
        {
            #region 註冊SignalR必要的檔案
            // 要使用SignalR + SqlDependency才使用這段。
            //Page.ClientScript.RegisterClientScriptInclude("signalR", "../../../Scripts/jquery.signalR-2.2.1.js");
            //Page.ClientScript.RegisterClientScriptInclude("hubs", "/signalr/hubs");
            #endregion

            #region LoadProcess
            //加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("QueryCardExt", "QueryCardExt.js");

            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(btnQuery);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 300);";
            js = "<script type='text/javascript'>" + js + "</script>";
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);

            // 要使用SignalR + SqlDependency才使用這段。不然用 GetGridViewForButtonAndNoSiganlR() 就夠了。
            // btnQuery.Attributes["onClick"] = "Block(); GetGridViewForButton(); return false;";
            btnQuery.Attributes["onClick"] = "Block(); GetGridViewForButtonAndNoSiganlR(); return false;";

            ExportButton.Attributes["onClick"] = "ExportQuery()";
            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                // 要使用SignalR + SqlDependency才使用這段
                // ClientScript.RegisterStartupScript(GetType(), "Javascript", "ConnectSignalRHub(); ", true);

                #region GridView排序相關
                ViewState["SortExpression"] = "CardNo";
                ViewState["SortDire"] = "ASC";
                #endregion

                #region 取得下拉式選單 ddlCtrlArea 的值
                DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
                DataTable dt = null;

                string strSQL = "SELECT CtrlAreaNo, CtrlAreaName FROM B01_CtrlArea ORDER BY CtrlAreaID";
                oAcsDB.GetDataTable("ddlCtrlArea", strSQL, out dt);

                ddlCtrlArea.Items.Clear();

                ListItem Item = new ListItem();
                Item.Text = this.GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString();
                Item.Value = this.GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString();
                ddlCtrlArea.Items.Add(Item);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        Item = new ListItem();
                        Item.Text = "[" + dr["CtrlAreaNo"].ToString() + "]" + dr["CtrlAreaName"].ToString();
                        Item.Value = dr["CtrlAreaNo"].ToString();
                        ddlCtrlArea.Items.Add(Item);
                    }
                }
                #endregion

                Query("", ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == this.ExportButton.ClientID)
                {
                    Query("ExportButton", ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                }
                else
                {
                    Query("", ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                }

                // 清除 __EVENTTARGET、__EVENTARGUMENT 的值
                Sa.Web.Fun.RunJavaScript(this,
                @" theForm.__EVENTTARGET.value   = '' ;
                   theForm.__EVENTARGUMENT.value = '' ; ");

                // 要使用SignalR + SqlDependency才使用這段
                // 移動目前的GridView Panel的Scroll位置
                //Sa.Web.Fun.RunJavaScript(this, "MoveScroll(); ");
            }

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI", "$.unblockUI();", true);
        }

        #region VerifyRenderingInServerForm
        public override void VerifyRenderingInServerForm(Control control)
        {
            // 修正 'XX'型別 必須置於有 runat=server 的表單標記之中 Override此Methods
        }
        #endregion

        protected void MainGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    int[] HeaderWidth = { 80, 80, 100, 100, 100, 140, 140 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

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
                    GrRow.ClientIDMode = ClientIDMode.Static;
                    GrRow.ID = "GV_Row" + oRow.Row["CardNo"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 84, 84, 104, 104, 104, 144, 144 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 最後讀卡時間
                    if (!string.IsNullOrEmpty(oRow.Row["LastTime"].ToString()))
                    {
                        e.Row.Cells[5].Text = DateTime.Parse(e.Row.Cells[5].Text).ToString("yyyy/MM/dd HH:mm:ss");
                    }
                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    //for (int x = 0; x < e.Row.Cells.Count; x++)
                    //{
                    //    if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                    //        e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    //}
                    //e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 20, true);
                    //e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text, 32, true);
                    //e.Row.Cells[3].Text = LimitText(e.Row.Cells[3].Text, 11, true);
                    //e.Row.Cells[5].Text = LimitText(e.Row.Cells[5].Text, 20, true);
                    //e.Row.Cells[6].Text = LimitText(e.Row.Cells[6].Text, 20, true);
                    //e.Row.Cells[7].Text = LimitText(e.Row.Cells[7].Text, 32, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["CardNo"].ToString() + "', '', '');");
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
                        lbtnPage.ID = "Pages_" + (i).ToString();
                        lbtnPage.Text = (i + 1).ToString();
                        //lbtnPage.CommandName = "Pages";
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
                        }
                    };

                    lbtnNext.Click += delegate (object obj, EventArgs args)
                    {
                        if (gv.PageIndex < gv.PageCount)
                        {
                            gv.PageIndex = gv.PageIndex + 1;
                            MainGridView.DataBind();
                        }
                    };
                    #endregion

                    #region 首末頁
                    lbtnFirst.Click += delegate (object obj, EventArgs args)
                    {
                        gv.PageIndex = 0;
                        MainGridView.DataBind();
                    };

                    lbtnLast.Click += delegate (object obj, EventArgs args)
                    {
                        gv.PageIndex = MainGridView.PageCount;
                        MainGridView.DataBind();
                    };

                    #endregion

                    #endregion

                    #region 顯示總頁數及目前頁數
                    phdPageNumber.Controls.Add(
                        new LiteralControl(string.Format("　{0} / {1}　", pageIndex + 1, pageCount == 0 ? 1 : pageCount)));
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

        protected void MainGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            MainGridView.PageIndex = e.NewPageIndex;
            if (MainGridView.Rows[0].Cells.Count == MainGridView.HeaderRow.Cells.Count)
                MainGridView.DataBind();
        }

        protected void MainGridView_DataBound(object sender, EventArgs e)
        {
            td_showGridView.Attributes["colspan"] = MainGridView.Columns.Count.ToString();
        }

        private void Query(string strFun, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            List<string> liSqlPara = new List<string>();
            DataTable dt = new DataTable();

            string pMgaID = oAcsDB.GetStrScalar("SELECT CtrlAreaID FROM B01_CtrlArea WHERE CtrlAreaNo = ?", new List<string>() { "S:" + ddlCtrlArea.SelectedValue.ToString() });


            string strSQL = @"
                SELECT
                    CE.CardID, 
	                ISNULL(CD.CardNo, '') CardNo, 
                    PN.PsnNo, 
                    ISNULL(PN.PsnName, '') PsnName, 
	                '' DepNo, 
                    '' DepName, 
                    CE.LastTime, 
                    CE.LastDoorNo, 
                    CA.CtrlAreaName,
	                OS.OrgStrucNo 
                FROM B01_CardExt CE
                LEFT JOIN B01_Card CD ON CD.CardID = CE.CardID
                LEFT JOIN B01_Person PN ON PN.PsnID = CD.PsnID
                LEFT JOIN B01_CtrlArea CA ON CA.CtrlAreaNo = CE.CtrlAreaNo
                LEFT JOIN B01_OrgStruc OS ON OS.OrgStrucID = PN.OrgStrucID
                WHERE CE.CtrlAreaNo = ? ORDER BY CE.CardID ";

            liSqlPara.Add("S:" + ddlCtrlArea.SelectedValue.ToString());

            bool isSuccess = oAcsDB.GetDataTable("dtEquGroupList", strSQL, liSqlPara, out dt);

            if (isSuccess)
            {
                #region 處理單位代碼、名稱
                strSQL = @"
                    SELECT DISTINCT OrgID, OrgNo, OrgName, OrgClass  
                    FROM [B01_OrgData] WHERE OrgClass IN ( 'Unit', 'Department')";
                DataTable tmpOrg = new DataTable();

                bool isOk = oAcsDB.GetDataTable("tmpOrgData", strSQL, out tmpOrg);

                if (isOk)
                {
                    #region 判斷以UNIT還是DEPARMENT做標準
                    int intUnit = (int)tmpOrg.Compute("Count(OrgID)", "OrgClass='Unit'");
                    int intDepartment = (int)tmpOrg.Compute("Count(OrgID)", "OrgClass='Department'");

                    string strFlag = "U";
                    if (intUnit > intDepartment)
                    {
                        strFlag = "U";
                    }
                    else if (intUnit < intDepartment)
                    {
                        strFlag = "D";
                    }
                    else
                    {
                        if (intUnit == intDepartment && intUnit == 0)
                        {
                            strFlag = "";
                        }
                        else
                        {
                            strFlag = "D";
                        }
                    }
                    #endregion

                    if (strFlag != "")
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr["OrgStrucNo"].ToString() != "")
                            {
                                string[] strCondiition = new string[] { "\t", ".", "_", ",", "-", "、", "。", "\n" };
                                string[] tmp_OrgStrucNo = dr["OrgStrucNo"].ToString().Split(strCondiition, StringSplitOptions.RemoveEmptyEntries);

                                //string[] tmp_OrgStrucNo = dr["OrgStrucNo"].ToString().Split('_');
                                string strOrgNo = "";

                                for (int j = 0; j <= tmp_OrgStrucNo.GetUpperBound(0); j++)
                                {
                                    if (!tmp_OrgStrucNo[j].Equals(""))
                                    {
                                        if (tmp_OrgStrucNo[j].Substring(0, 1).ToUpper().Equals(strFlag))
                                        {
                                            strOrgNo = tmp_OrgStrucNo[j].Trim();
                                            DataRow[] drs = tmpOrg.Select("OrgNo = '" + strOrgNo + "'");

                                            if (drs.GetUpperBound(0) != -1)
                                            {
                                                foreach (DataRow dr1 in drs)
                                                {
                                                    dr["DepNo"] = dr1["OrgNo"];
                                                    dr["DepName"] = dr1["OrgName"];
                                                }
                                            }

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion

                hDataRowCount.Value = dt.Rows.Count.ToString();

                // 處理排序
                dt.DefaultView.Sort = SortExpression + " " + SortDire;
                DataTable dtResult = new DataTable();
                dtResult = dt.DefaultView.ToTable();

                if (strFun == "ExportButton")
                {
                    // 匯出報表到EXCEL
                    ExportExcel(dtResult);
                }
                else
                {
                    GirdViewDataBind(this.MainGridView, dtResult);
                    UpdatePanel1.Update();
                }
            }
        }

        public void GirdViewDataBind(GridView ProcessGridView, DataTable dt)
        {
            if (dt.Rows.Count != 0)
            {
                //Gridview中有資料
                ProcessGridView.DataSource = dt;
                ProcessGridView.DataBind();
            }
            else
            {
                //Gridview中沒有資料
                dt.Rows.Add(dt.NewRow());
                ProcessGridView.DataSource = dt;
                ProcessGridView.DataBind();

                int columnCount = ProcessGridView.Rows[0].Cells.Count;
                ProcessGridView.Rows[0].Cells.Clear();
                ProcessGridView.Rows[0].Cells.Add(new TableCell());
                ProcessGridView.Rows[0].Cells[0].ColumnSpan = columnCount;
                ProcessGridView.Rows[0].Cells[0].Text = GetGlobalResourceObject("Resource", "NonData").ToString();
            }
        }

        #region ExportExcel
        public void ExportExcel(DataTable ProcDt)
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("CtrlArea");
            DataTable dt = ProcDt;

            // 把不必要的欄位去掉
            dt.Columns.Remove("CardID");
            dt.Columns.Remove("OrgStrucNo");

            //Title
            ws.Cells[1, 1].Value = "卡片號碼";
            ws.Cells[1, 2].Value = "人員編號";
            ws.Cells[1, 3].Value = "人員姓名";
            ws.Cells[1, 4].Value = "部門編號";
            ws.Cells[1, 5].Value = "部門名稱";
            ws.Cells[1, 6].Value = "最後讀卡時間";
            ws.Cells[1, 7].Value = "最後讀卡位置";
            ws.Cells[1, 8].Value = "管制區名稱";

            //Content
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Rows[i].ItemArray.Length; j++)
                {
                    ws.Cells[i + 2, j + 1].Value = dt.Rows[i].ItemArray[j].ToString().Replace("&nbsp;", "").Trim();
                }
            }

            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=CardLog.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }
        #endregion

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
            Query("", ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }
    }
}