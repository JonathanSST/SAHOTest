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
using DapperDataObjectLib;



namespace SahoAcs
{
    public partial class ExamImport : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        static bool PsnCardCorrectFlag = false;
        static int PsnTableCount = 0;
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

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
            ClientScript.RegisterClientScriptInclude("ExamImport", "ExamImport.js");//加入同一頁面所需的JavaScript檔案

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
                IsSuccess = SetHastSetValueFormXlsx(FileUpload_PsnCard, "考試編號", out PsnCard_set);

            if (!IsSuccess)
            {
                Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('未選擇檔案或格式不符！');");
            }

            #region 暫存資料庫清空
            if (IsSuccess)
            {
                SQLComResult = this.odo.Execute("DELETE B00_SyncTable_ExamData");
            }
            #endregion

            #region 暫存資料庫寫入
            sSql = @" INSERT dbo.B00_SyncTable_ExamData
                               (ExamNo, ExamName)
                               VALUES (@ExamNo,@ExamName) ";
            List<DBModel.B01Examdata> datas = new List<DBModel.B01Examdata>();
            if (IsSuccess)
            {
                for (int i = 0; i < PsnCard_set.Count; i++)
                {
                    datas.Add(new DBModel.B01Examdata()
                    {
                        ExamNo= ((string[])PsnCard_set.ElementAt(i))[0].ToString(),
                        ExamName= ((string[])PsnCard_set.ElementAt(i))[1].ToString()
                    });
                    if (datas.Count % 50 == 0)
                    {
                        SQLComResult = this.odo.Execute(sSql, datas);                        
                        datas.Clear();
                        if (SQLComResult == 0)
                        {
                            IsSuccess = false;
                            break;
                        }
                    }
                }
                if (SQLComResult != -1)
                    this.odo.Execute(sSql, datas);
                if (SQLComResult == 0)
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
        


        #region PsnCard_ImportButton_Click
        protected void PsnCard_ImportButton_Click(object sender, EventArgs e)
        {
            string sSql = "", MsgStr = "", sCardNo = "";
            
            List<string> liSqlPara = new List<string>();
            int iRet = 0;

            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string sIPAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            
            if (PsnCardCorrectFlag && PsnTableCount > 0)
            {
                #region 新增Exam資料
                if (iRet > -1)
                {
                    //sSql = @" INSERT INTO B01_ExamData (OrgNo,ExamNo,ExamName,ExamBeginTime,ExamEndTime,CreateUserID,CreateTime)
                    //          SELECT '',ExamNo,ExamName,GETDATE(),'2099/12/31',@UserID,GETDATE() FROM B00_SyncTable_ExamData ";
                    //轉入時CreateUserID空白，後續USER選承辦單位時再寫入
                    sSql = @" INSERT INTO B01_ExamData (OrgNo,ExamNo,ExamName,ExamBeginTime,ExamEndTime,CreateUserID,CreateTime)
                              SELECT '',ExamNo,ExamName,GETDATE(),'2099/12/31','',GETDATE() FROM B00_SyncTable_ExamData ";
                    iRet = this.odo.Execute(sSql, new {UserID=Session["UserID"]});
                }
                #endregion

                if (iRet > 0)
                {
                    MsgStr += "試務資料同步完成,處理共" + iRet + "筆。\n";
                }
                
                #region 清空暫存資料表
                if (iRet > 0)
                {
                    sSql = @" DELETE FROM B00_SyncTable_ExamData ";
                    iRet = this.odo.Execute(sSql);
                }
                #endregion

                if (iRet > 0)
                {
                    MsgStr += "資料同步完成,已清空暫存資料表。\n";
                }
                #endregion



                if (iRet > -1)
                {
                    //oAcsDB.Commit();
                    TextBox_EquMsg.Text += MsgStr;
                    EquMsg_UpdatePanel.Update();
                    CheckPsnCardSyncTable();
                }
                else
                {
                    //oAcsDB.Rollback();
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
            sSql = "SELECT COUNT(*) FROM B00_SyncTable_ExamData";
            RowCount = this.odo.GetIntScalar(sSql);
            #endregion

            #region 檢查必填欄位是否填寫
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_ExamData AS SyncTable
                          WHERE ExamNo = '' OR ExamName = '' ";

                ErrorCount += this.odo.GetIntScalar(sSql);
            }
            #endregion

            #region 檢查必填欄位正確性
            
            
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
            //DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
            #region 檢查必填欄位是否填寫
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_ExamData AS SyncTable_ExamData
                          WHERE ExamNo = '' OR ExamName = '' ";
                ErrorCount += odo.GetIntScalar(sSql);       //取得未輸入必要欄位的筆數
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_ExamData AS SyncTable_ExamData WHERE ExamNo = '' OR ExamName = '' ";
                    ErrorPsnCardData = odo.GetDataTableBySql(sSql);
                    objRet.message += "必填欄位請填寫：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion
            
            if (ErrorCount != 0)
            {
                for (int i = 0; i < ErrorPsnCardData.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(objRet.message))
                        objRet.message += ";";
                    objRet.message += @"考市編號：" + ErrorPsnCardData.Rows[i]["ExamNo"].ToString() + "\n"
                                    + @"人員名稱：" + ErrorPsnCardData.Rows[i]["ExamName"].ToString() + "\n";
                    if (i + 1 < ErrorPsnCardData.Rows.Count)
                        objRet.message += "------------------------------" + "\n";
                }

                objRet.message = "試務資料有誤編號：\n" + objRet.message;
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