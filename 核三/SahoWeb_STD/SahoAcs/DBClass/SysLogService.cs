using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DapperDataObjectLib;
using SahoAcs.DBModel;

namespace SahoAcs.DBClass
{
    public static class SysLogService
    {
        public static string cmd_str = @"INSERT INTO B00_SysLog (LogTime,LogType,UserID,UserName,LogFrom,LogInfo,LogIP,LogDesc,EquNo,EquName) 
                        VALUES (GETDATE(),@LogType,@UserID,@UserName,@LogFrom,@LogInfo,@LogIP,@LogDesc,@EquNo,@EquName) ";
        public static void SetSysLogCreate(this OrmDataObject _odo,SysLogEntity _LogDto,IEnumerable<EquGroupLog> _equgrplogs,string action="")
        {
            foreach(var o in _equgrplogs)
            {
                _LogDto.LogInfo = string.Format("功能:'{0}',卡號:'{1}',群組編號:'{2}',群組ID:'{3}'",o.Action,o.CardNo,o.EquGrpNo,o.EquGrpID);
                _LogDto.LogTime = DateTime.Now;
                _odo.Execute(cmd_str, _LogDto);
            }
        }       

        public static void SetSysLogCreate(this OrmDataObject _odo, SysLogEntity _LogDto, IEnumerable<EquAdjLog> _equgrplogs, string action = "")
        {
            string equ_cmd_str = @"INSERT INTO B00_SysLog (LogTime,LogType,UserID,UserName,EquNo,EquName,LogFrom,LogInfo,LogIP) 
                                                            VALUES (GETDATE(),@LogType,@UserID,@UserName,@EquNo,@EquName,@LogFrom,@LogInfo,@LogIP) ";
            foreach (var o in _equgrplogs)
            {
                _LogDto.LogInfo = string.Format("作業模式:'{0}_{6}',卡號:'{1}',設備ID:'{2}',設備編號:'{3}',CardExtData:'{4}',CardRule:'{5}'", o.Action, o.CardNo, o.EquID, o.EquNo,o.CardExtData,o.CardRule
                    ,o.OpMode=="+"?"附加":"減少");
                if (o.EquClass == "Elevator")
                {
                    _LogDto.LogInfo += ", 目前樓層為 ";
                    var elevators = _odo.GetQueryResult("SELECT * FROM B01_ElevatorFloor WHERE EquID=@EquID", new { EquID = o.EquID });
                    var str16To2Ext = string.Join("", Sa.Change.HexToBin(o.CardExtData, 48).Reverse());
                    for (int i = 0; i < str16To2Ext.Length; i++)
                    {
                        if (str16To2Ext.Substring(i, 1) == "1" && i < elevators.Count())
                        {
                            _LogDto.LogInfo += "," + Convert.ToString(elevators.ElementAt(i).FloorName);
                        }
                    }
                }
                _LogDto.EquName = o.EquName;
                _LogDto.EquNo = o.EquNo;
                _odo.Execute(equ_cmd_str, _LogDto);
            }
        }

        public static void SetSysLogCreate(this OrmDataObject _odo,SysLogEntity _LogDto,IEnumerable<EquParaData> _equParalogs,string action = "")
        {
            string equ_cmd_str = @"INSERT INTO B00_SysLog (LogTime,LogType,UserID,UserName,EquNo,EquName,LogFrom,LogInfo,LogIP) 
                                                            VALUES (GETDATE(),@LogType,@UserID,@UserName,@EquNo,@EquName,@LogFrom,@LogInfo,@LogIP) ";
            foreach (var o in _equParalogs)
            {
                _LogDto.LogInfo = string.Format("修改設備參數，設備編號:{0}，參數名稱:{2}，參數值:{3}",o.EquNo,o.EquName,o.ParaName,o.ParaValue);
                if (o.ParaName=="ElevCtrlOnOpen" || o.ParaName == "ElevAlwysOpen")
                {
                    if (o.ParaName == "")
                    {
                        _LogDto.LogInfo += ", 管制模式下開放樓層為 ";
                    }
                    else
                    {
                        _LogDto.LogInfo += ", 開放模式下開放樓層為 ";
                    }
                    var elevators = _odo.GetQueryResult("SELECT * FROM B01_ElevatorFloor WHERE EquID=@EquID", new { EquID = o.EquID });
                    var str16To2Ext = string.Join("", Sa.Change.HexToBin(o.ParaValue, 48).Reverse());
                    for (int i = 0; i < str16To2Ext.Length; i++)
                    {
                        if (str16To2Ext.Substring(i, 1) == "1" && i < elevators.Count())
                        {
                            _LogDto.LogInfo += "," + Convert.ToString(elevators.ElementAt(i).FloorName);
                        }
                    }
                }
                _LogDto.EquName = o.EquName;
                _LogDto.EquNo = o.EquNo;
                _odo.Execute(equ_cmd_str, _LogDto);
            }
            
        }


