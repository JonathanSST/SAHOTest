using Sa.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBClass;
using SahoAcs.DBModel;

namespace SahoAcs
{
    public partial class CardEquAdj4 : System.Web.UI.Page
    {
        List<CardEntity> PersonList = new List<CardEntity>();

        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;     //宣告Ajax元件
        Hashtable hFloorType = new Hashtable(); //記載設備模組類型(門禁、電梯....))
        Hashtable hRuleType = new Hashtable();  //記錄設備的讀卡規則
        Hashtable hRuleList = new Hashtable();  //記載規則資訊
        Hashtable hFloorAllData = new Hashtable(); //記載各電梯設備的樓層選擇
        string sUserID = "";
        string sUserName ="";
        static string NonData = "";
        #region 定義Dapper
        OrmDataObject odo = new OrmDataObject("MsSql"
                    , string.Format(Pub.db_connection_template, Pub.db_source_ip, Pub.db_data_name, Pub.db_user_id, Pub.db_pwd));
        #endregion

        #endregion

        #region 網頁前置作業

        #region LoadProcess
        private void LoadProcess()
        {
            NonData = this.GetGlobalResourceObject("Resource", "NonData").ToString();
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.btStart);
            oScriptManager.RegisterAsyncPostBackControl(this.btRemove);
            //oScriptManager.RegisterAsyncPostBackControl(QueryButton);
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            //js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);            
            ClientScript.RegisterClientScriptInclude("CardEquAdj4", "CardEquAdj4.js?"+Pub.GetNowTime); //加入同一頁面所需的JavaScript檔案
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "OrgClassData();", true);
        }
        #endregion

        #region RegisterObj
        private void RegisterObj()
        {
            #region 設定彈跳視窗
            Pub.SetModalPopup(ModalPopupExtender1, 1);
            //Pub.SetModalPopup(ModalPopupExtender2, 2);
            #endregion

            #region 註冊主頁Button動作
            btSelectData.Attributes["onclick"] = "SelectItem(); return false;";
            //btStart.Attributes["onclick"] = "ExecProc(); return false;";
            ddl_input.Attributes["onchange"] = "SelectStateProc(); return false;";
            #endregion

            #region 註冊pop1頁Button動作
            ImgCloseButton1.Attributes["onclick"] = "CancelTrigger1.click(); return false;";
            popB_Query.Attributes["onclick"] = "QueryData(); return false;";
            popB_OK1.Attributes["onclick"] = "LoadCardData(); return false;";
            popB_Cancel1.Attributes["onclick"] = "CancelTrigger1.click(); return false;";
            popB_Enter1.Attributes["onclick"] = "DataEnterRemove('Add'); return false;";
            popB_Remove1.Attributes["onclick"] = "DataEnterRemove('Del'); return false;";
            //popTxt_Query.Attributes["onkeypress"] = "KeyDownEvent(event); return false;";
            #endregion
        }
        #endregion

        #region PageLoad
        protected void Page_Load(object sender, EventArgs e)
        {
            hUserId.Value = Session["UserID"].ToString();
            hSelectState.Value = ddl_input.SelectedValue;
            LoadProcess();
            RegisterObj();

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                SelectAllEquData();
                if(Request["PageEvent"]==null)
                    Session["FloorData"] = hFloorAllData;
            }
            else
            {
                this.sUserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                //取得使用者名稱資訊
                var user_list = this.odo.GetQueryResult("SELECT * FROM B00_SysUser WHERE UserID=@UserID", new {UserID=this.sUserID });
                foreach (var user in user_list)
                {
                    this.sUserName = Convert.ToString(user.UserName);
                }
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "CardEquAdj")
            {
                SetCardEquAdj();
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "CardEquGroup")
            {
                SetCardEquGroup();
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "RemoveFloorData")
            {
                SetRemoveFloorData();
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "LoadCardData")
            {
                //SetCardEquAdj();
                SetLoadCardData();
            }
        }
        #endregion

        #region VerifyRenderingInServerForm
        public override void VerifyRenderingInServerForm(Control control)
        {
            //覆寫此方法用來修正'XX'型別必須置於含有runat=server的標記屬性
        }
        #endregion

        #region 加入動作
        protected void AddEquButton_Click(object sender, EventArgs e)
        {
            DataTable dt1 = null;
            DataTable dt2 = null;
            DataRow dr = null;
            GridView gv1 = new GridView();
            GridView gv2 = new GridView();
            this.hEquType.Value = this.ddlEquType.SelectedValue;
            switch (ddlEquType.SelectedValue)
            {
                case "Equ":
                    gv1 = GridView1;
                    gv2 = GridView2;                    
                    break;
                case "EquGrp":
                    gv1 = GridView3;
                    gv2 = GridView4;                    
                    break;
            }

            if (hAddData.Value != "")
            {
                try
                {
                    String[] addData = (hAddData.Value.Substring(0, hAddData.Value.Length - 1)).Split('|');
                    if (addData[0] == gv2.Rows.Count.ToString())
                    {
                        if (ViewState["GV1"] != null)
                            dt1 = (DataTable)ViewState["GV1"];
                        if (dt1 == null)
                        {
                            dt1 = new DataTable();
                            dt1.Columns.Add(new DataColumn("EquNo", typeof(string)));
                            dt1.Columns.Add(new DataColumn("EquName", typeof(string)));
                            dt1.Columns.Add(new DataColumn("CardRule", typeof(string)));
                            dt1.Columns.Add(new DataColumn("Floor", typeof(string)));
                            dt1.Columns.Add(new DataColumn("EquID", typeof(string)));
                            int rowindex;
                            for (int i = 1; i < addData.Length; i++)
                            {
                                rowindex = int.Parse(addData[i]);
                                dr = dt1.NewRow();
                                dr["EquNo"] = gv2.Rows[rowindex].Cells[1].Text;
                                dr["EquName"] = gv2.Rows[rowindex].Cells[2].Text;
                                dr["CardRule"] = "";
                                dr["Floor"] = "";
                                dr["EquID"] = gv2.Rows[rowindex].Cells[5].Text;
                                dt1.Rows.Add(dr);
                            }
                        }
                        else
                        {
                            int rowindex;
                            for (int i = 1; i < addData.Length; i++)
                            {
                                rowindex = int.Parse(addData[i]);
                                dr = dt1.NewRow();
                                dr["EquNo"] = gv2.Rows[rowindex].Cells[1].Text;
                                dr["EquName"] = gv2.Rows[rowindex].Cells[2].Text;
                                dr["CardRule"] = "";
                                dr["Floor"] = "";
                                dr["EquID"] = gv2.Rows[rowindex].Cells[5].Text;
                                dt1.Rows.Add(dr);
                            }
                        }

                        if (ViewState["GV2"] != null)
                            dt2 = (DataTable)ViewState["GV2"];
                        int rowindex2;
                        int a = dt2.Rows.Count;
                        for (int i = addData.Length - 1; i > 0; i--)
                        {
                            rowindex2 = int.Parse(addData[i]);
                            dt2.Rows[rowindex2].Delete();
                        }

                        gv1.DataSource = dt1;
                        gv1.DataBind();
                        ViewState["GV1"] = dt1;
                        gv2.DataSource = dt2;
                        gv2.DataBind();
                        ViewState["GV2"] = dt2;
                        hAddData.Value = "";
                        if (hddlState.Value != "")
                        {
                            string[] gvCnt = hddlState.Value.ToString().Split('|');
                            Hashtable hGvCnt = new Hashtable();

                            for (int i = 0; i < gvCnt.Length - 1; i += 2)
                                hGvCnt.Add(gvCnt[i], gvCnt[i + 1]);
                            for (int i = 0; i < gv1.Rows.Count; i++)
                            {
                                if (hGvCnt.ContainsKey(gv1.Rows[i].Cells[1].Text))
                                {
                                    DropDownList ddl = (DropDownList)gv1.Rows[i].Cells[3].FindControl("ddlCardRule");
                                    ddl.SelectedIndex = int.Parse(hGvCnt[gv1.Rows[i].Cells[1].Text].ToString());
                                }
                            }
                        }
                        hFinalEquData.Value = "";
                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            hFinalEquData.Value += ((DataRow)dt1.Rows[i])["EquID"].ToString() + "|" + ((DataRow)dt1.Rows[i])["EquNo"].ToString() + "|";
                        }
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "ClearAll();EquData('" + hFinalEquData.Value + "');", true);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "msg2", "SetEquType('" + this.hEquType.Value + "');", true);
                    }
                    else
                    {
                        if (hddlState.Value != "")
                        {
                            string[] gvCnt = hddlState.Value.ToString().Split('|');
                            Hashtable hGvCnt = new Hashtable();

                            for (int i = 0; i < gvCnt.Length - 1; i += 2)
                                hGvCnt.Add(gvCnt[i], gvCnt[i + 1]);
                            for (int i = 0; i < gv1.Rows.Count; i++)
                            {
                                if (hGvCnt.ContainsKey(gv1.Rows[i].Cells[1].Text))
                                {
                                    DropDownList ddl = (DropDownList)gv1.Rows[i].Cells[3].FindControl("ddlCardRule");
                                    ddl.SelectedIndex = int.Parse(hGvCnt[gv1.Rows[i].Cells[1].Text].ToString());
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    string ex_str = ex.Message;
                }
                
            }
            else
            {
                if (hddlState.Value != "")
                {
                    string[] gvCnt = hddlState.Value.ToString().Split('|');
                    Hashtable hGvCnt = new Hashtable();

                    for (int i = 0; i < gvCnt.Length - 1; i += 2)
                        hGvCnt.Add(gvCnt[i], gvCnt[i + 1]);
                    for (int i = 0; i < gv1.Rows.Count; i++)
                    {
                        if (hGvCnt.ContainsKey(gv1.Rows[i].Cells[1].Text))
                        {
                            DropDownList ddl = (DropDownList)gv1.Rows[i].Cells[3].FindControl("ddlCardRule");
                            ddl.SelectedIndex = int.Parse(hGvCnt[gv1.Rows[i].Cells[1].Text].ToString());
                        }
                    }
                }
            }
        }
        #endregion

        #region 移除動作
        protected void RemoveEquButton_Click(object sender, EventArgs e)
        {
            DataTable dt1 = null;
            DataRow dr = null;
            GridView gv1 = new GridView();
            GridView gv2 = new GridView();

            switch (ddlEquType.SelectedValue)
            {
                case "Equ":
                    gv1 = GridView1;
                    gv2 = GridView2;
                    break;
                case "EquGrp":
                    gv1 = GridView3;
                    gv2 = GridView4;
                    break;
            }

            if (hRemoveData.Value != "")
            {
                String[] removeData = (hRemoveData.Value.Substring(0, hRemoveData.Value.Length - 1)).Split('|');
                if (removeData[0] == gv1.Rows.Count.ToString())
                {
                    if (ViewState["GV2"] != null)
                        dt1 = (DataTable)ViewState["GV2"];
                    Hashtable hfloorTemp = (Hashtable)Session["FloorData"];
                    if (dt1 == null)
                    {
                        dt1 = new DataTable();
                        dt1.Columns.Add(new DataColumn("EquNo", typeof(string)));
                        dt1.Columns.Add(new DataColumn("EquName", typeof(string)));
                        dt1.Columns.Add(new DataColumn("CardRule", typeof(string)));
                        dt1.Columns.Add(new DataColumn("Floor", typeof(string)));
                        dt1.Columns.Add(new DataColumn("EquID", typeof(string)));
                        int rowindex;
                        for (int i = 1; i < removeData.Length; i++)
                        {
                            rowindex = int.Parse(removeData[i]);
                            dr = dt1.NewRow();
                            dr["EquNo"] = gv1.Rows[rowindex].Cells[1].Text;
                            dr["EquName"] = gv1.Rows[rowindex].Cells[2].Text;
                            dr["CardRule"] = "";
                            dr["Floor"] = "";
                            dr["EquID"] = gv1.Rows[rowindex].Cells[5].Text;
                            dt1.Rows.Add(dr);
                            hfloorTemp.Remove(gv1.Rows[rowindex].Cells[5].Text);
                        }
                    }
                    else
                    {
                        int rowindex;
                        for (int i = 1; i < removeData.Length; i++)
                        {
                            rowindex = int.Parse(removeData[i]);
                            dr = dt1.NewRow();
                            dr["EquNo"] = gv1.Rows[rowindex].Cells[1].Text;
                            dr["EquName"] = gv1.Rows[rowindex].Cells[2].Text;
                            dr["CardRule"] = "";
                            dr["Floor"] = "";
                            dr["EquID"] = gv1.Rows[rowindex].Cells[5].Text;
                            dt1.Rows.Add(dr);
                            hfloorTemp.Remove(gv1.Rows[rowindex].Cells[5].Text);
                        }
                    }

                    DataTable dt2 = null;
                    if (ViewState["GV1"] != null)
                        dt2 = (DataTable)ViewState["GV1"];
                    int rowindex2;
                    int a = dt2.Rows.Count;
                    for (int i = removeData.Length - 1; i > 0; i--)
                    {
                        rowindex2 = int.Parse(removeData[i]);
                        dt2.Rows[rowindex2].Delete();
                    }

                    gv1.DataSource = dt2;
                    gv1.DataBind();
                    ViewState["GV1"] = dt2;
                    gv2.DataSource = dt1;
                    gv2.DataBind();
                    ViewState["GV2"] = dt1;
                    hRemoveData.Value = "";
                    if (hddlState.Value != "")
                    {
                        string[] gvCnt = hddlState.Value.ToString().Split('|');
                        Hashtable hGvCnt = new Hashtable();

                        for (int i = 0; i < gvCnt.Length - 1; i += 2)
                            hGvCnt.Add(gvCnt[i], gvCnt[i + 1]);
                        for (int i = 0; i < gv1.Rows.Count; i++)
                        {
                            if (hGvCnt.ContainsKey(gv1.Rows[i].Cells[1].Text))
                            {
                                DropDownList ddl = (DropDownList)gv1.Rows[i].Cells[3].FindControl("ddlCardRule");
                                ddl.SelectedIndex = int.Parse(hGvCnt[gv1.Rows[i].Cells[1].Text].ToString());
                            }
                        }

                    }
                    hFinalEquData.Value = "";
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        hFinalEquData.Value += ((DataRow)dt2.Rows[i])["EquID"].ToString() + "|" + ((DataRow)dt2.Rows[i])["EquNo"].ToString() + "|";
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "ddlState();ClearAll();EquData('" + hFinalEquData.Value + "');", true);
                }
                else
                {
                    if (hddlState.Value != "")
                    {
                        string[] gvCnt = hddlState.Value.ToString().Split('|');
                        Hashtable hGvCnt = new Hashtable();

                        for (int i = 0; i < gvCnt.Length - 1; i += 2)
                            hGvCnt.Add(gvCnt[i], gvCnt[i + 1]);
                        for (int i = 0; i < gv1.Rows.Count; i++)
                        {
                            if (hGvCnt.ContainsKey(gv1.Rows[i].Cells[1].Text))
                            {
                                DropDownList ddl = (DropDownList)gv1.Rows[i].Cells[3].FindControl("ddlCardRule");
                                ddl.SelectedIndex = int.Parse(hGvCnt[gv1.Rows[i].Cells[1].Text].ToString());
                            }
                        }
                    }
                }
            }
            else
            {
                if (hddlState.Value != "")
                {
                    string[] gvCnt = hddlState.Value.ToString().Split('|');
                    Hashtable hGvCnt = new Hashtable();

                    for (int i = 0; i < gvCnt.Length - 1; i += 2)
                        hGvCnt.Add(gvCnt[i], gvCnt[i + 1]);
                    for (int i = 0; i < gv1.Rows.Count; i++)
                    {
                        if (hGvCnt.ContainsKey(gv1.Rows[i].Cells[1].Text))
                        {
                            DropDownList ddl = (DropDownList)gv1.Rows[i].Cells[3].FindControl("ddlCardRule");
                            ddl.SelectedIndex = int.Parse(hGvCnt[gv1.Rows[i].Cells[1].Text].ToString());
                        }
                    }
                }
            }
        }
        #endregion



        void SetRemoveFloorData()
        {
            #region 取得客戶端IP位址
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string sIPAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(sIPAddress))
            {
                sIPAddress = context.Request.ServerVariables["REMOTE_ADDR"];
            }
            else
            {
                string[] ipArray = sIPAddress.Split(new Char[] { ',' });
            }

            if (sIPAddress == "::1")
                sIPAddress = "127.0.0.1";
            #endregion

            var NowCardEquAdj = this.odo.GetQueryResult<CardEquAdjObj>("SELECT * FROM B01_CardEquAdj WHERE CardID=@CardID ",new {CardID=Request["CardID"] });
            this.sUserID = Request["UserID"];
            var Floors = Request["FloorData"].ToString().Split('|');
            for(int i = 0; i < Floors.Length; i += 2)
            {
                if (Floors[i + 1] == "")
                    Floors[i + 1] = "000000000000";
                if (NowCardEquAdj.Where(o => o.CardID == Request["CardID"] && o.EquID == Floors[i]).Count() > 0)
                {
                    var newfloor = Sa.Change.HexToBin(Floors[i + 1], 48);
                    var cardadj = NowCardEquAdj.Where(o => o.CardID == Request["CardID"] && o.EquID == Floors[i]).First();
                    cardadj.CardExtData = Sa.Change.HexToBin(cardadj.CardExtData, 48);
                    string resultextdata = "";
                    for (int ele = 0; ele < newfloor.Length; ele++)
                    {
                        if (newfloor[ele] == '1')
                        {
                            resultextdata += "0";
                        }
                        else
                        {
                            resultextdata += cardadj.CardExtData[ele];
                        }
                    }
                    cardadj.CardExtData = Sa.Change.BinToHex(resultextdata, 12);
                    odo.Execute(@"EXEC CardEquAdj_Update 
                                                    @CardID = @CardID, @EquID = @EquID, @OpMode = @OpMode,
                                                    @CardRule = @CardRule, @CardExtData = @CardExtData, @BeginTime = '', @EndTime = '',
                                                    @CreateUserID = @CreateUserID, @CreateTime = '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'", cardadj);
                }
            }
            odo.Execute(" EXEC CardAuth_Update @sCardNo = @CardNo,@sUserID = @UserID,@sFromProc = 'CardEquAdj',@sFromIP = '" + sIPAddress + "',@sOpDesc = '增加權限' ; "
                        , new { UserID = Request["UserID"], CardNo = Request["CardNo"] });



            //完成處理訊息
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                    new { message = "OK" }));
            Response.End();

        }//end update 電梯樓層移除


        /// <summary>設定載入異動資料的卡號</summary>
        void SetLoadCardData()
        {
            //OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
            var selectstate = Request["Condition"];
            var paraIDs = Request["DataList"].ToString().Split('|').ToList();
            paraIDs.RemoveAt(paraIDs.Count - 1);
            string sqlcmd = "SELECT * FROM V_PsnCard ";
            switch (selectstate)
            {
                case "Card":
                    this.PersonList = odo.GetQueryResult<CardEntity>(sqlcmd + " WHERE CardID IN @CardData", new {CardData = paraIDs}).ToList();
                    break;
                case "Org":
                    this.PersonList = odo.GetQueryResult<CardEntity>(sqlcmd + " WHERE OrgStrucID IN @CardData", new { CardData = paraIDs }).ToList();
                    break;
                case "No":
                    this.PersonList = odo.GetQueryResult<CardEntity>(sqlcmd + " WHERE PsnID IN @CardData", new { CardData = paraIDs }).ToList();
                    break;
                case "Name":
                    this.PersonList = odo.GetQueryResult<CardEntity>(sqlcmd + " WHERE PsnID IN @CardData", new { CardData = paraIDs }).ToList();
                    break;

            }

            //完成處理訊息
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                    new { message = "OK", datalist=string.Join(",",this.PersonList.Select(i=>i.CardNo)), card_list = this.PersonList } ));
            Response.End();
        }


        /// <summary>進行設備群組的新增及刪除</summary>
        void SetCardEquGroup()
        {
            #region 取得客戶端IP位址
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string sIPAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            string log_info = string.Format("卡號{0}進行設備群組{1}...",Request["CardNo"],Request["OpType"]);
            if (string.IsNullOrEmpty(sIPAddress))
            {
                sIPAddress = context.Request.ServerVariables["REMOTE_ADDR"];
            }
            else
            {
                string[] ipArray = sIPAddress.Split(new Char[] { ',' });
            }

            if (sIPAddress == "::1")
                sIPAddress = "127.0.0.1";
            #endregion


            //判斷要做設備調整還是設備群組調整
            var EquDatas = Request["FinalEquData"].ToString().Split('|');
            //var Rules = Request["ddlState"].ToString().Substring(0, Request["ddlState"].ToString().Length).Split('|');
            //var Floors = Request["FloorData"].ToString().Split('|');
            var EquAdjData = new List<CardEquAdjObj>();


            for (int i = 0; i < EquDatas.Length; i+=2)
            {
                if (EquDatas[i] != null && EquDatas[i] != "")
                {
                    CardEquAdjObj obj = new CardEquAdjObj();
                    obj.EquID = EquDatas[i];
                    obj.CardID = Request["CardID"].ToString();
                    obj.CreateUserID = Request["UserID"];
                    EquAdjData.Add(obj);
                }                
            }
            if (Request["OpType"] == "Add")
            {
                var cmdstr = @"INSERT INTO B01_CardEquGroup (CardID,EquGrpID,CreateUserID,CreateTime) 
                                            SELECT @CardID,EquGrpID,@CreateUser,GETDATE()  
                                                FROM 
	                                                B01_EquGroup	WHERE EquGrpID IN @EquGrpIDs
	                                                AND EquGrpID NOT IN (SELECT EquGrpID FROM B01_CardEquGroup WHERE CardID=@CardID) ";
                odo.Execute(cmdstr,new {CardID=Request["CardID"], CreateUser=Request["UserID"], EquGrpIDs=EquAdjData.Select(i=>i.EquID)});
            }
            else
            {
                odo.Execute("DELETE B01_CardEquGroup WHERE CardID=@CardID AND EquGrpID=@EquID", EquAdjData);                
                //odo.SetLogs("");
            }
            #region 處理log
            var EquGroups = this.odo.GetQueryResult<DBModel.EquGroupLog>("SELECT * FROM B01_EquGroup WHERE EquGrpID IN @Grps", new { Grps = EquAdjData.Select(i => i.EquID)}).ToList();
            EquGroups.ForEach(i => i.CardNo = Request["CardNo"]);
            SysLogEntity _logObj = new SysLogEntity();
            _logObj.LogFrom = "0301";
            _logObj.LogType = DB_Acs.Logtype.卡片一般權限調整.ToString();
            _logObj.LogIP = sIPAddress;
            _logObj.UserID = Request["UserID"].ToString();
            _logObj.UserName = "";
            this.odo.SetSysLogCreate(_logObj, EquGroups, Request["OpType"]);
            #endregion
            this.DoExecProc(Request["CardNo"]);                                   
            //完成處理訊息
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                    new { message = "OK" }));
            Response.End();
        }


        /// <summary>透過非同步處理一般權限調整</summary>
        void SetCardEquAdj()
        {
            #region 取得客戶端IP位址
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string sIPAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(sIPAddress))
            {
                sIPAddress = context.Request.ServerVariables["REMOTE_ADDR"];
            }
            else
            {
                string[] ipArray = sIPAddress.Split(new Char[] { ',' });
            }

            if (sIPAddress == "::1")
                sIPAddress = "127.0.0.1";
            #endregion
            string log_info = string.Format("卡號{0}進行設備資料..{1}...", Request["CardNo"], Request["OpType"]);
            //判斷要做設備調整還是設備群組調整
            var EquDatas = Request["FinalEquData"].ToString().Split('|');
            var Rules = Request["ddlState"].ToString().Substring(0,Request["ddlState"].ToString().Length).Split('|');
            var Floors = Request["FloorData"].ToString().Split('|');
            var EquAdjData = new List<CardEquAdjObj>();
            var EquAdjLogs = new List<DBModel.EquAdjLog>();
            for (int i = 0; i < EquDatas.Length; i += 2)
            {
                if (EquDatas[i] != "")
                {
                    CardEquAdjObj obj = new CardEquAdjObj();
                    obj.EquID = EquDatas[i];
                    obj.EquNo = EquDatas[i + 1];
                    obj.CardNo = Request["CardNo"];
                    obj.CardID = Request["CardID"];
                    obj.CardRule = "0";
                    obj.CardExtData = "";
                    obj.OpMode = Request["OpType"].ToString()=="Add"?"+":"-";
                    obj.CreateUserID = Request["UserID"];
                    EquAdjData.Add(obj);                    
                }
            }
            for(int i = 0; i < Floors.Length; i += 2)
            {
                if (EquAdjData.Where(o => o.EquID == Floors[i]).Count() > 0)
                {
                    if (Floors[i + 1] == "")
                        Floors[i + 1] = "000000000000";
                    EquAdjData.Where(o => o.EquID == Floors[i]).First().CardExtData = Floors[i+1];
                }
            }            
            for(int i = 0; i < Rules.Length; i += 2)
            {
                if (EquAdjData.Where(o => o.EquNo == Rules[i]).Count() > 0)
                {
                    EquAdjData.Where(o => o.EquNo == Rules[i]).First().CardRule = Rules[i + 1];
                }
            }
            odo.Execute(@"EXEC CardEquAdj_Update 
                                                    @CardID = @CardID, @EquID = @EquID, @OpMode = @OpMode,
                                                    @CardRule = @CardRule, @CardExtData = @CardExtData, @BeginTime = '', @EndTime = '',
                                                    @CreateUserID = @CreateUserID, @CreateTime = '"+ DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'",EquAdjData);

            #region 處理log
            EquAdjData.ForEach(obj =>
            {
                EquAdjLogs.Add(new EquAdjLog() { EquNo = obj.EquNo, EquID = int.Parse(obj.EquID), OpMode = obj.OpMode, CardNo = Request["CardNo"] });
            });
            SysLogEntity _logObj = SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.卡片一般權限調整, Request["UserID"], "", "0301");            
            this.odo.SetSysLogCreate(_logObj, EquAdjLogs, Request["OpType"]);
            #endregion

            this.DoExecProc(Request["CardNo"]);
            //完成處理訊息
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                    new { message = "OK" }));
            Response.End();
        }



        #region 重整動作
        protected void btStart_Click2(object sender, EventArgs e)
        {
            #region 取得客戶端IP位址
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string sIPAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(sIPAddress))
            {
                sIPAddress = context.Request.ServerVariables["REMOTE_ADDR"];
            }
            else
            {
                string[] ipArray = sIPAddress.Split(new Char[] { ',' });
            }

            if (sIPAddress == "::1")
                sIPAddress = "127.0.0.1";
            #endregion

            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            Hashtable hCardAdj = new Hashtable();
            Hashtable hEquInfo = new Hashtable();
            Queue<CardEquAdjObj> AdjQue = new Queue<CardEquAdjObj>();
            List<CardEquAdjObj> NowEquAdj = new List<CardEquAdjObj>();
            DataTable EquAdjData = new DataTable();
            
            string sql = "", beginTime = "", endTime = "", sType = ddlOpType.Text, sFinalEquData = hFinalEquData.Value,
                sDataList = hDataList.Value, AddData = hddlState.Value, sFloorData = hFinalFloorData.Value,
                UserID = hUserId.Value, strCondition = hSelectState.Value, strEquType = ddlEquType.Text;
            List<string> liSqlPara = new List<string>();
            int result = 0;
            Queue<String> resultmsg = new Queue<string>();
            Hashtable hProcCnt = new Hashtable();
            CardEquAdjObj oCardEquAdj = null;
            Sa.DB.DBReader dr;
            System.Data.DataTable dt = new DataTable();
            List_Msg.Items.Clear();

            if (!sFinalEquData.Equals("") && sFinalEquData != null && !sDataList.Equals(""))
            {
                #region 權限資訊
                string strObj = "";

                if (strEquType.Equals("EquGrp"))
                {
                    String[] objGrp = sFinalEquData.Substring(0, sFinalEquData.Length - 1).Split('|');
                    string strSql = "SELECT * FROM B01_EquGroup WHERE EquGrpID IN (";                    
                    for (int i = 0, j = objGrp.Length; i < j; i += 2)
                    {                        
                        if (i > 0)
                            strSql += ",";
                        strSql += objGrp[i];
                    }
                    strSql += ") ";                    
                    oAcsDB.GetDataTable("EquGroups",strSql, liSqlPara, out dt);                    
                }
                else
                {
                    strObj = sFinalEquData;
                }
                string[] obj = { };
                if (!strEquType.Equals("EquGrp"))
                {
                    obj = strObj.Substring(0, strObj.Length - 1).Split('|');
                }                

                for (int i = 0; i < obj.Length; i += 2)
                {
                    oCardEquAdj = new CardEquAdjObj();
                    oCardEquAdj.CardID = "";
                    oCardEquAdj.CardNo = "";
                    oCardEquAdj.EquID = obj[i];
                    oCardEquAdj.EquNo = obj[i + 1];
                    oCardEquAdj.EquClass = oAcsDB.GetStrScalar("SELECT EquClass FROM B01_EquData WHERE EquID = " + obj[i]);
                    oCardEquAdj.CardRule = "0";

                    if (oCardEquAdj.EquClass == "Elevator")
                    {
                        oCardEquAdj.CardExtData = "000000000000";
                    }
                    else
                    {
                        oCardEquAdj.CardExtData = "";
                    }

                    oCardEquAdj.BeginTime = "";
                    oCardEquAdj.EndTime = "";

                    if (sType == "Add")
                    {
                        oCardEquAdj.OpMode = "+";
                    }
                    else
                    {
                        oCardEquAdj.OpMode = "-";
                    }

                    hCardAdj.Add(obj[i + 1], oCardEquAdj);
                    hEquInfo.Add(obj[i], obj[i + 1]);
                    oCardEquAdj = null;
                }
                #endregion

                #region 規則資訊
                if (AddData != "")
                {
                    obj = AddData.Substring(0, AddData.Length - 1).Split('|');

                    for (int i = 0; i < obj.Length; i += 2)
                    {
                        if (hCardAdj.ContainsKey(obj[i]))
                        {
                            ((CardEquAdjObj)hCardAdj[obj[i]]).CardRule = obj[i + 1];
                        }
                    }
                }
                #endregion

                #region 電梯資訊
                if (sFloorData != "")
                {
                    obj = sFloorData.Split('|');

                    for (int i = 0; i < obj.Length; i += 2)
                    {
                        if (hEquInfo.ContainsKey(obj[i]))
                        {
                            ((CardEquAdjObj)hCardAdj[hEquInfo[obj[i]]]).CardExtData = obj[i + 1];
                        }
                    }
                }
                #endregion

                #region 卡片資訊
                dr = null;
                if (strCondition.Equals("Card"))
                {
                    obj = sDataList.Substring(0, sDataList.Length - 1).Split('|');
                }
                else
                {
                    String[] objCard = sDataList.Substring(0, sDataList.Length - 1).Split('|');
                    string wheresql = " (", strDataList = "";
                    switch (strCondition)
                    {
                        case "Org":
                            for (int i = 0; i < objCard.Length; i += 2)
                            {
                                wheresql += "B01_Person.OrgStrucID = " + objCard[i + 1] + " OR ";
                            }
                            break;
                        case "No":
                        case "Name":
                            for (int i = 0; i < objCard.Length; i += 2)
                            {
                                wheresql += "B01_Person.PsnID = " + objCard[i + 1] + " OR ";
                            }
                            break;
                    }
                    wheresql = wheresql.Substring(0, wheresql.Length - 4);
                    wheresql += ") ";
                    sql = DataListStr(wheresql);
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            strDataList += dr.DataReader["CardNo"].ToString() + "|" + dr.DataReader["CardID"].ToString() + "|";
                        }
                    }
                    obj = strDataList.Substring(0, strDataList.Length - 1).Split('|');
                }
                #endregion

                #region 定義 CardEquAdjObj

                if (sDataList != "")
                {
                    CardEquAdjObj oCEAO = null;
                   
                    for (int i = 0; i < obj.Length; i += 2)
                    {
                        beginTime = oAcsDB.GetStrScalar(" SELECT CONVERT(VARCHAR,CardSTime,120) FROM B01_Card WHERE CardID = " + obj[i + 1]);
                        endTime = oAcsDB.GetStrScalar(" SELECT CONVERT(VARCHAR,CardETime,120) FROM B01_Card WHERE CardID = " + obj[i + 1]);

                        foreach (DictionaryEntry cardadj in hCardAdj)
                        {
                            oCEAO = new CardEquAdjObj();
                            oCardEquAdj = (CardEquAdjObj)cardadj.Value;
                            oCEAO.BeginTime = beginTime;
                            oCEAO.EndTime = endTime;
                            oCEAO.CardID = obj[i + 1];
                            oCEAO.CardNo = obj[i];
                            oCEAO.EquID = oCardEquAdj.EquID;
                            oCEAO.EquNo = oCardEquAdj.EquNo;
                            oCEAO.EquClass = oCardEquAdj.EquClass;
                            oCEAO.OpMode = oCardEquAdj.OpMode;
                            oCEAO.CardRule = oCardEquAdj.CardRule;
                            oCEAO.CardExtData = oCardEquAdj.CardExtData;
                            oCEAO.CreateUserID = this.hUserId.Value;
                            AdjQue.Enqueue(oCEAO);
                            oCEAO = null;
                        }
                    }
                }

                #endregion

                if (!strEquType.Equals("EquGrp"))
                {
                    #region 取得現有卡片設備資料調整清單
                    oAcsDB.GetDataTable("EquAdj", "SELECT * FROM B01_CardEquAdj WHERE OpMode='+' ", out EquAdjData);
                    foreach (DataRow r in EquAdjData.Rows)
                    {
                        NowEquAdj.Add(new CardEquAdjObj()
                        {
                            CardExtData = Convert.ToString(r["CardExtData"]),
                            CardID = Convert.ToString(r["CardID"]),
                            EquID = Convert.ToString(r["EquID"])
                        });
                    }
                    #endregion
                    
                    string now_card_no = "";

                    #region 設備調整
                    if (AdjQue.Count > 0)
                    {
                        oAcsDB.BeginTransaction();

                        while (AdjQue.Count > 0)
                        {
                            oCardEquAdj = AdjQue.Dequeue();
                            
                            #region 執行前一張卡號的權限重整
                            if (now_card_no != "" && now_card_no != oCardEquAdj.CardNo)
                            {
                                result = this.DoExecProc(now_card_no);      //執行卡片權限重整
                                if (!hProcCnt.ContainsKey(now_card_no))
                                {
                                    hProcCnt.Add(now_card_no, now_card_no);
                                }
                                if (odo.isSuccess)
                                {
                                    List_Msg.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - [卡號:" + now_card_no + "] 權限修改完成");
                                }
                                else
                                {
                                    List_Msg.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - [卡號:" + now_card_no + "] 權限修改失敗");
                                }
                            }
                            #endregion

                            #region 進行原電梯設備已開放樓層的變更
                            if (NowEquAdj.Where(i => i.EquID == oCardEquAdj.EquID && i.CardID == oCardEquAdj.CardID).Count() > 0
                                && oCardEquAdj.EquClass == "Elevator")
                            {
                                var cardext = NowEquAdj.Where(i => i.EquID == oCardEquAdj.EquID && i.CardID == oCardEquAdj.CardID).FirstOrDefault().CardExtData;
                                if (cardext != null && cardext != "")
                                {
                                    cardext = Sa.Change.HexToBin(cardext, 48);
                                    oCardEquAdj.CardExtData = Sa.Change.HexToBin(oCardEquAdj.CardExtData, 48);
                                    string result_ext = "";
                                    for (int i = 0; i < cardext.Length; i++)
                                    {
                                        if (oCardEquAdj.CardExtData[i] == '1')
                                        {
                                            result_ext += oCardEquAdj.CardExtData[i];
                                        }
                                        else
                                        {
                                            result_ext += cardext[i];
                                        }
                                    }
                                    oCardEquAdj.CardExtData = Sa.Change.BinToHex(result_ext, 12);
                                }
                            }
                            #endregion

                            #region call CardEquAdj_UPDATE

                            result=odo.Execute(@"EXEC CardEquAdj_Update 
                                                    @CardID=@CardID,@EquID=@EquID,@OpMode=@OpMode,
                                                    @CardRule=@CardRule,@CardExtData=@CardExtData,@BeginTime='',@EndTime='',
                                                    @CreateUserID=@CreateUserID,@CreateTime='"+DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+"'", oCardEquAdj);
                            #endregion

                            this.odo.SetSysLogCreateByNo(int.Parse(oCardEquAdj.CardID), oCardEquAdj.CardNo, oCardEquAdj.EquNo, "Update CardEquAdj OpMode "+oCardEquAdj.OpMode, "0301");

                            if (odo.isSuccess)
                            {
                                List_Msg.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - [卡號:" + oCardEquAdj.CardNo + "] 權限 " + oCardEquAdj.OpMode + " [" + oCardEquAdj.EquNo + "] 完成");
                            }
                            else
                            {                                
                                List_Msg.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - [卡號:" + oCardEquAdj.CardNo + "] 權限 " + oCardEquAdj.OpMode + " [" + oCardEquAdj.EquNo + "] 失敗");                                
                            }
                            now_card_no = oCardEquAdj.CardNo;
                            oCardEquAdj = null;
                        }
                        //執行最後一張卡的權限重整
                        if (now_card_no != "")
                        {
                            result = this.DoExecProc(now_card_no);
                            if (!hProcCnt.ContainsKey(now_card_no))
                            {
                                hProcCnt.Add(now_card_no, now_card_no);
                            }
                            if (odo.isSuccess)
                            {
                                List_Msg.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - [卡號:" + now_card_no + "] 權限修改完成");
                            }
                            else
                            {
                                List_Msg.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - [卡號:" + now_card_no + "] 權限修改失敗");
                            }
                        }
                        List_Msg.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - 處理資料筆數: " + hProcCnt.Count + " 筆");
                        UpdatePanel5.Update();
                        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Block", "$.unblockUI();", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Block", "$.unblockUI();", true);
                        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('無選取處理資料');", true);
                    }
                    #endregion
                }
                else
                {
                    #region EquGroup Setting 
                    for (int i = 0; i < obj.Length; i += 2)
                    {
                        string card_no = obj[i].ToString();
                        string card_id = obj[i + 1].ToString();
                        string cmdstr="";
                        string equgrp = "";
                        liSqlPara.Clear();
                        List<int> equgrps = new List<int>();
                        foreach (DataRow r in dt.Rows)
                        {
                            if (dt.Rows.IndexOf(r) > 0)
                                equgrp += ",";
                            equgrp += r["EquGrpID"].ToString();
                            equgrps.Add(Convert.ToInt32(r["EquGrpID"]));
                        }                                                                    
                        if (this.ddlOpType.SelectedValue.Equals("Add"))
                        { 
                            cmdstr = @"INSERT INTO B01_CardEquGroup (CardID,EquGrpID,CreateUserID,CreateTime) 
                                            SELECT @CardID,EquGrpID,@CreateUser,GETDATE()  
                                                FROM 
	                                                B01_EquGroup	WHERE EquGrpID IN @EquGrpIDs
	                                                AND EquGrpID NOT IN (SELECT EquGrpID FROM B01_CardEquGroup WHERE CardID=@CardID) ";
                                //.Replace("@EquGrpIDs", equgrp);
                            //result = oAcsDB.SqlCommandExecute(cmdstr, liSqlPara);
                            result = odo.Execute(cmdstr, new {CardID=card_id,CreateUser=Session["UserID"].ToString(),EquGrpIDs=equgrps});
                            //加入CardNo 增加設備群組的log
                            oAcsDB.WriteLog(DB_Acs.Logtype.卡片一般權限調整,this.sUserID,this.sUserName,"0301","","",
                                    string.Format("{0}..增加設備群組........{1}",card_no,string.Join(",",equgrps)),"");
                        }
                        else
                        {
                            /*
                            cmdstr = @"DELETE B01_CardEquGroup WHERE CardID=?;INSERT INTO B01_CardEquGroup (CardID,EquGrpID,CreateUserID,CreateTime) 
                                            SELECT ?,EquGrpID,?,GETDATE()  
                                                FROM 
	                                                B01_EquGroup	WHERE EquGrpID IN (@EquGrpIDs)
	                                                AND EquGrpID NOT IN (SELECT EquGrpID FROM B01_CardEquGroup WHERE CardID=?) ".Replace("@EquGrpIDs", equgrp);
                             */
                            liSqlPara.Add("S:" + card_id);
                            cmdstr = @"DELETE B01_CardEquGroup WHERE CardID=@CardID AND EquGrpID IN @EquGrpIDs";
                                //.Replace("@EquGrpIDs",equgrp);
                            //result = oAcsDB.SqlCommandExecute(cmdstr, liSqlPara);
                            result = odo.Execute(cmdstr, new { CardID = card_id, EquGrpIDs = equgrps });
                            //加入CardNo 減少設備群組的log
                            oAcsDB.WriteLog(DB_Acs.Logtype.卡片一般權限調整, this.sUserID, this.sUserName, "0301", "", "",
                                    string.Format("{0}..刪除設備群組........{1}", card_no, string.Join(",", equgrps)), "");
                        }                        
                        #region 設備群組調整
                        List_Msg.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - 處理資料筆數: " + result + " 筆");
                        UpdatePanel5.Update();
                        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Block", "$.unblockUI();", true);
                        #endregion
                        //if (result > 0)
                        //{
                            //result = oAcsDB.SqlCommandExecute(" EXEC CardAuth_Update @sCardNo = '" + card_no + "',@sUserID = '" + UserID + "',@sFromProc = 'CardEquAdj',@sFromIP = '" + sIPAddress + "',@sOpDesc = '增加權限' ; ");                            
                        //}
                        result = odo.Execute(" EXEC CardAuth_Update @sCardNo = '" + card_no + "',@sUserID = '" + UserID + "',@sFromProc = 'CardEquAdj',@sFromIP = '" + sIPAddress + "',@sOpDesc = '增加權限' ; ");
                    }//each by card                    
                    #endregion
                }//checked equgroup or equ                                    
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Block", "$.unblockUI();", true);
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('無選取處理資料');", true);
            }                    
        }
        #endregion

        #region 執行權限重整
        private int DoExecProc(string card_no)
        {
            int EffectCnt = 0;
            //odo.BeginTransaction();
            EffectCnt += odo.Execute(" EXEC CardAuth_Update @sCardNo = @CardNo,@sUserID = @UserID,@sFromProc = 'CardEquAdj',@sFromIP = '::1',@sOpDesc = '增加權限' ; "
                , new { UserID = Request["UserID"], CardNo = Request["CardNo"] });
            var EquList = this.VerifiEquList();
            EffectCnt += odo.Execute(@"UPDATE B01_CardAuth SET VerifiMode =@Mode,ErrCnt = 0,OpMode = 'Reset',OpStatus = '',UpdateTime=GETDATE(),UpdateUserID =@UserID
                                        WHERE CardNo=@CardNo AND EquID IN (SELECT EquID FROM B01_EquData WHERE EquModel IN @EquList )", new {Equlist=EquList, CardNo = Request["CardNo"], Mode = Request["VerifiMode"], UserID = Request["UserID"] });
            return EffectCnt;
        }
        #endregion

        #region 清除樓層動作

        protected void btRemove_Click(object sender, EventArgs e)
        {
            #region 取得客戶端IP位址
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string sIPAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(sIPAddress))
            {
                sIPAddress = context.Request.ServerVariables["REMOTE_ADDR"];
            }
            else
            {
                string[] ipArray = sIPAddress.Split(new Char[] { ',' });
            }

            if (sIPAddress == "::1")
                sIPAddress = "127.0.0.1";
            #endregion

            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));            
            Pub.MessageObject sRet = new Pub.MessageObject();
            Hashtable hCardAdj = new Hashtable();
            Hashtable hEquInfo = new Hashtable();
            Queue<CardEquAdjObj> AdjQue = new Queue<CardEquAdjObj>();
            List<CardEquAdjObj> NowEquAdj = new List<CardEquAdjObj>();
            DataTable EquAdjData = new DataTable();

            string sql = "", beginTime = "", endTime = "", sType = ddlOpType.Text, sFinalEquData = hFinalEquData.Value,
                sDataList = hDataList.Value, AddData = hddlState.Value, sFloorData = hFinalFloorData.Value,
                UserID = hUserId.Value, strCondition = hSelectState.Value, strEquType = ddlEquType.Text;
            List<string> liSqlPara = new List<string>();
            int result = 0;
            Queue<String> resultmsg = new Queue<string>();
            Hashtable hProcCnt = new Hashtable();
            CardEquAdjObj oCardEquAdj = null;
            Sa.DB.DBReader dr;
            System.Data.DataTable dt = new DataTable();
            List_Msg.Items.Clear();

            if (!sFinalEquData.Equals("") && sFinalEquData != null && !sDataList.Equals(""))
            {
                #region 權限資訊
                string strObj = "";

                strObj = sFinalEquData;
                string[] obj = { };
                obj = strObj.Substring(0, strObj.Length - 1).Split('|');

                for (int i = 0; i < obj.Length; i += 2)
                {
                    oCardEquAdj = new CardEquAdjObj();
                    oCardEquAdj.CardID = "";
                    oCardEquAdj.CardNo = "";
                    oCardEquAdj.EquID = obj[i];
                    oCardEquAdj.EquNo = obj[i + 1];
                    oCardEquAdj.EquClass = oAcsDB.GetStrScalar("SELECT EquClass FROM B01_EquData WHERE EquID = " + obj[i]);
                    oCardEquAdj.CardRule = "0";

                    if (oCardEquAdj.EquClass == "Elevator")
                    {
                        oCardEquAdj.CardExtData = "000000000000";
                    }
                    else
                    {
                        oCardEquAdj.CardExtData = "";
                    }

                    oCardEquAdj.BeginTime = "";
                    oCardEquAdj.EndTime = "";                    
                    hCardAdj.Add(obj[i + 1], oCardEquAdj);
                    hEquInfo.Add(obj[i], obj[i + 1]);
                    oCardEquAdj = null;
                }
                #endregion

                #region 規則資訊
                if (AddData != "")
                {
                    obj = AddData.Substring(0, AddData.Length - 1).Split('|');

                    for (int i = 0; i < obj.Length; i += 2)
                    {
                        if (hCardAdj.ContainsKey(obj[i]))
                        {
                            ((CardEquAdjObj)hCardAdj[obj[i]]).CardRule = obj[i + 1];
                        }
                    }
                }
                #endregion

                #region 電梯資訊
                if (sFloorData != "")
                {
                    obj = sFloorData.Split('|');

                    for (int i = 0; i < obj.Length; i += 2)
                    {
                        if (hEquInfo.ContainsKey(obj[i]))
                        {
                            ((CardEquAdjObj)hCardAdj[hEquInfo[obj[i]]]).CardExtData = obj[i + 1];
                        }
                    }
                }
                #endregion

                #region 卡片資訊
                dr = null;
                if (strCondition.Equals("Card"))
                {
                    obj = sDataList.Substring(0, sDataList.Length - 1).Split('|');
                }
                else
                {
                    String[] objCard = sDataList.Substring(0, sDataList.Length - 1).Split('|');
                    string wheresql = " (", strDataList = "";
                    switch (strCondition)
                    {
                        case "Org":
                            for (int i = 0; i < objCard.Length; i += 2)
                            {
                                wheresql += "B01_Person.OrgStrucID = " + objCard[i + 1] + " OR ";
                            }
                            break;
                        case "No":
                        case "Name":
                            for (int i = 0; i < objCard.Length; i += 2)
                            {
                                wheresql += "B01_Person.PsnID = " + objCard[i + 1] + " OR ";
                            }
                            break;
                    }
                    wheresql = wheresql.Substring(0, wheresql.Length - 4);
                    wheresql += ") ";
                    sql = DataListStr(wheresql);
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            strDataList += dr.DataReader["CardNo"].ToString() + "|" + dr.DataReader["CardID"].ToString() + "|";
                        }
                    }
                    obj = strDataList.Substring(0, strDataList.Length - 1).Split('|');
                }
                #endregion

                #region 定義 CardEquAdjObj

                if (sDataList != "")
                {
                    CardEquAdjObj oCEAO = null;

                    for (int i = 0; i < obj.Length; i += 2)
                    {
                        //beginTime = oAcsDB.GetStrScalar(" SELECT CardSTime FROM B01_Card WHERE CardID = " + obj[i + 1]);
                        //endTime = oAcsDB.GetStrScalar(" SELECT CardETime FROM B01_Card WHERE CardID = " + obj[i + 1]);
                        foreach (DictionaryEntry cardadj in hCardAdj)
                        {
                            oCEAO = new CardEquAdjObj();
                            oCardEquAdj = (CardEquAdjObj)cardadj.Value;
                            oCEAO.BeginTime = beginTime;
                            oCEAO.EndTime = endTime;
                            oCEAO.CardID = obj[i + 1];
                            oCEAO.CardNo = obj[i];
                            oCEAO.EquID = oCardEquAdj.EquID;
                            oCEAO.EquNo = oCardEquAdj.EquNo;
                            oCEAO.EquClass = oCardEquAdj.EquClass;
                            oCEAO.OpMode = "+";
                            oCEAO.CardRule = oCardEquAdj.CardRule;
                            oCEAO.CardExtData = oCardEquAdj.CardExtData;
                            oCEAO.CreateUserID = this.hUserId.Value;
                            AdjQue.Enqueue(oCEAO);
                            oCEAO = null;
                        }
                    }
                }

                #endregion
                
                #region 取得現有卡片設備資料調整清單
                oAcsDB.GetDataTable("EquAdj", "SELECT * FROM B01_CardEquAdj WHERe OpMode='+' ", out EquAdjData);
                foreach (DataRow r in EquAdjData.Rows)
                {
                    NowEquAdj.Add(new CardEquAdjObj()
                    {
                        CardExtData = Convert.ToString(r["CardExtData"]),
                        CardID = Convert.ToString(r["CardID"]),
                        EquID = Convert.ToString(r["EquID"])
                    });
                }
                #endregion

                #region 設備調整
                if (AdjQue.Count > 0)
                {
                    oAcsDB.BeginTransaction();
                    while (AdjQue.Count > 0)
                    {
                        oCardEquAdj = AdjQue.Dequeue();                            
                        if (NowEquAdj.Where(i => i.EquID == oCardEquAdj.EquID && i.CardID == oCardEquAdj.CardID).Count() > 0
                            && oCardEquAdj.EquClass == "Elevator")
                        {
                            var cardext = NowEquAdj.Where(i => i.EquID == oCardEquAdj.EquID && i.CardID == oCardEquAdj.CardID).FirstOrDefault().CardExtData;
                            if (cardext != null && cardext != "")
                            {
                                cardext = Sa.Change.HexToBin(cardext, 48);
                                oCardEquAdj.CardExtData = Sa.Change.HexToBin(oCardEquAdj.CardExtData, 48);
                                string result_ext = "";
                                for (int i = 0; i < cardext.Length; i++)
                                {
                                    if (oCardEquAdj.CardExtData[i] == '1')
                                    {
                                        result_ext += "0";
                                    }
                                    else
                                    {
                                        result_ext += cardext[i];
                                    }
                                }
                                oCardEquAdj.CardExtData = Sa.Change.BinToHex(result_ext, 12);
                                List_Msg.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - [卡號:" + oCardEquAdj.CardNo + "] [設備" + oCardEquAdj.EquNo + "] 電梯權限 ["+oCardEquAdj.CardExtData+"]");
                                result = odo.Execute(@"EXEC CardEquAdj_Update 
                                                    @CardID=@CardID,@EquID=@EquID,@OpMode=@OpMode,
                                                    @CardRule=@CardRule,@CardExtData=@CardExtData,@BeginTime='',@EndTime='',
                                                    @CreateUserID=@CreateUserID,@CreateTime='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'", oCardEquAdj);                            

                                if (this.odo.isSuccess)
                                {                                    
                                    result = odo.Execute(" EXEC CardAuth_Update @sCardNo = '" + oCardEquAdj.CardNo + "',@sUserID = '" 
                                        + UserID + "',@sFromProc = 'CardEquAdj',@sFromIP = '" + sIPAddress + "',@sOpDesc = '增加權限' ; ");
                                    if (!hProcCnt.ContainsKey(oCardEquAdj.CardNo))
                                    {
                                        hProcCnt.Add(oCardEquAdj.CardNo, oCardEquAdj.CardNo);
                                    }

                                    if (result > 0)
                                    {
                                        List_Msg.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - [卡號:" + oCardEquAdj.CardNo + "] 權限 " + oCardEquAdj.OpMode + " [" + oCardEquAdj.EquNo + "] 成功");
                                        oAcsDB.Commit();
                                    }
                                    else
                                    {
                                        List_Msg.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - [卡號:" + oCardEquAdj.CardNo + "] 權限 " + oCardEquAdj.OpMode + " [" + oCardEquAdj.EquNo + "] 失敗");
                                        oAcsDB.Rollback();
                                    }
                                }
                                else
                                {
                                    List_Msg.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - [卡號:" + oCardEquAdj.CardNo + "] 權限 " + oCardEquAdj.OpMode + " [" + oCardEquAdj.EquNo + "] 失敗");
                                    oAcsDB.Rollback();
                                }
                            }
                        }
                        oCardEquAdj = null;
                    }
                    List_Msg.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - 處理資料筆數: " + hProcCnt.Count + " 筆");
                    UpdatePanel5.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Block", "$.unblockUI();", true);
                }
                else
                {
                    UpdatePanel5.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Block", "$.unblockUI();", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('無選取處理資料');", true);
                }
                #endregion                         
            }
            else
            {
                UpdatePanel5.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Block", "$.unblockUI();", true);
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('無選取處理資料');", true);
            }
        }

        #endregion 

        #endregion

        #region 搜尋

        #region 記錄GV2記錄
        private void GV2Memory()
        {
            DataTable dt = null;
            DataRow dr = null;
            GridView gv = new GridView();

            switch (ddlEquType.SelectedValue)
            {
                case "Equ":
                    gv = GridView2;
                    break;
                case "EquGrp":
                    gv = GridView4;
                    break;
            }

            if (ViewState["GV2"] != null)
                dt = (DataTable)ViewState["GV2"];
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add(new DataColumn("EquNo", typeof(string)));
                dt.Columns.Add(new DataColumn("EquName", typeof(string)));
                dt.Columns.Add(new DataColumn("CardRule", typeof(string)));
                dt.Columns.Add(new DataColumn("Floor", typeof(string)));
                dt.Columns.Add(new DataColumn("EquID", typeof(string)));
                for (int i = 0; i < gv.Rows.Count; i++)
                {
                    dr = dt.NewRow();
                    dr["EquNo"] = gv.Rows[i].Cells[1].Text;
                    dr["EquName"] = gv.Rows[i].Cells[2].Text;
                    dr["CardRule"] = "";
                    dr["Floor"] = "";
                    dr["EquID"] = gv.Rows[i].Cells[5].Text;
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                for (int i = 0; i < gv.Rows.Count; i++)
                {
                    dr = dt.NewRow();
                    dr["EquNo"] = gv.Rows[i].Cells[1].Text;
                    dr["EquName"] = gv.Rows[i].Cells[2].Text;
                    dr["CardRule"] = "";
                    dr["Floor"] = "";
                    dr["EquID"] = gv.Rows[i].Cells[5].Text;
                    dt.Rows.Add(dr);
                }
            }
            ViewState["GV2"] = dt;
        }
        #endregion

        #region 設備權限列表選取資料
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //清空目前暫存的ViewState
            ViewState["GV1"] = null;
            ViewState["GV2"] = null;

            switch (ddlEquType.SelectedValue)
            {
                case "Equ":
                    PanelVisable("Equ");
                    SelectAllEquData();
                    break;
                case "EquGrp":
                    PanelVisable("EquGrp");
                    SelectEquGroupData();
                    break;
            }
        }
        #endregion

        #region 查詢可設定的設備資料
        private void SelectAllEquData()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;
            DBReader dr = null;
            List<String> CheckData = new List<String>();
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = @" SELECT DISTINCT(B01_EquData.EquID) AS 'EquID', B01_EquData.EquNo, B01_EquData.EquName,
                B01_EquData.EquEName, B01_EquData.Floor, B01_EquData.EquClass, '' AS CardRule FROM B00_ManageArea
                LEFT JOIN B01_MgnEquGroup ON B00_ManageArea.MgaID = B01_MgnEquGroup.MgaID
                INNER JOIN B01_EquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroup.EquGrpID
                INNER JOIN B01_EquGroupData ON B01_EquGroup.EquGrpID = B01_EquGroupData.EquGrpID
                LEFT JOIN B01_EquData ON B01_EquGroupData.EquID = B01_EquData.EquID
                LEFT JOIN B00_SysUserMgns ON B00_ManageArea.MgaID = B00_SysUserMgns.MgaID
                WHERE UserID = ? ORDER BY EquNo";

            liSqlPara.Add("S:" + hUserId.Value);
            #endregion

            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            GirdViewDataBind(GridView2, dt);
            dt = null;

            #region 取得設備模組類型資料
            hFloorType = null;
            hFloorType = new Hashtable();
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    if (!hFloorType.ContainsKey(dr.DataReader["EquID"].ToString()))
                    {
                        hFloorType.Add(dr.DataReader["EquID"].ToString(), dr.DataReader["EquClass"].ToString());
                    }
                }
            }

            ViewState["ModelType"] = hFloorType;
            dr = null;
            #endregion

            #region 取得模組規則資料
            hRuleType = null;
            hRuleType = new Hashtable();

            sql = @"SELECT B01_EquParaData.ParaValue, B01_EquParaData.EquID FROM B01_EquParaData
                LEFT OUTER JOIN B01_EquParaDef ON B01_EquParaData.EquParaID = B01_EquParaDef.EquParaID
                WHERE (B01_EquParaDef.ParaName = 'CardRule')";

            liSqlPara.Clear();
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    if (!hRuleType.ContainsKey(dr.DataReader["EquID"].ToString()))
                    {
                        hRuleType.Add(dr.DataReader["EquID"].ToString(), dr.DataReader["ParaValue"].ToString());
                    }
                }
            }

            ViewState["RuleType"] = hRuleType;
            dr = null;
            #endregion

            #region 取得規則資訊
            hRuleList = null;
            hRuleList = new Hashtable();
            sql = @"SELECT RuleID, EquModel, RuleNo, RuleName FROM B01_CardRuleDef";
            liSqlPara.Clear();
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    //EquModel,RuleNo,RuleName
                    if (!hRuleList.ContainsKey(dr.DataReader["RuleID"].ToString()))
                    {
                        hRuleList.Add(dr.DataReader["RuleID"].ToString(), dr.DataReader["EquModel"].ToString() + "|" +
                            dr.DataReader["RuleNo"].ToString() + "|" + dr.DataReader["RuleName"].ToString());
                    }
                }
            }

            ViewState["RuleList"] = hRuleList;
            dr = null;
            #endregion

            #region Process String 2
            sql = @"SELECT EquNo, EquName, '' AS CardRule, '' AS Floor, EquID FROM B01_EquData WHERE EquID = 0";
            liSqlPara = new List<string>();
            #endregion

            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            GirdViewDataBind(GridView1, dt);
            GV2Memory();
        }
        #endregion

        #region 查詢可設定的設備群組資料
        private void SelectEquGroupData()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;
            List<String> CheckData = new List<String>();
            List<string> liSqlPara = new List<string>();

            #region Process String
            //sql = @"SELECT DISTINCT(B01_EquGroup.EquGrpID) AS 'EquID', B01_EquGroup.EquGrpNo AS 'EquNo',
            //    B01_EquGroup.EquGrpName AS 'EquName', '' AS CardRule, '' AS Floor FROM B00_ManageArea
            //    LEFT JOIN B01_MgnEquGroup ON B00_ManageArea.MgaID = B01_MgnEquGroup.MgaID
            //    INNER JOIN B01_EquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroup.EquGrpID
            //    INNER JOIN B01_EquGroupData ON B01_EquGroup.EquGrpID = B01_EquGroupData.EquGrpID
            //    LEFT JOIN B00_SysUserMgns ON B00_ManageArea.MgaID = B00_SysUserMgns.MgaID
            //    WHERE UserID = ? ";
            sql = @"SELECT DISTINCT(B01_EquGroup.EquGrpID) AS 'EquID', B01_EquGroup.EquGrpNo AS 'EquNo',
                B01_EquGroup.EquGrpName AS 'EquName', '' AS CardRule, '' AS Floor FROM B00_ManageArea
                LEFT JOIN B01_MgnEquGroup ON B00_ManageArea.MgaID = B01_MgnEquGroup.MgaID
                INNER JOIN B01_EquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroup.EquGrpID
                LEFT JOIN B00_SysUserMgns ON B00_ManageArea.MgaID = B00_SysUserMgns.MgaID
                WHERE UserID =?";

            liSqlPara.Add("S:" + hUserId.Value);
            #endregion

            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            GirdViewDataBind(GridView4, dt);
            dt = null;

            #region Process String 2
            sql = @"SELECT EquGrpNo AS 'EquNo', EquGrpName AS 'EquName','' AS CardRule,'' AS Floor, EquGrpID AS 'EquID' FROM B01_EquGroup WHERE EquGrpID = 0 ";
            liSqlPara = new List<string>();
            #endregion

            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            GirdViewDataBind(GridView3, dt);
            GV2Memory();
        }
        #endregion

        #region Panel顯示隱藏
        private void PanelVisable(String strType)
        {
            if (strType.Equals("Equ"))
            {
                tablePanel1.Visible = true;
                tablePanel2.Visible = true;
                tablePanel1_Grp.Visible = false;
                tablePanel2_Grp.Visible = false;
            }
            else if (strType.Equals("EquGrp"))
            {
                tablePanel1.Visible = false;
                tablePanel2.Visible = false;
                tablePanel1_Grp.Visible = true;
                tablePanel2_Grp.Visible = true;
            }
        }
        #endregion

        #endregion

        #region GridView處理

        #region Equ-GridView處理
        protected void GridView1_Data_DataBound(object sender, EventArgs e)
        {
            td_showGridView1.Attributes["colspan"] = GridView1.Columns.Count.ToString();
        }

        protected void GridView1_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    e.Row.Cells[0].Width = 20;
                    e.Row.Cells[1].Width = 60;
                    e.Row.Cells[2].Width = 80;
                    e.Row.Cells[3].Width = 170;
                    #endregion

                    #region 設定表頭欄位Visible狀態
                    //不想顯示的欄位，可在此處關掉
                    e.Row.Cells[5].Visible = false;
                    #endregion

                    #region 寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);
                    e.Row.CssClass = "GVStyle";
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    li_header1.Text = Header_sw.ToString();
                    #endregion

                    break;
                #endregion

                #region DataRow
                case DataControlRowType.DataRow:
                    DataRowView oRow = (DataRowView)e.Row.DataItem;

                    #region 指定Row的ID
                    GridViewRow GrRow = e.Row;
                    GrRow.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    GrRow.ID = "GV_Row" + oRow.Row["EquID"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    e.Row.Cells[0].Width = 24;
                    e.Row.Cells[1].Width = 64;
                    e.Row.Cells[2].Width = 84;
                    e.Row.Cells[3].Width = 174;
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 設備ID
                    e.Row.Cells[5].Visible = false;
                    #endregion

                    #region CheckBox
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    ((CheckBox)e.Row.Cells[0].FindControl("RowCheckState1")).Attributes["onChange"] = "";
                    #endregion

                    #region 設備編號
                    #endregion

                    #region 設備名稱
                    #endregion

                    #region 規則
                    hRuleType = (Hashtable)ViewState["RuleType"];
                    String[] EquRuleList = null;
                    DropDownList ddl_Rule = (DropDownList)e.Row.Cells[3].FindControl("ddlCardRule");
                    ddl_Rule.Attributes["onChange"] = "ddlState();";
                    if (hRuleType.ContainsKey(e.Row.Cells[5].Text))
                    {
                        char[] other = { ',', ':' };
                        EquRuleList = hRuleType[e.Row.Cells[5].Text].ToString().Split(other);
                        hRuleList = (Hashtable)ViewState["RuleList"];
                        if (EquRuleList.Length > 1)
                        {
                            for (int i = 0; i < EquRuleList.Length; i += 2)
                            {
                                try
                                {
                                    string datalist = hRuleList[EquRuleList[i + 1]].ToString();
                                    string[] data = datalist.Split('|');
                                    ListItem oneItem = new ListItem();
                                    oneItem.Text = EquRuleList[i] + "-" + data[1] + "(" + data[2] + ")";
                                    oneItem.Value = EquRuleList[i + 1];
                                    ddl_Rule.Items.Add(oneItem);
                                    oneItem = null;
                                }
                                catch (Exception ex)
                                {
                                    ListItem oneItem = new ListItem();
                                    oneItem.Text = "未定義規則";
                                    oneItem.Value = "0";
                                    ddl_Rule.Items.Add(oneItem);
                                    oneItem = null;
                                }
                                
                            }
                        }
                        else
                        {
                            ListItem oneItem = new ListItem();
                            oneItem.Text = "未定義規則";
                            oneItem.Value = "0";
                            ddl_Rule.Items.Add(oneItem);
                            oneItem = null;
                        }
                    }
                    else
                    {
                        ListItem oneItem = new ListItem();
                        oneItem.Text = "未定義規則";
                        oneItem.Value = "0";
                        ddl_Rule.Items.Add(oneItem);
                        oneItem = null;
                    }
                    ddl_Rule.Width = 150;
                    e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Center;
                    #endregion

                    #region 樓層
                    hFloorType = (Hashtable)ViewState["ModelType"];
                    Button Bt_Floor = (Button)e.Row.Cells[4].FindControl("btFloor");
                    Bt_Floor.Font.Size = 10;
                    Bt_Floor.Style.Add("margin", "0px 2px 0px 2px");
                    //Bt_Floor.Text = "樓層";
                    Bt_Floor.CommandArgument = e.Row.Cells[5].Text;
                    Bt_Floor.CommandName = e.Row.Cells[5].Text;
                    if (hFloorType.ContainsKey(e.Row.Cells[5].Text))
                    {
                        if (hFloorType[e.Row.Cells[5].Text].ToString() == "Door Access")
                            Bt_Floor.Enabled = false;
                        else
                        {
                            if (!hFloorAllData.ContainsKey(e.Row.Cells[5].Text))
                                hFloorAllData.Add(e.Row.Cells[5].Text, "");
                        }
                    }
                    else
                        Bt_Floor.Enabled = false;
                    Bt_Floor.Attributes["onClick"] = "window.open('../CardFloor.aspx?EquID=" + e.Row.Cells[5].Text + "&FinalFloorData=" + this.hFinalFloorData.ClientID + "','','height=400,width=405,status=no,toolbar=no,menubar=no,location=no,top=50,left=100',''); return false;";
                    //Bt_Floor.Attributes["onClick"] = "window.showModalDialog('../CardFloor.aspx?EquID=" + e.Row.Cells[5].Text + "&FinalFloorData=" + this.hFinalFloorData.ClientID + "','dialogHeight:200px;dialogWidth:200px;center:yes;scroll:no;','');";
                    e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Center;
                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }

                    e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 12, true);
                    e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text, 12, true);
                    #endregion

                    //e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    //e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    //e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectCardNo,'" + oRow.Row["CardNo"].ToString() + "', '', '');");
                    break;
                #endregion
            }
        }

        protected void GridView2_Data_DataBound(object sender, EventArgs e)
        {
            td_showGridView2.Attributes["colspan"] = GridView2.Columns.Count.ToString();
        }

        protected void GridView2_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    e.Row.Cells[0].Width = 20;
                    e.Row.Cells[1].Width = 80;
                    #endregion

                    #region 設定表頭欄位Visible狀態
                    //不想顯示的欄位，可在此處關掉
                    e.Row.Cells[3].Visible = false;
                    e.Row.Cells[4].Visible = false;
                    e.Row.Cells[5].Visible = false;
                    #endregion

                    #region 寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);
                    e.Row.CssClass = "GVStyle";
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    li_header2.Text = Header_sw.ToString();
                    #endregion

                    break;
                #endregion

                #region DataRow
                case DataControlRowType.DataRow:
                    DataRowView oRow = (DataRowView)e.Row.DataItem;

                    #region 指定Row的ID
                    GridViewRow GrRow = e.Row;
                    GrRow.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    GrRow.ID = "GV_Row" + oRow.Row["EquID"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    e.Row.Cells[0].Width = 24;
                    e.Row.Cells[1].Width = 84;
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 設備ID
                    e.Row.Cells[3].Visible = false;
                    e.Row.Cells[4].Visible = false;
                    e.Row.Cells[5].Visible = false;
                    #endregion
                    #region CheckBox
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    ((CheckBox)e.Row.Cells[0].FindControl("RowCheckState2")).Attributes["onChange"] = "";
                    #endregion
                    #region 設備編號
                    #endregion
                    #region 設備名稱
                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }

                    e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 12, true);
                    e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text, 18, true);
                    #endregion

                    //e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    //e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    //e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectCardNo,'" + oRow.Row["CardNo"].ToString() + "', '', '');");
                    break;
                #endregion
            }
        }
        #endregion

        #region EquGrp-GridView處理
        protected void GridView3_Data_DataBound(object sender, EventArgs e)
        {
            td_showGridView1.Attributes["colspan"] = GridView3.Columns.Count.ToString();
        }

        protected void GridView3_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    e.Row.Cells[0].Width = 20;
                    e.Row.Cells[1].Width = 75;
                    //e.Row.Cells[2].Width = 90;
                    //e.Row.Cells[3].Width = 160;
                    #endregion

                    #region 設定表頭欄位Visible狀態
                    //不想顯示的欄位，可在此處關掉
                    e.Row.Cells[3].Visible = false;
                    e.Row.Cells[4].Visible = false;
                    e.Row.Cells[5].Visible = false;
                    #endregion

                    #region 寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);
                    e.Row.CssClass = "GVStyle";
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    li_header1.Text = Header_sw.ToString();
                    #endregion

                    break;
                #endregion

                #region DataRow
                case DataControlRowType.DataRow:
                    DataRowView oRow = (DataRowView)e.Row.DataItem;

                    #region 指定Row的ID
                    GridViewRow GrRow = e.Row;
                    GrRow.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    GrRow.ID = "GV_Row" + oRow.Row["EquID"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    e.Row.Cells[0].Width = 24;
                    e.Row.Cells[1].Width = 79;
                    //e.Row.Cells[2].Width = 94;
                    //e.Row.Cells[3].Width = 164;
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 設備ID
                    e.Row.Cells[3].Visible = false;
                    e.Row.Cells[4].Visible = false;
                    e.Row.Cells[5].Visible = false;
                    #endregion

                    #region CheckBox
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    ((CheckBox)e.Row.Cells[0].FindControl("RowCheckState3")).Attributes["onChange"] = "";
                    #endregion

                    
                    /*

                    #region 規則
                    hRuleType = (Hashtable)ViewState["RuleType"];
                    String[] EquRuleList = null;
                    DropDownList ddl_Rule = (DropDownList)e.Row.Cells[3].FindControl("ddlCardRule");
                    ddl_Rule.Attributes["onChange"] = "ddlState();";
                    if (hRuleType.ContainsKey(e.Row.Cells[5].Text))
                    {
                        char[] other = { ',', ':' };
                        EquRuleList = hRuleType[e.Row.Cells[5].Text].ToString().Split(other);
                        hRuleList = (Hashtable)ViewState["RuleList"];
                        if (EquRuleList.Length > 1)
                        {
                            for (int i = 0; i < EquRuleList.Length; i += 2)
                            {
                                string datalist = hRuleList[EquRuleList[i + 1]].ToString();
                                string[] data = datalist.Split('|');
                                ListItem oneItem = new ListItem();
                                oneItem.Text = EquRuleList[i] + "-" + data[1] + "(" + data[2] + ")";
                                oneItem.Value = EquRuleList[i + 1];
                                ddl_Rule.Items.Add(oneItem);
                                oneItem = null;
                            }
                        }
                        else
                        {
                            ListItem oneItem = new ListItem();
                            oneItem.Text = "未定義規則";
                            oneItem.Value = "0";
                            ddl_Rule.Items.Add(oneItem);
                            oneItem = null;
                        }
                    }
                    else
                    {
                        ListItem oneItem = new ListItem();
                        oneItem.Text = "未定義規則";
                        oneItem.Value = "0";
                        ddl_Rule.Items.Add(oneItem);
                        oneItem = null;
                    }
                    ddl_Rule.Width = 150;
                    e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Center;
                    #endregion

                    #region 樓層
                    hFloorType = (Hashtable)ViewState["ModelType"];
                    Button Bt_Floor = (Button)e.Row.Cells[4].FindControl("btFloor");
                    Bt_Floor.Font.Size = 10;
                    Bt_Floor.Style.Add("margin", "0px 2px 0px 2px");
                    //Bt_Floor.Text = "樓層";
                    Bt_Floor.CommandArgument = e.Row.Cells[5].Text;
                    Bt_Floor.CommandName = e.Row.Cells[5].Text;
                    if (hFloorType.ContainsKey(e.Row.Cells[5].Text))
                    {
                        if (hFloorType[e.Row.Cells[5].Text].ToString() == "Door Access")
                            Bt_Floor.Enabled = false;
                        else
                        {
                            if (!hFloorAllData.ContainsKey(e.Row.Cells[5].Text))
                                hFloorAllData.Add(e.Row.Cells[5].Text, "");
                        }
                    }
                    else
                        Bt_Floor.Enabled = false;
                    Bt_Floor.Attributes["onClick"] = "window.open('../CardFloor.aspx?EquID=" + e.Row.Cells[5].Text + "&FinalFloorData=" + this.hFinalFloorData.ClientID + "','','height=400,width=405,status=no,toolbar=no,menubar=no,location=no,top=50,left=100','');return false;";
                    //Bt_Floor.Attributes["onClick"] = "window.showModalDialog('CardFloor.aspx?EquID=" + e.Row.Cells[5].Text + "','','')";
                    e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Center;
                    #endregion

                    */

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }

                    e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 12, true);
                    e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text, 12, true);
                    #endregion

                    //e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    //e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    //e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectCardNo,'" + oRow.Row["CardNo"].ToString() + "', '', '');");
                    break;
                #endregion
            }
        }

        protected void GridView4_Data_DataBound(object sender, EventArgs e)
        {
            td_showGridView2.Attributes["colspan"] = GridView4.Columns.Count.ToString();
        }

        protected void GridView4_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    e.Row.Cells[0].Width = 20;
                    e.Row.Cells[1].Width = 80;
                    #endregion

                    #region 設定表頭欄位Visible狀態
                    //不想顯示的欄位，可在此處關掉
                    e.Row.Cells[3].Visible = false;
                    e.Row.Cells[4].Visible = false;
                    e.Row.Cells[5].Visible = false;
                    #endregion

                    #region 寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);
                    e.Row.CssClass = "GVStyle";
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    li_header2.Text = Header_sw.ToString();
                    #endregion
                    break;
                #endregion

                #region DataRow
                case DataControlRowType.DataRow:
                    DataRowView oRow = (DataRowView)e.Row.DataItem;

                    #region 指定Row的ID
                    GridViewRow GrRow = e.Row;
                    GrRow.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    GrRow.ID = "GV_Row" + oRow.Row["EquID"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    e.Row.Cells[0].Width = 24;
                    e.Row.Cells[1].Width = 84;
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 設備ID
                    e.Row.Cells[3].Visible = false;
                    e.Row.Cells[4].Visible = false;
                    e.Row.Cells[5].Visible = false;
                    #endregion

                    #region CheckBox
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    ((CheckBox)e.Row.Cells[0].FindControl("RowCheckState4")).Attributes["onChange"] = "";
                    #endregion

                    #region 設備編號
                    #endregion

                    #region 設備名稱
                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }

                    e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 12, true);
                    e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text, 18, true);
                    #endregion

                    //e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    //e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    //e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectCardNo,'" + oRow.Row["CardNo"].ToString() + "', '', '');");
                    break;
                #endregion
            }
        }
        #endregion

        #region 查無資料時，GridView顯示查無資料資訊
        public void GirdViewDataBind(GridView ProcessGridView, DataTable dt)
        {
            if (dt.Rows.Count != 0)//Gridview中有資料
            {
                ProcessGridView.DataSource = dt;
                ProcessGridView.DataBind();
            }
            else//Gridview中沒有資料
            {
                dt.Rows.Add(dt.NewRow());
                ProcessGridView.DataSource = dt;
                ProcessGridView.DataBind();
                ProcessGridView.Rows[0].Visible = false;
            }
        }
        #endregion

        #endregion

        #region JavaScript及aspx共用方法

        #region 載入資料(卡片、組織、工號、姓名)
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object SelectTable(String UserID, String sType)
        {
            string[] EditData = null;
            //因前後端傳遞資料JSON有4MB限制故不適用查詢大量資料故Mark下段程式碼 By Eric 20140919
            /*
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "", wheresql = "";
            List<string> DataList = new List<string>();
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Process String
            sql = @"SELECT DISTINCT(OrgStrucID), OrgStrucNo FROM
                (SELECT B00_SysUserMgns.UserID, B00_SysUserMgns.MgaID, B01_MgnOrgStrucs.OrgStrucID, B01_OrgStruc.OrgStrucNo,
                B01_OrgStruc.OrgIDList FROM B00_SysUserMgns
                INNER JOIN B01_MgnOrgStrucs ON B00_SysUserMgns.MgaID = B01_MgnOrgStrucs.MgaID
                INNER JOIN B01_OrgStruc ON B01_MgnOrgStrucs.OrgStrucID = B01_OrgStruc.OrgStrucID) AS Mgns
                WHERE Mgns.UserID = '" + UserID + "' ORDER BY OrgStrucNo";

            oAcsDB.GetDataReader(sql, out dr);

            switch (sType)
            {
                case "Card":
                    if (dr.HasRows)
                    {
                        wheresql = " (";

                        while (dr.Read())
                        {
                            wheresql += "B01_Person.OrgStrucID = '" + dr.DataReader["OrgStrucID"].ToString() + "' OR ";
                        }

                        wheresql = wheresql.Substring(0, wheresql.Length - 4);
                        wheresql += ")";
                    }

                    sql = DataListStr(wheresql) + " ORDER BY B01_Card.CardNo";
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                    break;
                case "No":
                    wheresql = " B01_Person.PsnNo IS NOT NULL AND B01_Person.PsnNo <> '0' ";
                    sql = DataListStr(wheresql) + " ORDER BY B01_Person.PsnNo";
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                    break;
                case "Name":
                    wheresql = " B01_Person.PsnName IS NOT NULL AND B01_Person.PsnName <> '' ";
                    sql = DataListStr(wheresql) + " ORDER BY B01_Person.PsnName";
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                    break;
                default:
                    break;
            }

            if (dr.HasRows)
            {
                switch (sType)
                {
                    case "Card":
                        while (dr.Read())
                        {
                            DataList.Add(dr.DataReader["CardNo"].ToString() + "|" + dr.DataReader["CardID"].ToString());
                        }
                        break;
                    case "Org":
                        while (dr.Read())
                        {
                            DataList.Add(dr.DataReader["OrgStrucNo"].ToString() + "|" + dr.DataReader["OrgStrucID"].ToString());
                        }
                        break;
                    case "No":
                        while (dr.Read())
                        {
                            DataList.Add(dr.DataReader["PsnNo"].ToString() + "(" + dr.DataReader["PsnName"].ToString() +
                                ")|" + dr.DataReader["PsnID"].ToString());
                        }
                        break;
                    case "Name":
                        while (dr.Read())
                        {
                            DataList.Add(dr.DataReader["PsnName"].ToString() + "(" + dr.DataReader["PsnNo"].ToString() +
                                ")|" + dr.DataReader["PsnID"].ToString());
                        }
                        break;
                    default:
                        break;
                }

                EditData = new string[DataList.Count];

                for (int i = 0; i < DataList.Count; i++)
                {
                    EditData[i] = DataList[i];
                }
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "系統中無此資料！";
            }

            #endregion
            */
            switch (sType)
            {
                case "Card":
                case "Org":
                case "No":
                case "Name":
                    EditData = new string[2];
                    EditData[0] = sType;
                    EditData[1] = sType;
                    break;
                default:
                    EditData = new string[2];
                    EditData[0] = "Saho_SysErrorMassage";
                    EditData[1] = "系統中無此資料！";
                    break;
            }

            return EditData;
        }
        #endregion
        

        #region 查詢資料(卡片、組織、工號、姓名)
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object QueryData(String UserID, String sType, String sQuery)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "", wheresql = "", strType = sType.Substring(0, 2);
            List<string> DataList = new List<string>();
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Process String
            sql = @"SELECT DISTINCT(OrgStrucID), OrgStrucNo, OrgNameList FROM
                (SELECT B00_SysUserMgns.UserID, B00_SysUserMgns.MgaID, B01_MgnOrgStrucs.OrgStrucID, OSA.OrgStrucNo,
                OSA.OrgNameList+'('+OSA.OrgStrucNo+')' AS OrgNameList,OSA.OrgIDList FROM B00_SysUserMgns
                INNER JOIN B01_MgnOrgStrucs ON B00_SysUserMgns.MgaID = B01_MgnOrgStrucs.MgaID
                INNER JOIN OrgStrucAllData('') OSA ON B01_MgnOrgStrucs.OrgStrucID = OSA.OrgStrucID) AS Mgns
                WHERE Mgns.UserID = '" + UserID;
            if (sType.Contains("組織")||sType.Contains("Org"))
            {
                strType = "Organization";
            }
            else if (sType.Contains("姓名")||sType.Contains("Name"))
            {
                strType = "Name";
            }
            else if (sType.Contains("工號")||sType.Contains("PsnNo"))
            {
                strType = "PsnNo";
            }
            else
            {
                strType = "Card";
            }

            if (strType.Equals("Organization"))
            {
                sql += "' AND Mgns.OrgStrucNo LIKE '%" + sQuery + "%' ORDER BY OrgStrucNo";
            }
            else
            {
                sql += "' ORDER BY OrgStrucNo";
            }

            oAcsDB.GetDataReader(sql, out dr);

            switch (strType)
            {
                case "Card":
                    if (dr.HasRows)
                    {
                        wheresql = " (";

                        while (dr.Read())
                        {
                            wheresql += "B01_Person.OrgStrucID = '" + dr.DataReader["OrgStrucID"].ToString() + "' OR ";
                        }

                        wheresql = wheresql.Substring(0, wheresql.Length - 4);
                        wheresql += ") AND ";
                    }

                    sql = DataListStr(wheresql) + " B01_Card.CardNo LIKE '%" + sQuery + "%' ORDER BY B01_Card.CardNo";
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                    break;
                case "PsnNo":
                    wheresql = " B01_Person.PsnNo IS NOT NULL AND B01_Person.PsnNo <> '0' AND B01_Person.PsnNo LIKE '%" +
                        sQuery + "%' ";
                    sql = DataListStr(wheresql) + " ORDER BY B01_Person.PsnNo";
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                    break;
                case "Name":
                    wheresql = " B01_Person.PsnName IS NOT NULL AND B01_Person.PsnName <> '' AND B01_Person.PsnName LIKE '%" +
                        sQuery + "%' ";
                    sql = DataListStr(wheresql) + " ORDER BY B01_Person.PsnName";
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                    break;
                default:
                    break;
            }

            if (dr.HasRows)
            {
                switch (strType)
                {
                    case "Card":
                        while (dr.Read())
                        {
                            DataList.Add(dr.DataReader["CardNo"].ToString() + "|" + dr.DataReader["CardID"].ToString());
                        }
                        break;
                    case "Organization":
                        while (dr.Read())
                        {
                            DataList.Add(dr.DataReader["OrgNameList"].ToString() + "|" + dr.DataReader["OrgStrucID"].ToString());
                        }
                        break;
                    case "PsnNo":
                        while (dr.Read())
                        {
                            DataList.Add(dr.DataReader["PsnNo"].ToString() + "(" + dr.DataReader["PsnName"].ToString() +
                                ")|" + dr.DataReader["PsnID"].ToString());
                        }
                        break;
                    case "Name":
                        while (dr.Read())
                        {
                            DataList.Add(dr.DataReader["PsnName"].ToString() + "(" + dr.DataReader["PsnNo"].ToString() +
                                ")|" + dr.DataReader["PsnID"].ToString());
                        }
                        break;
                    default:
                        break;
                }
                EditData = new string[DataList.Count];

                for (int i = 0; i < DataList.Count; i++)
                {
                    EditData[i] = DataList[i];
                }
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = NonData+"!!";
            }
            #endregion

            return EditData;
        }
        #endregion

        #region LimitText
        public string LimitText(string str, int len, bool ellipsis)
        {
            Encoding big5 = Encoding.GetEncoding("big5");
            byte[] b = big5.GetBytes(str);
            if (b.Length <= len)
                return str;
            else
            {
                if (ellipsis) len -= 3;

                string res = big5.GetString(b, 0, len);
                if (!big5.GetString(b).StartsWith(res))
                    res = big5.GetString(b, 0, len - 1);
                return res + (ellipsis ? "..." : "");
            }
        }
        #endregion

        #endregion

        #region  物件
        public class CardEquAdjObj
        {
            public String CardID { get; set; }
            public String CardNo { get; set; }
            public String EquID { get; set; }
            public String EquNo { get; set; }
            public String EquClass { get; set; }
            public String OpMode { get; set; }
            public String CardRule { get; set; }
            public String CardExtData { get; set; }
            public String BeginTime { get; set; }
            public String EndTime { get; set; }
            public string CreateUserID { get; set; }
        }
        #endregion

        #region 靜態變數及函式
        protected static string DataListStr(string strWhereSql)
        {
            return @"SELECT DISTINCT TOP 750 B01_Person.PsnID, B01_Person.PsnNo, B01_Person.PsnName, B01_Person.PsnEName, 
                B01_Person.PsnType, B01_Person.IDNum, B01_Person.Birthday, B01_Person.OrgStrucID, B01_Person.PsnAccount,
                B01_Person.PsnPW, B01_Person.PsnAuthAllow, B01_Person.PsnSTime, B01_Person.PsnETime, B01_Person.PsnPicSource,
                B01_Person.Remark, B01_Person.CreateUserID, B01_Person.CreateTime, B01_Person.UpdateUserID, 
                B01_Person.UpdateTime, B01_Person.Rev01, B01_Person.Rev02, OrgStrucAllData_1.OrgStrucNo, 
                OrgStrucAllData_1.OrgNameList, OrgStrucAllData_1.OrgNoList, B01_Card.CardNo, B01_Card.CardType,
                B01_Card.CardID FROM B01_Person
                INNER JOIN OrgStrucAllData('') AS OrgStrucAllData_1 ON B01_Person.OrgStrucID = OrgStrucAllData_1.OrgStrucID
                LEFT OUTER JOIN B01_Card ON B01_Person.PsnID = B01_Card.PsnID
                WHERE (B01_Card.CardType <> 'R') AND " + strWhereSql;
        }
        #endregion
    }
}