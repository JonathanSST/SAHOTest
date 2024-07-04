using Sa.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web
{
    public partial class CardLogFill : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 100, _datacount = 1, _pageindex = 0, _pagecount = 0;
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
            oScriptManager.RegisterAsyncPostBackControl(QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("CardLogFill", "CardLogFill.js"); //加入同一頁面所需的JavaScript檔案
            #endregion

            #region RegisterObj

            #region 設定彈跳視窗
            Pub.SetModalPopup(ModalPopupExtender1, 1);
            #endregion

            #region 註冊主頁Button動作
            QueryButton.Attributes["onClick"] = "SelectState(); return false;";
            btnEdit.Attributes["onClick"] = "CallEdit('" + this.GetLocalResourceObject("CallEdit_Title") + "', '" +
                this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";
            #endregion

            #region 註冊pop1頁Button動作
            ImgCloseButton1.Attributes["onClick"] = "CancelTrigger1.click(); return false;";
            popB_Save.Attributes["onClick"] = "FillCardLog(); return false;";
            popB_Cancel.Attributes["onClick"] = "CancelTrigger1.click(); return false;";
            #endregion

            //設定DataGridView每頁顯示的列數
            this.MainGridView.PageSize = _pagesize;
            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                DropBind(this.popDrop_EquName, "EquID", "EquName", @"SELECT EquID, EquName FROM V_MCRInfo");
                DropBind(this.popDrop_LogStatus, "Code", "StateDesc", @"SELECT Code, StateDesc FROM B00_CardLogState");
                Query();
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == this.QueryButton.ClientID)
                {
                    Query();
                }
                else
                {
                    string[] arrControl = sFormTarget.Split('$');

                    if (arrControl.Length == 5)
                    {
                        _pageindex = Convert.ToInt32(arrControl[4].Split('_')[1]);
                    }

                    Query();
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
                    int[] HeaderWidth = { 100, 100, 100 };
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
                    GrRow.ID = "GV_Row" + oRow.Row["PsnID"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 103, 104, 104 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理
                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }
                    //e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 20, true);
                    //e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text, 32, true);
                    //e.Row.Cells[3].Text = LimitText(e.Row.Cells[3].Text, 11, true);
                    //e.Row.Cells[5].Text = LimitText(e.Row.Cells[5].Text, 20, true);
                    //e.Row.Cells[6].Text = LimitText(e.Row.Cells[6].Text, 20, true);
                    //e.Row.Cells[7].Text = LimitText(e.Row.Cells[7].Text, 32, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["PsnID"].ToString() + "', '', '');");
                    e.Row.Attributes.Add("OnDblclick", "CallEdit('" + this.GetLocalResourceObject("CallEdit_Title") + "', '" +
                        this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "')");

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
                    if (_pageindex == 0)
                    {
                        lbtnPrev.Enabled = false;
                    }
                    else
                    {
                        lbtnPrev.Enabled = true;
                        lbtnPrev.ID = "lbtnPrev_" + (_pageindex - 1);
                    }

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
                    if (_pageindex == 0 || _pagecount == 0)
                    {
                        lbtnFirst.Enabled = false;
                    }
                    else
                    {
                        lbtnFirst.Enabled = true;
                        lbtnFirst.ID = "lbtnFirst_" + 0;
                    }

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
                    phdPageNumber.Controls.Add(
                        new LiteralControl(string.Format("　{0} / {1}　", pageIndex + 1, pageCount == 0 ? 1 : pageCount)));
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

        #region VerifyRenderingInServerForm
        public override void VerifyRenderingInServerForm(Control control)
        {
            // 修正 'XX'型別 必須置於有 runat=server 的表單標記之中 Override此Methods
        }
        #endregion

        #endregion

        #region Method

        #region Query
        public void Query()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = @" SELECT ROW_NUMBER() OVER(ORDER BY B01_Person.PsnID) AS NewIDNum, B01_Person.PsnID,
                B01_Person.PsnNo, B01_Person.PsnName, B01_Card.CardNo FROM B01_Person 
                INNER JOIN B01_Card ON B01_Person.PsnID = B01_Card.PsnID ";

            #region SQL Condition
            if (!string.IsNullOrEmpty(txtCondition.Text.Trim()))
            {
                wheresql = " ( B01_Person.PsnNo LIKE ? ) OR (B01_Person.PsnName LIKE ?) OR (B01_Card.CardNo LIKE ?) ";
                liSqlPara.Add("S:%" + txtCondition.Text.Trim() + "%");
                liSqlPara.Add("S:%" + txtCondition.Text.Trim() + "%");
                liSqlPara.Add("S:%" + txtCondition.Text.Trim() + "%");
            }

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql;
            #endregion

            _datacount = oAcsDB.DataCount(sql, liSqlPara);
            CardLogTable = oAcsDB.PageData(sql, liSqlPara, _pageindex, _pagesize);
            #endregion

            GirdViewDataBind(this.MainGridView, CardLogTable);
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
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            DataTable dtData = new DataTable();

            oAcsDB.GetDataTable("DropData", strCmd, out dtData);
            dropData.DataValueField = strValue;
            dropData.DataTextField = strText;
            dropData.DataSource = dtData;
            dropData.DataBind();
        }
        #endregion

        #region 載入人員資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadData(string PsnID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            DBReader dr = null;
            String sql = "";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = @"  SELECT B01_Person.PsnID, B01_Person.PsnNo, B01_Person.PsnName, B01_Card.CardNo FROM B01_Person 
                INNER JOIN B01_Card ON B01_Person.PsnID = B01_Card.PsnID WHERE B01_Person.PsnID = ? ";
            liSqlPara.Add("S:" + PsnID.Trim());

            oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (dr.Read())
            {
                EditData = new string[dr.DataReader.FieldCount];
                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                    EditData[i] = dr.DataReader[i].ToString();
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "系統中無此資料！";
            }
            #endregion

            return EditData;
        }
        #endregion

        #region 新增補登記錄
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] FillCardLogData(string PsnID, string EquID, string LogStatus, string CardTime)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            DBReader dr = null;
            String sql = "";
            int result = 0;
            CardLogData oCardLogData = new CardLogData();
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();

            #region Process String

            #region 人員資料
            sql = @"  SELECT B01_Person.PsnID, B01_Person.PsnNo, B01_Person.PsnName, B01_OrgStruc.OrgIDList,
                B01_Card.CardNo, B01_Card.CardVer FROM B01_Person 
                INNER JOIN B01_Card ON B01_Person.PsnID = B01_Card.PsnID 
                INNER JOIN B01_OrgStruc ON B01_Person.OrgStrucID = B01_OrgStruc.OrgStrucID 
                WHERE B01_Person.PsnID = ? ";
            liSqlPara.Add("S:" + PsnID.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (dr.Read())
            {
                oCardLogData.PsnID = dr.DataReader["PsnID"].ToString();
                oCardLogData.PsnNo = dr.DataReader["PsnNo"].ToString();
                oCardLogData.PsnName = dr.DataReader["PsnName"].ToString();
                oCardLogData.OrgStruc = dr.DataReader["OrgIDList"].ToString();
                oCardLogData.CardNo = dr.DataReader["CardNo"].ToString();
                oCardLogData.CardVer = dr.DataReader["CardVer"].ToString();
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "系統中無此人員資料！";
                return EditData;
            }
            #endregion

            #region 設備資料
            dr = null;
            liSqlPara.Clear();

            sql = @" SELECT EquClass, EquNo, EquName, Dir, MstConnParam, CtrlAddr, ReaderNo, IsAndTrt FROM V_MCRInfo WHERE EquID = ? ";
            liSqlPara.Add("S:" + EquID.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (dr.Read())
            {
                oCardLogData.EquClass = dr.DataReader["EquClass"].ToString();
                oCardLogData.EquNo = dr.DataReader["EquNo"].ToString();
                oCardLogData.EquName = dr.DataReader["EquName"].ToString();
                oCardLogData.EquDir = dr.DataReader["Dir"].ToString();
                oCardLogData.MstConnParam = dr.DataReader["MstConnParam"].ToString();
                oCardLogData.CtrlAddr = dr.DataReader["CtrlAddr"].ToString();
                oCardLogData.ReaderNo = dr.DataReader["ReaderNo"].ToString();
                oCardLogData.IsAndTrt = dr.DataReader["IsAndTrt"].ToString();
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "系統中無此設備資料！";
                return EditData;
            }
            #endregion

            #region 額外資訊
            oCardLogData.CardInfo = "";
            oCardLogData.VerifyMode = "0";
            oCardLogData.LogStatus = LogStatus;
            oCardLogData.CardType = "A";
            #endregion

            try
            {
                liSqlPara.Clear();
                oAcsDB.BeginTransaction();
                sql = @" INSERT INTO B01_CardLogFill(LogTime, CardTime, CardNo, CardVer, PsnNo, PsnName, OrgStruc,
                    EquClass, EquNo, EquName, EquDir, MstConnParam, CtrlAddr, ReaderNo, IsAndTrt, CardInfo, VerifyMode,
                    LogStatus, CardType) VALUES (getdate(), ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?) ";
                liSqlPara.Add("S:" + CardTime);
                liSqlPara.Add("S:" + oCardLogData.CardNo);
                liSqlPara.Add("S:" + oCardLogData.CardVer);
                liSqlPara.Add("S:" + oCardLogData.PsnNo);
                liSqlPara.Add("S:" + oCardLogData.PsnName);
                liSqlPara.Add("S:" + oCardLogData.OrgStruc);
                liSqlPara.Add("S:" + oCardLogData.EquClass);
                liSqlPara.Add("S:" + oCardLogData.EquNo);
                liSqlPara.Add("S:" + oCardLogData.EquName);
                liSqlPara.Add("S:" + oCardLogData.EquDir);
                liSqlPara.Add("S:" + oCardLogData.MstConnParam);
                liSqlPara.Add("S:" + oCardLogData.CtrlAddr);
                liSqlPara.Add("S:" + oCardLogData.ReaderNo);
                liSqlPara.Add("S:" + oCardLogData.IsAndTrt);
                liSqlPara.Add("S:" + oCardLogData.CardInfo);
                liSqlPara.Add("S:" + oCardLogData.VerifyMode);
                liSqlPara.Add("S:" + oCardLogData.LogStatus);
                liSqlPara.Add("S:" + oCardLogData.CardType);
                result = oAcsDB.SqlCommandExecute(sql, liSqlPara);

                if (result > 0)
                {
                    oAcsDB.Commit();
                    EditData = new string[2];
                    EditData[0] = "";
                    EditData[1] = "補登刷卡資料成功！";
                }
                else
                {
                    oAcsDB.Rollback();
                    EditData = new string[2];
                    EditData[0] = "Saho_SysErrorMassage";
                    EditData[1] = "補登刷卡資料失敗！";
                }

                return EditData;
            }
            catch (Exception ex)
            {
                oAcsDB.Rollback();
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = ex.Message;
                return EditData;
            }
            #endregion

        }
        #endregion

        #endregion

        #region Object
        public class CardLogData
        {
            public String PsnID { get; set; }
            public String CardNo { get; set; }
            public String CardVer { get; set; }
            public String PsnNo { get; set; }
            public String PsnName { get; set; }
            public String OrgStruc { get; set; }
            public String EquClass { get; set; }
            public String EquNo { get; set; }
            public String EquName { get; set; }
            public String EquDir { get; set; }
            public String MstConnParam { get; set; }
            public String CtrlAddr { get; set; }
            public String ReaderNo { get; set; }
            public String IsAndTrt { get; set; }
            public String CardInfo { get; set; }
            public String VerifyMode { get; set; }
            public String LogStatus { get; set; }
            public String CardType { get; set; }
        }
        #endregion
    }
}