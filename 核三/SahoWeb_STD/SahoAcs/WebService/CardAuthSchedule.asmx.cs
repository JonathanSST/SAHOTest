using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;
using SMS_Communication;

using DapperDataObjectLib;
using SahoAcs.DBModel;

namespace SahoAcs.WebService
{
    /// <summary>
    ///CardAuthSchedule 的摘要描述
    /// </summary>
    [WebService(Namespace = "SahoWeb")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class CardAuthSchedule : System.Web.Services.WebService
    {
        public Authentication authentication;

        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod(EnableSession = true)]
        [SoapHeader("authentication")]
        public string GetCardAuthUpdateTask(string CardNo)
        {
            CardAuthCore.CardAuthUpdateTask.ConnString = Pub.GetDapperConnString();
            //CardAuthCore.CardAuthUpdateTask.SetCardAuth(CardNo);
            return CardAuthCore.CardAuthUpdateTask.GetCardAuth(CardNo);
        }


        [WebMethod(EnableSession = true)]
        [SoapHeader("authentication")]
        public string GetCardAuthCopyTask(string CardNoS,string CardNoE)
        {
            return "OK";
        }

        [WebMethod(EnableSession = true)]
        [SoapHeader("authentication")]
        public string GetCardAuthProcTask(string CardNo, List<int> EquList)
        {
            CardAuthCore.CardAuthProcTask.ConnString = Pub.GetDapperConnString();
            return CardAuthCore.CardAuthProcTask.GetCardAuthProc(CardNo, EquList);
        }


        [WebMethod(EnableSession = true)]
        [SoapHeader("authentication")]
        public string GetCardAuthProc()
        {
            var results = this.odo.GetQueryResult("SELECT TOP 2000 * FROM B01_CardAuthSchedule WHERE OpStatus=0 ORDER BY RecordID ");
            string message = "";
            try
            {
                foreach (var o in results)
                {
                    this.odo.Execute("EXEC CardAuth_Update @sCardNo = @CardNo, @sUserID = @UserID, @sFromProc = 'Person', @sFromIP = '', @sOpDesc = '卡片資料內容更新' ; "
                             , new { CardNo = o.CardNo, UserID = this.authentication.sysAccount });
                    bool success = this.odo.isSuccess;
                    this.odo.Execute("UPDATE B01_CardAuthSchedule SET OpStatus=@Status,UpdateUserID=@UserID,UpdateTime=GETDATE() WHERE RecordID=@RecordID",
                        new { UserID = this.authentication.sysAccount, Status = success ? "1" : "2", RecordID = o.RecordID });
                    message += this.odo.DbExceptionMessage + ";";
                }
                if (this.odo.GetQueryResult("SELECT * FROM B01_CardAuthSchedule WHERE OpStatus=0").Count() > 0)
                {
                    return "Success";
                }
                else
                {
                    return "Finish";
                }
            }
            catch(Exception ex)
            {
                return "Error..........."+ex.Message;
            }                        
        }


    }//end class
}//end namespace
