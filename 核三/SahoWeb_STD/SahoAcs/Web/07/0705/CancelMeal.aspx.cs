using DapperDataObjectLib;
using PagedList;
using SahoAcs.DBModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SahoAcs.Web._07._0705
{
    public partial class CancelMeal : System.Web.UI.Page
    {
        #region Global block
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public DataTable DataResult = new DataTable();
        public List<PersonEntity> PersonList = new List<PersonEntity>();
        public List<OrgDataEntity> OrgDataInit = new List<OrgDataEntity>();
        public List<ColNameObj> ListCols = new List<ColNameObj>();
        public List<LogState> LogStatus = new List<LogState>();
        public List<B03_MealOrder> ListLog = new List<B03_MealOrder>();


        public IPagedList<B03_MealOrder> PagedList;
        #endregion End 分頁參數
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null && Request["PageEvent"] == "QueryPerson")
            {
                QueryPerson();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "Cancel")
            {
                Cancel();
            }
            else
            {
                this.Calendar_CardTimeSDate.DateValue = DateTime.Now.ToString("yyyy/MM/dd");
            }
            ClientScript.RegisterClientScriptInclude("JsInclude1", "CancelMeal.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsCheck", "/Scripts/Check/JS_CHECK.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("JsUtil", "/Scripts/Check/JS_UTIL.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("QueryTool", "/uc/QueryTool.js?" + DateTime.Now.ToString("yyyyMMddHHmmss"));//加入同一頁面所需的JavaScript檔案
        }

        protected void QueryPerson()
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            string CardNo = Request["CardNo"];
            string PsnNo = Request["PsnNo"];
            string PsnName = Request["PsnName"];
            PersonEntity entity = new PersonEntity();
            string Sql = string.Empty;
            Sql = @"Select
P.PsnID,
P.PsnNo,
P.PsnName,
C.CardNo, 
C.CardVer,
IsNull(P.MealFood,0) As MealFood
From B01_Person P
Inner Join B01_Card C On P.PsnID = C.PsnID
WHERE 1 = 1 ";
            if (!string.IsNullOrEmpty(CardNo))
                Sql += " And C.CardNo = '" + CardNo + "' ";
            if (!string.IsNullOrEmpty(PsnNo))
                Sql += " And PsnNo = '" + PsnNo + "' ";
            if (!string.IsNullOrEmpty(PsnName))
                Sql += " And PsnName = '" + PsnName + "' ";
            var Result = this.odo.GetQueryResult<PersonEntity>(Sql, new
            {
                CardNo = CardNo,
                PsnNo = PsnNo,
                PsnName = PsnName
            });
            if (Result.Count() != 0)
            {
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new
                {
                    message = "OK",
                    success = true,
                    resp = entity,
                    mga_list = Result
                }));
                Response.End();
            }
            else
            {
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new
                {
                    message = "查無人員！\n請再確認所輸入的人員資料!!",
                    success = false,
                    resp = entity
                }));
                Response.End();
            }

        }

        protected void Cancel()
        {
            B03_MealOrder entity = new B03_MealOrder();
            string CardNo = Request["CardNo"];
            string PsnNo = Request["PsnNo"];
            string PsnName = Request["PsnName"];
            string SDate = Request["SDate"];
            string MealNo = Request["MealNo"];
            int MealFood = 0;

            //DateTime P_StaTm = DateTime.Parse(SDate);
            //if (P_StaTm.CompareTo(DateTime.Now.AddDays(-1)) > 0)    //
            //{
            //    Pub.MessageObject sRet = new Pub.MessageObject()
            //    {
            //        result = true,
            //        act = "Insert",
            //        message = "取消日，不可大於今日，請確認！"
            //    };
            //    Response.Clear();
            //    Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(sRet));
            //    Response.End();
            //    return;
            //}


            string Sql = string.Empty;

            Sql = @"Select
P.PsnID,
P.PsnNo,
P.PsnName,
C.CardNo, 
C.CardVer,
IsNull(P.MealFood,0) As MealFood
From B01_Person P
Inner Join B01_Card C On P.PsnID = C.PsnID
WHERE 1 = 1 ";
            if (!string.IsNullOrEmpty(CardNo))
                Sql += " And C.CardNo = '" + CardNo + "' ";
            if (!string.IsNullOrEmpty(PsnNo))
                Sql += " And PsnNo = '" + PsnNo + "' ";
            if (!string.IsNullOrEmpty(PsnName))
                Sql += " And PsnName = '" + PsnName + "' ";
            var PersonData = this.odo.GetQueryResult<PersonEntity>(Sql, new
            {
                CardNo = CardNo,
                PsnNo = PsnNo,
                PsnName = PsnName
            });

            if (PersonData.Count() != 0)
            {
                foreach (var o in PersonData)
                {
                    MealFood = int.Parse(o.MealFood);
                }
            }

            Sql = @"SELECT	
RecordID, OrderTime, MealDate, MealNo, CardNo, OrderSrc, CancelSrc, [Status], ProcTime, MealFood
From B03_MealOrder
WHERE 1 = 1 ";

            Sql += " And CONVERT(varchar(8), OrderTime, 112)  = '" + SDate.Replace(@"/", "") + "' ";
            Sql += " And CardNo = '" + CardNo + "' ";
            Sql += " And MealNo = '" + MealNo + "' ";
            var Result = this.odo.GetQueryResult<B03_MealOrder>(Sql, new
            {
                SDate = SDate,
                MealNo = MealNo,
                CardNo = CardNo
            });

            if (Result.Count() == 0)
            {
                Pub.MessageObject sRet = new Pub.MessageObject()
                {
                    result = true,
                    act = "Fail",
                    message = "查無該員！\n" + SDate + " 訂餐資料!!！"
                };
                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(sRet));
                Response.End();
            }
            else
            {
                int status = 0;
                int RecordID = 0;
                if (Result.Count() != 0)
                {
                    foreach (var o in Result)
                    {
                        status = o.Status;
                        RecordID = o.RecordID;

                        if (status != 5)
                        {
                            if (RecordID != 0)
                            {
                                this.odo.Execute(@"Update B03_MealOrder SET
                       MealDate=@MealDate,OrderSrc=6,CancelSrc=1,Status=6,ProcTime=@ProcTime 
                       Where  RecordID=@RecordID ",
                                  new
                                  {
                                      MealDate = SDate,
                                      ProcTime = Convert.ToDateTime(SDate),
                                      RecordID = RecordID
                                  });

                                string ErrMsg = this.odo.DbExceptionMessage;
                                if (string.IsNullOrEmpty(ErrMsg))
                                {
                                    InsertLog("Update", CardNo, SDate);
                                }
                                Pub.MessageObject sRet = new Pub.MessageObject()
                                {
                                    result = true,
                                    act = "Update",
                                    message = "取消成功！"
                                };
                                Response.Clear();
                                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(sRet));
                                Response.End();
                            }
                        }
                        else
                        {
                            InsertLog("Fail", CardNo, SDate);
                            Pub.MessageObject sRet = new Pub.MessageObject()
                            {
                                result = true,
                                act = "Update",
                                message = "該員, 這一天的這一餐，已經有 [取消用餐] 過了！"
                            };
                            Response.Clear();
                            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(sRet));
                            Response.End();
                        }
                    }
                }
            }
        }

        void InsertLog(string LogType, string CardNo, string OrderDate)
        {
            switch (LogType)
            {
                case "Insert":
                    this.odo.Execute(@"INSERT INTO B00_SysLog (LogTime, LogType, UserID , UserName, LogFrom, EquNo, EquName, LogInfo, LogIP) 
                                            VALUES (GETDATE(), @LogType, @UserID, @UserName, @LogFrom, @EquNo, @EquName, @LogInfo, @LogIP)",
                                       new
                                       {
                                           UserID = Sa.Web.Fun.GetSessionStr(this, "UserID"),
                                           UserName = Sa.Web.Fun.GetSessionStr(this, "UserName"),
                                           LogFrom = "ZZ0705",
                                           LogType = DB_Acs.Logtype.資料新增.ToString(),
                                           EquNo = "",
                                           EquName = "",
                                           LogInfo = "取消用餐(Web)寫入成功，CardNo :  " + CardNo + " OrderDate : " + OrderDate,
                                           LogIP = Request.UserHostAddress
                                       });
                    break;
                case "Update":
                    this.odo.Execute(@"INSERT INTO B00_SysLog (LogTime, LogType, UserID , UserName, LogFrom, EquNo, EquName, LogInfo, LogIP) 
                                            VALUES (GETDATE(), @LogType, @UserID, @UserName, @LogFrom, @EquNo, @EquName, @LogInfo, @LogIP)",
                                       new
                                       {
                                           UserID = Sa.Web.Fun.GetSessionStr(this, "UserID"),
                                           UserName = Sa.Web.Fun.GetSessionStr(this, "UserName"),
                                           LogFrom = "ZZ0705",
                                           LogType = DB_Acs.Logtype.資料修改.ToString(),
                                           EquNo = "",
                                           EquName = "",
                                           LogInfo = "取消用餐(Web)修改成功，CardNo :  " + CardNo + " OrderDate : " + OrderDate,
                                           LogIP = Request.UserHostAddress
                                       });
                    break;
                case "Fail":
                    this.odo.Execute(@"INSERT INTO B00_SysLog (LogTime, LogType, UserID , UserName, LogFrom, EquNo, EquName, LogInfo, LogIP) 
                                            VALUES (GETDATE(), @LogType, @UserID, @UserName, @LogFrom, @EquNo, @EquName, @LogInfo, @LogIP)",
                                       new
                                       {
                                           UserID = Sa.Web.Fun.GetSessionStr(this, "UserID"),
                                           UserName = Sa.Web.Fun.GetSessionStr(this, "UserName"),
                                           LogFrom = "ZZ0705",
                                           LogType = DB_Acs.Logtype.資料修改.ToString(),
                                           EquNo = "",
                                           EquName = "",
                                           LogInfo = "取消用餐(Web)-嘗試了重覆取消，CardNo :  " + CardNo + " OrderDate : " + OrderDate,
                                           LogIP = Request.UserHostAddress
                                       });
                    break;
            }
        }
    }
}