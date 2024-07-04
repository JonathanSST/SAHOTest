using System;

namespace SahoAcs.DBModel
{
    public partial class B00Sysuser
    {


        public Decimal RecordID { get; set; }


        public String UserID { get; set; }


        public String UserPW { get; set; }


        public String UserName { get; set; }


        public String UserEName { get; set; }


        public String UserPWCtrl { get; set; }


        public DateTime UserSTime { get; set; }


        public DateTime? UserETime { get; set; }


        public DateTime? PWChgTime { get; set; }


        public Byte IsOptAllow { get; set; }


        public String EMail { get; set; }


        public String Remark { get; set; }


        public String OwnerID { get; set; }


        public String OwnerList { get; set; }


        public String CreateUserID { get; set; }


        public DateTime CreateTime { get; set; }


        public String UpdateUserID { get; set; }


        public DateTime? UpdateTime { get; set; }


        public Int32? Rev01 { get; set; }


        public String Rev02 { get; set; }

        public string OwnUserID { get; set; }

        public int MgaID { get; set; }
        public string MgaNo { get; set; }
        public string MgaName { get; set; }
        public string MgaEName { get; set; }

    }//end entity class
}//end namespace