using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Threading;
using SahoAcs.DB.Fileds;
using SahoAcs.DBClass;

namespace SahoAcs
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        DB_Acs oAcsDB = null;
        List<SysMenu> liMenu = new List<SysMenu>();
        string sUserID, sUserPW, sUserName, sOwnerList, sMenuNo, sMenuName, sPsnID;
        public string TimeOnceID = "";
        //-------------------------------------------------------------------------------------------------------------------------------------------

        protected void Page_Init(object sender, EventArgs e)
        {
            //if (Response.Cookies["LogInfo"] != null)
            //{
            //    this.SetLoadInfo();
            //}else
            //{
            //    Response.Redirect("~/Web/MessagePage/LoginSorry.aspx");//Session Timeout
            //}            

            Int32 iLoop;
            oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader oReader = null;

            //HighLight：搭配GridView顯示光棒用
            //bootstrap：前端網頁框架
            //bootstrap-dialog：自訂Alert Show視窗
            Page.ClientScript.RegisterClientScriptInclude("xFun", Request.ApplicationPath.TrimEnd('/')+ "/Scripts/xFun.js");
            Page.ClientScript.RegisterClientScriptInclude("xObj", Request.ApplicationPath.TrimEnd('/')+ "/Scripts/xObj.js");
            Page.ClientScript.RegisterClientScriptInclude("Common", Request.ApplicationPath.TrimEnd('/')+ "/Scripts/Common.js");
            Page.ClientScript.RegisterClientScriptInclude("HighLight", Request.ApplicationPath.TrimEnd('/')+ "/Scripts/HighLight.js");
            Page.ClientScript.RegisterClientScriptInclude("jquery", Pub.JqueyNowVer);
            //Page.ClientScript.RegisterClientScriptInclude("jquery", "/Scripts/jquery-1.9.0.js");
            Page.ClientScript.RegisterClientScriptInclude("jquery-ui", Request.ApplicationPath.TrimEnd('/')+ "/Scripts/jquery-ui.js");
            Page.ClientScript.RegisterClientScriptInclude("blockUI", Request.ApplicationPath.TrimEnd('/')+ "/Scripts/jquery.blockUI.js");
            //Page.ClientScript.RegisterClientScriptInclude("multiselect", "/Scripts/jquery.multiselect.js");
            //Page.ClientScript.RegisterClientScriptInclude("multiselectfilter", "/Scripts/jquery.multiselect.filter.js");
            var JsFile = oAcsDB.GetStrScalar("SELECT ParaValue FROM B00_SysParameter WHERE ParaNo='ImportRecheck' ");
            if (JsFile != null&&JsFile!="")
            {
                //Page.ClientScript.RegisterClientScriptInclude("ImportRecheck", JsFile);
                string jsFileEnd = "<script src=\""+JsFile+"\" Type=\"text/javascript\"></script>";
                Page.ClientScript.RegisterStartupScript(typeof(string), "ImportRecheck", jsFileEnd, false);
            }
            //Page.ClientScript.RegisterClientScriptInclude("bootstrap", "/Scripts/bootstrap.js");
            //Page.ClientScript.RegisterClientScriptInclude("BootstrapDialog", "/Scripts/bootstrap-dialog.js");

            Boolean IsOptAllow = false;

            if(Request.Url.PathAndQuery.Contains("0602_1.aspx") && (Session["UserID"] == null || Session["UserID"].ToString() == ""))
            {
                if (oAcsDB.GetDataReader("SELECT * FROM B00_SysUser WHERE UserID='User' ", liSqlPara, out oReader))
                {
                    if (oReader.HasRows)
                    {
                        oReader.Read();
                        Session["UserID"] = "User";
                        Session["UserPW"] = oReader.ToString("UserPW");
                        Session["UserName"] = "一般使用者";
                        Session["MenuNo"] = "";
                        Session["MenuName"] = "";
                        Session["FunAuthSet"] = "";
                        //Response.Redirect("/Web/06/0602/0602_1.aspx");
                    }
                }
            }
            //oAcsDB.WriteLog(DB_Acs.Logtype.功能選單切換, Sa.Web.Fun.GetSessionStr(this.Page, "UserID"), Sa.Web.Fun.GetSessionStr(this.Page, "UserID"), "Login", "", "", "使用者「" + Sa.Web.Fun.GetSessionStr(this.Page, "UserID") + "」必須登出", "查無session");
            if (Sa.Web.Fun.GetSessionStr(this.Page, "UserID") == "")
            {
                //session 過期的處理頁面
                string[] accept = Request.AcceptTypes;
                if (accept.Where(i => i.Contains("json")).Count() > 0)
                {
                    Response.Clear();
                    Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                        new { message = "系統工作階段已過期或被登出", result = false }));
                    Response.End();
                }
                Response.Redirect("~/Web/MessagePage/LoginSorry.aspx");//Session Timeout                
            }
            else
            {
                //oAcsDB.WriteLog(DB_Acs.Logtype.功能選單切換, sUserID, sUserName, "Login", "", "", "使用者「" + sUserName + "」登入", "");
            }

            sUserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            sUserPW = Sa.Web.Fun.GetSessionStr(this.Page, "UserPW");
            sMenuNo = Sa.Web.Fun.GetSessionStr(this.Page, "MenuNo");
            sMenuName = Sa.Web.Fun.GetSessionStr(this.Page, "MenuName");
            sPsnID = Sa.Web.Fun.GetSessionStr(this.Page, "PsnID");

            sUserName = "";
            sOwnerList = "";
            this.SetTitleName();
            string sSqlCmd;
            string[] arrTarget, arrSource;
            

            # region 查驗使用者使用期限及是否暫停使用
            DateTime dtBegin = DateTime.Now.AddDays(-1);
            DateTime dtEnd = DateTime.Now.AddDays(-1);
            
            if (this.sPsnID.Equals(""))
            {
                sSqlCmd = @" SELECT UserID, UserName, OwnerList, UserSTime, UserETime, IsOptAllow FROM B00_SysUser WHERE (UserID = ?) AND (UserPW = ?)";
                liSqlPara.Add("S:" + sUserID);
                liSqlPara.Add("S:" + sUserPW);
            }
            else
            {
                sSqlCmd = @"SELECT B01_Person.PsnID, U.UserID, '['+PsnNo +'] '+ PsnName AS UserName, U.UserSTime, U.UserETime, U.PWChgTime, U.IsOptAllow
                    FROM B01_Person, (SELECT UserID, UserName, UserSTime, UserETime, PWChgTime, IsOptAllow
                    FROM B00_SysUser WHERE UserID = 'User') AS U
                    WHERE B01_Person.PsnID=?";
                liSqlPara.Add("S:" + this.sPsnID);
                //liSqlPara.Add("S:" + sUserID);
                //liSqlPara.Add("S:" + sUserPW);
            }

            

            if (oAcsDB.GetDataReader(sSqlCmd, liSqlPara, out oReader))
            {
                if (oReader.HasRows)
                {
                    oReader.Read();
                    sUserName = oReader.ToString("UserName");
                    sOwnerList = oReader.ToString("OwnerList");
                    dtBegin = oReader.ToDateTime("UserSTime").GetZoneTime(this.Page);
                    dtEnd = oReader.ToDateTime("UserETime").GetZoneTime(this.Page);
                    IsOptAllow = oReader.ToBoolean("IsOptAllow");
                }

                oReader.Free();
            }
            else
            {
                Response.Redirect("~/Web/MessagePage/DBConnError.aspx");
            }

            if (!IsOptAllow)
            {
                Response.Redirect("~/Web/MessagePage/LoginError2.aspx");//使用者帳號暫停使用
            }
            else
            {
                DateTime dtNow = this.Page.GetZoneTime();
                if (dtEnd == DateTime.MinValue) dtEnd = DateTime.MaxValue;
                if (dtBegin > dtNow || dtEnd < dtNow) Response.Redirect("~/Web/MessagePage/LoginError3.aspx");//未在使用期限內使用
            }
            Session["UserName"] = sUserName;
            Session["OwnerList"] = sOwnerList;
            # endregion

            # region 載入使用者角色操作選單及權限
            liSqlPara.Clear();
            sSqlCmd = @" SELECT Sm.MenuNo, Sm.MenuName, Sm.MenuEName, Sm.MenuDesc, Sm.UpMenuNo, Sm.MenuOrder, Sm.MenuType," +
                " Sm.FunTrackType, Sm.FunTrack, Sm.FunParameter, Sm.FunIcon, Sm.FunTarget, Srm.FunAuthSet, Sm.Rev01," +
                " Sm.Rev02 FROM B00_SysRole AS Sr" +
                " INNER JOIN B00_SysUserRoles AS Sur ON Sr.RoleID = Sur.RoleID" +
                " INNER JOIN B00_SysRoleMenus AS Srm ON Sur.RoleID = Srm.RoleID" +
                " INNER JOIN B00_SysMenu AS Sm ON Srm.MenuNo = Sm.MenuNo" +
                " RIGHT OUTER JOIN B00_SysUser AS Su ON Sur.UserID = Su.UserID" +
                " WHERE Su.IsOptAllow = 1 AND Sr.RoleState = 1 AND Sm.MenuIsUse = 1 AND IsAuthCtrl = 1 AND Su.UserID = ? AND Su.UserPW = ?" +
                " UNION" +
                " SELECT MenuNo, MenuName, MenuEName, MenuDesc, UpMenuNo, MenuOrder, MenuType, FunTrackType, FunTrack," +
                " FunParameter, FunIcon, FunTarget, FunAuthDef, Rev01, Rev02" +
                " FROM B00_SysMenu" +
                " WHERE IsAuthCtrl = 0 ORDER BY MenuNo";
            liSqlPara.Add("S:" + sUserID);
            liSqlPara.Add("S:" + sUserPW);

            if (oAcsDB.GetDataReader(sSqlCmd, liSqlPara, out oReader))
            {
                if (oReader.HasRows)
                {
                    int intCheckNo = 0;
                    string strCheckNo = "";

                    while (oReader.Read())
                    {
                        if (oReader.ToString("MenuType") == "S")
                        {
                            Pub.sTitel = oReader.ToString("MenuName");
                        }
                        else
                        {
                            if (oReader.ToString("MenuNo").Equals(strCheckNo))
                            {
                                //如果MenuNo等於上筆MenuNo表示該使用者有多個Role權限
                                arrTarget = liMenu[intCheckNo].FunAuthSet.Value.Split(',');
                                arrSource = oReader.ToString("FunAuthSet").Split(',');
                                //篩選權限集合將權限集合最大化
                                string strCheck = FunAuthAdj(arrTarget, arrSource, "");
                                liMenu[intCheckNo].FunAuthSet.Value += strCheck;//新增沒有的權限
                            }
                            else
                            {
                                SysMenu oMenu = new SysMenu();
                                oMenu.MenuNo.Value = oReader.ToString("MenuNo");
                                oMenu.MenuName.Value = oReader.ToString("MenuName");
                                oMenu.MenuEName.Value = oReader.ToString("MenuEName");
                                oMenu.MenuDesc.Value = oReader.ToString("MenuDesc");
                                oMenu.UpMenuNo.Value = oReader.ToString("UpMenuNo");
                                oMenu.MenuOrder.Value = oReader.ToInt32("MenuOrder");
                                oMenu.MenuType.Value = oReader.ToString("MenuType");
                                oMenu.FunTrackType.Value = oReader.ToString("FunTrackType");
                                oMenu.FunTrack.Value = oReader.ToString("FunTrack");
                                oMenu.FunParameter.Value = oReader.ToString("FunParameter");
                                oMenu.FunIcon.Value = oReader.ToString("FunIcon");
                                oMenu.FunTarget.Value = oReader.ToString("FunTarget");
                                oMenu.FunAuthSet.Value = oReader.ToString("FunAuthSet");
                                oMenu.Rev01.Value = oReader.ToInt32("Rec01");
                                oMenu.Rev02.Value = oReader.ToString("Rec02");
                                liMenu.Add(oMenu);
                                strCheckNo = oReader.ToString("MenuNo");//記錄本次新增的MenuNo當做下筆資料判斷的依據
                                intCheckNo = liMenu.Count - 1;//記錄當下liMenu最大的Count才知道要更新那筆資料
                            }
                        }
                    }
                }
                oReader.Free();
            }
            else
            {
                Response.Redirect("~/Web/MessagePage/DBConnError.aspx");
            }
            # endregion

            # region 載入使用者指定操作選單及權限
            liSqlPara.Clear();
            sSqlCmd = @" SELECT Sm.MenuNo, Sm.MenuName, Sm.MenuEName, Sm.MenuDesc, Sm.UpMenuNo, Sm.MenuOrder, Sm.MenuType,
                Sm.FunTrackType, Sm.FunTrack, Sm.FunParameter, Sm.FunIcon, Som.FunAuthSet, Sm.FunTarget, Som.OPMode,
                Sm.Rev01, Sm.Rev02 FROM B00_SysUser AS Su
                INNER JOIN B00_SysUserMenus AS Som ON Su.UserID = Som.UserID
                INNER JOIN B00_SysMenu AS Sm ON Som.MenuNo = Sm.MenuNo
                WHERE Su.IsOptAllow = 1 AND Sm.MenuIsUse = 1 AND IsAuthCtrl = 1 AND Su.UserID = ? AND Su.UserPW = ?";
            liSqlPara.Add("S:" + sUserID);
            liSqlPara.Add("S:" + sUserPW);

            if (oAcsDB.GetDataReader(sSqlCmd, liSqlPara, out oReader))
            {
                if (oReader.HasRows)
                {
                    int intCheckNo = 0;
                    string _sMenuNo, strCheck;
                    SysMenu oMenu9 = null;

                    while (oReader.Read())
                    {
                        oMenu9 = null;
                        strCheck = "";
                        _sMenuNo = oReader.ToString("MenuNo");

                        for (iLoop = 0; iLoop < liMenu.Count; iLoop++)
                        {
                            if (liMenu[iLoop].MenuNo.Value == _sMenuNo)
                            {
                                oMenu9 = liMenu[iLoop];
                                intCheckNo = iLoop;
                                break;
                            }
                        }

                        if (oReader.ToString("OPMode") == "-" && oMenu9 != null)
                        {
                            arrTarget = liMenu[intCheckNo].FunAuthSet.Value.Split(',');
                            arrSource = oReader.ToString("FunAuthSet").Split(',');
                            strCheck = FunAuthAdj(arrTarget, arrSource, oReader.ToString("OPMode"));

                            if (strCheck.Equals("") || strCheck == null)
                            {
                                liMenu.Remove(oMenu9);//減少權限回傳為null則表示該選單不可使用
                            }
                            else
                            {
                                liMenu[intCheckNo].FunAuthSet.Value = strCheck;//減少權限
                            }
                        }
                        else if (oReader.ToString("OPMode") == "+")
                        {
                            if (oMenu9 == null)
                            {
                                SysMenu oMenu = new SysMenu();
                                oMenu.MenuNo.Value = oReader.ToString("MenuNo");
                                oMenu.MenuName.Value = oReader.ToString("MenuName");
                                oMenu.MenuEName.Value = oReader.ToString("MenuEName");
                                oMenu.MenuDesc.Value = oReader.ToString("MenuDesc");
                                oMenu.UpMenuNo.Value = oReader.ToString("UpMenuNo");
                                oMenu.MenuOrder.Value = oReader.ToInt32("MenuOrder");
                                oMenu.MenuType.Value = oReader.ToString("MenuType");
                                oMenu.FunTrackType.Value = oReader.ToString("FunTrackType");
                                oMenu.FunTrack.Value = oReader.ToString("FunTrack");
                                oMenu.FunParameter.Value = oReader.ToString("FunParameter");
                                oMenu.FunIcon.Value = oReader.ToString("FunIcon");
                                oMenu.FunTarget.Value = oReader.ToString("FunTarget");
                                oMenu.FunAuthSet.Value = oReader.ToString("FunAuthSet");
                                oMenu.Rev01.Value = oReader.ToInt32("Rec01");
                                oMenu.Rev02.Value = oReader.ToString("Rec02");
                                liMenu.Add(oMenu);
                            }
                            else
                            {
                                arrTarget = liMenu[intCheckNo].FunAuthSet.Value.Split(',');
                                arrSource = oReader.ToString("FunAuthSet").Split(',');
                                strCheck = FunAuthAdj(arrTarget, arrSource, oReader.ToString("OPMode"));
                                liMenu[intCheckNo].FunAuthSet.Value += strCheck;//新增權限
                            }
                        }
                    }
                }
            }
            else
            {
                Response.Redirect("~/Web/MessagePage/DBConnError.aspx");
            }
            # endregion

            if (!IsPostBack)
            {
                if (sMenuNo == "")
                {
                    this.Page.Title = this.GetGlobalResourceObject("Resource", "lblTitle").ToString();
                    if (Request.Cookies["i18n"].Value.ToString() == "en-US")
                    {
                        labProcName.Text = "Please select process..";
                    }
                    else
                    {
                        labProcName.Text = "請選擇執行項目";
                    }

                }
                else
                {
                    this.Page.Title = this.GetGlobalResourceObject("Resource", "lblTitle").ToString() + " - " + sMenuName;
                    //labProcName.Text = sMenuName;
                }
                labUserName.Text = sUserName;
                GenMenu();
                WriteLogAction();       // syslog 相關
                Thread.Sleep(1000);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //先取得目前資料庫位置的本地時間
            var DbLocTime = DateTime.Now.GetUtcTime(this.Page);
            //增加SC狀態驗證功能
            if (!SahoAcs.DBClass.DongleVaries.GetScAliveTime(DbLocTime))
            {
                //導入SC無效
                //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ScAlive", "");
                Response.Redirect("/Web/MessagePage/ScAliveError.aspx");
            }
            if (Request.Url.Authority != (Request.UrlReferrer != null ? Request.UrlReferrer.Authority : ""))
            {
                if (Session["LightID"] != null)
                {
                    var liSqlPara = new List<string>()
                    {
                        "S:" + Sa.Web.Fun.GetSessionStr(this.Page, "UserID"),
                        "S:" + Sa.Web.Fun.GetSessionStr(this.Page, "LightID")
                    };
                    DataTable dt = new DataTable();
                    this.oAcsDB.GetDataTable("", "SELECT * FROM B03_ExtTokenInfo WHERE UserID=? AND LightID=?", liSqlPara, out dt);
                    if (dt.Rows.Count == 0)
                    {
                        Response.Redirect("~/Default.aspx");
                    }
                }
                else
                {
                    Response.Redirect("~/Default.aspx");
                }
            }
            if (Session["TimeOnceID"] != null)
            {
                this.TimeOnceID = Session["TimeOnceID"].ToString();
            }
            if (Request.HttpMethod.Equals("GET") && Request.QueryString.Count > 0)
            {
                if (Request.Url.AbsolutePath.Contains("0209.10/TimeTable.aspx")&&Request.QueryString["Mode"]!=null)
                {
                    
                }
                else if(Request.Url.AbsolutePath.Contains("0611.12.13/QueryDeviceOPLog.aspx") && Request.QueryString["Mode"] != null)
                {
                    
                }
                else
                {
                    Response.Redirect(Request.Url.AbsolutePath);
                }
            }
            #region 過往的程式碼            
            #endregion
        }

        private void SetTitleName()
        {
            string[] aStr;
            string sStr, sTmp = "";
            string sCustomer = "商合行內部專用-禁止外流";
            string current_path = System.AppDomain.CurrentDomain.BaseDirectory;
            string[] aFiles = Directory.GetFiles(current_path, "*.sms", SearchOption.TopDirectoryOnly);
            this.oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            if (aFiles.Length > 0)
            {
                sTmp = File.ReadAllText(aFiles[0]);
                if (sTmp != "")
                {
                    sStr = Sa.Fun.Decrypt(sTmp);
                    if (!string.IsNullOrEmpty(sStr))
                    {
                        aStr = sStr.Split(new char[] { '|' });
                        if (aStr.Length >= 2 && aStr[0] == "Sms")
                        {
                            sCustomer = aStr[1];
                            oAcsDB.GetSysParameter("HideSystem", "CustName", "客戶名稱", "", "", out sStr);
                            if (sStr != sTmp)
                            {
                                oAcsDB.UpdateSysParameter("HideSystem", "CustName", sTmp);
                            }
                        }
                    }
                }
            }
            else
            {
                //oAcsDB.GetSysParameter("HideSystem", "CustName", "客戶名稱", "", "", out sTmp);
                //if (sTmp != "")
                //{
                //    sStr = Sa.Fun.Decrypt(sTmp);
                //    if (!string.IsNullOrEmpty(sStr))
                //    {
                //        aStr = sStr.Split(new char[] { '|' });
                //        if (aStr.Length >= 2 && aStr[0] == "Sms")
                //        {
                //            sCustomer = aStr[1];
                //        }
                //    }
                //}
            }
            var DongleCustTitle = DongleVaries.GetCustTitle();
            if (DongleCustTitle != "")
            {
                sCustomer = DongleCustTitle;
            }
            this.lblTitle.Text = GetGlobalResourceObject("Resource", "lblTitle") + "(" + sCustomer + ")";
            //設定版本
            sTmp = this.Page.GetSysVersion();
            if (sTmp != "")
            {
                this.lblTitle.Text += "-" + sTmp;
            }
        }

        #region 產生功能選單
        private Hashtable htOrder = new Hashtable();

        private void GenMenu()
        {
            string chkMenuInfo = "";
            Int32 iMenuOrder;
            Boolean IsInsert;
            string sText, sValue, sType, sUpMenu, sToolTip, sIcon;
            MainMenu.Items.Clear();
            //this.Page.GetMenuList();
            List<string> MenuList = this.Page.GetMenuList();
            //SahoAcs.DBClass.DongleVaries.GetMenuList();            
            for (Int32 i = 0; i < liMenu.Count; i++)
            {
                sValue = liMenu[i].MenuNo.Value;
                if (sValue != "")
                {
                    if (Request.Cookies["i18n"].Value.ToString() == "en-US")
                    {
                        sText = liMenu[i].MenuEName.Value;
                    }
                    else
                    {
                        sText = liMenu[i].MenuName.Value;
                    }

                    sType = liMenu[i].MenuType.Value;
                    sUpMenu = liMenu[i].UpMenuNo.Value;
                    sToolTip = liMenu[i].MenuDesc.Value;
                    sIcon = liMenu[i].FunIcon.Value;
                    iMenuOrder = liMenu[i].MenuOrder.Value;

                    if (sToolTip == "")
                    {
                        sToolTip = sText;
                    }

                    if (!File.Exists(Server.MapPath(sIcon)))
                    {
                        sIcon = "";
                    }

                    MenuItem oMenu = new MenuItem(sText, sValue);
                    oMenu.ToolTip = sToolTip;

                    // 因應 B00_SysMenu 的 MenuEName 出現空白或資料重複的情形
                    try
                    {
                        htOrder.Add(sToolTip, iMenuOrder.ToString());
                    }
                    catch
                    {

                    }

                    oMenu.ImageUrl = sIcon;
                    oMenu.Target = "_self";

                    if (!string.IsNullOrEmpty(liMenu[i].FunParameter.Value))
                    {
                        oMenu.NavigateUrl = liMenu[i].FunTrack.Value + "?" + liMenu[i].FunParameter.Value;
                    }
                    else
                    {
                        oMenu.NavigateUrl = liMenu[i].FunTrack.Value;
                    }

                    if (sType == "M")
                    {
                        oMenu.Selectable = false;//純粹用來做選單顯示時，即設定為不可選取。
                        oMenu.ToolTip = "";
                        MainMenu.Items.Add(oMenu);
                    }
                    else
                    {
                        oMenu.Selectable = true;

                        foreach (MenuItem oSubMenu in MainMenu.Items)
                        {
                            if (oSubMenu.Value == sUpMenu)
                            {
                                IsInsert = false;
                                for (Int32 j = 0; j < oSubMenu.ChildItems.Count; j++)
                                {
                                    string strOrder = htOrder[oSubMenu.ChildItems[j].ToolTip].ToString();

                                    if (iMenuOrder < Convert.ToInt32(strOrder))
                                    {
                                        if (MenuList.Count>0&&MenuList.Contains(sValue))
                                        {
                                            oSubMenu.ChildItems.AddAt(j, oMenu);
                                            IsInsert = true;
                                            break;
                                        }
                                        if(MenuList.Count==0)
                                        {
                                            oSubMenu.ChildItems.AddAt(j, oMenu);
                                            IsInsert = true;
                                            break;
                                        }
                                        
                                    }
                                }

                                if (!IsInsert)
                                {
                                    if (MenuList.Count > 0 && MenuList.Contains(sValue))
                                    {
                                        oSubMenu.ChildItems.Add(oMenu);
                                    }
                                    if (MenuList.Count == 0)
                                    {
                                        oSubMenu.ChildItems.Add(oMenu);
                                    }
                                }

                                break;
                            }
                            else
                            {
                                if (oSubMenu.ChildItems.Count > 0)
                                {
                                    AddSubMenu(oSubMenu, oMenu, sUpMenu, iMenuOrder);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region 產生子功能選單
        private void AddSubMenu(MenuItem oSubMenu, MenuItem oMenu, string sUpMenu, Int32 iMenuOrder)
        {
            Boolean IsInsert;
            foreach (MenuItem oItemMenu in oSubMenu.ChildItems)
            {
                if (oItemMenu.Value == sUpMenu)
                {
                    IsInsert = false;

                    for (Int32 j = 0; j < oItemMenu.ChildItems.Count; j++)
                    {
                        string strOrder = htOrder[oSubMenu.ChildItems[j].ToolTip].ToString();

                        if (iMenuOrder < Convert.ToInt32(strOrder))
                        {
                            oItemMenu.ChildItems.AddAt(j, oMenu);
                            IsInsert = true;
                            break;
                        }
                    }

                    if (!IsInsert)
                    {
                        oItemMenu.ChildItems.Add(oMenu);
                    }

                    break;
                }
                else
                {
                    if (oItemMenu.ChildItems.Count > 0)
                    {
                        AddSubMenu(oItemMenu, oMenu, sUpMenu, iMenuOrder);
                    }
                }
            }
        }
        #endregion

        private void WriteLogAction()
        {
            int iLoop = 0;
            SysMenu oCurrMenu = new SysMenu();
            string temp_path = "";
            # region 找出選到的選單的項目 oCurrMenu
            for (iLoop = 0; iLoop < liMenu.Count; iLoop++)
            {
                string strCurrentPath = "~" + Request.Url.PathAndQuery;
                if (liMenu[iLoop].FunParameter.Value.Trim() != "")
                {
                    temp_path = liMenu[iLoop].FunTrack.Value + "?" + liMenu[iLoop].FunParameter.Value;
                }else
                {
                    temp_path = liMenu[iLoop].FunTrack.Value;
                }
                if (temp_path.ToLower() == strCurrentPath.ToLower())
                {
                    oCurrMenu = liMenu[iLoop];
                    break;
                }
            }
            #endregion

            if (oCurrMenu != null)
            {
                Session["MenuNo"] = oCurrMenu.MenuNo.Value;
                if (Request.Cookies["i18n"].Value.ToString() == "en-US")
                {
                    Session["MenuName"] = oCurrMenu.MenuEName.Value;
                }
                else
                {
                    Session["MenuName"] = oCurrMenu.MenuName.Value.Equals("")? "請選擇執行項目" : oCurrMenu.MenuName.Value;
                    
                }

                Session["FunAuthSet"] = oCurrMenu.FunAuthSet.Value;

                if (oCurrMenu.FunTrackType.Value == "URL" && oCurrMenu.FunTrack.Value != "")
                {
                    if (File.Exists(Server.MapPath(oCurrMenu.FunTrack.Value)))
                    {
                        oAcsDB.WriteLog(DB_Acs.Logtype.功能選單切換, sUserID, sUserName, oCurrMenu.MenuNo.Value, "", "", "使用者「" + sUserName + "」進入「" + oCurrMenu.MenuName.Value + "」", "");

                        //labProcName.Text = oCurrMenu.MenuName.Value.ToString();
                        
                        this.Page.Title = this.GetGlobalResourceObject("Resource", "lblTitle").ToString() + " - " + labProcName.Text;
                    }
                }
                this.labProcName.Text = Session["MenuName"].ToString();
                this.Page.Title = this.GetGlobalResourceObject("Resource", "lblTitle").ToString() + " - " + labProcName.Text;
            }
            else
            {
                Session["MenuNo"] = "";
                Session["MenuName"] = "";
                Session["FunAuthSet"] = "";
                Response.Redirect("~/MainForm.aspx");
            }
        }

        protected void MainMenu_MenuItemClick(object sender, MenuEventArgs e)
        {
            Int32 iLoop;
            DB.Fileds.SysMenu oCurrMenu = null;

            # region 找出選到的選單的項目 oCurrMenu
            for (iLoop = 0; iLoop < liMenu.Count; iLoop++)
            {
                if (liMenu[iLoop].MenuNo.Value == e.Item.Value)
                {
                    oCurrMenu = liMenu[iLoop];
                    break;
                }
            }
            # endregion

            if (oCurrMenu != null)
            {
                Session["MenuNo"] = oCurrMenu.MenuNo.Value;
                if (Request.Cookies["i18n"].Value.ToString() == "en-US")
                {
                    Session["MenuName"] = oCurrMenu.MenuEName.Value;
                }
                else
                {
                    Session["MenuName"] = oCurrMenu.MenuName.Value;
                }
                Session["FunAuthSet"] = oCurrMenu.FunAuthSet.Value;

                if (oCurrMenu.FunTrackType.Value == "URL" && oCurrMenu.FunTrack.Value != "")
                {
                    if (File.Exists(Server.MapPath(oCurrMenu.FunTrack.Value)))
                    {
                        labProcName.Text = sMenuName + " 載入中..";

                        if (!string.IsNullOrEmpty(oCurrMenu.FunParameter.Value))
                        {
                            oCurrMenu.FunTrack.Value = oCurrMenu.FunTrack.Value + "?" + oCurrMenu.FunParameter.Value;
                        }

                        oAcsDB.WriteLog(DB_Acs.Logtype.功能選單切換, sUserID, sUserName, oCurrMenu.MenuNo.Value, "", "", "使用者「" + sUserName + "」進入「" + oCurrMenu.MenuName.Value + "」", "");
                        Response.Redirect(oCurrMenu.FunTrack.Value);
                    }
                }
            }
            else
            {
                Session["MenuNo"] = "";
                Session["MenuName"] = "";
                Session["FunAuthSet"] = "";
                Response.Redirect("~/MainForm.aspx");
            }
        }


        private void SetLoadInfo()
        {
            this.HdLoginInfo.Value = Request.Cookies["LogInfo"].Value;
            var loginfos = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<SahoAcs.DBModel.LogInfoModel>(Sa.Fun.Decrypt(this.HdLoginInfo.Value));
            TimeSpan t1 = new TimeSpan(loginfos.RefreshTime.Ticks);
            TimeSpan t2 = new TimeSpan(DateTime.Now.Ticks);
            Session["UserID"] = loginfos.UserID;
            loginfos.RefreshTime = DateTime.Now;
            //20200803 關掉這三項
            //this.HdLoginInfo.Value = Sa.Fun.Encrypt(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(loginfos));
            //Response.Cookies["LogInfo"].Value = this.HdLoginInfo.Value;
            //Response.Cookies["LogInfo"].Expires = DateTime.Now.AddMinutes(180);
        }

        #region 頁面共用變數及函式
        /// <summary>
        /// Menu權限調整
        /// </summary>
        /// <param name="arrTarget">目標陣列</param>
        /// <param name="arrSource">來源陣列</param>
        /// <param name="strOPMode">權限增減</param>
        /// <returns></returns>
        protected string FunAuthAdj(string[] _arrTarget, string[] _arrSource, string _strOPMode)
        {
            string strAuth = "", strAdj = "";

            if (_strOPMode.Equals("+") || _strOPMode.Equals(""))
            {
                for (int sStart = 0, sEnd = _arrSource.Length; sStart < sEnd; sStart++)
                {
                    for (int tStart = 0, tEnd = _arrTarget.Length; tStart < tEnd; tStart++)
                    {
                        if (_arrSource[sStart].ToString().Equals(_arrTarget[tStart].ToString()))
                        {
                            strAdj = "";
                            break;
                        }
                        else
                        {
                            strAdj = _arrSource[sStart].ToString();
                        }
                    }

                    if (!strAdj.Equals(""))
                    {
                        strAuth += "," + strAdj;//增加沒有的權限
                    }
                }
            }
            else if (_strOPMode.Equals("-"))
            {
                for (int tStart = 0, tEnd = _arrTarget.Length; tStart < tEnd; tStart++)
                {
                    for (int sStart = 0, sEnd = _arrSource.Length; sStart < sEnd; sStart++)
                    {
                        if (_arrTarget[tStart].ToString().Equals(_arrSource[sStart].ToString()))
                        {
                            strAdj = "";
                            break;
                        }
                        else
                        {
                            strAdj = _arrTarget[tStart].ToString();
                        }
                    }

                    if (!strAdj.Equals(""))
                    {
                        strAuth += strAdj + ",";//增加沒有的權限
                    }
                }

                if (!strAuth.Equals(""))
                {
                    strAuth = strAuth.Substring(0, strAuth.Length - 1);
                }
            }

            return strAuth;
        }
        #endregion
    }
}