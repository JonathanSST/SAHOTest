using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardAuthCore.DataModel
{
    public class SahoFPData
    {
        public string CardNo { get; set; }
        public int FPAmount { get; set; }
        public string FPData { get; set; }

        public string FPScore { get; set; }

        public string FPTemplateType { get; set; }


    }//end entity class
}//end namespace
