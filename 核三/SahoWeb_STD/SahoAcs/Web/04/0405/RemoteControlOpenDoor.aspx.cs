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
using SMS_Communication;
using System.Configuration;
using DapperDataObjectLib;
using SahoAcs.DBClass;


namespace SahoAcs
{
    public partial class RemoteControlOpenDoor : System.Web.UI.Page
    {
        #region 一.宣告
        private int m_PageSize  = 10;                                    //設定GridView控制項每頁可顯示的資料列數
        private int m_ShowRange = 5;                                     //設定GridView制制項最大可顯示的頁數範圍
        private DataTable m_MainGridViewDataTable  = null;               //記錄GridView控制項的資料表
        private Hashtable m_GVRCheckStateHashtable = null;               //記錄GridView控制項所有資料列的勾選狀態
        AjaxControlToolkit.ToolkitScriptManager m_ToolKitScriptManager;  //宣告AjaxControlToolkit元件
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        #endregion

        #region 二.網頁

        #region 2-1A.網頁：事件方法
        /// <summary>
        /// 初始化網頁相關的動作
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            #region 註冊與設定網頁的相關資料
            RegisterComponeAndScript();
            RegisterWindowsAndButton();

            ChangeLanguage();
            MainGridView.PageSize    = m_PageSize;
            m_GVRCheckStateHashtable = GetGVRCheckStateHashtable(hSaveGVRCheckStateList.Value);
            #endregion

            #region 處理網頁載入與PostBack的相關動作
            if ((!IsPostBack) && (!m_ToolKitScriptManager.IsInAsyncPostBack))
            {
                #region 處理網頁首次載入的相關動作
                hSaveComplexQueryData.Value = "";
                hUserID.Value    = Session["UserID"].ToString();
                hOwnerList.Value = Session["OwnerList"].ToString();
                this.sLocArea.Value = "";
                this.sLocBuilding.Value = "";
                this.sLocFloor.Value = "";
                this.tmpLocArea.Value = "";
                this.tmpLocBuilding.Value = "";

                if (Session["SahoWebSocket"] != null) { ((SahoWebSocket)Session["SahoWebSocket"]).ClearCmdResult(); }

                Query(true, "");
                #endregion
            }
            else
            {
                #region 處理網頁PostBack的相關動作
                string sFormTarget = Request.Form["__EVENTTARGET"];    //取得來源目標
                string sFormArg    = Request.Form["__EVENTARGUMENT"];  //取得來源參數

                if ((sFormTarget == this.QueryButton.ClientID) || (sFormTarget == this.ComplexQueryButton.ClientID))
                {
                    #region 處理按下查詢、複合查詢或傳送指令按鈕後的相關動作
                    if (Session["SahoWebSocket"] != null) { ((SahoWebSocket)Session["SahoWebSocket"]).ClearCmdResult(); }

                    Query(true, "");
                    #endregion
                }
                else
                {
                    if (string.IsNullOrEmpty(sFormArg))
                    {
                        #region 處理按下第一頁、前一頁、下一頁、最末頁與索引頁按鈕後的相關動作
                        Query(false, "");
                        #endregion
                    }
                    else
                    {
                        if (sFormArg == "popPagePost")
                        {
                            #region 進行因應新增或編輯後所需的換頁動作
                            //此網頁沒有新增或編輯的功能需求暫時保留
                            //int iFind = Query(false, "popPagePost");
                            //Sa.Web.Fun.RunJavaScript(this, "__doPostBack('ctl00$ContentPlaceHolder1$MainGridView', 'Page$" + iFind + "');");
                            #endregion
                        }
                        else if (sFormArg.Substring(0, 5) == "Page$")
                        {
                            #region 換頁完成後進行GridView控制項資料列的移動動作
                            Query(false, "popPagePost");
                            Sa.Web.Fun.RunJavaScript(this, "GridSelect();");
                            #endregion
                        }
                    }
                }
                #endregion
            }
            #endregion

            CreateRemoteControlOpenDoorPageComponent();
            SetDDLLocItemList();

            ScriptManager.RegisterStartupScript(this, GetType(), "SetLocListItem", "SetLocListItem();", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "SetADVLocListItem", "SetADVLocListItem();", true);
        }
        #endregion

        #region 2-1B.網頁：自訂方法
        /// <summary>
        /// 覆寫此方法用來修正'XX'型別必須置含runat=server的標記屬性
        /// </summary>
        /// <param name="oControl">伺服器控制項</param>
        public override void VerifyRenderingInServerForm(Control oControl)
        {
            //覆寫此方法用來修正'XX'型別必須置含runat=server的標記屬性
        }

        /// <summary>
        /// 註冊開啟與編輯對話視窗的JavaScript方法
        /// </summary>
        private void OpenDialog_Js()
        {
            string sJScript = "";

            sJScript = @"
                function OpenDialogAdd(theURL, win_width, win_height) {
                    var PosX = (screen.width - win_width) / 2;
                    var PosY = (screen.height - win_height) / 2;
                    features = 'dialogWidth=' + win_width + ',dialogHeight=' + win_height + ',dialogTop=' + PosY + ',dialogLeft=' + PosX + ';' 
                    window.showModalDialog(theURL, '', features);
                }

                function OpenDialogEdit(theURL, key, win_width, win_height) { 
                    var PosX = (screen.width - win_width) / 2; 
                    var PosY = (screen.height - win_height) / 2; 
                    features = 'dialogWidth=' + win_width + ',dialogHeight=' + win_height + ',dialogTop=' + PosY + ',dialogLeft=' + PosX + ';' 
                    window.showModalDialog(theURL+key, '', features);
                }";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "OpenDialog_Js", sJScript, true);
        }

