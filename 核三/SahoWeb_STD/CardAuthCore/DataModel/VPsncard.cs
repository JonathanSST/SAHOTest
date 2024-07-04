using System;

namespace CardAuthCore.DataModel
{
  public class VPsncard
  {
     

       public virtual Decimal PsnID { get; set; }


       public virtual String PsnNo { get; set; }


       public virtual String PsnName { get; set; }


       public virtual String PsnEName { get; set; }


       public virtual String PsnType { get; set; }


       public virtual String IDNum { get; set; }


       public virtual DateTime? Birthday { get; set; }


       public virtual Decimal OrgStrucID { get; set; }


       public virtual String PsnAccount { get; set; }


       public virtual String PsnPW { get; set; }


       public virtual Byte PsnAuthAllow { get; set; }


       public virtual DateTime PsnSTime { get; set; }


       public virtual DateTime? PsnETime { get; set; }


       public virtual String PsnPicSource { get; set; }


       public virtual String Remark { get; set; }


       public virtual Decimal CardID { get; set; }


       public virtual String CardNo { get; set; }


       public virtual String CardVer { get; set; }


       public virtual String CardPW { get; set; }


       public virtual String CardSerialNo { get; set; }


       public virtual String CardNum { get; set; }


       public virtual String CardType { get; set; }


       public virtual Byte CardAuthAllow { get; set; }


       public virtual DateTime CardSTime { get; set; }


       public virtual DateTime? CardETime { get; set; }


       public virtual String CardDesc { get; set; }


       public virtual Int32? PsnRev01 { get; set; }


       public virtual String PsnRev02 { get; set; }


       public virtual Int32? CardRev01 { get; set; }


       public virtual String CardRev02 { get; set; }


       public virtual Int32 VerifiMode { get; set; }

  }
}