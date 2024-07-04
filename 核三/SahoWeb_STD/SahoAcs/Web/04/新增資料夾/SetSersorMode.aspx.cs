using PagedList;
using SahoAcs.DBModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBClass;
using OfficeOpenXml;

namespace SahoAcs
{
    public partial class SetSersorMode : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        public List<SensorData> SenList = new List<SensorData>();
        public List<string> ListLogPsnNo = new List<string>();

        public List<ColNameObj> ListCols = new List<ColNameObj>();

        public DataTable DataResult = new DataTable();
        public DataTable dt = null;
        public DataTable dtCheck = null;

        public int PageIndex = 1;

        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;

        public IPagedList<SensorData> PagedList;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Query")
            {
                this.SetInitData();
                this.SetQueryData();
                this.SetDoPage();
            }
            else if (Request["PageEvent"] != null && Request["PageEvent"] == "SetStatus")
            {
                this.SetInitData();
                this.SetStatus();
            }
            else
            {
                this.SetInitData();
                this.SetQueryData();
                this.SetDoPage();
            }

            ClientScript.RegisterClientScriptInclude("0414", "SetSersorMode.js?" + Pub.GetNowTime);//加入同一頁面所需的JavaScript檔案
        }

        private void SetInitData()
        {
            #region Give hideValue
            this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
            #endregion

            SetDDLCtrlItemList();
            SetDDLLocItemList();

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

        private void SetQueryData()
        {

            if (Request["PageIndex"] != null)
            {
                this.PageIndex = Convert.ToInt32(Request["PageIndex"]);
            }

            string sqlStr = "", sWhereSql = "";

            sqlStr = @"
                    SELECT 
                        Dci.DciID, IoMst.IOMstID, IoMst.IOMstStatus, Iom.IOMID, Iom.IOMNo, Iom.IOMName, Iom.IOMStatus, 
                        Sen.SenID, Sen.SenNo, Sen.SenName, dbo.Get_LocLongName(Sen.LocID, ' / ') AS 'LocLongName' ,
                        Sen.SenStatus
                    FROM B01_Sensor Sen 
                    LEFT JOIN B01_IOModule Iom ON Iom.IOMID = Sen.IOMID
                    LEFT JOIN B01_IOMaster IoMst ON IoMst.IOMstID = Iom.IOMstID
                    LEFT JOIN B01_DeviceConnInfo Dci ON Dci.DciID = IoMst.DciID
                    WHERE Iom.IOMUsage = 0
            ";

            #region 設定查詢條件
            string QueryCtrlModel = sCtrlModel.Value;
            string QueryArea = sLocArea.Value;
            string QueryBuilding = sLocBuilding.Value;
            string QueryFloor = sLocFloor.Value;

            if (!string.IsNullOrEmpty(QueryCtrlModel))
            {
                sWhereSql += $" AND IoMst.CtrlModel= '{QueryCtrlModel}' ";
            }

            if (!string.IsNullOrEmpty(QueryArea))
            {
                sWhereSql += $" AND dbo.Get_LocParentID(Sen.LocID, 'AREA') = {QueryArea} ";
            }

            if (!string.IsNullOrEmpty(QueryBuilding))
            {
                sWhereSql += $" AND dbo.Get_LocParentID(Sen.LocID, 'BUILDING') = {QueryBuilding} ";
            }

            if (!string.IsNullOrEmpty(QueryFloor))
            {
                sWhereSql += $" AND dbo.Get_LocParentID(Sen.LocID, 'FLOOR') = {QueryFloor} ";
            }
            #endregion

            sqlStr += (sWhereSql + " ORDER BY Dci.DciNo, IoMst.IOMstNo, Iom.IOMNo, Sen.SenID;");

            this.SenList = this.odo.GetQueryResult<SensorData>(sqlStr).ToList();

            //轉datatable
            this.PagedList = this.SenList.ToPagedList(PageIndex, 100);
            this.DataResult = OrmDataObject.IEnumerableToTable(PagedList);

            this.DataResult.Columns.Add(new DataColumn("RowStatus"));
            foreach (DataRow r in this.DataResult.Rows)
            {
                if (r["IOMstStatus"].ToString() == "0" || r["IOMstStatus"].ToString() == "2")
                {
                    r["RowStatus"] = "<span style='color:red'>連線裝置限制/停用</span>";
                }
                else if (r["IOMStatus"].ToString() == "0" || r["IOMStatus"].ToString() == "2")
                {
                    r["RowStatus"] = "<span style='color:red'>IO模組限制/停用</span>";
                }
                else if (r["SenStatus"].ToString() == "0" || r["SenStatus"].ToString() == "2")
                {
                    r["RowStatus"] = "<span style='color:red'>限制/停用</span>";
                }
                else
                {
                    r["RowStatus"] = "<span style='color:blue'>啟用</span>";
                }
            }

        }

        private void SetStatus()
        {
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sStatus = this.sSenStatus.Value.Trim();
            string SenIDList = this.sSaveCheckList.Value.Trim().TrimEnd('|');

            if (!string.IsNullOrEmpty(SenIDList))
            {
                string[] SenItems = SenIDList.Split(new char[] { '|' });

                if (SenItems.Length > 0)
                {
                    foreach (var Item in SenItems)
                    {
                        string sSql = "";
                        string[] tID = Item.Split(new char[] { ',' });

                        sSql += "UPDATE B01_Sensor SET SenStatus=@sStatus, isUpdate=1, UpdateUserID=@UserID, UpdateTime=getdate() WHERE SenID=@SenID; ";
                        sSql += "UPDATE B01_IOModule SET IOMStatus=@sStatus, isUpdate=1, UpdateUserID=@UserID, UpdateTime=getdate() WHERE IOMID=@IOMID; ";
                        sSql += "UPDATE B01_IOMaster SET IOMstStatus=@sStatus, isUpdate=1, UpdateUserID=@UserID, UpdateTime=getdate() WHERE IOMstID=@IOMstID; ";

                        var effectRow = this.odo.Execute(sSql, new
                        {
                            sStatus = sStatus,
                            UserID = Sa.Web.Fun.GetSessionStr(this.Page, "UserID"),
                            SenID = tID[2].ToString().Trim(),
                            IOMID = tID[1].ToString().Trim(),
                            IOMstID = tID[0].ToString().Trim()
                        });

                        if (effectRow > 0)
                        {
                            objRet.result = true;
                            objRet.message = "設定完成";
                            objRet.act = "Y";
                        }
                        else
                        {
                            objRet.result = false;
                            objRet.message = "設定失敗，資料庫異常！";
                            objRet.act = "Y";
                            break;
                        }
                    }
                }
                else
                {
                    objRet.result = false;
                    objRet.message = "沒有要設定的項目";
                    objRet.act = "N";
                }
            }
            else
            {
                objRet.result = false;
                objRet.message = "沒有要設定的項目";
                objRet.act = "N";
            }

            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(objRet));
            Response.End();



        }


        #region  載入下拉式選單

        private void SetDDLCtrlItemList()
        {
            System.Web.UI.WebControls.ListItem Item;

            this.ddlCtrlMode.Items.Clear();

            var result = this.odo.GetQueryResult("SELECT distinct CtrlModel FROM B01_IOMaster");

            if (result.Count() > 0)
            {
                foreach (var o in result)
                {
                    if (Convert.ToString(o.DepName) != "")
                    {
                        Item = new System.Web.UI.WebControls.ListItem();
                        Item.Text = Convert.ToString(o.CtrlModel);
                        Item.Value = Convert.ToString(o.CtrlModel);
                        this.ddlCtrlMode.Items.Add(Item);
                    }
                }

                this.ddlCtrlMode.Items.Insert(0, new ListItem("選取資料", ""));
            }
            else
            {
                this.ddlCtrlMode.Items.Add(new ListItem() { Text = "尚無資料", Value = "" });
            }
        }

        private void SetDDLLocItemList()
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt = new DataTable();

            sql = @"
                select * from (
                select LocType, LocID, LocNo, LocName, LocPID from B01_Location where LocType = 'AREA'
                UNION
                select LocType, LocID, LocNo, LocName, LocPID from B01_Location where LocType = 'BUILDING' 
                UNION 
                select LocType, LocID, LocNo, LocName, LocPID from B01_Location where LocType = 'FLOOR' 
                ) as R
                ORDER BY R.LocType, LocID
            ";

            this.txtAreaList.Value = "";
            this.txtBuildingList.Value = "";
            this.txtFloorList.Value = "";
            //this.ddlLocArea.Items.Clear();
            //this.ddlLocBuilding.Items.Clear();
            //this.ddlLocFloor.Items.Clear();

            oAcsDB.GetDataTable("LocInfo", sql, liSqlPara, out dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["LocType"].ToString() == "AREA")
                {
                    this.txtAreaList.Value += $"{dt.Rows[i]["LocID"].ToString()}|{dt.Rows[i]["LocNo"].ToString()}|{dt.Rows[i]["LocName"].ToString()}|{dt.Rows[i]["LocPID"].ToString()},";
                    //this.ddlLocArea.Items.Add(new ListItem("[" + dt.Rows[i]["LocNo"].ToString() + "]" + dt.Rows[i]["LocName"].ToString(), dt.Rows[i]["LocID"].ToString()));
                }
                if (dt.Rows[i]["LocType"].ToString() == "BUILDING")
                {
                    this.txtBuildingList.Value += $"{dt.Rows[i]["LocID"].ToString()}|{dt.Rows[i]["LocNo"].ToString()}|{dt.Rows[i]["LocName"].ToString()}|{dt.Rows[i]["LocPID"].ToString()},";
                    //this.ddlLocBuilding.Items.Add(new ListItem("[" + dt.Rows[i]["LocNo"].ToString() + "]" + dt.Rows[i]["LocName"].ToString(), dt.Rows[i]["LocID"].ToString()));
                }
                if (dt.Rows[i]["LocType"].ToString() == "FLOOR")
                {
                    this.txtFloorList.Value += $"{dt.Rows[i]["LocID"].ToString()}|{dt.Rows[i]["LocNo"].ToString()}|{dt.Rows[i]["LocName"].ToString()}|{dt.Rows[i]["LocPID"].ToString()},";
                    //this.ddlLocFloor.Items.Add(new ListItem("[" + dt.Rows[i]["LocNo"].ToString() + "]" + dt.Rows[i]["LocName"].ToString(), dt.Rows[i]["LocID"].ToString()));
                }
            }

            this.txtAreaList.Value = this.txtAreaList.Value.TrimEnd(',');
            this.txtBuildingList.Value = this.txtBuildingList.Value.TrimEnd(',');
            this.txtFloorList.Value = this.txtFloorList.Value.TrimEnd(',');
        }
        #endregion

    }

}