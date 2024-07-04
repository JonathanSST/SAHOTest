using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs
{
    public partial class MsgPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void NewMsgTimer_Tick(object sender, EventArgs e)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "", wheresql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt;
            string result = "";

            sql = " SELECT * FROM B00_MessageNotice ";

            wheresql += " ReceiveUserID = ? AND ReadTime IS NULL ";
            liSqlPara.Add("S:" + Sa.Web.Fun.GetSessionStr(this.Page, "UserID"));

            if (wheresql != "")
                sql += " WHERE " + wheresql;

            oAcsDB.GetDataTable("table", sql, liSqlPara, out dt);

            if (dt.Rows.Count > 0)
                result = "您有 " + dt.Rows.Count.ToString() + " 筆新的訊息";
            oAcsDB.CloseConnect();
            LabNewMsg.Text = result;
            LabNewMsg.Visible = true;
          
        }
    }
}