using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Script.Serialization;

using DapperDataObjectLib;
using SahoAcs.DBModel;
using SahoAcs.DBClass;
using PagedList;

namespace SahoAcs.Web._06._0640
{
    public partial class EquMapAliveList : System.Web.UI.Page
    {
        public List<VMcrinfo> McrInfos = new List<VMcrinfo>();
        public string MapSrc = "";

        OrmDataObject od = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public List<MapBackground> MapDataList = new List<MapBackground>();

        public int PageIndex = 1;
        public string PsnNo = "";
        public int StartPage = 0;
        public int EndPage = 0;
        public int ShowPage = 5;
        public int NextPage = 1;
        public int PrePage = 1;
        public string PsnID = "";
        public string AuthList = "";
        public IPagedList<MapBackground> PagedList;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string sql = "SELECT * From B03_MapBackground where pictype='Equ' And IsOpen=1 order by PicID ";
                this.MapDataList = this.od.GetQueryResult<MapBackground>(sql).ToList();
                this.PagedList = this.MapDataList.ToPagedList(PageIndex, 100);
            }
            catch (Exception ex)
            {
                Sa.Fun.EventLog(ex.GetBaseException().Message);
            }

            ClientScript.RegisterClientScriptInclude("EquMapAliveList", "EquMapAliveList.js");//加入同一頁面所需的JavaScript檔案
        }
    }
}