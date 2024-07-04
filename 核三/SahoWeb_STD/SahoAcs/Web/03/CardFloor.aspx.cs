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

namespace SahoAcs
{
    public partial class CardFloor : Sa.BasePage
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;     //宣告Ajax元件
        #endregion

        #region LoadProcess
        private void LoadProcess()
        {
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Page.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            //oScriptManager.RegisterAsyncPostBackControl(QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            //js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("CardFloor", "CardFloor.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "OrgClassData();", true);
        }
        #endregion

        #region RegisterObj
        private void RegisterObj()
        {
            #region 設定彈跳視窗
            //Pub.SetModalPopup(ModalPopupExtender1, 1);
            //Pub.SetModalPopup(ModalPopupExtender2, 2);
            #endregion

            #region 註冊主頁Button動作
            BtCancel.Attributes["onClick"] = "window.close();return false;";
            #endregion
        }
        #endregion

        #region Page_Load
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            this.Title = "樓層選擇";
            LoadProcess();
            RegisterObj();
            if (!IsPostBack)
            {
                SelectData();
                Hashtable hFloorData = (Hashtable)Session["FloorData"];

                if (hFloorData != null && hFloorData.ContainsKey(Request.QueryString["EquID"]))
                {
                    if (!hFloorData[Request.QueryString["EquID"]].ToString().Equals(""))
                    {
                        String BinStr = Sa.Change.HexToBin(hFloorData[Request.QueryString["EquID"]].ToString(), 48);
                        String obj = "";
                        for (int i = 0; i < BinStr.Length; i++)
                            obj = BinStr[i] + obj;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "CheckBoxData('" + obj + "');", true);
                    }
                    //若有PsnID 則顯示設備樓層開放資訊
                    if (Request.QueryString["CardID"]!=null)
                    {
                        //取得設備資訊
                        DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
                        List<string> paras = new List<string>();
                        paras.Add("S:" + Request.QueryString["CardID"]);
                        paras.Add("S:" + Request.QueryString["EquID"]);
                        string floor_data = oAcsDB.GetStrScalar("SELECT CardExtData FROM B01_CardEquAdj WHERE CardID=? AND EquID=? ",paras);
                        String BinStr = Sa.Change.HexToBin(floor_data, 48);
                        String obj = "";
                        for (int i = 0; i < BinStr.Length; i++)
                            obj = BinStr[i] + obj;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "CheckBoxData('" + obj + "');", true);
                        this.BtSave.Enabled = false;
                    }
                }
            }
        }
        #endregion

        #region VerifyRenderingInServerForm
        public override void VerifyRenderingInServerForm(Control control)
        {
            //如有使用UpdatePanel配合GridVew才需要這個方法
            // 修正 'XX'型別 必須置於有 runat=server 的表單標記之中 Override此Methods
        }
        #endregion

        protected void GridView1_Data_DataBound(object sender, EventArgs e)
        {
            td_showGridView1.Attributes["colspan"] = GridView1.Columns.Count.ToString();
        }

        protected void GridView1_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    e.Row.Cells[0].Width = 20;
                    e.Row.Cells[1].Width = 55;
                    #endregion

                    #region 設定表頭欄位Visible狀態
                    //不想顯示的欄位，可在此處關掉
                    CheckBox cb = (CheckBox)e.Row.Cells[0].FindControl("CheckBox1");
                    cb.Attributes["onclick"] = "SelectAllCheckboxes();";
                    #endregion

                    #region 寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);
                    e.Row.CssClass = "GVStyle";
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    li_header1.Text = Header_sw.ToString();
                    #endregion

                    break;
                #endregion

                #region DataRow
                case DataControlRowType.DataRow:
                    DataRowView oRow = (DataRowView)e.Row.DataItem;

                    #region 指定Row的ID
                    GridViewRow GrRow = e.Row;
                    GrRow.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    GrRow.ID = "GV_Row" + oRow.Row["IOIndex"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    e.Row.Cells[0].Width = 24;
                    e.Row.Cells[1].Width = 59;
                    #endregion

                    #region 針對各欄位做所需處理

                    #region CheckBox
                    CheckBox rcb = (CheckBox)e.Row.Cells[0].FindControl("RowCheckState1");
                    rcb.Attributes["onClick"] = "SelectState();";
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    #endregion
                    #region IO點
                    #endregion
                    #region 樓層名稱
                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }
                    #endregion
                    break;
                #endregion
            }
        }

        private void SelectData()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = " SELECT * FROM B01_ElevatorFloor WHERE EquID = ? ";
            liSqlPara.Add("I:" + Request.QueryString["EquID"]);
            #endregion

            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            GridView1.DataSource = dt;
            GridView1.DataBind();
            GirdViewDataBind(GridView1, dt);
        }

        #region 查無資料時，GridView顯示查無資料資訊
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
                ProcessGridView.Rows[0].Visible = false;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "CheckBoxEnable();", true);
            }
        }
        #endregion

        protected void BtSave_Click(object sender, EventArgs e)
        {
            String FinalData = this.hFloor.Value;
            String HexStr = "";

            #region 不足位數補處理
            for (int i = FinalData.Length; i < 48; i++)
            {
                FinalData = "0" + FinalData;
            }
            #endregion

            #region 轉換16進位值
            HexStr = Sa.Change.BinToHex(FinalData, 12);
            #endregion

            Hashtable hFloorData = (Hashtable)Session["FloorData"];
            if (hFloorData.ContainsKey(Request.QueryString["EquID"]))
                hFloorData[Request.QueryString["EquID"]] = HexStr;
            else
                hFloorData.Add(Request.QueryString["EquID"], HexStr);

            String objdata = "";

            if (hFloorData.Count > 0)
            {
                foreach (DictionaryEntry obj in hFloorData)
                    objdata += obj.Key.ToString() + "|" + obj.Value.ToString() + "|";
                objdata = objdata.Substring(0, objdata.Length - 1);
            }

            string Script;
            Script = "<Script>";
            Script = Script + "window.opener.document.all." + this.Request.QueryString["FinalFloorData"] + ".value='" + objdata + "';window.close();";
            Script = Script + "</Script>";
            ClientScript.RegisterStartupScript(this.GetType(), "", Script);
        }
    }
}