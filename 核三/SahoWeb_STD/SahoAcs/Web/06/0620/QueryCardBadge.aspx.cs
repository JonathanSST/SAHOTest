using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;
using DapperDataObjectLib;


namespace SahoAcs.Web._06._0620
{    
    public partial class CardBadge : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        private int _pagesize = 100, _datacount = 1, _pageindex = 0, _pagecount = 0;
        string _sqlcommand, sPsnID;
        Hashtable TableInfo;
        DataTable CardLogTable = null;
        public List<CardAuthList> auth_lists = new List<CardAuthList>();
        public List<MakeCard> make_cards = new List<MakeCard>();
        #endregion

        OrmDataObject od = new OrmDataObject("MsSql",
            string.Format(Pub.db_connection_template, Pub.db_source_ip, Pub.db_data_name, Pub.db_user_id, Pub.db_pwd));

        protected void Page_Load(object sender, EventArgs e)
        {
            
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            //js += "\nSetMode('');";
            //js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            #endregion            

            if (!IsPostBack)
            {

            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];
                if (sFormTarget == "NewQuery")
                {
                    this.Query(sFormArg);
                }
            }
        }

        private void Query(string psn_no)
        {
            string query_no = psn_no;
            //取得人員基本資料
            var psn_items = this.od.GetQueryResult(@"SELECT p.*,c.CardNo,mc.CertNo
                                FROM B01_Person p INNER JOIN B01_Card c ON p.PsnID=c.PsnID 
                                LEFT JOIN B05_MakeCard mc ON mc.PsnNo=p.PsnNo
                                WHERE 
                                p.PsnNo = @PsnNo OR c.CardNo = @PsnNo OR PsnName = @PsnNo", 
                                new {PsnNo=query_no});
            //List<OrgData> org_data_list = this.od.GetQueryResult<OrgData>("SELECT * FROM B01_OrgData").ToList();
            var items = this.od.GetQueryResult<ItemInfo>
                (@"SELECT B01_OrgData.OrgClass AS ItemNo, B00_ItemList.ItemName, B00_ItemList.ItemInfo2 AS Info2,
                B00_ItemList.ItemOrder FROM B01_OrgData 
                INNER JOIN B00_ItemList ON B01_OrgData.OrgClass = B00_ItemList.ItemNo
                WHERE B00_ItemList.ItemClass = 'OrgClass'
                GROUP BY B01_OrgData.OrgClass, B00_ItemList.ItemName, B00_ItemList.ItemInfo2, B00_ItemList.ItemOrder
                ORDER BY B00_ItemList.ItemOrder ").ToList();
            List<OrgStrucAll> org_struc_all = this.od.GetQueryResult<OrgStrucAll>("SELECT * FROM OrgStrucAllData('')").ToList();            
            foreach (var o in psn_items)
            {
                var orgs = org_struc_all.Where(i => i.OrgStrucID == Convert.ToInt32(o.OrgStrucID)).FirstOrDefault();
                string[] org_no = orgs.OrgNoList.Replace(@"\", "|").Split('|').Where(i=>i!="").ToArray();
                string[] org_name = orgs.OrgNameList.Replace(@"\", "|").Split('|').Where(i=>i!="").ToArray();
                this.txt_AccountNo.Text = Convert.ToString(o.PsnNo);
                this.txt_BadgeNo.Text = Convert.ToString(o.CertNo);
                this.txt_Name.Text = Convert.ToString(o.PsnName);
                this.txt_PsnETime.Text = o.PsnETime!=null?string.Format("{0:yyyy/MM/dd}", o.PsnETime):"";
                this.txt_PsnSTime.Text = string.Format("{0:yyyy/MM/dd}", o.PsnSTime);
                this.txt_CardNo.Text = Convert.ToString(o.CardNo);
                this.txt_EName.Text = Convert.ToString(o.PsnEName);
                this.txt_Desc.Text = Convert.ToString(o.Remark);
                for (int i = 0; i < items.Count();i++ )
                {
                    if (i<org_no.Length && org_no[i].Substring(0, 1) == items[i].Info2)
                    {
                        if (items[i].Info2 == "D")
                        {
                            items[i].ItemNo = "Unit";
                        }
                        TextBox txtObj = this.UpdatePanel1.FindControl("txt_" + items[i].ItemNo) as TextBox;
                        if (txtObj != null)
                        {
                            txtObj.Text = org_name[i];
                        }
                    }
                }
                this.auth_lists = this.od.GetQueryResult<CardAuthList>(@"SELECT
	                    e.*,d.CardNo,d.OpStatus,
	                    ISNULL(f.ParaValue+'-'+t.TimeName,'0-預設') AS TimeName
                    from 
	                    B01_Card c
	                    INNER JOIN B01_CardAuth d ON c.CardNo=d.CardNo
	                    INNER JOIN B01_EquData e ON d.EquID=e.EquID
	                    LEFT JOIN B01_EquParaData f ON d.EquID=f.EquID
	                    LEFT JOIN B01_EquParaDef g ON f.EquParaID=g.EquParaID AND g.ParaName='CreadReaderSetting'
	                    LEFT JOIN B01_TimeTableDef t ON t.EquModel=e.EquModel AND f.ParaValue=CONVERT(VARCHAR,t.TimeID)
                    WHERE
	                    d.OpStatus='Setted' AND c.PsnID=@PsnID", new {PsnID=Convert.ToInt32(o.PsnID)}).ToList();
                //取得車證資料
                this.make_cards = this.od.GetQueryResult<MakeCard>(@"SELECT PsnNo,CertNo,CarA1,CarA2,CarA3,'Car' AS CarType,zonetype from B05_MakeCard 
                    LEFT JOIN PData ON CertNo=PID
                    WHERE PsnNo=@PsnNo                    
                    UNION
                    SELECT PsnNo,CertNo,CarB1,CarB2,CarB3,'Motor' AS CarType,zonetype from B05_MakeCard 
                    LEFT JOIN PData ON CertNo=PID
                    WHERE PsnNo=@PsnNo       "
                    , new { PsnNo = o.PsnNo }).ToList();               
            }
            this.UpdatePanel1.Update();
        }

        public class CardAuthList
        {
            public string CardNo { get; set; }
            public string EquName { get; set; }
            public string EquNo { get; set; }
            public string OpStatus { get; set; }
            public string TimeName { get; set; }
        }

        public class OrgData
        {
            public int OrgID { get; set; }
            public string OrgNo { get; set; }
            public string OrgName { get; set; }
            public string OrgClass { get; set; }
        }

        public class ItemInfo
        {
            public string Info2 { get; set; }
            public string ItemNo { get; set; }
        }

        public class OrgStrucAll
        {
            public int OrgStrucID { get; set; }
            public string OrgStrucNo { get; set; }
            public string OrgIDList { get; set; }
            public string OrgNameList { get; set; }
            public string OrgNoList { get; set; }

        }

        public class MakeCard
        {
            public string PsnNo { get; set; }
            public string CarA1 { get; set; }
            public string CarType { get; set; }
            public string CertNo { get; set; }
            public string zonetype { get; set; }
            public string CarA2 { get; set; }
            public string CarA3 { get; set; }

            public string CarB1 { get; set; }
            public string CarB2 { get; set; }
            public string CarB3 { get; set; }
        }

    }//end class
}//end namespace