using DapperDataObjectLib;
using SahoAcs.DBClass;
using SahoAcs.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Configuration;


namespace SahoAcs.Web._06._0674
{
    public partial class WorkDataExport : System.Web.UI.Page
    {
        public OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public DataTable DataResult = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.RegisterClientScriptInclude("WorkDataExport", "WorkDataExport.js?" + Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案

            if (Request["PageEvent"] != null && Request["PageEvent"] == "Export")
            {
                //this.ExportData();
                this.SetDataExport();
            }
            else if (this.GetFormEqlValue("PageEvent").Equals("Calc"))
            {
                this.SetCalcWork();
            }
            else
            {
                SetInitData();
            }
        }


            private void SetInitData()
        {
            this.ExportDate.DateValue = this.GetZoneTime().AddDays(-1).ToString("yyyy/MM/dd");
            this.ExportDate2.DateValue = this.GetZoneTime().AddDays(-1).ToString("yyyy/MM/dd");
        }

        public static void SetLogs(string _exception)
        {
            string sFileName = string.Format("{0:yyyyMMdd}.txt", DateTime.Now);
            string sPath = System.AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "System_Log\\" + "DataExport_" + sFileName;
            StreamWriter oSw = null;
            try
            {
                oSw = File.AppendText(sPath);
                oSw.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "        " + _exception);
            }
            catch (Exception e)
            {
                throw e;
            }
            if (oSw != null)
                oSw.Dispose();
        }


