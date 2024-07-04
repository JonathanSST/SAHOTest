using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SahoAcs.DBClass;
using SahoAcs.DBModel;
using DapperDataObjectLib;


namespace SahoAcs.Web._02._0212
{
    public partial class FloorPara : System.Web.UI.Page
    {
        public List<EquParaData> EquParaList = new List<EquParaData>();
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["DoAction"] != null&&Request["DoAction"].ToString()=="Save")
            {
                this.SetParaValue();
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                    new { isSuccess = "OK", UserID = Session["UserID"] }));
                Response.End();
            }
            else
            {
                this.SetBuildData();
            }
        }


        private void SetParaValue()
        {
            string[] ParaNames = Request.Form.GetValues("ParaName");
            string[] ParaValues = Request.Form.GetValues("ParaValue");
            string[] EquParaIDs = Request.Form.GetValues("EquParaID");
            string[] EquIDs = Request.Form.GetValues("EquID");
            string[] M_ParaValues = Request.Form.GetValues("M_ParaValue");
            string[] CtrlIDs = Request.Form.GetValues("CtrlID");
            string[] ParaTypes = Request.Form.GetValues("ParaType");
            string[] EquNames = Request.Form.GetValues("EquName");
            string[] EquNos = Request.Form.GetValues("EquNo");            
            List<EquParaData> paralist = new List<EquParaData>();
            for (int i = 0; i < EquIDs.Length; i++)
            {
                paralist.Add(new EquParaData()
                {
                    EquID = int.Parse(EquIDs[i]),
                    ParaValue = ParaValues[i],
                    ParaName = ParaNames[i],
                    CtrlID = int.Parse(CtrlIDs[i]),
                    ParaType = ParaTypes[i],
                    EquParaID = EquParaIDs[i],
                    EquNo = EquNos[i],
                    EquName=EquNames[i],
                    UserID = Session["UserID"].ToString()
                });
            }
            string update_cmd1 = @"UPDATE B01_EquParaData 
                                        SET ParaValue = @ParaValue, IsReSend = '1', ErrCnt = '0', OpStatus='',
                                        UpdateUserID = @UserID, UpdateTime = GETDATE()
                                    WHERE EquParaID = @EquParaID AND EquID = @EquID";
            string update_cmd2 = @"UPDATE B01_EquParaData 
                                        SET ParaValue = @ParaValue, IsReSend = '1', ErrCnt = '0', OpStatus='',
                                        UpdateUserID = @UserID, UpdateTime = GETDATE()
                                    WHERE EquParaID = @EquParaID 
                                    AND EquID IN 
                                    (
		                                SELECT ED.EquID FROM B01_EquData ED 
		                                INNER JOIN B01_Reader RD ON RD.EquNo = ED.EquNo 
		                                INNER JOIN B01_Controller CR ON CR.CtrlID = RD.CtrlID 
		                                WHERE CR.CtrlID = @CtrlID 
                                    )";
            SysLogEntity syslog_dto = new SysLogEntity();
            syslog_dto.LogType = DB_Acs.Logtype.設備設消碼操作.ToString();
            syslog_dto.UserID = Session["UserId"].ToString();
            syslog_dto.UserName = Session["UserName"].ToString();
            syslog_dto.LogIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            syslog_dto.LogFrom = "0212";
            this.odo.SetSysLogCreate(syslog_dto, paralist);
            foreach (var o in paralist)
            {
                if (o.ParaType == "Reader")
                {                    
                    this.odo.Execute(update_cmd1, o);
                }
                else
                {
                    this.odo.Execute(update_cmd2, o);
                }
            }
        }

       private void SetBuildData()
        {
            string cmd = @"SELECT 
                     ROW_NUMBER()OVER(ORDER BY ParaDef.EquParaID) AS Seq,
                     EquData.EquID, ParaDef.EquParaID,EquName,EquData.EquNo,
                     ParaDef.ParaName, ParaDef.ParaDesc, ParaDef.InputType,
                     ParaDef.ValueOptions,CtrlID,ParaType,
					 CASE InputType WHEN 3 THEN 'ElevPop.aspx' ELSE '' END AS EditFormURL, 
					 ParaDef.FormSize,
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
                     INNER JOIN B01_Reader DR ON DR.EquNo=EquData.EquNo		
                     WHERE 1=1 AND EquData.EquID=@EquID AND ParaName IN ('ElevCtrlOnOpen','ElevAlwysOpen','MgnMode') AND EquData.EquModel IN ('SC300.Elev','SCM320.Elev')
                     ORDER BY ParaDef.EquParaID ";
            this.EquParaList = this.odo.GetQueryResult<EquParaData>(cmd,new { EquID = Request["EquID"] }).ToList();
        }

    }//end class
}//end namespace