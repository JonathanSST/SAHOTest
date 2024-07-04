using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;


namespace Sa
{
    //----------------------------------------------------------------------------------------------------------------------------
    // 檔案操作模式列舉
    public enum FileWriteMode : byte
    {
        Append = 0,
        AppendLine = 1,
        Write = 2,
        WriteLine = 3
    }

    //----------------------------------------------------------------------------------------------------------------------------
    // 記錄資料時間加註操作模式列舉
    public enum SaveTimeMarkMode : byte
    {
        NoSpace = 0,                 
        UseSpaceMark = 1,           // "         -> "
        UseTimeMark = 2             // "hh:mm:ss -> "
    }

    //----------------------------------------------------------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential)]
    public struct CopyDataStruct
    {
        public IntPtr dwData;
        public int cbData;
        public IntPtr lpData;
    }


    public class EMailInfo
    {
        public string sSender = "";
        public string sDisplayName = "";
        public string sServer = "";
        public Boolean IsCheckAuth = false;
        public string sLoginName = "";
        public string sLoginPass = "";
    }

    public partial class Fun
    {
        public static void ShowMessage(string sCaption, string sMessage)
        {
            MessageBoxButtons Buttons = MessageBoxButtons.OK;
            MessageBoxIcon Icon = MessageBoxIcon.Information;
            MessageBox.Show(sMessage, sCaption, Buttons, Icon);
        }

        //------------------------------------------------------------------------------------------------------------------------
        public static Boolean SendEmail(EMailInfo oMailInfo, string sMailTo, string sSubject, string sBody, string sFile, out string sResult)
        {
            Boolean IsOK = true;
            Array aMailTo, aFile;

            if (Check.IsEmptyStr(oMailInfo.sSender) || 
                Check.IsEmptyStr(oMailInfo.sServer) || 
                (oMailInfo.IsCheckAuth && Check.IsEmptyStr(oMailInfo.sLoginName)) ||
                Check.IsEmptyStr(sMailTo))
            {
                sResult = "電子郵件未設定，無法傳送電子郵件：";
                if (Check.IsEmptyStr(oMailInfo.sSender)) sResult += "未設定寄件者;";
                if (Check.IsEmptyStr(oMailInfo.sServer)) sResult += "未設定寄件伺服器;";
                if (Check.IsEmptyStr(sMailTo)) sResult += "未設定收件者;";
                if (oMailInfo.IsCheckAuth && Check.IsEmptyStr(oMailInfo.sLoginName)) sResult += "已設定認證，但未設定登入帳號;";
                IsOK = false;
            }
            else
            {
                sResult = "電子郵件傳送成功";

                MailMessage oMail = new System.Net.Mail.MailMessage();
                try
                {
                    // 處理收件人
                    aMailTo = sMailTo.Trim().Split(new char[] { ';', ',' });
                    foreach (string sToUser in aMailTo)
                    {
                        if (Check.IsEMailAddress(sToUser)) oMail.To.Add(new MailAddress(sToUser));
                    }
                    if (oMail.To.Count == 0)
                    {
                        sResult = "電子郵件傳送-沒有設定收件人";
                        IsOK = false;
                    }

                    if (IsOK)
                    {
                        // 處理附件
                        if (sFile.Trim() != "")
                        {
                            aFile = sFile.Trim().Split(new char[] { ';', ',' });
                            foreach (string sAttachFile in aFile)
                            {
                                if ((sAttachFile != "") && (File.Exists(sAttachFile))) oMail.Attachments.Add(new Attachment(sAttachFile));
                            }
                        }

                        if (oMailInfo.sDisplayName == "") oMailInfo.sDisplayName = oMailInfo.sSender;
                        oMail.From = new MailAddress(oMailInfo.sSender, oMailInfo.sDisplayName, System.Text.Encoding.UTF8);
                        oMail.Subject = sSubject;
                        oMail.SubjectEncoding = System.Text.Encoding.UTF8;
                        
                        oMail.Body = sBody;
                        oMail.IsBodyHtml = false;
                        oMail.Priority = MailPriority.Normal;
                        SmtpClient oSmtp = new SmtpClient();

                        if (oMailInfo.IsCheckAuth)  // 登入認證
                        {
                            oSmtp.Credentials = new System.Net.NetworkCredential(oMailInfo.sLoginName, oMailInfo.sLoginPass);
                        }

                        oSmtp.Host = oMailInfo.sServer;

                        try
                        {
                            oSmtp.Send(oMail);
                        }
                        catch (System.Net.Mail.SmtpException em)
                        {
                            sResult = em.Message + "-" + em.InnerException.Message;
                            IsOK = false;
                        }
                    }
                }
                finally
                {
                    oMail.Dispose();
                }
            }
            return IsOK;
        }

