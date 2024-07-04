using Sa.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using SahoAcs.DBClass;
using System.Web.UI.WebControls;

namespace SahoAcs.Web
{
    public partial class PersonDateChangeCloud : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;     //宣告Ajax元件
        #endregion

        #region 網頁前置作業

        #region LoadProcess
        private void LoadProcess()
        {
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            //oScriptManager.RegisterAsyncPostBackControl(QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nDefault('" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("PersonDateChange", "PersonDateChange.js");                                // 加入同一頁面所需的 JavaScript 檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");               // 加入頁面共用的 JavaScript 檔案
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");               // 加入頁面共用的 JavaScript 檔案
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");           // 加入頁面共用的 JavaScript 檔案
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");     // 加入搭配 GridView 顯示光棒用的 JavaScript 檔案

            Input_Time.SetWidth(180);
        }
        #endregion

        #region RegisterObj
        private void RegisterObj()
        {
            #region 設定彈跳視窗
            Pub.SetModalPopup(ModalPopupExtender1, 1);
            #endregion

            #region 註冊主頁Button動作
            btSelectData.Attributes["onClick"] = "ChangeText(L_popName1, '" +
                this.GetLocalResourceObject("SelectData_Title") + "'); PopupTrigger1.click(); return false;";
            btExec.Attributes["onClick"] = "ExecProcData(); return false;";
            #endregion

            #region 註冊pop1頁Button動作
            ImgCloseButton1.Attributes["onClick"] = "CancelTrigger1.click(); return false;";
            popB_OK1.Attributes["onClick"] = "LoadPsnDataList(); return false;";
            popB_Cancel1.Attributes["onClick"] = "CancelTrigger1.click(); return false;";
            popB_Enter1.Attributes["onClick"] = "DataEnterRemove('Add'); return false;";
            popB_Remove1.Attributes["onClick"] = "DataEnterRemove('Del'); return false;";
            popB_Query.Attributes["onClick"] = "QueryPsnData(); return false;";
            #endregion

            Input_Time.SetWidth(180);
        }
        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null)
            {
                hUserId.Value = Session["UserID"].ToString();
                LoadProcess();
                RegisterObj();
                if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
                {
                    //hSelectState.Value = "true";
                    //Query(true);
                    //this.time
                    this.Input_Time.DateValue = this.GetZoneTime().ToString("yyyy/MM/dd HH:mm:ss");
                }
                else
                {
                    string sFormTarget = Request.Form["__EVENTTARGET"];
                    string sFormArg = Request.Form["__EVENTARGUMENT"];
                    if (!string.IsNullOrEmpty(sFormArg))
                    {
                        //if (sFormArg == "popPagePost") //進行因應新增或編輯後所需的換頁動作
                        //{
                        //    int find = Query("popPagePost");
                        //    Sa.Web.Fun.RunJavaScript(this, "__doPostBack('ctl00$ContentPlaceHolder1$MainGridView', 'Page$" + find + "');");
                        //}
                        //else if (sFormArg.Substring(0, 5) == "Page$") //換頁完成後進行GridViewRow的移動
                        //{
                        //    Query("popPagePost");
                        //    Sa.Web.Fun.RunJavaScript(this, "GridSelect();");
                        //}
                        //else if (sFormArg == "NewQuery")
                        //{
                        //    hSelectState.Value = "true";
                        //    Query(true);
                        //}
                    }
                    else
                    {
                        //hSelectState.Value = "false";
                        //Query(false);
                    }
                }
            }
            else
            {
                Response.Redirect("~/Web/MessagePage/LoginSorry.aspx");
            }
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

        #region JavaScript及aspx共用方法

        #region 載入人員資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object QueryPsnData(String sQueryStr, String UserID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string sql = "", wheresql = "";
            DBReader dr = null;
            List<String> CheckData = new List<String>();
            List<string> liSqlPara = new List<string>();
            String[] EditData = null;

            #region Process String
            sql = " SELECT DISTINCT(OrgStrucID),OrgStrucNo FROM ";
            sql += " (SELECT B00_SysUserMgns.UserID, B00_SysUserMgns.MgaID, B01_MgnOrgStrucs.OrgStrucID, B01_OrgStruc.OrgStrucNo, B01_OrgStruc.OrgIDList ";
            sql += " FROM B00_SysUserMgns INNER JOIN ";
            sql += " B01_MgnOrgStrucs ON B00_SysUserMgns.MgaID = B01_MgnOrgStrucs.MgaID INNER JOIN ";
            sql += " B01_OrgStruc ON B01_MgnOrgStrucs.OrgStrucID = B01_OrgStruc.OrgStrucID)a ";
            sql += " WHERE a.UserID = '" + UserID + "' ";
            oAcsDB.GetDataReader(sql, out dr);
            if (dr.HasRows)
            {
                wheresql = " (";
                while (dr.Read())
                    wheresql += "B01_Person.OrgStrucID = " + dr.DataReader["OrgStrucID"].ToString() + " OR ";
                wheresql = wheresql.Substring(0, wheresql.Length - 4);
                wheresql += ") ";
            }

            sql = " SELECT DISTINCT TOP 100 (B01_Person.PsnID), B01_Person.PsnNo, B01_Person.PsnName, B01_Person.PsnEName, B01_Person.PsnType,  ";
            sql += " B01_Person.IDNum, B01_Person.Birthday, B01_Person.OrgStrucID, B01_Person.PsnAccount, B01_Person.PsnPW,  ";
            sql += " B01_Person.PsnAuthAllow, B01_Person.PsnSTime, B01_Person.PsnETime, B01_Person.PsnPicSource, B01_Person.Remark,  ";
            sql += " B01_Person.CreateUserID, B01_Person.CreateTime, B01_Person.UpdateUserID, B01_Person.UpdateTime, B01_Person.Rev01, ";
            sql += " B01_Person.Rev02, OrgStrucAllData_1.OrgStrucNo, OrgStrucAllData_1.OrgNameList, OrgStrucAllData_1.OrgNoList ";
            sql += " FROM B01_Person INNER JOIN ";
            sql += " OrgStrucAllData('') AS OrgStrucAllData_1 ON B01_Person.OrgStrucID = OrgStrucAllData_1.OrgStrucID LEFT OUTER JOIN ";
            sql += " B01_Card ON B01_Person.PsnID = B01_Card.PsnID ";

            if (!string.IsNullOrEmpty(sQueryStr))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " ( PsnNo LIKE ? OR PsnName LIKE ? OR PsnEName LIKE ? OR CardNo LIKE ? OR OrgNameList LIKE ? OR OrgNoList LIKE ? ) ";
                liSqlPara.Add("S:" + '%' + sQueryStr + '%');
                liSqlPara.Add("S:" + '%' + sQueryStr + '%');
                liSqlPara.Add("S:" + '%' + sQueryStr + '%');
                liSqlPara.Add("S:" + '%' + sQueryStr + '%');
                liSqlPara.Add("S:" + '%' + sQueryStr + '%');
                liSqlPara.Add("S:" + '%' + sQueryStr + '%');
            }

            if (wheresql != "")
                sql += " WHERE ";

            sql += wheresql + " ORDER BY PsnNo ";
            #endregion

            dr = null;
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.HasRows)
            {
                EditData = new string[4];
                while (dr.Read())
                {
                    EditData[0] += dr.DataReader["PsnID"].ToString() + "|";
                    EditData[1] += "[工號:" + dr.DataReader["PsnNo"].ToString() + "]|";
                    EditData[2] += dr.DataReader["PsnName"].ToString() + "|";
                    EditData[3] += dr.DataReader["PsnType"].ToString() + "|";
                }
                EditData[0] = EditData[0].Substring(0, EditData[0].Length - 1);
                EditData[1] = EditData[1].Substring(0, EditData[1].Length - 1);
                EditData[2] = EditData[2].Substring(0, EditData[2].Length - 1);
                EditData[3] = EditData[3].Substring(0, EditData[3].Length - 1);
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "系統查無您所需的資料！";
            }

            return EditData;
        }
        #endregion

        #region 執行期限變更
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object ExecProcData(String sPsnData, String sTimeType, String sDateTime, String UserID)
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
            string sql = "", sql2 = "", sqlexec = "";
            DBReader dr = null;
            string[] EditData = new String[2];
            List<string> liSqlPara = new List<string>();
            int result = 0;
            Queue<String> resultmsg = new Queue<string>();
            Hashtable hPsnData = new Hashtable();
            int CardCnt = 0;
            int PsnCnt = 0;
            String sMsg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "|";
            if (sPsnData != "")
            {
                String[] PsnList = sPsnData.Substring(0, sPsnData.Length - 1).Split('|');

                for (int i = 0; i < PsnList.Length; i += 2)
                    hPsnData.Add(PsnList[i], PsnList[i + 1]);

                #region 取得人員變更前資料
                sql = " SELECT * FROM B01_Person WHERE ";

                foreach (DictionaryEntry obj in hPsnData)
                    sql += " PsnID = " + obj.Key.ToString() + " OR ";

                sql = sql.Substring(0, sql.Length - 4);

                dr = null;
                oAcsDB.GetDataReader(sql, out dr);
                string UtcDateTime = sDateTime;
                Page page = HttpContext.Current.Handler as Page;
                if (page != null)
                {
                    UtcDateTime = DateTime.Parse(sDateTime).GetUtcTime(page).ToString("yyyy/MM/dd HH:mm:ss");
                }
                if (dr.HasRows)
                {                    
                    String msg = " (無異動)";
                    while (dr.Read())
                    {
                        if (sTimeType == "PsnSTime")
                        {
                            if ((DateTime.Parse(dr.DataReader["PsnSTime"].ToString())).ToString("yyyy/MM/dd HH:mm:ss") != UtcDateTime)
                                sMsg += hPsnData[dr.DataReader["PsnID"].ToString()] + " - [啟用時間異動] " + (DateTime.Parse(dr.DataReader["PsnSTime"].ToString())).ToString("yyyy/MM/dd HH:mm:ss") + " -> " + sDateTime + "|";
                            else
                            {
                                sMsg += hPsnData[dr.DataReader["PsnID"].ToString()] + " - [啟用時間異動] " + (DateTime.Parse(dr.DataReader["PsnSTime"].ToString())).ToString("yyyy/MM/dd HH:mm:ss") + " -> " + sDateTime + msg + "|";
                                hPsnData.Remove(dr.DataReader["PsnID"].ToString());
                            }
                        }
                        else
                        {
                            if ((DateTime.Parse(dr.DataReader["PsnETime"].ToString())).ToString("yyyy/MM/dd HH:mm:ss") != UtcDateTime)
                                sMsg += hPsnData[dr.DataReader["PsnID"].ToString()] + " - [停用時間異動] " + (DateTime.Parse(dr.DataReader["PsnETime"].ToString())).ToString("yyyy/MM/dd HH:mm:ss") + " -> " + sDateTime + "|";
                            else
                            {
                                sMsg += hPsnData[dr.DataReader["PsnID"].ToString()] + " - [停用時間異動] " + (DateTime.Parse(dr.DataReader["PsnETime"].ToString())).ToString("yyyy/MM/dd HH:mm:ss") + " -> " + sDateTime + msg + "|";
                                hPsnData.Remove(dr.DataReader["PsnID"].ToString());
                            }
                        }
                    }
                }
                #endregion

                if (hPsnData.Count > 0)
                {
                    sqlexec = " SELECT * FROM B01_Card WHERE ";
                    sql = " UPDATE B01_Person SET " + sTimeType + " = '" + UtcDateTime + "' WHERE  ";
                    sql2 = " UPDATE B01_Card SET " + ((sTimeType == "PsnSTime") ? "CardSTime" : "CardETime") + " = '" + UtcDateTime + "' WHERE ";
                    string sql3 = " UPDATE B01_CardEquAdj SET " + ((sTimeType == "PsnSTime") ? "BeginTime" : "EndTime") + " = '" + UtcDateTime + "' WHERE ";
                    foreach (DictionaryEntry obj in hPsnData)
                    {
                        sql += " PsnID = " + obj.Key.ToString() + " OR ";
                        sqlexec += " PsnID = " + obj.Key.ToString() + " OR ";
                        sql2 += " PsnID = " + obj.Key.ToString() + " OR ";
                        sql3 += " CardID=(SELECT CardID FROM B01_Card WHERE PsnID="+obj.Key.ToString()+")";
                    }
                    sql = sql.Substring(0, sql.Length - 4);
                    sqlexec = sqlexec.Substring(0, sqlexec.Length - 4);
                    sql2 = sql2.Substring(0, sql2.Length - 4);

                    #region 取得要處理人員的所有卡號
                    dr = null;
                    oAcsDB.GetDataReader(sqlexec, out dr);
                    sqlexec = "";
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            sqlexec += " EXEC CardAuth_Update @sCardNo = '" + dr.DataReader["CardNo"].ToString() + "',@sUserID = '" + UserID + "',@sFromProc = 'PersonDateChange',@sFromIP = '" + sIPAddress + "',@sOpDesc = '執行人員期限變更' ; ";
                            CardCnt++;
                        }
                    }
                    #endregion

                    oAcsDB.BeginTransaction();
                    result = oAcsDB.SqlCommandExecute(sql3);
                    result = oAcsDB.SqlCommandExecute(sql2);
                    result = oAcsDB.SqlCommandExecute(sql);
                    if (result > 0)
                    {
                        //紀載處理人員筆數
                        PsnCnt = result;
                        //變更完狀態後須再執行CardAuth_Update
                        result = oAcsDB.SqlCommandExecute(sqlexec);
                        //if (result > 0)
                        //{
                        //    oAcsDB.Commit();
                        //    EditData[0] = "變更成功，人員筆數 " + PsnCnt + " 筆，卡片筆數 " + CardCnt + " 筆";
                        //    EditData[1] = sMsg.Substring(0, sMsg.Length - 1);
                        //}
                        //else
                        //{
                        //    oAcsDB.Rollback();
                        //    EditData[0] = "Saho_SysErrorMassage";
                        //    EditData[1] = "期限變更卡號重整失敗！";
                        //}
                        oAcsDB.Commit();
                        EditData[0] = "變更成功，人員筆數 " + PsnCnt + " 筆，卡片筆數 " + CardCnt + " 筆";
                        EditData[1] = sMsg.Substring(0, sMsg.Length - 1);
                    }
                    else
                    {
                        oAcsDB.Rollback();
                        EditData[0] = "Saho_SysErrorMassage";
                        EditData[1] = "期限變更失敗！";
                    }
                }
                else
                {
                    EditData[0] = "處理完成，人員筆數 " + PsnCnt + " 筆，卡片筆數 " + CardCnt + " 筆";
                    EditData[1] = sMsg + "資料無須異動";
                }
            }
            else
            {
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "無選取資料！";
            }
            return EditData;
        }
        #endregion

        #endregion
    }
}