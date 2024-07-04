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


namespace SahoAcs
{
    public partial class OrgFloorSetting:Sa.BasePage
    {
        public List<Floor> list_floor=new List<Floor> ();
        public string ext_data_bin = "";
        public class Floor
        {
            public int EquID { get; set; }
            public string FloorName { get; set; }
            public int IoIndex { get; set; }            
        }

        #region Main Description        
        Hashtable TableInfo;
        DataTable UITable = new DataTable();
        DataColumn UICol1, UICol2;
        DapperDataObjectLib.OrmDataObject odo = new OrmDataObject("MsSql",
             string.Format(Pub.db_connection_template, Pub.db_source_ip, Pub.db_data_name, Pub.db_user_id, Pub.db_pwd));
        #endregion

        #region Events


            #region Page_Load
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            #region LoadProcess            

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            /*
            js = "<script type='text/javascript'>" + js + "</script>";
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);            
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");            
            */
            ClientScript.RegisterClientScriptInclude("FloorSetting", "FloorSetting.js");//加入同一頁面所需的JavaScript檔案
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
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion                
                this.list_floor = this.odo.GetQueryResult<Floor>("SELECT * FROM B01_ElevatorFloor WHERE EquID=@EquID ORDER BY IoIndex"
                   ,new{EquID=Request["equ_id"]}).ToList();
                if (!Request["card_ext_data"].ToString().Equals(""))
                {
                    this.ext_data_bin = Sa.Change.HexToBin(Request["card_ext_data"], 48);
                }                
            }
            else
            {
                
            }
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