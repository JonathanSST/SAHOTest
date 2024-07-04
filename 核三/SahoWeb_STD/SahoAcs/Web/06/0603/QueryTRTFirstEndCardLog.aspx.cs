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
using DapperDataObjectLib;
using OfficeOpenXml;
using PagedList;


namespace SahoAcs
{
    public partial class QueryTRTFirstEndCardLog : System.Web.UI.Page
    {
        #region 一.宣告
        AjaxControlToolkit.ToolkitScriptManager m_ToolKitScriptManager;  //宣告AjaxControlToolkit元件
        private int _pagesize = 100, _datacount = 1, _pageindex = 0, _pagecount = 0; //設定GridView控制項每頁可顯示的資料列數
        DataTable CardLogTable = null;
        string sPsnID = "";
        #endregion

        #region 二.網頁

        #region 2-1A.網頁：事件方法
        /// <summary>
        /// 初始化網頁相關的動作
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterComponeAndScript();
            RegisterWindowsAndButton();
            //QueryLabel_CalendarSTime.SetWidth(180);
            //QueryLabel_CalendarETime.SetWidth(180);
            this.MainGridView.PageSize = _pagesize;

            #region Check Person Data
            this.sPsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
            if (!this.sPsnID.Equals(""))
            {
                this.ShowPsnInfo1.Visible = false;
                this.ShowPsnInfo2.Visible = false;
                this.ShowPsnInfo3.Visible = false;
                this.ShowPsnInfo4.Visible = false;
                this.ShowPsnInfo5.Visible = false;
                this.ShowPsnInfo6.Visible = false;
            }
            #endregion


