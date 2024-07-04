using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using PagedList;


namespace SahoAcs.Web._06._0608
{
    public partial class UnitTest : System.Web.UI.Page
    {
        OrmDataObject ormDB = new OrmDataObject("MsSql",
                string.Format(Pub.db_connection_template, Pub.db_source_ip, Pub.db_data_name, Pub.db_user_id, Pub.db_pwd));
        string sql = "", wheresql = "";
        public IPagedList<PsnEquData> person_equ_list;

        protected void Page_Load(object sender, EventArgs e)
        {
            var sql1 = @" SELECT DISTINCT 
                     Building,Floor,EquClass,EquNo,EquName, Person.* FROM dbo.B01_Person AS Person
                     INNER JOIN dbo.B01_Card AS Card ON Card.PsnID = Person.PsnID
                     INNER JOIN dbo.B01_CardAuth AS CardAuth ON CardAuth.CardNo = Card.CardNo
                     INNER JOIN dbo.B01_EquData AS EquData ON EquData.EquID = CardAuth.EquID 
                     INNER JOIN B01_OrgStruc AS OrgStruc ON OrgStruc.OrgStrucID = Person.OrgStrucID 
                        WHERE OrgStruc.OrgStrucID IN @Strucs";
            var sql2 = @"SELECT MgnOrgStrucs.MgaID,OrgStrucID 
                     FROM B01_MgnOrgStrucs AS MgnOrgStrucs
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnOrgStrucs.MgaID
                     INNER JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID WHERE SysUserMgns.UserID=@UserID";
            List<MgaOrgStruc> mga_org_list = ormDB.GetQueryResult<MgaOrgStruc>(sql2, new { UserID = "Saho" }).ToList();
            if (Request["PageEvent"] != null)
            {
                int page_number = Convert.ToInt32(Request["PageNumber"]);
                var strucs = mga_org_list.Select(i => i.OrgStrucID).ToList();
                this.person_equ_list = ormDB.GetQueryResult<PsnEquData>(sql1, new { Strucs = strucs }).ToPagedList(page_number, 100); 
            }
            else
            {
                var strucs = mga_org_list.Select(i => i.OrgStrucID).ToList();
                this.person_equ_list = ormDB.GetQueryResult<PsnEquData>(sql1, new { Strucs = strucs }).ToPagedList(1, 100); 
            }
            
        }





        public class PsnEquData
        {
            public string PsnNo { get; set; }
            public string PsnName { get; set; }
            public string Building { get; set; }
            public string Floor { get; set; }
            public string EquClass { get; set; }
            public string EquNo { get; set; }
            public string EquName { get; set; }
        }

        public class MgaOrgStruc
        {
            public int MgaID { get; set; }
            public int OrgStrucID { get; set; }
        }
    }//end class
}//end namespace