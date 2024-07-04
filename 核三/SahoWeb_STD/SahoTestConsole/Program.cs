using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Sa.DB;
using TempCore;
using TempCore.manage;

namespace SahoTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var conn1 = @"Provider=SQLOLEDB;Persist Security Info=False;Initial Catalog=kinmen_SMS2015;Data Source=127.0.0.1\SQLEXPRESS;User ID=sa;Password=5841450";
            var conn2 = @"data source=127.0.0.1\SQLEXPRESS;initial catalog=kinmen_SMS2015;
                        user id=sa;password=5841450;MultipleActiveResultSets=True;App=EntityFramework";
            BasicCore core = new PersonSyncCore("MsSql",conn2);
            var sql = @"SELECT * FROM ( SELECT ROW_NUMBER() OVER(ORDER BY CONVERT(VARCHAR(10),B01_CardLog.CardTime,121) DESC, B01_CardLog.CardNo) AS NewIDNum,
                B00_CardLogState.StateDesc AS 'LogStatusName', CONVERT(VARCHAR(10),B01_CardLog.CardTime,121) AS 'Date',
                MIN(SUBSTRING(CONVERT(VARCHAR(50),B01_CardLog.CardTime,121),12,8)) AS 'First',
                MAX(SUBSTRING(CONVERT(VARCHAR(50),B01_CardLog.CardTime,121),12,8)) AS 'Last',
                B01_CardLog.CardNo, (SELECT OrgName FROM OrgStrucAllData('Department') WHERE OrgIDList = B01_CardLog.OrgStruc) AS 'Dept',
                B01_CardLog.PsnNo, B01_CardLog.PsnName
                FROM B01_CardLog
                LEFT JOIN B00_CardLogState ON B00_CardLogState.Code = B01_CardLog.LogStatus
                LEFT JOIN B00_ItemList ON B00_ItemList.ItemClass = 'CardType' AND B00_ItemList.ItemNo = B01_CardLog.CardType
                INNER JOIN (SELECT PsnNo FROM B01_Person AS Person
                INNER JOIN B01_OrgStruc AS OrgStruc ON OrgStruc.OrgStrucID = Person.OrgStrucID
                INNER JOIN B01_MgnOrgStrucs AS MgnOrgStrucs ON MgnOrgStrucs.OrgStrucID = OrgStruc.OrgStrucID
                INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnOrgStrucs.MgaID
                INNER JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID  WHERE SysUser.UserID = 'Saho' ) AS MgnPerson ON MgnPerson.PsnNo = B01_CardLog.PsnNo  
				WHERE  (B01_CardLog.CardType = 'E' OR B01_CardLog.CardType = 'T') AND (B01_CardLog.IsAndTrt = '1' OR B01_CardLog.EquClass = 'TRT')  
				AND  (CONVERT(VARCHAR(10),B01_CardLog.CardTime,111) >= @DateS )  AND  (CONVERT(VARCHAR(10),B01_CardLog.CardTime,111) <= @DateE )  
				GROUP BY CONVERT(VARCHAR(10),B01_CardLog.CardTime,121), B00_CardLogState.StateDesc, B01_CardLog.CardNo, B01_CardLog.CardVer, B01_CardLog.PsnNo, B01_CardLog.PsnName, 
				B01_CardLog.OrgStruc ) AS Q WHERE Q.NewIDNum BETWEEN 1 AND 100";
            DataTable dt = new DataTable();
            List<object> liSqlPara = new List<object>();
            Dictionary<string, object> para = new Dictionary<string, object>();            
            para.Add("@DateS", "2016/02/09");
            para.Add("@DateE", "2016/03/09");
            para.Add("@SysID", "Saho");
            //liSqlPara.Add("Saho");
            liSqlPara.Add("2016/02/11");
            liSqlPara.Add("2016/03/09");
            dt = core.GetQueryData(sql, para);                        
            Console.WriteLine(dt.Rows.Count);
            Console.WriteLine(core.Excetion);
            Console.Read();
        }
    }
}
}
}
