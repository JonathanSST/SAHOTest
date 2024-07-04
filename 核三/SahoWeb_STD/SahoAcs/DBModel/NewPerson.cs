using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class NewPerson
    {
        public string PsnNo { get; set; }
        public string PsnName { get; set; }
        public string CardNo { get; set; }
        public string UnitNo { get; set; }
        public string Unit { get; set; }
        public string TitleNo { get; set; }
        public string Title { get; set; }
        public string PsnSTime { get; set; }
        public string PsnETime { get; set; }
        public List<EquGroupModel> EquGroupNoList { get; set; }
        public int OrgStrucID { get; set; }
    }
}