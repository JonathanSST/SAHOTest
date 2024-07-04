using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DapperDataObjectLib;
using System.Web.SessionState;


namespace SahoAcs.Web._01._0101
{
    /// <summary>
    /// OrgDataFun 的摘要描述
    /// </summary>
    public class OrgDataFun : IHttpHandler, IRequiresSessionState 
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.Request["PageEvent"] != null && context.Request["PageEvent"] == "SaveOrgData")
            {
                string error_message = "";
                if (context.Request["CHK_COL_0"] != null)
                {
                    List<string> checkOrg = context.Request.Form.GetValues("CHK_COL_0").ToList();
                    List<string> org_names = context.Request.Form.GetValues("NewOrgName").ToList();
                    List<string> org_nos = context.Request.Form.GetValues("NewOrgNo").ToList();
                    List<string> org_class_s = context.Request.Form.GetValues("NewOrgClass").ToList();
                    foreach (var s in checkOrg)
                    {
                        int key_index = int.Parse(s);
                        string name = org_names[key_index];
                        string no = org_nos[key_index];
                        string org_class = org_class_s[key_index];
                        OrmDataObject orm_data_obj = new OrmDataObject("MsSql"
                            , string.Format(Pub.db_connection_template, Pub.db_source_ip, Pub.db_data_name, Pub.db_user_id, Pub.db_pwd));
                        var query_orgs = orm_data_obj.GetQueryResult("SELECT * FROM B01_OrgData WHERE OrgNo=@OrgNo ",
                            new { OrgNo = string.Concat(org_class.Substring(0, 1), no) });
                        if (query_orgs.Count() == 0)
                        {
                            orm_data_obj.Execute(@"INSERT INTO B01_OrgData 
                        (OrgNo,OrgName,OrgClass,CreateUserID,CreateTime) 
                        VALUES (@OrgNo,@OrgName,@OrgClass,@CreateUser,GETDATE())",
                                new { OrgNo = string.Concat(org_class.Substring(0, 1), no), OrgName = name, OrgClass = org_class, 
                                    CreateUser = context.Session["UserID"].ToString() });
                            error_message = orm_data_obj.DbExceptionMessage;
                        }
                    }
                }//end check request
                context.Response.Clear();
                context.Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new { message = "Ok", success = true
                    ,error_msg=error_message }));
                context.Response.End();
            }
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