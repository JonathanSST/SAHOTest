using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Text;
using System.Xml.XPath;
using System.Xml.Linq;


namespace SahoAcs
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            XmlDocument xd = new XmlDocument();
            string path = Request.PhysicalPath;
            xd.Load(Request.Url.Scheme + "://" + Request.Url.Authority + "/SysPara.xml");
            XElement doc = XElement.Parse(xd.OuterXml);
            path = doc.Element("MobileImg").Value;
        }
    }
}