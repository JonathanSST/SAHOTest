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
    public partial class QueryAlarmMap : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 100;
        Hashtable TableInfo;
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.DropDown_Building);
            oScriptManager.RegisterAsyncPostBackControl(this.QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js = "<script type='text/javascript'>" + js + "</script>";
            ClientScript.RegisterClientScriptInclude("QueryAlarmMap", "QueryAlarmMap.js");        //加入同一頁面所需的JavaScript檔案

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
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion
                ViewState["SortExpression"] = "LogTime";
                ViewState["SortDire"] = "ASC";
                Query_LogTimeS.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 00:00:00";
                Query_LogTimeE.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 23:59:59";
                CreateEquGroupDropItem();
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
                    int[] HeaderWidth = { 140, 100, 140, 120, 120, 130, 70, 100, 130 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 警報時間

                    #endregion
                    #region 記錄狀態
                    #endregion
                    #region 讀卡時間
                    #endregion
                    #region 設備編號
                    #endregion
                    #region 設備名稱
                    #endregion
                    #region 人員編號
                    #endregion
                    #region 人員姓名
                    #endregion
                    #region 卡片編號
                    #endregion
                    #region 卡片版次
                    #endregion
                    #region 卡片類別
                    #endregion

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
                    if (!oRow.Row.IsNull("RecordID"))
                    {
                        #region 指定Row的ID
                        GridViewRow GrRow = e.Row;
                        GrRow.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        GrRow.ID = "GV_Row" + oRow.Row["RecordID"].ToString();
                        #endregion

                        #region 設定欄位寛度
                        int[] DataWidth = { 143, 104, 144, 124, 124, 133, 74, 104, 13 };
                        for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                            e.Row.Cells[i].Width = DataWidth[i];
                        #endregion

                        #region 針對各欄位做所需處理

                        #region 警報時間
                        e.Row.Cells[0].Text = DateTime.Parse(oRow["LogTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                        #endregion
                        #region 記錄狀態
                        #endregion
                        #region 讀卡時間
                        e.Row.Cells[2].Text = DateTime.Parse(oRow["CardTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                        #endregion
                        #region 設備編號
                        #endregion
                        #region 設備名稱
                        #endregion
                        #region 人員編號
                        #endregion
                        #region 人員姓名
                        #endregion
                        #region 卡片編號
                        if (!string.IsNullOrEmpty(oRow.Row["CardTime"].ToString()))
                        {
                            LinkButton btn = (LinkButton)e.Row.Cells[8].FindControl("BtnMap");
                            btn.Attributes["href"] = "#";
                            btn.Attributes["onclick"] = "OpenMap('" + oRow.Row["RecordID"].ToString() + "')";
                            //   btn.Attributes.Add("onclick", "OpenVideo(" + (Convert.ToDateTime(oRow.Row["CardTime"]).AddHours(-8) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds.ToString() + "," +
                            //(DateTime.Now.AddHours(-8) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds.ToString() +
                            //",'" + vms_dic["VmsHost"] + "','" + this.EquVmsList.Where(i => i.EquNo == Convert.ToString(oRow.Row["EquNo"])).FirstOrDefault().VmsName + "')");
                        }
                        #endregion
                        #region 卡片版次
                        #endregion
                        #region 卡片類別
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
                    }
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

        #region QueryButton_Click
        protected void QueryButton_Click(object sender, EventArgs e)
        {
            Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }
        #endregion

        #endregion

        #region Method

        #region CreateEquGroupDropItem
        private void CreateEquGroupDropItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;

            #region Give Empty Item
            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = this.GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString();
            Item.Value = this.GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString();
            this.DropDown_Building.Items.Add(Item);
            #endregion

            #region Process String
            sql = @" SELECT DISTINCT Building FROM B01_EquData ";
            #endregion

            oAcsDB.GetDataTable("DropListItem", sql, out dt);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new System.Web.UI.WebControls.ListItem();
                Item.Text = dr["Building"].ToString();
                Item.Value = dr["Building"].ToString();
                this.DropDown_Building.Items.Add(Item);
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
        public void Query(string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = @" SELECT DISTINCT
                     AlarmLog.RecordID, AlarmLog.LogTime, 
                     CardLogState.StateDesc AS CardLogStateName, 
                     AlarmLog.CardTime, EquData.Building,
                     AlarmLog.EquNo, AlarmLog.EquName,
                     AlarmLog.EquClass, AlarmLog.EquDir,
                     Person.PsnNo, Person.PsnName, 
                     AlarmLog.CardNo, AlarmLog.CardVer, 
                     ItemList.ItemName AS CardTypeName
                     FROM B01_AlarmLog AS AlarmLog
                     LEFT JOIN B00_CardLogState AS CardLogState ON CardLogState.Code = AlarmLog.LogStatus
                     LEFT JOIN dbo.B01_Card AS Card ON Card.CardNo = AlarmLog.CardNo
                     LEFT JOIN dbo.B01_Person AS Person ON Person.PsnID = Card.PsnID
                     LEFT JOIN B00_ItemList AS ItemList ON ItemList.ItemNo = Card.CardType
                     INNER JOIN dbo.B01_EquData AS EquData ON EquData.EquNo = AlarmLog.EquNo  
                     INNER JOIN B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID
                     INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID
                     INNER JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID ";

            #region DataAuth
            if (wheresql != "") wheresql += " AND ";
            wheresql += " (SysUser.UserID = ? ) ";
            liSqlPara.Add("S:" + Session["UserID"].ToString());
            #endregion

            if (!string.IsNullOrEmpty(this.Query_LogTimeS.DateValue.Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " ? <= AlarmLog.LogTime  ";
                liSqlPara.Add("D:" + this.Query_LogTimeS.DateValue.Trim());
            }

            if (!string.IsNullOrEmpty(this.Query_LogTimeE.DateValue.Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += "AlarmLog.LogTime <= ? ";
                liSqlPara.Add("D:" + this.Query_LogTimeE.DateValue.Trim());
            }

            if (!string.IsNullOrEmpty(this.Query_Psn.Text.Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += "(Person.PsnNo LIKE ? OR  Person.PsnName LIKE ?)";
                liSqlPara.Add("S:" + "%" + this.Query_Psn.Text.Trim() + "%");
                liSqlPara.Add("S:" + "%" + this.Query_Psn.Text.Trim() + "%");
            }

            if (string.Compare(DropDown_Building.SelectedValue, this.GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString()) != 0)
            {
                if (wheresql != "") wheresql += " AND ";
                if (DropDown_Building.SelectedValue != "")
                {
                    wheresql += " (EquData.Building = ?) ";
                    liSqlPara.Add("S:" + DropDown_Building.SelectedValue);
                }
                else
                {
                    wheresql += " (EquData.Building = '' OR EquData.Building IS NULL) ";
                }
            }

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("QueryAlarmLog", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();
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