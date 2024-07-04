using System;

namespace SahoAcs.DBModel
{
  public class B00Sysdeviceoplog
  {
     

       public virtual Decimal RecordID { get; set; }


       public virtual String DOPType { get; set; }


       public virtual String DOPActive { get; set; }


       public virtual String DOPState { get; set; }


       public virtual String EquNo { get; set; }


       public virtual String IPAddress { get; set; }


       public virtual String ResultMsg { get; set; }


       public virtual Byte Readed { get; set; }


       public virtual String CreateUserID { get; set; }


       public virtual DateTime CreateTime { get; set; }


       public virtual Int32? Rev01 { get; set; }


       public virtual String Rev02 { get; set; }

  }
}