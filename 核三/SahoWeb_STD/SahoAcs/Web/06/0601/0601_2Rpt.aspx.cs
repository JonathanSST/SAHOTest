using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using SahoAcs.DBClass;
using System.Data;
using PagedList;
using OfficeOpenXml;
using System.IO;
using System.Text;


namespace SahoAcs.Web._06._0601
{
    public partial class _0601_2Rpt : System.Web.UI.Page
    {
        public List<CardLogModel> ListLog = new List<CardLogModel>();
        public OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public PagedList<CardLogModel> RptPages;

        public List<string> EquDatas = new List<string>();
        public List<string> OrgStrucDatas = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AdvData"] != null)
            {
                this.ListLog = (List<CardLogModel>)Session["AdvData"];
            }
            else
            {
                this.SetQueryData();
            }
            
        }

        private void SetQueryData()
        {           
            List<ColNameObj> collist = new List<ColNameObj>();                        
            string sql = "";
            string sqlorgjoin = @"SELECT B01_OrgStruc.OrgStrucID,OrgStrucNo,OrgIDList
                                  FROM B01_OrgStruc                
                                  INNER JOIN B01_MgnOrgStrucs ON B01_OrgStruc.OrgStrucID=B01_MgnOrgStrucs.OrgStrucID
	                              INNER JOIN B00_SysUserMgns ON B00_SysUserMgns.MgaID = B01_MgnOrgStrucs.MgaID
                                   WHERE B00_SysUserMgns.UserID = @UserID ";
            if (Sa.Web.Fun.GetSessionStr(this.Page, "UserID") != "")
            {
                this.OrgStrucDatas = this.odo.GetQueryResult<OrgStrucInfo>(sqlorgjoin, new { UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID") }).Select(i => i.OrgIDList).ToList();
            }

            string filterType = Request["Type"];

            //在中科一所的環境，須可以用身份證號查詢自己的讀卡記錄
            sql = @"SELECT A.*,IDNum FROM B01_CardLog A INNER JOIN B01_Person B ON A.PsnNo=B.PsnNo WHERE ";


            switch (filterType)
            {
                case "1":
                    sql += " 1=1 ";
                    break;
                case "2":
                    sql += "(EquClass='TRT' OR IsAndTrt=1) ";
                    break;
                case "3":
                    sql += "(EquClass='Door Access' and IsAndTrt=0) ";
                    break;
                default:
                    break;
            }
            var logstatus = this.odo.GetQueryResult("SELECT * FROM B00_CardLogState");
            sql += this.GetConditionCmdStr();
            this.ListLog = this.odo.GetQueryResult<CardLogModel>(sql, new
            {
                OrgStrucDatas = OrgStrucDatas,
                PsnName = Request["CardNoPsnNo"].ToString(),
                CardTimeS = Request["CardTimeS"],
                CardTimeE = Request["CardTimeE"],
                                
            }).OrderByField("CardTime",true).ToList();
            this.ListLog.ForEach(i => i.LogStatus = logstatus.Where(j => Convert.ToInt32(j.Code) == int.Parse(i.LogStatus)).First().StateDesc);
        }

        private string GetConditionCmdStr()
        {
            string sql = "";
            string CardNo_PsnName = Request["CardNoPsnNo"];
            
            //一般查詢的方法

            sql += " AND (CardTime BETWEEN @CardTimeS AND @CardTimeE) ";

            

            if (CardNo_PsnName != "")
            {
                sql += " AND (IDNum=@PsnName) ";
            }            
            return sql;
        }

    }//end page class
}//end namespace