        protected void ExportData()
        {
            string Message = "";
            string finger_paravalue = string.Empty;
            var result = true;

            string clerkno = string.Empty;

            try
            {
                string WorkDate = (Request["WorkDate"] ?? this.ExportDate.DateValue);
                string WorkDateE = this.GetFormEqlValue("WorkDateE") ?? this.ExportDate2.DateValue;
                if (WorkDateE.CompareTo(DateTime.Now.ToString("yyyy/MM/dd"))>0)
                {
                    Message = "匯出日期只能小於或等於當天日期!!";
                    return;
                }
                if (WorkDate.CompareTo(WorkDateE) > 0)
                {
                    Message = "起始日期不得大於結束日期";
                    return;
                }
                OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
                var paradata = odo.GetQueryResult("Select * From B00_SysParameter Where ParaNo='TargetDataBase' ");
                //var paradata = odo.GetQueryResult("Select * From B00_SysParameter Where ParaNo='TargetDataBase1' ");
                if (paradata.Count() != 0)
                {
                    foreach (var o in paradata)
                    {
                        finger_paravalue = Convert.ToString(o.ParaValue);
                    }
                }
                else
                {
                    SetLogs("寫入發生異常 : 查無finger資料庫參數資料");
                    Message = "查無finger資料庫參數資料!!";
                    return;
                }

                if (string.IsNullOrEmpty(finger_paravalue))
                {
                    SetLogs("寫入發生異常 : finger資料庫參數無資料");
                    Message = "finger資料庫參數無資料!!";
                    return;
                }

                List<tcanalysis> models = new List<tcanalysis>();
                string sql = @"Select distinct
                                            WorkDate,
                                            PsnNo As clerkno,
                                            (CONVERT(VARCHAR(3),CONVERT(VARCHAR(4),WorkDate,20) - 1911) + 
                                             SUBSTRING(CONVERT(VARCHAR(10),WorkDate,20),6,2) + 
                                             SUBSTRING(CONVERT(VARCHAR(10),WorkDate,20),9,2)) As sdate,
                                            REPLACE(WorkTimeS,':','') AS WorkTimeS,
                                             REPLACE(WorkTimeE,':','') AS WorkTimeE,
                                             '' As [source],
                                             ClassNo As [kind],
                                             REPLACE(RealTimeS,':','') AS RealTimeS,
                                             REPLACE(RealTimeE,':','') AS RealTimeE,
                                            [Delay],
                                            StealTime,
                                            OverTime
                                            From B03_WorkDetail
                                            where 1=1
                                            AND (WorkDate BETWEEN Convert(varchar(10),@WorkDate,111) AND Convert(varchar(10),@WorkDateE,111)) and ClassNo <>''
                                            --and PsnNo in ('690953');";

                var sourcedata = this.odo.GetQueryResult(sql, new
                {
                    WorkDate = WorkDate, WorkDateE=WorkDateE
                });
                if (sourcedata.Count() != 0)
                {
                    var ResultCardLog = this.odo.GetQueryResult(
                    @"Select distinct PsnNo,EquNo,EquDir,CtrlAddr,Convert(varchar(10),CardTime, 111) AS CardDate,
                                    REPLACE(CONVERT(VARCHAR(3),CONVERT(VARCHAR(4),CardTime,20) - 1911) + '/' +
                                   SUBSTRING(CONVERT(VARCHAR(10),CardTime,20),6,2) + '/' +
                                   SUBSTRING(CONVERT(VARCHAR(10),CardTime,20),9,2),'/','') AS CardTimeE
                                  From V_ShowCardLog
                                 where (Convert(varchar(10),CardTime, 111) BETWEEN @WorkDate AND @WorkDateE)  And EquDir in ('進','出') ", new { WorkDate = Convert.ToDateTime(WorkDate).AddDays(-1).ToString("yyyy/MM/dd"),
                        WorkDateE = Convert.ToDateTime(WorkDateE).AddDays(1).ToString("yyyy/MM/dd") }).ToList();
                    DataTable V_ShowCardLog = OrmDataObject.ToDataTable<dynamic>(ResultCardLog);
                    SetLogs("匯出資料開始執行，匯出日期 : " + WorkDate + " ，共有 : " + sourcedata.Count().ToString() + " 筆");

                    string go_to_work = string.Empty;   //上班
                    string go_off_work = string.Empty;  //下班
                    string go_to_status = string.Empty;
                    string go_off_status = string.Empty, CardTimeS = "", CardTimeE = "";
                    int delaytimes = 0;

                    foreach (var d in sourcedata)
                    {
                        go_to_work = "正常";
                        go_off_work = "正常";
                        go_to_status = "1";
                        go_off_status = "1";
                        delaytimes = 0;
                        CardTimeS = d.sdate;
                        CardTimeE = d.sdate;
                        if (string.IsNullOrEmpty(d.RealTimeS))
                        {
                            go_to_work = "未刷";                            
                        }
                        if (string.IsNullOrEmpty(d.RealTimeE))
                        {
                            go_off_work = "未刷";
                        }
                        if (d.Delay > 0)
                        {
                            go_to_status = "2";
                            go_to_work = "遲到";
                            delaytimes = d.Delay;
                        }
                        if (d.StealTime > 0)
                        {
                            go_off_status = "3";
                            go_off_work = "早退";
                        }
                        if (d.OverTime > 0)
                        {
                            go_off_work = "加班";
                        }
                        string Source = string.Empty;
                        string Source1 = string.Empty;
                        if (V_ShowCardLog.Rows.Count != 0)
                        {
                            DataRow[] rows = V_ShowCardLog.Select("PsnNo='" + d.clerkno + "' AND CardDate='"+Convert.ToString(d.WorkDate)+"' ");                            
                            if (rows.Length != 0 && !string.IsNullOrEmpty(d.RealTimeS))
                            {//上班卡鐘
                                Source = rows.OrderBy(r => r["CardTimeE"]).First()["EquNo"].ToString().Trim(); //rows[0]["EquNo"].ToString().Trim();                                
                            }
                            //如果RealTimeE 小於 RealTimeS ，換找前一日的最後一筆紀錄
                            if (!string.IsNullOrEmpty(d.WorkTimeS) && !string.IsNullOrEmpty(d.WorkTimeE))
                            {
                                string TimeS = Convert.ToString(d.WorkTimeS);
                                string TimeE = Convert.ToString(d.WorkTimeE);
                                if (TimeS.CompareTo(TimeE) > 0)
                                {
                                    rows = V_ShowCardLog.Select("PsnNo='" + d.clerkno + "' AND CardDate='" + Convert.ToDateTime(d.WorkDate).AddDays(1).ToString("yyyy/MM/dd") + "'");   //隔日的刷卡卡鐘
                                    CardTimeE = Convert.ToDateTime(d.WorkDate).AddDays(1).Year - 1911 + ""+ Convert.ToDateTime(d.WorkDate).AddDays(1).ToString("MMdd");    //隔日的刷卡日期
                                }
                            }                        
                            if (rows.Length != 0 && !string.IsNullOrEmpty(d.RealTimeE))
                            {//下班卡鐘
                                Source1 = rows.OrderBy(r => r["CardTimeE"]).First()["EquNo"].ToString().Trim(); //rows[0]["EquNo"].ToString().Trim();                                
                            }                           
                        }
                        if (d.kind == "0")
                        {
                            models.Add(new tcanalysis()
                            {
                                clerkno = d.clerkno,
                                sdate = d.sdate,
                                ctime = "",
                                source = "0",
                                kind = "0",
                                sourcename = "公休",
                                status = "",
                                statusname = "",
                                carddate = "",
                                cardtime = "",
                                timenbr = "0",
                                chkcode = "免刷",
                                cdatetime = "",
                                ndate = d.sdate,
                                hourtot = null,
                                absent = ""
                            });
                        }
                        else
                        {
                            models.Add(new tcanalysis()
                            {
                                clerkno = d.clerkno,
                                sdate = d.sdate,
                                ctime = d.WorkTimeS,
                                source = Source,
                                kind = "1",
                                sourcename = "上班",
                                status = go_to_status,
                                statusname = go_to_work,
                                carddate = string.IsNullOrEmpty(d.RealTimeS)?"":d.sdate,
                                cardtime = d.RealTimeS,
                                timenbr = delaytimes.ToString(),
                                chkcode = "應刷",
                                cdatetime = "",                                
                                ndate = d.sdate,
                                hourtot = null,
                                absent = ""
                            });
                            models.Add(new tcanalysis()
                            {
                                clerkno = d.clerkno,
                                sdate = CardTimeE,
                                ctime = d.WorkTimeE,
                                source = Source1,
                                kind = "2",
                                sourcename = "  下班",
                                status = go_off_status,
                                statusname = go_off_work,
                                carddate = string.IsNullOrEmpty(d.RealTimeE)?"":CardTimeE,
                                cardtime = d.RealTimeE,
                                timenbr = delaytimes.ToString(),
                                chkcode = "應刷",
                                cdatetime = "",
                                ndate = d.sdate,
                                hourtot = null,
                                absent = ""
                            });
                        }

                    }
                    models = models.Where(i => !i.statusname.Equals("未刷")).ToList();
                    PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(tcanalysis));
                    DataTable SourceTable = new DataTable();
                    foreach (PropertyDescriptor prop in properties)
                        SourceTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    foreach (tcanalysis item in models)
                    {
                        DataRow row = SourceTable.NewRow();
                        foreach (PropertyDescriptor prop in properties)
                            row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                        SourceTable.Rows.Add(row);
                    }

                    if (SourceTable.Rows.Count != 0)
                    {
                        SetLogs("整理過後資料有 " + SourceTable.Rows.Count + " 筆");

                        using (SqlConnection destinationConnection = new SqlConnection(finger_paravalue.ToString().Trim()))
                        {
                            destinationConnection.Open();

                            DateTime cdate = DateTime.Parse(WorkDate);
                            string sdate = Convert.ToInt16(cdate.AddYears(-1911).Year) + cdate.ToString("MMdd");
                            string edate = Convert.ToInt16(DateTime.Parse(WorkDateE).AddYears(-1911).Year) + DateTime.Parse(WorkDateE).ToString("MMdd");
                            OrmDataObject DestinationDO = new OrmDataObject("MsSql", finger_paravalue.ToString().Trim());
                            DestinationDO.Execute(@"Delete From tcanalysis Where ndate BETWEEN @sdate AND @edate", new { sdate = sdate, edate = edate });

                            #region 起始日前一天大夜班資料一併刪除
                            //取大夜班班表資料
                            var classdata = odo.GetQueryResult("Select * From B00_ClassData Where CNo='1' ");
                            string WBTime = string.Empty;
                            string WETime = string.Empty;
                            if (classdata.Count() != 0)
                            {
                                foreach (var o in classdata)
                                {
                                    WBTime = o.WBTime.Replace(":", "");
                                    WETime = o.WETime.Replace(":", "");
                                }
                            }
                            //起始日前一天
                            string _sdate = DateTime.Parse(WorkDate).AddDays(-1).AddYears(-1911).Year + DateTime.Parse(WorkDate).AddDays(-1).ToString("MMdd");
                            //Convert.ToInt16(DateTime.Parse(WorkDate).AddYears(-1911).Year) + DateTime.Parse(WorkDate).AddDays(-1).ToString("MMdd");
                            if (!string.IsNullOrEmpty(WBTime))
                            {
                                DestinationDO.Execute(@"Delete From tcanalysis Where sdate = @sdate And ctime=@WBTime and sourcename='上班'", new { sdate = _sdate, WBTime = WBTime });
                            }
                            if (!string.IsNullOrEmpty(WETime))
                            {
                                DestinationDO.Execute(@"Delete From tcanalysis Where sdate = @sdate And ctime=@WETime and sourcename='  下班'", new { sdate = sdate, WETime = WETime });
                            }
                            #endregion

                            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection))
                            {
                                bulkCopy.DestinationTableName = "tcanalysis";

                                try
                                {
                                    bulkCopy.WriteToServer(SourceTable);
                                }
                                catch (Exception ex)
                                {
                                    SetLogs("寫入發生異常 : " + ex.ToString());
                                    Message = ex.ToString();
                                    return;
                                }
                                finally
                                {
                                    destinationConnection.Close();
                                }
                            }
                        }
                    }

                    SetLogs("匯出完畢，已成功匯出 " + SourceTable.Rows.Count + " 筆");
                    Message = "成功匯出  " + SourceTable.Rows.Count.ToString() + "  筆數";
                }
                else
                {
                    Message = "查無資料可匯出!!";
                }
            }
            catch (Exception ex)
            {
                SetLogs("寫入發生異常 : " + clerkno + "    " + ex.ToString());
                Message = ex.ToString();
            }
            finally
            {
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                    new { message = Message, success = result }));
                Response.End();
            }
        }


        public void SetCalcWork()
        {
            string WorkDate = (Request["WorkDate"] ?? this.ExportDate.DateValue);
            string WorkDateE = this.GetFormEqlValue("WorkDateE") ?? this.ExportDate2.DateValue;
            string Message = "";
            bool result = false;
            if (Convert.ToDateTime(WorkDateE) > DateTime.Now)
            {
                Message = "匯出日期只能小於或等於當天日期!!";
                return;
            }
            if (WorkDate.CompareTo(WorkDateE) > 0)
            {
                Message = "起始日期不得大於結束日期";
                return;
            }
            try
            {
                CPCWorkDetail.CalcFunc func1 = new CPCWorkDetail.CalcFunc();
                //設定相關的參數
                func1.UseMode = "1";
                func1.WaitMin = int.Parse(ConfigurationManager.AppSettings["WaitMin"]);
                func1.OverMin = int.Parse(ConfigurationManager.AppSettings["OverMin"]);
                func1.CustomType = 1;
                //func.CalcRunMode = 0;
                //func.CalcRunCycle = 60;
                //func.CalcBeforeDay = 2;
                func1.SWTag = "ABCR";
                func1.SWTagList = "1,04:00,06:00;2,12:00,13:00;3,18:00,19:00;0,00:00,00:00";
                //func.CalcRunTimes = "";
                //func.CalcDateRange = "";
                func1.LogFileRoot = ConfigurationManager.AppSettings["LogFileRoot"];
                func1.LogKeepDay = 100;
                func1.DbUserID = ConfigurationManager.AppSettings["DbUserID"];
                func1.DbUserPW = ConfigurationManager.AppSettings["DbUserPW"];
                func1.DbDataBase = ConfigurationManager.AppSettings["DbDataBase"];
                func1.DbDataSource = ConfigurationManager.AppSettings["DbDataSource"];
                func1.Start();
                result = func1.CalcCPCWorkDetail(WorkDate, WorkDateE);
                func1.Stop();
                func1.Close();
                func1 = null;
            }
            catch (Exception ex)
            {
                result = false;
                Message = ex.Message;
            }
            finally
            {
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                    new { message = Message, success = result }));
                Response.End();
            }
            
            
        }


        public void SetDataExport()
        {            
            string Message = "";
            string finger_paravalue = string.Empty;
            var result = true;
            string clerkno = string.Empty;
            try
            {
                string WorkDate = (Request["WorkDate"] ?? this.ExportDate.DateValue);
                string WorkDateE = this.GetFormEqlValue("WorkDateE") ?? this.ExportDate2.DateValue;
                if (WorkDateE.CompareTo(DateTime.Now.ToString("yyyy/MM/dd")) > 0)
                {
                    Message = "匯出日期只能小於或等於當天日期!!";
                    return;
                }
                if (WorkDate.CompareTo(WorkDateE) > 0)
                {
                    Message = "起始日期不得大於結束日期";
                    return;
                }                
                var paradata = odo.GetQueryResult("Select * From B00_SysParameter Where ParaNo='TargetDataBase' ");               //var paradata = odo.GetQueryResult("Select * From B00_SysParameter Where ParaNo='TargetDataBase1' ");
                if (paradata.Count() != 0)
                {
                    foreach (var o in paradata)
                    {
                        finger_paravalue = Convert.ToString(o.ParaValue);
                    }
                }
                else
                {
                    SetLogs("寫入發生異常 : 查無finger資料庫參數資料");
                    Message = "查無finger資料庫參數資料!!";
                    return;
                }
                if (string.IsNullOrEmpty(finger_paravalue))
                {
                    SetLogs("寫入發生異常 : finger資料庫參數無資料");
                    Message = "finger資料庫參數無資料!!";
                    return;
                }
                var sourcedata = this.odo.GetQueryResult<tcanalysis>(this.MainCmdStr, new
                {
                    DateS = WorkDate, DateE = WorkDateE
                }).ToList();
                var sourcedata2 = this.odo.GetQueryResult<tcanalysis>(this.MainCmdStr, new
                {
                    DateS = Convert.ToDateTime(WorkDate).AddDays(-1).ToString("yyyy/MM/dd"),
                    DateE = Convert.ToDateTime(WorkDate).AddDays(-1).ToString("yyyy/MM/dd")
                }).ToList();
                sourcedata2 = sourcedata2.Where(i => i.ClassNo.Equals("1")).ToList();                
                if (sourcedata.Count() != 0)
                {
                    OrmDataObject OdoTarg = new OrmDataObject("MsSql", finger_paravalue);                    
                    OdoTarg.Execute("DELETE tcanalysis WHERE ndate BETWEEN @DateS AND @DateE",new {DateE = sourcedata.Max(i => i.sdate), DateS = sourcedata.Min(i=>i.sdate) });     //刪除ndate重複的紀錄
                    OdoTarg.Execute("DELETE tcanalysis WHERE ndate BETWEEN @DateS AND @DateE AND ctime BETWEEN '2230' AND '2300' AND kind='1' ", new { DateE = sourcedata2.Max(i => i.sdate), DateS = sourcedata2.Min(i => i.sdate) });     //刪除ndate重複的紀錄
                    OdoTarg.Execute("DELETE tcanalysis WHERE ndate BETWEEN @DateS AND @DateE AND ctime BETWEEN '0800' AND '0830' AND kind='2' ", new { DateE = sourcedata2.Max(i => i.sdate), DateS = sourcedata2.Min(i => i.sdate) });     //刪除ndate重複的紀錄
                    sourcedata = sourcedata.Union(sourcedata2).ToList();
                    this.SetSourceDataProc(ref sourcedata);                    
                    var count = OdoTarg.Execute(this.InsertCmdExt, sourcedata);    //執行資料匯出                    
                    Message = "完成資料匯出共" + count + "筆";
                    SetLogs(Message);
                    if (!OdoTarg.isSuccess)
                    {
                        Message = OdoTarg.DbExceptionMessage;
                        SetLogs("寫入發生異常:" + Message);
                    }
                }
                else
                {
                    Message = " 查無可匯出的差勤紀錄";
                    SetLogs(Message);
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                SetLogs("寫入發生異常 : " + ex.ToString());
            }
            finally
            {
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                    new { message = Message, success = result }));
                Response.End();
            }
        }


        /// <summary>執行原始資料判斷處理</summary>
        /// <param name="RefList">須進行處理的原始資料</param>
        public void SetSourceDataProc(ref List<tcanalysis> RefList)
        {
            string DateS = (Request["WorkDate"] ?? this.ExportDate.DateValue);
            string DateE = this.GetFormEqlValue("WorkDateE") ?? this.ExportDate2.DateValue;
            var CardLog = this.odo.GetQueryResult(this.CardLogCmdStr,new {DateS = Convert.ToDateTime(DateS).AddDays(-1).ToString("yyyy/MM/dd"),
                DateE = Convert.ToDateTime(DateE).AddDays(1).ToString("yyyy/MM/dd")}).ToList();
            foreach (var o in RefList)
            {
                o.chkcode = "應刷";
                o.status = "1";
                o.statusname = "正常";
                o.ndate = o.sdate;
                o.timenbr = "0";
                if (o.Delay > 0 && o.kind.Equals("1"))
                {
                    o.status = "2";
                    o.statusname = "遲到";
                }
                if (o.StealTime > 0 && o.kind.Equals("2"))
                {
                    o.status = "3";
                    o.statusname = "早退";
                }
                if (o.ClassNo.Equals("1") && o.kind.Equals("2"))
                {
                    o.WorkDate = Convert.ToDateTime(o.WorkDate).AddDays(1).ToString("yyyy/MM/dd");
                    o.sdate = (Convert.ToDateTime(o.WorkDate).Year - 1911).ToString() + Convert.ToDateTime(o.WorkDate).ToString("MMdd");                    
                }                
                if (o.cardtime.Equals(""))
                {
                    o.carddate = "";
                    o.statusname = "未刷";
                }
                else
                {
                    if(CardLog.Where(i=>Convert.ToString(i.CardDate).Equals(o.WorkDate) && Convert.ToString(i.PsnNo).Equals(o.clerkno)).Count()>0)
                    {
                        o.source = CardLog.Where(i => Convert.ToString(i.CardDate).Equals(o.WorkDate) && Convert.ToString(i.PsnNo).Equals(o.clerkno)).First().EquNo;
                    }
                    o.carddate = o.sdate;
                }
                if (o.CrossDay == 1)
                {
                    o.WorkDate = Convert.ToDateTime(o.WorkDate).AddDays(1).ToString("yyyy/MM/dd");
                    o.carddate = (Convert.ToDateTime(o.WorkDate).Year - 1911).ToString() + Convert.ToDateTime(o.WorkDate).ToString("MMdd");
                }
            }
            RefList = RefList.Where(i => !string.IsNullOrEmpty(i.carddate)).ToList();
        }

        string InsertCmdExt = @"INSERT INTO tcanalysis (ndate, clerkno, sdate, ctime, source, kind, sourcename, status, statusname, carddate, cardtime, timenbr, chkcode, cdatetime, absent) 
                                                    VALUES (@ndate, @clerkno, @sdate, @ctime, @source, @kind, @sourcename, @status, @statusname, @carddate, @cardtime, @timenbr, @chkcode, '', '')";

        string CardLogCmdStr = @"Select distinct PsnNo,EquNo,EquDir,CtrlAddr,Convert(varchar(10),CardTime, 111) AS CardDate,
                                                                        CONVERT(varchar, CardTime, 108) AS CardTime
                                  From V_ShowCardLog
                                 WHERE (Convert(varchar(10),CardTime, 111) BETWEEN @DateS AND @DateE) AND PsnNo IS  NOT NULL AND EquNo IS NOT NULL";
        string MainCmdStr = @"SELECT * FROM (
                                                SELECT 
                                            WorkDate,
                                            PsnNo As clerkno,
                                            (CONVERT(VARCHAR(3),CONVERT(VARCHAR(4),WorkDate,20) - 1911) + 
                                             SUBSTRING(CONVERT(VARCHAR(10),WorkDate,20),6,2) + 
                                             SUBSTRING(CONVERT(VARCHAR(10),WorkDate,20),9,2)) As sdate,
                                            REPLACE(WorkTimeS,':','') AS ctime,
                                             '' As [source],
                                             '1' As [kind], '上班' AS sourcename,
											 ClassNo,
                                             REPLACE(RealTimeS,':','') AS cardtime,
                                            [Delay],
                                            StealTime,
                                            OverTime,
											0 AS CrossDay
                                            FROM B03_WorkDetail
                                            WHERE 1=1
                                            AND (WorkDate BETWEEN @DateS AND @DateE)
                                        UNION
                                        SELECT 
                                            WorkDate,
                                            PsnNo As clerkno,
                                            (CONVERT(VARCHAR(3),CONVERT(VARCHAR(4),WorkDate,20) - 1911) + 
                                             SUBSTRING(CONVERT(VARCHAR(10),WorkDate,20),6,2) + 
                                             SUBSTRING(CONVERT(VARCHAR(10),WorkDate,20),9,2)) As sdate,
                                            REPLACE(WorkTimeE,':','') AS ctime,
                                             '' As [source],
                                             '2' As [kind], '  下班' AS sourcename,
											ClassNo,
                                             REPLACE(RealTimeE,':','') AS cardtime,
                                            [Delay],
                                            StealTime,
                                            OverTime,
											CASE WHEN (RealTimeE BETWEEN '00:00' AND '12:00' ) AND ClassNo='3' THEN 1 ELSE 0 END AS CrossDay
                                            FROM B03_WorkDetail
                                            WHERE 1=1
                                            AND (WorkDate BETWEEN @DateS AND @DateE)) AS Result where ctime<>''   AND ClassNo <>'' ";
        
    }//end page class
}//end namespace