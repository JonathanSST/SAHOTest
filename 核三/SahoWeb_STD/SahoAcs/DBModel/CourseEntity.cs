using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class CourseEntity
    {

        public int CourseID { get; set; }
        public string CourseNo { get; set; }
        public string CourseName { get; set; }
        public DateTime CourseTimeS { get; set; }
        public DateTime CourseTimeE { get; set; }
        public string CourseTs { get; set; }
        public string CourseTe { get; set; }
        public DateTime CourseRealTimeS { get; set; }
        public DateTime CourseRealTimeE { get; set; }
        public string City { get; set; }
        public string CourseType { get; set; }        
        public string Season { get; set; }
        public int EquID { get; set; }
        public string EquName { get; set; }
        public string EquNo { get; set; }
        
        public string CourseUnit { get; set; }
        public int RealHour { get; set; }
        public int DigitHour { get; set; }
        public string CourseScore { get; set; }
        public int CourseAmount { get; set; }
        public string CourseProp { get; set; }


        #region 與讀卡記錄相關的property

        public string PsnNo { get; set; }
        public string CardNo { get; set; }
        public string PsnName { get; set; }
        public string CardTime { get; set; }

        #endregion

        #region 其他欄位
        public int TrainScore { get; set; }
        public string LicenceNo { get; set; }
        public string TrainStatu { get; set; }
        public string BirthDay { get; set; }
        #endregion


    }//end entity class
}//end namespace