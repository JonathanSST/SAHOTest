using iTextSharp.text.pdf;
using SahoAcs.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DB.DBInfo
{
    public class CtrlAreaInfo 
    {
        private string _ExceptionMsg = "";

        private int _CtrlAreaID = -1;        //區域ID
        private string _CtrlAreaNo = "";        //區域代碼
        private string _CtrlAreaName = "";        //區域名稱
        private int _CtrlAreaPID = 0;        //上層區域ID
        private int _IsControl = 1;   //是否管制

        private string _CtrlAreaDesc = "";        //說明
        private List<EquParaData> _EquInList = new List<EquParaData>();        //進入讀卡機列表
        private List<EquParaData> _EquOutList = new List<EquParaData>();        //離開讀卡機列表
        private int _Level = 1;
        private CtrlAreaInfo _ParentCtrlArea = null;        //父區域
        private List<CtrlAreaInfo> _SubCtrlAreas = new List<CtrlAreaInfo>();        //子區域列表


        public int CtrlAreaID { get { return this._CtrlAreaID; } set { this._CtrlAreaID = value; } }
        public string CtrlAreaNo { get { return this._CtrlAreaNo; } set { this._CtrlAreaNo = value; } }
        public string CtrlAreaName { get { return this._CtrlAreaName; } set { this._CtrlAreaName = value; } }
        public int CtrlAreaPID { get { return this._CtrlAreaPID; } set { this._CtrlAreaPID = value; } }
        public int IsControl { get { return this._IsControl; } set { this._IsControl = value; } }
        public string CtrlAreaDesc { get { return this._CtrlAreaDesc; } set { this._CtrlAreaDesc = value; } }
        public List<EquParaData> EquInList { get { return this._EquInList; } set { this._EquInList = value; } }
        public List<EquParaData> EquOutList { get { return this._EquOutList; } set { this._EquOutList = value; } }
        public int Level { get { return this._Level; } set { this._Level = value; } }
        public CtrlAreaInfo ParentMgrArea { get { return this._ParentCtrlArea; } set { this._ParentCtrlArea = value; } }
        public List<CtrlAreaInfo> SubMgrAreas { get { return this._SubCtrlAreas; } set { this._SubCtrlAreas = value; } }
    }


}