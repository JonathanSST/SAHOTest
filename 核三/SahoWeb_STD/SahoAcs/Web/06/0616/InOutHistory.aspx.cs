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
using System.Web.Configuration;

using OfficeOpenXml;

namespace SahoAcs
{
    public partial class InOutHistory : System.Web.UI.Page
    {
        #region Main Description
        int m_PageSize = 100, m_PageIndex = 0, m_PageCount = 0, m_DataCount = 0, m_PsnCount = 0;
        float m_WorkTime = 0, m_RealWorkTime = 0;
        DataTable m_ReportTable   = null;
        Hashtable m_FTRTHashtable = null;
        AjaxControlToolkit.ToolkitScriptManager m_ScriptManager = null;
        #endregion

        #region Events
        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            m_ScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            m_ScriptManager.EnablePageMethods = true;
            m_ScriptManager.RegisterAsyncPostBackControl(this.QueryButton);

            #region 加入主頁JavaScript函式
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js  = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("InOutHistory", "InOutHistory.js");
            #endregion

            #region 註冊主頁Button動作
            QueryButton.Attributes["onClick"]   = "SelectState(); return false;";
            dropCtrl.Attributes["onchange"]     = "CheckDrop('Ctrl');";
            dropEquGroup.Attributes["onchange"] = "CheckDrop('EquGroup');";
            #endregion

            this.MainGridView.PageSize = m_PageSize;
            #endregion

