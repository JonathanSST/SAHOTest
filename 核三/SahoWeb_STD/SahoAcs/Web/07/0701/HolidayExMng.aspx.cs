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
using DapperDataObjectLib;
using SahoAcs.DBModel;
using SahoAcs.DBClass;
using PagedList;


namespace SahoAcs._0701
{
    public partial class HolidayExMng : System.Web.UI.Page
    {
        #region Main Description
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        private int _pagesize = 50;        //DataGridView每頁顯示的列數
        private static string MsgDuplicate = "";
        public IPagedList<B00Holidayex> PagedList;
        public string SortName = "HEDate";
        public string SortType = "ASC";
        #region 分頁設定
        public int PageIndex = 1;

        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        #endregion

        #endregion


        #region LoadProcess
        private void LoadProcess()
        {            

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "\nsetTimeout('SetHighLifthEnabled()', 500);";
            js += "\nSetMode('');";
            js += "\nSetUserLevel('" + Session["FunAuthSet"].ToString() + "');";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("HolidayEx", "HolidayExMng.js?"+Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "msg", "YYData();", true);
        }
        #endregion

        #region RegisterObj
        private void RegisterObj()
        {
            //設定DataGridView每頁顯示的列數
            //this.MainGridView.PageSize = _pagesize;
        }
        #endregion

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            MsgDuplicate = this.GetLocalResourceObject("MsgDouble").ToString();
            this.YYData();
            hUserId.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            //hMenuNo.Value = Sa.Web.Fun.GetSessionStr(this.Page, "MenuNo");
            //hMenuName.Value = Sa.Web.Fun.GetSessionStr(this.Page, "MenuName");
            LoadProcess();
            RegisterObj();

            if (!IsPostBack)
            {
                if (this.GetFormEqlValue("PageEvent").Equals("Query"))
                {
                    this.SelectYear.Value = this.GetFormEndValue("Year");
                    //Query(true, this.SortName, this.SortType);
                }
                if (this.GetFormEqlValue("PageEvent").Equals("Build"))
                {
                    this.SelectYear.Value = this.GetFormEndValue("Year");
                    BulidHolidayEx();       //建立例假日資訊          
                    //Query(true, this.SortName, this.SortType);
                }
                if (this.GetFormEqlValue("PageEvent").Equals("Add"))
                {
                    this.Insert();
                }
                if (this.GetFormEqlValue("PageEvent").Equals("Update"))
                {
                    this.Update();
                }
                if (this.GetFormEndValue("PageEvent").Equals("Delete"))
                {
                    this.Delete();
                }
                Query(true, this.SortName, this.SortType);
            }
            else
            {

            }
        }
        #endregion
        

        #region 其他方法

        #region YYData
        public void YYData()
        {            
            String sData = "";            
            int iYear = int.Parse(DateTime.Now.ToString("yyyy"));
            for (int i = 0; i < 3; i++)
            {
                this.Input_Year.Items.Add(new ListItem() { Text = iYear.ToString(), Value = iYear.ToString() });
                iYear++;
            }           
            //return EditData;
        }
        #endregion


