using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DapperDataObjectLib;
using SahoAcs.DBClass;



namespace SahoAcs.DBClass
{
    public static class DongleVaries
    {

        public static OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());


        public static int GetMaxPerson()
        {           
            string CmdStr = "SELECT * FROM B00_SysParameter WHERE ParaNo IN ('SysInfo2','SysInfo1') ";
            string ParaValue = "";
            string Key = "";
            string DbIV = "";
            int maxdata = int.MaxValue;
            foreach(var para in odo.GetQueryResult(CmdStr))
            {
                if (Convert.ToString(para.ParaNo) == "SysInfo1" && Convert.ToString(para.ParaValue).Length > 32)
                {
                    DbIV = Convert.ToString(para.ParaValue);
                    DbIV = DbIV.Substring(16, 8);
                }
                if (Convert.ToString(para.ParaNo) == "SysInfo2" && Convert.ToString(para.ParaValue).Length > 32)
                {
                    Key = Convert.ToString(para.ParaValue);
                    Key = Key.Substring(16, 8);
                    ParaValue = Convert.ToString(para.ParaValue);
                    ParaValue = DesCryp.DecrypMsg(ParaValue.Substring(32,ParaValue.Length-32),DbIV,Key);
                    string[] LicInfo = ParaValue.Split(',');
                    if (LicInfo.Length >= 13)
                    {
                        if (LicInfo[7] == "0")
                            return maxdata;
                        maxdata = int.Parse(LicInfo[12]);
                    }
                }                
            }            
            return maxdata;
        }

        public static int GetCurrentCard()
        {
            string CmdStr = "SELECT COUNT(*) AS CardCount FROM B01_Card  ";
            var CardResult =  odo.GetQueryResult(CmdStr);
            int maxdata = 0;
            foreach(var o in CardResult)
            {
                maxdata = Convert.ToInt32(o.CardCount);
            }
            return maxdata;
        }

        public static string GetCustTitle()
        {
            string CmdStr = "SELECT * FROM B00_SysParameter WHERE ParaNo IN ('SysInfo2','SysInfo1') ";
            string ParaValue = "";
            string Key = "";
            string DbIV = "";
            foreach (var para in odo.GetQueryResult(CmdStr))
            {
                if (Convert.ToString(para.ParaNo) == "SysInfo1" && Convert.ToString(para.ParaValue).Length > 32)
                {
                    DbIV = Convert.ToString(para.ParaValue);
                    DbIV = DbIV.Substring(16, 8);
                }
                if (Convert.ToString(para.ParaNo) == "SysInfo2" && Convert.ToString(para.ParaValue).Length > 32)
                {
                    Key = Convert.ToString(para.ParaValue);
                    Key = Key.Substring(16, 8);
                    ParaValue = Convert.ToString(para.ParaValue);
                    ParaValue = DesCryp.DecrypMsg(ParaValue.Substring(32, ParaValue.Length - 32), DbIV, Key);
                    string[] LicInfo = ParaValue.Split(',');
                    if (LicInfo.Length >= 3)
                    {
                        ParaValue = LicInfo[2];
                    }
                }               
            }
            return ParaValue;
        }

        public static int GetMaxCtrls()
        {
            string CmdStr = "SELECT * FROM B00_SysParameter WHERE ParaNo IN ('SysInfo2','SysInfo1') ";
            string ParaValue = "";
            string Key = "";
            string DbIV = "";
            int maxdata = int.MaxValue;
            foreach (var para in odo.GetQueryResult(CmdStr))
            {
                if (Convert.ToString(para.ParaNo) == "SysInfo1" && Convert.ToString(para.ParaValue).Length > 32)
                {
                    DbIV = Convert.ToString(para.ParaValue);
                    DbIV = DbIV.Substring(16, 8);
                }
                if (Convert.ToString(para.ParaNo) == "SysInfo2" && Convert.ToString(para.ParaValue).Length > 32)
                {
                    Key = Convert.ToString(para.ParaValue);
                    Key = Key.Substring(16, 8);
                    ParaValue = Convert.ToString(para.ParaValue);
                    ParaValue = DesCryp.DecrypMsg(ParaValue.Substring(32, ParaValue.Length - 32), DbIV, Key);
                    string[] LicInfo = ParaValue.Split(',');
                    if (LicInfo.Length >= 14)
                    {
                        if (LicInfo[7] == "0")
                            return maxdata;
                        maxdata = int.Parse(LicInfo[13]);
                    }
                }
            }            
            return maxdata;
        }


