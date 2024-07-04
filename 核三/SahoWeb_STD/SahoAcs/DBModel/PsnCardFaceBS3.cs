using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class PsnCardFaceBS3
    {
        public string PsnNo { get; set; }
        public string PsnName { get; set; }
        public string CardNo { get; set; }
        public string CardNoFace { get; set; }
        public int FaceAmount { get; set; }
        public byte[] ImageData { get; set; }
        public int ImageLen { get; set; }
        public byte[] TemplateData { get; set; }
        public int TemplateDataLen { get; set; }
        public int NumOfTemplate { get; set; }
        public int FaceKey { get; set; }
        public int Flag { get; set; }
        public int SecurityLevel { get; set; }
        public int FaceType { get; set; }
        public int CardType { get; set; }
        public int IDType { get; set; }
        public int AuthType { get; set; }
        public int UserType { get; set; }
        public byte[] ImageData2 { get; set; }
        public int ImageLen2 { get; set; }
        public int OrgStrucID { get; set; }

    }
}