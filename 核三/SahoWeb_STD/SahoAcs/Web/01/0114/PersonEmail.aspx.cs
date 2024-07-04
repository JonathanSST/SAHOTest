using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using SahoAcs.DBClass;
using System.Data;
using PagedList;
using OfficeOpenXml;
using ZXing;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.Linq;


namespace SahoAcs
{
    public partial class PersonEmail : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        //public List<OracleTemp> ListLog = new List<OracleTemp>();
        public List<PersonEntity> PsnDatas = new List<PersonEntity>();
        public List<ColNameObj> ListCols = new List<ColNameObj>();
        //public List<LogState> LogStatus = new List<LogState>();
        public DataTable DataResult = new DataTable();

        public string txt_CardNo_PsnName = "";

        public string SortName = "CardTime";
        public string SortType = "ASC";

        public List<string> EquDatas = new List<string>();

        public int PageIndex = 1;

        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string PsnID = "";

        public string Base64Image = "";
        public string ServerUrl = "";
        public string LoginAuthId = "";
        public string SmtpName = "";
        public string SmtpAccount = "";
        public string SmtpPwd = "";
        public string SmtpPort = "";
        LinkedResource imageResource;

        public IPagedList<PersonEntity> PagedList;

        protected void Page_Load(object sender, EventArgs e)
        {            
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
                this.SetInitData();
                this.SetQueryData();
                this.SetDoPage();
            }
            else if(Request["PageEvent"]!=null && Request["PageEvent"] == "Email")
            {
                //this.SetOneCardLog();
                this.SendMail();
            }            
            else
            {
                this.SetInitData();
                //this.PsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");
                //this.EmptyCondition();
            }
            if (!IsPostBack)
            {
               
            }
            ClientScript.RegisterClientScriptInclude("0114", "PersonEmail.js?"+DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }

