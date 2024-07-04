using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web._97._9706
{
    public partial class OpenActiVideo : System.Web.UI.Page
    {
        public string MyUrl = "", ServerLocate = "", ErrMsg = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["RtspList"] == null)
            {
                ErrMsg = "驗證碼遺失，請重新整理攝影機檢視頁面";
            }
            else
            {
                if (Request.QueryString["TargetCode"] != null)
                {
                    var target = Request.QueryString["TargetCode"];
                    var RtspList = Session["RtspList"] as List<SahoAcs.DBModel.RtspEntity>;
                    if (RtspList.Where(i => i.ChkTarget.Equals(target)).Count() == 0)
                    {
                        ErrMsg = "授權碼無效，畫面即將關閉";
                    }
                }
                else
                {
                    ErrMsg = "請求影像必須包含驗證碼";
                }
            }
            
            if (Request.QueryString["RtspVideo"] != null)
            {
                MyUrl = Request.QueryString["RtspVideo"].ToString();
            }
            if (Request.QueryString["ServerLocate"] != null)
            {
                ServerLocate = Request.QueryString["ServerLocate"].ToString();
            }
        }
    }
}