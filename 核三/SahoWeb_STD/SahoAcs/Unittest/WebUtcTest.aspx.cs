﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using System.Xml.Linq;
using System.Xml;


namespace SahoAcs.Unittest
{
    public partial class WebUtcTest : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        protected void Page_Load(object sender, EventArgs e)
        {
            foreach (var o in this.odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo=@ParaNo", new { ParaNo = "DbLocalUTC"}))
            {
                XmlDocument xd = new XmlDocument();
                //string path = Request.PhysicalPath;
                var page = System.Web.HttpContext.Current;
                xd.Load(System.Web.HttpContext.Current.Server.MapPath("~/SysPara.xml"));
                XElement doc = XElement.Parse(xd.OuterXml);
                if (doc.Element("DbUTC") != null && Convert.ToString(o.ParaNo) == "DbLocalUTC")
                {
                    //return int.Parse(doc.Element("DbUTC").Value);
                    doc.Element("DbUTC").SetValue(Convert.ToString(o.ParaValue));
                    doc.Save(System.Web.HttpContext.Current.Server.MapPath("~/SysPara.xml"));
                }              
            }
        }


    }//end page class
}//end namespace