using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DapperDataObjectLib;
using SahoAcs.DBModel;


namespace SahoAcs.Web { 
    /// <summary>
    /// FillLog 的摘要描述
    /// </summary>
    public class FillLog : IHttpHandler
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());


        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (context.Request["PsnNo"] != null && context.Request["EquNo"] != null)
            {

                string PsnNo = context.Request["PsnNo"];
                string EquNo = context.Request["EquNo"];
                string status = "0";
                                
                var sql = @"  SELECT B01_Person.PsnID, B01_Person.PsnNo, B01_Person.PsnName, B01_OrgStruc.OrgIDList AS OrgStruc,CardType
                ,B01_Card.CardNo, B01_Card.CardVer FROM B01_Person 
                INNER JOIN B01_Card ON B01_Person.PsnID = B01_Card.PsnID 
                INNER JOIN B01_OrgStruc ON B01_Person.OrgStrucID = B01_OrgStruc.OrgStrucID 
                WHERE PsnNo=@PsnNo ";
                List<CardLogFillData> list = this.odo.GetQueryResult<DBModel.CardLogFillData>(sql, new { PsnNo = PsnNo }).ToList();
                if (list.Count() > 0)
                {
                    CardLogFillData data = list.First();
                    sql = @" SELECT EquClass, EquNo, EquName, Dir, MstConnParam, CtrlAddr, ReaderNo, IsAndTrt FROM V_MCRInfo WHERE EquNo = @EquNo ";
                    var result = this.odo.GetQueryResult(sql, new { EquNo = EquNo });                   
                    foreach (var o in result)
                    {
                        data.EquClass = Convert.ToString(o.EquClass);
                        data.EquNo = Convert.ToString(o.EquNo);
                        data.EquName = Convert.ToString(o.EquName);
                        data.EquDir = Convert.ToString(o.Dir);
                        data.MstConnParam = Convert.ToString(o.MstConnParam);
                        data.CtrlAddr = Convert.ToString(o.CtrlAddr);
                        data.ReaderNo = Convert.ToString(o.ReaderNo);
                        data.IsAndTrt = Convert.ToString(o.IsAndTrt);
                    }
                    data.CardInfo = "";
                    data.VerifyMode = "0";
                    data.LogStatus = status;
                    data.CardType = Convert.ToString(list.First().CardType);
                    data.CardTime = string.Format("{0:yyyy/MM/dd HH:mm:ss}",DateTime.Now);
                    sql = @" INSERT INTO B01_CardLog (LogTime, CardTime, CardNo, CardVer, PsnNo, PsnName, OrgStruc
                    ,EquClass, EquNo, EquName, EquDir, MstConnParam, CtrlAddr, ReaderNo, IsAndTrt, CardInfo, VerifyMode
                    ,LogStatus, CardType) VALUES (getdate(), @CardTime, @CardNo, @CardVer, @PsnNo, @PsnName, @OrgStruc
                     ,@EquClass, @EquNo, @EquName, @EquDir, @MstConnParam, @CtrlAddr, @ReaderNo, @IsAndTrt, @CardInfo, @VerifyMode
                     ,@LogStatus, @CardType) ";
                    this.odo.Execute(sql, data);
                    string messae = this.odo.DbExceptionMessage;
                    context.Response.Write(messae);
                }
            }

            context.Response.Write("End Fill");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }//end class
}//end namespace