        /// <summary>預設查詢內容</summary>
        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            
            #region 設定欄位寬度、名稱內容預設
            this.ListCols.Add(new ColNameObj() { ColName = "PsnNo", DataWidth = 104, TitleWidth = 100, TitleName = "2道入廠時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "DepName", DataWidth = 104, TitleWidth = 100, TitleName = "2道出廠時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "PsnName", DataWidth = 104, TitleWidth = 100, TitleName = "1道入廠時間" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardNo", DataWidth = 104, TitleWidth = 100, TitleName = "人員ID" });
            this.ListCols.Add(new ColNameObj() { ColName = "CardTime", DataWidth = 124, TitleWidth = 120, TitleName = "姓名" });
            this.ListCols.Add(new ColNameObj() { ColName = "EquName", DataWidth = 124, TitleWidth = 120, TitleName = "廠商名稱" });
            this.ListCols.Add(new ColNameObj() { ColName = "EquNo", DataWidth = 84, TitleWidth = 80, TitleName = "廠商編號" });
            this.ListCols.Add(new ColNameObj() { ColName = "LogStatus", DataWidth = 124, TitleWidth = 120, TitleName = "工程編號" });
            foreach (var o in this.ListCols.Where(i => i.DataRealName == null || i.DataRealName == ""))
                o.DataRealName = o.ColName;

            #endregion            
        }

        private void SendMail()
        {
            XmlDocument xd = new XmlDocument();
            string path = Request.PhysicalPath;
            xd.Load(Request.Url.Scheme + "://" + Request.Url.Authority + "/SysPara.xml");
            XElement doc = XElement.Parse(xd.OuterXml);
            path = doc.Element("MobileApi").Value;
            this.SmtpName = doc.Element("Smtp").Value;
            this.SmtpPort = doc.Element("SmtpPort").Value;
            this.SmtpPwd = doc.Element("SmtpPwd").Value;
            this.SmtpAccount = doc.Element("SmtpAccount").Value;
            this.ServerUrl = path;
            this.LoginAuthId = Guid.NewGuid().ToString();
            string comp_name = "商合行專用請勿外流";
            string current_path = System.AppDomain.CurrentDomain.BaseDirectory;
            string[] aFiles = Directory.GetFiles(current_path, "*.sms", SearchOption.TopDirectoryOnly);
            if (aFiles.Length > 0)
            {
                string[] aStr;
                var sTmp = File.ReadAllText(aFiles[0]);
                if (sTmp != "")
                {
                    sTmp = Sa.Fun.Decrypt(sTmp);
                    if (!string.IsNullOrEmpty(sTmp))
                    {
                        aStr = sTmp.Split(new char[] { '|' });
                        if (aStr.Length > 1)
                        {
                            comp_name = aStr[1];
                        }
                    }
                }
            }
            string MyQrCodeStrContent = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new { server_url = this.ServerUrl, login_auth_id = this.LoginAuthId, comp_name=comp_name, comp_no=Sa.Fun.Encrypt(comp_name) });
            BarcodeWriter write = new BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new ZXing.QrCode.QrCodeEncodingOptions
                {
                    //產生出圖片的高度
                    Height = 180,
                    //產生出圖片的寬度
                    Width = 180,
                    //文字是使用哪種編碼方式
                    CharacterSet = "UTF-8",
                    ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.H
                }
            };
            System.Drawing.Bitmap bitmap = write.Write(MyQrCodeStrContent);
            System.IO.MemoryStream m = new System.IO.MemoryStream();
            bitmap.Save(m, System.Drawing.Imaging.ImageFormat.Png);
            byte[] byteImage = m.ToArray();
            this.Base64Image = Convert.ToBase64String(byteImage);
            if (this.imageResource != null)
            {
                this.imageResource.Dispose();
            }
            var filePath = Server.MapPath("\\Img\\QrCode.png");
            try
            {
                using (System.IO.FileStream file = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    m.WriteTo(file);
                    file.Close();
                    m.Close();
                }
                this.SetSendEmai();
            }
            catch (IOException ex)
            {

                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                        new { message = "傳送郵件發生錯誤", error = ex.Message }));
                Response.End();
            }
            

            //完成處理訊息
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                    new { message = "二維條碼郵件傳送完成", max_mobile = DongleVaries.GetMaxMobile(), current_mobile = DongleVaries.GetCurrentMobile() }));
            Response.End();

        }

        /// <summary>設定主資料表的分頁</summary>
        private void SetDoPage()
        {
            //啟始及結束頁
            StartPage = (PageIndex < ShowPage) ? 1 : PageIndex;
            if (StartPage > 1)
            {
                StartPage = (PageIndex + ShowPage / 2 >= this.PagedList.PageCount) ? this.PagedList.PageCount - ShowPage + 1 : PageIndex - ShowPage / 2;
            }
            EndPage = (StartPage - 1 > this.PagedList.PageCount - ShowPage) ? this.PagedList.PageCount + 1 : StartPage + ShowPage;
            //上下頁
            PrePage = PageIndex - 1 <= 1 ? 1 : PageIndex - 1;
            NextPage = PageIndex + 1 >= this.PagedList.PageCount ? this.PagedList.PageCount : PageIndex + 1;
        }


        private void SetQueryData()
        {
            this.txt_CardNo_PsnName = Request["PsnName"];
            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }
            List<ColNameObj> collist = new List<ColNameObj>();
            bool boolSort = this.SortType.Equals("ASC");         
            string sql = "";            
            sql = @"SELECT DISTINCT A.*,Text1 AS PsnEName,ISNULL(B.PsnDesc,'0') AS EmpNo  FROM B01_Person A 
                        LEFT JOIN LoginAuthTable B ON CONVERT(VARCHAR,A.PsnID)=B.PsnDesc  
                        WHERE 
                        (PsnNo LIKE @PsnName OR PsnName LIKE @PsnName) AND
                        Text1 LIKE '%_@__%.__%'  AND PsnAuthAllow=1 ORDER BY PsnNo";
            // 
            this.PsnDatas = this.odo.GetQueryResult<PersonEntity>(sql, new
            {                
                PsnName = this.txt_CardNo_PsnName+"%"
            }).ToList();

            this.PagedList = this.PsnDatas.ToPagedList(PageIndex, 100);
        }


        private string GetConditionCmdStr()
        {
            string sql = "";                       
            //一般查詢的方法            
            if (this.txt_CardNo_PsnName != "")
            {
                sql += " WHERE  (PsnNo LIKE @PsnName OR PsnName LIKE @PsnName) ";
            }            
            return sql;
        }             

        private void EmptyCondition()
        {
            for(int i = 0; i < 1; i++)
            {
                
            }
            //轉datatable
            this.PagedList = this.PsnDatas.ToPagedList(1, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable(this.PagedList);            
        }


        void SetSendEmai()
        {
            //取得收件者帳號資訊

            var OneResult = this.odo.GetQueryResult("SELECT * FROM B01_Person WHERE PsnID=@PsnID", new { PsnID = Request["PsnID"] });
            string StrMailTo = "", PsnName = "";
            foreach (var m in OneResult)
            {
                this.odo.Execute("INSERT INTO LoginAuthTable (LoginAuthID,PsnDesc,ExpireTime) VALUES (@LoginAuthID,@PsnDesc,@ExpireTime)", 
                    new { LoginAuthID = LoginAuthId, ExpireTime = DateTime.Now.AddDays(1), PsnDesc = m.PsnID });
                StrMailTo = Convert.ToString(m.Text1);
                PsnName = Convert.ToString(m.PsnName);
            }
            if (StrMailTo != "")
            {
                string mailcontent = string.Format("{0} 先生、小姐 您好：<br/><br/>", "Sam");
                mailcontent += "請掃瞄附件條碼取得登入權限，<br/>本條碼掃描使用後將立即失效";
                mailcontent += "<img width=\"200\" height=\"200\" alt=\"\" hspace=0 src=\"cid:QrCode\" align=baseline border=0 />";
                System.Net.Mail.MailMessage MyMail = new System.Net.Mail.MailMessage();
                MyMail.From = new System.Net.Mail.MailAddress("csh0741@gmail.com");
                MyMail.To.Add(StrMailTo); //設定收件者Email
                MyMail.Subject = "Mobile App 2維條碼驗證通知";
                //MyMail.Body = mailcontent;
                this.mailBody(MyMail, PsnName);
                MyMail.IsBodyHtml = true; //是否使用html格式
                MyMail.Priority = MailPriority.High;                
                AlternateView plainView = AlternateView.CreateAlternateViewFromString(mailcontent, null, "text/plain");
                imgResource(plainView, "QrCode2.png", "image/jpg");
                System.Net.Mail.SmtpClient MySMTP = new System.Net.Mail.SmtpClient(this.SmtpName);
                MySMTP.Credentials = new System.Net.NetworkCredential(this.SmtpAccount, this.SmtpPwd);
                    //"lsjpewbefzywlgbj;
                MySMTP.EnableSsl = true;
                MySMTP.Port = int.Parse(this.SmtpPort) ;
                try
                {
                    MySMTP.Send(MyMail);
                    MyMail.Dispose(); //釋放資源
                    this.imageResource.Dispose();
                }
                catch (Exception ex)
                {                    
                    Response.Clear();
                    Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                            new { message = "傳送郵件發生錯誤", error = ex.Message,max_mobile=DongleVaries.GetMaxMobile(),current_mobile=DongleVaries.GetCurrentMobile() }));
                    Response.End();
                }
            }                        
        }


        public void mailBody(MailMessage mail, string MainPsn)
        {
            string palinBody = "【XXXX】";
            AlternateView plainView = AlternateView.CreateAlternateViewFromString(
                     palinBody, null, "text/plain");            
            string htmlBody = string.Format("{0} 先生、小姐 您好：<br/><br/>", MainPsn) + "<p> 請掃瞄附件條碼取得登入權限，本條碼掃描使用後將立即失效</p> ";
            htmlBody = "" + htmlBody;
            htmlBody += "<img alt=\"\" hspace=0 src=\"cid:QrCode\" align=baseline border=0  />";

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            imgResource(htmlView, "QrCode.png", "image/png");
            // add the views
            mail.AlternateViews.Add(plainView);
            mail.AlternateViews.Add(htmlView);
        }


        public void imgResource(AlternateView htmlView, string imgName, string imgType)
        {
            // create image resource from image path using LinkedResource class..   
            //LinkedResource imageResource = new LinkedResource(getImgPath("QrCode.png"), imgType);
            this.imageResource = new LinkedResource(getImgPath("QrCode2.png"), imgType);
            string[] imgArr = imgName.Split('.');
            imageResource.ContentId = imgArr[0];
            imageResource.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
            htmlView.LinkedResources.Add(imageResource);
            //imageResource.Dispose();
        }

        private string getImgPath(string strImgName)
        {
            //設定(絕對)圖片路徑
            string strImgPath = Server.MapPath("\\Img\\QrCode.png");
            return strImgPath;
        }


    }//end page class
}//end namespace