        /// <summary>
        /// 註冊公用的元件與JavaScript檔案
        /// </summary>
        private void RegisterComponeAndScript()
        {
            #region 註冊ToolKitScriptManager元件
            m_ToolKitScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");

            m_ToolKitScriptManager.EnablePageMethods = true;
            m_ToolKitScriptManager.RegisterAsyncPostBackControl(QueryButton);
            m_ToolKitScriptManager.RegisterAsyncPostBackControl(ComplexQueryButton);
            #endregion

            #region 註冊JavaScript方法
            string sJScript = Sa.Web.Fun.ControlToJavaScript(this);

            sJScript += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            sJScript += "\nSetViewWindowMode('');";
            sJScript += "\nSetComplexQueryWindowMode('');";
            sJScript  = "<script type='text/javascript'>" + sJScript + "</script>";

            ClientScript.RegisterStartupScript(sJScript.GetType(), "OnPageLoad", sJScript);
            #endregion

            #region 註冊JavaScript檔案
            ClientScript.RegisterClientScriptInclude("SetddlLocScript", "/Web/04/SetddlLocScript.js?jsdate=" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入位置連動式下拉式選單(一般搜尋)
            ClientScript.RegisterClientScriptInclude("SetADVddlLocScript", "/Web/04/SetADVddlLocScript.js?jsdate=" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入位置連動式下拉式選單(進階搜尋)
            ClientScript.RegisterClientScriptInclude("RemoteControlOpenDoorScript", "RemoteControlOpenDoor.js?jsdate=" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
            #endregion
        }

        /// <summary>
        /// 註冊網頁的視窗與按鈕動作
        /// </summary>
        private void RegisterWindowsAndButton()
        {
            #region 註冊主要作業畫面的按鈕動作：網頁畫面設計一
            QueryButton.Attributes["onClick"]        = "SelectState(); return false;";
            ComplexQueryButton.Attributes["onClick"] = "CallComplexQueryWindow('" + GetGlobalResourceObject("ResourceCtrls","ttAdvance").ToString() + "'); return false;";
            ViewButton.Attributes["onClick"] = "CallViewWindow('" + GetGlobalResourceObject("ResourceCtrls", "ttAdvance").ToString() + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";
            #endregion

            #region 註冊複合查詢畫面的按鈕動作：網頁畫面設計二
            ImgCloseButton0.Attributes["onClick"]         = "CancelTrigger0.click(); return false;";
            popBtn0_Query.Attributes["onClick"]           = "SaveComplexQueryData(); SetComplexQueryWheresql(); return false;";
            popBtn0_Cancel.Attributes["onClick"]          = "CancelTrigger0.click(); return false;";
            popBtn0_ClearQueryParam.Attributes["onClick"] = "SetComplexQueryWindowMode(''); return false;";

            Pub.SetModalPopup(ModalPopupExtender0, 0);
            #endregion

            #region 註冊彈出作業畫面的按鈕動作：網頁畫面設計三
            ImgCloseButton1.Attributes["onClick"] = "SetViewWindowMode(''); CancelTrigger1.click(); return false;";
            popBtn1_Exit.Attributes["onClick"]    = "SetViewWindowMode(''); CancelTrigger1.click(); return false;";

            Pub.SetModalPopup(ModalPopupExtender1, 1);
            #endregion
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
        #endregion

        #region 3-1B.元件-動態：自訂方法
        /// <summary>
        /// 建立門禁搖控開門網頁的相關元件
        /// </summary>
        private void CreateRemoteControlOpenDoorPageComponent()
        {
            DB_Acs oAcsDB = null;
            DataTable dt  = null;

            try
            {
                oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

                string sSql = "";

                #region SQL：SELECT B00_ItemList
                sSql  = " SELECT ItemOrder , ItemNo , ItemName";
                sSql += " FROM B00_ItemList";
                sSql += " WHERE ItemClass = 'EquModel'";
                sSql += " ORDER BY ItemOrder; ";

                oAcsDB.GetDataTable("ItemListTable", sSql, out dt);
                #endregion

                #region 建立「複合查詢」視窗畫面該「刷卡結果」欄位的「DropDownList」元件
                System.Web.UI.WebControls.DropDownList popDDList0_EquModel = new System.Web.UI.WebControls.DropDownList();

                popDDList0_EquModel.Enabled = true;
                popDDList0_EquModel.Width   = 200;
                popDDList0_EquModel.ID      = "popInput0_EquModel";

                popDDList0_EquModel.Items.Add(GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString());
                popDDList0_EquModel.Items[0].Value = "";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    popDDList0_EquModel.Items.Add(dt.Rows[i]["ItemName"].ToString().Trim());
                    popDDList0_EquModel.Items[i + 1].Value = dt.Rows[i]["ItemNo"].ToString().Trim();
                }

                popPanel0_EquModel.Controls.Add(popDDList0_EquModel);
                #endregion
            }
            finally
            {
                oAcsDB = null;
                if (dt != null) { dt.Clear(); dt = null; }
            }
        }

        /// <summary>
        /// 建立位置的相關元件
        /// </summary>
        private void SetDDLLocItemList()
        {
            DB_Acs oAcsDB = null;
            DataTable dt = null;

            this.txtAreaList.Value = "";
            this.txtBuildingList.Value = "";
            this.txtFloorList.Value = "";

            try
            {
                oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

                string sSql = "";

                #region SQL
                sSql = @"
                        select * from (
                        select LocType, LocID, LocNo, LocName, LocPID from B01_Location where LocType = 'AREA'
                        UNION
                        select LocType, LocID, LocNo, LocName, LocPID from B01_Location where LocType = 'BUILDING' 
                        UNION 
                        select LocType, LocID, LocNo, LocName, LocPID from B01_Location where LocType = 'FLOOR' 
                        ) as R
                        ORDER BY R.LocType, LocID
                    ";

                oAcsDB.GetDataTable("LocInfo", sSql, out dt);
                #endregion

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["LocType"].ToString() == "AREA")
                    {
                        this.txtAreaList.Value += $"{dt.Rows[i]["LocID"].ToString()}|{dt.Rows[i]["LocNo"].ToString()}|{dt.Rows[i]["LocName"].ToString()}|{dt.Rows[i]["LocPID"].ToString()},";
                    }
                    if (dt.Rows[i]["LocType"].ToString() == "BUILDING")
                    {
                        this.txtBuildingList.Value += $"{dt.Rows[i]["LocID"].ToString()}|{dt.Rows[i]["LocNo"].ToString()}|{dt.Rows[i]["LocName"].ToString()}|{dt.Rows[i]["LocPID"].ToString()},";
                    }
                    if (dt.Rows[i]["LocType"].ToString() == "FLOOR")
                    {
                        this.txtFloorList.Value += $"{dt.Rows[i]["LocID"].ToString()}|{dt.Rows[i]["LocNo"].ToString()}|{dt.Rows[i]["LocName"].ToString()}|{dt.Rows[i]["LocPID"].ToString()},";
                    }
                }

                this.txtAreaList.Value = this.txtAreaList.Value.TrimEnd(',');
                this.txtBuildingList.Value = this.txtBuildingList.Value.TrimEnd(',');
                this.txtFloorList.Value = this.txtFloorList.Value.TrimEnd(',');

            }
            finally
            {
                oAcsDB = null;
                if (dt != null) { dt.Clear(); dt = null; }
            }
        }
        #endregion

        #region 3-2A.元件-表格：事件方法
        /// <summary>
        /// 處理GridView控制項在變更分頁索引時的相關動作
        /// </summary>
        protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            MainGridView.PageIndex = e.NewPageIndex;
            if (MainGridView.Rows[0].Cells.Count == MainGridView.HeaderRow.Cells.Count) { MainGridView.DataBind(); }
        }

