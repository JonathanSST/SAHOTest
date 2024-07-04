using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAnnotationMapper;

namespace TempCore.Model
{
    [TableName("B01_Person")]
    [PrimaryKey("PsnId")]
    public class Person
    {   
        [Column("PsnNo")]
        public virtual string PsnNo { get; set; }        
        [Column("PsnId")]
        public virtual decimal PsnId { get; set; }
        [Column("PsnNum")]
        public virtual string PsnNum { get; set; }
        [Column("PsnName")]
        public virtual string PsnName { get; set; }
        [Column("PsnEName")]
        public virtual string PsnEName { get; set; }
        [Column("PsnType")]
        public virtual string PsnType { get; set; }
        [Column("PsnPicSource")]
        public virtual string PsnPicSource { get; set; }
        [Column("PsnAccount")]
        public virtual string PsnAccount { get; set; }
        [Column("PsnPW")]
        public virtual string PsnPw { get; set; }
        [Column("PsnSTime")]
        public virtual DateTime PsnSTime { get; set; }
        [Column("PsnETime")]
        public virtual DateTime? PsnETime { get; set; }
        [Column("PsnAuthAllow")]
        public virtual byte PsnAuthAllow { get; set; }
        [Column("Sex")]
        public virtual string Sex { get; set; }
        [Column("Birthday")]
        public virtual DateTime? Birthday { get; set; }
        [Column("Remark")]
        public virtual string Remark { get; set; }
        [Column("OrgStrucNo")]
        public virtual string OrgStrucNo { get; set; }
        [Column("RowStatus")]
        public virtual byte RowStatus { get; set; }
        [Column("CreateUserID")]
        public virtual string CreateUserId { get; set; }
        [Column("CreateTime")]
        public virtual DateTime CreateTime { get; set; }
        [Column("UpdateUserID")]
        public virtual string UpdateUserId { get; set; }
        [Column("UpdateTime")]
        public virtual DateTime? UpdateTime { get; set; }
        [Column("Rev01")]
        public virtual int? Rev01 { get; set; }
        [Column("Rev02")]
        public virtual string Rev02 { get; set; }
    }
}
