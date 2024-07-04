using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sa.DB;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using SahoAcs.DBClass;


namespace SahoAcs
{
    public partial class UserV3 : BasePage
    {
        #region 1.Main Description
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        
        B00Sysuser MainEntity = new B00Sysuser();

        public List<ItemList> MenuAuthList = new List<ItemList>();
        public List<B00Sysuser> UserList = new List<B00Sysuser>();
        public List<FunMenu> SysMenuList = new List<FunMenu>();
        public List<UserRole> SysUserRole = new List<UserRole>();
        public List<B00Sysuser> SysUserMga = new List<B00Sysuser>();

        private int _pagesize = 20;   //DataGridView每頁顯示的資料列數
        private static string UserID = "";
        private static string OwnerList = "";
        private static string query_id = "", query_name = "", query_states = "";
        public string SortType = "ASC";
        public string SortName = "UserID";
        #endregion

        #region 2.RegisterStartupScript
        
        #endregion

        #region 3.Events

        #region 3-1.Page_Load - r
        protected override void Page_Load(object sender, EventArgs e)
        {
            this.MenuAuthList = this.odo.GetQueryResult<ItemList>(@"SELECT ItemNo, ItemName 
                     FROM B00_ItemList 
                     WHERE ItemClass = 'MenuAuth' 
                     ORDER BY ItemOrder ").ToList();
            this.SysMenuList = this.odo.GetQueryResult<FunMenu>(@" SELECT MenuNo, MenuName, FunAuthDef
                     FROM B00_SysMenu 
                     WHERE IsAuthCtrl = '1' AND MenuIsUse='1'
                     ORDER BY MenuNo").ToList();
            this.SysUserRole = this.odo.GetQueryResult<UserRole>(@"SELECT * FROM B00_SysRole ORDER BY RoleNo").ToList();
            this.SysUserMga = this.odo.GetQueryResult<B00Sysuser>(@"SELECT * FROM B00_ManageArea ORDER BY MgaNo").ToList();
            //取得登入者本身的OwnerList
            OwnerList = Session["OwnerList"].ToString().Trim();
            #region 3-1-1.LoadProcess
            
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js += "\nSetMode('');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("User", "UserV3.js?"+Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
                       
            #endregion

            if (!IsPostBack)
            {
                query_id = "";
                query_name = "";
                query_states = "";
                UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");                
                Query("UserID", "");
                this.UserSTime.DateValue = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                this.UserETime.DateValue = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                if (Request["DoAction"] != null && Request["DoAction"] =="Edit")
                {
                    this.SetLoadData();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "Insert")
                {
                    this.SetInert();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "Update")
                {
                    this.SetUpdate();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "Delete")
                {
                    this.SetDelete();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "Query")
                {
                    if (Request.Form["SortName"] != null)
                    {
                        this.SortName = this.Request.Form["SortName"].ToString();
                    }
                    if (Request.Form["SortType"] != null)
                    {
                        this.SortType = this.Request.Form["SortType"].ToString();
                    }
                    this.Query(this.SortName, this.SortType);
                }
                if (Request.Form["DoAction"] != null && Request.Form["DoAction"].Equals("Reset"))
                {
                    this.SetResetPwd();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "UserMenu")
                {
                    this.SetUserMenu();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "SaveMenu")
                {
                    this.SetSaveMenu();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "UserRole")
                {
                    this.SetUserRole();   
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "UserMgns")
                {
                    this.SetUserMgns();
                }                
                if (Request["DoAction"] != null && Request["DoAction"] == "SaveMgns")
                {
                    this.SetSaveMgns();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "SaveRole")
                {
                    this.SetSaveRole();
                }
            }
            else
            {

            }

        }
        #endregion

        #region DoAction

        public B00Sysuser GetMainObj()
        {
            var model = new B00Sysuser();
            var para = this.GetMasterPackage(this.odo.GetDataInfoList("B00_SysUser"));
            model = this.DictionaryToObject<B00Sysuser>(para);
            model.UserSTime = Request[Request.Form.AllKeys.Where(i => i.Contains("UserSTime")).First()] != "" ? DateTime.Parse(Request[Request.Form.AllKeys.Where(i => i.Contains("UserSTime")).First()]) : DateTime.Now;
            model.UserETime = Request[Request.Form.AllKeys.Where(i => i.Contains("UserETime")).First()] != "" ? DateTime.Parse(Request[Request.Form.AllKeys.Where(i => i.Contains("UserETime")).First()]) : new DateTime(2099, 12, 31);
            model.OwnUserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            model.OwnerID= Sa.Web.Fun.GetSessionStr(this, "UserID");
            model.OwnerList = Sa.Web.Fun.GetSessionStr(this,"OwnerList");
            if (string.IsNullOrEmpty(model.OwnerList))
            {
                model.OwnerList = @"\" + model.OwnerID + @"\";
            }
            else
            {
                model.OwnerList = model.OwnerList + model.OwnerID + @"\";
            }
            return model;
        }

        /// <summary>儲存系統使用者目錄 B00_SysUserMenus</summary>
        void SetSaveMenu()
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            var RequestUserMenu = Request.Form.AllKeys.Where(i => i.StartsWith("UserMenusOPMode_")).ToList();
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            //有勾選權限狀態，則進行資料庫寫入
            List<string> ListAuth = new List<string>();
            foreach (var menu in RequestUserMenu)
            {
                string menu_no = menu.Split('_').Length > 1 ? menu.Split('_')[1] : "";
                if (menu_no != "")
                {
                    //取得使用權限清單資訊
                    var RequestChkList = Request.Form.AllKeys.Where(i => i.StartsWith("Chk_" + menu_no)).ToList();
                    if (RequestChkList.Count > 0)
                    {
                        ListAuth.Clear();
                        foreach (var auth in RequestChkList)
                        {
                            ListAuth.Add(Request[auth].ToString());
                        }
                        //進行B00_SysUserMenus的資料處理
                        this.odo.Execute("DELETE B00_SysUserMenus WHERE UserID=@SysUserID AND MenuNo=@MenuNo", new { SysUserID = Request["UserID"], MenuNo = menu_no });
                        this.odo.Execute(@"INSERT INTO B00_SysUserMenus (UserID,MenuNo,FunAuthSet,OPMode,CreateUserID,CreateTime) 
                                                    VALUES (@SysUserID, @MenuNo, @FunAuthSet, @OPMode,@UserID,GETDATE()) "
                                                    , new { SysUserID = Request["UserID"], MenuNo = menu_no, FunAuthSet = string.Join(",", ListAuth), OPMode = Request[menu], UserID = UserID });
                        objRet.result = this.odo.isSuccess;
                        objRet.message = this.odo.DbExceptionMessage;
                    }
                    else
                    {
                        if(Request[menu]!=null && Request[menu].Equals("-"))
                        {
                            //進行B00_SysUserMenus的資料處理
                            this.odo.Execute("DELETE B00_SysUserMenus WHERE UserID=@SysUserID AND MenuNo=@MenuNo", new { SysUserID = Request["UserID"], MenuNo = menu_no });
                        }
                        
                    }
                }
            }
            //完成系統使用者目錄設定 UserMenu
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                resp = objRet
            }));
            Response.End();
        }//end SaveMenu


        /// <summary>儲存系統使用者角色設定</summary>
        void SetSaveRole()
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            var UserRoles = new List<string>();
            if (Request["UserRolesAuth"] != null)
            {
                UserRoles = Request.Form.GetValues("UserRolesAuth").ToList();
            }
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            //有勾選權限狀態，則進行資料庫寫入
            this.odo.Execute("DELETE B00_SysUserRoles WHERE UserID=@SysUserID", new {SysUserID=Request["UserID"] });
            UserRoles.ForEach(role =>
            {
                this.odo.Execute("INSERT INTO B00_SysUserRoles (UserID,RoleID,CreateTime,CreateUserID) VALUES (@SysUserID,@RoleID,GETDATE(),@UserID)", new {UserID=UserID, SysUserID = Request["UserID"], RoleID =role});
                objRet.result = this.odo.isSuccess;
                objRet.message = this.odo.DbExceptionMessage;
            });            
            //完成系統使用者角色設定 B00_SysUserRoles
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                resp = objRet
            }));
            Response.End();
        }


        /// <summary>儲存系統使用者角色設定</summary>
        void SetSaveMgns()
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            var UserMgns = new List<string>();
            if (Request["UserMgaAuth"] != null)
            {
                UserMgns = Request.Form.GetValues("UserMgaAuth").ToList();
            }
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            //有勾選權限狀態，則進行資料庫寫入
            this.odo.Execute("DELETE B00_SysUserMgns WHERE UserID=@SysUserID", new { SysUserID = Request["UserID"] });
            UserMgns.ForEach(mga =>
            {
                this.odo.Execute("INSERT INTO B00_SysUserMgns (UserID,MgaID,CreateTime,CreateUserID) VALUES (@SysUserID,@MgaID,GETDATE(),@UserID)", new { UserID = UserID, SysUserID = Request["UserID"], MgaID = mga });
                objRet.result = this.odo.isSuccess;
                objRet.message = this.odo.DbExceptionMessage;
            });
            //完成系統使用者角色設定 B00_SysUserRoles
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                resp = objRet
            }));
            Response.End();
        }


        /// <summary>取得系統使用者角色資訊</summary>
        void SetUserRole()
        {
            var UserRoles = this.odo.GetQueryResult<UserRole>("SELECT * FROM B00_SysUserRoles WHERE UserID=@UserID ",new {UserID=Request["SelectValue"]} ).ToList();
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                roles = UserRoles
            }));
            Response.End();
        }


        /// <summary>取得系統管理區資訊</summary>
        void SetUserMgns()
        {
            var UserMgns = this.odo.GetQueryResult<B00Sysuser>("SELECT * FROM B00_SysUserMgns WHERE UserID=@UserID ", new { UserID = Request["SelectValue"] }).ToList();
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                mgns = UserMgns
            }));
            Response.End();
        }


        /// <summary>取得目前的系統使用者目錄 Get B00_SysUserMenus</summary>
        void SetUserMenu()
        {
            var UserAuths = this.odo.GetQueryResult<FunMenu>("SELECT * FROM B00_SysUserMenus WHERE UserID=@UserID", new { UserID = Request["UserID"] });
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                auths = UserAuths
            }));
            Response.End();
        }


        void SetUpdate()
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            objRet = this.Check_Input_DB("Update");
            this.MainEntity = this.GetMainObj();
            this.MainEntity.OwnerID = Request["SelectValue"];
            if (objRet.result)
            {
                var sql = @" UPDATE B00_SysUser SET UserID=@UserID, UserPW = @UserPW, UserName = @UserName, UserEName = @UserEName , ErrCnt=0,
                    UserSTime = @UserSTime, UserETime = @UserETime, PWChgTime =GETDATE(), IsOptAllow = @IsOptAllow , EMail = @Email, Remark = @Remark, UpdateUserID = @OwnUserID,
                    UpdateTime = GETDATE() WHERE UserID = @OwnerID AND UserID<>'User' ";
                var effect = this.odo.Execute(sql, this.MainEntity);
                if (effect > 0 && !this.MainEntity.UserID.Equals(this.MainEntity.OwnerID))
                {
                    //修改管理區、系統使用者目錄、系統角色
                    this.odo.Execute("UPDATE B00_SysUserRoles SET UserID=@UserID WHERE UserID=@UserID0", new { UserID = this.MainEntity.UserID, UserID0 = this.MainEntity.OwnerID });
                    this.odo.Execute("UPDATE B00_SysUserMenus SET UserID=@UserID WHERE UserID=@UserID0", new { UserID = this.MainEntity.UserID, UserID0 = this.MainEntity.OwnerID });
                    this.odo.Execute("UPDATE B00_SysUserMgns SET UserID=@UserID WHERE UserID=@UserID0", new { UserID = this.MainEntity.UserID, UserID0 = this.MainEntity.OwnerID });
                }
                objRet.result = this.odo.isSuccess;
                if (!this.odo.isSuccess)
                {
                    objRet.message = this.odo.DbExceptionMessage;
                }
                else
                {
                    objRet.message = Request["UserID"];
                }
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                resp = objRet
            }));
            Response.End();
        }

        void SetResetPwd()
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            objRet = this.Check_Input_DB("Update");
            this.MainEntity = this.GetMainObj();
            this.MainEntity.OwnerID = Request["SelectValue"];
            if (objRet.result)
            {
                var sql = @" UPDATE B00_SysUser SET UserPW = 'AbcdEfgh1234', ErrCnt=0, PWChgTime=DATEADD(MONTH,-3,GETDATE()),
                    UpdateUserID=@OwnUserID,
                    UpdateTime = GETDATE() WHERE UserID = @OwnerID AND UserID<>'User' ";
                if(this.odo.Execute(sql, this.MainEntity) > 0)
                {
                    var log = SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.更改使用者密碼, Sa.Web.Fun.GetSessionStr(this, "UserID"), Sa.Web.Fun.GetSessionStr(this,"UserName"), "9803");
                    log.LogInfo = "重置使用者密碼"+this.MainEntity.UserID;
                    this.odo.SetSysLogCreate(log);
                }
                
                objRet.result = this.odo.isSuccess;
                if (!this.odo.isSuccess)
                {
                    objRet.message = this.odo.DbExceptionMessage;
                }
                else
                {
                    objRet.message = Request["UserID"];
                }
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                resp = objRet
            }));
            Response.End();
        }


        /// <summary>資料新增</summary>
        void SetInert()
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            objRet = this.Check_Input_DB("Insert");
            this.MainEntity = this.GetMainObj();
            //string OwnUserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            if (objRet.result)
            {
                var sql = @"INSERT INTO B00_SysUser (UserID, UserPW, UserName, UserEName, UserSTime, UserETime, IsOptAllow,
                    EMail, Remark, OwnerID, OwnerList, CreateUserID) 
                    VALUES (@UserID, @UserPW, @UserName, @UserEName, @UserSTime, @UserETime, @IsOptAllow,
                    @EMail, @Remark, @OwnerID, @OwnerList, @OwnerID)";
                this.odo.Execute(sql, this.MainEntity);
                objRet.result = this.odo.isSuccess;
                if (!this.odo.isSuccess)
                {
                    objRet.message = this.odo.DbExceptionMessage;
                }
                else
                {
                    objRet.message = Request["UserID"];
                }
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                resp = objRet
            }));
            Response.End();
        }

        void SetDelete()
        {
            var Users = this.odo.GetQueryResult<B00Sysuser>("SELECT * FROM B00_SysUser WHERE UserID=@UserID", new { UserID = Request["SelectValue"] });
            Pub.MessageObject objRet = new Pub.MessageObject();
            if (Users.Count() > 0)
            {
                var user = Users.First();
                odo.BeginTransaction();
                this.odo.Execute(Pub.GetCmdStr("DelUserMgns"), user);
                if (this.odo.isSuccess)
                    this.odo.Execute(Pub.GetCmdStr("DelUserRoles"), user);
                if (this.odo.isSuccess)
                    this.odo.Execute(Pub.GetCmdStr("DelUserMenus"), user);
                if (this.odo.isSuccess)
                    this.odo.Execute("DELETE B00_SysUser WHERE UserID=@UserID", new { UserID = Request["SelectValue"] });
                if (this.odo.isSuccess)
                {
                    this.odo.Commit();
                    //this.odo.SetSysLogCreate()
                }
                else
                    this.odo.Rollback();
                objRet.message = this.odo.DbExceptionMessage;
                objRet.result = this.odo.isSuccess;
            }
            else
            {
                objRet.result = false;
                objRet.message = "not find any data";
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                resp = objRet
            }));
            Response.End();
        }


        /// <summary>資料修改</summary>
        void SetLoadData()
        {
            var entity = new B00Sysuser();
            foreach (var o in this.odo.GetQueryResult<B00Sysuser>("SELECT * FROM B00_SysUser WHERE UserID=@UserID", new { UserID = Request["UserID"] }))
            {
                entity = o;
            }
            if (entity.UserID != "")
            {
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK", success = true, resp = entity }));
                Response.End();
            }
            else
            {
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = this.GetNonData, success = false, resp = entity }));
                Response.End();
            }
            
        }

        #endregion



        #region 3-3.GridView_Data_DataBound - r
        protected void GridView_Data_DataBound(object sender, EventArgs e)
        {
            //td_showGridView.Attributes["colspan"] = MainGridView.Columns.Count.ToString();
        }
        #endregion

        
        #endregion

        #region 4.Method

        #region 4-1.LimitText - r
        public string LimitText(string str, int len)
        {
            if (str.Length > len)
            {
                return str.Substring(0, len) + "...";
            }
            else
            {
                return str;
            }
        }
        #endregion
        

        #region 4-4.Check_Input_DB
        protected Pub.MessageObject Check_Input_DB(string Mode)
        { 
            Pub.MessageObject objRet = new Pub.MessageObject();            
            string sql = "";

            

            #region 4-4-2.DB
            //查詢只為驗證是否帳號存在盡量避免用*，以免浪費無謂的效能
            switch (Mode)
            {
                case "Insert":
                    sql = @" SELECT UserID FROM B00_SysUser WHERE UserID = @UserID 
                                     UNION  SELECT PsnAccount FROM B01_Person WHERE PsnAccount=@UserID ";                    
                    break;
                case "Update":
                    sql = @" SELECT UserID FROM B00_SysUser WHERE UserID = @UserID AND UserID <> @SelectValue
                                    UNION  SELECT PsnAccount FROM B01_Person WHERE PsnAccount=@UserID AND PsnAccount<>@SelectValue ";                    
                    break;
            }
            
            if(this.odo.GetQueryResult(sql,new {UserID=Request["UserID"], SelectValue=Request["SelectValue"]}).Count()>0)
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "此帳號已存在於系統中";
            }
            #endregion

            return objRet;
        }
        #endregion

        #region 4-5.Query - r
        public void Query(string SortExpression, string SortDire)
        {
            string sql = "SELECT * FROM B00_SysUser  ";
            string cmd = " OwnerList LIKE @OwnList ";
            string OwnerList = Sa.Web.Fun.GetSessionStr(this, "OwnerList");
            string p1 = "";
            string p2 = "";
            if (Request["Input_ID"] != null && Request["Input_ID"]!="")
            {
                if (cmd != "")
                    cmd += " AND ";
                cmd += " UserID LIKE @UserID";
                p1 = "%" + Request["Input_ID"] + "%";
            }
            if (Request["Input_Name"] != null && Request["Input_Name"] != "")
            {
                if (cmd != "")
                    cmd += " AND ";
                cmd += " UserName LIKE @UserName";
                p2 = "%" + Request["Input_Name"] + "%";
            }
            //UserID = "";
            if (cmd != "")
                cmd = " WHERE " + cmd;
            this.UserList = this.odo.GetQueryResult<B00Sysuser>(sql+cmd,new {UserID=p1, UserName=p2, OwnList = "%"+ OwnerList+UserID+@"\%"}).OrderByField(SortExpression, SortDire.Equals("ASC")).ToList();
        }
        #endregion


                     
        #region 4-13.排序欄位及條件
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


        
        /// <summary>依使用者名稱，取得dongle系統功能目錄</summary>
        /// <returns>功能目錄清單</returns>
        public List<string> GetMenuList()
        {
            string UserID = Session["UserID"].ToString();
            List<string> MenuList = new List<string>();
            MenuList = UserID.GetMenuList();
            return MenuList;
        }

    }//end page class
}//end namespace