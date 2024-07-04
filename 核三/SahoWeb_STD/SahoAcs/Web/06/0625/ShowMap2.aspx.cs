using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DapperDataObjectLib;
using SahoAcs.DBModel;


namespace SahoAcs.Web._06._0625
{
    public partial class ShowMap2 : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public List<CardLogMap> logs = new List<CardLogMap>();
        public int DefaultRec0 = 0;
        public int DefaultRec1 = 0;
        public int DefaultRec2 = 0;
        public string EquName = "";
        public string CardTime = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["CardNo"] != null)
            {
                this.logs = this.odo.GetQueryResult<CardLogMap>(@"SELECT 
	                                                A.*,B.EquName,B.EquID
                                                FROM 
	                                                B01_CardLog A
	                                                INNER JOIN B01_EquData B ON A.EquNo=B.EquNo
	                                                WHERE CardNo=@CardNo AND CardTime BETWEEN @DateS AND @DateE ORDER BY CardTime ",
                                                    new { CardNo = Request["CardNo"], DateS = Request["DateS"], DateE = Request["DateE"] }).ToList();
                if (this.logs.Count > 0)
                {
                    
                    if (Request["DefaultRecordID"] != null)
                    {
                        this.DefaultRec1 = int.Parse(Request["DefaultRecordID"]);
                        //取得後一筆刷卡記錄ID
                        if (this.logs.Where(o => o.RecordID > this.DefaultRec1).Count() > 0)
                        {
                            this.DefaultRec2 = this.logs.Where(o => o.RecordID > this.DefaultRec1).First().RecordID;
                        }
                        else
                        {
                            this.DefaultRec2 = this.DefaultRec1;
                        }
                        //取得前一筆刷卡記錄ID
                        if (this.logs.Where(o => o.RecordID < this.DefaultRec1).Count() > 0)
                        {
                            this.DefaultRec0 = this.logs.Where(o => o.RecordID < this.DefaultRec1).Last().RecordID;
                        }
                        else
                        {
                            this.DefaultRec0 = this.DefaultRec1;
                        }
                        this.EquName = this.logs.Where(i => i.RecordID == this.DefaultRec1).FirstOrDefault().EquName;
                        this.CardTime = this.logs.Where(i => i.RecordID == this.DefaultRec1).FirstOrDefault().CardTime.ToString("yyyy/MM/dd HH:mm:ss");
                    }

                }
            }


        }//end Page_Load

    }//end class
}//end namespace