using System;

namespace SahoAcs.DBModel
{
    public partial class NewsInfoEntity
    {
        

       public Int32 NewsID { get; set; }


       public String NewsTitle { get; set; }


       public String NewsContent { get; set; }


       public DateTime NewsDate { get; set; }


       public String CreateUserID { get; set; }


       public DateTime? CreateTime { get; set; }


       public String UpdateUserID { get; set; }


       public DateTime? UpdateTime { get; set; }

    }
}