        #region 產生假日資料
        private void BulidHolidayEx()
        {
            if (SelectYear.Value == "")
                return;
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            List<string> liSqlPara = new List<string>();
            List<string> lis = new List<string>();
            String sYear = SelectYear.Value;
            String sYear2 = (int.Parse(sYear) + 1).ToString();
            DateTime startDate = DateTime.Parse(sYear + "-01-01 00:00:00");
            DateTime endDate = DateTime.Parse(sYear2 + "-01-01 00:00:00");

            string sql = "";
            int istat = 0;
            oAcsDB.BeginTransaction();
            while (startDate < endDate)
            {
                if ((int)startDate.DayOfWeek == 0)
                {
                    sql = " if not exists (select HEID from B00_HolidayEx where HEDate = ?) begin insert into B00_HolidayEx(HEDate,HEDesc,CreateUserID) values(?,?,?) end ";
                    liSqlPara.Add("S:" + startDate.ToString("yyyy-MM-dd"));
                    liSqlPara.Add("S:" + startDate.ToString("yyyy-MM-dd"));
                    liSqlPara.Add("S:" + "星期日");
                    liSqlPara.Add("S:" + hUserId.Value);
                }
                else if ((int)startDate.DayOfWeek == 6)
                {
                    sql = " if not exists (select HEID from B00_HolidayEx where HEDate = ?) begin insert into B00_HolidayEx(HEDate,HEDesc,CreateUserID) values(?,?,?) end ";
                    liSqlPara.Add("S:" + startDate.ToString("yyyy-MM-dd"));
                    liSqlPara.Add("S:" + startDate.ToString("yyyy-MM-dd"));
                    liSqlPara.Add("S:" + "星期六");
                    liSqlPara.Add("S:" + hUserId.Value);
                }
                else
                    sql = "";
                if (sql != "")
                {
                    istat += oAcsDB.SqlCommandExecute(sql, liSqlPara);
                    liSqlPara.Clear();
                }
                startDate = startDate.AddDays(1);
            }

            if (istat > -1)
                oAcsDB.Commit();
            else
                oAcsDB.Rollback();

            hSelectState.Value = "true";
        }
        #endregion


        #region 記載查詢條件的紀錄，防止頁數按鈕切換時查詢錯誤
        private void CatchSession(List<String> Data)
        {
            String datalist = "";
            for (int i = 0; i < Data.Count; i++)
                datalist += Data[i] + "|";
            Session["OldSearchList"] = datalist;
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

        #region 查詢

        private void Query(bool select_state, string SortExpression, string SortDire)
        {
            string sYear = SelectYear.Value;
            int iPageIndex = 1;
            string sql = "";            
            List<String> CheckData = new List<String>();
            List<string> liSqlPara = new List<string>();
            String NowCondition = "", NowYear = "", wheresql = "";

            if (select_state)
            {
                CheckData.Add(sYear);
                CatchSession(CheckData);
                NowYear = sYear;
            }

            #region Process String
            sql = " SELECT * FROM B00_HolidayEx ";

            //設定查詢條件
            if (string.IsNullOrEmpty(NowYear))
            {
                NowYear = DateTime.Now.Year.ToString();
                SelectYear.Value = NowYear;
            }           
            wheresql += " WHERE HEDate >= @Start AND HEDate <= @End ";
            liSqlPara.Add(NowYear + "-01-01");
            liSqlPara.Add(NowYear + "-12-31");

            sql += wheresql + " ORDER BY HEDate ";
            #endregion
            var result = this.odo.GetQueryResult<B00Holidayex>(sql,new {Start=liSqlPara[0], End=liSqlPara[1] });
            if (!this.GetFormEqlValue("Index").Equals(""))
            {
                PageIndex = int.Parse(this.GetFormEqlValue("Index"));
            }
            this.PagedList = result.ToPagedList(PageIndex, _pagesize);
            hDataRowCount.Value = result.Count().ToString();
            SetDoPage();
        }

        private void SetDoPage()
        {
            //啟始及結束頁
            StartPage = (PageIndex < ShowPage) ? 1 : PageIndex;
            if (StartPage > 1)
            {
                StartPage = (PageIndex + ShowPage / 2 >= this.PagedList.PageCount) ? this.PagedList.PageCount - ShowPage + 1 : PageIndex - ShowPage / 2;
            }
            EndPage = (StartPage - 1 > this.PagedList.PageCount - ShowPage) ? this.PagedList.PageCount + 1 : StartPage + ShowPage;
            //上下頁
            PrePage = PageIndex - 1 <= 1 ? 1 : PageIndex - 1;
            NextPage = PageIndex + 1 >= this.PagedList.PageCount ? this.PagedList.PageCount : PageIndex + 1;
        }

        #endregion

        #region Insert、Update、Delete 資料
        public void Insert()
        {
            string UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            string sql = " INSERT INTO B00_HolidayEx(HEDate,HEDesc,HEIsCus,CreateUserID) values(@Date,@Desc,1,@User) ";
            var effectRow = this.odo.Execute(sql, new {Date=this.GetFormEqlValue("HEDate"), Desc=this.GetFormEqlValue("HEDesc"), User=UserID });
            Pub.MessageObject sRet = new Pub.MessageObject();            
            if (effectRow > 0)
            {
                sRet.result = true;
                sRet.message = "新增完成";
            }
            else
            {
                sRet.result = false;
                sRet.message = "新增失敗，資料庫異常！";
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(sRet));
            Response.End();
        }

        public void Update()
        {
            string UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            string sql = " UPDATE B00_HolidayEx SET HEDate=@Date, HEDesc=@Desc, UpdateUserID=@User, UpdateTime=GETDATE() WHERE HEID=@HEID";
            var effectRow = this.odo.Execute(sql, new { Date = this.GetFormEqlValue("HEDate"), Desc = this.GetFormEqlValue("HEDesc"), User = UserID, HEID=this.GetFormEndValue("HEID") });
            Pub.MessageObject sRet = new Pub.MessageObject();
            if (effectRow > 0)
            {
                sRet.result = true;
                sRet.message = "修改完成";
            }
            else
            {
                sRet.result = false;
                sRet.message = "修改失敗，資料庫異常！";
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(sRet));
            Response.End();
        }

        public void Delete()
        {
            string UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            string sql = " Delete B00_HolidayEx WHERE HEID=@HEID";
            var effectRow = this.odo.Execute(sql, new {User = UserID, HEID = this.GetFormEndValue("HEID") });
            Pub.MessageObject sRet = new Pub.MessageObject();
            if (effectRow > 0)
            {
                sRet.result = true;
                sRet.message = "刪除完成";
            }
            else
            {
                sRet.result = false;
                sRet.message = "刪除失敗，資料庫異常！";
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(sRet));
            Response.End();
        }
        #endregion


        #region 載入單筆資料
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] LoadData(string HEID, String UserID, String mode)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            String sql = "";
            
