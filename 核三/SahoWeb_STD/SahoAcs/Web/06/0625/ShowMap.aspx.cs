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
    public partial class ShowMap : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public List<CardLogMap> logs = new List<CardLogMap>();
        public int DefaultRec1 = 0;
        public int DefaultRec2 = 0;

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
                    TableRow tr;
                    TableCell td;
                    tr = new TableRow();
                    int i = 0;
                    if (Request["DefaultRecordID"] != null)
                    {
                        this.DefaultRec1 = int.Parse(Request["DefaultRecordID"]);
                        if (this.logs.Where(o => o.RecordID > this.DefaultRec1).Count() > 0)
                        {
                            this.DefaultRec2 = this.logs.Where(o => o.RecordID > this.DefaultRec1).First().RecordID;
                        }
                        else
                        {
                            this.DefaultRec2 = this.DefaultRec1;
                        }
                        foreach (var log in this.logs)
                        {
                            if (i % 6 == 0)
                            {
                                tr = new TableRow();
                                i = 0;
                            }
                            td = new TableCell();
                            td.Text = log.EquName;
                            td.Text += "<br/>" + string.Format("{0:yyyy/MM/dd HH:mm:ss}", log.CardTime);
                            td.ID = "LogTD" + log.RecordID;
                            if (this.logs.Where(o => o.RecordID > log.RecordID).Count() > 0)
                            {
                                td.Attributes.Add("OnClick", "onSelect('" + td.ClientID + "','" + log.RecordID + "','" + this.logs.Where(o => o.RecordID > log.RecordID).First().RecordID + "')");
                            }
                            else
                            {
                                td.Attributes.Add("OnClick", "onSelect('" + td.ClientID + "','" + log.RecordID + "','" + log.RecordID + "')");
                            }
                            //ParaValueStr = ParaValueStr.Substring(4);
                            td.Style.Add("white-space", "nowrap");
                            td.Style.Add("backcolor", "#FFFFFF");
                            td.Style.Add("Padding", "8px");
                            td.Style.Add("Width", "70px");
                            tr.Controls.Add(td);
                            this.CardLogTable.Controls.Add(tr);
                            i++;
                        }
                    }
                    CardLogTable.Style.Add("word-break", "break-all");
                    CardLogTable.Attributes.Add("border", "1");
                    CardLogTable.Style.Add("border-color", "#999999");
                    CardLogTable.Style.Add("border-style", "solid");
                    CardLogTable.Style.Add("border-collapse", "collapse");
                }
            }


        }//end Page_Load

    }//end class
}//end namespace