        /// <summary>依照 CardEquGroup 或 CardEquAdj 建立所有記錄</summary>
        /// <param name="_odo"></param>
        /// <param name="_LogDto"></param>
        /// <param name="_CardID"></param>
        /// <param name="_CardNo"></param>
        public static void SetSysLogCreateByEquAuth(this OrmDataObject _odo, SysLogEntity _LogDto,int _CardID,string _CardNo)
        {
            _LogDto.LogType = DB_Acs.Logtype.重設卡片權限.ToString();
            _LogDto.LogDesc = _LogDto.LogType;
            _LogDto.LogInfo = string.Format("卡號{0} 開始重設權限",_CardNo);
            _odo.Execute(cmd_str, _LogDto);
            _LogDto.LogType = DB_Acs.Logtype.卡片權限調整.ToString();
            _LogDto.LogDesc = _LogDto.LogType;
            var grouplist = _odo.GetQueryResult<EquGroupData>(@"SELECT 
                            * FROM B01_EquGroup 
                            WHERE EquGrpID IN (SELECT EquGrpID FROM B01_CardEquGroup WHERE CardID=@CardID)",new { CardID=_CardID});
            var equlist = _odo.GetQueryResult<EquAdjLog>(@"SELECT 
	                                                                                                            A.*,B.CardExtData,B.CardRule,
	                                                                                                            CASE B.OpMode WHEN '+' THEN '增加' ELSE '減少' END AS OpMode
                                                                                                            FROM 
	                                                                                                            B01_EquData A
	                                                                                                            INNER JOIN B01_CardEquAdj B ON A.EquID=B.EquID WHERE CardID=@CardID", new {CardID=_CardID });
            var equlistByGroup = _odo.GetQueryResult<EquAdjLog>(@"SELECT 
	                                                                                        C.*,B.CardExtData,B.CardRule
                                                                                        FROM 
	                                                                                        B01_EquGroup A
	                                                                                        INNER JOIN B01_EquGroupData B ON A.EquGrpID=B.EquGrpID
	                                                                                        INNER JOIN B01_EquData C ON B.EquID=C.EquID
                                                                                        WHERE 
	                                                                                        A.EquGrpID IN (SELECT EquGrpID FROM B01_CardEquGroup WHERE CardID=@CardID)",new {CardID=_CardID });
            foreach(var o in grouplist)
            {
                _LogDto.LogInfo = string.Format("卡號{0} 加入設備群組{1},{2}",_CardNo,o.EquGrpNo,o.EquGrpName);
                _odo.Execute(cmd_str, _LogDto);
            }
            foreach (var o in equlistByGroup)
            {
                _LogDto.EquName = o.EquName;
                _LogDto.EquNo = o.EquNo;
                _LogDto.LogInfo = string.Format("卡號{0} {3}設備{1},{2}", _CardNo, o.EquNo, o.EquName, "增加");
                _odo.Execute(cmd_str, _LogDto);
                if (o.EquClass == "Elevator"&&o.CardExtData!="")
                {
                    var elevators = _odo.GetQueryResult("SELECT * FROM B01_ElevatorFloor WHERE EquID=@EquID", new { EquID = o.EquID });
                    var str16To2Ext = string.Join("", Sa.Change.HexToBin(o.CardExtData, 48).Reverse());
                    for (int i = 0; i < str16To2Ext.Length; i++)
                    {
                        if (str16To2Ext.Substring(i, 1) == "1" && i < elevators.Count())
                        {
                            _LogDto.LogInfo = string.Format("卡號{0} 在設備{1},{2}，{3}電梯樓層{4}", _CardNo, o.EquNo, o.EquName, "增加", Convert.ToString(elevators.ElementAt(i).FloorName));
                            _odo.Execute(cmd_str, _LogDto);
                        }
                    }//end 電梯參數 for
                }//end 判斷電梯 if Elevator
            }//end 設備調整foreach
            foreach (var o in equlist)
            {
                _LogDto.EquName = o.EquName;
                _LogDto.EquNo = o.EquNo;
                _LogDto.LogInfo = string.Format("卡號{0} {3}設備{1},{2}", _CardNo, o.EquNo, o.EquName,o.OpMode);
                _odo.Execute(cmd_str, _LogDto);
                if (o.EquClass == "Elevator"&&o.CardExtData!="")
                {
                    var elevators = _odo.GetQueryResult("SELECT * FROM B01_ElevatorFloor WHERE EquID=@EquID", new { EquID = o.EquID });
                    var str16To2Ext = string.Join("", Sa.Change.HexToBin(o.CardExtData, 48).Reverse());
                    for (int i = 0; i < str16To2Ext.Length; i++)
                    {
                        if (str16To2Ext.Substring(i, 1) == "1" && i < elevators.Count())
                        {
                            _LogDto.LogInfo = string.Format("卡號{0} 在設備{1},{2}，{3}電梯樓層{4}", _CardNo, o.EquNo, o.EquName, o.OpMode, Convert.ToString(elevators.ElementAt(i).FloorName));
                            _odo.Execute(cmd_str, _LogDto);
                        }
                    }//end 電梯參數 for
                }//end 判斷電梯 if Elevator
            }//end 設備調整foreach
        }//end method

        public static void SetSysLogCreate(this OrmDataObject _odo, SysLogEntity _LogDto)
        {          
            _odo.Execute(cmd_str, _LogDto);
        }


        /// <summary>卡號對設備及設備群組的異動記錄</summary>
        /// <param name="_odo"></param>
        /// <param name="_CardID">CardID</param>
        /// <param name="_CardNo">CardNo</param>
        /// <param name="EquGroupOrDataNo">設備</param>
        /// <param name="action">異動</param>
        public static void SetSysLogCreateByNo(this OrmDataObject _odo,int? _CardID,string _CardNo,string EquGroupOrDataNo,string action,string LogFrom)
        {
            SysLogEntity log = new DBModel.SysLogEntity();
            log.LogType = DB_Acs.Logtype.卡片一般權限調整.ToString();
            log.UserName = HttpContext.Current.Session["UserName"].ToString();
            log.UserID = HttpContext.Current.Session["UserID"].ToString();
            log.EquName = EquGroupOrDataNo;
            log.EquNo = EquGroupOrDataNo;
            log.LogInfo = string.Format("卡號{0}, {1}, 設備或設備群組編號 {2}", _CardNo, action, EquGroupOrDataNo);
            log.LogDesc = HttpContext.Current.Request.Url.PathAndQuery+" 卡片一般設備權限調整";
            log.LogIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            log.LogFrom = LogFrom;
            _odo.Execute(cmd_str, log);
        }

        public static List<EquAdjLog> GetEquAdjLogsPackage(this List<EquAdj> adjs,string action,string CardNo)
        {
            List<EquAdjLog> logs = new List<EquAdjLog>();
            foreach(var o in adjs)
            {
                logs.Add(new DBModel.EquAdjLog()
                {
                    EquID = o.EquID,
                    Action = action,
                    EquNo = o.EquNo,
                    EquName = o.EquName,
                    CardExtData = o.CardExtData,
                    CardRule = o.CardRule,
                    CardNo = CardNo,
                    OpMode = o.OpMode,
                    EquClass=o.EquClass                              
                });
            }
            return logs;
        }

       

        public static SysLogEntity GetSysLogEntityPackage(DB_Acs.Logtype log_type,string user_id,string user_name,string log_from)
        {            
            SysLogEntity log = new DBModel.SysLogEntity();
            log.UserName = user_name;
            log.UserID = user_id;
            log.LogType = log_type.ToString();
            log.LogFrom = log_from;
            log.LogIP= HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            return log;
        }

       

    }//end class
}//end namesapce