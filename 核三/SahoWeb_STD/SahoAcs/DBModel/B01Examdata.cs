using System;

namespace SahoAcs.DBModel
{
    public partial class B01Examdata
    {
        

       public Decimal ExamID { get; set; }


       public String ExamNo { get; set; }


       public String ExamName { get; set; }


       public String OrgNo { get; set; }


       public DateTime ExamBeginTime { get; set; }


       public DateTime ExamEndTime { get; set; }


       public String CreateUserID { get; set; }


       public DateTime CreateTime { get; set; }


       public String UpdateUserID { get; set; }


       public DateTime? UpdateTime { get; set; }

    }
}