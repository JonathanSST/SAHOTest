using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace TestRefDll
{
    public class PublicUtility
    {
        public static string Custom = ConfigurationManager.AppSettings["Custom"].ToString();
        public static string SWTag = ConfigurationManager.AppSettings["SWTag"].ToString();
        public static string SWTagList = GetConfigPara("SWTagList");
        public static string IsSrvMode = GetConfigPara("IsSrvMode");
        public static string DbUserID = GetConfigPara("DbUserID");
        public static string DbUserPW = GetConfigPara("DbUserPW");
        public static string DbDataBase = GetConfigPara("DbDataBase");
        public static string DbDataSource = GetConfigPara("DbDataSource");
        public static string LogFileRoot = GetConfigPara("LogFileRoot");
        public static string GetConfigPara(string para)
        {
            return ConfigurationManager.AppSettings[para].ToString();
        }

    }
}
