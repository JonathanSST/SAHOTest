using System;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Collections.Generic;
using System.IO;
using SahoAcs;
using System.Xml;
using System.Xml.Linq;


public class Pub
{
    public static string sConnName = "SahoSMS-Sql";
    public static string[] aMenu = new string[8];
    public static string sTitel = ConfigurationManager.AppSettings["TitleName"].ToString();
    public static string sNumStr = "一二三四五六七八九十";

    public static string db_user_id = WebConfigurationManager.AppSettings["DbID"];
    public static string db_pwd = WebConfigurationManager.AppSettings["DbPW"];
    public static string db_source_ip = WebConfigurationManager.AppSettings["DbSource"];
    public static string db_data_name = ConfigurationManager.AppSettings["DbName"].ToString();
    public static string db_connection_template = @"Addr={0};Database={1};uid={2};pwd='{3}'";
    public static string WorkOverTime = WebConfigurationManager.AppSettings["WorkOverTime"] != null
        ? WebConfigurationManager.AppSettings["WorkOverTime"] : "24";
    public static string RestRange = WebConfigurationManager.AppSettings["RestRange"] != null
        ? WebConfigurationManager.AppSettings["RestRange"] : "12:00-12:30";
    public static string ReportOverTime = WebConfigurationManager.AppSettings["ReportOverTime"] != null
        ? WebConfigurationManager.AppSettings["ReportOverTime"] : "12";
    public static string SmsTitlePath = WebConfigurationManager.AppSettings["SmsTitlePath"] != null
        ? WebConfigurationManager.AppSettings["SmsTitlePath"] : @"D:\Saho\SmsTitle";

    public static string StrInsertSysLog = @"INSERT INTO B00_SysLog (LogTime,LogType,UserID,UserName,LogFrom,LogIP,EquNo,EquName,LogInfo,LogDesc)
                                                                VALUES (GETDATE(),@p1,@p2,@p3,@p4, @p5, @p6, @p7, @p8, @p9)";

    public static string SetHeat = WebConfigurationManager.AppSettings["SetHeat"] != null
        ? WebConfigurationManager.AppSettings["SetHeat"] : "38";

    public static string ETAGEquList = WebConfigurationManager.AppSettings["ETAGEquList"] != null
    ? WebConfigurationManager.AppSettings["ETAGEquList"] : "";

    public static string PCardPath = WebConfigurationManager.AppSettings["PCardPath"] != null
? WebConfigurationManager.AppSettings["PCardPath"] : "";

    public static string isShowCreatePic = WebConfigurationManager.AppSettings["isShowCreatePic"] != null
    ? WebConfigurationManager.AppSettings["isShowCreatePic"] : "1";

    public static string isUserFaceImg = WebConfigurationManager.AppSettings["isUserFaceImg"] != null
? WebConfigurationManager.AppSettings["isUserFaceImg"] : "0";

    public static string PsnPhotoUrl = WebConfigurationManager.AppSettings["PsnPhotoUrl"] != null
? WebConfigurationManager.AppSettings["PsnPhotoUrl"] : "Img";

    //-------------------------------------------------------------------------------------------------------------------------------------------
    // 設置主頁抬頭，並顯示程示執行項目名稱
    public static void SetProcName(Page oPage, string sProcName)
    {
        Label oProcName = (Label)oPage.Master.FindControl("labProcName");
        Label oTitle = (Label)oPage.Master.FindControl("lblTitle");
        if (sProcName == "")
        {
            oPage.Title = oTitle.Text;
            oProcName.Text = "請選擇執行項目";
        }
        else
        {
            oPage.Title = oTitle.Text + " - " + sProcName;
            oProcName.Text = sProcName;
        }
    }


    public static string JqueyNowVer
    {
        get
        {
            var page = System.Web.HttpContext.Current;
            return page.Request.ApplicationPath.TrimEnd('/') + @"/Scripts/jquery-3.6.0.js";
        }
    }



    #region 日期時間相關預設值

