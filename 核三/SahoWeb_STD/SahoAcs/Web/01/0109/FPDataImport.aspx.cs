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
using SahoAcs.DBModel;

namespace SahoAcs
{
    public partial class FPDataImport : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        static bool PsnCardCorrectFlag = false;
        static int PsnTableCount = 0;
        string SysMessage = "";
        string MessageList = "";

        #endregion


        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            
            //oScriptManager.RegisterAsyncPostBackControl(this.FPData_ImportButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("FPDataImport", "FPDataImport.js");//加入同一頁面所需的JavaScript檔案

            #region 註冊主頁Button動作
            //SaveButton.Attributes["onClick"] = "SaveExcute();return false;";
            #endregion

            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion

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
                    TextBox_EquMsg.Text += ErrorNoStr;
                    #endregion
                }                
                else if (sFormArg == "PsnCard_UpdateState")
                {

                }

                #endregion
            }
        }
        #endregion


        #region 匯入事件


        protected void FPData_ImportButton_Click(object sender, EventArgs e)
        {
            bool IsSuccess = false;
            string sSql = "", sParaStr = "";
            int SQLComResult = 0, linecount = 0;
            DateTime ToDay = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            HashSet<string[]> PsnCard_set = new HashSet<string[]>();
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            if (this.FileUpload_FPData.HasFile && FileUpload_FPData.PostedFile.FileName == "FP.Data")
            {
                //IsSuccess = SetHastSetValueFormXlsx(FileUpload_FPData, "人員編號", out PsnCard_set);
                IsSuccess = GetFPDataImport();
            }

            this.TextBox_EquMsg.Text += this.SysMessage;
            Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('" + this.SysMessage + "');");

        }

        #endregion


        #region 資料匯入處理
        private bool GetFPDataImport()
        {
            string rep_message = "";

            try
            {
                string uploadname = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".Data";
                string real_uploadname = Server.MapPath("~/Img/Maps/" + uploadname);
                //Request.Files[0].SaveAs(real_uploadname);
                this.FileUpload_FPData.SaveAs(real_uploadname);
                OrmDataObject odo = new OrmDataObject("Sqlite", string.Format("Data Source={0};Version=3;", real_uploadname));
                using (OrmDataObject odoSms = new OrmDataObject("MsSql", Pub.GetDapperConnString()))
                {
                    var inserts = odo.GetQueryResult<FPDataObj>("SELECT * FROM FPData");

                    var result2 = odoSms.GetQueryResult<B02FPData>("SELECT * FROM B02_FPData").ToList();

                    //未匯入指紋檔的資料
                    //var inserts = result1.Where(i => !result2.Select(o => o.CardNo).Contains(i.CardNo)).ToList();

                    var cards = odoSms.GetQueryResult("SELECT * FROM B01_Card");

                    foreach(var o in inserts)
                    {
                        if (cards.Where(i =>Convert.ToString(i.CardNo) == o.CardNo).Count() == 0)
                        {
                            this.SysMessage += "卡號資料查無卡號==>"+o.CardNo+" \n";
                        }
                    }

                    foreach (var o in cards)
                    {
                        if (inserts.Where(i => i.CardNo == Convert.ToString(o.CardNo)).Count() > 0)
                        {
                            string FPData = "";
                            string FPDataAll = "";
                            foreach (var data in inserts.Where(i => i.CardNo == Convert.ToString(o.CardNo)).ToList())
                            {
                                FPData = "";
                                for (int i = 0; i < data.FPData.Length; i += 2)
                                {
                                    if (FPData != "")
                                        FPData += "-";
                                    FPData += data.FPData.Substring(i, 2);
                                }
                                if (FPDataAll != "")
                                    FPDataAll += "/";
                                FPDataAll += FPData;
                            }
                            string FPScore = string.Join("/", inserts.Where(i => i.CardNo == Convert.ToString(o.CardNo)).Select(i => i.FPQuality));
                            int amounts = inserts.Where(i => i.CardNo == Convert.ToString(o.CardNo)).Count();
                            string CardNo = Convert.ToString(o.CardNo);

                            //判斷新增或是修改指紋檔
                            if (result2.Where(i => i.CardNo == CardNo).Count() == 0)
                            {
                                odoSms.Execute(@"INSERT INTO B02_FPData (CardNo,FPAmount,FPData,FPSCore,FPTemplateType) 
                                                                VALUES (@CardNo,@FPAmount,@FPData,@FPScore,'2001')", new { CardNo = CardNo, FPAmount = amounts, FPData = FPDataAll, FPScore = FPScore });
                            }
                            else
                            {
                                odoSms.Execute(@"UPDATE B02_FPData SET FPAmount=@FPAmount,FPData=@FPData,FPScore=@FPScore WHERE CardNo=@CardNo",
                                    new { CardNo = CardNo, FPAmount = amounts, FPData = FPDataAll, FPScore = FPScore });
                            }
                            this.TextBox_EquMsg.Text += string.Format("處理卡號{0}....完成",CardNo) + "\n";
                            //針對該卡號的指紋機重新設碼
                            odoSms.Execute(@"UPDATE B01_CardAuth 
                                            SET OpMode='Reset',M_CardPW='',UpdateUserID=@UserID,UpdateTime=GETDATE(),ErrCnt = 0 
                                            WHERE CardNo=@CardNo AND OpMode<>'Del' ", new { CardNo = CardNo, UserID = Session["UserID"] });
                        }
                    }
                }
                System.IO.File.Delete(real_uploadname);
                this.SysMessage += "資料匯入完成!!";
            }
            catch (Exception ex)
            {
                this.SysMessage = ex.Message;
            }

            return true;
        }

        #endregion



    }//end class
}//end namespace