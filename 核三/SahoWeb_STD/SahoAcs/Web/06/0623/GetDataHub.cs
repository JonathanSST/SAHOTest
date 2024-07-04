using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Web.Configuration;
using System.Threading;

namespace SahoAcs.Web._06._0623
{
    [HubName("SignalRHub")]

    public class SignalRHub : Hub
    {
        public static void ShowData()
        {
            // 呼叫用戶端方法
            var context = GlobalHost.ConnectionManager.GetHubContext<SignalRHub>();
            context.Clients.All.SqlHasChange();
        }

        public void SetState(string ddlCtrlArea)
        {
            // 被用戶端呼叫的伺服器端方法
            ShowCardExData(ddlCtrlArea);
        }

        string strUserId = WebConfigurationManager.AppSettings["DbID"];
        string strPassword = WebConfigurationManager.AppSettings["DbPW"];
        string strDataSource = WebConfigurationManager.AppSettings["DbSource"];
        string strInitialCatalog = ConfigurationManager.AppSettings["DbName"].ToString();

        public void ShowCardExData(string ddlCtrlArea)
        {
            string strConnString = string.Format(@"Data Source={0};Initial Catalog={1};User Id={2};Password={3};",
                strDataSource, strInitialCatalog, strUserId, strPassword);

            using (SqlConnection conn = new SqlConnection(strConnString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(string.Format(@"
                    SELECT CardID, LastTime, LastDoorNo, CtrlAreaNo 
                    FROM dbo.B01_CardExt WHERE CtrlAreaNo = '{0}'", ddlCtrlArea),
                    conn))
                {
                    cmd.Notification = null;
                    SqlDependency dependency = new SqlDependency(cmd);

                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (conn.State == ConnectionState.Closed) conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // SignalR 必要的一段
                    }
                }
            }
        }

        // 這裡是觸發異動事件
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Info == SqlNotificationInfo.Insert || e.Info == SqlNotificationInfo.Update)
            {
                // 應該沒必要，但範例上有。
                SqlDependency dependency = (SqlDependency)sender;
                dependency.OnChange -= dependency_OnChange;

                SignalRHub.ShowData();
                Thread.Sleep(10000);
            }
        }
    }
}



