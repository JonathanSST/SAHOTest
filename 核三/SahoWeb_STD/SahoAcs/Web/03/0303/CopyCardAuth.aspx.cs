using Sa.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SahoAcs.DBClass;

namespace SahoAcs
{
    public partial class CopyCardAuth : System.Web.UI.Page
    {

        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.SourceQueryButton);
            oScriptManager.RegisterAsyncPostBackControl(this.TargetQueryButton);
            oScriptManager.RegisterAsyncPostBackControl(this.AuthCopyButton);
            oScriptManager.RegisterAsyncPostBackControl(this.AuthAddButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("CopyCardAuth", "CopyCardAuth.js");//加入同一頁面所需的JavaScript檔案

            #region 註冊主頁Button動作
            AuthCopyButton.Attributes["onClick"] = "Block();";
            AuthAddButton.Attributes["onClick"] = "Block();";
            SourceQueryButton.Attributes["onClick"] = "SourceQueryState(); Block(); return false;";
            TargetQueryButton.Attributes["onClick"] = "TargetQueryState(); Block(); return false;";

            btnShowDetail.Attributes["onClick"] = "ShowDetail(SelectValue.value, '" + this.GetGlobalResourceObject("Resource", "NotSelectForQuery") + "') ; return false;";
            #endregion

            #region 語系切換
            //DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            //oAcsDB.GetTableHash("B01_DeviceConnInfo", "zhtw", out TableInfo);
            //Label_No.Text = TableInfo["DciNo"].ToString();
            //Label_Name.Text = TableInfo["DciName"].ToString();
            //Label_Ip.Text = TableInfo["IpAddress"].ToString();
            //Label_IsAssign.Text = TableInfo["IsAssignIP"].ToString();
            //popLabel_No.Text = TableInfo["DciNo"].ToString();
            //popLabel_Name.Text = TableInfo["DciName"].ToString();
            //popLabel_PassWD.Text = TableInfo["DciPassWD"].ToString();
            //popLabel_Ip.Text = TableInfo["IpAddress"].ToString();
            //popLabel_Port.Text = TableInfo["TcpPort"].ToString();
            //popLabel_IsAssign.Text = TableInfo["IsAssignIP"].ToString();
            #endregion

            #endregion

            this.tbAuth.Visible = false;

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion

                ViewState["query_SourceType"] = "";
                ViewState["query_SourceCardNo"] = "";
                ViewState["query_SourcePsnInfo"] = "";
                ViewState["query_TargetCardNo"] = "";
                ViewState["query_TargetPsnInfo"] = "";
                CreateSourceTypeDropItem();
                LoadSourceCardList();
                LoadTargetCardList();
                //ViewState["EquList"] = null;
                //顯示筆數訊息
                this.lblSource.Text = string.Format(GetLocalResourceObject("ttCardSource").ToString(), 0);
                this.lblTarget.Text = string.Format(GetLocalResourceObject("ttCardTarget").ToString(), 0);
                this.lblEquList.Text = string.Format(GetLocalResourceObject("lblCardAuthCopyList").ToString(), 0);
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                try
                {
                    if (sFormTarget == this.SourceQueryButton.ClientID)
                    {
                        //ViewState["query_SourceType"] = this.Input_SourceType.SelectedValue.Trim();
                        //ViewState["query_SourceCardNo"] = this.Input_SourceCardNo.Text.Trim();
                        //ViewState["query_SourcePsnInfo"] = this.Input_SourcePsnInfo.Text.Trim();
                        LoadSourceCardList();
                    }

                    if (sFormTarget == this.TargetQueryButton.ClientID)
                    {
                        LoadTargetCardList();
                    }

                    if (sFormTarget == this.EquListUpdatePanel.ClientID)
                    {
                        CreateEuqList(sFormArg);
                    }

                    if (sFormTarget == this.SourcePanel.ClientID)
                    {
                        Sa.Web.Fun.RunJavaScript(this, "GridSelect();");

                        Sa.Web.Fun.RunJavaScript(this, "ContentPlaceHolder1_SourcePanel.scrollTop = SourcePanelScrollValue.value ;");
                    }

                }
                catch { }
                finally
                {
                    // 清除 __EVENTTARGET、__EVENTARGUMENT 的值
                    Sa.Web.Fun.RunJavaScript(this.Page,
                        @" theForm.__EVENTTARGET.value   = '' ;
                       theForm.__EVENTARGUMENT.value = '' ; ");

                    Sa.Web.Fun.RunJavaScript(this.Page, "$.unblockUI();");
                }
            }
        }
        #endregion

