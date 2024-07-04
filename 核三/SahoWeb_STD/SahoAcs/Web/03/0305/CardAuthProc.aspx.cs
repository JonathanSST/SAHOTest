using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBClass;


namespace SahoAcs.Web._03._0305
{
    public partial class CardAuthProc : Sa.BasePage
    {
        
        
        OrmDataObject odo = new OrmDataObject("MsSql"
            ,string.Format(Pub.db_connection_template,Pub.db_source_ip,Pub.db_data_name,Pub.db_user_id,Pub.db_pwd));

        public List<CardData> card_list = new List<CardData>();

        public string UserID = "";

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            this.UserID=Sa.Web.Fun.GetSessionStr(this.Page, "UserID");

            if (Request["action"] == null)
            {
                //this.card_list = this.odo.GetQueryResult<CardData>("SELECT * FROM B01_Card WHERE CardType='E' ORDER BY CardNo ").ToList();
                this.SetCardAuth2();
            }
            if (Request["action"] == "Save")
            {
                this.SetCardAuth();
            }
            if (Request["action"] == "SaveBache")
            {
                this.SetCardAuth2();
            }
        }


        private void SetCardAuth()
        {
            this.odo.Execute(@"EXEC CardAuth_Update @sCardNo = @CardNo,
        @sUserID = @UserID,@sFromProc = 'Saho',@sFromIP = '127.0.0.1',
        @sOpDesc = '一般權限重整' ;", new { CardNo = Request["card_no"].ToString(),UserID=this.UserID });
            Response.Clear();
            if (this.odo.isSuccess)
            {
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK", card_no = Request["card_no"].ToString() }));
            }
            else
            {
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "Error", card_no = Request["card_no"].ToString(), error_msg = this.odo.DbExceptionMessage }));
            }
            Response.End();

        }


        private void SetCardAuth2()
        {
            string json;
            using (var reader = new System.IO.StreamReader(Request.InputStream))
            {
                json = reader.ReadToEnd();
            }
            var cards = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<string>>(json);
                //System.Web.Helpers.Json.Decode<List<string>>(json);
               //
                //System.Web.Helpers.Json.Decode<List<string>>(json);
            json = "";
            //List<string> success_list = new List<string>();
            //List<string> error_list = new List<string>();
            string sSuccess = "";
            string sError = "";
            foreach (string card_no in cards)
            {
                if (card_no != "")
                {
                    this.odo.DoCardAuthExec(card_no, this.UserID);
                    if (this.odo.isSuccess)
                    {
                        sSuccess += card_no + ",";
                    }
                    else
                    {
                        this.odo.SetLogs(string.Format("{1:yyyy/MM/dd HH:mm:ss}卡片{0}權限重整失敗....", card_no, DateTime.Now));
                        sError += card_no + ",";                        
                    }
                }
            }
            Response.Clear();            
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK", cards_ok=sSuccess.TrimEnd(','),cards_error=sError}));
            Response.End();

        }
    }//end class

    public class CardData
    {
        public int PsnID { get; set; }
        public int CardID { get; set; }
        public string CardNo { get; set; }
    }

    public class CardDataProc
    {
        public string CardNo { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }

}//end namespace