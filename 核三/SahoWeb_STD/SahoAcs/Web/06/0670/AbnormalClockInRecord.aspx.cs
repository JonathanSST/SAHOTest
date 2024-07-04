using DapperDataObjectLib;
using PagedList;
using SahoAcs.DBClass;
using SahoAcs.DBModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using SahoAcs.DBClass;




namespace SahoAcs.Web._06._0670
{
    public partial class AbnormalClockInRecord : SahoAcs.DBClass.BasePage
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<B03WorkAbnormal> ListLog = new List<B03WorkAbnormal>();
        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
        public DataTable DataResult = new DataTable();
        public List<PersonEntity> PersonList = new List<PersonEntity>();
        public string txt_CardNo_PsnName = "";
        public string SortName = "WorkDate";
        public string SortType = "ASC";
        public List<string> EquDatas = new List<string>();

        public int PageIndex = 1;
        public string PsnNo = "";
        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string PsnID = "";
        public string AuthList = "";
        public IPagedList<B03WorkAbnormal> PagedList;
        public string ServerUrl = "";
        public string LoginAuthId = "";
        public string SmtpName = "";
        public string SmtpAccount = "";
        public string SmtpPwd = "";
        public string SmtpPort = "";
        public string MailFrom = "";
        public string UseSsl = "";

