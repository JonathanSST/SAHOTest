using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using SahoAcs.DBClass;

namespace SahoAcs.Unittest
{

    public partial class AddMasterController : System.Web.UI.Page
    {

        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public List<dynamic> MasterData = new List<dynamic>();
        public List<dynamic> MasterData2 = new List<dynamic>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null && Request["PageEvent"]== "InsertController")
            {
                this.SetInsertData();
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Del")
            {
                this.SetDeleteData();
            }
            else
            {
                this.MasterData = this.odo.GetQueryResult("SELECT * FROM B01_Master WHERE MstID NOT IN (SELECT MstID FROM B01_Controller)").ToList();
                this.MasterData2 = this.odo.GetQueryResult(@"SELECT  dbo.B01_DeviceConnInfo.DciID, dbo.B01_DeviceConnInfo.DciNo, dbo.B01_DeviceConnInfo.DciName, dbo.B01_DeviceConnInfo.IsAssignIP, 
               dbo.B01_DeviceConnInfo.IpAddress, dbo.B01_DeviceConnInfo.TcpPort, dbo.B01_DeviceConnInfo.DciPassWD, dbo.B01_Master.MstID, 
               dbo.B01_Master.MstNo, dbo.B01_Master.MstDesc, dbo.B01_Master.MstType, dbo.B01_Master.MstConnParam, dbo.B01_Master.CtrlModel AS MCtrlModel, 
               dbo.B01_Master.LinkMode, dbo.B01_Master.AutoReturn, dbo.B01_Master.MstModel, dbo.B01_Master.MstFwVer, dbo.B01_Master.MstStatus, 
               dbo.B01_Controller.CtrlID, dbo.B01_Controller.CtrlNo, dbo.B01_Controller.CtrlName, dbo.B01_Controller.CtrlDesc, dbo.B01_Controller.CtrlAddr, 
               dbo.B01_Controller.CtrlModel AS CCtrlModel, dbo.B01_Controller.CtrlFwVer, dbo.B01_Controller.CtrlStatus, dbo.B01_Reader.ReaderID, 
               dbo.B01_Reader.ReaderNo, dbo.B01_Reader.ReaderName, dbo.B01_Reader.ReaderDesc, dbo.B01_EquData.EquID, dbo.B01_Reader.EquNo, 
               dbo.B01_Reader.Dir, dbo.B01_EquData.CardNoLen, dbo.B01_EquData.EquClass, dbo.B01_EquData.IsAndTrt, dbo.B01_EquData.Building, 
               dbo.B01_EquData.Floor, dbo.B01_EquData.InToCtrlAreaID, dbo.B01_EquData.EquName, dbo.B01_EquData.OutToCtrlAreaID, 
               dbo.B01_EquData.EquModel
                FROM     dbo.B01_DeviceConnInfo INNER JOIN
                               dbo.B01_Master ON dbo.B01_DeviceConnInfo.DciID = dbo.B01_Master.DciID INNER JOIN
                               dbo.B01_Controller ON dbo.B01_Master.MstID = dbo.B01_Controller.MstID INNER JOIN
                               dbo.B01_Reader ON dbo.B01_Controller.CtrlID = dbo.B01_Reader.CtrlID INNER JOIN
                               dbo.B01_EquData ON dbo.B01_Reader.EquNo = dbo.B01_EquData.EquNo
                WHERE   (dbo.B01_Master.MstStatus = '0')  ").ToList();
            }
        }



        /// <summary>刪除設備</summary>
        private void SetDeleteData()
        {
            var ctrls = this.odo.GetQueryResult("SELECT * FROM B01_Controller WHERE CtrlID=@CtrlID", new {CtrlID=Request["CtrlID"]});
            var EquDatas = this.odo.GetQueryResult(@"SELECT 
	                                                                                R.*,E.EquName,E.EquID 
                                                                                FROM 
	                                                                                B01_EquData E
	                                                                                INNER JOIN B01_Reader R ON E.EquNo=R.EquNo AND CtrlID=@CtrlID", new { CtrlID = Request["CtrlID"] });
            if (ctrls.Count() > 0)
            {
                int CtrlID = Convert.ToInt32(ctrls.First().CtrlID);
                this.odo.Execute("DELETE B01_Controller WHERE CtrlID=@CtrlID", new { CtrlID = CtrlID });
                this.odo.Execute("DELETE B01_Reader WHERE CtrlID=@CtrlID", new {CtrlID=CtrlID});
            }
            foreach(var o in EquDatas)
            {
                this.odo.Execute("DELETE B01_EquData WHERE EquID=@EquID", new {EquID=Convert.ToInt32(o.EquID)});
                this.odo.Execute("DELETE B01_EquParaData WHERE EquID=@EquID", new { EquID = Convert.ToInt32(o.EquID) });
                //新增控制器及讀卡機的log
                var log = SahoAcs.DBClass.SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.資料刪除, "Saho", "商合行", "020102");
                log.LogInfo = string.Format("讀卡機編號：{0}，讀卡機名稱：{1}", Convert.ToString(o.ReaderNo), Convert.ToString(o.ReaderName));
                log.LogDesc = "刪除讀卡機";
                this.odo.SetSysLogCreate(log);
                log.LogInfo = string.Format("設備編號：{0}，設備名稱：{1}", Convert.ToString(o.EquNo), Convert.ToString(o.EquName));
                log.LogDesc = "刪除設備";
                this.odo.SetSysLogCreate(log);
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new { message = "OK" }));
            Response.End();
            //刪除設備

            //刪除設備參數

            //刪除讀卡機

            //刪除控制器


        }