        /// <summary>取得最大的使用者數量</summary>
        /// <returns></returns>
        public static int GetMaxUser()
        {
            string CmdStr = "SELECT * FROM B00_SysParameter WHERE ParaNo IN ('SysInfo2','SysInfo1') ";
            string ParaValue = "";
            string Key = "";
            string DbIV = "";
            int maxdata = int.MaxValue;
            foreach (var para in odo.GetQueryResult(CmdStr))
            {
                if (Convert.ToString(para.ParaNo) == "SysInfo1" && Convert.ToString(para.ParaValue).Length > 32)
                {
                    DbIV = Convert.ToString(para.ParaValue);
                    DbIV = DbIV.Substring(16, 8);
                }
                if (Convert.ToString(para.ParaNo) == "SysInfo2" && Convert.ToString(para.ParaValue).Length > 32)
                {
                    Key = Convert.ToString(para.ParaValue);
                    Key = Key.Substring(16, 8);
                    ParaValue = Convert.ToString(para.ParaValue);
                    ParaValue = DesCryp.DecrypMsg(ParaValue.Substring(32, ParaValue.Length - 32), DbIV, Key);
                    string[] LicInfo = ParaValue.Split(',');
                    if (LicInfo.Length >= 15)
                    {
                        if (LicInfo[7] == "0")
                            return maxdata;
                        maxdata = int.Parse(LicInfo[14]);
                    }
                }
            }           
            return maxdata;
        }

        public static string GetCurrentMobile()
        {
            string ResultVal = odo.GetStrScalar("SELECT COUNT(*) AS COUNTS FROM B01_Person WHERE CONVERT(VARCHAR,PsnID) IN (SELECT PsnDesc FROM LoginAuthTable) AND PsnAuthAllow=1 ");
            if (ResultVal == "")
                ResultVal = "0";
            return ResultVal;
        }


        public static int GetMaxMobile()
        {
            string CmdStr = "SELECT * FROM B00_SysParameter WHERE ParaNo IN ('SysInfo2','SysInfo1') ";
            string ParaValue = "";
            string Key = "";
            string DbIV = "";
            int maxdata = int.MaxValue;
            foreach (var para in odo.GetQueryResult(CmdStr))
            {
                if (Convert.ToString(para.ParaNo) == "SysInfo1" && Convert.ToString(para.ParaValue).Length > 32)
                {
                    DbIV = Convert.ToString(para.ParaValue);
                    DbIV = DbIV.Substring(16, 8);
                }
                if (Convert.ToString(para.ParaNo) == "SysInfo2" && Convert.ToString(para.ParaValue).Length > 32)
                {
                    Key = Convert.ToString(para.ParaValue);
                    Key = Key.Substring(16, 8);
                    ParaValue = Convert.ToString(para.ParaValue);
                    ParaValue = DesCryp.DecrypMsg(ParaValue.Substring(32, ParaValue.Length - 32), DbIV, Key);
                    string[] LicInfo = ParaValue.Split(',');
                    if (LicInfo.Length >= 17)
                    {
                        if (LicInfo[7] == "0")
                            return maxdata;
                        maxdata = int.Parse(LicInfo[16]);
                    }
                }
            }
            return maxdata;
        }

        public static bool GetEvoOpen()
        {
            string CmdStr = "SELECT * FROM B00_SysParameter WHERE ParaNo='SysInfo6' ";
            foreach (var para in odo.GetQueryResult(CmdStr))
            {
                string ParaValue = DesCryp.DecrypMsg(Convert.ToString(para.ParaValue));
                string[] LicInfo = ParaValue.Split(',');
            }
            return false;
        }