        protected override void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
                this.SetQueryData();
                this.SetDoPage();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "SendMail")
            {
                //this.SetQueryData();
                this.SendMail();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "QueryPerson")
            {
                this.SetQueryPerson();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "ModifyAbnormal")
            {
                this.SetModifyAbnormal();
            }
            else if (this.GetFormEqlValue("PageEvent").Equals("Save"))
            {
                try
                {
                    XmlDocument xd = new XmlDocument();                    
                    var page = System.Web.HttpContext.Current;
                    xd.Load(System.Web.HttpContext.Current.Server.MapPath("~/SysPara.xml"));
                    XElement doc = XElement.Parse(xd.OuterXml);
                    doc.SetElementValue("Smtp", Convert.ToString(this.GetFormEqlValue("Smtp")));
                    doc.SetElementValue("SmtpAccount", Convert.ToString(this.GetFormEqlValue("SmtpAccount")));
                    doc.SetElementValue("SmtpPwd", Convert.ToString(this.GetFormEqlValue("SmtpPwd")));
                    doc.SetElementValue("MailFrom", Convert.ToString(this.GetFormEqlValue("MailFrom")));                    
                    doc.Save(System.Web.HttpContext.Current.Server.MapPath("~/SysPara.xml"));
                }
                catch (Exception ex)
                {

                }
                
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                        new { message = "寄件資訊更新完成"}));
                Response.End();
            }
            else
            {
                this.SetInitData();
                if (Request.UrlReferrer != null && (Request.UrlReferrer.PathAndQuery.Contains("Default.aspx") || Request.UrlReferrer.PathAndQuery.Equals("/")))
                {
                    this.SetQueryData();
                }
            }
            ClientScript.RegisterClientScriptInclude("jsfun", "AbnormalClockInRecord.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScrip
        }

        private void SetModifyAbnormal()
        {
            var Records = this.Request.Form.GetValues("RecordID");
            var AbnormalList = this.Request.Form.GetValues("AbnormalDesc");
            if (Records.Length == AbnormalList.Length)
            {
                int count = 0;
                foreach(var o in AbnormalList)
                {
                    this.odo.Execute("UPDATE B03_WorkAbnormal SET AbnormalDesc=@Desc WHERE RecordID=@ID", new { Desc = o, ID=Records[count] });
                    count++;
                }
            }
            Response.Clear();
            Response.Write("更新完成!!");
            Response.End();
        }


        private void SetInitData()
        {
            #region 設定電子郵件參數

            this.SmtpName = WebAppService.GetSysParaData("Smtp");
            this.SmtpPort = WebAppService.GetSysParaData("SmtpPort");
            this.SmtpPwd = WebAppService.GetSysParaData("SmtpPwd");
            this.SmtpAccount = WebAppService.GetSysParaData("SmtpAccount");
            this.MailFrom = WebAppService.GetSysParaData("MailFrom");
            this.UseSsl = WebAppService.GetSysParaData("UseSsl");
            #endregion


            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            this.PersonList = this.odo.GetQueryResult<PersonEntity>(@"
                                                        select A.* from B01_Person A
                                                        INNER JOIN B01_MgnOrgStrucs B ON A.OrgStrucID=B.OrgStrucID
                                                        INNER JOIN B00_ManageArea C ON B.MgaID=C.MgaID
                                                        INNER JOIN B00_SysUserMgns D ON C.MgaID=D.MgaID
                                                        WHERE D.UserID=@UserID", new { UserID = this.hideUserID.Value }).OrderBy(i => i.PsnName).ToList();
            #endregion

            #region 給開始、結束時間預設值
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            DateTime dtLastCardTime = this.odo.GetDateTimeScalar(@"SELECT ISNULL(MAX(WorkDate),GETDATE()) FROM B03_WorkAbnormal WHERE WorkDate <= @ZoneTime", new { ZoneTime = this.GetZoneTime().ToString("yyyy/MM/dd") });
            this.PsnNo = Sa.Web.Fun.GetSessionStr(this, "PsnNo");
            if (dtLastCardTime >= this.GetZoneTime())
            {
                this.CardDayS.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd");
                this.CardDayE.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd");
            }
            else
            {
                this.CardDayS.DateValue = dtLastCardTime.GetZoneTime(this).ToString("yyyy/MM/dd");
                this.CardDayE.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd");
            }
            #endregion

            this.PagedList = this.ListLog.ToPagedList(PageIndex, 1);
        }

        private void SetQueryPerson()
        {
            this.PagedList = this.ListLog.ToPagedList(PageIndex, 1);

            if (Request["PsnName"] != null && !string.IsNullOrEmpty(Request["PsnName"]))
            {
                string sqlcmd = "SELECT TOP 200 * FROM B01_Person WHERE (PsnNo LIKE @Key or PsnName Like @Key) ORDER BY PsnNo";
                this.PersonList = this.odo.GetQueryResult<PersonEntity>(sqlcmd, new { Key = Request["PsnName"] + "%" }).ToList();
            }
            else
            {
                this.PersonList = this.odo.GetQueryResult<PersonEntity>("SELECT TOP 200 * FROM B01_Person WHERE PsnNo LIKE '%' ORDER BY PsnNo").ToList();
            }

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
            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }
            List<ColNameObj> collist = new List<ColNameObj>();
            bool boolSort = this.SortType.Equals("ASC");

            StringBuilder sql = new StringBuilder();
            string DateS = (Request["CardDateS"] ?? this.CardDayS.DateValue) + " 00:00:00";
            string DateE = (Request["CardDateE"] ?? this.CardDayE.DateValue) + " 23:59:59";
            string PsnName = Request["PsnName"] ?? "";
            string IsSend = Request["IsSend"] ?? "0";
            string PsnNo = this.GetFormEqlValue("PsnName").Trim();
           

            sql.Append(
                @"SELECT 
                        A.RecordID,A.WorkDate,A.PsnNo,A.ClassNo,A.WorkTimeS,A.WorkTimeE,A.RealTimeS,A.RealTimeE,
                        A.WorkTimeO,A.WorkTimeI,A.RestTimeO,A.RestTimeI,A.Delay,A.StealTime,A.OverTime,A.StatuDesc,A.AbnormalDesc2,
                        A.AbnormalDesc,A.IsSend,A.CreateUserID,A.CreateTime,A.UpdateUserID,A.UpdateTime ,B.PsnName,B.OrgStrucID
                        FROM B03_WorkAbnormal A 
                        INNER JOIN B01_Person B On A.PsnNo = B.PsnNo
                        WHERE 1=1 ");

            if (!string.IsNullOrEmpty(DateS))
            {
                sql.Append(" and A.WorkDate >= Convert(Varchar(10),@DateS, 111) ");
            }
            else
            {
                sql.Append(" and A.WorkDate >= convert(varchar(10),Getdate(), 111) ");
            }

            if (!string.IsNullOrEmpty(DateE))
            {
                sql.Append(" and A.WorkDate <= Convert(varchar(10),@DateE, 111) ");
            }
            else
            {
                sql.Append(" and A.WorkDate <= convert(varchar(10),Getdate(), 111) ");
            }

            if (!string.IsNullOrEmpty(PsnName))
            {
                sql.Append(" AND (B.PsnNo LIKE @PsnName OR B.PsnName LIKE @PsnName) ");
            }

            sql.Append(" And ISNULL(A.IsSend,0) = @IsSend ");            
            sql.Append(" ORDER BY A.PsnNo,A.WorkDate ");
            this.ListLog = this.odo.GetQueryResult<B03WorkAbnormal>(sql.ToString(), new
            {
                DateS = DateS,
                DateE = DateE,
                PsnName = PsnName + "%",
                PsnNo = PsnNo,
                IsSend = IsSend
            }).OrderByField("WorkDate", false).ToList();

            string _RealTimeS = string.Empty;
            string _RealTimeE = string.Empty;
            DateTime _WorkDate = DateTime.Now;
            string OrgNameList = string.Empty;

            string orgsql = @" Select
                distinct B.OrgNameList,B.OrgStrucID
                From B01_Person A
                Inner Join OrgStrucAllData('') B ON A.OrgStrucID = B.OrgStrucID ";
            DataTable orgs = this.odo.GetDataTableBySql(orgsql);

            foreach (var d in this.ListLog)
            {
                if (!string.IsNullOrEmpty(d.OrgStrucID))
                {
                    if (orgs.Rows.Count != 0)
                    {
                        DataRow[] rows = orgs.Select("OrgStrucID='" + d.OrgStrucID + "' ");
                        if (rows.Length != 0)
                        {
                            OrgNameList = rows[0]["OrgNameList"].ToString();
                            if (!string.IsNullOrEmpty(OrgNameList))
                            {
                                string[] Org = OrgNameList.Split('\\');
                                foreach (var o in Org)
                                {
                                    if (!string.IsNullOrEmpty(o))
                                    {
                                        d.OrgStrucName = o;
                                    }
                                }
                            }
                        }
                    }
                }
                //if (!string.IsNullOrEmpty(d.RealTimeS))
                //{
                //    _RealTimeS = d.RealTimeS.ToString();
                //}

                //if (!string.IsNullOrEmpty(d.RealTimeE))
                //{
                //    _RealTimeE = d.RealTimeE.ToString();
                //}

                //if (!string.IsNullOrEmpty(_RealTimeS) && !string.IsNullOrEmpty(_RealTimeE))
                //{
                //    DateTime dt1 = Convert.ToDateTime(_RealTimeS);
                //    DateTime dt2 = Convert.ToDateTime(_RealTimeE);
                //    if (DateTime.Compare(dt1, dt2) > 0)
                //    {
                //        _WorkDate = Convert.ToDateTime(d.WorkDate).AddDays(-1);
                //        d.WorkDate = _WorkDate.ToString("yyyy/MM/dd");
                //    }
                //}
            }

            if (Request["PageEvent"] == "Print")
            {
                this.DataResult = OrmDataObject.IEnumerableToTable(this.ListLog);
            }
            else
            {
                this.PagedList = this.ListLog.ToPagedList(PageIndex, 100);
            }
        }

        private void SendMail()
        {
            string Message = "";
            var result = true;
            if (Request["PersonList"] != null)
            {
                string[] Persons = Request["PersonList"].Split(',');

                if (Persons != null && Persons.Length > 0)
                {
                    foreach (string P in Persons)
                    {
                        if (!string.IsNullOrEmpty(P))
                        {
                            //XmlDocument xd = new XmlDocument();
                            //string path = Request.PhysicalPath;
                            //xd.Load(Request.Url.Scheme + "://" + Request.Url.Authority + "/SysPara.xml");
                            //XElement doc = XElement.Parse(xd.OuterXml);
                            //path = doc.Element("MobileApi").Value;
                            this.SmtpName = WebAppService.GetSysParaData("Smtp");
                            this.SmtpPort = WebAppService.GetSysParaData("SmtpPort");
                            this.SmtpPwd = WebAppService.GetSysParaData("SmtpPwd");
                            this.SmtpAccount = WebAppService.GetSysParaData("SmtpAccount");
                            this.MailFrom = WebAppService.GetSysParaData("MailFrom");
                            string sql = @"Select 
                                              A.PsnNo,
                                              B.psnName,               --個人姓名
                                              A.WorkDate,
                                              A.RealTimeS,
                                              A.RealTimeE,
                                              B.Personal               --個人Email
                                             ,B.Supervisor            --主管Email
                                             ,A.AbnormalDesc,ISNULL(A.AbnormalDesc2,'') AS AbnormalDesc2
                                              From B03_WorkAbnormal A 
                                              Inner join B01_Person B On A.PsnNo = B.PsnNo 
                                              --Left Join B01_Person C On B.Supervisor = C.Personal
                                            Where A.RecordID = @RecordID ";
                            var OneResult = this.odo.GetQueryResult<B03WorkAbnormal>(sql, new { RecordID = P });

                            foreach (var m in OneResult)
                            {
                                if (!string.IsNullOrEmpty(m.Personal))
                                {
                                    string mailcontent = string.Format("{0} 先生、小姐 您好：<br/><br/>", m.PsnName);
                                    System.Net.Mail.MailMessage MyMail = new System.Net.Mail.MailMessage();
                                    MyMail.From = new System.Net.Mail.MailAddress(MailFrom);
                                    MyMail.To.Add(m.Personal); //設定收件者Email
                                    if (!string.IsNullOrEmpty(m.Supervisor))
                                    {
                                        string[] SupervisorEmail = m.Supervisor.Split(',');
                                        if (SupervisorEmail.Length != 0)
                                        {
                                            foreach (var s in SupervisorEmail)
                                            {
                                                if (!string.IsNullOrEmpty(s))
                                                {
                                                    MyMail.CC.Add(s.ToString().Trim());
                                                }
                                            }
                                        }
                                    }
                                    MyMail.Subject = "這是從SMS系統發送的刷卡異常通知信";
                                    this.mailBody(MyMail, m.PsnName ,m.WorkDate, m.AbnormalDesc+"<br/>"+ m.AbnormalDesc2);
                                    MyMail.IsBodyHtml = true; //是否使用html格式
                                    MyMail.Priority = MailPriority.High;
                                    AlternateView plainView = AlternateView.CreateAlternateViewFromString(mailcontent, null, "text/plain");
                                    System.Net.Mail.SmtpClient MySMTP = new System.Net.Mail.SmtpClient(this.SmtpName);
                                    MySMTP.Credentials = new System.Net.NetworkCredential(this.SmtpAccount, this.SmtpPwd);
                                    if (WebAppService.GetSysParaData("UseSsl").Equals("1"))
                                    {
                                        MySMTP.EnableSsl = true;
                                    }
                                    if (!string.IsNullOrEmpty(this.SmtpPort))
                                    {
                                        MySMTP.Port = int.Parse(this.SmtpPort);
                                    }                                    
                                    try
                                    {
                                        MySMTP.Send(MyMail);
                                        MyMail.Dispose(); //釋放資源
                                        string update =
                                         @" Update B03_WorkAbnormal 
                                             Set IsSend=1,
                                             UpdateTime=getdate(), 
                                             UpdateUserID = @UpdateUserID
                                             Where RecordID=@RecordID";
                                        this.odo.Execute(update, new
                                        {
                                            RecordID = P,
                                            UpdateUserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID")
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        Response.Clear();
                                        Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                                                new { message = "傳送郵件發生錯誤==>"+ex.Message, error = ex.Message, max_mobile = DongleVaries.GetMaxMobile(), current_mobile = DongleVaries.GetCurrentMobile() }));
                                        Response.End();
                                    }
                                }
                                else
                                {
                                    Response.Clear();
                                    Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                                        new { message = "請確認 " + m.PsnName + " E-Mail是否已設定。", success = false }));
                                    Response.End();
                                }
                            }
                        }
                    }
                    Response.Clear();
                    Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                        new { message = "OK", success = result }));
                    Response.End();
                }
            }
        }

        public void mailBody(MailMessage mail, string MainPsn,string WorkDate,string Desc)
        {
            string palinBody = "【刷卡異常通知信】";
            AlternateView plainView = AlternateView.CreateAlternateViewFromString(
                     palinBody, null, "text/plain");
            string htmlBody = string.Format("{0} 先生、小姐 您好：<br/>", MainPsn);
            htmlBody += " 於 " + WorkDate + " 因 " + Desc + "<br/>";
            htmlBody += " 故特此來信通知。 ";

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            // add the views
            mail.AlternateViews.Add(plainView);
            mail.AlternateViews.Add(htmlView);
        }
    }
}