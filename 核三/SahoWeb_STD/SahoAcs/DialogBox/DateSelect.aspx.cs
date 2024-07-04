using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.DialogBox
{
    public partial class DateSelect : System.Web.UI.Page
    {
        public string DateNow = DateTime.Now.ToString("yyyy/MM/dd");
        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.RegisterClientScriptInclude("DateTimeSelect", Request.ApplicationPath.TrimEnd('/') + "/DialogBox/DateSelect.js");//加入同一頁面所需的JavaScript檔案                        
            if (Request.Form["para_date"] != null)
            {
                this.DateNow = Request.Form["para_date"];
            }            
            if (!IsPostBack)
            {
                DDListY.Items.Clear();
                DDListM.Items.Clear();
                for (Int32 i = 1940; i < 2101; i++) 
                    DDListY.Items.Add(Sa.Change.Ntoc(i, 4));
                for (Int32 i = 1; i < 13; i++) 
                    DDListM.Items.Add(Sa.Change.Ntoc(i, 2));                
            }

            string js = Sa.Web.Fun.ControlToJavaScript(this);//將JavaScript會使用到的物件變數先指定到控制項上            
            if ((!IsPostBack))
            {
                js += "\nPage_Init();";
            }                
            js = "<script type='text/javascript'>" + js + "</script>";
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("xFun", Request.ApplicationPath.TrimEnd('/')+ "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", Request.ApplicationPath.TrimEnd('/')+ "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", Request.ApplicationPath.TrimEnd('/')+ "/Scripts/Common.js");
            //ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");            

            #region 更改元件Style
            ChangeStyle(LaToDay);            
            ChangeStyle(LaClean);
            #endregion

            #region 註冊元件動作
            //DDListY.Attributes["onChange"] = "DateOnChange();";
            //DDListM.Attributes["onChange"] = "DateOnChange();";            

            //LaToDay.Attributes["onClick"] = "Call_ToDay(); return false;";            
            //LaClean.Attributes["onClick"] = "Call_Clean(); return false;";

            //ImgPrevious.Attributes["onClick"] = "Call_Previous(); return false;";
            //ImgNext.Attributes["onClick"] = "Call_Next(); return false;";
            //BSubmit.Attributes["onClick"] = "Call_Yes(); return false;";
            //BCancel.Attributes["onClick"] = "Call_Cancel(); return false;";
            #endregion
        }

        protected void ChangeStyle(Label item)
        {
            item.Attributes.Add("OnMouseMove", "this.style.color='#1E75BB';");
            item.Attributes.Add("OnMouseOut", "this.style.color='#000000';");
            item.Style.Add("cursor", "pointer;");
        }

    }//end class 
}//end namespace