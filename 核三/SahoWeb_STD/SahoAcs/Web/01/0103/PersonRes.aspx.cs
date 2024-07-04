using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SahoAcs.DBModel;
using Sa.DB;

namespace SahoAcs.Web._01._0103
{
    public partial class PersonRes : System.Web.UI.Page
    {
        public DataTable DataOrgCompany = new DataTable();
        public DataTable DataOrgUnit = new DataTable();
        public DataTable DataOrgDept = new DataTable();
        public DataTable DataOrgTitle = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {                
            if (Request["action"] != null && Request["action"].Equals("q"))
                this.QueryPeron();
            else
            {
                this.SetInitial();   
            }
        }

        public void SetInitial()
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string sql = "SELECT * FROM B01_OrgData WHERE OrgClass='Company' ";
            string wheresql = "";
            List<string> liSqlPara = new List<string>();
            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out this.DataOrgCompany);
            sql = "SELECT * FROM B01_OrgData WHERE OrgClass='Department' ";
            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out this.DataOrgDept);
            sql = "SELECT * FROM B01_OrgData WHERE OrgClass='Unit' ";
            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out this.DataOrgUnit);
            sql = "SELECT * FROM B01_OrgData WHERE OrgClass='Title' ";
            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out this.DataOrgTitle);
        }



        public void QueryPeron()
        {
            Response.ContentType = "application/json";
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string sql = "SELECT * FROM B01_Person WHERE PsnId=? ", wheresql = "";
            List<String> CheckData = new List<String>();
            List<string> liSqlPara = new List<string>();
            liSqlPara.Add("S:" +Request["id"]);
            DataTable dt = new DataTable();
            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            PersonModel model = new PersonModel();
            foreach (DataRow r in dt.Rows)
            {
                model = new PersonModel()
                {
                    psn_id = Convert.ToInt32(r["PsnId"]),
                    psn_name = Convert.ToString(r["PsnName"]),
                    psn_no = Convert.ToString(r["PsnNo"]),
                    id_num=Convert.ToString(r["IdNum"]),
                    org_struct_id=Convert.ToString(r["OrgStrucId"])
                };
            }
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(model));
        }


        [System.Web.Services.WebMethod]
        public static PersonModel GetPerson(int id)
        {            
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string sql = "SELECT * FROM B01_Person WHERE PsnId=? ", wheresql = "";
            List<String> CheckData = new List<String>();
            List<string> liSqlPara = new List<string>();
            liSqlPara.Add("S:" + id.ToString());
            DataTable dt = new DataTable();
            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            PersonModel model = new PersonModel();
            foreach (DataRow r in dt.Rows)
            {
                model = new PersonModel()
                {
                    psn_id = Convert.ToInt32(r["PsnId"]),
                    psn_name = Convert.ToString(r["PsnName"]),
                    psn_no = Convert.ToString(r["PsnNo"]),
                    id_num = Convert.ToString(r["IdNum"]),
                    org_struct_id = Convert.ToString(r["OrgStrucId"])
                };
            }
            return model;
        }

    }
}