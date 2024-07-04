using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Script.Serialization;

using DapperDataObjectLib;
using SahoAcs.DBModel;



namespace SahoAcs.Web
{
    public partial class AntiPassList : System.Web.UI.Page
    {
        public List<EquVms> EquVmsList = new List<EquVms>();
        public List<EquVms> AllEquList = new List<EquVms>();

        OrmDataObject od = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] == null)
            {
                this.SetEquVmsList();
            }
            
            using (var reader = new System.IO.StreamReader(Request.InputStream))
            {                
                string json = reader.ReadToEnd();
                var serializer = new JavaScriptSerializer();
                //var result = serializer.DeserializeObject(json);             
                dynamic resp = serializer.DeserializeObject(json);
                if (resp != null)
                {
                    string PageEvent = Convert.ToString(resp["PageEvent"]);
                    if (PageEvent == "Save")
                    {
                        this.SetSaveVmsList(resp);                        
                    }
                    //string result_json = Convert.ToString(result);
                }                
            }

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            //js += "\nSetMode('');";            
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("AntiPassList", "AntiPassList.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
        }




        private void SetSaveVmsList(dynamic paradic)
        {
            List<EquVms> InputEquVmsPara = new List<EquVms>();
            foreach (var s in paradic["EquVmsList"])
            {
                InputEquVmsPara.Add(new EquVms()
                {
                    EquNo = Convert.ToString(s["EquNo"])
                });
            }
            var ParaValue=string.Join(",", InputEquVmsPara.Select(i=>i.EquNo));            
            if (this.od.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='AntiPassList' ").Count() > 0)
            {
                this.od.Execute("UPDATE B00_SysParameter SET ParaValue=@ParaValue,UpdateUserID=@User,UpdateTime=GETDATE() WHERE ParaNo=@ParaNo", 
                    new { ParaValue = ParaValue,ParaNo="AntiPassList",User="Saho" });
            }
            else
            {
                this.od.Execute(@"INSERT INTO B00_SysParameter 
                        (ParaNo,ParaValue,ParaClass,ParaName,ParaType,CreateUserID,CreateTime,UpdateUserID,UpdateTime) 
                        VALUES (@ParaNo,@ParaValue,'HideSystem',@ParaName,'String',@User,GETDATE(),@User,GETDATE())",
                        new {ParaValue=ParaValue,ParaNo= "AntiPassList", User="Saho",ParaName= "Anti Pass 設備清單" });
            }            
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK",success=true }));
            Response.End();
        }


        private void SetEquVmsList()
        {
            //this.EquVmsList = this.od.GetQueryResult<EquVms>("SELECT * FROM B01_EquData").ToList();
            var paras = this.od.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='AntiPassList'");
            Dictionary<string, string> vms_dic = new Dictionary<string, string>();
            foreach (var s in paras)
            {
                vms_dic.Add(Convert.ToString(s.ParaNo), Convert.ToString(s.ParaValue));
                foreach (string EquNo in vms_dic["AntiPassList"].Split(','))
                {
                    this.EquVmsList.Add(new EquVms()
                    {
                        EquNo = EquNo
                    });
                }
            }
            this.AllEquList = this.od.GetQueryResult<EquVms>("SELECT * FROM B01_EquData").ToList();
        }

    }//end class
}//end namespace