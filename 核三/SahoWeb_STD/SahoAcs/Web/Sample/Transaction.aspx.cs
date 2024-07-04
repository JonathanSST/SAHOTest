using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web.Sample
{
    public partial class Transaction : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

            string sql = "";
            int iRet;
            List<string> liSqlPara = new List<string>();
            oAcsDB.BeginTransaction();

            sql = "Insert into Test(Field1) VALUES('Derek')";
            iRet = oAcsDB.SqlCommandExecute(sql);

            if (iRet > -1)
            {
                sql = "Insert into Test2(ID,Field1) VALUES(NEWID(),'Derek')";
                iRet = oAcsDB.SqlCommandExecute(sql);
            }

            if (iRet > -1)
            {
                Response.Write("Commit");
                oAcsDB.Commit();
            }
            else
            {
                Response.Write("Rollback");
                oAcsDB.Rollback();
            }

        }
    }
}