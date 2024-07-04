using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sa.DB;
using SahoAcs.DBClass;
using DapperDataObjectLib;
using SahoAcs.DBModel;


namespace SahoAcs
{
    public partial class TempCardCreate : Page
    {
        #region 一.宣告
        private int _pagesize = 10;             //設定GridView控制項每頁可顯示的資料列數
        private static string UserID = "";      //儲存目前使用者的UserID
        private static string UserName = "";    //儲存目前使用者的UserName
        public string CardLen = "10";
        public int MaxCard = 0;
        public int CurrentCard = 0;
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public List<CardEntity> ListTempCard = new List<CardEntity>();

        #endregion
        
        #region 2-1A.網頁：事件方法
        /// <summary>
        /// 初始化網頁相關的動作
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MaxCard = DongleVaries.GetMaxPerson();
            this.CurrentCard = DongleVaries.GetCurrentCard();
            this.CardLen = this.odo.GetStrScalar(" SELECT TOP 1 [ParaValue] FROM [B00_SysParameter] WHERE ParaClass = 'CardID' AND ParaNo = 'CardLen' ");
            UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            UserName = Sa.Web.Fun.GetSessionStr(this.Page, "UserName");

            this.RegisterComponeAndScript();
            if (!IsPostBack)
            {
                if(Request["DoAction"]!=null && Request["DoAction"] == "LoadData")
                {
                    LoadData();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "Insert")
                {
                    this.Insert();
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "Update")
                {
                    this.Update();
                }
                if(Request["DoAction"]!=null && Request["DoAction"] == "Query")
                {
                    this.Query(Request["OrderName"].ToString(), Request["OrderType"].ToString());
                }
                if (Request["DoAction"] != null && Request["DoAction"] == "Delete")
                {
                    this.Delete();
                }
                if (Request["DoAction"] == null)
                {
                    this.Query("CardNo", "ASC");
                }
                
            }
            else
            {
                
            }
        }
        #endregion

        #region 2-1B.網頁：自訂方法