        /// <summary>
        /// 處理GridView控制項在完成資料繫結後的相關動作
        /// </summary>
        protected void GridView_Data_DataBound(object sender, EventArgs e)
        {
            td_showGridView.Attributes["colspan"] = MainGridView.Columns.Count.ToString();

            #region 設定表格資料列的勾選狀態與事件
            string sEquID = "";

            foreach (GridViewRow oGridViewRow in MainGridView.Rows)
            {
                sEquID = oGridViewRow.Cells[1].Text.Trim();
                if (m_GVRCheckStateHashtable.ContainsKey(sEquID)) { ((CheckBox)oGridViewRow.Cells[0].Controls[1]).Checked = (m_GVRCheckStateHashtable[sEquID].ToString() == "1") ? true : false; }
            }
            #endregion
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
                    e.Row.Cells[0].Width = 40;   //自訂：選項
                    e.Row.Cells[1].Width = 50;   //設備識別碼
                    e.Row.Cells[2].Width = 120;  //設備編號
                    e.Row.Cells[3].Width = 200;  //護備名稱
                    e.Row.Cells[4].Width = 40;   //是否兼做卡鐘
                    e.Row.Cells[5].Width = 90;   //設備型號
                    e.Row.Cells[6].Width = 90;   //設備類別
                    e.Row.Cells[7].Width = 220;  //位置
                    e.Row.Cells[8].Width = 40;   //自訂：傳送狀態
                    //e.Row.Cells[9].Width = 150;  //自訂：回應狀態(此部份不用設定)
                    #endregion

                    #region A-2.設定表格標題列狀態與與事件的方法
                    CheckBox cbxRow = (CheckBox)e.Row.Cells[0].Controls[1];

                    cbxRow.Checked = bool.Parse(hSaveGVHCheckState.Value);
                    cbxRow.Attributes.Add("OnClick", "SelectAll(this);");
                    #endregion

                    #region A-3.寫入Literal_Header
                    StringWriter Header_sw       = null;
                    HtmlTextWriter Header_writer = null;

                    try
                    {
                        Header_sw     = new StringWriter();
                        Header_writer = new HtmlTextWriter(Header_sw);

                        e.Row.CssClass = "GVStyle";
                        e.Row.RenderControl(Header_writer);
                        e.Row.Visible  = false;

                        li_header.Text = Header_sw.ToString();
                    }
                    finally
                    {
                        if (Header_writer != null) { Header_writer.Close(); Header_writer.Dispose(); Header_writer = null; }
                        if (Header_sw != null) { Header_sw.Close(); Header_sw.Dispose(); Header_sw = null; }
                    }
                    #endregion
                    break;
                #endregion

                #region B.設定表格的資料部份
                case DataControlRowType.DataRow:
                    #region B-1.指定資料列的代碼與函式
                    GridViewRow oGVRow  = e.Row;

                    oGVRow.ClientIDMode = ClientIDMode.Static;
                    oGVRow.ID           = "GVRow_" + oGVRow.Cells[1].Text.Trim();  //設定資料列代碼
                    ((CheckBox)oGVRow.Cells[0].Controls[1]).Attributes.Add("OnClick", "ChangeGVRCheckState('" + oGVRow.Cells[1].Text.Trim() + "'); ");
                    #endregion

                    #region B-2.設定欄位隱藏與寬度
                    e.Row.Cells[0].Width = 43;   //自訂：選項
                    e.Row.Cells[1].Width = 54;   //設備識別碼
                    e.Row.Cells[2].Width = 124;  //設備編號
                    e.Row.Cells[3].Width = 204;  //護備名稱
                    e.Row.Cells[4].Width = 44;   //是否兼做卡鐘
                    e.Row.Cells[5].Width = 94;   //設備型號
                    e.Row.Cells[6].Width = 94;   //設備類別
                    e.Row.Cells[7].Width = 224;   //位置
                    e.Row.Cells[8].Width = 44;   //自訂：傳送狀態
                                                 //e.Row.Cells[9].Width = 154;  //自訂：回應狀態(此部份不用設定)
                    #endregion

                    #region B-3.處理欄位資料的格式
                    //不用處理的欄位分別有：2.設備編號、3.設備名稱、4.設備英文名稱、9.建築物名稱與11.自訂：回應結果

                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;  //0.自訂：選項
                    e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Center;  //1.設備識別碼
                    e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Center;  //2.設備編號
                    e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Center;  //4.是否兼做卡鐘
                    e.Row.Cells[5].HorizontalAlign = HorizontalAlign.Center;  //5.設備型號
                    e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Center;  //6.設備類位
                    e.Row.Cells[8].HorizontalAlign = HorizontalAlign.Center;  //10.自訂：指令狀態
                    #endregion

                    #region B-4.限制欄位資料的長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && (e.Row.Cells[x].Text != "&nbsp;")) { e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text; }

                    #endregion

                    #region B-5.設定表格資料列的事件方法
                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0, this, '', '');");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0, this);");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, hSelectValue, '" + oGVRow.Cells[1].Text.Trim() + "', '', '');");  //設定資料列代碼
                    e.Row.Attributes.Add("OnDblclick", "CallViewWindow('" + "檢視「控制器」設備資料" + "');");
                    #endregion
                    break;
                #endregion

                #region C.設定表格的換頁部份
                case DataControlRowType.Pager:
                    #region C-1.取得相關的換頁控制項
                    GridView oGridView = sender as GridView;

                    LinkButton lbtnFirst = e.Row.FindControl("lbtnFirst") as LinkButton;
                    LinkButton lbtnLast  = e.Row.FindControl("lbtnLast") as LinkButton;
                    PlaceHolder phdPageNumber = e.Row.FindControl("phdPageNumber") as PlaceHolder;
                    LinkButton lbtnPrev  = e.Row.FindControl("lbtnPrev") as LinkButton;
                    LinkButton lbtnNext  = e.Row.FindControl("lbtnNext") as LinkButton;
                    LinkButton lbtnPage;
                    #endregion

                    #region C-2.顯示頁數列及處理上下頁、首頁、末頁動作
                    int iShowRange  = m_ShowRange;
                    int iPageCount  = oGridView.PageCount;
                    int iPageIndex  = oGridView.PageIndex;
                    int iStartIndex = ((iPageIndex + 1) < iShowRange) ? 0 : (((iPageIndex + 1 + (iShowRange / 2)) >= iPageCount) ? (iPageCount - iShowRange) : (iPageIndex - iShowRange / 2));
                    int iEndIndex   = (iStartIndex >= (iPageCount - iShowRange)) ? iPageCount : (iStartIndex + iShowRange);

                    #region C-2-1.頁數列
                    phdPageNumber.Controls.Add(new LiteralControl("  "));

                    //指定頁數及改變文字風格
                    for (int i = iStartIndex; i < iEndIndex; i++)
                    {
                        lbtnPage = new LinkButton();

                        lbtnPage.Text            = (i + 1).ToString();
                        lbtnPage.CommandName     = "Page";
                        lbtnPage.CommandArgument = (i + 1).ToString();
                        lbtnPage.Font.Overline   = false;

                        if (i != iPageIndex)
                        {
                            lbtnPage.Font.Bold = false;
                        }
                        else
                        {
                            lbtnPage.Font.Bold       = true;
                            lbtnPage.ForeColor       = System.Drawing.Color.White;
                            lbtnPage.CommandArgument = "";
                        }

                        phdPageNumber.Controls.Add(lbtnPage);
                        phdPageNumber.Controls.Add(new LiteralControl(" "));
                    }
                    #endregion

                    #region C-2-2.上下頁
                    lbtnPrev.Click += delegate(object obj, EventArgs args)
                    {
                        if (oGridView.PageIndex > 0)
                        {
                            oGridView.PageIndex = oGridView.PageIndex - 1;
                            Query(false, "");
                        }
                    };

                    lbtnNext.Click += delegate(object obj, EventArgs args)
                    {
                        if (oGridView.PageIndex < oGridView.PageCount)
                        {
                            oGridView.PageIndex = oGridView.PageIndex + 1;
                            Query(false, "");
                        }
                    };
                    #endregion

                    #region C-2-3.首末頁
                    lbtnFirst.Click += delegate(object obj, EventArgs args)
                    {
                        oGridView.PageIndex = 0;
                        Query(false, "");
                    };

                    lbtnLast.Click += delegate(object obj, EventArgs args)
                    {
                        oGridView.PageIndex = MainGridView.PageCount;
                        Query(false, "");
                    };
                    #endregion
                    #endregion

                    #region C-3.顯示總頁數及目前頁數
                    phdPageNumber.Controls.Add(new LiteralControl(string.Format("　{0} / {1}　", iPageIndex + 1, iPageCount)));
                    phdPageNumber.Controls.Add(new LiteralControl("  "));
                    #endregion

                    #region C-4.顯示總筆數
                    phdPageNumber.Controls.Add(
                        new LiteralControl(string.Format(this.GetGlobalResourceObject("Resource","lblGvCounts").ToString(), hDataRowCount.Value)));
                    phdPageNumber.Controls.Add(
                        new LiteralControl("  "));
                    #endregion

                    #region C-5.寫入Literal_Pager
                    StringWriter Pager_sw      = null;
                    HtmlTextWriter Pager_writer = null;

                    try
                    {
                        Pager_sw     = new StringWriter();
                        Pager_writer = new HtmlTextWriter(Pager_sw);

                        e.Row.CssClass = "GVStylePgr";
                        e.Row.RenderControl(Pager_writer);
                        e.Row.Visible  = false;

                        li_Pager.Text = Pager_sw.ToString();
                    }
                    finally
                    {
                        if (Pager_writer != null) { Pager_writer.Close(); Pager_writer.Dispose(); Pager_writer = null; }
                        if (Pager_sw != null) { Pager_sw.Close(); Pager_sw.Dispose(); Pager_sw = null; }
                    }
                    #endregion
                    break;
                #endregion
            }
        }
        #endregion

        #region 3-2B.元件-表格：自訂方法
        /// <summary>
        /// 限制來源字串的顯示長度
        /// </summary>
        /// <param name="sText">來源字串</param>
        /// <param name="iLength">顯示長度</param>
        /// <param name="bIsEllipsis">是否省略</param>
        /// <returns>String</returns>
        private string LimitText(string sText, int iLength, bool bIsEllipsis)
        {
            byte[] byteTextArray = null;
            Encoding oBig5       = null;

            try
            {
                oBig5 = Encoding.GetEncoding("big5");
                byteTextArray = oBig5.GetBytes(sText);

                if (byteTextArray.Length > iLength)
                {
                    if (bIsEllipsis) { iLength -= 3; }

                    sText = oBig5.GetString(byteTextArray, 0, iLength);
                    if (!oBig5.GetString(byteTextArray).StartsWith(sText)) { sText = oBig5.GetString(byteTextArray, 0, (iLength - 1)); }

                    sText += (bIsEllipsis ? "..." : "");
                }

                return sText;
            }
            finally
            {
                oBig5 = null;
                byteTextArray = null;
            }
        }

        /// <summary>
        /// 處理資料表繫結至GridView控制項時的相關動作
        /// </summary>
        /// <param name="oGridView">GridView控制項</param>
        /// <param name="dt">資料表</param>
        private void GirdViewDataBind(GridView oGridView, DataTable dt)
        {
            if (dt.Rows.Count != 0)
            {
                #region 處理GridView控制項含有資料內容的情況
                oGridView.DataSource = dt;
                oGridView.DataBind();
                #endregion
            }
            else
            {
                #region 處理GridView控制項沒有含有資料內容的情況
                dt.Rows.Add(dt.NewRow());

                oGridView.DataSource = dt;
                oGridView.DataBind();

                int columnCount = oGridView.Rows[0].Cells.Count;

                oGridView.Rows[0].Cells.Clear();
                oGridView.Rows[0].Cells.Add(new TableCell());
                oGridView.Rows[0].Cells[0].ColumnSpan = columnCount;
                oGridView.Rows[0].Cells[0].Text = this.GetGlobalResourceObject("Resource","NonData").ToString();
                #endregion
            }
        }

        /// <summary>
        /// 取得GridView控制項每個資料列的勾選狀態清單之雜湊表
        /// </summary>
        /// <param name="sGVRCheckStateList">GridView控制項每個資料列的勾選狀態清單</param>
        /// <returns>Hashtable</returns>
        private Hashtable GetGVRCheckStateHashtable(string sGVRCheckStateList)
        {
            string[] sGVCheckStateArray = null;
            Hashtable htResult = new Hashtable();

            try
            {
                if (!string.IsNullOrEmpty(sGVRCheckStateList))
                {
                    sGVCheckStateArray = sGVRCheckStateList.Split('/');

                    if ((sGVCheckStateArray != null) && (sGVCheckStateArray.Length > 0))
                    {
                        htResult.Clear();
                        for (int i = 0; i < sGVCheckStateArray.Length; i += 2) { htResult.Add(sGVCheckStateArray[i], sGVCheckStateArray[i + 1]); }
                    }
                }
            }
            finally
            {
                sGVCheckStateArray = null;
            }

            return htResult;
        }
        #endregion
        #endregion

        #region 四.查詢、複合查詢語法
        /// <summary>
        /// 依據指定的模式與條件內容查詢資料並更新顯示於GridView控制項
        /// </summary>
        /// <param name="bIsResetGVCheckState">是否重置GridView控制項標題列與資料列勾選狀態</param>
        /// <param name="sQueryMode">查詢模式(空子串(一般查詢)、popPagePost(新增或刪除時的查詢)</param>
        /// <returns>查詢模式：空字串(預設為-1)、popPagePost(傳回目前的索引頁)</returns>
        private int Query(bool bIsResetGVCheckState, string sQueryMode)
        {
            DB_Acs oAcsDB = null;

            try
            {
                oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

                int iResult = -1;
                string sSql = "", sWhereSql = "", sIsAndTrtListSql = "";

                if (m_MainGridViewDataTable != null) { m_MainGridViewDataTable.Clear(); }

                #region SQL：SELECT B01_EquData
                sIsAndTrtListSql = @"((SELECT '0' AS IsAndTrtCode , 'N' AS IsAndTrtName) UNION (SELECT '1' AS IsAndTrtCode , 'Y' AS IsAndTrtName))";

                sSql = @"SELECT DISTINCT EquData.EquID, EquData.EquNo, EquData.EquName, EquData.EquEName, EquData.IsAndTrt,
                    EquData.EquModel, EquData.EquClass, dbo.Get_LocParentName(LocID, 'FLOOR') AS 'Floor', dbo.Get_LocParentName(LocID, 'Building') AS 'Building', 
                    IsAndTrt.IsAndTrtName AS IsAndTrtName , ItemList.ItemName AS EquModelName, dbo.Get_LocParentName(LocID, 'AREA') AS 'AREA',
                    dbo.Get_LocLongName(LocID, ' / ') AS LocLongName
                    FROM B01_EquData AS EquData
                    LEFT JOIN " + sIsAndTrtListSql + @" AS IsAndTrt ON IsAndTrt.IsAndTrtCode = EquData.IsAndTrt
                    LEFT JOIN B00_ItemList AS ItemList ON ItemList.ItemNo = EquData.EquModel AND ItemList.ItemNo = EquData.EquModel
                    INNER JOIN B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID
                    INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID
                    INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID
                    INNER JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID";

                #region 設定查詢條件
                if (!string.IsNullOrEmpty(hComplexQueryWheresql.Value))  //複合查詢
                {
                    sWhereSql = hComplexQueryWheresql.Value;
                }
                else                                                     //關鍵字查詢
                {
                    string sArea = this.sLocArea.Value.ToString().Trim();
                    string sBuilding = this.sLocBuilding.Value.ToString().Trim();
                    string sFloor = this.sLocFloor.Value.ToString().Trim();

                    if (!string.IsNullOrEmpty(sArea) && !string.IsNullOrEmpty(sBuilding) && !string.IsNullOrEmpty(sFloor))
                    {
                        sWhereSql += "( dbo.Get_LocParentID(EquData.LocID, 'FLOOR') = " + sFloor + " )";
                    }
                    else if (!string.IsNullOrEmpty(sArea) && !string.IsNullOrEmpty(sBuilding) && string.IsNullOrEmpty(sFloor))
                    {
                        sWhereSql += "( dbo.Get_LocParentID(EquData.LocID, 'BUILDING') = " + sBuilding + " )";
                    }
                    else if (!string.IsNullOrEmpty(sArea) && string.IsNullOrEmpty(sBuilding) && string.IsNullOrEmpty(sFloor))
                    {
                        sWhereSql += "( dbo.Get_LocParentID(EquData.LocID, 'AREA') = " + sArea + " )";
                    }
                }
                #endregion

                #region DataAuth
                if (sWhereSql != "")
                    sWhereSql += " AND ";
                sWhereSql += " (SysUser.UserID ='" + Session["UserID"].ToString() + "')";
                if (sWhereSql != "")
                    sWhereSql += " AND ";
                sWhereSql += " EquClass='Door Access' ";
                #endregion

                if (sWhereSql != "") { sWhereSql = " WHERE " + sWhereSql; }

                //設定排序條件
                sSql += (sWhereSql + " ORDER BY EquData.EquNo ASC;");

                oAcsDB.GetDataTable("EquDataTable", sSql, out m_MainGridViewDataTable);
                hDataRowCount.Value = m_MainGridViewDataTable.Rows.Count.ToString();
                #endregion

                #region 設定GridView控制項標題列與資料列的勾選狀態均設為預設值(勾消)
                if (bIsResetGVCheckState)
                {
                    string sGVRCheckStateList = "";

                    if ((m_MainGridViewDataTable != null) && (m_MainGridViewDataTable.Rows.Count > 0))
                    {
                        foreach (DataRow dataRow in m_MainGridViewDataTable.Rows)
                        {
                            if (!string.IsNullOrEmpty(sGVRCheckStateList)) { sGVRCheckStateList += "/"; }
                            sGVRCheckStateList += (dataRow["EquID"].ToString() + "/" + "0");
                        }
                    }

                    hSaveGVHCheckState.Value     = "false";
                    hSaveGVRCheckStateList.Value = sGVRCheckStateList;
                }
                #endregion

                #region 更新查詢條件與GridView控制項的資料內容
                GirdViewDataBind(this.MainGridView, m_MainGridViewDataTable);
                UpdatePanel0.Update();
                UpdatePanel1.Update();
                #endregion

                #region 完成查詢後並取得資料目前的表格頁數
                if (sQueryMode == "popPagePost")
                {
                    int iFind = 0;

                    for (int i = 0; i < m_MainGridViewDataTable.Rows.Count; i++) { if (hSelectValue.Value == m_MainGridViewDataTable.Rows[i]["EquID"].ToString()) { iFind = i; break; } }
                    iResult = (iFind / m_PageSize) + 1;
                }
                #endregion

                return iResult;
            }
            finally
            {
                oAcsDB = null;
            }
        }

        /// <summary>
        /// 設定複合查詢的SQL條件語法
        /// </summary>
        /// <param name="sParamArray">查詢參數資料</param>
        /// <returns>Pub.MessageObject</returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object SetComplexQueryWheresql(string[] sParamArray)
        {
            DB_Acs oAcsDB             = null;
            Pub.MessageObject oMsgObj = null;

            try
            {
                oAcsDB  = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
                oMsgObj = new Pub.MessageObject();
                oMsgObj.act     = "ComplexQuery";
                oMsgObj.message = "";
                oMsgObj.result  = false;

                string sWhereSql = "";

                #region 設定複合查詢相關的欄位條件
                if ((sParamArray != null) && (sParamArray.Length > 0))
                {
                    //0.設備編號
                    if (!string.IsNullOrEmpty(sParamArray[0].Trim()))
                    {
                        sWhereSql += "(EquData.EquNo LIKE '%" + sParamArray[0].Trim() + "%')";
                    }

                    //1.設備名稱
                    if (!string.IsNullOrEmpty(sParamArray[1].Trim()))
                    {
                        if (sWhereSql != "") { sWhereSql += " AND "; }
                        sWhereSql += "(EquData.EquName LIKE '%" + sParamArray[1].Trim() + "%')";
                    }

                    //2.設備英文名稱
                    if (!string.IsNullOrEmpty(sParamArray[2].Trim()))
                    {
                        if (sWhereSql != "") { sWhereSql += " AND "; }
                        sWhereSql += "(EquData.EquEName LIKE '%" + sParamArray[2].Trim() + "%')";
                    }

                    //3.是否兼做卡鐘
                    if (!string.IsNullOrEmpty(sParamArray[3].Trim()))
                    {
                        if (sWhereSql != "") { sWhereSql += " AND "; }
                        sWhereSql += "(EquData.IsAndTrt = '" + sParamArray[3].Trim() + "')";
                    }

                    //4.設備型號
                    if (!string.IsNullOrEmpty(sParamArray[4].Trim()))
                    {
                        if (sWhereSql != "") { sWhereSql += " AND "; }
                        sWhereSql += "(EquData.EquModel = '" + sParamArray[4].Trim() + "')";
                    }

                    //5.設備類別
                    if (!string.IsNullOrEmpty(sParamArray[5].Trim()))
                    {
                        if (sWhereSql != "") { sWhereSql += " AND "; }
                        sWhereSql += "(EquData.EquClass LIKE '%" + sParamArray[5].Trim() + "%')";
                    }

                    //位置
                    string sArea = sParamArray[6].Trim();
                    string sBuilding = sParamArray[7].Trim();
                    string sFloor = sParamArray[8].Trim();

                    if (!string.IsNullOrEmpty(sArea))
                    {
                        if (sWhereSql != "") { sWhereSql += " AND "; }
                        sWhereSql += "( dbo.Get_LocParentID(EquData.LocID, 'AREA') = " + sArea + " )";
                    }

                    if (!string.IsNullOrEmpty(sBuilding))
                    {
                        if (sWhereSql != "") { sWhereSql += " AND "; }
                        sWhereSql += "( dbo.Get_LocParentID(EquData.LocID, 'BUILDING') = " + sBuilding + " )";
                    }

                    if (!string.IsNullOrEmpty(sFloor))
                    {
                        if (sWhereSql != "") { sWhereSql += " AND "; }
                        sWhereSql += "( dbo.Get_LocParentID(EquData.LocID, 'FLOOR') = " + sFloor + " )";
                    }
                }
                #endregion

                oMsgObj.message = sWhereSql;
                oMsgObj.result  = true;
                return oMsgObj;
            }
            finally
            {
                oAcsDB = null;
            }
        }
        #endregion

        #region 五.載入
        /// <summary>
        /// 載入「檢視控制器設備資料」視窗相關的欄位資料
        /// </summary>
        /// <param name="sEquID">設備識別碼</param>
        /// <returns>string[] LoadViewWindosData</returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadViewWindowData(string sEquID)
        {
            DB_Acs oAcsDB     = null;
            Sa.DB.DBReader dr = null;
            string[] LoadViewWindosData = null;

            try
            {
                oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

                string sSql = "", sIsAndTrtListSql = "";

                #region SQL：SELECT B01_EquData
                sIsAndTrtListSql = @"((SELECT '0' AS IsAndTrtCode , 'N' AS IsAndTrtName) UNION (SELECT '1' AS IsAndTrtCode , 'Y' AS IsAndTrtName))";

                sSql = @"SELECT DISTINCT EquData.EquID, EquData.EquNo, EquData.EquName, EquData.EquEName, EquData.IsAndTrt,
                    EquData.EquModel, EquData.EquClass,  dbo.Get_LocParentName(LocID, 'FLOOR') AS 'Floor', dbo.Get_LocParentName(LocID, 'Building') AS 'Building',
                    IsAndTrt.IsAndTrtName AS IsAndTrtName , ItemList.ItemName AS EquModelName, dbo.Get_LocParentName(LocID, 'AREA') AS 'AREA',
                    dbo.Get_LocLongName(LocID, ' / ') AS LocLongName
                    FROM B01_EquData AS EquData
                    LEFT JOIN " + sIsAndTrtListSql + @" AS IsAndTrt ON IsAndTrt.IsAndTrtCode = EquData.IsAndTrt
                    LEFT JOIN B00_ItemList AS ItemList ON ItemList.ItemNo = EquData.EquModel AND ItemList.ItemNo = EquData.EquModel
                    WHERE EquData.EquID = '" + sEquID.Trim() + "';";

                oAcsDB.GetDataReader(sSql, out dr);
                #endregion

                #region 取得載入畫面相關的欄位資料
                if (dr.Read())
                {
                    LoadViewWindosData = new string[dr.DataReader.FieldCount];
                    for (int i = 0; i < dr.DataReader.FieldCount; i++) { LoadViewWindosData[i] = dr.DataReader[i].ToString(); }
                }
                else
                {
                    LoadViewWindosData = new string[2];
                    LoadViewWindosData[0] = "Saho_SysErrorMassage";
                    LoadViewWindosData[1] = "系統中無此資料！";
                }
                #endregion

                return LoadViewWindosData;
            }
            finally
            {
                oAcsDB = null;
                if (dr != null) { dr.Free(); dr = null; }
            }
        }
        #endregion

        #region 六.傳送指令、更新結果、指令字串清單、傳送指令字串清單
        /// <summary>
        /// 傳送指令按鈕
        /// </summary>
        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            SendAppCmdStrList(GetCheckedCtrlEquCmdStrList());
            RefreshButton_Click(sender, e);
        }

        /// <summary>
        /// 更新GridView控制項的勾選狀態與指令執行狀態和結果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RefreshButton_Click(object sender, EventArgs e)
        {
            string sEquID = "", sEquNo = "";
            SahoWebSocket oSWSocket = null;

            try
            {
                oSWSocket = (SahoWebSocket)Session["SahoWebSocket"];

                foreach (GridViewRow oGridViewRow in MainGridView.Rows)
                {
                    sEquID = oGridViewRow.Cells[1].Text;
                    sEquNo = oGridViewRow.Cells[2].Text;

                    //#region 更新GridView控制項的勾選狀態
                    //if (m_GVRCheckStateHashtable.ContainsKey(sEquID)) { ((CheckBox)oGridViewRow.Cells[0].Controls[1]).Checked = (m_GVRCheckStateHashtable[sEquID].ToString() == "1") ? true : false; }
                    //#endregion

                    #region 更新GridView控制項的指令執行狀態和結果

                    oGridViewRow.Cells[8].Text = "";
                    oGridViewRow.Cells[9].Text = "";

                    if ((oSWSocket != null) && oSWSocket.EquNoRecordIDHashtable.ContainsKey(sEquNo))
                    {
                        oGridViewRow.Cells[8].Text = ((SahoWebSocketCmdResult)oSWSocket.EquNoRecordIDHashtable[sEquNo]).CmdStateDesc;
                        oGridViewRow.Cells[9].Text = ((SahoWebSocketCmdResult)oSWSocket.EquNoRecordIDHashtable[sEquNo]).ResultMsg;
                    }

                    #endregion
                }
            }
            catch
            {
                oSWSocket = null;
            }
        }

        /// <summary>
        /// 取得被勾選的控制器設備指令字串清單
        /// </summary>
        /// <returns>控制器設備指令字串清單</returns>
        private string GetCheckedCtrlEquCmdStrList()
        {
            string[] sEquIDArray = null, sGVRCheckStateArray = null;
            DB_Acs oAcsDB = null;
            DataTable dt  = null;

            try
            {
                string sSql = "", sCmdStrList = "", sEquIDList = "";

                #region 取得被勾選得控制器設備代碼清單
                //sGVRCheckStateArray = hSaveGVRCheckStateList.Value.Split('/');

                //if (sGVRCheckStateArray.Length >= 2)
                //{
                //    for (int i = 1; i < sGVRCheckStateArray.Length; i += 2)
                //    {
                //        if (sGVRCheckStateArray[i] == "1")
                //        {
                //            if (!string.IsNullOrEmpty(sEquIDList)) { sEquIDList += ","; }

                //            sEquIDList += sGVRCheckStateArray[i - 1];
                //        }
                //    }
                //}

                foreach (GridViewRow row in MainGridView.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        CheckBox chkRow = (row.Cells[0].FindControl("RowCheckState") as CheckBox);
                        if (chkRow.Checked)
                        {
                            if (!string.IsNullOrEmpty(sEquIDList)) sEquIDList += ",";
                            sEquIDList += row.Cells[1].Text;
                        }
                    }
                }
                #endregion

                #region 取得被勾選的控制器設備代號指令字串清單
                if (!string.IsNullOrEmpty(sEquIDList))
                {
                    oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

                    #region SQL：SELECT B01_EquData
                    sSql  = " SELECT EquID,EquNo";
                    sSql += " FROM B01_EquData";
                    sSql += " ORDER BY EquID ASC; ";

                    oAcsDB.GetDataTable("EquDataTable", sSql, out dt);
                    #endregion

                    #region 取得傳送的指令字串清單
                    if ((dt != null) && (dt.Rows.Count > 0))
                    {
                        sEquIDArray = sEquIDList.Split(',');

                        for (int i = 0; i < sEquIDArray.Length; i++)
                        {
                            foreach (DataRow dataRow in dt.Rows)
                            {
                                if (sEquIDArray[i].Trim() == dataRow["EquID"].ToString().Trim())
                                {
                                    if (sCmdStrList != "") { sCmdStrList += ";"; }
                                    sCmdStrList += dataRow["EquID"].ToString().Trim();
                                    sCmdStrList += "@" + dataRow["EquNo"].ToString().Trim();
                                    sCmdStrList += "@" + "OpenDoorSet";
                                    sCmdStrList += "@";
                                    //產生開門紀錄到SysLog
                                    this.odo.Execute(@"INSERT INTO B00_SysLog (LogTime, LogType, UserID , UserName, LogFrom, EquNo, EquName, LogInfo) 
                                            VALUES (GETDATE(), @LogType, @UserID, @UserName, @LogFrom, @EquNo, @EquName, @LogInfo)", 
                                            new {UserID = Sa.Web.Fun.GetSessionStr(this,"UserID"),
                                                UserName = Sa.Web.Fun.GetSessionStr(this,"UserName"),
                                                LogFrom = "",
                                                LogType = DB_Acs.Logtype.指令傳送.ToString(),
                                                EquNo = dataRow["EquNo"].ToString(),
                                                EquName = "",
                                                LogInfo = "搖控開門資訊紀錄"});
                                    break;
                                }
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                return sCmdStrList;
            }
            finally
            {
                oAcsDB              = null;
                sEquIDArray         = null;
                sGVRCheckStateArray = null;
                if (dt != null) { dt.Clear(); dt = null; }
            }
        }

        /// <summary>
        /// 傳送APP指令字串清單至SahoWebSocket
        /// </summary>
        /// <param name="sCmdStrList">指令字串清單</param>
        private void SendAppCmdStrList(string sCmdStrList)
        {
            string[] sIPArray = null, sAppCmdStrArray = null;
            SahoWebSocket oSWSocket = null;

            try
            {
                if (!string.IsNullOrEmpty(sCmdStrList))
                {
                    #region 建立與設定SahoWebSocket
                    if (Session["SahoWebSocket"] != null)
                    {
                        if ((!((SahoWebSocket)Session["SahoWebSocket"]).IsWorking) || ((SahoWebSocket)Session["SahoWebSocket"]).IsGameOver)
                        {
                            ((SahoWebSocket)Session["SahoWebSocket"]).Stop();
                            ((SahoWebSocket)Session["SahoWebSocket"]).Start();
                        }
                    }
                    else
                    {
                        #region 取得APP的IP位址
                        string sIPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                        if (!string.IsNullOrEmpty(sIPAddress)) { sIPArray = sIPAddress.Split(new char[] { ',' }); } else { sIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; }
                        if (sIPAddress == "::1") { sIPAddress = "127.0.0.1"; }
                        #endregion

                        #region 建立與設定SahoWebSocket物件及基本資料
                        Session["SahoWebSocket"] = new SahoWebSocket();
                        ((SahoWebSocket)Session["SahoWebSocket"]).UserID = hUserID.Value;
                        ((SahoWebSocket)Session["SahoWebSocket"]).SourceIP = sIPAddress;
                        ((SahoWebSocket)Session["SahoWebSocket"]).SCListenIPPort = System.Web.Configuration.WebConfigurationManager.AppSettings["SCListenIPPort"].ToString();
                        ((SahoWebSocket)Session["SahoWebSocket"]).DbConnectionString = Pub.GetConnectionString(Pub.sConnName);
                        ((SahoWebSocket)Session["SahoWebSocket"]).Start();
                        #endregion
                    }
                    #endregion

                    #region 傳送APP指令字串
                    oSWSocket = (SahoWebSocket)Session["SahoWebSocket"];
                    string BeType = "0";
                    if (ConfigurationManager.AppSettings["LanguageOption"] != null && new string[] { "1", "0" }.Contains(ConfigurationManager.AppSettings["LanguageOption"]))
                    {
                        BeType = System.Configuration.ConfigurationManager.AppSettings["LanguageOption"];
                    }
                    oSWSocket.SetBECommTag(BeType);
                    oSWSocket.ClearCmdResult();
                    sAppCmdStrArray = sCmdStrList.Split(';');
                    for (int i = 0; i < sAppCmdStrArray.Length; i++)
                    {
                        oSWSocket.SendAppCmdStr(sAppCmdStrArray[i]);
                    }
                    #endregion
                }
            }
            finally
            {
                sIPArray        = null;
                oSWSocket       = null;
                sAppCmdStrArray = null;
            }
        }
        #endregion
    }
}