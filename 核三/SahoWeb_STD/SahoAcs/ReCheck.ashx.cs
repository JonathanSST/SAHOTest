using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SahoAcs.DBClass;


namespace SahoAcs
{
    /// <summary>
    /// ReCheck 的摘要描述
    /// </summary>
    public class ReCheck : IHttpHandler,System.Web.SessionState.IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            string TimeOnceID = "";
            if (context.Request.Form["TimeOnceID"] != null)
            {
                TimeOnceID = context.Request.Form["TimeOnceID"].ToString();
            }
            context.Response.Clear();
            //context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            //context.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
            //context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
            if (context.Request.Form["PageEvent"] != null && context.Request.Form["PageEvent"].Equals("CheckStatus"))
            {
                bool Status = context.Session["UserID"] != null;
                string Message = Status ? "" : "工作階段授權遺失";
                context.Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new { isSuccess = Status, message = Message }));
                context.Response.End();
            }
            //訪問方法驗證 not allow get method
            if (context.Request.HttpMethod.Equals("GET"))
            {
                context.Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { isSuccess = false, message = "not allow use get method" }));
                context.Response.End();
            }
            if (context.Session["TimeOnceID"] == null)
            {
                context.Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { isSuccess = false, message = "time once id diffrent" }));
                context.Response.End();
            }
            else
            {
                //context.Session["TimeOnceID"] = Guid.NewGuid();
                TimeOnceID = context.Session["TimeOnceID"].ToString();
            }
            //同源驗證
            if (context.Request.Url.Authority != (context.Request.UrlReferrer!=null?context.Request.UrlReferrer.Authority:""))
            {
                context.Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { isSuccess = false, message="referrer url diffrent" }));
                context.Response.End();
            }
           
            if (context.Request.Form["TimeOffset"] != null)
            {
                context.Session["TimeOffset"] = context.Request.Form["TimeOffset"].ToString();
            }
            //RsaCryptorService _rsa = new RsaCryptorService("RsaCspParameters_Key");
            var encryptKey = RSAEnc.Encrypt(context.Session["UserID"].ToString());
            context.Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { isSuccess =true, UserID=encryptKey, TimeOffset = context.Session["TimeOffset"] != null ? context.Session["TimeOffset"].ToString() : "",time_once=TimeOnceID }));
            context.Response.End();
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