    public static string GetNowTime
    {
        get
        {
            return string.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now);
        }
    }

    public static string GetToday
    {
        get
        {
            return string.Format("{0:yyyy/MM/dd}", DateTime.Now);
        }
    }




    #endregion



    //-------------------------------------------------------------------------------------------------------------------------------------------
    public static string GetConnectionString(string sConnectionName)
    {
        //return WebConfigurationManager.ConnectionStrings[sConnectionName].ConnectionString;
        //update by Sam on 2016/01/06 
        string user_id = WebConfigurationManager.AppSettings["DbID"];
        string pwd = WebConfigurationManager.AppSettings["DbPW"];
        string source_ip = WebConfigurationManager.AppSettings["DbSource"];
        string data_name = ConfigurationManager.AppSettings["DbName"].ToString();
        string provider = ConfigurationManager.AppSettings["DbProvider"] != null ? ConfigurationManager.AppSettings["DbProvider"].ToString() : "SQLOLEDB.1";
        //WebConfigurationManager.AppSettings["DbName"];
        string connection_string = @"Provider={4};Persist Security Info=False;data source={0};initial catalog={1};
        user id={2};password='{3}';MultipleActiveResultSets=True;App=EntityFramework;Use Encryption for Data=True";
        if (ConfigurationManager.AppSettings["DbEncrypt"] != null && ConfigurationManager.AppSettings["DbEncrypt"].ToString() == "1")
        {
            connection_string += ";Use Encryption for Data=True";
        }
        else
        {
            connection_string += ";Use Encryption for Data=False";
        }
        return string.Format(connection_string, source_ip, data_name, user_id, pwd, provider);
    }

    public static string GetDapperConnString()
    {
        //return WebConfigurationManager.ConnectionStrings[sConnectionName].ConnectionString;
        //update by Sam on 2016/01/06 
        string user_id = WebConfigurationManager.AppSettings["DbID"];
        string pwd = WebConfigurationManager.AppSettings["DbPW"];
        string source_ip = WebConfigurationManager.AppSettings["DbSource"];
        string data_name = ConfigurationManager.AppSettings["DbName"].ToString();
        //WebConfigurationManager.AppSettings["DbName"];
        string connection_string = @"Addr={0};Database={1};uid={2};pwd='{3}'";
        if (ConfigurationManager.AppSettings["DbEncrypt"] != null && ConfigurationManager.AppSettings["DbEncrypt"].ToString() == "1")
        {
            connection_string += ";Encrypt=yes";
        }
        else
        {
            connection_string += ";Encrypt=no";
        }
        return string.Format(connection_string, source_ip, data_name, user_id, pwd);
    }


    public static string GetProviderNameString(string sConnectionName)
    {
        return WebConfigurationManager.ConnectionStrings[sConnectionName].ProviderName;
    }

    public static string GetPsnNo()
    {
        DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
        string PsnNo = "";
        PsnNo = oAcsDB.GetStrScalar("SELECT  ISNULL(MAX(RIGHT(PsnNo, 6)) + 1, '000001') FROM B01_Person WHERE ISNUMERIC(PsnNo) = 1");
        PsnNo = Pub.AddZero(PsnNo, 6);
        return PsnNo;
    }

    public static string AddZero(string paramDig, int paramDiff)
    {
        while (paramDig.Length < paramDiff)
        {
            paramDig = "0" + paramDig;
        }
        return paramDig;
    }

    public static string SetToNarrow(string strFrom)
    {
        return Microsoft.VisualBasic.Strings.StrConv(strFrom, Microsoft.VisualBasic.VbStrConv.Narrow, 0).Trim();
    }


    // strFun -- 選取要 SELECT 的欄位使用之
    // strStartTime 格式 -- "2015/09/21 00:00:00" 或 "2015/09/21"
    // strEndTime   格式 -- "2015/09/21 23:59:59" 或 "2015/09/21
    public static string ReturnNewNestedSerachSQL(string strFun, string strStartTime, string strEndTime)
    {
        #region 測試使用 連線資料庫
        //// -----------測試使用-----------
        //string user_id = "test1234";
        //string pwd = "1234";
        //string source_ip = "localhost\\sqlexpress";
        //string data_name = "SMS_AIDC1";

        //string strConn =
        //    @"Provider=SQLOLEDB.1;Persist Security Info=False;data source={0};initial catalog={1};user id={2};password={3};MultipleActiveResultSets=True;App=EntityFramework";

        //strConn = string.Format(strConn, source_ip, data_name, user_id, pwd);

        //DB_Acs oAcsDB = new DB_Acs(strConn);
        //// -----------測試使用-----------
        #endregion

        DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

        // 1. 先取得 B01_CardLog 的分資料表名稱，EX. B01_CardLog201603 ~ 
        string strSQL = "SELECT name FROM SYS.TABLES WHERE name <> 'B01_CardLog' and name LIKE 'B01_CardLog%'";

        List<string> liSqlPara = new List<string>();
        DataTable dt = new DataTable();
        oAcsDB.GetDataTable("", strSQL, liSqlPara, out dt);

        // 2.0 strStartTime 和 strEndTime 若傳入空值，先給個預設值
        if (strStartTime.ToString().Equals("")) strStartTime = "1950/01/01 00:00:00";
        if (strEndTime.ToString().Equals("")) strEndTime = "2050/12/31 23:59:59";

        // 2.1 strStartTime 和 strEndTime 若是10碼，ex. 2015/09/15，補齊時分秒
        if (strStartTime.ToString().Length == 10) strStartTime = strStartTime + " 00:00:00";
        if (strEndTime.ToString().Length == 10) strEndTime = strEndTime + " 23:59:59";

        // 宣告SQL相關的字串變數，並指派預設值
        string strNestedSerach = "";    // 產生的結果字串
        string strSelect = "";          // SELECT 的欄位
        string strFrom = "";            // FROM 的 TABLE名稱
        string strWhere = "";           // SQL 條件
        string strConditionStart = "";  // 開始時間
        string strConditionEnd = "";    // 結束時間

        switch (strFun)
        {
            case "0601":
            case "0622":
                strSelect = @" SELECT RecordID, CardTime, DepName, PsnNo, PsnName, CardNo,
                            CardVer, ReaderNo, LogTime, EquNo, DepID, LogStatus, TempCardNo  ";
                break;
            case "0601_LoadData":
            case "0622_LoadData":
                strSelect = @" SELECT RecordID, CardTime, LogTime, CardVer, LogStatus, CardNo,EquNo ";
                break;
            case "0602":
                strSelect = @" SELECT RecordID, CardTime, DepName, PsnNo, PsnName, CardNo,
                            CardVer, ReaderNo, LogTime, EquNo, DepID, LogStatus, CtrlNo, TempCardNo,
                            CardType, IsAndTrt, EquClass  ";
                break;
            case "0602_LoadData":
                strSelect = @" SELECT RecordID, CardTime, LogTime, CardVer, LogStatus, CardNo,
                            EquNo  ";
                break;
            case "0603":
                strSelect = @" SELECT RecordID, CardTime, CardNo, OrgStruc, PsnNo, PsnName, 
                            LogStatus, CardType, IsAndTrt, EquClass, CardVer  ";
                break;
            case "0604":
                strSelect = @" SELECT RecordID, CardTime, DepID, DepName, PsnNo, PsnName, 
                            LogTime, CardNo, CtrlNo, ReaderNo, LogStatus, EquNo, CardType, 
                            EquClass, CardVer  ";
                break;
            case "0604_LoadData":
                strSelect = @" SELECT RecordID, CardTime, LogTime, CardVer, LogStatus, CardNo,
                            EquNo  ";
                break;
            case "0621":
                strSelect = @" SELECT PsnName, PsnNo, OrgStruc, CardTime, LogStatus, EquName, 
                            EquDir,RecordId";
                break;
        }

        strFrom = " FROM B01_CardLog";
        strConditionStart = " CardTime >= '" + strStartTime + "' ";
        strConditionEnd = " CardTime <= '" + strEndTime + "' ";
        strWhere = " WHERE " + strConditionStart + " AND " + strConditionEnd;

        // 3. 讀取CardLog的MIN(CardTime)，如果 strEndTime 大於這個值，表示需要讀取CardLog
        liSqlPara = new List<string>();
        strSQL = "SELECT MIN(CardTime) FROM B01_CardLog";
        DateTime dtMinCardTime = oAcsDB.GetDateTimeScalar(strSQL, liSqlPara);
        DateTime dtEndTime = Convert.ToDateTime(strEndTime);    //將 strEndTime 轉成時間格式

        if (dtEndTime >= dtMinCardTime)
        {
            strNestedSerach = strSelect + strFrom + strWhere;
        }

        // 轉成6碼數字，EX. 201609 、 201610
        int intStartTime = Int32.Parse(Convert.ToDateTime(strStartTime.Substring(0, 10)).ToString("yyyyMM"));
        int intEndTime = Int32.Parse(Convert.ToDateTime(strEndTime.Substring(0, 10)).ToString("yyyyMM"));


        // 4. 開始組字串
        for (int intCurrentTime = intStartTime; intCurrentTime <= intEndTime; intCurrentTime++)
        {
            // 判斷資料表是否存在
            string strTN = "B01_CardLog" + Convert.ToString(intCurrentTime);

            DataRow[] dr = dt.Select("name='" + strTN + "'");
            if (dr.Length > 0)
            {
                strFrom = " FROM B01_CardLog" + Convert.ToString(intCurrentTime) + " ";


                strWhere = " WHERE " + strConditionStart + " AND " + strConditionEnd;

                // 判斷是否多個TABLE滿足，則需要使用UNION ALL
                if (strNestedSerach.ToString().Equals(""))
                {
                    strNestedSerach = strSelect + strFrom + strWhere;
                }
                else
                {
                    strNestedSerach += " UNION ALL " + strSelect + strFrom + strWhere;
                }
            }
        }
        strNestedSerach = "( " + strNestedSerach + " ) ";

        //Response.Write(strNestedSerach + "<BR />");
        return strNestedSerach;
    }

    public static string GetIDToAscCard(string PsnNo)
    {
        byte[] array = new byte[1];   //定義一組數組array
        array = System.Text.Encoding.ASCII.GetBytes(PsnNo.Substring(0, 1)); //string轉換的字母
        int asciicode = (short)(array[0]) - 64;

        return string.Format("{0:00}", asciicode) + PsnNo.Substring(1, 9);
    }


    public static string GetIDToCard(string PsnNo)
    {
        Dictionary<string, string> area_code = new Dictionary<string, string>();
        area_code.Add("A", "10");
        area_code.Add("B", "11");
        area_code.Add("C", "12");
        area_code.Add("D", "13");
        area_code.Add("E", "14");
        area_code.Add("F", "15");
        area_code.Add("G", "16");
        area_code.Add("H", "17");
        area_code.Add("I", "34");
        area_code.Add("J", "18");
        area_code.Add("K", "19");

        area_code.Add("M", "21");
        area_code.Add("N", "22");
        area_code.Add("O", "35");
        area_code.Add("P", "23");
        area_code.Add("Q", "24");
        area_code.Add("T", "27");
        area_code.Add("U", "28");
        area_code.Add("V", "29");
        area_code.Add("W", "32");
        area_code.Add("X", "30");
        area_code.Add("Z", "33");

        area_code.Add("S", "26");
        area_code.Add("Y", "31");
        area_code.Add("L", "20");
        area_code.Add("R", "25");
        return (area_code.ContainsKey(PsnNo.Substring(0, 1)) ? area_code[PsnNo.Substring(0, 1)] : PsnNo.Substring(0, 1)) + PsnNo.Substring(1, 9);
    }


    public static string GetCmdStr(string para_no)
    {
        XmlDocument xd = new XmlDocument();
        //string path = Request.PhysicalPath;
        var page = System.Web.HttpContext.Current;
        xd.Load(System.Web.HttpContext.Current.Server.MapPath("~/CmdStr.xml"));
        XElement doc = XElement.Parse(xd.OuterXml);
        if (doc.Element(para_no) != null)
        {
            return doc.Element(para_no).Value;
        }
        else
        {
            return "";
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------
    // 建立資料庫連線物件並進行連線，並回傳連線結果
    //public static Boolean GetConnect(out OleDbConnection oConn)
    //{
    //    return Sa.Dbu.GetOleDbConnect(DB_String(), out oConn);
    //}

    //-------------------------------------------------------------------------------------------------------------------------------------------
    // 建立資料庫連線物件並進行連線，並回傳連線結果，如果失敗則將網頁導入連線失敗頁面
    //public static Boolean GetConnect(Page oPage, out OleDbConnection oConn)
    //{
    //    Boolean IsOK = true;
    //    if (!xs.Dbu.GetOleDbConnect(DB_String(), out oConn))
    //    {
    //        oPage.Response.Redirect("~/Web/MessagePage/DBConnError.aspx");
    //        IsOK = false;
    //    }
    //    return IsOK;
    //}


    // 設定 Popup 獨占視窗相關參數
    public static void SetModalPopup(AjaxControlToolkit.ModalPopupExtender oModalPopup, Int32 nPopup)
    {
        string sPopup = nPopup.ToString();
        oModalPopup.TargetControlID = "PopupTrigger" + sPopup;
        oModalPopup.CancelControlID = "CancelTrigger" + sPopup;
        oModalPopup.PopupControlID = "PanelPopup" + sPopup;
        oModalPopup.PopupDragHandleControlID = "PanelDrag" + sPopup;
        oModalPopup.BackgroundCssClass = "modalbackground";
        oModalPopup.DropShadow = false;
    }

    public class MessageObject
    {
        public bool result = true;
        public string act;
        public string message;
        public int CurrentData = 0;
    }




    /// <summary>
    /// 取得RecordID值之ModifyBackupInfo_Style1
    /// </summary>
    /// <param name="TableName">Table名稱</param>
    /// <param name="RecordID">備份的ID值</param>
    /// <returns></returns>
    #region ModifyBackupInfo
    public static string ModifyBackupInfo(string TableName, string RecordID)
    {
        Table MainTable;
        TableRow MainTr;
        TableCell MainTd;

        #region Get ModifyBackupTable & FieldTable

        DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
        Pub.MessageObject objRet = new Pub.MessageObject();
        string sql = "";
        List<string> liSqlPara = new List<string>();
        DataTable ModifyBackupTable;
        DataTable FieldTable;

        #region Process-GetModifyBackupTable
        sql = @" SELECT 
                     ModifyBackup.BackupTime, 
                     ModifyBackup.BackupUserID+' : '+SysUser.UserName AS BackupUserID,
                     ModifyBackup.TableName,
                     CASE (ModifyBackup.ModifyMode)
                       WHEN 'M' THEN '編輯'
                       WHEN 'D' THEN '刪除' END AS ModifyMode,
                     ModifyBackup.RecordID,
                     ModifyBackup.FieldInfo,
                     ModifyBackup.DataInfo
                     FROM B00_ModifyBackup AS ModifyBackup
                     LEFT JOIN B00_SysUser AS SysUser ON SysUser.UserID = ModifyBackup.BackupUserID
                     WHERE ModifyBackup.TableName = ? AND ModifyBackup.RecordID = ? ";
        liSqlPara.Add("S:" + TableName.Trim());
        liSqlPara.Add("S:" + RecordID.Trim());
        oAcsDB.GetDataTable("ModifyBackupTable", sql, liSqlPara, out ModifyBackupTable);
        #endregion

        liSqlPara.Clear();

        #region Process-GetFieldTable
        sql = @" SELECT DISTINCT 
                     FieldNameList.FieldName , FieldNameList.ChtName 
                     FROM  B00_ModifyBackup AS ModifyBackup
                     LEFT JOIN B00_FieldNameList AS FieldNameList ON ModifyBackup.FieldInfo like '%' + FieldNameList.FieldName + '%'
	                       AND FieldNameList.TableName = ModifyBackup.TableName OR FieldNameList.tablename = 'Common'
                     WHERE ModifyBackup.TableName = ? AND ModifyBackup.RecordID = ? ";
        liSqlPara.Add("S:" + TableName.Trim());
        liSqlPara.Add("S:" + RecordID.Trim());
        oAcsDB.GetDataTable("FieldTable", sql, liSqlPara, out FieldTable);
        #endregion

        #endregion

        MainTable = new Table();

        #region Header
        MainTr = new TableRow();

        #region Fixed-Header
        MainTd = new TableCell();
        MainTd.Text = "異動時間";
        MainTd.Width = 150;
        MainTd.Attributes.Add("nowrap", "norwap");
        MainTd.Style.Add("background-color", "#016B0A");
        MainTd.Style.Add("color", "#FBFBFB");
        MainTd.Style.Add("padding", "3px");
        MainTr.Controls.Add(MainTd);

        MainTd = new TableCell();
        MainTd.Text = "異動者帳號";
        MainTd.Width = 150;
        MainTd.Attributes.Add("nowrap", "norwap");
        MainTd.Style.Add("background-color", "#016B0A");
        MainTd.Style.Add("color", "#FBFBFB");
        MainTd.Style.Add("padding", "3px");
        MainTr.Controls.Add(MainTd);


        MainTd = new TableCell();
        MainTd.Text = "異動模式";
        MainTd.Attributes.Add("nowrap", "norwap");
        MainTd.Style.Add("background-color", "#016B0A");
        MainTd.Style.Add("color", "#FBFBFB");
        MainTd.Style.Add("padding", "3px");
        MainTr.Controls.Add(MainTd);
        #endregion

        #region GetTotleField
        List<string> FieldList = new List<string>();
        foreach (DataRow ModifyBackupRow in ModifyBackupTable.Rows)
        {
            string[] FieldSplit;
            FieldSplit = ModifyBackupRow["FieldInfo"].ToString().Split(new string[] { "|;" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < FieldSplit.Length; i++)
            {
                if (!FieldList.Contains(FieldSplit[i].ToString()))
                    FieldList.Add(FieldSplit[i].ToString());
            }
        }
        #endregion

        #region Flexible-Header

        for (int i = 0; i < FieldList.Count; i++)
        {
            MainTd = new TableCell();

            foreach (DataRow FieldRow in FieldTable.Rows)
            {
                if (string.Compare(FieldRow["FieldName"].ToString(), FieldList[i].ToString(), false) == 0)
                {
                    MainTd.Text = FieldRow["ChtName"].ToString();
                    break;
                }
            }
            MainTd.Attributes.Add("nowrap", "norwap");
            MainTd.Style.Add("background-color", "#016B0A");
            MainTd.Style.Add("color", "#FBFBFB");
            MainTd.Style.Add("padding", "3px");
            MainTr.Controls.Add(MainTd);
        }

        #endregion
        MainTable.Controls.Add(MainTr);
        #endregion

        if (ModifyBackupTable.Rows.Count > 0)
        {
            #region DataRow
            foreach (DataRow ModifyBackupRow in ModifyBackupTable.Rows)
            {
                MainTr = new TableRow();
                #region Fixed-Data
                MainTd = new TableCell();
                MainTd.Text = DateTime.Parse(ModifyBackupRow["BackupTime"].ToString()).ToString("yyyy/MM/dd hh:mm:ss");
                MainTd.Attributes.Add("nowrap", "norwap");
                MainTd.Style.Add("background-color", "#F5FFF5");
                MainTd.Style.Add("border-right", "1px solid black");
                MainTd.Style.Add("border-top", "1px solid black");
                MainTd.Style.Add("padding", "3px");
                MainTr.Controls.Add(MainTd);

                MainTd = new TableCell();
                MainTd.Text = ModifyBackupRow["BackupUserID"].ToString();
                MainTd.Attributes.Add("nowrap", "norwap");
                MainTd.Style.Add("background-color", "#F5FFF5");
                MainTd.Style.Add("border-right", "1px solid black");
                MainTd.Style.Add("border-top", "1px solid black");
                MainTd.Style.Add("padding", "3px");
                MainTr.Controls.Add(MainTd);


                MainTd = new TableCell();
                MainTd.Text = ModifyBackupRow["ModifyMode"].ToString();
                MainTd.Attributes.Add("nowrap", "norwap");
                MainTd.Style.Add("background-color", "#F5FFF5");
                MainTd.Style.Add("border-top", "1px solid black");
                MainTd.Style.Add("border-right", "1px solid black");
                MainTd.Style.Add("padding", "3px");
                MainTr.Controls.Add(MainTd);
                #endregion

                #region Flexible-Data

                string[] FieldInfoSplit;
                string[] DataInfoSplit;

                FieldInfoSplit = ModifyBackupRow["FieldInfo"].ToString().Split(new string[] { "|;" }, StringSplitOptions.None);
                DataInfoSplit = ModifyBackupRow["DataInfo"].ToString().Split(new string[] { "|;" }, StringSplitOptions.None);

                for (int i = 0; i < FieldList.Count; i++)
                {
                    MainTd = new TableCell();

                    for (int x = 0; x < FieldInfoSplit.Length; x++)
                    {
                        if (string.Compare(FieldInfoSplit[x].ToString(), FieldList[i].ToString(), false) == 0)
                        {
                            MainTd.Text = DataInfoSplit[x].ToString();
                            break;
                        }
                    }
                    MainTd.Attributes.Add("nowrap", "norwap");
                    MainTd.Style.Add("border-top", "1px solid black");
                    if (i != FieldList.Count - 1)
                        MainTd.Style.Add("border-right", "1px solid black");
                    MainTd.Style.Add("padding", "3px");
                    MainTr.Controls.Add(MainTd);
                }

                #endregion
                MainTable.Controls.Add(MainTr);
            }
            #endregion
        }
        else
        {
            MainTr = new TableRow();

            MainTd = new TableCell();
            MainTd.Text = "尚無備份資料";
            MainTd.ColumnSpan = 3;
            MainTd.Attributes.Add("nowrap", "norwap");
            MainTd.Style.Add("background-color", "#F5FFF5");
            MainTd.Style.Add("border-top", "1px solid black");
            MainTd.Style.Add("padding", "3px");
            MainTr.Controls.Add(MainTd);

            MainTable.Controls.Add(MainTr);
        }

        #region MainTable Style
        MainTable.Style.Add("word-break", "break-all");
        MainTable.Style.Add("border", "1px solid black");
        MainTable.Attributes.Add("cellpadding", "0");
        MainTable.Attributes.Add("cellspacing", "0");
        #endregion

        #region 讀取Main_RenderControl
        StringWriter StringWriter = new StringWriter();
        HtmlTextWriter HtmlWriter = new HtmlTextWriter(StringWriter);
        MainTable.RenderControl(HtmlWriter);
        #endregion

        return StringWriter.ToString();
    }
    #endregion

    public class XlsxComparer : IEqualityComparer<string[]>
    {
        public bool Equals(string[] Array1, string[] Array2)
        {
            bool AllCheckFlag = true;
            bool SingleCheckFlag = false;
            for (int i = 0; i < Array1.Length; i++)
            {
                if (string.Compare(Array1[i].ToString(), Array2[i].ToString()) == 0)
                {
                    SingleCheckFlag = true;
                }
                else
                {
                    SingleCheckFlag = false;
                    break;
                }
            }
            if (!SingleCheckFlag)
                AllCheckFlag = false;
            else
                AllCheckFlag = true;
            return AllCheckFlag;
        }

        public int GetHashCode(string[] obj)
        {
            return GetHashCode();
        }



    }//end class
}//end namespace
