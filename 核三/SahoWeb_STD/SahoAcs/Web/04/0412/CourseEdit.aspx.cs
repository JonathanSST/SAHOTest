using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using System.Reflection;

namespace SahoAcs.Web
{
    public partial class CourseEdit : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public CourseEntity entity = new CourseEntity();
        public List<City> Cities = new List<City>();
        City city = new City();
        public List<EquData> ListEqu = new List<EquData>();
        string MgaID = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            this.SetInit();
            if (Request["PageEvent"] != null)
            {
                if (Request["PageEvent"] == "Save")
                {
                    this.SaveData();
                }
                if (Request["PageEvent"] == "Edit")
                {
                    this.entity = this.odo.GetQueryResult<CourseEntity>("SELECT * FROM B03_Course WHERE CourseID=@CourseID", new {CourseID=Request["CourseID"]}).First();
                    string DtFormat = "{0:yyyy/MM/dd HH:mm:ss}";
                    this.CourseRealTimeE.DateValue = string.Format(DtFormat,this.entity.CourseRealTimeE);
                    this.CourseRealTimeS.DateValue = string.Format(DtFormat, this.entity.CourseRealTimeS);
                    this.CourseTimeE.DateValue = string.Format(DtFormat, this.entity.CourseTimeE);
                    this.CourseTimeS.DateValue = string.Format(DtFormat, this.entity.CourseTimeS);
                }
            }
            else
            {
                //this.SetInit();
            }


            
        }


        /// <summary>設定補登用餐記錄畫面</summary>
        public void SetInit()
        {
            try
            {
                this.ListEqu = this.odo.GetQueryResult<EquData>("SELECT * FROM V_McrInfo").ToList();
                //預設為當天的日期
                string DtFormat = "{0:yyyy/MM/dd 09:00:00}";
                string DtFormatE= "{0:yyyy/MM/dd 11:30:00}";
                this.CourseTimeS.DateValue = string.Format(DtFormat, DateTime.Now.AddDays(1));
                this.CourseRealTimeS.DateValue = string.Format(DtFormat, DateTime.Now.AddDays(1));
                this.CourseTimeE.DateValue = string.Format(DtFormatE, DateTime.Now.AddDays(1));
                this.CourseRealTimeE.DateValue = string.Format(DtFormatE, DateTime.Now.AddDays(1));
                this.entity.City = "A";
                this.entity.EquID = this.ListEqu.First().EquID;
                this.Cities = city.GetCities();
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }



        public void SaveData()
        {
            string[] keys = Request.Form.AllKeys;
            Dictionary<string, string> dicpara = new Dictionary<string, string>();
            foreach (var o in keys)
                dicpara.Add(o, Request.Form[o]);
            PropertyInfo[] properties = this.entity.GetType().GetProperties();
            foreach(PropertyInfo property in properties)
            {
                if (!dicpara.Any(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                    continue;

                KeyValuePair<string, string> item = dicpara.First(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));

                // Find which property type (int, string, double? etc) the CURRENT property is...
                Type tPropertyType = this.entity.GetType().GetProperty(property.Name).PropertyType;

                // Fix nullables...
                Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;
                if (newT.Name == "DateTime" && item.Value == "")
                {
                    continue;
                }
                // ...and change the type                
                object newA = Convert.ChangeType(item.Value, newT);
                entity.GetType().GetProperty(property.Name).SetValue(entity, newA, null);
            }
            this.entity.CourseTimeS = DateTime.Parse(Request["CourseTimeS$CalendarTextBox"]);
            this.entity.CourseTimeE= DateTime.Parse(Request["CourseTimeE$CalendarTextBox"]);
            this.entity.CourseRealTimeS= DateTime.Parse(Request["CourseRealTimeS$CalendarTextBox"]);
            this.entity.CourseRealTimeE= DateTime.Parse(Request["CourseRealTimeS$CalendarTextBox"]);
            string StrSqlCmd = @"INSERT INTO B03_Course (CourseNo,CourseName,CourseTimeS,CourseTimeE,CourseRealTimeS,CourseRealTimeE,City,CourseType
            ,Season,EquID,EquName,EquNo,CourseUnit,RealHour,DigitHour,CourseScore,CourseAmount,CourseProp) VALUES 
            (@CourseNo,@CourseName,@CourseTimeS,@CourseTimeE,@CourseRealTimeS,@CourseRealTimeE,@City,@CourseType,@Season,@EquID,@EquName
            ,@EquNo,@CourseUnit,@RealHour,@DigitHour,@CourseScore,@CourseAmount,@CourseProp) ";
            if (this.entity.CourseID != 0)
            {
                StrSqlCmd = @"UPDATE B03_Course SET CourseNo=@CourseNo,CourseName=@CourseName,CourseTimeS=@CourseTimeS,CourseTimeE=@CourseTimeE
                        ,CourseRealTimeS=@CourseRealTimeS,CourseREalTimeE=@CourseRealTimeE,City=@City,CourseType=@CourseType,Season=@Season,EquID=@EquID
                        ,CourseUnit=@CourseUnit,RealHour=@RealHour,DigitHour=@DigitHour,CourseScore=@CourseScore,CourseAmount=@CourseAmount,CourseProp=@CourseProp WHERE CourseID=@CourseID ";
            }
            this.odo.Execute(StrSqlCmd, this.entity);
            string messae = this.odo.DbExceptionMessage;
            Response.Clear();
            Response.Write("新增完成");
            Response.End();                        
        }


    }//end class
}//end namespace