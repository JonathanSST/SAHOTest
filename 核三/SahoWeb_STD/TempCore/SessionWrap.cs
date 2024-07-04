using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace TempCore
{
    public class SessionWrapper
    {
        public static ErpUser ErpUser
        {
            get
            {
                if (HttpContext.Current.Session["ErpUser"] != null)
                {
                    return HttpContext.Current.Session["ErpUser"] as ErpUser;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                HttpContext.Current.Session["ErpUser"] = value;
            }
        }       
    }//end class
}//end namespace
