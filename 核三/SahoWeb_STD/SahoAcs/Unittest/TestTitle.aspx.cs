using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;

namespace SahoAcs.Unittest
{
    public partial class TestTitle : System.Web.UI.Page
    {
        public string titleText = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["Customer"] != null)
            {
                string name = Sa.Fun.Encrypt(string.Format("Sms|{0}",Request["Customer"]));
                Response.Write(name);
                name = Sa.Fun.Encrypt(string.Format("{0}", Request["Customer"]));
                Response.Write("<br/>" + name);
            }
            #region 客戶名稱處理
            string[] aStr;
            string sStr, sTmp="";
            string sCustomer = "內部專用-禁止外流";
            string current_path = System.AppDomain.CurrentDomain.BaseDirectory;
            string[] aFiles = Directory.GetFiles(current_path+"\\unittest","*.sms", SearchOption.TopDirectoryOnly);
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            Response.Write("<br/>"+Encoding.Default.EncodingName);
            if (aFiles.Length > 0)
            {
                sTmp = File.ReadAllText(aFiles[0]);
                if (sTmp != "")
                {
                    try
                    {
                        sStr = Sa.Fun.Decrypt(sTmp);
                    }
                    catch (Exception ex)
                    {
                        sStr = ex.Message;
                    }
                                        
                    if (!string.IsNullOrEmpty(sStr))
                    {
                        aStr = sStr.Split(new char[] { '|' });
                        if (aStr.Length >= 2 && aStr[0] == "Sms")
                        {
                            sCustomer = aStr[1];                            
                            oAcsDB.GetSysParameter("Customer", "Name", "客戶名稱", "", "", out sStr);
                            if (sStr != sTmp)
                            {
                                oAcsDB.UpdateSysParameter("Customer", "Name",sTmp);
                            }
                        }
                    }
                }
            }
            else
            {
                oAcsDB.GetSysParameter("Customer", "Name", "客戶名稱", "", "", out sTmp);
                if (sTmp != "")
                {
                    sStr = Sa.Fun.Decrypt(sTmp);
                    if (!string.IsNullOrEmpty(sStr))
                    {
                        aStr = sStr.Split(new char[] { '|' });
                        if (aStr.Length >= 2 && aStr[0] == "Sms")
                        {
                            sCustomer = aStr[1];
                        }                            
                    }
                }
            }
            this.titleText = "商合行SMS管理系統(" + sCustomer + ") Ver:";
            #endregion

        }//end method
    }//end class
}//end namespace