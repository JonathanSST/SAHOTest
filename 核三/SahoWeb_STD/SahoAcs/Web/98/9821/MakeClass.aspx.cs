using Sa.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DapperDataObjectLib;


namespace SahoAcs
{
    public partial class MakeClass : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        public List<string> ListWeek = new List<string>();
        static string strMsgSelect = "";
        static string strMsgKeyin = "";
        static string strMsgDuplicate = "";
        static string strMsgNonData = "";
        static string strMsgMax = "";

        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            this.ListWeek.Clear();
            this.ListWeek.Add(GetLocalResourceObject("ttMon").ToString());
            this.ListWeek.Add(GetLocalResourceObject("ttTue").ToString());
            this.ListWeek.Add(GetLocalResourceObject("ttWed").ToString());
            this.ListWeek.Add(GetLocalResourceObject("ttThu").ToString());
            this.ListWeek.Add(GetLocalResourceObject("ttFri").ToString());
            this.ListWeek.Add(GetLocalResourceObject("ttSat").ToString());
            this.ListWeek.Add(GetLocalResourceObject("ttSun").ToString());
            #region LoadProcess
            strMsgDuplicate = this.GetLocalResourceObject("msgDuplicate").ToString();
            strMsgSelect = this.GetLocalResourceObject("msgSelect").ToString();
            strMsgKeyin = this.GetLocalResourceObject("msgKeyin").ToString();
            strMsgNonData = this.GetLocalResourceObject("msgNonData").ToString();
            strMsgMax = this.GetLocalResourceObject("msgMax").ToString();
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;            
            oScriptManager.RegisterAsyncPostBackControl(this.DeleteButton);
            oScriptManager.RegisterAsyncPostBackControl(this.AddButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("MakeClass", "MakeClass.js");//加入同一頁面所需的JavaScript檔案

            #region 註冊主頁Button動作
            AddButton.Attributes["onClick"] = "AddExcute();return false;";
            DeleteButton.Attributes["onClick"] = "DeleteExcute();return false;";
            #endregion

            #region 語系切換
            //DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            //oAcsDB.GetTableHash("B00_SysRole", "zhtw", out TableInfo);
            //Label_No.Text = TableInfo["RoleNo"].ToString();
            //Label_Name.Text = TableInfo["RoleName"].ToString();
            //Label_States.Text = TableInfo["RoleState"].ToString();
            //popLabel_No.Text = TableInfo["RoleNo"].ToString();
            //popLabel_Name.Text = TableInfo["RoleName"].ToString();
            //popLabel_EName.Text = TableInfo["RoleEName"].ToString();
            //popLabel_States.Text = TableInfo["RoleState"].ToString();
            //popLabel_Desc.Text = TableInfo["RoleDesc"].ToString();
            //popLabel_Remark.Text = TableInfo["Remark"].ToString();
            #endregion

            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                #region Give hideValue
                hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                #endregion

                CreateHolidayList();
            }
            else
            {
                string sFormTarget = Request.Form["__EVENTTARGET"];
                string sFormArg = Request.Form["__EVENTARGUMENT"];

                if (sFormTarget == this.AddButton.ClientID)
                {
                    CreateHolidayList();
                }

                if (sFormTarget == this.DeleteButton.ClientID)
                {
                    CreateHolidayList();
                }
            }
            if (Request["Event"] != null && Request["Event"].ToString() == "SetCode")
            {
                this.SetHolidayCode();
            }
        }
        #endregion

        #endregion

        #region Method

        #region CreateHolidayList
        private void CreateHolidayList()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            TableRow tr;
            TableCell td;
            string sql = "";
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            #region Process String
            sql = @" SELECT * FROM B00_SysParameter
                     WHERE ParaNo='Workday' ";
            #endregion

            oAcsDB.GetDataReader(sql, out dr);

            if (dr.Read())
            {
                int i = 0;
                tr = new TableRow();
                #region ProcessParaValue
                string ParaValueStr = dr.DataReader["ParaValue"].ToString();
                tr = new TableRow();
                while (ParaValueStr.Length > 0)
                {
                    if (i % 10 == 0)
                    {
                        tr = new TableRow();
                        i = 0;
                    }
                    td = new TableCell();
                    td.Text =this.GetLocalResourceObject("mon"+ParaValueStr.Substring(0, 2)).ToString()
                        + int.Parse(ParaValueStr.Substring(2, 2)) + this.GetLocalResourceObject("dayTh").ToString()+","+ParaValueStr.Substring(4,2);
                    td.ID = "HolidayTD" + ParaValueStr.Substring(0, 6);
                    td.Attributes.Add("OnClick", "onSelect('" + td.ClientID + "','" + ParaValueStr.Substring(0, 6) + "')");
                    ParaValueStr = ParaValueStr.Substring(6);
                    td.Style.Add("white-space", "nowrap");
                    td.Style.Add("backcolor", "#FFFFFF");
                    td.Style.Add("Padding", "8px");
                    td.Style.Add("Width", "70px");
                    tr.Controls.Add(td);
                    this.HolidayTable.Controls.Add(tr);
                    i++;
                }
                #endregion
            }
            else
            {
                tr = new TableRow();
                td = new TableCell();
                td.Text = this.GetLocalResourceObject("ttNonData").ToString();
                td.Width = 750;
                tr.Controls.Add(td);
                this.HolidayPanel.Controls.Add(tr);
            }
            HolidayTable.Style.Add("word-break", "break-all");
            HolidayTable.Attributes.Add("border", "1");
            HolidayTable.Style.Add("border-color", "#999999");
            HolidayTable.Style.Add("border-style", "solid");
            HolidayTable.Style.Add("border-collapse", "collapse");

            this.HolidayUpdatePanel.Update();
        }
        #endregion

        #region Add
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Add(string UserID, string ParaValue,string WorkDay)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            List<string> ParaValueList = new List<string>();
            Sa.DB.DBReader dr;

            if (!string.IsNullOrEmpty(ParaValue))
                ParaValue = ParaValue.Replace("/", "").Substring(4, 4)+WorkDay;

            #region Get HolidayString
            sql = @" SELECT * FROM B00_SysParameter
                     WHERE ParaNo='Workday' ";
            #endregion

            oAcsDB.GetDataReader(sql, out dr);

            if (dr.Read())
            {
                string ParaValueStr = dr.DataReader["ParaValue"].ToString();
                while (ParaValueStr.Length > 0)
                {
                    ParaValueList.Add(ParaValueStr.Substring(0, 6));
                    ParaValueStr = ParaValueStr.Substring(6);
                }

                objRet = Check_Input_DB(ParaValueList, ParaValue);

                if (objRet.result)
                {
                    ParaValueList.Add(ParaValue);
                    ParaValueList = RemoveRepeat(ParaValueList);
                    ParaValueList.Sort();
                    ParaValue = "";
                    for (int i = 0; i < ParaValueList.Count; i++)
                    {
                        ParaValue += ParaValueList[i];
                    }

                    #region 新增系統假日資料
                    if (objRet.result)
                    {
                        #region Process String - Updata Holiday
                        sql = @" UPDATE B00_SysParameter
                                 SET ParaValue = ?, UpdateUserID = ?, UpdateTime = ?
                                 WHERE ParaNo = 'Workday' ";
                        liSqlPara.Add("S:" + ParaValue.Trim());
                        liSqlPara.Add("S:" + UserID.Trim());
                        liSqlPara.Add("D:" + Time);
                        #endregion

                        oAcsDB.SqlCommandExecute(sql, liSqlPara);
                    }
                    #endregion
                }
            }
            else
            {
                objRet.result = false;
                objRet.message += strMsgNonData;
            }

            objRet.act = "Add";
            return objRet;
        }
        #endregion

        #region Delete
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object Delete(string SelectValue)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "", ParaValue = "";
            DateTime Time = DateTime.Now;
            List<string> liSqlPara = new List<string>();
            List<string> ParaValueList = new List<string>();
            string[] SelectValueArray;
            Sa.DB.DBReader dr;

            #region Get HolidayString
            sql = @" SELECT * FROM B00_SysParameter
                     WHERE ParaNo='Workday' ";
            #endregion
            oAcsDB.GetDataReader(sql, out dr);

            if (dr.Read())
            {
                string ParaValueStr = dr.DataReader["ParaValue"].ToString();
                while (ParaValueStr.Length > 0)
                {
                    ParaValueList.Add(ParaValueStr.Substring(0, 6));
                    ParaValueStr = ParaValueStr.Substring(6);
                }

                if (string.IsNullOrEmpty(SelectValue.Trim()))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += strMsgSelect;
                }
                else
                {
                    SelectValueArray = SelectValue.Split(',');                    
                    #region 刪除系統假日
                    foreach (var s in SelectValueArray)
                    {
                        ParaValueList.Remove(s);
                    }
                    /*
                    for (int i = ParaValueList.Count - 1; i >= 0; i--)
                    {
                        for (int k = 0; k < SelectValueArray.Length; k++)
                        {
                            if (ParaValueList[i] == SelectValueArray[k])
                                ParaValueList.RemoveAt(i);
                        }
                    }
                     */
                    #endregion
                    ParaValue = string.Join("", ParaValueList);
                    /*
                    for (int i = 0; i < ParaValueList.Count; i++)
                    {
                        ParaValue += ParaValueList[i];
                    }
                    */
                    #region 更新系統假日資料
                    #region Process String - Delete SysHoliday
                    sql = @" UPDATE B00_SysParameter
                         SET ParaValue = ?
                         WHERE ParaNo = 'Workday' ";
                    liSqlPara.Add("S:" + ParaValue);
                    #endregion
                    oAcsDB.SqlCommandExecute(sql, liSqlPara);

                    #endregion
                }
            }

            objRet.act = "Delete";
            return objRet;
        }
        #endregion

        #region Check_Input_DB
        protected static Pub.MessageObject Check_Input_DB(List<string> ParaValueList, string ParaValue)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();

            #region Input
            if (string.IsNullOrEmpty(ParaValue.Trim()))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += strMsgKeyin;
            }
            #endregion

            #region DB
            if (ParaValueList.Count >= 50)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += strMsgMax;
            }

            for (int i = 0; i < ParaValueList.Count; i++)
            {
                if (ParaValueList[i] == ParaValue)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += strMsgDuplicate;
                }
            }
            #endregion

            return objRet;
        }
        #endregion

        #region RemoveRepeat
        /// <summary>
        /// 移除重複數值
        /// </summary>
        /// <param name="ProcessList">傳入處理List</param>
        /// <returns></returns>
        public static List<string> RemoveRepeat(List<string> ProcessList)
        {
            for (int i = 0; i < ProcessList.Count; i++)
            {
                for (int j = ProcessList.Count - 1; j > i; j--)
                {
                    if (ProcessList[i] == ProcessList[j])
                    {
                        ProcessList.RemoveAt(j);
                    }
                }
            }
            return ProcessList;
        }
        #endregion

        private void SetHolidayCode()
        {
            OrmDataObject orm = new OrmDataObject("MsSql", 
                string.Format(Pub.db_connection_template, Pub.db_source_ip, Pub.db_data_name, Pub.db_user_id, Pub.db_pwd));
            string str_cmd1 = @"INSERT INTO B01_EquParaData (EquID,EquParaID,ParaValue,IsReSend,ErrCnt)
                                                SELECT A.EquID,C.EquParaID,DefaultValue,0 AS IsReSend,0 AS ErrCnt
                                                FROM 
	                                                B01_EquData A
	                                                INNER JOIN B01_EquParaDef C ON A.EquModel=C.EquModel AND ParaName IN ('HolidayMode','HolidayData')
	                                                LEFT JOIN B01_EquParaData B ON A.EquID=B.EquID AND A.EquID=B.EquID AND C.EquParaID=B.EquParaID
	                                                WHERE B.EquParaID IS NULL";
            orm.Execute(str_cmd1);
            string str_cmd2 = @"UPDATE B01_EquParaData 
                                                            SET ErrCnt=0,IsReSend=1,ParaValue='Y',UpdateUserID='Saho',UpdateTime=GETDATE()
                                                            WHERE 
                                                            EquParaID IN (SELECT EquParaID FROM B01_EquParaDef WHERE ParaName IN ('HolidayMode','HolidayData')) 
                                                            AND EquID IN (SELECT EquID FROM B01_EquData WHERE EquModel NOT LIKE 'ADM100%' OR EquModel NOT LIKE 'SST9500%')";
            orm.Execute(str_cmd2);
            string str_cmd3 = @"UPDATE B01_EquParaData 
                                                            SET ErrCnt=0,IsReSend=1,ParaValue='00',UpdateUserID='Saho',UpdateTime=GETDATE()
                                                            WHERE 
                                                            EquParaID IN (SELECT EquParaID FROM B01_EquParaDef WHERE ParaName ='HolidayMode') 
                                                            AND EquID IN (SELECT EquID FROM B01_EquData WHERE EquModel LIKE 'ADM100%' OR EquModel LIKE 'SST9500%')";
            string str_cmd4 = @"UPDATE B01_EquParaData 
                                                            SET ErrCnt=0,IsReSend=1,ParaValue='Y',UpdateUserID='Saho',UpdateTime=GETDATE()
                                                            WHERE 
                                                            EquParaID IN (SELECT EquParaID FROM B01_EquParaDef WHERE ParaName = 'HolidayData') 
                                                            AND EquID IN (SELECT EquID FROM B01_EquData WHERE EquModel LIKE 'ADM100%' OR EquModel LIKE 'SST9500%')";
            orm.Execute(str_cmd3);
            orm.Execute(str_cmd4);

            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { Message="Complete",Success=true }));
            Response.End();
        }


        #endregion
    }
}