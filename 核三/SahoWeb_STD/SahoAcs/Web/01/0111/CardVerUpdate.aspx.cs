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
    public partial class CardVerUpdate : System.Web.UI.Page
    {
        public OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<OrgStrucInfo> logmaps = new List<OrgStrucInfo>();

        string MainQueryLogStr = @"SELECT OrgNo,OrgStrucID,OrgNoList,OrgNameList,OrgName FROM OrgStrucAllData('Unit') WHERE OrgNoList LIKE @OrgName OR OrgNameList LIKE @OrgName";

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ProcessTime.DateValue = DateTime.Now.AddDays(1).ToString("yyyy/MM/dd HH:mm:ss");
            ClientScript.RegisterClientScriptInclude("CardVerUpdate", "CardVerUpdate.js?"+Pub.GetNowTime);        //加入同一頁面所需的JavaScript檔案
            if (Request["PageEvent"] != null&&Request["PageEvent"]=="Query")
            {
                this.SetQuery();                
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Save")
            {
                SetSaveProcess();
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
                OrgName="%"+Request["PsnNo"].ToString() + "%"
            };                        
            this.logmaps = this.odo.GetQueryResult<OrgStrucInfo>(this.MainQueryLogStr, param).ToList();
            //var logPerson = this.odo.GetQueryResult<CardLogModel>(this.QueryPerson).ToList();
            //var OrgCompany = this.odo.GetQueryResult<OrgStrucInfo>("SELECT * FROM OrgStrucAllData('Company')");          

            //this.logmaps = log1;                  
        }

        private void SetSaveProcess()
        {
            var OrgList = Request.Form.GetValues("ChkStrucID").ToList();
            var InsertData = from i in OrgList select new {CreateUserID=Session["UserID"].ToString(), OrgStrucID = i, CardVer=Request["CardVer"].ToString(), ProcessTime=Request["ProcessTime"].ToString()};
            this.odo.Execute("INSERT INTO B01_UpdateVerProc (OrgStrucID,CardVer,ProcessTime,CreateUserID) VALUES (@OrgStrucID,@CardVer,@ProcessTime,@CreateUserID) ", InsertData);
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
               new { message = "OK", ErrorMessage = "" }));
            Response.End();
        }


        private void SetSaveData()
        {
            var CardList = Request.Form.GetValues("CardID");
            var CardData = this.odo.GetQueryResult<CardVerModel>("SELECT * FROM B01_Card WHERE CardID IN @CardIDList ",new { CardIDList = CardList.Select(i => int.Parse(i)) }).ToList();
            int count = CardData.Count();
            var sql = @" EXEC CardAuth_Update @sCardNo =@CardNo, @sUserID =@UserID,@sFromProc = 'Person',@sFromIP=@IPAddress,@sOpDesc = '更新重整預設權限' ; ";
            CardData.ForEach(i =>
            {
                this.odo.Execute("UPDATE B01_Card SET CardAuthAllow=0 WHERE CardID=@CardID ",i);                
                this.odo.Execute(sql,new {CardNo=i.CardNo, UserID=Session["UserID"], IPAddress=Request.Url.Authority});
                //換卡號版次的動作
                Request["CardVer"].ToString() ;
                this.odo.Execute("UPDATE B01_Card SET CardVer=@CardVer, CardAuthAllow=1 WHERE CardID=@CardID", new {CardID=i.CardID ,CardVer=Request["CardVer"].ToString()});
                this.odo.Execute(sql, new { CardNo = i.CardNo, UserID = Session["UserID"], IPAddress = Request.Url.Authority });
            });
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
               new { message = "OK", ErrorMessage = "" }));
            Response.End();
        }

        
    }//end class
}//end namespace