        /// <summary>新增設備</summary>
        private void SetInsertData()
        {
            if(this.odo.GetQueryResult("SELECT * FROM B01_Controller WHERE CtrlNo=@CtrlNo ",new {CtrlNo=Request["CtrlNo"]}).Count() > 0)
            {
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new { message = "Error", desc = string.Format("控制器編號重複>>{0}",Request["CtrlNo"]) }));
                Response.End();
            }
            if (this.odo.GetQueryResult("SELECT * FROM B01_Controller WHERE CtrlNo=@CtrlAddr AND MstID=@MstID ", new { CtrlAddr = 1, MstID = Request["MstID"] }).Count() > 0)
            {
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new { message = "Error", desc = string.Format("此機號已存在於系統中") }));
                Response.End();
            }

            var sql = @"INSERT INTO B01_Controller (CtrlNo,CtrlName,CtrlDesc,CtrlAddr,CtrlModel,CtrlStatus,MstID,CreateUserID,CreateTime,UpdateuserID,UpdateTime) 
                                VALUES (@CtrlNo,@CtrlName,@CtrlDesc,@CtrlAddr,@CtrlModel,@CtrlStatus,@MstID,@User,GETDATE(),@User,GETDATE())";
            int intResult = odo.Execute(sql, new { CtrlNo = Request["CtrlNo"], CtrlName = Request["CtrlName"], CtrlDesc = Request["CtrlName"], CtrlAddr = 1, CtrlModel = Request["CtrlModel"], CtrlStatus = 1, MstID = Request["MstID"], User = "Saho" });
            if (intResult > 0)
            {
                int CtrlID = odo.GetIntScalar("SELECT IDENT_CURRENT('B01_Controller')");
                string DciID = Request["DciID"];

                //新增讀卡機及設備

                var strReaderNo = "1";      // 讀卡機編號

                // 1. 讀卡機名稱 = 控制器名稱_讀卡機編號
                var strReaderName = Request["CtrlName"] + "_" + strReaderNo;

                // 2. 設備編號 = 控制器編號_讀卡機編號
                var strEquNo = Request["CtrlNo"] + "_" + strReaderNo;

                // 3. 設備名稱 = 控制器名稱_讀卡機編號 
                var strEquName = Request["CtrlName"] + "_" + strReaderNo;

                // 4. 設備英語名稱 = 設備名稱
                var strEquEName = strEquName;

                // 5. 卡號長度
                var strCardLength = "10";

                #region 取得預設卡號長度
                string ss = "SELECT ParaValue FROM B00_SysParameter WHERE ParaNo='CardDefaultLength' ";
                strCardLength = odo.GetStrScalar(ss);                
                #endregion

                //設定參數
                var para = new
                {
                    ReaderNo = strReaderNo,
                    Readername = strReaderName,
                    CtrlID = CtrlID,
                    UserID = "Saho",
                    ReaderDesc = strReaderName,
                    EquEName = strEquEName,
                    EquNo = strEquNo,
                    Dir = "進",
                    EquClass = "TRT",
                    EquModel = Request["CtrlModel"],
                    Building = "",
                    Floor = "",
                    EquName = strEquName,
                    DciID = DciID,
                    CardNoLen = strCardLength,
                    InToCtrlAreaID = 0,
                    OutToCtrlAreaID = 0,
                    IsAndTrt = 0
                };

                #region 新增預設讀卡機

                string sql1 = @" 
                            INSERT INTO B01_Reader 
                            (
                                ReaderNo,ReaderName,CtrlID,CreateUserID,CreateTime,
                                UpdateUserID,UpdateTime,ReaderDesc,EquNo,[Dir]  
                            ) VALUES (@ReaderNo,@ReaderName,@CtrlID,@UserID,GETDATE(),@UserID,GETDATE(),@ReaderDesc,@EquNo,@Dir)";
                int effect_count = this.odo.Execute(sql1, para);
                #endregion

                #region 新增預設設備
                string sql2 = @" 
	                        INSERT INTO B01_EquData 
	                        (
		                        EquClass,EquModel,EquNo,Building,[Floor],
		                        EquName,EquEName,DciID,CardNoLen,InToCtrlAreaID,
		                        OutToCtrlAreaID,IsAndTrt,  
		                        CreateUserID,CreateTime,UpdateUserID, UpdateTime 
	                        )  VALUES  (@EquClass,@EquModel,@EquNo,@Building,@Floor,@EquName,@EquEName,@DciID,@CardNoLen,@InToCtrlAreaID,@OutToCtrlAreaID,@IsAndTrt,@UserID,GETDATE(),@UserID, GETDATE()) ";
                int effect_count2 = this.odo.Execute(sql2, para);
                #endregion

                if (effect_count == 0 && effect_count2 == 0)
                {
                    this.odo.Execute("DELETE B01_Controller WHERE CtrlID=@CtrlID",new {CtrlID=CtrlID });
                    Response.Clear();
                    Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new { message = "Error", desc = string.Format("設備新增錯誤") }));
                    Response.End();
                }
                else
                {
                    //新增控制器及讀卡機的log
                    var log = SahoAcs.DBClass.SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.資料新增, "Saho", "商合行", "020102");
                    log.LogInfo = string.Format("讀卡機編號：{0}，讀卡機名稱：{1}", strReaderNo,strReaderName);
                    log.LogDesc = "新增讀卡機";
                    this.odo.SetSysLogCreate(log);
                    log.LogInfo = string.Format("設備編號：{0}，設備名稱：{1}", strEquNo, strEquName);
                    log.LogDesc = "新增設備";
                    this.odo.SetSysLogCreate(log);
                }

                #region 新增參數設定
                int EquID = this.odo.GetIntScalar("SELECT IDENT_CURRENT('B01_EquData')");
                string strSQL = @"
                INSERT INTO B01_EquParaData 
                (
                    EquID, EquParaID, ParaValue, M_ParaValue, OpStatus, 
                    UpdateUserID, UpdateTime, SendTime, CompleteTime
                ) 
                SELECT 
                    @EquID, EquParaID, DefaultValue, DefaultValue, 'Setted', @UserID, GETDATE(), GETDATE(), GETDATE()   
                FROM B01_EquParaDef 
                WHERE EquParaID IS NOT NULL AND EquModel=@EquModel
                AND EquParaID NOT IN 
                (SELECT EquParaID FROM B01_EquParaData WHERE EquID=@EquID)";
                this.odo.Execute(strSQL, new {EquID=EquID,UserID="Saho",EquModel=Request["CtrlModel"]});

                #endregion               

            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new { message = "OK"}));
            Response.End();
        }


    }//end page class
}//end namespace