using DapperDataObjectLib;
using OfficeOpenXml;
using SahoAcs.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;


namespace SahoAcs.Web._01._0117
{
    public partial class ImportHoliday : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        string Msg = "";
        int EffRow = 0;
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
                        if (System.IO.Path.GetExtension(name) != ".txt")
                        {
                            Msg = "匯入格式錯誤，必須為txt 格式!";                          
                            break;
                        }
                        this.SetInputData(i);                        
                    }
                    Msg += string.Format("\n完成{0}筆休假紀錄匯入", this.EffRow);
                }
                else
                {
                    Msg = "請選取要匯入的休假憑證資料!";          
                }
                
                Response.Clear();
                Response.Write(Msg);
                Response.End();
            }
            string jsFileEnd = "<script src=\"../../../uc/QueryTool.js?" + Pub.GetNowTime + "\" Type=\"text/javascript\"></script>\n";            
            ClientScript.RegisterStartupScript(typeof(string), "QueryTools", jsFileEnd, false);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js = "<script type='text/javascript'>" + js + "</script>";
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("MainJs", "ImportHoliday.js?" + Pub.GetNowTime); //加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
        } // end page_load

        private void SetInputData(int Index)
        {            
            StreamReader sr = new StreamReader(this.Request.Files[Index].InputStream, System.Text.Encoding.Default);
            string line;
            B03PsnHoliday holiday = new B03PsnHoliday();
            holiday.CreateUserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            holiday.UpdateUserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            List<OrgDataEntity> OrgList = this.odo.GetQueryResult<OrgDataEntity>(@"
                    select * from OrgStrucAllData('Unit')
                    WHERE OrgNo<>''
                    UNION
                    select * from OrgStrucAllData('Department')
                    WHERE OrgNo<>''").ToList();
            List<PersonEntity> PsnList = this.odo.GetQueryResult<PersonEntity>("SELECT * FROM B01_Person").ToList();
            while ((line = sr.ReadLine()) != null)
            {
                line = line.TrimStart().TrimEnd();                
                if (line.Length != 52)
                {                    
                    continue;
                }
                if (line.Substring(18, 4).Trim().Equals(""))
                {
                    holiday.LicNo = line.Substring(8, 4);
                    holiday.PsnNo = line.Substring(12, 6);
                    if (PsnList.Where(i => i.PsnNo.Equals(holiday.PsnNo)).Count() == 0)
                    {
                        Msg += string.Format("\n系統查無此工號{0}",holiday.PsnNo);
                        continue;
                    }
                    holiday.LicType = line.Substring(6, 2);
                    holiday.HoliNo = line.Substring(22, 2);
                    holiday.OrgNo = line.Substring(3, 3);
                    holiday.Daily = line.Substring(24, 3);
                    holiday.Hours = line.Substring(27, 1);
                    holiday.Minutes = line.Substring(28, 2);
                    holiday.StartTime = this.GetStringToTime(line.Substring(30, 11));
                    holiday.EndTime = this.GetStringToTime(line.Substring(41, 11));
                    holiday.CreateTime = holiday.UpdateTime = DateTime.Now;
                    ////啟始日期和結束日期重複與個人資料重疊  這邊2022/03/30  依Q22032902 需求更新                
                    //if (this.odo.GetQueryResult("SELECT * FROM B03_PsnHoliday WHERE PsnNo=@PsnNo AND ((@StartTime BETWEEN StartTime AND EndTime) OR (@EndTime BETWEEN StartTime AND EndTime))", holiday).Count() > 0)
                    //{
                    //    Msg += string.Format("\n請休假日期與目前假表出現重複{0}，{1:yyyy/MM/dd}, {2:yyyy/MM/dd}", holiday.PsnNo, holiday.StartTime, holiday.EndTime);
                    //    continue;
                    //}

                    //由目前的 PsnNo 取得
                    var OrgStrucID = PsnList.Where(i => i.PsnNo.Equals(holiday.PsnNo)).First().OrgStrucID;
                    holiday.OrgNo = OrgList.Where(i => !i.OrgNo.Equals("") && i.OrgStrucID == OrgStrucID).First().OrgNo;
                    if (this.odo.GetQueryResult("SELECT * FROM B01_Person WHERE PsnNo=@PsnNo", holiday).Count() > 0)
                    {
                        this.odo.Execute(@"INSERT INTO B03_PsnHoliday (PsnNo,LicType,licNo,HoliNo,OrgNo,Daily,Hours,Minutes,StartTime,EndTime,CreateUserID,CreateTime,UpdateUserID,UpdateTime) 
                                                        VALUES (@PsnNo,@LicType,@LicNo,@HoliNo,@OrgNo,@Daily,@Hours,@Minutes,@StartTime,@EndTime,@CreateUserID,GETDATE(),@UpdateUserID,GETDATE())", holiday);
                        if (this.odo.isSuccess)
                            EffRow++;
                    }
                   
                }
                else
                {
                    continue;
                }                
            }

        }//end InputData Mehod



        #region Method
        
        DateTime GetStringToTime(string DateFormat)
        {
            DateTime TargetDate = DateTime.Now;
            try
            {
                DateFormat = (int.Parse(DateFormat.Substring(0, 3)) + 1911) + DateFormat.Substring(3, 8);
                DateTime.TryParse(DateFormat.Substring(0,4)+"/"+DateFormat.Substring(4,2)+"/"+DateFormat.Substring(6,2)+" "+DateFormat.Substring(8,2)+":"+DateFormat.Substring(10,2)+":00", 
                    out TargetDate);
                if (TargetDate.Year == 1)
                {
                    return DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
           
            return TargetDate;
        }


        #endregion


    }//end form class
}//end namespace