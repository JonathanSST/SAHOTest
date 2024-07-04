using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardAuthCore.DataModel
{
    public class VCardAuthProc
    {

        public String DciNo { get; set; }


        public String MstConnParam { get; set; }


        public String CtrlNo { get; set; }


        public Int32 CtrlAddr { get; set; }


        public String CtrlModel { get; set; }


        public String ReaderNo { get; set; }

        public string ReadNo { get; set; }

        public String EquID { get; set; }


        public String EquNo { get; set; }
        public string EquName { get; set; }
        public string EquIDList { get; set; }
        public string EquNoList { get; set; }



        public String CardNo { get; set; }


        public String CardVer { get; set; }


        public String CardPW { get; set; }


        public String CardRule { get; set; }


        public String CardExtData { get; set; }


        public DateTime? BeginTime { get; set; }


        public DateTime? EndTime { get; set; }


        public String OpMode { get; set; }
        public string OpStatus { get; set; }
        public int ErrCnt { get; set; }
        public int TimeState { get; set; }

        public Int32? FPAmount { get; set; }


        public String FPData { get; set; }


        public String FPScore { get; set; }


        public String CardType { get; set; }


        public String IDNum { get; set; }


        public int VerifiMode { get; set; }

        public string VerifiModeVar { get; set; }

        public String PsnNo { get; set; }


        public String PsnName { get; set; }

        public string EquClass { get; set; }

        public Int32 Mark { get; set; }


    }//end entity class
}//end namespace
