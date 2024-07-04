using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using DapperDataObjectLib;
using SahoAcs.DBModel;


namespace SahoAcs.DBClass
{
    public static class OrgStrucService
    {
        public static string SetOrgEquData(this OrmDataObject odo,string CardNo,int PsnID,string UserID,GlobalMsg gms)
        {
            string message = "";
            //建立Log基礎物件
            var LogEntity = SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.卡片權限調整, UserID, HttpContext.Current.Session["UserName"].ToString(), "0103");

            var CardInfos = odo.GetQueryResult<CardEntity>("SELECT * FROM B01_Card WHERE CardNo=@CardNo",new {CardNo=CardNo }).ToList();
            var cardlist = odo.GetQueryResult(@"SELECT B01_Card.CardID, B01_Card.CardNo, B00_ItemList.ItemName FROM B01_Card
                        INNER JOIN B00_ItemList ON B01_Card.CardType = B00_ItemList.ItemNo
                        WHERE ItemClass = 'CardType' AND PsnID = @PsnID AND CardNo=@CardNo", new { PsnID = PsnID,CardNo=CardNo });
            if (cardlist.Count() > 0)
            {            
                int CardID = CardInfos.FirstOrDefault().CardID;
                int counts = 0;
               
                //依照CardID 進行 CardEquGroup 的附加
                var CardEquGroups = odo.GetQueryResult<EquGroupModel>(@"SELECT A.EquGrpID,EquGrpNo,@CardID AS CardID FROM B01_OrgEquGroup A 
                INNER JOIN B01_EquGroup G ON A.EquGrpID=G.EquGrpID
                INNER JOIN B01_Person B ON A.OrgStrucID = B.OrgStrucID AND PsnID = @PsnID 
                AND A.EquGrpID NOT IN (SELECT EquGrpID FROM B01_CardEquGroup WHERE CardID=@CardID)", new { PsnID = PsnID, CardID = CardID }).ToList();

               

                counts += odo.Execute("INSERT INTO B01_CardEquGroup (CardID,EquGrpID,CreateUserID,CreateTime) VALUES (@CardID,@EquGrpID,@UserID,GETDATE())",
                    from i in CardEquGroups select new { UserID = UserID, i.CardID, i.EquGrpID });
                foreach(var o in CardEquGroups)
                {                    
                    odo.SetSysLogCreateByNo(CardID, CardNo, o.EquGrpNo, " Add EquGroup ", "0103");
                }


                //取得應加入的OrgEquAdj
                var OrgEquAdjByPsnID = odo.GetQueryResult<EquAdj>(@"SELECT A.*,EquNo,EquClass,@CardID AS CardID FROM B01_OrgEquAdj A 
                INNER JOIN B01_Person B ON A.OrgStrucID=B.OrgStrucID AND PsnID=@PsnID
                INNER JOIN B01_EquData E ON A.EquID=E.EquID
                WHERE A.EquID NOT IN (SELECT EquID FROM B01_CardEquAdj WHERE CardID=@CardID) ", new { PsnID = PsnID, CardID = CardID }).ToList();

                counts += odo.Execute(@"INSERT INTO B01_CardEquAdj (CardID,EquID,OpMode,CardRule,CardExtData,CreateUserID,CreateTime) 
                                            VALUES (@CardID,@EquID,@OpMode,@CardRule,@CardExtData,@UserID,GETDATE())",
                    from i in OrgEquAdjByPsnID select new{ UserID = UserID, i.CardID, i.EquID, i.OpMode, i.CardRule, i.CardExtData });
                foreach (var o in OrgEquAdjByPsnID)
                {
                    odo.SetSysLogCreateByNo(CardID, CardNo, o.EquNo, "Add CardEquAdj", "0103");
                }
                if (OrgEquAdjByPsnID.Count == 0 && CardEquGroups.Count == 0)
                {
                    message = gms.MsgUpdateSuccess1 + "|";
                }
                else
                {
                    message = gms.MsgUpdateSuccess + "|";
                }
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("UPDATE B01_CardExt SET CardBorrow='0' WHERE CardID=@CardID; ");
                builder.AppendLine("UPDATE dbo.B01_CardAuth SET OpMode='Reset',BeginTime = @BeginTime,EndTime = @EndTime WHERE CardNo = @CardNo AND OpMode<>'Del';");
                counts += odo.Execute(builder.ToString(), new { CardNo = CardNo, CardID = CardID, BeginTime = CardInfos.FirstOrDefault().CardSTime, EndTime = CardInfos.FirstOrDefault().CardETime });
                odo.SetSysLogCreateByEquAuth(LogEntity, CardID, CardNo);
                //重新查詢卡片清單
                var cardlist_r= odo.GetQueryResult(@"SELECT B01_Card.CardID, B01_Card.CardNo, B00_ItemList.ItemName FROM B01_Card
                        INNER JOIN B00_ItemList ON B01_Card.CardType = B00_ItemList.ItemNo
                        WHERE ItemClass = 'CardType' AND PsnID = @PsnID AND CardType NOT IN ('R','T','TEMP') ", new { PsnID = PsnID});
                foreach (var card in cardlist_r)
                {
                    message += Convert.ToString(card.CardNo).Replace("\0","")+ "|" + card.ItemName + "|" + card.CardID + "|";
                }                
                //if (counts > 0)
                //{
                    odo.Execute("EXEC CardAuth_Update @sCardNo = @CardNo, @sUserID = @UserID, @sFromProc = 'Person', @sFromIP = '', @sOpDesc = '卡片資料內容更新' ; "
                        ,new {CardNo=CardNo, UserID=UserID });
                //}

            }//確認是否權限設定
            if (message.Length > 0)
            {
                message = message.Substring(0, message.Length - 1);
            }
            return message;
        }//end method

    }//end class

}//end namespace