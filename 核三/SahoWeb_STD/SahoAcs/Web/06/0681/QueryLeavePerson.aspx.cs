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
using System.Web.Script.Serialization;
using System.IO;
using System.Text;
using Org.BouncyCastle.Asn1.Ocsp;

namespace SahoAcs
{
    public partial class QueryLeavePerson : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<VPerson> PsnList = new List<VPerson>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public DataTable DataResult = new DataTable();

        public string txt_inputPsn = "";
        public string txt_inputIDNum = "";
        public DateTime oSTime = new DateTime();
        public DateTime oETime = new DateTime();

        public string SortName = "PsnNo";
        public string SortType = "ASC";

        public int PageIndex = 1;

        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string PsnID = "";

        public IPagedList<VPerson> PagedList;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
                this.SetInitData();
                this.SetQueryData();
                this.SetDoPage();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "PrintExcel")
            {
                this.SetInitData();
                this.SetQueryData();
                this.ExportExcel();
            }
            else
            {
                this.SetInitData();
            }
            if (!IsPostBack)
            {

            }
            ClientScript.RegisterClientScriptInclude("QueryLeavePerson", "QueryLeavePerson.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            this.CalendarS.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd") + " 00:00:00";
            this.CalendarE.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd") + " 23:59:59";

            ADVCalendar_PsnSTime.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd") + " 00:00:00";
            ADVCalendar_PsnETime.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd") + " 23:59:59";

            this.CreateDropDownList_OrgItem();
        }


        /// <summary>設定主資料表的分頁</summary>
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
            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }

            bool boolSort = this.SortType.Equals("ASC");

            string sql = "";

            sql = @"SELECT * FROM V_Person";

            string whereSql = this.GetConditionCmdStr();

            if (!string.IsNullOrEmpty(whereSql)) sql += " WHERE " + whereSql;

            this.PsnList = this.odo.GetQueryResult<VPerson>(sql, new
            {
                PsnSTime = oSTime,
                PsnETime = oETime,
                PsnNo = "%" + this.txt_inputPsn + "%",
                IDNum = "%" + this.txt_inputIDNum + "%",
                OrgID1 = Request["ComID"].Split(','),
                OrgID3 = Request["DepID"].Split(','),
                OrgID4 = Request["TitID"].Split(',')

            }).ToList();

            this.PsnList = this.PsnList.OrderByField(this.SortName, boolSort).ToList();

            if (Request["PageEvent"] == "PrintExcel")
            {
                //轉datatable                
                this.DataResult = OrmDataObject.IEnumerableToTable(this.PsnList);
            }
            else
            {
                this.PagedList = this.PsnList.ToPagedList(PageIndex, 100);
                this.DataResult = OrmDataObject.IEnumerableToTable(PagedList);
            }
        }

        private string GetConditionCmdStr()
        {
            string sql = "";
            this.txt_inputPsn = Request["inputPsn"];
            this.txt_inputIDNum = Request["inputIDNum"];

            //進階查詢功能設定
            if (Request["QueryMode"] == "2")
            {
                this.txt_inputPsn = Request["ADVPsnNoPsnNam"];
                this.txt_inputIDNum = Request["ADVIDNum"];

                if (Request["ComID"] != null && Request["ComID"] != "")
                {
                    if (!string.IsNullOrEmpty(sql)) sql += " AND ";
                    sql += " OrgID1 IN @OrgID1 ";
                }
                if (Request["DepID"] != null && Request["DepID"] != "")
                {
                    if (!string.IsNullOrEmpty(sql)) sql += " AND ";
                    sql += " OrgID3 IN @OrgID3 ";
                }
                if (Request["TitpID"] != null && Request["TitID"] != "")
                {
                    if (!string.IsNullOrEmpty(sql)) sql += " AND ";
                    sql += " OrgID4 IN @OrgID4 ";
                }
            }

            //一般查詢的方法
            oSTime = DateTime.Parse(Request["PsnSTime"]).GetUtcTime(this);
            oETime = DateTime.Parse(Request["PsnETime"]).GetUtcTime(this);
            if (!string.IsNullOrEmpty(sql)) sql += " AND ";
            sql += " (PsnETime BETWEEN @PsnSTime AND @PsnETime) ";

            if (this.txt_inputPsn != "")
            {
                if (!string.IsNullOrEmpty(sql)) sql += " AND ";
                sql += " (PsnNo LIKE @PsnNo OR PsnName LIKE @PsnNo) ";
            }
            if (this.txt_inputIDNum != "")
            {
                if (!string.IsNullOrEmpty(sql)) sql += " AND ";
                sql += "IDNum LIKE @IDNum ";
            }

            return sql;
        }


        private void ExportExcel()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Reports");
            ws.Cells[1, 1].Value = "人員編號";
            ws.Cells[1, 2].Value = "人員姓名";
            ws.Cells[1, 3].Value = "身份證號";
            ws.Cells[1, 4].Value = "到職日";
            ws.Cells[1, 5].Value = "離職日";
            ws.Cells[1, 6].Value = "單位";
            ws.Cells[1, 7].Value = "職稱";

            for (int i = 0; i < this.DataResult.Rows.Count; i++)
            {
                ws.Cells[i + 2, 1].Value = this.DataResult.Rows[i]["PsnNo"].ToString();
                ws.Cells[i + 2, 2].Value = this.DataResult.Rows[i]["PsnName"].ToString();
                ws.Cells[i + 2, 3].Value = this.DataResult.Rows[i]["IDNum"].ToString();
                ws.Cells[i + 2, 4].Value = string.Format("{0:yyyy/MM/dd HH:mm:ss}", this.DataResult.Rows[i]["PsnSTime"].ToString());
                ws.Cells[i + 2, 5].Value = string.Format("{0:yyyy/MM/dd HH:mm:ss}", this.DataResult.Rows[i]["PsnETime"].ToString());
                ws.Cells[i + 2, 6].Value = "[" + this.DataResult.Rows[i]["OrgNo3"].ToString() + "]" + this.DataResult.Rows[i]["OrgName3"].ToString();
                ws.Cells[i + 2, 7].Value = "[" + this.DataResult.Rows[i]["OrgNo4"].ToString() + "]" + this.DataResult.Rows[i]["OrgName4"].ToString();
            }


            ws.Cells[this.DataResult.Rows.Count + 3, 1].Value = "總筆數：";
            ws.Cells[this.DataResult.Rows.Count + 3, 2].Value = this.DataResult.Rows.Count;
            ws.Cells.AutoFitColumns(); //自動欄寬
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=QueryLeavePerson.xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        #region CreateDropDownList_OrgItem
        private void CreateDropDownList_OrgItem()
        {
            System.Web.UI.WebControls.ListItem Item;
            string sql = "";

            ADVDropDownList_Com.Items.Clear();
            ADVDropDownList_Dep.Items.Clear();
            ADVDropDownList_Tit.Items.Clear();

            sql = @" SELECT OrgID, OrgClass, OrgNo, OrgName FROM B01_OrgData ORDER BY OrgNo ";

            var result = this.odo.GetQueryResult(sql);
            if (result.Count() > 0)
            {
                foreach (var o in result)
                {
                    if (Convert.ToString(o.DepName) != "")
                    {
                        switch (o.OrgClass)
                        {
                            case "Company":
                                Item = new System.Web.UI.WebControls.ListItem();
                                Item.Text = "[" + Convert.ToString(o.OrgNo) + "]" + Convert.ToString(o.OrgName);
                                Item.Value = Convert.ToString(o.OrgID);
                                ADVDropDownList_Com.Items.Add(Item);
                                break;

                            case "Department":
                                Item = new System.Web.UI.WebControls.ListItem();
                                Item.Text = "[" + Convert.ToString(o.OrgNo) + "]" + Convert.ToString(o.OrgName);
                                Item.Value = Convert.ToString(o.OrgID);
                                ADVDropDownList_Dep.Items.Add(Item);
                                break;

                            case "Title":
                                Item = new System.Web.UI.WebControls.ListItem();
                                Item.Text = "[" + Convert.ToString(o.OrgNo) + "]" + Convert.ToString(o.OrgName);
                                Item.Value = Convert.ToString(o.OrgID);
                                ADVDropDownList_Tit.Items.Add(Item);
                                break;
                        }
                    }
                }
            }
            else
            {
                ADVDropDownList_Com.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
                ADVDropDownList_Dep.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
                ADVDropDownList_Tit.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
            }
        }
        #endregion


        private void EmptyCondition()
        {
            for (int i = 0; i < 1; i++)
            {

            }
            //轉datatable
            this.PagedList = this.PsnList.ToPagedList(1, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable(this.PagedList);
        }



    }//end page class
}//end namespace