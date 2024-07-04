using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Reflection;
namespace SahoAcs.DBClass
{
    public class BasePage:Page
    {
        protected virtual void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString.Count > 0)
            {
                Response.Redirect("~/Web/MessagePage/DeniedAuth.html");
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

        public string GetNonData
        {
            get
            {

                return this.GetGlobalResourceObject("Resource", "NonData").ToString();
            }
        }


        protected Dictionary<string, string> GetMasterPackage(List<string> ListCols)
        {

            //取得可以加入columns的參數
            Dictionary<string, string> colDic;
            colDic = new Dictionary<string, string>();
            ListCols = ListCols.Distinct().ToList();
            foreach (string colname in ListCols)
            {
                if (Request.Form.AllKeys.Contains(colname))
                {
                    //Request.Form.AllKeys.Where(i => i.Contains("")).First();
                    colDic.Add(colname, Request[colname]);
                }
            }
            return colDic;
        }


        public Dictionary<string, object> GetClassToDictionary<T>(T t) where T : class
        {

            Dictionary<string, object> dict = new Dictionary<string, object>();
            PropertyInfo[] props = t.GetType().GetProperties();
            foreach (var p in props)
            {
                Type propType = p.PropertyType;
                if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    propType = Nullable.GetUnderlyingType(propType);
                }
                if (p.GetValue(t, null) == null)
                {
                    continue;
                }
                if (propType.Name == "DateTime" && Convert.ToDateTime(p.GetValue(t, null)).Year == 1)
                {
                    dict.Add("@" + p.Name, DateTime.Now);
                }
                else
                {
                    dict.Add("@" + p.Name, p.GetValue(t, null));
                }

            }
            return dict;
        }


        public T DictionaryToObject<T>(IDictionary<string, string> dict) where T : new()
        {
            var t = new T();
            PropertyInfo[] properties = t.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (!dict.Any(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                    continue;

                KeyValuePair<string, string> item = dict.First(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));

                // Find which property type (int, string, double? etc) the CURRENT property is...
                Type tPropertyType = t.GetType().GetProperty(property.Name).PropertyType;

                // Fix nullables...
                Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;
                if (newT.Name == "DateTime" && item.Value == "")
                {
                    continue;
                }
                // ...and change the type                
                object newA = Convert.ChangeType(item.Value, newT);
                t.GetType().GetProperty(property.Name).SetValue(t, newA, null);
            }
            return t;

        }


    }//end class
}//end namepage