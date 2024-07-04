using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace TestRefDll
{
    public class CoreInfo
    {
        public string Custom = ConfigurationManager.AppSettings["Custom"].ToString();
        public string SWTag = ConfigurationManager.AppSettings["SWTag"].ToString();
        public string SWTagList = GetConfigPara("SWTagList");
        public string IsSrvMode = GetConfigPara("IsSrvMode");
        public string DbUserID = GetConfigPara("DbUserID");
        public string DbUserPW = GetConfigPara("DbUserPW");
        public string DbDataBase = GetConfigPara("DbDataBase");
        public string DbDataSource = GetConfigPara("DbDataSource");

        public CoreInfo()
        {

        }

        public static string GetConfigPara(string para)
        {
            return ConfigurationManager.AppSettings[para].ToString();
        }

        public bool CalcCPCWorkDetail(string sSDate, string sEDate)
        {
            return true;
        }


    }//end class
}//end namesapce
