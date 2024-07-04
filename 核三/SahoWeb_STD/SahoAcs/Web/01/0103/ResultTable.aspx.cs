using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using PagedList;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using SahoAcs.DBClass;
using Sa.DB;

namespace SahoAcs.Web._01._0103
{
    public partial class ResultTable : Sa.BasePage
    {
        public DataTable DataResult = new DataTable();

        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public IPagedList<PersonEntity> PersonPage;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            this.SetQuery();            
        }

        private void SetQuery()
        {
            string Comp = "", QueryName = "";
            string Dept = "", PsnType = "";
            if (Request.Form["Dept"] != null)
            {
                Dept = Request.Form["Dept"];
            }
            if (Request.Form["Comp"] != null)
            {
                Comp = Request.Form["Comp"];
            }
            if (Request.Form["PsnType"] != null)
            {
                PsnType = Request.Form["PsnType"];
            }
            int PageIndex = 1;
            if (Request["PageIndex"] != null)
            {
                PageIndex = int.Parse(Request["PageIndex"]);
            }
            string SqlCmd = @"SELECT DISTINCT(B01_Person.PsnID), B01_Person.PsnNo, B01_Person.PsnName, B01_Person.PsnEName,
                B01_Person.PsnType, B01_Person.IDNum, B01_Person.Birthday, B01_Person.OrgStrucID, B01_Person.PsnAccount,
                B01_Person.PsnPW, B01_Person.PsnAuthAllow, B01_Person.PsnSTime, B01_Person.PsnETime, B01_Person.PsnPicSource,
                B01_Person.Remark, B01_Person.CreateUserID, B01_Person.CreateTime, B01_Person.UpdateUserID,
                B01_Person.UpdateTime, B01_Person.Rev01, B01_Person.Rev02, OrgStrucAllData_1.OrgStrucNo,
                OrgStrucAllData_1.OrgNameList, OrgStrucAllData_1.OrgNoList FROM B01_Person 
                INNER JOIN OrgStrucAllData('') AS OrgStrucAllData_1 ON B01_Person.OrgStrucID = OrgStrucAllData_1.OrgStrucID
                INNER JOIN B01_MgnOrgStrucs ON OrgStrucAllData_1.OrgStrucID=B01_MgnOrgStrucs.OrgStrucID
                INNER JOIN B00_SysUserMgns ON B00_SysUserMgns.MgaID=B01_MgnOrgStrucs.MgaID
                LEFT OUTER JOIN B01_Card ON B01_Person.PsnID = B01_Card.PsnID
				WHERE UserID=@UserID ";
            if (!string.IsNullOrEmpty(Dept) && !string.IsNullOrEmpty(Comp))
            {
                SqlCmd += string.Format(@" AND (OrgIDList LIKE '\{0}\%' AND OrgIDList LIKE '%\{1}\%' )", Comp.Replace("'","") , Dept.Replace("'",""));                
            }
            else if(!string.IsNullOrEmpty(Dept) || !string.IsNullOrEmpty(Comp))
            {
                SqlCmd += string.Format(@" AND (OrgIDList LIKE '%\{0}\%' ) ", Comp.Equals("")?Dept.Replace("'",""):Comp.Replace("'",""));
            }
            if (!string.IsNullOrEmpty(PsnType))
            {
                SqlCmd += " AND PsnType=@PsnType";
            }
            if (Request.Form["QueryName"] != null)
            {
                QueryName = Request.Form["QueryName"];
                SqlCmd += " AND (PsnNo LIKE @Key OR PsnName LIKE @Key OR CardNo=@Key1 OR IDNum=@Key1) ";
            }
            this.PersonPage = this.odo.GetQueryResult<PersonEntity>(SqlCmd, new
            {
                Key=QueryName+"%", Key1=QueryName,
                Comp =!string.IsNullOrEmpty(Comp)?Comp:Dept, Dept,
                UserID =Sa.Web.Fun.GetSessionStr(this,"UserID"),
                PsnType = this.GetFormEndValue("PsnType")
            }).OrderByField("PsnName",true).ToPagedList(PageIndex,20);
            this.DataResult = OrmDataObject.IEnumerableToTable(this.PersonPage);
        }



    }
}