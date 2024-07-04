using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Xml;
using System.Net;
using System.Text;
using System.IO;
using System.Configuration;

namespace SahoAcs.Unittest
{
    public partial class testService : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.ServiceTest();
        }

        private void ServiceTest()
        {
            XmlDocument XmlD = new XmlDocument();            

            //xmlStr = @"<root><record><key>1</key><id>X100187541</id><orgid>376560000A</orgid><oldorgid>376560000A</oldorgid><unitid>560011</unitid><oldunitid>560011</oldunitid><cardid>3840092820</cardid><name>高進中</name><titlecode>4769</titlecode><title>臨時人員</title><officialrankcode>M00</officialrankcode><officialrank>無資位</officialrank><potpe>4</potpe><petday></petday><sex>1</sex><birthday>0441127</birthday><email></email><pefstdate>0970401</pefstdate><peactdate>0970401</peactdate><pechief>0</pechief><peprofess>0</peprofess><pepoint>0000</pepoint><timestamp>2016-03-23 11:06:18</timestamp></record></root>";

            //建立 xml 的資料內容
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'");
            builder.AppendLine("xmlns:xsd='http://www.w3.org/2001/XMLSchema'");
            builder.AppendLine("xmlns:soap='http://www.w3.org/2003/05/soap-envelope'>");
            builder.AppendLine("<soap:Body>");
            builder.AppendLine("<DoQueryTest xmlns='SahoWeb'>");
            builder.AppendLine("</DoQueryTest>");
            builder.AppendLine("</soap:Body>");
            builder.AppendLine("</soap:Envelope>");
            //要求的對象網頁
            HttpWebRequest myRequest = null;
            Stream oWriter = null;
            StreamReader swText = null;
            try
            {
                myRequest = (HttpWebRequest)WebRequest.Create("http://localhost:9521/SyncService.asmx?wsdl");
                myRequest.Method = "POST";
                myRequest.Timeout = 300000;
                myRequest.ContentType = "text/xml;charset=utf-8 ";
                Console.WriteLine(builder.ToString());
                XmlD.LoadXml(builder.ToString());
                byte[] data = System.Text.Encoding.UTF8.GetBytes(XmlD.OuterXml);
                myRequest.ContentLength = data.Length;
                //發送並取得回應
                oWriter = myRequest.GetRequestStream();
                oWriter.Write(data, 0, data.Length);
                swText = new StreamReader(myRequest.GetResponse().GetResponseStream());
                oWriter.Close();
                string myResponseStr = swText.ReadToEnd();
                Response.Write("...已收到回應");
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }
}