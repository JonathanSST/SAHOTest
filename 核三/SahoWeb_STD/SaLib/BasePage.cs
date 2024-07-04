using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Sa
{
    public class BasePage:System.Web.UI.Page
    {

        /***

        protected void InitializeComponent()
        {

            //註冊頁面載入時呼叫的事件

            this.Load += new System.EventHandler(this.Page_Load);
        }


        protected override void OnInit(EventArgs e)
        {

            base.OnInit(e);
            InitializeComponent();
            //Response.Write("from class1 OnInit");
        }

       ***/

        protected virtual void Page_Load(object sender, System.EventArgs e)
        {
            //外部入侵網站的偵測
            if (Request.Url.Authority != (Request.UrlReferrer != null ? Request.UrlReferrer.Authority : ""))
            {
                Response.Redirect("~/Default.aspx");
            }
            if (Sa.Web.Fun.GetSessionStr(this.Page, "UserID") == "")
            {
                //session 過期的處理頁面
                string[] accept = Request.AcceptTypes;
                if (accept.Where(i => i.Contains("json")).Count() > 0)
                {
                    Response.Clear();
                    Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                        new { message = "系統工作階段已過期或被登出", result = false }));
                    Response.End();
                }
                Response.Redirect("~/Web/MessagePage/LoginSorry.aspx");//Session Timeout                
            }
        }

    }//end page class
}//end namespace
