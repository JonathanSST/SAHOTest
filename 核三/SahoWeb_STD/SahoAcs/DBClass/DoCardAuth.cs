using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DapperDataObjectLib;

namespace SahoAcs.DBClass
{
    public static class DoCardAuthExtension
    {
        public static void DoCardAuthExec(this OrmDataObject odo,string CardNo,string UserID)
        {
            //odo.Execute(@"INSERT INTO B01_CardAuthSchedule (CardNo,OpStatus,RequestUser,CreateUserID,CreateTime) 
            //VALUES (@CardNo,@OpStatus,@RequestUser,@RequestUser,@CreateTime)"
            //, new { CreateTime = DateTime.Now, RequestUser = UserID, CardNo = CardNo, OpStatus = 0 });
            odo.Execute(@"EXEC CardAuth_Update @sCardNo = @CardNo,
                    @sUserID = 'Saho',@sFromProc = 'Saho',@sFromIP = '127.0.0.1',
                    @sOpDesc = '一般權限重整' ;", new { CardNo = CardNo });
            WriteLogExtension.SetLogs(odo.DbExceptionMessage, "");
        }
    }
}