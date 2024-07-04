using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web._06._0623
{
    public partial class CardExtGridView : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";
            js = "<script type='text/javascript'>" + js + "</script>";
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);

            if (!IsPostBack)
            {
                DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
                List<string> liSqlPara = new List<string>();
                DataTable dt = new DataTable();

                string strCtrlArea = "";
                if (Request.Form["ddlCtrlArea"] != null)
                {
                    strCtrlArea = Request.Form["ddlCtrlArea"].ToString();
                }
                
                string strSQL = @"
                    SELECT 
                        CE.CardID, 
	                    ISNULL(CD.CardNo ,'') CardNo, 
                        ISNULL(PN.PsnName ,'') PsnName, 
                        CE.LastTime, 
                        CE.LastDoorNo, 
                        CE.CtrlAreaNo 
                    FROM B01_CardExt CE 
                    LEFT JOIN B01_Card CD ON CD.CardID = CE.CardID
                    LEFT JOIN B01_Person PN ON PN.PsnID = CD.PsnID  
                    WHERE CE.CtrlAreaNo = ? ";

                liSqlPara.Add("S:" + strCtrlArea);

                bool isSuccess = oAcsDB.GetDataTable("dtEquGroupList", strSQL, liSqlPara, out dt);

                if (dt.Rows.Count > 0)
                {
                    GridView1.DataSource = dt;
                    GridView1.DataBind();

                }
                else
                {
                    dt.Rows.Add(dt.NewRow());
                    GridView1.DataSource = dt;
                    GridView1.DataBind();

                    int columnCount = GridView1.Rows[0].Cells.Count;
                    GridView1.Rows[0].Cells.Clear();
                    GridView1.Rows[0].Cells.Add(new TableCell());
                    GridView1.Rows[0].Cells[0].ColumnSpan = columnCount;
                    GridView1.Rows[0].Cells[0].Text = GetGlobalResourceObject("Resource", "NonData").ToString();
                } 
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    int[] HeaderWidth = { 100, 100, 180, 100 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #endregion

                    #region 排序條件Header加工

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
                    int[] DataWidth = { 103, 104, 184, 105 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 讀卡時間
                    //if (!string.IsNullOrEmpty(oRow.Row["CardTime"].ToString()))
                    //{
                    //    e.Row.Cells[0].Text = DateTime.Parse(oRow.Row["CardTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                    //}
                    #endregion

                    #region 設備編號
                    //if (!string.IsNullOrEmpty(oRow.Row["CardTime"].ToString()))
                    //{
                    //    LinkButton btn = (LinkButton)e.Row.Cells[7].FindControl("BtnVideo");
                    //    btn.Attributes["href"] = "#";
                    //    btn.Attributes.Add("onclick", "OpenVideo(" + (Convert.ToDateTime(oRow.Row["CardTime"]).AddHours(-8) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds.ToString() + "," +
                    //        (DateTime.Now.AddHours(-8) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds.ToString() + ")");
                    //}
                    //else
                    //{
                    //    LinkButton btn = (LinkButton)e.Row.Cells[7].FindControl("BtnVideo");
                    //    btn.Attributes["href"] = "#";
                    //}
                    #endregion

                    #region 讀卡時間
                    //if (!string.IsNullOrEmpty(oRow.Row["LogTime"].ToString()))
                    //{
                    //    e.Row.Cells[e.Row.Cells.Count - 1].Text = DateTime.Parse(oRow.Row["LogTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                    //}
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
                    //e.Row.Attributes.Add("OnDblclick", "CallShowLogDetail()");
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
                            GridView1.DataBind();
                        }
                    };

                    lbtnNext.Click += delegate (object obj, EventArgs args)
                    {
                        if (gv.PageIndex < gv.PageCount)
                        {
                            gv.PageIndex = gv.PageIndex + 1;
                            GridView1.DataBind();
                        }
                    };
                    #endregion

                    #region 首末頁
                    lbtnFirst.Click += delegate (object obj, EventArgs args)
                    {
                        gv.PageIndex = 0;
                        GridView1.DataBind();
                    };

                    lbtnLast.Click += delegate (object obj, EventArgs args)
                    {
                        gv.PageIndex = GridView1.PageCount;
                        GridView1.DataBind();
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

        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            td_showGridView.Attributes["colspan"] = GridView1.Columns.Count.ToString();
        }

       
    }
}