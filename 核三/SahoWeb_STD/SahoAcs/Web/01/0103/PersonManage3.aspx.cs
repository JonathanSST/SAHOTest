using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using SahoAcs.DBClass;

namespace SahoAcs.Web
{
    public partial class PersonManage3 : BasePage
    {        
        private int _pagesize = 20; //DataGridView每頁顯示的列數
        private int years = 80;     //年份選單的預設年數
        private int yongyear = 16;  //成年人歲數        
        public int CardLen = 8;     //卡號長度
        DataTable dtAuthMode = new DataTable();
        static string MsgCardNoRequired, MsgCardNoReused, MsgCardSize, MsgCardSpace, MsgCardTypeReused;
        static string MsgNameRequired, MsgPsnNoReused, MsgPsnNoSpace, MsgPsnNoRequired, MsgPsnAccountReused;
        static string MsgPwdRequired, MsgPwdSize, MsgPwdSpace;
        static string UserName;
        public string ErrMessage = "";
        public bool ErrResult = true;
        static GlobalMsg gms = new GlobalMsg();
        public List<dynamic> ItemList = new List<dynamic>();
        public List<OrgDataEntity> OrgDataInit = new List<OrgDataEntity>();
        public List<dynamic> PsnTypeList = new List<dynamic>();
        public List<dynamic> TextInfo = new List<dynamic>();
        public List<dynamic> AuthMode = new List<dynamic>();
        public List<dynamic> CardList = new List<dynamic>();
        public Dictionary<int, string> DictTravel = new Dictionary<int, string>();
        public Dictionary<string, string> DictClass = new Dictionary<string, string>();
        public PersonEntity PersonObj = new PersonEntity();
        public OrgStrucInfo OrgStruc = new OrgStrucInfo();        
        public List<string> OrgInfoList2 = new List<string>();
        public string CardVer = "N";
        public string AuthList = "";
        //public List<SysParam>
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        protected override void Page_Load(object sender, EventArgs e)
        {            
            SetInitData();
            if (Request["PsnID"] != null && Request["PageEvent"] == "Edit")
            {
                this.DictTravel.Clear();
                this.DictTravel.Add(1, "汽車");
                this.DictTravel.Add(2, "機車");
                this.SetEdit(Request["PsnID"]);
            }
            else if (Request["PageEvent"] == "Delete")
            {
                this.SetDelete();
            }
            else if (Request["PageEvent"] == "VerifiMode")
            {
                this.SetVerifiMode();
            }
            else if (Request["PageEvent"] == "Save")
            {
                this.SetObjData();
                this.SetOrgStrucData();
                if (Request["PsnNo"] != null && Request["PsnNo"].Length == 6)
                {
                    if (Request["PsnID"] == "0")
                    {
                        //新增B01_Person
                        if (!this.GetDblPsnNo() && !this.GetDblCardNo())
                        {
                            this.DoInsert();
                            //新增人員完成
                            foreach (var o in this.odo.GetQueryResult<PersonEntity>("SELECT  * FROM B01_Person WHERE PsnNo=@PsnNo AND PsnName=@PsnName ", this.PersonObj))
                            {
                                this.SetEdit(o.PsnID.ToString());
                            }
                        }
                        //新增B01_Card，這裡確定已經新增人員完成，才可以做卡片新增                 
                        if (this.PersonObj.PsnID > 0)
                        {
                            this.PersonObj.CardNo = this.GetFormEqlValue("CardNo");
                            if (!this.GetDblCardNo())
                            {
                                this.DoInsertCard();
                                this.SetEdit(this.PersonObj.PsnID.ToString());
                                this.odo.SetOrgEquData(this.PersonObj.CardNo, this.PersonObj.PsnID, Session["UserID"].ToString(), gms);
                            }
                        }
                    }
                    else
                    {
                        //執行人員資料異動
                        if (!this.GetDblPsnNo())
                        {
                            this.DoUpdate();
                            this.SetEdit(this.PersonObj.PsnID.ToString());
                        }                        
                    }
                }
                else
                {
                    this.ErrMessage = "工號欄位長度必須為6碼";
                }
               
            }
            else
            {
                string jsFileEnd = "<script src=\"../../../uc/QueryTool.js?" + Pub.GetNowTime + "\" Type=\"text/javascript\"></script>\n";
                jsFileEnd += "<script src=\"CardGroupEdit.js?" + Pub.GetNowTime + "\" Type=\"text/javascript\"></script>\n";
                jsFileEnd += "<script src=\"CardCode.js?" + Pub.GetNowTime + "\" Type=\"text/javascript\"></script>\n";
                jsFileEnd += "<script src=\"CardEditV2.js?" + Pub.GetNowTime + "\" Type=\"text/javascript\"></script>\n";
                ClientScript.RegisterStartupScript(typeof(string), "CardGroupEdit", jsFileEnd, false);

                string js = Sa.Web.Fun.ControlToJavaScript(this);
                js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
                js = "<script type='text/javascript'>" + js + "</script>";
                ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
                ClientScript.RegisterClientScriptInclude("Person", "PersonManage.js?" + Pub.GetNowTime); //加入同一頁面所需的JavaScript檔案
                ClientScript.RegisterClientScriptInclude("JsCheck", "../../../Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
                ClientScript.RegisterClientScriptInclude("JsUtil", "../../../Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
            }
        }

        private void SetInitData()
        {
            string sql = @"SELECT B01_OrgData.OrgClass, B00_ItemList.ItemName, B00_ItemList.ItemInfo2,B00_ItemList.ItemNo,
                B00_ItemList.ItemOrder FROM B01_OrgData 
                INNER JOIN B00_ItemList ON B01_OrgData.OrgClass = B00_ItemList.ItemNo
                WHERE B00_ItemList.ItemClass = 'OrgClass'
                GROUP BY B01_OrgData.OrgClass, B00_ItemList.ItemName, B00_ItemList.ItemInfo2, B00_ItemList.ItemOrder,B00_ItemList.ItemNo
                ORDER BY B00_ItemList.ItemOrder";
            this.ItemList = this.odo.GetQueryResult(sql).ToList();
            //查詢組織相關資料
            this.OrgDataInit = this.odo.GetQueryResult<SahoAcs.DBModel.OrgDataEntity>("SELECT * FROM B01_OrgData").ToList();
            //查詢人員類別資料
            this.PsnTypeList = this.odo.GetQueryResult("SELECT ItemOrder, ItemNo, ItemName FROM dbo.B00_ItemList WHERE ItemClass='PsnType' ORDER BY ItemOrder").ToList();
            //查詢人員文字一~五
            this.TextInfo = this.odo.GetQueryResult("SELECT ParaName,Replace(ParaNo,'Input_','') AS ParaNo,'' AS ParaValue FROM B00_SysParameter WHERE ParaClass='InputText' AND ParaNo<>'Input_Text5'  AND ParaValue='Y' ").ToList();
            //認證模式
            this.AuthMode = this.odo.GetQueryResult("SELECT * FROM B00_ItemList WHERE ItemClass='AuthModel' ORDER BY ItemOrder").ToList();
            foreach(var o in this.odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='CardLen' AND ParaClass='CardID' "))
            {
                this.CardLen = int.Parse(Convert.ToString(o.ParaValue));
            }
            this.AuthList = Session["FunAuthSet"].ToString();
            this.PsnSTime.DateValue = DateTime.Now.ToString(CommFormat.DateTimeF);
            this.PersonObj.PsnAuthAllow = "1";
            this.PersonObj.VerifiMode = "0";
            foreach (var o in this.odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='DefaultEndDateTime' "))
            {
                this.PsnETime.DateValue = this.PersonObj.PsnETime = Convert.ToString(o.ParaValue) + " 23:59:59";
            }
            //設定班別資訊
            var ClassNoList = new List<string>();
            ClassNoList.Add("A");
            ClassNoList.Add("B");
            ClassNoList.Add("C");
            ClassNoList.Add("ABCR");
            foreach(var o in this.odo.GetQueryResult("SELECT * FROM B00_ClassData WHERE CNo NOT IN @ClassNoList ORDER BY CNo",new { ClassNoList = ClassNoList }))
            {
                this.DictClass.Add(o.CNo, o.CName);                
            }
            this.DictClass.Add("ABCR", "輪班人員");
            //設定預設組織架構=>公司代碼
            string DefaultOrg = "";
            if (System.Configuration.ConfigurationManager.AppSettings["DefaultOrg"] != null)
            {
                DefaultOrg = System.Configuration.ConfigurationManager.AppSettings["DefaultOrg"].ToString();
            }
            if (this.OrgDataInit.Where(i => string.Concat(i.OrgName, ".", i.OrgNo).Equals(DefaultOrg)).Count()>0){
                this.OrgInfoList2.Add(this.OrgDataInit.Where(i => string.Concat(i.OrgName, ".", i.OrgNo).Equals(DefaultOrg)).First().OrgID.ToString());
            }
        }


        private void SetVerifiMode()
        {
            Pub.MessageObject sRet = new Pub.MessageObject();
            OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
            var sql_cmd = @"UPDATE B01_CardAuth SET 
	                                        VerifiMode=@VerifiMode,
	                                        ErrCnt=0,
	                                        OpMode='Reset',
	                                        OpStatus='',
                                            UpdateTime=GETDATE(),
                                            UpdateUserID=@UserID
                                        WHERE CardNo IN (SELECT CardNo from B01_Card WHERE PsnID=@PsnID AND CardAuthAllow=1) 
                                        AND EquID IN (SELECT EquID FROM B01_EquData WHERE EquModel IN @EquList)  AND OpMode<>'Del' ;
                                        UPDATE B01_Person SET VerifiMode=@VerifiMode,UpdateTime=GETDATE(),UpdateUserID=@UserID WHERE PsnID =@PsnID ";

            int istat = 0;
            odo.BeginTransaction();
            var page = HttpContext.Current.Handler as Page;
            odo.Execute(sql_cmd, new { EquList = this.VerifiEquList(), PsnID = Request["PsnID"], VerifiMode = Request["VerifiMode"], UserID = Sa.Web.Fun.GetSessionStr(this,"UserID") });
            if (istat > -1)
            {
                odo.Commit();
                sRet.message = gms.MsgVerSuccess;
                sRet.result = true;
            }
            else
            {
                odo.Rollback();
                sRet.message = gms.MsgVerFailed;
                sRet.result = false;
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(sRet));
            Response.End();
        }


        /// <summary>設定組織架構</summary>
        private void SetOrgStrucData()
        {
            //設定新的組織架構
            if (Request.Form.GetValues("OrgDataNo").Where(i => i != "").Count() != 0)
            {
                this.PersonObj.OrgIDList = @"\" + string.Join(@"\", Request.Form.GetValues("OrgDataNo").Where(i => i != "")) + @"\";
            }
            else
            {
                this.PersonObj.OrgIDList = @"\1\";
            }
            var orgs = this.odo.GetQueryResult("SELECT * FROM OrgStrucAllData('') WHERE OrgIDList=@OrgIDList", new { OrgIDList = this.PersonObj.OrgIDList });
            foreach (var org in orgs)
            {
                this.PersonObj.OrgStrucID = Convert.ToInt32(org.OrgStrucID);
            }
            if (orgs.Count() == 0)
            {
                var OrgData = this.odo.GetQueryResult<OrgDataEntity>("SELECT * FROM B01_OrgData");
                string OrgStrucNo = "";
                foreach (var o in Request.Form.GetValues("OrgDataNo").Where(i => i != ""))
                {
                    OrgStrucNo += OrgData.Where(i => i.OrgID == int.Parse(o)).Count() > 0 ? OrgData.Where(i => i.OrgID == int.Parse(o)).FirstOrDefault().OrgNo + "." : "";
                }
                OrgStrucNo = OrgStrucNo.TrimEnd(new char[] { '.' });
                orgs = this.odo.GetQueryResult(@"INSERT INTO B01_OrgStruc(OrgStrucNo ,OrgIDList ,CreateUserID ,CreateTime)
                    VALUES(@OrgStrucNo, @OrgIDList, @UserID, GETDATE()); SELECT TOP 1 * FROM B01_OrgStruc ORDER BY OrgStrucID DESC ",
                    new { UserID = Session["UserID"].ToString(), OrgStrucNo = OrgStrucNo, OrgIDList = this.PersonObj.OrgIDList });
                foreach (var org in orgs)
                {                 
                    this.PersonObj.OrgStrucID = Convert.ToInt32(org.OrgStrucID);
                }
            }
        }//完成組織架構設定


        /// <summary>執行資料新增</summary>
        private void DoInsert()
        {
            if (this.GetDblUserID())
            {
                this.ErrMessage = "帳號欄位輸入重複";
                this.PersonObj.PsnAccount = "";
            }
            this.odo.Execute(@"INSERT INTO B01_Person 
                    (PsnPicSource,PsnNo,PsnName,PsnEName,PsnType,IDNum,Birthday,OrgStrucID,PsnAccount,PsnPW, PsnAuthAllow,PsnSTime,PsnETime,Remark,VerifiMode,CreateTime,CreateUserID,UpdateTime,UpdateUserID,Text5,Text1,Text2,Text3,Text4) 
                    VALUES ('',@PsnNo,@PsnName,@PsnEName,@PsnType,@IDNum,@BirthDay,@OrgStrucID,@PsnAccount,@PsnPW
                    ,@PsnAuthAllow,@PsnSTime,@PsnETime,@Remark,@VerifiMode,GETDATE(),@CreateUserID,GETDATE(),@UpdateUserID,@Text5,@Text1,@Text2,@Text3,@Text4) ", this.PersonObj);            
            if (this.odo.isSuccess)
            {
                var log = SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.資料修改, Sa.Web.Fun.GetSessionStr(this, "UserID"), UserName, "0103");
                log.LogInfo = string.Format("ADD PERSON PsnNo='{0}'", this.PersonObj.PsnNo);
                log.LogDesc = "INSERT Person ";
                this.odo.SetSysLogCreate(log);
                if (this.ErrMessage.Length > 0)
                    this.ErrMessage += ",";
                this.ErrMessage += "人員新增完成";
            }
            else
            {
                this.ErrMessage = this.odo.DbExceptionMessage;
                this.ErrResult = false;
            }
            
        }

        /// <summary>卡號新增</summary>
        private void DoInsertCard()
        {
            this.odo.Execute(@"INSERT INTO B01_Card 
                    (PsnID,CardNo,CardType,CardPW,CardAuthAllow,CardVer,CardSTime,CardETime,CreateTime,CreateUserID,UpdateTime,UpdateUserID) 
                    VALUES (@PsnID,@CardNo,@PsnType,'0000',@PsnAuthAllow,@CardVer,@PsnSTime,@PsnETime,GETDATE(),@CreateUserID,GETDATE(),@UpdateUserID) ", this.PersonObj);
            if (this.odo.isSuccess)
            {
                var log = SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.資料修改, Sa.Web.Fun.GetSessionStr(this, "UserID"), UserName, "0103");
                log.LogInfo = string.Format("ADD Card CardNo='{0}'", this.PersonObj.CardNo);
                log.LogDesc = "INSERT Card With Person";
                this.odo.SetSysLogCreate(log);
            }
            else
            {
                this.ErrMessage += "\n" + this.odo.DbExceptionMessage;
                this.ErrResult = false;
            }

        }

        /// <summary>人員基本資料異動</summary>
        private void DoUpdate()
        {
            if (this.GetDblUserID())
            {
                this.ErrMessage = $"登入帳號{this.PersonObj.PsnAccount}輸入重複";
                this.PersonObj.PsnAccount = "";
            }
            string UpdateCmd = @"UPDATE B01_Person SET Text5=@Text5,Text4=@Text4,Text3=@Text3,Text2=@Text2,Text1=@Text1,
                PsnNo=@PsnNo,PsnName=@PsnName,PsnEName=@PsnEName,PsnType=@PsnType,IDNum=@IDNum,Birthday=@BirthDay
                ,OrgStrucID=@OrgStrucID,PsnAccount=@PsnAccount,PsnPW=@PsnPW,PsnAuthAllow=@PsnAuthAllow,PsnSTime=@PsnSTime,PsnETime=@PsnETime
                ,Remark=@Remark,VerifiMode=@VerifiMode,UpdateTime=GETDATE(),UpdateUserID=@UpdateUserID";
            if (this.PersonObj.TravelMode != 0)
                UpdateCmd += ",TravelMode=@TravelMode";
            UpdateCmd += " WHERE PsnID=@PsnID";
            this.odo.Execute(UpdateCmd, this.PersonObj);
            if (this.odo.isSuccess)
            {
                var log=SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.資料修改, Sa.Web.Fun.GetSessionStr(this, "UserID"), UserName, "0103");
                log.LogInfo = string.Format("UPDATE PERSON PsnNo='{0}'", this.PersonObj.PsnNo);
                log.LogDesc = "UPDATE PERSON ";
                this.odo.SetSysLogCreate(log);
                //對主卡進行權限重整設定
                foreach (var o in this.odo.GetQueryResult<CardEntity>("SELECT * FROM B01_Card WHERE PsnID=@PsnID AND CardType NOT IN ('R','TEMP') ORDER BY CardID ",this.PersonObj))
                {                    
                    this.odo.SetOrgEquData(o.CardNo,o.PsnID, Session["UserID"].ToString(), gms);
                }
                if (this.ErrMessage.Length > 0)
                    this.ErrMessage += ",";
                this.ErrMessage += "人員修改完成";
            }
            else
            {
                this.ErrMessage += this.odo.DbExceptionMessage;
                this.ErrResult = false;
            }       
        }


        /// <summary>取得人員編輯介面</summary>
        /// <param name="StrPsnID">PsnID 人員編號</param>
        private void SetEdit(string StrPsnID)
        {            
            this.PersonObj = this.odo.GetQueryResult<PersonEntity>(@"SELECT A.*,CONVERT(VARCHAR(10),Birthday,111) AS BirthDay,B.OrgNameList,B.OrgNoList,B.OrgIDList
                ,REPLACE(CONVERT(VARCHAR(20),PsnSTime,120),'-','/') AS PsnSTime,REPLACE(CONVERT(VARCHAR(20),PsnETime,120),'-','/') AS PsnETime FROM B01_Person A 
                INNER JOIN OrgStrucAllData('') B ON A.OrgStrucID=B.OrgStrucID WHERE PsnID=@PsnID", new { PsnID = StrPsnID }).First();
            this.OrgStruc = this.odo.GetQueryResult<OrgStrucInfo>("SELECT * FROM OrgStrucAllData('') WHERE OrgStrucID=@OrgStrucID", this.PersonObj).First();
            this.PsnSTime.DateValue = this.PersonObj.PsnSTime;
            this.PsnETime.DateValue = this.PersonObj.PsnETime;
            this.CardList = this.odo.GetQueryResult(@"SELECT * FROM B01_Card INNER JOIN B00_ItemList ON CardType=ItemNo 
                                                                        WHERE PsnID=@PsnID AND CardType NOT IN ('R','TEMP') AND ItemClass='CardType' ORDER BY ItemOrder", new { PsnID = StrPsnID }).ToList();
            Dictionary<string, string> DictText = new Dictionary<string, string>();
            DictText.Add("Text1", this.PersonObj.Text1);
            DictText.Add("Text2", this.PersonObj.Text2);
            DictText.Add("Text3", this.PersonObj.Text3);
            DictText.Add("Text4", this.PersonObj.Text4);
            DictText.Add("Text5", this.PersonObj.Text5);
            foreach (var text in this.TextInfo)
            {
                if (DictText.Where(i => i.Key.Equals(Convert.ToString(text.ParaNo).Replace("Input_", ""))).Count() > 0)
                {
                    text.ParaValue = DictText.Where(i => i.Key.Equals(Convert.ToString(text.ParaNo).Replace("Input_", ""))).First().Value;
                }
            }
            foreach (var o in this.OrgStruc.OrgNoList.Split('\\'))
            {
                if (o != "")
                {                    
                    this.OrgInfoList2 = this.OrgStruc.OrgIDList.Split('\\').Where(i=>i!="").ToList();
                }                
            }
        }//end SetEdit method

        private bool GetDblUserID()
        {
            if (this.PersonObj.PsnAccount.Equals(""))
            {
                return false;
            }
            var UserCnt = this.odo.GetIntScalar(@"SELECT COUNT(*) FROM 
	                                                            (SELECT PsnAccount FROM B01_Person WHERE PsnAccount=@PsnAccount AND PsnNo<>@PsnNo
	                                                            UNION
	                                                            SELECT UserID FROM B00_SysUser WHERE UserID=@PsnAccount) AS T1", this.PersonObj);
            if (UserCnt > 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>驗證人員是否重複</summary>
        /// <returns></returns>
        private bool GetDblPsnNo()
        {
            if (this.odo.GetQueryResult(@"SELECT * FROM B01_Person WHERE PsnNo=@PsnNo AND PsnID<>@PsnID", this.PersonObj).Count() > 0)
            {
                this.ErrMessage = string.Format("工號{0}出現重複", this.PersonObj.PsnNo);
                this.ErrResult = false;
                return true;
            }
            return false;
        }

        /// <summary>驗證卡號是否重複</summary>
        /// <returns></returns>
        private bool GetDblCardNo()
        {
            this.PersonObj.CardNo = Request["CardNo"];
            this.PersonObj.CardVer = Request["CardVer"];
            if (this.PersonObj.CardVer == null)
                this.PersonObj.CardVer = "";
            if (this.odo.GetQueryResult("SELECT * FROM B01_Card WHERE CardNo=@CardNo", this.PersonObj).Count() > 0)
            {
                if (this.ErrMessage.Length > 0)
                    this.ErrMessage += ",";
                this.ErrMessage += string.Format("卡號{0}出現重複", this.PersonObj.CardNo);
                this.ErrResult = false;
                return true;
            }
            return false;
        }


        /// <summary>執行人員基本資料刪除</summary>
        private void SetDelete()
        {
            this.odo.Execute("DELETE B01_PersonPicture WHERE PsnID=@PsnID", new { PsnID = Request["PsnID"] });
            this.odo.Execute("DELETE B01_Person WHERE PsnID=@PsnID", new { PsnID = Request["PsnID"] });
            var CardData = this.odo.GetQueryResult<CardEntity>("SELECT * FROM B01_Card WHERE PsnID=@PsnID AND CardType NOT IN ('R','TEMP')", new { PsnID = Request["PsnID"] });
            var log = SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.資料刪除, Sa.Web.Fun.GetSessionStr(this, "UserID"), UserName, "0103");
            log.LogInfo = string.Format("Delete Card PsnNo='{0}'", Request["PsnNo"]);
            log.LogDesc = "DELETE PERSON";
            this.odo.SetSysLogCreate(log);
            foreach (var o in CardData)
            {
                this.odo.Execute("UPDATE B01_CardAuth SET OpMode='Del',OpStatus='',ErrCnt=0 WHERE CardNo=@CardNo", o);
                this.odo.Execute("DELETE B01_CardEquAdj WHERE CardID=@CardID", o);
                this.odo.Execute("DELETE B01_CardEquGroup WHERE CardID=@CardID", o);
                this.odo.Execute("DELETE B01_CardExt WHERE CardID=@CardID", o);
                this.odo.Execute("DELETE B01_Card WHERE CardID=@CardID", o);
                this.odo.Execute("DELETE B02_EyeData WHERE CardNo=@CardNo", o);
                this.odo.Execute("DELETE B02_FaceData WHERE CardNo=@CardNo", o);
                this.odo.Execute("DELETE B02_FPData WHERE CardNo=@CardNo", o);
                this.odo.Execute("DELETE B02_FaceData2 WHERE CardNo=@CardNo", o);
                this.odo.Execute("DELETE B02_FaceImageData WHERE CardNo=@CardNo", o);
                this.odo.Execute("DELETE B02_FaceImageData2 WHERE CardNo=@CardNo", o);
                this.odo.Execute("DELETE B02_FVPData WHERE CardNo=@CardNo", o);                
                log.LogInfo = string.Format("Delete Card CardNo='{0}'", o.CardNo);
                log.LogDesc = "DELETE PERSON WITH CARD";
                this.odo.SetSysLogCreate(log);
            }
        }

        /// <summary>產生人員基本資料物件</summary>
        private void SetObjData()
        {
            List<string> ListCols = new List<string>();
            ListCols.Add("PsnNo");
            ListCols.Add("CardNo");
            ListCols.Add("CardVer");
            ListCols.Add("IDNum");
            ListCols.Add("Birthday");
            ListCols.Add("PsnName");
            ListCols.Add("PsnEName");
            ListCols.Add("PsnType");
            ListCols.Add("PsnAuthAllow");
            ListCols.Add("PsnSTime");
            ListCols.Add("PsnETime");
            ListCols.Add("Remark");
            ListCols.Add("PsnAccount");
            ListCols.Add("PsnPW");
            ListCols.Add("OrgStrucID");
            ListCols.Add("VerifiMode");
            ListCols.Add("PsnID");
            ListCols.Add("TravelMode");
            ListCols.Add("Text5");
            ListCols.Add("Text4");
            ListCols.Add("Text3");
            ListCols.Add("Text2");
            ListCols.Add("Text1");
            var para = this.GetMasterPackage(ListCols);
            this.PersonObj = this.DictionaryToObject<PersonEntity>(para);            
            this.PersonObj.PsnSTime = Request[Request.Form.AllKeys.Where(i => i.Contains("PsnSTime")).First()];
            this.PersonObj.PsnETime = Request[Request.Form.AllKeys.Where(i => i.Contains("PsnETime")).First()];
            this.PersonObj.BirthDay = Request[Request.Form.AllKeys.Where(i => i.Contains("BirthDay")).First()];
            this.PersonObj.CreateUserID = Session["UserID"].ToString();
            this.PersonObj.UpdateUserID = Session["UserID"].ToString();
            this.PersonObj.PsnPicSource = "";
            this.PersonObj.CreateTime = DateTime.Now;
            this.PersonObj.UpdateTime = DateTime.Now;
        }//end method


    }//end page class
}//end namespace