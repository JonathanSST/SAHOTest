using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;


namespace DapperDataObjectLib
{
    public static class WriteLogExtension
    {
        public static void SetLogs(string _exception,string cmdstr = "")
        {            
            string sFileName = string.Format("{0:yyyyMMdd}.txt", DateTime.Now);
            string sPath = System.AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "System_Log\\" + sFileName;
            StreamWriter oSw = null;
            try
            {
                oSw = File.AppendText(sPath);
                oSw.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
                oSw.WriteLine(cmdstr);
                oSw.WriteLine(_exception);
            }
            catch (Exception e)
            {
            }
            if (oSw != null)
                oSw.Dispose();
        }//end method

        public static void SetLogs(this OrmDataObject odo, string cmdstr = "")
        {
            string sFileName = string.Format("{0:yyyyMMdd}.txt", DateTime.Now);
            string sPath = System.AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "System_Log\\" + sFileName;
            StreamWriter oSw = null;
            try
            {
                oSw = File.AppendText(sPath);
                oSw.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
                oSw.WriteLine(cmdstr);
                oSw.WriteLine(odo.DbExceptionMessage);
            }
            catch (Exception e)
            {
            }
            if (oSw != null)
                oSw.Dispose();
        }//end method

    }
}