        public static List<SahoAcs.DBModel.ItemList> GetCardType()
        {
            List<SahoAcs.DBModel.ItemList> CardTypes = odo.GetQueryResult<SahoAcs.DBModel.ItemList>("SELECT * FROM B00_ItemList WHERE ItemClass='CardType' AND ItemNo NOT IN ('R') ORDER BY ItemOrder").ToList();
            //new List<DBModel.ItemList>();
            string CmdStr = "SELECT * FROM B00_SysParameter WHERE ParaNo IN ('SysInfo2','SysInfo1') ";
            string ParaValue = "";
            string Key = "";
            string DbIV = "";
            foreach (var para in odo.GetQueryResult(CmdStr))
            {
                if (Convert.ToString(para.ParaNo) == "SysInfo1" && Convert.ToString(para.ParaValue).Length > 32)
                {
                    DbIV = Convert.ToString(para.ParaValue);
                    DbIV = DbIV.Substring(16, 8);
                }
                if (Convert.ToString(para.ParaNo) == "SysInfo2" && Convert.ToString(para.ParaValue).Length > 32)
                {
                    Key = Convert.ToString(para.ParaValue);
                    Key = Key.Substring(16, 8);
                    ParaValue = Convert.ToString(para.ParaValue);
                    ParaValue = DesCryp.DecrypMsg(ParaValue.Substring(32, ParaValue.Length - 32), DbIV, Key);
                    string[] LicInfo = ParaValue.Split(',');
                    if (LicInfo.Length >= 11)
                    {
                        if (LicInfo[7] != "0")
                        {
                            ParaValue = LicInfo[10].Substring(1, 1);
                        }
                    }
                }
            }
            if (ParaValue == "1")
            {
                CardTypes = CardTypes.Take(1).ToList();
            }
            if (ParaValue == "2")
            {
                CardTypes = CardTypes.Take(2).ToList();
            }
            return CardTypes;
        }


        /// <summary>取得目前系統的使用者數量</summary>
        /// <returns></returns>
        public static int GetCurrentUser()
        {
            int NowData = odo.GetIntScalar("SELECT COUNT(*) AS Amount FROM B00_SysUser WHERE UserID NOT IN ('User','Saho','Single','Simple')");
            return NowData;
        }

        public static int GetCurrentCtrl()
        {
            int NowData = odo.GetIntScalar("SELECT COUNT(*) AS Amount FROM B01_Controller ");
            return NowData;
        }

        public static string GetSysVersion(this Page page)
        {
            string CmdStr = "SELECT * FROM B00_SysParameter WHERE ParaNo IN ('SysInfo1','SysInfo2') ";
            string ParaValue = "",VerName="";
            string DbIV = "", Key = "";
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("PRO", "專業版");
            dic.Add("STD", "標準版");
            dic.Add("ENT", "企業版");
            foreach (var para in odo.GetQueryResult(CmdStr))
            {
                if (Convert.ToString(para.ParaNo) == "SysInfo1" && Convert.ToString(para.ParaValue).Length>32)
                {
                    DbIV = Convert.ToString(para.ParaValue);
                    //DbIV.Length
                    DbIV = DbIV.Substring(16, 8);
                }
                if (Convert.ToString(para.ParaNo) == "SysInfo2" && Convert.ToString(para.ParaValue).Length > 32)
                {
                    Key = Convert.ToString(para.ParaValue);
                    Key = Key.Substring(16, 8);
                    ParaValue = Convert.ToString(para.ParaValue);
                    ParaValue = DesCryp.DecrypMsg(ParaValue.Substring(32, ParaValue.Length - 32), DbIV, Key);
                    string[] LicInfo = ParaValue.Split(',');
                    if (LicInfo.Length >= 5)
                    {
                        VerName = LicInfo[4];
                        if (page.Request.Cookies["i18n"].ToString() != null && page.Request.Cookies["i18n"].Value == "en-US")
                        {

                        }
                        else
                        {
                            if (dic.Keys.Where(i => i == VerName).Count() > 0)
                            {
                                VerName = dic[VerName];
                            }
                        }
                    }
                }                
            }
            return VerName;
        }