        #region SourceGridView_Data_RowDataBound
        public void SourceGridView_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    int[] HeaderWidth = { 100, 100 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理
                        
                    #endregion

                    #region 寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    li_Sourceheader.Text = Header_sw.ToString();
                    #endregion
                    break;
                #endregion
                #region DataRow
                case DataControlRowType.DataRow:
                    DataRowView oRow = (DataRowView)e.Row.DataItem;

                    #region 指定Row的ID
                    GridViewRow GrRow = e.Row;
                    GrRow.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    GrRow.ID = "GV_Row" + oRow.Row["CardNo"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 105, 106 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    e.Row.Height = 23;
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 卡片編號
                    #endregion
                    #region 人員編號
                    #endregion
                    #region 人員姓名

                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    //for (int x = 0; x < e.Row.Cells.Count; x++)
                    //{
                    //    if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                    //        e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    //}
                    //e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 28, true);
                    //e.Row.Cells[5].Text = LimitText(e.Row.Cells[5].Text, 23, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    //e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["CardNo"].ToString() + "', '', ''); Block(); ShowEquList(); "); //ShowEquList();
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["CardNo"].ToString() + "', '', '');");

                    e.Row.Attributes.Add("OnDblclick", "ShowDetail('" + oRow.Row["CardNo"].ToString() + "', '')");
                    break;
                #endregion
            }
        }
        #endregion

        #region SourceGridView_Data_DataBound
        protected void SourceGridView_Data_DataBound(object sender, EventArgs e)
        {
            td_showSourceGridView.Attributes["colspan"] = SourceGridView.Columns.Count.ToString();
        }
        #endregion

        #region TargetGridView_Data_RowDataBound
        public void TargetGridView_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    int[] HeaderWidth = { 40, 100, 100 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = HeaderWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #endregion

                    #region 寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    li_Targetheader.Text = Header_sw.ToString();
                    #endregion
                    break;
                #endregion

                #region DataRow
                case DataControlRowType.DataRow:
                    DataRowView oRow = (DataRowView)e.Row.DataItem;

                    #region 指定Row的ID
                    GridViewRow GrRow = e.Row;
                    GrRow.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    GrRow.ID = "GV_Row" + oRow.Row["CardNo"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    int[] DataWidth = { 45, 106, 106 };
                    for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                        e.Row.Cells[i].Width = DataWidth[i];
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 控制
                    CheckBox ChkBox = (CheckBox)e.Row.FindControl("SelectControl");
                    ChkBox.ID = "SelectControl" + e.Row.RowIndex;
                    e.Row.Attributes.Add("OnClick", " CheckBoxSelected('" + ChkBox.ClientID.ToString() + "');");
                    ChkBox.Attributes.Add("OnClick", " CheckBoxSelected('" + ChkBox.ClientID.ToString() + "');");
                    #endregion
                    #region 卡片編號
                    #endregion
                    #region 人員編號
                    #endregion
                    #region 人員姓名

                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    //for (int x = 0; x < e.Row.Cells.Count; x++)
                    //{
                    //    if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                    //        e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    //}
                    //e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text, 28, true);
                    //e.Row.Cells[5].Text = LimitText(e.Row.Cells[5].Text, 23, true);
                    #endregion
                    break;
                #endregion
            }
        }
        #endregion

        #region TargetGridView_Data_DataBound
        protected void TargetGridView_Data_DataBound(object sender, EventArgs e)
        {
            td_showTargetGridView.Attributes["colspan"] = TargetGridView.Columns.Count.ToString();
        }
        #endregion

        #region AuthCopyButton_Click 權限覆蓋
        protected void AuthCopyButton_Click(object sender, EventArgs e)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sSql = "", ErrorMsg = "";
            int iResult = 0, iSuccess = 0, iError = 0;
            List<string> liSqlPara = new List<string>();
            Pub.MessageObject objRet = new Pub.MessageObject();
            DateTime Time = DateTime.Now;
            string liSurCardList = "", liTarCardList = "";
            string strSurCardID = "", strSurCardNo = "";
            string[] strSurCardAry, strTarCardIdAry, strTarCardNoAry;

            GetUIValue(out liSurCardList, out liTarCardList);

            // 取得來源卡片的CardID、CardNo
            strSurCardAry = liSurCardList.Split(',');
            if (strSurCardAry.Length >= 2)
            {
                strSurCardID = strSurCardAry[0].ToString();
                strSurCardNo = strSurCardAry[1].ToString();
            }

            // 取得目標卡片的CardNo，轉成CardID存到strTarCardIdAry
            strTarCardNoAry = liTarCardList.Split(',');
            strTarCardIdAry = CardIDChange(strTarCardNoAry);

            #region 判斷目標卡片的PsnId是否在使用臨時卡，若是則連那張臨時卡也需要加入陣列處理
            // 先組出 SQL WHERE IN 的字串， EX. (1,2,3)
            string strInCondition = "";
            for (int k = 0; k < strTarCardIdAry.Length; k++)
            {
                strInCondition += strTarCardIdAry[k] + ",";
            }
            strInCondition = strInCondition.Remove(strInCondition.Length - 1);
            strInCondition = "(" + strInCondition + ")";

            sSql = @"
                SELECT CardId,CardNo FROM B01_Card WHERE CardType = 'R' AND PsnID IN 
                ( SELECT PsnID FROM B01_Card WHERE CardType = 'E' AND CardID IN " + strInCondition + " )";

            DataTable dtTmp = new DataTable();
            oAcsDB.GetDataTable("dtTmp", sSql, out dtTmp);

            if (dtTmp.Rows.Count > 0)
            {
                foreach (DataRow dr in dtTmp.Rows)
                {
                    Array.Resize(ref strTarCardIdAry, strTarCardIdAry.Length + 1);
                    strTarCardIdAry[strTarCardIdAry.Length - 1] = dr["CardId"].ToString();

                    Array.Resize(ref strTarCardNoAry, strTarCardNoAry.Length + 1);
                    strTarCardNoAry[strTarCardNoAry.Length - 1] = dr["CardNo"].ToString();
                }     
            }
            #endregion

            objRet = Check_Input(strSurCardID, liTarCardList);

            if (objRet.result)
            {
                for (int i = 0; i < strTarCardIdAry.Length; i++)
                {
                    try
                    {
                        // 當來源卡片和目標卡片不同時才執行
                        if (strSurCardID != strTarCardIdAry[i])
                        {
                            #region 刪除目標卡片的B01_CardAuth、B01_CardEquAdj、B01_CardEquGroup的資料
                            //sSql = @" DELETE FROM B01_CardAuth WHERE CardNo = ? 
                            //          DELETE FROM B01_CardEquAdj WHERE CardID = ? ";

                            sSql = @" DELETE FROM B01_CardEquAdj WHERE CardID = ? 
                                      DELETE FROM B01_CardEquGroup WHERE CardID = ? ";

                            liSqlPara.Clear();
                            liSqlPara.Add("A:" + strTarCardIdAry[i].ToString());
                            liSqlPara.Add("A:" + strTarCardIdAry[i].ToString());
                            #endregion

                            #region 複製來源卡片相關的卡片設備權限到每一個目的卡片

                            sSql += @" 
                                INSERT INTO B01_CardEquAdj
                                (
                                    CardID, EquID, OpMode, CardRule, CardExtData, 
                                    BeginTime, EndTime, CreateUserID, CreateTime 
                                )
                                SELECT DISTINCT 
	                                ? , ED.EquID, CEA.OpMode, CEA.CardRule, CEA.CardExtData, 
	                                CEA.BeginTime, CEA.EndTime, ?, ? 
                                FROM B01_CardEquAdj AS CEA 
                                    INNER JOIN B01_Card AS CA ON CA.CardID = CEA.CardID	
                                    INNER JOIN B01_EquData AS ED ON ED.EquID = CEA.EquID
                                    INNER JOIN B01_EquGroupData AS EGD ON EGD.EquID = ED.EquID
                                    INNER JOIN B01_EquGroup AS EGP ON EGP.EquGrpID = EGD.EquGrpID
                                    INNER JOIN B01_MgnEquGroup AS MEG ON MEG.EquGrpID = EGP.EquGrpID
                                    INNER JOIN B00_SysUserMgns AS SUMS ON SUMS.MgaID = MEG.MgaID  
                                WHERE CEA.OpMode <> 'Del' AND CEA.OpMode <> '*' 
                                    AND CA.CardNo = ? AND SUMS.UserID = ? ";

                            liSqlPara.Add("A:" + strTarCardIdAry[i].ToString());
                            liSqlPara.Add("A:" + this.hideUserID.Value);
                            liSqlPara.Add("D:" + Time);
                            liSqlPara.Add("A:" + strSurCardNo.ToString());
                            liSqlPara.Add("A:" + this.hideUserID.Value);

                            sSql += @" 
                                INSERT INTO B01_CardEquGroup 
                                    ( CardID, EquGrpID, CreateUserID, CreateTime ) 
                                SELECT DISTINCT 
                                    ? , CEG.EquGrpID , ? , ?  
                                FROM B01_CardEquGroup CEG 
                                    INNER JOIN B01_EquGroup EGP ON EGP.EquGrpID = CEG.EquGrpID
                                    INNER JOIN B01_MgnEquGroup AS MEG ON MEG.EquGrpID = EGP.EquGrpID
                                    INNER JOIN B00_SysUserMgns AS SUMS ON SUMS.MgaID = MEG.MgaID
                                WHERE CEG.CardID = ? AND SUMS.UserID = ? ";

                            liSqlPara.Add("A:" + strTarCardIdAry[i].ToString());
                            liSqlPara.Add("A:" + this.hideUserID.Value);
                            liSqlPara.Add("D:" + Time);
                            liSqlPara.Add("A:" + strSurCardID.ToString());
                            liSqlPara.Add("A:" + this.hideUserID.Value);

                            oAcsDB.BeginTransaction();

                            try
                            {
                                iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);

                                if (iResult > 0)
                                {
                                    oAcsDB.Commit();
                                }
                                else
                                {
                                    oAcsDB.Rollback();

                                    iError++;
                                    ErrorMsg += GetLocalResourceObject("lblCardNo").ToString() + "：" + strTarCardNoAry[i].ToString() + Environment.NewLine;
                                }
                            }
                            catch (Exception ex)
                            {
                                oAcsDB.Rollback();

                                iError++;
                                ErrorMsg += GetLocalResourceObject("lblCardNo").ToString() + "：" + strTarCardNoAry[i].ToString() + " ([Exception]" + ex.Message + ") " + Environment.NewLine;
                            }

                            #endregion

                            #region 對目標卡片執行 CardAuth_Update
                            if (iResult > 0)
                            {
                                bool isSuccess =
                                    ExecCardAuthUpdate(strTarCardNoAry[i].ToString(), hideUserID.Value, true);

                                if (isSuccess)
                                {
                                    iSuccess++;
                                }
                                else
                                {
                                    iError++;
                                    ErrorMsg += GetLocalResourceObject("MsgProcessErr").ToString()
                                + "：" + strTarCardNoAry[i].ToString() + " EXEC CardAuth_Update Failure!" + Environment.NewLine;
                                }
                            }

                            #endregion
                        }
                    }
                    catch
                    {
                        iError++;
                        ErrorMsg += GetLocalResourceObject("MsgProcessErr").ToString() + "：" + strTarCardNoAry[i].ToString() + Environment.NewLine;
                    }
                }
            }
            else
            {
                Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('" + objRet.message.ToString() + "');");
            }
                
            MsgResultTextBox.Text = "";

            // 卡片權限複製完成
            MsgResultTextBox.Text += GetLocalResourceObject("MsgCopyEnd").ToString() + "！" + Environment.NewLine;

            // 複製成功筆數
            MsgResultTextBox.Text += GetLocalResourceObject("MsgCopySuccessCnt").ToString() + "：" + iSuccess.ToString() + Environment.NewLine;

            // 複製失敗筆數
            MsgResultTextBox.Text += GetLocalResourceObject("MsgCopyFailCnt").ToString()+"：" + iError.ToString() + Environment.NewLine;

            if (iError != 0)
            {
                // 失敗卡片
                MsgResultTextBox.Text += GetLocalResourceObject("MsgErrCards").ToString() + "：" + Environment.NewLine;
                MsgResultTextBox.Text += ErrorMsg;
            }

            MsgResultUpdatePanel.Update();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI", "$.unblockUI()", true);
        }
        #endregion

        #region AuthAddButton_Click 權限附加
        protected void AuthAddButton_Click(object sender, EventArgs e)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sSql = "", ErrorMsg = "";
            int iResult = 0, iSuccess = 0, iError = 0;
            Pub.MessageObject objRet = new Pub.MessageObject();
            DateTime Time = DateTime.Now;
            string liSurCardList = "", liTarCardList = "";
            string strSurCardID = "", strSurCardNo = "";
            string ConflictActMode = "";

            GetUIValue(out liSurCardList, out liTarCardList);

            // 取得來源卡片的CardID、CardNo
            string[] strSurCardAry = liSurCardList.Split(',');
            if (strSurCardAry.Length >= 2)
            {
                strSurCardID = strSurCardAry[0].ToString();
                strSurCardNo = strSurCardAry[1].ToString();
            }

            // 取得目標卡片的CardNo，轉成CardID存到strTarCardIdAry
            string[] strTarCardNoAry = liTarCardList.Split(',');
            string[] strTarCardIdAry = CardIDChange(strTarCardNoAry);

            // 選擇 新參數 或 舊參數
            ConflictActMode = (Input_ConflictAct1.Checked) ? "SourceValue" : (Input_ConflictAct2.Checked) ? "TargetValue" : "";

            objRet = Check_Input(strSurCardID, liTarCardList);

            if (objRet.result)
            {
                bool isSuccess = true;

                for (int i = 0; i < strTarCardIdAry.Length; i++)
                {
                    try
                    {
                        // 當來源卡片和目標卡片不同時才執行
                        if (strSurCardID != strTarCardIdAry[i])
                        {
                            DataTable dt = new DataTable();
                            DataTable dtGroup = new DataTable();
                            List<string> liSqlPara = new List<string>();

                            #region 取得來源卡片的權限資料
                            sSql = @"
                                SELECT DISTINCT
                                    ED.EquID, CEA.OpMode, CEA.CardRule, CEA.CardExtData, 
	                                CEA.BeginTime, CEA.EndTime, ED.EquClass, 
                                     tar.CardRule AS tar_CardRule, tar.CardExtData AS tar_CardExtData 
                                FROM B01_CardEquAdj AS CEA 
                                INNER JOIN B01_EquData AS ED ON ED.EquID = CEA.EquID
                                INNER JOIN B01_EquGroupData AS EGD ON EGD.EquID = ED.EquID
                                INNER JOIN B01_MgnEquGroup AS MEG ON MEG.EquGrpID = EGD.EquGrpID
                                INNER JOIN B00_SysUserMgns AS SUMS ON SUMS.MgaID = MEG.MgaID 
                                LEFT JOIN B01_CardEquAdj AS tar ON tar.EquID = CEA.EquID AND tar.CardID = ? 
                                WHERE CEA.OpMode <> 'Del' AND CEA.OpMode <> '*' AND ED.EquID > -1
                                AND SUMS.UserID = ?  
                                AND CEA.CardID = ?";

                            liSqlPara.Clear();
                            liSqlPara.Add("A:" + strTarCardIdAry[i].ToString());
                            liSqlPara.Add("A:" + hideUserID.Value);
                            liSqlPara.Add("A:" + strSurCardID.ToString());

                            try
                            {
                                isSuccess = oAcsDB.GetDataTable("pt", sSql, liSqlPara, out dt);

                                if (!isSuccess)
                                {
                                    iError++;
                                    ErrorMsg += GetLocalResourceObject("lblCardNo") + "：" + strTarCardNoAry[i].ToString() + Environment.NewLine;
                                }
                            }
                            catch (Exception ex)
                            {
                                iError++;
                                ErrorMsg += GetLocalResourceObject("lblCardNo") + "：" + strTarCardNoAry[i].ToString() + " ([Exception01]" + ex.Message + ") " + Environment.NewLine;
                            }
                            #endregion

                            #region 取得來源卡片的群組權限資料
                            if (isSuccess)
                            {
                                sSql = @"
                                    SELECT DISTINCT 
                                        CEG.EquGrpID  
                                    FROM B01_CardEquGroup CEG 
	                                    INNER JOIN B01_Card AS CA ON CA.CardID = CEG.CardID
                                        INNER JOIN B01_EquGroup EGP ON EGP.EquGrpID = CEG.EquGrpID
                                        INNER JOIN B01_MgnEquGroup AS MEG ON MEG.EquGrpID = EGP.EquGrpID
                                        INNER JOIN B00_SysUserMgns AS SUMS ON SUMS.MgaID = MEG.MgaID 
                                    WHERE SUMS.UserID = ? 
                                    AND CA.CardNo = ? 
                                    AND	CEG.EquGrpID NOT IN	
                                    (
	                                    SELECT EquGrpID FROM B01_CardEquGroup WHERE CardID =
	                                    ( SELECT CardID FROM B01_Card WHERE CardNo = ? )
                                    ) ";

                                liSqlPara.Clear();
                                liSqlPara.Add("A:" + hideUserID.Value);
                                liSqlPara.Add("A:" + strSurCardNo.ToString());
                                liSqlPara.Add("A:" + strTarCardNoAry[i].ToString());

                                try
                                {
                                    isSuccess = oAcsDB.GetDataTable("pgt", sSql, liSqlPara, out dtGroup);

                                    if (!isSuccess)
                                    {
                                        iError++;
                                        ErrorMsg += GetLocalResourceObject("lblCardNo") + "：" + strTarCardNoAry[i].ToString() + Environment.NewLine;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    iError++;
                                    ErrorMsg += GetLocalResourceObject("lblCardNo") + "：" + strTarCardNoAry[i].ToString() + " ([Exception02]" + ex.Message + ") " + Environment.NewLine;
                                }
                            }
                            #endregion

                            #region 附加權限資料和權限群組資料到目的卡片
                            if (isSuccess)
                            {
                                #region 設備權限資料部份  CardEquAdj

                                oAcsDB.BeginTransaction();

                                foreach (DataRow dr in dt.Rows)
                                {
                                    /*
                                        (一) 目標卡片無該權限，即新增該權限，無視新、舊參數。
                                        (二) 目標卡片有該權限，即修改該權限，依新、舊參數判斷，
                                             並要刪掉 B01_CardAuth 的權限資料
                                    */
                                    if (Convert.IsDBNull(dr["tar_CardRule"]) ||
                                        Convert.IsDBNull(dr["tar_CardExtData"]))
                                    {
                                        #region (一) 目標卡片無該權限
                                        string strEquClass = dr["EquClass"].ToString().Trim();
                                        string strCardExtData = dr["CardExtData"].ToString().Trim();
                                        strCardExtData = CardExtDataForElevator(strEquClass, strCardExtData);

                                        sSql = @" 
                                            INSERT INTO B01_CardEquAdj
                                            (
                                                CardID, EquID, OpMode, CardRule, CardExtData, 
                                                BeginTime, EndTime, CreateUserID, CreateTime 
                                            )
                                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?) ";

                                        // 當結束時間是空白時，改讀取卡片的結束時間替代
                                        string strEndTime = dr["EndTime"].ToString().Trim();
                                        if (strEndTime == "")
                                        {
                                            strEndTime = oAcsDB.GetStrScalar(string.Format(@"
                                                SELECT CardETime FROM B01_Card WHERE CardID='{0}'", strTarCardIdAry[i]));
                                        }

                                        liSqlPara.Clear();
                                        liSqlPara.Add("A:" + strTarCardIdAry[i]);
                                        liSqlPara.Add("A:" + dr["EquID"].ToString().Trim());
                                        liSqlPara.Add("S:" + dr["OpMode"].ToString().Trim());
                                        liSqlPara.Add("A:" + dr["CardRule"].ToString().Trim());
                                        liSqlPara.Add("A:" + strCardExtData);
                                        liSqlPara.Add("D:" + dr["BeginTime"].ToString().Trim());
                                        liSqlPara.Add("D:" + strEndTime);
                                        liSqlPara.Add("A:" + hideUserID.Value);
                                        liSqlPara.Add("D:" + Time.ToString());

                                        iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);

                                        if (iResult < 0)
                                        {
                                            isSuccess = false;
                                            break;
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region (二) 目標卡片有該權限
                                        string strCardRule = "";
                                        string strCardExtData = "";
                                        string strEquClass = dr["EquClass"].ToString().Trim();
                                        string strEquID = dr["EquID"].ToString().Trim();

                                        switch (ConflictActMode)
                                        {
                                            case "SourceValue":
                                                strCardRule = dr["CardRule"].ToString();
                                                strCardExtData = dr["CardExtData"].ToString();
                                                break;
                                            case "TargetValue":
                                                strCardRule = dr["tar_CardRule"].ToString();
                                                strCardExtData = dr["tar_CardExtData"].ToString();
                                                break;
                                        }

                                        strCardExtData = CardExtDataForElevator(strEquClass, strCardExtData);

                                        sSql = @"
                                            UPDATE B01_CardEquAdj SET
                                                CardRule = ?,
                                                CardExtData = ?, 
                                                CreateUserID = ? , 
                                                CreateTime = ? 
                                            WHERE EquID = ? AND CardId = ? ";

                                        liSqlPara.Clear();
                                        liSqlPara.Add("A:" + strCardRule);
                                        liSqlPara.Add("A:" + strCardExtData);
                                        liSqlPara.Add("A:" + hideUserID.Value);
                                        liSqlPara.Add("D:" + Time.ToString());
                                        liSqlPara.Add("A:" + strEquID);
                                        liSqlPara.Add("A:" + strTarCardIdAry[i]);

                                        sSql += @" DELETE FROM B01_CardAuth WHERE EquID = ? AND CardNo = ? ";
                                        liSqlPara.Add("A:" + strEquID);
                                        liSqlPara.Add("A:" + strTarCardNoAry[i]);

                                        iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);

                                        if (iResult < 0)
                                        {
                                            isSuccess = false;
                                            break;
                                        }

                                        #endregion
                                    }
                                }

                                if (isSuccess)
                                {
                                    oAcsDB.Commit();
                                }
                                else
                                {
                                    oAcsDB.Rollback();
                                }

                                #endregion

                                #region 權限群組資料部份   CardEquGroup

                                if (isSuccess)
                                {
                                    oAcsDB.BeginTransaction();

                                    foreach (DataRow drg in dtGroup.Rows)
                                    {
                                        sSql = @" 
                                    INSERT INTO B01_CardEquGroup  
                                    (
                                        CardID, EquGrpID, CreateUserID, CreateTime 
                                    ) 
                                    VALUES (?, ?, ?, ?) ";

                                        liSqlPara.Clear();
                                        liSqlPara.Add("A:" + strTarCardIdAry[i]);
                                        liSqlPara.Add("A:" + drg["EquGrpID"].ToString().Trim());
                                        liSqlPara.Add("A:" + hideUserID.Value);
                                        liSqlPara.Add("D:" + Time.ToString());

                                        iResult = oAcsDB.SqlCommandExecute(sSql, liSqlPara);

                                        if (iResult < 0)
                                        {
                                            isSuccess = false;
                                            break;
                                        }
                                    }

                                    if (isSuccess)
                                    {
                                        oAcsDB.Commit();
                                    }
                                    else
                                    {
                                        oAcsDB.Rollback();
                                    }
                                }

                                #endregion

                                if (!isSuccess)
                                {
                                    iError++;
                                    ErrorMsg += GetLocalResourceObject("MsgProcessErr").ToString() + "：" + strTarCardNoAry[i].ToString() + Environment.NewLine;
                                }
                            }
                            #endregion

                            #region 對目標卡片執行 CardAuth_Update
                            if (isSuccess)
                            {
                                // 執行 CardAuthUpdate
                                isSuccess =
                                    ExecCardAuthUpdate(strTarCardNoAry[i].ToString(), hideUserID.Value, true);

                                if (isSuccess)
                                {
                                    iSuccess++;
                                }
                                else
                                {
                                    iError++;
                                    ErrorMsg += GetLocalResourceObject("MsgProcessErr").ToString()
                                + "：" + strTarCardNoAry[i].ToString() + " EXEC CardAuth_Update Failure!" + Environment.NewLine;
                                }
                            }
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        oAcsDB.Rollback();
                        iError++;
                        ErrorMsg += GetLocalResourceObject("MsgProcessErr").ToString()
                            + "：" + strTarCardNoAry[i].ToString() + "，" + ex.Message + Environment.NewLine;
                    }
                }
            }
            else
            {
                Sa.Web.Fun.RunJavaScript(this, "ShowErrorMsg('" + objRet.message.ToString() + "');");
            }
               
            MsgResultTextBox.Text = "";

            MsgResultTextBox.Text += GetLocalResourceObject("MsgAddEnd").ToString() + "！" + Environment.NewLine;
            MsgResultTextBox.Text +=GetLocalResourceObject("MsgCopySuccessCnt").ToString()+ "：" + iSuccess.ToString() + Environment.NewLine;
            MsgResultTextBox.Text += GetLocalResourceObject("MsgCopyFailCnt").ToString() + "：" + iError.ToString() + Environment.NewLine;

            if (iError != 0)
            {
                MsgResultTextBox.Text += GetLocalResourceObject("MsgErrCards").ToString() + "：" + Environment.NewLine;
                MsgResultTextBox.Text += ErrorMsg;
            }

            MsgResultUpdatePanel.Update();

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI", "$.unblockUI()", true);
        }
        #endregion

        #endregion

        #region Method

        #region CreateSourceTypeDropItem
        private void CreateSourceTypeDropItem()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt = new DataTable();
            ListItem Item = new ListItem();
            
            sql = @" SELECT ItemNo,ItemName FROM B00_ItemList
                     WHERE ItemClass = 'CardType' AND ItemNo NOT IN ('R','T') ";

            oAcsDB.GetDataTable("DropListItem", sql, out dt);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Item = new ListItem();
                    Item.Text = dr["ItemName"].ToString();
                    Item.Value = dr["ItemNo"].ToString();
                    this.Input_SourceType.Items.Add(Item);
                }
            }
            else
            {
                #region Give Empty Item
                Item = new ListItem();
                Item.Text = "- " + GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString() + " -";
                Item.Value = "";
                this.Input_SourceType.Items.Add(Item);
                #endregion
            }
        }
        #endregion

        #region LoadSourceCardList
        public void LoadSourceCardList()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "", strCount = "";
            DataTable dt = new DataTable();
            List<string> liSqlPara = new List<string>();
            string NowCondition = Input_Condition1.Text.Trim();

            #region 清空EquListPanel和EquGroupListPanel、MsgResultTextBox
            this.lblEquList.Text = 
                string.Format(GetLocalResourceObject("lblCardAuthCopyList").ToString(), dt.Rows.Count);
            EquListPanel.Controls.Clear();
            EquListUpdatePanel.Update();

            this.lblEquGroupList.Text = 
                string.Format(GetLocalResourceObject("lblCardAuthGroupCopyList").ToString(), dt.Rows.Count);
            EquGroupListPanel.Controls.Clear();
            EquGroupListUpdatePanel.Update();

            MsgResultTextBox.Text = "";
            #endregion

            #region Process String
            sql = @" SELECT DISTINCT TOP 10   
                     Card.CardNo,
                     Person.PsnNo, Person.PsnName
                     FROM B01_Card AS Card
                     LEFT JOIN B01_Person AS Person ON Person.PsnID = Card.PsnID
                     INNER JOIN B01_OrgStruc AS OrgStruc ON OrgStruc.OrgStrucID = Person.OrgStrucID
                     INNER JOIN B01_MgnOrgStrucs AS MgnOrgStrucs ON MgnOrgStrucs.OrgStrucID = OrgStruc.OrgStrucID
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnOrgStrucs.MgaID
                     INNER JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID 
                     WHERE SysUser.UserID = ? ";

            liSqlPara.Add("S:" + this.hideUserID.Value);

            if (string.IsNullOrEmpty(NowCondition))
            {
                wheresql += " AND 1=0 ";
            }

            if (!string.IsNullOrEmpty(this.Input_SourceType.SelectedValue.Trim()))
            {
                wheresql += " AND Card.CardType = ? ";
                liSqlPara.Add("S:" + this.Input_SourceType.SelectedValue.Trim()); 
            }

            if (!string.IsNullOrEmpty(NowCondition))
            {
                string[] strCondiition = new string[] { "\t", " ", "　", ",", "，", "、", "。", "\n" };
                string[] strary = NowCondition.Split(strCondiition, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i <= strary.Length - 1; i++)
                {
                    if (strary[i].Trim() != "")
                    {
                        wheresql += " AND (ISNULL(Card.CardNo,'') + '__' + ISNULL(Person.PsnNo,'') + '__' + ISNULL(Person.PsnName,'')) LIKE ? ";
                        liSqlPara.Add("S:" + "%" + strary[i].Trim() + "%");
                    }
                }
            }

            sql += wheresql + " ORDER BY Card.CardNo  ";
            #endregion

            oAcsDB.GetDataTable("CardListTable", sql, liSqlPara, out dt);
            strCount = dt.Rows.Count.ToString();
            GirdViewDataBind(this.SourceGridView, dt);
            this.lblSource.Text = string.Format(GetLocalResourceObject("ttCardSource").ToString(), strCount);
            SourceUpdatePanel.Update();
        }
        #endregion

        #region LoadTargetCardList
        public void LoadTargetCardList()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "", strCount = "";
            DataTable dt;
            List<string> liSqlPara = new List<string>();
            string NowCondition = Input_Condition2.Text.Trim();

            #region Process String
            sql = @" SELECT DISTINCT TOP 50  
                     Card.CardNo,
                     Person.PsnNo, Person.PsnName
                     FROM dbo.B01_Card AS Card
                     LEFT JOIN dbo.B01_Person AS Person ON Person.PsnID = Card.PsnID
                     INNER JOIN B01_OrgStruc AS OrgStruc ON OrgStruc.OrgStrucID = Person.OrgStrucID
                     INNER JOIN B01_MgnOrgStrucs AS MgnOrgStrucs ON MgnOrgStrucs.OrgStrucID = OrgStruc.OrgStrucID
                     INNER JOIN B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnOrgStrucs.MgaID
                     INNER JOIN B00_SysUser AS SysUser ON SysUser.UserID = SysUserMgns.UserID 
                     WHERE SysUser.UserID = ? ";

            liSqlPara.Add("S:" + this.hideUserID.Value);

            if (string.IsNullOrEmpty(NowCondition))
            {
                wheresql += " AND 1=0 ";
            }

            if (!string.IsNullOrEmpty(this.Input_SourceType.SelectedValue.Trim()))
            {
                wheresql += " AND Card.CardType = ? ";
                liSqlPara.Add("S:" + this.Input_SourceType.SelectedValue.Trim());
            }

            if (!string.IsNullOrEmpty(NowCondition))
            {
                string[] strCondiition = new string[] { "\t", " ", "　", ",", "，", "、", "。", "\n" };
                string[] strary = NowCondition.Split(strCondiition, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i <= strary.Length - 1; i++)
                {
                    if (strary[i].Trim() != "")
                    {
                        wheresql += " AND (ISNULL(Card.CardNo,'') + '__' + ISNULL(Person.PsnNo,'') + '__' + ISNULL(Person.PsnName,'')) LIKE ? ";
                        liSqlPara.Add("S:" + "%" + strary[i].Trim() + "%");
                    }
                }
            }

            sql += wheresql + " ORDER BY Card.CardNo ";
            #endregion

            oAcsDB.GetDataTable("CardListTable", sql, liSqlPara, out dt);
            strCount = dt.Rows.Count.ToString();
            GirdViewDataBind(this.TargetGridView, dt);
            this.lblTarget.Text = string.Format(GetLocalResourceObject("ttCardTarget").ToString(), strCount);
            TargetUpdatePanel.Update();
        }
        #endregion

        #region CreateEuqList

        private void CreateEuqList(string SelectValue)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sSQL = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt = new DataTable();
            bool isSuccess = true;
            bool isBtn01 = true;
            bool isBtn02 = true;

            #region 取得 B01_CardEquAdj 的資料，並秀在EquList上
            sSQL = @" 
                SELECT 
                    DISTINCT ED.EquNo, ED.EquName, CEA.OpMode 
                FROM B01_CardEquAdj AS CEA 
                INNER JOIN B01_Card AS CA ON CA.CardID = CEA.CardID	
                INNER JOIN B01_EquData AS ED ON ED.EquID = CEA.EquID
                INNER JOIN B01_EquGroupData AS EGD ON EGD.EquID = ED.EquID
                INNER JOIN B01_EquGroup AS EGP ON EGP.EquGrpID = EGD.EquGrpID
                INNER JOIN B01_MgnEquGroup AS MEG ON MEG.EquGrpID = EGP.EquGrpID
                INNER JOIN B00_SysUserMgns AS SUMS ON SUMS.MgaID = MEG.MgaID
                WHERE CEA.OpMode <> 'Del' AND CEA.OpMode <> '*' 
                AND CA.CardNo = ? 
                AND SUMS.UserID = ? ";

            liSqlPara.Clear();
            liSqlPara.Add("S:" + SelectValue.Trim());
            liSqlPara.Add("S:" + hideUserID.Value);

            try
            {
                isSuccess = oAcsDB.GetDataTable("dtEquList", sSQL, liSqlPara, out dt);

                if (isSuccess)
                {
                    if (dt.Rows.Count > 0)
                    {
                        TableRow tr = new TableRow();

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (i % 2 == 0)
                            {
                                tr = new TableRow();
                            }

                            TableCell td = new TableCell();
                            td.Width = 280;
                            td.Text = "(" + dt.Rows[i]["EquNo"] + ") " + dt.Rows[i]["EquName"] + " [" + dt.Rows[i]["OpMode"] + "]";
                            td.Style.Add("white-space", "nowrap");
                            td.Style.Add("Padding", "2px");
                            tr.Controls.Add(td);

                            if (i == dt.Rows.Count - 1 && dt.Rows.Count % 2 != 0)
                            {
                                for (int k = 0; k < 2 - (dt.Rows.Count % 2); k++)
                                {
                                    td = new TableCell();
                                    td.Style.Add("white-space", "nowrap");
                                    td.Width = 280;
                                    td.Style.Add("Padding", "2px");
                                    tr.Controls.Add(td);
                                }
                            }

                            EquList.Controls.Add(tr);
                        }

                        isBtn01 = true;
                    }
                    else
                    {
                        TableRow tr = new TableRow();
                        TableCell td = new TableCell();
                        td.Text = GetLocalResourceObject("MsgEquAuth").ToString();
                        td.Width = 600;
                        tr.Controls.Add(td);
                        this.EquList.Controls.Add(tr);

                        isBtn01 = false; 
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert01",
                        string.Format("alert('[CreateEuqList01]' {0});", "Load Data failure！"), true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert01", 
                    string.Format("alert('[CreateEuqList01]' {0});", ex.Message), true);
            }

            EquList.Style.Add("word-break", "break-all");
            EquList.Attributes.Add("border", "1");
            EquList.Style.Add("border-color", "#999999");
            EquList.Style.Add("border-style", "solid");
            EquList.Style.Add("border-collapse", "collapse");
            lblEquList.Text = 
                string.Format(GetLocalResourceObject("lblCardAuthCopyList").ToString(), dt.Rows.Count);
            EquListUpdatePanel.Update();

            #endregion

            #region 取得 B01_CardEquGroup 的資料，並秀在EquGroupList上
            sSQL = @"
                SELECT
                    DISTINCT EG.EquGrpNo, EG.EquGrpName
                FROM B01_CardEquGroup AS CEG
                INNER JOIN B01_Card AS CA ON CA.CardID = CEG.CardID
                INNER JOIN B01_EquGroup AS EG ON EG.EquGrpID = CEG.EquGrpID
                INNER JOIN B01_MgnEquGroup AS MEG ON MEG.EquGrpID = EG.EquGrpID
                INNER JOIN B00_SysUserMgns AS SUMS ON SUMS.MgaID = MEG.MgaID
                WHERE CA.CardNo = ? AND SUMS.UserID = ? ";

            liSqlPara.Clear();
            liSqlPara.Add("S:" + SelectValue.Trim());
            liSqlPara.Add("S:" + hideUserID.Value);

            try
            {
                isSuccess = oAcsDB.GetDataTable("dtEquGroupList", sSQL, liSqlPara, out dt);

                if (isSuccess)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            TableRow tr = new TableRow();
                            TableCell td = new TableCell();
                            td.Width = 300;
                            td.Text = "(" + dt.Rows[i]["EquGrpNo"] + ") " + dt.Rows[i]["EquGrpName"];
                            td.Style.Add("white-space", "nowrap");
                            td.Style.Add("Padding", "2px");
                            tr.Controls.Add(td);

                            EquGroupList.Controls.Add(tr);
                        }

                        isBtn02 = true;
                    }
                    else
                    {
                        TableRow tr = new TableRow();
                        TableCell td = new TableCell();
                        td.Text = GetLocalResourceObject("MsgEquGroupAuth").ToString();
                        td.Width = 280;
                        tr.Controls.Add(td);
                        EquGroupList.Controls.Add(tr);

                        isBtn02 = false;
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert02",
                        string.Format("alert('[CreateEuqList02]' {0});", "Load Data failure！"), true);
                }

            }
            catch (Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert02",
                    string.Format("alert('[CreateEuqList02]' {0});", ex.Message), true);
            }

            EquGroupList.Style.Add("word-break", "break-all");
            EquGroupList.Attributes.Add("border", "1");
            EquGroupList.Style.Add("border-color", "#999999");
            EquGroupList.Style.Add("border-style", "solid");
            EquGroupList.Style.Add("border-collapse", "collapse");
            this.lblEquGroupList.Text = 
                string.Format(GetLocalResourceObject("lblCardAuthGroupCopyList").ToString(), dt.Rows.Count);
            EquGroupListUpdatePanel.Update();

            #endregion

            #region 控制權限複製或權限附加按鈕的啟用狀況
            //if (isBtn01 == true || isBtn02 == true)
            //{
            //    Sa.Web.Fun.RunJavaScript(this, "ButtonDisabled('false');");
            //}
            //else
            //{
            //    Sa.Web.Fun.RunJavaScript(this, "ButtonDisabled('true');");
            //}
            #endregion

            if (MsgResultTextBox.Text != "")
            {
                MsgResultTextBox.Text = "";
                MsgResultUpdatePanel.Update();
            }

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI", "$.unblockUI();", true);
            /*
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "unblockUI",
                    "ShowLabel('權限複製清單(共','" + dt.Rows.Count + "','EquList');$.unblockUI();", true);
            */
        }
        #endregion

        #region GirdViewDataBind
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

                int columnCount = ProcessGridView.Rows[0].Cells.Count;
                ProcessGridView.Rows[0].Cells.Clear();
                ProcessGridView.Rows[0].Cells.Add(new TableCell());
                ProcessGridView.Rows[0].Cells[0].ColumnSpan = columnCount;
                ProcessGridView.Rows[0].Cells[0].Text = GetGlobalResourceObject("Resource", "NonData").ToString();
            }
        }
        #endregion

        #region Check_Input
        public Pub.MessageObject Check_Input(string strSurCardNo, string liTarCardList)
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            if (string.IsNullOrEmpty(strSurCardNo.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\\n";
                objRet.result = false;
                objRet.message += GetLocalResourceObject("MsgSelectSource").ToString();
            }

            if (string.IsNullOrEmpty(liTarCardList.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\\n";
                objRet.result = false;
                objRet.message += GetLocalResourceObject("MsgSelectTarget").ToString();
            }

            return objRet;
        }
        #endregion

        #region CardIDChange
        public string[] CardIDChange(string[] liTarCardList)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string[] TargetCardIDList = new string[liTarCardList.Length];
            string sSql = "";
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;
            for (int i = 0; i < liTarCardList.Length; i++)
            {
                liSqlPara.Clear();
                sSql = @" SELECT CardId FROM B01_Card WHERE CardNo = ? ";
                liSqlPara.Add("S:" + liTarCardList[i].ToString());
                oAcsDB.GetDataReader(sSql, liSqlPara, out dr);

                if (dr.Read())
                {
                    TargetCardIDList[i] = dr.DataReader["CardId"].ToString();
                }
            }
            return TargetCardIDList;
        }
        #endregion

        #region GetUIValue
        public void GetUIValue(out string strSurCardID, out string liTarCardList)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            Sa.DB.DBReader dr;
            string sql = "";
            List<string> liSqlPara = new List<string>();
            string TargetCardListStr = "";

            #region GetSourceCardID
            sql = @" SELECT 
                     Card.CardID, Card.CardNo
                     FROM dbo.B01_Card AS Card
                     WHERE CardNo = ?";
            liSqlPara.Add("S:" + SelectValue.Value);

            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
                strSurCardID = dr.DataReader["CardID"].ToString() + "," + dr.DataReader["CardNo"].ToString();
            else
                strSurCardID = "";
            #endregion

            #region GetTargetCardList
            if (this.TargetGridView.Rows.Count > 0)
            {
                for (int i = 0; i < this.TargetGridView.Rows.Count; i++)
                {
                    int cs = TargetGridView.Rows[i].Cells[0].Controls.Count;
                    CheckBox SelectCheckBox = (CheckBox)TargetGridView.Rows[i].Cells[0].FindControl("SelectControl"+i.ToString());
                    if (Request.Form.AllKeys.Where(s => s.Contains("SelectControl" + i)).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(TargetCardListStr)) TargetCardListStr += ",";
                        TargetCardListStr += TargetGridView.Rows[i].Cells[1].Text;
                    }
                }
                liTarCardList = TargetCardListStr;
            }
            else
            {
                liTarCardList = "";
            }
            #endregion
        }
        #endregion

        private string CardExtDataForElevator(string strEquClass, string strCardExtData)
        {
            string strResult = "";

            // 當 Equclass 是電梯時，CardExtData的處理。
            if (strEquClass == "Elevator")
            {
                if (strCardExtData == "")
                {
                    strCardExtData = "000000000000";
                }

                strCardExtData = Sa.Change.HexToBin(strCardExtData, 48, false);

                for (int k = 0; k < strCardExtData.Length; k++)
                {
                    strResult += (strCardExtData.Substring(k, 1).ToString() == "1" ? "1" : "0");
                }

                strResult = Sa.Change.BinToHex(strResult, 12);
            }
            else
            {
                strResult = strCardExtData;
            }

            return strResult;
        }

        private bool ExecCardAuthUpdate(string strTarCardNoAry, string strUser, bool isPass)
        {
            // isPass -- 決定這段程式執行失敗後是否要重複呼叫，目前沒用。
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            List<string> liPara = new List<string>();
            bool isOK = true;
            int iResult = 0;

            string strSQL = " EXEC CardAuth_Update ?, ?, ?, ?, ? ";
            liPara.Add("S:" + strTarCardNoAry);
            liPara.Add("S:" + strUser);
            liPara.Add("S:CardAuthCopy.aspx");
            liPara.Add("S:" + "ip");
            liPara.Add("S:" + "Desc");

            try
            {
                iResult = oAcsDB.SqlCommandExecute(strSQL, liPara);

                if (iResult > 0)
                {
                    isOK = true;
                }
                else
                {
                    isOK = false;
                }
            }
            catch
            {
                isOK = false;
            }

            return isOK;
        }

        private bool DeleteRepeatCardAuth()
        {
            bool isOK = true;
            DBReader dr = new DBReader();
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));

            string sSqlCmd = @"
                SELECT 
                    CardNo,EquID,COUNT(CardNo) AS EquCnt,MIN(ProcKey), 
                    ('DELETE FROM [B01_CardAuth] WHERE CardNo = '''+ CardNo +''' AND EquID = ' + CONVERT(VARCHAR, EquID) + ' AND ProcKey = '+ CONVERT(VARCHAR, MIN(ProcKey))) AS DELCMD
                FROM B01_CardAuth 
                WHERE OpMode <> 'Del'     
                GROUP BY EquID,CardNo
                HAVING COUNT(CardNo) > 1";

            dr = new DBReader();

            try
            {
                isOK = oAcsDB.GetDataReader(sSqlCmd, out dr);

                if (isOK)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            string strCardNo = dr.ToString("CardNo");
                            string strDELCMD = dr.ToString("DELCMD");

                            int intRet = oAcsDB.SqlCommandExecute(strDELCMD);

                            if (intRet < 0)
                            {
                                isOK = false;
                                break;
                            }
                            else
                            {
                                isOK = true;
                                DeleteRepeatCardAuth();
                            }
                        }
                    }
                }
            }
            catch
            {
                isOK = false;
            }
            

            return isOK;
        }

        #endregion

    }
}