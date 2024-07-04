using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.IO;
using DapperDataObjectLib;
using System.Web.SessionState;


namespace SahoAcs.Web._06._0640
{
    /// <summary>
    /// Map 的摘要描述
    /// </summary>
    public class Map : IHttpHandler
    {
        public OrmDataObject odo;
        public string message = string.Empty;
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

                    string Equ_Sql = @"SELECT 
B.*,
A.EquNo,
C.AliveState,
ISNULL(C.VarStateTime,'') AS VarStateTime 
From B01_EquData A 
Left JOIN B03_MapPoint B ON A.EquID=B.EquID
Left JOIN V_EquAliveState C ON A.EquNo = C.EquNo
Where B.PicID=@PicID ";
                    var equ_result = this.odo.GetQueryResult(Equ_Sql, new { PicID = context.Request["Pic"] });

                    try
                    {
                        foreach (var equ in equ_result)
                        {
                            int pointx = Convert.ToInt32(equ.PointX);
                            int pointy = Convert.ToInt32(equ.PointY);
                            string equno = Convert.ToString(equ.EquNo);
                            if (equ.AliveState == 1)
                            {
                                graphics.DrawString("■ " + equno, arialFont, Brushes.Blue, pointx, pointy);
                            }
                            else
                            {
                                Font Font = new Font("Arial", 12, FontStyle.Bold);
                                var VarStateTime = equ.VarStateTime;
                                var strEquNo = equ.EquNo;
                                message = strEquNo + " 斷連線時間 : " + VarStateTime;
                                graphics.DrawString("■ " + message, Font, Brushes.Red, pointx, pointy);
                            }
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

        public bool IsReusable
        {
            get
            {
                return false;
            }
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
    }
}