using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.IO;
using DapperDataObjectLib;
using SahoAcs.DBModel;


namespace SahoAcs.Web._0633
{
    /// <summary>
    /// MapStory 的摘要描述
    /// </summary>
    public class MapAlarm : IHttpHandler
    {

        public OrmDataObject odo;

        List<CardLogMap> logs = new List<CardLogMap>();

        public void ProcessRequest(HttpContext context)
        {
            Bitmap bitmap = (Bitmap)Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"Img\Maps\404-not-found.png");
            this.odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
            int rotate = 0;

            #region
            this.logs = this.odo.GetQueryResult<CardLogMap>(@"SELECT 
	                                                A.*,B.EquName,B.EquID,C.PicID,PointX,PointY 
                                                FROM 
	                                                B01_AlarmLog A
	                                                INNER JOIN B01_EquData B ON A.EquNo=B.EquNo
	                                                INNER JOIN B03_MapPoint C ON B.EquID=C.EquID WHERE CardNo=@CardNo AND RecordID = @ID 
                                                    ORDER BY RecordID ",
                                                new {CardNo= context.Request["CardNo"], ID=context.Request["RecordID"]}).ToList();
            var Maps = this.odo.GetQueryResult<MapBackground>("SELECT * FROM B03_MapBackground WHERE PicID IN @PicList",new {PicList=logs.Select(i=>i.PicID)});
            var lines = this.odo.GetQueryResult<B03MapRoute>("SELECT * FROM B03_MapRoute WHERE PicID IN @PicList", new { PicList = logs.Select(i => i.PicID) });
            #endregion

            if (Maps.Count()>0)
            {
                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + Maps.First().PicName))
                {
                    bitmap = (Bitmap)Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + Maps.First().PicName);
                }                
                rotate = Convert.ToInt32(Maps.FirstOrDefault().PicAngle);
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
                   
                    #region 方位角設定
                    graphics.DrawImage(RotateImage(north_pic, rotate), new Rectangle(950, 0, 50, 50));
                    #endregion
                   
                    if (Maps.Count() > 0)
                    {
                        var AllEquOnMaps = this.odo.GetQueryResult(@"select A.*,B.EquNo from B03_MapPoint A 
                                                                                                                    INNER JOIN B01_EquData B ON A.EquID=B.EquID WHERE PicID=@PicID ", new {PicID=Maps.First().PicID});
                        var equ_result = this.logs.Where(i => i.PicID == Maps.First().PicID).ToList();
                        graphics.FillRectangle(Brushes.White, 10, 720, 200, 20);
                        graphics.DrawString(string.Format("{0:yyyyMMdd  HH:mm}",this.logs.First().CardTime), arialFont, Brushes.OrangeRed, 10, 720);
                        if (equ_result.Count() > 0)
                        {
                            graphics.FillRectangle(Brushes.White, 300, 720, 200, 20);
                            graphics.DrawString(equ_result.First().EquName, arialFont, Brushes.Blue, 300, 720);
                        }
                        try
                        {
                            foreach(var equ in AllEquOnMaps)
                            {
                                int pointx = Convert.ToInt32(equ.PointX);
                                int pointy = Convert.ToInt32(equ.PointY);
                                string equno = Convert.ToString(equ.EquNo);
                                graphics.DrawString(equno, arialFont, Brushes.OrangeRed, pointx, pointy);
                            }
                            foreach (var equ in equ_result)
                            {
                                int pointx = Convert.ToInt32(equ.PointX);
                                int pointy = Convert.ToInt32(equ.PointY);
                                string equno = Convert.ToString(equ.EquNo);
                                string equname = Convert.ToString(equ.EquName);
                                graphics.DrawString(equno, arialFont, Brushes.Blue, pointx, pointy);
                            }
                        }
                        catch (Exception ex)
                        {
                            string error = ex.Message;
                        }
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

        private List<CardLogMap> GetReSort(IEnumerable<CardLogMap> paramMaps)
        {
            var result = new List<CardLogMap>();
            foreach (var o in paramMaps)
            {
                if (result.Count() > 0 && result.Last().EquNo == o.EquNo)
                {
                    result.RemoveAt(result.Count() - 1);
                }
                result.Add(o);
            }
            return result;
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