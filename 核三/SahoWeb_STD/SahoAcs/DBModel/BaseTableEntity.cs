using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SahoAcs.DBModel
{
    public class BaseTableEntity
    {
        public string CreateUserID { get; set; }
        public DateTime CreateTime { get; set; }
        public string UpdateUserID { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
