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
    public partial class ImportData2 : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        static bool DciCorrectFlag = false, MstCorrectFlag = false, CtrlCorrectFlag = false, ReaderCorrectFlag = false, EquGroupCorrectFlag = false, 
            OrgCorrectFlag = false, OrgStrucCorrectFlag = false, PsnCardCorrectFlag = false;
        static int DciTableCount = 0, MstTableCount = 0, CtrlTableCount = 0, ReaderTableCount = 0, 
            EquGroupTableCount = 0, OrgTableCount = 0, OrgStrucTableCount = 0, PsnTableCount = 0;
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(CleanTable_Dci);
            oScriptManager.RegisterAsyncPostBackControl(CleanTable_Mst);
            oScriptManager.RegisterAsyncPostBackControl(CleanTable_Ctrl);
            oScriptManager.RegisterAsyncPostBackControl(CleanTable_Reader);
            oScriptManager.RegisterAsyncPostBackControl(Equ_ImportButton);

            oScriptManager.RegisterAsyncPostBackControl(CleanTable_EquGroup);
            oScriptManager.RegisterAsyncPostBackControl(EquGroup_ImportButton);

            oScriptManager.RegisterAsyncPostBackControl(CleanTable_Org);
            oScriptManager.RegisterAsyncPostBackControl(CleanTable_OrgStruc);
            oScriptManager.RegisterAsyncPostBackControl(Org_ImportButton);
            oScriptManager.RegisterAsyncPostBackControl(this.OrgStruc_ImportButton);

            oScriptManager.RegisterAsyncPostBackControl(CleanTable_PsnCard);
            oScriptManager.RegisterAsyncPostBackControl(PsnCard_ImportButton);
            
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("ImportData", "ImportData.js");//加入同一頁面所需的JavaScript檔案

            #region 註冊主頁Button動作
            //SaveButton.Attributes["onClick"] = "SaveExcute();return false;";
            #endregion

            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion

                CheckEquSyncTable();
                CheckEquGroupSyncTable();
                CheckOrgSyncTable();
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
                else if (sFormArg == "Equ_UpdateState")
                {
                    CheckEquSyncTable();
                }
                else if (sFormArg == "EquGroup_UpdateState")
                {
                    CheckEquGroupSyncTable();
                }
                else if (sFormArg == "Org_UpdateState")
                {
                    CheckOrgSyncTable();
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

        #region 設備資料
        #region 匯入事件

        #region UpLoadButton_Dci_Click
        protected void UpLoadButton_Dci_Click(object sender, EventArgs e)
        {
            bool IsSuccess = false;
            string sSql = "", sParaStr = "";
            int SQLComResult = 0;
            List<string> liSqlPara = new List<string>();
            HashSet<string[]> Dci_set = new HashSet<string[]>();

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            if (FileUpload_Dci.HasFile && System.IO.Path.GetExtension(FileUpload_Dci.PostedFile.FileName) == ".xlsx")
                IsSuccess = SetHastSetValueFormXlsx(FileUpload_Dci, "設備連線編號", out Dci_set);

            if (!IsSuccess)
            {
                Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('未選擇檔案或格式不符！');");
            }

            #region 暫存資料庫清空
            if (IsSuccess)
            {
                sSql = ""; sParaStr = "";
                liSqlPara.Clear();
                sSql = " DELETE B00_SyncTable_DeviceConnInfo ";
                SQLComResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
                if (SQLComResult == -1)
                    IsSuccess = false;
            }
            #endregion

            #region 暫存資料庫寫入
            if (IsSuccess)
            {
                sSql = ""; sParaStr = "";
                liSqlPara.Clear();
                for (int i = 0; i < Dci_set.Count; i++)
                {
                    sParaStr = "?, ?, ?, ?, ?, ?";
                    sSql += @" INSERT INTO B00_SyncTable_DeviceConnInfo(DciNo, DciName, IsAssignIP, IpAddress, TcpPort, DciPassWD)
                               VALUES (" + sParaStr + @") ";
                    liSqlPara.Add("S:" + ((string[])Dci_set.ElementAt(i))[0].ToString());   //DciNo
                    liSqlPara.Add("S:" + ((string[])Dci_set.ElementAt(i))[1].ToString());   //DciName
                    liSqlPara.Add("S:" + ((string[])Dci_set.ElementAt(i))[2].ToString());   //IsAssignIP
                    liSqlPara.Add("S:" + ((string[])Dci_set.ElementAt(i))[3].ToString());   //IpAddress
                    liSqlPara.Add("S:" + ((string[])Dci_set.ElementAt(i))[4].ToString());   //TcpPort
                    liSqlPara.Add("S:" + ((string[])Dci_set.ElementAt(i))[5].ToString());   //DciPassWD
                }
                SQLComResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
                if (SQLComResult == -1)
                    IsSuccess = false;
            }
            #endregion

            #region 資料庫安全性檢查
            if (IsSuccess)
            {
                IsSuccess = CheckDciCorrect(out DciTableCount);
            }
            #endregion

            Sa.Web.Fun.RunJavaScript(this, "__doPostBack('" + DciState_UpdatePanel.ClientID + "', 'Equ_UpdateState');");
        }
        #endregion

        #region UpLoadButton_Mst_Click
        protected void UpLoadButton_Mst_Click(object sender, EventArgs e)
        {
            bool IsSuccess = false;
            string sSql = "", sParaStr = "";
            int SQLComResult = 0;
            List<string> liSqlPara = new List<string>();
            HashSet<string[]> Mst_set = new HashSet<string[]>();

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            if (FileUpload_Mst.HasFile && System.IO.Path.GetExtension(FileUpload_Mst.PostedFile.FileName) == ".xlsx")
                IsSuccess = SetHastSetValueFormXlsx(FileUpload_Mst, "連線裝置編號", out Mst_set);

            if (!IsSuccess)
            {
                Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('未選擇檔案或格式不符！');");
            }

            #region 暫存資料庫清空
            if (IsSuccess)
            {
                sSql = ""; sParaStr = "";
                liSqlPara.Clear();
                sSql = " DELETE B00_SyncTable_Master ";
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
                for (int i = 0; i < Mst_set.Count; i++)
                {
                    sParaStr = "?, ?, ?, ?, ?, ?, ?, ?, ?, ?";
                    sSql += @" INSERT INTO B00_SyncTable_Master(MstNo, MstDesc, MstType, MstConnParam, CtrlModel, LinkMode, AutoReturn, MstModel, MstStatus, DciNo)
                               VALUES (" + sParaStr + @") ";

                    liSqlPara.Add("S:" + ((string[])Mst_set.ElementAt(i))[0].ToString());   //MstNo
                    liSqlPara.Add("S:" + ((string[])Mst_set.ElementAt(i))[1].ToString());   //MstDesc
                    liSqlPara.Add("S:" + ((string[])Mst_set.ElementAt(i))[2].ToString());   //MstType
                    liSqlPara.Add("S:" + ((string[])Mst_set.ElementAt(i))[3].ToString());   //MstConnParam
                    liSqlPara.Add("S:" + ((string[])Mst_set.ElementAt(i))[4].ToString());   //CtrlModel
                    liSqlPara.Add("S:" + ((string[])Mst_set.ElementAt(i))[5].ToString());   //LinkMode
                    liSqlPara.Add("S:" + ((string[])Mst_set.ElementAt(i))[6].ToString());   //AutoReturn
                    liSqlPara.Add("S:" + ((string[])Mst_set.ElementAt(i))[7].ToString());   //MstModel
                    liSqlPara.Add("S:" + ((string[])Mst_set.ElementAt(i))[8].ToString());   //MstStatus
                    liSqlPara.Add("S:" + ((string[])Mst_set.ElementAt(i))[9].ToString());   //DciNo
                }
                SQLComResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
                if (SQLComResult == -1)
                    IsSuccess = false;
            }
            #endregion

            #region 資料庫安全性檢查
            if (IsSuccess)
            {
                IsSuccess = CheckMstCorrect(out MstTableCount);
            }
            #endregion

            Sa.Web.Fun.RunJavaScript(this, "__doPostBack('" + MstState_UpdatePanel.ClientID + "', 'Equ_UpdateState');");
        }
        #endregion

        #region UpLoadButton_Ctrl_Click
        protected void UpLoadButton_Ctrl_Click(object sender, EventArgs e)
        {
            bool IsSuccess = false;
            string sSql = "", sParaStr = "";
            int SQLComResult = 0, linecount = 0;
            List<string> liSqlPara = new List<string>();
            HashSet<string[]> Ctrl_set = new HashSet<string[]>();

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            if (FileUpload_Ctrl.HasFile && System.IO.Path.GetExtension(FileUpload_Ctrl.PostedFile.FileName) == ".xlsx")
                IsSuccess = SetHastSetValueFormXlsx(FileUpload_Ctrl, "控制器編號", out Ctrl_set);

            if (!IsSuccess)
            {
                Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('未選擇檔案或格式不符！');");
            }

            #region 暫存資料庫清空
            if (IsSuccess)
            {
                sSql = ""; sParaStr = "";
                liSqlPara.Clear();
                sSql = " DELETE B00_SyncTable_Controller ";
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
                for (int i = 0; i < Ctrl_set.Count; i++)
                {
                    sParaStr = "?, ?, ?, ?, ?, ?, ?";
                    sSql += @" INSERT INTO B00_SyncTable_Controller (CtrlNo, CtrlName, CtrlDesc, CtrlAddr, CtrlType, CtrlStatus, MstNo)
                               VALUES (" + sParaStr + @") ";

                    liSqlPara.Add("S:" + ((string[])Ctrl_set.ElementAt(i))[0].ToString());   //CtrlNo
                    liSqlPara.Add("S:" + ((string[])Ctrl_set.ElementAt(i))[1].ToString());   //CtrlName
                    liSqlPara.Add("S:" + ((string[])Ctrl_set.ElementAt(i))[2].ToString());   //CtrlDesc
                    liSqlPara.Add("S:" + ((string[])Ctrl_set.ElementAt(i))[3].ToString());   //CtrlAddr
                    liSqlPara.Add("S:" + ((string[])Ctrl_set.ElementAt(i))[4].ToString());   //CtrlType
                    //liSqlPara.Add("S:" + ((string[])Ctrl_set.ElementAt(i))[5].ToString());   //CtrlModel
                    //liSqlPara.Add("S:" + ((string[])Ctrl_set.ElementAt(i))[6].ToString());   //CtrlFwVer
                    liSqlPara.Add("S:" + ((string[])Ctrl_set.ElementAt(i))[5].ToString());   //CtrlStatus
                    liSqlPara.Add("S:" + ((string[])Ctrl_set.ElementAt(i))[6].ToString());   //MstNo
                    linecount++;

                    if (linecount % 100 == 0)
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
                IsSuccess = CheckCtrlCorrect(out CtrlTableCount);
            }
            #endregion

            Sa.Web.Fun.RunJavaScript(this, "__doPostBack('" + CtrlState_UpdatePanel.ClientID + "', 'Equ_UpdateState');");
        }
        #endregion

        #region UpLoadButton_Reader_Click
        protected void UpLoadButton_Reader_Click(object sender, EventArgs e)
        {
            bool IsSuccess = false;
            string sSql = "", sParaStr = "";
            int SQLComResult = 0, linecount = 0;
            List<string> liSqlPara = new List<string>();
            HashSet<string[]> Reader_set = new HashSet<string[]>();

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            if (FileUpload_Reader.HasFile && System.IO.Path.GetExtension(FileUpload_Reader.PostedFile.FileName) == ".xlsx")
                IsSuccess = SetHastSetValueFormXlsx(FileUpload_Reader, "讀卡機編號", out Reader_set);

            if (!IsSuccess)
            {
                Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('未選擇檔案或格式不符！');");
            }

            #region 暫存資料庫清空
            if (IsSuccess)
            {
                sSql = ""; sParaStr = "";
                liSqlPara.Clear();
                sSql = " DELETE B00_SyncTable_Reader ";
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
                for (int i = 0; i < Reader_set.Count; i++)
                {
                    sParaStr = "?, ?, ?, ?, ?, ?";
                    sSql += @" INSERT INTO B00_SyncTable_Reader(ReaderNo, ReaderName, ReaderDesc, EquNo, Dir, CtrlNo)
                               VALUES (" + sParaStr + @") ";

                    liSqlPara.Add("S:" + ((string[])Reader_set.ElementAt(i))[0].ToString());   //ReaderNo
                    liSqlPara.Add("S:" + ((string[])Reader_set.ElementAt(i))[1].ToString());   //ReaderName
                    liSqlPara.Add("S:" + ((string[])Reader_set.ElementAt(i))[2].ToString());   //ReaderDesc
                    liSqlPara.Add("S:" + ((string[])Reader_set.ElementAt(i))[3].ToString());   //EquNo
                    liSqlPara.Add("S:" + ((string[])Reader_set.ElementAt(i))[4].ToString());   //Dir
                    liSqlPara.Add("S:" + ((string[])Reader_set.ElementAt(i))[5].ToString());   //CtrlNo
                    linecount++;

                    if (linecount % 100 == 0)
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
                IsSuccess = CheckReaderCorrect(out ReaderTableCount);
            }
            #endregion

            Sa.Web.Fun.RunJavaScript(this, "__doPostBack('" + ReaderState_UpdatePanel.ClientID + "', 'Equ_UpdateState');");
        }
        #endregion

        #region Equ_ImportButton_Click
        protected void Equ_ImportButton_Click(object sender, EventArgs e)
        {
            string sSql = "", MsgStr = "";
            List<string> liSqlPara = new List<string>();
            int iRet = 0;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            oAcsDB.BeginTransaction();

            if (DciCorrectFlag && DciTableCount >= 1 && MstCorrectFlag && MstTableCount >= 1 && CtrlCorrectFlag && CtrlTableCount >= 1 && ReaderCorrectFlag && ReaderTableCount >= 1)
            {
                #region 新增Dci資料
                if (iRet > -1)
                {
                    sSql = @" INSERT INTO B01_DeviceConnInfo(DciNo, DciName, IsAssignIP, IpAddress, TcpPort, DciPassWD, CreateUserID, CreateTime)
                              SELECT *,'SAHO',GETDATE() FROM B00_SyncTable_DeviceConnInfo ";
                    iRet = oAcsDB.SqlCommandExecute(sSql);
                }
                #endregion

                if (iRet > -1)
                {
                    MsgStr += "連線基本資料同步完成,處理共" + iRet + "筆。\n";
                }

                #region 新增Equ資料
                if (iRet > -1)
                {
                    sSql = @" INSERT INTO B01_EquData(EquNo, EquName, EquClass, EquModel, IsAndTrt, DciID, Building, CreateUserID, CreateTime)
                              SELECT Reader.EquNo, Reader.ReaderName,
                              CASE Controller.CtrlType 
                              WHEN 'Door' THEN 'Door Access'
                              WHEN 'TRT' THEN 'TRT'
                              WHEN 'Meal' THEN 'Meal'
                              WHEN 'Elev' THEN 'Elevator'
                              ELSE 'Door Access' END AS EquClass,
                              Master.CtrlModel AS EquModel,
                              CASE Controller.CtrlType WHEN 'TRT' THEN '1'
                              ELSE '0' END AS IsAndTrt,
                              DeviceConnInfo.DciID AS DciID,
                              'Import Data' AS Building,
                              'SAHO' AS CreateUserID,
                              GETDATE() AS CreateTime
                              FROM dbo.B00_SyncTable_Reader AS Reader
                              INNER JOIN B00_SyncTable_Controller AS Controller ON Controller.CtrlNo =  Reader.CtrlNo
                              INNER JOIN dbo.B00_SyncTable_Master AS Master ON Master.MstNo = Controller.MstNo
                              INNER JOIN dbo.B01_DeviceConnInfo AS DeviceConnInfo ON DeviceConnInfo.DciNo = Master.DciNo ";
                    iRet = oAcsDB.SqlCommandExecute(sSql);
                }
                #endregion

                if (iRet > -1)
                {
                    MsgStr += "設備資料同步完成,處理共" + iRet + "筆。\n";
                }

                #region 新增Mst資料
                if (iRet > -1)
                {
                    sSql = @" INSERT INTO B01_Master(MstNo, MstDesc, MstType, MstConnParam, CtrlModel, LinkMode, AutoReturn, MstModel, MstStatus, DciID, CreateUserID, CreateTime)
                              SELECT SyncTable_Master.MstNo, SyncTable_Master.MstDesc,
                              SyncTable_Master.MstType, SyncTable_Master.MstConnParam,
                              SyncTable_Master.CtrlModel, SyncTable_Master.LinkMode,
                              SyncTable_Master.AutoReturn, SyncTable_Master.MstModel,
                              SyncTable_Master.MstStatus, DeviceConnInfo.DciID,
                              'SAHO',GETDATE()
                              FROM B00_SyncTable_Master AS SyncTable_Master
                              INNER JOIN B01_DeviceConnInfo AS DeviceConnInfo ON DeviceConnInfo.DciNo = SyncTable_Master.DciNo ";
                    iRet = oAcsDB.SqlCommandExecute(sSql);
                }
                #endregion

                if (iRet > -1)
                {
                    MsgStr += "連線裝置資料同步完成,處理共" + iRet + "筆。\n";
                }

                #region 新增Ctrl資料
                if (iRet > -1)
                {
                    sSql = @" INSERT INTO B01_Controller(CtrlNo, CtrlName, CtrlDesc, CtrlAddr, CtrlModel, CtrlStatus, MstID, CreateUserID, CreateTime)
                              SELECT 
                              SyncTable_Controller.CtrlNo, SyncTable_Controller.CtrlName,
                              SyncTable_Controller.CtrlDesc, SyncTable_Controller.CtrlAddr,
                              Master.CtrlModel,
                              SyncTable_Controller.CtrlStatus, 
                              Master.MstID, 'SAHO',GETDATE() 
                              FROM B00_SyncTable_Controller AS SyncTable_Controller
                              INNER JOIN B01_Master AS Master ON Master.MstNo = SyncTable_Controller.MstNo";
                    iRet = oAcsDB.SqlCommandExecute(sSql);
                }
                #endregion

                if (iRet > -1)
                {
                    MsgStr += "控制器資料同步完成,處理共" + iRet + "筆。\n";
                }

                #region 新增Reader資料
                if (iRet > -1)
                {
                    sSql = @" INSERT INTO B01_Reader(ReaderNo, ReaderName, ReaderDesc, CtrlID, EquNo, Dir, CreateUserID, CreateTime)
                              SELECT
                              SyncTable_Reader.ReaderNo, SyncTable_Reader.ReaderName,
                              SyncTable_Reader.ReaderDesc, Controller.CtrlID,
                              SyncTable_Reader.EquNo, SyncTable_Reader.Dir,
                              'SAHO',GETDATE()
                              FROM B00_SyncTable_Reader AS SyncTable_Reader
                              INNER JOIN B01_Controller AS Controller ON Controller.CtrlNo = SyncTable_Reader.CtrlNo";
                    iRet = oAcsDB.SqlCommandExecute(sSql);
                }
                #endregion

                if (iRet > -1)
                {
                    MsgStr += "讀卡機資料同步完成,處理共" + iRet + "筆。\n";
                }

                #region 清空暫存資料表
                if (iRet > -1)
                {
                    sSql = @" DELETE FROM B00_SyncTable_DeviceConnInfo
                              DELETE FROM B00_SyncTable_Master
                              DELETE FROM B00_SyncTable_Controller
                              DELETE FROM B00_SyncTable_Reader";
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
                    CheckEquSyncTable();
                }
                else
                {
                    oAcsDB.Rollback();
                    MsgStr = "同步發生錯誤,同步中止。";
                    TextBox_EquMsg.Text += MsgStr;
                    EquMsg_UpdatePanel.Update();
                }
            }
            else
            {
                Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('設備資料不完全,請確認資料皆正確匯入！');");
            }
        }
        #endregion

        #endregion

        #region  清除資料表事件

        #region CleanTable_Dci_Click
        protected void CleanTable_Dci_Click(object sender, EventArgs e)
        {
            string sSql = "";
            int SQLComResult = 0;
            List<string> liSqlPara = new List<string>();

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            #region 暫存資料庫清空
            sSql = " DELETE B00_SyncTable_DeviceConnInfo ";
            SQLComResult = oAcsDB.SqlCommandExecute(sSql);
            #endregion

            Sa.Web.Fun.RunJavaScript(this, "__doPostBack('" + CleanTable_Dci.ClientID + "', 'Equ_UpdateState')");
        }
        #endregion

        #region CleanTable_Mst_Click
        protected void CleanTable_Mst_Click(object sender, EventArgs e)
        {
            string sSql = "";
            int SQLComResult = 0;
            List<string> liSqlPara = new List<string>();

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            #region 暫存資料庫清空
            sSql = " DELETE B00_SyncTable_Master ";
            SQLComResult = oAcsDB.SqlCommandExecute(sSql);
            #endregion

            Sa.Web.Fun.RunJavaScript(this, "__doPostBack('" + CleanTable_Mst.ClientID + "', 'Equ_UpdateState')");
        }
        #endregion

        #region CleanTable_Ctrl_Click
        protected void CleanTable_Ctrl_Click(object sender, EventArgs e)
        {
            string sSql = "";
            int SQLComResult = 0;
            List<string> liSqlPara = new List<string>();

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            #region 暫存資料庫清空
            sSql = " DELETE B00_SyncTable_Controller ";
            SQLComResult = oAcsDB.SqlCommandExecute(sSql);
            #endregion

            Sa.Web.Fun.RunJavaScript(this, "__doPostBack('" + CleanTable_Ctrl.ClientID + "', 'Equ_UpdateState')");
        }
        #endregion

        #region CleanTable_Reader_Click
        protected void CleanTable_Reader_Click(object sender, EventArgs e)
        {
            string sSql = "";
            int SQLComResult = 0;
            List<string> liSqlPara = new List<string>();

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            #region 暫存資料庫清空
            sSql = " DELETE B00_SyncTable_Reader ";
            SQLComResult = oAcsDB.SqlCommandExecute(sSql);
            #endregion

            Sa.Web.Fun.RunJavaScript(this, "__doPostBack('" + CleanTable_Reader.ClientID + "', 'Equ_UpdateState')");
        }
        #endregion

        #endregion
        #endregion

        #region 設備群組
        #region 匯入事件

        #region UpLoadButton_EquGroup_Click
        protected void UpLoadButton_EquGroup_Click(object sender, EventArgs e)
        {
            bool IsSuccess = false;
            string sSql = "", sParaStr = "";
            int SQLComResult = 0, linecount = 0, ErrorCount = 0;
            DateTime ToDay = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            int iRet = 0;
            HashSet<string[]> EuqGroup_set = new HashSet<string[]>();
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            if (FileUpload_EquGroup.HasFile && System.IO.Path.GetExtension(FileUpload_EquGroup.PostedFile.FileName) == ".xlsx")
                IsSuccess = SetHastSetValueFormXlsx(FileUpload_EquGroup, "設備群組編號", out EuqGroup_set);

            if (!IsSuccess)
            {
                Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('未選擇檔案或格式不符！');");
            }

            #region 暫存資料庫清空
            if (IsSuccess)
            {
                sSql = ""; sParaStr = "";
                liSqlPara.Clear();
                sSql = " DELETE B00_SyncTable_EquGroup ";
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
                for (int i = 0; i < EuqGroup_set.Count; i++)
                {
                    sParaStr = "?, ?, ?, ?";
                    sSql += @" INSERT INTO B00_SyncTable_EquGroup (EquGrpNo, EquGrpName, CtrlNoList, EquNoList)
                                VALUES (" + sParaStr + ") ";

                    liSqlPara.Add("S:" + ((string[])EuqGroup_set.ElementAt(i))[0].ToString());   //EquGrpNo
                    liSqlPara.Add("S:" + ((string[])EuqGroup_set.ElementAt(i))[1].ToString());   //EquGrpName
                    liSqlPara.Add("S:" + ((string[])EuqGroup_set.ElementAt(i))[2].ToString());   //CtrlList
                    liSqlPara.Add("S:" + ((string[])EuqGroup_set.ElementAt(i))[3].ToString());   //EquList
                    linecount++;

                    if (linecount % 100 == 0)
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
                IsSuccess = CheckEquGroupCorrect(out EquGroupTableCount);
            }
            #endregion

            Sa.Web.Fun.RunJavaScript(this, "__doPostBack('" + EquGroupState_UpdatePanel.ClientID + "', 'EquGroup_UpdateState');");
        }
        #endregion

        #region EquGroup_ImportButton_Click
        protected void EquGroup_ImportButton_Click(object sender, EventArgs e)
        {
            string sSql = "", sSql2 = "", sSql3 = "", ParaStr = "", MsgStr = "";
            List<string> liSqlPara = new List<string>();
            List<string> liSqlPara2 = new List<string>();
            List<string> liSqlPara3 = new List<string>();
            string[] ArrayParaCount;
            string[] EquNoList;
            int iRet = 0;
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            bool IsSuccess = false;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            oAcsDB.BeginTransaction();

            if (EquGroupCorrectFlag && EquGroupTableCount > 1)
            {
                #region 新增EquGroup資料
                if (iRet > -1)
                {
                    sSql = @" INSERT INTO B01_EquGroup(EquGrpNo, EquGrpName, OwnerID, OwnerList, CreateUserID, CreateTime)
                              SELECT EquGrpNo, EquGrpName, 'Saho', '\Saho\', 'SAHO', GETDATE() FROM B00_SyncTable_EquGroup ";
                    iRet = oAcsDB.SqlCommandExecute(sSql);
                }
                #endregion

                if (iRet > -1)
                {
                    MsgStr += "設備群組資料同步完成,處理共" + iRet + "筆。\n";
                }

                #region 匯入EquGroupData資料
                sSql = @" SELECT EquGroupBase.*, EquGroup.EquGrpID,
                          dbo.GetTableIDStr(CtrlNoList,'Controller') AS CtrlID,
                          dbo.GetTableIDStr(EquNoList,'Equ') AS EquID
                          FROM B00_SyncTable_EquGroup AS EquGroupBase 
                          LEFT JOIN dbo.B01_EquGroup AS EquGroup ON EquGroup.EquGrpNo = EquGroupBase.EquGrpNo ";
                oAcsDB.GetDataTable("OrgEquGroupTable", sSql, true, out dt);

                for (int x = 0; x < dt.Rows.Count; x++)
                {
                    Hashtable EquIDHash = new Hashtable();
                    #region 依CtrlID與EquID重整出所唯一的EquID HashTable

                    ArrayParaCount = dt.Rows[x]["CtrlID"].ToString().Split(',');
                    for (int i = 0; i < ArrayParaCount.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(ParaStr)) ParaStr += ",";
                        ParaStr += "?";
                        liSqlPara2.Add("S:" + (ArrayParaCount[i].ToString()));            //CtrlID
                    }

                    sSql2 = @" SELECT DISTINCT EquData.EquID FROM dbo.B01_Controller AS Controller
                               LEFT JOIN dbo.B01_Reader AS Reader ON Reader.CtrlID = Controller.CtrlID
                               LEFT JOIN dbo.B01_EquData AS EquData ON EquData.EquNo = Reader.EquNo
                               WHERE Controller.CtrlID IN (" + ParaStr + ") AND EquData.EquID IS NOT NULL ";
                    
                    oAcsDB.GetDataTable("OrgEquGroupTable", sSql2, liSqlPara2, out dt2);
                    ParaStr = "";
                    liSqlPara2.Clear();
                    if (dt2.Rows.Count > 0)
                    {
                        #region 將CtrlNoList轉化為EquID放入EquIDHash
                        for (int i = 0; i < dt2.Rows.Count; i++)
                        {
                            if (!EquIDHash.Contains(dt2.Rows[i]["EquID"].ToString()))
                                EquIDHash.Add(dt2.Rows[i]["EquID"].ToString(), dt2.Rows[i]["EquID"].ToString());
                        }
                        #endregion

                        #region 將EquNoList轉化為EquID放入EquIDHash
                        if (!string.IsNullOrEmpty(dt.Rows[x]["EquID"].ToString()))
                        {
                            EquNoList = dt.Rows[x]["EquID"].ToString().Split(',');

                            for (int p = 0; p < EquNoList.Length; p++)
                            {
                                if (!EquIDHash.Contains(EquNoList[p].ToString()))
                                    EquIDHash.Add(EquNoList[p].ToString(), EquNoList[p].ToString());
                            }
                        }
                        #endregion
                    }
                    #endregion

                    if (EquIDHash.Count > 0)
                    {
                        try
                        {
                            #region 依EquGropu依次新增
                            foreach (DictionaryEntry EquIDObj in EquIDHash)
                            {
                                sSql3 = @" INSERT INTO B01_EquGroupData ( EquGrpID, EquID, CreateUserID, CreateTime)
                                           VALUES ( ?, ?, ?, ?) ";

                                liSqlPara3.Add("S:" + (dt.Rows[x]["EquGrpID"].ToString()));     //EquGrpID
                                liSqlPara3.Add("S:" + EquIDObj.Value);                          //EquID
                                liSqlPara3.Add("S:Saho");                                       //CreateUserID
                                liSqlPara3.Add("D:" + DateTime.Now.ToString());                 //CreateTime
                                iRet = oAcsDB.SqlCommandExecute(sSql3, liSqlPara3);
                                sSql3 = "";
                                liSqlPara3.Clear();
                                if (iRet == -1) break;
                            }
                            #endregion
                        }
                        catch
                        {
                            iRet = -1;
                        }
                    }

                    EquIDHash.Clear();
                }
                #endregion

                if (iRet > -1)
                {
                    MsgStr += "設備群組列表資料同步完成,處理共" + iRet + "筆。\n";
                }

                #region 設備群組加入管理區
                sSql = @" INSERT INTO B01_MgnEquGroup (MgaID, EquGrpID, CreateUserID, CreateTime)
                          SELECT '1', EquGrpID, 'SAHO', GETDATE() FROM dbo.B01_EquGroup";
                iRet = oAcsDB.SqlCommandExecute(sSql);
                #endregion

                //if (iRet > -1)
                //{
                //    MsgStr += "設備群組加入管理區,處理共" + iRet + "筆。\n";
                //}

                #region 清空暫存資料表
                if (iRet > -1)
                {
                    sSql = @" DELETE FROM B00_SyncTable_EquGroup ";
                    iRet = oAcsDB.SqlCommandExecute(sSql);
                }
                #endregion

                if (iRet > -1)
                {
                    oAcsDB.Commit();
                    TextBox_EquMsg.Text += MsgStr;
                    EquMsg_UpdatePanel.Update();
                    CheckEquGroupSyncTable();
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

        #region Button_EquGroup_Click
        protected void Button_EquGroup_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Web/02/0207/EquGroup.aspx");
        }
        #endregion

        #endregion

        #region  清除資料表事件

        #region  CleanTable_EquGroup_Click
        protected void CleanTable_EquGroup_Click(object sender, EventArgs e)
        {
            string sSql = "";
            int SQLComResult = 0;
            List<string> liSqlPara = new List<string>();

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            #region 暫存資料庫清空
            sSql = " DELETE B00_SyncTable_EquGroup ";
            SQLComResult = oAcsDB.SqlCommandExecute(sSql);
            #endregion

            Sa.Web.Fun.RunJavaScript(this, "__doPostBack('" + EquGroupState_UpdatePanel.ClientID + "', 'EquGroup_UpdateState');");
        }
        #endregion

        #endregion
        #endregion

        #region 組織單位
        #region 匯入事件

        #region UpLoadButton_Org_Click
        protected void UpLoadButton_Org_Click(object sender, EventArgs e)
        {
            bool IsSuccess = false;
            string sSql = "", sParaStr = "", sLeadCode = "";
            int SQLComResult = 0, linecount = 0, ErrorCount = 0;
            DateTime ToDay = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            int iRet = 0;
            HashSet<string[]> Org_set = new HashSet<string[]>();
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            if (FileUpload_Org.HasFile && System.IO.Path.GetExtension(FileUpload_Org.PostedFile.FileName) == ".xlsx")
                IsSuccess = SetHastSetValueFormXlsx(FileUpload_Org, "組織類別", out Org_set);

            if (!IsSuccess)
            {
                Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('未選擇檔案或格式不符！');");
            }

            #region 暫存資料庫清空
            if (IsSuccess)
            {
                sSql = ""; sParaStr = "";
                liSqlPara.Clear();
                sSql = " DELETE B00_SyncTable_OrgData ";
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
                for (int i = 0; i < Org_set.Count; i++)
                {
                    sParaStr = "?, ?, ?";
                    sSql += @" INSERT INTO B00_SyncTable_OrgData(OrgClass, OrgNo, OrgName)
                               VALUES (" + sParaStr + ") ";

                    liSqlPara.Add("S:" + ((string[])Org_set.ElementAt(i))[0].ToString());   //OrgClass
                    switch (((string[])Org_set.ElementAt(i))[0].ToString())
                    {
                        case "Company":
                            sLeadCode = "C";
                            break;
                        case "Unit":
                            sLeadCode = "U";
                            break;
                        case "Department":
                            sLeadCode = "D";
                            break;
                        case "Title":
                            sLeadCode = "T";
                            break;
                    }
                    liSqlPara.Add("S:" + sLeadCode + ((string[])Org_set.ElementAt(i))[1].ToString());   //OrgNo
                    liSqlPara.Add("S:" + ((string[])Org_set.ElementAt(i))[2].ToString());   //OrgName
                    linecount++;

                    if (linecount % 100 == 0)
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
                IsSuccess = CheckOrgCorrect(out OrgTableCount);
            }
            #endregion

            Sa.Web.Fun.RunJavaScript(this, "__doPostBack('" + OrgState_UpdatePanel.ClientID + "', 'Org_UpdateState');");
        }
        #endregion

        #region UpLoadButton_OrgStruc_Click
        protected void UpLoadButton_OrgStruc_Click(object sender, EventArgs e)
        {
            bool IsSuccess = false;
            string sSql = "", sParaStr = "";
            int SQLComResult = 0, linecount = 0, ErrorCount = 0;
            DateTime ToDay = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            int iRet = 0;
            HashSet<string[]> OrgStruc_set = new HashSet<string[]>();
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            if (FileUpload_OrgStruc.HasFile && System.IO.Path.GetExtension(FileUpload_OrgStruc.PostedFile.FileName) == ".xlsx")
                IsSuccess = SetHastSetValueFormXlsx(FileUpload_OrgStruc, "公司", out OrgStruc_set);

            if (!IsSuccess)
            {
                Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('未選擇檔案或格式不符！');");
            }

            #region 暫存資料庫清空
            if (IsSuccess)
            {
                sSql = ""; sParaStr = "";
                liSqlPara.Clear();
                sSql = " DELETE B00_SyncTable_OrgStruc ";
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
                for (int i = 0; i < OrgStruc_set.Count; i++)
                {
                    sParaStr = "?, ?, ?, ?, ?";
                    sSql += @" INSERT INTO B00_SyncTable_OrgStruc(Company, Unit, Department, Title, EquGrpNoList)
                               VALUES (" + sParaStr + ") ";

                    liSqlPara.Add("S:" + ((string[])OrgStruc_set.ElementAt(i))[0].ToString());   //Company
                    liSqlPara.Add("S:" + ((string[])OrgStruc_set.ElementAt(i))[1].ToString());   //Unit
                    liSqlPara.Add("S:" + ((string[])OrgStruc_set.ElementAt(i))[2].ToString());   //Department
                    liSqlPara.Add("S:" + ((string[])OrgStruc_set.ElementAt(i))[3].ToString());   //Title
                    liSqlPara.Add("S:" + ((string[])OrgStruc_set.ElementAt(i))[4].ToString());   //EquGrpNoList
                    linecount++;

                    if (linecount % 100 == 0)
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
                IsSuccess = CheckOrgStrucCorrect(out OrgStrucTableCount);
            }
            #endregion

            Sa.Web.Fun.RunJavaScript(this, "__doPostBack('" + OrgStrucState_UpdatePanel.ClientID + "', 'Org_UpdateState');");
        }
        #endregion

        #region Org_ImportButton_Click
        protected void Org_ImportButton_Click(object sender, EventArgs e)
        {
            string sSql = "", MsgStr = "";
            string sSqlMaster = "", sSqlCondition = "";
            List<string> liSqlPara = new List<string>();
            int iRet = 0, linecount = 0;
            DataTable dt = new DataTable();
            DataTable dtErr = new DataTable();
            bool IsSuccess = false;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            oAcsDB.BeginTransaction();

            if (OrgCorrectFlag && OrgTableCount > 0)
            {
                #region 新增Org資料
                if (iRet > -1)
                {
                    sSql = @" INSERT INTO B01_OrgData(OrgClass, OrgNo, OrgName, CreateUserID, CreateTime)
                              SELECT *,'SAHO',GETDATE() FROM B00_SyncTable_OrgData WHERE OrgNo NOT IN (SELECT OrgNo FROM B01_OrgData) ";
                    iRet = oAcsDB.SqlCommandExecute(sSql);
                }
                else
                    iRet = -1;
                #endregion

                if (iRet > -1)
                {
                    MsgStr += "組織基本資料同步完成,處理共" + iRet + "筆。\n";
                }

                #region 清空暫存資料表
                if (iRet > -1)
                {
                    sSql = @" DELETE FROM B00_SyncTable_OrgData WHERE OrgNo IN (SELECT OrgNo FROM B01_OrgData)  ";
                    iRet = oAcsDB.SqlCommandExecute(sSql);
                }
                #endregion

                if (iRet > -1)
                {
                    oAcsDB.Commit();
                    TextBox_EquMsg.Text += MsgStr;
                    EquMsg_UpdatePanel.Update();
                    //CheckOrgSyncTable();
                }
                else
                {
                    oAcsDB.Rollback();
                    MsgStr = "同步發生錯誤,同步中止。";
                    TextBox_EquMsg.Text += MsgStr;
                    EquMsg_UpdatePanel.Update();
                }
            }                       
            CheckOrgSyncTable();
        }
        #endregion

        #region OrgStruc_ImportButton_Click

        protected void OrgStruc_ImportButton_Click(object sender, EventArgs e)
        {
            string sSql = "", MsgStr = "";
            string sSqlMaster = "", sSqlCondition = "";
            List<string> liSqlPara = new List<string>();
            int iRet = 0, linecount = 0;
            DataTable dt = new DataTable();
            DataTable dtErr = new DataTable();
            bool IsSuccess = false;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            oAcsDB.BeginTransaction();
            if (OrgStrucCorrectFlag && OrgStrucTableCount != 0)
            {
                #region 新增OrgStruc資料
                if (iRet > -1)
                {
                    sSqlMaster = @" SELECT * FROM (
                              SELECT 
                              LEFT(BaseTable.OrgStrucNo,LEN(BaseTable.OrgStrucNo)-1) AS OrgStrucNo, OrgStruc, CreateUserID, CreateTime,Company,Unit,Department,Title
                              FROM (
	                                SELECT 
	                                ISNULL ( ( SELECT CAST(OrgNo AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Company) + '.','') +
	                                ISNULL ( ( SELECT CAST(OrgNo AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Unit) + '.','') +
	                                ISNULL ( ( SELECT CAST(OrgNo AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Department) + '.','') +
	                                ISNULL ( ( SELECT CAST(OrgNo AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Title) + '.','') AS OrgStrucNo,
	                                ISNULL ( '\' + ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Company) + '\','') +
	                                ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Unit) + '\','') +
	                                ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Department) + '\','') +
	                                ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Title) + '\','') AS OrgStruc
                                    ,Company,Unit,Department,Title
	                                ,'saho' AS CreateUserID,GETDATE() AS CreateTime FROM dbo.B00_SyncTable_OrgStruc
                              ) AS BaseTable ) AS R1 ";
                    oAcsDB.GetDataTable("oTable", sSqlMaster + " WHERE OrgStrucNo IN (SELECT OrgStrucNo FROM B01_OrgStruc)", out dtErr);
                    foreach (DataRow r in dtErr.Rows)
                    {
                        MsgStr += string.Format("\n重複的組織架構編號：{0}---名稱：\\{1}\\{2}\\{3}\\{4}...\n", r["OrgStrucNo"], r["Company"], r["Unit"], r["Department"], r["Title"]);
                    }
                    sSql = @" INSERT INTO B01_OrgStruc(OrgStrucNo, OrgIDList, CreateUserID, CreateTime) "
                        + "SELECT OrgStrucNo, OrgStruc, CreateUserID, CreateTime FROM (" + sSqlMaster + " ) AS R2 "
                        + " WHERE OrgStrucNo NOT IN (SELECT OrgStrucNo FROM B01_OrgStruc) ";
                    iRet = oAcsDB.SqlCommandExecute(sSql);
                }
                else
                    iRet = -1;
                #endregion

                if (iRet > -1)
                {
                    MsgStr += "組織架構資料同步完成,處理共" + iRet + "筆。\n";
                }

                #region 新增OrgEquGroup資料
                sSql = @" SELECT OrgStrucAll.OrgStrucID, dbo.GetTableIDStr(EquGrpNoList,'EquGroup') AS EquGrpID
                          FROM B00_SyncTable_OrgStruc AS OrgStruc
                          LEFT JOIN OrgStrucAllData('') AS OrgStrucAll 
                          ON OrgStrucAll.OrgNameList = ( '\' +
	                          CASE Company WHEN NULL THEN '' WHEN '' THEN '' ELSE OrgStruc.Company + '\' END +
	                          CASE Unit WHEN NULL THEN '' WHEN '' THEN '' ELSE OrgStruc.Unit + '\' END +
	                          CASE Department WHEN NULL THEN '' WHEN '' THEN '' ELSE OrgStruc.Department + '\' END +
	                          CASE Title WHEN NULL THEN '' WHEN '' THEN '' ELSE OrgStruc.Title + '\' END   
                          ) WHERE OrgStruc.EquGrpNoList <> ''";
                oAcsDB.GetDataTable("OrgEquGroupTable", sSql, true, out dt);
                sSql = "";
                liSqlPara.Clear();

                string[] EquGrpIDArray = null;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    EquGrpIDArray = dt.Rows[i]["EquGrpID"].ToString().Split(',');
                    for (int p = 0; p < EquGrpIDArray.Length; p++)
                    {
                        sSql += @" INSERT dbo.B01_OrgEquGroup
                                   (OrgStrucID, EquGrpID, CreateUserID, CreateTime)
                                   VALUES (?, ?, ?, ?) ";

                        liSqlPara.Add("S:" + dt.Rows[i]["OrgStrucID"].ToString());     //OrgStrucID
                        liSqlPara.Add("S:" + EquGrpIDArray[p]);                        //EquGrpID
                        liSqlPara.Add("S:" + "Saho");                                  //CreateUserID
                        liSqlPara.Add("D:" + DateTime.Now.ToString());                 //PsnEName

                        linecount++;

                        if (linecount % 50 == 0)
                        {
                            iRet = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
                            sSql = "";
                            liSqlPara.Clear();
                            if (iRet == -1)
                            {
                                IsSuccess = false;
                                break;
                            }
                        }
                    }
                }
                if (iRet > -1 && !string.IsNullOrEmpty(sSql))
                {
                    iRet = oAcsDB.SqlCommandExecute(sSql, liSqlPara);
                }
                #endregion

                if (iRet > -1)
                {
                    MsgStr += "組織預設權限資料同步完成,處理共" + iRet + "筆。\n";
                }

                #region 組織架構加入管理區
                sSql = @" DELETE FROM B01_MgnOrgStrucs WHERE MgaID = '1';
                    INSERT INTO B01_MgnOrgStrucs (MgaID, OrgStrucID, CreateUserID, CreateTime)
                    SELECT '1', OrgStrucID, 'Saho', GETDATE() FROM dbo.B01_OrgStruc WHERE OrgStrucID
                    NOT IN (SELECT OrgStrucID FROM B01_MgnOrgStrucs WHERE MgaID='1');";
                iRet = oAcsDB.SqlCommandExecute(sSql);
                #endregion

                //if (iRet > -1)
                //{
                //    MsgStr += "組織架構加入管理區,處理共" + iRet + "筆。\n";
                //}

                #region 清空暫存資料表
                if (iRet > -1)
                {
                    sSql = @" DELETE FROM B00_SyncTable_OrgStruc ";
                    iRet = oAcsDB.SqlCommandExecute(sSql);
                }
                #endregion

                if (iRet > -1)
                {
                    oAcsDB.Commit();
                    TextBox_EquMsg.Text += MsgStr;
                    EquMsg_UpdatePanel.Update();
                    //CheckOrgSyncTable();
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

        #region CleanTable_Org_Click
        protected void CleanTable_Org_Click(object sender, EventArgs e)
        {
            string sSql = "";
            int SQLComResult = 0;
            List<string> liSqlPara = new List<string>();

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            #region 暫存資料庫清空
            sSql = " DELETE B00_SyncTable_OrgData ";
            SQLComResult = oAcsDB.SqlCommandExecute(sSql);
            #endregion

            Sa.Web.Fun.RunJavaScript(this, "__doPostBack('" + CleanTable_Org.ClientID + "', 'Org_UpdateState')");
        }
        #endregion

        #region CleanTable_OrgStruc_Click
        protected void CleanTable_OrgStruc_Click(object sender, EventArgs e)
        {
            string sSql = "";
            int SQLComResult = 0;
            List<string> liSqlPara = new List<string>();

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            #region 暫存資料庫清空
            sSql = " DELETE B00_SyncTable_OrgStruc ";
            SQLComResult = oAcsDB.SqlCommandExecute(sSql);
            #endregion

            Sa.Web.Fun.RunJavaScript(this, "__doPostBack('" + CleanTable_OrgStruc.ClientID + "', 'Org_UpdateState')");
        }
        #endregion

        #endregion

        #endregion

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
                    sParaStr = "?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?";
                    sSql += @" INSERT dbo.B00_SyncTable_PersonCard
                               (PsnNo, PsnName, PsnEName, PsnType, IDNum, Birthday, Company, Unit, Department, Title, PsnAccount, PsnPW, PsnAuthAllow, PsnSTime, PsnETime, PsnPicSource, CardNo, CardVer, CardPW, CardSerialNo)
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
                    var sql_master = @"SELECT 
                              PersonCard.PsnNo, PersonCard.PsnName, PersonCard.PsnEName, PersonCard.PsnType, 
                              PersonCard.IDNum, PersonCard.Birthday, OrgStrucAll.OrgStrucID,
                              PsnAccount, PsnPW, PsnAuthAllow, 
                              PsnSTime, PsnETime, PsnPicSource, 
                              'Saho', GETDATE()
                              FROM dbo.B00_SyncTable_PersonCard AS PersonCard
                              LEFT JOIN OrgStrucAllData('') AS OrgStrucAll ON OrgStrucAll.OrgIDList = (
                              ISNULL ( '\' + ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Company) + '\','') +
                              ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Unit) + '\','') +
                              ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Department) + '\','') +
                              ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Title) + '\','')
                          )";
                    sSql = @"INSERT INTO B01_Person(PsnNo, PsnName, PsnEName, PsnType, IDNum, Birthday, OrgStrucID, PsnAccount
                                    , PsnPW, PsnAuthAllow, PsnSTime, PsnETime, PsnPicSource, CreateUserID, CreateTime) " 
                        +sql_master+ " WHERE OrgStrucAll.OrgStrucID IS NOT NULL";
                    iRet = oAcsDB.SqlCommandExecute(sSql);
                    sSql = sql_master + " WHERE OrgStrucAll.OrgStrucID IS NULL";
                    oAcsDB.GetDataTable("PersonUnInsert",sSql,out SyncTable_CardTable);
                    foreach (DataRow r in SyncTable_CardTable.Rows)
                    {
                        MsgStr += string.Format("人員{0} 編號{1}無對應組織架構。\n",r["PsnName"],r["PsnNo"]);
                    }
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
                              INNER JOIN B01_Person AS Person ON Person.PsnNo = PersonCard.PsnNo ";
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
//                    sSql = @" INSERT INTO B01_CardEquGroup(CardID, EquGrpID, CreateUserID)
//                              SELECT Card.CardID, OrgEquGroup.EquGrpID,
//                              'Saho'
//                              FROM dbo.B01_Person AS Person
//                              LEFT JOIN dbo.B01_Card AS Card ON Card.PsnID = Person.PsnID
//                              LEFT JOIN B01_OrgEquGroup AS OrgEquGroup ON OrgEquGroup.OrgStrucID = Person.OrgStrucID
//                              WHERE OrgEquGroup.EquGrpID IS NOT NULL ";
                    sSql = @"INSERT INTO B01_CardEquGroup(CardID, EquGrpID, CreateUserID)
                                    SELECT 
	                                    Card.CardID, OrgEquGroup.EquGrpID,'Saho'
                                    FROM 
	                                    dbo.B01_Person AS Person
                                        INNER JOIN dbo.B01_Card AS Card ON Card.PsnID = Person.PsnID
                                        INNER JOIN B01_OrgEquGroup AS OrgEquGroup ON OrgEquGroup.OrgStrucID = Person.OrgStrucID
	                                    LEFT JOIN B01_CardEquGroup AS CardEquGroup ON Card.CardID=CardEquGroup.CardID
                                    WHERE 
	                                    CardEquGroup.CardID IS NULL";
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

                if (iRet > -1)
                {
                    oAcsDB.Commit();
                    TextBox_EquMsg.Text += MsgStr;
                    //EquMsg_UpdatePanel.Update();
                    //CheckPsnCardSyncTable();
                }
                else
                {
                    oAcsDB.Rollback();
                    MsgStr = "同步發生錯誤,同步中止。";
                    TextBox_EquMsg.Text += MsgStr;                    
                    CheckPsnCardSyncTable();
                    EquMsg_UpdatePanel.Update();
                }

                #region 權限重整              
                if (iRet > -1)
                {
                    sSql = @"SELECT C.* FROM B00_SyncTable_PersonCard S
                                        INNER JOIN B01_Card C ON S.CardNo=C.CardNo";
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
                    sSql = @" DELETE FROM B00_SyncTable_PersonCard WHERE PsnNo IN (SELECT PsnNo FROM B01_Person)";
                    iRet = oAcsDB.SqlCommandExecute(sSql);
                }
                #endregion

                if (iRet > -1)
                {
                    MsgStr += "資料同步完成,已清空暫存資料表。\n";
                    CheckPsnCardSyncTable();
                    EquMsg_UpdatePanel.Update();
                }
                else
                {                    
                    CheckPsnCardSyncTable();
                    EquMsg_UpdatePanel.Update();
                }
                              
            }//判斷同步資料正確性 End
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI", "$.unblockUI()", true);
        }//結束人員資料同步作業
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
            Sa.Web.Fun.RunJavaScript(this, "$.unblockUI()");
            
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

        #region 設備資料

        #region 確認Dci暫存資料表的正確性
        private bool CheckDciCorrect(out int ReturnRowCount)
        {
            bool IsSuccess = true;
            string sSql = "";
            int ErrorCount = 0, RowCount = 0;

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            #region 取得暫存資料表資料筆數
            sSql = "SELECT COUNT(*) FROM B00_SyncTable_DeviceConnInfo";
            RowCount = oAcsDB.GetIntScalar(sSql);
            #endregion

            #region 檢查必填欄位是否填寫
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_DeviceConnInfo AS SyncTable_DeviceConnInfo
                          WHERE DciNo = '' OR DciName = '' OR IsAssignIP = '' OR DciPassWD = '' ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region 檢查必填欄位正確性

            #region IsAssignIP
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_DeviceConnInfo AS SyncTable_DeviceConnInfo
                          WHERE IsAssignIP NOT IN ('0','1') ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #endregion

            #region 檢查Dci是否已存在系統中
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B01_DeviceConnInfo AS DeviceConnInfo
                          INNER JOIN B00_SyncTable_DeviceConnInfo AS SyncTable_DeviceConnInfo ON SyncTable_DeviceConnInfo.DciNo = DeviceConnInfo.DciNo ";

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

        #region 確認Mst暫存資料表的正確性
        private bool CheckMstCorrect(out int ReturnRowCount)
        {
            bool IsSuccess = true;
            string sSql = "";
            int ErrorCount = 0, RowCount = 0;

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            #region 取得暫存資料表資料筆數
            sSql = "SELECT COUNT(*) FROM B00_SyncTable_Master";
            RowCount = oAcsDB.GetIntScalar(sSql);
            #endregion

            #region 檢查必填欄位是否填寫
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Master AS Master 
                          WHERE MstNo = '' OR MstType = '' OR MstConnParam = '' OR CtrlModel = '' OR LinkMode = '' OR AutoReturn = '' OR MstStatus = '' OR DciNo = '' ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region 檢查必填欄位正確性

            #region MstType
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Master AS SyncTable_Master
                          WHERE MstType NOT IN ('T','C') ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region CtrlModel
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Master AS SyncTable_Master
                          WHERE CtrlModel NOT IN ( SELECT ItemNo FROM dbo.B00_ItemList WHERE ItemClass = 'EquModel' ) ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region LinkMode
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Master AS SyncTable_Master
                          WHERE LinkMode NOT IN ('0','1') ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region AutoReturn
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Master AS SyncTable_Master
                          WHERE AutoReturn NOT IN ('0','1') ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region MstStatus
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Master AS SyncTable_Master
                          WHERE MstStatus NOT IN ('0','1') ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #endregion

            #region 檢查Mst是否已存在系統中
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B01_Master AS Master
                          INNER JOIN B00_SyncTable_Master AS SyncTable_Master ON SyncTable_Master.MstNo = Master.MstNo ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region 檢查Mst暫存資料表與Dci暫存資料表的連結關係是否正確
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Master AS SyncTable_Master
                          LEFT JOIN B00_SyncTable_DeviceConnInfo AS SyncTable_DeviceConnInfo ON SyncTable_DeviceConnInfo.DciNo = SyncTable_Master.DciNo
                          WHERE SyncTable_DeviceConnInfo.DciNo IS NULL ";

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

        #region 確認Ctrl暫存資料表的正確性
        private bool CheckCtrlCorrect(out int ReturnRowCount)
        {
            bool IsSuccess = true;
            string sSql = "";
            int ErrorCount = 0, RowCount = 0;

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            #region 取得暫存資料表資料筆數
            sSql = "SELECT COUNT(*) FROM B00_SyncTable_Controller";
            RowCount = oAcsDB.GetIntScalar(sSql);
            #endregion

            #region 檢查必填欄位是否填寫
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Controller AS SyncTable_Controller
                          WHERE CtrlNo = '' OR CtrlAddr = '' OR CtrlType = '' OR CtrlStatus = '' OR MstNo = '' ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region 檢查必填欄位正確性

            #region CtrlType
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Controller AS SyncTable_Controller
                          WHERE CtrlType NOT IN ('Door','TRT','Meal','Elev') ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region CtrlStatus
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Controller AS SyncTable_Controller
                          WHERE CtrlStatus NOT IN ('0','1','2') ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #endregion

            #region 檢查Ctrl是否已存在系統中
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B01_Controller AS Controller
                          INNER JOIN B00_SyncTable_Controller AS SyncTable_Controller ON SyncTable_Controller.CtrlNo = Controller.CtrlNo ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region 檢查Ctel暫存資料表與Mst暫存資料表的連結關係是否正確
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Controller AS SyncTable_Controller
                          LEFT JOIN B00_SyncTable_Master AS SyncTable_Master ON SyncTable_Master.MstNo = SyncTable_Controller.MstNo
                          WHERE SyncTable_Master.MstNo IS NULL ";

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

        #region 確認Reader暫存資料表的正確性
        private bool CheckReaderCorrect(out int ReturnRowCount)
        {
            bool IsSuccess = true;
            string sSql = "";
            int ErrorCount = 0, RowCount = 0;

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            #region 取得暫存資料表資料筆數
            sSql = "SELECT COUNT(*) FROM B00_SyncTable_Reader";
            RowCount = oAcsDB.GetIntScalar(sSql);
            #endregion

            #region 檢查必填欄位是否填寫
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM dbo.B00_SyncTable_Reader AS SyncTable_Reader
                          WHERE ReaderNo = '' OR EquNo = '' OR Dir = '' OR CtrlNo = '' ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region 檢查必填欄位正確性

            #region Dir
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Reader AS SyncTable_Reader
                          WHERE Dir NOT IN ('進','出') ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #endregion

            #region 檢查Reader是否已存在系統中
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B01_Controller AS Controller
                          INNER JOIN dbo.B01_Reader AS Reader ON Reader.CtrlID = Controller.CtrlID
                          INNER JOIN dbo.B00_SyncTable_Reader AS SyncTable_Reader ON SyncTable_Reader.CtrlNo = Controller.CtrlNo AND SyncTable_Reader.ReaderNo = Reader.ReaderNo ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region 檢查Reader暫存資料表與Ctrl暫存資料表的連結關係是否正確
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Reader AS SyncTable_Reader
                          LEFT JOIN B00_SyncTable_Controller AS SyncTable_Controller ON SyncTable_Controller.CtrlNo = SyncTable_Reader.CtrlNo
                          WHERE SyncTable_Controller.CtrlNo IS NULL ";

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

        #region 顯示Dci錯誤資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object ShowDciErrorData()
        {
            string sSql = "";
            int ErrorCount = 0;
            DataTable ErrorDciData = null;
            Pub.MessageObject objRet = new Pub.MessageObject();
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

            #region 檢查必填欄位是否填寫
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_DeviceConnInfo AS SyncTable_DeviceConnInfo
                          WHERE DciNo = '' OR DciName = '' OR IsAssignIP = '' OR DciPassWD = '' ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT SyncTable_DeviceConnInfo.* FROM B00_SyncTable_DeviceConnInfo AS SyncTable_DeviceConnInfo
                              WHERE DciNo = '' OR DciName = '' OR IsAssignIP = '' OR DciPassWD = '' ";
                    oAcsDB.GetDataTable("ErrorDciData", sSql, out ErrorDciData);
                    objRet.message += "必填欄位請填寫：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region 檢查必填欄位正確性

            #region IsAssignIP
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_DeviceConnInfo AS SyncTable_DeviceConnInfo
                          WHERE IsAssignIP NOT IN ('0','1') ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT SyncTable_DeviceConnInfo.* FROM B00_SyncTable_DeviceConnInfo AS SyncTable_DeviceConnInfo
                              WHERE IsAssignIP NOT IN ('0','1') ";
                    oAcsDB.GetDataTable("ErrorDciData", sSql, out ErrorDciData);
                    objRet.message += "必填欄位有誤：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #endregion

            #region 檢查Dci是否已存在系統中
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B01_DeviceConnInfo AS DeviceConnInfo
                          INNER JOIN B00_SyncTable_DeviceConnInfo AS SyncTable_DeviceConnInfo ON SyncTable_DeviceConnInfo.DciNo = DeviceConnInfo.DciNo ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT SyncTable_DeviceConnInfo.* FROM B01_DeviceConnInfo AS DeviceConnInfo
                              INNER JOIN B00_SyncTable_DeviceConnInfo AS SyncTable_DeviceConnInfo ON SyncTable_DeviceConnInfo.DciNo = DeviceConnInfo.DciNo ";
                    oAcsDB.GetDataTable("ErrorDciData", sSql, out ErrorDciData);
                    objRet.message += "資料已存在於系統中：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            if (ErrorCount != 0)
            {
                for (int i = 0; i < ErrorDciData.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += ";";
                    objRet.message += @"設備連線編號：" + ErrorDciData.Rows[i]["DciNo"].ToString() + "\n"
                                    + @"設備連線：" + ErrorDciData.Rows[i]["DciName"].ToString() + "\n"
                                    + @"是否指定連入的設備驅動程式IP位置：" + ErrorDciData.Rows[i]["IsAssignIP"].ToString() + "\n"
                                    + @"設備驅動程式IP位置：" + ErrorDciData.Rows[i]["IpAddress"].ToString() + "\n"
                                    + @"設備驅動程式網路端口：" + ErrorDciData.Rows[i]["TcpPort"].ToString() + "\n"
                                    + @"設備連線密碼：" + ErrorDciData.Rows[i]["DciPassWD"].ToString() + "\n";
                    if (i + 1 < ErrorDciData.Rows.Count)
                        objRet.message += "------------------------------" + "\n";
                }

                objRet.message = "連線資料關聯有誤編號：\n" + objRet.message;
                objRet.message = objRet.message + @"-----====================================================================-----";
                objRet.result = false;
                objRet.act = "ShowDciError";
            }

            return objRet;
        }
        #endregion

        #region 顯示Mst錯誤資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object ShowMstErrorData()
        {
            string sSql = "";
            int ErrorCount = 0;
            DataTable ErrorMstData = null;
            Pub.MessageObject objRet = new Pub.MessageObject();
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

            #region 檢查必填欄位是否填寫
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Master AS SyncTable_Master 
                          WHERE MstNo = '' OR MstType = '' OR MstConnParam = '' OR CtrlModel = '' OR LinkMode = '' OR AutoReturn = '' OR MstStatus = '' OR DciNo = '' ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_Master AS SyncTable_Master 
                              WHERE MstNo = '' OR MstType = '' OR MstConnParam = '' OR CtrlModel = '' OR LinkMode = '' OR AutoReturn = '' OR MstStatus = '' OR DciNo = '' ";
                    oAcsDB.GetDataTable("ErrorMstData", sSql, out ErrorMstData);
                    objRet.message += "必填欄位請填寫：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region 檢查必填欄位正確性

            #region MstType
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Master AS SyncTable_Master
                          WHERE MstType NOT IN ('T','C') ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_Master AS SyncTable_Master
                              WHERE MstType NOT IN ('T','C') ";
                    oAcsDB.GetDataTable("ErrorMstData", sSql, out ErrorMstData);
                    objRet.message += "必填欄位有誤：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region CtrlModel
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Master AS SyncTable_Master
                          WHERE CtrlModel NOT IN ( SELECT ItemNo FROM dbo.B00_ItemList WHERE ItemClass = 'EquModel' ) ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_Master AS SyncTable_Master
                              WHERE CtrlModel NOT IN ( SELECT ItemNo FROM dbo.B00_ItemList WHERE ItemClass = 'EquModel' ) ";
                    oAcsDB.GetDataTable("ErrorMstData", sSql, out ErrorMstData);
                    objRet.message += "必填欄位有誤：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region LinkMode
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Master AS SyncTable_Master
                          WHERE LinkMode NOT IN ('0','1') ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_Master AS SyncTable_Master
                              WHERE LinkMode NOT IN ('0','1') ";
                    oAcsDB.GetDataTable("ErrorMstData", sSql, out ErrorMstData);
                    objRet.message += "必填欄位有誤：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region AutoReturn
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Master AS SyncTable_Master
                          WHERE AutoReturn NOT IN ('0','1') ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_Master AS SyncTable_Master
                              WHERE AutoReturn NOT IN ('0','1') ";
                    oAcsDB.GetDataTable("ErrorMstData", sSql, out ErrorMstData);
                    objRet.message += "必填欄位有誤：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region MstStatus
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Master AS SyncTable_Master
                          WHERE MstStatus NOT IN ('0','1') ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_Master AS SyncTable_Master
                              WHERE MstStatus NOT IN ('0','1') ";
                    oAcsDB.GetDataTable("ErrorMstData", sSql, out ErrorMstData);
                    objRet.message += "必填欄位有誤：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #endregion

            #region 檢查Mst是否已存在系統中
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B01_Master AS Master
                          INNER JOIN B00_SyncTable_Master AS SyncTable_Master ON SyncTable_Master.MstNo = Master.MstNo ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B01_Master AS Master
                              INNER JOIN B00_SyncTable_Master AS SyncTable_Master ON SyncTable_Master.MstNo = Master.MstNo ";
                    oAcsDB.GetDataTable("ErrorMstData", sSql, out ErrorMstData);
                    objRet.message += "資料已存在於系統中：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region 檢查Mst暫存資料表與Dci暫存資料表的連結關係是否正確
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Master AS SyncTable_Master
                          LEFT JOIN B00_SyncTable_DeviceConnInfo AS SyncTable_DeviceConnInfo ON SyncTable_DeviceConnInfo.DciNo = SyncTable_Master.DciNo
                          WHERE SyncTable_DeviceConnInfo.DciNo IS NULL ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_Master AS SyncTable_Master
                              LEFT JOIN B00_SyncTable_DeviceConnInfo AS SyncTable_DeviceConnInfo ON SyncTable_DeviceConnInfo.DciNo = SyncTable_Master.DciNo
                              WHERE SyncTable_DeviceConnInfo.DciNo IS NULL ";
                    oAcsDB.GetDataTable("ErrorMstData", sSql, out ErrorMstData);
                    objRet.message += "連線編號不存在：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            if (ErrorCount != 0)
            {
                for (int i = 0; i < ErrorMstData.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += ";";
                    objRet.message += @"連線裝置編號：" + ErrorMstData.Rows[i]["MstNo"].ToString() + "\n"
                                    + @"裝置說明：" + ErrorMstData.Rows[i]["MstDesc"].ToString() + "\n"
                                    + @"連線類型：" + ErrorMstData.Rows[i]["MstType"].ToString() + "\n"
                                    + @"連線資訊參數：" + ErrorMstData.Rows[i]["MstConnParam"].ToString() + "\n"
                                    + @"控制器機型：" + ErrorMstData.Rows[i]["CtrlModel"].ToString() + "\n"
                                    + @"連線模式：" + ErrorMstData.Rows[i]["LinkMode"].ToString() + "\n"
                                    + @"主動回傳：" + ErrorMstData.Rows[i]["AutoReturn"].ToString() + "\n"
                                    + @"連線裝置機型：" + ErrorMstData.Rows[i]["MstModel"].ToString() + "\n"
                                    + @"連線裝置狀態：" + ErrorMstData.Rows[i]["MstStatus"].ToString() + "\n"
                                    + @"連線編號：" + ErrorMstData.Rows[i]["DciNo"].ToString() + "\n";
                    if (i + 1 < ErrorMstData.Rows.Count)
                        objRet.message += "------------------------------" + "\n";
                }

                objRet.message = "連線裝置資料關聯有誤編號：\n" + objRet.message;
                objRet.message = objRet.message + @"-----====================================================================-----";
                objRet.result = false;
                objRet.act = "ShowMstError";
            }

            return objRet;
        }
        #endregion

        #region 顯示Ctrl錯誤資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object ShowCtrlErrorData()
        {
            string sSql = "";
            int ErrorCount = 0;
            DataTable ErrorCtrlData = null;
            Pub.MessageObject objRet = new Pub.MessageObject();
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

            #region 檢查必填欄位是否填寫
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Controller AS SyncTable_Controller
                          WHERE CtrlNo = '' OR CtrlAddr = '' OR CtrlType = '' OR CtrlStatus = '' OR MstNo = '' ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_Controller AS SyncTable_Controller
                              WHERE CtrlNo = '' OR CtrlAddr = '' OR CtrlType = '' OR CtrlStatus = '' OR MstNo = ''  ";
                    oAcsDB.GetDataTable("ErrorCtrlData", sSql, out ErrorCtrlData);
                    objRet.message += "必填欄位請填寫：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region 檢查必填欄位正確性

            #region CtrlType
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Controller AS SyncTable_Controller
                          WHERE CtrlType NOT IN ('Door','TRT','Meal','Elev') ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_Controller AS SyncTable_Controller
                              WHERE CtrlType NOT IN ('Door','TRT','Meal','Elev') ";
                    oAcsDB.GetDataTable("ErrorCtrlData", sSql, out ErrorCtrlData);
                    objRet.message += "必填欄位有誤：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region CtrlStatus
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Controller AS SyncTable_Controller
                          WHERE CtrlStatus NOT IN ('0','1','2') ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_Controller AS SyncTable_Controller
                              WHERE CtrlStatus NOT IN ('0','1','2') ";
                    oAcsDB.GetDataTable("ErrorCtrlData", sSql, out ErrorCtrlData);
                    objRet.message += "必填欄位有誤：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #endregion

            #region 檢查Ctrl是否已存在系統中
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B01_Controller AS Controller
                          INNER JOIN B00_SyncTable_Controller AS SyncTable_Controller ON SyncTable_Controller.CtrlNo = Controller.CtrlNo ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B01_Controller AS Controller
                              INNER JOIN B00_SyncTable_Controller AS SyncTable_Controller ON SyncTable_Controller.CtrlNo = Controller.CtrlNo ";
                    oAcsDB.GetDataTable("ErrorCtrlData", sSql, out ErrorCtrlData);
                    objRet.message += "資料已存在於系統中：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region 檢查Ctel暫存資料表與Mst暫存資料表的連結關係是否正確
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Controller AS SyncTable_Controller
                          LEFT JOIN B00_SyncTable_Master AS SyncTable_Master ON SyncTable_Master.MstNo = SyncTable_Controller.MstNo
                          WHERE SyncTable_Master.MstNo IS NULL ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_Controller AS SyncTable_Controller
                              LEFT JOIN B00_SyncTable_Master AS SyncTable_Master ON SyncTable_Master.MstNo = SyncTable_Controller.MstNo
                              WHERE SyncTable_Master.MstNo IS NULL ";
                    oAcsDB.GetDataTable("ErrorCtrlData", sSql, out ErrorCtrlData);
                    objRet.message += "連線裝置編號不存在：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            if (ErrorCount != 0)
            {
                for (int i = 0; i < ErrorCtrlData.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += ";";
                    objRet.message += @"控制器編號：" + ErrorCtrlData.Rows[i]["CtrlNo"].ToString() + "\n"
                                    + @"控制器名稱：" + ErrorCtrlData.Rows[i]["CtrlName"].ToString() + "\n"
                                    + @"控制器說明：" + ErrorCtrlData.Rows[i]["CtrlDesc"].ToString() + "\n"
                                    + @"機號：" + ErrorCtrlData.Rows[i]["CtrlAddr"].ToString() + "\n"
                                    + @"控制器狀況：" + ErrorCtrlData.Rows[i]["CtrlStatus"].ToString() + "\n"
                                    + @"連線裝置編號：" + ErrorCtrlData.Rows[i]["MstNo"].ToString() + "\n";
                    if (i + 1 < ErrorCtrlData.Rows.Count)
                        objRet.message += "------------------------------" + "\n";
                }

                objRet.message = "控制器資料關聯有誤編號：\n" + objRet.message;
                objRet.message = objRet.message + @"-----====================================================================-----";
                objRet.result = false;
                objRet.act = "ShowCtrlError";
            }

            return objRet;
        }
        #endregion

        #region 顯示Reader錯誤資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object ShowReaderErrorData()
        {
            string sSql = "";
            int ErrorCount = 0;
            DataTable ErrorReaderData = null;
            Pub.MessageObject objRet = new Pub.MessageObject();
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

            #region 檢查必填欄位是否填寫
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM dbo.B00_SyncTable_Reader AS SyncTable_Reader
                          WHERE ReaderNo = '' OR EquNo = '' OR Dir = '' OR CtrlNo = '' ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM dbo.B00_SyncTable_Reader AS SyncTable_Reader
                              WHERE ReaderNo = '' OR EquNo = '' OR Dir = '' OR CtrlNo = '' ";
                    oAcsDB.GetDataTable("ErrorReaderData", sSql, out ErrorReaderData);
                    objRet.message += "必填欄位請填寫：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region 檢查必填欄位正確性

            #region Dir
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Reader AS SyncTable_Reader
                          WHERE Dir NOT IN ('進','出') ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_Reader AS SyncTable_Reader
                              WHERE Dir NOT IN ('進','出') ";
                    oAcsDB.GetDataTable("ErrorReaderData", sSql, out ErrorReaderData);
                    objRet.message += "必填欄位有誤：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #endregion

            #region 檢查Reader是否已存在系統中
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B01_Controller AS Controller
                          INNER JOIN dbo.B01_Reader AS Reader ON Reader.CtrlID = Controller.CtrlID
                          INNER JOIN dbo.B00_SyncTable_Reader AS SyncTable_Reader ON SyncTable_Reader.CtrlNo = Controller.CtrlNo AND SyncTable_Reader.ReaderNo = Reader.ReaderNo ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B01_Controller AS Controller
                              INNER JOIN dbo.B01_Reader AS Reader ON Reader.CtrlID = Controller.CtrlID
                              INNER JOIN dbo.B00_SyncTable_Reader AS SyncTable_Reader ON SyncTable_Reader.CtrlNo = Controller.CtrlNo AND SyncTable_Reader.ReaderNo = Reader.ReaderNo ";
                    oAcsDB.GetDataTable("ErrorReaderData", sSql, out ErrorReaderData);
                    objRet.message += "資料已存在於系統中：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region 檢查Reader暫存資料表與Ctrl暫存資料表的連結關係是否正確
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_Reader AS SyncTable_Reader
                          LEFT JOIN B00_SyncTable_Controller AS SyncTable_Controller ON SyncTable_Controller.CtrlNo = SyncTable_Reader.CtrlNo
                          WHERE SyncTable_Controller.CtrlNo IS NULL ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_Reader AS SyncTable_Reader
                              LEFT JOIN B00_SyncTable_Controller AS SyncTable_Controller ON SyncTable_Controller.CtrlNo = SyncTable_Reader.CtrlNo
                              WHERE SyncTable_Controller.CtrlNo IS NULL ";
                    oAcsDB.GetDataTable("ErrorReaderData", sSql, out ErrorReaderData);
                    objRet.message += "控制器編號不存在：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            if (ErrorCount != 0)
            {

                for (int i = 0; i < ErrorReaderData.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += ";";
                    objRet.message += @"讀卡機編號：" + ErrorReaderData.Rows[i]["ReaderNo"].ToString() + "\n"
                                    + @"讀卡機名稱：" + ErrorReaderData.Rows[i]["ReaderName"].ToString() + "\n"
                                    + @"讀卡機描述：" + ErrorReaderData.Rows[i]["ReaderDesc"].ToString() + "\n"
                                    + @"設備編號：" + ErrorReaderData.Rows[i]["EquNo"].ToString() + "\n"
                                    + @"方向描述：" + ErrorReaderData.Rows[i]["Dir"].ToString() + "\n"
                                    + @"控制器編號：" + ErrorReaderData.Rows[i]["CtrlNo"].ToString() + "\n";
                    if (i + 1 < ErrorReaderData.Rows.Count)
                        objRet.message += "------------------------------" + "\n";
                }

                objRet.message = "讀卡機資料關聯有誤編號：\n" + objRet.message;
                objRet.message = objRet.message + @"-----====================================================================-----";
                objRet.result = false;
                objRet.act = "ShowReaderError";
            }

            return objRet;
        }
        #endregion

        #region 檢查Equ資料表並設定Label
        private void CheckEquSyncTable()
        {
            DciCorrectFlag = CheckDciCorrect(out DciTableCount);
            MstCorrectFlag = CheckMstCorrect(out MstTableCount);
            CtrlCorrectFlag = CheckCtrlCorrect(out CtrlTableCount);
            ReaderCorrectFlag = CheckReaderCorrect(out ReaderTableCount);

            Label_DciState.ForeColor = Color.White;
            Label_MstState.ForeColor = Color.White;
            Label_CtrlState.ForeColor = Color.White;
            Label_ReaderState.ForeColor = Color.White;
            if (DciTableCount == 0)
            {
                Label_DciState.Text = "同步資料表為空！";
                Label_DciState.Style.Add("cursor", "Default;");
                Label_DciState.Attributes.Remove("onClick");
            }
            else if (DciCorrectFlag && DciTableCount != 0)
            {
                Label_DciState.Text = "資料正確。共" + DciTableCount + "筆資料。";
                Label_DciState.Style.Add("cursor", "Default;");
                Label_DciState.Attributes.Remove("onClick");
            }
            else
            {
                Label_DciState.Text = "資料關聯出現問題！(點擊查看)";
                Label_DciState.ForeColor = Color.Red;
                Label_DciState.Style.Add("cursor", "pointer;");
                Label_DciState.Attributes["onClick"] = "ShowDciErrorData(); ScrollBottom(); ScrollBottom();";
            }

            if (MstTableCount == 0)
            {
                Label_MstState.Text = "同步資料表為空！";
                Label_MstState.Style.Add("cursor", "Default;");
                Label_MstState.Attributes.Remove("onClick");
            }
            else if (MstCorrectFlag && MstTableCount != 0)
            {
                Label_MstState.Text = "資料正確。共" + MstTableCount + "筆資料。";
                Label_MstState.Style.Add("cursor", "Default;");
                Label_MstState.Attributes.Remove("onClick");
            }
            else
            {
                Label_MstState.Text = "資料關聯出現問題！(點擊查看)";
                Label_MstState.ForeColor = Color.Red;
                Label_MstState.Style.Add("cursor", "pointer;");
                Label_MstState.Attributes["onClick"] = "ShowMstErrorData(); ScrollBottom(); ScrollBottom();";
            }

            if (CtrlTableCount == 0)
            {
                Label_CtrlState.Text = "同步資料表為空！";
                Label_CtrlState.Style.Add("cursor", "Default;");
                Label_CtrlState.Attributes.Remove("onClick");
            }
            else if (CtrlCorrectFlag && CtrlTableCount != 0)
            {
                Label_CtrlState.Text = "資料正確。共" + CtrlTableCount + "筆資料。";
                Label_CtrlState.Style.Add("cursor", "Default;");
                Label_CtrlState.Attributes.Remove("onClick");
            }
            else
            {
                Label_CtrlState.Text = "資料關聯出現問題！(點擊查看)";
                Label_CtrlState.ForeColor = Color.Red;
                Label_CtrlState.Style.Add("cursor", "pointer;");
                Label_CtrlState.Attributes["onClick"] = "ShowCtrlErrorData(); ScrollBottom(); ScrollBottom();";
            }

            if (ReaderTableCount == 0)
            {
                Label_ReaderState.Text = "同步資料表為空！";
                Label_ReaderState.Style.Add("cursor", "Default;");
                Label_ReaderState.Attributes.Remove("onClick");
            }
            else if (ReaderCorrectFlag && ReaderTableCount != 0)
            {
                Label_ReaderState.Text = "資料正確。共" + ReaderTableCount + "筆資料。";
                Label_ReaderState.Style.Add("cursor", "Default;");
                Label_ReaderState.Attributes.Remove("onClick");
            }
            else
            {
                Label_ReaderState.Text = "資料關聯出現問題！(點擊查看)";
                Label_ReaderState.ForeColor = Color.Red;
                Label_ReaderState.Style.Add("cursor", "pointer;");
                Label_ReaderState.Attributes["onClick"] = "ShowReaderErrorData(); ScrollBottom(); ScrollBottom();";
            }

            DciState_UpdatePanel.Update();
            MstState_UpdatePanel.Update();
            CtrlState_UpdatePanel.Update();
            ReaderState_UpdatePanel.Update();
        }
        #endregion

        #endregion

        #region 設備群組

        #region 確認EquGroup暫存資料表的正確性
        private bool CheckEquGroupCorrect(out int ReturnRowCount)
        {
            bool IsSuccess = true;
            string sSql = "";
            int ErrorCount = 0, RowCount = 0;

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            #region 取得暫存資料表資料筆數
            sSql = "SELECT COUNT(*) FROM B00_SyncTable_EquGroup";
            RowCount = oAcsDB.GetIntScalar(sSql);
            #endregion

            #region 檢查必填欄位是否填寫
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_EquGroup AS SyncTable_EquGroup
                          WHERE EquGrpNo = '' OR EquGrpName = '' ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region 檢查EquGroup是否已存在系統中
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_EquGroup AS SyncTable_EquGroup
                          INNER JOIN B01_EquGroup AS EquGroup ON EquGroup.EquGrpNo = SyncTable_EquGroup.EquGrpNo ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region 檢查控制器編號是否存在
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM 
                          (
                           SELECT EquGrpNo, CtrlNoList,
                           dbo.GetTableIDStr(CtrlNoList,'Controller') AS CtrlIDList,
                           CASE CtrlNoList WHEN '' THEN 'NotData' ELSE 'Data' END AS 'NoListData',
                           CASE dbo.GetTableIDStr(CtrlNoList,'Controller') WHEN '' THEN 'NotData' ELSE 'Data' END AS 'IDListData',
                           (LEN(CtrlNoList) - LEN(REPLACE(CtrlNoList,',',''))) / LEN(',') AS 'NoListCount',
                           (LEN(dbo.GetTableIDStr(CtrlNoList,'Controller')) - LEN(REPLACE(dbo.GetTableIDStr(CtrlNoList,'Controller'),',',''))) / LEN(',') AS 'IDListCount'
                           FROM dbo.B00_SyncTable_EquGroup  
                          ) AS Base WHERE Base.NoListData <> Base.IDListData OR Base.NoListCount <> Base.IDListCount ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region 檢查設備編號是否存在
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM 
                          (
                           SELECT EquGrpNo, EquNoList,
                           dbo.GetTableIDStr(EquNoList,'Equ') AS EquIDList,
                           CASE EquNoList WHEN '' THEN 'NotData' ELSE 'Data' END AS 'NoListData',
                           CASE dbo.GetTableIDStr(EquNoList,'Equ') WHEN '' THEN 'NotData' ELSE 'Data' END AS 'IDListData',
                           (LEN(EquNoList) - LEN(REPLACE(EquNoList,',',''))) / LEN(',') AS 'NoListCount',
                           (LEN(dbo.GetTableIDStr(EquNoList,'Equ')) - LEN(REPLACE(dbo.GetTableIDStr(EquNoList,'Equ'),',',''))) / LEN(',') AS 'IDListCount'
                           FROM dbo.B00_SyncTable_EquGroup  
                          ) AS Base WHERE Base.NoListData <> Base.IDListData OR Base.NoListCount <> Base.IDListCount ";

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

        #region 顯示EquGroup錯誤資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object ShowEquGroupErrorData()
        {
            string sSql = "";
            int ErrorCount = 0;
            DataTable ErrorEquGroupData = null;
            Pub.MessageObject objRet = new Pub.MessageObject();
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

            #region 檢查必填欄位是否填寫
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_EquGroup AS SyncTable_EquGroup
                          WHERE EquGrpNo = '' OR EquGrpName = '' ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_EquGroup AS SyncTable_EquGroup
                              WHERE EquGrpNo = '' OR EquGrpName = ''  ";
                    oAcsDB.GetDataTable("ErrorEquGroupData", sSql, out ErrorEquGroupData);
                    objRet.message += "必填欄位請填寫：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region 檢查EquGroup是否已存在系統中
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_EquGroup AS SyncTable_EquGroup
                          INNER JOIN B01_EquGroup AS EquGroup ON EquGroup.EquGrpNo = SyncTable_EquGroup.EquGrpNo ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT SyncTable_EquGroup.* FROM B00_SyncTable_EquGroup AS SyncTable_EquGroup
                              INNER JOIN B01_EquGroup AS EquGroup ON EquGroup.EquGrpNo = SyncTable_EquGroup.EquGrpNo ";
                    oAcsDB.GetDataTable("ErrorEquGroupData", sSql, out ErrorEquGroupData);
                    objRet.message += "資料已存在於系統中：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region 檢查控制器編號是否存在
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM 
                          (
                           SELECT EquGrpNo, CtrlNoList,
                           dbo.GetTableIDStr(CtrlNoList,'Controller') AS CtrlIDList,
                           CASE CtrlNoList WHEN '' THEN 'NotData' ELSE 'Data' END AS 'NoListData',
                           CASE dbo.GetTableIDStr(CtrlNoList,'Controller') WHEN '' THEN 'NotData' ELSE 'Data' END AS 'IDListData',
                           (LEN(CtrlNoList) - LEN(REPLACE(CtrlNoList,',',''))) / LEN(',') AS 'NoListCount',
                           (LEN(dbo.GetTableIDStr(CtrlNoList,'Controller')) - LEN(REPLACE(dbo.GetTableIDStr(CtrlNoList,'Controller'),',',''))) / LEN(',') AS 'IDListCount'
                           FROM dbo.B00_SyncTable_EquGroup  
                          ) AS Base WHERE Base.NoListData <> Base.IDListData OR Base.NoListCount <> Base.IDListCount ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT SyncTable_EquGroup.* FROM 
                              (
                               SELECT EquGrpNo, CtrlNoList,
                               dbo.GetTableIDStr(CtrlNoList,'Controller') AS CtrlIDList,
                               CASE CtrlNoList WHEN '' THEN 'NotData' ELSE 'Data' END AS 'NoListData',
                               CASE dbo.GetTableIDStr(CtrlNoList,'Controller') WHEN '' THEN 'NotData' ELSE 'Data' END AS 'IDListData',
                               (LEN(CtrlNoList) - LEN(REPLACE(CtrlNoList,',',''))) / LEN(',') AS 'NoListCount',
                               (LEN(dbo.GetTableIDStr(CtrlNoList,'Controller')) - LEN(REPLACE(dbo.GetTableIDStr(CtrlNoList,'Controller'),',',''))) / LEN(',') AS 'IDListCount'
                               FROM dbo.B00_SyncTable_EquGroup  
                              ) AS Base LEFT JOIN B00_SyncTable_EquGroup AS SyncTable_EquGroup ON SyncTable_EquGroup.EquGrpNo = Base.EquGrpNo
                              WHERE Base.NoListData <> Base.IDListData OR Base.NoListCount <> base.IDListCount ";
                    oAcsDB.GetDataTable("ErrorEquGroupData", sSql, out ErrorEquGroupData);
                    objRet.message += "控制器編號不存在：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region 檢查設備編號是否存在
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM 
                          (
                           SELECT EquGrpNo, EquNoList,
                           dbo.GetTableIDStr(EquNoList,'Equ') AS EquIDList,
                           CASE EquNoList WHEN '' THEN 'NotData' ELSE 'Data' END AS 'NoListData',
                           CASE dbo.GetTableIDStr(EquNoList,'Equ') WHEN '' THEN 'NotData' ELSE 'Data' END AS 'IDListData',
                           (LEN(EquNoList) - LEN(REPLACE(EquNoList,',',''))) / LEN(',') AS 'NoListCount',
                           (LEN(dbo.GetTableIDStr(EquNoList,'Equ')) - LEN(REPLACE(dbo.GetTableIDStr(EquNoList,'Equ'),',',''))) / LEN(',') AS 'IDListCount'
                           FROM dbo.B00_SyncTable_EquGroup  
                          ) AS Base WHERE Base.NoListData <> Base.IDListData OR Base.NoListCount <> Base.IDListCount ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT SyncTable_EquGroup.* FROM 
                              (
                               SELECT EquGrpNo, EquNoList,
                               dbo.GetTableIDStr(EquNoList,'Equ') AS EquIDList,
                               CASE EquNoList WHEN '' THEN 'NotData' ELSE 'Data' END AS 'NoListData',
                               CASE dbo.GetTableIDStr(EquNoList,'Equ') WHEN '' THEN 'NotData' ELSE 'Data' END AS 'IDListData',
                               (LEN(EquNoList) - LEN(REPLACE(EquNoList,',',''))) / LEN(',') AS 'NoListCount',
                               (LEN(dbo.GetTableIDStr(EquNoList,'Equ')) - LEN(REPLACE(dbo.GetTableIDStr(EquNoList,'Equ'),',',''))) / LEN(',') AS 'IDListCount'
                               FROM dbo.B00_SyncTable_EquGroup  
                              ) AS Base LEFT JOIN B00_SyncTable_EquGroup AS SyncTable_EquGroup ON SyncTable_EquGroup.EquGrpNo = Base.EquGrpNo
                              WHERE Base.NoListData <> Base.IDListData OR Base.NoListCount <> base.IDListCount ";
                    oAcsDB.GetDataTable("ErrorEquGroupData", sSql, out ErrorEquGroupData);
                    objRet.message += "設備編號不存在：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            if (ErrorCount != 0)
            {
                for (int i = 0; i < ErrorEquGroupData.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += ";";
                    objRet.message += @"設備群組編號：" + ErrorEquGroupData.Rows[i]["EquGrpNo"].ToString() + "\n"
                                    + @"設備群組名稱：" + ErrorEquGroupData.Rows[i]["EquGrpName"].ToString() + "\n"
                                    + @"控制器編號列表：" + ErrorEquGroupData.Rows[i]["CtrlNoList"].ToString() + "\n"
                                    + @"設備編號列表：" + ErrorEquGroupData.Rows[i]["EquNoList"].ToString() + "\n";
                    if (i + 1 < ErrorEquGroupData.Rows.Count)
                        objRet.message += "------------------------------" + "\n";
                }

                objRet.message = "設備群組資料關聯有誤編號：\n" + objRet.message;
                objRet.message = objRet.message + @"-----====================================================================-----";
                objRet.result = false;
                objRet.act = "ShowEquGroupError";
            }

            return objRet;
        }
        #endregion

        #region 檢查EquGroup資料表並設定Label
        private void CheckEquGroupSyncTable()
        {
            EquGroupCorrectFlag = CheckEquGroupCorrect(out EquGroupTableCount);
            Label_EquGroupState.ForeColor = Color.White;
            if (EquGroupTableCount == 0)
            {
                Label_EquGroupState.Text = "同步資料表為空！";
                Label_EquGroupState.Style.Add("cursor", "Default;");
                Label_EquGroupState.Attributes.Remove("onClick");
            }
            else if (EquGroupCorrectFlag && EquGroupTableCount != 0)
            {
                Label_EquGroupState.Text = "資料正確。共" + EquGroupTableCount + "筆資料。";
                Label_EquGroupState.Style.Add("cursor", "Default;");
                Label_EquGroupState.Attributes.Remove("onClick");
            }
            else
            {
                Label_EquGroupState.Text = "資料關聯出現問題！(點擊查看)";
                Label_EquGroupState.ForeColor = Color.Red;
                Label_EquGroupState.Style.Add("cursor", "pointer;");
                Label_EquGroupState.Attributes["onClick"] = "ShowEquGroupErrorData(); ScrollBottom();";
            }

            EquGroupState_UpdatePanel.Update();
        }
        #endregion

        #endregion

        #region 組織單位

        #region 確認Org暫存資料表的正確性
        private bool CheckOrgCorrect(out int ReturnRowCount)
        {
            bool IsSuccess = true;
            string sSql = "";
            int ErrorCount = 0, RowCount = 0;

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            #region 取得暫存資料表資料筆數
            sSql = "SELECT COUNT(*) FROM B00_SyncTable_OrgData";
            RowCount = oAcsDB.GetIntScalar(sSql);
            #endregion

            #region 檢查必填欄位是否填寫
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_OrgData AS SyncTable_OrgData
                          WHERE OrgClass = '' OR OrgNo = '' ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region 檢查Org是否已存在系統中
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM (
                        SELECT COUNT(*) OrgCnt,OrgNo FROM (
                        SELECT OrgNo,OrgName FROM B00_SyncTable_OrgData
                        UNION
                        SELECT OrgNo,OrgName FROM B01_OrgData
                        ) AS R1 GROUP BY OrgNo HAVING COUNT(*)>1
                        UNION
                        SELECT COUNT(*) OrgCnt,OrgName FROM (
                        SELECT OrgNo,OrgName FROM B00_SyncTable_OrgData
                        UNION
                        SELECT OrgNo,OrgName FROM B01_OrgData
                        ) AS R1 GROUP BY OrgName HAVING COUNT(*)>1) AS R2 ";

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

        #region 確認OrgStruc暫存資料表的正確性
        private bool CheckOrgStrucCorrect(out int ReturnRowCount)
        {
            bool IsSuccess = true;
            string sSql = "";
            int ErrorCount = 0, RowCount = 0;

            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            #region 取得暫存資料表資料筆數
            sSql = " SELECT COUNT(*) FROM B00_SyncTable_OrgStruc ";
            RowCount = oAcsDB.GetIntScalar(sSql);
            #endregion

            #region 檢查必填欄位是否填寫
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_OrgStruc AS SyncTable_OrgStruc WHERE Company = '' ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
            }
            #endregion

            #region 確認OrgStruc中所設定的單元是否在於OrgData中
            
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM 
                            (
	                            SELECT * FROM B00_SyncTable_OrgStruc AS SyncTable_OrgStruc
	                            LEFT JOIN B01_OrgData AS ComOrg ON ComOrg.OrgName = SyncTable_OrgStruc.Company
	                            WHERE ComOrg.OrgNo IS NULL AND (SyncTable_OrgStruc.Unit <> NULL OR SyncTable_OrgStruc.Company <> '')
	                            UNION 
	                            SELECT * FROM B00_SyncTable_OrgStruc AS SyncTable_OrgStruc
	                            LEFT JOIN B01_OrgData AS UnitOrg ON UnitOrg.OrgName = SyncTable_OrgStruc.Unit
	                            WHERE UnitOrg.OrgNo IS NULL AND (SyncTable_OrgStruc.Unit <> NULL OR SyncTable_OrgStruc.Unit <> '')
	                            UNION 
	                            SELECT * FROM B00_SyncTable_OrgStruc AS SyncTable_OrgStruc
	                            LEFT JOIN B01_OrgData AS DepOrg ON DepOrg.OrgName = SyncTable_OrgStruc.Department
	                            WHERE DepOrg.OrgNo IS NULL AND (SyncTable_OrgStruc.Department <> NULL OR SyncTable_OrgStruc.Department <> '')
	                            UNION
	                            SELECT * FROM B00_SyncTable_OrgStruc AS SyncTable_OrgStruc
	                            LEFT JOIN B01_OrgData AS TitleOrg ON TitleOrg.OrgName = SyncTable_OrgStruc.Title
	                            WHERE TitleOrg.OrgNo IS NULL AND (SyncTable_OrgStruc.Title <> NULL OR SyncTable_OrgStruc.Title <> '')
                            ) AS Base ";
                ErrorCount = oAcsDB.GetIntScalar(sSql);
            }
            
            #endregion

            
            if (ErrorCount == 0)
            {
                sSql = @"SELECT COUNT(*) FROM (
                              SELECT 
                              LEFT(BaseTable.OrgStrucNo,LEN(BaseTable.OrgStrucNo)-1) AS OrgStrucNo, OrgStruc, CreateUserID, CreateTime,Company,Unit,Department,Title
                              FROM (
	                                SELECT 
	                                ISNULL ( ( SELECT CAST(OrgNo AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Company) + '.','') +
	                                ISNULL ( ( SELECT CAST(OrgNo AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Unit) + '.','') +
	                                ISNULL ( ( SELECT CAST(OrgNo AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Department) + '.','') +
	                                ISNULL ( ( SELECT CAST(OrgNo AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Title) + '.','') AS OrgStrucNo,
	                                ISNULL ( '\' + ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Company) + '\','') +
	                                ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Unit) + '\','') +
	                                ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Department) + '\','') +
	                                ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Title) + '\','') AS OrgStruc
                                    ,Company,Unit,Department,Title
	                                ,'saho' AS CreateUserID,GETDATE() AS CreateTime FROM dbo.B00_SyncTable_OrgStruc
                              ) AS BaseTable ) AS R1 WHERE OrgStrucNo IN (SELECT OrgStrucNo FROM B01_OrgStruc)";
                int errs = oAcsDB.GetIntScalar(sSql);
                ErrorCount += errs;
            }       

            #region 檢查設備群組是否存在
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM 
                          (
                           SELECT *,
                           dbo.GetTableIDStr(EquGrpNoList,'EquGroup') AS EquGroupIDList,
                           CASE EquGrpNoList WHEN '' THEN 'NotData' ELSE 'Data' END AS 'NoListData',
                           CASE dbo.GetTableIDStr(EquGrpNoList,'EquGroup') WHEN '' THEN 'NotData' ELSE 'Data' END AS 'IDListData',
                           (LEN(EquGrpNoList) - LEN(REPLACE(EquGrpNoList,',',''))) / LEN(',') AS 'NoListCount',
                           (LEN(dbo.GetTableIDStr(EquGrpNoList,'EquGroup')) - LEN(REPLACE(dbo.GetTableIDStr(EquGrpNoList,'EquGroup'),',',''))) / LEN(',') AS 'IDListCount'
                           FROM dbo.B00_SyncTable_OrgStruc  
                          ) AS Base LEFT JOIN B00_SyncTable_OrgStruc AS SyncTable_OrgStruc ON SyncTable_OrgStruc.Company = Base.Company
                            AND SyncTable_OrgStruc.Unit = Base.Unit AND SyncTable_OrgStruc.Department = Base.Department AND SyncTable_OrgStruc.Unit = Base.Unit
                            WHERE Base.NoListData <> Base.IDListData OR Base.NoListCount <> base.IDListCount "; 

                ErrorCount = oAcsDB.GetIntScalar(sSql);
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

        #region 顯示Org錯誤資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object ShowOrgErrorData()
        {
            string sSql = "";
            int ErrorCount = 0;
            DataTable ErrorOrgData = null;
            Pub.MessageObject objRet = new Pub.MessageObject();
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

            #region 檢查必填欄位是否填寫
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_OrgData AS SyncTable_OrgData
                          WHERE OrgClass = '' OR OrgNo = '' ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_OrgData AS SyncTable_OrgData
                              WHERE OrgClass = '' OR OrgNo = '' ";
                    oAcsDB.GetDataTable("ErrorOrgData", sSql, out ErrorOrgData);
                    objRet.message += "必填欄位請填寫：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region 檢查Org是否已存在系統中
            if (ErrorCount == 0)
            {
                sSql = @"SELECT COUNT(*) FROM (
                        SELECT COUNT(*) OrgCnt,OrgNo FROM (
                        SELECT OrgNo,OrgName FROM B00_SyncTable_OrgData
                        UNION
                        SELECT OrgNo,OrgName FROM B01_OrgData
                        ) AS R1 GROUP BY OrgNo HAVING COUNT(*)>1
                        UNION
                        SELECT COUNT(*) OrgCnt,OrgName FROM (
                        SELECT OrgNo,OrgName FROM B00_SyncTable_OrgData
                        UNION
                        SELECT OrgNo,OrgName FROM B01_OrgData
                        ) AS R1 GROUP BY OrgName HAVING COUNT(*)>1) AS R2";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @"SELECT COUNT(*) OrgCnt,OrgNo FROM (
                        SELECT OrgNo,OrgName FROM B00_SyncTable_OrgData
                        UNION
                        SELECT OrgNo,OrgName FROM B01_OrgData
                        ) AS R1 GROUP BY OrgNo HAVING COUNT(*)>1
                        UNION
                        SELECT COUNT(*) OrgCnt,OrgName FROM (
                        SELECT OrgNo,OrgName FROM B00_SyncTable_OrgData
                        UNION
                        SELECT OrgNo,OrgName FROM B01_OrgData
                        ) AS R1 GROUP BY OrgName HAVING COUNT(*)>1";
                    oAcsDB.GetDataTable("ErrorOrgData", sSql, out ErrorOrgData);
                    objRet.message += "資料已存在於系統中或重複使用：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            if (ErrorCount != 0)
            {
                for (int i = 0; i < ErrorOrgData.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += ";";
                    objRet.message += @"組織資料：" + ErrorOrgData.Rows[i]["OrgNo"].ToString() + "\n";                                    
                    if (i + 1 < ErrorOrgData.Rows.Count)
                        objRet.message += "------------------------------" + "\n";
                }

                objRet.message = "組織資料關聯有誤編號：\n" + objRet.message;
                objRet.message = objRet.message + @"-----====================================================================-----";
                objRet.result = false;
                objRet.act = "ShowOrgError";
            }

            return objRet;
        }
        #endregion

        #region 顯示OrgStruc錯誤資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object ShowOrgStrucErrorData()
        {
            string sSql = "";
            int ErrorCount = 0;
            DataTable ErrorOrgStrucData = null;
            Pub.MessageObject objRet = new Pub.MessageObject();
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));

            #region 檢查必填欄位是否填寫
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(*) FROM B00_SyncTable_OrgStruc AS SyncTable_OrgStruc
                          WHERE Company = '' ";

                ErrorCount += oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM B00_SyncTable_OrgStruc AS SyncTable_OrgStruc
                              WHERE Company = '' ";
                    oAcsDB.GetDataTable("ErrorOrgStrucData", sSql, out ErrorOrgStrucData);
                    objRet.message += "必填欄位請填寫：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion

            #region 確認OrgStruc中所設定的單元是否在於OrgData中
            if (ErrorCount == 0)
            {
                
                sSql = @" SELECT COUNT(*) FROM 
                            (
	                            SELECT * FROM B00_SyncTable_OrgStruc AS SyncTable_OrgStruc
	                            LEFT JOIN B01_OrgData AS ComOrg ON ComOrg.OrgName = SyncTable_OrgStruc.Company
	                            WHERE ComOrg.OrgNo IS NULL AND (SyncTable_OrgStruc.Unit <> NULL OR SyncTable_OrgStruc.Company <> '')
	                            UNION 
	                            SELECT * FROM B00_SyncTable_OrgStruc AS SyncTable_OrgStruc
	                            LEFT JOIN B01_OrgData AS UnitOrg ON UnitOrg.OrgName = SyncTable_OrgStruc.Unit
	                            WHERE UnitOrg.OrgNo IS NULL AND (SyncTable_OrgStruc.Unit <> NULL OR SyncTable_OrgStruc.Unit <> '')
	                            UNION 
	                            SELECT * FROM B00_SyncTable_OrgStruc AS SyncTable_OrgStruc
	                            LEFT JOIN B01_OrgData AS DepOrg ON DepOrg.OrgName = SyncTable_OrgStruc.Department
	                            WHERE DepOrg.OrgNo IS NULL AND (SyncTable_OrgStruc.Department <> NULL OR SyncTable_OrgStruc.Department <> '')
	                            UNION
	                            SELECT * FROM B00_SyncTable_OrgStruc AS SyncTable_OrgStruc
	                            LEFT JOIN B01_OrgData AS TitleOrg ON TitleOrg.OrgName = SyncTable_OrgStruc.Title
	                            WHERE TitleOrg.OrgNo IS NULL AND (SyncTable_OrgStruc.Title <> NULL OR SyncTable_OrgStruc.Title <> '')
                            ) AS Base ";

                ErrorCount = oAcsDB.GetIntScalar(sSql);
                 
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT * FROM 
                              (
	                            SELECT * FROM B00_SyncTable_OrgStruc AS SyncTable_OrgStruc
	                            LEFT JOIN B01_OrgData AS ComOrg ON ComOrg.OrgName = SyncTable_OrgStruc.Company
	                            WHERE ComOrg.OrgNo IS NULL AND (SyncTable_OrgStruc.Unit <> NULL OR SyncTable_OrgStruc.Company <> '')
	                            UNION 
	                            SELECT * FROM B00_SyncTable_OrgStruc AS SyncTable_OrgStruc
	                            LEFT JOIN B01_OrgData AS UnitOrg ON UnitOrg.OrgName = SyncTable_OrgStruc.Unit
	                            WHERE UnitOrg.OrgNo IS NULL AND (SyncTable_OrgStruc.Unit <> NULL OR SyncTable_OrgStruc.Unit <> '')
	                            UNION 
	                            SELECT * FROM B00_SyncTable_OrgStruc AS SyncTable_OrgStruc
	                            LEFT JOIN B01_OrgData AS DepOrg ON DepOrg.OrgName = SyncTable_OrgStruc.Department
	                            WHERE DepOrg.OrgNo IS NULL AND (SyncTable_OrgStruc.Department <> NULL OR SyncTable_OrgStruc.Department <> '')
	                            UNION
	                            SELECT * FROM B00_SyncTable_OrgStruc AS SyncTable_OrgStruc
	                            LEFT JOIN B01_OrgData AS TitleOrg ON TitleOrg.OrgName = SyncTable_OrgStruc.Title
	                            WHERE TitleOrg.OrgNo IS NULL AND (SyncTable_OrgStruc.Title <> NULL OR SyncTable_OrgStruc.Title <> '')
                              ) AS Base ";
                    oAcsDB.GetDataTable("ErrorOrgStrucData", sSql, out ErrorOrgStrucData);
                    objRet.message += "組織架構找不到對應的組織單位：\n";
                    objRet.message += "-----====================================================================-----\n";
                }               
            }
            #endregion


            #region 增加組織架構重複建立的驗證機制            
            if (ErrorCount == 0)
            {
                sSql = @"SELECT * FROM (
                              SELECT 
                              LEFT(BaseTable.OrgStrucNo,LEN(BaseTable.OrgStrucNo)-1) AS OrgStrucNo, OrgStruc, CreateUserID, CreateTime,Company,Unit,Department,Title,EquGrpNoList
                              FROM (
	                                SELECT 
	                                ISNULL ( ( SELECT CAST(OrgNo AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Company) + '.','') +
	                                ISNULL ( ( SELECT CAST(OrgNo AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Unit) + '.','') +
	                                ISNULL ( ( SELECT CAST(OrgNo AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Department) + '.','') +
	                                ISNULL ( ( SELECT CAST(OrgNo AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Title) + '.','') AS OrgStrucNo,
	                                ISNULL ( '\' + ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Company) + '\','') +
	                                ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Unit) + '\','') +
	                                ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Department) + '\','') +
	                                ISNULL ( ( SELECT CAST(OrgID AS VARCHAR(10)) FROM B01_OrgData AS OrgDataByCode WHERE OrgDataByCode.OrgName = Title) + '\','') AS OrgStruc
                                    ,Company,Unit,Department,Title,EquGrpNoList
	                                ,'saho' AS CreateUserID,GETDATE() AS CreateTime FROM dbo.B00_SyncTable_OrgStruc
                              ) AS BaseTable ) AS R1 WHERE OrgStrucNo IN (SELECT OrgStrucNo FROM B01_OrgStruc)";
                oAcsDB.GetDataTable("ErrorOrgStrucData", sSql, out ErrorOrgStrucData);
                ErrorCount += ErrorOrgStrucData.Rows.Count;
                if (ErrorCount > 0)
                {
                    objRet.message += "組織架構重複建立：\n";
                    objRet.message += "-----====================================================================-----\n";

                }                                
            }
            #endregion

            #region 檢查設備群組是否存在
            if (ErrorCount == 0)
            {
                sSql = @" SELECT COUNT(SyncTable_OrgStruc.*) FROM 
                          (
                           SELECT *,
                           dbo.GetTableIDStr(EquGrpNoList,'EquGroup') AS EquGroupIDList,
                           CASE EquGrpNoList WHEN '' THEN 'NotData' ELSE 'Data' END AS 'NoListData',
                           CASE dbo.GetTableIDStr(EquGrpNoList,'EquGroup') WHEN '' THEN 'NotData' ELSE 'Data' END AS 'IDListData',
                           (LEN(EquGrpNoList) - LEN(REPLACE(EquGrpNoList,',',''))) / LEN(',') AS 'NoListCount',
                           (LEN(dbo.GetTableIDStr(EquGrpNoList,'EquGroup')) - LEN(REPLACE(dbo.GetTableIDStr(EquGrpNoList,'EquGroup'),',',''))) / LEN(',') AS 'IDListCount'
                           FROM dbo.B00_SyncTable_OrgStruc  
                          ) AS Base LEFT JOIN B00_SyncTable_OrgStruc AS SyncTable_OrgStruc ON SyncTable_OrgStruc.Company = Base.Company
                            AND SyncTable_OrgStruc.Unit = Base.Unit AND SyncTable_OrgStruc.Department = Base.Department AND SyncTable_OrgStruc.Unit = Base.Unit
                            WHERE Base.NoListData <> Base.IDListData OR Base.NoListCount <> base.IDListCount ";

                ErrorCount = oAcsDB.GetIntScalar(sSql);
                if (ErrorCount != 0)
                {
                    sSql = @" SELECT SyncTable_OrgStruc.* FROM 
                              (
                               SELECT *,
                               dbo.GetTableIDStr(EquGrpNoList,'EquGroup') AS EquGroupIDList,
                               CASE EquGrpNoList WHEN '' THEN 'NotData' ELSE 'Data' END AS 'NoListData',
                               CASE dbo.GetTableIDStr(EquGrpNoList,'EquGroup') WHEN '' THEN 'NotData' ELSE 'Data' END AS 'IDListData',
                               (LEN(EquGrpNoList) - LEN(REPLACE(EquGrpNoList,',',''))) / LEN(',') AS 'NoListCount',
                               (LEN(dbo.GetTableIDStr(EquGrpNoList,'EquGroup')) - LEN(REPLACE(dbo.GetTableIDStr(EquGrpNoList,'EquGroup'),',',''))) / LEN(',') AS 'IDListCount'
                               FROM dbo.B00_SyncTable_OrgStruc  
                              ) AS Base LEFT JOIN B00_SyncTable_OrgStruc AS SyncTable_OrgStruc ON SyncTable_OrgStruc.Company = Base.Company
                                AND SyncTable_OrgStruc.Unit = Base.Unit AND SyncTable_OrgStruc.Department = Base.Department AND SyncTable_OrgStruc.Unit = Base.Unit
                                WHERE Base.NoListData <> Base.IDListData OR Base.NoListCount <> base.IDListCount ";
                    oAcsDB.GetDataTable("ErrorOrgStrucData", sSql, out ErrorOrgStrucData);
                    objRet.message += "設備群組不存在：\n";
                    objRet.message += "-----====================================================================-----\n";
                }
            }
            #endregion


            if (ErrorCount != 0)
            {
                for (int i = 0; i < ErrorOrgStrucData.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += ";";
                    objRet.message += @"公司：" + ErrorOrgStrucData.Rows[i]["Company"].ToString() + "\n"
                                    + @"單位：" + ErrorOrgStrucData.Rows[i]["Unit"].ToString() + "\n"
                                    + @"部門：" + ErrorOrgStrucData.Rows[i]["Department"].ToString() + "\n"
                                    + @"職稱：" + ErrorOrgStrucData.Rows[i]["Title"].ToString() + "\n"
                                    + @"設備群組清單：" + ErrorOrgStrucData.Rows[i]["EquGrpNoList"].ToString() + "\n";
                    if (i + 1 < ErrorOrgStrucData.Rows.Count)
                        objRet.message += "------------------------------" + "\n";
                }

                //objRet.message = "組織架構關聯有誤編號：\n" + objRet.message;
                //objRet.message = objRet.message + @"-----====================================================================-----";
                objRet.result = false;
                objRet.act = "ShowOrgStrucError";
            }

            return objRet;
        }
        #endregion

        #region 檢查Org、OrgStruc資料表並設定Label
        private void CheckOrgSyncTable()
        {
            OrgCorrectFlag = CheckOrgCorrect(out OrgTableCount);
            Label_OrgState.ForeColor = Color.White;
            Label_OrgStrucState.ForeColor = Color.White;
            if (OrgTableCount == 0)
            {
                Label_OrgState.Text = "同步資料表為空！";
                Label_OrgState.Style.Add("cursor", "Default;");
                Label_OrgState.Attributes.Remove("onClick");
            }
            else if (OrgCorrectFlag && OrgTableCount != 0)
            {
                Label_OrgState.Text = "資料正確。共" + OrgTableCount + "筆資料。";
                Label_OrgState.Style.Add("cursor", "Default;");
                Label_OrgState.Attributes.Remove("onClick");
            }
            else
            {
                Label_OrgState.Text = "資料關聯出現問題！(點擊查看)";
                Label_OrgState.ForeColor = Color.Red;
                Label_OrgState.Style.Add("cursor", "pointer;");
                Label_OrgState.Attributes["onClick"] = "ShowOrgErrorData(); ScrollBottom();";
            }

            OrgStrucCorrectFlag = CheckOrgStrucCorrect(out OrgStrucTableCount);

            if (OrgStrucTableCount == 0)
            {
                Label_OrgStrucState.Text = "同步資料表為空！";
                Label_OrgStrucState.Style.Add("cursor", "Default;");
                Label_OrgStrucState.Attributes.Remove("onClick");
            }
            else if (OrgStrucCorrectFlag && OrgStrucTableCount != 0)
            {
                Label_OrgStrucState.Text = "資料正確。共" + OrgStrucTableCount + "筆資料。";
                Label_OrgStrucState.Style.Add("cursor", "Default;");
                Label_OrgStrucState.Attributes.Remove("onClick");
            }
            else
            {
                Label_OrgStrucState.Text = "資料關聯出現問題！(點擊查看)";
                Label_OrgStrucState.ForeColor = Color.Red;
                Label_OrgStrucState.Style.Add("cursor", "pointer;");
                Label_OrgStrucState.Attributes["onClick"] = "ShowOrgStrucErrorData(); ScrollBottom();";
            }

            OrgState_UpdatePanel.Update();
            OrgStrucState_UpdatePanel.Update();
        }
        #endregion

        #endregion

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
                    objRet.message += "必填欄位有誤：\n";
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
                    objRet.message += "必填欄位有誤：\n";
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