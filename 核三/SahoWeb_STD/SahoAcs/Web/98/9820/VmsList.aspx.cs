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



namespace SahoAcs.Web._98._9820
{
    public partial class VmsList : System.Web.UI.Page
    {
        public List<EquVms> EquVmsList = new List<EquVms>();

        public string VmsHost = "";
        public int VmsPre = 0;
        public int VmsNext = 0;

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
            ClientScript.RegisterClientScriptInclude("VmsList", "VmsList.js");//加入同一頁面所需的JavaScript檔案
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
                    EquNo = Convert.ToString(s["EquNo"]),
                    VmsName = Convert.ToString(s["VmsName"])
                });
            }
            var ParaValue=string.Join(";", InputEquVmsPara.Select(i=>i.EquNo+"|"+i.VmsName));            
            if (this.od.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='VmsList' ").Count() > 0)
            {
                this.od.Execute("UPDATE B00_SysParameter SET ParaValue=@ParaValue,UpdateUserID=@User,UpdateTime=GETDATE() WHERE ParaNo=@ParaNo", 
                    new { ParaValue = ParaValue,ParaNo="VmsList",User="Saho" });
            }
            else
            {
                this.od.Execute(@"INSERT INTO B00_SysParameter 
                        (ParaNo,ParaValue,ParaClass,ParaName,ParaType,CreateUserID,CreateTime,UpdateUserID,UpdateTime) 
                        VALUES (@ParaNo,@ParaValue,'HideSystem',@ParaName,'String',@User,GETDATE(),@User,GETDATE())",
                        new {ParaValue=ParaValue,ParaNo="VmsList",User="Saho",ParaName= "VMS攝影機清單列表" });
            }
            string[] para_array = { "VmsNext", "VmsPre", "VmsHost" };
            foreach(var para_key in para_array)
            {
                if (this.od.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo=@ParaKey ", new {ParaKey= para_key }).Count() > 0)
                {
                    this.od.Execute("UPDATE B00_SysParameter SET ParaValue=@ParaValue,UpdateUserID=@User,UpdateTime=GETDATE() WHERE ParaNo=@ParaNo",
                        new { ParaValue = paradic[para_key], ParaNo = para_key, User = "Saho" });
                }
                else
                {
                    this.od.Execute(@"INSERT INTO B00_SysParameter 
                        (ParaNo,ParaValue,ParaClass,ParaName,ParaType,CreateUserID,CreateTime,UpdateUserID,UpdateTime) 
                        VALUES (@ParaNo,@ParaValue,'HideSystem',@ParaName,'String',@User,GETDATE(),@User,GETDATE())",
                            new { ParaValue = paradic[para_key], ParaNo = para_key, User = "Saho", ParaName = para_key+"的參數" });
                }
            }            
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK",success=true }));
            Response.End();
        }


        private void SetEquVmsList()
        {
            this.EquVmsList = this.od.GetQueryResult<EquVms>("SELECT * FROM B01_EquData").ToList();
            var paras = this.od.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo IN ('VmsList','VmsHost','VmsPre','VmsNext')");
            Dictionary<string, string> vms_dic = new Dictionary<string, string>();
            foreach (var s in paras)
            {
                vms_dic.Add(Convert.ToString(s.ParaNo), Convert.ToString(s.ParaValue));
            }
            if (vms_dic.Keys.Where(i=>i=="VmsList").Count()>0)
            {
                foreach (string s in vms_dic["VmsList"].Split(';'))
                {
                    string[] arr_EquVms = s.Split('|');
                    if(arr_EquVms.Length>1&& this.EquVmsList.Where(i => i.EquNo == arr_EquVms[0]).Count()>0)
                        this.EquVmsList.Where(i => i.EquNo == arr_EquVms[0]).FirstOrDefault().VmsName = arr_EquVms[1];
                }
            }
            if (vms_dic.Keys.Where(i => i == "VmsHost").Count() > 0)
            {
                this.VmsHost = vms_dic["VmsHost"];
            }
            if (vms_dic.Keys.Where(i => i == "VmsPre").Count() > 0)
            {
                this.VmsPre = int.Parse(vms_dic["VmsPre"]);
            }
            if (vms_dic.Keys.Where(i => i == "VmsNext").Count() > 0)
            {
                this.VmsNext = int.Parse(vms_dic["VmsNext"]);
            }
        }

    }//end class
}//end namespace