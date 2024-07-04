using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using OfficeOpenXml;
using System.Web.Script.Serialization;

namespace SahoAcs.Web
{
    public partial class OrgTRTMapping : System.Web.UI.Page
    {
        public OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public List<OrgStrucInfo> OrgDataList = new List<OrgStrucInfo>();
        public List<CardVerModel> logmaps = new List<CardVerModel>();
        public List<VMcrinfo> TrtEquList = new List<VMcrinfo>();

        string MainQueryLogStr = @"SELECT *,CreateUserID AS OrgName FROM OrgStrucAllData('') WHERE (OrgNameList LIKE @OrgName OR OrgNoList LIKE @OrgName OR OrgStrucNo = @OrgName)";
        string QueryEquData = @"SELECT 
	                                                A.EquID,A.EquName,A.EquNo,A.Building,ISNULL(B.EquID,0) CtrlID  
                                                FROM 
	                                                B01_EquData A
	                                                LEFT JOIN B01_orgTRTMapping B ON A.EquID=B.EquID AND B.OrgIDList=@OrgIDList 
                                                    WHERE A.EquClass='TRT' OR A.IsAndTrt=1 ";

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.RegisterClientScriptInclude("OrgTRTMapping", "OrgTRTMapping.js?"+Pub.GetNowTime);        //加入同一頁面所需的JavaScript檔案
            if (Request.Form["PageEvent"] != null&&Request.Form["PageEvent"]=="Query")
            {
                this.SetQuery();                
            }
            if (Request.Form["PageEvent"] != null && Request.Form["PageEvent"] == "Save")
            {
                SetSaveData();
            }
            if (Request.Form["PageEvent"] != null && Request.Form["PageEvent"] == "QueryEqu")
            {
                this.TrtEquList = this.odo.GetQueryResult<VMcrinfo>(this.QueryEquData, new { OrgIDList = Request.Form["OrgIDList"].ToString().Replace("@",@"\") }).ToList();
            }
            //this.SetQuery();
        }

        protected void ExportButton_Click(object sender,EventArgs e)
        {
            //this.SetQueryByReport();
            //this.ExportExcel();
        }
     

        private void SetQuery()
        {            
            var param = new {
                OrgName = "%"+Request.Form["OrgName"].ToString() + "%"
            };                        
            var DataResult = this.odo.GetQueryResult<OrgStrucInfo>(this.MainQueryLogStr, param).ToList();
            //var logPerson = this.odo.GetQueryResult<CardLogModel>(this.QueryPerson).ToList();
            //var OrgCompany = this.odo.GetQueryResult<OrgStrucInfo>("SELECT * FROM OrgStrucAllData('Company')");
            this.OrgDataList = DataResult;
        }

        private void SetSaveData()
        {
            var EquList = Request.Form.GetValues("EquID").ToList();
            var paraList = from i in EquList select new {EquID=i,OrgIDList=Request.Form["OrgIDList"].ToString().Replace("@",@"\")};            
            this.odo.Execute("DELETE B01_OrgTRTMapping WHERE OrgIDList=@OrgIDList", new {OrgIDList= Request.Form["OrgIDList"].ToString().Replace("@", @"\") });
            this.odo.Execute("INSERT INTO B01_OrgTRTMapping (OrgIDList,EquID) VALUES (@OrgIDList,@EquID)", paraList);
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
               new { message = "OK", ErrorMessage = "" }));
            Response.End();
        }

        
    }//end class
}//end namespace