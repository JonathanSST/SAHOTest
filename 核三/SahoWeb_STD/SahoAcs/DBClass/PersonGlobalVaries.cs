using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using DapperDataObjectLib;
using SahoAcs.DBClass;



namespace SahoAcs.DBClass
{
    public static class PersonGlobalVaries
    {
        public static void SetPsnDelete(this OrmDataObject odo, string CardNo)
        {
            odo.Execute(@"DELETE B02_FPData WHERE CardNo=@CardNo",
                                   new { CardNo = CardNo });
            odo.Execute(@"DELETE B02_FaceData WHERE CardNo=@CardNo",
                new { CardNo = CardNo });
            odo.Execute(@"DELETE B02_FaceData2  WHERE CardNo=@CardNo",
               new { CardNo = CardNo });
            odo.Execute(@"DELETE B02_FVPData  WHERE CardNo=@CardNo",
                new { CardNo = CardNo });
            odo.Execute(@"DELETE B02_FaceImageData WHERE CardNo=@CardNo",
                new { CardNo = CardNo });
            odo.Execute(@"DELETE B02_FaceImageData2 WHERE CardNo=@CardNo",
                new { CardNo = CardNo });
            //20210412 update
            odo.Execute(@"DELETE B02_FaceData3 WHERE CardNo=@CardNo",
                new { CardNo = CardNo });
            odo.Execute(@"DELETE B02_FPData_F2 WHERE CardNo=@CardNo",
                new { CardNo = CardNo });
            odo.Execute(@"DELETE B02_FaceImageData3 WHERE CardNo=@CardNo",
                new { CardNo = CardNo });

            //20230711 BY TINA
            odo.Execute(@"DELETE B02_FaceDataBS3 WHERE CardNo=@CardNo",
               new { CardNo = CardNo });
            odo.Execute(@"DELETE B02_FaceImageDataBS3 WHERE CardNo=@CardNo",
                new { CardNo = CardNo });


        }

        public static string GetPic(string s_pic)
        {
            string[] tmpPic = s_pic.Split(new char[] { '.' });
            if (tmpPic.Length == 1)
            {
                if (System.IO.File.Exists(s_pic + ".jpg"))
                    s_pic = s_pic + ".jpg";
                else if (System.IO.File.Exists(s_pic + ".JPG"))
                    s_pic = s_pic + ".JPG";
                else if (System.IO.File.Exists(s_pic + ".jpeg"))
                    s_pic = s_pic + ".jpeg";
                else if (System.IO.File.Exists(s_pic + ".JPEG"))
                    s_pic = s_pic + ".JPEG";
                else if (System.IO.File.Exists(s_pic + ".bmp"))
                    s_pic = s_pic + ".bmp";
                else if (System.IO.File.Exists(s_pic + ".BMP"))
                    s_pic = s_pic + ".BMP";
                else if (System.IO.File.Exists(s_pic + ".png"))
                    s_pic = s_pic + ".png";
                else if (System.IO.File.Exists(s_pic + ".PNG"))
                    s_pic = s_pic + ".PNG";
                else if (System.IO.File.Exists(s_pic + ".gif"))
                    s_pic = s_pic + ".gif";
                else if (System.IO.File.Exists(s_pic + ".GIF"))
                    s_pic = s_pic + ".GIF";
                else if (System.IO.File.Exists(s_pic + ".svg"))
                    s_pic = s_pic + ".svg";
                else if (System.IO.File.Exists(s_pic + ".SVG"))
                    s_pic = s_pic + ".SVG";
                else
                    s_pic = "";
            }
            else
            {
                if (!System.IO.File.Exists(s_pic))
                {
                    s_pic = "";
                }
            }

            return s_pic;
        }

        public static string PicStr(String mode, String ImgPath)
        {
            Byte[] bytes = new Byte[1024];
            String src = "";

            if (mode == "File")
            {
                if (!string.IsNullOrEmpty(ImgPath.Trim()))
                {
                    Uri uri = new Uri(ImgPath);
                    WebRequest webRequest = WebRequest.Create(uri);
                    Stream stream = webRequest.GetResponse().GetResponseStream();
                    BinaryReader br = new BinaryReader(stream);
                    bytes = br.ReadBytes((Int32)stream.Length);
                    string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                    src = "data:image/png;base64," + base64String;

                    br.Close();
                    stream.Close();
                }
            }

            return src;
        }

    }//end gobal class
}//end namespace