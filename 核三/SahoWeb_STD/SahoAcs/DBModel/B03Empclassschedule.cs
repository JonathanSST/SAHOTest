using System;

namespace SahoAcs.DBModel
{
    public class B03Empclassschedule:BaseTableEntity
    {


        public Decimal RecordID { get; set; }


        public String PsnNo { get; set; }
        public string PsnName { get; set; }
        public string ClassNo1 { get; set; }
        public string ClassNo2 { get; set; }


        public String ClassMonth { get; set; }


        public String ClassNo { get; set; }


        public DateTime ClassDate { get; set; }


        public String ClassMemo { get; set; }


        public String AdjClassNo { get; set; }
     

    }
}