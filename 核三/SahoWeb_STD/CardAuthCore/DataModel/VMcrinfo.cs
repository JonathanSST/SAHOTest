using System;

namespace CardAuthCore.DataModel
{
    public class VMcrinfo
    {


        public Decimal DciID { get; set; }


        public String DciNo { get; set; }


        public String DciName { get; set; }


        public Byte? IsAssignIP { get; set; }


        public String IpAddress { get; set; }


        public Int32? TcpPort { get; set; }


        public String DciPassWD { get; set; }


        public Decimal MstID { get; set; }


        public String MstNo { get; set; }


        public String MstDesc { get; set; }


        public String MstType { get; set; }


        public String MstConnParam { get; set; }


        public String MCtrlModel { get; set; }


        public Byte LinkMode { get; set; }


        public Byte AutoReturn { get; set; }


        public String MstModel { get; set; }


        public String MstFwVer { get; set; }


        public Byte MstStatus { get; set; }


        public Decimal CtrlID { get; set; }


        public String CtrlNo { get; set; }


        public String CtrlName { get; set; }


        public String CtrlDesc { get; set; }


        public Byte CtrlAddr { get; set; }


        public String CCtrlModel { get; set; }


        public String CtrlFwVer { get; set; }


        public Byte CtrlStatus { get; set; }


        public Decimal ReaderID { get; set; }


        public string ReaderNo { get; set; }


        public String ReaderName { get; set; }


        public String ReaderDesc { get; set; }


        public Decimal EquID { get; set; }


        public String EquNo { get; set; }


        public String Dir { get; set; }


        public Byte CardNoLen { get; set; }


        public String EquClass { get; set; }


        public Byte IsAndTrt { get; set; }


        public String Building { get; set; }


        public String Floor { get; set; }


        public Decimal InToCtrlAreaID { get; set; }


        public String EquName { get; set; }


        public Decimal OutToCtrlAreaID { get; set; }

    }
}