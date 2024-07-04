using System;
using System.Data;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.VisualBasic;
using System.Collections.Generic;


namespace Sa
{
    public partial class Check
    {
        //-------------------------------------------------------------------------------------------------------------------------------------------
        public Check()
        {
        }
        
        //-------------------------------------------------------------------------------------------------------------------------------------------
        // 檢查字串內容是否符合 IP Address 字串格式
        public static Boolean IsIPAddress(string sIp)
        {
            if (sIp == null || sIp == string.Empty || sIp.Length < 7 || sIp.Length > 15) return false;
            string sFormat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";
            Regex oRegex = new Regex(sFormat, RegexOptions.IgnoreCase);
            return oRegex.IsMatch(sIp);
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------
        // 檢查字串內容是否為空值、空字串
        public static Boolean IsEmptyStr(string sStr) 
        { 
            return ((sStr == null) || (sStr.Trim() == "")); 
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------
        // 檢查字串內容是否符合整數字串格式 
        public static Boolean IsInt(string sInt)
        {
            if ((sInt == null) || (sInt.Trim() == "")) return false;
            return Regex.IsMatch(sInt, @"^[+-]?\d*$");
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------
        // 檢查字串內容是否符合不帶正負號的整數字串格式
        public static Boolean IsUInt(string sUInt)
        {
            if ((sUInt == null) || (sUInt.Trim() == "")) return false;
            return Regex.IsMatch(sUInt, @"^\d*$");
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------
        // 檢查字串內容是否符合十六進位字串格式 
        public static Boolean IsHex(string sHex)
        {
            if ((sHex == null) || (sHex.Trim() == "")) return false;
            return Regex.IsMatch(sHex, @"^[A-F0-9]+$");
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------
        // 檢查字串內容是否符合實數字串格式
        public static Boolean IsNumeric(string sNumeric)
        {
            if ((sNumeric == null) || (sNumeric.Trim() == "")) return false;
            return Regex.IsMatch(sNumeric, @"^[+-]?\d*[.]?\d*$");
        }
        
        //-------------------------------------------------------------------------------------------------------------------------------------------
        // 檢查字串內容是否符合不帶正負號的實數字串格式
        public static Boolean IsUNumeric(string sUNumeric)
        {
            if ((sUNumeric == null) || (sUNumeric.Trim() == "")) return false;
            return Regex.IsMatch(sUNumeric, @"^\d*[.]?\d*$");
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------
        // 測試是否為電子郵件信箱
        public static Boolean IsEMailAddress(string sEMail)
        {
            if ((sEMail == null) || (sEMail.Trim() == "")) return false;
            return Regex.IsMatch(sEMail, @"^([a-zA-Z0-9_-][.]?)+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{1,4}){1,4})$");
        }
        
        //-------------------------------------------------------------------------------------------------------------------------------------------
        // 檢查輸入的字串是否符合日期格式
        public static Boolean IsDate(string sDateStr)
        {
            return IsDate(sDateStr, false);
        }
        public static Boolean IsDate(string sDateStr, Boolean IsPassNoData)
        {
            string sRetDate = "";
            return IsDate(sDateStr, IsPassNoData, out sRetDate);
        }
        // 檢查輸入的字串是否符合日期格式，並依格式回傳日期字串
        public static Boolean IsDate(string sDataStr, Boolean IsPassNoData, out string sRetDate)
        {
            Boolean IsTestOK = true;
            Boolean IsLeapYear = true;
            Int32 iLen, iYear, iMon, iDay;

            sDataStr = sDataStr.Trim();
            iLen = sDataStr.Length;

            if (iLen == 4) sDataStr = sDataStr.Substring(0, 2) + "/" + sDataStr.Substring(2, 2);                                    // (MM/DD)
            if (iLen == 6) sDataStr = sDataStr.Substring(0, 4) + "/" + sDataStr.Substring(5, 2);                                    // (YYYY/MM)
            if (iLen == 8) sDataStr = sDataStr.Substring(0, 4) + "/" + sDataStr.Substring(5, 2) + "/" + sDataStr.Substring(7, 2);   // (YYYY/MM/DD)      
            sRetDate = sDataStr;

            if (sDataStr == "" || sDataStr == "----/--/--" || sDataStr == "----/--" || sDataStr == "--/--") IsTestOK = IsPassNoData;
            else
            {
                iLen = sDataStr.Length;
                if (iLen == 5)
                {
                    if (sDataStr.Substring(2, 1) != "/") IsTestOK = false;
                    if (!IsUInt(sDataStr.Substring(0, 2))) IsTestOK = false;
                    if (!IsUInt(sDataStr.Substring(3, 2))) IsTestOK = false;
                }
                if (iLen == 7)
                {
                    if (sDataStr.Substring(4, 1) != "/") IsTestOK = false;
                    if (!IsUInt(sDataStr.Substring(0, 4))) IsTestOK = false;
                    if (!IsUInt(sDataStr.Substring(5, 2))) IsTestOK = false;
                }
                if (iLen == 10)
                {
                    if (sDataStr.Substring(4, 1) != "/") IsTestOK = false;
                    if (sDataStr.Substring(7, 1) != "/") IsTestOK = false;
                    if (!IsUInt(sDataStr.Substring(0, 4))) IsTestOK = false;
                    if (!IsUInt(sDataStr.Substring(5, 2))) IsTestOK = false;
                    if (!IsUInt(sDataStr.Substring(8, 2))) IsTestOK = false;
                }
                if (IsTestOK)
                {
                    iYear = 0;
                    iMon = 0;
                    iDay = 0;
                    if (iLen == 5)
                    {
                        iMon = Convert.ToInt16(sDataStr.Substring(0, 2));
                        iDay = Convert.ToInt16(sDataStr.Substring(3, 2));
                    }
                    if (iLen == 7)
                    {
                        iYear = Convert.ToInt16(sDataStr.Substring(0, 4));
                        iMon = Convert.ToInt16(sDataStr.Substring(5, 2));
                    }
                    if (iLen == 10)
                    {
                        iYear = Convert.ToInt16(sDataStr.Substring(0, 4));
                        iMon = Convert.ToInt16(sDataStr.Substring(5, 2));
                        iDay = Convert.ToInt16(sDataStr.Substring(8, 2));
                    }

                    Int32 i2Day = 28;
                    if (iYear > 0) IsLeapYear = DateTime.IsLeapYear(iYear);
                    if (IsLeapYear) i2Day = 29;

                    if (iMon < 1 || iMon > 12) IsTestOK = false;
                    else
                    {
                        Int32 nKDay = 31;
                        string sTmp = "0123050780A0C";
                        if (sTmp.Substring(iMon, 1) == "0") nKDay = 30;
                        else if (iMon == 2) nKDay = i2Day;
                        if (iDay < 0 || iDay > nKDay) IsTestOK = false;
                    }
                }
            }
            return IsTestOK;
        }

        public static bool CheckIDNumber(string psn_no)
        {
            Dictionary<string, int> area_code = new Dictionary<string, int>();
            area_code.Add("A", 10);
            area_code.Add("B", 11);
            area_code.Add("C", 12);
            area_code.Add("D", 13);
            area_code.Add("E", 14);
            area_code.Add("F", 15);
            area_code.Add("G", 16);
            area_code.Add("H", 17);
            area_code.Add("I", 34);
            area_code.Add("J", 18);
            area_code.Add("K", 19);

            area_code.Add("M", 21);
            area_code.Add("N", 22);
            area_code.Add("O", 35);
            area_code.Add("P", 23);
            area_code.Add("Q", 24);
            area_code.Add("T", 27);
            area_code.Add("U", 28);
            area_code.Add("V", 29);
            area_code.Add("W", 32);
            area_code.Add("X", 30);
            area_code.Add("Z", 33);

            area_code.Add("S", 26);
            area_code.Add("Y", 31);
            area_code.Add("L", 20);
            area_code.Add("R", 25);
            if (psn_no.Length != 10 || !area_code.ContainsKey(psn_no.Substring(0, 1)))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------
        // 檢查輸入的字串是否符合時間格式
        public static Boolean IsTime(string sTimeStr)
        {
            return IsTime(sTimeStr, false);
        }
        public static Boolean IsTime(string sTimeStr, Boolean IsPassNoData)
        {
            string sRetTime = "";
            return IsTime(sTimeStr, IsPassNoData, out sRetTime);
        }
        // 檢查輸入的字串是否符合時間格式，並回傳指定格式之時間字串
        public static Boolean IsTime(string sTimeStr, Boolean IsPassNoData, out string sRetTime)
        {
            Boolean IsTestOK = true;
            Int32 iHour, iMinute, iSecond, iLen;

            sTimeStr = sTimeStr.Trim();
            iLen = sTimeStr.Length;

            if (iLen == 4) sTimeStr = sTimeStr.Substring(0, 2) + ":" + sTimeStr.Substring(2, 2);
            if (iLen == 6) sTimeStr = sTimeStr.Substring(0, 2) + ":" + sTimeStr.Substring(2, 2) + ":" + sTimeStr.Substring(4, 2);
            sRetTime = sTimeStr;
            
            if (sTimeStr == "" || sTimeStr == "--:--" || sTimeStr == "--:--:--") IsTestOK = IsPassNoData;
            else
            {
                iLen = sTimeStr.Length;
                if ((iLen == 5) || (iLen == 8))
                {
                    if (sTimeStr.Substring(2, 1) != ":") IsTestOK = false;
                    if (!IsUInt(sTimeStr.Substring(0, 2))) IsTestOK = false;
                    if (!IsUInt(sTimeStr.Substring(3, 2))) IsTestOK = false;
                    if (iLen == 8)
                    {
                        if (sTimeStr.Substring(5, 1) != ":") IsTestOK = false;
                        if (!IsUInt(sTimeStr.Substring(6, 2))) IsTestOK = false;
                    }
                    if (IsTestOK)
                    {
                        iHour = Convert.ToInt16(sTimeStr.Substring(0, 2));
                        iMinute = Convert.ToInt16(sTimeStr.Substring(3, 2));
                        if ((iHour < 0) || (iHour > 23)) IsTestOK = false;
                        if ((iMinute < 0) || (iMinute > 59)) IsTestOK = false;
                        if (iLen == 8)
                        {
                            iSecond = Convert.ToInt16(sTimeStr.Substring(6, 2));
                            if ((iSecond < 0) || (iSecond > 59)) IsTestOK = false;
                        }
                    }
                }
                else IsTestOK = false;
            }
            return IsTestOK;
        }

    }

}