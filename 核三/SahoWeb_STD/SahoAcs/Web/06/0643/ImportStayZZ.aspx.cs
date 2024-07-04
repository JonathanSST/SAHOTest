using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using OfficeOpenXml;
using DapperDataObjectLib;
using SahoAcs.DBModel;


namespace SahoAcs.Web._06._0643
{
    public partial class ImportStayZZ : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        protected void Page_Load(object sender, EventArgs e)
        {
            if(Request["PageEvent"]!=null && Request["PageEvent"] == "Save")
            {
                if (Request.Files.Count > 0)
                {
                    //Response.Write("檔案清單：<br/>");
                    for (int i = 0; i < Request.Files.Count; i++)
                    {                        
                        string name = Request.Files[i].FileName;
                        if (System.IO.Path.GetExtension(name) != ".xlsx")
                        {
                            Response.Clear();
                            Response.Write("匯入格式錯誤，必須為xlsx 格式");
                            Response.End();
                            break;
                        }                                          
                       this.SetInputExcel(i);
                    }

                }
                
            }
        }//end page_load






        private void SetInputExcel(int Index)
        {
            //using (var reader=new StreamReader(Request.Files[Index].InputStream))
            using (ExcelPackage ep = new ExcelPackage(Request.Files[Index].InputStream))
            {
                ExcelWorksheet sheet = ep.Workbook.Worksheets[1];       //取得Sheet1             
                int startRowNumber = sheet.Dimension.Start.Row;         //起始列編號，從1算起
                int endRowNumber = sheet.Dimension.End.Row;             //結束列編號，從1算起
                int startColumn = sheet.Dimension.Start.Column;         //開始欄編號，從1算起
                int endColumn = sheet.Dimension.End.Column;             //結束欄編號，從1算起
                try
                {
                    foreach (var st in ep.Workbook.Worksheets)
                    {
                        if (st.Dimension == null)
                        {
                            continue;
                        }
                        for (int r = 2; r <= st.Dimension.End.Row; r++)
                        {
                            if (st.Cells[r, 6].Value == null || st.Cells[r, 6].Value.ToString().Length != 11)
                            {
                                continue;
                            }
                            ClassRoomTable data = new ClassRoomTable();
                            data.PsnNo = Convert.ToString(st.Cells[r, 1].Value);
                            data.InOutDate = (int.Parse(st.Cells[r, 6].Value.ToString().Substring(0, 3)) + 1911) + "/" + st.Cells[r, 6].Value.ToString().Substring(3, 2) + "/" + st.Cells[r, 6].Value.ToString().Substring(5, 2);   //這邊要修改 20201221
                            odo.Execute("DELETE B03_ClassRoomTable WHERE InOutDate=@InOutDate AND PsnNo=@PsnNo", data);
                            string TimeVal = "";
                            if (st.Cells[r, 6].Value != null && st.Cells[r, 6].Value.ToString().Length == 11)
                            {
                                TimeVal = Convert.ToString(st.Cells[r, 6].Value);
                                data.InTime = new DateTime(int.Parse(TimeVal.Substring(0, 3)) + 1911, int.Parse(TimeVal.Substring(3, 2)), int.Parse(TimeVal.Substring(5, 2)), int.Parse(TimeVal.Substring(7, 2)), int.Parse(TimeVal.Substring(9, 2)), 0);
                            }
                            if (st.Cells[r, 8].Value != null && st.Cells[r, 8].Value.ToString().Length == 11)
                            {
                                TimeVal = Convert.ToString(st.Cells[r, 8].Value);
                                data.OutTime = new DateTime(int.Parse(TimeVal.Substring(0, 3)) + 1911, int.Parse(TimeVal.Substring(3, 2)), int.Parse(TimeVal.Substring(5, 2)), int.Parse(TimeVal.Substring(7, 2)), int.Parse(TimeVal.Substring(9, 2)), 0);
                            }
                            odo.Execute("INSERT INTO B03_ClassRoomTable (PsnNo,InOutDate,InTime,OutTime) VALUES (@PsnNo,@InOutDate,@InTime,@OutTime)", data);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }//end EpPlus
            Response.Clear();
            Response.Write("作業完成");
            Response.End();
        }


    }//end form class
}//end namespace