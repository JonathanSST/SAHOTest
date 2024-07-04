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
using System.Threading;

namespace SahoAcs
{
    public partial class LightInfoCopy : Sa.BasePage
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
        public List<DBModel.EquData> ProcessTable = new List<DBModel.EquData>();
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());        
        string[] ChkColParaList;
        public string EquName = "";
        public string CtrlModel = "";
        #endregion


        #region Page_Load
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            #region LoadProcess            

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("ParaSetting", "/Web/02/LightInfoCopy.js?"+Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
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
                    if (Request["PageEvent"] == "Save")
                    {
                        this.SetSaveParaData();                        
                    }                   
                    Response.Clear();
                    Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                        new { message = "OK"}));
                    Response.End();
                }
                else
                {                                    
                    this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                    var CtrlData = this.odo.GetQueryResult("SELECT * FROM B01_Controller WHERE CtrlNo=@CtrlNo", new { CtrlNo = Request["EquNo"] });
                    foreach (var o in CtrlData)
                    {
                        this.hideCtrlID.Value = Convert.ToString(o.CtrlID);
                        this.EquName = Convert.ToString(o.CtrlName);
                        this.CtrlModel = Convert.ToString(o.CtrlModel);
                    }
                        
                    this.ProcessTable = this.odo.GetQueryResult<DBModel.EquData>
                        (@"SELECT * FROM B01_EquData WHERE EquClass='TRT' AND EquModel=@EquModel AND EquNo NOT IN 
                        (SELECT  EquNo  FROM b01_controller C INNER JOIN B01_Reader R ON C.CtrlID=R.CtrlID
                        WHERE CtrlNo=@EquNo)", new { EquModel = this.CtrlModel,EquNo = Request["EquNo"] }).ToList();
                }
            }//判斷
        }
        #endregion
        

        #region Method


        private void SetSaveParaData()
        {
            //hideUserID = Saho
            //hideEquID = 
            //hideCtrlID = 35 
            //PageEvent = Save 
            //ParaType = Controller 
            //CHK_COL_1 = 31 
            //CHK_COL_1 = 32 
            //CHK_COL_1 = 33 
            //CHK_COL_1 = 36 
            //EquNo = M - 105
            //有挑選欲複製目標才處理
            if (!(Request["CHK_COL_1"] == null))
            {
                var aCHK_CO = Request["CHK_COL_1"].Split(',');
                var EquParaList = this.odo.GetQueryResult<EquParaData>
                    ("SELECT A.* FROM B01_EquParaData A INNER JOIN  B01_EquParaDef B " +
                    "ON A.EquParaID = B.EquParaID WHERE A.EquID = @EquID " +
                    "AND (B.ParaDesc LIKE '燈號文字%' OR B.ParaDesc LIKE '跳燈時間%')",
                    new { EquID = Request["hideCtrlID"] });
                string UpdateCmdStr = "";
                int i = 0;
                for (i = 0;i < aCHK_CO.Length; i++)
                {
                    foreach (var o in EquParaList)
                    {
                        var existEquParaID = this.odo.GetQueryResult<EquParaData>("SELECT * FROM B01_EquParaData WHERE EquID = @EquID AND EquParaID=@EquParaID",
                        new { EquID = aCHK_CO[i], o.EquParaID });
                        //沒設過的參數直接新增
                        if (existEquParaID.Count() == 0)
                        {
                            this.odo.Execute(@" INSERT INTO B01_EquParaData 
                                                (EquID,EquParaID,ParaValue,UpdateUserID,UpdateTime) VALUES (@EquID, @EquParaID, @ParaValue, @UserID, GETDATE())",
                                            new { EquID = aCHK_CO[i], @EquParaID = o.EquParaID, @ParaValue = o.ParaValue, @UserID = Request["hideUserID"] });
                        }
                        else
                        {
                            //取得原始設備參數                         
                            UpdateCmdStr = @"UPDATE B01_EquParaData SET ParaValue = @ParaValue,UpdateUserID = @UserID, UpdateTime = GETDATE()";
                            UpdateCmdStr += ", IsReSend = '1', ErrCnt = '0', OpStatus='' ";
                            UpdateCmdStr += " WHERE EquID = @EquID AND EquParaID = @EquParaID ";
                            this.odo.Execute(UpdateCmdStr, new { EquID = aCHK_CO[i], @EquParaID = o.EquParaID, @ParaValue = o.ParaValue, @UserID = Request["hideUserID"] });
                        }
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
                , new { OtherEquID = o.EquID,EquID=pEquID,UserID= Request["hideUserID"] });
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
                     WHERE ParaDef.ParaType = ? AND EquData.EquNo = ?  
                     ORDER BY ParaDef.EquParaID ";

            liSqlPara.Add("S:" + Request["ParaType"]);
            liSqlPara.Add("S:" + Request["EquNo"]);
            oAcsDB.GetDataTable("EquData", sql, liSqlPara, out Process);

            #endregion

            return Process;
        }
        #endregion


        #endregion







    }//end page class
}//end namespace