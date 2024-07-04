using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;


namespace SahoAcs.Unittest
{
    public partial class FormDataInput : System.Web.UI.Page
    {
        

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null && Request["PageEvent"].Equals("Save"))
            {
                this.SetFileUpload();
            }
        }

        private void SetFileUpload()
        {            
            string form_data = "";
            string form_value = "";
            for (int cnt = 0; cnt < Request.Form.Count; cnt++)
            {
                form_data += Request.Form.Keys[cnt].ToString();
                form_value += Request.Form[Request.Form.Keys[cnt].ToString()].ToString();
                form_value += Request[Request.Form.Keys[cnt].ToString()].ToString();
            }
            for (int cnt = 0; cnt < Request.QueryString.Count; cnt++)
            {
                form_data += Request.QueryString.Keys[cnt].ToString();
            }
            string end_data = form_data + "" + form_value;
        }

    }//end class
}//end namespace