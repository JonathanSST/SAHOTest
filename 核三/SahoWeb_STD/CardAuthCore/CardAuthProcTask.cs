using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Data;
using DapperDataObjectLib;
using System.Configuration;
using System.ComponentModel;
using System.IO;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using System.Reflection;
using CardAuthCore.DataModel;


namespace CardAuthCore
{
    public class CardAuthProcTask
    {
        public static string ConnString { get; set; }


        public static string GetCardAuthProc(string CardNo, List<int> EquList)
        {

            OrmDataObject odo = new OrmDataObject("MsSql", ConnString);
            List<VPsncard> PsnCards = odo.GetQueryResult<VPsncard>("SELECT * FROM V_PsnCard WHERE CardNo=@CardNo", new { CardNo = CardNo }).ToList();
            List<VCardAuthProc> CardAuthData = odo.GetQueryResult<VCardAuthProc>("SELECT * FROM B01_CardAuth WHERE CardNo=@CardNo AND OpMode<>'Del' ", new { CardNo = CardNo }).ToList();
            List<VCardAuthProc> DelAuthData = odo.GetQueryResult<VCardAuthProc>("SELECT * FROM B01_CardAuth WHERE CardNo=@CardNo AND OpMode='Del' ", new { CardNo = CardNo }).ToList();
            List<VCardAuthProc> CardAuthList = new List<VCardAuthProc>();
            foreach (var o in PsnCards)
            {
                List<VMcrinfo> McrList = odo.GetQueryResult<VMcrinfo>("SELECT * FROM V_McrInfo").ToList();
                List<SahoFPData> FpList = odo.GetQueryResult<SahoFPData>("SELECT * FROM B02_FPData WHERE CardNo=@CardNo", new { CardNo = o.CardNo }).ToList();
                string EquAdjListCmd = @"SELECT * FROM (SELECT 
	                                                                MCR.EquClass,EGD.EquID,EquNo,CardRule,CardExtData,DciNo,CtrlNo,MstConnParam,ReaderNo,CCtrlModel AS CtrlModel,CtrlAddr,'+' AS OpMode,NULL AS BeginTime,NULL AS EndTime 
                                                                FROM 
	                                                                B01_CardEquGroup CEG 
	                                                                INNER JOIN B01_EquGroupData EGD ON CEG.EquGrpID=EGD.EquGrpID
	                                                                INNER JOIN V_MCRInfo MCR ON MCR.EquID=EGD.EquID
                                                                WHERE EGD.EquID NOT IN (SELECT EquID FROM B01_CardEquAdj WHERE CardID=@CardID)
                                                                AND CardID=@CardID
                                                                UNION
                                                                SELECT 
	                                                                MCR.EquClass,MCR.EquID,EquNo,CardRule,CardExtData,DciNo,CtrlNo,MstConnParam,ReaderNo,CCtrlModel AS CtrlModel,CtrlAddr,
                                                                    CASE OpMode WHEN '*' THEN '+' ELSE OpMode END AS OpMode,BeginTime,EndTime  
                                                                FROM 
	                                                                B01_CardEquAdj CEA
	                                                                INNER JOIN V_MCRInfo MCR ON CEA.EquID=MCR.EquID                                                                                                                              
                                                                WHERE CardID=@CardID ) AS MCR ";
                if (EquList.Count > 0)
                {
                    EquAdjListCmd += " WHERE MCR.EquID IN @EquList ";
                }
                List<VCardAuthProc> EquAdjList = odo.GetQueryResult<VCardAuthProc>(EquAdjListCmd, new { CardID = o.CardID, EquList = EquList }).ToList();
                //整理電梯權限
                foreach (string equid in EquAdjList.Where(i =>i.OpMode=="+" && i.EquClass == "Elevator").Select(i => i.EquID).Distinct())
                {
                    Int64 CardExtAmt = 0;
                    foreach (var floor in EquAdjList.Where(i => i.EquID == equid).Select(i => i.CardExtData))
                    {
                        CardExtAmt = CardExtAmt | Convert.ToInt64(Convert.ToString(Convert.ToInt64(floor, 16), 2), 2);
                    }
                    EquAdjList.ForEach(i =>
                    {
                        if (i.EquID == equid)
                        {
                            i.CardExtData = Convert.ToString(CardExtAmt, 16).ToUpper().PadLeft(12, '0');
                        }
                    });
                }
                EquAdjList = EquAdjList.GroupBy(i => i.EquNo).Select(i => i.First()).ToList();
                if (o.PsnETime < o.CardETime)
                {
                    o.CardETime = o.PsnETime;
                }
                if (o.CardSTime < o.PsnSTime)
                {
                    o.CardSTime = o.PsnSTime;
                }
                if (o.CardPW == null || o.CardPW == "")
                {
                    o.CardPW = "0000";
                }
                if (o.CardVer == null)
                {
                    o.CardVer = "";
                }
                //設定沒權限的McrInfo List 資料至權限清單
                foreach (var mcr in McrList.Where(i => !EquAdjList.Select(j => j.EquNo).Contains(i.EquNo) && DelAuthData.Select(j => decimal.Parse(j.EquID)).Contains(i.EquID)))
                {
                    EquAdjList.Add(new VCardAuthProc()
                    {
                        CtrlNo = mcr.CtrlNo,
                        EquNo = mcr.EquNo,
                        EquID = mcr.EquID.ToString(),
                        MstConnParam = mcr.MstConnParam,
                        ReaderNo = mcr.ReaderNo,
                        DciNo = mcr.DciNo,
                        CtrlModel = mcr.CCtrlModel,
                        OpMode = "-"
                    });
                }                

                //取得以控制器為主的資料，並串接EquIDList、EquNoList、ReaderNo
                var CtrlNoList = EquAdjList.Select(i => i.CtrlNo).Distinct().ToList();
                //設定同台控制器，沒有權限的資料
                foreach (var CtrlNo in CtrlNoList)
                {
                    foreach (var mcr in McrList.Where(i =>
                    i.CtrlNo == CtrlNo &&
                    !EquAdjList.Where(ea => ea.CtrlNo == CtrlNo).Select(ea => ea.ReaderNo).Contains(i.ReaderNo)))
                    {
                        EquAdjList.Add(new VCardAuthProc()
                        {
                            CtrlNo = CtrlNo,
                            EquNo = mcr.EquNo,
                            EquID = mcr.EquID.ToString(),
                            MstConnParam = mcr.MstConnParam,
                            ReaderNo = mcr.ReaderNo,
                            DciNo = mcr.DciNo,
                            OpMode = "-"
                        });
                    }
                }                
                //設定每筆設備資料的人員卡片資料
                EquAdjList.ForEach(i =>
                {
                    if (i.EndTime == null)
                    {
                        i.EndTime = o.CardETime;
                        i.BeginTime = o.CardSTime;
                    }
                    if (o.CardAuthAllow == 0 || o.PsnAuthAllow == 0)
                    {
                        i.OpMode = "-";
                    }
                    i.CardNo = o.CardNo;
                    i.PsnNo = o.PsnNo;
                    i.PsnName = o.PsnName;
                    i.CardVer = o.CardVer;
                    i.CardPW = o.CardPW;
                    i.IDNum = o.IDNum;
                    i.Mark = 0;
                    i.ReadNo = i.ReaderNo;
                    i.CardType = o.CardType;
                    i.VerifiMode = CardAuthData.Where(cardauth => cardauth.EquID == i.EquID).Count() > 0 ? CardAuthData.Where(cardauth => cardauth.EquID == i.EquID).First().VerifiMode : 0;
                });
                foreach (var ctrlMajor in CtrlNoList)
                {
                    if (CardAuthList.Where(i => i.CardNo == ctrlMajor).Count() > 0)
                    {
                        continue;
                    }
                    var CtrlModelObj = EquAdjList.Where(i => i.CtrlNo == ctrlMajor).First();
                    CtrlModelObj.EquIDList = string.Join(",", EquAdjList.Where(i => i.CtrlNo == ctrlMajor).OrderBy(i => i.ReadNo).Select(i => i.EquID));
                    CtrlModelObj.EquNoList = string.Join(",", EquAdjList.Where(i => i.CtrlNo == ctrlMajor).OrderBy(i => i.ReadNo).Select(i => i.EquNo));
                    CtrlModelObj.VerifiModeVar = string.Join(",", EquAdjList.Where(i => i.CtrlNo == ctrlMajor).OrderBy(i => i.ReadNo).Select(i => i.VerifiMode));
                    if (EquAdjList.Where(i => i.CtrlNo == ctrlMajor && i.OpMode == "+").Count() > 0)
                    {
                        CtrlModelObj.CardRule = EquAdjList.Where(i => i.CtrlNo == ctrlMajor && i.OpMode == "+").First().CardRule;
                        CtrlModelObj.CardRule = AddZero(CtrlModelObj.CardRule, 2);
                        if (CtrlModelObj.EquClass == "Elevator")
                        {
                            CtrlModelObj.CardExtData = EquAdjList.Where(i => i.CtrlNo == ctrlMajor && i.OpMode == "+").First().CardExtData;
                        }
                        else
                        {
                            CtrlModelObj.CardExtData = o.PsnName;
                        }
                        if (CtrlModelObj.CtrlModel.Contains("ADM100FP") && FpList.Count > 0)
                        {
                            CtrlModelObj.FPAmount = FpList.First().FPAmount;
                            CtrlModelObj.FPData = FpList.First().FPData;
                            CtrlModelObj.FPScore = FpList.First().FPScore;
                        }
                        if (CtrlModelObj.CtrlModel.Contains("SST9500FP") && FpList.Count > 0)
                        {
                            CtrlModelObj.FPAmount = FpList.First().FPAmount;
                            CtrlModelObj.FPData = FpList.First().FPData;
                            CtrlModelObj.FPScore = FpList.First().FPScore;
                        }
                    }
                    CtrlModelObj.ReaderNo = "";
                    foreach (var reader in EquAdjList.Where(i => i.CtrlNo == ctrlMajor).OrderBy(i => i.ReadNo).ToList())
                    {
                        if (reader.OpMode == "+")
                        {
                            CtrlModelObj.ReaderNo += "1";
                        }
                        else
                        {
                            CtrlModelObj.ReaderNo += "0";
                        }
                    }
                    CardAuthList.Add(CtrlModelObj);
                }
                odo.Execute(@"INSERT INTO B01_CardAuthProc 
                        (DciNo,MstConnParam,CtrlNo,CtrlAddr,CtrlModel,ReaderNo,EquIDList,EquNoList
                        ,CardNo,CardVer,CardPW,CardRule,CardExtData,BeginTime,EndTime,CardType,IDNum,PsnNo,PsnName,Mark,VerifiMode) 
                        VALUES (@DciNo,@MstConnParam,@CtrlNo,@CtrlAddr,@CtrlModel,@ReaderNo,@EquIDList,@EquNoList
                        ,@CardNo,@CardVer,@CardPW,@CardRule,@CardExtData,@BeginTime,@EndTime,@CardType,@IDNum,@PsnNo,@PsnName,@Mark,@VerifiModeVar)  ", CardAuthList);
                if (!odo.isSuccess)
                {
                    return "Error....." + odo.DbExceptionMessage;
                }
            }

            return "OK";
        }





