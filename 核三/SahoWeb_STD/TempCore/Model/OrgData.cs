using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAnnotationMapper;

namespace TempCore.Model
{
    [TableName("B01_OrgData")]
    [PrimaryKey("OrgID")]
    public class OrgData
    {
        [Column("OrgClass")]
        public virtual string OrgClass { get; set; }
        [Column("OrgNo")]
        public virtual string OrgNo { get; set; }
        [Column("OrgID")]
        public virtual decimal OrgId { get; set; }
        [Column("OrgOrder")]
        public virtual int OrgOrder { get; set; }
        [Column("OrgName")]
        public virtual string OrgName { get; set; }
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
        [ResultColumn("ItemName")]
        public virtual string ItemName { get; set; }
    }
}
