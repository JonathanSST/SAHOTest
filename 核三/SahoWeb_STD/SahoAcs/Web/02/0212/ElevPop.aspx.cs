using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;

namespace SahoAcs.Web._02._0212
{
    public partial class ElevPop : System.Web.UI.Page
    {
        public List<FloorAccess> FloorList = new List<FloorAccess>();
        public string FloorBinValue="";
        string FloorName = "";
        string ParaValue = "";
        public string ParaName = "";
        public string ParaDesc = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            FloorName = Request["FloorName"].ToString();
            ParaValue = Request["ParaValue"].ToString();
            ParaName = Request["ParaName"].ToString();
            string[] floor = FloorName.Split(',');
            foreach(var f in floor)
            {
                if (f.Split(':').Count() > 1)
                {
                    this.FloorList.Add(new FloorAccess()
                    {
                        IoIndex=f.Split(':')[0],
                        FloorName=f.Split(':')[1]
                    });
                }
            }
            this.FloorBinValue = Sa.Change.HexToBin(ParaValue, 48);          
        }

    }//end class

}//end namespace