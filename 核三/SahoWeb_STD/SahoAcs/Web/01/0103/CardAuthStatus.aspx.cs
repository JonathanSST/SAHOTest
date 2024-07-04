using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using CardAuthCore.DataModel;

namespace SahoAcs.Web._01._0103
{
    public partial class CardAuthStatus : Sa.BasePage
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public List<VCardAuthProc> CardAuthList = new List<VCardAuthProc>();
        public string CardNo = "";
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            ClientScript.RegisterClientScriptInclude("CardAuthStatus", "CardAuthStatus.js");//加入同一頁面所需的JavaScript檔案
            this.CardAuthList = this.odo.GetQueryResult<VCardAuthProc>(@"SELECT A.*,B.EquName,B.EquNo FROM B01_CardAuth A  INNER JOIN 
                                            B01_EquData B ON A.EquID=B.EquID INNER JOIN B01_Card C ON C.CardNo=A.CardNo WHERE CardID=@CardID ORDER BY EquNo ", new { CardID = Request["CardID"] }).ToList();
            foreach (var o in this.CardAuthList)
            {
                this.CardNo = o.CardNo;
                if (o.OpMode != "Del" && o.ErrCnt < 5 && new int[] { 1, 2 }.Contains(o.TimeState))
                {
                    o.OpStatus = "設備權限於起迄時間外";
                    o.EquNoList = "2";
                }
                else if (o.ErrCnt >= 5 || o.BeginTime>o.EndTime)
                {
                    o.OpStatus = "設消碼異常";
                    o.EquNoList = "1";
                }
                else if (o.OpMode.Trim() == "" && o.OpStatus == "Setted" && o.TimeState == 0 && o.ErrCnt < 5 && (o.BeginTime <= DateTime.Now && (o.EndTime == null || (o.EndTime>=DateTime.Now&& o.BeginTime<=o.EndTime))))
                {
                    o.OpStatus = "設碼完成";
                    o.EquNoList = "3";
                }
                else if (new string[] {"Del","Reset"}.Contains(o.OpMode) && !new string[] { "Setting", "Resetting", "Deleting" }.Contains(o.OpStatus) && o.TimeState == 0)
                {
                    o.OpStatus = "等待重新設碼或消碼";
                    o.EquNoList = "4";
                }
                else if (o.ErrCnt < 5 && new string[] { "Setting", "Resetting", "Deleting" }.Contains(o.OpStatus) && o.TimeState == 0)
                {
                    o.OpStatus = "重新設消碼中";
                    o.EquNoList = "4";
                }
                else if (o.ErrCnt < 5 && o.OpStatus=="" &&o.OpMode=="" && o.TimeState == 0)
                {
                    o.OpStatus = "等待設碼";
                    o.EquNoList = "4";
                }
            }
        }

    }//end page clsss
}//end namespace