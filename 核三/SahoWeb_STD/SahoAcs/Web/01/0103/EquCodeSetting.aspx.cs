using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using Sa.DB;
using Sa.Web;
using DapperDataObjectLib;
using SahoAcs.DBClass;

namespace SahoAcs.Web._01._0103
{
    public partial class EquCodeSetting : Sa.BasePage
    {
        OrmDataObject od = new OrmDataObject("MsSql",
            string.Format(Pub.db_connection_template, Pub.db_source_ip, Pub.db_data_name, Pub.db_user_id, Pub.db_pwd));
        public DataTable DataResult = new DataTable();
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (Request["DoAction"] != null && Request["DoAction"].ToString() == "Setting")
            {
                string card_id = Request["card_id"];
                string equ_id = Request["equ_id"];
                var result = this.od.GetQueryResult("SELECT * FROM B01_Card WHERE CardID=@CardID", new { CardID = card_id });
                foreach (var c in result)
                {
                    string card_no = Convert.ToString(c.CardNo);
                    this.od.Execute(@"UPDATE B01_CardAuth SET OpMode='Reset',M_CardPW=''
                        OpStatus='',UpdateUserID=@UserID,UpdateTime=GETDATE(),ErrCnt = 0 WHERE CardNo=@CardNo AND EquID=@EquID AND OpMode<>'Del' "
                        , new {CardNo=card_no,EquID=equ_id,UserID=Session["UserID"]});
                }
            }
            else if (Request["DoAction"] != null && Request["DoAction"].ToString() == "All")
            {
                string card_id = Request["card_id"];                
                var result = this.od.GetQueryResult("SELECT * FROM B01_Card WHERE CardID=@CardID", new { CardID = card_id });
                foreach (var c in result)
                {
                    string card_no = Convert.ToString(c.CardNo);
                    this.od.Execute(@"UPDATE B01_CardAuth 
                        SET OpStatus='',OpMode='Reset',M_CardPW='',UpdateUserID=@UserID,UpdateTime=GETDATE(),ErrCnt=0
                        WHERE CardNo=@CardNo AND OpMode<>'Del' ", new { CardNo = card_no, UserID = Session["UserID"] });                    
                }
            }
            else
            {
                string cmd_str = @"SELECT DISTINCT A.*,B.EquName,D.CtrlNo,B.EquNo FROM B01_CardAuth A
                INNER JOIN B01_EquData B ON A.EquID=B.EquID
                INNER JOIN B01_Reader C ON B.EquNo=C.EquNo
                INNER JOIN B01_Controller D ON C.CtrlID=D.CtrlID
                INNER JOIN B01_Card E ON A.CardNo=E.CardNo
                WHERE CardID=@CardID AND A.OpMode<>'Del' ";
                var result = this.od.GetQueryResult(cmd_str, new { CardID = this.GetFormEqlValue("card_id") }).ToList();
                this.DataResult = this.ToDataTable<dynamic>(result);
            }
        }//end page_load


        public DataTable ToDataTable<T>(List<dynamic> items)
        {

            DataTable dtDataTable = new DataTable();
            if (items.Count == 0)
                return dtDataTable;

            ((System.Collections.IEnumerable)items[0]).Cast<dynamic>().Select(p => p.Key).ToList().ForEach(col => { dtDataTable.Columns.Add(col); });

            ((System.Collections.IEnumerable)items).Cast<dynamic>().ToList().
                ForEach(data =>
                {
                    DataRow dr = dtDataTable.NewRow();
                    ((System.Collections.IEnumerable)data).Cast<dynamic>().ToList().ForEach(Col => { dr[Col.Key] = Col.Value; });
                    dtDataTable.Rows.Add(dr);
                });
            return dtDataTable;
        }


    }//end class
}//end namespace