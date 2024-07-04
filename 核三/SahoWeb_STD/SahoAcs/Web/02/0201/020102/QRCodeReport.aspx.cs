using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;
using DapperDataObjectLib;
//using System.DirectoryServices;

namespace SahoAcs
{
    public partial class QRCodeReport : System.Web.UI.Page
    {
        
        public string Base64Image = "";

        public string EquName = "";

        public string EquBuilder = "";

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request["EquNo"] != null)
            {
                OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
                string content ="http://"+ Request.Url.Authority+"/CheckQrCode.aspx?DataInfo=" + Sa.Fun.Encrypt(Request["EquNo"]);
                var paradata = odo.GetQueryResult("SELECT * FROM B00_SysParameter WHERE ParaNo='CustName' ");
                string MyQrCodeStrContent = "";
                foreach(var o in paradata)
                {
                    MyQrCodeStrContent = Convert.ToString(o.ParaValue) + "|" + Request["EquNo"];
                        //new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new { equ_id = Convert.ToString(o.ParaValue) + "|" + Request["EquNo"] });
                }
                    //new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new {equ_no=""});
                BarcodeWriter write = new BarcodeWriter
                {
                    Format = ZXing.BarcodeFormat.QR_CODE,
                    Options = new ZXing.QrCode.QrCodeEncodingOptions
                    {
                        //產生出圖片的高度
                        Height = 180,
                        //產生出圖片的寬度
                        Width = 180,
                        //文字是使用哪種編碼方式
                        CharacterSet = "UTF-8",

                        //錯誤修正容量
                        //L水平	7%的字碼可被修正
                        //M水平	15%的字碼可被修正
                        //Q水平	25%的字碼可被修正
                        //H水平	30%的字碼可被修正
                        ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.H
                    }
                };
                System.Drawing.Bitmap bitmap = write.Write(MyQrCodeStrContent);
                System.IO.MemoryStream m = new System.IO.MemoryStream();
                bitmap.Save(m, System.Drawing.Imaging.ImageFormat.Png);
                byte[] byteImage = m.ToArray();
                this.Base64Image = Convert.ToBase64String(byteImage);
                var equdatas = odo.GetQueryResult("SELECT * FROM B01_EquData WHERE EquNo=@EquNo", new { EquNo = Request["EquNo"] });
                foreach(var o in equdatas)
                {
                    this.EquName = Convert.ToString(o.EquName);
                    this.EquBuilder = Convert.ToString(o.Building);
                }
            }
        }

    }//end page class
}//end namespace