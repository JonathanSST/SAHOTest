using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using System.Data;

using SahoAcs.DBModel;

namespace SahoAcs.Web._02._0207
{
    public partial class EquGroupSettingPop : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql",Pub.GetDapperConnString());

        public List<EquGroupData> QueueList = new List<EquGroupData>();

        public List<EquGroupData> PendingList = new List<EquGroupData>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null)
            {
                string PageEvent = Request["PageEvent"].ToString();

                if (PageEvent == "Queue")
                {

                }
                else if (PageEvent == "Pending")
                {

                }
                else if (PageEvent == "Query")
                {                  
                    this.QueueList = this.odo.GetQueryResult<EquGroupData>(@"SELECT
	                            B.*,C.EquName,C.EquModel,A.EquGrpID,A.EquGrpNo,C.EquNo
                            FROM 
	                            B01_EquGroup A
	                            INNER JOIN B01_EquGroupData B ON A.EquGrpID=B.EquGrpID
	                            INNER JOIN b01_EquData C ON C.EquID=B.EquID WHERE A.EquGrpNo=@EquGrpID ", new { EquGrpID = Request["EquGrpID"] }).ToList();

                    this.PendingList = this.odo.GetQueryResult<EquGroupData>(@"SELECT * FROM B01_EquData 
                                                WHERE EquID NOT IN (SELECT EquID FROM B01_EquGroupData WHERE 
                        EquGrpID=(SELECT TOP 1 EquGrpID FROM B01_EquGroup WHERE EquGrpNo=@EquGrpID))", new { EquGrpID = Request["EquGrpID"] }).ToList();
                }
            }
            
        }


        protected void QueueInput_EquClass_Init(object sender,EventArgs e)
        {
            System.Web.UI.WebControls.ListItem Item;

            string sql = "";
            DataTable dt;

            #region Give Empty Item
            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = string.Format("- {0} -", GetGlobalResourceObject("Resource", "ddlSelectDefault"));
            Item.Value = "";
            this.QueueInput_EquClass.Items.Add(Item);
            #endregion

            #region Process String
            sql = @" SELECT DISTINCT ItemInfo1 FROM B00_ItemList
                     WHERE ItemClass = 'EquModel' ";
            #endregion

            dt = this.odo.GetDataTableBySql(sql);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new System.Web.UI.WebControls.ListItem();
                switch (dr["ItemInfo1"].ToString())
                {
                    case "Door Access":
                        Item.Text = GetGlobalResourceObject("ResourceGrp", "EquTypeDA").ToString();
                        break;
                    case "Elevator":
                        Item.Text = GetGlobalResourceObject("ResourceGrp", "EquTypeElev").ToString();
                        break;
                    case "TRT":
                        Item.Text = GetGlobalResourceObject("ResourceGrp", "EquTypeTRT").ToString();
                        break;
                    case "Meal":
                        Item.Text = GetGlobalResourceObject("ResourceGrp", "EquTypeMeal").ToString();
                        break;
                }
                Item.Value = dr["ItemInfo1"].ToString();
                this.QueueInput_EquClass.Items.Add(Item);
            }
        }

        protected void PendingInput_EquClass_Init(object sender, EventArgs e)
        {
            System.Web.UI.WebControls.ListItem Item;
            
            string sql = "";
            DataTable dt;

            #region Give Empty Item
            Item = new System.Web.UI.WebControls.ListItem();
            Item.Text = string.Format("- {0} -", GetGlobalResourceObject("Resource", "ddlSelectDefault"));
            Item.Value = "";
            this.PendingInput_EquClass.Items.Add(Item);
            #endregion

            #region Process String
            sql = @" SELECT DISTINCT ItemInfo1 FROM B00_ItemList
                     WHERE ItemClass = 'EquModel' ";
            #endregion

            dt=this.odo.GetDataTableBySql(sql);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new System.Web.UI.WebControls.ListItem();
                switch (dr["ItemInfo1"].ToString())
                {
                    case "Door Access":
                        Item.Text = GetGlobalResourceObject("ResourceGrp", "EquTypeDA").ToString();
                        break;
                    case "Elevator":
                        Item.Text = GetGlobalResourceObject("ResourceGrp", "EquTypeElev").ToString();
                        break;
                    case "TRT":
                        Item.Text = GetGlobalResourceObject("ResourceGrp", "EquTypeTRT").ToString();
                        break;
                    case "Meal":
                        Item.Text = GetGlobalResourceObject("ResourceGrp", "EquTypeMeal").ToString();
                        break;
                }
                Item.Value = dr["ItemInfo1"].ToString();
                this.PendingInput_EquClass.Items.Add(Item);
            }
        }


    }
}