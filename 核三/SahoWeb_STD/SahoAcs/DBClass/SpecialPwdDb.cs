using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DapperDataObjectLib;



namespace SahoAcs.DBClass
{
    public static class SpecialPwdDb
    {
        public static void SetUpdatePwd(string strEquNo, string strPwd)
        {
            var odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
            odo.Execute(@"UPDATE B01_EquParaData SET ParaValue=@Pwd,IsReSend=1,UpdateUserID='Saho',UpdateTime=GETDATE() WHERE 
            EquID IN (SELECT EquID FROM B01_EquData WHERE EquNo=@EquNo) 
            AND EquParaID IN (SELECT EquParaID FROM B01_EquParaDef WHERE ParaName='SpecialPassword')", new { EquNo = strEquNo, Pwd = strPwd });
        }


    }
}