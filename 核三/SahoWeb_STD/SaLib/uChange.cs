using System;
using System.Data;
using System.Configuration;
using System.Text;
using Microsoft.VisualBasic;
using System.Collections.Generic;


namespace Sa
{
    public partial class Change
    {
        //------------------------------------------------------------------------------------------------------------------------
        // IntToBin 將十進制數字轉成二進制字串
        public static string IntToBin(Int64 nNum) 
        { 
            return IntToBin(nNum, 8, false); 
        }
        public static string IntToBin(Int64 nNum, Boolean IsReverse) 
        { 
            return IntToBin(nNum, 8, IsReverse); 
        }
        public static string IntToBin(Int64 nNum, Int16 nLen) 
        { 
            return IntToBin(nNum, nLen, false); 
        }
        public static string IntToBin(Int64 nNum, Int32 nLen, Boolean IsReverse)
        {
            string sTmp = Convert.ToString(nNum, 2).PadLeft(nLen, '0');
            return (IsReverse) ? Fun.StrReverse(sTmp) : sTmp;
        }

        //------------------------------------------------------------------------------------------------------------------------
        // IntToHex 將十進制數字轉成十六進制字串
        public static string IntToHex(Int64 nNum) 
        { 
            return IntToHex(nNum, 2); 
        }
        public static string IntToHex(Int64 nNum, Int32 nLen) 
        { 
            return Convert.ToString(nNum, 16).PadLeft(nLen, '0').ToUpper(); 
        }

        //------------------------------------------------------------------------------------------------------------------------
        // BinToInt 將二進制字串轉成十進制數字
        public static Int16 BinToInt16(string sBin) 
        { 
            return Convert.ToInt16(sBin, 2); 
        }
        public static Int16 BinToInt16(string sBin, Boolean IsReverse) 
        { 
            return Convert.ToInt16((IsReverse) ? Fun.StrReverse(sBin) : sBin, 2); 
        }
        public static Int32 BinToInt32(string sBin) 
        { 
            return Convert.ToInt32(sBin, 2); 
        }
        public static Int32 BinToInt32(string sBin, Boolean IsReverse) 
        { 
            return Convert.ToInt32((IsReverse) ? Fun.StrReverse(sBin) : sBin, 2); 
        }
        public static Int64 BinToInt64(string sBin) 
        { 
            return Convert.ToInt64(sBin, 2); 
        }
        public static Int64 BinToInt64(string sBin, Boolean IsReverse) 
        { 
            return Convert.ToInt64((IsReverse) ? Fun.StrReverse(sBin) : sBin, 2); 
        }

        //------------------------------------------------------------------------------------------------------------------------
        // HexToInt 將十六進制字串轉成十進制數字
        public static Int16 HexToInt16(string sHex) 
        { 
            return Convert.ToInt16(sHex, 16); 
        }
        public static Int32 HexToInt32(string sHex) 
        { 
            return Convert.ToInt32(sHex, 16); 
        }
        public static Int64 HexToInt64(string sHex) 
        { 
            return Convert.ToInt64(sHex, 16); 
        }

        //------------------------------------------------------------------------------------------------------------------------
        // BinToHex 將二進制字串轉換成十六進制字串
        public static string BinToHex(string sBin) 
        { 
            return BinToHex(sBin, 2, false); 
        }
        public static string BinToHex(string sBin, Boolean IsReverse) 
        { 
            return BinToHex(sBin, 2, IsReverse); 
        }
        public static string BinToHex(string sBin, Int16 nLen) 
        { 
            return BinToHex(sBin, nLen, false); 
        }
        public static string BinToHex(string sBin, Int16 nLen, Boolean IsReverse) 
        { 
            return IntToHex(BinToInt64(sBin, IsReverse), nLen); 
        }

        //------------------------------------------------------------------------------------------------------------------------
        // HexToBin 將十六進制字串轉二進制字串
        public static string HexToBin(string sHex) 
        { 
            return HexToBin(sHex, 2, false); 
        }
        public static string HexToBin(string sHex, Boolean IsReverse) 
        { 
            return HexToBin(sHex, 2, IsReverse); 
        }
        public static string HexToBin(string sHex, Int16 nLen) 
        { 
            return HexToBin(sHex, nLen, false); 
        }
        public static string HexToBin(string sHex, Int16 nLen, Boolean IsReverse) 
        { 
            return IntToBin(HexToInt64(sHex), nLen, IsReverse); 
        }

        //------------------------------------------------------------------------------------------------------------------------
        // 將傳入之數字 nSource 依 nLen 之長度轉換為數字字串，且前面空白處補零
        public static string Ntoc(Int32 nSource, Int32 nLen) 
        { 
            return nSource.ToString().PadLeft(nLen, '0'); 
        }
        // 將傳入之字串 sSource 依 nLen 之長度轉換為數字字串，且前面空白處補零
        public static string Ntoc(string sSource, Int32 nLen)
        {
            Int32 nSource = Convert.ToInt32(sSource);
            return nSource.ToString().PadLeft(nLen, '0');
        }

        //------------------------------------------------------------------------------------------------------------------------
        // 將字串 TRUE,YES,ON,Y,1 等轉換為 true 其他則為 false
        public static Boolean StringToBoolean(string sBoolStr)
        {
            string sTmp = ":TRUE:YES:ON:Y:1:"; 
            return (sTmp.IndexOf(":" + sBoolStr.ToUpper() + ":") > -1);
        }


    }

}