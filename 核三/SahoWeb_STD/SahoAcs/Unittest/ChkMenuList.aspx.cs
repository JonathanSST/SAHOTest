using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SahoAcs.DBClass;
using DapperDataObjectLib;


namespace SahoAcs.Unittest
{
    public partial class ChkMenuList : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public class MenuInfo
        {
            public string MenuNo { get; set; }
            public string MenuName { get; set; }
            public string UpMenuNo { get; set; }
        }

        public List<MenuInfo> ListMenus = new List<MenuInfo>();

        protected void Page_Load(object sender, EventArgs e)
        {
            string User = "Saho";
            if (Request["UserID"] != null)
            {
                User = Request["UserID"].ToString();
            }
            this.ListMenus = this.odo.GetQueryResult<MenuInfo>("SELECT * FROM B00_SysMenu WHERE MenuType='F' ORDER BY MenuOrder ").ToList();
            var MenuAuth = User.GetMenuList();
            this.ListMenus = this.ListMenus.Where(i => MenuAuth.Contains(i.MenuNo)).ToList();
        }
    }//end page class
}//end namespace