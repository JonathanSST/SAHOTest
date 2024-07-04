using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using System.Web.Script.Serialization;


namespace SahoAcs
{
    public partial class FloorNameListPop : Sa.BasePage
    {
        
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        DataTable UITable = new DataTable();
        DataRow UIRow;
        DataColumn UICol1, UICol2;
        OrmDataObject od = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        #endregion

        #region Events

        #region Page_Load
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            #region LoadProcess
            //oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Page.FindControl("ToolkitScriptManager1");
            //oScriptManager.EnablePageMethods = true;
            //oScriptManager.RegisterAsyncPostBackControl(this.popB_SetElevatorButton);
            //oScriptManager.RegisterAsyncPostBackControl(this.popB_Save);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("FloorNameListPop", "/Web/02/ParaPop/FloorNameListPop.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");

            #region 註冊Button動作
            //popB_SetElevatorButton.Attributes["onClick"] = "SetElevatorRow(); return false;";
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

            #endregion

            if (!IsPostBack)
            {
                if (Request["PageEvent"] == null)
                {
                    #region Give hideValue
                    hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                    this.hideEquID.Value = Request["EquID"];
                    this.hideEquParaID.Value = Request["EquParaID"];
                    this.hideParaValue.Value = Request["ParaValue"];
                    #endregion


                    #region ElevatorTable
                    var dataresult = this.od.GetQueryResult<FloorAccess>("SELECT * FROM B01_ElevatorFloor WHERE EquID=@EquID ORDER BY IOIndex "
                       , new { EquID = this.hideEquID.Value }).ToList();
                    if (dataresult.Count == 0)
                    {
                        dataresult.Add(new FloorAccess() { IoIndex = "0" });
                    }
                    DataTable tableresult = OrmDataObject.IEnumerableToTable(dataresult);
                    this.Elevator_GridView.DataSource = tableresult;
                    this.Elevator_GridView.DataBind();
                    #endregion

                    popL_FunctionRemind.Text = "提醒：樓層名稱設定並不會直接影響權限開放與否，請於名稱設定完成後，進一步調整權限。";
                }
                else
                {
                    using (var reader = new System.IO.StreamReader(Request.InputStream))
                    {
                        //string json = reader.ReadToEnd();
                        var serializer = new JavaScriptSerializer();
                        dynamic resp = serializer.DeserializeObject(Request["PostData"]);
                        if (resp != null)
                        {
                            List<FloorAccess> InputFloorAccess = new List<FloorAccess>();
                            foreach (var s in resp["NameList"])
                            {
                                InputFloorAccess.Add(new FloorAccess()
                                {
                                    EquID = int.Parse(Convert.ToString(resp["EquID"])),
                                    FloorName = Convert.ToString(s["FloorName"]),
                                    IoIndex = Convert.ToString(s["IoIndex"])
                                });                                
                            }
                            int EquID = int.Parse(Convert.ToString(resp["EquID"]));
                            this.od.Execute("DELETE B01_ElevatorFloor WHERE EquID=@EquID", new { EquID = EquID });
                            this.od.Execute("INSERT INTO B01_ElevatorFloor (EquID,IOIndex,FloorName) VALUES (@EquID,@IoIndex,@FloorName)", InputFloorAccess);
                            Response.Clear();
                            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                                new { message = "OK", success = true }));
                            Response.End();
                        }
                    }
                }
            }
            
        }
        #endregion


        #region LoadData
        private void LoadData()
        {
            #region Give hideValue
            hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            this.hideEquID.Value = Request["EquID"];
            this.hideEquParaID.Value = Request["EquParaID"];
            this.hideParaValue.Value = Request["ParaValue"];
            #endregion


            #region ElevatorTable
            this.Elevator_GridView.DataSource = this.od.GetQueryResult<FloorAccess>("SELECT * FROM B01_ElevatorFloor WHERE EquID=@EquID ORDER BY IOIndex "
                , new { EquID = this.hideEquID.Value });
            this.Elevator_GridView.DataBind();
            #endregion

            popL_FunctionRemind.Text = "提醒：樓層名稱設定並不會直接影響權限開放與否，請於名稱設定完成後，進一步調整權限。";
        }

        #endregion

        #region Elevator_GridView_RowDataBound
        protected void Elevator_GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    e.Row.Cells[0].Width = 100;
                    #endregion

                    #region 寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    popli_header.Text = Header_sw.ToString();
                    #endregion

                    break;
                #endregion

                #region DataRow
                case DataControlRowType.DataRow:
                    //DataRowView oRow = (DataRowView)e.Row.DataItem;
                    //DataTable ProcessTable = (DataTable)ViewState["UITable"];

                    #region 設定欄位寛度
                    e.Row.Cells[0].Width = 100;
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 樓層名稱
                    //TextBox tb = (TextBox)e.Row.FindControl("FloorName");
                    //tb.Text = ProcessTable.Rows[e.Row.RowIndex]["FloorName"].ToString();
                    #endregion

                    #endregion

                    break;
                #endregion
            }
        }
        #endregion

        #region popB_Save_Click
        protected void popB_Save_Click(object sender, EventArgs e)
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            GridViewToDatatable();
            string FloorNamestr = "";
            UITable = (DataTable)ViewState["UITable"];
            objRet = Check_Input_DB(hideEquID.Value, hideEquParaID.Value, UITable);
            if (objRet.result)
            {
                for (int i = 0; i < UITable.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(FloorNamestr)) FloorNamestr += ",";
                    FloorNamestr += UITable.Rows[i][0] + ":" + UITable.Rows[i][1];
                }

                Sa.Web.Fun.RunJavaScript(this, "window.returnValue = '" + "FloorNameList/SahoFromURL" + FloorNamestr + "';");
                Sa.Web.Fun.RunJavaScript(this, "window.close();");
            }
            else
                Sa.Web.Fun.RunJavaScript(this, objRet.message);
        }
        #endregion

        #endregion

        #region Method

        #region LoadRowToDatatable
        public DataTable LoadRowToDatatable(DataTable Process, string FloorName)
        {
            if (!string.IsNullOrEmpty(FloorName) && string.Compare(FloorName, "undefined") != 0)
            {
                string[] FloorNameArray = FloorName.Split(',');

                for (int i = 0; i < FloorNameArray.Length; i++)
                {
                    string[] Item = FloorNameArray[i].Split(':');
                    Process.Rows.Add(Process.NewRow());
                    Process.Rows[Process.Rows.Count - 1]["IOIndex"] = Item[0].ToString();
                    Process.Rows[Process.Rows.Count - 1]["FloorName"] = Server.HtmlDecode(Item[1].ToString());
                }
            }
            return Process;
        }
        #endregion

        #region SetRowToDatatable
        protected DataTable SetRowToDatatable()
        {
            return new DataTable();
        }
        #endregion

        #region GridViewToDatatable
        public void GridViewToDatatable()
        {
            if (ViewState["UITable"] != null)
            {
                DataTable ProcessTable = (DataTable)ViewState["UITable"];
                for (int i = 0; i < Elevator_GridView.Rows.Count; i++)
                {
                    TextBox tb = (TextBox)Elevator_GridView.Rows[i].FindControl("FloorName");
                    ProcessTable.Rows[i]["FloorName"] = tb.Text;
                }
                ViewState["UITable"] = ProcessTable;
            }
        }
        #endregion

        #region Check_Input_DB
        protected Pub.MessageObject Check_Input_DB(string EquID, string EquParaID, DataTable UITable)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            int tempint;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Input
            if (string.IsNullOrEmpty(EquID))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "設備 必須指定";
            }

            if (string.IsNullOrEmpty(EquParaID))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "參數項目 必須指定";
            }

            for (int i = 0; i < UITable.Rows.Count; i++)
            {
                if (!int.TryParse(UITable.Rows[i][0].ToString(), out tempint))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "接點編號 必須為整數";
                    break;
                }
            }




            #endregion
            #region DB

            sql = @" SELECT EquClass FROM B01_EquData WHERE EquID = ? ";
            liSqlPara.Add("S:" + EquID.ToString());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                if (string.Compare(dr.DataReader["EquClass"].ToString(), "Elevator") != 0)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "設備需為電梯";
                }
            }
            #endregion

            return objRet;
        }
        #endregion

        #endregion
    }
}
