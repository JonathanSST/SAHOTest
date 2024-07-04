using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using Sa.DB;
using System.Net;
using OfficeOpenXml;
using SahoAcs.DBModel;
using SahoAcs.DBClass;
using DapperDataObjectLib;
using iTextSharp;
using iTextSharp.tool.xml;
using iTextSharp.text.pdf;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Luxriot.Internal.LibCommon;
using System.Web.UI.HtmlControls;
using System.Drawing;


namespace SahoAcs
{
  
    public partial class EvoClient : System.Web.UI.Page
    {
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        List<VideoAccessModel> cardlogs = null;
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        
       
        protected void Page_Load(object sender, EventArgs e)
        {
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("EvoClient", "QueryCardLog2.js");//加入同一頁面所需的JavaScript檔案

            ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                ViewState["query_sMode"] = "Normal";
                ViewState["query_CardTimeSDate"] = "";
                ViewState["query_CardTimeEDate"] = "";

                DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
                DateTime dtLastCardTime = oAcsDB.GetDateTimeScalar(@"SELECT MAX(CardTime) FROM B01_CardLog WHERE CardTime <= GETDATE()");

                if (dtLastCardTime == DateTime.MinValue)
                {
                    Calendar_CardTimeSDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 00:00:00";
                    Calendar_CardTimeEDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd") + " 23:59:59";
                }
                else
                {
                    Calendar_CardTimeSDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 00:00:00";
                    Calendar_CardTimeEDate.DateValue = dtLastCardTime.ToString("yyyy/MM/dd") + " 23:59:59";
                }

                ddl.Attributes.Add("onchange", "CallApi(this);");
            } 

        }


        public void Query()
        {   
 
            ViewState["query_CardTimeSDate"] = this.Calendar_CardTimeSDate.DateValue.ToString();
            ViewState["query_CardTimeEDate"] = this.Calendar_CardTimeEDate.DateValue.ToString();
            
            string qrystr = "declare @input varchar(max), @sep varchar(2) " +
                            "set @sep = ';' " +
                            "set @input = (select paraValue from B00_SysParameter where ParaNo = 'EvoList') " +
                            "select distinct a.CardTime, a.EquNo, a.EquName, b.CamNo, a.CardNo, a.EquDir, a.PsnName from B01_CardLog a " +
                            "left join (select dbo.GetLeftStr(Value, '|') as EquNo, dbo.GetRightStr(Value, '|') as CamNo from dbo.SplitString(@input, @sep, 1)) b " +
                            "on a.EquNo = b.EquNo " +
                            "where a.CardTime BETWEEN @CardTimeS AND @CardTimeE " +
                            "order by a.CardTime, a.CardNo";

            //CardTime, CardNo, PsnNo, PsnName, EquNo, EquName
            //string qrystr = "select * from B01_CardLog where CardTime BETWEEN @CardTimeS AND @CardTimeE";

            this.cardlogs = this.odo.GetQueryResult<VideoAccessModel>(qrystr, new
            {
                CardTimeS = ViewState["query_CardTimeSDate"].ToString(),
                CardTimeE = ViewState["query_CardTimeEDate"].ToString()

            }).ToList();

            ddl.Items.Add("");
            foreach (var item in cardlogs)
            {
                
                ddl.Items.Add("卡號_"+item.CardNo + ";人員_" + item.PsnName + ";時間_" + item.CardTime + ";卡機_" + item.EquNo + ";攝影機_" + item.CamNo + ";方向_" + item.EquDir);
            }


            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI", "$.unblockUI();", true);
        }


        protected void Button1_Click(object sender, EventArgs e)
        {
            ddl.Items.Clear();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "block", "SelectState(); return false;", true);
            Query();    
        }
        
        protected void ddl_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        public string ToMilliSecond(string logTime)
        {
            DateTime dt1970 = new DateTime(1970, 1, 1);
            DateTime current = Convert.ToDateTime(logTime);
            TimeSpan span = current - dt1970;
            return span.TotalMilliseconds.ToString();
        }
       
        public string getLogValue(string name, string input)
        {
            string output="";

            string[] tmp = input.Split(';');
            foreach (string item in tmp)
            {
                if (item.Contains(name))
                {
                    tmp = item.Split('_');
                    output = tmp[1];
                }
            }
            return output;         
        }

        public void DoAuth()
        {
            DigestWebRequest dig = new DigestWebRequest("admin", "1qaz@WSX#EDC", "CMSSystem");

            string camid = getLogValue("攝影機", ddl.SelectedValue);
            string logTime = getLogValue("時間", ddl.SelectedValue);
 
            string snapstring = "http://192.168.0.153:8080/archive/" + camid + "/snapshot?type=video1&time=" + ToMilliSecond(logTime);
            string info = "http://192.168.0.153:8080/info";
            string bound = "http://192.168.0.153:8080/archive/" + camid + "/boundaries";
            string twentySec = "http://192.168.0.153:8080/archive/" + camid + "/stream?time=" + ToMilliSecond(logTime);

            Uri infoURI = new Uri(info);
            Uri boundariesURI = new Uri(bound);
            Uri archiveSnapshotURI = new Uri(snapstring);
            Uri twentySecURI = new Uri(twentySec);

            Stream receiveStream = dig.GetResponse(archiveSnapshotURI).GetResponseStream();

            //Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            readStream.ReadToEnd();
            //TextBox1.Text= readStream.ReadToEnd();
            readStream.Close();

            Image1.ImageUrl = snapstring;
            //style="vertical-align:top; width:auto; height:330px; border:1px solid silver; border-radius:15px;"
            video1.Src = twentySec;
            
        }
        protected void Button2_Click(object sender, EventArgs e)
        {

            DoAuth();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "plusSlides", "plusSlides(0);", true);

        }
    }

}