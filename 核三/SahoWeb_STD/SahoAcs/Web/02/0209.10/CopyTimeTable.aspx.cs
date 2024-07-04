using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;


namespace SahoAcs.Web
{
    public partial class CopyTimeTable : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql",Pub.GetDapperConnString());

        public List<EquAdj> EquModelList = new List<EquAdj>();

        public string TimeName = "";

        public string TimeNo = "";

        public int TimeID = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            #region 查詢時區資料及設備
            if (Request["DoAction"] == "Query")
            {
                string paraName = Request["time_type"] + "Setting";
                var rule = this.odo.GetQueryResult("SELECT * FROM B01_TimeTableDef WHERE TimeNo=@NO ", new {NO=Request["time_no"] });
                this.EquModelList = this.odo.GetQueryResult<EquAdj>(@" SELECT A.ItemName AS EquName,A.ItemNo AS EquModel 
                    FROM B00_ItemList A INNER JOIN B01_EquParaDef B ON A.ItemNo=B.EquModel AND B.ParaName=@ParaName
                     WHERE ItemClass = 'EquModel'",new {ParaName=paraName}).ToList();
                if (rule.Count() > 0)
                {
                    TimeName =Convert.ToString(rule.First().TimeName);
                    TimeNo =Convert.ToString(rule.First().TimeNo);
                    TimeID = Convert.ToInt32(rule.First().TimeID);
                }
            }
            #endregion

            #region 驗證資料正確性
            if (Request["DoAction"] == "CheckData")
            {
                this.TimeNo = Request["time_no"];
                this.TimeName = Request["time_name"];
                string EquModel = Request["equ_model"];

                var counts = this.odo.GetQueryResult("SELECT * FROM B01_TimeTableDef WHERE TimeNo=@NO", new { NO = this.TimeNo }).Count();

                Response.Clear();

                if (counts > 0)
                {
                    Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new { message = "Error", success = false }));
                }
                else
                {                    
                    var source_data = this.odo.GetQueryResult("SELECT * FROM B01_TimeTableDef WHERE TimeID=@TimeID", new {TimeID=Request["time_id"]});
                    if (source_data.Count() > 0)
                    {
                        string TimeInfo = Convert.ToString(source_data.First().TimeInfo);
                        string TimeType = Convert.ToString(source_data.First().TimeType);
                        this.odo.Execute(@"INSERT INTO 
                                                             B01_TimeTableDef (EquModel,TimeType,TimeNo,TimeName,TimeInfo,CreateUserID,CreateTime) 
                                                             VALUES (@EquModel,@TimeType,@TimeNo,@TimeName,@TimeInfo,@CreateUserID,GETDATE())", 
                                                            new {EquModel=EquModel,TimeNo=this.TimeNo,TimeType=TimeType,
                                                                TimeName =this.TimeName,TimeInfo=TimeInfo,CreateUserID=Request["user_id"]});
                    }
                    Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new { message = "OK", success = true }));
                }               
                Response.End();
            }
            #endregion

        }
    }//end class
}//end namespace