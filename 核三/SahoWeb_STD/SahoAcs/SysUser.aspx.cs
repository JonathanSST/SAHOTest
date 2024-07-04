using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using DapperDataObjectLib;

namespace SahoAcs
{
    public partial class SysUser : System.Web.UI.Page
    {
        public DataTable DataResult = new DataTable();

        OrmDataObject orm = new OrmDataObject("MsSql",
            string.Format(Pub.db_connection_template, Pub.db_source_ip, Pub.db_data_name, Pub.db_user_id, Pub.db_pwd));

        protected void Page_Load(object sender, EventArgs e)
        {
            this.DataResult = orm.GetDataTableBySql("SELECT * FROM B00_SysUser");
        }
    }
}