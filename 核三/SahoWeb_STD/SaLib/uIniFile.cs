using System;
using System.IO;
using System.Text;
using System.Data;
using System.Configuration;
using System.Runtime.InteropServices;


namespace Sa
{
    public partial class IniFile
    {
        // IniFile API 函數宣告
        [DllImport("kernel32")]     // 返回 0 表示失敗，非 0 為成功
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]     // 返回取得字串緩衝區的長度
        private static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        string sIniFilePath = "";

        //------------------------------------------------------------------------------------------------------------------------
        public IniFile(string iniFilePath)
        {
            sIniFilePath = iniFilePath;
        }

        //------------------------------------------------------------------------------------------------------------------------
        public string ReadString(string sSection, string sKey, string sDefault)
        {
            StringBuilder sTemp = new StringBuilder(1024);
            GetPrivateProfileString(sSection, sKey, "@@@ No Data @@@", sTemp, 1024, sIniFilePath);
            if (sTemp.ToString() == "@@@ No Data @@@")
            {
                WriteString(sSection, sKey, sDefault);
                return sDefault;
            }
            return sTemp.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------
        public Int32 ReadInt32(string sSection, string sKey, Int32 iDefault)
        {
            string sTemp = ReadString(sSection, sKey, iDefault.ToString());
            if (Check.IsInt(sTemp)) return Convert.ToInt32(sTemp);
            else return 0;
        }

        //------------------------------------------------------------------------------------------------------------------------
        public Boolean ReadBool(string sSection, string sKey, Boolean blDefault)
        {
            string sTemp = ReadString(sSection, sKey, blDefault.ToString());
            sTemp = sTemp.ToUpper();
            return Change.StringToBoolean(sTemp);
        }

        //------------------------------------------------------------------------------------------------------------------------
        public void WriteString(string sSection, string sKey, string sValue)
        {
            WritePrivateProfileString(sSection, sKey, sValue, sIniFilePath);
        }

        //------------------------------------------------------------------------------------------------------------------------
        public void WriteInt32(string sSection, string sKey, Int32 iValue)
        {
            WritePrivateProfileString(sSection, sKey, iValue.ToString(), sIniFilePath);
        }

        //------------------------------------------------------------------------------------------------------------------------
        public void WriteBool(string sSection, string sKey, Boolean blValue)
        {
            WritePrivateProfileString(sSection, sKey, blValue.ToString(), sIniFilePath);
        }
    }
}