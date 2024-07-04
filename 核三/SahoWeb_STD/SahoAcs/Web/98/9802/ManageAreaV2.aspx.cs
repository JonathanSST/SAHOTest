using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;

namespace SahoAcs
{
    public partial class ManageAreaV2 : System.Web.UI.Page
    {
        #region Main Description
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        private int _pagesize = 20;        //DataGridView每頁顯示的列數
        public List<MngEntity> MainList = new List<MngEntity>();
        #endregion

        #region 網頁前置作業

        #region LoadProcess
        private void LoadProcess()
        {
            
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";            
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";
            //js += "<script src='ManageArea.js?"+Pub.GetNowTime+"'></script>";
            js += "<script type='text/javascript'>SetMode('');</script>";
            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("ManageArea", "ManageAreaV2.js?"+Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
        }
        #endregion

        #region RegisterObj
        private void RegisterObj()
        {
                        
        }
        #endregion

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            this.hUserId.Value = Session["UserID"].ToString();
            if (Session["OwnerList"].ToString() == "")
                hOwnerList.Value = @"\";
            else
                hOwnerList.Value = Session["OwnerList"].ToString();

            LoadProcess();
            RegisterObj();

            if (!IsPostBack)
            {                
                hSelectState.Value = "true";

                if (Request["DoAction"] != null && Request["DoAction"] == "Insert")
                {
                    this.SetInsertData();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "Update")
                {
                    this.SetUpdateData();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "Edit")
                {
                    this.SetLoadData();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "Delete")
                {
                    this.SetDelete();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "OrgList")
                {
                    this.SetOrgList();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "EquGrList")
                {
                    this.SetEquGrList();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "AuthUpdate")
                {
                    this.SetAuthUpdate();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "AuthUpdate2")
                {
                    this.SetAuthUpdate2();
                }
                Query(true, "MgaNo", "ASC");



            }
            else
            {
               
            }
        }
        #endregion
        
        #endregion
        


        #region 查詢
        private void Query(bool select_state, string SortExpression, string SortDire)
        {
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            string paraName = "", paraNo = "";
            string sql = "SELECT * FROM B00_ManageArea WHERE OwnerList LIKE @Owner ";
            if (Request["MgaNo"] != null && Request["MgaNo"] != "")
            {
                sql += " AND MgaNo LIKE @MgaNo ";
                paraNo = "%" + Request["MgaNo"] + "%";
            }
                
            if (Request["MgaName"] != null && Request["MgaName"] != "")
            {
                sql += " AND MgaName LIKE @MgaName ";
                paraName = "%" + Request["MgaName"] + "%";
            }
            this.MainList = this.odo.GetQueryResult<MngEntity>(sql, new {Owner = @"%\"+UserID+@"\%", MgaName=paraName,MgaNo=paraNo}).ToList();
            
        }

        private void SetLoadData()
        {
            var Result = this.odo.GetQueryResult<MngEntity>("SELECT * FROM B00_ManageArea WHERE MgaID=@MgaID",new { MgaID = Request["MgaID"]});
            MngEntity entity = new MngEntity();
            foreach(var o in Result)
            {
                entity = o;
            }
            if (entity.MgaNo != null && entity.MgaNo!="")
            {
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK", success = true, resp = entity, mga_list = this.odo.GetQueryResult<MngEntity>("SELECT * FROM B00_ManageArea WHERE MgaID<>@MgaID",new {MgaID=Request["MgaID"]}) }));
                Response.End();
            }
            else
            {
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = GetGlobalResourceObject("Resource","NonData"), success = false, resp = entity }));
                Response.End();
            }
        }

