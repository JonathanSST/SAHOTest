using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SahoAcs.uc;
using Sa.DB;
using System.Xml;

namespace SahoAcs
{
    public partial class Empty : System.Web.UI.Page
    {

        private void Page_Load(object sender, System.EventArgs e)
        {
            string str = "abc3353,iejlk332";
            string[] TempArray;
            List<string> PWArray = new List<string>();
            TempArray = str.Split(',');
            for (int i = 0; i < TempArray.Length; i++)
            {
                PWArray.Add(TempArray[i].ToString());
            }
            PWArray.Add("...");

            for (int i = 0; i < PWArray.Count; i++)
            {
                if (string.Compare(PWArray[i], "iejlk332") == 0)
                {
                    Response.Write("相同");
                    break;
                }
                else
                    Response.Write("不相同");
            }

            

        }



    }
}
