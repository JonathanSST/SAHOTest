using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using SahoAcs.DBModel;
using Sa.DB;

namespace SahoAcs.Web._01._0103
{
    /// <summary>
    /// Person 的摘要描述
    /// </summary>
    public class Person : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            if(context.Request["action"]!=null &&context.Request["action"].Equals("q"))
            {
                this.QueryPeron(ref context);
            }
        }


        public void QueryPeron(ref HttpContext context)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string sql = @"SELECT 
	                        a.*,
	                        B.OrgIDList	 
                        FROM 
	                        B01_Person a
	                        INNER JOIN B01_OrgStruc B ON a.OrgStrucID=B.OrgStrucID WHERE PsnId=? ";
            string wheresql = "";
            List<String> CheckData = new List<String>();
            List<string> liSqlPara = new List<string>();
            liSqlPara.Add("S:" + context.Request["id"]);
            DataTable dt = new DataTable();
            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            PersonModel model = new PersonModel();
            foreach (DataRow r in dt.Rows)
            {
                model = new PersonModel()
                {
                    psn_id = Convert.ToInt32(r["PsnId"]),
                    psn_name = Convert.ToString(r["PsnName"]),
                    psn_e_name=Convert.ToString(r["PsnEName"]),
                    psn_e_time=string.Format("{0:yyyy/MM/dd}",r["PsnETime"]),
                    psn_s_time = string.Format("{0:yyyy/MM/dd}", r["PsnSTime"]),
                    birthday=string.Format("{0:yyyy/MM/dd}",r["Birthday"]),
                    psn_no = Convert.ToString(r["PsnNo"]),
                    id_num = Convert.ToString(r["IdNum"]),
                    org_struct_id = Convert.ToString(r["OrgStrucId"]),
                    org_c = Convert.ToString(r["OrgIDList"]).Split('\\')[1],
                    org_u = Convert.ToString(r["OrgIDList"]).Split('\\')[2],
                    org_d = Convert.ToString(r["OrgIDList"]).Split('\\')[3],
                    org_t = Convert.ToString(r["OrgIDList"]).Split('\\')[4],
                    psn_account=Convert.ToString(r["PsnAccount"]),
                    psn_pw=Convert.ToString(r["PsnPW"]),
                    remark=Convert.ToString(r["Remark"])
                };
            }
            context.Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(model));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}