using Sa.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

using SahoAcs.DBModel;
using DapperDataObjectLib;
using SahoAcs.DBClass;


namespace SahoAcs
{
    public partial class PersonMultiCard : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;     //宣告Ajax元件
        private int _pagesize = 20; //DataGridView每頁顯示的列數
        private int years = 80;     //年份選單的預設年數
        private int yongyear = 16;  //成年人歲數
        Hashtable hFloorType = new Hashtable();     //記載設備模組類型(門禁、電梯....))
        Hashtable hRuleType = new Hashtable();      //記錄設備的讀卡規則
        Hashtable hRuleList = new Hashtable();      //記載規則資訊
        Hashtable hFloorAllData = new Hashtable();  //記載各電梯設備的樓層選擇
        DataTable dtAuthMode = new DataTable();
        static string MsgCardNoRequired,MsgCardNoReused,MsgCardSize,MsgCardSpace,MsgCardTypeReused;
        static string MsgNameRequired,MsgPsnNoReused,MsgPsnNoSpace,MsgPsnNoRequired,MsgPsnAccountReused;
        static string MsgPwdRequired, MsgPwdSize, MsgPwdSpace;
        static string UserName;
        static GlobalMsg gms = new GlobalMsg();
        public DataTable DataInputText = new DataTable();
        #endregion


        #region 設定多語系變數區

        private void SetLangMsg()
        {            
            foreach(System.Reflection.PropertyInfo m in typeof(GlobalMsg).GetProperties()){
                m.SetValue(gms,this.GetLocalResourceObject(m.Name).ToString(),null);
            }
            MsgCardNoRequired = this.GetLocalResourceObject("MsgCardNoRequired").ToString();
            MsgCardNoReused = this.GetLocalResourceObject("MsgCardNoReused").ToString();
            MsgCardSize = this.GetLocalResourceObject("MsgCardSize").ToString();
            MsgCardSpace = this.GetLocalResourceObject("MsgCardSpace").ToString();
            MsgCardTypeReused = this.GetLocalResourceObject("MsgCardTypeReused").ToString();
            MsgNameRequired = this.GetLocalResourceObject("MsgNameRequired").ToString();
            MsgPsnNoReused = this.GetLocalResourceObject("MsgPsnNoReused").ToString();
            MsgPsnNoSpace = this.GetLocalResourceObject("MsgPsnNoSpace").ToString();
            MsgPsnNoRequired = this.GetLocalResourceObject("MsgPsnNoRequired").ToString();
            MsgPsnAccountReused = this.GetLocalResourceObject("MsgPsnAccountReused").ToString();
            MsgPwdRequired = this.GetLocalResourceObject("MsgPwdRequired").ToString();
            MsgPwdSize = this.GetLocalResourceObject("MsgPwdSize").ToString();
            MsgPwdSpace = this.GetLocalResourceObject("MsgPwdSpace").ToString();            
        }

        #endregion

        #region 網頁前置作業

        #region LoadProcess
        private void LoadProcess()
        {
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(QueryButton);

            string jsFileEnd = "<script src=\"../../../uc/QueryTool.js\" Type=\"text/javascript\"></script>\n";
            jsFileEnd += "<script src=\"CardGroupEdit.js?" + DateTime.Now.ToString("yyyyMMddHHmmss") + "\" Type=\"text/javascript\"></script>\n";
            jsFileEnd += "<script src=\"CardCode.js?" + DateTime.Now.ToString("yyyyMMddHHmmss") + "\" Type=\"text/javascript\"></script>\n";
            ClientScript.RegisterStartupScript(typeof(string), "CardGroupEdit", jsFileEnd, false);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";
            js += "\nBirthdayData(" + (DateTime.Now.Year - yongyear) + "," + years + ")";
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("Person", "PersonMultiCard.js?jsdate="+DateTime.Now.ToString("yyyyMMddHHmmss")); //加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
            //ClientScript.RegisterClientScriptInclude("CardGroupEdit", "CardGroupEdit.js");//加入同一頁面所需的JavaScript檔案
            Input_PsnSTime.SetWidth(180);
            Input_PsnETime.SetWidth(180);
        }
        #endregion

        #region RegisterObj
        private void RegisterObj()
        {
            #region 設定彈跳視窗
            Pub.SetModalPopup(ModalPopupExtender1, 1);
            #endregion

            #region 註冊主頁Button動作
            btnPsnAdd.Attributes["onClick"] = "AddPerson(); return false;";
            this.BtnAuthMode.Attributes["onClick"] = "SetAuthMode(); return false;";
            Input_PsnPicSource.Attributes["onkeyup"] = "InputPicText();";
            btSave.Attributes["onClick"] = "SaveData('" + hUserId.Value + "'); return false;";
            btDelete.Attributes["onClick"] = "DeleteData('" + hUserId.Value + "'); return false;";
            btCardInfo.Attributes["onClick"] = "CallCardEdit('" +
                this.GetLocalResourceObject("CallCardEdit_Title") + "'); return false;";
            btCardAdd.Attributes["onClick"] = "CallCardAdd('" +
                this.GetLocalResourceObject("CallCardAdd_Title") + "'); return false;";
            QueryButton.Attributes["onClick"] = "SelectState(); return false;";
            #endregion

            #region 註冊pop1頁Button動作
            ImgCloseButton1.Attributes["onClick"] = "CancelTrigger1.click();UnSelectItem(); return false;";
            popB_Add.Attributes["onClick"] = "SaveCardData('" + hUserId.Value + "'); return false;";
            popB_Edit.Attributes["onClick"] = "SaveCardData('" + hUserId.Value + "'); return false;";
            popB_Cancel.Attributes["onClick"] = "CancelTrigger1.click();UnSelectItem(); return false;";
            popB_Delete.Attributes["onClick"] = "DeleteCardData('" + hUserId.Value + "'); return false;";
            this.Input_PsnNo.Attributes["onkeyup"] = @"value=value.replace(/[^\w]/ig,'')";

            #endregion

            //設定DataGridView每頁顯示的列數
            this.MainGridView.PageSize = _pagesize;
            Input_PsnSTime.SetWidth(180);
            Input_PsnETime.SetWidth(180);
            popInput_CardSTime.SetWidth(180);
            popInput_CardETime.SetWidth(180);
        }
        #endregion

        #region Page_Load
        static string strEnabled = "";
        static string strDisabled = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            strEnabled = GetLocalResourceObject("SelectEnabled").ToString();
            strDisabled = GetLocalResourceObject("SelectDisabled").ToString();
            try
            {
                hUserId.Value = Session["UserID"].ToString();
                hPsnId.Value = Session["PsnID"].ToString();
                UserName = Session["UserName"].ToString();
                this.ShowCardAuthAdj.Value = ConfigurationManager.AppSettings["ShowCardAuthAdj"].ToString();                
                LoadProcess();
                RegisterObj();
                GetCardItemInfo();
                this.Input_CardNo.MaxLength = popInput_CardNo.MaxLength;
                this.SetLangMsg();
                if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
                {
                    hSelectState.Value = "true";
                    SelectAllEquData();
                    Session["FloorData"] = hFloorAllData;
                    ViewState["SortExpression"] = "PsnNo";
                    ViewState["SortDire"] = "ASC";
                    Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                }
                else
                {
                    string sFormTarget = Request.Form["__EVENTTARGET"];
                    string sFormArg = Request.Form["__EVENTARGUMENT"];

                    if (!string.IsNullOrEmpty(sFormArg))
                    {
                        if (sFormArg == "popPagePost") //進行因應新增或編輯後所需的換頁動作
                        {
                            //int find = Query("popPagePost", ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                            //Sa.Web.Fun.RunJavaScript(this, "__doPostBack('ctl00$ContentPlaceHolder1$MainGridView', 'Page$" + find + "');");
                            Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                            Sa.Web.Fun.RunJavaScript(this, "GridSelect();");
                        }
                        else if (sFormArg.Substring(0, 5) == "Page$") //換頁完成後進行GridViewRow的移動
                        {
                            Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                            Sa.Web.Fun.RunJavaScript(this, "GridSelect();");
                        }
                        else if (sFormArg == "NewQuery")
                        {
                            hSelectState.Value = "true";
                            this.MainGridView.PageIndex = 0;
                            this.SelectAllEquData();
                            Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        }
                    }
                    else
                    {
                        hSelectState.Value = "false";
                        Query(false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    }
                }                
            }
            catch
            {
                Response.Redirect("~/Web/MessagePage/LoginSorry.aspx");
            }
            this.td_showEquList.Style.Add("display", "none");
            this.td_showEquList.Style.Add("width", "0px");
            this.PanelPopup1.Width = 400;
        }
        #endregion

        #region Page_Init
        private void Page_Init(object sender, EventArgs e)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DBReader dr = null, drOrg = null;

            #region Sam 20171218 增加文字欄位設定
            oAcsDB.GetDataTable("InputText", "SELECT ParaName,ParaNo FROM B00_SysParameter WHERE ParaClass='InputText' AND ParaValue='Y' ", out this.DataInputText);

            #endregion

            #region REX20150611：動態人員類別的查詢資料
            oAcsDB.GetDataReader(" SELECT ItemOrder, ItemNo, ItemName FROM dbo.B00_ItemList WHERE ItemClass='PsnType' ORDER BY ItemOrder; ", out dr);
            if (dr.HasRows)
            {
                dropPsnType.Items.Add(new ListItem() { Text = GetGlobalResourceObject("Resource","ddlSelectDefault").ToString(), Value = "" });

                while (dr.Read())
                {
                    dropPsnType.Items.Add(new ListItem() { Text = dr.DataReader["ItemName"].ToString() + "." + dr.DataReader["ItemNo"].ToString(), Value = dr.DataReader["ItemNo"].ToString() });
                }
            }
            #endregion

            #region Process String
            //1、部門別處理
            sql = @"SELECT B01_OrgData.OrgClass, B00_ItemList.ItemName, B00_ItemList.ItemInfo2,B00_ItemList.ItemNo,
                B00_ItemList.ItemOrder FROM B01_OrgData 
                INNER JOIN B00_ItemList ON B01_OrgData.OrgClass = B00_ItemList.ItemNo
                WHERE B00_ItemList.ItemClass = 'OrgClass'
                GROUP BY B01_OrgData.OrgClass, B00_ItemList.ItemName, B00_ItemList.ItemInfo2, B00_ItemList.ItemOrder,B00_ItemList.ItemNo
                ORDER BY B00_ItemList.ItemOrder";
            oAcsDB.GetDataReader(sql, out dr);

