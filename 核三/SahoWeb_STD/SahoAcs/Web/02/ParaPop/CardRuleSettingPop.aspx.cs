using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs
{
    public partial class CardRuleSettingPop : Sa.BasePage
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        public DataTable ProcessTable = new DataTable();
        DataRow ProcessRow;
        DataColumn UICol1, UICol2, UICol3, UICol4;
        DataTable CardRuleTable = new DataTable();

        #endregion

        #region Events

        #region Page_Load
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";
            
            ClientScript.RegisterClientScriptInclude("CardRuleSetting", "/Web/02/ParaPop/CardRuleSettingPop.js");//加入同一頁面所需的JavaScript檔案            

            if (!IsPostBack)
            {
                if (Request["PageEvent"] != null)
                {
                    this.SetCheckRule();
                }
                else
                {
                    #region Give hideValue
                    hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                    this.hideEquID.Value = Request["EquID"];
                    this.hideEquParaID.Value = Request["EquParaID"];
                    this.hideParaValue.Value = Request["ParaValue"];

                    CreateDropDownCardRule();

                    #region CardRuleTable
                    UICol1 = new DataColumn("CardRuleIndex", typeof(string));
                    UICol2 = new DataColumn("CardRule", typeof(string));
                    UICol3 = new DataColumn("CardRuleName", typeof(string));
                    UICol4 = new DataColumn("CardRuleSort", typeof(string));
                    if (ViewState["CardRuleTable"] == null)
                    {
                        this.ProcessTable.Columns.Add(UICol1);
                        this.ProcessTable.Columns.Add(UICol2);
                        this.ProcessTable.Columns.Add(UICol3);
                        this.ProcessTable.Columns.Add(UICol4);
                    }
                    //ViewState["CardRuleTable"] = ProcessTable;
                    #endregion

                    popL_FunctionRemind.Text = "提醒：時區規則為0~99，且時區編號不得重覆。";
                    this.LoadRowToDatatable(this.hideParaValue.Value);
                    this.popInput_CardRuleIndex.Text = this.ProcessTable.Rows.Count.ToString();
                    #endregion
                }

            }
        }
        #endregion
        

        private void SetCheckRule()
        {
            var HasRule = false;
            string CardRule = Request["CardRule"];
            string ReturnRule = "";
            DapperDataObjectLib.OrmDataObject odo = new DapperDataObjectLib.OrmDataObject("MsSql", Pub.GetDapperConnString());
            foreach(var rule in CardRule.Split(','))
            {
                if (odo.GetQueryResult(@"SELECT EquID,CardRule FROM B01_CardEquAdj WHERE EquID=@EquID AND CardRule=@Rule 
                                                                  UNION SELECT EquID,CardRule FROM B01_CardEquAdj WHERE EquID=@EquID AND CardRule=@Rule"
                         , new { EquID = Request["EquID"], Rule = rule }).Count() > 0)
                {
                    HasRule = true;
                    ReturnRule = rule;
                    break;
                }
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK", has_rule = HasRule,rule=ReturnRule }));
            Response.End();
        }


        #region popB_Save_Click
        protected void popB_Save_Click(object sender, EventArgs e)
        {
            string CardRuleStr = "";

            ProcessTable = (DataTable)ViewState["CardRuleTable"];

            for (int i = 0; i < ProcessTable.Rows.Count; i++)
            {
                if (CardRuleStr != "") CardRuleStr += ",";
                    CardRuleStr += int.Parse(ProcessTable.Rows[i]["CardRuleIndex"].ToString()).ToString() + ":" + ProcessTable.Rows[i]["CardRule"].ToString();
            }
            //Sa.Web.Fun.RunJavaScript(this, "window.returnValue = '" + CardRuleStr + "';");
            //Sa.Web.Fun.RunJavaScript(this, "window.close();");
        }
        #endregion

        #endregion

        #region Method

        #region CreateDropDownCardRule
        protected void CreateDropDownCardRule()
        {
            System.Web.UI.WebControls.ListItem Item;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt;

            sql = @" SELECT CardRuleDef.RuleNo + ' ( ' + CardRuleDef.RuleName + ' )' AS CardRuleName, CardRuleDef.RuleID AS CardRuleValue
                     FROM B01_EquData AS EquData
                     LEFT JOIN B01_CardRuleDef AS CardRuleDef ON CardRuleDef.EquModel = EquData.EquModel
                     WHERE EquData.EquID = ? ";
            liSqlPara.Add("S:" + hideEquID.Value);

            oAcsDB.GetDataTable("CardRuleDef", sql, liSqlPara, out dt);

            foreach (DataRow dr in dt.Rows)
            {
                Item = new System.Web.UI.WebControls.ListItem();
                Item.Text = dr["CardRuleName"].ToString();
                Item.Value = dr["CardRuleValue"].ToString();
                this.popInput_CardRule.Items.Add(Item);
            }
        }
        #endregion

        #region LoadCardRule
        public void LoadCardRule()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            sql = @" SELECT * FROM B01_CardRuleDef ";
            oAcsDB.GetDataTable("CardRuleTable", sql, out CardRuleTable);
        }
        #endregion

        #region LoadRowToDatatable
        public void LoadRowToDatatable(string ParaValue)
        {
            string[] ItemList, Itemarray; ;
            if (!string.IsNullOrEmpty(ParaValue))
            {
                ItemList = ParaValue.Split(',');
                for (int i = 0; i < ItemList.Length; i++)
                {
                    Itemarray = ItemList[i].Split(':');
                    ProcessRow = ProcessTable.NewRow();
                    ProcessRow["CardRuleIndex"] = Itemarray[0].PadLeft(2, '0');
                    ProcessRow["CardRuleSort"] = Itemarray[0];
                    foreach (System.Web.UI.WebControls.ListItem s in this.popInput_CardRule.Items)
                    {                        
                        if (s.Value == Itemarray[1])
                        {
                            ProcessRow["CardRule"] =Itemarray[1];
                            ProcessRow["CardRuleName"] = s.Text;
                        }
                    }                    
                    this.ProcessTable.Rows.Add(ProcessRow);
                }
            }
            //return ProcessTable;
        }
        #endregion

        #region AddRowToDatatable
        protected DataTable AddRowToDatatable()
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            DataTable ProcessTable = (DataTable)ViewState["CardRuleTable"];
            if (ProcessTable.Rows.Count < 100)
            {
                for (int i = 0; i < ProcessTable.Rows.Count; i++)
                {
                    if (ProcessTable.Rows[i]["CardRuleIndex"].ToString() == this.popInput_CardRuleIndex.Text.ToString().PadLeft(2, '0'))
                    {
                        objRet.result = false;
                        objRet.message += "規則編號重復！";
                    }
                }
            }
            else
            {
                objRet.result = false;
                objRet.message += "規則已滿！";
            }

            if (objRet.result)
            {
                ProcessRow = ProcessTable.NewRow();
                ProcessRow["CardRuleIndex"] = this.popInput_CardRuleIndex.Text.Trim().PadLeft(2, '0');
                ProcessRow["CardRule"] = this.popInput_CardRule.SelectedValue;
                ProcessTable.Rows.Add(ProcessRow);
            }
            else
                Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('" + objRet.message.ToString() + "');");
            ProcessTable = DataTableSort(ProcessTable, "CardRuleIndex ASC");
            ViewState["CardRuleTable"] = ProcessTable;
            return ProcessTable;
        }
        #endregion

        #region DataTableSort
        public DataTable DataTableSort(DataTable BeSortTable, string SortRule)
        {
            DataTable SortTable;

            SortTable = BeSortTable.Clone();
            foreach (DataRow dr in BeSortTable.Select(" 1 = 1 ", SortRule))
            {
                SortTable.ImportRow(dr);
            }
            BeSortTable.Clear();
            return SortTable;
        }
        #endregion

        #endregion
    }
}
