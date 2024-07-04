using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;

namespace SahoAcs.Web._04._0413
{
    public partial class PsnCardList : System.Web.UI.Page
    {
        public List<DBModel.CardEntity> ListPerson = new List<CardEntity>();
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ListPerson = this.odo.GetQueryResult<CardEntity>(
                "SELECT TOP 250 * FROM V_PsnCard WHERE PsnNo LIKE @PsnNo OR PsnName LIKE @PsnNo ORDER BY PsnNo", new { PsnNo = Request["PsnNo"]+"%" }).ToList();
        }
    }
}