            if (dr.HasRows)
            {
                //Sam Update 20160719                
                dropDepartment.Items.Add(new ListItem() { Text = GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString(), Value = "" });
                dropCompany.Items.Add(new ListItem() { Text = GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString(), Value = "" });
                while (dr.Read())
                {
                    //2、動態建立控制項
                    TableRow tr = new TableRow();
                    TableCell td = new TableCell();

                    #region 新增項目名稱
                    if (Request.Cookies["i18n"].Value.ToString() == "en-US")
                    {
                        td.Text = dr.DataReader["ItemNo"].ToString() + "：";
                    } else
                    {
                        td.Text = dr.DataReader["ItemName"].ToString() + "：";
                    }
                    if (new string[] { "Department", "Unit"}.Contains(dr.DataReader["ItemNo"].ToString()))
                    {
                        if (Request.Cookies["i18n"].Value.ToString() == "en-US")
                        {
                            this.lblDepartment.Text = dr.DataReader["ItemNo"].ToString();
                        }
                        else
                        {
                            this.lblDepartment.Text = dr.DataReader["ItemName"].ToString();
                        }
                    }
                    tr.Controls.Add(td);
                    tblOrgData.Controls.Add(tr);
                    #endregion

                    #region 新增項目對應的DropDownList
                    tr = new TableRow();
                    td = new TableCell();
                    DropDownList dropInit = new DropDownList();
                    dropInit.ID = "dropOrg_" + dr.DataReader["ItemInfo2"].ToString();
                    hItemInfo2.Value += dr.DataReader["ItemInfo2"].ToString() + "|";
                    ListItem Item = new ListItem();
                    Item.Text = GetGlobalResourceObject("Resource", "ddlSelectDefault").ToString();
                    Item.Value = "0";
                    dropInit.Items.Add(Item);                    

                    //3、部門別資料處理
                    sql = @"SELECT OrgID, OrgNo, OrgName FROM B01_OrgData
                        WHERE OrgClass = '" + dr.DataReader["OrgClass"].ToString() + "' ORDER BY OrgNo";
                    oAcsDB.GetDataReader(sql, out drOrg);

                    #region 動態寫入部門別資料
                    while (drOrg.Read())
                    {
                        Item = new ListItem();
                        Item.Text = drOrg.DataReader["OrgName"].ToString() + "." +
                            drOrg.DataReader["OrgNo"].ToString();
                        Item.Value = drOrg.DataReader["OrgID"].ToString();
                        dropInit.Items.Add(Item);
                    }
                    if (ConfigurationManager.AppSettings["DefaultOrg"] != null && dr.DataReader["ItemInfo2"].ToString() == "C")
                    {
                        this.DefaultOrg.Value = ConfigurationManager.AppSettings["DefaultOrg"];
                    }
                    #endregion                    

                    td.Controls.Add(dropInit);
                    tr.Controls.Add(td);
                    tblOrgData.Controls.Add(tr);
                    #endregion

                    #region REX20150611
                    string sOrgClass = dr.DataReader["OrgClass"].ToString();

                    sql = " SELECT OrgID, OrgNo, OrgName FROM B01_OrgData";
                    sql += " WHERE OrgClass = '" + sOrgClass + "' ";
                    sql += " ORDER BY OrgNo ";

                    oAcsDB.GetDataReader(sql, out drOrg);

                    #region 動態寫入公司、部門的查詢資料
                    while (drOrg.Read())
                    {
                        ListItem oItem = new ListItem();

                        oItem.Text = drOrg.DataReader["OrgName"].ToString() + "." + drOrg.DataReader["OrgNo"].ToString();
                        oItem.Value = drOrg.DataReader["OrgNo"].ToString();

                        if (sOrgClass == "Company")
                        {
                            dropCompany.Items.Add(oItem);
                        }
                        else if (sOrgClass == "Department" || sOrgClass == "Unit")
                        {
                            dropDepartment.Items.Add(oItem);
                        }

                        oItem = null;
                    }

                    #endregion
                    #endregion
                }

                hItemInfo2.Value = hItemInfo2.Value.Remove(hItemInfo2.Value.Length - 1);
            }
            #endregion
        }
        #endregion

        #region VerifyRenderingInServerForm
        public override void VerifyRenderingInServerForm(Control control)
        {
            //如有使用UpdatePanel配合GridVew才需要這個方法
            // 修正 'XX'型別 必須置於有 runat=server 的表單標記之中 Override此Methods
        }
        #endregion

        #endregion

        #region 其他方法

        #region 記載查詢條件的紀錄，防止頁數按鈕切換時查詢錯誤
        private void CatchSession(List<String> Data)
        {
            String datalist = "";

            for (int i = 0; i < Data.Count; i++)
            {
                datalist += Data[i] + "|";
            }

            Session["OldSearchList"] = datalist;
        }
        #endregion

        #region LimitText

        public string LimitText(string str, int len, bool ellipsis)
        {
            Encoding big5 = Encoding.GetEncoding("big5");
            byte[] b = big5.GetBytes(str);

            if (b.Length <= len)
            {
                return str;
            }
            else
            {
                if (ellipsis) len -= 3;
                string res = big5.GetString(b, 0, len);

                if (!big5.GetString(b).StartsWith(res))
                {
                    res = big5.GetString(b, 0, len - 1);
                }

                return res + (ellipsis ? "..." : "");
            }
        }
        #endregion

        #region 取得相關資訊設定(卡號長度)
        private void GetCardItemInfo()
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Session["CardLen"] = oAcsDB.GetIntScalar(@"SELECT TOP 1 CASE ISNUMERIC(ItemInfo2) WHEN 0 THEN '8' ELSE ItemInfo2 END  AS ItemInfo2 
                                                                                FROM B00_ItemList WHERE ItemClass='CardType' ORDER BY ItemOrder");//取得主卡長度
            Session["CardVer"] = oAcsDB.GetStrScalar("SELECT ParaValue FROM B00_SysParameter WHERE ParaClass = 'CardID' AND ParaNo = 'CardVer' ");//取得卡號版次設定
            popInput_CardNo.MaxLength = int.Parse(Session["CardLen"].ToString());
            this.Input_PsnIDNum.Attributes.Add("MUST_KEYIN_YN", oAcsDB.GetStrScalar("SELECT ISNULL(MAX(ParaValue),'N') FROM B00_SysParameter WHERE ParaNo='IDNumMustKeyIn'"));
            this.Input_PsnIDNum.MaxLength = oAcsDB.GetIntScalar("SELECT ISNULL(MAX(ParaValue),12) FROM B00_SysParameter WHERE ParaNo='IDNumLen' ");
            popInput_CardVer.Enabled = Session["CardVer"].ToString() == "Y" ? true : false;
            Input_CardVer.Enabled = popInput_CardVer.Enabled;

            //驗證卡號認證模式的資訊是否顯示            
            oAcsDB.GetDataTable("AuthMode","SELECT * FROM B00_ItemList WHERE ItemClass='AuthModel' ",out dtAuthMode);
            if (dtAuthMode.Rows.Count == 0)
            {
                this.DivVerifiContent.Style.Add("display", "none");
                this.DivVerifiTitle.Style.Add("display", "none");
            }
            else
            {
                foreach (DataRow r in dtAuthMode.Rows)
                {
                    this.ddlVerifiMode.Items.Add(new ListItem()
                    {
                        Value=Convert.ToString(r["ItemNo"]),
                        Text=Convert.ToString(r["ItemName"])
                    });
                }
                this.ddlVerifiMode.SelectedValue = "0";
            }
        }
        #endregion

        #endregion

        #region 查詢
        private void Query(bool select_state, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            DBReader dr = null;
            List<String> CheckData = new List<String>();
            List<string> liSqlPara = new List<string>();
            String NowData = "";            
            //REX20150611
            String NowCompanyNo = SelectCompanyNo.Value;
            String NowDepartmentNo = SelectDepartmentNo.Value;
            String NowPsnTypeNo = SelectPsnTypeNo.Value;            
            if (select_state)
            {
                CheckData.Add(this.InputWord.Text.Trim());
                CatchSession(CheckData);
                NowData = this.InputWord.Text.Trim();
            }
            else
            {
                if (Session["OldSearchList"].ToString() != "" && Session["OldSearchList"].ToString() != null)
                {
                    String[] mgalist = Session["OldSearchList"].ToString().Split('|');
                    NowData = mgalist[0];
                }
            }

            #region Process String
            sql = @"SELECT DISTINCT(OrgStrucID), OrgStrucNo FROM 
                (SELECT B00_SysUserMgns.UserID, B00_SysUserMgns.MgaID, B01_MgnOrgStrucs.OrgStrucID, B01_OrgStruc.OrgStrucNo,
                B01_OrgStruc.OrgIDList FROM B00_SysUserMgns 
                INNER JOIN B01_MgnOrgStrucs ON B00_SysUserMgns.MgaID = B01_MgnOrgStrucs.MgaID
                INNER JOIN B01_OrgStruc ON B01_MgnOrgStrucs.OrgStrucID = B01_OrgStruc.OrgStrucID) AS Mgns
                WHERE Mgns.UserID = '" + hUserId.Value + "'";

            oAcsDB.GetDataReader(sql, out dr);

            if (dr.HasRows)
            {
                wheresql = " (";
                while (dr.Read())
                    wheresql += "B01_Person.OrgStrucID = " + dr.DataReader["OrgStrucID"].ToString() + " OR ";
                wheresql = wheresql.Substring(0, wheresql.Length - 4);
                wheresql += ") ";
            }

            sql = @"SELECT DISTINCT(B01_Person.PsnID), B01_Person.PsnNo, B01_Person.PsnName, B01_Person.PsnEName,
                B01_Person.PsnType, B01_Person.IDNum, B01_Person.Birthday, B01_Person.OrgStrucID, B01_Person.PsnAccount,
                B01_Person.PsnPW, B01_Person.PsnAuthAllow, B01_Person.PsnSTime, B01_Person.PsnETime, B01_Person.PsnPicSource,
                B01_Person.Remark, B01_Person.CreateUserID, B01_Person.CreateTime, B01_Person.UpdateUserID,
                B01_Person.UpdateTime, B01_Person.Rev01, B01_Person.Rev02, OrgStrucAllData_1.OrgStrucNo,
                OrgStrucAllData_1.OrgNameList, OrgStrucAllData_1.OrgNoList FROM B01_Person 
                INNER JOIN OrgStrucAllData('') AS OrgStrucAllData_1 ON B01_Person.OrgStrucID = OrgStrucAllData_1.OrgStrucID
                LEFT OUTER JOIN B01_Card ON B01_Person.PsnID = B01_Card.PsnID ";

            if (!string.IsNullOrEmpty(NowData))
            {
                if (wheresql != "")
                    wheresql += " AND ";
                wheresql += " ( PsnNo LIKE ? OR PsnName LIKE ? OR (CardNo=? AND ISNULL(B01_Card.PsnID,0)!=0)) ";
                liSqlPara.Add("S:" + '%' + NowData + '%');
                liSqlPara.Add("S:" + '%' + NowData + '%');
                liSqlPara.Add("S:" + NowData);
            }

            //REX20150611
            if ((NowCompanyNo != "") && (NowDepartmentNo != ""))
            {
                if (wheresql != "") { wheresql += " AND"; }
                wheresql += string.Format(@" (OrgNoList LIKE '\{0}\%' AND OrgNoList LIKE '%\{1}\%')", NowCompanyNo, NowDepartmentNo);
            }
            else if ((NowCompanyNo != "") || (NowDepartmentNo != ""))
            {
                if (wheresql != "") { wheresql += " AND"; }
                wheresql += string.Format(@" (OrgNoList LIKE '%\{0}\%')", (NowCompanyNo != "" ? NowCompanyNo : NowDepartmentNo));
            }

            //REX20150611
            if (NowPsnTypeNo != "")
            {
                if (wheresql != "") { wheresql += " AND"; }
                wheresql += " (PsnType = ?)";
                liSqlPara.Add("S:" + NowPsnTypeNo);
            }

            if (wheresql != "" && !hUserId.Value.Equals("User") && hPsnId.Value.Equals(""))
            {
                sql += " WHERE ";
            }
            else if (wheresql == "" && !hUserId.Value.Equals("User") && hPsnId.Value.Equals(""))
            {
                sql += " WHERE ";
                wheresql = " (B01_Person.OrgStrucID = 99999999 OR ";
                wheresql = wheresql.Substring(0, wheresql.Length - 4);
                wheresql += ") ";
            }
            else if (hUserId.Value.Equals("User") && !hPsnId.Value.Equals(""))
            {
                if (wheresql != "")
                {
                    sql += " WHERE B01_Person.PsnID = ? AND";
                }
                else
                {
                    sql += " WHERE B01_Person.PsnID = ?";
                }

                liSqlPara.Add("S:" + hPsnId.Value);
            }

            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            GirdViewDataBind(this.MainGridView, dt);
            UpdatePanel1.Update();            
            hSelectState.Value = "false";
        }

        private int Query(string mode, string SortExpression, string SortDire)
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DataTable dt;
            DBReader drd = null;
            List<String> CheckData = new List<String>();
            List<string> liSqlPara = new List<string>();
            String NowData = "";            

            CheckData.Add(this.InputWord.Text.Trim());
            CatchSession(CheckData);
            NowData = this.InputWord.Text.Trim();

            #region Process String
            sql = @"SELECT DISTINCT(OrgStrucID), OrgStrucNo FROM
                (SELECT B00_SysUserMgns.UserID, B01_MgnOrgStrucs.MgaID, B01_OrgStruc.OrgStrucID, B01_OrgStruc.OrgStrucNo,
                B01_OrgStruc.OrgIDList FROM B01_MgnOrgStrucs
                INNER JOIN B01_OrgStruc ON B01_MgnOrgStrucs.OrgStrucID = B01_OrgStruc.OrgStrucID
                INNER JOIN B00_SysUserMgns ON B01_MgnOrgStrucs.MgaID = B00_SysUserMgns.MgaID) AS Mgns
                WHERE Mgns.UserID = '" + hUserId.Value + "'";

            oAcsDB.GetDataReader(sql, out drd);

            if (drd.HasRows)
            {
                wheresql = " (";
                while (drd.Read())
                    wheresql += "B01_Person.OrgStrucID = " + drd.DataReader["OrgStrucID"].ToString() + " OR ";
                wheresql = wheresql.Substring(0, wheresql.Length - 4);
                wheresql += ") ";
            }

            sql = @"SELECT DISTINCT(B01_Person.PsnID), B01_Person.PsnNo, B01_Person.PsnName, B01_Person.PsnEName,
                B01_Person.PsnType, B01_Person.IDNum, B01_Person.Birthday, B01_Person.OrgStrucID, B01_Person.PsnAccount,
                B01_Person.PsnPW, B01_Person.PsnAuthAllow, B01_Person.PsnSTime, B01_Person.PsnETime, B01_Person.PsnPicSource,
                B01_Person.Remark, B01_Person.CreateUserID, B01_Person.CreateTime, B01_Person.UpdateUserID,
                B01_Person.UpdateTime, B01_Person.Rev01, B01_Person.Rev02, OrgStrucAllData_1.OrgStrucNo,
                OrgStrucAllData_1.OrgNameList, OrgStrucAllData_1.OrgNoList, B01_Person.Text1,B01_Person.Text2 FROM B01_Person
                INNER JOIN OrgStrucAllData('') AS OrgStrucAllData_1 ON B01_Person.OrgStrucID = OrgStrucAllData_1.OrgStrucID
                LEFT OUTER JOIN B01_Card ON B01_Person.PsnID = B01_Card.PsnID ";

            if (!string.IsNullOrEmpty(NowData))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " ( PsnNo LIKE ? OR PsnName LIKE ? OR Text1 LIKE ? OR Text2 LIKE ? ) ";
                liSqlPara.Add("S:" + '%' + NowData + '%');
                liSqlPara.Add("S:" + '%' + NowData + '%');
                liSqlPara.Add("S:" + '%' + NowData + '%');
                liSqlPara.Add("S:" + '%' + NowData + '%');
            }
           
            if (wheresql != "" && !hUserId.Value.Equals("User") && hPsnId.Value.Equals(""))
            {
                sql += " WHERE ";
            }
            else if (hUserId.Value.Equals("User") && !hPsnId.Value.Equals(""))
            {
                if (wheresql != "")
                {
                    sql += " WHERE B01_Person.PsnID = ? AND";
                }
                else
                {
                    sql += " WHERE B01_Person.PsnID = ?";
                }

                liSqlPara.Add("S:" + hPsnId.Value);
            }

            sql += wheresql + " ORDER BY " + SortExpression + " " + SortDire;
            #endregion

            oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            hDataRowCount.Value = dt.Rows.Count.ToString();
            GirdViewDataBind(this.MainGridView, dt);            
            UpdatePanel1.Update();            
            int find = 0;

            #region 取得RoleTable後的動作
            DataRow dr = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dr = dt.Rows[i];

                if (SelectValue.Value == dr["PsnID"].ToString())
                {
                    find = i;
                    break;
                }
            }
            #endregion

            return (find / _pagesize) + 1;
        }

        protected static Pub.MessageObject Check_Input_DB(string[] NoArray, string mode)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;
            //預設先取得主卡的卡號長度
            int cardlen = int.Parse(oAcsDB.GetStrScalar(@"SELECT 
                                                TOP 1 CASE ISNUMERIC(ItemInfo2) WHEN 0 THEN '8' ELSE ItemInfo2 END  AS ItemInfo2 FROM B00_ItemList WHERE ItemClass='CardType' ORDER BY ItemOrder "));
            string sql_check_user = @"SELECT COUNT(*) FROM 
	                                                            (SELECT PsnAccount FROM B01_Person WHERE PsnAccount=? AND PsnNo<>?
	                                                            UNION
	                                                            SELECT UserID FROM B00_SysUser WHERE UserID=?) AS T1";
            #region Input

            #region Person Insert
            if (mode == "Insert")
            {
                if (string.IsNullOrEmpty(NoArray[0].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += MsgPsnNoRequired;
                }

                if (NoArray[0].Contains(" "))
                {
                    sRet.result = false;
                    sRet.message += MsgPsnNoSpace;
                }

                if (string.IsNullOrEmpty(NoArray[1].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += MsgNameRequired;
                }
                //多一段驗證帳號重複或是使用User
                if (!string.IsNullOrEmpty(NoArray[3].Trim()))
                {                    
                    liSqlPara.Clear();
                    liSqlPara.Add("S:"+NoArray[3]);
                    liSqlPara.Add("S:" + NoArray[0]);
                    liSqlPara.Add("S:" + NoArray[3]);
                    if(NoArray[3].ToUpper().Equals("USER")
                        ||int.Parse(oAcsDB.GetStrScalar(sql_check_user,liSqlPara))>0)
                    {
                        if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                        sRet.result = false;
                        sRet.message += MsgPsnAccountReused;
                    }
                    liSqlPara.Clear();
                }
                if (string.IsNullOrEmpty(NoArray[2].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += MsgCardNoRequired;
                }
                if (NoArray[2].Replace(" ", "").Length != cardlen)
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += string.Format(MsgCardSize, cardlen);
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(NoArray[2], @"\A\b[0-9A-F]+\b\Z"))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += string.Format("CardNo {0} input formmat is error", NoArray[2]);
                }
            }
            #endregion

            #region Person Update
            if (mode == "Update")
            {
                if (string.IsNullOrEmpty(NoArray[0].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += MsgPsnNoRequired;
                }

                if (NoArray[0].Contains(" "))
                {
                    sRet.result = false;
                    sRet.message += MsgPsnNoSpace;
                }
                //多一段驗證帳號重複或是使用User
                if (!string.IsNullOrEmpty(NoArray[3].Trim()))
                {
                    liSqlPara.Clear();
                    liSqlPara.Add("S:" + NoArray[3]);
                    liSqlPara.Add("S:" + NoArray[0]);
                    liSqlPara.Add("S:" + NoArray[3]);
                    if (NoArray[3].ToUpper().Equals("USER")
                        || int.Parse(oAcsDB.GetStrScalar(sql_check_user, liSqlPara)) > 0)
                    {
                        if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                        sRet.result = false;
                        sRet.message += MsgPsnAccountReused;
                    }
                    liSqlPara.Clear();
                }
                if (string.IsNullOrEmpty(NoArray[1].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += MsgNameRequired;
                }
            }
            #endregion

            #region Person Delete
            if (mode == "Delete")
            {
                if (string.IsNullOrEmpty(NoArray[0].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += "請選取欲刪除之人員資料。";
                }
                else
                {
                    sRet.message += "是否確定刪除 [工號：" + NoArray[1].Trim() + "] 人員資料？";
                }
            }
            #endregion

            #region Card Insert
            if (mode == "CardInsert")
            {
                //取得新的卡別卡號長度
                List<string> para = new List<string>();
                para.Add("S:" + NoArray.Last());
                cardlen = int.Parse(oAcsDB.GetStrScalar("SELECT CASE ISNUMERIC(ItemInfo2) WHEN 0 THEN '8' ELSE ItemInfo2 END AS ItemInfo2 FROM B00_ItemList WHERE ItemClass='CardType' AND ItemNo=? ", para));
                if (string.IsNullOrEmpty(NoArray[0].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += MsgCardNoRequired;
                }

                if (NoArray[0].Contains(" "))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += MsgCardSpace;
                }

                if (NoArray[0].Replace(" ", "").Length != cardlen)
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += string.Format(MsgCardSize,cardlen);
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(NoArray[0], @"\A\b[0-9A-F]+\b\Z"))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += string.Format("CardNo {0} input formmat is error", NoArray[0]);
                }

                if (string.IsNullOrEmpty(NoArray[1].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += MsgPwdRequired;
                }

                if (NoArray[1].Contains(" "))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += MsgPwdSpace;
                }

                if (NoArray[1].Replace(" ", "").Length < 4)
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += string.Format(MsgPwdSize,4);
                }
            }
            #endregion

            #region Card Update
            if (mode == "CardUpdate")
            {
                if (string.IsNullOrEmpty(NoArray[0].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += MsgCardNoRequired;
                }

                if (NoArray[0].Contains(" "))
                {
                    sRet.result = false;
                    sRet.message += MsgCardSpace;
                }

                if (string.IsNullOrEmpty(NoArray[1].Trim()))
                {
                    if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                    sRet.result = false;
                    sRet.message += MsgPwdRequired;
                }

                if (NoArray[1].Contains(" "))
                {
                    sRet.result = false;
                    sRet.message += MsgPwdSpace;
                }
            }
            #endregion

            #endregion

            #region DB
            switch (mode)
            {
                case "Insert":
                    sql = @" SELECT PsnID FROM B01_Person WHERE PsnNo = ? ";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                    if (dr.Read())
                    {
                        if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                        sRet.result = false;
                        sRet.message += MsgPsnNoReused;
                    }
                    else
                    {
                        dr = null;
                        liSqlPara.Clear();
                        sql = @" SELECT CardID FROM B01_Card WHERE CardNo = ? ";
                        liSqlPara.Add("S:" + NoArray[2].Trim());
                        oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                        if (dr.Read())
                        {
                            if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                            sRet.result = false;
                            sRet.message += MsgCardNoReused;
                        }
                    }

                    break;
                case "Update":
                    sql = @" SELECT PsnID FROM B01_Person WHERE PsnNo = ? AND PsnNo <> ?  ";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    liSqlPara.Add("S:" + NoArray[2].Trim());
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                    if (dr.Read())
                    {
                        if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                        sRet.result = false;
                        sRet.message += MsgPsnNoReused;
                    }

                    break;
                case "CardInsert":
                    sql = @" SELECT CardID FROM B01_Card WHERE CardNo = ? OR (PsnID=? AND CardType=?)";
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    liSqlPara.Add("I:" + NoArray[2].Trim());
                    liSqlPara.Add("S:" + NoArray[3].Trim());                    
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                    if (dr.Read())
                    {
                        if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                        sRet.result = false;
                        sRet.message += MsgCardNoReused;
                    }

                    break;
                case "CardUpdate":
                    sql = @" SELECT CardID FROM B01_Card WHERE PsnID = ? AND CardType = ? AND CardNo <> ? ";
                    liSqlPara.Add("I:" + NoArray[2].Trim());
                    liSqlPara.Add("S:" + NoArray[3].Trim());
                    liSqlPara.Add("S:" + NoArray[0].Trim());
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                    if (dr.Read())
                    {
                        if (!string.IsNullOrEmpty(sRet.message)) sRet.message += "\n";
                        sRet.result = false;
                        sRet.message += MsgCardTypeReused;
                    }

                    break;
            }
            #endregion

            return sRet;
        }

        protected static string CheckOrgList(string strOrgIDList, string strOrgNoList, string sUserID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string sql = "", strOrgStrucID = "0";
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region 檢查是否存在該組織架構
            sql = @"SELECT OrgStrucID FROM B01_OrgStruc WHERE OrgIDList = ?";
            liSqlPara.Add("S:" + strOrgIDList);
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (dr.Read())
            {
                //存在該組織架構則回傳該組織架構ID
                strOrgStrucID = dr.DataReader["OrgStrucID"].ToString() + "|" + "true";
            }
            else
            {
                liSqlPara.Clear();
                //不存在該組織架構則新增該組織架構並回傳新ID
                #region Process String
                sql = @"INSERT INTO B01_OrgStruc(OrgStrucNo ,OrgIDList ,CreateUserID ,CreateTime)
                    VALUES (?, ?, ?, GETDATE());SELECT SCOPE_IDENTITY();";
                liSqlPara.Add("S:" + strOrgNoList);
                liSqlPara.Add("S:" + strOrgIDList);
                liSqlPara.Add("S:" + sUserID);
                #endregion
                try
                {
                    oAcsDB.BeginTransaction();
                    string sOrgSturcID = oAcsDB.GetStrScalar(sql, liSqlPara);
                    if (!sOrgSturcID.Equals(""))
                    {
                        if (oAcsDB.Commit())
                        {
                            #region 將新增組織納入全區的管理區下
                            liSqlPara.Clear();
                            sql = @"INSERT INTO B01_MgnOrgStrucs(MgaID, OrgStrucID, CreateUserID) VALUES(?, ?, ?)";
                            liSqlPara.Add("S:" + "1");
                            liSqlPara.Add("S:" + sOrgSturcID);
                            liSqlPara.Add("S:" + sUserID);
                            //oAcsDB.SqlCommandExecute(sql, liSqlPara);
                            #endregion
                            strOrgStrucID = sOrgSturcID.ToString() + "|" + "false";
                        }
                    }
                    else
                    {
                        oAcsDB.Rollback();
                    }
                }
                catch(Exception ex)
                {
                    oAcsDB.Rollback();
                }
            }
            #endregion

            return strOrgStrucID;
        }
        #endregion

        #region GridView處理
        protected void GridView_Data_DataBound(object sender, EventArgs e)
        {
            td_showGridView.Attributes["colspan"] = MainGridView.Columns.Count.ToString();
        }

        protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            MainGridView.PageIndex = e.NewPageIndex;
            this.SelectAllEquData();
            if (MainGridView.Rows[0].Cells.Count == MainGridView.HeaderRow.Cells.Count)
                MainGridView.DataBind();
        }

        protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            String SortField = e.SortExpression, SortDire = "DESC";

            if (ViewState["SortExpression"] != null)
            {
                if (ViewState["SortExpression"].ToString().Equals(SortField))
                {
                    if (!ViewState["SortDire"].Equals("ASC"))
                    {
                        SortDire = "ASC";
                    }
                    else
                    {
                        SortDire = "DESC";
                    }
                }
            }

            ViewState["SortExpression"] = SortField;
            ViewState["SortDire"] = SortDire;
            hSelectState.Value = "true";
            Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
        }

        protected void GridView_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    e.Row.Cells[1].Width = 150;
                    //e.Row.Cells[2].Width = 250;
                    #endregion

                    #region 設定表頭欄位Visible狀態
                    //不想顯示的欄位，可在此處關掉
                    e.Row.Cells[0].Visible = false;
                    #endregion

                    #region 排序條件Header加工
                    foreach (DataControlField dataControlField in MainGridView.Columns)
                    {
                        if (dataControlField.SortExpression.Equals(this.SortExpression))
                        {
                            Label label = new Label();
                            label.Text = this.SortDire.Equals("ASC") ? "▲" : "▼";
                            e.Row.Cells[MainGridView.Columns.IndexOf(dataControlField)].Controls.Add(label);
                        }
                    }
                    #endregion

                    #region 寫入Literal_Header
                    StringWriter Header_sw = new StringWriter();
                    HtmlTextWriter Header_writer = new HtmlTextWriter(Header_sw);
                    e.Row.CssClass = "GVStyle";
                    e.Row.RenderControl(Header_writer);
                    e.Row.Visible = false;
                    li_header.Text = Header_sw.ToString();
                    #endregion

                    break;
                #endregion

                #region DataRow
                case DataControlRowType.DataRow:
                    DataRowView oRow = (DataRowView)e.Row.DataItem;

                    #region 指定Row的ID
                    GridViewRow GrRow = e.Row;
                    GrRow.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    GrRow.ID = "GV_Row" + oRow.Row["PsnID"].ToString();
                    #endregion

                    #region 設定欄位寛度
                    e.Row.Cells[1].Width = 154;
                    //e.Row.Cells[2].Width = 254;
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 員工ID
                    e.Row.Cells[0].Visible = false;
                    #endregion

                    #region 編號
                    #endregion

                    #region 名稱
                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                        {
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                        }
                    }

                    //e.Row.Cells[0].Text = LimitText(e.Row.Cells[0].Text, 10);
                    e.Row.Cells[1].Text = LimitText(e.Row.Cells[1].Text.Trim(), 10, true);
                    e.Row.Cells[2].Text = LimitText(e.Row.Cells[2].Text.Trim(), 20, true);
                    #endregion

                    e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectValue,'" + oRow.Row["PsnID"].ToString() + "', '', '');CallEdit('" + oRow.Row["PsnNo"].ToString() + "');");
                    break;
                #endregion

                #region Pager
                case DataControlRowType.Pager:

                    #region 取得控制項
                    GridView gv = sender as GridView;
                    PlaceHolder phdPageNumber = e.Row.FindControl("phdPageNumber") as PlaceHolder;
                    PlaceHolder phdPageInfo = e.Row.FindControl("phdPageInfo") as PlaceHolder;
                    LinkButton lbtnFirst = e.Row.FindControl("lbtnFirst") as LinkButton;
                    LinkButton lbtnLast = e.Row.FindControl("lbtnLast") as LinkButton;
                    LinkButton lbtnPrev = e.Row.FindControl("lbtnPrev") as LinkButton;
                    LinkButton lbtnNext = e.Row.FindControl("lbtnNext") as LinkButton;
                    LinkButton lbtnPage;
                    #endregion

                    #region 決定顯示頁數及上下頁處理
                    int showRange = 5; //顯示快捷頁數
                    int pageCount = gv.PageCount;
                    int pageIndex = gv.PageIndex;
                    int startIndex = (pageIndex + 1 < showRange) ?
                        0 : (pageIndex + 1 + showRange / 2 >= pageCount) ? pageCount - showRange : pageIndex - showRange / 2;
                    int endIndex = (startIndex >= pageCount - showRange) ? pageCount : startIndex + showRange;

                    #region 頁數列
                    phdPageNumber.Controls.Add(new LiteralControl("  "));
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        lbtnPage = new LinkButton();
                        lbtnPage.Text = (i + 1).ToString();
                        lbtnPage.CommandName = "Page";
                        lbtnPage.CommandArgument = (i + 1).ToString();
                        lbtnPage.Font.Overline = false;
                        if (i == pageIndex)
                        {
                            lbtnPage.Font.Bold = true;
                            lbtnPage.ForeColor = System.Drawing.Color.White;
                            lbtnPage.OnClientClick = "return false;";
                        }
                        else
                            lbtnPage.Font.Bold = false;
                        phdPageNumber.Controls.Add(lbtnPage);
                        phdPageNumber.Controls.Add(new LiteralControl(" "));
                    }
                    #endregion

                    #region 上下頁
                    lbtnPrev.Click += delegate(object obj, EventArgs args)
                    {
                        if (gv.PageIndex > 0)
                        {
                            gv.PageIndex = gv.PageIndex - 1;
                            if (hSelectState.Value == "true")
                                Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                            else
                                Query(false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        }
                    };

                    lbtnNext.Click += delegate(object obj, EventArgs args)
                    {
                        if (gv.PageIndex < gv.PageCount)
                        {
                            gv.PageIndex = gv.PageIndex + 1;
                            if (hSelectState.Value == "true")
                                Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                            else
                                Query(false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        }
                    };
                    #endregion

                    #region 首末頁
                    lbtnFirst.Click += delegate(object obj, EventArgs args)
                    {
                        gv.PageIndex = 0;
                        if (hSelectState.Value == "true")
                            Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        else
                            Query(false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    };

                    lbtnLast.Click += delegate(object obj, EventArgs args)
                    {
                        gv.PageIndex = MainGridView.PageCount;
                        if (hSelectState.Value == "true")
                            Query(true, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                        else
                            Query(false, ViewState["SortExpression"].ToString(), ViewState["SortDire"].ToString());
                    };
                    #endregion

                    #endregion

                    #region 顯示總頁數及目前頁數
                    phdPageInfo.Controls.Add(
                        new LiteralControl("<br/><br/>"+string.Format(" {0} / {1} ", pageIndex + 1, pageCount)));
                    phdPageInfo.Controls.Add(
                        new LiteralControl("  "));
                    #endregion

                    #region 顯示總筆數
                    phdPageInfo.Controls.Add(
                        new LiteralControl(string.Format(this.GetGlobalResourceObject("Resource", "lblGvCounts").ToString(), hDataRowCount.Value)));
                    phdPageInfo.Controls.Add(
                        new LiteralControl("  "));
                    #endregion

                    #region 寫入Literal_Pager
                    StringWriter Pager_sw = new StringWriter();
                    HtmlTextWriter Pager_writer = new HtmlTextWriter(Pager_sw);
                    e.Row.CssClass = "GVStylePgr";

                    e.Row.RenderControl(Pager_writer);
                    e.Row.Visible = false;
                    li_Pager.Text = Pager_sw.ToString();
                    #endregion

                    break;
                #endregion
            }
        }

        protected void GridView1_Data_DataBound(object sender, EventArgs e)
        {
            //td_showGridView1.Attributes["colspan"] = GridView1.Columns.Count.ToString();
        }

        protected void GridView1_Data_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                #region Header
                case DataControlRowType.Header:

                    #region 設定欄位寛度
                    e.Row.Cells[0].Width = 20;
                    e.Row.Cells[1].Width = 100;
                    //e.Row.Cells[2].Width = 80;
                    e.Row.Cells[2].Width = 210;
                    e.Row.Cells[3].Width = 100;
                    #endregion

                    #region 設定表頭欄位Visible狀態
                    //不想顯示的欄位，可在此處關掉
                    e.Row.Cells[4].Visible = false;
                    e.Row.Cells[0].Visible = false;
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
                    e.Row.Cells[1].Width = 104;
                    //e.Row.Cells[2].Width = 84;
                    e.Row.Cells[2].Width = 214;
                    e.Row.Cells[3].Width = 104;
                    if ((oRow["EquModel"].ToString() == "ADM100FP" || oRow["EquModel"].ToString() == "SST9500FP") && this.dtAuthMode.Rows.Count>0)
                    {
                        foreach (DataRow r in this.dtAuthMode.Rows)
                        {
                            ((DropDownList)e.Row.Cells[5].FindControl("ddlCardVirife")).Items.Add(
                                new ListItem() { Text = Convert.ToString(r["ItemName"]), Value = Convert.ToString(r["ItemNo"]) });
                        }
                        ((DropDownList)e.Row.Cells[5].FindControl("ddlCardVirife")).Enabled = false;
                    }
                    else
                    {
                        ((DropDownList)e.Row.Cells[5].FindControl("ddlCardVirife")).Visible = false; 
                    }
                    e.Row.Cells[5].HorizontalAlign = HorizontalAlign.Center;
                    #endregion

                    #region 針對各欄位做所需處理

                    #region 設備ID
                    e.Row.Cells[4].Visible = false;
                    e.Row.Cells[0].Visible = false;
                    #endregion

                    #region CheckBox
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    ((CheckBox)e.Row.Cells[0].FindControl("RowCheckState1")).Attributes["onChange"] = "";
                    #endregion               

                    #region 樓層
                    this.hFloorType = (Hashtable)ViewState["ModelType"];
                    Button Bt_Floor = (Button)e.Row.Cells[3].FindControl("btFloor");
                    Bt_Floor.Font.Size = 10;
                    Bt_Floor.Style.Add("margin", "0px 2px 0px 2px");
                    Bt_Floor.Text = "樓層";
                    Bt_Floor.CommandArgument = e.Row.Cells[4].Text;
                    Bt_Floor.CommandName = e.Row.Cells[4].Text;
                    if (this.hFloorType != null && this.hFloorType.ContainsKey(e.Row.Cells[4].Text))
                    {
                        if (this.hFloorType[e.Row.Cells[4].Text].ToString() != "Elevator")
                            Bt_Floor.Enabled = false;
                        else
                        {
                            if (!hFloorAllData.ContainsKey(e.Row.Cells[4].Text))
                                hFloorAllData.Add(e.Row.Cells[4].Text, "");
                        }
                    }
                    else
                        Bt_Floor.Enabled = false;
                    /*
                    Bt_Floor.Attributes["onClick"] = "window.open('../../../web/03/CardFloor.aspx?EquID=" + e.Row.Cells[5].Text 
                        + "&FinalFloorData=" + this.hFinalFloorData.ClientID 
                        + "','','height=400,width=405,status=no,toolbar=no,menubar=no,location=no,top=50,left=100',''); return false;";
                     */ 
                    Bt_Floor.Attributes["onClick"] = "SetCardFloor('" + e.Row.Cells[4].Text + "','"+this.hFinalFloorData.ClientID+"');return false;    ";
                    e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Center;
                    #endregion

                    #endregion

                    #region 檢查內容是否有超出欄位長度
                    for (int x = 0; x < e.Row.Cells.Count; x++)
                    {
                        if (!string.IsNullOrEmpty(e.Row.Cells[x].Text.Trim()) && e.Row.Cells[x].Text != "&nbsp;")
                            e.Row.Cells[x].ToolTip = e.Row.Cells[x].Text;
                    }                   
                    #endregion

                    //e.Row.Attributes.Add("OnMouseOver", "onMouseMoveIn(0,this,'','')");
                    //e.Row.Attributes.Add("OnMouseOut", "onMouseMoveOut(0,this)");
                    //e.Row.Attributes.Add("OnClick", "SingleRowSelect(0, this, SelectCardNo,'" + oRow.Row["CardNo"].ToString() + "', '', '');");
                    break;
                #endregion
            }
        }

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

                int columnCount = ProcessGridView.Rows[0].Cells.Count;
                ProcessGridView.Rows[0].Cells.Clear();
                ProcessGridView.Rows[0].Cells.Add(new TableCell());
                ProcessGridView.Rows[0].Cells[0].ColumnSpan = columnCount;
                ProcessGridView.Rows[0].Cells[0].Text = "查無資料";
                ProcessGridView.RowStyle.HorizontalAlign = HorizontalAlign.Center;
            }
        }
        #endregion

        #endregion

        #region JavaScript及aspx共用方法

        #region SingleData 取得單人資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static ArrayList SingleData(String sPsnID, String UserID)
        {            
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            string[] EditData = null;
            ArrayList result_list = new ArrayList();    
            String sPicPara = "";
            List<string> liSqlPara = new List<string>();            
            var verifi_cnt = AuthModelCount();
            String sql = @"SELECT PsnID, PsnNo, PsnName, PsnEName, PsnType, IDNum, Birthday, OrgStrucID,
                PsnAccount, PsnPW, PsnAuthAllow, PsnSTime, PsnETime, PsnPicSource, Remark, UpdateTime,
                Text1, Text2, Text3, Text4, Text5 FROM B01_Person WHERE PsnNo = '" + sPsnID + "'";
            oAcsDB.GetDataReader(sql, out dr);
            if (dr.HasRows)
            {
                EditData = new string[dr.DataReader.FieldCount + 4];//基本資訊(21) + 額外資訊(4)

                while (dr.Read())
                {
                    for (int i = 0; i < dr.DataReader.FieldCount; i++)
                    {
                        if (i == 6)
                        {
                            if (!Convert.IsDBNull(dr.DataReader[i]))
                            {
                                EditData[i] = DateTime.Parse(dr.DataReader[i].ToString()).ToString("yyyy/MM/dd");
                            }
                        }
                        else if (i == 11 || i == 12)
                        {
                            if (!Convert.IsDBNull(dr.DataReader[i]))
                            {
                                EditData[i] = DateTime.Parse(dr.DataReader[i].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                            }
                        }
                        else
                        {
                            EditData[i] = dr.DataReader[i].ToString();
                        }
                    }
                }
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "系統中無此資料！";
            }

            if (EditData.Length > 2)
            {
                #region 組織設備列表
                dr = null;
                sql = @"SELECT OrgIDList, OrgNoList FROM OrgStruc('" + EditData[7] + "')";
                oAcsDB.GetDataReader(sql, out dr);

                if (dr.HasRows)
                {
                    dr.Read();
                    EditData[EditData.Length - 4] = dr.DataReader["OrgIDList"].ToString();
                    EditData[EditData.Length - 3] = dr.DataReader["OrgNoList"].ToString();
                }
                else
                {
                    EditData[EditData.Length - 4] = "";
                    EditData[EditData.Length - 3] = "";
                }
                #endregion

                #region 卡片資訊
                dr = null;
                sql = @"SELECT B01_Card.CardID, B01_Card.CardNo, B00_ItemList.ItemName
                    FROM B01_Card INNER JOIN B00_ItemList ON B01_Card.CardType = B00_ItemList.ItemNo
                    WHERE ItemClass = 'CardType' AND PsnID = '" + EditData[0] + "' AND CardType NOT IN ('R','TEMP','T') ";
                oAcsDB.GetDataReader(sql, out dr);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        EditData[EditData.Length - 2] += dr.DataReader["CardNo"].ToString() + "|" +
                            dr.DataReader["ItemName"].ToString() + "|" +
                            dr.DataReader["CardID"].ToString() + "|";
                    }

                    EditData[EditData.Length - 2] = EditData[EditData.Length - 2].Substring(0, EditData[EditData.Length - 2].Length - 1);
                }
                else
                {
                    EditData[EditData.Length - 2] = "";
                }
                #endregion

                #region 相片
                sql = @"SELECT ParaValue FROM B00_SysParameter WHERE ParaClass = 'Person' AND ParaNo = 'PicSource'";
                sPicPara = oAcsDB.GetStrScalar(sql);

                if (sPicPara != "")
                {
                    if (sPicPara == "File")
                    {
                        sql = @"SELECT ParaValue FROM B00_SysParameter WHERE ParaClass = 'Person' AND ParaNo = '" + sPicPara + "'";
                        sPicPara = oAcsDB.GetStrScalar(sql);
                        EditData[EditData.Length - 1] = PicStr("File", GetPic(sPicPara, EditData[13]));
                    }
                    else if (sPicPara == "Url")
                    {
                        sql = @"SELECT ParaValue FROM B00_SysParameter WHERE ParaClass = 'Person' AND ParaNo = '" + sPicPara + "'";
                        sPicPara = oAcsDB.GetStrScalar(sql);
                        //if (EditData[13] != "")
                        //    EditData[EditData.Length - 1] = PicStr("Url", sPicPara + EditData[13]);
                        //else
                            EditData[EditData.Length - 1] = "/Img/default.png";
                    }
                    else if (sPicPara == "Sql")
                    {

                    }
                }                
                #endregion
                result_list.Add(EditData);
                if (verifi_cnt > 0)
                {
                    sql = @"SELECT VerifiMode FROM B01_Person WHERE PsnID=? ";
                    liSqlPara.Clear();
                    liSqlPara.Add("S:" + EditData[0]);
                    var verifi_mode = oAcsDB.GetStrScalar(sql, liSqlPara);
                    result_list.Add(verifi_mode);
                }
                else
                {
                    result_list.Add("0");
                }
            }            
            //return_arr[0] = return_arr;                        
            return result_list;
        }
        #endregion

        #region 人員資料所有控制項初始動作
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static String[] DefaultData(String UserID, String PsnID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            string[] EditData = new string[2];//0：人員類型、1：起始日

            #region 人員類型
            String sql = @" SELECT ItemNo, ItemName FROM B00_ItemList WHERE ItemClass IN ('PsnType') ORDER BY ItemOrder ";
            oAcsDB.GetDataReader(sql, out dr);

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    EditData[0] += dr.DataReader[0].ToString() + "|" + dr.DataReader[1].ToString() + "|";
                }

                if (EditData[0] != "")
                {
                    EditData[0] = EditData[0].Substring(0, EditData[0].Length - 1);
                }
            }
            else
            {
                EditData[0] = "";
            }
            #endregion

            #region 起始日期
            EditData[1] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            #endregion

            return EditData;
        }
        #endregion

        #region 動態切換相片
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static String PicChange(String PicName)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            String sPicPara = "";
            String src = "";
            String sql = " SELECT ParaValue FROM B00_SysParameter WHERE ParaClass = 'Person' AND ParaNo = 'PicSource' ";
            sPicPara = oAcsDB.GetStrScalar(sql);
            if (sPicPara != "")
            {
                if (sPicPara == "File")
                {
                    sql = " SELECT ParaValue FROM B00_SysParameter WHERE ParaClass = 'Person' AND ParaNo = '" + sPicPara + "' ";
                    sPicPara = oAcsDB.GetStrScalar(sql);
                    src = PicStr("File", GetPic(sPicPara, PicName));
                }
                else if (sPicPara == "Url")
                {
                    sql = " SELECT ParaValue FROM B00_SysParameter WHERE ParaClass = 'Person' AND ParaNo = '" + sPicPara + "' ";
                    sPicPara = oAcsDB.GetStrScalar(sql);
                    src = PicStr("Url", sPicPara + PicName);
                }
                else if (sPicPara == "Sql")
                {

                }
            }
            return src;
        }
        #endregion

        #region 轉換OrgStruc
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static String OrgStrucChange(String OrgStrucID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            List<string> liSqlPara = new List<string>();
            String sql = @" SELECT OrgNameList FROM OrgStruc(" + OrgStrucID + ") ";
            String sdata = oAcsDB.GetStrScalar(sql);
            return sdata;
        }
        #endregion

        #region 轉換src
        private static String PicStr(String mode, String urlImagePath)
        {
            Byte[] bytes = new Byte[1024];
            String src = "";
            String filename = urlImagePath.Trim();
            if (mode == "File")
            {
                if (filename != "" && filename != null)
                {
                    Uri uri = new Uri(filename);
                    WebRequest webRequest = WebRequest.Create(uri);
                    Stream stream = webRequest.GetResponse().GetResponseStream();
                    BinaryReader br = new BinaryReader(stream);
                    bytes = br.ReadBytes((Int32)stream.Length);
                    string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                    src = "data:image/png;base64," + base64String;
                }
                else
                    src = "/Img/default.png";
            }
            if (mode == "Url")
            {
                String[] lastname = { ".jpg", ".jpeg", ".bmp", ".png", ".gif", ".svg" };
                for (int i = 0; i < lastname.Length; i++)
                {
                    if (src == "")
                    {
                        try
                        {
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(filename + lastname[i]));
                            ServicePointManager.Expect100Continue = false;
                            ((HttpWebResponse)request.GetResponse()).Close();
                            src = filename + lastname[i];
                            goto HavePic;
                        }
                        catch
                        {
                        }
                    }
                }
            HavePic:
                if (src == "")
                    src = "/Img/default.png";
            }
            return src;
        }
        #endregion

        #region 取得相片完整路徑 for file
        private static String GetPic(String m_file, String s_pic)
        {
            if (File.Exists(m_file + s_pic + ".jpg"))
                s_pic = m_file + s_pic + ".jpg";
            else if (File.Exists(m_file + s_pic + ".JPG"))
                s_pic = m_file + s_pic + ".JPG";
            else if (File.Exists(m_file + s_pic + ".jpeg"))
                s_pic = m_file + s_pic + ".jpeg";
            else if (File.Exists(m_file + s_pic + ".JPEG"))
                s_pic = m_file + s_pic + ".JPEG";
            else if (File.Exists(m_file + s_pic + ".bmp"))
                s_pic = m_file + s_pic + ".bmp";
            else if (File.Exists(m_file + s_pic + ".BMP"))
                s_pic = m_file + s_pic + ".BMP";
            else if (File.Exists(m_file + s_pic + ".png"))
                s_pic = m_file + s_pic + ".png";
            else if (File.Exists(m_file + s_pic + ".PNG"))
                s_pic = m_file + s_pic + ".PNG";
            else if (File.Exists(m_file + s_pic + ".gif"))
                s_pic = m_file + s_pic + ".gif";
            else if (File.Exists(m_file + s_pic + ".GIF"))
                s_pic = m_file + s_pic + ".GIF";
            else if (File.Exists(m_file + s_pic + ".svg"))
                s_pic = m_file + s_pic + ".svg";
            else if (File.Exists(m_file + s_pic + ".SVG"))
                s_pic = m_file + s_pic + ".SVG";
            else
                s_pic = "";

            return s_pic;
        }
        #endregion

        #region 取得客戶端IP位址
        private static String IPAddress()
        {
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
            {
                sIPAddress = "127.0.0.1";
            }

            return sIPAddress;
        }
        #endregion

        #region 預設權限查詢
        private static Dictionary<string,string> ResetDefaultAuth(string PsnID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DBReader dr = null;
            Dictionary<string, string> hEquGrIDList = new Dictionary<string, string>();
            /*
            sql = @"SELECT DISTINCT B01_EquGroup.EquGrpID FROM B01_OrgStruc
                INNER JOIN B01_OrgEquGroup ON B01_OrgStruc.OrgStrucID = B01_OrgEquGroup.OrgStrucID
                INNER JOIN B01_EquGroup ON B01_OrgEquGroup.EquGrpID = B01_EquGroup.EquGrpID
                INNER JOIN B01_EquGroupData ON B01_OrgEquGroup.EquGrpID = B01_EquGroupData.EquGrpID
                INNER JOIN B01_EquData ON B01_EquGroupData.EquID = B01_EquData.EquID
                WHERE B01_OrgEquGroup.OrgStrucID = (SELECT OrgStrucID FROM B01_Person WHERE PsnID = '" + PsnID + "')";
             */
            sql = @"SELECT DISTINCT B01_EquGroup.EquGrpID FROM B01_OrgStruc
                INNER JOIN B01_OrgEquGroup ON B01_OrgStruc.OrgStrucID = B01_OrgEquGroup.OrgStrucID
                INNER JOIN B01_EquGroup ON B01_OrgEquGroup.EquGrpID = B01_EquGroup.EquGrpID
                INNER JOIN B01_EquGroupData ON B01_OrgEquGroup.EquGrpID = B01_EquGroupData.EquGrpID
                INNER JOIN B01_EquData ON B01_EquGroupData.EquID = B01_EquData.EquID
                WHERE 
				B01_OrgEquGroup.OrgStrucID = (SELECT OrgStrucID FROM B01_Person WHERE PsnID ={0})
				AND B01_EquGroup.EquGrpID 
				NOT IN (SELECT EquGrpID FROM B01_Person P 
				INNER JOIN B01_Card C ON C.PsnID=P.PsnID 
				INNER JOIN B01_CardEquGroup C2 ON C.CardID=C2.CardID WHERE P.PsnID={1})";
            oAcsDB.GetDataReader(string.Format(sql,PsnID,PsnID), out dr);

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    if (!hEquGrIDList.ContainsKey(dr.DataReader["EquGrpID"].ToString()))
                    {
                        hEquGrIDList.Add(dr.DataReader["EquGrpID"].ToString(), dr.DataReader["EquGrpID"].ToString());
                    }
                }
            }

            return hEquGrIDList;
        }
        #endregion

        #region 組織架構檢查是否存在
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object CheckOrgStruc(string sUserID, string sBirthday, string sOrgIDList, string sOrgNoList)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string sql = "", strOrgStrucID = "0";
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;
            string[] NoArray = { sUserID, sBirthday, sOrgIDList, sOrgNoList, "" };

            #region 檢查是否存在該組織架構
            sql = @"SELECT OrgStrucID FROM B01_OrgStruc WHERE OrgIDList = ?";
            liSqlPara.Add("S:" + sOrgIDList);
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);

            if (dr.Read())
            {
                NoArray[4] = "true";
                return NoArray;
            }
            else
            {
                NoArray[4] = "false";
                return NoArray;
            }
            #endregion

        }
        #endregion

        #region 檢查是否建立指紋認證模式

        public static int AuthModelCount()
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DataTable dt = new DataTable();
            oAcsDB.GetDataTable("Table1", "SELECT * FROM B00_ItemList WHERE ItemClass='AuthModel'", out dt);
            return dt.Rows.Count;           
        }

        #endregion

        #region Person Insert 新增人員資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Insert(string sPsnNo, string sPsnName, string sPsnEName, string sPsnType, string sPsnIDNum, string sBirthday, string sOrgIDList, string sOrgNoList, 
            string sCardNo, string sPsnAccount, string sPsnPW, string sPsnAuthAllow, string sPsnSTime, string sPsnETime, string sPsnPicSource, string sRemark, 
            string sText1, string sText2, string sText3, string sText4, string sText5, string verifiMode, string UserID, string sCardVer)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            string[] arrOrgStrucID;
            List<string> liSqlPara = new List<string>();
            string[] NoArray = { sPsnNo, sPsnName, sCardNo,sPsnAccount };
            String DefaultETime = "";
            sRet = Check_Input_DB(NoArray, "Insert");//檢查欄位必填值
            arrOrgStrucID = CheckOrgList(sOrgIDList, sOrgNoList, UserID).Split('|');//檢查組織列表是否存在

            if (sRet.result && !arrOrgStrucID[0].Equals(""))
            {
                #region Process String
                int verifi_cnt = AuthModelCount();
                sql = @"INSERT INTO B01_Person (PsnNo, PsnName, PsnEName, PsnType, IDNum, Birthday, OrgStrucID,
                    PsnAccount, PsnPW, PsnAuthAllow, PsnSTime, PsnETime, PsnPicSource, Remark, Text1, Text2, Text3,
                    Text4, Text5, CreateUserID, UpdateUserID, UpdateTime ";
                if (verifi_cnt > 0)
                {
                    sql += ",VerifiMode";
                }
                sql += " ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?,  GETDATE()";
                if (verifi_cnt > 0)
                {
                    sql += ",? ";
                }
                sql += ")";                                                               
                liSqlPara.Add("S:" + sPsnNo.Trim());
                liSqlPara.Add("S:" + sPsnName.Trim());
                liSqlPara.Add("S:" + sPsnEName.Trim());
                liSqlPara.Add("S:" + sPsnType);
                liSqlPara.Add("S:" + sPsnIDNum.Trim());
                liSqlPara.Add("S:" + sBirthday);
                liSqlPara.Add("I:" + arrOrgStrucID[0]);
                liSqlPara.Add("S:" + sPsnAccount.Trim());
                liSqlPara.Add("S:" + sPsnPW.Trim());
                liSqlPara.Add("S:" + sPsnAuthAllow);
                liSqlPara.Add("S:" + sPsnSTime);

                if (sPsnETime != "")
                {
                    DefaultETime = sPsnETime;
                    liSqlPara.Add("S:" + sPsnETime);
                }
                else
                {
                    DefaultETime = oAcsDB.GetStrScalar("SELECT ParaValue FROM B00_SysParameter WHERE ParaClass = 'System' AND ParaNo = 'DefaultEndDateTime' ");
                    liSqlPara.Add("S:" + DefaultETime);
                }

                liSqlPara.Add("S:" + sPsnPicSource.Trim());
                liSqlPara.Add("S:" + sRemark.Trim());
                liSqlPara.Add("S:" + sText1.Trim());
                liSqlPara.Add("S:" + sText2.Trim());
                liSqlPara.Add("S:" + sText3.Trim());
                liSqlPara.Add("S:" + sText4.Trim());
                liSqlPara.Add("S:" + sText5.Trim());                
                liSqlPara.Add("S:" + UserID);
                liSqlPara.Add("S:" + UserID);
                if (verifi_cnt > 0)
                {
                    liSqlPara.Add("S:" + verifiMode);
                }                
                #endregion

                oAcsDB.BeginTransaction();
                int istat = oAcsDB.SqlCommandExecute(sql, liSqlPara);

                if (istat > -1)
                {
                    if (oAcsDB.Commit())
                    {
                        #region 取得新資料ID

                        #region Process String
                        sql = " SELECT * FROM B01_Person WHERE PsnNo = ? ";
                        liSqlPara.Clear();
                        liSqlPara.Add("S:" + sPsnNo.Trim());
                        #endregion

                        Sa.DB.DBReader oReader = null;

                        if (oAcsDB.GetDataReader(sql, liSqlPara, out oReader))
                        {
                            if (oReader.HasRows)
                            {
                                oReader.Read();

                                #region 新增主卡預設啟用
                                //20151201 修改新增人員基本資料同時新增卡片時，卡片啟用狀態帶入人員啟用狀態及卡片類型帶入人員類型 例：PsnType = 'E' CardType = 'E'
	                            //原為取得ItemList的主卡代碼 A
                                //String DefaultETime = oAcsDB.GetStrScalar("SELECT ParaValue FROM B00_SysParameter WHERE ParaClass = 'System' AND ParaNo = 'DefaultEndDateTime' ");
                                CardInsert_ForPerson(oReader.DataReader["PsnID"].ToString(), sCardNo.Trim(), sCardVer, "0000", "", "", sPsnType, sPsnAuthAllow, DateTime.Parse(oReader.DataReader["PsnSTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"), DefaultETime, "", UserID);
                                liSqlPara.Clear();
                                #endregion

                                if (arrOrgStrucID[1].Equals("true"))
                                {
                                    sRet.message = sPsnNo.Trim() + "|" + oReader.DataReader["PsnID"].ToString() + "|"+gms.MsgAddPsnSuccess;
                                }
                                else
                                {
                                    sRet.message = sPsnNo.Trim() + "|" + oReader.DataReader["PsnID"].ToString() + "|"+gms.MsgAddPsnSuccess1;
                                }
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    oAcsDB.Rollback();
                    sRet.result = false;
                    sRet.message = gms.MsgAddPsnFailed;
                }
            }
            else if (sRet.result && arrOrgStrucID[0].Equals(""))
            {
                sRet.result = false;
                sRet.message = gms.MsgAddPsnFailed1;
            }

            sRet.act = "Add";
            return sRet;
        }

        //20151201 新增人員基本資料同時新增卡片
        public static bool CardInsert_ForPerson(string SelectValue, string sCardNo, string sCardVer, string sCardPW, string sCardSerialNo, string sCardNum, string sCardType, string sCardAuthAllow, string sCardSTime, string sCardETime, string sCardDesc, String UserID)
        {
            string sIPAddress = IPAddress();//取得客戶端IP位址
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            DBReader dr = null;
            Queue<String> EquIDList = new Queue<String>();
            Dictionary<string, string> h_EquGrIDList = new Dictionary<string, string>();
            int istat = 0;
            List<string> liSqlPara = new List<string>();
            String DefaultETime = oAcsDB.GetStrScalar("SELECT ParaValue FROM B00_SysParameter WHERE ParaClass = 'System' AND ParaNo = 'DefaultEndDateTime' ");            
            string[] NoArray = { sCardNo, sCardPW, SelectValue, sCardType };
            sRet = Check_Input_DB(NoArray, "CardInsert");

            OrmDataObject odo = new OrmDataObject("MsSql"
            , string.Format(Pub.db_connection_template, Pub.db_source_ip, Pub.db_data_name, Pub.db_user_id, Pub.db_pwd));
            StringBuilder builders = new StringBuilder();

            if (sRet.result)
            {
                if (istat > -1)
                {
                    #region Process String
                    sql = @" INSERT INTO B01_Card(CardNo, CardVer, CardPW, CardSerialNo, CardNum,
                        CardType, PsnID, CardAuthAllow, CardSTime, CardETime, CardDesc, CreateUserID,
                        UpdateUserID, UpdateTime) VALUES (?, ?, ?, ?,?, 
                       (SELECT TOP 1 ItemNo FROM B00_ItemList WHERE ItemClass='CardType' ORDER BY ItemOrder )
                        , ?, ?, ?, ?, ?, ?, ?, GETDATE()); ";
                    System.Web.HttpContext context = System.Web.HttpContext.Current;
                    liSqlPara.Add("S:" + sCardNo.Trim());
                    liSqlPara.Add("S:" + sCardVer.Trim());
                    liSqlPara.Add("S:" + sCardPW.Trim());
                    liSqlPara.Add("S:" + sCardSerialNo);
                    liSqlPara.Add("S:" + sCardNum.Trim());
                    //liSqlPara.Add("S:" + sCardType);
                    liSqlPara.Add("I:" + SelectValue);
                    liSqlPara.Add("S:" + sCardAuthAllow);
                    liSqlPara.Add("S:" + sCardSTime);

                    if (sCardETime != "")
                    {
                        liSqlPara.Add("S:" + sCardETime);
                    }
                    else
                    {
                        sCardETime = DefaultETime;
                        liSqlPara.Add("S:" + DefaultETime);
                    }

                    liSqlPara.Add("S:" + sCardDesc);
                    liSqlPara.Add("S:" + UserID);
                    liSqlPara.Add("S:" + UserID);
                    #endregion

                    oAcsDB.BeginTransaction();
                    istat = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }

                if (istat > -1)
                {
                    oAcsDB.Commit();
                    sRet.message = gms.MsgAddSuccess+"|";
                    dr = null;
                    liSqlPara = new List<string>();
                    var result=odo.GetQueryResult(@"SELECT B01_Card.CardID, B01_Card.CardNo, B00_ItemList.ItemName FROM B01_Card
                        INNER JOIN B00_ItemList ON B01_Card.CardType = B00_ItemList.ItemNo
                        WHERE ItemClass = 'CardType' AND PsnID = @PsnID AND CardType NOT IN ('T','R','TEMP')", new {PsnID=SelectValue});
                    string sCardID = "";

                    if (result.Count()>0)
                    {
                        foreach (var o in result)
                        {
                            sRet.message += o.CardNo.ToString() + "|" +
                                    o.ItemName.ToString() + "|" +
                                    o.CardID.ToString() + "|";
                            if (o.CardNo.ToString() == sCardNo)
                            {
                                sCardID = o.CardID.ToString();
                                odo.Execute("INSERT INTO B01_CardExt(CardID, CardBorrow) VALUES (@CardID, 0)",new {CardID=sCardID});
                            }
                        }

                        #region 預設群組權限
                        if (sCardID != "")
                        {
                            //新的卡片預設權限設定    20170604 update by Sam
                            odo.SetOrgEquData(sCardNo, int.Parse(SelectValue), UserID, gms);
                        }
                        #endregion

                        sRet.message = sRet.message.Substring(0, sRet.message.Length - 1);
                    }
                }
                else
                {
                    oAcsDB.Rollback();
                    sRet.result = false;
                    sRet.message = gms.MsgAddFailed;
                }
            }
            sRet.act = "Add";
            return sRet.result;
        }

        #endregion

        #region Person Update 更新人員資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Update(string SelectValue, string sPsnOldNo, string sPsnNo, string sPsnName, string sPsnEName, string sPsnType, string sPsnIDNum, string sBirthday,
            string sOrgIDList, string sOrgNoList, string sPsnAccount, string sPsnPW, string sPsnAuthAllow, string sPsnSTime, string sPsnETime, string sPsnPicSource, string sRemark,
            string sText1, string sText2, string sText3, string sText4, string sText5, string verifiMode, string UserID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            OrmDataObject odo = new OrmDataObject("MsSql",Pub.GetDapperConnString());
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "", sCardID = "", sCardNo = "";
            string[] arrOrgStrucID;
            List<string> liSqlPara = new List<string>();
            Dictionary<string, string> h_EquGrIDList = new Dictionary<string, string>();
            string[] NoArray = { sPsnNo, sPsnName, sPsnOldNo,sPsnAccount };

            sRet = Check_Input_DB(NoArray, "Update");//檢查欄位必填值
            arrOrgStrucID = CheckOrgList(sOrgIDList, sOrgNoList, UserID).Split('|');//檢查組織列表是否存在
            //取得該人員舊的組織架構
            var persons = odo.GetQueryResult(@"SELECT P.*,O.EquGrpID FROM B01_Person P	
	            LEFT JOIN B01_OrgEquGroup O ON O.OrgStrucID=P.OrgStrucID WHERE PsnID=@PsnID", new { PsnID = SelectValue });
            if (sRet.result && !arrOrgStrucID[0].Equals(""))
            {
                int verifi_cnt = AuthModelCount();
                sql = @"UPDATE B01_Person SET PsnNo = @PsnNo, PsnName = @PsnName, PsnEName = @PsnEName, PsnType = @PsnType, IDNum = @IDNum,
                    Birthday = @Birthday, OrgStrucID = @OrgStrucID, PsnAccount = @PsnAccount, PsnPW = @PsnPW, PsnAuthAllow = @PsnAuthAllow, PsnSTime = @PsnSTime,
                    PsnETime = @PsnETime, PsnPicSource = @PsnPicSource, Remark = @Remark,UpdateUserID=@UpdateUserID,UpdateTime=GETDATE() ";
                string chkInputText = "SELECT * FROM B00_SysParameter WHERE ParaClass='InputText' AND ParaNo=@InputText AND ParaValue='Y'";
                #region Process String
                if (verifi_cnt > 0)
                {
                    sql += " ,VerifiMode = @VerifiMode";
                }
                if(odo.GetQueryResult(chkInputText,new {InputText="Input_Text1"}).Count() > 0)
                {
                    sql += ",Text1=@Text1";
                }
                if (odo.GetQueryResult(chkInputText, new { InputText = "Input_Text2" }).Count() > 0)
                {
                    sql += ",Text2=@Text2";
                }
                if(odo.GetQueryResult(chkInputText, new { InputText = "Input_Text3" }).Count() > 0)
                {
                    sql += ",Text3=@Text3";
                }
                if (odo.GetQueryResult(chkInputText, new { InputText = "Input_Text4" }).Count() > 0)
                {
                    sql += ",Text4=@Text4";
                }
                if (odo.GetQueryResult(chkInputText, new { InputText = "Input_Text5" }).Count() > 0)
                {
                    sql += ",Text5=@Text5";
                }
                sql += " WHERE PsnID=@PsnID";
              
                var DefaultEndTime = sPsnETime!=""?sPsnETime: oAcsDB.GetStrScalar(@"SELECT ParaValue FROM B00_SysParameter WHERE ParaClass = 'System' 
                    AND ParaNo = 'DefaultEndDateTime'");

                #endregion

                odo.BeginTransaction();
                int istat = odo.Execute(sql, new
                {
                    PsnID = SelectValue,
                    PsnNo = sPsnNo.Trim(),
                    PsnName = sPsnName.Trim(),
                    PsnEName = sPsnEName.Trim(),
                    PsnType = sPsnType,
                    IDNum = sPsnIDNum.Trim(),
                    Birthday = sBirthday.Trim(),
                    OrgStrucID = arrOrgStrucID[0],
                    PsnAccount = sPsnAccount.Trim(),
                    PsnPW = sPsnPW.Trim(),
                    PsnAuthAllow = sPsnAuthAllow,
                    PsnSTime = sPsnSTime,
                    PsnETime = DefaultEndTime,
                    PsnPicSource=sPsnPicSource,
                    Remark = sRemark.Trim(),
                    UpdateUserID = UserID,
                    Text1 = sText1.Trim(),
                    Text2 = sText2.Trim(),
                    Text3 = sText3.Trim(),
                    Text4 = sText4.Trim(),
                    Text5 = sText5.Trim(),
                    VerifiMode = verifiMode.Trim()
                });
                if (istat > -1)
                {
                    odo.Commit();
                    //記錄刪除預設權限的log                
                    var log = DBClass.SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.人員權限設定, UserID, UserName, "0103");
                    log.LogInfo = string.Format("Person Update PsnNo='{0}',OrgSturcID={1},PsnSTime='{2}',PsnSTime='{3}'",sPsnNo, arrOrgStrucID[0], sPsnSTime, sPsnETime);
                    odo.SetSysLogCreate(log);

                    #region 有卡片資料需執行CardAuth_Update
                    liSqlPara.Clear();
                    DataTable dtCardID = new DataTable();
                    oAcsDB.GetDataTable("CardID", "SELECT CardID, CardNo FROM B01_Card WHERE CardType NOT IN ('R','T','TEMP') AND PsnID = '" + SelectValue + "'", out dtCardID); ;                    
                    if (dtCardID.Rows.Count > 0)
                    {
                        sCardID = dtCardID.Rows[0].ItemArray[dtCardID.Columns.IndexOf("CardID")].ToString();
                        sCardNo = dtCardID.Rows[0].ItemArray[dtCardID.Columns.IndexOf("CardNo")].ToString();
                        odo.SetOrgEquData(sCardNo, int.Parse(SelectValue), UserID, gms);
                        sRet.message = gms.MsgUpdatePsnSuccess;                                                                       
                    }
                    else
                    {
                        if (istat > 0)
                        {
                            if (arrOrgStrucID[1].Equals("true"))
                            {
                                sRet.message = gms.MsgUpdatePsnSuccess;
                            }
                            else
                            {
                                sRet.message = gms.MsgUpdatePsnSuccess1;
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    oAcsDB.Rollback();
                    sRet.result = false;
                    sRet.message = gms.MsgUpdatePsnFailed;
                }
            }
            else if (sRet.result && arrOrgStrucID[0].Equals(""))
            {
                sRet.result = false;
                sRet.message = gms.MsgUpdatePsnFailed1;
            }

            sRet.act = "Edit";
            return sRet;
        }
        #endregion

        #region Confirm Person Delete 刪除人員資料確認
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object DeleteConfirm(string SelectValue, string sPsnNo, string sUserID)
        {
            Pub.MessageObject sRet = new Pub.MessageObject();
            string[] NoArray = new string[2];
            NoArray[0] = SelectValue;
            NoArray[1] = sPsnNo;

            sRet = Check_Input_DB(NoArray, "Delete");
            sRet.message += "|" + sUserID;
            sRet.act = "Delete";
            return sRet;
        }
        #endregion

        #region Person Delete 刪除人員資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Delete(string SelectValue, string sPsnNo, String UserID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            int istat = 0;
            Queue<String> CardIDList = new Queue<String>();
            String CardID = "";
            DBReader dr = null;
            List<string> liSqlPara = new List<string>();
            sql = @" SELECT CardID FROM B01_Card WHERE PsnID = '" + SelectValue.Trim() + "' AND CardType NOT IN ('R','TEMP','T'); ";
            oAcsDB.GetDataReader(sql, out dr);

            if (dr.HasRows)
            {
                while (dr.Read())
                    CardIDList.Enqueue(dr.DataReader[0].ToString());
            }
            if (sRet.result)
            {
                string organism_sql = "";
                if (istat > -1)
                {
                    #region Process String
                    sql = @"DELETE FROM B01_PersonPicture WHERE PsnID = ?;";
                    liSqlPara.Add("S:" + SelectValue.Trim());
                    sql += @"DELETE FROM B01_Person WHERE PsnID = ? AND PsnNo = ? ; ";
                    liSqlPara.Add("S:" + SelectValue.Trim());
                    liSqlPara.Add("S:" + sPsnNo.Trim());
                    string sCardNo = "";
                    while (CardIDList.Count > 0)
                    {
                        CardID = (String)CardIDList.Dequeue();
                        sCardNo = oAcsDB.GetStrScalar("SELECT CardNo FROM B01_Card WHERE CardID = " + CardID);
                        sql += @" UPDATE B01_CardAuth SET OpMode = 'Del',OpStatus='' WHERE CardNo = '" + sCardNo + "' ; ";
                        sql += @" DELETE FROM B01_CardEquAdj WHERE CardID = '" + CardID + "' ; ";
                        sql += @" DELETE FROM B01_CardEquGroup WHERE CardID = '" + CardID + "' ; ";
                        sql += @" DELETE FROM B01_CardExt WHERE CardID = '" + CardID + "' ; ";
                        sql += @" DELETE FROM B01_Card WHERE CardID = '" + CardID + "' ; ";
                        odo.SetPsnDelete(sCardNo);
                    }
                    #endregion
                    oAcsDB.BeginTransaction();
                    istat = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                if (istat > -1)
                {
                    oAcsDB.Commit();                    
                    var log = SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.人員權限設定, UserID, UserName, "0103");
                    log.LogInfo = string.Format("Delete Person {0}",sPsnNo);
                    log.LogDesc = "刪除人員";
                    odo.SetSysLogCreate(log);
                    try
                    {                        
                    }
                    catch (Exception ex)
                    {

                    }                    
                }                   
                else
                    oAcsDB.Rollback();
            }

            sRet.act = "Delete";
            return sRet;
        }
        #endregion

        #region SingleCardData 取得單張卡片資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object SingleCardData(string CardID)
        {
            DBReader dr = null;
            DataTable dt = new DataTable();
            string[] EditData = null;

            if (CardID != "")
            {
                DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
                List<string> liSqlPara = new List<string>();
                String sql = @" SELECT CardID, CardNo, CardVer, CardPW, CardSerialNo, CardNum,
                    CardType, PsnID, CardAuthAllow, CardSTime, CardETime, CardDesc FROM B01_Card
                    WHERE CardID = ? ; ";
                liSqlPara.Add("I:" + CardID.Trim());
                oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                if (dr.HasRows)
                {
                    EditData = new string[dr.DataReader.FieldCount + 3];

                    while (dr.Read())
                    {
                        for (int i = 0; i < dr.DataReader.FieldCount; i++)
                        {
                            if (i == 9 || i == 10)
                            {
                                if (!Convert.IsDBNull(dr.DataReader[i]))
                                {
                                    EditData[i] = DateTime.Parse(dr.DataReader[i].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                                }
                                else
                                {
                                    EditData[i] = "";
                                }
                            }
                            else
                            {
                                EditData[i] = dr.DataReader[i].ToString();
                            }
                        }
                    }

                    #region 卡片類型
                    dr = null;
                    sql = @" SELECT ItemNo, ItemName FROM B00_ItemList
                    WHERE ItemClass = 'CardType' AND ItemNo<>'R' AND ItemInfo1 = 'Default' ORDER BY ItemOrder ";
                    oAcsDB.GetDataReader(sql, out dr);

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            EditData[EditData.Length - 3] += dr.DataReader["ItemNo"].ToString() + "|" +
                                dr.DataReader["ItemName"].ToString() + "|";
                        }

                        if (EditData[EditData.Length - 3] != "")
                        {
                            EditData[EditData.Length - 3] = EditData[EditData.Length - 3].Substring(0, EditData[EditData.Length - 3].Length - 1);
                        }
                    }
                    else
                    {
                        EditData[EditData.Length - 3] = "";
                    }
                    #endregion

                    #region 卡片權限
                    EditData[EditData.Length - 2] =string.Format("0|{0}|1|{1}",strDisabled,strEnabled);
                    #endregion

                    #region 卡片設碼資料
                    dr = null;
                    liSqlPara.Clear();
                    sql = @" SELECT A.EquID, CardRule, EquModel ";
                    int verifi_cnt = AuthModelCount();
                    if(verifi_cnt>0){
                        sql += " ,VerifiMode ";
                    }
                    sql+=" FROM B01_CardAuth A INNER JOIN B01_EquData B ON A.EquID=B.EquID WHERE CardNo = ? ; ";
                    liSqlPara.Add("S:" + EditData[1].ToString().Trim());
                    oAcsDB.GetDataReader(sql, liSqlPara, out dr);

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            EditData[EditData.Length - 1] += dr.DataReader["EquID"].ToString() + "|" +
                                dr.DataReader["CardRule"].ToString() + "%" + dr.DataReader["EquModel"].ToString()+ "%";
                            if(verifi_cnt>0)
                            {
                                EditData[EditData.Length - 1] += dr.DataReader["VerifiMode"].ToString();
                            }
                            else
                            {
                                EditData[EditData.Length - 1] += "0";
                            }                            
                            EditData[EditData.Length - 1] += "|";
                        }

                        if (EditData[EditData.Length - 1] != "")
                        {
                            EditData[EditData.Length - 1] = EditData[EditData.Length - 1].Substring(0, EditData[EditData.Length - 1].Length - 1);
                        }
                    }
                    else
                    {
                        EditData[EditData.Length - 1] = "";
                    }
                    #endregion
                }
                else
                {
                    EditData = new string[2];
                    EditData[0] = "Saho_SysErrorMassage";
                    EditData[1] = "系統中無此資料！";
                }
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "請選取欲查詢卡片資料！";
            }

            return EditData;
        }
        #endregion

        #region 卡片資料所有控制項初始動作
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object DefaultCardData()
        {
            DBReader dr = null;
            string[] EditData = new string[3];
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            List<string> liSqlPara = new List<string>();

            #region 卡片類型
            String sql = @" SELECT ItemNo, ItemName, CASE ISNUMERIC(ItemInfo2) WHEN 0 THEN '8' ELSE ItemInfo2 END  AS ItemInfo2 FROM B00_ItemList
                WHERE ItemClass = 'CardType' AND ItemNo NOT IN ('R','T') AND ItemInfo1 = 'Default' ORDER BY ItemOrder ";
            oAcsDB.GetDataReader(sql, out dr);

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    EditData[0] += dr.DataReader["ItemNo"].ToString() + "|" +
                        dr.DataReader["ItemName"].ToString() + "|" + dr.DataReader["ItemInfo2"].ToString() +"|";
                }

                if (EditData[0] != "")
                {
                    EditData[0] = EditData[0].Substring(0, EditData[0].Length - 1);
                }
            }
            else
            {
                EditData[0] = "";
            }
            #endregion

            #region 卡片權限
            EditData[1] = string.Format("0|{0}|1|{1}", strDisabled, strEnabled);
            #endregion

            #region 啟用時間
            EditData[2] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            #endregion

            return EditData;
        }
        #endregion

        #region Card Insert 新增卡片資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object CardInsert(string SelectValue, string sCardNo, string sCardVer, string sCardPW, string sCardSerialNo, string sCardNum, string sCardType, string sCardAuthAllow, string sCardSTime, string sCardETime, string sCardDesc, String UserID)
        {
            string sIPAddress = IPAddress();//取得客戶端IP位址
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            DBReader dr = null;
            Queue<String> EquIDList = new Queue<String>();
            Dictionary<string, string> h_EquGrIDList = new Dictionary<string, string>();
            int istat = 0;
            List<string> liSqlPara = new List<string>();
            String DefaultETime = oAcsDB.GetStrScalar("SELECT ParaValue FROM B00_SysParameter WHERE ParaClass = 'System' AND ParaNo = 'DefaultEndDateTime' ");
            string[] NoArray = { sCardNo, sCardPW, SelectValue, sCardType };
            sRet = Check_Input_DB(NoArray, "CardInsert");

            if (sRet.result)
            {
                if (istat > -1)
                {
                    #region Process String
                    sql = @" INSERT INTO B01_Card(CardNo, CardVer, CardPW, CardSerialNo, CardNum,
                        CardType, PsnID, CardAuthAllow, CardSTime, CardETime, CardDesc, CreateUserID,
                        UpdateUserID, UpdateTime) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, GETDATE()); ";
                    System.Web.HttpContext context = System.Web.HttpContext.Current;
                    liSqlPara.Add("S:" + sCardNo.Trim());
                    liSqlPara.Add("S:" + sCardVer.Trim());
                    liSqlPara.Add("S:" + sCardPW.Trim());
                    liSqlPara.Add("S:" + sCardSerialNo);
                    liSqlPara.Add("S:" + sCardNum.Trim());
                    liSqlPara.Add("S:" + sCardType);
                    liSqlPara.Add("I:" + SelectValue);
                    liSqlPara.Add("S:" + sCardAuthAllow);
                    liSqlPara.Add("S:" + sCardSTime);

                    if (sCardETime != "")
                    {
                        liSqlPara.Add("S:" + sCardETime);
                    }
                    else
                    {
                        sCardETime = DefaultETime;
                        liSqlPara.Add("S:" + DefaultETime);
                    }

                    liSqlPara.Add("S:" + sCardDesc);
                    liSqlPara.Add("S:" + UserID);
                    liSqlPara.Add("S:" + UserID);
                    #endregion

                    oAcsDB.BeginTransaction();
                    istat = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }

                if (istat > -1)
                {
                    oAcsDB.Commit();
                    OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
                    var log = SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.資料新增, UserID, UserName, "0103");
                    log.LogInfo = string.Format("Insert Card CardNo='{0}',CardSTime='{1}',CardETime='{2}'", sCardNo, sCardSTime, sCardETime);
                    log.LogDesc = "新增卡片權限";                   
                    odo.SetSysLogCreate(log);

                    sRet.message = gms.MsgAddSuccess+"|";
                    dr = null;
                    liSqlPara = new List<string>();
                    sql = @" SELECT B01_Card.CardID, B01_Card.CardNo, B00_ItemList.ItemName FROM B01_Card
                        INNER JOIN B00_ItemList ON B01_Card.CardType = B00_ItemList.ItemNo
                        WHERE ItemClass = 'CardType' AND PsnID = '" + SelectValue + "' ";
                    oAcsDB.GetDataReader(sql, out dr);
                    var result = odo.GetQueryResult(@"SELECT B01_Card.CardID, B01_Card.CardNo, B00_ItemList.ItemName FROM B01_Card
                                                                                        INNER JOIN B00_ItemList ON B01_Card.CardType = B00_ItemList.ItemNo
                                                                                        WHERE ItemClass = 'CardType' AND PsnID = @PsnID", new { PsnID = SelectValue });
                    string sCardID = "";
                    if (result.Count()>0)
                    {                        
                        foreach(var o in result)
                        {
                            sRet.message += o.CardNo.ToString() + "|" +
                                o.ItemName.ToString() + "|" +
                                o.CardID.ToString() + "|";
                            if (o.CardNo.ToString() == sCardNo)
                            {
                                sCardID = o.CardID.ToString();
                                odo.Execute(" INSERT INTO B01_CardExt(CardID, CardBorrow) VALUES (@CardID, 0)", new { CardID = sCardID });
                            }
                        }

                        #region 預設群組權限
                        if (sCardID != "")
                        {
                            //新的卡片預設權限設定    20170604 update by Sam
                            odo.SetOrgEquData(sCardNo, int.Parse(SelectValue), UserID, gms);
                        }
                        #endregion

                        sRet.message = sRet.message.Substring(0, sRet.message.Length - 1);
                    }
                }
                else
                {
                    oAcsDB.Rollback();
                    sRet.result = false;
                    sRet.message = gms.MsgAddFailed;
                }
            }
            sRet.act = "Add";
            return sRet;
        }
        #endregion

        #region Set CardVerifiMode 修改所有卡號的指紋認證模式

        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object SetCardVerifiMode(string SelectValue,string VerifiMode,string userid)
        {
            Pub.MessageObject sRet = new Pub.MessageObject();
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));            
            var sql_cmd = @"UPDATE B01_CardAuth SET 
	                                        VerifiMode=?,
	                                        ErrCnt=0,
	                                        OpMode='Reset',
	                                        OpStatus='',
                                            UpdateTime=GETDATE(),
                                            UpdateUserID=?
                                        WHERE CardNo IN (SELECT CardNo from B01_Card WHERE PsnID=? AND CardAuthAllow=1) 
                                        AND EquID IN (SELECT EquID FROM B01_EquData WHERE EquModel in ('ADM100FP','SST9500FP')) AND OpMode<>'Del' ;
                                        UPDATE B01_Person SET VerifiMode=? WHERE PsnID =? ";
            List<string> para = new List<string>();
            para.Add("S:" + VerifiMode);
            para.Add("S:" + userid);
            para.Add("I:" + SelectValue);
            para.Add("S:" + VerifiMode);            
            para.Add("I:" + SelectValue);
            int istat = 0;
            oAcsDB.BeginTransaction();
            istat = oAcsDB.SqlCommandExecute(sql_cmd,para);
            if (istat > -1)
            {
                oAcsDB.Commit();
                sRet.message = gms.MsgVerSuccess;
            }
            else
            {
                sRet.message = gms.MsgVerFailed;
            }
            return sRet;
        }

        #endregion

        #region Card Update 更新卡片資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object CardUpdate(string SelectValue, string SelectCardValue, string sCardNo
            , string sCardVer, string sCardPW, string sCardSerialNo, string sCardNum, string sCardType
            , string sCardAuthAllow, string sCardSTime, string sCardETime, string sCardDesc,string adm100setting, String UserID)
        {            
            string sIPAddress = IPAddress();//取得客戶端IP位址            
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));            
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            DBReader dr = null;
            DataTable dt = new DataTable();
            Queue<String> EquIDList = new Queue<String>();
            Dictionary<string, string> h_EquGrIDList = new Dictionary<string, string>();
            int istat = 0;
            List<string> liSqlPara = new List<string>();
            OrmDataObject odo = new OrmDataObject("MsSql",Pub.GetDapperConnString());
                               
            liSqlPara.Clear();
            string[] NoArray = { sCardNo, sCardPW, SelectValue, sCardType };
            sRet = Check_Input_DB(NoArray, "CardUpdate");
            if (sRet.result)
            {
                if (istat > -1)
                {
                    #region Process String
                    sql = @" UPDATE B01_Card SET CardNo = ?, CardVer = ?, CardPW = ?, CardSerialNo = ?,
                        CardNum = ?, CardType = ?, CardAuthAllow = ?, CardSTime = ?, CardETime = ?,
                        CardDesc = ?, UpdateUserID = ?, UpdateTime = GETDATE() WHERE CardID = ?; ";                    
                    liSqlPara.Add("S:" + sCardNo.Trim());
                    liSqlPara.Add("S:" + sCardVer.Trim());
                    liSqlPara.Add("S:" + sCardPW.Trim());
                    liSqlPara.Add("S:" + sCardSerialNo);
                    liSqlPara.Add("S:" + sCardNum.Trim());
                    liSqlPara.Add("S:" + sCardType);
                    liSqlPara.Add("S:" + sCardAuthAllow);
                    liSqlPara.Add("S:" + sCardSTime);
                    liSqlPara.Add("S:" + sCardETime);
                    liSqlPara.Add("S:" + sCardDesc);
                    liSqlPara.Add("S:" + UserID);
                    liSqlPara.Add("I:" + SelectCardValue);                    
                    #endregion
                    oAcsDB.BeginTransaction();
                    istat = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                if (istat > -1)
                {
                    oAcsDB.Commit();                    
                    var log = SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.卡片權限調整, UserID, UserName, "0103");
                    log.LogInfo = string.Format("Update Card CardNo='{0}',CardSTime='{1}',CardETime='{2}'", sCardNo, sCardSTime, sCardETime);
                    log.LogDesc = "修改卡片權限";
                    odo.SetSysLogCreate(log);
                    //新的卡片預設權限設定    20170524 update by Sam
                    sRet.message += odo.SetOrgEquData(sCardNo, int.Parse(SelectValue), UserID, gms);
                }
                else
                {
                    oAcsDB.Rollback();
                    sRet.result = false;
                    sRet.message = gms.MsgUpdateFailed;
                }
            }

            sRet.act = "Edit";
            return sRet;
        }
        #endregion

        #region Card Delete 刪除卡片資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object CardDelete(bool IsConfirm, string SelectValue, string SelectCardValue, String UserID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            DBReader dr = null;
            int istat = 0;
            List<string> liSqlPara = new List<string>();
            string sCardNo = "";
            if (IsConfirm)
            {
                if (sRet.result)
                {
                    string organism_sql = "";
                    if (istat > -1)
                    {
                        #region Process String
                        sql = @" UPDATE B01_Card SET PsnID=NULL WHERE CardID = ?  AND CardType IN ('R','TEMP','T') ; ";
                        sql += @" DELETE FROM B01_Card WHERE CardID = ?  AND CardType NOT IN ('R','TEMP','T') ; ";
                        sql += @" DELETE FROM B01_CardEquGroup WHERE CardID = ? ; ";
                        sql += @" DELETE FROM B01_CardExt WHERE CardID = ? ; ";
                        sql += @" DELETE FROM B01_CardEquAdj WHERE CardID = ? ; ";
                        sql += @" UPDATE B01_CardAuth SET OpMode = 'Del' WHERE CardNo = ? ; ";                        
                        sCardNo = oAcsDB.GetStrScalar("SELECT CardNo FROM B01_Card WHERE CardID = " + SelectCardValue);
                        liSqlPara.Add("I:" + SelectCardValue);
                        liSqlPara.Add("I:" + SelectCardValue);
                        liSqlPara.Add("I:" + SelectCardValue);
                        liSqlPara.Add("I:" + SelectCardValue);
                        liSqlPara.Add("I:" + SelectCardValue);
                        liSqlPara.Add("S:" + sCardNo);
                        organism_sql = string.Format("DELETE B02_EyeData WHERE CardNo='{0}';", sCardNo);
                        organism_sql += string.Format("DELETE B02_FaceData WHERE CardNo='{0}';", sCardNo);
                        organism_sql += string.Format("DELETE B02_FPData WHERE CardNo='{0}';", sCardNo);
                        organism_sql += string.Format("DELETE B02_FaceData2 WHERE CardNo='{0}';", sCardNo);
                        organism_sql += string.Format("DELETE B02_FaceImageData WHERE CardNo='{0}';", sCardNo);
                        organism_sql += string.Format("DELETE B02_FaceImageData2 WHERE CardNo='{0}';", sCardNo);
                        organism_sql += string.Format("DELETE B02_FVPData WHERE CardNo='{0}';", sCardNo);
                        #endregion

                        oAcsDB.BeginTransaction();
                        istat = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                    }
                    if (istat > -1)
                    {
                        oAcsDB.Commit();
                        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
                        odo.SetPsnDelete(sCardNo);
                        var log = SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.人員權限設定, UserID, UserName, "0103");
                        log.LogInfo = string.Format("Delete Card {0} ",sCardNo);
                        log.LogDesc = "刪除卡號";
                        odo.SetSysLogCreate(log);
                        try
                        {                            
                        }
                        catch (Exception e)
                        {

                        }

                        sRet.message = gms.MsgDelSuccess+"|" + IsConfirm.ToString() + "|";
                        dr = null;
                        sql = " SELECT B01_Card.*, B00_ItemList.ItemName ";
                        sql += " FROM B01_Card INNER JOIN B00_ItemList ";
                        sql += " ON B01_Card.CardType = B00_ItemList.ItemNo ";
                        sql += " WHERE ItemClass = 'CardType' AND PsnID = '" + SelectValue + "' ";
                        oAcsDB.GetDataReader(sql, out dr);
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                sRet.message += dr.DataReader[1].ToString() + "|" + dr.DataReader[18].ToString() + "|" + dr.DataReader[0].ToString() + "|";
                            }
                            sRet.message = sRet.message.Substring(0, sRet.message.Length - 1);
                        }
                    }
                    else
                    {
                        oAcsDB.Rollback();
                        sRet.result = false;
                        sRet.message = gms.MsgDelFailed;
                    }
                }
            }
            else
            {
                sCardNo = oAcsDB.GetStrScalar("SELECT CardNo FROM B01_Card WHERE CardID = " + SelectCardValue);
                if (sCardNo != "")
                {
                    sRet.result = true;
                    sRet.message = "是否確定刪除此卡片[卡號：" + sCardNo + "]？刪除後將立即消碼|" + IsConfirm.ToString() + "|";
                    liSqlPara.Clear();
                    liSqlPara.Add("S:" + sCardNo);
                    sCardNo = oAcsDB.GetStrScalar("SELECT OrigCardNo FROM B01_TempCardRecord WHERE OrigCardNo = ? AND ReturnTime IS NULL",liSqlPara);
                    if (sCardNo!=null && sCardNo != "")
                    {
                        sRet.result = false;
                        sRet.message = "無法刪除，此卡片[卡號：" + sCardNo + "]已借用臨時卡。";
                    }
                }
                else
                {
                    sRet.result = false;
                    sRet.message = "刪除失敗，此卡片[卡號：" + sCardNo + "]已被刪除。";
                }
                

            }
            sRet.act = "Delete";
            return sRet;
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
                B01_EquData.Floor, B01_EquData.EquClass, '' AS CardRule FROM B00_ManageArea
                LEFT JOIN B01_MgnEquGroup ON B00_ManageArea.MgaID = B01_MgnEquGroup.MgaID
                INNER JOIN B01_EquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroup.EquGrpID
                INNER JOIN B01_EquGroupData ON B01_EquGroup.EquGrpID = B01_EquGroupData.EquGrpID
                LEFT JOIN B01_EquData ON B01_EquGroupData.EquID = B01_EquData.EquID
                LEFT JOIN B00_SysUserMgns ON B00_ManageArea.MgaID = B00_SysUserMgns.MgaID
                WHERE UserID = ? ";
            liSqlPara.Add("S:" + hUserId.Value);

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
            oAcsDB.GetDataReader(sql, out dr);

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

            sql = @" SELECT DISTINCT(B01_EquData.EquID) AS 'EquID', B01_EquData.EquNo, B01_EquData.EquName,B01_EquData.EquModel,
                B01_EquData.Floor, B01_EquData.EquClass, '' AS CardRule FROM B00_ManageArea
                LEFT JOIN B01_MgnEquGroup ON B00_ManageArea.MgaID = B01_MgnEquGroup.MgaID
                INNER JOIN B01_EquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroup.EquGrpID
                INNER JOIN B01_EquGroupData ON B01_EquGroup.EquGrpID = B01_EquGroupData.EquGrpID
                LEFT JOIN B01_EquData ON B01_EquGroupData.EquID = B01_EquData.EquID
                LEFT JOIN B00_SysUserMgns ON B00_ManageArea.MgaID = B00_SysUserMgns.MgaID
                WHERE UserID = ? ";
            #endregion

            //oAcsDB.GetDataTable("DataTable", sql, liSqlPara, out dt);
            //GirdViewDataBind(GridView1, dt);
        }
        #endregion

        #region 排序欄位及條件
        public string SortExpression
        {
            get
            {
                if (ViewState["SortExpression"] == null)
                {
                    ViewState["SortExpression"] = "SortCode";
                }
                return ViewState["SortExpression"].ToString();
            }
            set
            {
                ViewState["SortExpression"] = value;
            }
        }

        public string SortDire
        {
            get
            {
                if (ViewState["SortDire"] == null)
                {
                    ViewState["SortDire"] = " DESC ";
                }
                return ViewState["SortDire"].ToString();
            }
            set
            {
                ViewState["SortDire"] = value;
            }
        }
        #endregion

        #endregion
      
    }
}