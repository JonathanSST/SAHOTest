using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;


namespace SahoAcs.DBClass
{
    public static class WebAppService
    {
        /// <summary>
        /// 將UTC轉本地時間
        /// </summary>
        /// <param name="oPage"></param>
        /// <returns></returns>
        public static DateTime GetZoneTime(this System.Web.UI.Page oPage)
        {
            if (!IsUtc)
            {
                return DateTime.Now;
            }
            string TimeOffset = "TimeOffset";
            int TimeDiff = -480;
            if (oPage.Session[TimeOffset] != null && oPage.Session[TimeOffset].ToString()!=string.Empty)
            {
                TimeDiff = Convert.ToInt32(oPage.Session[TimeOffset]);
            }
            return DateTime.UtcNow.AddMinutes(-TimeDiff);
        }
        


        /// <summary>將參數來源時間減去DbUTC時區設定值變UTC，再轉本地時間</summary>
        /// <param name="oTime">UTC格式的時間</param>
        /// <param name="oPage"></param>
        /// <returns></returns>
        public static DateTime GetZoneTime(this DateTime oTime, System.Web.UI.Page oPage)
        {
            if (!IsUtc)
            {
                return oTime;
            }
            string TimeOffset = "TimeOffset";
            int TimeDiff = -480;
            if (oPage.Session[TimeOffset] != null && oPage.Session[TimeOffset].ToString() != string.Empty)
            {
                TimeDiff = Convert.ToInt32(oPage.Session[TimeOffset]);
            }
            return oTime.AddHours(-DbUTC).AddMinutes(-TimeDiff);
        }



        /// <summary>將參數來源時間減去時區設定值變UTC，再轉本地時間(來源必須要為UTC時間)</summary>
        /// <param name="oTime">UTC格式的時間</param>
        /// <param name="oPage"></param>
        /// <returns></returns>
        public static DateTime GetUtcToZone(this DateTime oTime, System.Web.UI.Page oPage)
        {            
            string TimeOffset = "TimeOffset";
            int TimeDiff = -480;
            if (oPage.Session[TimeOffset] != null && oPage.Session[TimeOffset].ToString() != string.Empty)
            {
                TimeDiff = Convert.ToInt32(oPage.Session[TimeOffset]);
            }
            return oTime.AddMinutes(-TimeDiff);
        }


        /// <summary>將參數來源時間減去時區設定值變UTC，再加DbUTC轉為主機資料本地時間</summary>
        /// <param name="oTime">客戶端的本地時間</param>
        /// <param name="oPage"></param>
        /// <returns></returns>
        public static DateTime GetUtcTime(this DateTime oTime, System.Web.UI.Page oPage)
        {
            if (!IsUtc)
            {
                return oTime;
            }
            string TimeOffset = "TimeOffset";
            int TimeDiff = -480;
            if (oPage.Session[TimeOffset] != null && oPage.Session[TimeOffset].ToString() != string.Empty)
            {
                TimeDiff = Convert.ToInt32(oPage.Session[TimeOffset]);
            }           
            //DateTime.Now.utc
            return oTime.AddMinutes(TimeDiff).AddHours(DbUTC);
        }


        /// <summary>將參數來源時間減去時區設定值變UTC</summary>
        /// <param name="oTime">客戶端的本地時間</param>
        /// <param name="oPage"></param>
        /// <returns></returns>
        public static DateTime GetUtcData(this DateTime oTime, System.Web.UI.Page oPage)
        {
            if (!IsUtc)
            {
                return oTime;
            }
            string TimeOffset = "TimeOffset";
            int TimeDiff = -480;
            if (oPage.Session[TimeOffset] != null && oPage.Session[TimeOffset].ToString() != string.Empty)
            {
                TimeDiff = Convert.ToInt32(oPage.Session[TimeOffset]);
            }
            //DateTime.Now.utc
            return oTime.AddMinutes(TimeDiff);
        }



        public static DateTime GetUtcNow(this DateTime oTime)
        {
            if (!IsUtc)
                return DateTime.Now;
            return DateTime.UtcNow;
        }


        public static List<string> VerifiEquList(this System.Web.UI.Page oPage)
        {
            List<string> list = new List<string>();
            list.Add("ADM100FP");
            list.Add("ADM100FT");
            list.Add("SST9500FP");
            list.Add("SST9500FT");
            try
            {
                XmlDocument xd = new XmlDocument();
                //string path = Request.PhysicalPath;
                var page = System.Web.HttpContext.Current;
                xd.Load(System.Web.HttpContext.Current.Server.MapPath("~/SysPara.xml"));
                XElement doc = XElement.Parse(xd.OuterXml);
                if (doc.Element("VerifiEquList") != null)
                {
                    return doc.Element("VerifiEquList").Value.Split(',').ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
            return list;
        }


        public static int DbUTC
        {
            get
            {
                try
                {
                    XmlDocument xd = new XmlDocument();
                    //string path = Request.PhysicalPath;
                    var page = System.Web.HttpContext.Current;
                    xd.Load(System.Web.HttpContext.Current.Server.MapPath("~/SysPara.xml"));
                    XElement doc = XElement.Parse(xd.OuterXml);
                    if (doc.Element("DbUTC") != null)
                    {
                        return int.Parse(doc.Element("DbUTC").Value);
                    }
                }
                catch (Exception ex)
                {
                    return 8;
                }                
                return 8;
            }
        }

        public static string GetSysParaData(string ParaName)
        {
            try
            {
                XmlDocument xd = new XmlDocument();
                var page = System.Web.HttpContext.Current;
                xd.Load(System.Web.HttpContext.Current.Server.MapPath("~/SysPara.xml"));
                XElement doc = XElement.Parse(xd.OuterXml);
                if (doc.Element(ParaName) != null)
                {
                    return doc.Element(ParaName).Value;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public static bool IsUtc
        {
            get
            {
                try
                {
                    XmlDocument xd = new XmlDocument();
                    //string path = Request.PhysicalPath;
                    var page = System.Web.HttpContext.Current;                    
                    xd.Load(System.Web.HttpContext.Current.Server.MapPath("~/SysPara.xml"));
                    XElement doc = XElement.Parse(xd.OuterXml);
                    if (doc.Element("IsUtc") != null)
                    {
                        if (doc.Element("IsUtc").Value == "1")
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                }
                catch (Exception ex)
                {
                    return false;
                }
                return false;
            }

        }

        public static string GetKeyInfo()
        {
            try
            {
                XmlDocument xd = new XmlDocument();
                var page = System.Web.HttpContext.Current;
                xd.Load(System.Web.HttpContext.Current.Server.MapPath("~/BearerToken.xml"));
                XElement doc = XElement.Parse(xd.OuterXml);
               return  doc.Element("VersionCode").Value;
            }
            catch (Exception ex)
            {
                //return ex.Message;
            }
            return "";
        }


        /// <summary>
        /// 執行list深複製
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToClone"></param>
        /// <returns></returns>
        public static List<T> Clone<T>(this List<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

    }//end service class
}//end namesapce