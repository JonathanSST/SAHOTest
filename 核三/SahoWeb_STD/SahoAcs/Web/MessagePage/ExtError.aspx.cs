using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web.MessagePage
{
    public partial class ExtError : System.Web.UI.Page
    {
        public string UserName = "";

        public string UserId = "";

        public string ErrorMessage = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["ErrorCode"] != null)
            {
                string ErrorCode = Request["ErrorCode"];
                if (ErrorCode == "1")
                {
                    ErrorMessage = string.Format("無效的LigthtID或逾時處理，LightID={0}",Request["LightID"]);
                }
                if (ErrorCode == "2")
                {
                    this.UserId = Request["UserID"];
                    this.UserName = Request["UserName"];
                    ErrorMessage = string.Format("使用者帳號 {0} 姓名 {1} 尚未建立", this.UserId, this.UserName);
                        //$"使用者帳號 {this.UserId} 姓名 {this.UserName} 尚未建立";
                }
            }
        }//end method


    }//end pae class
}//end namespace