        private void RegisterComponeAndScript()
        {
         
            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("TempCardRecord", "TempCardCreate.js?" + Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
        }


        /// <summary>
        /// *開啟對話視窗的JavaScript語法
        /// </summary>
        private void OpenDialog_Js()
        {
            string jstr = "";

            jstr = @"
                    function OpenDialogAdd(theURL,win_width,win_height) { 
                        var PosX = (screen.width-win_width)/2; 
                        var PosY = (screen.height-win_height)/2; 
                        features = 'dialogWidth='+win_width+',dialogHeight='+win_height+',dialogTop='+PosY+',dialogLeft='+PosX+';' 
                        window.showModalDialog(theURL, '', features);
                    }

                    function OpenDialogEdit(theURL,key,win_width,win_height) { 
                        var PosX = (screen.width-win_width)/2; 
                        var PosY = (screen.height-win_height)/2; 
                        features = 'dialogWidth='+win_width+',dialogHeight='+win_height+',dialogTop='+PosY+',dialogLeft='+PosX+';' 
                        window.showModalDialog(theURL+key, '', features);
                    }";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "OpenDialog_Js", jstr, true);
        }

        #endregion


        #region 三.元件


        #region 3-1B.*元件-表格：自訂方法
        /// <summary>
        /// *限制來源字串的顯示長度
        /// </summary>
        /// <param name="str">來源字串</param>
        /// <param name="len">顯示長度</param>
        /// <param name="ellipsis">是否省略</param>
        /// <returns>string</returns>
        public string LimitText(string str, int len, bool ellipsis)
        {
            Encoding big5 = Encoding.GetEncoding("big5");
            byte[] b = big5.GetBytes(str);

            if (b.Length <= len)
                return str;
            else
            {
                if (ellipsis)
                    len -= 3;

                string res = big5.GetString(b, 0, len);

                if (!big5.GetString(b).StartsWith(res))
                    res = big5.GetString(b, 0, len - 1);

                return res + (ellipsis ? "..." : "");
            }
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

        #region 四.查詢
        /// <summary>
        /// 依據指定的條件內容查詢資料並更新顯示於表格
        /// </summary>
        /// <param name="SortExpression">排序欄位</param>
        /// <param name="SortDire">排序定序</param>
        public void Query(string SortExpression, string SortDire)
        {
                        
            string sql = "";
            string QueryCardNo = "";
            #region Process String - B01_Card

            //設定臨時卡查詢條件 C.CardType = 'R'
            sql = @"SELECT C.CardNo, C.CardDesc, 
                    CASE 
                        WHEN C.CardAuthAllow = 1 THEN '有效'
                        WHEN C.CardAuthAllow = 0 THEN '無效'
                    END AS CardNum, 
                    (CONVERT(VARCHAR, C.PsnID) + ' / ' + P.PsnName) AS PsnNo     
                FROM B01_Card C 
                LEFT JOIN B01_Person P ON P.PsnID = C.PsnID  
                WHERE C.CardType = 'R' ";

            //設定查詢條件
            if (Request["query_CardNo"]!=null)
            {
                sql += " AND C.CardNo LIKE @CardNo ";               
                QueryCardNo = Request["query_CardNo"];
            }
            this.ListTempCard = this.odo.GetQueryResult<CardEntity>(sql, new { CardNo = "%" + QueryCardNo + "%" }).OrderByField(SortExpression, SortDire=="ASC"?true:false).ToList();

            //sql += " ORDER BY " + SortExpression + " " + SortDire;
            //取得查詢資料
            
            #endregion
        }

        /// <summary>
        /// 重新查詢資料並更新顯示於表格，以及取得資料目前的表格頁數
        /// </summary>
        /// <param name="mode">查詢模式</param>
        /// <param name="SortExpression">排序欄位</param>
        /// <param name="SortDire">排序定序</param>
        /// <returns>資料目前的表格頁數</returns>
        public int Query(string mode, string SortExpression, string SortDire)
        {            
            DataTable dt = new DataTable();
            List<string> liSqlPara = new List<string>();

            string sql = "";

            #region Process String - Get B01_Card
            sql = @"
                SELECT 
                    C.CardID, 
                    C.CardNo, 
                    C.CardDesc, 
                    CASE 
                        WHEN C.CardAuthAllow = 1 THEN '有效'
                        WHEN C.CardAuthAllow = 0 THEN '無效'
                    END AS CardAuthAllow, 
                    (CONVERT(VARCHAR, C.PsnID) + ' / ' + P.PsnName) AS PersonData  
                FROM B01_Card C 
                LEFT JOIN B00_ItemList L ON L.ItemClass = 'CardType' AND L.ItemNo = C.CardType  
                LEFT JOIN B01_Person P ON P.PsnID = C.PsnID 
                WHERE C.CardID IS NOT NULL AND C.CardType = 'R' ";

            if (!string.IsNullOrEmpty(ViewState["query_CardNo"].ToString().Trim()))
            {
                sql += " AND C.CardNo LIKE ? ";
                liSqlPara.Add("S:" + "%" + ViewState["query_CardNo"].ToString().Trim() + "%");
            }

            //設定查詢排序
            sql += " ORDER BY " + SortExpression + " " + SortDire;

            //取得查詢資料
         
            #endregion
         
            //完成查詢後並取得資料目前的表格頁數
            int find = 0;
            DataRow dr = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dr = dt.Rows[i];

                //if (hSelectValue.Value == dr["CardNo"].ToString().Trim())
                //{
                //    find = i;
                //    break;
                //}
            }

            return (find / _pagesize) + 1;
        }
        #endregion

        #region 五.載入、新增、編輯、刪除        


        /// <summary>
        /// 載入編輯刪除資料
        /// </summary>
        public void LoadData()
        {            
            Pub.MessageObject objRet = new Pub.MessageObject();
            List<string> liSqlPara = new List<string>();            

            bool IsOk = true;
            string sSql = "";
            string[] EditData = null;

            if (Request["CardNo"]==null || Request["CardNo"] == "")
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "傳入LoadData方法的卡片識別碼為空字串！";
                IsOk = false;
            }

            #region 取得載入畫面相關的欄位資料
            
            if (IsOk)
            {
                sSql = @"SELECT CardNo, CardDesc, CardAuthAllow FROM B01_Card WHERE CardNo = @CardNo";
                var Result = this.odo.GetQueryResult(sSql, new { CardNo = Request["CardNo"] });
                if (Result.Count()>0)
                {
                    EditData = new string[2];
                    EditData[0] = Result.First().CardNo;
                    if (Result.First().CardAuthAllow != 0)
                    {
                        EditData[1] = string.Format("卡號{0}借出無法刪除", Result.First().CardNo);
                        IsOk = false;
                    }
                    else
                    {
                        EditData[1] = Result.First().CardDesc;
                    }
                    
                }
                else
                {
                    EditData = new string[2];
                    EditData[0] = "Saho_SysErrorMassage";
                    EditData[1] = "系統中無此資料！";
                    IsOk = false;
                }
            }
            else
            {
                EditData = new string[2];
                EditData[0] = "Saho_SysErrorMassage";
                EditData[1] = "讀取資料失敗！";
            }
            #endregion            
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
             new {success =IsOk, resp = EditData }));
            Response.End();
        }

        /// <summary>
        /// 新增臨時卡建檔作業視窗相關的欄位資料
        /// </summary>                        
        public void Insert()
        {            
            Pub.MessageObject objRet = new Pub.MessageObject();         
            DateTime Time = DateTime.Now;

            int iResult = -1;
            string sSql = "";

            objRet.act = "Add";
            objRet.result = true;

            #region 輸入條件判斷
            
            sSql = " SELECT CardNo FROM B01_Card WHERE CardNo = @CardNo ";
            
            
            if (this.odo.GetQueryResult(sSql,new {CardNo=Request["CardNo"]}).Count()>0)
            {
                objRet.result = false;
                objRet.message = "CardNo is found duplicate " + Request["CardNo"] + "！";
            }

            // wei 20170207
            sSql = " SELECT TOP 1 [ParaValue] FROM [B00_SysParameter] WHERE ParaClass = 'CardID' AND ParaNo = 'CardLen' ";

            string strCardLen = this.odo.GetStrScalar(sSql);
                     
            int intCardNoLen = Request["CardNo"].Length;

            if (strCardLen == "")
            {
                strCardLen = "10";                
            }
            if (Int32.Parse(strCardLen) != intCardNoLen)
            {
                objRet.result = false;
                objRet.message = string.Format("卡片長度需為 {0} 碼", strCardLen);
            }

            #endregion            
            #region 新增欄位資料至資料庫
            string CardVer = "";
            if(this.odo.GetStrScalar("SELECT ParaValue FROM B00_SysParameter WHERE ParaClass = 'CardID' AND ParaNo = 'CardVer' ") == "Y")
            {
                CardVer = "0";
            }
            if (objRet.result)
            {
                sSql = @"INSERT INTO B01_Card 
                (
                    CardNo, CardSerialNo, CardVer, CardType, CardAuthAllow, CardSTime, CardETime, CardDesc, CreateUserID, CreateTime 
                ) VALUES (@CardNo, '', @CardVer, 'R', 0, @CardSTime, '2099/12/31 23:59:59', @CardDesc, @UserID, GETDATE())";
                //string strTime = Time.ToString("yyyy-MM-dd HH:mm:ss.fff");            
                iResult = this.odo.Execute(sSql, new {CardVer=CardVer, CardNo = Request["CardNo"], CardDesc = Request["CardDesc"], UserID = Sa.Web.Fun.GetSessionStr(this, "UserID"), CardSTime = Time });
            }           
            //這裡要設定log
            if (this.odo.isSuccess)
            {
                objRet.result = true;
                objRet.message = Request["CardNo"];
            }
            else
            {
                objRet.result = false;               
                objRet.message = this.odo.DbExceptionMessage;
            }            
            #endregion

            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(objRet));
            Response.End();

        }//end method


        /// <summary>
        /// 修改臨時卡建檔作業視窗相關的欄位資料
        /// </summary>                
        public void Update()
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            DateTime Time = DateTime.Now;

            int iResult = -1;
            string sSql = "";

            objRet.act = "Update";
            objRet.result = true;

            #region 輸入條件判斷

            
            #endregion

            #region update 欄位資料至資料庫
            if (objRet.result)
            {
                sSql = @"UPDATE B01_Card SET CardDesc=@CardDesc,UpdateTime=GETDATE(),UpdateUserID=@UserID WHERE CardNo=@CardNo";
                //string strTime = Time.ToString("yyyy-MM-dd HH:mm:ss.fff");            
                iResult = this.odo.Execute(sSql, new { CardNo = Request["CardNo"], CardDesc = Request["CardDesc"], UserID = Sa.Web.Fun.GetSessionStr(this, "UserID")});
            }
            //這裡要設定log
            if (iResult > 0)
            {
                objRet.result = true;
                objRet.message = Request["CardNo"];
                //oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, UserID, UserName, "0106", "", "", string.Format("新增卡片號碼：[{0}] 的資料成功", sCardNo.Trim()), "B01_Card");
            }
            else
            {
                objRet.result = false;
                objRet.message = "temporary card data create fails！";
                SysLogEntity log = SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.資料修改, Sa.Web.Fun.GetSessionStr(this, "UserID"), "", this.Request.Url.PathAndQuery);
                log.LogInfo = "修改臨時卡 " + Request["CardNo"];
                this.odo.SetSysLogCreate(log);
            }
            #endregion

            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(objRet));
            Response.End();

        }//end method


        public void Delete()
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            DateTime Time = DateTime.Now;

            int iResult = -1;
            string sSql = "";

            objRet.act = "Delete";
            objRet.result = true;

            #region 輸入條件判斷


            #endregion

            #region update 欄位資料至資料庫
            if (objRet.result)
            {
                sSql = @"Delete B01_Card WHERE CardNo=@CardNo";                
                iResult = this.odo.Execute(sSql, new { CardNo = Request["CardNo"]});
            }
            //這裡要設定log
            if (iResult > 0)
            {
                objRet.result = true;
                objRet.message = Request["CardNo"];
                SysLogEntity log = SysLogService.GetSysLogEntityPackage(DB_Acs.Logtype.資料刪除, Sa.Web.Fun.GetSessionStr(this, "UserID"), "", this.Request.Url.PathAndQuery);
                log.LogInfo = "刪除臨時卡 " + Request["CardNo"];
                this.odo.SetSysLogCreate(log);
                //oAcsDB.WriteLog(DB_Acs.Logtype.資料新增, UserID, UserName, "0106", "", "", string.Format("新增卡片號碼：[{0}] 的資料成功", sCardNo.Trim()), "B01_Card");
            }
            else
            {
                objRet.result = false;
                objRet.message = "temporary card data create fails！";
            }
            #endregion

            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(objRet));
            Response.End();

        }//end method

        #endregion


    }//end page class
}//end namespace