using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAnnotationMapper;

namespace TempCore.Model
{
    public class ItemClass
    {
        [ResultColumn("ItemNo")]
        public string ItemNo { get; set; }
        [ResultColumn("ItemName")]
        public string ItemName { get; set; }
        [ResultColumn("ItemInfo2")]
        public string ItemInfo { get; set; }
    }
}
