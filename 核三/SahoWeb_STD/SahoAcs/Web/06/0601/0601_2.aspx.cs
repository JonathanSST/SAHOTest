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
using System.Web.UI.WebControls;
using Sa.DB;
using OfficeOpenXml;
using SahoAcs.DBModel;
using SahoAcs.DBClass;
using DapperDataObjectLib;
using PagedList;
//using iTextSharp.text;
using iTextSharp;
using iTextSharp.tool.xml;
using iTextSharp.text.pdf;


namespace SahoAcs
{
    public partial class _0601_2 : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 100, _datacount = 1, _pageindex = 0, _pagecount = 0;
        string _sqlcommand,sPsnID;
        Hashtable TableInfo;
        DataTable CardLogTable = null;
        List<CardLogModel> cardlogs = new List<CardLogModel>();
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
       
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.QueryButton);
            oScriptManager.RegisterAsyncPostBackControl(this.ADVQueryShowButton);
            oScriptManager.RegisterAsyncPostBackControl(this.ADVQueryButton);
            oScriptManager.RegisterAsyncPostBackControl(this.ADVCloseButton);
         
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);           
            ClientScript.RegisterClientScriptInclude("0601_2", "0601_2.js?jsdate=" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案

            #region 註冊主頁Button動作
            QueryButton.Attributes["onClick"] = "SelectState(); return false;";
            ADVQueryShowButton.Attributes["onClick"] = "CallAdvancedQuery(); return false;";
            //ViewButton.Attributes["onClick"] = "CallShowLogDetail(); return false;";
            #endregion

            #region 註冊pop1頁Button動作
            ImgCloseButton1.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            popB_Cancel.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            #endregion

            #region 註冊pop2頁Button動作
            ImgCloseButton2.Attributes["onClick"] = "SetMode(''); CancelTrigger2.click(); return false;";
            ADVQueryButton.Attributes["onClick"] = "AVDQuery(); CancelTrigger2.click(); return false;";
            ADVCloseButton.Attributes["onClick"] = "SetMode(''); CancelTrigger2.click(); return false;";
            #endregion


            Pub.SetModalPopup(ModalPopupExtender1, 1);
            Pub.SetModalPopup(ModalPopupExtender2, 2);
            this.MainGridView.PageSize = _pagesize;
            #endregion

            #region Check Person Data
            this.sPsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
            if (!this.sPsnID.Equals(""))
            {
                this.ShowPsnInfo1.Visible = false;
                this.ShowPsnInfo2.Visible = false;
                //this.ShowPsnInfo3.Visible = false;
            }
            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion

                CreateDropDownList_DepItem();
                CreateDropDownList_EquItem();
                //CreateDropDownList_LogStatusItem();
                CreateDropDownList_ADVLogStatusItem();

                ViewState["query_CardNo_PsnName"] = "";
                ViewState["query_LogStatus"] = "";
                ViewState["query_ADVCardTimeSDate"] = "";
                ViewState["query_ADVCardTimeEDate"] = "";
                ViewState["query_ADVLogTimeSDate"] = "";
                ViewState["query_ADVLogTimeEDate"] = "";
                ViewState["query_ADVDepNoDepName"] = "";
                ViewState["query_ADVDep"] = "";
                ViewState["query_ADVEquNoEquName"] = "";
                ViewState["query_ADVEqu"] = "";
                ViewState["query_ADVPsnNo"] = "";
                ViewState["query_ADVPsnNameCardNo"] = "";
                ViewState["query_ADVLogStatus"] = "";
                ViewState["SortExpression"] = "CardTime";
                ViewState["SortDire"] = "ASC";
                ViewState["query_custtype"] = "";

                #region 給開始、結束時間預設值
                DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
                DateTime dtLastCardTime = oAcsDB.GetDateTimeScalar(@"
                    SELECT MAX(CardTime) FROM B01_CardLog WHERE CardTime <= GETDATE()");

                if (dtLastCardTime == DateTime.MinValue)
                {
                    Calendar_CardTimeSDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 00:00:00";
                    Calendar_CardTimeEDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 23:59:59";

                    ADVCalendar_CardTimeSDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 00:00:00";
                    ADVCalendar_CardTimeEDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 23:59:59";
                }
                else
                {
                    Calendar_CardTimeSDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 00:00:00";
                    Calendar_CardTimeEDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 23:59:59";

                    ADVCalendar_CardTimeSDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 00:00:00";
                    ADVCalendar_CardTimeEDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 23:59:59";
                }

                ViewState["query_CardTimeSDate"] = Calendar_CardTimeSDate.DateValue;
                ViewState["query_CardTimeEDate"] = Calendar_CardTimeEDate.DateValue;

                ViewState["query_ADVCardTimeSDate"] = ADVCalendar_CardTimeSDate.DateValue;
                ViewState["query_ADVCardTimeEDate"] = ADVCalendar_CardTimeEDate.DateValue;


                #endregion

                ViewState["query_sMode"] = "Normal";
                ViewState["query_custtype"] = RadioButtonList1.SelectedValue;
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == this.QueryButton.ClientID)
                {
                    ViewState["query_CardTimeSDate"] = this.Calendar_CardTimeSDate.DateValue.ToString();
                    ViewState["query_CardTimeEDate"] = this.Calendar_CardTimeEDate.DateValue.ToString();
                    ViewState["query_CardNo_PsnName"] = this.TextBox_CardNo_PsnName.Text.ToString().Trim();
                    //ViewState["query_LogStatus"] = this.DropDownList_LogStatus.SelectedValuesCSV.ToString();
                    ViewState["query_custtype"] = this.RadioButtonList1.SelectedValue.ToString();
                    ViewState["query_sMode"] = "Normal";
                    this.hQueryMode.Value = "";
                    if (this.sPsnID == "")
                        Query(ViewState["query_sMode"].ToString(), ViewState["query_custtype"].ToString(), false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    else
                        QueryByPerson(ViewState["query_sMode"].ToString(), false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                }
                else if (sFormTarget == this.ADVQueryButton.ClientID)
                {
                    ViewState["query_ADVCardTimeSDate"] = this.ADVCalendar_CardTimeSDate.DateValue.ToString();
                    ViewState["query_ADVCardTimeEDate"] = this.ADVCalendar_CardTimeEDate.DateValue.ToString();
                    ViewState["query_ADVLogTimeSDate"] = this.ADVCalendar_LogTimeSDate.DateValue.ToString();
                    ViewState["query_ADVLogTimeEDate"] = this.ADVCalendar_LogTimeEDate.DateValue.ToString();
                    ViewState["query_ADVDepNoDepName"] = this.ADVTextBox_DepNoDepName.Text.ToString().Trim();
                    ViewState["query_ADVDep"] = this.ADVDropDownList_Dep.SelectedValuesCSV.ToString();
                    ViewState["query_ADVEquNoEquName"] = this.ADVTextBox_EquNoEquName.Text.ToString().Trim();
                    ViewState["query_ADVEqu"] = this.ADVDropDownList_Equ.SelectedTextCSV.ToString();
                    ViewState["query_ADVPsnNo"] = this.ADVTextBox_PsnNo.Text.ToString().Trim();
                    ViewState["query_ADVPsnNameCardNo"] = this.ADVTextBox_PsnNameCardNo.Text.ToString().Trim();
                    ViewState["query_ADVLogStatus"] = this.ADVDropDownList_LogStatus.SelectedValuesCSV.ToString();
                    ViewState["query_custtype"] = this.RadioButtonList1.SelectedValue.ToString();
                    ViewState["query_sMode"] = "ADV";
                    this.hQueryMode.Value = "ADV";
                    Query(ViewState["query_sMode"].ToString(), ViewState["query_custtype"].ToString(), false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                }
                else
                {
                    string[] arrControl = sFormTarget.Split('$');

                    if (arrControl.Length == 5)
                    {
                        //_pageindex = Convert.ToInt32(arrControl[4].Split('_')[1]);
                        if (this.sPsnID == "")
                            Query(ViewState["query_sMode"].ToString(), ViewState["query_custtype"].ToString(), false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        else
                            QueryByPerson(ViewState["query_sMode"].ToString(), false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    }
                }
            }
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
                    int[] HeaderWidth = { 150, 120, 120, 80, 120,120, 40, 100, 100, 100};
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
                    GrRow.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    GrRow.ID = "GV_Row" + oRow["RecordID"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 154, 124, 124, 84, 124, 124, 44, 104, 104, 104 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 讀卡時間
                    if (!string.IsNullOrEmpty(oRow.Row["CardTime"].ToString()))
                    {
                        e.Row.Cells[0].Text = DateTime.Parse(oRow.Row["CardTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                    }
                   

                    #endregion
                    #region 部門代碼
                    #endregion
                    #region 部門名稱
                    #endregion
                    #region 人員編號
                    #endregion
                    #region 姓名
                    #endregion
                    #region 卡號
                    #endregion
                    #region 版次
                    #endregion
                    #region 設備編號
                    #endregion
                    #region 設備名稱
                    #endregion
                    #region 讀卡結果
                    #endregion
                    #region 讀卡時間
                    if (!string.IsNullOrEmpty(oRow.Row["LogTime"].ToString()))
                    {
                        e.Row.Cells[e.Row.Cells.Count - 1].Text = DateTime.Parse(oRow.Row["LogTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
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
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow["RecordID"].ToString() + "', '', '')");
                    e.Row.Attributes.Add("OnDblclick", "CallShowLogDetail('" + oRow["RecordID"].ToString() + "')");

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
                    //_pagecount = (_datacount % _pagesize) == 0 ? (_datacount / _pagesize) : (_datacount / _pagesize) + 1;
                    //int pageCount = _pagecount;
                    //int pageIndex = _pageindex;
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

                        //lbtnPage = new LinkButton();
                        //lbtnPage.Text = (i + 1).ToString();
                        //lbtnPage.CommandName = "Page";
                        //lbtnPage.CommandArgument = (i + 1).ToString();
                        //lbtnPage.Font.Overline = false;
                        //if (i == pageIndex)
                        //{
                        //    lbtnPage.Font.Bold = true;
                        //    lbtnPage.ForeColor = System.Drawing.Color.White;
                        //    lbtnPage.OnClientClick = "return false;";
                        //}
                        //else
                        //    lbtnPage.Font.Bold = false;

                        //phdPageNumber.Controls.Add(lbtnPage);
                        //phdPageNumber.Controls.Add(new LiteralControl(" "));
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
                            //Query(ViewState["query_sMode"].ToString());
                        }
                    };
                    //if (_pageindex == 0)
                    //{
                    //    lbtnPrev.Enabled = false;
                    //}
                    //else
                    //{
                    //    lbtnPrev.Enabled = true;
                    //    lbtnPrev.ID = "lbtnPrev_" + (_pageindex - 1);
                    //}

                    lbtnNext.Click += delegate (object obj, EventArgs args)
                    {
                        if (gv.PageIndex < gv.PageCount)
                        {
                            gv.PageIndex = gv.PageIndex + 1;
                            MainGridView.DataBind();
                            //Query(ViewState["query_sMode"].ToString());
                        }
                    };
                    //if (_pageindex == _pagecount - 1 || _pagecount == 0)
                    //{
                    //    lbtnNext.Enabled = false;
                    //}
                    //else
                    //{
                    //    lbtnNext.Enabled = true;
                    //    lbtnNext.ID = "lbtnNext_" + (_pageindex + 1);
                    //}
                    #endregion

                    #region 首末頁
                    lbtnFirst.Click += delegate (object obj, EventArgs args)
                    {
                        gv.PageIndex = 0;
                        MainGridView.DataBind();
                        //Query(ViewState["query_sMode"].ToString());
                    };

                    //if (_pageindex == 0 || _pagecount == 0)
                    //{
                    //    lbtnFirst.Enabled = false;
                    //}
                    //else
                    //{
                    //    lbtnFirst.Enabled = true;
                    //    lbtnFirst.ID = "lbtnFirst_" + 0;
                    //}

                    lbtnLast.Click += delegate (object obj, EventArgs args)
                    {
                        gv.PageIndex = MainGridView.PageCount;
                        MainGridView.DataBind();

                        //Query(ViewState["query_sMode"].ToString());
                    };
                    //if (_pageindex == _pagecount - 1 || _pagecount == 0)
                    //{
                    //    lbtnLast.Enabled = false;
                    //}
                    //else
                    //{
                    //    lbtnLast.Enabled = true;
                    //    lbtnLast.ID = "lbtnLast_" + (_pagecount - 1);
                    //}
                    #endregion

                    #endregion

                    #region 顯示總頁數及目前頁數
                    phdPageNumber.Controls.Add(
                        new LiteralControl(string.Format("　{0} / {1}　", pageIndex + 1, pageCount == 0 ? 1 : pageCount)));
                    phdPageNumber.Controls.Add(new LiteralControl("  "));
                    #endregion

                    #region 顯示總筆數
                    phdPageNumber.Controls.Add(
                        new LiteralControl(string.Format(this.GetGlobalResourceObject("Resource", "lblGvCounts").ToString(), hDataRowCount.Value)));
                    phdPageNumber.Controls.Add(new LiteralControl("  "));
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
            Query(ViewState["query_sMode"].ToString(), ViewState["query_custtype"].ToString(),false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }
        #endregion

        #region VerifyRenderingInServerForm
        public override void VerifyRenderingInServerForm(Control control)
        {
            // 修正 'XX'型別 必須置於有 runat=server 的表單標記之中 Override此Methods
        }
        #endregion

        #region ExportButton_Click
        protected void ExportButton_Click(object sender, EventArgs e)
        {
            Query(ViewState["query_sMode"].ToString(), ViewState["query_custtype"].ToString(), true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }
        #endregion


        #endregion

        #region Method


        bool IsEnglishLetter(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        }


        public string ConvertId(string PsnNo)
        {
            byte[] array = new byte[1];   //定義一組數組array
            array = System.Text.Encoding.ASCII.GetBytes(PsnNo.Substring(0, 1)); //string轉換的字母
            int asciicode = (short)(array[0]) - 64;
            return string.Format("{0:00}", asciicode) + PsnNo.Substring(1, PsnNo.Length - 1);
        }



        #region Query

        public void Query(string sMode, string custType, bool bMode, string SortExpression, string SortDire)
        {
            string sql = "", wheresql = "", LogStatusSql = "", DepSql = "", EquSql = "";
            string[] ArrayLogStatus, ArrayDep, ArrayEqu;
            List<string> liSqlPara = new List<string>();
            ViewState["query_sMode"] = sMode;
            ViewState["query_custtype"] = custType;
            string tmp = "";

            #region Process String

            string sqlorgjoin = @"SELECT OrgData.OrgID FROM OrgStrucAllData('Department') AS OrgData
                INNER JOIN B01_MgnOrgStrucs ON B01_MgnOrgStrucs.OrgStrucID = OrgData.OrgStrucID
                INNER JOIN B00_SysUserMgns ON B00_SysUserMgns.MgaID = B01_MgnOrgStrucs.MgaID  
                WHERE B00_SysUserMgns.UserID = @UserID 
                GROUP BY OrgData.OrgID ";
            var orgdata = this.odo.GetQueryResult<OrgDataEntity>(sqlorgjoin, new { UserID = this.hideUserID.Value });
            sqlorgjoin = @"SELECT B01_EquData.EquNo, B01_EquData.EquName FROM B01_EquData
                INNER JOIN B01_EquGroupData ON B01_EquGroupData.EquID = B01_EquData.EquID
                INNER JOIN B01_MgnEquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroupData.EquGrpID
                INNER JOIN B00_SysUserMgns ON B00_SysUserMgns.MgaID = B01_MgnEquGroup.MgaID
                WHERE B00_SysUserMgns.UserID = @UserID GROUP BY B01_EquData.EquNo, B01_EquData.EquName";
            var EquDatas = this.odo.GetQueryResult<EquGroupData>(sqlorgjoin, new { UserID = this.hideUserID.Value });

            switch (custType)
            {
                case "1":
                    sql = @"SELECT * FROM B01_CardLog WHERE EquNo IN @EquList ";
                    break;
                case "2":
                    sql = @"SELECT * FROM B01_CardLog WHERE (EquNo IN @EquList) AND (EquClass='TRT' OR IsAndTrt=1) ";
                    break;
                case "3":

                    sql = @"SELECT * FROM B01_CardLog WHERE (EquNo IN @EquList) AND (EquClass='Door Access' and IsAndTrt=0) ";
                    break;
            }
           
            var logstatus = this.odo.GetQueryResult("SELECT * FROM B00_CardLogState");

            if (sMode == "Normal")
            {
                
                #region 一般查詢
                sql += " AND (CardTime BETWEEN @CardTimeS AND @CardTimeE) ";

               
                if (!string.IsNullOrEmpty(TextBox_CardNo_PsnName.Text))
                {
                    if (IsEnglishLetter(TextBox_CardNo_PsnName.Text.First()))
                        sql += " AND (CardNo = @PsnName) ";
                    else
                        sql += " AND (PsnNo LIKE @PsnName OR PsnName LIKE @PsnName) ";                    
                }

                //if (!string.IsNullOrEmpty(ViewState["query_LogStatus"].ToString().Trim()))
                //{
                //    sql += " AND ( LogStatus IN ( " + ViewState["query_LogStatus"].ToString() + " ) ) ";                    
                //}
                //sql += " ORDER BY CardTime ";

                if (!string.IsNullOrEmpty(TextBox_CardNo_PsnName.Text)) {
                    if (IsEnglishLetter(TextBox_CardNo_PsnName.Text.First()))
                        tmp = ConvertId(TextBox_CardNo_PsnName.Text.Trim());
                    else
                        tmp = "%" + TextBox_CardNo_PsnName.Text.Trim() + "%";
                }

                this.cardlogs = this.odo.GetQueryResult<CardLogModel>(sql, new
                {
                    EquList = EquDatas.Select(i => i.EquNo),
                    PsnName = tmp,
                    //EquNo = this.QueryLogType.SelectedValue + "%",
                    CardTimeS = ViewState["query_CardTimeSDate"].ToString(),
                    CardTimeE = ViewState["query_CardTimeEDate"].ToString()
                }).ToList();
                       
                #endregion
            }
            else if (sMode == "ADV")
            {
                #region 進階查詢
                //sql += " AND EquNo LIKE @EquType ";

                if (!string.IsNullOrEmpty(ViewState["query_ADVCardTimeSDate"].ToString()))
                {
                    sql += " AND (CardTime >= @CardTimeS ) ";
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVCardTimeEDate"].ToString()))
                {
                    sql += " AND (CardTime <= @CardTimeE ) ";
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVLogTimeSDate"].ToString()))
                {                    
                    sql += " AND (LogTime >= @LogTimeS ) ";                    
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVLogTimeEDate"].ToString()))
                {                    
                    sql += " AND (LogTime <= @LogTimeE ) ";                 
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVDepNoDepName"].ToString()))
                {                    
                    sql += " AND ( DepID LIKE @DepName OR DepName LIKE @DepName ) ";       
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVDep"].ToString().Trim()))
                {
                    sql += " AND ( DepID IN @ADVDep ) AND (DepName IS NOT NULL  OR DepName !='') ";
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVEquNoEquName"].ToString()))
                {                    
                    sql += " AND ( EquNo LIKE @EquName OR EquName LIKE @EquName ) ";
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVEqu"].ToString().Trim()))
                {                                 
                   sql += " AND ( EquName IN @ADVEqu )";
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVPsnNo"].ToString()))
                {                    
                    sql += " AND ( PsnNo LIKE @PsnNo ) ";   
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVPsnNameCardNo"].ToString()))
                {                    
                    sql += " AND (PsnNo LIKE @PsnName OR PsnName LIKE @PsnName OR CardNo LIKE @PsnName) ";
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVLogStatus"].ToString().Trim()))
                {                                     
                    sql += " AND ( LogStatus IN @ADVLogStatus )";
                }
                #endregion

                //sql += " ORDER BY CardTime ";

                this.cardlogs = this.odo.GetQueryResult<CardLogModel>(sql, new
                {
                    EquList = EquDatas.Select(i => i.EquNo),
                    PsnName = "%" + ViewState["query_ADVPsnNameCardNo"].ToString().Trim() + "%",
                    PsnNo = "%" + ViewState["query_ADVPsnNo"].ToString() + "%",
                    EquName = "%" + ViewState["query_ADVEquNoEquName"].ToString() + "%",
                    //EquType = this.ADV_LogType.SelectedValue + "%",
                    CardTimeS = ViewState["query_ADVCardTimeSDate"].ToString(),
                    CardTimeE = ViewState["query_ADVCardTimeEDate"].ToString(),
                    DepName="%"+ViewState["query_ADVDepNoDepName"].ToString()+"%",
                    LogTimeS = ViewState["query_ADVLogTimeSDate"].ToString(),
                    LogTimeE=ViewState["query_ADVLogTimeEDate"].ToString(),
                    ADVDep=ViewState["query_ADVDep"].ToString().Split(','),
                    ADVEqu=ViewState["query_ADVEqu"].ToString().Split(','),
                    ADVLogStatus=ViewState["query_ADVLogStatus"].ToString().Split(',')
                }).ToList();
                Session["AdvData"] = this.cardlogs;
                //if (this.cardlogs.Count > 0)
                //{
                //    this.cardlogs.ForEach(i => i.LogStatus = logstatus.Where(j => j.Code == Convert.ToInt32(i.LogStatus)).FirstOrDefault().StateDesc);
                //}
            }
            if (this.cardlogs.Count > 0)
            {
                this.cardlogs.ForEach(i =>
                {
                    i.LogStatus = logstatus.Where(j => j.Code == Convert.ToInt32(i.LogStatus)).FirstOrDefault().StateDesc;
                });          
            }
            if (SortDire == "DESC")
            {
                this.cardlogs = this.cardlogs.OrderByField(SortExpression, false).ToList();
            }
            else
            {
                this.cardlogs = this.cardlogs.OrderByField(SortExpression, true).ToList();
            }
            // (一)先用 _sqlcommand 得到 dtTmp，然後得到總筆數            
            hDataRowCount.Value = this.cardlogs.Count.ToString();

            if (bMode == true)
            {
                //oAcsDB.GetDataTable("CardLog", _sqlcommand, liSqlPara, out CardLogTable);
                CardLogTable = OrmDataObject.IEnumerableToTable(this.cardlogs);
            }
            else
            {
                //CardLogTable = OrmDataObject.IEnumerableToTable(this.cardlogs.ToPagedList(_pageindex+1,this._pagesize));
                CardLogTable = OrmDataObject.IEnumerableToTable(this.cardlogs);
            }
            #endregion

            if (bMode == true)
            {
                if (CardLogTable.Rows.Count != 0)
                {
                    ExportExcel(CardLogTable);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('無資料可匯出!!');location.href='0601_2.aspx';", true);
                }
            }
            else
            {
                GirdViewDataBind(this.MainGridView, CardLogTable);
                UpdatePanel1.Update();
            }
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI", "$.unblockUI();", true);
        }

        public void QueryByPerson(string sMode, bool bMode, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "", LogStatusSql = "", DepSql = "", EquSql = "";
            string[] ArrayLogStatus, ArrayDep, ArrayEqu;
            bool bQueryCondition = false;
            List<string> liSqlPara = new List<string>();
            ViewState["query_sMode"] = sMode;

            #region Process String

            string strUnionTB = Pub.ReturnNewNestedSerachSQL("0622", ViewState["query_CardTimeSDate"].ToString(), ViewState["query_CardTimeEDate"].ToString());

            if (SortExpression == "EquNo" || SortExpression == "EquName")
                SortExpression = "B01_EquData." + SortExpression;
            sql = @" SELECT ROW_NUMBER() OVER(ORDER BY " + SortExpression + " " + SortDire + @") AS NewIDNum,
                B01_CardLog.RecordID, B01_CardLog.CardTime,
                B01_CardLog.DepName, B01_CardLog.PsnNo, B01_CardLog.PsnName,
                B01_CardLog.CardNo, B01_CardLog.CardVer, B01_CardLog.ReaderNo,
                B01_EquData.EquNo, B01_EquData.EquName,
                B00_CardLogState.StateDesc AS LogStatus, B01_CardLog.LogTime
                FROM " + strUnionTB + " AS B01_CardLog " +
                @"INNER JOIN B00_CardLogState ON B00_CardLogState.Code = B01_CardLog.LogStatus
                INNER JOIN (SELECT B01_EquData.EquNo, B01_EquData.EquName FROM B01_EquData
                INNER JOIN B01_EquGroupData ON B01_EquGroupData.EquID = B01_EquData.EquID
                INNER JOIN B01_MgnEquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroupData.EquGrpID
                INNER JOIN B00_SysUserMgns ON B00_SysUserMgns.MgaID = B01_MgnEquGroup.MgaID
                GROUP BY B01_EquData.EquNo, B01_EquData.EquName) AS B01_EquData ON B01_EquData.EquNo = B01_CardLog.EquNo 
                INNER JOIN (SELECT OrgData.OrgID FROM OrgStrucAllData('Department') AS OrgData
                INNER JOIN B01_MgnOrgStrucs ON B01_MgnOrgStrucs.OrgStrucID = OrgData.OrgStrucID
                INNER JOIN B00_SysUserMgns ON B00_SysUserMgns.MgaID = B01_MgnOrgStrucs.MgaID                  
                GROUP BY OrgData.OrgID) AS B01_OrgData ON B01_OrgData.OrgID = B01_CardLog.DepID  
                AND PsnNo=(SELECT TOP 1 PsnNo FROM B01_Person WHERE PsnID=?)";

            #region DataAuth
            liSqlPara.Add("S:" + this.sPsnID);            
            #endregion

            if (sMode == "Normal")
            {
                #region 一般查詢

                if (!string.IsNullOrEmpty(ViewState["query_CardTimeSDate"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_CardLog.CardTime >= ? ) ";
                    liSqlPara.Add("D:" + ViewState["query_CardTimeSDate"].ToString());
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_CardTimeEDate"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_CardLog.CardTime <= ? ) ";
                    liSqlPara.Add("D:" + ViewState["query_CardTimeEDate"].ToString());
                    bQueryCondition = true;
                }
               
                if (!string.IsNullOrEmpty(ViewState["query_LogStatus"].ToString().Trim()))
                {
                    ArrayLogStatus = ViewState["query_LogStatus"].ToString().Split(',');
                    for (int i = 0; i < ArrayLogStatus.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(LogStatusSql)) LogStatusSql += ",";
                        LogStatusSql += "?";
                        liSqlPara.Add("S:" + ArrayLogStatus[i].ToString());
                        bQueryCondition = true;
                    }
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B00_CardLogState.StateDesc IN ( " + LogStatusSql + " ) ) ";
                    bQueryCondition = true;
                }
                #endregion
            }
            else if (sMode == "ADV")
            {
                #region 進階查詢

                if (!string.IsNullOrEmpty(ViewState["query_ADVCardTimeSDate"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_CardLog.CardTime >= ? ) ";
                    liSqlPara.Add("D:" + ViewState["query_ADVCardTimeSDate"].ToString());
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVCardTimeEDate"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_CardLog.CardTime <= ? ) ";
                    liSqlPara.Add("D:" + ViewState["query_ADVCardTimeEDate"].ToString());
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVLogTimeSDate"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_CardLog.LogTime >= ? ) ";
                    liSqlPara.Add("S:" + ViewState["query_ADVLogTimeSDate"].ToString());
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVLogTimeEDate"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_CardLog.LogTime <= ? ) ";
                    liSqlPara.Add("S:" + ViewState["query_ADVLogTimeEDate"].ToString());
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVDepNoDepName"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_CardLog.DepID LIKE ? OR B01_CardLog.DepName LIKE ? ) ";
                    liSqlPara.Add("S:" + "%" + ViewState["query_ADVDepNoDepName"].ToString() + "%");
                    liSqlPara.Add("S:" + "%" + ViewState["query_ADVDepNoDepName"].ToString() + "%");
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVDep"].ToString().Trim()))
                {
                    ArrayDep = ViewState["query_ADVDep"].ToString().Split(',');
                    for (int i = 0; i < ArrayDep.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(DepSql)) DepSql += ",";
                        DepSql += "?";
                        liSqlPara.Add("S:" + ArrayDep[i].ToString());
                    }
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_CardLog.DepName IN ( " + DepSql + " ) )";
                    bQueryCondition = true;
                }


                if (!string.IsNullOrEmpty(ViewState["query_ADVEquNoEquName"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_EquData.EquNo LIKE ? OR B01_EquData.EquName LIKE ? ) ";
                    liSqlPara.Add("S:" + "%" + ViewState["query_ADVEquNoEquName"].ToString() + "%");
                    liSqlPara.Add("S:" + "%" + ViewState["query_ADVEquNoEquName"].ToString() + "%");
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVEqu"].ToString().Trim()))
                {
                    ArrayEqu = ViewState["query_ADVEqu"].ToString().Split(',');
                    for (int i = 0; i < ArrayEqu.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(EquSql)) EquSql += ",";
                        EquSql += "?";
                        liSqlPara.Add("S:" + ArrayEqu[i].ToString());
                    }
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_EquData.EquName IN ( " + EquSql + " ) )";
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVPsnNo"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_CardLog.PsnNo LIKE ? ) ";
                    liSqlPara.Add("S:" + "%" + ViewState["query_ADVPsnNo"].ToString() + "%");
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVPsnNameCardNo"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B01_CardLog.PsnNo LIKE ? OR B01_CardLog.PsnName LIKE ? OR B01_CardLog.CardNo LIKE ?) ";
                    liSqlPara.Add("S:" + "%" + ViewState["query_ADVPsnNameCardNo"].ToString() + "%");
                    liSqlPara.Add("S:" + "%" + ViewState["query_ADVPsnNameCardNo"].ToString() + "%");
                    liSqlPara.Add("S:" + "%" + ViewState["query_ADVPsnNameCardNo"].ToString() + "%");
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVLogStatus"].ToString().Trim()))
                {
                    ArrayLogStatus = ViewState["query_ADVLogStatus"].ToString().Split(',');
                    for (int i = 0; i < ArrayLogStatus.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(LogStatusSql)) LogStatusSql += ",";
                        LogStatusSql += "?";
                        liSqlPara.Add("S:" + ArrayLogStatus[i].ToString());
                    }
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( B00_CardLogState.StateDesc IN ( " + LogStatusSql + " ) )";
                    bQueryCondition = true;
                }

                #endregion
            }

            if (bQueryCondition)
            {
                if (wheresql != "")
                    sql += " WHERE ";
            }
            else
            {
                if (wheresql != "")
                {
                    sql += " WHERE 1 = 0 AND ";
                }
                else
                {
                    sql += " WHERE 1 = 0 ";
                }
            }

            _sqlcommand = sql += wheresql;

            //_datacount = oAcsDB.DataCount(_sqlcommand, liSqlPara);
            //hDataRowCount.Value = _datacount.ToString();

            // (一)先用 _sqlcommand 得到 dtTmp，然後得到總筆數
            DataTable dtTmp = new DataTable();
            oAcsDB.GetDataTable("ALL", _sqlcommand, liSqlPara, out dtTmp);
            _datacount = dtTmp.Rows.Count;
            hDataRowCount.Value = _datacount.ToString();

            if (bMode == true)
            {
                oAcsDB.GetDataTable("CardLog", _sqlcommand, liSqlPara, out CardLogTable);
            }
            else
            {
                //// old
                //CardLogTable = oAcsDB.PageData(_sqlcommand, liSqlPara, "NewIDNum", _pageindex + 1, _pagesize);

                //// new 
                //CardLogTable = oAcsDB.PageData(sql, liSqlPara, _pageindex, _pagesize);

                // (二)再用 DefaultView 設條件 strCondition，回填 CardLogTable，然後DISPLAY
                string strCondition =
                    @"NewIDNum >= " + Convert.ToString((_pagesize * _pageindex + 1)) +
                    " AND NewIDNum <= " + Convert.ToString((_pagesize * (_pageindex + 1)));

                dtTmp.DefaultView.RowFilter = strCondition;
                CardLogTable = dtTmp.DefaultView.ToTable();
            }
            #endregion

            if (bMode == true)
            {
                if (CardLogTable.Rows.Count != 0)
                {
                    ExportExcel(CardLogTable);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('無資料可匯出!!');location.href='QueryCardLog.aspx';", true);
                }
            }
            else
            {
                GirdViewDataBind(this.MainGridView, CardLogTable);
                UpdatePanel1.Update();
            }

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI", "$.unblockUI();", true);
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

            string strUnionTB = Pub.ReturnNewNestedSerachSQL("0622_LoadData", "", "");

            sql = @" SELECT 
                     B01_CardLog.CardTime,CardLogState.StateDesc AS LogStatus,
                     OrgStrucAllData.OrgStrucID AS DepID, OrgStrucAllData.OrgName AS DepName,
                     Person.PsnNo, Person.PsnName,
                     Card.CardNo, Card.CardVer,
                     EquData.EquNo, EquData.EquName,
                     B01_CardLog.LogTime
                     FROM B01_CardLog AS B01_CardLog " +
                     @"INNER JOIN dbo.B01_Card AS Card ON Card.CardNo = B01_CardLog.CardNo AND (Card.CardVer = B01_CardLog.CardVer OR (Card.CardVer IS NULL AND B01_CardLog.CardVer IS NULL ))
                     INNER JOIN dbo.B01_Person AS Person ON Person.PsnID = Card.PsnID
                     INNER JOIN dbo.B01_EquData AS EquData ON EquData.EquNo = B01_CardLog.EquNo
                     INNER JOIN OrgStrucAllData('Department') AS OrgStrucAllData ON OrgStrucAllData.OrgStrucID = Person.OrgStrucID
                     INNER JOIN B00_CardLogState AS CardLogState ON CardLogState.Code = B01_CardLog.LogStatus 
                     INNER JOIN B01_MgnOrgStrucs AS MgnOrgStrucs ON MgnOrgStrucs.OrgStrucID = OrgStrucAllData.OrgStrucID
                     INNER JOIN B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID
                     INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnOrgStrucs.MgaID AND SysUserMgns.MgaID = MgnEquGroup.MgaID
                     WHERE B01_CardLog.RecordID = ? ";
            sql = @"SELECT 
                     B01_CardLog.CardTime,CardLogState.StateDesc AS LogStatus,
                     OrgStrucAllData.OrgStrucID AS DepID, OrgStrucAllData.OrgName AS DepName,
                     Person.PsnNo, Person.PsnName,
                     Card.CardNo, Card.CardVer,
                     EquData.EquNo, EquData.EquName,
                     B01_CardLog.LogTime
                     FROM B01_CardLog AS B01_CardLog 
                    INNER JOIN dbo.B01_Card AS Card ON Card.CardNo = B01_CardLog.CardNo AND (Card.CardVer = B01_CardLog.CardVer OR (Card.CardVer IS NULL AND B01_CardLog.CardVer IS NULL ))
                     INNER JOIN dbo.B01_Person AS Person ON Person.PsnID = Card.PsnID
                     INNER JOIN dbo.B01_EquData AS EquData ON EquData.EquNo = B01_CardLog.EquNo
                     INNER JOIN OrgStrucAllData('Department') AS OrgStrucAllData ON OrgStrucAllData.OrgStrucID = Person.OrgStrucID
                     INNER JOIN B00_CardLogState AS CardLogState ON CardLogState.Code = B01_CardLog.LogStatus 
                     WHERE B01_CardLog.RecordID = ? ";
            liSqlPara.Add("S:" + SelectValue.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                TableData = new string[dr.DataReader.FieldCount];
                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                {
                    switch (i)
                    {
                        case 0:
                            TableData[i] = DateTime.Parse(dr.DataReader[i].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                            break;
                        case 10:
                            TableData[i] = DateTime.Parse(dr.DataReader[i].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                            break;
                        default:
                            TableData[i] = dr.DataReader[i].ToString();
                            break;
                    }
                }
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

        #region GirdViewDataBind

        public void GridViewDataBindList(GridView ProcessGridView, List<CardLogModel> paramEntity)
        {
            if (paramEntity.Count != 0)//Gridview中有資料
            {
                ProcessGridView.DataSource = paramEntity;
                ProcessGridView.DataBind();
            }
            else//Gridview中沒有資料
            {
                //dt.Rows.Add(dt.NewRow());
                paramEntity.Add(new DBModel.CardLogModel());
                ProcessGridView.DataSource = paramEntity;
                ProcessGridView.DataBind();

                int columnCount = ProcessGridView.Rows[0].Cells.Count;
                ProcessGridView.Rows[0].Cells.Clear();
                ProcessGridView.Rows[0].Cells.Add(new TableCell());
                ProcessGridView.Rows[0].Cells[0].ColumnSpan = columnCount;
                ProcessGridView.Rows[0].Cells[0].Text = "查無資料";
            }
        }
       
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

        #region CreateDropDownList_LogStatusItem
        //private void CreateDropDownList_LogStatusItem()
        //{
        //    System.Web.UI.WebControls.ListItem Item;
        //    DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
        //    string sql = "", wheresql = "";
        //    DataTable dt;
        //    List<string> liSqlPara = new List<string>();

        //    //DropDownList_LogStatus.Items.Clear();

        //    #region Process String
        //    sql = @" SELECT Code, StateDesc FROM B00_CardLogState AS CardLogState ";
        //    #endregion

        //    oAcsDB.GetDataTable("LogStatusDropItem", sql, liSqlPara, out dt);

        //    if (dt.Rows.Count > 0)
        //    {
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            Item = new System.Web.UI.WebControls.ListItem();
        //            Item.Text = dr["StateDesc"].ToString();
        //            Item.Value = dr["Code"].ToString();
        //            DropDownList_LogStatus.Items.Add(Item);
        //        }
        //    }
        //    else
        //    {
        //        DropDownList_LogStatus.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
        //    }
        //}
        #endregion

        #region CreateDropDownList_ADVLogStatusItem
        private void CreateDropDownList_ADVLogStatusItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            ADVDropDownList_LogStatus.Items.Clear();

            #region Process String
            sql = @" SELECT Code, StateDesc FROM B00_CardLogState AS CardLogState ";
            #endregion

            oAcsDB.GetDataTable("ADVLogStatusDropItem", sql, liSqlPara, out dt);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Item = new System.Web.UI.WebControls.ListItem();
                    Item.Text = dr["StateDesc"].ToString();
                    Item.Value = dr["Code"].ToString();
                    ADVDropDownList_LogStatus.Items.Add(Item);
                }
            }
            else
            {
                ADVDropDownList_LogStatus.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
            }
        }
        #endregion

        #region CreateDropDownList_DepItem
        private void CreateDropDownList_DepItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            ADVDropDownList_Dep.Items.Clear();

            #region Process String
            string sqlouttable = "";
            sql = @" SELECT DISTINCT
                     OrgStrucAllData.OrgID AS DepID, OrgStrucAllData.OrgName AS DepName,UserID
                     FROM  OrgStrucAllData('@Type') AS OrgStrucAllData
                     INNER JOIN B01_MgnOrgStrucs AS MgnOrgStrucs ON MgnOrgStrucs.OrgStrucID = OrgStrucAllData.OrgStrucID
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnOrgStrucs.MgaID  ";
            sqlouttable += "SELECT * FROM (";
            sqlouttable += sql.Replace("@Type", "Department");
            sqlouttable += " UNION ";
            sqlouttable += sql.Replace("@Type", "Unit");
            sqlouttable += " UNION ";
            sqlouttable += sql.Replace("@Type", "Title");
            sqlouttable += ") AS ResultTable ";
            #region DataAuth
            if (wheresql != "") wheresql += " AND ";
            wheresql += " (UserID = ? ) ";
            wheresql += " AND DepID<>'' ";
            liSqlPara.Add("S:" + this.hideUserID.Value);
            #endregion

            #endregion

            sqlouttable += " WHERE " + wheresql + " ORDER BY DepName ";

            oAcsDB.GetDataTable("DepDropItem", sqlouttable, liSqlPara, out dt);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (!dr["DepName"].ToString().Equals(""))
                    {
                        Item = new System.Web.UI.WebControls.ListItem();
                        Item.Text = dr["DepName"].ToString();
                        Item.Value = dr["DepID"].ToString();
                        ADVDropDownList_Dep.Items.Add(Item);
                    }
                    
                }
            }
            else
            {
                ADVDropDownList_Dep.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
            }
        }
        #endregion

        #region CreateDropDownList_EquItem
        private void CreateDropDownList_EquItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            ADVDropDownList_Equ.Items.Clear();

            #region Process String
            sql = @" SELECT DISTINCT
                     EquData.EquNo, EquData.EquName
                     FROM B01_EquData AS EquData
                     INNER JOIN B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID
                     INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID ";

            #region DataAuth
            if (wheresql != "") wheresql += " AND ";
            wheresql += " (SysUserMgns.UserID = ? ) ";
            liSqlPara.Add("S:" + this.hideUserID.Value);
            #endregion

            #endregion

            sql += " WHERE " + wheresql + " ORDER BY EquData.EquNo ";

            oAcsDB.GetDataTable("EquDropItem", sql, liSqlPara, out dt);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Item = new System.Web.UI.WebControls.ListItem();
                    Item.Text = dr["EquName"].ToString();
                    Item.Value = dr["EquNo"].ToString();
                    ADVDropDownList_Equ.Items.Add(Item);
                }
            }
            else
            {
                ADVDropDownList_Equ.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
            }
        }
        #endregion

        #region ExportExcel
        public void ExportExcel(DataTable ProcDt)
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("CardLog");
            DataTable dtCardLog = ProcDt;
            string[] colnamelist = { "CardTime", "DepName", "PsnNo", "PsnName", "CardNo", "CardVer", "EquName", "LogStatus","LogTime" };
            //Title
            ws.Cells[1, 1].Value = "讀卡時間";
            ws.Cells[1, 2].Value = "部門名稱";
            ws.Cells[1, 3].Value = "人員編號";
            ws.Cells[1, 4].Value = "人員姓名";
            ws.Cells[1, 5].Value = "卡號";
            //ws.Cells[1, 6].Value = "臨時卡號";
            ws.Cells[1, 6].Value = "版次";
            //ws.Cells[1, 8].Value = "設備編號";
            ws.Cells[1, 7].Value = "設備名稱";
            ws.Cells[1, 8].Value = "讀卡結果";
            ws.Cells[1, 9].Value = "記錄時間";
            //Content
            for (int i = 0, iCount = dtCardLog.Rows.Count; i < iCount; i++)
            {
                for (int j = 0; j < colnamelist.Length; j++)
                {
                    ws.Cells[i + 2, j + 1].Value = dtCardLog.Rows[i][colnamelist[j]].ToString().Replace("&nbsp;", "").Trim();
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
                    sb.Append("<tr><td>卡號</td>");
                    sb.Append("<td>工號</td>");
                    sb.Append("<td>人員姓名</td>");
                    sb.Append("<td>單位</td>");
                    sb.Append("<td>版次</td>");
                    sb.Append("<td>臨時卡號</td>");
                    sb.Append("<td>讀卡時間</td>");
                    sb.Append("<td>記錄時間</td>");
                    sb.Append("<td>設備編號</td>");
                    sb.Append("<td>設備名稱</td>");
                    sb.Append("<td>讀卡結果</td></tr>");
                    foreach (var r in ProcDt.AsEnumerable().ToPagedList(p, 18))
                    {
                        sb.Append("<tr><td>" + r["CardNo"].ToString() + "</td>");
                        sb.Append(string.Format("<td>{0}</td>", r["PsnNo"]));
                        sb.Append(string.Format("<td>{0}</td>", r["PsnName"]));
                        sb.Append(string.Format("<td>{0}</td>", r["DepName"]));
                        sb.Append(string.Format("<td>{0}</td>", r["CardVer"]));
                        sb.Append(string.Format("<td>{0}</td>", r["TempCardNo"]));
                        sb.Append(string.Format("<td>{0:yyyy/MM/dd HH:mm:ss}</td>", r["CardTime"]));
                        sb.Append(string.Format("<td>{0:yyyy/MM/dd HH:mm:ss}</td>", r["LogTime"]));
                        sb.Append(string.Format("<td>{0}</td>", r["EquNo"]));
                        sb.Append(string.Format("<td>{0}</td>", r["EquName"]));
                        sb.Append(string.Format("<td>{0}</td></tr>", r["LogStatus"]));
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
            PdfWriter writer = PdfWriter.GetInstance(doc, Response.OutputStream);
            PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
            doc.Open();
            XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.UTF8, new SahoAcs.UnicodeFontFactory());
            //將pdfDest設定的資料寫到PDF檔
            PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, writer);
            writer.SetOpenAction(action);
            doc.Close();
            msInput.Close();
            //這裡控制Response回應的格式內容            
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=ReportData.pdf");
            Response.End();
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