using DapperDataObjectLib;
using OfficeOpenXml;
using PagedList;
using Sa.DB;
using SahoAcs.DBClass;
using SahoAcs.DBModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace SahoAcs.Web._07._0702
{
    public partial class MealOrderRecordInq : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public string SortName = "OrderTime";
        public string SortType = "ASC";
        public DataTable DataResult = new DataTable();
        public List<PersonEntity> PersonList = new List<PersonEntity>();
        public List<OrgDataEntity> OrgDataInit = new List<OrgDataEntity>();
        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
        public List<B03_MealOrder> ListLog = new List<B03_MealOrder>();

        public int PageIndex = 1;
        public string PsnNo = "", PsnID = "", PsnName = "";
        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string AuthList = "";
        public IPagedList<B03_MealOrder> PagedList;
        Pub.MessageObject sRet = new Pub.MessageObject() { result = true, message = "" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
                this.SetQueryData();
                this.SetDoPage();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "PrintExcel")
            {
                this.SetQueryData();
                this.ExportExcel();
            }
            else
            {
                this.SetInitData();
                this.Calendar_CardTimeSDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd 00:00:00");
                this.Calendar_CardTimeEDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd 23:59:59");
                this.PagedList = this.ListLog.ToPagedList(PageIndex, 1);
            }
            ClientScript.RegisterClientScriptInclude("JsInclude1", "MealOrderRecordInq.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("QueryTool", "/uc/QueryTool.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }

        private void SetInitData()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            DBReader drOrg = null;
            string sql = string.Empty;

            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            //部門別資料處理
            sql = @"SELECT OrgID, OrgNo, OrgName FROM B01_OrgData
                        WHERE OrgClass = 'Department' ORDER BY OrgNo";
            oAcsDB.GetDataReader(sql, out drOrg);
            while (drOrg.Read())
            {
                ListItem Item = new ListItem();
                Item.Text = drOrg.DataReader["OrgName"].ToString() + "." + drOrg.DataReader["OrgNo"].ToString();
                Item.Value = drOrg.DataReader["OrgID"].ToString();
                this.dropDepartment.Items.Add(Item);
            }
            this.dropDepartment.Items.Insert(0, new ListItem(" == 全部 == ", ""));
        }

        private void SetQueryData()
        {
            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }
            List<ColNameObj> collist = new List<ColNameObj>();
            bool boolSort = this.SortType.Equals("ASC");

            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            string CardTimeSDate = Request["CardTimeSDate"];
            string CardTimeEDate = Request["CardTimeEDate"];
            string CardNo = Request["CardNo"];
            string PsnNo = Request["PsnNo"];
            string PsnName = Request["PsnName"];
            string Dep = Request["Dep"];
            string MealNo = Request["MealNo"];
            string MealFood = Request["MealFood"];

            string Sql = string.Empty;
            Sql = @" Select
            MO.RecordID,MO.OrderTime,MO.MealDate,MO.MealNo,CASE (MO.MealNo) When '1' Then '午餐' When '2' Then '宵夜' Else '' END AS 'MealNoDesc' ,
            MO.CardNo,MO.PsnNo,MO.PsnName,OrgData.OrgID,OrgData.OrgNo,O.OrgName,MO.OrderSrc,MO.MealFood,
            CASE (MO.MealFood) When '0' Then '葷食' When '1' Then '素食' Else '' END AS 'MealFoodDesc' ,
            MO.[Status],MO.ProcTime,MO.CancelSrc
            From B03_MealOrder MO
            LEFT JOIN OrgStrucAllData('Department') OrgData ON MO.OrgStrucID = OrgData.OrgStrucID
            LEFT JOIN B01_OrgData O ON OrgData.OrgNo = O.OrgNo
            Where 1=1 AND MO.OrderSrc in ('1','5') And MO.[Status] IN (1,3) ";
            Sql += " And convert(varchar, MO.OrderTime, 120) Between '" + CardTimeSDate.Replace(@"/", "-") + "' And '" + CardTimeEDate.Replace(@"/", "-") + "'";

            if (!string.IsNullOrEmpty(CardNo))
            {
                Sql += " And MO.CardNo = '" + CardNo.ToString().Trim() + "' ";
            }

            if (!string.IsNullOrEmpty(PsnNo))
            {
                Sql += " And MO.PsnNo = '" + PsnNo.ToString().Trim() + "' ";
            }

            if (!string.IsNullOrEmpty(PsnName))
            {
                Sql += " And MO.PsnName like N'" + PsnName.ToString().Trim() + "%'";
            }

            if (!string.IsNullOrEmpty(Dep))
            {
                Sql += " And OrgData.OrgID = '" + Dep.ToString().Trim() + "' ";
            }

            if (!string.IsNullOrEmpty(MealNo))
            {
                Sql += " And MO.MealNo = '" + MealNo.ToString().Trim() + "' ";
            }

            if (!string.IsNullOrEmpty(MealFood))
            {
                Sql += " And MO.MealFood = '" + MealFood.ToString().Trim() + "' ";
            }

            Sql += " ORDER BY MO.OrderTime ";

            this.ListLog = this.odo.GetQueryResult<B03_MealOrder>(Sql.ToString(), new
            {
                CardTimeSDate = CardTimeSDate,
                CardTimeEDate = CardTimeEDate,
                CardNo = CardNo,
                PsnNo = PsnNo,
                PsnName = PsnName,
                Dep = Dep,
                MealNo = MealNo,
                MealFood = MealFood,
                UserID = UserID
            }).OrderByField("OrderTime", true).ToList();

            if (Request["PageEvent"] == "PrintExcel")
            {
                this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
                this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog);
            }
            else
            {
                this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
            }
        }

        private void SetDoPage()
        {
            //啟始及結束頁
            StartPage = (PageIndex < ShowPage) ? 1 : PageIndex;
            if (StartPage > 1)
            {
                StartPage = (PageIndex + ShowPage / 2 >= this.PagedList.PageCount) ? this.PagedList.PageCount - ShowPage + 1 : PageIndex - ShowPage / 2;
            }
            EndPage = (StartPage - 1 > this.PagedList.PageCount - ShowPage) ? this.PagedList.PageCount + 1 : StartPage + ShowPage;
            //上下頁
            PrePage = PageIndex - 1 <= 1 ? 1 : PageIndex - 1;
            NextPage = PageIndex + 1 >= this.PagedList.PageCount ? this.PagedList.PageCount : PageIndex + 1;
        }
  
        private void ExportExcel()
        {
            
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");

            string CardTimeSDate = Request["CardTimeSDate"];
            string CardTimeEDate = Request["CardTimeEDate"];
            string CardNo = Request["CardNo"];
            string PsnNo = Request["PsnNo"];
            string PsnName = Request["PsnName"];
            string Dep = Request["Dep"];
            string MealNo = Request["MealNo"];
            string MealFood = Request["MealFood"];

            string Sql = string.Empty;
            Sql = @"  Select
            MO.RecordID,MO.OrderTime,MO.MealDate,MO.MealNo,CASE (MO.MealNo) When '1' Then '午餐' When '2' Then '宵夜' Else '' END AS 'MealNoDesc' ,
            MO.CardNo,MO.PsnNo,MO.PsnName,OrgData.OrgID,OrgData.OrgNo,O.OrgName,MO.OrderSrc,MO.MealFood,
            CASE (MO.MealFood) When '0' Then '葷食' When '1' Then '素食' Else '' END AS 'MealFoodDesc' ,
            MO.[Status],MO.ProcTime			
			,MO.CancelSrc
            From B03_MealOrder MO
            LEFT JOIN OrgStrucAllData('Department') OrgData ON MO.OrgStrucID = OrgData.OrgStrucID
            LEFT JOIN B01_OrgData O ON OrgData.OrgNo = O.OrgNo
            Where 1=1 AND MO.OrderSrc in ('1','5') And MO.[Status] IN (1,3) ";

            Sql += " And convert(varchar, MO.OrderTime, 120) Between '" + CardTimeSDate.Replace(@"/", "-") + "' And '" + CardTimeEDate.Replace(@"/", "-") + "'";

            if (!string.IsNullOrEmpty(CardNo))
            {
                Sql += " And MO.CardNo = '" + CardNo.ToString().Trim() + "' ";
            }

            if (!string.IsNullOrEmpty(PsnNo))
            {
                Sql += " And MO.PsnNo = '" + PsnNo.ToString().Trim() + "' ";
            }

            if (!string.IsNullOrEmpty(PsnName))
            {
                Sql += " And MO.PsnName like N'" + PsnName.ToString().Trim() + "%'";
            }

            if (!string.IsNullOrEmpty(Dep))
            {
                Sql += " And OrgData.OrgID = '" + Dep.ToString().Trim() + "' ";
            }

            if (!string.IsNullOrEmpty(MealNo))
            {
                Sql += " And MO.MealNo = '" + MealNo.ToString().Trim() + "' ";
            }

            if (!string.IsNullOrEmpty(MealFood))
            {
                Sql += " And MO.MealFood = '" + MealFood.ToString().Trim() + "' ";
            }

            Sql += " ORDER BY MO.OrderTime ";

            this.ListLog = this.odo.GetQueryResult<B03_MealOrder>(Sql.ToString(), new
            {
                CardTimeSDate = CardTimeSDate,
                CardTimeEDate = CardTimeEDate,
                CardNo = CardNo,
                PsnNo = PsnNo,
                PsnName = PsnName,
                Dep = Dep,
                MealNo = MealNo,
                MealFood = MealFood,
                UserID = UserID
            }).OrderByField("OrderTime", true).ToList();

            if (this.ListLog.Count == 0)
            {
                this.ListLog.Insert(0, new B03_MealOrder()
                {
                    OrderTime = DateTime.MinValue,
                    MealNoDesc = "",
                    CardNo = "",
                    PsnNo = "",
                    PsnName = "",
                    OrgName = "",
                    MealFoodDesc = "",
                });
            }
            else
            {
                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("訂餐記錄查詢");
                ws.Cells[1, 1].Value = "訂餐時間";
                ws.Cells[1, 2].Value = "餐別";
                ws.Cells[1, 3].Value = "卡號";
                ws.Cells[1, 4].Value = "人員編號";
                ws.Cells[1, 5].Value = "姓名";
                ws.Cells[1, 6].Value = "部門";
                ws.Cells[1, 7].Value = "葷素食";

                int count = 2;
                foreach (var o in this.ListLog)
                {
                    ws.Cells[count, 1].Value = string.Format("{0:yyyy/MM/dd HH:mm:ss}", o.OrderTime);
                    ws.Cells[count, 2].Value = o.MealNoDesc;
                    ws.Cells[count, 3].Value = o.CardNo;
                    ws.Cells[count, 4].Value = o.PsnNo;
                    ws.Cells[count, 5].Value = o.PsnName;
                    ws.Cells[count, 6].Value = o.OrgName;
                    ws.Cells[count, 7].Value = o.MealFoodDesc;
                    count++;
                }

                ws.Cells.AutoFitColumns(); //自動欄寬
                Response.Clear();
                Response.AddHeader("content-disposition", "attachment; filename=訂餐記錄查詢.xls");
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
            }
        }
    }
}