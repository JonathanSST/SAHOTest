using SahoAcs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;


namespace SahoAcs
{
    public partial class ParaSettingBox : Sa.BasePage
    {
        public class EquParaData
        {
            public string EquID { get; set; }
            public string EquParaID { get; set; }
            public string ParaValue { get; set; }
            public string UserID { get; set; }
            public string ParaType { get; set; }

            public string ParaDesc { get; set; }
            public string ParaUI { get; set; }
        }

        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        DataTable ParaTable = new DataTable();
        public DataTable ProcessTable = new DataTable();
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        List<EquParaData> para_list = new List<EquParaData>();
        string[] ChkColParaList;
        public string EquName = "";
        #endregion


        #region Page_Load
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            #region LoadProcess            

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("ParaSetting", "/Web/02/ParaSettingBox.js");//加入同一頁面所需的JavaScript檔案
            //ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            //ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            //ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            //ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");

            #endregion

            if (!IsPostBack)
            {
                if (Request["PageEvent"] != null)
                {
                    //執行資料儲存                    
                  
                    foreach (string s in Request.Form.GetValues("EquParaID").ToList())
                    {
                        int index = Request.Form.GetValues("EquParaID").ToList().IndexOf(s);
                        para_list.Add(new EquParaData()
                        {
                            EquParaID = s,
                            EquID = Request["hideEquID"],
                            ParaValue = Request.Form.GetValues("ParaValue")[index],
                            ParaUI=Request.Form.GetValues("ParaUI")[index],
                            ParaType = Request.Form["ParaType"],
                            UserID=Request["hideUserID"]                            
                        });
                    }
                    if (Request.Form["ParaType"] == "Reader")
                    {
                        this.ParaTable = LoadData();
                    }
                    else
                    {
                        this.ParaTable = this.LoadDataForControl();
                    }
                    
                    Pub.MessageObject objRet = this.CheckData(para_list);
                    if (Request["PageEvent"] == "Save")
                    {
                        this.SetSaveParaData();                        
                    }
                    if(Request["PageEvent"]=="Refresh")
                    {
                        this.ChkColParaList = Request.Form.GetValues("CHK_COL_1");
                        this.SetRefreshParaData();
                    }
                    if(Request.Form["ParaType"]!=null && Request.Form["ParaType"] == "Controller")
                    {
                        this.SetCopyParaToOtherReader(Request.Form["hideEquID"], Request.Form["hideCtrlID"]);
                    }
                    Response.Clear();
                    Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                        new { message = "OK"}));
                    Response.End();
                }
                else
                {
                    #region Give hideValue                    
                   
                    this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                    #endregion
                    if (Request.Form["ParaType"] == "Reader")
                    {
                        this.ProcessTable = LoadData();
                    }
                    else
                    {
                        var CtrlData = this.odo.GetQueryResult("SELECT * FROM B01_Controller WHERE CtrlNo=@CtrlNo", new { CtrlNo = Request.Form["EquNo"] });
                        foreach (var o in CtrlData)
                            this.hideCtrlID.Value = Convert.ToString(o.CtrlID);
                        this.ProcessTable = LoadDataForControl();
                    }
                    foreach(DataRow r in this.ProcessTable.Rows)
                    {
                        this.hideEquID.Value = Convert.ToString(r["EquID"]);
                        this.EquName = Convert.ToString(r["EquName"]);
                    }
                }
            }//判斷
        }
        #endregion
        

        #region Method


        private void SetSaveParaData()
        {
            var EquParaList = this.odo.GetQueryResult<EquParaData>("SELECT * FROM B01_EquParaData WHERE EquID=@EquID", new { EquID = Request.Form["hideEquID"] });
            string UpdateCmdStr = "";
            foreach(var o in this.para_list)
            {
                
                if(EquParaList.Where(i=>i.EquID==o.EquID&&i.EquParaID==o.EquParaID).Count() == 0)
                {
                    this.odo.Execute(@" INSERT INTO B01_EquParaData 
                                        (EquID,EquParaID,ParaValue,UpdateUserID,UpdateTime) 
                                        VALUES (@EquID, @EquParaID, @ParaValue, @UserID, GETDATE())", o);
                }
                else
                {
                    //取得原始設備參數                         
                    UpdateCmdStr = @"UPDATE B01_EquParaData 
                                         SET ParaValue = @ParaValue,UpdateUserID = @UserID, UpdateTime = GETDATE()";
                    
                    if(o.ParaValue!= EquParaList.Where(i => i.EquID == o.EquID && i.EquParaID == o.EquParaID).First().ParaValue)
                    {
                        UpdateCmdStr += ", IsReSend = '1', ErrCnt = '0', OpStatus='' ";
                    }
                    UpdateCmdStr += " WHERE EquID = @EquID AND EquParaID = @EquParaID ";
                    this.odo.Execute(UpdateCmdStr, o);
                    #region 若有必要，須同時進行另一台設備的更新，但不做指令送出

                    #endregion
                }
            }
        }


        private void SetRefreshParaData()
        {
            if (this.ChkColParaList != null)
            {
                foreach (var o in this.para_list.Where(i => this.ChkColParaList.Contains(i.EquParaID)))
                {
                    if (this.odo.GetQueryResult("SELECT * FROM B01_EquParaData WHERE EquID=@EquID AND EquParaID=@EquParaID ", o).Count() == 0)
                    {
                        this.odo.Execute(@" INSERT INTO B01_EquParaData 
                                        (EquID,EquParaID,ParaValue,UpdateUserID,UpdateTime,IsResend,SendTime) 
                                        VALUES (@EquID, @EquParaID, @ParaValue, @UserID,GETDATE(),1,GETDATE())", o);
                    }
                    else
                    {
                        this.odo.Execute(@"UPDATE B01_EquParaData 
                                         SET ParaValue = @ParaValue, IsReSend = '1', ErrCnt = '0', OpStatus='',
                                         UpdateUserID = @UserID, UpdateTime = GETDATE(), SendTime = GETDATE()
                                         WHERE EquID = @EquID AND EquParaID = @EquParaID", o);
                    }
                }
            }
            
        }


       private void SetCopyParaToOtherReader(string pEquID,string pCtrlID)
        {
            var others = this.odo.GetQueryResult(@"SELECT * FROM B01_EquData ED 
	                    INNER JOIN B01_Reader RD ON RD.EquNo = ED.EquNo 
	                    INNER JOIN B01_Controller CR ON CR.CtrlID = RD.CtrlID 
	                    WHERE  EquID<>@EquID AND CR.CtrlID=@CtrlID ",new {EquID=pEquID,CtrlID=pCtrlID });
            foreach(var o in others)
            {
                this.odo.Execute(@"DELETE B01_EquParaData 
                WHERE EquID=@EquID 
                AND EquParaID IN (SELECT EquParaID FROM B01_EquParaDef WHERE ParaType='Controller') ",new {EquID=o.EquID});
                //重建參數
                this.odo.Execute(@"INSERT INTO B01_EquParaData (EquID,EquParaID,ParaValue,M_ParaValue,UpdateUserID,UpdateTime,IsResend,OpStatus) 
                        SELECT @OtherEquID,EquParaID,ParaValue,ParaValue,@UserID,GETDATE(),'0','Setted' FROM B01_EquParaData
                        WHERE EquID=@EquID AND EquParaID IN (SELECT EquParaID FROM B01_EquParaDef WHERE ParaType='Controller') "
                , new { OtherEquID = o.EquID,EquID=pEquID,UserID= Request.Form["hideUserID"] });
            }
            
        }


        #region LoadData
        public DataTable LoadData()
        {
            DataTable Process = new DataTable();
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = @" SELECT 
                     ROW_NUMBER()OVER(ORDER BY ParaDef.EquParaID) AS Seq,
                     EquData.EquID, ParaDef.EquParaID,EquName,
                     ParaDef.ParaName, ParaDef.ParaDesc, ParaDef.InputType AS ParaUI,
                     ParaDef.ValueOptions, ParaDef.EditFormURL, ParaDef.FormSize,
                     ParaDef.MinValue, ParaDef.MaxValue, ParaDef.DefaultValue,
                     ISNULL(CASE WHEN ParaName = 'ElevCtrlOnOpen' AND ParaValue IS NULL THEN 
						'FFFFFFFFFFFF'
					 WHEN ParaName = 'CardTransferMode' AND EquData.EquModel='SCM320.Elev' AND ParaValue IS NULL THEN 
						'000108'
					WHEN ParaName = 'CardTransferMode' AND EquData.EquModel='SCM320' AND ParaValue IS NULL THEN 
						'000008'
                     ELSE 
						ParaData.ParaValue END,'') AS ParaValue,
                    CASE WHEN ParaName = 'ElevCtrlOnOpen' AND ParaValue IS NULL THEN 
						'FFFFFFFFFFFF'
					 WHEN ParaName = 'CardTransferMode' AND EquData.EquModel='SCM320.Elev' AND ParaValue IS NULL THEN 
						'000108'
					WHEN ParaName = 'CardTransferMode' AND EquData.EquModel='SCM320' AND ParaValue IS NULL THEN 
						'000008'
                     ELSE 
						ParaData.ParaValue End as ParaValueO, 
                     ParaData.M_ParaValue,
                     ParaData.UpdateTime, ParaData.SendTime, ParaData.CompleteTime,
                     '' AS ParaStatus,
                     Replace(STUFF( (SELECT ',' + CAST( IOIndex as NVARCHAR ) + ':' + CAST( FloorName as NVARCHAR ) 
                             FROM B01_ElevatorFloor 
                             WHERE EquID = EquData.EquID AND (ParaDef.ParaName = 'ElevCount' OR ParaDef.ParaName = 'ElevAlwysOpen' OR ParaDef.ParaName = 'ElevCtrlOnOpen') FOR XML PATH(''))
                     ,1,1,''),'&amp;','&') AS FloorName 
                     FROM B01_EquData AS EquData 
                     LEFT JOIN B01_EquParaDef AS ParaDef ON ParaDef.EquModel = EquData.EquModel
                     LEFT JOIN B01_EquParaData AS ParaData ON ParaData.EquID = EquData.EquID AND ParaData.EquParaID = ParaDef.EquParaID
                     WHERE ParaDef.ParaType = @ParaType AND EquData.EquNo = @EquNo 
                     ORDER BY ParaDef.EquParaID ";
            var result = this.odo.GetQueryResult(sql, new {ParaType=Request.Form["ParaType"],EquNo=Request.Form["EquNo"]});
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            Process = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(DataTable));
            //oAcsDB.GetDataTable("EquData", sql, liSqlPara, out Process);

            #endregion

            return Process;
        }
        #endregion

        #region LoadDataForControl
        public DataTable LoadDataForControl()
        {
            DataTable Process = new DataTable();
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();

            #region Process String

            // 1. 加入控制器參數的判斷 (ParaDef.ParaType = ? )
            // 2. 從 CtrlID 取得 EquNo 其一做為修改的標準，用 CreateTime 最舊的那筆。

            sql = @" 
                SELECT 
                    ROW_NUMBER() OVER(ORDER BY ParaDef.EquParaID) AS Seq,
                    EquData.EquID, ParaDef.EquParaID,EquName,
                    ParaDef.ParaName, ParaDef.ParaDesc, ParaDef.InputType AS ParaUI,
                    ParaDef.ValueOptions, ParaDef.EditFormURL, ParaDef.FormSize,
                    ParaDef.MinValue, ParaDef.MaxValue, ParaDef.DefaultValue,
                    ISNULL(
                        CASE 
                        WHEN ParaName='ElevCtrlOnOpen' AND ParaValue IS NULL THEN 'FFFFFFFFFFFF'
					    WHEN ParaName='CardTransferMode' AND EquData.EquModel='SCM320.Elev' 
                            AND ParaValue IS NULL THEN '000108'
				        WHEN ParaName='CardTransferMode' AND EquData.EquModel='SCM320' 
                            AND ParaValue IS NULL THEN '000008'
                        ELSE 
					        ParaData.ParaValue END, ''
                    ) AS ParaValue,
                    CASE 
                    WHEN ParaName='ElevCtrlOnOpen' AND ParaValue IS NULL THEN 'FFFFFFFFFFFF'
					WHEN ParaName='CardTransferMode' AND EquData.EquModel='SCM320.Elev' 
                        AND ParaValue IS NULL THEN '000108'
				    WHEN ParaName='CardTransferMode' AND EquData.EquModel='SCM320' 
                        AND ParaValue IS NULL THEN '000008'
                    ELSE 
					ParaData.ParaValue End AS ParaValueO, 
                    ParaData.M_ParaValue,
                    ParaData.UpdateTime, ParaData.SendTime, ParaData.CompleteTime,
                    '' AS ParaStatus,
                    Replace(STUFF(
                        (SELECT ',' + CAST( IOIndex as NVARCHAR ) + ':' + CAST( FloorName as NVARCHAR ) 
                            FROM B01_ElevatorFloor 
                            WHERE EquID = EquData.EquID AND (ParaDef.ParaName = 'ElevCount' OR ParaDef.ParaName = 'ElevAlwysOpen' OR ParaDef.ParaName = 'ElevCtrlOnOpen') FOR XML PATH(''))
                    ,1,1,''),'&amp;','&') AS FloorName 
                FROM B01_EquData AS EquData 
                LEFT JOIN B01_EquParaDef AS ParaDef ON ParaDef.EquModel = EquData.EquModel
                LEFT JOIN B01_EquParaData AS ParaData ON ParaData.EquID = EquData.EquID AND ParaData.EquParaID = ParaDef.EquParaID
                WHERE ParaDef.ParaType = @ParaType
                AND EquData.EquNo = 
                (
	                SELECT TOP 1 ED.EquNo FROM B01_EquData ED 
	                INNER JOIN B01_Reader RD ON RD.EquNo = ED.EquNo 
	                INNER JOIN B01_Controller CR ON CR.CtrlID = RD.CtrlID 
                    WHERE CR.CtrlNo = @EquNo 
                    ORDER BY CR.CreateTime 
                )
                ORDER BY ParaDef.EquParaID ";

            //liSqlPara.Add("S:" + Request["ParaType"]);
            //liSqlPara.Add("S:" + Request["EquNo"]);
            //oAcsDB.GetDataTable("EquData", sql, liSqlPara, out Process);
            var result = this.odo.GetQueryResult(sql, new { ParaType = Request.Form["ParaType"], EquNo = Request.Form["EquNo"] });
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            Process = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(DataTable));
            #endregion

            return Process;
        }
        #endregion



        #region CheckData
        protected Pub.MessageObject CheckData(List<EquParaData> ParaDatas)
        {            
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";            
            int tempint;
            
            foreach (var para in ParaDatas)
            {
                int index = ParaDatas.IndexOf(para);
                var row = this.ParaTable.Rows[index];
                switch (para.ParaUI)
                {
                    case "0":
                        #region 文字欄位
                        if (para.ParaValue.Length > 1024)
                        {
                            if (!string.IsNullOrEmpty(objRet.message))
                                objRet.message += "\\n";
                            objRet.result = false;
                            objRet.message += row["ParaDesc"].ToString() + " 字數超過上限";
                        }
                        #endregion
                        break;
                    case "1":
                        #region 數字數值
                        if (!string.IsNullOrEmpty(para.ParaValue))
                        {
                            if (!int.TryParse(para.ParaValue, out tempint))
                            {
                                if (!string.IsNullOrEmpty(objRet.message))
                                    objRet.message += "\\n";
                                objRet.result = false;
                                objRet.message += row["ParaDesc"].ToString() + " 必需為數字";
                            }
                            if (para.ParaValue.Length > 1024)
                            {
                                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\\n";
                                objRet.result = false;
                                objRet.message += para.ParaDesc + " 字數超過上限";
                            }
                            if (int.TryParse(para.ParaValue, out tempint))
                            {
                                int minvalue = int.Parse(row["MinValue"].ToString());
                                int maxvalue = int.Parse(row["MaxValue"].ToString());
                                if (tempint < minvalue || tempint > maxvalue)
                                {
                                    objRet.result = false;
                                    objRet.message += row["ParaDesc"].ToString() + " 需介於 " + minvalue + " ~ " + maxvalue + " 之間。";
                                }
                            }
                        }
                        #endregion
                        break;
                }
            }
            objRet.act = "CheckData";
            return objRet;
        }
        #endregion

       
        #endregion
    }
}