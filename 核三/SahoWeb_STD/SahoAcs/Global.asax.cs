using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.SessionState;

namespace SahoAcs
{
    public class Global : System.Web.HttpApplication
    {
        //string strUserId = WebConfigurationManager.AppSettings["DbID"];
        //string strPassword = WebConfigurationManager.AppSettings["DbPW"];
        //string strDataSource = WebConfigurationManager.AppSettings["DbSource"];
        //string strInitialCatalog = ConfigurationManager.AppSettings["DbName"].ToString();
       
        protected void Application_Start(object sender, EventArgs e)
        {
            #region 啟用SqlDependency
            //DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

            //string strConnString = string.Format(@"Data Source={0};Initial Catalog={1};User Id={2};Password={3};",
            //    strDataSource, strInitialCatalog, strUserId, strPassword);

            //try
            //{
            //    System.Data.SqlClient.SqlDependency.Start(strConnString);
            //}
            //catch
            //{
            //    #region 啟用SQL SERVER Service BROKER
            //    // 取得本機連線資料庫名稱
            //    string strLocalDBName = oAcsDB.GetStrScalar("SELECT DB_NAME()");

            //    // 1&2. 立刻將資料庫設定為單一使用者模式，並ROLLBACK目前所有交易
            //    // 3.   建立 BROKER
            //    // 4.   再將資料庫設定為多使用者模式
            //    string strSQL = string.Format(@"
            //        ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            //        ALTER DATABASE {0} SET SINGLE_USER;
            //        ALTER DATABASE {0} SET NEW_BROKER;
            //        ALTER DATABASE {0} SET MULTI_USER;", strLocalDBName);

            //    oAcsDB.SqlCommandExecute(strSQL);
            //    System.Data.SqlClient.SqlDependency.Start(strConnString);
            //    #endregion
            //}
            #endregion
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            //if(HttpContext.Current)
            Session.Add("tmpList", new List<string>()); // 記錄已讀取資料的SQL參數的List
            Session.Add("tmpDatatable", new object());  // 記錄已讀取資料的Datatable           
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Request.Cookies["i18n"] != null)
            {
                if (Request.Cookies["i18n"].Value.ToString() == "en-US")
                {
                    CultureInfo currentInfo = new CultureInfo("en-US");
                    Thread.CurrentThread.CurrentCulture = currentInfo;
                    Thread.CurrentThread.CurrentUICulture = currentInfo;
                }
                else if (Request.Cookies["i18n"].Value.ToString() == "zh-TW")
                {
                    CultureInfo currentInfo = new CultureInfo("zh-TW");
                    Thread.CurrentThread.CurrentCulture = currentInfo;                    
                    Thread.CurrentThread.CurrentUICulture = currentInfo;
                }
            }
            else
            {
                Request.Cookies.Add(new HttpCookie("i18n", "zh-TW"));
                CultureInfo currentInfo = new CultureInfo("zh-TW");
                Thread.CurrentThread.CurrentCulture = currentInfo;
                Thread.CurrentThread.CurrentUICulture = currentInfo;
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
            
        }

        protected void Application_End(object sender, EventArgs e)
        {
            #region 關閉SqlDependency
            //string strConnString = string.Format(@"Data Source={0};InitialCatalog={1};User Id={2};Password={3};",
            //    strDataSource, strInitialCatalog, strUserId, strPassword);

            //try
            //{
            //    System.Data.SqlClient.SqlDependency.Stop(strConnString);
            //}
            //catch
            //{

            //}
            #endregion
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {            
            HttpResponse response = HttpContext.Current.Response;
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
        }

    }
}