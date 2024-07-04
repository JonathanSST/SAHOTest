using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sa.DB;
using System.Web.Configuration;

namespace SahoAcs
{
    public partial class Manual : System.Web.UI.Page
    {
        public class ManualDoc{
            public string DocName { get; set; }
            public string DocPath { get; set; }
            public string DocKey { get; set; }
        }

        private string Install = "商合行SMS管理系統安裝手冊.pdf";
        private string Operating = "商合行SMS管理系統操作手冊.pdf";
        private string FlowDoc1 = "權限簡易設定流程圖.pdf";
        private string FlowDoc2 = "資料與權限關係表.pdf";
        private string FlowDoc3 = "資料與權限簡易設定表.pdf";
        private string FlowDoc4 = "商合行SMS管理系統預設資料與權限簡易設定流程表.pdf";        
        public Dictionary<string, string> DicManual = new Dictionary<string, string>();
        public List<ManualDoc> doc_list = new List<ManualDoc>();
        #region Page_Load


        protected void Page_Load(object sender, EventArgs e)
        {
            DicManual.Add("Install", "商合行SMS管理系統安裝手冊.pdf");
            DicManual.Add("Operating", "商合行SMS管理系統操作手冊.pdf");
            DicManual.Add("FlowDoc1", "權限簡易設定流程圖.pdf");
            DicManual.Add("FlowDoc2", "資料與權限關係表.pdf");
            DicManual.Add("FlowDoc3", "資料與權限簡易設定表.pdf");
            DicManual.Add("FlowDoc4", "SMS管理系統預設資料與權限簡易設定流程表.pdf");
            doc_list.Add(new ManualDoc()
            {
                DocKey = "Install",
                DocName = "商合行SMS管理系統安裝手冊",
                DocPath = "商合行SMS管理系統安裝手冊.pdf"
            });
            doc_list.Add(new ManualDoc()
            {
                DocKey = "Operating",
                DocName = "商合行SMS管理系統操作手冊",
                DocPath = "商合行SMS管理系統操作手冊.pdf"
            });
            doc_list.Add(new ManualDoc()
            {
                DocKey = "FlowDoc1",
                DocName = "權限簡易設定流程圖",
                DocPath = "商合行SMS管理系統功能權限簡易設定流程表.pdf"
            });
            doc_list.Add(new ManualDoc()
            {
                DocKey = "FlowDoc2",
                DocName = "資料與權限關係表",
                DocPath = "商合行SMS管理系統資料與權限關係表.pdf"
            });
            doc_list.Add(new ManualDoc()
            {
                DocKey = "FlowDoc3",
                DocName = "資料與權限簡易設定表",
                DocPath = "商合行SMS管理系統資料與權限簡易設定流程表.pdf"
            });
            doc_list.Add(new ManualDoc()
            {
                DocKey = "FlowDoc4",
                DocName = "SMS管理系統預設資料與權限簡易設定流程表",
                DocPath = "商合行SMS管理系統預設資料與權限簡易設定流程表.pdf"
            });
            foreach (var s in this.doc_list)
            {
                if (WebConfigurationManager.AppSettings[s.DocKey] != null)
                {
                    s.DocPath = WebConfigurationManager.AppSettings[s.DocKey];
                }            
            }
            this.Repeater1.DataSource = this.doc_list;
            this.Repeater1.DataBind();
        }
        #endregion

        #region FileLink_Click
        protected void FileLink_Click(object sender, EventArgs e)
        {
            try
            {
                //網站 根目錄+檔案位置路徑
                string f = Request.PhysicalApplicationPath + "/doc/" + this.doc_list.Where(i=>
                    i.DocKey==((LinkButton)sender).CommandArgument).FirstOrDefault().DocPath;
                System.Net.WebClient wc = new System.Net.WebClient();
                byte[] a = wc.DownloadData(f);
                string FileName = System.IO.Path.GetFileName(f);
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", Server.UrlPathEncode(FileName)));
                Response.BinaryWrite(a);
            }
            catch (Exception ex)
            {
                string err_msg = ex.Message;
            }
        }

        #endregion


        #region File_1_Click
        protected void File_1_Click(object sender, EventArgs e)
        {
            try
            {
                //網站 根目錄+檔案位置路徑
                string f = Request.PhysicalApplicationPath + "/doc/" + this.Install;
                System.Net.WebClient wc = new System.Net.WebClient();
                byte[] a = wc.DownloadData(f);
                string FileName = System.IO.Path.GetFileName(f);
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", Server.UrlPathEncode(FileName)));
                Response.BinaryWrite(a);
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region File_2_Click
        protected void File_2_Click(object sender, EventArgs e)
        {
            try
            {
                //網站 根目錄+檔案位置路徑
                string f = Request.PhysicalApplicationPath + "/doc/" + this.Operating;
                System.Net.WebClient wc = new System.Net.WebClient();
                byte[] a = wc.DownloadData(f);
                string FileName = System.IO.Path.GetFileName(f);
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", Server.UrlPathEncode(FileName)));
                Response.BinaryWrite(a);
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}