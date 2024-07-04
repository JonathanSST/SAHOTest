using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;



namespace SahoAcs.Web
{
    public partial class ShowNews : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public CardEntity entity = new CardEntity();
        public NewsInfoEntity news = new NewsInfoEntity();
        public CardLogFillData cardlog = new CardLogFillData();
        public List<CourseEntity> ListCourse = new List<CourseEntity>();

        protected void Page_Load(object sender, EventArgs e)
        {
            this.SetInit();
            if (Request["PageEvent"] != null && Request["PageEvent"]=="Add")
            {
                //this.news.NewsID = 0;
                //this.NewsDate.DateValue=string.Format("",this.news)
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Edit")
            {
                this.news.NewsID = int.Parse(Request["NewsID"].ToString());
                var QueryResult = this.odo.GetQueryResult<NewsInfoEntity>("SELECT * FROM B01_NewsInfo WHERE NewsID=@NewsID", this.news);
                foreach(var o in QueryResult)
                {
                    this.news = o;
                    //this.NewsDate.DateValue = string.Format("{0:yyyy/MM/dd}", this.news.NewsDate);
                }
            }
            else if(Request["PageEvent"]!=null && Request["PageEvent"] == "Save")
            {
                
            }
            else
            {
                //this.NewsDate.DateValue = string.Format("{0:yyyy/MM/dd}",DateTime.Now);
            }            
        }


        /// <summary>設定補登用餐記錄畫面</summary>
        public void SetInit()
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }



        public void SaveData()
        {            
            Response.Clear();
            Response.Write("新增完成");
            Response.End();                        
        }
    

    }//end class
}//end namespace