            string[] EditData = null;
            List<string> liSqlPara = new List<string>();
            dr = null;

            #region Process String
            sql = @" SELECT * FROM B00_HolidayEx WHERE HEID = ? ";
            liSqlPara.Add("S:" + HEID.Trim());
            oAcsDB.GetDataReader(sql, liSqlPara, out dr);
            if (dr.Read())
            {
                EditData = new string[dr.DataReader.FieldCount];
                for (int i = 0; i < dr.DataReader.FieldCount; i++)
                    EditData[i] = dr.DataReader[i].ToString();
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

        #region GridView處理
       
        

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
                //ProcessGridView.RowStyle.HorizontalAlign = HorizontalAlign.Center;
            }
        }
        #endregion

        #endregion

        #region JavaScript及aspx共用方法

        
        #region MMData
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static String[] MMData(String CtrlID)
        {
            string[] EditData = null;
            String sData = "";
            EditData = new string[3];
            sData += CtrlID + "|";
            for (int i = 0; i < 12; i++)
            {
                sData += (i + 1).ToString().PadLeft(2, '0') + "|" + (i + 1).ToString().PadLeft(2, '0') + "|";
            }
            EditData[EditData.Length - 1] = sData.Substring(0, sData.Length - 1);
            return EditData;
        }
        #endregion

        #region DDData
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static String[] DDData(String CtrlID)
        {
            string[] EditData = null;
            String sData = "";
            EditData = new string[3];
            sData += CtrlID + "|";
            for (int i = 0; i < 31; i++)
            {
                sData += (i + 1).ToString().PadLeft(2, '0') + "|" + (i + 1).ToString().PadLeft(2, '0') + "|";
            }
            EditData[EditData.Length - 1] = sData.Substring(0, sData.Length - 1);
            return EditData;
        }
        #endregion

        #endregion


    }
}