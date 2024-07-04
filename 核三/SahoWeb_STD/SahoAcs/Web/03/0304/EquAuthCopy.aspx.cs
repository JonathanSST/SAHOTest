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
    public partial class EquAuthCopy : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.QueryButton);
            oScriptManager.RegisterAsyncPostBackControl(this.AuthCopyButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("EquAuthCopy", "EquAuthCopy.js");//加入同一頁面所需的JavaScript檔案

            #region 註冊主頁Button動作
            AuthCopyButton.Attributes["onClick"] = "Block();";
            QueryButton.Attributes["onClick"] = "Block(); SelectState();  return false;";
            //Input_AuthAct1.Attributes["onClick"] = "AuthActSelected('" + Input_AuthAct1.ClientID.ToString() + "')";
            //Input_AuthAct2.Attributes["onClick"] = "AuthActSelected('" + Input_AuthAct2.ClientID.ToString() + "')";
            #endregion

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
                #endregion

                ViewState["query_type"] = "";
                ViewState["query_no"] = "";
                ViewState["query_name"] = "";
                CreateDropItem();
                LoadEquList();
                //LoadSourceEquList();
                //LoadTargetEquList();
                ViewState["PersonList"] = null;
                //this.lblTarget.Text = string.Format(GetLocalResourceObject("ttEquTarget").ToString(), 0);
                //this.lblSource.Text = string.Format(GetLocalResourceObject("ttEquSource").ToString(), 0);
                this.lblPersonList.Text = string.Format(GetLocalResourceObject("lblEquAuthCopyList").ToString(), 0);
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == this.QueryButton.ClientID)
                {
                    ViewState["query_type"] = this.Input_Type.SelectedValue.Trim();
                    ViewState["query_no"] = this.Input_No.Text.Trim();
                    ViewState["query_name"] = this.Input_Name.Text.Trim();
                    LoadEquList();
                    //LoadSourceEquList();
                    //LoadTargetEquList();
                }

                if (sFormTarget == this.PersonListUpdatePanel.ClientID)
                {
                    CreatePersonList(sFormArg);
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

        #region SourceGridView_Data_RowDataBound
        public void SourceGridView_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    int[] HeaderWidth = { 100, 100 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #endregion

                    #region 寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    li_Sourceheader.Text = Header_sw.ToString();
                    #endregion
                    break;
                #endregion

                #region DataRow
                case DataControlRowType.DataRow:
                    DataRowView oRow = (DataRowView)e.Row.DataItem;

                    #region 指定Row的ID
                    GridViewRow GrRow = e.Row;
                    GrRow.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    GrRow.ID = "GV_Row" + oRow.Row["EquNo"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 105, 106 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    e.Row.Height = 23;
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 建築物名稱
                    #endregion
                    #region 設備編號
                    #endregion
                    #region 設備名稱

                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }

                    e.Row.Cells[0].Text = LimitText(e.Row.Cells[0].Text, 14, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["EquNo"].ToString() + "', '', ''); ShowPersonList(); Block();");
                    break;
                #endregion
            }
        }
        #endregion

        #region SourceGridView_Data_DataBound
        protected void SourceGridView_Data_DataBound(object sender, EventArgs e)
        {
            td_showSourceGridView.Attributes["colspan"] = SourceGridView.Columns.Count.ToString();
        }
        #endregion

        #region TargetGridView_Data_RowDataBound
        public void TargetGridView_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    int[] HeaderWidth = { 40, 100, 100 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #endregion

                    #region 寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    li_Targetheader.Text = Header_sw.ToString();
                    #endregion
                    break;
                #endregion

                #region DataRow
                case DataControlRowType.DataRow:
                    DataRowView oRow = (DataRowView)e.Row.DataItem;

                    #region 指定Row的ID
                    GridViewRow GrRow = e.Row;
                    GrRow.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    GrRow.ID = "GV_Row" + oRow.Row["EquNo"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 45, 106, 106 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 控制
                    CheckBox ChkBox = (CheckBox)e.Row.FindControl("SelectControl");
                    ChkBox.ID = "SelectControl" + e.Row.RowIndex;                    
                    e.Row.Attributes.Add("OnClick", " CheckBoxSelected('" + ChkBox.ClientID.ToString() + "');");
                    ChkBox.Attributes.Add("OnClick", " CheckBoxSelected('" + ChkBox.ClientID.ToString() + "');");
                    #endregion
                    #region 建築物名稱
                    #endregion
                    #region 設備編號
                    #endregion
                    #region 設備名稱

                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }

                    e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 14, true);
                    #endregion
                    break;
                #endregion
            }
        }
        #endregion

        #region TargetGridView_Data_DataBound
        protected void TargetGridView_Data_DataBound(object sender, EventArgs e)
        {
            td_showTargetGridView.Attributes["colspan"] = TargetGridView.Columns.Count.ToString();
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

        #region CreateDropItem
        private void CreateDropItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;

            #region Give Empty Item
            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = "- " + GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString() + " -";
            Item.Value = "";
            this.Input_Type.Items.Add(Item);
            #endregion

            #region Process String
            sql = @" SELECT DISTINCT ItemInfo1 FROM B00_ItemList
                     WHERE ItemClass = 'EquModel' ";
            #endregion

            oAcsDB.GetDataTable("DropListItem", sql, out dt);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new System.Web.UI.WebControls.ListItem();
                switch (dr["ItemInfo1"].ToString())
                {
                    case "Door Access":
                        Item.Text = GetGlobalResourceObject("ResourceEquData","EquTypeDA").ToString();// "門禁設備";
                        break;
                    case "Elevator":
                        Item.Text = GetGlobalResourceObject("ResourceEquData","EquTypeElev").ToString();//"電梯設備";
                        break;
                    case "TRT":
                        Item.Text =GetGlobalResourceObject("ResourceEquData","EquTypeTRT").ToString(); //"考勤設備";
                        break;
                    case "Meal":
                        Item.Text =GetGlobalResourceObject("ResourceEquData","EquTypeMeal").ToString(); // "餐勤設備";
                        break;
                }
                Item.Value = dr["ItemInfo1"].ToString();
                this.Input_Type.Items.Add(Item);
            }
        }
        #endregion

        #region CreatePersonList
        private void CreatePersonList(string SelectValue)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            TableRow tr;
            TableCell td;
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt;

            #region Process String
            sql = @" SELECT DISTINCT Person.PsnNo, Person.PsnName, Card.CardID, Card.CardNo
                     FROM dbo.B01_CardAuth AS CardAuth
                     INNER JOIN dbo.B01_Card AS Card ON Card.CardNo = CardAuth.CardNo
                     INNER JOIN dbo.B01_Person AS Person ON Person.PsnID = Card.PsnID
                     INNER JOIN B01_OrgStruc AS OrgStruc ON OrgStruc.OrgStrucID = Person.OrgStrucID
                     INNER JOIN B01_MgnOrgStrucs AS MgnOrgStrucs ON MgnOrgStrucs.OrgStrucID = OrgStruc.OrgStrucID
                     INNER JOIN dbo.B01_EquData AS EquData ON EquData.EquID = CardAuth.EquID
                     INNER JOIN dbo.B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID
                     INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID
                     INNER JOIN dbo.B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID AND SysUserMgns.MgaID = MgnOrgStrucs.MgaID 
                     WHERE CardAuth.OpMode != 'Del' AND EquData.EquNo = ? AND SysUserMgns.UserID = ? ";
            #endregion

            liSqlPara.Add("S:" + SelectValue.Trim());
            liSqlPara.Add("S:" + hideUserID.Value);

            oAcsDB.GetDataTable("PersonListTable", sql, liSqlPara, out dt);

            ViewState["PersonList"] = dt;
            string hidden_card = @"<input type='hidden' name='CardNoSrc' value='{0}' />";
            if (dt.Rows.Count > 0)
            {
                tr = new TableRow();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i % 3 == 0)
                        tr = new TableRow();
                    td = new TableCell();
                    td.Width = 295;
                    td.Text = "(" + dt.Rows[i]["PsnNo"] + ") " + dt.Rows[i]["PsnName"] + " - [" + dt.Rows[i]["CardNo"] + "]" + string.Format(hidden_card, dt.Rows[i]["CardNo"].ToString());
                    td.Style.Add("white-space", "nowrap");
                    td.Style.Add("Padding", "3px");
                    tr.Controls.Add(td);
                    if (i == dt.Rows.Count - 1 && dt.Rows.Count % 3 != 0)
                    {
                        for (int k = 0; k < 3 - (dt.Rows.Count % 3); k++)
                        {
                            td = new TableCell();
                            td.Style.Add("white-space", "nowrap");
                            td.Width = 295;
                            td.Style.Add("Padding", "3px");
                            tr.Controls.Add(td);
                        }
                    }

                    this.PersonList.Controls.Add(tr);
                }

            }
            else
            {
                tr = new TableRow();
                td = new TableCell();
                td.Text = GetLocalResourceObject("MsgEquAuth").ToString();                    
                td.Width = 750;
                tr.Controls.Add(td);
                this.PersonList.Controls.Add(tr);
            }
            PersonList.Style.Add("word-break", "break-all");
            PersonList.Attributes.Add("border", "1");
            PersonList.Style.Add("border-color", "#999999");
            PersonList.Style.Add("border-style", "solid");
            PersonList.Style.Add("border-collapse", "collapse");
            this.lblPersonList.Text = string.Format(GetLocalResourceObject("lblEquAuthCopyList").ToString(), dt.Rows.Count);
            PersonListUpdatePanel.Update();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI", "$.unblockUI()", true);
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI", "ShowLabel('權限複製清單(共','" +
            //   dt.Rows.Count + "','PersonList');$.unblockUI()", true);
        }
        #endregion

        #region LoadEquList
        public void LoadEquList()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "", strSourceCount = "", strTargetCount = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Source EquList
            sql = @" SELECT DISTINCT EquData.* FROM B01_EquData AS EquData
                     INNER JOIN B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID
                     INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID
                     INNER JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID ";

            #region DataAuth
            if (wheresql != "") wheresql += " AND ";
            wheresql += " (SysUser.UserID = ? OR EquData.CreateUserID = ? OR EquData.CreateUserID IN (SELECT UserID FROM B00_SysUser WHERE OwnerList LIKE ?)) ";
            liSqlPara.Add("S:" + this.hideUserID.Value);
            liSqlPara.Add("S:" + this.hideUserID.Value);
            liSqlPara.Add("S:" + '%' + this.hideUserID.Value + '%');
            #endregion

            if (!string.IsNullOrEmpty(ViewState["query_type"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (EquData.EquClass = ?) ";
                liSqlPara.Add("S:" + ViewState["query_type"].ToString().Trim());
            }
            else
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " 1 = 0 ";
            }

            if (!string.IsNullOrEmpty(ViewState["query_no"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (EquData.EquNo LIKE ?) ";
                liSqlPara.Add("S:" + "%" + ViewState["query_no"].ToString().Trim() + "%");
            }

            if (!string.IsNullOrEmpty(ViewState["query_name"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (EquData.EquName LIKE ?) ";
                liSqlPara.Add("S:" + "%" + ViewState["query_name"].ToString().Trim() + "%");
            }

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY EquData.EquNo ";

            oAcsDB.GetDataTable("EquListTable", sql, liSqlPara, out dt);
            strSourceCount = dt.Rows.Count.ToString();
            GirdViewDataBind(this.SourceGridView, dt);
            this.lblSource.Text = string.Format(GetLocalResourceObject("ttEquSource").ToString(), strSourceCount);
            SourceUpdatePanel.Update();
            #endregion

            dt = null;
            wheresql = "";
            liSqlPara.Clear();

            #region Target EquList
            sql = @" SELECT DISTINCT EquData.* FROM B01_EquData AS EquData
                     INNER JOIN B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID
                     INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID
                     INNER JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID ";

            #region DataAuth
            if (wheresql != "") wheresql += " AND ";
            wheresql += " (SysUser.UserID = ? OR EquData.CreateUserID = ? OR EquData.CreateUserID IN (SELECT UserID FROM B00_SysUser WHERE OwnerList LIKE ?)) ";
            liSqlPara.Add("S:" + this.hideUserID.Value);
            liSqlPara.Add("S:" + this.hideUserID.Value);
            liSqlPara.Add("S:" + '%' + this.hideUserID.Value + '%');
            #endregion

            if (!string.IsNullOrEmpty(ViewState["query_type"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (EquData.EquClass = ?) ";
                liSqlPara.Add("S:" + ViewState["query_type"].ToString().Trim());
            }
            else
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " 1 = 0 ";
            }

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY EquData.EquNo ";

            oAcsDB.GetDataTable("EquListTable", sql, liSqlPara, out dt);
            strTargetCount = dt.Rows.Count.ToString();
            GirdViewDataBind(this.TargetGridView, dt);
            this.lblTarget.Text = string.Format(GetLocalResourceObject("ttEquTarget").ToString(), strTargetCount);            
            TargetUpdatePanel.Update();
            #endregion            
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "msg", "$.unblockUI();", true);
        }
        #endregion

        #region LoadSourceEquList
        public void LoadSourceEquList()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "", strCount = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = @" SELECT DISTINCT EquData.* FROM B01_EquData AS EquData
                     INNER JOIN B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID
                     INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID
                     INNER JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID ";

            #region DataAuth
            if (wheresql != "") wheresql += " AND ";
            wheresql += " (SysUser.UserID = ? OR EquData.CreateUserID = ? OR EquData.CreateUserID IN (SELECT UserID FROM B00_SysUser WHERE OwnerList LIKE ?)) ";
            liSqlPara.Add("S:" + this.hideUserID.Value);
            liSqlPara.Add("S:" + this.hideUserID.Value);
            liSqlPara.Add("S:" + '%' + this.hideUserID.Value + '%');
            #endregion

            if (!string.IsNullOrEmpty(ViewState["query_type"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (EquData.EquClass = ?) ";
                liSqlPara.Add("S:" + ViewState["query_type"].ToString().Trim());
            }
            else
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " 1 = 0 ";
            }

            if (!string.IsNullOrEmpty(ViewState["query_no"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (EquData.EquNo LIKE ?) ";
                liSqlPara.Add("S:" + "%" + ViewState["query_no"].ToString().Trim() + "%");
            }

            if (!string.IsNullOrEmpty(ViewState["query_name"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (EquData.EquName LIKE ?) ";
                liSqlPara.Add("S:" + "%" + ViewState["query_name"].ToString().Trim() + "%");
            }

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY EquData.EquNo ";
            #endregion

            oAcsDB.GetDataTable("EquListTable", sql, liSqlPara, out dt);
            strCount = dt.Rows.Count.ToString();
            GirdViewDataBind(this.SourceGridView, dt);
            this.lblSource.Text = string.Format(GetLocalResourceObject("ttEquSource").ToString(), strCount);
            SourceUpdatePanel.Update();
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "msg", "ShowLabel('來源設備(共','" + strCount + "','Source');", true);
        }
        #endregion

        #region LoadTargetEquList
        public void LoadTargetEquList()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "", strCount = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = @" SELECT DISTINCT EquData.* FROM B01_EquData AS EquData
                     INNER JOIN B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID
                     INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID
                     INNER JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID ";

            #region DataAuth
            if (wheresql != "") wheresql += " AND ";
            wheresql += " (SysUser.UserID = ? OR EquData.CreateUserID = ? OR EquData.CreateUserID IN (SELECT UserID FROM B00_SysUser WHERE OwnerList LIKE ?)) ";
            liSqlPara.Add("S:" + this.hideUserID.Value);
            liSqlPara.Add("S:" + this.hideUserID.Value);
            liSqlPara.Add("S:" + '%' + this.hideUserID.Value + '%');
            #endregion

            if (!string.IsNullOrEmpty(ViewState["query_type"].ToString().Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (EquData.EquClass = ?) ";
                liSqlPara.Add("S:" + ViewState["query_type"].ToString().Trim());
            }
            else
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " 1 = 0 ";
            }

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY EquData.EquNo ";
            #endregion

            oAcsDB.GetDataTable("EquListTable", sql, liSqlPara, out dt);
            strCount = dt.Rows.Count.ToString();
            GirdViewDataBind(this.TargetGridView, dt);
            this.lblTarget.Text = string.Format(GetLocalResourceObject("ttEquTarget").ToString(), strCount);
            TargetUpdatePanel.Update();
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "msg", "ShowLabel('目的設備(共','" + strCount + "','Target');", true);
        }
        #endregion

        #region Check_Input
        public Pub.MessageObject Check_Input(string SourceEid, List<string> TargetEid, string AuthActMode, string ConflictActMode)
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            if (string.IsNullOrEmpty(SourceEid.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\\n";
                objRet.result = false;
                objRet.message += GetLocalResourceObject("MsgSelectSource");// "來源設備 必須指定";
            }

            if (TargetEid.Count <= 0)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\\n";
                objRet.result = false;
                objRet.message += GetLocalResourceObject("MsgSelectTarget");// "目的設備 必須指定";
            }

            if (string.IsNullOrEmpty(AuthActMode))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\\n";
                objRet.result = false;
                objRet.message += "權限復制動作 必須指定";
            }
            else if (string.Compare(AuthActMode, "Add") == 0 && string.IsNullOrEmpty(ConflictActMode))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\\n";
                objRet.result = false;
                objRet.message += "權限附加進階動作 必須指定";
            }
            return objRet;
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
                ProcessGridView.Rows[0].Cells[0].Text = GetGlobalResourceObject("Resource", "NonData").ToString();
            }
        }
        #endregion

        #endregion
    }
}