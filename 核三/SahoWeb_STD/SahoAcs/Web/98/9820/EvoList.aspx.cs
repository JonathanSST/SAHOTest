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
    public partial class EvoList : System.Web.UI.Page
    {
        public List<EquEvo> EquEvoList = new List<EquEvo>();

        public string EvoHost = "";
        public int EvoPre = 0;
        public string EvoPwd = "";
        public string EvoUid = "";

        OrmDataObject od = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] == null)
            {
                this.SetEquEvoList();
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
                        this.SetSaveEvoList(resp);                        
                    }
                    //string result_json = Convert.ToString(result);
                }                
            }

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            //js += "\nSetMode('');";            
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("EvoList", "EvoList.js?"+Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
        }




        private void SetSaveEvoList(dynamic paradic)
        {
            List<EquEvo> InputEquEvoPara = new List<EquEvo>();
            foreach (var s in paradic["EquEvoList"])
            {
                InputEquEvoPara.Add(new EquEvo()
                {
                    EquNo = Convert.ToString(s["EquNo"]),
                    EvoName = Convert.ToString(s["EvoName"])
                });
            }
            var ParaValue=string.Join(";", InputEquEvoPara.Select(i=>i.EquNo+"|"+i.EvoName));            
            if (this.od.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='EvoList' ").Count() > 0)
            {
                this.od.Execute("UPDATE B00_SysParameter SET ParaValue=@ParaValue,UpdateUserID=@User,UpdateTime=GETDATE() WHERE ParaNo=@ParaNo", 
                    new { ParaValue = ParaValue,ParaNo="EvoList",User="Saho" });
            }
            else
            {
                this.od.Execute(@"INSERT INTO B00_SysParameter 
                        (ParaNo,ParaValue,ParaClass,ParaName,ParaType,CreateUserID,CreateTime,UpdateUserID,UpdateTime) 
                        VALUES (@ParaNo,@ParaValue,'HideSystem',@ParaName,'String',@User,GETDATE(),@User,GETDATE())",
                        new {ParaValue=ParaValue,ParaNo="EvoList",User="Saho",ParaName= "EVO攝影機清單列表" });
            }
            string[] para_array = { "EvoUid", "EvoPwd", "EvoHost" };
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


        private void SetEquEvoList()
        {
            this.EquEvoList = this.od.GetQueryResult<EquEvo>("SELECT * FROM B01_EquData").ToList();
            var paras = this.od.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo IN ('EvoList','EvoHost','EvoPwd','EvoUid')");
            Dictionary<string, string> evo_dic = new Dictionary<string, string>();
            foreach (var s in paras)
            {
                evo_dic.Add(Convert.ToString(s.ParaNo), Convert.ToString(s.ParaValue));
            }
            if (evo_dic.Keys.Where(i=>i=="EvoList").Count()>0)
            {
                foreach (string s in evo_dic["EvoList"].Split(';'))
                {
                    string[] arr_EquEvo = s.Split('|');
                    if(arr_EquEvo.Length>1&& this.EquEvoList.Where(i => i.EquNo == arr_EquEvo[0]).Count()>0)
                        this.EquEvoList.Where(i => i.EquNo == arr_EquEvo[0]).FirstOrDefault().EvoName = arr_EquEvo[1];
                }
            }
            if (evo_dic.Keys.Where(i => i == "EvoHost").Count() > 0)
            {
                this.EvoHost = evo_dic["EvoHost"];
            }
            if (evo_dic.Keys.Where(i => i == "EvoPwd").Count() > 0)
            {
                this.EvoPwd = evo_dic["EvoPwd"];
            }
            if (evo_dic.Keys.Where(i => i == "EvoUid").Count() > 0)
            {
                this.EvoUid = evo_dic["EvoUid"];
            }
        }

    }//end class
}//end namespace