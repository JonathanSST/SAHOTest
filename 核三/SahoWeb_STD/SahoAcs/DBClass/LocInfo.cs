using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DB.DBInfo
{

    public class LocInfo
    {
        private int _LocID = 0;
        private int _LocPID = 0;
        private string _LocType = "";
        private string _LocNo = "";
        private string _LocName = "";
        private string _LocPList = "";
        private List<SubLocInfo> _SubLocInfo = new List<SubLocInfo>();

        public int LocID { get { return this._LocID; } set { this._LocID = value; } }
        public int LocPID { get { return this._LocPID; } set { this._LocPID = value; } }
        public string LocType { get { return this._LocType; } set { this._LocType = value; } }
        public string LocNo { get { return this._LocNo; } set { this._LocNo = value; } }
        public string LocName { get { return this._LocName; } set { this._LocName = value; } }
        public string LocPList { get { return this._LocPList; } set { this._LocPList = value; } }
        public List<SubLocInfo> SubLocInfo { get { return this._SubLocInfo; } set { this._SubLocInfo = value; } }

    }


    public class SubLocInfo
    {
        private int _LocID = 0;
        private int _LocPID = 0;
        private string _LocType = "";
        private string _LocNo = "";
        private string _LocName = "";
        private string _LocPList = "";


        public int LocID { get { return this._LocID; } set { this._LocID = value; } }
        public int LocPID { get { return this._LocPID; } set { this._LocPID = value; } }
        public string LocType { get { return this._LocType; } set { this._LocType = value; } }
        public string LocNo { get { return this._LocNo; } set { this._LocNo = value; } }
        public string LocName { get { return this._LocName; } set { this._LocName = value; } }
        public string LocPList { get { return this._LocPList; } set { this._LocPList = value; } }
     
    }

    //public class LocInfoThred
    //{
    //    private int _LocID = 0;
    //    private int _LocPID = 0;
    //    private string _LocType = "";
    //    private string _LocNo = "";
    //    private string _LocName = "";
    //    private string _LocPList = "";

    //    public int LocID { get { return this._LocID; } set { this._LocID = value; } }
    //    public int LocPID { get { return this._LocPID; } set { this._LocPID = value; } }
    //    public string LocType { get { return this._LocType; } set { this._LocType = value; } }
    //    public string LocNo { get { return this._LocNo; } set { this._LocNo = value; } }
    //    public string LocName { get { return this._LocName; } set { this._LocName = value; } }
    //    public string LocPList { get { return this._LocPList; } set { this._LocPList = value; } }
    //}
}