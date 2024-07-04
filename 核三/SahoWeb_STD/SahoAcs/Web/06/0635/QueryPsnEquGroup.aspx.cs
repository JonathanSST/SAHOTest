using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using DapperDataObjectLib;

namespace SahoAcs
{
    public partial class QueryPsnEquGroup : Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 30;
        //Hashtable TableInfo = new Hashtable();
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
            js = "<script type='text/javascript'>" + js + "</script>";
            js += "<script src='FillLog.js' type='text/javascript'></script>";
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("QueryPsnEquGroup", "QueryPsnEquGroup.js");//加入同一頁面所需的JavaScript檔案      

            #region 註冊主頁Button動作
            //Input_EquModel.Attributes["onChange"] = "SelectState('" + this.Input_EquModel .ClientID+ "');";
            //AddButton.Attributes["onClick"] = "CallAdd('" + "設備參數定義資料新增" + "','設備型號 必須指定'); return false;";
            //EditButton.Attributes["onClick"] = "CallEdit('" + "設備參數定義資料編輯" + "'); return false;";
            //DeleteButton.Attributes["onClick"] = "CallDelete('" + "設備參數定義資料刪除" + "'); return false;";
            this.QueryButton.Attributes.Add("OnClick", "SelectState();return false;");
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
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion

                ViewState["SortExpression"] = "PsnNo";
                ViewState["SortDire"] = "ASC";
                EmptyCondition();
            }
            else
            {
                Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
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
                    int[] HeaderWidth = { 80, 80, 80,100 };
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
                    GrRow.ClientIDMode = ClientIDMode.Static;
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 84, 84, 84,104};
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
                    e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Left;                    
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

                    e.Row.Cells[0].Text = LimitText(e.Row.Cells[0].Text, 13, true);
                    e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 13, true);
                    e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text, 17, true);
                    e.Row.Cells[3].Text = LimitText(e.Row.Cells[3].Text, 255, true);  
                    #endregion
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
                            MainGridView.DataBind();

                            //Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        }
                    };

                    lbtnNext.Click += delegate(object obj, EventArgs args)
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
                    lbtnFirst.Click += delegate(object obj, EventArgs args)
                    {
                        gv.PageIndex = 0;
                        MainGridView.DataBind();

                        //Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    };

                    lbtnLast.Click += delegate(object obj, EventArgs args)
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
            {
                MainGridView.DataBind();
            }                
        }
        #endregion

        #region QueryButton_Click
        protected void QueryButton_Click(object sender, EventArgs e)
        {
            //if (!this.Query_Psn.Text.Trim().Equals(""))
            //{
            //    Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
            //}
            //else
            //{
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert1", "alert('"+this.GetLocalResourceObject("alertPerson")+"!!!')", true);
            //}            
        }
        #endregion

        #region Excel output

        protected void ExportButton_Click(object sender, EventArgs e)
        {
            Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString(),true);
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
        public void Query(string SortExpression, string SortDire,bool _mode=false)
        {
            if (!_mode)
            {
                //Sa.Web.Fun.RunJavaScript(this.Page, "Block();");
            }
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            OrmDataObject ormDB = new OrmDataObject("MsSql",Pub.GetDapperConnString());

            DataTable dt = new DataTable();
            List<string> liSqlPara = new List<string>();

            var sql1 = @"SELECT A.*,B.OrgName FROM V_PsnCard A 
                                    INNER JOIN OrgStrucAllData('Unit') B ON A.OrgStrucID=B.OrgStrucID
                                     WHERE OrgName LIKE @Key OR PsnNo LIKE @Key OR CardNo LIKE @Key OR PsnName LIKE @Key ORDER BY +" + SortExpression+" "+SortDire;
            List<SahoAcs.DBModel.EquGroupModel> grouplist = ormDB.GetQueryResult<SahoAcs.DBModel.EquGroupModel>(@"SELECT CardID,EquGrpNo,EquGrpName 
            FROM 
	            B01_CardEquGroup A
	            INNER JOIN B01_EquGroup B ON A.EquGrpID=B.EquGrpID").ToList() ;
            List<SahoAcs.DBModel.CardEntity> lists = ormDB.GetQueryResult<SahoAcs.DBModel.CardEntity>(sql1,new {Key=this.Query_Psn.Text+"%"}).ToList();            
            foreach(var o in lists)
            {
                o.EquGrpList = string.Join(",", grouplist.Where(i => i.CardID == o.CardID).Select(i => string.Format("{0}[{1}]",i.EquGrpName,i.EquGrpNo)));
            }
            dt = OrmDataObject.IEnumerableToTable<SahoAcs.DBModel.CardEntity>(lists);
            
            hDataRowCount.Value = dt.Rows.Count.ToString();

            if (_mode)
            {
                if (dt.Rows.Count != 0)
                {
                    ExportExcel(dt);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('無資料可匯出!!');location.href='QueryPsnEquGroup.aspx';", true);
                }
            }
            else
            {
                Sa.Web.Fun.RunJavaScript(this.Page, "$.unblockUI();");
                GirdViewDataBind(this.MainGridView, dt);
                UpdatePanel1.Update();
            }            
        }
        #endregion

        #region ExportExcel add by Sam 20160624
        public void ExportExcel(DataTable ProcDt)
        {
            ExcelPackage pck = new ExcelPackage();
            try
            {                
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("PsnEquGroup");
                DataTable equData = ProcDt;

                //Title
                ws.Cells[1, 1].Value = "人員編號";
                ws.Cells[1, 2].Value = "人員姓名";
                ws.Cells[1, 3].Value = "卡號";
                ws.Cells[1, 4].Value = "單位";
                ws.Cells[1, 5].Value = "設備群組";
                //Content
                for (int i = 0, iCount = equData.Rows.Count; i < iCount; i++)
                {
                   
                    ws.Cells[i + 2, 1].Value = equData.Rows[i]["PsnNo"].ToString().Replace("&nbsp;", "").Trim();
                    ws.Cells[i + 2, 2].Value = equData.Rows[i]["PsnName"].ToString().Replace("&nbsp;", "").Trim();
                    ws.Cells[i + 2, 3].Value = equData.Rows[i]["CardNo"].ToString().Replace("&nbsp;", "").Trim();
                    ws.Cells[i + 2, 4].Value = equData.Rows[i]["OrgName"].ToString().Replace("&nbsp;", "").Trim();
                    ws.Cells[i + 2, 5].Value = equData.Rows[i]["EquGrpList"].ToString().Replace("&nbsp;", "").Trim();                    
                }

                ws.Cells.AutoFitColumns(); //自動欄寬
                
                
                //System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (System.Threading.ThreadAbortException ex)
            {
                Sa.Fun.SaveExceptionLog(ex.GetBaseException());
            }
            finally
            {
                Response.Clear();
                Response.AddHeader("content-disposition", "attachment; filename=PsnEquGroup.xlsx");
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
            }

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
                ProcessGridView.Rows[0].Cells[0].Text = this.GetGlobalResourceObject("Resource","NonData").ToString();
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
        
            dtEmpty.Columns.Add("PsnNo");
            dtEmpty.Columns.Add("PsnName");
            dtEmpty.Columns.Add("CardNo");
            dtEmpty.Columns.Add("EquGrpList");
            dtEmpty.Columns.Add("OrgName");
            GirdViewDataBind(this.MainGridView, dtEmpty);
            UpdatePanel1.Update();
            #endregion
        }

        #endregion
       
    }//end class
}//end namespace