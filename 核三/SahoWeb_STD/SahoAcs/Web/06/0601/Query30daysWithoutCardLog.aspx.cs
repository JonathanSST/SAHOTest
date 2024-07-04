using PagedList;
using SahoAcs.DBModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBClass;
using OfficeOpenXml;

namespace SahoAcs
{
    public partial class Query30daysWithoutCardLog : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<PersonCardList> ListPerson = new List<PersonCardList>();
        public List<string> ListLogPsnNo = new List<string>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();

        public DataTable DataResult = new DataTable();

        public string txt_CardNo_PsnName = "";

        public string SortName = "CardTime";
        public string SortType = "DESC";

        public List<string> EquDatas = new List<string>();

        public int PageIndex = 1;

        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;

        public string PsnID = "";

        public IPagedList<PersonCardList> PagedList;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
                this.SetInitData();
                this.SetQueryData();
                this.SetDoPage();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "Print")
            {
                this.SetInitData();
                this.SetQueryData();
                this.ExportExcel();
            }
            else
            {
                this.SetInitData();
                this.SetQueryData();
                this.SetDoPage();
            }

            ClientScript.RegisterClientScriptInclude("0601_01", "Query30daysWithoutCardLog.js?" + Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
        }

        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

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

        private void SetQueryData()
        {
            if (Request["SortName"] != null)
            {
                this.SortName = Request["SortName"];
                this.SortType = Request["SortType"];
            }
            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }

            bool boolSort = this.SortType.Equals("ASC");

            string sqlorgjoin = @"SELECT B01_EquData.EquNo, B01_EquData.EquName FROM B01_EquData
                INNER JOIN B01_EquGroupData ON B01_EquGroupData.EquID = B01_EquData.EquID
                INNER JOIN B01_MgnEquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroupData.EquGrpID
                INNER JOIN B00_SysUserMgns ON B00_SysUserMgns.MgaID = B01_MgnEquGroup.MgaID
                WHERE B00_SysUserMgns.UserID = @UserID GROUP BY B01_EquData.EquNo, B01_EquData.EquName";
            this.EquDatas = this.odo.GetQueryResult<EquGroupData>(sqlorgjoin, new { UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID") }).Select(i => i.EquNo).ToList();

            //30天有刷卡的人員記錄 
            string sqlStr = @"SELECT PsnNo FROM B01_CardLog
                            WHERE (EquNo IN @EquList)
                            AND (CONVERT(VARCHAR(10),CardTime,111) between CONVERT(VARCHAR(10),DATEADD(DAY, -30, GETDATE()),111) AND CONVERT(VARCHAR(10),GETDATE(),111))
                            AND PsnNo IS NOT NULL
                            GROUP BY PsnNo
            ";

            this.ListLogPsnNo = this.odo.GetQueryResult<string>(sqlStr, new
            {
                EquList = EquDatas
            }).ToList();

            //所有Person
            sqlStr = @"select B01_Card.CardNo, V_Person.OrgNameList, V_Person.PsnNo, V_Person.PsnName from V_Person
                       LEFT JOIN B01_Card ON V_Person.PsnID = B01_Card.PsnID            
            ";

            this.ListPerson = this.odo.GetQueryResult<PersonCardList>(sqlStr).ToList();

            foreach (var item in ListLogPsnNo)
            {
                //排除有刷卡的員工
                var whereRemove = ListPerson.FirstOrDefault(x => x.PsnNo == item.ToString());
                if (whereRemove != null)
                {
                    ListPerson.Remove(whereRemove);
                }
            }


            if (Request["PageEvent"] == "Print")
            {
                //轉datatable                
                this.DataResult = OrmDataObject.IEnumerableToTable(this.ListPerson);
            }
            else
            {
                //轉datatable
                this.PagedList = this.ListPerson.ToPagedList(PageIndex, 100);
                this.DataResult = OrmDataObject.IEnumerableToTable(PagedList);
            }

        }

        private void ExportExcel()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Reports");
            ws.Cells[1, 1].Value = "卡號";
            ws.Cells[1, 2].Value = "部門名稱";
            ws.Cells[1, 3].Value = "人員編號";
            ws.Cells[1, 4].Value = "人員姓名";

            //Content
            for (int i = 0; i < this.ListPerson.Count; i++)
            {
                ws.Cells[i + 2, 1].Value = this.ListPerson[i].CardNo;
                ws.Cells[i + 2, 2].Value = this.ListPerson[i].OrgNameList;
                ws.Cells[i + 2, 3].Value = this.ListPerson[i].PsnNo;
                ws.Cells[i + 2, 4].Value = this.ListPerson[i].PsnName;
                //i++;
            }
            ws.Cells[this.ListPerson.Count + 2, 1].Value = "總筆數：";
            ws.Cells[this.ListPerson.Count + 2, 2].Value = this.ListPerson.Count;
            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=Query30daysWithoutCardLog.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }
    }

    public class PersonCardList
    {
        public string CardNo { get; set; }
        public string OrgNameList { get; set; }
        public string PsnNo { get; set; }
        public string PsnName { get; set; }
    }
}