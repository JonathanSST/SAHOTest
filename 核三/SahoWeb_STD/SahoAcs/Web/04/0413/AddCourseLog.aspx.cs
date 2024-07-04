using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;

namespace SahoAcs.Web
{
    public partial class AddCourseLog : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public CardEntity entity = new CardEntity();
        public CardLogFillData cardlog = new CardLogFillData();
        public List<CourseEntity> ListCourse = new List<CourseEntity>();

        protected void Page_Load(object sender, EventArgs e)
        {
            this.SetInit();
            if (Request["PageEvent"] != null && Request["PageEvent"]=="Add")
            {
                this.ListCourse = this.odo.GetQueryResult<CourseEntity>(@"SELECT CourseNo,CourseName,CourseID,City,CourseType,Season,B.EquID,B.EquName,B.EquNo,CourseUnit,RealHour,DigitHour,CourseScore,CourseAmount,CourseProp 
                FROM B03_Course A
                    INNER JOIN B01_EquData B ON A.EquID=B.EquID WHERE CourseTimeS  BETWEEN @DateS AND @DateE ", new {DateS=Request["DateS"],DateE=Request["DateE"]}).ToList();
            }
            else if(Request["PageEvent"]!=null && Request["PageEvent"] == "Save")
            {
                var CourseList = this.odo.GetQueryResult<CourseEntity>(@"SELECT CourseNo,CourseName,CourseID,City,CourseType,Season,B.EquID,B.EquName,B.EquNo
                    ,CourseUnit,RealHour,DigitHour,CourseScore,CourseAmount,CourseProp,CourseTimeS
                    FROM B03_Course A
                    INNER JOIN B01_EquData B ON A.EquID=B.EquID WHERE CourseID=@CourseID ",new {CourseID=Request["CourseID"]});
                var CardList = this.odo.GetQueryResult("SELECT * FROM B01_Card WHERE CardID=@CardID", new { CardID = Request["CardID"] });
                if(CourseList.Count()>0 && CardList.Count() > 0)
                {
                    string EquNo = CourseList.First().EquNo;
                    string CardNo = CardList.First().CardNo;
                    DateTime CardTime = CourseList.First().CourseTimeS.AddMinutes(5);
                    this.odo.Execute("INSERT INTO B01_CardLog (CardNo,SyncMark3,EquNo,CardTime,LogTime,EquDir) VALUES (@CardNo,'2',@EquNo,@CardTime,GETDATE(),'進')",
                        new {CardNo=CardNo,EquNo=EquNo,CardTime=CardTime});
                }
                
            }
            else
            {
                
            }


            
        }


        /// <summary>設定補登用餐記錄畫面</summary>
        public void SetInit()
        {
            try
            {
            

            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }



        public void SaveData()
        {
            
                Response.Clear();
                Response.Write("新增完成");
                Response.End();
            
            
        }
    

    }//end class
}//end namespace