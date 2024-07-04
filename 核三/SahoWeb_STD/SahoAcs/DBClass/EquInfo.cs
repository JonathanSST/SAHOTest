using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DB.DBInfo
{

    public class DciInfo
    {
        private int _DciID = 0;
        private string _DciNo = "";
        private string _DciName = "";
        private List<MasterInfo> _MasterList = new List<MasterInfo>();

        public int DciID { get { return this._DciID; } set { this._DciID = value; } }
        public string DciNo { get { return this._DciNo; } set { this._DciNo = value; } }
        public string DciName { get { return this._DciName; } set { this._DciName = value; } }
        public List<MasterInfo> MasterList { get { return this._MasterList; } set { this._MasterList = value; } }
    }

    public class MasterInfo
    {
        private int _MstID = 0;
        private string _MstNo = "";
        private string _MstDesc = "";
        private string _MstStatus = "";

        private int _ParentDciID = 0;
        private List<ControllerInfo> _ControllerList = new List<ControllerInfo>();

        public int MstID { get { return this._MstID; } set { this._MstID = value; } }
        public string MstNo { get { return this._MstNo; } set { this._MstNo = value; } }
        public string MstDesc { get { return this._MstDesc; } set { this._MstDesc = value; } }
        public string MstStatus { get { return this._MstStatus; } set { this._MstStatus = value; } }
        public int ParentDciID { get { return this._ParentDciID; } set { this._ParentDciID = value; } }

        public List<ControllerInfo> ControllerList { get { return this._ControllerList; } set { this._ControllerList = value; } }
    }

    public class ControllerInfo
    {
        private int _CtrlID = 0;
        private string _CtrlNo = "";
        private string _CtrlName = "";
        private string _CtrlStatus = "";

        private int _ParentMstID = 0;
        private List<ReaderInfo> _ReaderList = new List<ReaderInfo>();

        public int CtrlID { get { return this._CtrlID; } set { this._CtrlID = value; } }
        public string CtrlNo { get { return this._CtrlNo; } set { this._CtrlNo = value; } }
        public string CtrlName { get { return this._CtrlName; } set { this._CtrlName = value; } }
        public string CtrlStatus { get { return this._CtrlStatus; } set { this._CtrlStatus = value; } }
        public int ParentMstID { get { return this._ParentMstID; } set { this._ParentMstID = value; } }
        public List<ReaderInfo> ReaderList { get { return this._ReaderList; } set { this._ReaderList = value; } }
    }

    public class ReaderInfo
    {
        private int _ReaderID = 0;
        private string _ReaderNo = "";
        private string _ReaderName = "";
        private int _ParentCtrlID = 0;

        public int ReaderID { get { return this._ReaderID; } set { this._ReaderID = value; } }
        public string ReaderNo { get { return this._ReaderNo; } set { this._ReaderNo = value; } }
        public string ReaderName { get { return this._ReaderName; } set { this._ReaderName = value; } }
        public int ParentCtrlID { get { return this._ParentCtrlID; } set { this._ParentCtrlID = value; } }
    }


    public class IODciInfo
    {
        private int _DciID = 0;
        private string _DciNo = "";
        private string _DciName = "";
        private List<IOMasterInfo> _IOMasterList = new List<IOMasterInfo>();

        public int DciID { get { return this._DciID; } set { this._DciID = value; } }
        public string DciNo { get { return this._DciNo; } set { this._DciNo = value; } }
        public string DciName { get { return this._DciName; } set { this._DciName = value; } }
        public List<IOMasterInfo> IOMasterList { get { return this._IOMasterList; } set { this._IOMasterList = value; } }
    }

    public class IOMasterInfo
    {
        private int _IOMstID = 0;
        private string _IOMstNo = "";
        private string _IOMstName = "";
        private string _IOMstStatus = "";

        private int _ParentDciID = 0;
        private List<IOModuleInfo> _IOModuleList = new List<IOModuleInfo>();

        public int IOMstID { get { return this._IOMstID; } set { this._IOMstID = value; } }
        public string IOMstNo { get { return this._IOMstNo; } set { this._IOMstNo = value; } }
        public string IOMstName { get { return this._IOMstName; } set { this._IOMstName = value; } }
        public string IOMstStatus { get { return this._IOMstStatus; } set { this._IOMstStatus = value; } }
        public int ParentDciID { get { return this._ParentDciID; } set { this._ParentDciID = value; } }

        public List<IOModuleInfo> IOModuleList { get { return this._IOModuleList; } set { this._IOModuleList = value; } }
    }

    public class IOModuleInfo
    {
        private int _IOMID = 0;
        private string _IOMNo = "";
        private string _IOMName = "";
        private string _IOMStatus = "";
        private int _ParentIOMstID = 0;
        private List<SensorInfo> _SensorList = new List<SensorInfo>();

        public int IOMID { get { return this._IOMID; } set { this._IOMID = value; } }
        public string IOMNo { get { return this._IOMNo; } set { this._IOMNo = value; } }
        public string IOMName { get { return this._IOMName; } set { this._IOMName = value; } }
        public string IOMStatus { get { return this._IOMStatus; } set { this._IOMStatus = value; } }
        public int ParentIOMstID { get { return this._ParentIOMstID; } set { this._ParentIOMstID = value; } }
        public List<SensorInfo> SensorList { get { return this._SensorList; } set { this._SensorList = value; } }

    }

    public class SensorInfo
    {
        private int _SenID = 0;
        private string _SenNo = "";
        private string _SenName = "";
        private string _SenStatus = "";
        private int _ParentIOMID = 0;

        public int SenID { get { return this._SenID; } set { this._SenID = value; } }
        public string SenNo { get { return this._SenNo; } set { this._SenNo = value; } }
        public string SenName { get { return this._SenName; } set { this._SenName = value; } }
        public string SenStatus { get { return this._SenStatus; } set { this._SenStatus = value; } }
        public int ParentIOMID { get { return this._ParentIOMID; } set { this._ParentIOMID = value; } }
    }
}