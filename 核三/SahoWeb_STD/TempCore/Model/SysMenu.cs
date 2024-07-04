using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAnnotationMapper;

namespace TempCore.Model
{
    [TableName("B00_SysMenu")]    
    public class SysMenu
    {
        [Column("MenuNo")]
        public virtual string MenuNo { get; set; }
        [Column("UpMenuNo")]
        public virtual string UpMenuNo { get; set; }
        [Column("MenuName")]
        public virtual string MenuName { get; set; }
        [Column("MenuEName")]
        public virtual string MenuEName { get; set; }
        [Column("MenuDesc")]
        public virtual string MenuDesc { get; set; }
        [Column("MenuOrder")]
        public virtual byte MenuOrder { get; set; }
        [Column("IsAuthCtrl")]
        public virtual byte IsAuthCtrl { get; set; }
        [Column("MenuIsUse")]
        public virtual byte MenuIsUse { get; set; }
        [Column("MenuType")]
        public virtual string MenuType { get; set; }
        [Column("FunTrackType")]
        public virtual string FunTrackType { get; set; }
        [Column("FunTrack")]
        public virtual string FunTrack { get; set; }
        [Column("FunParameter")]
        public virtual string FunParameter { get; set; }
        [Column("FunAuthDef")]
        public virtual string FunAuthDef { get; set; }
        [Column("FunTarget")]
        public virtual string FunTarget { get; set; }
        [Column("FunIcon")]
        public virtual string FunIcon { get; set; }
        [Column("CreateTime")]
        public virtual DateTime CreateTime { get; set; }
        [Column("UpdateTime")]
        public virtual DateTime? UpdateTime { get; set; }
        [Column("Rev01")]
        public virtual int? Rev01 { get; set; }
        [Column("Rev02")]
        public virtual string Rev02 { get; set; }

        public string EditType { get; set; }
    }
}