        private void SetInsertData()
        {
            var objRet = Check_Input_DB("Insert");
            string User = Sa.Web.Fun.GetSessionStr(this,"UserID");
            string OwnerList = Sa.Web.Fun.GetSessionStr(this, "OwnerList");
            if (OwnerList == "")
                OwnerList += @"\";
            OwnerList += User + @"\";
            //string OwnerID = User;
            if (objRet.result)
            {
                this.odo.Execute(@"INSERT INTO B00_ManageArea (MgaNo ,MgaName ,MgaEName ,MgaDesc ,Remark ,OwnerID ,OwnerList ,CreateUserID ,UpdateUserID ,UpdateTime,Email) VALUES (
                                                @MgaNo ,@MgaName ,@MgaEName ,@MgaDesc ,@Remark ,@OwnerID ,@OwnerList ,@UserID ,@UserID ,GETDATE(),@Email)",new { MgaNo = Request["MgaNo"],
                                                OwnerID=User, MgaEName = Request["MgaEName"] ,MgaName=Request["MgaName"], MgaDesc=Request["MgaDesc"], Remark=Request["Remark"], OwnerList=OwnerList,UserID=User,Email=Request["MgaEmail"] });
                objRet.result = this.odo.isSuccess;
                if (this.odo.isSuccess)
                {
                    foreach(var o in this.odo.GetQueryResult("SELECT *, FROM B00_ManageArea WHERE MgaNo=@MgaNo",new { MgaNo = Request["MgaNo"] }))
                    {
                        objRet.message = o.MgaID;
                    }
                    
                }
                else
                {
                    objRet.message = this.odo.DbExceptionMessage;
                }
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                resp = objRet
            }));
            Response.End();
        }

        private void SetUpdateData()
        {
            var objRet = Check_Input_DB("Update");
            string User = Sa.Web.Fun.GetSessionStr(this, "UserID");
            string OwnerList = Sa.Web.Fun.GetSessionStr(this, "OwnerList")+User+@"\";

            if (objRet.result)
            {
                this.odo.Execute(@"UPDATE B00_ManageArea SET MgaNo = @MgaNo ,MgaName = @MgaName ,MgaEName = @MgaEName 
                                            ,MgaDesc=@MgaDesc ,Remark =@Remark ,UpdateUserID = @UserID ,Email = @Email,UpdateTime = GETDATE() WHERE MgaID = @MgaID ;", new
                {
                    MgaNo = Request["MgaNo"],
                    MgaID=Request["MgaID"],
                    MgaName = Request["MgaName"],
                    MgaEName = Request["MgaEName"],
                    MgaDesc = Request["MgaDesc"],
                    Remark = Request["Remark"],
                    Email = Request["MgaEmail"],
                    UserID = User
                });
                objRet.result = this.odo.isSuccess;
                if (this.odo.isSuccess)
                {
                    objRet.message = Request["MgaID"];
                }
                else
                {
                    objRet.message = this.odo.DbExceptionMessage;
                }
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                resp = objRet
            }));
            Response.End();
        }

        private void SetDelete()
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            this.odo.Execute("DELETE B00_ManageArea WHERE MgaID=@MgaID", new { MgaID = Request["MgaID"] });
            this.odo.Execute("DELETE B00_SysUserMgns WHERE MgaID=@MgaID", new { MgaID = Request["MgaID"] });
            objRet.result = this.odo.isSuccess;
            if (!this.odo.isSuccess)
                objRet.message = this.odo.DbExceptionMessage;
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
            {
                resp = objRet
            }));
            Response.End();
        }

        private void SetOrgList()
        {
            var Result = this.odo.GetQueryResult<MngEntity>("SELECT * FROM B00_ManageArea WHERE MgaID=@MgaID", new { MgaID = Request["MgaID"] });
            MngEntity entity = new MngEntity();
            foreach (var o in Result)
            {
                entity = o;
            }
            if (entity.MgaNo != null && entity.MgaNo != "")
            {
                var OrgList1 = this.odo.GetQueryResult<OrgStrucInfo>(@"SELECT OrgNameList,OrgNoList,A.OrgStrucID 
                                                                                                    FROM OrgStrucAllData('') A
                                                                                                    LEFT JOIN B01_MgnOrgStrucs B ON A.OrgStrucID=B.OrgStrucID AND MgaID=@MgaID WHERE B.OrgStrucID IS NULL ",new {MgaID=Request["MgaID"]}).ToList();
                var OrgList2 = this.odo.GetQueryResult<OrgStrucInfo>(@"SELECT OrgNameList,OrgNoList,A.OrgStrucID
                                                                                                    FROM OrgStrucAllData('') A
                                                                                                    INNER JOIN B01_MgnOrgStrucs B ON A.OrgStrucID=B.OrgStrucID AND MgaID=@MgaID ",new {MgaID=Request["MgaID"]}).ToList();
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK", success = true, resp = entity, OrgList1=OrgList1, OrgList2=OrgList2 }));
                Response.End();
            }
            else
            {
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = GetGlobalResourceObject("Resource", "NonData"), success = false, resp = entity }));
                Response.End();
            }
        }

        private void SetEquGrList()
        {
            var Result = this.odo.GetQueryResult<MngEntity>("SELECT * FROM B00_ManageArea WHERE MgaID=@MgaID", new { MgaID = Request["MgaID"] });
            MngEntity entity = new MngEntity();
            foreach (var o in Result)
            {
                entity = o;
            }

            if (entity.MgaNo != null && entity.MgaNo != "")
            {
                var GrList1 = this.odo.GetQueryResult<EquGroupModel>(@"SELECT EquGrpName,EquGrpNo,A.EquGrpID
                                                FROM B01_EquGroup A
                                                LEFT JOIN B01_MgnEquGroup B ON A.EquGrpID=B.EquGrpID AND MgaID=@MgaID WHERE B.EquGrpID IS NULL ", new { MgaID = Request["MgaID"] }).ToList();
                var GrList2 = this.odo.GetQueryResult<EquGroupModel>(@"SELECT EquGrpName,EquGrpNo,A.EquGrpID
                                                FROM B01_EquGroup A
                                                INNER JOIN B01_MgnEquGroup B ON A.EquGrpID=B.EquGrpID AND MgaID=@MgaID ", new { MgaID = Request["MgaID"] }).ToList();
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK", success = true, resp = entity, EquGrList1 = GrList1, EquGrList2 = GrList2 }));
                Response.End();
            }
            else
            {
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = GetGlobalResourceObject("Resource", "NonData"), success = false, resp = entity }));
                Response.End();
            }

        }

        #region AuthUpdate 權限資料編輯
        public void SetAuthUpdate()
        {
            Pub.MessageObject sRet = new Pub.MessageObject();
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            int istat = 0;
            List<string> liSqlPara = new List<string>();

            if (sRet.result)
            {
                if (istat > -1)
                {
                    #region Process String
                    odo.BeginTransaction();
                    istat += odo.Execute("DELETE B01_MgnOrgStrucs WHERE MgaID=@MgaID", new { MgaID = Request["MgaID"] });
                    if (Request["OrgList"] != null && Request["OrgList"].Split(',').Count() > 0)
                    {
                        string[] OSL = Request["OrgList"].Split(',');
                        for (int i = 0; i < OSL.Length; i++)
                        {
                            istat += odo.Execute("INSERT INTO B01_MgnOrgStrucs(OrgStrucID ,MgaID ,CreateUserID) VALUES (@OrgStrucID,@MgaID,@CreateUserID)"
                                , new { MgaID = Request["MgaID"], OrgStrucID = OSL[i], CreateUserID = UserID });
                        }
                    }
                    #endregion                    
                }
                if (istat > 0)
                {
                    odo.Commit();
                }
                else
                {
                    odo.Rollback();
                }
            }
            sRet.act = "AuthUpdate";
            sRet.result = this.odo.isSuccess;
            if (!this.odo.isSuccess)
            {
                sRet.message = this.odo.DbExceptionMessage;
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
            new { resp = sRet }));
            Response.End();
        }

        public void SetAuthUpdate2()
        {
            Pub.MessageObject sRet = new Pub.MessageObject();
            string UserID = Sa.Web.Fun.GetSessionStr(this, "UserID");
            int istat = 0;            
            if (sRet.result)
            {
                if (istat > -1)
                {
                    #region Process String
                    odo.BeginTransaction();
                    istat += odo.Execute("DELETE B01_MgnEquGroup WHERE MgaID=@MgaID", new { MgaID = Request["MgaID"] });
                    if (Request["EquGrList"] != null && Request["EquGrList"].Split(',').Count() > 0)
                    {
                        string[] OSL = Request["EquGrList"].Split(',');
                        for (int i = 0; i < OSL.Length; i++)
                        {
                            istat += odo.Execute("INSERT INTO B01_MgnEquGroup(EquGrpID ,MgaID ,CreateUserID) VALUES (@EquGrpID,@MgaID,@CreateUserID)"
                                , new { MgaID = Request["MgaID"], EquGrpID = OSL[i], CreateUserID = UserID });
                        }
                    }
                    #endregion                    
                }
                if (istat > 0)
                {
                    odo.Commit();
                }
                else
                {
                    odo.Rollback();
                }
            }
            sRet.act = "AuthUpdate2";
            sRet.result = this.odo.isSuccess;
            if (!this.odo.isSuccess)
            {
                sRet.message = this.odo.DbExceptionMessage;
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
            new { resp = sRet }));
            Response.End();
        }

        #endregion


        protected Pub.MessageObject Check_Input_DB(string mode)
        {            
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            
            #region DB
            switch (mode)
            {
                case "Insert":
                    sql = @" SELECT * FROM B00_ManageArea WHERE MgaNo = @MgaNo ";
                    break;
                case "Update":
                    sql = @" SELECT * FROM B00_ManageArea WHERE MgaNo = @MgaNo AND MgaID <> @MgaID ";
                    break;
            }
            string RequestMgaNo = Request["MgaNo"].ToString().Trim();
            string RequestMgaEmail = Request["MgaEmail"].ToString().Trim();
            string RequestMgaENname = Request["MgaEName"].ToString().Trim();
            string RequestMgaID = Request["MgaID"];

         

            //驗証編號
            if (!string.IsNullOrEmpty(RequestMgaNo))
            {
                if(RequestMgaNo.Contains(" "))
                {
                    sRet.result = false;
                    sRet.message += "編號格式不可有空白符號";
                }
                var result = this.odo.GetQueryResult(sql, new { MgaNo = RequestMgaNo, MgaID = RequestMgaID });
                if (result.Count() > 0)
                {
                    if (!string.IsNullOrEmpty(sRet.message))
                    {
                        sRet.message += "\n";
                        sRet.result = false;
                        sRet.message += "此編號已存在於系統中";
                    }
                }
            }
            else
            {
                sRet.result = false;
                sRet.message += "編號不可空白";
            }
            //驗証英文名稱
            if (!string.IsNullOrEmpty(RequestMgaENname))
            {
                bool ENnameOK = IsENname(RequestMgaENname);
                if (ENnameOK == false)
                {
                    sRet.result = false;
                    sRet.message += "英文名稱格式不正確";
                }
                if (RequestMgaEmail.Contains(" "))
                {
                    sRet.result = false;
                    sRet.message += "英文名稱格式不可有空白符號";
                }
            }
                //驗証信箱
            if (!string.IsNullOrEmpty(RequestMgaEmail))
            {
                bool EmailOK = IsEmail(RequestMgaEmail);
                if (EmailOK == false)
                {
                    sRet.result = false;
                    sRet.message += "電子郵件格式不正確";
                }
                if (RequestMgaEmail.Contains(" "))
                {
                    sRet.result = false;
                    sRet.message += "電子郵件格式不可有空白符號";
                }
            }
            #endregion

            return sRet;
        }

        #endregion

        #region JavaScript及aspx共用方法
   
        

        #region AuthUpdate2 權限資料編輯
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static object AuthUpdate2(String MgaID, String EquGrList, String UserID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            int istat = 0;
            List<string> liSqlPara = new List<string>();

            if (sRet.result)
            {
                if (istat > -1)
                {
                    #region Process String
                    sql = @" DELETE FROM B01_MgnEquGroup WHERE MgaID = ? ; ";
                    liSqlPara.Add("S:" + MgaID);
                    if (EquGrList.Length > 0)
                    {
                        String[] OSL = EquGrList.Split('|');
                        for (int i = 0; i < OSL.Length - 1; i++)
                        {
                            sql += @" INSERT INTO B01_MgnEquGroup(MgaID ,EquGrpID ,CreateUserID) ";
                            sql += @" VALUES (?, ?, ?) ; ";
                            liSqlPara.Add("S:" + MgaID);
                            liSqlPara.Add("S:" + OSL[i]);
                            liSqlPara.Add("S:" + UserID);
                        }
                    }

                    #endregion
                    oAcsDB.BeginTransaction();
                    istat = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                }
                if (istat > -1)
                    oAcsDB.Commit();
                else
                    oAcsDB.Rollback();
            }
            sRet.act = "AuthUpdate2";
            return sRet;
        }
        #endregion

        #region 載入設備群組資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static String[] LoadEquGrList(string Mga_ID)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject sRet = new Pub.MessageObject();
            string sql = "";
            String[] EditData = null;
            List<String> EquGrList = null;
            List<string> liSqlPara = new List<string>();
            Sa.DB.DBReader dr;

            //SELECT MgaNo FROM B00_ManageArea WHERE MgaID = 5

            #region Process String
            sql = @" SELECT MgaNo FROM B00_ManageArea WHERE MgaID = ? ";
            liSqlPara.Add("S:" + Mga_ID.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            EditData = new String[3];
            if (dr.HasRows)
            {
                if (dr.Read())
                    EditData[0] = dr.DataReader[0].ToString();
            }
            else
            {
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "系統中無此資料！";
            }

            #endregion

            #region Process String2
            if (EditData[0] != "Saho_SysErrorMassage")
            {
                dr = null;
                liSqlPara = new List<string>();
                sql = " SELECT B00_ManageArea.MgaID, B01_EquGroup.EquGrpID, B01_EquGroup.EquGrpNo, B01_EquGroup.EquGrpName ";
                sql += " FROM B00_ManageArea INNER JOIN ";
                sql += " B01_MgnEquGroup ON B00_ManageArea.MgaID = B01_MgnEquGroup.MgaID INNER JOIN ";
                sql += " B01_EquGroup ON B01_MgnEquGroup.EquGrpID = B01_EquGroup.EquGrpID ";
                sql += " WHERE B00_ManageArea.MgaID = ? ";
                liSqlPara.Add("S:" + Mga_ID.Trim());
                oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                EquGrList = new List<string>();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        EditData[2] += dr.DataReader[1].ToString() + "|" + dr.DataReader[2].ToString() + "|" + dr.DataReader[3].ToString() + "|";
                        EquGrList.Add(dr.DataReader[1].ToString());
                    }
                    EditData[2] = EditData[2].Substring(0, EditData[2].Length - 1);
                }
                else
                    EditData[2] = "";
            }
            #endregion

            #region Process String3
            if (EditData[0] != "Saho_SysErrorMassage")
            {
                dr = null;
                liSqlPara = new List<string>();
                sql = @" SELECT * FROM B01_EquGroup ";
                if (EquGrList.Count > 0)
                {
                    sql += " WHERE EquGrpID NOT IN ( ";
                    for (int i = 0; i < EquGrList.Count; i++)
                        sql += "'" + EquGrList[i] + "',";
                    sql = sql.Substring(0, sql.Length - 1) + ") ";
                }
                oAcsDB.GetDataReader(sql, liSqlPara, out dr);
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        EditData[1] += dr.DataReader[0].ToString() + "|" + dr.DataReader[1].ToString() + "|" + dr.DataReader[2].ToString() + "|";
                    }
                    EditData[1] = EditData[1].Substring(0, EditData[1].Length - 1);
                }
                else
                    EditData[1] = "";
            }
            #endregion

            return EditData;
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

        #region 驗証信箱格式
        protected static bool IsEmail(string Email)
        {
            // bool Isok = false;
            //return System.Text.RegularExpressions.Regex.IsMatch(Email, @"^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{2,3}){1,2})$");
            return System.Text.RegularExpressions.Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            //return Isok = true;
        }
        #endregion 驗証信箱格式_end

        #region 驗証英文名稱
        protected static bool IsENname(string name)
        {
            // bool Isok = false;
            return System.Text.RegularExpressions.Regex.IsMatch(name, @"^[0-9a-zA-Z_]*$");
            //return Isok = true;
        }
        #endregion 驗証英文名稱_end

        #endregion
    }
}