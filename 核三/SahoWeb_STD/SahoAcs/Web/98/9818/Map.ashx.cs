using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.IO;
using DapperDataObjectLib;


namespace SahoAcs.Web._98._9818
{
    /// <summary>
    /// Map 的摘要描述
    /// </summary>
    public class Map : IHttpHandler
    {
        public OrmDataObject odo;

        string connection_string = @"Addr={0};Database={1};uid={2};pwd={3}";

        public void ProcessRequest(HttpContext context)
        {
            Bitmap bitmap = (Bitmap)Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"Img\Maps\404-not-found.png");
            this.odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
            int rotate = 0;
            if (context.Request["Pic"] != null)
            {
                var map_result = this.odo.GetQueryResult("SELECT * FROM B03_MapBackground WHERE PicID=@PicID", new { PicID = context.Request["Pic"] });
                if (map_result.Count() > 0)
                {
                    if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + map_result.First().PicName))
                    {
                        bitmap = (Bitmap)Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + map_result.First().PicName);
                    }                    
                    rotate = Convert.ToInt32(map_result.FirstOrDefault().PicAngle);
                }
            }

            bitmap = ResizeBitmap(bitmap, 1024, 768);
            //bitmap.Save(string.Format(@"Maps\{0:yyyyMMddHHmmss}.png",DateTime.Now));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                using (Font arialFont = new Font("Arial", 14))
                {
                    Bitmap newIcon = (Bitmap)Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"Img\Maps\Go.png");
                    Bitmap north_pic = (Bitmap)Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"Img\Maps\ICON_North.png");

                    int top = 500;
                    int left = 500;
                    if (context.Request["top"] != null && context.Request["left"] != null)
                    {
                        top = int.Parse(context.Request["top"]);
                        left = int.Parse(context.Request["left"]);
                        Rectangle rect = new Rectangle(left, top, 15, 15);
                        graphics.DrawImage(newIcon, rect);
                    }

                    #region 旋轉圖形             
                    if (context.Request["MapRotate"] != null)
                    {
                        this.odo.Execute("UPDATE B03_MapBackground SET PicAngle=@Rotate WHERE PicID=@PicID",
                            new { PicID = context.Request["Pic"], Rotate = context.Request["MapRotate"] });
                        rotate = int.Parse(context.Request["MapRotate"]);
                    }
                    graphics.DrawImage(RotateImage(north_pic, rotate), new Rectangle(950, 0, 50, 50));
                    #endregion

                    if (context.Request["EquID"] != null)
                    {
                        //var equ_result = this.odo.GetQueryResult<SahoAcs.DBModel.EquAdj>("SELECT * FROM B01_EquData WHERE EquID=@EquID", new { EquID = context.Request["EquID"] });
                        this.odo.Execute("DELETE B03_MapPoint WHERE EquID=@EquID AND PicID=@PicID",
                            new { EquID = context.Request["EquID"], PicID = context.Request["Pic"] });
                        this.odo.Execute("INSERT INTO B03_MapPoint (PicID,EquID,PointX,PointY) VALUES (@PicID,@EquID,@PointX,@PointY)", new
                        {
                            PicID = context.Request["Pic"],
                            EquID = context.Request["EquID"],
                            PointX = left,
                            PointY = top
                        });
                    }
                    var equ_result = this.odo.GetQueryResult(@"SELECT B.*,A.EquNo FROM 
	                                                                                                     B01_EquData A
	                                                                                                     INNER JOIN B03_MapPoint B ON A.EquID=B.EquID AND PicID=@PicID ", new { PicID = context.Request["Pic"] });
                    try
                    {
                        foreach (var equ in equ_result)
                        {
                            int pointx = Convert.ToInt32(equ.PointX);
                            int pointy = Convert.ToInt32(equ.PointY);
                            string equno = Convert.ToString(equ.EquNo);
                            graphics.DrawString(equno, arialFont, Brushes.OrangeRed, pointx, pointy);
                        }
                    }
                    catch (Exception ex)
                    {
                        string error = ex.Message;
                    }


                    newIcon.Dispose();
                }
            }
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);//save the image file
            bitmap.Dispose();
            context.Response.ClearContent();
            context.Response.ContentType = "image/Jpeg";
            context.Response.BinaryWrite(ms.ToArray());
        }


        private static Bitmap ResizeBitmap(Bitmap sourceBMP, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(sourceBMP, 0, 0, width, height);
            return result;
        }


        public static Bitmap RotateImage(Bitmap b, float angle)
        {
            //create a new empty bitmap to hold rotated image
            Bitmap returnBitmap = new Bitmap(b.Width, b.Height);
            //make a graphics object from the empty bitmap
            using (Graphics g = Graphics.FromImage(returnBitmap))
            {
                //move rotation point to center of image
                g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
                //rotate
                g.RotateTransform(angle);
                //move image back
                g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
                //draw passed in image onto graphics object
                g.DrawImage(b, new Point(0, 0));
            }
            return returnBitmap;
        }



        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}