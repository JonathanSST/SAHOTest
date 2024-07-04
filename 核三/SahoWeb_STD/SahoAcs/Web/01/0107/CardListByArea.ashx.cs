using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using SahoAcs.DBModel;
using DapperDataObjectLib;

namespace SahoAcs.Web._01._0107
{
    /// <summary>
    /// CardListByArea 的摘要描述
    /// </summary>
    public class CardListByArea : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
            string json = "";
            using (var reader = new System.IO.StreamReader(context.Request.InputStream))
            {
                json = reader.ReadToEnd();
            }
            dynamic resp = new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(json);
            List<CardEntity> cardlist = new List<DBModel.CardEntity>();
            OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
            string cmdstr = "SELECT * FROM B01_Card WHERE CardType='R' ";
            if (resp["Area"].ToString() != "")
            {
                cmdstr += " AND Rev02=@Area ";
            }
            cardlist = odo.GetQueryResult<CardEntity>(cmdstr,new {Area=resp["Area"].ToString() }).ToList();
            context.Response.Clear();
            context.Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(cardlist));
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        [System.Web.Services.WebMethod]
        public static List<CardEntity> GetCardList(string Area)
        {
            List<CardEntity> cardlist = new List<DBModel.CardEntity>();
            OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
            cardlist = odo.GetQueryResult<CardEntity>("SELECT * FROM B01_Card WHERE CardType='R' ").ToList();
            return cardlist;
        }


    }//end ashx class
}//end namespace