        public static void DoCardAuthProc(string CardNo)
        {
            Console.WriteLine(DateTime.Now);
            OrmDataObject odo = new OrmDataObject("MsSql", ConfigurationManager.ConnectionStrings["SahoConn"].ConnectionString);
            List<VPsncard> PsnCards = odo.GetQueryResult<VPsncard>("SELECT * FROM V_PsnCard WHERE CardNo=@CardNo", new { CardNo = CardNo }).ToList();     
            List<VCardAuthProc> CardAuthData = odo.GetQueryResult<VCardAuthProc>("SELECT * FROM B01_CardAuth WHERE CardNo=@CardNo AND OpMode<>'Del' ", new { CardNo = CardNo }).ToList();
            List<VCardAuthProc> DelAuthData = odo.GetQueryResult<VCardAuthProc>("SELECT * FROM B01_CardAuth WHERE CardNo=@CardNo AND OpMode='Del' ", new { CardNo = CardNo }).ToList();
            List<VCardAuthProc> CardAuthList = new List<VCardAuthProc>();
            foreach (var o in PsnCards)
            {
                List<VMcrinfo> McrList = odo.GetQueryResult<VMcrinfo>("SELECT * FROM V_McrInfo").ToList();
                List<SahoFPData> FpList = odo.GetQueryResult<SahoFPData>("SELECT * FROM B02_FPData WHERE CardNo=@CardNo", new { CardNo = o.CardNo }).ToList();
                List<VCardAuthProc> EquAdjList = odo.GetQueryResult<VCardAuthProc>(@"SELECT 
	                                                                                                                                                        MCR.EquClass,EGD.EquID,EquNo,CardRule,CardExtData,DciNo,CtrlNo,MstConnParam,ReaderNo,CCtrlModel AS CtrlModel,CtrlAddr,'+' AS OpMode,NULL AS BeginTime,NULL AS EndTime 
                                                                                                                                                        FROM 
	                                                                                                                                                        B01_CardEquGroup CEG 
	                                                                                                                                                        INNER JOIN B01_EquGroupData EGD ON CEG.EquGrpID=EGD.EquGrpID
	                                                                                                                                                        INNER JOIN V_MCRInfo MCR ON MCR.EquID=EGD.EquID
                                                                                                                                                        WHERE EGD.EquID NOT IN (SELECT EquID FROM B01_CardEquAdj WHERE CardID=@CardID)
                                                                                                                                                        AND CardID=@CardID
                                                                                                                                                        UNION
                                                                                                                                                        SELECT 
	                                                                                                                                                        MCR.EquClass,MCR.EquID,EquNo,CardRule,CardExtData,DciNo,CtrlNo,MstConnParam,ReaderNo,CCtrlModel AS CtrlModel,CtrlAddr,OpMode,BeginTime,EndTime  
                                                                                                                                                        FROM 
	                                                                                                                                                        B01_CardEquAdj CEA
	                                                                                                                                                        INNER JOIN V_MCRInfo MCR ON CEA.EquID=MCR.EquID
                                                                                                                                                        WHERE CardID=@CardID ", new { CardID = o.CardID }).ToList();
                //整理電梯權限
                foreach (string equid in EquAdjList.Where(i => i.OpMode == "+" && i.EquClass == "Elevator").Select(i => i.EquID).Distinct())
                {
                    Int64 CardExtAmt = 0;
                    foreach (var floor in EquAdjList.Where(i => i.EquID == equid).Select(i => i.CardExtData))
                    {
                        CardExtAmt = CardExtAmt | Convert.ToInt64(Convert.ToString(Convert.ToInt64(floor, 16), 2), 2);
                    }
                    EquAdjList.ForEach(i =>
                    {
                        if (i.EquID == equid)
                        {
                            i.CardExtData = Convert.ToString(CardExtAmt, 16).PadLeft(12, '0').ToUpper();
                        }
                    });
                }
                EquAdjList = EquAdjList.GroupBy(i => i.EquNo).Select(i => i.First()).ToList();
                if (o.PsnETime < o.CardETime)
                {
                    o.CardETime = o.PsnETime;
                }
                if (o.CardSTime < o.PsnSTime)
                {
                    o.CardSTime = o.PsnSTime;
                }

                //設定沒權限的McrInfo List 資料至權限清單
                foreach (var mcr in McrList.Where(i => !EquAdjList.Select(j => j.EquNo).Contains(i.EquNo) && DelAuthData.Select(j => decimal.Parse(j.EquID)).Contains(i.EquID)))
                {
                    EquAdjList.Add(new VCardAuthProc()
                    {
                        CtrlNo = mcr.CtrlNo,
                        EquNo = mcr.EquNo,
                        EquID = mcr.EquID.ToString(),
                        MstConnParam = mcr.MstConnParam,
                        ReaderNo = mcr.ReaderNo,
                        DciNo = mcr.DciNo,
                        CtrlModel = mcr.CCtrlModel,
                        OpMode = "-"
                    });
                }

                //取得以控制器為主的資料，並串接EquIDList、EquNoList、ReaderNo
                var CtrlNoList = EquAdjList.Select(i => i.CtrlNo).Distinct().ToList();
                //設定同台控制器，沒有權限的資料
                foreach (var CtrlNo in CtrlNoList)
                {
                    foreach (var mcr in McrList.Where(i => i.CtrlNo == CtrlNo && !EquAdjList.Where(ea => ea.CtrlNo == CtrlNo).Select(ea => ea.ReaderNo).Contains(i.ReaderNo)))
                    {
                        EquAdjList.Add(new VCardAuthProc()
                        {
                            CtrlNo = CtrlNo,
                            EquNo = mcr.EquNo,
                            EquID = mcr.EquID.ToString(),
                            MstConnParam = mcr.MstConnParam,
                            ReaderNo = mcr.ReaderNo,
                            DciNo = mcr.DciNo,
                            OpMode = "-"
                        });
                    }
                }
                //設定每筆設備資料的人員卡片資料
                EquAdjList.ForEach(i =>
                {
                    if (i.EndTime == null)
                    {
                        i.EndTime = o.CardETime;
                        i.BeginTime = o.CardSTime;
                    }
                    if (o.CardAuthAllow == 0 || o.PsnAuthAllow == 0)
                    {
                        i.OpMode = "-";
                    }
                    i.CardNo = o.CardNo;
                    i.PsnNo = o.PsnNo;
                    i.PsnName = o.PsnName;
                    i.CardVer = o.CardVer;
                    i.CardPW = o.CardPW;
                    i.IDNum = o.IDNum;
                    i.Mark = 0;
                    i.ReadNo = i.ReaderNo;
                    i.CardType = o.CardType;
                    i.VerifiMode = CardAuthData.Where(cardauth => cardauth.EquID == i.EquID).Count() > 0 ? CardAuthData.Where(cardauth => cardauth.EquID == i.EquID).First().VerifiMode : 0;
                });
                foreach (var ctrlMajor in CtrlNoList)
                {
                    if (CardAuthList.Where(i => i.CardNo == ctrlMajor).Count() > 0)
                    {
                        continue;
                    }
                    var CtrlModelObj = EquAdjList.Where(i => i.CtrlNo == ctrlMajor).First();
                    CtrlModelObj.EquIDList = string.Join(",", EquAdjList.Where(i => i.CtrlNo == ctrlMajor).OrderBy(i => i.ReadNo).Select(i => i.EquID));
                    CtrlModelObj.EquNoList = string.Join(",", EquAdjList.Where(i => i.CtrlNo == ctrlMajor).OrderBy(i => i.ReadNo).Select(i => i.EquNo));
                    CtrlModelObj.VerifiModeVar = string.Join(",", EquAdjList.Where(i => i.CtrlNo == ctrlMajor).OrderBy(i => i.ReadNo).Select(i => i.VerifiMode));
                    if (EquAdjList.Where(i => i.CtrlNo == ctrlMajor && i.OpMode == "+").Count() > 0)
                    {
                        CtrlModelObj.CardRule = EquAdjList.Where(i => i.CtrlNo == ctrlMajor && i.OpMode == "+").First().CardRule;
                        CtrlModelObj.CardRule = AddZero(CtrlModelObj.CardRule, 2);
                        if (CtrlModelObj.EquClass == "Elevator")
                        {
                            CtrlModelObj.CardExtData = EquAdjList.Where(i => i.CtrlNo == ctrlMajor && i.OpMode == "+").First().CardExtData;
                        }
                        else
                        {
                            CtrlModelObj.CardExtData = o.PsnName;
                        }
                        if (CtrlModelObj.CtrlModel.Contains("ADM100FP") && FpList.Count > 0)
                        {
                            CtrlModelObj.FPAmount = FpList.First().FPAmount;
                            CtrlModelObj.FPData = FpList.First().FPData;
                            CtrlModelObj.FPScore = FpList.First().FPScore;
                        }
                        if (CtrlModelObj.CtrlModel.Contains("SST9500FP") && FpList.Count > 0)
                        {
                            CtrlModelObj.FPAmount = FpList.First().FPAmount;
                            CtrlModelObj.FPData = FpList.First().FPData;
                            CtrlModelObj.FPScore = FpList.First().FPScore;
                        }
                    }
                    CtrlModelObj.ReaderNo = "";
                    foreach (var reader in EquAdjList.Where(i => i.CtrlNo == ctrlMajor).OrderBy(i => i.ReadNo).ToList())
                    {
                        if (reader.OpMode == "+")
                        {
                            CtrlModelObj.ReaderNo += "1";
                        }
                        else
                        {
                            CtrlModelObj.ReaderNo += "0";
                        }
                    }
                    CardAuthList.Add(CtrlModelObj);
                }
                odo.Execute(@"INSERT INTO B01_CardAuthProc 
                        (DciNo,MstConnParam,CtrlNo,CtrlAddr,CtrlModel,ReaderNo,EquIDList,EquNoList
                        ,CardNo,CardVer,CardPW,CardRule,CardExtData,BeginTime,EndTime,CardType,IDNum,PsnNo,PsnName,Mark,VerifiMode) 
                        VALUES (@DciNo,@MstConnParam,@CtrlNo,@CtrlAddr,@CtrlModel,@ReaderNo,@EquIDList,@EquNoList
                        ,@CardNo,@CardVer,@CardPW,@CardRule,@CardExtData,@BeginTime,@EndTime,@CardType,@IDNum,@PsnNo,@PsnName,@Mark,@VerifiModeVar)  ", CardAuthList);
            }
            Console.WriteLine("新增完成");
            Console.WriteLine(DateTime.Now);
        }




        static string AddZero(string paramDig, int paramDiff)
        {
            while (paramDig.Length < paramDiff)
            {
                paramDig = "0" + paramDig;
            }
            return paramDig;
        }





    }//end process class
}//end namespace
