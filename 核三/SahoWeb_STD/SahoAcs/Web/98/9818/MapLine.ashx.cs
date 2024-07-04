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
    /// MapLine 的摘要描述
    /// </summary>
    public class MapLine : IHttpHandler
    {
        public OrmDataObject odo;

        public void ProcessRequest(HttpContext context)
        {
            Bitmap bitmap = (Bitmap)Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"Img\Maps\404-not-found.png");
            Bitmap north_pic = (Bitmap)Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"Img\Maps\ICON_North.png");

            int rotate = 0;
            this.odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

            if (context.Request["Pic"] != null)
            {
                var map_result = this.odo.GetQueryResult("SELECT * FROM B03_MapBackground WHERE PicID=@PicID", new { PicID = context.Request["Pic"] });
                if (map_result.Count() > 0)
                {
                    //改相對路徑
                    if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + map_result.First().PicName))
                    {
                        bitmap = (Bitmap)Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + map_result.First().PicName);
                    }                    
                    rotate = Convert.ToInt32(map_result.FirstOrDefault().PicAngle);
                }
            }

            bitmap = ResizeBitmap(bitmap, 1024, 768);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                using (Font arialFont = new Font("Arial", 14))
                {
                    Bitmap newIcon = (Bitmap)Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"Img\Maps\Go.png");
                    int top = 500;
                    int left = 500;
                    if (context.Request["top"] != null && context.Request["left"] != null)
                    {
                        top = int.Parse(context.Request["top"]);
                        left = int.Parse(context.Request["left"]);
                    }

                    #region 旋轉圖形                 
                    graphics.DrawImage(RotateImage(north_pic, rotate), new Rectangle(950, 0, 50, 50));
                    #endregion

                    Rectangle rect = new Rectangle(left, top, 15, 15);
                    if (context.Request["twds"] != null)
                    {
                        string[] tws_arr = context.Request["twds"].Split('-');
                        Point oldP = new Point();
                        Point nowP = new Point();
                        Pen greenPen = new Pen(Color.FromArgb(255, 0, 255, 0), 5);
                        greenPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                        foreach (var tws in tws_arr)
                        {
                            int x = int.Parse(tws.Split(',')[0]);
                            int y = int.Parse(tws.Split(',')[1]);
                            rect = new Rectangle(x, y, 15, 15);
                            nowP = new Point(rect.Left + 7, rect.Top + 7);
                            //graphics.DrawImage(newIcon, rect);
                            if (oldP.X > 0 && oldP.Y > 0)
                            {
                                graphics.DrawLine(greenPen, oldP, nowP);
                            }
                            oldP = nowP;
                        }
                    }
                    else
                    {

                    }

                    var equ_result = this.odo.GetQueryResult(@"SELECT B.*,A.EquNo FROM 
	                                                                                                     B01_EquData A
	                                                                                                     INNER JOIN B03_MapPoint B ON A.EquID=B.EquID WHERE A.EquID IN (@EquNoS,@EquNoE) AND PicID=@PicID"
                                                                                                        , new { PicID = context.Request["Pic"], EquNoS = context.Request["EquNoS"], EquNoE = context.Request["EquNoE"] });
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




    }//end class
}//end namespace