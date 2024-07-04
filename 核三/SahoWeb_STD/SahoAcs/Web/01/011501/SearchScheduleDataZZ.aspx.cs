using DapperDataObjectLib;
using PagedList;
using SahoAcs.DBModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web._01._011501
{
    public partial class SearchScheduleDataZZ : System.Web.UI.Page
    {
        #region Global block
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public string SortName = "CardTime";
        public string SortType = "ASC";

        public List<ScheduleTable> ScheduleDatas = new List<ScheduleTable>();
        public List<ColNameObj> ListCols = new List<ColNameObj>();
        #region 分頁參數
        public int PageIndex = 1;
        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string PsnID = "";

        public IPagedList<ScheduleTable> PagedList;
        #endregion End 分頁參數


        #endregion end Global block


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
                //按下查詢時 query 結果
                //this.SetInitData();
                this.SetQueryData();
                this.SetDoPage();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "DeleteChk" || Request["PageEvent"] == "EditChk")
            {
                //按下刪除鍵/修改鍵 先彈出確認畫面
                this.SetLoadData();
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Insert")
            {
                this.SetInsertData();
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Edit")
            {
                this.SetIEditData();
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Delete")
            {
                this.SetDelete();
            }
            if (!IsPostBack)
            {
              
            }


            ClientScript.RegisterClientScriptInclude("011501", "SearchScheduleDataZZ.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }// End Page_load



        private void SetQueryData()
        {
            //搜尋字眼
            string KeyWord = Request["PsnName"].ToString().Trim() ;
            string CurrentSysid = Session["UserID"].ToString();
            string CurYear = DateTime.Now.Date.Year.ToString();
            string CurMonth = Monthaddzero(DateTime.Now.Date.Month.ToString());
            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }
           //找出目前user 所管理的區域代碼
            List<string> OrgIDdatas = FinduserMgarea(CurrentSysid);
            string QueryPsnsql = "";
            bool boolSort = this.SortType.Equals("ASC");
            QueryPsnsql = @"select c.ScheduleID,c.employeeID,c.employeeName,c.SyncMark,c.VacationDate,c.VacationInfo,c.Year,c.Month,b.OrgName,a.OrgStrucID
                                                         from B01_Person a
                                                         inner join OrgStrucAllData('Unit') b on a.OrgStrucID=b.OrgStrucID
                                                         inner join B03_ScheduleTable c on c.employeeID=a.PsnNo ";
            if (KeyWord != "")
            {
                //舊的條件
                //QueryPsnsql += "where (a.OrgStrucID in @OrgIDList) and (c.employeeID LIKE @Qurerykey OR c.employeeName LIKE @Qurerykey)  and Month=@Month  and Year=@Year and  c.VacationDate > GETDATE() order by c.employeeID ASC,c.VacationDate ASC";
                QueryPsnsql += "where (a.OrgStrucID in @OrgIDList) and (c.employeeID LIKE @Qurerykey OR c.employeeName LIKE @Qurerykey)  AND  c.VacationDate > GETDATE() order by c.employeeID ASC,c.VacationDate ASC";
                this.ScheduleDatas = this.odo.GetQueryResult<ScheduleTable>(QueryPsnsql, new
                {
                    Year = CurYear,
                    Month = CurMonth,
                    OrgIDList = OrgIDdatas,
                    Qurerykey= KeyWord
                }).ToList();
            }
            else
            {
                QueryPsnsql += "where (a.OrgStrucID in @OrgIDList) AND c.VacationDate > GETDATE()  order by c.employeeID ASC,c.VacationDate ASC";
                this.ScheduleDatas = this.odo.GetQueryResult<ScheduleTable>(QueryPsnsql, new
                {
                    Year = CurYear,
                    Month = CurMonth,
                    OrgIDList = OrgIDdatas,
                }).ToList();
            }
            this.PagedList = this.ScheduleDatas.ToPagedList(PageIndex, 100);
        }

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

        private void SetLoadData()
        {
            //string selectsvalue = Request["VacationDate"].ToString().Trim();
            string[] Selects = Request["VacationDate"].Split('_');
            var Result = this.odo.GetQueryResult<ScheduleTable>("select * from B03_ScheduleTable where ScheduleID=@ScheduleID", new { ScheduleID = Selects[0] });
            if (Result.Count() > 0)
            {
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
               new { message = "OK", success = true, resp = Result, VacationDate= Selects[1], OrgName= Selects[2] }));
                Response.End();
            }
            else
            {
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = GetGlobalResourceObject("Resource", "NonData"), success = false, resp = Result }));
                Response.End();
            }
        

        }

        private void SetInsertData()
        {
            string CurrentSysid = Session["UserID"].ToString();
            string RequestTpPsnNo = Request["TpPsnNo"].ToString().Trim();
            string RequestTPPsnName = Request["TPPsnName"].ToString().Trim();
            string RequestVacationDate = Request["popDropDownList_Char2"].ToString().Trim();
            string RequestRemark = Request["Remark"];
            int ChkVacationDate = 0;
            string[] VacationDate = RequestVacationDate.Split('/');
            string ReturnMsg = "";
            string IsOK = "";
            string DebugMsg = "";
            string ChkpsnMsg = "";
           List<string> OrgIDlist =  FinduserMgarea(CurrentSysid);
            //判斷新增人員是否正確 與 該人員是否屬於目前系統使用者所管理的人員
            ChkpsnMsg = VerificationPsnData(RequestTpPsnNo, RequestTPPsnName, OrgIDlist);
            // 判斷是否有新增同一筆例假資料 防止同一人新增重複例假資料。
              ChkVacationDate = GetVacationDateResult(RequestTpPsnNo, RequestVacationDate);
            if (ChkpsnMsg == "")
            {
                if (ChkVacationDate == 0 && RequestVacationDate != "0")
                {
                    DateTime CurrentTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    this.odo.Execute(@"INSERT INTO B03_ScheduleTable
                                                        (employeeID,
                                                         employeeName,
                                                         CreateTime,
                                                         VacationDate,
                                                         VacationInfo,
                                                         SyncMark,
                                                          Year,
                                                          Month,
                                                          ChangeLog) VALUES 
                                                                                              (@employeeID,
                                                                                                @employeeName,
                                                                                                @CreateTime,
                                                                                                @VacationDate,
                                                                                                @VacationInfo,
                                                                                                @SyncMark,
                                                                                                @Year,
                                                                                                @Month,
                                                                                                @ChangeLog)"
                        , new
                        {
                            employeeID = RequestTpPsnNo,
                            employeeName = RequestTPPsnName,
                            CreateTime = CurrentTime,
                            VacationDate = RequestVacationDate,
                            VacationInfo = RequestRemark,
                            SyncMark = "0",
                            Year = VacationDate[0],
                            Month = Monthaddzero(VacationDate[1]),
                            ChangeLog = "系統使用者:[" + CurrentSysid + "],於時間:[" + CurrentTime + "],進行網頁系統人員例假表新增此筆資料",
                        });
                    odo.BeginTransaction();
                    try
                    {
                        odo.Commit();
                        IsOK = "OK";
                        DebugMsg = "NoError";
                        ReturnMsg = "例假資料新增成功";
                    }
                    catch (Exception ex)
                    {
                        IsOK = "NO";
                        odo.Rollback();
                        ReturnMsg = "員工姓名:" + RequestTPPsnName + "例假資料新增失敗" + "請檢查資料是否有誤或洽系統管理員";
                        DebugMsg = ex.ToString();
                        throw;
                    }
                }
                else
                {
                    IsOK = "NO";
                    if (RequestVacationDate == "0")
                    {
                        ReturnMsg = "請選擇日期";
                        DebugMsg = "日期數值不正確";
                    }
                    else
                    {
                        ReturnMsg = "員工姓名:" + RequestTPPsnName + ",已有[" + RequestVacationDate + "]例假資料,請勿重複新增";
                        DebugMsg = "重複資料";
                    }
                }// end else
            } // end if
            else
            {
                ReturnMsg = ChkpsnMsg;
                DebugMsg = "人員資料有誤";
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
               Message= IsOK,  resp = ReturnMsg , Debugmsg= DebugMsg,SelctValue= RequestTpPsnNo+ RequestVacationDate
            }));
            Response.End();
        }

        private void SetIEditData()
        {
            string[] Selects = Request["VacationDate"].Split('_');
            string CurrentSysid = Session["UserID"].ToString();
            string RequestTpPsnNo = Request["TpPsnNo"].ToString().Trim();
            string RequestOldDate = Request["OldDate"].ToString().Trim();
            string RequestTPPsnName = Request["TPPsnName"].ToString().Trim();
            string RequestVacationDate = Request["popDropDownList_Char2"].ToString().Trim();
            string RequestRemark = Request["Remark"];
            int ChkVacationDate = 0;
            string[] VacationDate = RequestVacationDate.Split('/');
            string ReturnMsg = "";
            string IsOK = "";
            string DebugMsg = "";
            string ChkpsnMsg = "";
            List<string> OrgIDlist = FinduserMgarea(CurrentSysid);
            //判斷新增人員是否正確 與 該人員是否屬於目前系統使用者所管理的人員
            ChkpsnMsg = VerificationPsnData(RequestTpPsnNo, RequestTPPsnName, OrgIDlist);
            //判斷是否修改同一筆例假資料
            if(RequestVacationDate != "0"&& Convert.ToDateTime(RequestOldDate) != Convert.ToDateTime(RequestVacationDate))
            {
                // 判斷是否有新增同一筆例假資料 防止同一人新增重複例假資料。
                ChkVacationDate = GetVacationDateResult(RequestTpPsnNo, RequestVacationDate);
            }
            else
            {
                if (RequestVacationDate == "0")
                {
                    Response.Clear();
                    Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
                    {
                        Message ="NO",
                        resp = "請選擇日期",
                        Debugmsg = "日期沒有選擇",
                    }));
                    Response.End();
                }
                else
                {
                    ChkVacationDate = 0;
                }
                
            }


            if (ChkpsnMsg == "")
            {
                if (ChkVacationDate == 0 && RequestVacationDate != "0")
                {
                    DateTime CurrentTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    this.odo.Execute(@"UPDATE B03_ScheduleTable 
                                                          SET VacationInfo =@VacationInfo,VacationDate=@VacationDate,SyncMark=@SyncMark,
                                                                   ChangeLog=@ChangeLog,CreateTime=@CreateTime                        
                                                          WHERE ScheduleID=@ScheduleID"
                        , new
                        {
                            VacationDate = RequestVacationDate,
                            VacationInfo = RequestRemark,
                            CreateTime= CurrentTime,
                            ChangeLog = "系統使用者:[" + CurrentSysid + "],於時間:[" + CurrentTime + "],進行網頁系統人員例假表修改此筆資料",
                            SyncMark = "0",
                            ScheduleID= Selects[0]
                        }) ;
                    odo.BeginTransaction();
                    try
                    {
                        odo.Commit();
                        IsOK = "OK";
                        DebugMsg = "NoError";
                        ReturnMsg = "例假資料修改成功";
                    }
                    catch (Exception ex)
                    {
                        IsOK = "NO";
                        odo.Rollback();
                        ReturnMsg = "員工姓名:" + RequestTPPsnName + "例假資料修改失敗" + "請檢查資料是否有誤或洽系統管理員";
                        DebugMsg = ex.ToString();
                        throw;
                    }
                }
                else
                {
                    IsOK = "NO";
                    if (RequestVacationDate == "0")
                    {
                        ReturnMsg = "請選擇日期";
                        DebugMsg = "日期數值不正確";
                    }
                    else
                    {
                        ReturnMsg = "員工姓名:" + RequestTPPsnName + ",已有[" + RequestVacationDate + "]例假資料,請勿重複新增,如需修改請先刪除該筆資料";
                        DebugMsg = "重複資料";
                    }
                }// end else
            } // end if
            else
            {
                ReturnMsg = ChkpsnMsg;
                DebugMsg = "人員資料有誤";
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                Message = IsOK,
                resp = ReturnMsg,
                Debugmsg = DebugMsg,
                SelctValue = RequestTpPsnNo + RequestVacationDate
            }));
            Response.End();

        }

        private void SetDelete()
        {
            string[] Selects = Request["VacationDate"].Split('_');
            Pub.MessageObject objRet = new Pub.MessageObject();
            this.odo.Execute("Delete B03_ScheduleTable where ScheduleID=@ScheduleID", new { ScheduleID = Selects[0]});
            objRet.result = this.odo.isSuccess;
            if (!this.odo.isSuccess)
                objRet.message = this.odo.DbExceptionMessage;
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                resp = objRet
            }));
            Response.End();
        }

        private int GetVacationDateResult(string requestTpPsnNo, string requestVacationDate)
        {
            string Sqlstr = @" SELECT * FROM B03_ScheduleTable WHERE employeeID=@employeeID AND VacationDate=@VacationDate";
            this.ScheduleDatas = this.odo.GetQueryResult<ScheduleTable>(Sqlstr, new
            {
                employeeID = requestTpPsnNo,
                VacationDate = requestVacationDate
            }).ToList();
            return ScheduleDatas.Count();
        }

        /// <summary>
        /// 月份補0 方便之後排序
        /// </summary>
        /// <param name="month">月份</param>
        /// <returns></returns>
        protected string Monthaddzero(string month)
        {
            string Currentmonth = "";
            if (Convert.ToInt32(month) < 10)
            {
                Currentmonth = "0" + month;
            }
            else
            {
                Currentmonth = month;
            }
            return Currentmonth;
        }

        /// <summary>
        /// 判斷新增人員是否正確 與 該人員是否屬於目前系統使用者所管理的人員
        /// </summary>
        /// <param name="TPpsnNo">工號</param>
        /// <param name="TPpsnName">員工姓名</param>
        /// <param name="OrgIDlist">目前系統使用者所管理的區域</param>
        /// <returns></returns>
        protected string VerificationPsnData(string TPpsnNo,string TPpsnName,List<string> OrgIDlist)
        {
            string VerificationMsg = "";
            //判斷新增人員是否正確 與 該人員是否屬於目前系統使用者所管理的人員
            var PsnData = this.odo.GetQueryResult("select * from B01_Person  where PsnNo =@PsnNo and PsnName =@PsnName and (OrgStrucID in @OrgList)",
                new
                {
                    PsnNo= TPpsnNo,
                    PsnName = TPpsnName,
                    OrgList= OrgIDlist
                });
            if (TPpsnNo == "" || TPpsnName == "")
            {
                VerificationMsg = "工號或姓名不得為空白";
            }
            else
            {
        
                if (PsnData.Count() == 0)
                {
                    VerificationMsg += "人員姓名:" + TPpsnName + "  ,工號:" + TPpsnNo + ",人員資料有誤 或 此人員不屬於目前系統使用者所管理人員";
                }
                else
                {
                    VerificationMsg = "";
                }
            }
            return VerificationMsg;
        } //end VerificationPsnData_method

        /// <summary>
        /// 找出目前系統使用者所管理區域
        /// </summary>
        /// <param name="CurrentSysid">目前系統使用者id</param>
        /// <returns></returns>
        protected List<string> FinduserMgarea(string CurrentSysid)
        {
            var OrgMgn = this.odo.GetQueryResult("select a.UserID,a.MgaID,c.OrgStrucID from B00_SysUserMgns a inner join B00_ManageArea b on a.MgaID = b.MgaID inner join B01_MgnOrgStrucs c on b.MgaID = c.MgaID");
            var Orgfind = OrgMgn.Where(x => x.UserID == CurrentSysid).ToList();
            List<string> OrgIDdatas = new List<string>();
            for (int i = 0; i < Orgfind.Count; i++)
            {
                OrgIDdatas.Add(Convert.ToString(Orgfind[i].OrgStrucID));    
            }
            return OrgIDdatas;
        }

    } // End page class 
} // End namespace