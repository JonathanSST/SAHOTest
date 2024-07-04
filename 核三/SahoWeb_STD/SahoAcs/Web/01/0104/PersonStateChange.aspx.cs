using Sa.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;

namespace SahoAcs.Web
{
    public partial class PersonStateChange : System.Web.UI.Page
    {
        private static string strUserId = "";
        private static string strUserName = "";

        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;     //宣告Ajax元件
        #endregion

        static string non_data = "";

        #region 網頁前置作業

        #region LoadProcess
        private void LoadProcess()
        {

            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            //oScriptManager.RegisterAsyncPostBackControl(QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js = "<script src='PersonStateChange.js?" + Pub.GetNowTime+ "'></script><script type='text/javascript'>" + js + "</script>";
            

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            //ClientScript.RegisterClientScriptInclude("PersonStateChange", "PersonStateChange.js");// 加入同一頁面所需的 JavaScript 檔案
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

        }
        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
           
            non_data = GetGlobalResourceObject("Resource", "NonData").ToString();
            if (Session["UserID"] != null)
            {
                hUserId.Value = Session["UserID"].ToString();
                strUserId = Session["UserID"].ToString();
                strUserName = Session["UserName"].ToString();

                LoadProcess();
                RegisterObj();
                if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
                {
                    //hSelectState.Value = "true";
                    //Query(true);
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
        public static object QueryPsnData(String sQueryStr, String UserID,String  poptype)
        {
           // string test = dlltype;
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

            sql = " SELECT DISTINCT top 100  (B01_Person.PsnID), B01_Person.PsnNo, B01_Person.PsnName, B01_Person.PsnEName, B01_Person.PsnType,  ";
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
            sql += "PsnAuthAllow="+ poptype+" and ";
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
                EditData[1] = non_data + "!!";//"系統查無您所需的資料！";                
            }

            return EditData;
        }
        #endregion

        #region 執行權限狀態變更
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object ExecProcData(String sPsnData, String sStateType, String UserID)
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
            int result2 = 0;
            Queue<String> resultmsg = new Queue<string>();
            Hashtable hPsnData = new Hashtable();
            Hashtable hPsnState = new Hashtable();
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
                if (dr.HasRows)
                {
                    String statetype = "", msg = " (無異動)";
                    while (dr.Read())
                    {
                        if (sStateType == "1")
                            statetype = "有效";
                        else
                            statetype = "無效";

                        if (dr.DataReader["PsnAuthAllow"].ToString() == "0")
                        {
                            if (dr.DataReader["PsnAuthAllow"].ToString() == sStateType)
                            {
                                statetype += msg;
                                sMsg += hPsnData[dr.DataReader["PsnID"].ToString()] + " - 無效 -> " + statetype + "|";
                                hPsnData.Remove(dr.DataReader["PsnID"].ToString());
                            }
                            else
                                sMsg += hPsnData[dr.DataReader["PsnID"].ToString()] + " - 無效 -> " + statetype + "|";
                            hPsnState.Add(dr.DataReader["PsnID"].ToString(), "無效 -> " + statetype);
                        }
                        else
                        {
                            if (dr.DataReader["PsnAuthAllow"].ToString() == sStateType)
                            {
                                statetype += msg;
                                sMsg += hPsnData[dr.DataReader["PsnID"].ToString()] + " - 有效 -> " + statetype + "|";
                                hPsnData.Remove(dr.DataReader["PsnID"].ToString());
                            }
                            else
                                sMsg += hPsnData[dr.DataReader["PsnID"].ToString()] + " - 有效 -> " + statetype + "|";
                            hPsnState.Add(dr.DataReader["PsnID"].ToString(), "有效 -> " + statetype);
                        }
                    }
                }
                #endregion

                if (hPsnData.Count > 0)
                {
                    sqlexec = " SELECT [CardNo] FROM [B01_Card] WHERE ";
                    sql =  " UPDATE B01_Person SET PsnAuthAllow = '" + sStateType + "' WHERE ";
                    sql2 = " UPDATE B01_Card SET CardAuthAllow = '" + sStateType + "' WHERE ";

                    List<string> liPara = new List<string>();

                    foreach (DictionaryEntry obj in hPsnData)
                    {
                        sqlexec += " [PsnID] = ? OR ";
                        liPara.Add("I:" + obj.Key.ToString());

                        sql += " PsnID = " + obj.Key.ToString() + " OR ";
                        sql2 += " PsnID = " + obj.Key.ToString() + " OR ";
                    }
                    sql = sql.Substring(0, sql.Length - 4);
                    sqlexec = sqlexec.Substring(0, sqlexec.Length - 4);
                    sql2 = sql2.Substring(0, sql2.Length - 4);

                    try
                    {
                        result2 = oAcsDB.SqlCommandExecute(sql2, liPara);
                        result = oAcsDB.SqlCommandExecute(sql, liPara);

                        //變更完狀態後須再執行CardAuth_Update
                        if (result != -1 && result2 != -1)
                        {
                            //紀載處理人員筆數
                            PsnCnt = result;

                            #region 取得要處理人員的所有卡號並執行 CardAuth_Update

                            dr = new DBReader();
                            bool IsOK = oAcsDB.GetDataReader(sqlexec, liPara, out dr);

                            if (IsOK)
                            {
                                int intRN = 0;

                                if (dr.HasRows)
                                {
                                    while (dr.Read())
                                    {
                                        string strCardNo = dr.DataReader["CardNo"].ToString();
                                        string strSQL = "CardAuth_Update";
                                        liSqlPara.Clear();
                                        liSqlPara.Add("S:" + strCardNo);
                                        liSqlPara.Add("S:" + UserID);
                                        liSqlPara.Add("S:PersonStateChange");
                                        liSqlPara.Add("S:" + sIPAddress);
                                        liSqlPara.Add("S:執行人員權限狀態變更");

                                        intRN = oAcsDB.SqlProcedureExecute(strSQL, liSqlPara);

                                        if (intRN != -1)
                                        {
                                            CardCnt++;

                                            // syslog
                                            oAcsDB.WriteLog(DB_Acs.Logtype.卡片權限調整, strUserId, strUserName, "0104", "", "", string.Format("卡片「{0}」卡片權限重整成功",strCardNo), "卡片權限重整");
                                        }
                                        else
                                        {
                                            oAcsDB.Rollback();
                                            EditData[0] = "Saho_SysErrorMassage";
                                            EditData[1] = "權限狀態變更失敗！";
                                            break;
                                        }
                                    }

                                    foreach (DictionaryEntry obj in hPsnData)
                                    {
                                        oAcsDB.WriteLog(DB_Acs.Logtype.資料修改, strUserId, strUserName, "0104", "", "", string.Format("PsnId：{0}，PsnAuthAllow：{1}", obj.Key.ToString(), sStateType), "B01_Person");

                                        oAcsDB.WriteLog(DB_Acs.Logtype.資料修改, strUserId, strUserName, "0104", "", "", string.Format("PsnId：{0}，CardAuthAllow：{1}", obj.Key.ToString(), sStateType), "B01_Card");
                                    }
                                }

                                #endregion

                                #region //comment out old code//
                                //if (result > 0)
                                //{
                                //    oAcsDB.Commit();
                                //    EditData[0] = "處理完成，人員筆數 " + PsnCnt + " 筆，卡片筆數 " + CardCnt + " 筆";
                                //    EditData[1] = sMsg.Substring(0, sMsg.Length - 1);
                                //}
                                //else
                                //{
                                //    oAcsDB.Rollback();
                                //    EditData[0] = "Saho_SysErrorMassage";
                                //    EditData[1] = "權限狀態變更卡號重整失敗！";
                                //}
                                #endregion

                                EditData[0] = "處理完成，人員筆數 " + PsnCnt + " 筆，卡片筆數 " + CardCnt + " 筆";
                                EditData[1] = sMsg.Substring(0, sMsg.Length - 1);
                            }
                            else
                            {
                                EditData[0] = "Saho_SysErrorMassage";
                                EditData[1] = "權限狀態變更失敗！";
                            }
                        }
                        else
                        {
                            EditData[0] = "Saho_SysErrorMassage";
                            EditData[1] = "權限狀態變更失敗！";
                        }
                    }
                    catch (Exception ex )
                    {
                        EditData[0] = "Saho_SysErrorMassage";
                        EditData[1] = ex.Message.ToString();
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