            if (!IsPostBack && !m_ScriptManager.IsInAsyncPostBack)
            {
                #region 首次網頁載入作業
                DropBind(this.dropCompany, "OrgID", "OrgName", @"SELECT OrgID, OrgName FROM B01_OrgData WHERE OrgClass = 'Company' ORDER BY OrgName");
                DropBind(this.dropDept, "OrgID", "OrgName", @"SELECT OrgID, OrgName FROM B01_OrgData WHERE OrgClass = 'Department' ORDER BY OrgName");
                DropBind(this.dropPsnType, "ItemNo", "ItemName", @"SELECT ItemNo, ItemName FROM B00_ItemList WHERE ItemClass = 'PsnType' ORDER BY ItemOrder");
                DropBind(this.dropCtrl, "CtrlNo", "CtrlName", @"SELECT CtrlNo, CtrlName FROM B01_Controller ORDER BY CtrlName");
                DropBind(this.dropEquGroup, "LogEquGrpID", "LogEquGrpName", @"SELECT LogEquGrpID, LogEquGrpName FROM B01_LogEquGroup GROUP BY LogEquGrpName, LogEquGrpID");
                
                Query();

                Calendar_CardTimeSDate.DateValue = DateTime.Today.AddDays(-1).ToString("yyyy/MM/dd") + " 00:00:00";
                Calendar_CardTimeEDate.DateValue = DateTime.Today.ToString("yyyy/MM/dd") + " 23:59:59";
                ShowReportData();
                
                #endregion
            }
            else
            {
                #region 非首次網頁載入作業
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg    = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == this.QueryButton.ClientID)  //來源為查詢按鈕
                {
                    Query();
                    ShowReportData();
                }
                else if (sFormTarget != "")                    //來源為頁索引按鈕
                {
                    LoadPastPageData();
                    string[] arrControl = sFormTarget.Split('$');

                    if (arrControl.Length == 5)
                    {
                        m_PageIndex = Convert.ToInt32(arrControl[4].Split('_')[1]);
                        ShowReportData();
                    }
                }
                #endregion
            }
        }
        #endregion

        #region GridView_Data_RowDataBound
        protected void GridView_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:
                    #region 設定欄位寛度
                    int[] HeaderWidth = { 100, 100, 100, 100, 100, 100, 150, 200, 150, 200, 100, 100, 126 };
                    for (int i = 0; i < e.Row.Cells.Count - 0; i++) { e.Row.Cells[i].Width = HeaderWidth[i]; }
                    #endregion

                    #region 寫入Literal_Header
                    StringWriter Header_sw       = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);

                    e.Row.CssClass = "GVStyle";
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible  = false;
                    li_header.Text = Header_sw.ToString();
                    #endregion
                    break;
                #endregion

                #region DataRow
                case DataControlRowType.DataRow:
                    DataRowView oRow = (DataRowView)e.Row.DataItem;

                    #region 指定Row的ID
                    GridViewRow GrRow  = e.Row;
                    GrRow.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    GrRow.ID = "GV_Row" + oRow.Row["NewIDNum"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 103, 104, 104, 104, 104, 104, 154, 204, 154, 204, 104, 104, 130 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++) { e.Row.Cells[i].Width = DataWidth[i]; }
                    #endregion

                    #region 針對各欄位做所需處理
                    #region 刷進時間
                    if (!string.IsNullOrEmpty(oRow.Row["InTime"].ToString()))
                    {
                        e.Row.Cells[6].Text = DateTime.Parse(oRow.Row["InTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                    }
                    #endregion

                    #region 刷出時間
                    if (!string.IsNullOrEmpty(oRow.Row["OutTime"].ToString()))
                    {
                        e.Row.Cells[8].Text = DateTime.Parse(oRow.Row["OutTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                    }
                    #endregion
                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;") { e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text; }
                    }

                    e.Row.Cells[0].Text = LimitText(e.Row.Cells[0].Text, 18, true);
                    e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text, 18, true);
                    e.Row.Cells[7].Text = LimitText(e.Row.Cells[7].Text, 33, true);
                    e.Row.Cells[9].Text = LimitText(e.Row.Cells[9].Text, 33, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    break;
                #endregion

                #region Pager
                case DataControlRowType.Pager:
                    #region 取得控制項
                    GridView gv                      = sender as GridView;
                    LinkButton lbtnFirst             = e.Row.FindControl("lbtnFirst") as LinkButton;
                    LinkButton lbtnLast              = e.Row.FindControl("lbtnLast") as LinkButton;
                    PlaceHolder phdPageNumber        = e.Row.FindControl("phdPageNumber") as PlaceHolder;
                    LinkButton lbtnPrev              = e.Row.FindControl("lbtnPrev") as LinkButton;
                    LinkButton lbtnNext              = e.Row.FindControl("lbtnNext") as LinkButton;
                    LinkButton lbtnPage;
                    PlaceHolder phdPsnCount          = e.Row.FindControl("phdPsnCount") as PlaceHolder;
                    PlaceHolder phdInOutCount        = e.Row.FindControl("phdInOutCount") as PlaceHolder;
                    PlaceHolder phdTotalWorkTime     = e.Row.FindControl("phdTotalWorkTime") as PlaceHolder;
                    PlaceHolder phdTotalRealWorkTime = e.Row.FindControl("phdTotalRealWorkTime") as PlaceHolder;
                    #endregion

                    #region 顯示頁數及[上下頁、首頁、末頁]處理
                    int showRange  = 5;  //顯示快捷頁數
                    m_PageCount    = (m_DataCount % m_PageSize) == 0 ? (m_DataCount / m_PageSize) : (m_DataCount / m_PageSize) + 1;
                    int pageCount  = m_PageCount;
                    int pageIndex  = m_PageIndex;
                    int startIndex = (pageIndex + 1 < showRange) ? 0 : (pageIndex + 1 + showRange / 2 >= pageCount) ? pageCount - showRange : pageIndex - showRange / 2;
                    int endIndex   = (startIndex >= pageCount - showRange) ? pageCount : startIndex + showRange;

                    #region 頁數列
                    phdPageNumber.Controls.Add(new LiteralControl("  "));

                    #region 指定頁數及改變文字風格
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        lbtnPage = new LinkButton();

                        lbtnPage.ID              = "Pages_" + (i).ToString();
                        lbtnPage.Text            = (i + 1).ToString();
                        lbtnPage.CommandName     = "Pages";
                        lbtnPage.CommandArgument = (i + 1).ToString();
                        lbtnPage.Font.Overline   = false;

                        if (i != pageIndex)
                        {
                            lbtnPage.Font.Bold = false;
                        }
                        else
                        {
                            lbtnPage.Font.Bold     = true;
                            lbtnPage.ForeColor     = System.Drawing.Color.White;
                            lbtnPage.OnClientClick = "return false;";
                        }

                        phdPageNumber.Controls.Add(lbtnPage);
                        phdPageNumber.Controls.Add(new LiteralControl(" "));
                    }
                    #endregion
                    #endregion

                    #region 上下頁
                    if (m_PageIndex == 0)
                    {
                        lbtnPrev.Enabled = false;
                    }
                    else
                    {
                        lbtnPrev.Enabled = true;
                        lbtnPrev.ID      = "lbtnPrev_" + (m_PageIndex - 1);
                    }

                    if (m_PageIndex == m_PageCount - 1 || m_PageCount == 0)
                    {
                        lbtnNext.Enabled = false;
                    }
                    else
                    {
                        lbtnNext.Enabled = true;
                        lbtnNext.ID      = "lbtnNext_" + (m_PageIndex + 1);
                    }
                    #endregion

                    #region 首末頁
                    if (m_PageIndex == 0 || m_PageCount == 0)
                    {
                        lbtnFirst.Enabled = false;
                    }
                    else
                    {
                        lbtnFirst.Enabled = true;
                        lbtnFirst.ID      = "lbtnFirst_" + 0;
                    }

                    if (m_PageIndex == m_PageCount - 1 || m_PageCount == 0)
                    {
                        lbtnLast.Enabled = false;
                    }
                    else
                    {
                        lbtnLast.Enabled = true;
                        lbtnLast.ID      = "lbtnLast_" + (m_PageCount - 1);
                    }
                    #endregion
                    #endregion

                    #region 顯示總頁數及目前頁數
                    phdPageNumber.Controls.Add(new LiteralControl(string.Format("　{0} / {1}　", pageIndex + 1, pageCount == 0 ? 1 : pageCount)));
                    phdPageNumber.Controls.Add(new LiteralControl("  "));
                    phdPsnCount.Controls.Add(new LiteralControl("　　總人數：" + m_PsnCount + "人"));
                    phdInOutCount.Controls.Add(new LiteralControl("　　總筆數：" + m_DataCount + "筆"));
                    phdTotalWorkTime.Controls.Add(new LiteralControl("　　總工時：" + string.Format("{0:F}", m_WorkTime) + "時"));
                    phdTotalRealWorkTime.Controls.Add(new LiteralControl("　　總扣除午休實際工時：" + string.Format("{0:F}", m_RealWorkTime) + "時"));
                    #endregion

                    #region 寫入Literal_Pager
                    StringWriter Pager_sw       = new StringWriter();
                    HtmlTextWriter Pager_writer = new HtmlTextWriter(Pager_sw);

                    e.Row.CssClass = "GVStylePgr";
                    e.Row.RenderControl(Pager_writer);
                    e.Row.Visible  = false;
                    li_Pager.Text  = Pager_sw.ToString();
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
            if (MainGridView.Rows[0].Cells.Count == MainGridView.HeaderRow.Cells.Count) { MainGridView.DataBind(); }
        }
        #endregion

        #region ExportButton_Click
        protected void ExportButton_Click(object sender, EventArgs e)
        {
            LoadPastPageData();

            if (m_ReportTable != null && m_ReportTable.Rows.Count != 0)
            {
                ExportExcel(m_ReportTable);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('無資料可匯出!!'); location.href='InOutHistory.aspx';", true);
            }
        }
        #endregion
        #endregion

        #region Method
        #region LoadPastPageData
        private void LoadPastPageData()
        {
            string[] sPastPageData = null;

            m_ReportTable       = (DataTable)Session["ReportTable"];
            try { sPastPageData = ((string)Session["PastPageData"]).Split(';'); } catch { sPastPageData = null; }

            if (sPastPageData != null && sPastPageData.Length == 5)
            {
                m_PageCount    = int.Parse(sPastPageData[0]);
                m_DataCount    = int.Parse(sPastPageData[1]);
                m_PsnCount     = int.Parse(sPastPageData[2]);
                m_WorkTime     = float.Parse(sPastPageData[3]);
                m_RealWorkTime = float.Parse(sPastPageData[4]);
            }

            if (m_ReportTable == null || sPastPageData == null) { Query(); }  //預防Sesson已逾時故重新查詢資料
            sPastPageData = null;
        }
        #endregion

        #region ShowReportData
        private void ShowReportData()
        {
            int iRowS = 0, iRowE = 0;
            DataTable oDataTable = null;

            //顯示指定範圍的報表資料
            iRowS      = m_PageIndex * m_PageSize;
            iRowE      = m_PageSize * (m_PageIndex + 1);
            oDataTable = m_ReportTable.Clone();

            if (iRowE > m_DataCount) { iRowE = m_DataCount; }
            for (int i = iRowS; i < iRowE; i++) { oDataTable.ImportRow(m_ReportTable.Rows[i]); }

            GirdViewDataBind(this.MainGridView, oDataTable);
            UpdatePanel1.Update();

            oDataTable = null;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI", "$.unblockUI();", true);
        }
        #endregion

        #region GirdViewDataBind
        private void GirdViewDataBind(GridView oGridView, DataTable oDataTable)
        {
            if (oDataTable.Rows.Count != 0)  //Gridview中有資料
            {
                oGridView.DataSource = oDataTable;
                oGridView.DataBind();
            }
            else                             //Gridview中沒有資料
            {
                oDataTable.Rows.Add(oDataTable.NewRow());
                oGridView.DataSource = oDataTable;
                oGridView.DataBind();

                int columnCount = oGridView.Rows[0].Cells.Count;
                oGridView.Rows[0].Cells.Clear();
                oGridView.Rows[0].Cells.Add(new TableCell());
                oGridView.Rows[0].Cells[0].ColumnSpan = columnCount;
                oGridView.Rows[0].Cells[0].Text = "查無資料";
            }
        }
        #endregion

        #region VerifyRenderingInServerForm
        public override void VerifyRenderingInServerForm(Control control)
        {
            //修正'XX'型別必須置於有runat=server的表單標記之中Override此Methods
        }
        #endregion

        #region LimitText
        private string LimitText(string str, int len, bool ellipsis)
        {
            Encoding big5 = Encoding.GetEncoding("big5");
            byte[] b      = big5.GetBytes(str);

            if (b.Length <= len) { return str; }
            else
            {
                if (ellipsis) len -= 3;

                string res = big5.GetString(b, 0, len);
                if (!big5.GetString(b).StartsWith(res)) { res = big5.GetString(b, 0, len - 1); }

                return res + (ellipsis ? "..." : "");
            }
        }
        #endregion

        #region DropBind
        private void DropBind(DropDownList dropData, string strValue, string strText, string strCmd)
        {
            DB_Acs oAcsDB        = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            DataTable oDataTable = null;
            System.Web.UI.WebControls.ListItem oListItem = null;

            oAcsDB.GetDataTable("DropData", strCmd, out oDataTable);

            #region Give Empty Item
            oListItem = new System.Web.UI.WebControls.ListItem();

            oListItem.Text  = "- 請選擇 -";
            oListItem.Value = "";

            dropData.Items.Add(oListItem);
            oListItem = null;
            #endregion

            foreach (DataRow dr in oDataTable.Rows)
            {
                oListItem = new ListItem();

                oListItem.Text  = dr[strText].ToString();
                oListItem.Value = dr[strValue].ToString();

                dropData.Items.Add(oListItem);
                oListItem = null;
            }
        }
        #endregion

        #region Query
        private void Query()
        {
            float fWorkTimeTemp = 0, fRealWorkTimeTemp = 0;
            string sSql = "", sWhere = "", sM = "", sD = "", sY = "", sKey = "", sCardNo = "", sCardNoTemp = "", sWorkTime = "", sRealWorkTime = "", sYMD = "", sItemNo = "", sLogEquGrpID1 = "", sLogEquGrpID2 = "";
            DataTable oDataTable   = null;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            List<string> liSqlPara = new List<string>();
            DateTime oCardDate;

            #region Process String
            #region 依設備查詢所要的刷卡資料在來計算進出記錄
            #region 取得廠商在日期範圍的考勤刷卡資料
            sSql = string.Format(@" SELECT DISTINCT(CardNo), STR(YEAR(InTime), 4, 0) + '-' + REPLACE(STR(MONTH(InTime), 2, 0), ' ','0') + '-' + REPLACE(STR(DAY(InTime), 2, 0), ' ', '0') AS 'InCardDate' 
                                    FROM dbo.GetInOutTable('A', {0}, '{1}', '{2}', '{3}') 
                                    WHERE ItemNo = 'F' AND CheckState = '1' 
                                    ORDER BY CardNo ", Pub.WorkOverTime, Pub.RestRange, Calendar_CardTimeSDate.DateValue, Calendar_CardTimeEDate.DateValue);
            

            liSqlPara.Clear();
            oAcsDB.GetDataTable("FTRTData", sSql, liSqlPara, out oDataTable);
            #endregion

            #region 設定廠商考勤刷卡資料對照表
            if (oDataTable != null)
            {
                m_FTRTHashtable = new Hashtable();

                foreach (DataRow oDataRow in oDataTable.Rows)
                {
                    sY   = oDataRow["InCardDate"].ToString().Substring(0, 4).Trim();
                    sM   = oDataRow["InCardDate"].ToString().Substring(5, 2).Trim();
                    sD   = oDataRow["InCardDate"].ToString().Substring(8, 2).Trim();
                    sKey = string.Format("{0}_{1}-{2}-{3}", oDataRow["CardNo"], sY, sM.PadLeft(2, '0'), sD.PadLeft(2, '0'));

                    m_FTRTHashtable.Add(sKey, "1");
                }

                oDataTable = null;
            }
            #endregion
            #endregion

            #region 查詢報表資料
            #region 設定SQL語法
            sSql = string.Format(@" SELECT ROW_NUMBER() OVER(ORDER BY CompanyName, DeptName, ItemName, PsnName, InTime) AS NewIDNum, CompanyName, DeptName, ItemName, PsnName, PsnNo, CardNo, InTime, InEquName, OutTime, OutEquName, WorkTime AS 'InOutTime', WorkTime, WorkTime - DelWorkTime AS 'RealWorkTime', 
                                    CompanyID, DeptID, ItemNo, InEquNo, OutEquNo, InCtrlNo, OutCtrlNo, InLogEquGrpID, OutLogEquGrpID FROM ( 
                                    SELECT * FROM dbo.GetInOutTable('A', {0}, '{1}', '{2}', '{3}') UNION 
                                    SELECT * FROM dbo.GetInOutTable('B', {0}, '{1}', '{2}', '{3}') UNION 
                                    SELECT * FROM dbo.GetInOutTable('C1', {0}, '{1}', '{2}', '{3}') UNION 
                                    SELECT * FROM dbo.GetInOutTable('C2', {0}, '{1}', '{2}', '{3}') UNION 
                                    SELECT * FROM dbo.GetInOutTable('C3', {0}, '{1}', '{2}', '{3}') UNION 
                                    SELECT * FROM dbo.GetInOutTable('C4', {0}, '{1}', '{2}', '{3}') UNION 
                                    SELECT * FROM dbo.GetInOutTable('C5', {0}, '{1}', '{2}', '{3}')) YA ", 
                                    Pub.WorkOverTime, 
                                    Pub.RestRange, 
                                    Calendar_CardTimeSDate.DateValue, 
                                    Calendar_CardTimeEDate.DateValue);
            /*
            sSql = string.Format(@"SELECT  
                                     ROW_NUMBER() OVER(ORDER BY CompanyName, DeptName, ItemName, PsnName, InTime) AS NewIDNum, CompanyName, 
                                    DeptName, ItemName, PsnName, PsnNo, CardNo, 
	                                InTime, InEquName, OutTime, OutEquName, WorkTime AS 'InOutTime', WorkTime, WorkTime - DelWorkTime AS 'RealWorkTime', 
                                    CompanyID, DeptID, ItemNo, InEquNo, OutEquNo, InCtrlNo, OutCtrlNo, InLogEquGrpID, OutLogEquGrpID
                                    FROM B03_InOutTable_Temp AS YA
                                    WHERE (InTime BETWEEN '{0}' AND '{1}'
                                    OR OutTime BETWEEN '{0}' AND '{1}') ", Calendar_CardTimeSDate.DateValue, Calendar_CardTimeEDate.DateValue);
             */
            #endregion

            #region 設定WHERE條件
            liSqlPara.Clear();
            sWhere = " WHERE YA.CheckState = '1' ";

            if (!string.IsNullOrEmpty(dropCompany.SelectedValue))
            {
                sWhere += " AND (CompanyID = ?) ";
                liSqlPara.Add("S:" + dropCompany.SelectedValue);
            }

            if (!string.IsNullOrEmpty(dropDept.SelectedValue))
            {
                sWhere += " AND (DeptID = ?) ";
                liSqlPara.Add("S:" + dropDept.SelectedValue);
            }

            if (!string.IsNullOrEmpty(dropPsnType.SelectedValue))
            {
                sWhere += " AND (ItemNo = ?) ";
                liSqlPara.Add("S:" + dropPsnType.SelectedValue);
            }

            if (!string.IsNullOrEmpty(txtPsnNoOrName.Text.Trim()))
            {
                sWhere += " AND (PsnNo LIKE ? OR PsnName LIKE ?) ";
                liSqlPara.Add("S:" + "%" + txtPsnNoOrName.Text.Trim() + "%");
                liSqlPara.Add("S:" + "%" + txtPsnNoOrName.Text.Trim() + "%");
            }

            if (!string.IsNullOrEmpty(txtCardNo.Text.Trim()))
            {
                sWhere += " AND (CardNo = ?) ";
                liSqlPara.Add("S:" + txtCardNo.Text.Trim());
            }

            if (!string.IsNullOrEmpty(dropCtrl.SelectedValue))
            {
                sWhere += " AND ((InCtrlNo = ?) OR (OutCtrlNo = ?)) ";
                liSqlPara.Add("S:" + dropCtrl.SelectedValue);
                liSqlPara.Add("S:" + dropCtrl.SelectedValue);
            }
            else if (!string.IsNullOrEmpty(dropEquGroup.SelectedValue))
            {
                sWhere += " AND ((InLogEquGrpID = ?) OR (OutLogEquGrpID = ?))";
                liSqlPara.Add("S:" + dropEquGroup.SelectedValue);
                liSqlPara.Add("S:" + dropEquGroup.SelectedValue);
            }
            #endregion

            sSql += sWhere + " ORDER BY NewIDNum ";
            
            //oAcsDB.GetDataTable("ReportTable", sSql, liSqlPara, out m_ReportTable);
            //解決資料庫連線timeout的問題，將timeout時間增加為300秒
            oAcsDB.GetDataTable("ReportTable", sSql, liSqlPara, "InOutHistory.aspx", out m_ReportTable);
            
            #endregion

            #region 處理廠商考勤及2與3號門設備的工時與午休工時並計算相關的網頁資料
            Session["ReportTable"] = null; Session["PastPageData"] = null;
            m_DataCount = 0; m_PsnCount = 0; m_WorkTime = 0; m_RealWorkTime = 0;

            if (m_ReportTable != null)
            {
                foreach (DataRow oDataRow in m_ReportTable.Rows)
                {
                    #region 處理廠商考勤及2與3號門設備的工時與午休工時
                    sItemNo       = oDataRow["ItemNo"].ToString();
                    sLogEquGrpID1 = oDataRow["InLogEquGrpID"].ToString();
                    sLogEquGrpID2 = oDataRow["OutLogEquGrpID"].ToString();

                    if (sItemNo == "F" && ((sLogEquGrpID1 == "2" || sLogEquGrpID1 == "3") || (sLogEquGrpID2 == "2" || sLogEquGrpID2 == "3")))
                    {
                        if (DateTime.TryParse(oDataRow["InTime"].ToString(), out oCardDate))
                        {
                            sYMD = oCardDate.ToString("yyyy-MM-dd");
                            sKey = string.Format("{0}_{1}", oDataRow["CardNo"], sYMD);

                            if (m_FTRTHashtable.ContainsKey(sKey))
                            {
                                oDataRow["WorkTime"]     = DBNull.Value;
                                oDataRow["RealWorkTime"] = DBNull.Value;
                            }
                        }
                    }
                    #endregion

                    #region 計算相關的網頁資料
                    //處理總筆數
                    m_DataCount = m_ReportTable.Rows.Count;

                    //計算總人數
                    sCardNo = oDataRow["CardNo"].ToString();
                    if (sCardNo != sCardNoTemp) { m_PsnCount++; sCardNoTemp = sCardNo; }

                    //計算工時小計
                    sWorkTime = oDataRow["WorkTime"].ToString();
                    if (float.TryParse(sWorkTime, out fWorkTimeTemp)) { m_WorkTime += fWorkTimeTemp; }

                    //計算扣除午休實際工時
                    sRealWorkTime = oDataRow["RealWorkTime"].ToString();
                    if (float.TryParse(sRealWorkTime, out fRealWorkTimeTemp)) { m_RealWorkTime += fRealWorkTimeTemp; }
                    #endregion
                }
            }

            //儲存查詢資料與網頁資料
            m_FTRTHashtable         = null;
            Session["ReportTable"]  = m_ReportTable;
            Session["PastPageData"] = string.Format("{0};{1};{2};{3};{4}", m_PageCount, m_DataCount, m_PsnCount, m_WorkTime, m_RealWorkTime);
            #endregion
            #endregion
        }
        #endregion

        #region ExportExcel
        private void ExportExcel(DataTable oDataTable)
        {
            DataTable oReportData = oDataTable;
            ExcelPackage pck      = new ExcelPackage();
            ExcelWorksheet ws     = pck.Workbook.Worksheets.Add("InOutHistory");

            //設定標題
            ws.Cells[1, 1].Value  = "公司";
            ws.Cells[1, 2].Value  = "部門";
            ws.Cells[1, 3].Value  = "類型";
            ws.Cells[1, 4].Value  = "人員";
            ws.Cells[1, 5].Value  = "人員編號";
            ws.Cells[1, 6].Value  = "卡號";
            ws.Cells[1, 7].Value  = "刷進時間";
            ws.Cells[1, 8].Value  = "刷進設備";
            ws.Cells[1, 9].Value  = "刷出時間";
            ws.Cells[1, 10].Value = "刷出設備";
            ws.Cells[1, 11].Value = "進出小計";
            ws.Cells[1, 12].Value = "工時小計";
            ws.Cells[1, 13].Value = "扣除午休實際工時";

            //設定內容
            for (int i = 0, iCount = oReportData.Rows.Count; i < iCount; i++)
            {
                for (int j = 0, jCount = 13; j < jCount; j++)
                {
                    ws.Cells[i + 2, j + 1].Value = oReportData.Rows[i][j + 1].ToString().Replace("&nbsp;", "").Trim();
                }
            }

            //設定註記
            ws.Cells[oReportData.Rows.Count + 3, 1].Value = "總人數：" + m_PsnCount + "人";
            ws.Cells[oReportData.Rows.Count + 4, 1].Value = "總筆數：" + oReportData.Rows.Count + "筆";
            ws.Cells[oReportData.Rows.Count + 5, 1].Value = "總工時小計：" + string.Format("{0:F}", m_WorkTime) + "時";
            ws.Cells[oReportData.Rows.Count + 6, 1].Value = "總扣除午休實際工時：" + string.Format("{0:F}", m_RealWorkTime) + "時";

            //設定格式
            ws.Cells.AutoFitColumns();
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_InOutHistory.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }
        #endregion
        #endregion
    }
}