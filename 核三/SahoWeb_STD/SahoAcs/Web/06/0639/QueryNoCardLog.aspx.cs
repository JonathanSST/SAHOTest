using DapperDataObjectLib;
using OfficeOpenXml;
using PagedList;
using Sa.Web;
using SahoAcs.DBClass;
using SahoAcs.DBModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace SahoAcs.Web._06._0639
{
    public partial class QueryNoCardLog : System.Web.UI.Page
    {
        private OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<StayCardLog> ListLog = new List<StayCardLog>();

        public List<StayCardLog> ListLogOut = new List<StayCardLog>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();

        public DataTable DataResult = new DataTable();

        public string txt_CardNo_PsnName = "";

        public string SortName = "PsnNo";

        public string SortType = "ASC";

        public List<string> EquDatas = new List<string>();

        public int PageIndex = 1;

        public int StartPage;

        public int EndPage;

        public int ShowPage = 5;

        public int NextPage = 1;

        public int PrePage = 1;

        public string PsnID = "";

        public IPagedList<StayCardLog> PagedList;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.Form["PageEvent"] != null && this.Request.Form["PageEvent"] == "Query")
            {
                this.SetInitData();
                this.SetQueryData();
            }
            else if (this.Request.Form["PageEvent"] != null && this.Request.Form["PageEvent"] == "Print")
            {
                this.SetInitData();
                this.SetQueryData();
                this.ExportExcel();
            }
            else if (this.Request.Form["PageEvent"] == null || !(this.Request.Form["PageEvent"] == "QueryOneLog"))
            {
                this.SetInitData();
            }            
            base.ClientScript.RegisterClientScriptInclude("0639", "QueryNoCardLog.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }





        private void SetInitData()
        {
            this.hideUserID.Value = Fun.GetSessionStr(this.Page, "UserID");
            new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            List<string> equList = new List<string>();
            if (ConfigurationManager.AppSettings["DoorList"] != null)
            {
                equList = ConfigurationManager.AppSettings["DoorList"].Split(new char[]
                {
                    ','
                }).ToList<string>();
            }
            IEnumerable<dynamic> queryResult = this.odo.GetQueryResult("SELECT ISNULL(MAX(CardTime),GETDATE()) AS MaxTime FROM B01_CardLog WHERE CardTime <= GETDATE() AND EquNo IN @EquList", new
            {
                EquList = equList
            });
            if (queryResult.Count() == 0)
            {
                this.CalendarS.DateValue = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                this.CalendarE.DateValue = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            }
            else
            {
                this.CalendarS.DateValue = string.Format("{0:yyyy/MM/dd} 00:00:00", queryResult.First().MaxTime);
                this.CalendarE.DateValue = string.Format("{0:yyyy/MM/dd} 23:59:59", queryResult.First().MaxTime);
            }



            this.ListCols.Add(new ColNameObj
            {
                ColName = "PsnNo",
                DataWidth = 104,
                TitleWidth = 101,
                TitleName = "人員編號"
            });
            this.ListCols.Add(new ColNameObj
            {
                ColName = "DepName",
                DataWidth = 208,
                TitleWidth = 202,
                TitleName = "部門單位"
            });
            this.ListCols.Add(new ColNameObj
            {
                ColName = "PsnName",
                DataWidth = 104,
                TitleWidth = 101,
                TitleName = "姓名"
            });
            this.ListCols.Add(new ColNameObj
            {
                ColName = "CardNo",
                DataWidth = 104,
                TitleWidth = 101,
                TitleName = "卡號"
            });
            this.ListCols.Add(new ColNameObj
            {
                ColName = "EquName",
                DataWidth = 208,
                TitleWidth = 202,
                TitleName = "設備名稱"
            });
            this.ListCols.Add(new ColNameObj
            {
                ColName = "EquNo",
                DataWidth = 84,
                TitleWidth = 81,
                TitleName = "設備編號"
            });
            



        }//end init data


        private void SetDoPage()
        {
            this.StartPage = ((this.PageIndex < this.ShowPage) ? 1 : this.PageIndex);
            if (this.StartPage > 1)
            {
                this.StartPage = ((this.PageIndex + this.ShowPage / 2 >= this.PagedList.PageCount) ? (this.PagedList.PageCount - this.ShowPage + 1) : (this.PageIndex - this.ShowPage / 2));
            }
            this.EndPage = ((this.StartPage - 1 > this.PagedList.PageCount - this.ShowPage) ? (this.PagedList.PageCount + 1) : (this.StartPage + this.ShowPage));
            this.PrePage = ((this.PageIndex - 1 <= 1) ? 1 : (this.PageIndex - 1));
            this.NextPage = ((this.PageIndex + 1 >= this.PagedList.PageCount) ? this.PagedList.PageCount : (this.PageIndex + 1));
        }


        private void SetQueryData()
        {
            if (this.Request.Form["SortName"] != null)
            {
                this.SortName = this.Request.Form["SortName"];
                this.SortType = this.Request.Form["SortType"];
            }
            if (this.Request.Form["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(this.Request.Form["PageIndex"]);
            }
            List<ColNameObj> list = new List<ColNameObj>();
            bool ascending = this.SortType.Equals("ASC");
            string[] values = base.Request.Form.GetValues("ColName");
            for (int j = 0; j < values.Length; j++)
            {
                string s = values[j];
                list.Add((from i in this.ListCols
                          where i.ColName == s
                          select i).FirstOrDefault<ColNameObj>());
            }
            this.ListCols = list;
            string cmd = @"SELECT B01_EquData.EquNo, B01_EquData.EquName FROM B01_EquData INNER JOIN 
					B01_EquGroupData ON B01_EquGroupData.EquID = B01_EquData.EquID
					INNER JOIN B01_MgnEquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroupData.EquGrpID
					INNER JOIN B00_SysUserMgns ON B00_SysUserMgns.MgaID = B01_MgnEquGroup.MgaID
					WHERE B00_SysUserMgns.UserID = @UserID GROUP BY B01_EquData.EquNo, B01_EquData.EquName";
         
            string cmd2 = @"SELECT EquNo, CardNo INTO #CL FROM B01_CardLog  
						WHERE CardTime BETWEEN @DateS AND @DateE GROUP BY EquNo, CardNo
						SELECT CA.CardNo,ED.EquName,ED.EquNo,OD.OrgName as DepName,PS.PsnNo,PS.PsnName,
						#CL.CardNo AS CardNo2 FROM B01_CardAuth AS CA
						INNER JOIN B01_Card AS CD ON CA.CardNo = CD.CardNo
						INNER JOIN B01_Person AS PS ON CD.PsnID = PS.PsnID
						INNER JOIN B01_EquData AS ED ON ED.EquID = CA.EquID
						INNER JOIN B01_OrgData AS OD ON OD.OrgID = PS.OrgStrucID
						LEFT JOIN  #CL ON #CL.EquNo = ED.EquNo AND #CL.CardNo = CA.CardNo
						WHERE NOT CA.OpMode = 'Del' AND CD.CardAuthAllow = 1 AND PS.PsnAuthAllow = 1 AND #CL.CardNo IS NULL
						GROUP BY  CA.CardNo,ED.EquName,ED.EquNo,OD.OrgName,PS.PsnNo,PS.PsnName,#CL.CardNo
						DROP TABLE #CL";
            this.ListLog = this.odo.GetQueryResult<StayCardLog>(cmd2, new
            {
                DateS = this.Request.Form["CardDateS"],
                DateE = this.Request.Form["CardDateE"]
            }).ToList<StayCardLog>();
            this.ListLog = this.ListLog.OrderByField(this.SortName, ascending).ToList<StayCardLog>();
            if (base.Request["PageEvent"] == "Print")
            {
                this.DataResult = OrmDataObject.IEnumerableToTable<StayCardLog>(this.ListLog);
            }
            else
            {
                this.DataResult = OrmDataObject.IEnumerableToTable<StayCardLog>(this.ListLog);
            }
        }

        private string GetConditionCmdStr()
        {
            string text = "";
            text += " AND (CardTime BETWEEN @CardTimeS AND @CardTimeE) ";
            if (this.txt_CardNo_PsnName != "")
            {
                text += " AND (PsnNo LIKE @PsnName OR PsnName LIKE @PsnName OR CardNo LIKE @PsnName) ";
            }
            if (this.Request.Form["MgaName"] != null && this.Request.Form["MgaName"] != "")
            {
                text += " AND MgaName = @MgaName ";
            }
            if (this.Request.Form["EquNo"] != null && this.Request.Form["EquNo"] != "")
            {
                text += " AND EquNo = @EquNo ";
            }
            if (this.Request.Form["PsnID"] != "")
            {
                text += " AND PsnNo IN (SELECT PsnNo FROM B01_Person WHERE PsnID=@PsnID)";
            }
            return text;
        }

        private void ExportExcel()
        {
            ExcelPackage excelPackage = new ExcelPackage();
            ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("Reports");
            for (int i = 0; i < this.ListCols.Count; i++)
            {
                excelWorksheet.Cells[1, i + 1].Value = this.ListCols[i].TitleName;
            }
            for (int j = 0; j < this.DataResult.Rows.Count; j++)
            {
                for (int k = 0; k < this.ListCols.Count<ColNameObj>(); k++)
                {
                    excelWorksheet.Cells[j + 2, k + 1].Value = this.DataResult.Rows[j][this.ListCols[k].ColName].ToString();
                }
            }
            excelWorksheet.Cells[this.DataResult.Rows.Count + 2, 1, this.DataResult.Rows.Count + 2, this.ListCols.Count].Merge = true;
            excelWorksheet.Cells[this.DataResult.Rows.Count + 2, 1].Value = string.Format("所有筆數:{0}", this.ListLog.Count<StayCardLog>(), this.ListLog.Count<StayCardLog>());
            excelWorksheet.Cells.AutoFitColumns();
            this.Response.Clear();
            this.Response.AddHeader("content-disposition", "attachment; filename=NoCardLog.xlsx");
            this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            this.Response.BinaryWrite(excelPackage.GetAsByteArray());
            this.Response.End();
        }

        private void EmptyCondition()
        {
            for (int i = 0; i < 1; i++)
            {
                this.ListLog.Add(new StayCardLog
                {
                    CardTime = DateTime.Now.ToString("HH:mm:ss"),
                    CardDate = DateTime.Now.ToString("yyyy/MM/dd")
                });
            }
            this.PagedList = this.ListLog.ToPagedList(1, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable<StayCardLog>(this.PagedList);
        }
    }//end form class
}//end namespace