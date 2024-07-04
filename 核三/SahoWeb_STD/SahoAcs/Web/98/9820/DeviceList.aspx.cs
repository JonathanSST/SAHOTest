using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using Luxriot.Internal.LibCommon;
using System.Web.Script.Serialization;
using DapperDataObjectLib;



namespace SahoAcs.Web._98._9820
{
    public partial class DeviceList : System.Web.UI.Page
    {

        public List<string> AllResourceList = new List<string>();


        protected void Page_Load(object sender, EventArgs e)
        {
            OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
            var paradata = odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='EvoNoList' ");
            foreach(var o in paradata)
            {
                string paravalue = Convert.ToString(o.ParaValue);
                foreach(var v in paravalue.Split(','))
                {
                    AllResourceList.Add(v);
                }
            }

            if (Request["PageEvent"]!=null && Request["PageEvent"] == "Refresh")
            {
                string uid = Request["admin"];
                string password = Request["password"];
                string host = Request["host"];
                string ErrorMessage = "";
                DigestWebRequest dwr = new DigestWebRequest(uid, password);
                
                System.Net.HttpWebResponse myWebResponse = null;
                Uri myUri = new Uri(host + "monitor/channels?once=1");
                try
                {
                    myWebResponse = (HttpWebResponse)dwr.GetResponse(myUri);

                    Stream responseStream = myWebResponse.GetResponseStream();
                    StreamReader myStreamReader = new StreamReader(responseStream, System.Text.Encoding.Default);
                    string pageContent = myStreamReader.ReadToEnd();
                    //Response.Write(pageContent);
                    var serializer = new JavaScriptSerializer();
                    //var result = serializer.DeserializeObject(json);
                    dynamic resp = serializer.DeserializeObject(pageContent);
                    List<string> ResourceList = new List<string>();
                    foreach (var resp_obj in resp)
                    {
                        ResourceList.Add(Convert.ToString(resp_obj["updated"]["resourceId"]));
                    }

                    if (ResourceList.Count > 0)
                    {
                        var ParaValue = string.Join(",", ResourceList);
                        if (odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='EvoNoList' ").Count() > 0)
                        {
                            odo.Execute("UPDATE B00_SysParameter SET ParaValue=@ParaValue,UpdateUserID=@User,UpdateTime=GETDATE() WHERE ParaNo=@ParaNo",
                                new { ParaValue = ParaValue, ParaNo = "EvoNoList", User = "Saho" });
                        }
                        else
                        {
                            odo.Execute(@"INSERT INTO B00_SysParameter 
                                            (ParaNo,ParaValue,ParaClass,ParaName,ParaType,CreateUserID,CreateTime,UpdateUserID,UpdateTime) 
                                            VALUES (@ParaNo,@ParaValue,'HideSystem',@ParaName,'String',@User,GETDATE(),@User,GETDATE())",
                                            new { ParaValue = ParaValue, ParaNo = "EvoNoList", User = "Saho", ParaName = "EVO攝影機表" });
                        }
                        this.AllResourceList.AddRange(ResourceList);
                        this.AllResourceList = this.AllResourceList.Distinct().ToList();
                    }
                    responseStream.Close();
                }
                catch (WebException ex)
                {
                    ErrorMessage = ex.Message;
                }                               
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                    new { message = "OK", resource=this.AllResourceList, err_message=ErrorMessage }));
                Response.End();

            }
        }




    }//end page class
}//end namespace