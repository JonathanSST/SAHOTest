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
    public class CardAuthUpdateTask
    {

        public static string ConnString { get; set; }


        /// <summary>
        /// 權限重整
        /// </summary>
        /// <param name="CardNo"></param>
        public static string GetCardAuth(string CardNo)
        {
            OrmDataObject odo = new OrmDataObject("MsSql", ConnString);
            List<VPsncard> PsnCards = odo.GetQueryResult<VPsncard>("SELECT * FROM V_PsnCard WHERE CardNo=@CardNo", new { CardNo = CardNo }).ToList();
            List<VCardAuthProc> CardAuthData = odo.GetQueryResult<VCardAuthProc>("SELECT * FROM B01_CardAuth WHERE CardNo=@CardNo ", new { CardNo = CardNo }).ToList();
            foreach (var o in PsnCards)
            {
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
	                                                                                                                                                        MCR.EquClass,MCR.EquID,EquNo,CardRule,CardExtData,DciNo,CtrlNo,MstConnParam,ReaderNo,CCtrlModel AS CtrlModel,CtrlAddr,
                                                                                                                                                            CASE OpMode WHEN '*' THEN '+' ELSE OpMode END AS OpMode, BeginTime,EndTime  
                                                                                                                                                        FROM 
	                                                                                                                                                        B01_CardEquAdj CEA
	                                                                                                                                                        INNER JOIN V_MCRInfo MCR ON CEA.EquID=MCR.EquID
                                                                                                                                                        WHERE CardID=@CardID ", new { CardID = o.CardID }).ToList();               
                //整理電梯權限
                foreach(string equid in EquAdjList.Where(i=> i.OpMode=="+" && i.EquClass=="Elevator").Select(i => i.EquID).Distinct())
                {
                    Int64 CardExtAmt = 0;
                    foreach(var floor in EquAdjList.Where(i => i.EquID==equid).Select(i => i.CardExtData))
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
                EquAdjList = EquAdjList.GroupBy(i => i.EquNo).Select(I => I.First()).ToList();
                //設定每筆設備資料的人員卡片資料
                EquAdjList.ForEach(i =>
                {
                    if (i.EndTime == null)
                    {
                        i.EndTime = o.CardETime;
                        i.BeginTime = o.CardSTime;
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
                    if (o.CardAuthAllow == 0 || o.PsnAuthAllow == 0)
                    {
                        i.OpMode = "-";
                    }
                    if (i.EquClass == "TRT")
                    {
                        i.CardExtData = o.PsnName;
                    }
                });

                //執行權限新增
                odo.Execute(@"INSERT INTO B01_CardAuth 
                                            (EquID,CardNo,CardVer,CArdPW,CardRule,CardExtData,BeginTime,EndTime,UpdateUserID,UpdateTime) 
                                            VALUES (@EquID,@CardNo,@CardVer,@CardPW,@CardRule,@CardExtData,@BeginTime,@EndTime,'Saho',GETDATE()) ",
                                            EquAdjList.Where(i => i.OpMode=="+" && !CardAuthData.Where(ca => ca.OpMode != "Del").Select(ca => ca.EquID).Contains(i.EquID)));
                if (!odo.isSuccess)
                {
                    return "Error...." + odo.DbExceptionMessage;
                }
                //執行權限修改
                odo.Execute(@"UPDATE B01_CardAuth 
                                            SET CardRule=@CardRule,CardExtData=@CardExtData,CardPW=@CardPW,CardVer=@CardVer,UpdateUserID='Saho',UpdateTime=GETDATE(),OpStatus='' 
                                             WHERE CardNo=@CardNo AND EquID=@EquID AND OpMode<>'Del' "
                , EquAdjList.Where(i => i.OpMode=="+" && CardAuthData.Where(ca => ca.OpMode != "Del").Select(ca => ca.EquID).Contains(i.EquID)));
                if (!odo.isSuccess)
                {
                    return "Error...." + odo.DbExceptionMessage;
                }                
                //執行權限刪除
                odo.Execute(@"UPDATE B01_CardAuth 
                                            SET OpMode='Del',UpdateUserID='Saho',UpdateTime=GETDATE(),OpStatus='' 
                                             WHERE CardNo=@CardNo AND EquID=@EquID AND OpMode<>'Del' "
                , CardAuthData.Where(i => !EquAdjList.Where(ca => ca.OpMode == "+").Select(ca => ca.EquID).Contains(i.EquID)));
                if (!odo.isSuccess)
                {
                    return "Error...." + odo.DbExceptionMessage;
                }
            }
            return "OK";
        }//end method



    }//end process class
}//end namespace
