using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
//using SahoAcs.DBClass;
using SahoAcs.DBModel;

namespace SahoAcs.Web
{
    public partial class UploadPic : Sa.BasePage
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public string PsnID = "0";
        public string ErrorMessage = "";
        public string oPicStr = "";
        //public string oPicPara = "File";
        public string oUrl = "http://127.0.0.1:8080/";
        //public string oPicPath = "";
        //public string oPicSource = "";
        //public string oPsnImg = "";

        //public CardEntity cardentity = new CardEntity();

        public PersonEntity PsnEntity = new PersonEntity();



        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Save")
            {
                Response.Clear();
                //this.SetChangeCard();
                this.SetUploadFile();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                   new { message = "OK", ErrorMessage = this.ErrorMessage }));
                Response.End();
            }
            else
            {
                this.QueryPic();
            }
        }//end page_load

        private void SetUploadFile()
        {
            string sFilePath = Server.MapPath("~/" + Pub.PsnPhotoUrl);
            if (!Directory.Exists(sFilePath)) Directory.CreateDirectory(sFilePath);

            for (int cnt = 0; cnt < Request.Files.Count; cnt++)
            {
                this.PsnID = Request["PsnID"].ToString();
                string uploadname = Request["PsnIDNum"].ToString() + "." + Request.Files[cnt].FileName.Split('.')[1];
                string real_uploadname = sFilePath + "/" + uploadname;

                Request.Files[cnt].SaveAs(real_uploadname);
                this.odo.Execute("UPDATE B01_Person SET PsnPicSource=@PicSource WHERE PsnID=@PsnID", new { PicSource = uploadname, PsnID = this.PsnID });
            }
        }


        private void QueryPic()
        {
            string path = System.IO.Path.GetDirectoryName(this.Request.PhysicalPath);
            path = Request.Url.AbsoluteUri;
            path = path.Substring(0, path.ToLower().LastIndexOf(@"/web"));

            this.PsnID = Request["PsnID"];
            var PsnInfo = this.odo.GetQueryResult<PersonEntity>(@"SELECT * FROM B01_Person WHERE PsnID=@PsnID",
                            new { PsnID = PsnID }).ToList();
            foreach (var o in PsnInfo)
            {
                this.PsnEntity = o;
                if (string.IsNullOrEmpty(PsnEntity.PsnPicSource))
                {
                    PsnEntity.PsnPicSource = "/Img/default.png";
                }
            }

            if (PsnEntity.PsnPicSource != "/Img/default.png")
            {
                string FilePath = HttpContext.Current.Server.MapPath("~/" + Pub.PsnPhotoUrl + "/" + PsnEntity.PsnPicSource);
                if (System.IO.File.Exists(FilePath))
                {
                    oPicStr = "/" + Pub.PsnPhotoUrl + "/" + PsnEntity.PsnPicSource;
                } else
                {
                    oPicStr = "/Img/default.png";
                }
            } else
            {
                oPicStr = "/Img/default.png";
            }

            oPicStr = path + oPicStr + "?ts=" + System.DateTime.Now.ToString("yyyyMMddhhmmss");

        }

    }//end class
}//end namespace