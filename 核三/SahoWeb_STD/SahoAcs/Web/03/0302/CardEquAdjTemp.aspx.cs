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
using SahoAcs.DBClass;

namespace SahoAcs
{
    public partial class CardEquAdjTemp : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;     //宣告Ajax元件
        Hashtable hFloorType = new Hashtable(); //紀載設備模組類型(門禁、電梯....))
        Hashtable hRuleType = new Hashtable();  //紀錄設備的讀卡規則
        Hashtable hRuleList = new Hashtable();  //紀載規則資訊
        Hashtable hFloorAllData = new Hashtable(); //紀載各電梯設備的樓層選擇
        #endregion

        #region 網頁前置作業

        #region LoadProcess
        private void LoadProcess()
        {
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.btStart);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nDefault('" + this.GetZoneTime().ToString("yyyy/MM/dd HH:mm:ss") + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("CardEquAdjTemp", "CardEquAdjTemp.js"); //加入同一頁面所需的JavaScript檔案
        }
        #endregion

        #region RegisterObj
        private void RegisterObj()
        {
            #region 設定彈跳視窗
            Pub.SetModalPopup(ModalPopupExtender1, 1);
            #endregion

            #region 註冊主頁Button動作
            btSelectData.Attributes["onClick"] = "SelectItem(); return false;";
            //btStart.Attributes["onClick"] = "ExecProc(); return false;";
            ddl_input.Attributes["onchange"] = "SelectStateProc(); return false;";
            #endregion

            #region 註冊pop1頁Button動作
            ImgCloseButton1.Attributes["onClick"] = "CancelTrigger1.click(); return false;";
            popB_Query.Attributes["onclick"] = "QueryData(); return false;";
            popB_OK1.Attributes["onClick"] = "LoadCardData(); return false;";
            popB_Cancel1.Attributes["onClick"] = "CancelTrigger1.click(); return false;";
            popB_Enter1.Attributes["onClick"] = "DataEnterRemove('Add'); return false;";
            popB_Remove1.Attributes["onClick"] = "DataEnterRemove('Del'); return false;";
            popTxt_Query.Attributes["onkeypress"] = "KeyDownEvent(event); return false;";
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
                //LoadData();
                SelectAllEquData();
                Session["FloorData"] = hFloorAllData;
            }
            else
            {

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
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "ClearAll();EquData('" + hFinalEquData.Value +
                        "');GridCount('" + dt2.Rows.Count.ToString() + "','" + dt1.Rows.Count.ToString() + "');", true);

                    
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
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "ddlState();ClearAll();EquData('" + hFinalEquData.Value +
                        "');GridCount('" + dt1.Rows.Count.ToString() + "','" + dt2.Rows.Count.ToString() + "');", true);

                    
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

        #region 重整動作
        protected void btStart_Click(object sender, EventArgs e)
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
            string sql = "", sTime = DateTime.Parse(Input_STime.DateValue).GetUtcTime(this).ToString("yyyy/MM/dd HH:mm:ss"), eTime = Input_ETime.DateValue,
                sFinalEquData = hFinalEquData.Value, sDataList = hDataList.Value, AddData = hddlState.Value,
                sFloorData = hFinalFloorData.Value, UserID = hUserId.Value, strCondition = hSelectState.Value,
                strEquType = ddlEquType.Text;
            List<string> liSqlPara = new List<string>();
            int result = 0;
            Queue<String> resultmsg = new Queue<string>();
            Hashtable hProcCnt = new Hashtable();
            CardEquAdjObj oCardEquAdj = null;
            Sa.DB.DBReader dr;
            List_Msg.Items.Clear();

            if (!sFinalEquData.Equals("") && sFinalEquData != null)
            {
                #region 權限資訊
                string strObj = "";

                if (strEquType.Equals("EquGrp"))
                {
                    String[] objGrp = sFinalEquData.Substring(0, sFinalEquData.Length - 1).Split('|');
                    string strSql = "";

                    for (int i = 0, j = objGrp.Length; i < j; i += 2)
                    {
                        strSql += " SELECT B01_EquData.EquID, B01_EquData.EquNo FROM B01_EquGroup " +
                            "INNER JOIN B01_EquGroupData ON B01_EquGroup.EquGrpID = B01_EquGroupData.EquGrpID " +
                            "INNER JOIN B01_EquData ON B01_EquGroupData.EquID = B01_EquData.EquID " +
                            "WHERE B01_EquGroup.EquGrpID = '" + objGrp[i] + "' UNION";
                    }

                    strSql = strSql.Substring(0, strSql.Length - 5);
                    oAcsDB.GetDataReader(strSql, liSqlPara, out dr);

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            strObj += dr.DataReader["EquID"].ToString() + "|" + dr.DataReader["EquNo"].ToString() + "|";
                        }
                    }
                }
                else
                {
                    strObj = sFinalEquData;
                }

                String[] obj = strObj.Substring(0, strObj.Length - 1).Split('|');

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

                    if (sTime.Trim() != "")
                    {
                        oCardEquAdj.BeginTime = sTime;
                    }

                    if (eTime.Trim() == "")
                    {
                        oCardEquAdj.EndTime = oAcsDB.GetStrScalar("SELECT ParaValue FROM B00_SysParameter WHERE ParaClass = 'System' AND ParaNo = 'DefaultEndDateTime'") + " 23:59:59";
                    }
                    sTime = DateTime.Parse(Input_STime.DateValue).GetUtcTime(this).ToString("yyyy/MM/dd HH:mm:ss");
                    eTime = DateTime.Parse(Input_ETime.DateValue).GetUtcTime(this).ToString("yyyy/MM/dd HH:mm:ss");
                    oCardEquAdj.EndTime = eTime;
                    oCardEquAdj.OpMode = "*";
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

                if (sDataList != "")
                {
                    CardEquAdjObj oCEAO = null;

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

                    for (int i = 0; i < obj.Length; i += 2)
                    {
                        foreach (DictionaryEntry cardadj in hCardAdj)
                        {
                            oCEAO = new CardEquAdjObj();
                            oCardEquAdj = (CardEquAdjObj)cardadj.Value;
                            oCEAO.BeginTime = oCardEquAdj.BeginTime;
                            oCEAO.EndTime = oCardEquAdj.EndTime;
                            oCEAO.CardID = obj[i + 1];
                            oCEAO.CardNo = obj[i];
                            oCEAO.EquID = oCardEquAdj.EquID;
                            oCEAO.EquNo = oCardEquAdj.EquNo;
                            oCEAO.EquClass = oCardEquAdj.EquClass;
                            oCEAO.OpMode = oCardEquAdj.OpMode;
                            oCEAO.CardRule = oCardEquAdj.CardRule;
                            oCEAO.CardExtData = oCardEquAdj.CardExtData;
                            AdjQue.Enqueue(oCEAO);
                            oCEAO = null;
                        }
                    }

                    hCardAdj = null;
                }
                #endregion

                if (AdjQue.Count > 0)
                {
                    oAcsDB.BeginTransaction();

                    while (AdjQue.Count > 0)
                    {
                        oCardEquAdj = AdjQue.Dequeue();
                        sql = " UPDATE B01_CardEquAdj SET CardRule = '" + oCardEquAdj.CardRule + "',CardExtData = '" + oCardEquAdj.CardExtData + "',BeginTime = '" + oCardEquAdj.BeginTime + "',EndTime = '" 
                            + oCardEquAdj.EndTime + "',OpMode = '*',CreateUserID = '" + UserID + "' ,CreateTime = GETDATE() WHERE CardID = '" + oCardEquAdj.CardID + "' AND EquID = '" + oCardEquAdj.EquID + "'";// "' AND OpMode = '-' ";
                        result = oAcsDB.SqlCommandExecute(sql);

                        if (result < 1)
                        {
                            sql = " UPDATE B01_CardEquAdj SET CardRule = '" + oCardEquAdj.CardRule + "',CardExtData = '" + oCardEquAdj.CardExtData + "',BeginTime = '" 
                                + oCardEquAdj.BeginTime + "',EndTime = '" + oCardEquAdj.EndTime + "',CreateUserID = '" + UserID + "' ,CreateTime = GETDATE() WHERE CardID = '" + oCardEquAdj.CardID + "' AND EquID = '" + oCardEquAdj.EquID + "'";// "' AND OpMode = '" + oCardEquAdj.OpMode + "' ";
                            result = oAcsDB.SqlCommandExecute(sql);
                        }

                        if (result < 1)
                        {
                            sql = " INSERT B01_CardEquAdj( CardID ,EquID ,OpMode ,CardRule ,CardExtData ,BeginTime ,EndTime ,CreateUserID ,CreateTime) ";
                            sql += " VALUES ( " + oCardEquAdj.CardID + " ," + oCardEquAdj.EquID + " ,'" + oCardEquAdj.OpMode + "' ,'" 
                                + oCardEquAdj.CardRule + "' , '" + oCardEquAdj.CardExtData + "' ,'" + oCardEquAdj.BeginTime + "' ,'" + oCardEquAdj.EndTime + "' ,'" + UserID + "' ,GETDATE()) ";
                            result = oAcsDB.SqlCommandExecute(sql);
                        }

                        if (result > 0)
                        {
                            result = oAcsDB.SqlCommandExecute(" EXEC CardAuth_Update @sCardNo = '" + oCardEquAdj.CardNo + "',@sUserID = '" + UserID 
                                + "',@sFromProc = 'CardEquAdjTemp',@sFromIP = '" + sIPAddress + "',@sOpDesc = '增加修改臨時權限' ; ");

                            if (!hProcCnt.ContainsKey(oCardEquAdj.CardNo))
                            {
                                hProcCnt.Add(oCardEquAdj.CardNo, oCardEquAdj.CardNo);
                            }

                            if (result > 0)
                            {
                                //resultmsg.Enqueue(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - [卡號:" + oCardEquAdj.CardNo + "] 臨時權限 " + oCardEquAdj.OpMode + " [" + oCardEquAdj.EquNo + "] 成功");
                                List_Msg.Items.Add(this.GetZoneTime().ToString("yyyy/MM/dd HH:mm:ss") + " - [卡號:" + oCardEquAdj.CardNo + "] 臨時權限 " + oCardEquAdj.OpMode + " [" + oCardEquAdj.EquNo + "] 成功");
                                oAcsDB.Commit();
                            }
                            else
                            {
                                //resultmsg.Enqueue(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - [卡號:" + oCardEquAdj.CardNo + "] 臨時權限 " + oCardEquAdj.OpMode + " [" + oCardEquAdj.EquNo + "] 失敗");
                                List_Msg.Items.Add(this.GetZoneTime().ToString("yyyy/MM/dd HH:mm:ss") + " - [卡號:" + oCardEquAdj.CardNo + "] 臨時權限 " + oCardEquAdj.OpMode + " [" + oCardEquAdj.EquNo + "] 失敗");
                                oAcsDB.Rollback();
                            }
                        }
                        else
                        {
                            //resultmsg.Enqueue(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - [卡號:" + oCardEquAdj.CardNo + "] 臨時權限 " + oCardEquAdj.OpMode + " [" + oCardEquAdj.EquNo + "] 失敗");
                            List_Msg.Items.Add(this.GetZoneTime().ToString("yyyy/MM/dd HH:mm:ss") + " - [卡號:" + oCardEquAdj.CardNo + "] 臨時權限 " + oCardEquAdj.OpMode + " [" + oCardEquAdj.EquNo + "] 失敗");
                            oAcsDB.Rollback();
                        }

                        oCardEquAdj = null;
                    }

                    //resultmsg.Enqueue(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - 處理資料筆數: " + hProcCnt.Count + " 筆");
                    List_Msg.Items.Add(this.GetZoneTime().ToString("yyyy/MM/dd HH:mm:ss") + " - 處理資料筆數: " + hProcCnt.Count + " 筆");
                    UpdatePanel5.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Block", "$.unblockUI();", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Block", "$.unblockUI();", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('無選取處理資料');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Block", "$.unblockUI();", true);
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('無選取處理資料');", true);
            }
        }
        #endregion

        #endregion

        #region 搜尋

        #region 紀錄GV2紀錄
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
            string sql = "", strNotAdd = "", strAdded = "";
            DataTable dt;
            DBReader dr = null;
            List<String> CheckData = new List<String>();
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = @"SELECT DISTINCT(B01_EquData.EquID) AS 'EquID', B01_EquData.EquNo, B01_EquData.EquName,
                B01_EquData.EquEName, B01_EquData.Floor, B01_EquData.EquClass, '' AS CardRule FROM B00_ManageArea
                LEFT JOIN B01_MgnEquGroup ON B00_ManageArea.MgaID = B01_MgnEquGroup.MgaID
                INNER JOIN B01_EquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroup.EquGrpID
                INNER JOIN B01_EquGroupData ON B01_EquGroup.EquGrpID = B01_EquGroupData.EquGrpID
                LEFT JOIN B01_EquData ON B01_EquGroupData.EquID = B01_EquData.EquID
                LEFT JOIN B00_SysUserMgns ON B00_ManageArea.MgaID = B00_SysUserMgns.MgaID
                WHERE UserID = ? AND B01_EquData.EquID IS NOT NULL";

            liSqlPara.Add("S:" + hUserId.Value);
            #endregion

            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            strNotAdd = dt.Rows.Count.ToString();
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
            strAdded = dt.Rows.Count.ToString();
            GirdViewDataBind(GridView1, dt);
            GV2Memory();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "GridCount('" + strNotAdd + "','" + strAdded + "');", true);
        }
        #endregion

        #region 查詢可設定的設備群組資料
        private void SelectEquGroupData()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", strNotAdd = "", strAdded = "";
            DataTable dt;
            List<String> CheckData = new List<String>();
            List<string> liSqlPara = new List<string>();

            #region Process String
            sql = @"SELECT DISTINCT(B01_EquGroup.EquGrpID) AS 'EquID', B01_EquGroup.EquGrpNo AS 'EquNo',
                B01_EquGroup.EquGrpName AS 'EquName', '' AS CardRule, '' AS Floor FROM B00_ManageArea
                LEFT JOIN B01_MgnEquGroup ON B00_ManageArea.MgaID = B01_MgnEquGroup.MgaID
                LEFT JOIN B01_EquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroup.EquGrpID
                LEFT JOIN B01_EquGroupData ON B01_EquGroup.EquGrpID = B01_EquGroupData.EquGrpID
                LEFT JOIN B00_SysUserMgns ON B00_ManageArea.MgaID = B00_SysUserMgns.MgaID
                WHERE UserID = ? ";

            liSqlPara.Add("S:" + hUserId.Value);
            #endregion

            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            strNotAdd = dt.Rows.Count.ToString();
            GirdViewDataBind(GridView4, dt);
            dt = null;

            #region Process String 2
            sql = @"SELECT EquGrpNo AS 'EquNo', EquGrpName AS 'EquName','' AS CardRule,'' AS Floor, EquGrpID AS 'EquID' FROM B01_EquGroup WHERE EquGrpID = 0";
            liSqlPara = new List<string>();
            #endregion

            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            strAdded = dt.Rows.Count.ToString();
            GirdViewDataBind(GridView3, dt);
            GV2Memory();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "GridCount('" + strNotAdd + "','" + strAdded + "');", true);

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
                    Bt_Floor.Text = GetLocalResourceObject("Elevator").ToString();
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
                    e.Row.Cells[0].Width = 28;
                    e.Row.Cells[1].Width = 68;
                    e.Row.Cells[2].Width = 91;
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
                    e.Row.Cells[0].Width = 32;
                    e.Row.Cells[1].Width = 72;
                    e.Row.Cells[2].Width = 95;
                    e.Row.Cells[3].Width = 174;
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 設備ID
                    e.Row.Cells[5].Visible = false;
                    #endregion

                    #region CheckBox
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    ((CheckBox)e.Row.Cells[0].FindControl("RowCheckState3")).Attributes["onChange"] = "";
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
                    //Bt_Floor.Style.Add("padding", "0px 15px");
                    Bt_Floor.Text = GetLocalResourceObject("Elevator").ToString();
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
                            wheresql += "B01_Person.OrgStrucID = " + dr.DataReader["OrgStrucID"].ToString() + " OR ";
                        }

                        wheresql = wheresql.Substring(0, wheresql.Length - 4);
                        wheresql += ")";
                    }

                    sql = DaraListStr(wheresql) + " ORDER BY B01_Card.CardNo";
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                    break;
                case "No":
                    wheresql = " B01_Person.PsnNo IS NOT NULL AND B01_Person.PsnNo <> '0' ";
                    sql = DaraListStr(wheresql) + " ORDER BY B01_Person.PsnNo";
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                    break;
                case "Name":
                    wheresql = " B01_Person.PsnName IS NOT NULL AND B01_Person.PsnName <> '' ";
                    sql = DaraListStr(wheresql) + " ORDER BY B01_Person.PsnName";
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

        #region 執行處理(卡片)
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object ExecCardProc(String sTime, String eTime, String sFinalEquData, String sDataList, String AddData, String sFloorData, String UserID, String strCondition, String strEquType)
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
            string sql = "";
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            int result = 0;
            Queue<String> resultmsg = new Queue<string>();
            Hashtable hProcCnt = new Hashtable();
            CardEquAdjObj oCardEquAdj = null;
            Sa.DB.DBReader dr;

            if (!sFinalEquData.Equals("") && sFinalEquData != null)
            {
                #region 權限資訊
                string strObj = "";

                if (strEquType.Equals("EquGrp"))
                {
                    String[] objGrp = sFinalEquData.Substring(0, sFinalEquData.Length - 1).Split('|');
                    string strSql = "";

                    for (int i = 0, j = objGrp.Length; i < j; i += 2)
                    {
                        strSql += " SELECT B01_EquData.EquID, B01_EquData.EquNo FROM B01_EquGroup " +
                            "INNER JOIN B01_EquGroupData ON B01_EquGroup.EquGrpID = B01_EquGroupData.EquGrpID " +
                            "INNER JOIN B01_EquData ON B01_EquGroupData.EquID = B01_EquData.EquID " +
                            "WHERE B01_EquGroup.EquGrpID = '" + objGrp[i] + "' UNION";
                    }

                    strSql = strSql.Substring(0, strSql.Length - 5);
                    oAcsDB.GetDataReader(strSql, liSqlPara, out dr);

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            strObj += dr.DataReader["EquID"].ToString() + "|" + dr.DataReader["EquNo"].ToString() + "|";
                        }
                    }
                }
                else
                {
                    strObj = sFinalEquData;
                }

                String[] obj = strObj.Substring(0, strObj.Length - 1).Split('|');

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
                        oCardEquAdj.CardExtData = "000000000000";
                    else
                        oCardEquAdj.CardExtData = "";
                    if (sTime.Trim() != "")
                        oCardEquAdj.BeginTime = sTime;
                    if (eTime.Trim() == "")
                        eTime = oAcsDB.GetStrScalar("SELECT ParaValue FROM B00_SysParameter WHERE ParaClass = 'System' AND ParaNo = 'DefaultEndDateTime'") + " 23:59:59";
                    oCardEquAdj.EndTime = eTime;
                    oCardEquAdj.OpMode = "*";

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
                            ((CardEquAdjObj)hCardAdj[obj[i]]).CardRule = obj[i + 1];
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
                            ((CardEquAdjObj)hCardAdj[hEquInfo[obj[i]]]).CardExtData = obj[i + 1];
                    }
                }
                #endregion

                #region 卡片資訊
                dr = null;
                if (sDataList != "")
                {
                    CardEquAdjObj oCEAO = null;

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

                    for (int i = 0; i < obj.Length; i += 2)
                    {
                        foreach (DictionaryEntry cardadj in hCardAdj)
                        {
                            oCEAO = new CardEquAdjObj();
                            oCardEquAdj = (CardEquAdjObj)cardadj.Value;
                            oCEAO.BeginTime = oCardEquAdj.BeginTime;
                            oCEAO.EndTime = oCardEquAdj.EndTime;
                            oCEAO.CardID = obj[i + 1];
                            oCEAO.CardNo = obj[i];
                            oCEAO.EquID = oCardEquAdj.EquID;
                            oCEAO.EquNo = oCardEquAdj.EquNo;
                            oCEAO.EquClass = oCardEquAdj.EquClass;
                            oCEAO.OpMode = oCardEquAdj.OpMode;
                            oCEAO.CardRule = oCardEquAdj.CardRule;
                            oCEAO.CardExtData = oCardEquAdj.CardExtData;
                            AdjQue.Enqueue(oCEAO);
                            oCEAO = null;
                        }
                    }
                    hCardAdj = null;
                }
                #endregion

                if (AdjQue.Count > 0)
                {
                    var page = HttpContext.Current.Handler as Page;
                    oAcsDB.BeginTransaction();
                    while (AdjQue.Count > 0)
                    {
                        oCardEquAdj = AdjQue.Dequeue();
                        sql = " UPDATE B01_CardEquAdj SET CardRule = '" + oCardEquAdj.CardRule + "',CardExtData = '" + oCardEquAdj.CardExtData + "',BeginTime = '" 
                            + oCardEquAdj.BeginTime + "',EndTime = '" + oCardEquAdj.EndTime + "',OpMode = '*',CreateUserID = '" + UserID + "' ,CreateTime = GETDATE() WHERE CardID = '" + oCardEquAdj.CardID + "' AND EquID = '" + oCardEquAdj.EquID + "'";// +"' AND OpMode = '-' ";
                        result = oAcsDB.SqlCommandExecute(sql);

                        if (result < 1)
                        {
                            sql = " UPDATE B01_CardEquAdj SET CardRule = '" + oCardEquAdj.CardRule + "',CardExtData = '" + oCardEquAdj.CardExtData + "',BeginTime = '" 
                                + oCardEquAdj.BeginTime + "',EndTime = '" 
                                + oCardEquAdj.EndTime + "',CreateUserID = '" + UserID + "' ,CreateTime = GETDATE() WHERE CardID = '" + oCardEquAdj.CardID + "' AND EquID = '" + oCardEquAdj.EquID + "' AND OpMode = '" + oCardEquAdj.OpMode + "' ";
                            result = oAcsDB.SqlCommandExecute(sql);
                        }

                        if (result < 1)
                        {
                            sql = " INSERT B01_CardEquAdj( CardID ,EquID ,OpMode ,CardRule ,CardExtData ,BeginTime ,EndTime ,CreateUserID ,CreateTime) ";
                            sql += " VALUES ( " + oCardEquAdj.CardID + " ," + oCardEquAdj.EquID + " ,'" + oCardEquAdj.OpMode + "' ,'" + oCardEquAdj.CardRule + "' , '" + oCardEquAdj.CardExtData + "' ,'" 
                                + oCardEquAdj.BeginTime + "' ,'" + oCardEquAdj.EndTime + "' ,'" + UserID + "' ,GETDATE()) ";
                            result = oAcsDB.SqlCommandExecute(sql);
                        }

                        if (result > 0)
                        {
                            result = oAcsDB.SqlCommandExecute(" EXEC CardAuth_Update @sCardNo = '" + oCardEquAdj.CardNo + "',@sUserID = '" + UserID + "',@sFromProc = 'CardEquAdjTemp',@sFromIP = '" + sIPAddress + "',@sOpDesc = '增加修改臨時權限' ; ");
                            if (!hProcCnt.ContainsKey(oCardEquAdj.CardNo))
                                hProcCnt.Add(oCardEquAdj.CardNo, oCardEquAdj.CardNo);
                            if (result > 0)
                            {
                                resultmsg.Enqueue(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - [卡號:" + oCardEquAdj.CardNo + "] 臨時權限 " + oCardEquAdj.OpMode + " [" + oCardEquAdj.EquNo + "] 成功");
                                oAcsDB.Commit();
                            }
                            else
                            {
                                resultmsg.Enqueue(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - [卡號:" + oCardEquAdj.CardNo + "] 臨時權限 " + oCardEquAdj.OpMode + " [" + oCardEquAdj.EquNo + "] 失敗");
                                oAcsDB.Rollback();
                            }
                        }
                        else
                        {
                            resultmsg.Enqueue(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - [卡號:" + oCardEquAdj.CardNo + "] 臨時權限 " + oCardEquAdj.OpMode + " [" + oCardEquAdj.EquNo + "] 失敗");
                            oAcsDB.Rollback();
                        }
                        oCardEquAdj = null;
                    }
                    resultmsg.Enqueue(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - 處理資料筆數: " + hProcCnt.Count + " 筆");
                    EditData = new String[resultmsg.Count];
                    for (int i = 0; i < EditData.Length; i++)
                    {
                        EditData[i] = resultmsg.Dequeue();
                    }
                }
                else
                {
                    EditData = new string[2];
                    EditData[0] = "Saho_SysErrorMassage";
                    EditData[1] = "無選取處理資料！";
                }
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "無選取處理資料！";
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
            sql = @"SELECT DISTINCT(OrgStrucID), OrgStrucNo FROM
                (SELECT B00_SysUserMgns.UserID, B00_SysUserMgns.MgaID, B01_MgnOrgStrucs.OrgStrucID, B01_OrgStruc.OrgStrucNo,
                B01_OrgStruc.OrgIDList FROM B00_SysUserMgns
                INNER JOIN B01_MgnOrgStrucs ON B00_SysUserMgns.MgaID = B01_MgnOrgStrucs.MgaID
                INNER JOIN B01_OrgStruc ON B01_MgnOrgStrucs.OrgStrucID = B01_OrgStruc.OrgStrucID) AS Mgns
                WHERE Mgns.UserID = '" + UserID;

            if (strType.Equals("組織"))
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
                case "卡片":
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

                    sql = DataListStr(wheresql) + " AND B01_Card.CardNo LIKE '%" + sQuery + "%' ORDER BY B01_Card.CardNo";
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                    break;
                case "工號":
                    wheresql = " B01_Person.PsnNo IS NOT NULL AND B01_Person.PsnNo <> '0' AND B01_Person.PsnNo LIKE '%" +
                        sQuery + "%' ";
                    sql = DataListStr(wheresql) + " ORDER BY B01_Person.PsnNo";
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                    break;
                case "姓名":
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
                    case "卡片":
                        while (dr.Read())
                        {
                            DataList.Add(dr.DataReader["CardNo"].ToString() + "|" + dr.DataReader["CardID"].ToString());
                        }
                        break;
                    case "組織":
                        while (dr.Read())
                        {
                            DataList.Add(dr.DataReader["OrgStrucNo"].ToString() + "|" + dr.DataReader["OrgStrucID"].ToString());
                        }
                        break;
                    case "工號":
                        while (dr.Read())
                        {
                            DataList.Add(dr.DataReader["PsnNo"].ToString() + "(" + dr.DataReader["PsnName"].ToString() +
                                ")|" + dr.DataReader["PsnID"].ToString());
                        }
                        break;
                    case "姓名":
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