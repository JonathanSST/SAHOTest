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
    public class CardAuthCopyTask
    {

        public static void SetDoAuthCopy(string CardNoS, string CardNoT)
        {
            Console.WriteLine(DateTime.Now);
            OrmDataObject odo = new OrmDataObject("MsSql", ConfigurationManager.ConnectionStrings["SahoConn"].ConnectionString);
            //取得來源卡號
            List<VCardAuthProc> CardAuthDataS = odo.GetQueryResult<VCardAuthProc>("SELECT * FROM B01_CardAuth WHERE CardNo=@CardNo AND OpMode<>'Del' ", new { CardNo = CardNoS }).ToList();
            List<VCardAuthProc> CardAuthDataT = odo.GetQueryResult<VCardAuthProc>("SELECT * FROM B01_CardAuth WHERE CardNo=@CardNo AND OpMode<>'Del'  ", new { CardNo = CardNoT }).ToList();
            List<VPsncard> PsnCards = odo.GetQueryResult<VPsncard>("SELECT * FROM V_PsnCard WHERE CardNo=@CardNo", new { CardNo = CardNoT }).ToList();
            var AddAuthList = CardAuthDataS.Where(i => !CardAuthDataT.Select(t => t.EquID).Contains(i.EquID)).ToList();
            var DelAuthList = CardAuthDataT.Where(i => !CardAuthDataS.Select(t => t.EquID).Contains(i.EquID)).ToList();
            if (PsnCards.Count > 0)
            {
                var PsnCardOb = PsnCards.First();
                AddAuthList.ForEach(i =>
                {
                    i.CardPW = PsnCardOb.CardPW;
                    i.CardVer = PsnCardOb.CardVer;
                    i.CardNo = CardNoT;
                }
                );
            }
            odo.Execute("UPDATE B01_CardAuth SET OpMode='Del',UpdateTime=GETDATE() WHERE CardNo=@CardNo AND EquID=@EquID ", DelAuthList);
            Console.WriteLine(odo.DbExceptionMessage);
            odo.Execute(@"INSERT INTO B01_CardAuth 
                                            (EquID,CardNo,CardVer,CArdPW,CardRule,CardExtData,BeginTime,EndTime,UpdateuserID,UpdateTime) 
                                            VALUES (@EquID,@CardNo,@CardVer,@CardPW,@CardRule,@CardExtData,@BeginTime,@EndTime,'Saho',GETDATE()) ", AddAuthList);
            Console.WriteLine(odo.DbExceptionMessage);
            Console.WriteLine("複製完成..");
            Console.WriteLine(DateTime.Now);
        }


    }//end task class
}//end namespace
