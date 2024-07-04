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

namespace SahoAcs
{
    public partial class QueryCardLogCAPCO : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 100, _datacount = 1, _pageindex = 0, _pagecount = 0;
        string _sqlcommand;
        Hashtable TableInfo;
        DataTable CardLogTable = null;
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
            oScriptManager.RegisterAsyncPostBackControl(this.ViewButton);
            oScriptManager.RegisterAsyncPostBackControl(this.ADVQueryButton);
            oScriptManager.RegisterAsyncPostBackControl(this.ADVCloseButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("QueryCardLog", "QueryCardLogCAPCO.js");//加入同一頁面所需的JavaScript檔案

            #region 註冊主頁Button動作
            QueryButton.Attributes["onClick"]        = "SelectState(); return false;";
            ADVQueryShowButton.Attributes["onClick"] = "CallAdvancedQuery(); return false;";
            ViewButton.Attributes["onClick"]         = "CallShowLogDetail(); return false;";
            dropCtrl.Attributes["onchange"]          = "CheckDrop('Ctrl');";
            dropEquGroup.Attributes["onchange"]      = "CheckDrop('EquGroup');";
            #endregion

            #region 註冊pop1頁Button動作
            ImgCloseButton1.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            popB_Cancel.Attributes["onClick"]     = "SetMode(''); CancelTrigger1.click(); return false;";
            #endregion

            #region 註冊pop2頁Button動作
            ImgCloseButton2.Attributes["onClick"] = "SetMode(''); CancelTrigger2.click(); return false;";
            ADVQueryButton.Attributes["onClick"]  = "AVDQuery(); CancelTrigger2.click(); return false;";
            ADVCloseButton.Attributes["onClick"]  = "SetMode(''); CancelTrigger2.click(); return false;";
            #endregion

            Pub.SetModalPopup(ModalPopupExtender1, 1);
            Pub.SetModalPopup(ModalPopupExtender2, 2);
            this.MainGridView.PageSize = _pagesize;
            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion

                CreateDropDownList_DepItem();
                CreateDropDownList_EquItem();
                CreateDropDownList_LogStatusItem();
                CreateDropDownList_ADVLogStatusItem();

                ViewState["query_CardTimeSDate"]    = "";
                ViewState["query_CardTimeEDate"]    = "";
                ViewState["query_CardNo_PsnName"]   = "";
                ViewState["query_LogStatus"]        = "";

                ViewState["query_ADVCardTimeSDate"] = "";
                ViewState["query_ADVCardTimeEDate"] = "";
                ViewState["query_ADVLogTimeSDate"]  = "";
                ViewState["query_ADVLogTimeEDate"]  = "";
                ViewState["query_ADVDepNoDepName"]  = "";
                ViewState["query_ADVDep"]           = "";
                ViewState["query_ADVEquNoEquName"]  = "";
                ViewState["query_ADVEqu"]           = "";
                ViewState["query_ADVPsnNo"]         = "";
                ViewState["query_ADVPsnNameCardNo"] = "";
                ViewState["query_ADVLogStatus"]     = "";

                ViewState["query_CardLogTable"]     = "";

                ViewState["query_sMode"]            = "Normal";

                Calendar_CardTimeSDate.DateValue = DateTime.Today.AddDays(-1).ToString("yyyy/MM/dd") + " 00:00:00";
                Calendar_CardTimeEDate.DateValue = DateTime.Today.ToString("yyyy/MM/dd") + " 23:59:59";

                DropBind(this.dropCtrl, "CtrlNo", "CtrlName", @"SELECT CtrlNo, CtrlName FROM B01_Controller WHERE CtrlStatus = '1' ORDER BY CtrlName");
                DropBind(this.dropEquGroup, "LogEquGrpID", "LogEquGrpName", @"SELECT LogEquGrpID, LogEquGrpName FROM B01_LogEquGroup GROUP BY LogEquGrpID, LogEquGrpName");

                Query("Normal", false);
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
                    ViewState["query_LogStatus"] = this.DropDownList_LogStatus.SelectedTextCSV.ToString();
                    ViewState["query_sMode"] = "Normal";
                    Query(ViewState["query_sMode"].ToString(), false);
                }
                else if (sFormTarget == this.ADVQueryButton.ClientID)
                {
                    ViewState["query_ADVCardTimeSDate"] = this.ADVCalendar_CardTimeSDate.DateValue.ToString();
                    ViewState["query_ADVCardTimeEDate"] = this.ADVCalendar_CardTimeEDate.DateValue.ToString();
                    ViewState["query_ADVLogTimeSDate"] = this.ADVCalendar_LogTimeSDate.DateValue.ToString();
                    ViewState["query_ADVLogTimeEDate"] = this.ADVCalendar_LogTimeEDate.DateValue.ToString();
                    ViewState["query_ADVDepNoDepName"] = this.ADVTextBox_DepNoDepName.Text.ToString().Trim();
                    ViewState["query_ADVDep"] = this.ADVDropDownList_Dep.SelectedTextCSV.ToString();
                    ViewState["query_ADVEquNoEquName"] = this.ADVTextBox_EquNoEquName.Text.ToString().Trim();
                    ViewState["query_ADVEqu"] = this.ADVDropDownList_Equ.SelectedTextCSV.ToString();
                    ViewState["query_ADVPsnNo"] = this.ADVTextBox_PsnNo.Text.ToString().Trim();
                    ViewState["query_ADVPsnNameCardNo"] = this.ADVTextBox_PsnNameCardNo.Text.ToString().Trim();
                    ViewState["query_ADVLogStatus"] = this.ADVDropDownList_LogStatus.SelectedTextCSV.ToString();

                    ViewState["query_sMode"] = "ADV";
                    Query(ViewState["query_sMode"].ToString(), false);
                }
                else
                {
                    //if (ViewState["query_CardLogTable"] != null && ViewState["query_CardLogTable"].ToString() != "")
                    //{
                    //    GirdViewDataBind(this.MainGridView, (DataTable)ViewState["query_CardLogTable"]);
                    //}
                    string[] arrControl = sFormTarget.Split('$');

                    if (arrControl.Length == 5)
                    {
                        _pageindex = Convert.ToInt32(arrControl[4].Split('_')[1]);
                        Query(ViewState["query_sMode"].ToString(), false);
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
                    int[] HeaderWidth = { 150, 100, 100, 100, 100, 100, 100, 200, 100, 200, 200 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

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
                    GrRow.ID = "GV_Row" + oRow.Row["RecordID"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 153, 104, 104, 104, 104, 104, 104, 204, 104, 204, 204 };
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
                    #region 公司
                    #endregion
                    #region 部門
                    #endregion
                    #region 人員編號
                    #endregion
                    #region 人員
                    #endregion
                    #region 卡號
                    #endregion
                    #region 設備編號
                    #endregion
                    #region 設備名稱
                    #endregion
                    #region 控制器
                    #endregion
                    #region 設備群組
                    #endregion
                    #region 刷卡結果
                    #endregion
                    #region 記錄時間
                    if (!string.IsNullOrEmpty(oRow.Row["LogTime"].ToString()))
                    {
                        e.Row.Cells[11].Text = DateTime.Parse(oRow.Row["LogTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
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
                    
                    e.Row.Cells[1].Text  = LimitText(e.Row.Cells[1].Text, 18, true);
                    e.Row.Cells[2].Text  = LimitText(e.Row.Cells[2].Text, 18, true);
                    e.Row.Cells[7].Text  = LimitText(e.Row.Cells[7].Text, 33, true);
                    e.Row.Cells[9].Text  = LimitText(e.Row.Cells[9].Text, 33, true);
                    e.Row.Cells[10].Text = LimitText(e.Row.Cells[10].Text, 33, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["RecordId"].ToString() + "', '', '')");
                    e.Row.Attributes.Add("OnDblclick", "CallShowLogDetail()");

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
                    PlaceHolder phdCardLogCount = e.Row.FindControl("phdCardLogCount") as PlaceHolder;
                    #endregion

                    #region 顯示頁數及[上下頁、首頁、末頁]處理
                    int showRange = 5; //顯示快捷頁數
                    //int pageCount = gv.PageCount;
                    //int pageIndex = gv.PageIndex;
                    _pagecount = (_datacount % _pagesize) == 0 ? (_datacount / _pagesize) : (_datacount / _pagesize) + 1;
                    int pageCount = _pagecount;
                    int pageIndex = _pageindex;
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
                        lbtnPage.CommandName = "Pages";
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
                    //lbtnPrev.Click += delegate(object obj, EventArgs args)
                    //{
                    //    if (gv.PageIndex > 0)
                    //    {
                    //        gv.PageIndex = gv.PageIndex - 1;
                    //        Query(ViewState["query_sMode"].ToString());
                    //    }
                    //};
                    if (_pageindex == 0)
                    {
                        lbtnPrev.Enabled = false;
                    }
                    else
                    {
                        lbtnPrev.Enabled = true;
                        lbtnPrev.ID = "lbtnPrev_" + (_pageindex - 1);
                    }

                    //lbtnNext.Click += delegate(object obj, EventArgs args)
                    //{
                    //    if (gv.PageIndex < gv.PageCount)
                    //    {
                    //        gv.PageIndex = gv.PageIndex + 1;
                    //        Query(ViewState["query_sMode"].ToString());
                    //    }
                    //};
                    if (_pageindex == _pagecount - 1 || _pagecount == 0)
                    {
                        lbtnNext.Enabled = false;
                    }
                    else
                    {
                        lbtnNext.Enabled = true;
                        lbtnNext.ID = "lbtnNext_" + (_pageindex + 1);
                    }
                    #endregion

                    #region 首末頁
                    //lbtnFirst.Click += delegate(object obj, EventArgs args)
                    //{
                    //    gv.PageIndex = 0;
                    //    Query(ViewState["query_sMode"].ToString());
                    //};
                    if (_pageindex == 0 || _pagecount == 0)
                    {
                        lbtnFirst.Enabled = false;
                    }
                    else
                    {
                        lbtnFirst.Enabled = true;
                        lbtnFirst.ID = "lbtnFirst_" + 0;
                    }

                    //lbtnLast.Click += delegate(object obj, EventArgs args)
                    //{
                    //    gv.PageIndex = MainGridView.PageCount;
                    //    Query(ViewState["query_sMode"].ToString());
                    //};
                    if (_pageindex == _pagecount - 1 || _pagecount == 0)
                    {
                        lbtnLast.Enabled = false;
                    }
                    else
                    {
                        lbtnLast.Enabled = true;
                        lbtnLast.ID = "lbtnLast_" + (_pagecount - 1);
                    }
                    #endregion

                    #endregion

                    #region 顯示總頁數及目前頁數
                    phdPageNumber.Controls.Add(new LiteralControl(string.Format("　{0} / {1}　", pageIndex + 1, pageCount == 0 ? 1 : pageCount)));
                    phdPageNumber.Controls.Add(new LiteralControl("  "));
                    phdCardLogCount.Controls.Add(new LiteralControl("　　總筆數：" + _datacount + "筆"));
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

        #region VerifyRenderingInServerForm
        public override void VerifyRenderingInServerForm(Control control)
        {
            // 修正 'XX'型別 必須置於有 runat=server 的表單標記之中 Override此Methods
        }
        #endregion

        #region ExportButton_Click
        protected void ExportButton_Click(object sender, EventArgs e)
        {
            //ExportExcel((DataTable)ViewState["query_CardLogTable"]);
            Query(ViewState["query_CardLogTable"].ToString(), true);
        }
        #endregion

        #endregion

        #region Method

        #region Query
        public void Query(string sMode, bool bMode)
        {
            bool bQueryCondition = false;
            string sql = "", wheresql = "", LogStatusSql = "", DepSql = "", EquSql = "";
            string[] ArrayLogStatus, ArrayDep, ArrayEqu;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            List<string> liSqlPara = new List<string>();
            ViewState["query_CardLogTable"] = sMode;

            #region Process String
            sql = @" SELECT ROW_NUMBER() OVER(ORDER BY V_CardLogSP.CardTime) AS NewIDNum, 
                V_CardLogSP.RecordID, V_CardLogSP.CardTime, Company.OrgName AS 'CompName', V_CardLogSP.DepName, V_CardLogSP.PsnName, V_CardLogSP.PsnNo, V_CardLogSP.CardNo, 
                B01_EquData.EquNo, B01_EquData.EquName, B00_CardLogState.StateDesc AS LogStatus, V_CardLogSP.CtrlName, V_CardLogSP.LogEquGrpName, V_CardLogSP.LogTime, V_CardLogSP.DepID, V_CardLogSP.CardVer, V_CardLogSP.CtrlNo, V_CardLogSP.LogEquGrpID 
                FROM V_CardLogSP 
                INNER JOIN B00_CardLogState ON B00_CardLogState.Code = V_CardLogSP.LogStatus 
                LEFT JOIN (SELECT OrgIDList, OrgNo, OrgName FROM dbo.OrgStrucAllData('Company')) 
                AS Company ON Company.OrgIDList = V_CardLogSP.OrgStruc 
                LEFT JOIN (SELECT B01_EquData.EquNo, B01_EquData.EquName FROM B01_EquData 
                INNER JOIN B01_EquGroupData ON B01_EquGroupData.EquID = B01_EquData.EquID 
                INNER JOIN B01_MgnEquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroupData.EquGrpID 
                INNER JOIN B00_SysUserMgns ON B00_SysUserMgns.MgaID = B01_MgnEquGroup.MgaID 
                WHERE B00_SysUserMgns.UserID = ? GROUP BY B01_EquData.EquNo, B01_EquData.EquName) AS B01_EquData ON B01_EquData.EquNo = V_CardLogSP.EquNo 
                LEFT JOIN (SELECT OrgData.OrgID FROM OrgStrucAllData('Department') AS OrgData 
                INNER JOIN B01_MgnOrgStrucs ON B01_MgnOrgStrucs.OrgStrucID = OrgData.OrgStrucID 
                INNER JOIN B00_SysUserMgns ON B00_SysUserMgns.MgaID = B01_MgnOrgStrucs.MgaID 
                WHERE B00_SysUserMgns.UserID = ? GROUP BY OrgData.OrgID) AS B01_OrgData ON B01_OrgData.OrgID = V_CardLogSP.DepID ";

            #region DataAuth
            liSqlPara.Add("S:" + this.hideUserID.Value);
            liSqlPara.Add("S:" + this.hideUserID.Value);
            #endregion

            if (sMode == "Normal")
            {
                #region 一般查詢

                if (!string.IsNullOrEmpty(ViewState["query_CardTimeSDate"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( V_CardLogSP.CardTime >= ? ) ";
                    liSqlPara.Add("D:" + ViewState["query_CardTimeSDate"].ToString());
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_CardTimeEDate"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( V_CardLogSP.CardTime <= ? ) ";
                    liSqlPara.Add("D:" + ViewState["query_CardTimeEDate"].ToString());
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_CardNo_PsnName"].ToString().Trim()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( V_CardLogSP.PsnNo LIKE ? OR V_CardLogSP.PsnName LIKE ? OR V_CardLogSP.CardNo LIKE ? ) ";
                    liSqlPara.Add("S:" + "%" + ViewState["query_CardNo_PsnName"].ToString().Trim() + "%");
                    liSqlPara.Add("S:" + "%" + ViewState["query_CardNo_PsnName"].ToString().Trim() + "%");
                    liSqlPara.Add("S:" + "%" + ViewState["query_CardNo_PsnName"].ToString().Trim() + "%");
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
                    wheresql += " ( V_CardLogSP.CardTime >= ? ) ";
                    liSqlPara.Add("D:" + ViewState["query_ADVCardTimeSDate"].ToString());
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVCardTimeEDate"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( V_CardLogSP.CardTime <= ? ) ";
                    liSqlPara.Add("D:" + ViewState["query_ADVCardTimeEDate"].ToString());
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVLogTimeSDate"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( V_CardLogSP.LogTime >= ? ) ";
                    liSqlPara.Add("D:" + ViewState["query_ADVLogTimeSDate"].ToString());
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVLogTimeEDate"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( V_CardLogSP.LogTime <= ? ) ";
                    liSqlPara.Add("D:" + ViewState["query_ADVLogTimeEDate"].ToString());
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVDepNoDepName"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( V_CardLogSP.DepID LIKE ? OR V_CardLogSP.DepName LIKE ? OR Company.OrgNo LIKE ? OR Company.OrgName LIKE ? ) ";
                    liSqlPara.Add("S:" + "%" + ViewState["query_ADVDepNoDepName"].ToString() + "%");
                    liSqlPara.Add("S:" + "%" + ViewState["query_ADVDepNoDepName"].ToString() + "%");
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
                    wheresql += " ( V_CardLogSP.DepName IN ( " + DepSql + " ) )";
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
                    wheresql += " ( V_CardLogSP.PsnNo LIKE ? ) ";
                    liSqlPara.Add("S:" + "%" + ViewState["query_ADVPsnNo"].ToString() + "%");
                    bQueryCondition = true;
                }

                if (!string.IsNullOrEmpty(ViewState["query_ADVPsnNameCardNo"].ToString()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " ( V_CardLogSP.PsnNo LIKE ? OR V_CardLogSP.PsnName LIKE ? OR V_CardLogSP.CardNo LIKE ?) ";
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

            if (!string.IsNullOrEmpty(dropCtrl.SelectedValue))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " ( CtrlNo = ? ) ";
                liSqlPara.Add("S:" + dropCtrl.SelectedValue);
            }
            else if (!string.IsNullOrEmpty(dropEquGroup.SelectedValue))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " ( LogEquGrpID = ? ) ";
                liSqlPara.Add("S:" + dropEquGroup.SelectedValue);
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

            //sql += wheresql + " ORDER BY CardLog.CardTime ";
            _sqlcommand = sql += wheresql;

            _datacount = oAcsDB.DataCount(_sqlcommand, liSqlPara);

            if (bMode == true)
            {
                oAcsDB.GetDataTable("CardLog", _sqlcommand, liSqlPara, out CardLogTable);
            }
            else
            {
                //CardLogTable = oAcsDB.PageData(_sqlcommand, liSqlPara, "NewIDNum", _pageindex + 1, _pagesize);
                CardLogTable = oAcsDB.PageData(sql, liSqlPara, _pageindex, _pagesize);
            }
            #endregion

            //oAcsDB.GetDataTable("CardLog", sql, liSqlPara, out CardLogTable);

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
            sql = @" SELECT 
                     CardLog.CardTime, OrgStrucAllData1.OrgName AS ComName, OrgStrucAllData2.OrgName AS DepName, Person.PsnName, Person.PsnNo, Card.CardNo, 
                     EquData.EquNo, EquData.EquName, CardLogState.StateDesc AS LogStatus, CardLog.CtrlName, CardLog.LogEquGrpName, CardLog.LogTime, OrgStrucAllData2.OrgStrucID AS DepID, Card.CardVer, CtrlNo, LogEquGrpID 
                     FROM dbo.V_CardLogSP AS CardLog 
                     INNER JOIN dbo.B01_Card AS Card ON Card.CardNo = CardLog.CardNo AND (Card.CardVer = CardLog.CardVer OR (Card.CardVer IS NULL AND CardLog.CardVer IS NULL )) 
                     INNER JOIN dbo.B01_Person AS Person ON Person.PsnID = Card.PsnID 
                     INNER JOIN dbo.B01_EquData AS EquData ON EquData.EquNo = CardLog.EquNo 
                     INNER JOIN OrgStrucAllData('Company') AS OrgStrucAllData1 ON OrgStrucAllData1.OrgStrucID = Person.OrgStrucID 
                     INNER JOIN OrgStrucAllData('Department') AS OrgStrucAllData2 ON OrgStrucAllData2.OrgStrucID = Person.OrgStrucID 
                     INNER JOIN B00_CardLogState AS CardLogState ON CardLogState.Code = CardLog.LogStatus 
                     INNER JOIN B01_MgnOrgStrucs AS MgnOrgStrucs ON MgnOrgStrucs.OrgStrucID = OrgStrucAllData2.OrgStrucID 
                     INNER JOIN B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID 
                     INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID 
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnOrgStrucs.MgaID AND SysUserMgns.MgaID = MgnEquGroup.MgaID 
                     WHERE CardLog.RecordID = ? ";

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
                        case 11:
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
        private void CreateDropDownList_LogStatusItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            DropDownList_LogStatus.Items.Clear();

            #region Process String
            sql = @" SELECT Code, StateDesc FROM B00_CardLogState AS CardLogState ";
            #endregion

            oAcsDB.GetDataTable("LogStatusDropItem", sql, liSqlPara, out dt);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Item = new System.Web.UI.WebControls.ListItem();
                    Item.Text = dr["StateDesc"].ToString();
                    Item.Value = dr["Code"].ToString();
                    DropDownList_LogStatus.Items.Add(Item);
                }
            }
            else
            {
                DropDownList_LogStatus.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
            }
        }
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
            sql = @" SELECT DISTINCT
                     OrgStrucAllData.OrgID AS DepID, OrgStrucAllData.OrgName AS DepName
                     FROM  OrgStrucAllData('Department') AS OrgStrucAllData
                     INNER JOIN B01_MgnOrgStrucs AS MgnOrgStrucs ON MgnOrgStrucs.OrgStrucID = OrgStrucAllData.OrgStrucID
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnOrgStrucs.MgaID  ";

            #region DataAuth
            if (wheresql != "") wheresql += " AND ";
            wheresql += " (SysUserMgns.UserID = ? ) ";
            liSqlPara.Add("S:" + this.hideUserID.Value);
            #endregion

            #endregion

            sql += " WHERE " + wheresql + " ORDER BY OrgStrucAllData.OrgName ";

            oAcsDB.GetDataTable("DepDropItem", sql, liSqlPara, out dt);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Item = new System.Web.UI.WebControls.ListItem();
                    Item.Text = dr["DepName"].ToString();
                    Item.Value = dr["DepID"].ToString();
                    ADVDropDownList_Dep.Items.Add(Item);
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

        #region DropDownList繫結
        /// <summary>
        /// DropDownList繫結
        /// </summary>
        /// <param name="dropData">DropDownList物件</param>
        /// <param name="strValue">項目值欄位</param>
        /// <param name="strText">文字內容欄位</param>
        /// <param name="strCmd">查詢語法</param>
        public void DropBind(DropDownList dropData, string strValue, string strText, string strCmd)
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            DataTable dtData = new DataTable();
            oAcsDB.GetDataTable("DropData", strCmd, out dtData);

            #region Give Empty Item
            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = "- 請選擇 -";
            Item.Value = "";
            dropData.Items.Add(Item);
            #endregion

            foreach (DataRow dr in dtData.Rows)
            {
                Item = new ListItem();
                Item.Text = dr[strText].ToString();
                Item.Value = dr[strValue].ToString();
                dropData.Items.Add(Item);
            }
        }
        #endregion

        #region ExportExcel
        public void ExportExcel(DataTable ProcDt)
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("CardLog");
            DataTable dtCardLog = ProcDt;

            //Title
            ws.Cells[1, 1].Value  = "讀卡時間";
            ws.Cells[1, 2].Value  = "公司";
            ws.Cells[1, 3].Value  = "部門";
            ws.Cells[1, 4].Value  = "人員";
            ws.Cells[1, 5].Value  = "人員編號";
            ws.Cells[1, 6].Value  = "卡號";
            ws.Cells[1, 7].Value  = "設備編號";
            ws.Cells[1, 8].Value  = "設備名稱";
            ws.Cells[1, 9].Value  = "刷卡結果";
            ws.Cells[1, 10].Value = "控制器";
            ws.Cells[1, 11].Value = "設備群組";
            ws.Cells[1, 12].Value = "記錄時間";
            ws.Cells[dtCardLog.Rows.Count + 3, 1].Value = "總筆數：" + dtCardLog.Rows.Count + "筆";

            //Content
            for (int i = 0, iCount = dtCardLog.Rows.Count; i < iCount; i++)
            {
                for (int j = 2, jCount = dtCardLog.Rows[i].ItemArray.Length - 4; j < jCount; j++)
                {
                    ws.Cells[i + 2, j - 1].Value = dtCardLog.Rows[i].ItemArray[j].ToString().Replace("&nbsp;", "").Trim();
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

        #endregion
    }
}