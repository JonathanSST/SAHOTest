using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using System.IO;
using SahoAcs.DBModel;
using System.Drawing;



namespace SahoAcs.Web._98._9818
{
    public partial class MapEdit : System.Web.UI.Page
    {

        OrmDataObject odo = new OrmDataObject("MsSql",Pub.GetDapperConnString());

        public MapBackground mapobj = new MapBackground();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Add")
            {
                this.SetAdd();
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Update")
            {
                this.SetUpdate();
            }   
            if(Request["PageEvent"]!=null && Request["PageEvent"] == "Edit")
            {
                var result = this.odo.GetQueryResult<MapBackground>("SELECT * FROM B03_MapBackground WHERE PicID=@PicID", new {PicID=Request["PicID"] });
                foreach(var o in result)
                {
                    this.mapobj = o;
                }
            }     
        }

        private void SetUpdate()
        {
            string resize_name = "";
            if (Request.Files.Count > 0)
            {
                string[] name1 = Request.Files[0].FileName.Split('.');
                if (name1.Length > 1)
                {
                    string map_path = "Img/Maps/" + name1[0] + "_Res_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + name1[1];
                    //resize_name = Server.MapPath("~"+map_path);
                    resize_name = map_path.Replace("/", @"\");
                    GenerateThumbnailImage(Request.Files[0].InputStream, Server.MapPath("~/" + map_path), 1024, 768, "Saho");
                }
            }
            string cmd = "UPDATE B03_MapBackground SET PicDesc=@PicDesc ";
            if (resize_name != "")
            {
                cmd += ",PicName=@PicName";
            }
            cmd += " WHERE PicID=@PicID ";
            this.odo.Execute(cmd, new { PicName = resize_name, PicDesc = Request["PicDesc"], PicID = Request["PicID"] });
        }

        private void SetAdd()
        {
            for (int cnt = 0; cnt < Request.Files.Count; cnt++)
            {
                string[] name = Request.Files[cnt].FileName.Split('.');
                if (name.Length < 2)
                    continue;

                string uploadname = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + name[1];
                string real_uploadname = Server.MapPath("~/Img/Maps/" + name[0] + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + name[1]);
                string map_path = "Img/Maps/" + name[cnt] + "_Res_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + name[1];                
                string resize_name = map_path.Replace("/", @"\");
                GenerateThumbnailImage(Request.Files[0].InputStream, Server.MapPath("~/" + map_path), 1024, 768, "Saho");                
                this.odo.Execute("INSERT INTO B03_MapBackground (PicName,PicDesc,PicAngle) VALUES (@PicName,@PicDesc,0)", new { PicName = resize_name, PicDesc = Request["PicDesc"] });
            }
        }


        public void GenerateThumbnailImage(Stream File, string Path_s, int width, int height, string Texts)
        {
            MemoryStream ms = new MemoryStream();
            File.CopyTo(ms);
            System.Drawing.Image originalImage = System.Drawing.Image.FromStream(ms);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            if (ow > oh)
            {
                toheight = originalImage.Height * width / originalImage.Width;
            }
            else
            {
                towidth = originalImage.Width * height / originalImage.Height;
            }

            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            g.Clear(System.Drawing.Color.Transparent);

            g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight), new System.Drawing.Rectangle(x, y, ow, oh), System.Drawing.GraphicsUnit.Pixel);

            if (!string.IsNullOrEmpty(Texts))
            {
                //自動調整文字最適大小 
                Graphics canvas = Graphics.FromImage(bitmap);
                int[] sizes = new int[] { 16, 14, 12, 10, 8, 6, 4 };
                System.Drawing.Font crFont = null;
                SizeF crSize = new SizeF();

                for (int i = 0; i <= 6; i++)
                {
                    crFont = new System.Drawing.Font("arial", sizes[i], FontStyle.Bold);
                    crSize = canvas.MeasureString(Texts, crFont);

                    if (Convert.ToUInt16(crSize.Width) < Convert.ToUInt16(bitmap.Width))
                    {
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }

                // 指定文字位置 
                int yPixlesFromBottom = Convert.ToInt32((bitmap.Height * 0.05));
                float yPosFromBottom = ((bitmap.Height - yPixlesFromBottom) - (crSize.Height / 2));
                float xCenterOfImg = (bitmap.Width / 2);

                // 指定文字格式(置中對齊) 
                StringFormat StrFormat = new StringFormat();
                StrFormat.Alignment = StringAlignment.Center;

                // 指定文字筆刷(製造陰影效果) 
                SolidBrush semiTransBrush = new SolidBrush(System.Drawing.Color.FromArgb(153, 255, 255, 255));
                SolidBrush semiTransBrush2 = new SolidBrush(System.Drawing.Color.FromArgb(153, 0, 0, 0));

                canvas.DrawString(Texts, crFont, semiTransBrush2, new PointF(xCenterOfImg + 1, yPosFromBottom + 1), StrFormat);
                canvas.DrawString(Texts, crFont, semiTransBrush, new PointF(xCenterOfImg, yPosFromBottom), StrFormat);
            }

            try
            {
                bitmap.Save(Path_s, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }


    }//end class
}//end namespace