            if (!IsPostBack && !m_ToolKitScriptManager.IsInAsyncPostBack)  //網頁初載入時相關的動作
            {
                #region Give hideValue
                hUserID.Value = Session["UserID"].ToString();
                hOwnerList.Value = Session["OwnerList"].ToString();
                hPsnID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
                #endregion

                //User不啟用其他查詢條件
                if (hUserID.Value.Equals("User") && !hPsnID.Value.Equals(""))
                {
                    QueryLabel_CardNo.Visible = false;
                    QueryTextBox_CardNo.Visible = false;
                    QueryLabel_PsnNo.Visible = false;
                    QueryTextBox_PsnNo.Visible = false;
                    QueryLabel_Dept.Visible = false;
                    ddlDept.Visible = false;
                }

                ChangeLanguage();
                CreateQueryCardLogPageComponent();
                CreateDeptDropItem();
                ViewState["SortExpression"] = "CONVERT(VARCHAR(10),B01_CardLog.CardTime,121)";
                ViewState["SortDire"] = "ASC";
                #region 查詢時間區間的預設值
                DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
                DateTime dtLastCardTime = oAcsDB.GetDateTimeScalar(@"SELECT MAX(CardTime) FROM B01_CardLog WHERE CardTime <= GETDATE() AND (EquClass='TRT' OR IsAndTRT=1) ");

                if (dtLastCardTime == DateTime.MinValue)
                {
                    QueryLabel_PickDate_SDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd");
                    QueryLabel_PickDate_EDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd");
                }
                else
                {
                    QueryLabel_PickDate_SDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd");
                    QueryLabel_PickDate_EDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd");
                }
                #endregion


                if (this.sPsnID == "")
                {
                    //this.Query("", false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                }
                else
                {
                    //this.QueryByPerson("", false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                }

            }
            else //網頁PostBack時相關的動作
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];    //取得網頁目標
                string sFormArg = Request.Form["__EVENTARGUMENT"];  //取得網頁參數

                if (sFormTarget == this.QueryButton.ClientID)
                {
                    if (this.sPsnID == "")
                        this.Query("", false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    else
                        this.QueryByPerson("", false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                }
                else
                {
                    string[] arrControl = sFormTarget.Split('$');

                    if (arrControl.Length == 5)
                    {
                        _pageindex = Convert.ToInt32(arrControl[4].Split('_')[1]);
                        if (this.sPsnID == "")
                            this.Query("", false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        else
                            this.QueryByPerson("", false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    }
                }
            }
        }
        #endregion

        #region 2-1B.網頁：自訂方法
        /// <summary>
        /// *覆寫此方法用來修正'XX'型別必須置於含有runat=server的標記屬性
        /// </summary>
        /// <param name="control">伺服器控制項</param>
        public override void VerifyRenderingInServerForm(Control control)
        {
            //覆寫此方法用來修正'XX'型別必須置於含有runat=server的標記屬性
        }

        /// <summary>
        /// *開啟對話視窗的JavaScript語法
        /// </summary>
        private void OpenDialog_Js()
        {
            string jstr = "";

            jstr = @"
                    function OpenDialogAdd(theURL,win_width,win_height) { 
                        var PosX = (screen.width-win_width)/2; 
                        var PosY = (screen.height-win_height)/2; 
                        features = 'dialogWidth='+win_width+',dialogHeight='+win_height+',dialogTop='+PosY+',dialogLeft='+PosX+';' 
                        window.showModalDialog(theURL, '', features);
                    }

                    function OpenDialogEdit(theURL,key,win_width,win_height) { 
                        var PosX = (screen.width-win_width)/2; 
                        var PosY = (screen.height-win_height)/2; 
                        features = 'dialogWidth='+win_width+',dialogHeight='+win_height+',dialogTop='+PosY+',dialogLeft='+PosX+';' 
                        window.showModalDialog(theURL+key, '', features);
                    }";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "OpenDialog_Js", jstr, true);
        }

        /// <summary>
        /// 註冊公用的元件與JavaScript檔案
        /// </summary>
        private void RegisterComponeAndScript()
        {
            m_ToolKitScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            m_ToolKitScriptManager.EnablePageMethods = true;
            m_ToolKitScriptManager.RegisterAsyncPostBackControl(this.QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("QueryTRTFirstEndCardLogScript", "QueryTRTFirstEndCardLog.js");//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>
        /// 註冊網頁的視窗與按鈕動作
        /// </summary>
        private void RegisterWindowsAndButton()
        {
            //註冊主要作業畫面的按鈕動作：網頁畫面設計一
            QueryButton.Attributes["onClick"] = "SelectState(); return false;";
        }

        /// <summary>
        /// 切換網頁的顯示語系
        /// </summary>
        private void ChangeLanguage()
        {
            //等待新系統整個完成後再一併處理
        }
        #endregion

        #endregion

        #region 三.元件
        #region 3-1A.元件-動態：事件方法
        protected void ExportButton_Click(object sender, EventArgs e)
        {
            Query("", true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }

        #region PdfButton_Click
        protected void PdfButton_Click(object sender, EventArgs e)
        {
            Query("", true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }
        #endregion


        public void ExportExcel(DataTable ProcDt)
        {
            if (Request.Form.AllKeys.Where(i => i.Contains("PdfButton")).Count() > 0)
            {
                this.ExportPdf(ProcDt);
            }


            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("TRTFirstEndCardLog");
            DataTable dtCardLog = ProcDt;

            //Title
            ws.Cells[1, 1].Value = "讀卡結果";
            ws.Cells[1, 2].Value = "讀卡日期";
            ws.Cells[1, 3].Value = "首筆";
            ws.Cells[1, 4].Value = "末筆";
            ws.Cells[1, 5].Value = "卡片號碼";
            ws.Cells[1, 6].Value = "部門";
            ws.Cells[1, 7].Value = "人員編號";
            ws.Cells[1, 8].Value = "人員名稱";
            //Content
            for (int i = 0, iCount = dtCardLog.Rows.Count; i < iCount; i++)
            {
                for (int j = 1, jCount = dtCardLog.Rows[i].ItemArray.Length; j < jCount; j++)
                {
                    ws.Cells[i + 2, j].Value = dtCardLog.Rows[i].ItemArray[j].ToString().Replace("&nbsp;", "").Trim();
                }
            }

            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=TRTFirstEndCardLog.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

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
                    sb.Append("<tr><td>讀卡結果</td>");
                    sb.Append("<td>讀卡日期</td>");
                    sb.Append("<td>首筆</td>");
                    sb.Append("<td>末筆</td>");
                    sb.Append("<td>卡片號碼</td>");
                    sb.Append("<td>部門</td>");
                    sb.Append("<td>人員編號</td>");
                    sb.Append("<td>人員名稱</td>");                    
                    sb.Append("</tr>");
                    foreach (var r in ProcDt.AsEnumerable().ToPagedList(p, 18))
                    {
                        sb.Append("<tr>");
                        sb.Append(string.Format("<td>{0}</td>", r["LogStatusName"]));
                        sb.Append(string.Format("<td>{0}</td>", r["Date"]));
                        sb.Append(string.Format("<td>{0}</td>", r["First"]));
                        sb.Append(string.Format("<td>{0}</td>", r["Last"]));
                        sb.Append(string.Format("<td>{0}</td>", r["CardNo"]));
                        sb.Append(string.Format("<td>{0}</td>", r["Dept"]));                        
                        sb.Append(string.Format("<td>{0}</td>", r["PsnNo"]));
                        sb.Append(string.Format("<td>{0}</td>", r["PsnName"]));
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
            Response.AddHeader("content-disposition", "attachment;filename=TrtFirstLast.pdf");
            Response.End();
        }

        #endregion


        #endregion

        #region 3-1B.元件-動態：自訂方法
        /// <summary>
        /// 建立「查詢讀卡資料」網頁相關的元件
        /// </summary>
        public void CreateQueryCardLogPageComponent()
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();

            DataTable dt_CardLogState, dt_CardType, dt_EquClass;

            string sql = "";

            //1.刷卡結果
            #region Process String - Get B00_CardLogState
            sql = " SELECT Code , StateDesc";
            sql += " FROM B00_CardLogState";
            sql += " ORDER BY Code ";

            oAcsDB.GetDataTable("CardLogStateTable", sql, out dt_CardLogState);
            #endregion

            //建立「複合查詢」視窗畫面該「讀卡結果」欄位的「DropDownList」元件
            System.Web.UI.WebControls.DropDownList popDDList0_LogStatus = new System.Web.UI.WebControls.DropDownList();

            popDDList0_LogStatus.Enabled = true;
            popDDList0_LogStatus.Width = 98;
            popDDList0_LogStatus.ID = "popInput0_LogStatus";

            popDDList0_LogStatus.Items.Add(this.GetGlobalResourceObject("Resource","ddlSelectDefault").ToString());
            popDDList0_LogStatus.Items[0].Value = "";
            for (int i = 0; i < dt_CardLogState.Rows.Count; i++)
            {
                popDDList0_LogStatus.Items.Add(dt_CardLogState.Rows[i]["StateDesc"].ToString().Trim());
                popDDList0_LogStatus.Items[i + 1].Value = dt_CardLogState.Rows[i]["Code"].ToString().Trim();
            }

            //popPanel0_LogStatus.Controls.Add(popDDList0_LogStatus);

            //2.卡片類型
            #region Process String - Get B00_ItemList - CardType
            sql = " SELECT ItemOrder , ItemNo , ItemName";
            sql += " FROM B00_ItemList";
            sql += " WHERE ItemClass = 'CardType'";
            sql += " ORDER BY ItemOrder ";

            oAcsDB.GetDataTable("CardTypeTable", sql, out dt_CardType);
            #endregion

            //建立「複合查詢」視窗畫面該「卡片類型」欄位的「DropDownList」元件
            System.Web.UI.WebControls.DropDownList popDDList0_CardType = new System.Web.UI.WebControls.DropDownList();

            popDDList0_CardType.Enabled = true;
            popDDList0_CardType.Width = 98;
            popDDList0_CardType.ID = "popInput0_CardType";

            popDDList0_CardType.Items.Add(this.GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString());
            popDDList0_CardType.Items[0].Value = "";
            for (int i = 0; i < dt_CardType.Rows.Count; i++)
            {
                popDDList0_CardType.Items.Add(dt_CardType.Rows[i]["ItemName"].ToString().Trim());
                popDDList0_CardType.Items[i + 1].Value = dt_CardType.Rows[i]["ItemNo"].ToString().Trim();
            }

            //popPanel0_CardType.Controls.Add(popDDList0_CardType);

            //3.設備類型
            #region Process String - Get B00_ItemList - EquClass
            sql = " SELECT DISTINCT ItemInfo1";
            sql += " FROM B00_ItemList";
            sql += " WHERE ItemClass = 'EquModel'";
            sql += " ORDER BY ItemInfo1";

            oAcsDB.GetDataTable("EquClassTable", sql, out dt_EquClass);
            #endregion

            //建立「複合查詢」視窗畫面該「認證模式」欄位的「DropDownList」元件
            System.Web.UI.WebControls.DropDownList popDDList0_EquClass = new System.Web.UI.WebControls.DropDownList();

            popDDList0_EquClass.Enabled = true;
            popDDList0_EquClass.Width = 200;
            popDDList0_EquClass.ID = "popInput0_EquClass";

            popDDList0_EquClass.Items.Add(this.GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString());
            popDDList0_EquClass.Items[0].Value = "";
            for (int i = 0; i < dt_EquClass.Rows.Count; i++)
            {
                popDDList0_EquClass.Items.Add(dt_EquClass.Rows[i]["ItemInfo1"].ToString().Trim());
                popDDList0_EquClass.Items[i + 1].Value = dt_EquClass.Rows[i]["ItemInfo1"].ToString().Trim();
            }

            //popPanel0_EquClass.Controls.Add(popDDList0_EquClass);
        }

        private void CreateDeptDropItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            OrmDataObject odo = new OrmDataObject("MsSql",Pub.GetDapperConnString());
            string sql = "";
            DataTable dt;

            #region Give Empty Item
            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = this.GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString();
            Item.Value = "";
            this.ddlDept.Items.Add(Item);
            #endregion

            #region Process String
            sql = @"SELECT OrgID, (OrgName + '(' + OrgNo + ')') AS 'OrgName' FROM
                (SELECT B00_SysUserMgns.UserID, OrgStrucAllData.OrgID, OrgStrucAllData.OrgNo,
                 OrgStrucAllData.OrgName FROM B00_SysUserMgns
                INNER JOIN B01_MgnOrgStrucs ON B00_SysUserMgns.MgaID = B01_MgnOrgStrucs.MgaID
                INNER JOIN B01_Person ON B01_MgnOrgStrucs.OrgStrucID = B01_Person.OrgStrucID
                LEFT OUTER JOIN OrgStrucAllData('Department') AS OrgStrucAllData ON B01_Person.OrgStrucID = OrgStrucAllData.OrgStrucID
                ) AS Mgns
                WHERE Mgns.UserID =@UserID AND OrgName<>'' GROUP BY OrgID, (OrgName + '(' + OrgNo + ')')";
            #endregion

            //oAcsDB.GetDataTable("DropListItem", sql, out dt);
            foreach(var o in odo.GetQueryResult(sql,new { UserID = this.hUserID.Value }).Where(i=>i.OrgName!=""))
            {
                Item = new System.Web.UI.WebControls.ListItem();
                Item.Text = Convert.ToString(o.OrgName);//dr["OrgName"].ToString();
                Item.Value = Convert.ToString(o.OrgID);// dr["OrgID"].ToString();
                this.ddlDept.Items.Add(Item);
            }
            sql = sql.Replace("Department", "Unit");
            foreach (var o in odo.GetQueryResult(sql, new { UserID = this.hUserID.Value }).Where(i => i.OrgName != ""))
            {
                Item = new System.Web.UI.WebControls.ListItem();
                Item.Text = Convert.ToString(o.OrgName);//dr["OrgName"].ToString();
                Item.Value = Convert.ToString(o.OrgID);// dr["OrgID"].ToString();
                this.ddlDept.Items.Add(Item);
            }
        }
        #endregion

        #region 3-2A.元件-表格：事件方法
        /// <summary>
        /// *處理GridView控制項在變更分頁索引時的相關動作
        /// </summary>
        protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            MainGridView.PageIndex = e.NewPageIndex;

            if (MainGridView.Rows[0].Cells.Count == MainGridView.HeaderRow.Cells.Count)
                MainGridView.DataBind();
        }

        /// <summary>
        /// 處理GridView控制項在完成資料繫結後的相關動作
        /// </summary>
        protected void GridView_Data_DataBound(object sender, EventArgs e)
        {
            td_showGridView.Attributes["colspan"] = MainGridView.Columns.Count.ToString();
        }

        /// <summary>
        /// 處理GridView控制項在排序的相關動作
        /// </summary>
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
            Query("", false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }

        /// <summary>
        /// 處理資料列繫結至GridView控制項資料時的相關動作
        /// </summary>
        public void GridView_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region A.設定表格的標題部份
                case DataControlRowType.Header:
                    #region A-1.設定欄位隱藏與寬度
                    e.Row.Cells[0].Width = 100;   //讀卡結果
                    e.Row.Cells[1].Width = 100;  //讀卡日期
                    e.Row.Cells[2].Width = 70;  //首筆
                    e.Row.Cells[3].Width = 70;  //末筆
                    e.Row.Cells[4].Width = 90;   //卡片號碼
                    e.Row.Cells[5].Width = 90;   //部門
                    e.Row.Cells[6].Width = 90;   //人員編號
                    #endregion

                    #region A-2.*排序條件Header加工
                    //foreach (DataControlField dataControlField in MainGridView.Columns)
                    //{
                    //    if (dataControlField.SortExpression.Equals(this.SortExpression))
                    //    {
                    //        Label label = new Label();
                    //        label.Text = this.SortDire.Equals("ASC") ? "▲" : "▼";
                    //        e.Row.Cells[MainGridView.Columns.IndexOf(dataControlField)].Controls.Add(label);
                    //    }
                    //}
                    #endregion

                    #region A-3.*寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);

                    e.Row.CssClass = "GVStyle";
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    li_header.Text = Header_sw.ToString();
                    #endregion
                    break;
                #endregion

                #region B.設定表格的資料部份
                case DataControlRowType.DataRow:
                    #region B-1.指定資料列的代碼
                    DataRowView drvRow = (DataRowView)e.Row.DataItem;
                    GridViewRow gvrRow = e.Row;

                    gvrRow.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    #endregion

                    #region B-2.設定欄位隱藏與寬度
                    e.Row.Cells[0].Width = 103;   //讀卡結果
                    e.Row.Cells[1].Width = 104;  //記錄日期
                    e.Row.Cells[2].Width = 74;   //首筆
                    e.Row.Cells[3].Width = 74;   //末筆
                    e.Row.Cells[4].Width = 94;   //卡片號碼
                    e.Row.Cells[5].Width = 94;   //部門
                    e.Row.Cells[6].Width = 94;   //人員編號
                    #endregion

                    #region B-3.處理欄位資料的格式
                    //0.讀卡結果
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;

                    //1.讀卡日期
                    e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Center;

                    //2.首筆
                    e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Center;

                    //3.末筆
                    e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Center;
                    #endregion

                    #region B-4.限制欄位資料的長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }
                    #endregion

                    #region B-5.設定表格資料列的事件方法
                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    #endregion
                    break;
                #endregion

                #region C.*設定表格的換頁部份
                case DataControlRowType.Pager:
                    #region C-1.*建立相關的換頁控制項
                    GridView gv = sender as GridView;
                    LinkButton lbtnFirst = e.Row.FindControl("lbtnFirst") as LinkButton;
                    LinkButton lbtnLast = e.Row.FindControl("lbtnLast") as LinkButton;
                    PlaceHolder phdPageNumber = e.Row.FindControl("phdPageNumber") as PlaceHolder;
                    LinkButton lbtnPrev = e.Row.FindControl("lbtnPrev") as LinkButton;
                    LinkButton lbtnNext = e.Row.FindControl("lbtnNext") as LinkButton;
                    LinkButton lbtnPage;
                    #endregion

                    #region C-2.*顯示頁數列及處理上下頁、首頁、末頁動作
                    int showRange = 5;  //顯示快捷頁數
                    //int pageCount = gv.PageCount;
                    //int pageIndex = gv.PageIndex;
                    _pagecount = (_datacount % _pagesize) == 0 ? (_datacount / _pagesize) : (_datacount / _pagesize) + 1;
                    int pageCount = _pagecount;
                    int pageIndex = _pageindex;
                    int startIndex = (pageIndex + 1 < showRange) ?
                        0 : (pageIndex + 1 + showRange / 2 >= pageCount) ? pageCount - showRange : pageIndex - showRange / 2;
                    int endIndex = (startIndex >= pageCount - showRange) ? pageCount : startIndex + showRange;

                    #region C-2-1.*頁數列
                    phdPageNumber.Controls.Add(new LiteralControl("  "));

                    //指定頁數及改變文字風格
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
                            lbtnPage.CommandArgument = "";
                        }
                        else
                            lbtnPage.Font.Bold = false;

                        phdPageNumber.Controls.Add(lbtnPage);
                        phdPageNumber.Controls.Add(new LiteralControl(" "));
                    }
                    #endregion

                    #region C-2-2.*上下頁
                    //lbtnPrev.Click += delegate(object obj, EventArgs args)
                    //{
                    //    if (gv.PageIndex > 0)
                    //    {
                    //        gv.PageIndex = gv.PageIndex - 1;
                    //        Query("");
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
                    //        Query("");
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

                    #region C-2-3.*首末頁
                    //lbtnFirst.Click += delegate(object obj, EventArgs args)
                    //{
                    //    gv.PageIndex = 0;
                    //    Query("");
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
                    //    Query("");
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

                    #region C-3.*顯示總頁數及目前頁數
                    phdPageNumber.Controls.Add(new LiteralControl(string.Format("　{0} / {1}　", pageIndex + 1, pageCount == 0 ? 1 : pageCount)));
                    phdPageNumber.Controls.Add(new LiteralControl("  "));
                    #endregion

                    #region C-4.*顯示總筆數
                    phdPageNumber.Controls.Add(
                        new LiteralControl(string.Format(this.GetGlobalResourceObject("Resource", "lblGvCounts").ToString(), hDataRowCount.Value)));
                    phdPageNumber.Controls.Add(
                        new LiteralControl("  "));
                    #endregion

                    #region C-5.*寫入Literal_Pager
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

        #region 3-2B.*元件-表格：自訂方法
        /// <summary>
        /// *限制來源字串的顯示長度
        /// </summary>
        /// <param name="str">來源字串</param>
        /// <param name="len">顯示長度</param>
        /// <param name="ellipsis">是否省略</param>
        /// <returns>string</returns>
        public string LimitText(string str, int len, bool ellipsis)
        {
            Encoding big5 = Encoding.GetEncoding("big5");
            byte[] b = big5.GetBytes(str);

            if (b.Length <= len)
                return str;
            else
            {
                if (ellipsis)
                    len -= 3;

                string res = big5.GetString(b, 0, len);

                if (!big5.GetString(b).StartsWith(res))
                    res = big5.GetString(b, 0, len - 1);

                return res + (ellipsis ? "..." : "");
            }
        }

        /// <summary>
        /// *處理資料列繫結至GridView控制項有無資料時的動作
        /// </summary>
        /// <param name="ProcessGridView">表格</param>
        /// <param name="dt">資料表</param>
        public void GirdViewDataBind(GridView ProcessGridView, DataTable dt)
        {
            if (dt.Rows.Count != 0)  //處理Gridview控制項含有資料內容的情況
            {
                ProcessGridView.DataSource = dt;
                ProcessGridView.DataBind();
            }
            else                     //處理Gridview控制項沒有含有資料內容的情況
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

        #region 四.查詢、複合查詢語法
        /// <summary>
        /// 依據指定的模式查詢資料並更新顯示於表格
        /// </summary>
        /// <param name="QueryMode">查詢模式(空子串(一般查詢)、popPagePost(新增或刪除時的查詢)</param>
        /// <param name="bMode">是否匯出資料</param>
        /// <returns>查詢模式：空字串(預設為-1)、popPagePost(傳回目前的索引頁)</returns>
        public void Query(string QueryMode, bool bMode, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            List<string> liSqlPara = new List<string>();
            bool bQueryCondition = false;
            string sql = "", wheresql = "";

            #region Process String - B01_CardLog

            //string strUnionTB = Pub.ReturnNewNestedSerachSQL("0603", QueryLabel_PickDate_SDate.DateValue.ToString().Trim(), QueryLabel_PickDate_EDate.DateValue.ToString().Trim());

            //sql = @" SELECT ROW_NUMBER() OVER(ORDER BY " + SortExpression + " " + SortDire + @") AS NewIDNum,
            sql = @" SELECT ROW_NUMBER() OVER(ORDER BY CONVERT(VARCHAR(10),B01_CardLog.CardTime,121) ASC, B01_CardLog.CardNo) AS NewIDNum,
                B00_CardLogState.StateDesc AS 'LogStatusName', CONVERT(VARCHAR(10),B01_CardLog.CardTime,121) AS 'Date',
                MIN(SUBSTRING(CONVERT(VARCHAR(50),B01_CardLog.CardTime,121),12,8)) AS 'First',
                MAX(SUBSTRING(CONVERT(VARCHAR(50),B01_CardLog.CardTime,121),12,8)) AS 'Last',
                B01_CardLog.CardNo, DepName AS Dept,
                B01_CardLog.PsnNo, B01_CardLog.PsnName
                FROM B01_CardLog AS B01_CardLog LEFT JOIN B00_CardLogState ON B00_CardLogState.Code = B01_CardLog.LogStatus
                LEFT JOIN B00_ItemList ON B00_ItemList.ItemClass = 'CardType' AND B00_ItemList.ItemNo = B01_CardLog.CardType
                INNER JOIN (SELECT PsnNo FROM V_PsnCard AS Person
                INNER JOIN B01_OrgStruc AS OrgStruc ON OrgStruc.OrgStrucID = Person.OrgStrucID
                INNER JOIN B01_MgnOrgStrucs AS MgnOrgStrucs ON MgnOrgStrucs.OrgStrucID = OrgStruc.OrgStrucID
                INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnOrgStrucs.MgaID
                INNER JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID ";

            #region DataAuth
            if (hUserID.Value.Equals("User") && !hPsnID.Value.Equals(""))
            {
                sql += " WHERE (SysUser.UserID = ? AND Person.PsnID = ?)) AS MgnPerson ON MgnPerson.PsnNo = B01_CardLog.PsnNo ";
                liSqlPara.Add("A:" + hUserID.Value);
                liSqlPara.Add("A:" + hPsnID.Value);
            }
            else
            {
                if (!string.IsNullOrEmpty(QueryTextBox_CardNo.Text.ToString().Trim()))
                {
                    sql += " WHERE (IDNum=? OR CardNo LIKE ?) AND (SysUser.UserID = ?)) AS MgnPerson ON MgnPerson.PsnNo = B01_CardLog.PsnNo ";
                    liSqlPara.Add("A:" + QueryTextBox_CardNo.Text);
                    liSqlPara.Add("A:" + "%"+QueryTextBox_CardNo.Text+"%");
                }
                else
                {
                    sql += " WHERE (SysUser.UserID = ?)) AS MgnPerson ON MgnPerson.PsnNo = B01_CardLog.PsnNo ";
                }                
                liSqlPara.Add("A:" + hUserID.Value);
            }

            if (wheresql != "") wheresql += " AND ";
            //20151201 修改查詢(Query Function)條件 CardType = 'E'(原為 A)
            //wheresql += " (B01_CardLog.CardType = 'E' OR B01_CardLog.CardType = 'T') AND (B01_CardLog.IsAndTrt = '1' OR B01_CardLog.EquClass = 'TRT') ";

            // 20170327 拿掉CardType的判斷
            wheresql += " (B01_CardLog.IsAndTrt = '1' OR B01_CardLog.EquClass = 'TRT') ";

            #endregion

            ////設定查詢條件
            if (wheresql != "") wheresql += " AND ";
            wheresql += " (CONVERT(VARCHAR(10),B01_CardLog.CardTime,111) BETWEEN ? AND ?) ";
            liSqlPara.Add("S:" + QueryLabel_PickDate_SDate.DateValue.ToString());
            liSqlPara.Add("S:" + QueryLabel_PickDate_EDate.DateValue.ToString());
            bQueryCondition = true;

            if (!string.IsNullOrEmpty(this.ddlDept.SelectedValue.ToString().Trim())&&this.ddlDept.SelectedIndex>0)
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (B01_CardLog.DepID = ? ) ";
                liSqlPara.Add("S:" +this.ddlDept.SelectedValue);
                bQueryCondition = true;
            }
            if (!string.IsNullOrEmpty(QueryTextBox_PsnNo.Text.ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (B01_CardLog.PsnNo LIKE ? OR B01_CardLog.PsnName LIKE ?) ";
                liSqlPara.Add("S:" + "%" + QueryTextBox_PsnNo.Text.ToString().Trim() + "%");
                liSqlPara.Add("S:" + "%" + QueryTextBox_PsnNo.Text.ToString().Trim() + "%");
                bQueryCondition = true;
            }

            //設定查詢排序
            if (bQueryCondition)
            {
                if (wheresql != "")
                    sql += " WHERE ";
            }
            else
                sql += " WHERE 1 = 0 AND ";


            sql += wheresql + @" GROUP BY CONVERT(VARCHAR(10),B01_CardLog.CardTime,121), B00_CardLogState.StateDesc, B01_CardLog.CardNo
            , B01_CardLog.CardVer, B01_CardLog.PsnNo, B01_CardLog.PsnName, B01_CardLog.OrgStruc,DepName ";            
            //_datacount = oAcsDB.DataCount(sql, liSqlPara);
            //hDataRowCount.Value = _datacount.ToString();

            // (一)先用 _sqlcommand 得到 dtTmp，然後得到總筆數
            DataTable dtTmp = new DataTable();
            oAcsDB.GetDataTable("ALL", sql, liSqlPara, out dtTmp);
            _datacount = dtTmp.Rows.Count;
            hDataRowCount.Value = _datacount.ToString();

            if (bMode == true)
            {
                oAcsDB.GetDataTable("CardLog", sql, liSqlPara, out CardLogTable);
            }
            else
            {
                ////old
                //CardLogTable = oAcsDB.PageData(sql, liSqlPara, "NewIDNum", _pageindex + 1, _pagesize);
                ////new
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
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('無資料可匯出!!');location.href='QueryTRTFirstEndCardLog.aspx';", true);
                }
            }
            else
            {
                //更新查詢條件與表格資料
                GirdViewDataBind(this.MainGridView, CardLogTable);
                UpdatePanel1.Update();
            }

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI", "$.unblockUI();", true);
        }

        public void QueryByPerson(string QueryMode, bool bMode, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            List<string> liSqlPara = new List<string>();
            bool bQueryCondition = false;
            string sql = "", wheresql = "";

            #region Process String - B01_CardLog

            string strUnionTB = Pub.ReturnNewNestedSerachSQL("0603", QueryLabel_PickDate_SDate.DateValue.ToString().Trim(), QueryLabel_PickDate_EDate.DateValue.ToString().Trim());

            //sql = @" SELECT ROW_NUMBER() OVER(ORDER BY " + SortExpression + " " + SortDire + @") AS NewIDNum,
            sql = @" SELECT ROW_NUMBER() OVER(ORDER BY CONVERT(VARCHAR(10),B01_CardLog.CardTime,121) DESC, B01_CardLog.CardNo) AS NewIDNum,
                B00_CardLogState.StateDesc AS 'LogStatusName', CONVERT(VARCHAR(10),B01_CardLog.CardTime,121) AS 'Date',
                MIN(SUBSTRING(CONVERT(VARCHAR(50),B01_CardLog.CardTime,121),12,8)) AS 'First',
                MAX(SUBSTRING(CONVERT(VARCHAR(50),B01_CardLog.CardTime,121),12,8)) AS 'Last',
                B01_CardLog.CardNo, (SELECT OrgName FROM OrgStrucAllData('Department') WHERE OrgIDList = B01_CardLog.OrgStruc) AS 'Dept',
                B01_CardLog.PsnNo, B01_CardLog.PsnName
                FROM " + strUnionTB + " AS B01_CardLog " +
                @"LEFT JOIN B00_CardLogState ON B00_CardLogState.Code = B01_CardLog.LogStatus
                LEFT JOIN B00_ItemList ON B00_ItemList.ItemClass = 'CardType' AND B00_ItemList.ItemNo = B01_CardLog.CardType ";

            #region DataAuth
            /*
            if (hUserID.Value.Equals("User") && !hPsnID.Value.Equals(""))
            {
                sql += " WHERE (SysUser.UserID = ? AND Person.PsnID = ?)) AS MgnPerson ON MgnPerson.PsnNo = B01_CardLog.PsnNo ";
                liSqlPara.Add("A:" + hUserID.Value);
                liSqlPara.Add("A:" + hPsnID.Value);
            }
            else
            {
                sql += " WHERE (SysUser.UserID = ?)) AS MgnPerson ON MgnPerson.PsnNo = B01_CardLog.PsnNo ";
                liSqlPara.Add("A:" + hUserID.Value);
            }
            */
            if (wheresql != "") wheresql += " AND ";
            wheresql += " PsnNo=(SELECT PsnNo FROM B01_Person WHERE PsnID=?) ";
            liSqlPara.Add("S:" + this.sPsnID);

            if (wheresql != "") wheresql += " AND ";
            //20151201 修改查詢(Query Function)條件 CardType = 'E'(原為 A)
            //wheresql += " (B01_CardLog.CardType = 'E' OR B01_CardLog.CardType = 'T') AND (B01_CardLog.IsAndTrt = '1' OR B01_CardLog.EquClass = 'TRT') ";

            // 20170327 拿掉CardType的判斷
            wheresql += " (B01_CardLog.IsAndTrt = '1' OR B01_CardLog.EquClass = 'TRT') ";

            #endregion

            ////設定查詢條件
            if (!string.IsNullOrEmpty(QueryLabel_PickDate_SDate.DateValue.ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (CONVERT(VARCHAR(10),B01_CardLog.CardTime,111) >= ? ) ";
                liSqlPara.Add("S:" + QueryLabel_PickDate_SDate.DateValue.ToString());
                bQueryCondition = true;
            }

            if (!string.IsNullOrEmpty(QueryLabel_PickDate_EDate.DateValue.ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (CONVERT(VARCHAR(10),B01_CardLog.CardTime,111) <= ? ) ";
                liSqlPara.Add("S:" + QueryLabel_PickDate_EDate.DateValue.ToString());
                bQueryCondition = true;
            }

            if (!string.IsNullOrEmpty(QueryTextBox_CardNo.Text.ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (B01_CardLog.CardNo LIKE ? ) ";
                liSqlPara.Add("S:" + "%" + QueryTextBox_CardNo.Text.ToString().Trim() + "%");
                bQueryCondition = true;
            }

            if (!string.IsNullOrEmpty(QueryTextBox_PsnNo.Text.ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (B01_CardLog.PsnNo LIKE ? OR B01_CardLog.PsnName LIKE ?) ";
                liSqlPara.Add("S:" + "%" + QueryTextBox_PsnNo.Text.ToString().Trim() + "%");
                liSqlPara.Add("S:" + "%" + QueryTextBox_PsnNo.Text.ToString().Trim() + "%");
                bQueryCondition = true;
            }

            //設定查詢排序
            if (bQueryCondition)
            {
                if (wheresql != "")
                    sql += " WHERE ";
            }
            else
                sql += " WHERE 1 = 0 AND ";


            sql += wheresql + @" GROUP BY CONVERT(VARCHAR(10),B01_CardLog.CardTime,121), B00_CardLogState.StateDesc, B01_CardLog.CardNo, B01_CardLog.CardVer, B01_CardLog.PsnNo, B01_CardLog.PsnName, B01_CardLog.OrgStruc ";
            //_datacount = oAcsDB.DataCount(sql, liSqlPara);
            //hDataRowCount.Value = _datacount.ToString();

            // (一)先用 _sqlcommand 得到 dtTmp，然後得到總筆數
            DataTable dtTmp = new DataTable();
            oAcsDB.GetDataTable("ALL", sql, liSqlPara, out dtTmp);
            _datacount = dtTmp.Rows.Count;
            hDataRowCount.Value = _datacount.ToString();

            if (bMode == true)
            {
                oAcsDB.GetDataTable("CardLog", sql, liSqlPara, out CardLogTable);
            }
            else
            {
                //CardLogTable = oAcsDB.PageData(sql, liSqlPara, "NewIDNum", _pageindex + 1, _pagesize);
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
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('無資料可匯出!!');location.href='QueryTRTFirstEndCardLog.aspx';", true);
                }
            }
            else
            {
                //更新查詢條件與表格資料
                GirdViewDataBind(this.MainGridView, CardLogTable);
                UpdatePanel1.Update();
            }

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI", "$.unblockUI();", true);
        }

        #endregion
    }
}