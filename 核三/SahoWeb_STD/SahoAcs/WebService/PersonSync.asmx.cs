using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;
using SMS_Communication;

using DapperDataObjectLib;
using SahoAcs.DBModel;

namespace SahoAcs.WebService
{

    public class Authentication : SoapHeader
    {
        public string sysAccount;
        public string sysPassword;
    }



    /// <summary>
    ///PersonSync 的摘要描述
    /// </summary>
    [WebService(Namespace = "SahoWeb")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    [System.Web.Script.Services.ScriptService]
    public class PersonSync : System.Web.Services.WebService
    {
        public Authentication authentication;

        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());


        public int GetDoOrgSync(NewPerson record)
        {
            //若組織未存在，則組織架構要設為全群組
            string unit_no = record.UnitNo != "" ? string.Concat("U", record.UnitNo) : "";
            string title_no = record.TitleNo != "" ? string.Concat("T", record.TitleNo) : "";
            XDocument xDoc = XDocument.Load(HttpContext.Current.Request.PhysicalApplicationPath+@"ParaSetting\ParaCY.xml");

            string comp_no = xDoc.Root.Element("CompanyNo").Value;
                //ConfigurationManager.AppSettings["CompanyNo"].ToString();
            List<OrgDataEntity> OrgSync = new List<OrgDataEntity>();
            string OrgNoList = string.Format(@"\{0}\", comp_no);
            if (record.UnitNo.Trim() != "")
            {
                OrgSync.Add(new OrgDataEntity() { OrgNo = string.Concat("U", record.UnitNo), OrgName = record.Unit, OrgClass = "Unit" });
                OrgNoList +=unit_no + @"\";
            }
            if (record.TitleNo.Trim() != "")
            {
                OrgSync.Add(new OrgDataEntity() { OrgNo = string.Concat("T", record.TitleNo), OrgName = record.Title, OrgClass = "Title" });
                OrgNoList += title_no + @"\";
            }
            var OrgDatas = this.odo.GetQueryResult<OrgDataEntity>("SELECT * FROM B01_OrgData").ToList();
            foreach (var o in OrgSync)
            {
                if (OrgDatas.Where(i => i.OrgNo == o.OrgNo).Count() == 0)
                {
                    //設定 OrgData
                    this.odo.Execute(@"INSERT INTO B01_OrgData 
                                                            (OrgNo,OrgName,OrgClass,CreateUserID,CreateTime,UpdateUserID,UpdateTime) 
                                                            VALUES (@OrgNo,@OrgName,@OrgClass,'Saho',GETDATE(),'Saho',GETDATE())", o);
                    OrgDatas.Add(o);
                }
            }           
            if (this.odo.GetQueryResult(@"SELECT * FROM OrgStrucAllData('') WHERE OrgNoList=@OrgNoList ",
                                                        new { OrgNoList = OrgNoList }).Count() == 0)
            {
                this.odo.Execute(@"INSERT INTO B01_OrgStruc (OrgIDList,OrgStrucNo,CreateUserID,CreateTime,UpdateUserID,UpdateTime) 
                    SELECT '\'+ISNULL((SELECT TOP 1 CONVERT(VARCHAR,OrgID) FROM B01_OrgData WHERE OrgNo=@comp_no)+'\','')
	                                                        +ISNULL((SELECT TOP 1 CONVERT(VARCHAR,OrgID) FROM B01_OrgData WHERE OrgNo=@UnitNo)+'\','')
	                                                        +ISNULL((SELECT TOP 1 CONVERT(VARCHAR,OrgID) FROM B01_ORGDATA WHERE OrgNo=@TitleNo)+'\','') AS OrgIDList
	                                                        ,@comp_no
	                                                        +ISNULL('_'+(SELECT TOP 1 CONVERT(VARCHAR,OrgNo) FROM B01_OrgData WHERE OrgNo=@UnitNo),'')
	                                                        +ISNULL('_'+(SELECT TOP 1 CONVERT(VARCHAR,OrgNo) FROM B01_OrgData WHERE OrgNo=@TitleNo),'') AS OrgStrucNo
                                                            ,'Saho',GETDATE(),'Saho',GETDATE()",
                                                        new { UnitNo = "U" + record.UnitNo, TitleNo = "T" + record.TitleNo, comp_no = comp_no });

            }
            var result = this.odo.GetQueryResult(@"SELECT * FROM OrgStrucAllData('') WHERE OrgNoList=@OrgNoList ",new { OrgNoList = OrgNoList });
            return Convert.ToInt32(result.FirstOrDefault().OrgStrucID);
        }


        [WebMethod]
        [SoapHeader("authentication")]
        public string CreatePerson(NewPerson record)
        {
            string id = this.authentication.sysAccount;
            string pwd = this.authentication.sysPassword;
            var persons = this.odo.GetQueryResult<PersonEntity>("SELECT * FROM B01_Person").ToList();
            var cards = this.odo.GetQueryResult<CardEntity>("SELECT B.*,A.PsnNo,A.PsnName FROM B01_Person A INNER JOIN B01_Card B ON A.PsnID=B.PsnID").ToList();

            if (persons.Where(i => i.PsnNo == record.PsnNo).Count() == 0)
            {
                //若組織未存在，則組織架構要設為全群組
                record.OrgStrucID=this.GetDoOrgSync(record);
                //人員基本資料驗證
                DateTime checktime = DateTime.Now;
                var result = this.odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo=@ParaNo",new {ParaNo= "DefaultEndDateTime" });
                DateTime checketime = DateTime.Parse(result.FirstOrDefault().ParaValue);
                if(!DateTime.TryParse(record.PsnSTime, out checktime))
                {
                    record.PsnSTime = checktime.ToString();
                }
                if(!DateTime.TryParse(record.PsnETime,out checketime))
                {
                    record.PsnETime = checketime.ToString();
                }
                //執行人員基本資料新增
                this.odo.Execute(@"INSERT INTO B01_Person (PsnNo,PsnName,PsnAuthAllow,PsnType,PsnSTime,PsnETime,OrgStrucID,Rev01,
                                                            CreateUserId, CreateTime, UpdateUserId, UpdateTime) VALUES
                                                               (@PsnNo, @PsnName, 1, 'E', @PsnSTime, @PsnETime, @OrgStrucID,1, 'Saho', GETDATE(), 'Saho', GETDATE())", record);
                var IsSuccess = this.odo.isSuccess;
                if (IsSuccess && cards.Where(i=>i.CardNo==record.CardNo).Count()==0)
                {
                    if (record.CardNo.Length != 8)
                    {
                        IsSuccess = false;
                    }
                    this.odo.Execute(@"INSERT INTO B01_Card (CardNo,PsnID,CardAuthAllow,CardType,CardSTime,CardETime,
                                                            CreateUserId,CreateTime,UpdateUserId,UpdateTime) VALUES 
                                                            (@CardNo,(SELECT TOP 1 PsnID FROM B01_Person WHERE PsnNo=@PsnNo),1,'E',@PsnSTime,@PsnETime,
                                                            'Saho',GETDATE(),'SahoSync',GETDATE())", record);
                    IsSuccess = this.odo.isSuccess;
                }else{
                    IsSuccess = false;
                }
               
                if (IsSuccess)
                {
                    var result2 = this.odo.GetQueryResult("SELECT * FROM B01_Card WHERE CardNo=@CardNo",record);
                    var cardid = Convert.ToInt32(result2.FirstOrDefault().CardID);
                    List<EquGroupModel> groups = this.odo.GetQueryResult<EquGroupModel>("SELECT *,CardID=@CardID FROM B01_EquGroup WHERE EquGrpNo IN @EquGroups", 
                        new { EquGroups = record.EquGroupNoList.Select(i => i.EquGrpNo), CardID = cardid }).ToList();
                    this.odo.Execute("INSERT INTO B01_CardEquGroup (CardID,EquGrpID,CreateUserID,CreateTime) VALUES (@CardID,@EquGrpID,'Saho',GETDATE())", groups);
                    this.odo.Execute("EXEC CardAuth_Update @CardNo, 'Saho' ,'ResetCardAuth:ExcuteCardAuthReset','','卡片權限重整';", record);
                }
            }
            return "Success";
        }

        [WebMethod]
        [SoapHeader("authentication")]
        public string ModifyPerson(string PsnNo, string PsnName, string OrgId, string OrgName, string UnitId, string UnitName)
        {
            return "Success";
        }

        [WebMethod]
        [SoapHeader("authentication")]
        public string DeletePerson(string PsnNo, string PsnName)
        {
            return "Success";
        }

        [WebMethod(EnableSession =true)]
        [SoapHeader("authentication")]
        public string OpenDoor(string EquNo)
        {
            string[] sIPArray = null;
            string routeList = "Start";

            string sIPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(sIPAddress)) { sIPArray = sIPAddress.Split(new char[] { ',' }); } else { sIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; }
            if (sIPAddress == "::1") { sIPAddress = "127.0.0.1"; }

            SahoWebSocket oSWSocket = null;
            try
            {
                if (!string.IsNullOrEmpty(EquNo))
                {
                    #region 傳送APP指令字串
                    oSWSocket = new SahoWebSocket();
                    oSWSocket.UserID = "Saho";
                    oSWSocket.SourceIP = sIPAddress;
                    oSWSocket.SCListenIPPort = System.Web.Configuration.WebConfigurationManager.AppSettings["SCListenIPPort"].ToString();
                    oSWSocket.DbConnectionString = Pub.GetConnectionString(Pub.sConnName);
                    oSWSocket.Start();
                    oSWSocket.ClearCmdResult();
                    var equs = this.odo.GetQueryResult("SELECT * FROM B01_EquData WHERE EquNo=@EquNo", new { EquNo = EquNo });
                    foreach (var o in equs)
                    {
                        oSWSocket.SendAppCmdStr(Convert.ToString(o.EquID)+"@" + EquNo + "@OpenDoorSet@");
                    }
                    System.Threading.Thread.Sleep(3000);
                    if (oSWSocket.EquNoRecordIDHashtable.ContainsKey(EquNo))
                    {
                        routeList +=","+ ((SahoWebSocketCmdResult)oSWSocket.EquNoRecordIDHashtable[EquNo]).ResultMsg;
                    }
                    #endregion
                }
            }
            catch(Exception ex)
            {
                return "Error...."+ex.Message+"  "+routeList;
            }
            finally
            {
                sIPArray = null;
                oSWSocket = null;
            }
            return "Success+"+routeList;
        }


        [WebMethod(EnableSession = true)]
        [SoapHeader("authentication")]
        public string GetCtrlVer(string EquNo)
        {
            string[] sIPArray = null;
            string routeList = "Start";

            string sIPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(sIPAddress)) { sIPArray = sIPAddress.Split(new char[] { ',' }); } else { sIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; }
            if (sIPAddress == "::1") { sIPAddress = "127.0.0.1"; }
            SahoWebSocket oSWSocket = null;
            try
            {
                if (!string.IsNullOrEmpty(EquNo))
                {
                    #region 傳送APP指令字串
                    oSWSocket = new SahoWebSocket();
                    oSWSocket.UserID = "Saho";
                    oSWSocket.SourceIP = sIPAddress;
                    oSWSocket.SCListenIPPort = System.Web.Configuration.WebConfigurationManager.AppSettings["SCListenIPPort"].ToString();
                    oSWSocket.DbConnectionString = Pub.GetConnectionString(Pub.sConnName);
                    oSWSocket.Start();
                    oSWSocket.ClearCmdResult();
                    var equs = this.odo.GetQueryResult("SELECT * FROM B01_EquData WHERE EquNo=@EquNo", new { EquNo = EquNo });
                    foreach (var o in equs)
                    {
                        oSWSocket.SendAppCmdStr(Convert.ToString(o.EquID) + "@" + EquNo + "@CtrlVerGet@");
                    }
                    System.Threading.Thread.Sleep(3000);
                    if (oSWSocket.EquNoRecordIDHashtable.ContainsKey(EquNo))
                    {
                        routeList += "," + ((SahoWebSocketCmdResult)oSWSocket.EquNoRecordIDHashtable[EquNo]).ResultMsg;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                return "Error...." + ex.Message + "  " + routeList;
            }
            finally
            {
                sIPArray = null;
                oSWSocket = null;
            }
            return "Success+" + routeList;
        }


    }//end class
}//end namespace
