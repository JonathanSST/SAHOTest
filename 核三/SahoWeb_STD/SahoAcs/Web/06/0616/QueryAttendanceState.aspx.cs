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
    public partial class QueryAttendanceState : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 20, _datacount = 1, _pageindex = 0, _pagecount = 0;
        Hashtable TableInfo;
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.ddlDept);
            oScriptManager.RegisterAsyncPostBackControl(this.QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("QueryAttendanceState", "QueryAttendanceState.js");//加入同一頁面所需的JavaScript檔案

            #region 註冊主頁Button動作
            QueryButton.Attributes["onClick"] = "SelectState(); return false;";
            #endregion

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

            this.MainGridView.PageSize = _pagesize;
            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                hidePsnID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
                #endregion

                //User不啟用其他查詢條件
                if (hideUserID.Value.Equals("User") && !hidePsnID.Value.Equals(""))
                {
                    lblDept.Visible = false;
                    ddlDept.Visible = false;
                    lblPsnNo.Visible = false;
                    txtPsnNo.Visible = false;
                }

                CreateDeptDropItem();
                pdSDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd");
                pdEDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd");
                Query(false);
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];


                if (sFormTarget == this.QueryButton.ClientID)
                {
                    Query(false);
                }
                else
                {
                    Query(true);
                }
                //string[] arrControl = sFormTarget.Split('$');

                //if (arrControl.Length == 5)
                //{
                //    _pageindex = Convert.ToInt32(arrControl[4].Split('_')[1]);
                //    Query(true);
                //}
                //else
                //{
                //    Query(false);
                //}
            }
        }
        #endregion

        #region VerifyRenderingInServerForm
        public override void VerifyRenderingInServerForm(Control control)
        {
            // 修正 'XX'型別 必須置於有 runat=server 的表單標記之中 Override此Methods
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
                    int[] HeaderWidth = { 100, 100, 100, 80, 100, 75, 75, 75, 75, 75, 75, 80 };
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
                    //GrRow.ID = "GV_Row" + oRow.Row["NewIDNum"].ToString();
                    GrRow.ID = "GV_Row" + e.Row.RowIndex;
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 103, 103, 104, 84, 104, 79, 79, 79, 79, 79, 79, 84 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 參數名稱
                    #endregion

                    #region 參數描述
                    string[] strCardLog = e.Row.Cells[4].Text.Remove(e.Row.Cells[4].Text.Length - 1).Split(',');

                    for (int i = 0, j = strCardLog.Length; i < j; i++)
                    {
                        e.Row.Cells[4 + i].Text = strCardLog[i].ToString();
                    }
                    #endregion

                    #region 輸入方式
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                    {
                        e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Center;
                    }
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

                    e.Row.Cells[0].Text = LimitText(e.Row.Cells[0].Text.Trim(), 14, true);
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
                        //lbtnPage.ID = "Pages_" + (i).ToString();
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
                        {
                            lbtnPage.Font.Bold = false;
                        }

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
                            Query(true);
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

                    lbtnNext.Click += delegate(object obj, EventArgs args)
                    {
                        if (gv.PageIndex < gv.PageCount)
                        {
                            gv.PageIndex = gv.PageIndex + 1;
                            Query(true);
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
                    lbtnFirst.Click += delegate(object obj, EventArgs args)
                    {
                        gv.PageIndex = 0;
                        Query(true);
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

                    lbtnLast.Click += delegate(object obj, EventArgs args)
                    {
                        gv.PageIndex = MainGridView.PageCount;
                        Query(true);
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
                    phdPageNumber.Controls.Add(new LiteralControl(string.Format("　{0} / {1}　", pageIndex + 1, pageCount)));
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
            {
                MainGridView.DataBind();
            }
        }
        #endregion

        #endregion

        #region Method

        #region CreateDeptDropItem
        private void CreateDeptDropItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;

            #region Give Empty Item
            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = "- 請選擇 -";
            Item.Value = "";
            this.ddlDept.Items.Add(Item);
            #endregion

            #region Process String
            sql = @" SELECT OrgID, ('(' + SUBSTRING(OrgNo, 2, LEN(OrgNo)) + ')' + OrgName) AS 'OrgName' FROM
                (SELECT B00_SysUserMgns.UserID, OrgStrucAllData.OrgID, OrgStrucAllData.OrgNo,
                 OrgStrucAllData.OrgName FROM B00_SysUserMgns
                INNER JOIN B01_MgnOrgStrucs ON B00_SysUserMgns.MgaID = B01_MgnOrgStrucs.MgaID
                INNER JOIN B01_Person ON B01_MgnOrgStrucs.OrgStrucID = B01_Person.OrgStrucID
                LEFT OUTER JOIN OrgStrucAllData('Department') AS OrgStrucAllData ON B01_Person.OrgStrucID = OrgStrucAllData.OrgStrucID
                ) AS Mgns WHERE Mgns.UserID = '" + hideUserID.Value + "' GROUP BY OrgID, OrgName, OrgNo ORDER BY OrgNo ";
            #endregion

            oAcsDB.GetDataTable("DropListItem", sql, out dt);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new System.Web.UI.WebControls.ListItem();
                Item.Text = dr["OrgName"].ToString();
                Item.Value = dr["OrgID"].ToString();
                this.ddlDept.Items.Add(Item);
            }
        }
        #endregion

        #region LimitText
        public string LimitText(string str, int len, bool ellipsis)
        {
            Encoding big5 = Encoding.GetEncoding("big5");
            byte[] b = big5.GetBytes(str);

            if (b.Length <= len)
            {
                return str;
            }
            else
            {
                if (ellipsis)
                {
                    len -= 3;
                }

                string res = big5.GetString(b, 0, len);

                if (!big5.GetString(b).StartsWith(res))
                {
                    res = big5.GetString(b, 0, len - 1);
                }

                return res + (ellipsis ? "..." : "");
            }
        }
        #endregion

        #region Query
        public void Query(bool boolPage)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region LastUpdateTime
            sql = @"SELECT MAX(UpdateTime) AS 'UpdateTime' FROM B01_OrgData WHERE OrgClass = 'Company'";
            oAcsDB.GetDataTable("LastUpdateTime", sql, liSqlPara, out dt);

            if (Sa.Check.IsEmptyStr(dt.Rows[0].ItemArray[0].ToString()))
            {
                lblLastTime.Text = "未同步";
            }
            else
            {
                lblLastTime.Text = dt.Rows[0].ItemArray[0].ToString();
            }
            #endregion

            #region Process String
            dt = null;

            if (boolPage)
            {
                dt = (DataTable)Session["PageData"];
            }
            else
            {
                if (hideUserID.Value.Equals("User") && !hidePsnID.Value.Equals(""))
                {
                    sql = @" SELECT DateData, PsnNo, PsnName, OrgID, OrgName, CardLog,
                    ItemName, AskTime FROM QueryAttendanceState('" + pdSDate.DateValue +
                        "', '" + pdEDate.DateValue + "', '" + hideUserID.Value +
                        "', '" + hidePsnID.Value + "')";
                }
                else
                {
                    sql = @" SELECT DateData, PsnNo, PsnName, OrgID, OrgName, CardLog,
                    ItemName, AskTime FROM QueryAttendanceState('" + pdSDate.DateValue +
                        "', '" + pdEDate.DateValue + "', '" + hideUserID.Value + "', '')";
                }

                if (!string.IsNullOrEmpty(txtPsnNo.Text.Trim()))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " (PsnNo LIKE ? OR PsnName LIKE ?) ";
                    liSqlPara.Add("S:" + "%" + txtPsnNo.Text.Trim() + "%");
                    liSqlPara.Add("S:" + "%" + txtPsnNo.Text.Trim() + "%");
                }

                if (!string.IsNullOrEmpty(ddlDept.SelectedValue))
                {
                    if (wheresql != "") wheresql += " AND ";
                    wheresql += " (OrgID = ?) ";
                    liSqlPara.Add("S:" + ddlDept.SelectedValue);
                }

                if (wheresql != "")
                {
                    //sql += " WHERE " + wheresql + " ORDER BY PsnNo, DateData";
                    sql += " WHERE " + wheresql + " ORDER BY PsnName, PsnNo, DateData";  //REX20150618：更改顯示的排序方式
                }
                else
                {
                    //sql += " ORDER BY PsnNo, DateData";
                    sql += " ORDER BY PsnName, PsnNo, DateData";  //REX20150618：更改顯示的排序方式
                }

                //_datacount = oAcsDB.DataCount(sql, liSqlPara);

                //string strRowPageSql = "SELECT * FROM (" + sql + ") AS Q WHERE Q.NewIDNum BETWEEN " +
                //    (int)(_pagesize * _pageindex + 1) + " AND " + (int)(_pagesize * (_pageindex + 1));

                oAcsDB.GetDataTable("QueryPsnEqu", sql, liSqlPara, out dt);

                #region REX20150618：處理刷卡時間重覆問題
                string sOldCardTimeList = "", sNewCardTimeList = "";
                object[] oItemArray          = null;
                string[] sCardTimeArray      = null;
                Hashtable oCardTimeHashtable = new Hashtable();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sOldCardTimeList = dt.Rows[i].ItemArray[5].ToString();

                    if (sOldCardTimeList != "")
                    {
                        sNewCardTimeList = "";
                        sCardTimeArray   = sOldCardTimeList.Split(',');

                        //比對並處理重覆的刷卡時間
                        for (int j = 0; j < (sCardTimeArray.Length - 1); j++)
                        {
                            if (!oCardTimeHashtable.ContainsKey(sCardTimeArray[j]))
                            {
                                oCardTimeHashtable.Add(sCardTimeArray[j], "");
                                sNewCardTimeList = string.Format("{0}{1},", sNewCardTimeList, sCardTimeArray[j]);
                            }
                        }

                        //更新表格顯示的刷炩時間
                        if (sNewCardTimeList != sOldCardTimeList)
                        {
                            oItemArray = dt.Rows[i].ItemArray;

                            oItemArray[5] = (object)sNewCardTimeList;

                            dt.Rows[i].ItemArray = oItemArray;
                        }

                        oItemArray = null; sCardTimeArray = null; oCardTimeHashtable.Clear();
                    }
                }

                oCardTimeHashtable = null; 
                #endregion

                Session["PageData"] = dt;
            }
            #endregion

            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI", "$.unblockUI();", true);
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

        #endregion
    }
}