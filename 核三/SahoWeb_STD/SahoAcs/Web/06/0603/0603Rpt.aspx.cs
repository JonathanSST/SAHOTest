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

namespace SahoAcs.Web._06._0603
{
    public partial class _0603Rpt : System.Web.UI.Page
    {
        public List<CardLogModel> ListLog = new List<CardLogModel>();
        public OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public PagedList<CardLogModel> RptPages;

        public List<string> EquDatas = new List<string>();
        public List<CardLogModel> OrgStrucDatas = new List<CardLogModel>();

        protected void Page_Load(object sender, EventArgs e)
        {
            this.SetQueryData();
        }


        private void SetQueryData()
        {            
            List<ColNameObj> collist = new List<ColNameObj>();                        
            string sql = "";
            string sqlorgjoin = @"SELECT 
	                                                        PsnNo,IDNum
                                                        FROM                                                             
	                                                        B01_MgnOrgStrucs 
                                                            INNER JOIN B01_Person ON B01_MgnOrgStrucs.OrgStrucID=B01_Person.OrgStrucID
	                                                        INNER JOIN B00_SysUserMgns ON B00_SysUserMgns.MgaID = B01_MgnOrgStrucs.MgaID
                                                        WHERE B00_SysUserMgns.UserID = @UserID";
            if (Sa.Web.Fun.GetSessionStr(this.Page, "UserID") != "")
            {
                this.OrgStrucDatas = this.odo.GetQueryResult<CardLogModel>(sqlorgjoin, new { UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID") }).ToList();
            }            
            //在中科一所的環境，須可以用身份證號查詢自己的讀卡記錄
            sql = @"SELECT A.*,IDNum FROM B01_CardLog A INNER JOIN B01_Person B ON A.PsnNo=B.PsnNo WHERE CardTime BETWEEN @CardTimeS AND @CardTimeE ";
            if (Request["EquClass"] != null)
            {
                if (Request["EquClass"] == "TRT")
                {
                    sql += " AND (EquClass='TRT' OR IsAndTrt=1) ";
                }
                if (Request["EquClass"] == "Door")
                {
                    sql += " AND (EquClass='Door Access' AND IsAndTrt=0) ";
                }
            }
            var logstatus = this.odo.GetQueryResult("SELECT * FROM B00_CardLogState");
            var psndata = this.OrgStrucDatas.Select(i => i.PsnNo).ToList();
            sql += this.GetConditionCmdStr();
            this.ListLog = this.odo.GetQueryResult<CardLogModel>(sql, new
            {
                //OrgStrucDatas = OrgStrucDatas,
                PsnName = "%" + Request["CardNoPsnNo"].ToString() + "%",
                CardTimeS = Request["CardTimeS"],
                CardTimeE = Request["CardTimeE"],
                IDNum = Request["CardNoPsnNo"]
            }).OrderByField("CardTime",true).ToList();
            this.ListLog = this.ListLog.Where(i => psndata.Contains(i.PsnNo)).ToList();
            this.ListLog.ForEach(i => i.LogStatus = logstatus.Where(j => Convert.ToInt32(j.Code) == int.Parse(i.LogStatus)).First().StateDesc);
        }

        private string GetConditionCmdStr()
        {
            string sql = "";
            string CardNo_PsnName = Request["CardNoPsnNo"];
            //一般查詢的方法
            //sql += " AND (CardTime BETWEEN @CardTimeS AND @CardTimeE) ";
            if (CardNo_PsnName != "")
            {
                sql += " AND (A.PsnNo LIKE @PsnName OR A.PsnName LIKE @PsnName OR CardNo LIKE @PsnName OR IDNum=@IDNum) ";
            }            
            return sql;
        }

    }//end page class
}//end namespace