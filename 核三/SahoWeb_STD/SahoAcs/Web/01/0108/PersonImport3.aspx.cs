using OfficeOpenXml;
using Sa.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs
{
    public partial class PersonImport3 : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        static bool DciCorrectFlag = false, MstCorrectFlag = false, CtrlCorrectFlag = false, ReaderCorrectFlag = false, EquGroupCorrectFlag = false, OrgCorrectFlag = false, OrgStrucCorrectFlag = false, PsnCardCorrectFlag = false;
        static int DciTableCount = 0, MstTableCount = 0, CtrlTableCount = 0, ReaderTableCount = 0, EquGroupTableCount = 0, OrgTableCount = 0, OrgStrucTableCount = 0, PsnTableCount = 0;
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;

            oScriptManager.RegisterAsyncPostBackControl(CleanTable_PsnCard);
            oScriptManager.RegisterAsyncPostBackControl(PsnCard_ImportButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("PersonImport", "PersonImport3.js");//加入同一頁面所需的JavaScript檔案

            #region 註冊主頁Button動作
            //SaveButton.Attributes["onClick"] = "SaveExcute();return false;";
            #endregion

            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion

                CheckPsnCardSyncTable();
            }
            else
            {
                #region 非初次進入網頁
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];
                string[] ErrorNo;
                string ErrorNoStr = "";
                if (sFormTarget == this.EquMsg_UpdatePanel.ClientID)
                {
                    #region 更新訊息視窗部份
                    ErrorNo = hideErrorData.Value.Split(';');
                    for (int i = 0; i < ErrorNo.Length; i++)
                    {
                        //if (!string.IsNullOrEmpty(ErrorNoStr)) ErrorNoStr += ";";
                        ErrorNoStr += ErrorNo[i].ToString();
                    }
                    //switch (sFormArg)
                    //{
                    //    case "ShowDciError":
                    //        ErrorNoStr = "連線資料關聯有誤編號：\n" + ErrorNoStr + "\n";
                    //        break;
                    //    case "ShowMstError":
                    //        ErrorNoStr = "連線裝置資料關聯有誤編號：\n" + ErrorNoStr + "\n";
                    //        break;
                    //    case "ShowCtrlError":
                    //        ErrorNoStr = "控制器資料關聯有誤編號：\n" + ErrorNoStr + "\n";
                    //        break;
                    //    case "ShowReaderError":
                    //        ErrorNoStr = "讀卡機資料關聯有誤編號：\n" + ErrorNoStr + "\n";
                    //        break;
                    //    case "ShowEquGroupError":
                    //        ErrorNoStr = "設備群組關聯有誤編號：\n" + ErrorNoStr + "\n";
                    //        break;
                    //}
                    TextBox_EquMsg.Text += ErrorNoStr;
                    #endregion
                }                
                else if (sFormArg == "PsnCard_UpdateState")
                {
                    CheckPsnCardSyncTable();
                }

                #endregion
            }
        }
        #endregion

        #region 主要事件分區

        #region 人員資料
        #region 匯入事件

        #region UpLoadButton_PsnCard_Click
        protected void UpLoadButton_PsnCard_Click(object sender, EventArgs e)
        {
            bool IsSuccess = false;
            string sSql = "", sParaStr = "";
            int SQLComResult = 0, linecount = 0;
            DateTime ToDay = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            HashSet<string[]> PsnCard_set = new HashSet<string[]>();
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            if (FileUpload_PsnCard.HasFile && System.IO.Path.GetExtension(FileUpload_PsnCard.PostedFile.FileName) == ".xlsx")
                IsSuccess = SetHastSetValueFormXlsx(FileUpload_PsnCard, "人員編號", out PsnCard_set);

            if (!IsSuccess)
            {
                Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('未選擇檔案或格式不符！');");
            }

            #region 暫存資料庫清空
            if (IsSuccess)
            {
                sSql = ""; sParaStr = "";
                liSqlPara.Clear();
                sSql = " DELETE B00_SyncTable_PersonCard ";
                SQLComResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
                if (SQLComResult == -1)
                    IsSuccess = false;
            }
            #endregion

            #region 暫存資料庫寫入
            sSql = ""; sParaStr = "";
            liSqlPara.Clear();
            if (IsSuccess)
            {
                for (int i = 0; i < PsnCard_set.Count; i++)
                {
                    sParaStr = "?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?";
                    sSql += @" INSERT dbo.B00_SyncTable_PersonCard
                               (PsnNo, PsnName, PsnEName, PsnType, IDNum, Birthday, Company, Unit, Department, Title, PsnAccount, PsnPW, PsnAuthAllow, PsnSTime, PsnETime, PsnPicSource, CardNo, CardVer, CardPW, CardSerialNo, Text1, Remark)
                               VALUES (" + sParaStr + ") ";

                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[0].ToString());       //PsnNo
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[1].ToString());       //PsnName
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[2].ToString());       //PsnEName
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[3].ToString());       //PsnType
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[4].ToString());       //IDNum
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[5].ToString());       //Birthday
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[6].ToString());       //Company
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[7].ToString());       //Unit
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[8].ToString());       //Department
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[9].ToString());       //Title
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[10].ToString());      //PsnAccount
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[11].ToString());      //PsnPW
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[12].ToString());      //PsnAccount
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[13].ToString());      //PsnSTime
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[14].ToString());      //PsnETime
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[15].ToString());      //PsnPicSource
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[16].ToString());      //CardNo
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[17].ToString());      //CardVer
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[18].ToString());      //CardPW
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[19].ToString());      //CardSerialNo
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[20].ToString());      //Text1
                    liSqlPara.Add("S:" + ((string[])PsnCard_set.ElementAt(i))[21].ToString());      //Text2
                    linecount++;

                    if (linecount % 50 == 0)
                    {
                        SQLComResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
                        sSql = ""; sParaStr = "";
                        liSqlPara.Clear();
                        if (SQLComResult == -1)
                        {
                            IsSuccess = false;
                            break;
                        }
                    }
                }
                if (SQLComResult != -1)
                    SQLComResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
                if (SQLComResult == -1)
                    IsSuccess = false;
            }
            #endregion

            #region 資料庫安全性檢查
            if (IsSuccess)
            {
                IsSuccess = CheckPsnCardCorrect(out PsnTableCount);
            }
            #endregion

            Sa.Web.Fun.RunJavaScript(this, "__doPostBack('" + PsnCardState_UpdatePanel.ClientID + "', 'PsnCard_UpdateState');");
        }
        #endregion

        #region UpLoadButton_Card_Click Mark
        //        protected void UpLoadButton_Card_Click(object sender, EventArgs e)
        //        {
        //            bool IsSuccess = false;
        //            string sSql = "", sParaStr = "";
        //            int SQLComResult = 0, linecount = 0;
        //            DateTime ToDay = DateTime.Now;
        //            List<string> liSqlPara = new List<string>();
        //            HashSet<string[]> Card_set = new HashSet<string[]>();
        //            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

        //            if (FileUpload_Card.HasFile && System.IO.Path.GetExtension(FileUpload_Card.PostedFile.FileName) == ".xlsx")
        //                IsSuccess = SetHastSetValueFormXlsx(FileUpload_Card, "CardNo", out Card_set);

        //            if (!IsSuccess)
        //            {
        //                Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('未選擇檔案或格式不符！');");
        //            }

        //            #region 暫存資料庫清空
        //            if (IsSuccess)
        //            {
        //                sSql = ""; sParaStr = "";
        //                liSqlPara.Clear();
        //                sSql = " DELETE B00_SyncTable_Card ";
        //                SQLComResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
        //                if (SQLComResult == -1)
        //                    IsSuccess = false;
        //            }
        //            #endregion

        //            #region 暫存資料庫寫入
        //            sSql = ""; sParaStr = "";
        //            liSqlPara.Clear();
        //            if (IsSuccess)
        //            {
        //                for (int i = 0; i < Card_set.Count; i++)
        //                {
        //                    sParaStr = "?, ?, ?, ?, ?, ?, ?, ?, ?, ?";
        //                    sSql += @" INSERT INTO B00_SyncTable_Card(CardNo, CardVer, CardPW, CardSerialNo, CardNum, CardType, CardAuthAllow, CardSTime, CardETime, CardDesc)
        //                               VALUES(" + sParaStr + ") ";

        //                    liSqlPara.Add("S:" + ((string[])Card_set.ElementAt(i))[0].ToString());    //CardNo
        //                    liSqlPara.Add("S:" + ((string[])Card_set.ElementAt(i))[1].ToString());    //CardVer
        //                    liSqlPara.Add("S:" + ((string[])Card_set.ElementAt(i))[2].ToString());    //CardPW
        //                    liSqlPara.Add("S:" + ((string[])Card_set.ElementAt(i))[3].ToString());    //CardSerialNo
        //                    liSqlPara.Add("S:" + ((string[])Card_set.ElementAt(i))[4].ToString());    //CardNum
        //                    liSqlPara.Add("S:" + ((string[])Card_set.ElementAt(i))[5].ToString());    //CardType
        //                    liSqlPara.Add("S:" + ((string[])Card_set.ElementAt(i))[6].ToString());    //CardAuthAllow
        //                    liSqlPara.Add("S:" + ((string[])Card_set.ElementAt(i))[7].ToString());    //CardSTime
        //                    liSqlPara.Add("S:" + ((string[])Card_set.ElementAt(i))[8].ToString());    //CardETime
        //                    liSqlPara.Add("S:" + ((string[])Card_set.ElementAt(i))[9].ToString());    //CardDesc

        //                    linecount++;

        //                    if (linecount % 100 == 0)
        //                    {
        //                        SQLComResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
        //                        sSql = ""; sParaStr = "";
        //                        liSqlPara.Clear();
        //                        if (SQLComResult == -1)
        //                        {
        //                            IsSuccess = false;
        //                            break;
        //                        }
        //                    }
        //                }
        //                if (SQLComResult != -1)
        //                    SQLComResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
        //                if (SQLComResult == -1)
        //                    IsSuccess = false;
        //            }
        //            #endregion

        //            #region 資料庫安全性檢查
        //            if (IsSuccess)
        //            {
        //                IsSuccess = CheckCardCorrect(out CardTableCount);
        //            }
        //            #endregion

        //            Sa.Web.Fun.RunJavaScript(this, "__doPostBack('" + ReaderState_UpdatePanel.ClientID + "', 'PsnCard_UpdateState');");
        //        }
        #endregion

        #region PsnCard_ImportButton_Click
        protected void PsnCard_ImportButton_Click(object sender, EventArgs e)
        {
            string sSql = "", MsgStr = "", sCardNo = "";
            DataTable SyncTable_CardTable;
            List<string> liSqlPara = new List<string>();
            int iRet = 0;

            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string sIPAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            oAcsDB.BeginTransaction();

            if (PsnCardCorrectFlag && PsnTableCount > 0)
            {
                #region 新增Psn資料
                if (iRet > -1)
                {
                    sSql = @" INSERT INTO B01_Person(PsnNo, PsnName, PsnEName, PsnType, IDNum, Birthday, 
                                OrgStrucID, PsnAccount, PsnPW, PsnAuthAllow, PsnSTime, PsnETime, PsnPicSource, CreateUserID, CreateTime, Text1, Remark)
                              SELECT 
                              PersonCard.PsnNo, PersonCard.PsnName, PersonCard.PsnEName, PersonCard.PsnType, 
                              PersonCard.IDNum, PersonCard.Birthday, OrgStrucAll.OrgStrucID,
                              PsnAccount, PsnPW, PsnAuthAllow, 
                              PsnSTime, PsnETime, PsnPicSource, 
                              'Saho', GETDATE(),Text1,Remark
                              FROM dbo.B00_SyncTable_PersonCard AS PersonCard
                              LEFT JOIN OrgStrucAllData('') AS OrgStrucAll 
                              ON OrgStrucAll.OrgNameList = ( '\' +
	                              CASE Company WHEN NULL THEN '' WHEN '' THEN '' ELSE PersonCard.Company + '\' END +
	                              CASE Unit WHEN NULL THEN '' WHEN '' THEN '' ELSE PersonCard.Unit + '\' END +
	                              CASE Department WHEN NULL THEN '' WHEN '' THEN '' ELSE PersonCard.Department + '\' END +
	                              CASE Title WHEN NULL THEN '' WHEN '' THEN '' ELSE PersonCard.Title + '\' END   
                              ) ";
                    iRet = oAcsDB.SqlCommandExecute(sSql);
                }
                #endregion

                if (iRet > -1)
                {
                    MsgStr += "人員基本資料同步完成,處理共" + iRet + "筆。\n";
                }

                #region 新增Card資料
                if (iRet > -1)
                {
                    sSql = @" INSERT INTO dbo.B01_Card (CardNo, CardVer, CardPW, CardSerialNo, CardType, PsnID, CardAuthAllow, CardSTime, CardETime, CreateUserID, CreateTime)
                              SELECT 
                              PersonCard.CardNo, PersonCard.CardVer, PersonCard.CardPW, PersonCard.CardSerialNo,
                              (SELECT TOP 1 ItemNo FROM B00_ItemList WHERE ItemClass='CardType' ORDER BY ItemOrder), 
                               Person.PsnID, PersonCard.PsnAuthAllow,
                              PersonCard.PsnSTime, PersonCard.PsnETime,
                              'Saho', GETDATE()
                              FROM B00_SyncTable_PersonCard AS PersonCard
                              LEFT JOIN B01_Person AS Person ON Person.PsnNo = PersonCard.PsnNo ";

                    iRet = oAcsDB.SqlCommandExecute(sSql);
                }
                #endregion

                if (iRet > -1)
                {
                    MsgStr += "卡片基本資料同步完成,處理共" + iRet + "筆。\n";
                }

                #region 新增CardEquGroup資料(由OrgEquGroup取得) 預設權限
                if (iRet > -1)
                {
                    sSql = @" INSERT INTO B01_CardEquGroup(CardID, EquGrpID, CreateUserID)
                              SELECT Card.CardID, OrgEquGroup.EquGrpID,
                              'Saho'
                              FROM B00_SyncTable_PersonCard AS PersonCard
                              INNER JOIN dbo.B01_Person AS Person ON Person.PsnNo = PersonCard.PsnNo
                              LEFT JOIN dbo.B01_Card AS Card ON Card.PsnID = Person.PsnID
                              LEFT JOIN B01_OrgEquGroup AS OrgEquGroup ON OrgEquGroup.OrgStrucID = Person.OrgStrucID
                              WHERE OrgEquGroup.EquGrpID IS NOT NULL ";

                    iRet = oAcsDB.SqlCommandExecute(sSql);
                }
                #endregion

                #region 新增EquGroup資料
                //                if (iRet > -1)
                //                {
                //                    sSql = @" INSERT INTO B01_CardEquGroup (CardID, EquGrpID, CreateUserID, CreateTime)
                //                              SELECT Card.CardID, EquGroup.EquGrpID,
                //                              'SAHO',GETDATE()
                //                              FROM B00_SyncTable_Person AS SyncTable_Person
                //                              INNER JOIN B01_Card AS Card ON Card.CardNum = SyncTable_Person.PsnNo
                //                              INNER JOIN B01_EquGroup 00derekAS EquGroup ON EquGroup.EquGrpNo = SyncTable_Person.EquGrpNo ";

                //                    iRet = oAcsDB.SqlCommandExecute(sSql);
                //                }
                #endregion

                #region 權限重整
                if (iRet > -1)
                {
                    sSql = " SELECT CardNo FROM B00_SyncTable_PersonCard ";
                    oAcsDB.GetDataTable("CardTable", sSql, out SyncTable_CardTable);

                    sSql = "";
                    for (int i = 0; i < SyncTable_CardTable.Rows.Count; i++)
                    {
                        sCardNo = SyncTable_CardTable.Rows[i]["CardNo"].ToString();
                        sSql += " EXEC dbo.CardAuth_Update @sCardNo = '" + sCardNo + "',@sUserID = 'SAHO',@sFromProc = 'ImportData',@sFromIP = '" + sIPAddress + "',@sOpDesc = '資料匯入預設權限' ; ";
                    }

                    iRet = oAcsDB.SqlCommandExecute(sSql);
                }
                #endregion

                if (iRet > -1)
                {
                    MsgStr += "卡片群組資料同步完成,處理共" + iRet + "筆。\n";
                }

                #region 清空暫存資料表
                if (iRet > -1)
                {
                    sSql = @" DELETE FROM B00_SyncTable_PersonCard ";
                    iRet = oAcsDB.SqlCommandExecute(sSql);
                }
                #endregion

                if (iRet > -1)
                {
                    MsgStr += "資料同步完成,已清空暫存資料表。\n";
                }

                if (iRet > -1)
                {
                    oAcsDB.Commit();
                    TextBox_EquMsg.Text += MsgStr;
                    EquMsg_UpdatePanel.Update();
                    CheckPsnCardSyncTable();
                }
                else
                {
                    oAcsDB.Rollback();
                    MsgStr = "同步發生錯誤,同步中止。";
                    TextBox_EquMsg.Text += MsgStr;
                    EquMsg_UpdatePanel.Update();
                }
            }
        }
        #endregion

        #endregion

        #region  清除資料表事件

        #region CleanTable_PsnCard_Click
        protected void CleanTable_PsnCard_Click(object sender, EventArgs e)
        {
            string sSql = "";
            int SQLComResult = 0;
            List<string> liSqlPara = new List<string>();

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            #region 暫存資料庫清空
            sSql = " DELETE B00_SyncTable_PersonCard ";
            SQLComResult = oAcsDB.SqlCommandExecute(sSql);
            #endregion

            Sa.Web.Fun.RunJavaScript(this, "__doPostBack('" + CleanTable_PsnCard.ClientID + "', 'PsnCard_UpdateState')");
        }
        #endregion

        #region CleanTable_Card_Click Mark
        //protected void CleanTable_Card_Click(object sender, EventArgs e)
        //{
        //    string sSql = "";
        //    int SQLComResult = 0;
        //    List<string> liSqlPara = new List<string>();

        //    DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

        //    #region 暫存資料庫清空
        //    sSql = " DELETE B00_SyncTable_Card ";
        //    SQLComResult = oAcsDB.SqlCommandExecute(sSql);
        //    #endregion

        //    Sa.Web.Fun.RunJavaScript(this, "__doPostBack('" + CleanTable_Reader.ClientID + "', 'PsnCard_UpdateState')");
        //}
        #endregion

        #endregion
        #endregion

        #endregion

        #endregion

        #region Method

        #region 將資料讀取至HashSet中
        private bool SetHastSetValueFormCSV(FileUpload Obj_FileUpLoad, string Checkfield, out HashSet<string[]> OutSet)
        {
            bool IsSuccess = false;

            HashSet<string[]> Process_set = new HashSet<string[]>();
            if (Obj_FileUpLoad.HasFile && System.IO.Path.GetExtension(Obj_FileUpLoad.PostedFile.FileName) == ".xlsx")
            {
                try
                {
                    using (StreamReader reader = new StreamReader(Obj_FileUpLoad.PostedFile.InputStream, System.Text.Encoding.Default))
                    {
                        string content = "";
                        string[] data = null;
                        int ReaderCount = 0;
                        while ((content = reader.ReadLine()) != null)
                        {
                            if (ReaderCount == 0)
                            {
                                data = content.Split(',');
                                if (string.Compare(data[0].ToString(), Checkfield) != 0)
                                    throw new Exception();
                            }
                            else if (ReaderCount >= 2)
                            {
                                data = content.Split(',');
                                if (!Process_set.Contains(data, new Pub.XlsxComparer()))
                                {
                                    Process_set.Add(data);
                                }
                            }
                            ReaderCount++;
                        }
                        IsSuccess = true;
                    }
                }
                catch
                {
                    IsSuccess = false;
                }
            }
            OutSet = Process_set;
            return IsSuccess;
        }

        private bool SetHastSetValueFormXlsx(FileUpload Obj_FileUpLoad, string Checkfield, out HashSet<string[]> OutSet)
        {
            bool IsSuccess = false;

            HashSet<string[]> Process_set = new HashSet<string[]>();
            //string[] data = null;
            List<string> data = new List<string>();

            string FilePath = System.IO.Path.GetFullPath(Obj_FileUpLoad.PostedFile.FileName);

            if (Obj_FileUpLoad.HasFile && System.IO.Path.GetExtension(Obj_FileUpLoad.PostedFile.FileName) == ".xlsx")
            {
                try
                {
                    //開檔
                    using (Stream fs = Obj_FileUpLoad.FileContent)// new FileStream(Obj_FileUpLoad.PostedFile.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        //載入Excel檔案
                        using (ExcelPackage ep = new ExcelPackage(fs))
                        {
                            ExcelWorksheet sheet = ep.Workbook.Worksheets[1];       //取得Sheet1
                            int startRowNumber = sheet.Dimension.Start.Row;         //起始列編號，從1算起
                            int endRowNumber = sheet.Dimension.End.Row;             //結束列編號，從1算起
                            int startColumn = sheet.Dimension.Start.Column;         //開始欄編號，從1算起
                            int endColumn = sheet.Dimension.End.Column;             //結束欄編號，從1算起

                            bool isHeader = true;
                            if (isHeader)
                            {
                                startRowNumber += 2;
                            }

                            if (string.Compare(sheet.Cells[1, 1].Text, Checkfield) != 0)
                                throw new Exception();

                            for (int currentRow = startRowNumber; currentRow <= endRowNumber; currentRow++)
                            {
                                ExcelRange range = sheet.Cells[currentRow, startColumn, currentRow, endColumn];//抓出目前的Excel列
                                if (range.Any(c => !string.IsNullOrEmpty(c.Text)) == false)//這是一個完全空白列(使用者用Delete鍵刪除動作)
                                {
                                    continue;//略過此列
                                }
                                //讀值
                                for (int currentCell = startColumn; currentCell <= endColumn; currentCell++)
                                    data.Add(sheet.Cells[currentRow, currentCell].Text);

                                if (!Process_set.Contains(data.ToArray(), new Pub.XlsxComparer()))
                                {
                                    Process_set.Add(data.ToArray());
                                }
                                data.Clear();
                            }
                        }//end   using
                        IsSuccess = true;
                    }//end using
                }
                catch (Exception ex)
                {
                    IsSuccess = false;
                }
            }
            OutSet = Process_set;
            return IsSuccess;
        }
        #endregion

        #region 正確性檢查區塊

        #region 人員資料

        #region 確認PsnCard暫存資料表的正確性
        private bool CheckPsnCardCorrect(out int ReturnRowCount)
        {
            bool IsSuccess = true;
            string sSql = "";
            int ErrorCount = 0, RowCount = 0;

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            #region 取得暫存資料表資料筆數
            sSql = "SELECT COUNT(*) FROM B00_SyncTable_PersonCard";
            RowCount = oAcsDB.GetIntScalar(sSql);
            #endregion

            #region 檢查必填欄位是否填寫
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                          WHERE PsnNo = '' OR PsnType = '' OR Company = '' OR PsnAuthAllow = '' OR PsnSTime = '' OR CardNo = '' OR CardPW = '' ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region 檢查必填欄位正確性

            #region PsnType
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                          WHERE PsnType NOT IN ( SELECT ItemNo FROM dbo.B00_ItemList WHERE ItemClass = 'PsnType' ) ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region CardNo length check
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                                    WHERE LEN(CardNo) !=ISNULL((SELECT TOP 1 CONVERT(int,ParaValue) FROM B00_SysParameter WHERE ParaNo='CardLen'),10) ";
                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion


            #region PsnAuthAllow
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                          WHERE PsnAuthAllow NOT IN ('0','1') ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #endregion

            #region 檢查Person是否已存在系統中
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                          INNER JOIN B01_Person AS Person ON Person.PsnNo = SyncTable_PersonCard.PsnNo ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region 檢查Card是否已存在系統中
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                          INNER JOIN B01_Card AS Card ON Card.CardNo = SyncTable_PersonCard.CardNo ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region 確認PsnCard中的組織架構是否確實存在
            if (ErrorCount == 0)
            {
                sSql = @" SELECT 
                          COUNT(*)
                          FROM dbo.B00_SyncTable_PersonCard
                          LEFT JOIN OrgStrucAllData('') AS StrucAllData ON StrucAllData.OrgIDList = (
                              ISNULL ( '\' + ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Company) + '\','') +
                              ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Unit) + '\','') +
                              ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Department) + '\','') +
                              ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Title) + '\','')
                          )
                          WHERE StrucAllData.OrgStrucID IS NULL ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            if (ErrorCount != 0)
            {
                IsSuccess = false;
            }

            ReturnRowCount = RowCount;
            return IsSuccess;
        }
        #endregion

        #region 顯示PsnCard錯誤資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object ShowPsnCardErrorData()
        {
            string sSql = "";
            int ErrorCount = 0;
            DataTable ErrorPsnCardData = null;
            Pub.MessageObject objRet = new Pub.MessageObject();
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

            #region 檢查必填欄位是否填寫
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                          WHERE PsnNo = '' OR PsnType = '' OR Company = '' OR PsnAuthAllow = '' OR PsnSTime = '' OR CardNo = '' OR CardPW = '' ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                              WHERE PsnNo = '' OR PsnType = '' OR Company = '' OR PsnAuthAllow = '' OR PsnSTime = '' OR CardNo = '' OR CardPW = '' ";
                    oAcsDB.GetDataTable("ErrorPsnCardData", sSql, out ErrorPsnCardData);
                    objRet.message += "必填欄位請填寫：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region 檢查必填欄位正確性

            #region PsnType
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                          WHERE PsnType NOT IN ( SELECT ItemNo FROM dbo.B00_ItemList WHERE ItemClass = 'PsnType' ) ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                              WHERE PsnType NOT IN ( SELECT ItemNo FROM dbo.B00_ItemList WHERE ItemClass = 'PsnType' ) ";
                    oAcsDB.GetDataTable("ErrorPsnCardData", sSql, out ErrorPsnCardData);
                    objRet.message += "必填欄位有誤(不存在的人員分類)：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region CardNo length check

            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                                    WHERE LEN(CardNo) !=ISNULL((SELECT TOP 1 CONVERT(int,ParaValue) FROM B00_SysParameter WHERE ParaNo='CardLen'),10) ";
                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @"SELECT * FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                                    WHERE LEN(CardNo) !=ISNULL((SELECT TOP 1 CONVERT(int,ParaValue) FROM B00_SysParameter WHERE ParaNo='CardLen'),10) ";
                    oAcsDB.GetDataTable("ErrorPsnCardData", sSql, out ErrorPsnCardData);
                    objRet.message += "卡號欄位長度錯誤:\n";
                    foreach(DataRow o in ErrorPsnCardData.Rows)
                    {
                        objRet.message += @"人員編號：" + o["PsnNo"].ToString() + "\n"
                                   + @"人員名稱：" + o["PsnName"].ToString() + "\n"
                                   + @"卡號：" + o["CardNo"].ToString() + "\n";
                    }
                    objRet.message += "-----====================================================================-----\n";
                }
            }

            #endregion


            #region PsnAuthAllow
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                          WHERE PsnAuthAllow NOT IN ('0','1') ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                              WHERE PsnAuthAllow NOT IN ('0','1') ";
                    oAcsDB.GetDataTable("ErrorPsnCardData", sSql, out ErrorPsnCardData);
                    objRet.message += "必填欄位有誤(人員資料是否啟用欄位)：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #endregion

            #region 檢查資料是否已存在系統中

            #region Person
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                          INNER JOIN B01_Person AS Person ON Person.PsnNo = SyncTable_PersonCard.PsnNo ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                              INNER JOIN B01_Person AS Person ON Person.PsnNo = SyncTable_PersonCard.PsnNo ";
                    oAcsDB.GetDataTable("ErrorPsnCardData", sSql, out ErrorPsnCardData);
                    objRet.message += "資料已存在於系統中：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region Card
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                          INNER JOIN B01_Card AS Card ON Card.CardNo = SyncTable_PersonCard.CardNo ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                              INNER JOIN B01_Card AS Card ON Card.CardNo = SyncTable_PersonCard.CardNo ";
                    oAcsDB.GetDataTable("ErrorPsnCardData", sSql, out ErrorPsnCardData);
                    objRet.message += "資料已存在於系統中：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }

            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                          INNER JOIN B01_Card AS Card ON Card.CardNo = SyncTable_PersonCard.CardNo ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_PersonCard AS SyncTable_PersonCard
                              INNER JOIN B01_Card AS Card ON Card.CardNo = SyncTable_PersonCard.CardNo ";
                    oAcsDB.GetDataTable("ErrorPsnCardData", sSql, out ErrorPsnCardData);
                    objRet.message += "資料已存在於系統中：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion



            #endregion

            #region 確認PsnCard中的組織架構是否確實存在
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*)
                          FROM dbo.B00_SyncTable_PersonCard
                          LEFT JOIN OrgStrucAllData('') AS StrucAllData ON StrucAllData.OrgIDList = (
                              ISNULL ( '\' + ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Company) + '\','') +
                              ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Unit) + '\','') +
                              ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Department) + '\','') +
                              ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Title) + '\','')
                          )
                          WHERE StrucAllData.OrgStrucID IS NULL ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT *
                              FROM dbo.B00_SyncTable_PersonCard
                              LEFT JOIN OrgStrucAllData('') AS StrucAllData ON StrucAllData.OrgIDList = (
                                  ISNULL ( '\' + ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Company) + '\','') +
                                  ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Unit) + '\','') +
                                  ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Department) + '\','') +
                                  ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Title) + '\','')
                              )
                              WHERE StrucAllData.OrgStrucID IS NULL ";
                    oAcsDB.GetDataTable("ErrorPsnCardData", sSql, out ErrorPsnCardData);
                    objRet.message += "人員卡片所設定的組織架構並不存在：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            if (ErrorCount != 0)
            {
                for (int i = 0; i < ErrorPsnCardData.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += ";";
                    objRet.message += @"人員編號：" + ErrorPsnCardData.Rows[i]["PsnNo"].ToString() + "\n"
                                    + @"人員名稱：" + ErrorPsnCardData.Rows[i]["PsnName"].ToString() + "\n"
                                    + @"人員英文名稱：" + ErrorPsnCardData.Rows[i]["PsnEName"].ToString() + "\n"
                                    + @"人員類型：" + ErrorPsnCardData.Rows[i]["PsnType"].ToString() + "\n"
                                    + @"身份證號碼：" + ErrorPsnCardData.Rows[i]["IDNum"].ToString() + "\n"
                                    + @"生日：" + ErrorPsnCardData.Rows[i]["Birthday"].ToString() + "\n"
                                    + @"公司：" + ErrorPsnCardData.Rows[i]["Company"].ToString() + "\n"
                                    + @"單位：" + ErrorPsnCardData.Rows[i]["Unit"].ToString() + "\n"
                                    + @"部門：" + ErrorPsnCardData.Rows[i]["Department"].ToString() + "\n"
                                    + @"職稱：" + ErrorPsnCardData.Rows[i]["Title"].ToString() + "\n"
                                    + @"人員帳號：" + ErrorPsnCardData.Rows[i]["PsnAccount"].ToString() + "\n"
                                    + @"人員帳號密碼：" + ErrorPsnCardData.Rows[i]["PsnPW"].ToString() + "\n"
                                    + @"人員權限：" + ErrorPsnCardData.Rows[i]["PsnAuthAllow"].ToString() + "\n"
                                    + @"人員啟用時間：" + ErrorPsnCardData.Rows[i]["PsnSTime"].ToString() + "\n"
                                    + @"人員結束時間：" + ErrorPsnCardData.Rows[i]["PsnETime"].ToString() + "\n"
                                    + @"人員相片：" + ErrorPsnCardData.Rows[i]["PsnPicSource"].ToString() + "\n"
                                    + @"卡片編號：" + ErrorPsnCardData.Rows[i]["CardNo"].ToString() + "\n"
                                    + @"卡片版本：" + ErrorPsnCardData.Rows[i]["CardVer"].ToString() + "\n"
                                    + @"卡片密碼：" + ErrorPsnCardData.Rows[i]["CardPW"].ToString() + "\n"
                                    + @"卡片序號：" + ErrorPsnCardData.Rows[i]["CardSerialNo"].ToString() + "\n";
                    if (i + 1 < ErrorPsnCardData.Rows.Count)
                        objRet.message += "------------------------------" + "\n";
                }

                objRet.message = "人員卡片資料關聯有誤編號：\n" + objRet.message;
                objRet.message = objRet.message + @"-----====================================================================-----";
                objRet.result = false;
                objRet.act = "ShowPsnError";
            }

            return objRet;
        }
        #endregion

        #region 檢查Psn資料表並設定Label
        private void CheckPsnCardSyncTable()
        {
            PsnCardCorrectFlag = CheckPsnCardCorrect(out PsnTableCount);
            Label_PsnCardState.ForeColor = Color.White;
            if (PsnTableCount == 0)
            {
                Label_PsnCardState.Text = "同步資料表為空！";
                Label_PsnCardState.Style.Add("cursor", "Default;");
                Label_PsnCardState.Attributes.Remove("onClick");
            }
            else if (PsnCardCorrectFlag && PsnTableCount != 0)
            {
                Label_PsnCardState.Text = "資料正確。共" + PsnTableCount + "筆資料。";
                Label_PsnCardState.Style.Add("cursor", "Default;");
                Label_PsnCardState.Attributes.Remove("onClick");
            }
            else
            {
                Label_PsnCardState.Text = "資料關聯出現問題！(點擊查看)";
                Label_PsnCardState.ForeColor = Color.Red;
                Label_PsnCardState.Style.Add("cursor", "pointer;");
                Label_PsnCardState.Attributes["onClick"] = "ShowPsnCardErrorData(); ScrollBottom();";
            }
            PsnCardState_UpdatePanel.Update();
        }
        #endregion

        #endregion

        #endregion

        #endregion
    }
}