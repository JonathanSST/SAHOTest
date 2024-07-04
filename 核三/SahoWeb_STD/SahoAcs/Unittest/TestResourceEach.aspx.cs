using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Unittest
{
    public partial class TestResourceEach : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Resources.ResourceSet resourceSet = 
                Resources.ResourceEquData.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentUICulture, true, true);
            foreach (System.Collections.DictionaryEntry entry in resourceSet)
            {
                Response.Write(entry.Key + ";;");
                Response.Write(entry.Value + ";;");                
            }
            //foreach(System.col)
            this.GetResxNameByValue("btnSave");
        }


        private string GetResxNameByValue(string value)
        {
            //System.Resources.ResourceManager rm = new System.Resources.ResourceManager("SahoAcs.App_GlobalResources.Resource", typeof(SahoAcs.App_GlobalResources.Resource).Assembly);            
            global::System.Resources.ResourceManager rm = new global::System.Resources.ResourceManager("Resources.Resource", global::System.Reflection.Assembly.Load("App_GlobalResources"));
            global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Resources.Resource",typeof(SahoAcs.App_GlobalResources.Resource).Assembly);
            var entry =
                rm.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true)
                  .OfType<DictionaryEntry>()
                  .FirstOrDefault(e => e.Value.ToString() == value);
            var entries = rm.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true)
                  .OfType<DictionaryEntry>();
            //var key = entry.Key.ToString();
            return "";
        }

    }
}