        /// <summary>取得目前SC的狀態</summary>
        /// <returns></returns>
        public static bool GetScAliveTime(DateTime DbTime)
        {
            string CmdStr = "SELECT * FROM B00_SysParameter WHERE ParaNo IN ('SysInfo2','SysInfo1') ";
            string ParaValue = "", DbIV="", Key="";
            //int maxdata = int.MaxValue;
            foreach (var para in odo.GetQueryResult(CmdStr))
            {
                if (Convert.ToString(para.ParaNo) == "SysInfo1" && Convert.ToString(para.ParaValue).Length > 32)
                {
                    DbIV = Convert.ToString(para.ParaValue);
                    DbIV = DbIV.Substring(16, 8);
                }
                if (Convert.ToString(para.ParaNo) == "SysInfo2" && Convert.ToString(para.ParaValue).Length > 32)
                {
                    Key = Convert.ToString(para.ParaValue);
                    Key = Key.Substring(16, 8);
                    ParaValue = Convert.ToString(para.ParaValue);
                    ParaValue = DesCryp.DecrypMsg(ParaValue.Substring(32,ParaValue.Length-32),DbIV,Key);
                    string[] LicInfo = ParaValue.Split(',');
                    if (LicInfo.Length >= 11)
                    {
                        ParaValue = LicInfo[7];
                    }
                }                 
            }
            var result = odo.GetQueryResult("SELECT * FROM B01_CommConnInfo ");
            if (result.Count() > 0)
            {
                try
                {
                    DateTime time1 = Convert.ToDateTime(result.First().UpdStateTime);
                    double timestamp = (DbTime - time1).TotalSeconds;
                    if (timestamp > 180 && ParaValue=="1")
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return true;
                }                
            }
            return true;
        }

        /// <summary>取得目前SD的狀態</summary>
        /// <returns></returns>
        public static bool GetSdAliveTime()
        {
            var result = odo.GetQueryResult("SELECT * FROM B01_DeviceConnInfo");
            if (result.Count() > 0)
            {
                try
                {
                    DateTime time1 = Convert.ToDateTime(result.First().UpdStateTime);
                    double timestamp = (DateTime.Now - time1).TotalSeconds;
                    if (timestamp > 180)
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return true;
                }                
            }
            return true;
        }