        //------------------------------------------------------------------------------------------------------------------------
        // 向指定的控制項內所有的子控制項傳送訊息
        public static void BroadcastMessage(Control oControls, int WinMessageNo, IntPtr wParam, int lParam)
        {
            foreach (Control oObj in oControls.Controls)
            {
                WinApi.PostMessage(oObj.Handle, WinMessageNo, wParam, lParam);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        // 向指定的視窗傳送字串訊息
        public static void SendMessageToWindows(IntPtr hWnd, int iMsg, string sMsgStr)
        {
            IntPtr ptr = Marshal.StringToHGlobalAuto(sMsgStr);
            CopyDataStruct cds = new CopyDataStruct();
            cds.dwData = IntPtr.Zero;
            cds.cbData = sMsgStr.Length;
            cds.lpData = ptr;

            WinApi.SendMessage(hWnd, iMsg, 0, ref cds);
            Marshal.FreeHGlobal(ptr);
        }

        //------------------------------------------------------------------------------------------------------------------------
        // 向指定的程式傳送字串訊息
        public static void SendMessageToWindows(string sFromName, int iMsg, string sMsgStr)
        {
            Process[] aProc = Process.GetProcessesByName(sFromName);
            if (aProc.Length > 0)
            {
                foreach (Process oProc in aProc)
                {
                    SendMessageToWindows(oProc.Handle, iMsg, sMsgStr);
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        // 從傳給視窗的訊息中取出字串
        public static string GetWindowMessageString(Message m)
        {
            CopyDataStruct vCopyDataStruct = (CopyDataStruct)Marshal.PtrToStructure(m.LParam, typeof(CopyDataStruct));
            return Marshal.PtrToStringAuto(vCopyDataStruct.lpData);
        }




        //------------------------------------------------------------------------------------------------------------------------
        // 從字串參數 cStr 中的 nPoint 位置取出 nLength 個字元以 sReplceStr 插入取代
        public static string Stuff(string sStr, Int32 iPoint, Int32 iLength, string sReplceStr)
        {
            Int32 iStrLen = sStr.Length;
            string sLastStr = "";
            if (iPoint >= iStrLen) iPoint = iStrLen;
            if ((iPoint + iLength) < iStrLen) sLastStr = sStr.Substring(iPoint + iLength);
            return sStr.Substring(0, iPoint) + sReplceStr + sLastStr;
        }

        //------------------------------------------------------------------------------------------------------------------------
        // 從字串後面取出 nLength 個字元字串
        public static string RStr(string sStr, Int32 iLength)
        {
            Int32 iStrLen = sStr.Length;
            if (iLength >= iStrLen) iLength = iStrLen;
            return sStr.Substring(iStrLen - iLength, iLength);
        }

        //------------------------------------------------------------------------------------------------------------------------
        // 產生重覆的字串
        public static string Repeat(string sBaseStr, Int32 iFrequency)
        {
            StringBuilder sTmp = new StringBuilder("");
            for (Int32 i = 0; i < iFrequency; i++) sTmp.Append(sBaseStr);
            return sTmp.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------
        // 將字串進行前後反轉處理
        public static string StrReverse(string sStr)
        {
            StringBuilder sTmp = new StringBuilder("");
            for (Int32 i = sStr.Length - 1; i > -1; i--) sTmp.Append(sStr.Substring(i, 1));
            return sTmp.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------
        // 取出在 sSource 中 cSeparate 之前的子字串(不含 sSeparate)
        public static string GetStrL(string sSource, string sSeparate)
        {
            string sRet = "";
            Int32 iLen = sSource.IndexOf(sSeparate);
            if (iLen > 0) sRet = sSource.Substring(0, iLen);
            return sRet;
        }
        // 取出在 sSource 中 cSeparate 之前的子字串(不含 sSeparate)，並可選擇移除取出的字串(含 sSeparate)
        public static string GetStrL(ref string sSource, string sSeparate, Boolean IsMove)
        {
            string sRet = "";
            Int32 iLen = sSource.IndexOf(sSeparate);
            if (iLen > 0)
            {
                sRet = sSource.Substring(0, iLen);
                if (IsMove) sSource = sSource.Substring(iLen + sSeparate.Length);
            }
            return sRet;
        }

        //------------------------------------------------------------------------------------------------------------------------
        // 產生一個亂數字串
        public static string GetRndString()
        {
            string sSub1 = DateTime.Now.Ticks.ToString();
            Int32 i = sSub1.Length - 1;
            while (sSub1.Substring(i, 1) == "0")
            {
                i--;
                if (i == -1) break;
            }
            i++;
            if (i < sSub1.Length) sSub1 = sSub1.Remove(i);
            if (i > 5) sSub1 = sSub1.Substring(i - 5);

            VBMath.Randomize(DateTime.Now.Ticks);
            float nRnd = VBMath.Rnd() * 1000000;
            string sSub2 = nRnd.ToString();
            i = sSub2.LastIndexOf('.');
            if (i > -1) sSub2 = sSub2.Remove(i);
            i = sSub2.Length - 1;
            while (sSub2.Substring(i, 1) == "0")
            {
                i--;
                if (i == -1) break;
            }
            i++;
            if (i < sSub2.Length) sSub2 = sSub2.Remove(i);
            if (i > 3) sSub2 = sSub2.Substring(i - 3);
            return sSub1 + "_" + sSub2;
        }


        //------------------------------------------------------------------------------------------------------------------------
        // 將文字串寫入指定檔案中
        public static Boolean SaveToFile(string sFileName, string sData, FileWriteMode emMode)
        {
            Boolean IsOK = false;
            string sPath = Path.GetDirectoryName(sFileName);
            if (Directory.Exists(sPath))
            {
                StreamWriter oSw = null;
                try
                {
                    if (File.Exists(sFileName)) 
                        if (emMode == FileWriteMode.Write || emMode == FileWriteMode.WriteLine) File.Delete(sFileName);
                    oSw = File.AppendText(sFileName);
                    if (emMode == FileWriteMode.Append || emMode == FileWriteMode.Write) oSw.Write(sData);
                    if (emMode == FileWriteMode.AppendLine || emMode == FileWriteMode.WriteLine) oSw.WriteLine(sData);
                    IsOK = true;
                }
                catch (Exception e)
                {
                    EventLog(e.ToString());
                    EventLog("Save Data '" + sData + "' To " + sFileName, SaveTimeMarkMode.UseTimeMark);
                }
                if (oSw != null) oSw.Dispose();
            }
            else 
            {
                EventLog("存檔路徑不存在");
                EventLog("Save Data '" + sData + "' To " + sFileName, SaveTimeMarkMode.UseTimeMark);
            }
            return IsOK;
        }

        //------------------------------------------------------------------------------------------------------------------------
        // 取得自身執行檔路徑
        public static string GetApPath()
        {
            return Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
        }
        
        //------------------------------------------------------------------------------------------------------------------------
        // 取得自身執行檔名稱
        public static string GetApName()
        {
            return System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        }

        //------------------------------------------------------------------------------------------------------------------------
        // 取得自身程式版號
        public static string GetVersion()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            if (attributes.Length == 0)
            {
                return "";
            }
            else
            {
                return ((AssemblyFileVersionAttribute)attributes[0]).Version;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        // 取得到指定程序集版號
        public static string GetVersion(string sName)
        {
            byte[] aData = File.ReadAllBytes(sName);
            return Assembly.Load(aData).GetName().Version.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------
        // 將發生的錯誤訊息記錄下來
        public static void SaveExceptionLog(Exception e)
        {
            string sErrorLog =
                "     Message : " + e.Message + "\r\n" +
                "      Source : " + e.Source + "\r\n" +
                "  StackTrace : " + e.StackTrace + "\r\n" +
                "  TargetSite : " + e.TargetSite;
            EventLog(sErrorLog, SaveTimeMarkMode.NoSpace);
        }

        //------------------------------------------------------------------------------------------------------------------------
        // 將錯誤訊息寫入記錄中
        public static void EventLog(string sLogMsg)
        {
            EventLog(sLogMsg, SaveTimeMarkMode.UseTimeMark);
        }
        public static void EventLog(string sLogMsg, SaveTimeMarkMode enTimeMark)
        {
            string sRootPath = Path.DirectorySeparatorChar + "System_Log";
            string sLogPath = "";
            /*
            if (Directory.Exists("V:" + Path.DirectorySeparatorChar + "Temp"))
            {
                sLogPath = "V:" + sRootPath;
                Directory.CreateDirectory(sLogPath);
            }

            if (sLogPath == "")
            {
                if (Directory.Exists("C:" + sRootPath)) 
                    sLogPath = "C:" + sRootPath;
            }
            */
            if (sLogPath == "")
            {
                //sLogPath = Directory.GetCurrentDirectory() + sRootPath;
                sLogPath = System.AppDomain.CurrentDomain.BaseDirectory+ sRootPath;
                if (!Directory.Exists(sLogPath)) 
                    Directory.CreateDirectory(sLogPath);
            }

            sLogPath = sLogPath + Path.DirectorySeparatorChar + "Event";
            if (!Directory.Exists(sLogPath)) 
                Directory.CreateDirectory(sLogPath);

            string sApName = Path.GetFileNameWithoutExtension(GetApName());
            string sDate = DateTime.Today.ToString("_yyyy-MM-dd");
            string sLogFile = sLogPath + Path.DirectorySeparatorChar + sApName + sDate + ".Txt";
            string sTime = "";
            if (enTimeMark == SaveTimeMarkMode.UseSpaceMark) 
                sTime = "         > ";
            if (enTimeMark == SaveTimeMarkMode.UseTimeMark) 
                sTime = DateTime.Now.ToString("HH:mm:ss") + " > ";
            SaveToFile(sLogFile, sTime + sLogMsg, FileWriteMode.AppendLine);
        }

        //------------------------------------------------------------------------------------------------------------------------
        // 將除錯訊息寫入記錄中
        public static void DebugLog(string sLogMsg)
        {
            DebugLog(sLogMsg, SaveTimeMarkMode.UseTimeMark);
        }
        public static void DebugLog(string sLogMsg, SaveTimeMarkMode enTimeMark)
        {
            string sRootPath = Path.DirectorySeparatorChar + "System_Log";
            string sLogPath = "";

            if (Directory.Exists("V:" + Path.DirectorySeparatorChar + "Temp"))
            {
                sLogPath = "V:" + sRootPath;
                Directory.CreateDirectory(sLogPath);
            }

            if (sLogPath == "")
            {
                if (Directory.Exists("C:" + sRootPath)) 
                    sLogPath = "C:" + sRootPath;
            }

            if (sLogPath == "")
            {
                sLogPath = Directory.GetCurrentDirectory() + sRootPath;
                if (!Directory.Exists(sLogPath)) 
                    Directory.CreateDirectory(sLogPath);
            }

            sLogPath = sLogPath + Path.DirectorySeparatorChar + "Debug";
            if (Directory.Exists(sLogPath))
            {
                string sApName = Path.GetFileNameWithoutExtension(GetApName());
                string sDate = DateTime.Today.ToString("_yyyy-MM-dd");
                string sLogFile = sLogPath + Path.DirectorySeparatorChar + sApName + sDate + ".Txt";
                string sTime = "";
                if (enTimeMark == SaveTimeMarkMode.UseSpaceMark) sTime = "         > ";
                if (enTimeMark == SaveTimeMarkMode.UseTimeMark) sTime = DateTime.Now.ToString("HH:mm:ss") + " > ";
                SaveToFile(sLogFile, sTime + sLogMsg, FileWriteMode.AppendLine);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        // 將訊息寫入記錄中
        public static void MsgLog(string sLogMsg)
        {
            MsgLog(sLogMsg, SaveTimeMarkMode.UseTimeMark);
        }
        public static void MsgLog(string sLogMsg, SaveTimeMarkMode enTimeMark)
        {
            string sApName = Path.GetFileNameWithoutExtension(GetApName());

            string sLogPath = "";
            if (Directory.Exists("V:" + Path.DirectorySeparatorChar + "Temp"))
            {
                sLogPath = "V:" + Path.DirectorySeparatorChar + sApName + "_Log";
                if (!Directory.Exists(sLogPath)) Directory.CreateDirectory(sLogPath);
            }

            if (sLogPath == "")
            {
                sLogPath = Directory.GetCurrentDirectory() + "Log";
                if (!Directory.Exists(sLogPath)) Directory.CreateDirectory(sLogPath);
            }

            string sDate = DateTime.Today.ToString("yyyy-MM-dd");
            string sLogFile = sLogPath + Path.DirectorySeparatorChar + sDate + ".Txt";
            string sTime = "";
            if (enTimeMark == SaveTimeMarkMode.UseSpaceMark) sTime = "         > ";
            if (enTimeMark == SaveTimeMarkMode.UseTimeMark) sTime = DateTime.Now.ToString("HH:mm:ss") + " > ";
            SaveToFile(sLogFile, sTime + sLogMsg, FileWriteMode.AppendLine);
        }

        //-------------------------------------------------------------------------------------------------------------
        public static void SaveToLogFile(string sLogFile, SaveTimeMarkMode enTimeMark, string sLogMsg)
        {
            string sTime = "";
            if (enTimeMark == SaveTimeMarkMode.UseSpaceMark) sTime = "         > ";
            if (enTimeMark == SaveTimeMarkMode.UseTimeMark) sTime = DateTime.Now.ToString("HH:mm:ss") + " > ";
            SaveToFile(sLogFile, sTime + sLogMsg, FileWriteMode.AppendLine);
        }



        //------------------------------------------------------------------------------------------------------------------------
        // 將除錯訊息寫入系統記錄中
        public static void SaveToSystemLog(string sLogMsg)
        {
            System.Diagnostics.EventLog oLog = new System.Diagnostics.EventLog();
            oLog.Source = "XBaby_" + Path.GetFileNameWithoutExtension(GetApName());
            oLog.WriteEntry(sLogMsg);
            oLog.Close();
            oLog.Dispose();
        }

    
        //------------------------------------------------------------------------------------------------------------------------
        // 字串加密
        static string constEncryptKey = "CADmvNlOBSYKjLZWu2PgIabinef4H7Qd8y3R0tTpE9wVhxXo6sJ1qFkzMrUG5c";
        
        public static string Encrypt(string sData)
        {
            Byte[] aData, aKey;
            StringBuilder sbStr = new StringBuilder("");
            Int32 nLoop, nIndex, nAdd, nTmp1, nTmp2;
            string sStr, sTmp;

            if (sData != "")
            {
                aKey = System.Text.Encoding.Default.GetBytes(constEncryptKey);
                aData = System.Text.Encoding.Default.GetBytes(sData);
                nIndex = (aData.Length % aKey.Length);
                nAdd = aKey[nIndex];
                for (nLoop = 0; nLoop < aData.Length; nLoop++)
                {
                    sTmp = Change.Ntoc((aData[nLoop] << 3) + nAdd, 4);
                    nTmp1 = Convert.ToInt32(sTmp.Substring(0, 2));
                    nTmp2 = Convert.ToInt32(sTmp.Substring(2, 2));
                    if (nTmp2 > 40)
                    {
                        nTmp1 = nTmp1 + 40;
                        nTmp2 = nTmp2 - 40;
                    }
                    sTmp = Change.Ntoc(nTmp1, 2) + Change.Ntoc(nTmp2, 2);
                    sbStr.Append(RStr(sTmp, 4));
                }
                sStr = sbStr.ToString();
                sbStr.Length = 0;
                nTmp1 = sStr.Length / 2;
                for (nLoop = 0; nLoop < nTmp1; nLoop++) sbStr.Append(Convert.ToChar(aKey[Convert.ToInt32(sStr.Substring(nLoop * 2, 2))]));
            }
            return sbStr.ToString();
        }
        
        //------------------------------------------------------------------------------------------------------------------------
        // 字串解密
        public static string Decrypt(string sData)
        {
            byte[] array = new byte[sData.Length / 2];
            StringBuilder stringBuilder = new StringBuilder("");
            //bool flag = iKeyNo < 0 || iKeyNo > Fun.asEncryptKey.Length - 1;
            //if (flag)
            //{
            //    iKeyNo = 0;
            //}
            bool flag2 = sData != "";
            if (flag2)
            {
                byte[] bytes = Encoding.Default.GetBytes(constEncryptKey);
                byte[] bytes2 = Encoding.Default.GetBytes(sData);               
                int num = bytes2.Length / 2 % bytes.Length;
                int num2 = (int)bytes[num];
                int num3;
                for (int i = 0; i < bytes2.Length; i = num3 + 1)
                {
                    num = constEncryptKey.IndexOf(Convert.ToChar(bytes2[i]));
                    stringBuilder.Append(Change.Ntoc(num, 2));
                    num3 = i;
                }
                string text = stringBuilder.ToString();
                stringBuilder.Length = 0;
                for (int i = 0; i < text.Length / 4; i = num3 + 1)
                {
                    string text2 = text.Substring(i * 4, 4);
                    int num4 = Convert.ToInt32(text2.Substring(0, 2));
                    int num5 = Convert.ToInt32(text2.Substring(2, 2));
                    bool flag3 = num4 > 40;
                    if (flag3)
                    {
                        num4 -= 40;
                        num5 += 40;
                    }
                    text2 = Change.Ntoc(num4, 2) + Change.Ntoc(num5, 2);
                    num4 = Convert.ToInt32(text2) - num2 >> 3;
                    array[i] = (byte)num4;
                    num3 = i;
                }
            }
            return Encoding.Default.GetString(array);
        }//end method

    }//end class
}//end namespace