using DapperDataObjectLib;
using OfficeOpenXml;
using SahoAcs.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web._01._0115
{
    public partial class ImportScheduleDataZZ : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        string Msg = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Save")
            {
                if (Request.Files.Count > 0)
                {
                    //Response.Write("檔案清單：<br/>");
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        string name = Request.Files[i].FileName;
                        if (System.IO.Path.GetExtension(name) != ".xlsx")
                        {
                            Msg = "匯入格式錯誤，必須為xlsx 格式!";                          
                            break;
                        }
                        this.SetInputExcel(i);
                    }
                }
                else
                {
                    Msg = "請選取要匯入次月排班表資料!";          
                }
                Response.Clear();
                Response.Write(Msg);
                Response.End();
            }
        } // end page_load

        private void SetInputExcel(int Index)
        {    
            using (ExcelPackage ep = new ExcelPackage(Request.Files[Index].InputStream))
            {
                ExcelWorksheet sheet = ep.Workbook.Worksheets[1];       //取得Sheet1             
                int startRowNumber = sheet.Dimension.Start.Row;         //起始列編號，從1算起
                int endRowNumber = sheet.Dimension.End.Row;             //結束列編號，從1算起
                int startColumn = sheet.Dimension.Start.Column;         //開始欄編號，從1算起
                int endColumn = sheet.Dimension.End.Column;             //結束欄編號，從1算起

                //只跑第一張工作表
                int SheetCount = 1;
                try
                {
                    foreach (var st in ep.Workbook.Worksheets)
                    {
                        if (SheetCount < 2)
                        {
                            if (st.Dimension == null)
                            {
                                continue;
                            }
                            ScheduleTable scheduleTable = new ScheduleTable();
                            //init      
                            string ExcelCurrentyear = "";
                            string ExcelNextmonth = "";
                            string ExcelemployeeID = "";
                            string ExcelemployeeName = "";
                            string CurrentYear = "";
                            string NextMonth = "";
                            string CurrentDayCellvalue = "";
                            //年份
                            ExcelCurrentyear = Convert.ToString(st.Cells[1, 2].Value);
                            //次月份
                            ExcelNextmonth = Monthaddzero(Convert.ToString(st.Cells[2, 2].Value));
                            string YearMonthExl = ExcelCurrentyear + ExcelNextmonth;
                            //系統時間 年
                            CurrentYear = DateTime.Now.Year.ToString();
                            //系統時間 次月
                            NextMonth = DateTime.Parse(DateTime.Now.ToString()).AddMonths(1).ToString("MM");
                            //驗証匯入的年份與次月份是否正確
                            //if (ExcelCurrentyear.CompareTo(CurrentYear) >=0 || Convert.ToInt32(NextMonth) != Convert.ToInt32(ExcelNextmonth))
                            if (YearMonthExl.CompareTo(DateTime.Now.ToString("yyyyMM"))<=0)
                            {
                                Msg = "請檢查Excel年份資料是否為今年 或 月份只能選取次月份!";
                            }
                            else
                            {
                                //驗証姓名 跟 工號是否跟person table相同
                                string VerificationMsg = "";
                                VerificationMsg = VerificationPsnData(st);
                                //無回傳錯誤
                                if (VerificationMsg=="")
                                {
                                    //例假表人員row開始跑迴圈
                                    for (int Cellrow = 5; Cellrow < st.Dimension.End.Row; Cellrow++)
                                    {
                                        //員工姓名
                                         ExcelemployeeName = Convert.ToString(sheet.Cells[Cellrow, 1].Value).ToString().Trim();
                                        //工號
                                         ExcelemployeeID = Convert.ToString(sheet.Cells[Cellrow, 2].Value).ToString().Trim();
                                        //驗証是否有匯入次月排班表
                                        //Excel工號
                                        scheduleTable.employeeID = ExcelemployeeID;
                                        //Excel員工姓名
                                        scheduleTable.employeeName = ExcelemployeeName;
                                        //Excel建立時間
                                        scheduleTable.CreateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                        //Excel例假備註
                                        scheduleTable.VacationInfo = Convert.ToString(st.Cells[Cellrow, 34].Value);
                                        //Excel年份
                                        scheduleTable.Year = ExcelCurrentyear;
                                        //Excel次月份
                                        scheduleTable.Month = ExcelNextmonth;

                                        scheduleTable.SyncMark = "0";

                                        var ScheduleLog = this.odo.GetQueryResult<CardLogModel>("SELECT * FROM  B03_ScheduleTable WHERE employeeID = @employeeID AND Year = @Year AND Month =@Month", 
                                            new { employeeID = ExcelemployeeID, Year = ExcelCurrentyear, Month = ExcelNextmonth });
                                        if (ScheduleLog.Count()==0)
                                        {
                                            // For excel  cells 例假日 
                                            for (int Sc = 3; Sc < 34; Sc++)
                                            {
                                                CurrentDayCellvalue = Convert.ToString(st.Cells[4, Sc].Value);
                                                if (Convert.ToString(st.Cells[Cellrow, Sc].Value) == "例" && CurrentDayCellvalue != "")
                                                {
                                                    //排休日期
                                                    scheduleTable.VacationDate = Convert.ToDateTime(ExcelCurrentyear + "/" + ExcelNextmonth + "/" + Convert.ToString(st.Cells[4, Sc].Value));
                                                    odo.Execute("INSERT INTO B03_ScheduleTable (employeeID,employeeName,CreateTime,VacationDate,VacationInfo,SyncMark,Year,Month) " +
                                                        "VALUES (@employeeID,@employeeName,@CreateTime,@VacationDate,@VacationInfo,@SyncMark,@Year,@Month)", scheduleTable);
                                                }
                                            }// end For excel  cells 例假日  
                                           
                                        }  // end if 驗証是否有匯入次月排班表
                                        else
                                        {
                                            /* 刪除原本的休假紀錄 這部份先停用 By Sam 20210602*/
                                            //如果有次月資料 先刪除再新增
                                            string SqlStr = "";
                                            SqlStr += @"DELETE B03_ScheduleTable WHERE employeeID=@employeeID AND Month=@Month AND Year=@Year";
                                            odo.BeginTransaction();
                                            int IsDel = odo.Execute(SqlStr, new
                                            {
                                                employeeID = ExcelemployeeID,
                                                Month= ExcelNextmonth,
                                                Year= ExcelCurrentyear
                                            });
                                            if (IsDel > -1)
                                            {
                                                odo.Commit();
                                                //ExcelCurrentDay
                                                for (int Sc = 3; Sc < 34; Sc++)
                                                {
                                                    CurrentDayCellvalue = Convert.ToString(st.Cells[4, Sc].Value);
                                                    if (Convert.ToString(st.Cells[Cellrow, Sc].Value) == "例" && CurrentDayCellvalue != "")
                                                    {
                                                        //排休日期
                                                        scheduleTable.VacationDate = Convert.ToDateTime(ExcelCurrentyear + "/" + ExcelNextmonth + "/" + Convert.ToString(st.Cells[4, Sc].Value));
                                                        odo.Execute(@"INSERT INTO B03_ScheduleTable (employeeID,employeeName,CreateTime,VacationDate,VacationInfo,SyncMark,Year,Month) 
                                                        VALUES (@employeeID,@employeeName,@CreateTime,@VacationDate,@VacationInfo,@SyncMark,@Year,@Month)", scheduleTable);
                                                    }
                                                }// end For excel  cells 例假日                                        
                                            }
                                            else
                                            {
                                                odo.Rollback();
                                            }
                                        }
                                    }// end for 例假表開始跑迴圈
                                    Msg = "作業完成";
                                }// end if 判斷是否有錯誤人員資訊
                                else
                                {
                                    Msg = VerificationMsg;
                                }// end else
                            }// end else 驗証匯入的年份與次月份是否正確
                        }//end if
                        SheetCount++;
                    }//end foreach 
                }
                catch (Exception ex)
                {
                    Msg = "上傳失敗，請洽系統管理員 或 檢查是否為正確排班表格式";                               
                }
            }//end EpPlus
            Response.Clear();
            Response.Write(Msg);
            Response.End();
        }



        #region Method

        /// <summary>
        /// 月份補0 方便之後排序
        /// </summary>
        /// <param name="month">月份</param>
        /// <returns></returns>
        protected string Monthaddzero(string month)
        {
            string Currentmonth = "";
            if (Convert.ToInt32(month) < 10)
            {
                Currentmonth = "0" + month;
            }
            else
            {
                Currentmonth = month;
            }
            return Currentmonth;
        }

        /// <summary>
        /// 驗証PsnNo是否真實存在與是否有重複工號
        /// </summary>
        /// <param name="sheet">Excel Sheet</param>
        /// <returns></returns>
        protected string VerificationPsnData(ExcelWorksheet sheet)
        {
            bool PersonAllpass = false;
            string VerificationMsg = "";
            //判斷是excel是否有重複工號
            string ChkExcelid = GetidResult(sheet);
            var PsnData = this.odo.GetQueryResult("select PsnNo,PsnName,PsnID,PsnAccount,OrgStrucID from B01_Person order by OrgStrucID");
            if (ChkExcelid=="")
            {
                for (int Cellrow = 5; Cellrow < sheet.Dimension.End.Row; Cellrow++)
                {
                    //員工姓名
                    string ExcelemployeeName = Convert.ToString(sheet.Cells[Cellrow, 1].Value).ToString().Trim();
                    //工號
                    string ExcelemployeeID = Convert.ToString(sheet.Cells[Cellrow, 2].Value).ToString().Trim();
                    //判斷例假表員工姓名或工號是否空白 代表沒資料 就中斷迴圈不再新增資料
                    if (ExcelemployeeName == "" || ExcelemployeeID == "")
                    {
                        break;
                    }
                    else
                    {
                        var PsnFind = PsnData.Where(x => x.PsnNo == ExcelemployeeID).FirstOrDefault();
                        if (PsnFind == null)
                        {
                            VerificationMsg += "人員姓名:" + ExcelemployeeName + "  ,工號:" + ExcelemployeeID + ",人員資料有誤，請檢查排班表資料";
                            PersonAllpass = false;
                            break;
                        }
                        else
                        {
                            PersonAllpass = true;
                        }
                    }

                }// end for 
            }
            else
            {
                VerificationMsg += "排班表資料有重複工號,工號:["+ ChkExcelid + "],請修正排班表內容";
            }
          
            return VerificationMsg;
        } //end VerificationPsnData_method

        private string GetidResult(ExcelWorksheet sheet)
        {
            string ChkMsg = "";
            //宣告一個空list
            List<string> ExcelidList = new List<string>();
            //foreach 目前的sheet 讀出 id
            for (int Cellrow = 5; Cellrow < sheet.Dimension.End.Row; Cellrow++)
            {
                //員工姓名
                string ExcelemployeeName = Convert.ToString(sheet.Cells[Cellrow, 1].Value).ToString().Trim();
                //工號
                string ExcelemployeeID = Convert.ToString(sheet.Cells[Cellrow, 2].Value).ToString().Trim();
                if (ExcelemployeeID != "")
                {
                    ExcelidList.Add(Convert.ToString(sheet.Cells[Cellrow, 2].Value).ToString().Trim());
                }
            }
            for(int i=0;i< ExcelidList.Count; i++)
            {
                for (int j=i+1; j< ExcelidList.Count;j++)
                {
                    if (ExcelidList[i] == ExcelidList[j])
                    {
                        ChkMsg = ExcelidList[i].ToString();
                        break;
                    }
                }
            }
            return ChkMsg;
        }

        #endregion


    }//end form class
}//end namespace