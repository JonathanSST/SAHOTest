using Sa.DB;
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
    public partial class ExamInfo : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 20;
        Hashtable TableInfo;
        static string non_data = "";
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
            js += "\nSetMode('');";
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("ExamInfo", "ExamInfo.js");//加入同一頁面所需的JavaScript檔案
            string btnDel = " "+GetGlobalResourceObject("Resource", "btnDelete").ToString();
            string btnAdd = " " + GetGlobalResourceObject("Resource", "btnAdd").ToString();
            string btnEdit = " " + GetGlobalResourceObject("Resource", "btnEdit").ToString();
            #region 註冊主頁Button動作
            QueryButton.Attributes["onClick"] = "SelectState();return false;";
            AddButton.Attributes["onClick"] = "CallAdd('考試資料 新增'); return false;";
            EditButton.Attributes["onClick"] = "CallEdit('考試資料 編輯','" + this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";
            DeleteButton.Attributes["onClick"] = "CallDelete('考試資料 刪除','" + this.GetGlobalResourceObject("Resource", "NotSelectForDelete") + "'); return false;";
            btSelectData.Attributes["onClick"] = "PersonChoice('閱卷人員資料選取','" + this.GetGlobalResourceObject("Resource", "NotSelectForEdit") + "'); return false;";
            btAuthConfirm.Attributes["onClick"] = "CallConfirm('通行權限重整','請選擇考試，以重整通行權限'); return false;";
            #endregion

            #region 註冊pop1頁Button動作
            //ImgCloseButton1.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            ImgCloseButton1.Attributes["onClick"] = "CancelTrigger1.click(); return false;";
            popB_Add.Attributes["onClick"] = "AddExcute(); return false;";
            popB_Edit.Attributes["onClick"] = "EditExcute(); return false;";
            popB_Delete.Attributes["onClick"] = "DeleteExcute(); return false;";
            //popB_Cancel.Attributes["onClick"] = "SetMode(''); CancelTrigger1.click(); return false;";
            popB_Cancel.Attributes["onClick"] = "CancelTrigger1.click(); return false;";
            popB_Confirm.Attributes["onClick"] = "ConfirmExcute(); return false;";
            #endregion

            #region 註冊pop2頁Button動作
            ImgCloseButton2.Attributes["onClick"] = "CancelTrigger2.click(); return false;";
            popB_OK1.Attributes["onClick"] = "UpdateExamPsn(); return false;";
            //popB_Cancel1.Attributes["onClick"] = "SetMode('');CancelTrigger2.click(); return false;";
            popB_Cancel1.Attributes["onClick"] = "CancelTrigger2.click(); return false;";
            popB_Enter1.Attributes["onClick"] = "DataEnterRemove('Add'); return false;";
            popB_Remove1.Attributes["onClick"] = "DataEnterRemove('Del'); return false;";
            popB_Query.Attributes["onClick"] = "QueryPsnData(); return false;";
            #endregion

            Pub.SetModalPopup(ModalPopupExtender1, 1);
            Pub.SetModalPopup(ModalPopupExtender2, 2);
            this.MainGridView.PageSize = _pagesize;

            DBReader drOrg = null;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            ListItem Item = new ListItem();
            Item.Text = "請選擇單位";
            Item.Value = "";
            DropDownList dropInit = (DropDownList)this.Master.FindControl("ContentPlaceHolder1").FindControl("PanelPopup1").FindControl("popInput_OrgNo");
            //UserControl _uc1 = (UserControl)this.Master.FindControl("ContentPlaceHolder1").FindControl("PanelPopup1").FindControl("popInput_ExamBeginTime");
            ListBox popB_PsnList2 = (ListBox)this.Master.FindControl("ContentPlaceHolder1").FindControl("PanelPopup2").FindControl("popB_PsnList2");
            dropInit.Items.Add(Item);
            //3、部門別資料處理
            sql = @"SELECT OrgID, OrgNo, OrgName FROM B01_OrgData WHERE OrgClass = 'Unit' ORDER BY OrgNo";
            oAcsDB.GetDataReader(sql, out drOrg);

            //#region 動態寫入部門別資料
            while (drOrg.Read())
            {
                Item = new ListItem();
                Item.Text = drOrg.DataReader["OrgName"].ToString();
                Item.Value = drOrg.DataReader["OrgNo"].ToString();
                dropInit.Items.Add(Item);
            }

            #region 語系切換
            //DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            //oAcsDB.GetTableHash("B01_DeviceConnInfo", "zhtw", out TableInfo);
            //Label_No.Text = TableInfo["DciNo"].ToString();
            //Label_Name.Text = TableInfo["DciName"].ToString();
            //Label_Ip.Text = TableInfo["IpAddress"].ToString();
            //Label_IsAssign.Text = TableInfo["IsAssignIP"].ToString();
            //popLabel_No.Text = TableInfo["DciNo"].ToString();
            //popLabel_Name.Text = TableInfo["DciName"].ToString();
            //popLabel_PassWD.Text = TableInfo["DciPassWD"].ToString();
            //popLabel_Ip.Text = TableInfo["IpAddress"].ToString();
            //popLabel_Port.Text = TableInfo["TcpPort"].ToString();
            //popLabel_IsAssign.Text = TableInfo["IsAssignIP"].ToString();
            #endregion

            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                //hideOwnerList.Value = Sa.Web.Fun.GetSessionStr(this.Page, "OwnerList");
                #endregion

                ViewState["query_no"] = "";
                ViewState["query_name"] = "";
                ViewState["SortExpression"] = "ExamNo";
                ViewState["SortDire"] = "DESC";
                Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == this.QueryButton.ClientID)
                {
                    ViewState["query_no"] = this.Input_No.Text.Trim();
                    ViewState["query_name"] = this.Input_Name.Text.Trim();
                }

                if (!string.IsNullOrEmpty(sFormArg))
                {
                    if (sFormArg == "popPagePost") //進行因應新增或編輯後所需的換頁動作
                    {
                        int find = Query("popPagePost", ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        Sa.Web.Fun.RunJavaScript(this, "__doPostBack('ctl00$ContentPlaceHolder1$MainGridView', 'Page$" + find + "');");
                    }
                    else if (sFormArg.Substring(0, 5) == "Page$") //換頁完成後進行GridViewRow的移動
                    {
                        Query("popPagePost", ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        Sa.Web.Fun.RunJavaScript(this, "GridSelect();");
                    }
                }
                else
                {
                    Query(ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                }
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
                    int[] HeaderWidth = { 120, 320,200,180,180 };
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
                    GrRow.ID = "GV_Row" + oRow.Row["ExamNo"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 123, 324 ,203,183,183};
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion


                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }
                    e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 34, true);
                    e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text, 38, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["ExamNo"].ToString() + "', '', '')");
                    e.Row.Attributes.Add("OnDblclick", "CallEdit('" + GetGlobalResourceObject("ResourceGrp", "ttEquGroup") + " " + GetGlobalResourceObject("Resource", "btnEdit")+ "')");
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
        public void Query(string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = @"SELECT ExamData.ExamID,ExamData.ExamNo,ExamData.ExamName,B.OrgName,
                    ExamData.ExamBeginTime,ExamData.ExamEndTime FROM B01_ExamData ExamData  
                    INNER JOIN B01_OrgData B ON ExamData.OrgNo = B.OrgNo ";
            sql = @"SELECT ExamData.ExamID,ExamData.ExamNo,ExamData.ExamName,
                    Case when B.OrgName IS NULL then '' else B.OrgName  End as OrgName,
                    CONVERT(varchar(100), ExamData.ExamBeginTime, 111) + ' ' + 
                    CONVERT(varchar(100), ExamData.ExamBeginTime, 108) as ExamBeginTime,
                    CONVERT(varchar(100), ExamData.ExamEndTime, 111) + ' ' + 
                    CONVERT(varchar(100), ExamData.ExamEndTime, 108) as ExamEndTime
                    FROM B01_ExamData ExamData  
                    LEFT OUTER JOIN B01_OrgData B ON ExamData.OrgNo = B.OrgNo ";

            #region DataAuth
            //Saho、System可看全部資料
            if (!(this.hideUserID.Value == "Saho" || this.hideUserID.Value == "System"))
            {
                if (wheresql != "")
                {
                    wheresql += " AND ";
                }
                wheresql += " (ExamData.CreateUserID = ? OR ExamData.CreateUserID='') ";
                liSqlPara.Add("S:" + this.hideUserID.Value);
            }
            ////liSqlPara.Add("S:" + "%\\" + this.hideUserID.Value + "\\%");
            #endregion

            if (!string.IsNullOrEmpty(ViewState["query_no"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (ExamData.ExamNo LIKE ?) ";
                liSqlPara.Add("S:" + "%" + ViewState["query_no"].ToString().Trim() + "%");
            }

            if (!string.IsNullOrEmpty(ViewState["query_name"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (ExamData.ExamName LIKE ?) ";
                liSqlPara.Add("S:" + "%" + ViewState["query_name"].ToString().Trim() + "%");
            }

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("ExamDataTable", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();
        }
        #endregion

        #region Query(string mode)
        public int Query(string mode, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Process String

            sql = @"SELECT ExamData.ExamID,ExamData.ExamNo,ExamData.ExamName,
                    CONVERT(varchar(100),ExamData.ExamBeginTime,111) +' '+ 
                    CONVERT(varchar(100),ExamData.ExamBeginTime,108) as ExamBeginTime,
                    CONVERT(varchar(100),ExamData.ExamEndTime,111) +' '+ 
                    CONVERT(varchar(100),ExamData.ExamEndTime,108) as ExamEndTime
                    ,Case when OrgData.OrgName IS NULL then '' else OrgData.OrgName  End as OrgName 
                    FROM B01_ExamData AS ExamData
                      LEFT OUTER JOIN B01_OrgData AS OrgData ON ExamData.OrgNo = OrgData.OrgNo";

            #region DataAuth
            //Saho、System可看全部資料
            if (!(this.hideUserID.Value == "Saho" || this.hideUserID.Value == "System"))
            {
                if (wheresql != "")
                {
                    wheresql += " AND ";
                }
                wheresql += " (ExamData.CreateUserID = ? OR ExamData.CreateUserID='') ";
                liSqlPara.Add("S:" + this.hideUserID.Value);
            }   
            #endregion

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("ExamDataTable", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();

            int find = 0;

            #region 取得ExamDataTable後的動作
            DataRow dr = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dr = dt.Rows[i];

                if (SelectValue.Value == dr["ExamNo"].ToString())
                {
                    find = i;
                    break;
                }
            }
            #endregion

            return (find / _pagesize) + 1;
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
            sql = @" SELECT ExamNo, ExamName, OrgNo,
      CONVERT(varchar(100),ExamBeginTime,111) +' '+ CONVERT(varchar(100),ExamBeginTime,108)as ExamBeginTime,
      CONVERT(varchar(100),ExamEndTime,111) +' '+ CONVERT(varchar(100),ExamEndTime,108)asExamEndTime
      FROM B01_ExamData WHERE ExamNo = ? ";
            liSqlPara.Add("S:" + SelectValue.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                TableData = new string[dr.DataReader.FieldCount];
                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                    TableData[i] = dr.DataReader[i].ToString();
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

        #region 載入人員資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object QueryPsnData(String sQueryStr, String UserID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DBReader dr = null;
            List<String> CheckData = new List<String>();
            List<string> liSqlPara = new List<string>();
            String[] EditData = null;

            #region Process String
            sql = @"SELECT B01_Person.PsnID, B01_Person.PsnNo, B01_Person.PsnName,B01_Person.PsnType,
                    B01_Person.OrgStrucID,B00_ItemList.ItemName ,B01_Person.PsnAuthAllow,B01_Card.CardNo
                    FROM B01_Person
                    INNER JOIN B00_ItemList ON B00_ItemList.ItemNo = B01_Person.PsnType
                    LEFT OUTER JOIN B01_Card ON B01_Person.PsnID = B01_Card.PsnID
                    WHERE B00_ItemList.ItemClass = 'PsnType' ";

            if (!string.IsNullOrEmpty(sQueryStr))
            {
                wheresql += "  AND (PsnNo LIKE ? OR PsnName LIKE ? OR ItemName LIKE ? OR OrgStrucID IN ";
                wheresql += "(SELECT B.OrgStrucID FROM B01_OrgData A,B01_OrgStruc B WHERE(A.OrgName LIKE ? ";
                wheresql += "AND B.OrgStrucNo LIKE '%' + A.OrgNo + '%') and A.OrgClass='Unit'))";
                liSqlPara.Add("S:" + '%' + sQueryStr + '%');
                liSqlPara.Add("S:" + '%' + sQueryStr + '%');
                liSqlPara.Add("S:" + '%' + sQueryStr + '%');
                liSqlPara.Add("S:" + '%' + sQueryStr + '%'); //以單位名稱查詢
                sql += wheresql;
            }
            sql += " ORDER BY B01_Person.PsnType,B01_Person.PsnID";
            #endregion

           dr = null;
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.HasRows)
            {
                EditData = new string[5];
                while (dr.Read())
                {
                    EditData[0] += dr.DataReader["PsnNo"].ToString() + "|";
                    EditData[1] += "[類別:" + dr.DataReader["ItemName"].ToString() + "]" +
                        dr.DataReader["PsnNo"].ToString() + "|";
                    EditData[2] += dr.DataReader["PsnName"].ToString() + "|";
                    EditData[3] += dr.DataReader["PsnType"].ToString() + "|";
                    EditData[4] += dr.DataReader["CardNo"].ToString() + "|";
                }
                EditData[0] = EditData[0].Substring(0, EditData[0].Length - 1);
                EditData[1] = EditData[1].Substring(0, EditData[1].Length - 1);
                EditData[2] = EditData[2].Substring(0, EditData[2].Length - 1);
                EditData[3] = EditData[3].Substring(0, EditData[3].Length - 1);
                EditData[4] = EditData[4].Substring(0, EditData[4].Length - 1);
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = non_data + "!!";//"系統查無您所需的資料！";                
            }

            return EditData;
        }
        #endregion

        #region 載入已加入閱卷人員
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object QueryExamPsnData(String sExamNo, String UserID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DBReader dr = null;
            List<String> CheckData = new List<String>();
            List<string> liSqlPara = new List<string>();
            String[] EditData = null;           

            #region Process String
           sql = @"SELECT B01_Person.PsnID, B01_Person.PsnNo, B01_Person.PsnName,B01_Person.PsnType,
                    B01_Person.OrgStrucID,B00_ItemList.ItemName , B01_Person.PsnAuthAllow, B01_Card.CardNo,
					Case when B01_ExamPerson.ExamNo IS NULL then '' else B01_ExamPerson.ExamNo End as ExamNo
                    FROM B01_Person 
                    INNER JOIN B00_ItemList ON B00_ItemList.ItemNo=B01_Person.PsnType 
                    LEFT OUTER JOIN B01_Card ON B01_Person.PsnID = B01_Card.PsnID 
                    LEFT OUTER JOIN B01_ExamPerson ON B01_ExamPerson.PersonNo=B01_Person.PsnNo AND B01_ExamPerson.ExamNo=?
                    where B00_ItemList.ItemClass='PsnType' ORDER BY B01_Person.PsnType,B01_Person.PsnID";
            #endregion
            liSqlPara.Clear();
            liSqlPara.Add("S:" + sExamNo);
            dr = null;
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.HasRows)
            {
                EditData = new string[6];
                while (dr.Read())
                {
                    //popB_PsnList2.Items.Add("[類別: " + dr.DataReader["ItemName"].ToString() + "]"+
                       // dr.DataReader["PsnNo"].ToString()+ dr.DataReader["PsnName"].ToString());
                    EditData[0] += dr.DataReader["PsnNo"].ToString() + "|";
                    EditData[1] += "[類別:" + dr.DataReader["ItemName"].ToString() + "]"+
                        dr.DataReader["PsnNo"].ToString() + "|";
                    EditData[2] += dr.DataReader["PsnName"].ToString() + "|";
                    EditData[3] += dr.DataReader["PsnType"].ToString() + "|";
                    EditData[4] += dr.DataReader["ExamNo"].ToString() + "|";
                    EditData[5] += dr.DataReader["CardNo"].ToString() + "|";
                }
                EditData[0] = EditData[0].Substring(0, EditData[0].Length - 1);
                EditData[1] = EditData[1].Substring(0, EditData[1].Length - 1);
                EditData[2] = EditData[2].Substring(0, EditData[2].Length - 1);
                EditData[3] = EditData[3].Substring(0, EditData[3].Length - 1);
                EditData[4] = EditData[4].Substring(0, EditData[4].Length - 1);
                EditData[5] = EditData[5].Substring(0, EditData[5].Length - 1);
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = non_data + "!!";//"系統查無您所需的資料！";                
            }

            return EditData;
        }
        #endregion


        #region Insert
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Insert(string UserID, string popInput_No, string popInput_Name, string popInput_OrgNo, string popInput_ExamBeginTime, string popInput_ExamEndTime)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            string insOwnerID = "";

            insOwnerID = UserID;

            //if (string.IsNullOrEmpty(OwnerList))
            //    insOwnerList = "\\" + insOwnerID + "\\";
            //else
            //    insOwnerList = OwnerList + insOwnerID + "\\";

            string[] NoArray = new string[2];
            NoArray[0] = popInput_No;

            objRet = Check_Input_DB(NoArray, popInput_Name, popInput_OrgNo, popInput_ExamBeginTime, popInput_ExamEndTime, "Insert");

            if (objRet.result)
            {
                #region Process String - Add ExamData
                sql = @" INSERT INTO B01_ExamData (ExamNo, ExamName, OrgNo, ExamBeginTime, ExamEndTime, 
                            CreateUserID, CreateTime, UpdateUserID, UpdateTime)
                         VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?);SELECT SCOPE_IDENTITY();";
                liSqlPara.Add("S:" + popInput_No.Trim());
                liSqlPara.Add("S:" + popInput_Name.Trim());
                liSqlPara.Add("S:" + popInput_OrgNo.Trim());
                liSqlPara.Add("D:" + popInput_ExamBeginTime);
                liSqlPara.Add("D:" + popInput_ExamEndTime);
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                #endregion
                //oAcsDB.SqlCommandExecute(sql, liSqlPara);

                try
                {
                    oAcsDB.BeginTransaction();
                    string sExamID = oAcsDB.GetStrScalar(sql, liSqlPara);

                    if (!sExamID.Equals(""))
                    {
                        if (oAcsDB.Commit())
                        {
                             liSqlPara.Clear();
                        }
                    }
                    else
                    {
                        oAcsDB.Rollback();
                        objRet.result = false;
                        objRet.message = "新增考試資料失敗！";
                    }
                }
                catch(Exception ex)
                {
                    oAcsDB.Rollback();
                    objRet.result = false;
                    objRet.message = ex.Message;
                }
            }

            objRet.act = "Add";
            return objRet;
        }
        #endregion

        #region Update
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Update(string UserID, string SelectValue, string popInput_No, string popInput_Name, string popInput_OrgNo, string popInput_ExamBeginTime, string popInput_ExamEndTime)
          {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();

            string[] NoArray = new string[2];
            NoArray[0] = popInput_No;
            NoArray[1] = SelectValue;

            objRet = Check_Input_DB(NoArray, popInput_Name, popInput_OrgNo, popInput_ExamBeginTime, popInput_ExamEndTime, "Update");
            if (objRet.result)
            {
                #region Process String - Updata ExamData
                //考試轉入時不掛CreateUserID，轉入後User選單位(編輯)時才寫入
                sql = @" UPDATE B01_ExamData SET
                         ExamNo = ?, ExamName = ?, OrgNo = ?,ExamBeginTime = ?, ExamEndTime = ?, 
                         CreateUserID = ?, UpdateUserID = ?, UpdateTime = ?
                         WHERE ExamNo = ? ";

                liSqlPara.Add("S:" + popInput_No.Trim());
                liSqlPara.Add("S:" + popInput_Name.Trim());
                liSqlPara.Add("S:" + popInput_OrgNo.Trim());
                liSqlPara.Add("D:" + popInput_ExamBeginTime);
                liSqlPara.Add("D:" + popInput_ExamEndTime);
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("S:" + UserID.ToString());
                liSqlPara.Add("D:" + Time);
                liSqlPara.Add("S:" + SelectValue.Trim());

                #endregion
                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }

            objRet.act = "Edit";
            return objRet;
        }
        #endregion

        #region Delete
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Delete(string SelectValue, string userid, string orgno)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "", gid = "";
            int iRet = 0;
            int IsOk = 0;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader drAdj;
            oAcsDB.BeginTransaction();

            #region 先取出目前CardEquAdj的卡片，刪除卡片Adj資料
            sql = @"SELECT B01_CardEquAdj.CardID,CardNo
                    FROM B01_CardEquAdj 
                    INNER JOIN B01_Card ON B01_CardEquAdj.CardID=B01_Card.CardID
                    WHERE EquID IN (SELECT EquID FROM B01_EquGroupData 
                    WHERE EquGrpID=(SELECT EquGrpID FROM B01_EquGroup WHERE EquGrpNo=?))
                    GROUP BY B01_CardEquAdj.CardID,CardNo";
            //原先採用考試代碼連結EquGrpNo，後來改採用OrgNo對照
            liSqlPara.Add("S:" + SelectValue.Trim());
            //liSqlPara.Add("S:" + orgno.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out drAdj);
            if (drAdj.HasRows)
            {
                //先清除該卡片原有CardEquAdj,並做CardAuthUpdate
                while (drAdj.Read())
                {
                    sql = @"DELETE FROM B01_CardEquAdj WHERE CardID=? AND  EquID IN (SELECT EquID FROM B01_EquGroupData 
                            WHERE EquGrpID=(SELECT EquGrpID FROM B01_EquGroup WHERE EquGrpNo=?))";
                    liSqlPara.Clear();
                    liSqlPara.Add("S:" + drAdj.DataReader["CardID"].ToString());
                    liSqlPara.Add("S:" + SelectValue.Trim());
                    //liSqlPara.Add("S:" + orgno.Trim());
                    IsOk = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                    liSqlPara.Clear();
                    sql = "CardAuth_Update";
                    liSqlPara.Add("S:" + drAdj.DataReader["CardNo"].ToString());
                    liSqlPara.Add("S:" + userid);
                    liSqlPara.Add("S:ExamInfo");
                    liSqlPara.Add("S:127.0.0.1");
                    liSqlPara.Add("S:");
                    IsOk = oAcsDB.SqlProcedureExecute(sql, liSqlPara);
                }
            }
            #endregion

            if (objRet.result)
            {
                if (iRet > -1)
                {
                    #region Process String - Delete ExamData
                    liSqlPara.Clear();
                    sql = " DELETE FROM B01_ExamData WHERE ExamNo = ? ";
                    liSqlPara.Add("S:" + SelectValue.Trim());
                    #endregion
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                if (iRet > -1)
                {
                    #region Process String - Delete ExamPerson
                    liSqlPara.Clear();
                    sql = " DELETE FROM B01_ExamPerson WHERE ExamNo = ? ";
                    liSqlPara.Add("S:" + SelectValue.Trim());
                    #endregion
                    iRet = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                if (iRet > -1)
                    oAcsDB.Commit();
                else
                    oAcsDB.Rollback();
            }
            objRet.act = "Delete";
            return objRet;
        }
        #endregion

        #region 權限設定
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object AuthConfirm(string SelectValue,string userid,string orgno)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "", gid = "";
            int IsOk = 0;
            int iRet = 0;
            Sa.DB.DBReader drAdj;
            Sa.DB.DBReader drEqu;
            List<string> liSqlPara = new List<string>();

            oAcsDB.BeginTransaction();
            //先取出目前CardEquAdj的卡片，刪除卡片Adj資料
             sql = @"SELECT B01_CardEquAdj.CardID,CardNo
                    FROM B01_CardEquAdj 
                    INNER JOIN B01_Card ON B01_CardEquAdj.CardID=B01_Card.CardID
                    WHERE EquID IN (SELECT EquID FROM B01_EquGroupData 
                    WHERE EquGrpID=(SELECT EquGrpID FROM B01_EquGroup WHERE EquGrpNo=?))
                    GROUP BY B01_CardEquAdj.CardID,CardNo";
            //原先採用考試代碼連結EquGrpNo，後來改採用OrgNo對照(又改回考試代碼)
            liSqlPara.Add("S:" + SelectValue.Trim());
            //liSqlPara.Add("S:" + orgno.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out drAdj);
            if (drAdj.HasRows)
            {
                //先清除該卡片原有CardEquAdj,並做CardAuthUpdate
                while (drAdj.Read())
                {
                    sql = @"DELETE FROM B01_CardEquAdj WHERE CardID=? AND  EquID IN (SELECT EquID FROM B01_EquGroupData 
                            WHERE EquGrpID=(SELECT EquGrpID FROM B01_EquGroup WHERE EquGrpNo=?))";
                    liSqlPara.Clear();
                    liSqlPara.Add("S:" + drAdj.DataReader["CardID"].ToString());
                    liSqlPara.Add("S:" + SelectValue.Trim());
                    //liSqlPara.Add("S:" + orgno.Trim());
                    IsOk = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                    liSqlPara.Clear();
                    sql = "CardAuth_Update";
                    liSqlPara.Add("S:" + drAdj.DataReader["CardNo"].ToString());
                    liSqlPara.Add("S:" + userid);
                    liSqlPara.Add("S:ExamInfo");
                    liSqlPara.Add("S:127.0.0.1");
                    liSqlPara.Add("S:");
                    IsOk = oAcsDB.SqlProcedureExecute(sql, liSqlPara);
                }
            }
            #region 取得設備群組與閱卷人員
            //取出閱卷人員卡片資料
            //sql = @"SELECT C.CardID,C.CardNo,D.EquGrpID,E.ExamBeginTime,E.ExamEndTime
            //      FROM B01_ExamPerson A
            //      INNER JOIN B01_PERSON B ON A.PersonNo=B.PsnNo
            //      INNER JOIN B01_Card C ON B.PsnID=C.PsnID
            //      INNER JOIN B01_EquGroup D ON A.ExamNo=D.EquGrpNo
            //      INNER JOIN B01_ExamData E ON A.ExamNo=E.ExamNo
            //      WHERE A.ExamNo=?";
            sql = @"SELECT C.CardID,C.CardNo,E.ExamBeginTime,E.ExamEndTime
                  FROM B01_ExamPerson A
                  INNER JOIN B01_PERSON B ON A.PersonNo=B.PsnNo
                  INNER JOIN B01_Card C ON B.PsnID=C.PsnID
                  INNER JOIN B01_ExamData E ON A.ExamNo=E.ExamNo
                  WHERE A.ExamNo=?";
            liSqlPara.Clear();
            liSqlPara.Add("S:" + SelectValue.Trim());
            Sa.DB.DBReader dr1;
            oAcsDB.GetDataReader(sql, liSqlPara, out dr1);
            #endregion
            string sCardID;
            string sExamBeginTime;
            string sExamEndTime;
            if (dr1.HasRows)
            {
                while (dr1.Read())
                {
                    sCardID = dr1.DataReader["CardID"].ToString();
                    sExamBeginTime = dr1.DataReader["ExamBeginTime"].ToString();
                    sExamEndTime = dr1.DataReader["ExamEndTime"].ToString();
                    //取出該考試設備群組的設備，每張卡都綁入CardEquAdj
                    sql = @"SELECT EquID FROM B01_EquGroupData
                        WHERE EquGrpID = (SELECT EquGrpID FROM B01_EquGroup WHERE EquGrpNo = ?)";
                    liSqlPara.Clear();
                    liSqlPara.Add("S:" + SelectValue.Trim());
                    //liSqlPara.Add("S:" + orgno.Trim());
                    oAcsDB.GetDataReader(sql, liSqlPara, out drEqu);
                    if (drEqu.HasRows)
                    {
                        while (drEqu.Read())
                        {
                            liSqlPara.Clear();
                            sql = @"INSERT B01_CardEquAdj( CardID ,EquID ,OpMode ,CardExtData ,BeginTime ,EndTime ,CreateUserID ,CreateTime) 
                                    VALUES(?,?,'*','',?,?,?, GETDATE()) ";
                            liSqlPara.Clear();
                            liSqlPara.Add("S:" + sCardID);
                            liSqlPara.Add("S:" + drEqu.DataReader["EquID"].ToString());
                            liSqlPara.Add("D:" + sExamBeginTime);
                            liSqlPara.Add("D:" + sExamEndTime);
                            liSqlPara.Add("S:" + userid);
                            IsOk = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                        }
                    }
                    liSqlPara.Clear();
                    sql = "CardAuth_Update";
                    liSqlPara.Add("S:" + dr1.DataReader["CardNo"].ToString());
                    liSqlPara.Add("S:" + userid);
                    liSqlPara.Add("S:ExamInfo");
                    liSqlPara.Add("S:127.0.0.1");
                    liSqlPara.Add("S:");
                    oAcsDB.SqlProcedureExecute(sql, liSqlPara);
                }
            }

            if (objRet.result)
            {
                if (iRet > -1)
                {
                    
                }

                if (iRet > -1)
                    oAcsDB.Commit();
                else
                    oAcsDB.Rollback();
            }

            objRet.act = "Confirm";
            return objRet;
        }
        #endregion

        #region InsertExamPsn
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object InsertExamPsn(string UserID, string examno, string[] popChoiced)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            string sPerson = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            string insOwnerID = "";

            insOwnerID = UserID;
            string[] NoArray = new string[2];
 
            #region Process String - Add ExamData
            liSqlPara.Clear();
            sql = @" DELETE FROM B01_ExamPerson WHERE  ExamNo=?";
            liSqlPara.Add("S:" + examno);
            oAcsDB.SqlCommandExecute(sql, liSqlPara);
            for (int i = 0; i < popChoiced.Length; i++)
            {
                sPerson = popChoiced[i].ToString();
                liSqlPara.Clear();
                sql = @" INSERT INTO B01_ExamPerson (ExamNo,PersonNo,CreateUserID,CreateTime)
                          VALUES ( ?, ?, ?, GETDATE());SELECT SCOPE_IDENTITY();";
                liSqlPara.Add("S:" + examno);
                liSqlPara.Add("S:" + popChoiced[i].ToString());
                liSqlPara.Add("S:" + UserID);

                //oAcsDB.SqlCommandExecute(sql, liSqlPara);

                oAcsDB.SqlCommandExecute(sql, liSqlPara);
            }
            #endregion
            objRet.act = "Add";
            return objRet;
        }
        #endregion

        #region Check_Input_DB
        protected static Pub.MessageObject Check_Input_DB(string[] NoArray, string Name, string OrgNo, string BeginTime, string EndTime, string mode)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Input
            if (string.IsNullOrEmpty(NoArray[0].Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "考試代碼 必須輸入";
            }
            //else if (NoArray[0].Trim().Count() != 6)
            //{
            //    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
            //    objRet.result = false;
            //    objRet.message += "考試代碼 長度須為 6 碼";
            //}

            if (string.IsNullOrEmpty(Name.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "考試名稱 必須輸入";
            }
            else if (Name.Trim().Count() > 150)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "考試名稱 字數超過上限";
            }

            if (string.IsNullOrEmpty(OrgNo.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "閱卷負責單位 必須輸入";
            }

            if (string.IsNullOrEmpty(BeginTime.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "閱卷起始時間 必須輸入";
            }

            if (string.IsNullOrEmpty(EndTime.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "閱卷結束時間 必須輸入";
            }


            #endregion

            #region DB
            switch (mode)
            {
                case "Insert":
                    sql = @" SELECT * FROM B01_ExamData WHERE ExamNo = ? ";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    break;
                case "Update":
                    sql = @" SELECT * FROM B01_ExamData WHERE ExamNo = ? AND ExamNo != ? ";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    liSqlPara.Add("S:" + NoArray[1].Trim());
                    break;
            }


            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "此代碼已存在於系統中";
            }
            #endregion

            return objRet;
        }
        #endregion

        #region LoadMgaData
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadMgaData(string SelectValue)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            string sql = "", _equGid = "";
            List<string> liSqlPara = new List<string>();
            string[] EditData = new string[2];
            EditData[1] = "";

            #region 取得考試代號
            sql = " SELECT ExamID, ExamNo FROM B01_ExamData WHERE ExamNo = ? ";
            liSqlPara.Add("S:" + SelectValue.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    EditData[0] = dr.DataReader["ExamNo"].ToString();
                    _equGid = dr.DataReader["ExamID"].ToString();
                }
            }
            else
            {
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "系統中無此資料！";
            }
            #endregion

            return EditData;
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

        #region EndTimeChk
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object EndTimeChk(string userid,string examno,string optype)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            objRet.result = true;
            objRet.act = optype;
            string sql = "";
            DateTime Time = DateTime.Now;
            string sExamEndTime="";
            List<string> liSqlPara = new List<string>();
            DBReader dr = null;
            liSqlPara.Clear();
            //取得閱卷結束時間
            if (userid == "Saho" || userid == "System") return objRet; //系統管理員bypass

            sql = " SELECT ExamEndTime FROM B01_ExamData WHERE ExamNo = ? ";
            liSqlPara.Add("S:" + examno.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    sExamEndTime = dr.DataReader["ExamEndTime"].ToString();
                }
            }
            if (Time > DateTime.Parse(sExamEndTime))
            {
                objRet.result = false;
            }
            //objRet.result = false;
            return objRet;// true;
        }
        #endregion

        #endregion
        //#endregion
    }
}