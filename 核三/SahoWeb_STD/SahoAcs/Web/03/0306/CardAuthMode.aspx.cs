using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using System.Collections;
using System.Data;
using System.IO;
using OfficeOpenXml;
using SahoAcs.DBModel;


namespace SahoAcs.Web._03._0306
{
    public partial class CardAuthMode : System.Web.UI.Page
    {
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        OrmDataObject odo = new OrmDataObject("MsSql",Pub.GetDapperConnString());
        private int _pagesize = 100, _datacount = 1, _pageindex = 0, _pagecount = 0;

        public List<CardEntity> CardList = new List<CardEntity>();
        public List<EquData> EquList = new List<EquData>();
        public List<ResetCardData> logs = new List<ResetCardData>();
        public int group_count = 0;
        public string OldPsnName = "";


        #region VerifyRenderingInServerForm
        public override void VerifyRenderingInServerForm(Control control)
        {
            // 修正 'XX'型別 必須置於有 runat=server 的表單標記之中 Override此Methods
        }
        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            this.oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            this.oScriptManager.EnablePageMethods = true;
            this.hideUserID.Value = Session["UserID"].ToString();
            string js = "<script type='text/javascript'>OnLoad();</script>";
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("CardAuthMode", "CardAuthMode.js");        //加入同一頁面所需的JavaScript檔案

            if (!IsPostBack)
            {
                this.EquList = this.odo.GetQueryResult<EquData>(@"SELECT A.*,B.ItemInfo3 FROM B01_EquData A
                                                                                    INNER  JOIN B00_ItemList B ON A.EquModel=B.ItemNo
                                                                                    WHERE 
                                                                                    ItemClass='EquModel' AND (ItemInfo3='ESD' OR ItemNo='ADM100FP')").ToList();
                if (Request["PageEvent"] == "Save"&&this.Request["ChkCardNo"] !=null)
                {
                    string[] CardNoList = Request.Form.GetValues("ChkCardNo");
                    string[] EquIDList = Request.Form.GetValues("EquID");
                    string[] VerifiList = Request.Form.GetValues("VerifiMode");
                    foreach(var cardno in CardNoList)
                    {
                        int index = 0;
                        foreach(var equid in EquIDList)
                        {                            
                            odo.Execute("UPDATE B01_CardAuth SET UpdateUserID=@UserID,UpdateTime=GETDATE(),OpMode = 'Reset', OpStatus = '', ErrCnt = 0,VerifiMode=@VerifiMode WHERE EquID=@EquID AND CardNo=@CardNo AND OpMode <> 'Del' "
                                , new { CardNo = cardno, EquID = equid, VerifiMode = VerifiList[index], UserID=Request["UserID"] });
                            index++;
                        }
                    }
                    Response.Write("更新完成");
                }
                if(Request["PageEvent"]=="Query")
                {
                    this.Query(Request["QueryType"], Request["QueryName"]);
                }                
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];             
            }
        }

       

        #region Query
        private void Query(string QueryType,string QueryName)
        {
            string MainSqlCmd = @"SELECT DISTINCT TOP 750 B01_Person.PsnID, B01_Person.PsnNo, B01_Person.PsnName, B01_Person.PsnEName, 
                B01_Person.PsnType, B01_Person.IDNum, B01_Person.Birthday, B01_Person.OrgStrucID, B01_Person.PsnAccount,
                B01_Person.PsnPW, B01_Person.PsnAuthAllow, B01_Person.PsnSTime, B01_Person.PsnETime, B01_Person.PsnPicSource,
                B01_Person.Remark, B01_Person.CreateUserID, B01_Person.CreateTime, B01_Person.UpdateUserID, 
                B01_Person.UpdateTime, B01_Person.Rev01, B01_Person.Rev02, OrgStrucAllData_1.OrgStrucNo, 
                OrgStrucAllData_1.OrgNameList, OrgStrucAllData_1.OrgNoList, B01_Card.CardNo, B01_Card.CardType,
                B01_Card.CardID FROM B01_Person
                INNER JOIN OrgStrucAllData('') AS OrgStrucAllData_1 ON B01_Person.OrgStrucID = OrgStrucAllData_1.OrgStrucID
                INNER JOIN B01_MgnOrgStrucs AS MOS ON MOS.OrgStrucID=OrgStrucAllData_1.OrgStrucID
                INNER JOIN B00_ManageArea AS MGA ON MGA.MgaID=MOS.MgaID
                LEFT OUTER JOIN B01_Card ON B01_Person.PsnID = B01_Card.PsnID
                WHERE (B01_Card.CardType <> 'R') AND MGA.OwnerID=@UserID ";
            if(new string[] {"CardNo","PsnNo","PsnName"}.Contains(QueryType))
            {
                QueryName = QueryName + "%";
            }
            if (QueryType == "CardNo")
            {             
                MainSqlCmd += " AND CardNo LIKE @Name";
            }
            else if (QueryType == "PsnName")
            {             
                MainSqlCmd += " AND PsnName LIKE @Name";
            }
            else if (QueryType == "Org")
            {
                QueryName = "%"+ QueryName + "%";
                MainSqlCmd += " AND OrgNoList LIKE @Name";
            }
            else if (QueryType == "PsnNo")
            {                
                MainSqlCmd += " AND PsnNo LIKE @Name";
            }

            this.CardList = this.odo.GetQueryResult<CardEntity>(MainSqlCmd
                ,new {UserID=Request["UserID"],Name=QueryName}).ToList();
           
            this.hDataRowCount.Value = this.logs.Count().ToString();           
            this._datacount = this.logs.Count();
            
        }
        #endregion                
       

    }//end page class 
}//end namespace