        public static bool GetDateAlive()
        {
            var result = odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo IN ('SysInfo2','SysInfo1') ");
            string ParaValue = "", Key="", DbIV = "";
            if (result.Count() > 0)
            {
                try
                {                                        
                    foreach(var para in result)
                    {
                        if (Convert.ToString(para.ParaNo) == "SysInfo1" && Convert.ToString(para.ParaValue).Length > 32)
                        {
                            DbIV = Convert.ToString(para.ParaValue);
                            DbIV = DbIV.Substring(16, 8);
                        }
                        if (Convert.ToString(para.ParaNo) == "SysInfo2" && Convert.ToString(para.ParaValue).Length > 32)
                        {
                            Key = Convert.ToString(para.ParaValue);
                            Key = Key.Substring(16, 8);
                            ParaValue = Convert.ToString(para.ParaValue);
                            ParaValue = DesCryp.DecrypMsg(ParaValue.Substring(32, ParaValue.Length - 32), DbIV, Key);
                            if (ParaValue.Split(',')[7] == "0")
                            {
                                return false;
                            }
                            DateTime time1 = Convert.ToDateTime(ParaValue.Split(',')[6]);
                            double timestamp = (time1 - DateTime.Now).TotalDays;
                            if (timestamp < 20)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }                    
                }
                catch (Exception ex)
                {
                    return false;
                }               
            }
            return false;
        }
        


        /// <summary>取得系統租用到期時間資訊</summary>
        /// <returns></returns>
        public static Tuple<bool, DateTime> GetDateAliveInfo()
        {
            var result = odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo IN ('SysInfo2','SysInfo1') ");
            string ParaValue = "", Key = "", DbIV = "";
            if (result.Count() > 0)
            {
                try
                {
                    foreach (var para in result)
                    {
                        if (Convert.ToString(para.ParaNo) == "SysInfo1" && Convert.ToString(para.ParaValue).Length > 32)
                        {
                            DbIV = Convert.ToString(para.ParaValue);
                            DbIV = DbIV.Substring(16, 8);
                        }
                        if (Convert.ToString(para.ParaNo) == "SysInfo2" && Convert.ToString(para.ParaValue).Length > 32)
                        {
                            Key = Convert.ToString(para.ParaValue);
                            Key = Key.Substring(16, 8);
                            ParaValue = Convert.ToString(para.ParaValue);
                            ParaValue = DesCryp.DecrypMsg(ParaValue.Substring(32,ParaValue.Length-32),DbIV,Key);
                            DateTime time1 = Convert.ToDateTime(ParaValue.Split(',')[6]);
                            double timestamp = (time1 - DateTime.Now).TotalDays;
                            if (timestamp < -1)
                            {
                                return Tuple.Create(true, time1);
                            }
                            else
                            {
                                return Tuple.Create(false, time1);
                            }
                        }
                    }                            
                }
                catch (Exception ex)
                {
                    return Tuple.Create(false, DateTime.Now);
                }
            }
            return Tuple.Create(false, DateTime.Now);
        }


        /// <summary>取得系統功能目錄</summary>
        /// <returns>功能目錄清單</returns>
        public static List<string> GetMenuList(this string UserID)
        {            
            string StrMenuNo = @"";
            List<string> MenuList = new List<string>();
            var keyinfo = odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo IN ('SysInfo2','SysInfo1') ");
            string ParaValue = "", Key = "", DbIV = "";
            foreach (var para in keyinfo)
            {
                if (Convert.ToString(para.ParaNo) == "SysInfo1" && Convert.ToString(para.ParaValue).Length > 32)
                {
                    DbIV = Convert.ToString(para.ParaValue);
                    DbIV = DbIV.Substring(16, 8);
                }
                if (Convert.ToString(para.ParaNo) == "SysInfo2" && Convert.ToString(para.ParaValue).Length > 32)
                {
                    Key = Convert.ToString(para.ParaValue);
                    Key = Key.Substring(16, 8);
                    ParaValue = Convert.ToString(para.ParaValue);
                    ParaValue = DesCryp.DecrypMsg(ParaValue.Substring(32, ParaValue.Length - 32), DbIV, Key);
                    string[] LicInfo = ParaValue.Split(',');
                    if (LicInfo.Length > 7 && LicInfo[7] == "0")
                        return MenuList;
                }
            }
            var result = odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='SysInfo5' ");
            if (UserID == "Saho")
            {
                result = odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='SysInfo6' ");
            }
            foreach(var o in result)
            {
                StrMenuNo = DesCryp.DecrypMsg(Convert.ToString(o.ParaValue),DbIV,Key);
            }
            string[] MainFun = StrMenuNo.Split(',');           
            
            foreach(var menuup in MainFun)
            {
                if (menuup.Split(':').Length > 1)
                {
                    foreach(var submenu in menuup.Split(':')[1].Split(';'))
                    {
                        if (submenu.Split('|').Length > 0)
                        {
                            MenuList.Add(submenu.Split('|')[0]);
                        }
                    }
                }
            }
            if (MenuList.Count > 0)
            {
                var result2 = odo.GetQueryResult("SELECT * FROM B00_SysMenu WHERE MenuNo LIKE '%zz%' ");
                foreach (var o in result2)
                {
                    MenuList.Add(Convert.ToString(o.MenuNo));
                }
            }


            #region 加入客製化網頁 SysInfoC 04:0401|1+6,06:0601|1+2;0602|
            //var resultC = odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='SysInfoC' ");
            //string strMenuNoC = string.Empty;
            //foreach (var o in resultC)
            //{
            //    strMenuNoC = DesCryp.DecrypMsg(Convert.ToString(o.ParaValue), DbIV, Key);
            //}

            //string[] MainFunC = null;
            //List<string> MenuListC = new List<string>();

            //if (!string.IsNullOrEmpty(strMenuNoC))
            //{
            //    MainFunC = strMenuNoC.Split(',');
            //}

            //foreach (var menuup in MainFunC)
            //{
            //    if (menuup.Split(':').Length > 1)
            //    {
            //        foreach (var submenu in menuup.Split(':')[1].Split(';'))
            //        {
            //            if (submenu.Split('|').Length > 0)
            //            {
            //                MenuListC.Add(submenu.Split('|')[0]);
            //            }
            //        }
            //    }
            //}

            //if (MenuListC.Count>0)
            //{
            //    foreach (var item in MenuListC)
            //    {
            //        MenuList.Add(item);
            //    }
            //    MenuList.Sort((x, y) => x.CompareTo(y));
            //}
            #endregion


            return MenuList;
        }


        /// <summary>取得系統功能目錄</summary>
        /// <returns>功能目錄清單</returns>
        public static List<string> GetMenuList(this Page page)
        {
            string UserID = page.Session["UserID"].ToString();
            List<string> MenuList = new List<string>();
            MenuList = UserID.GetMenuList();            
            return MenuList;
        }

    }//end class
}//end namespace