﻿using System;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Drawing;




namespace TelomereAnalyzer

{
    class NucleiEdgeDetection
    {
        FormOne _formOne = null;



        public NucleiEdgeDetection(FormOne formOne)
        {
            this._formOne = formOne;
        }

        public void FindingContours()
        {
            //rtfResultBox.Text = "";
            Image<Gray, UInt16> grayImage = _formOne._NucleiImageNormalized;

            /*
            if (ProcessedImage == null)
            {
                grayImage = MainImage.Convert<Gray, Byte>();
                ProcessedImage = MainImage.Copy();
            }
            else
                grayImage = ProcessedImage.Convert<Gray, Byte>();
            */

            MemStorage storage = new MemStorage();
                Bgr color = new Bgr(Color.Red);

                Int32 contourFound = 0;
                String resultValues = null;
                Point[] centerPoints = null;
                Point centerPoint = new Point();
                Point[] contourPoints = null;
                Point[][] allContours = null;


                MCvMoments momentsOfContour;
                MCvHuMoments huMoments;

                for (var contour = grayImage.FindContours(CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_NONE, RETR_TYPE.CV_RETR_CCOMP, storage); contour != null; contour = contour.HNext)
                {
                    centerPoint.X = contour.BoundingRectangle.X + contour.BoundingRectangle.Width / 2;
                    centerPoint.Y = contour.BoundingRectangle.Y + contour.BoundingRectangle.Height / 2;
                    momentsOfContour = contour.GetMoments();
                    Double value = Convert.ToDouble(momentsOfContour.GravityCenter.x);
                    if (Double.IsNaN(value) == false)
                    {
                        if (contour.Area > 50)
                        {
                            //if (chbConvexHull.Checked == true)
                            //{
                                var ch = contour.GetConvexHull(ORIENTATION.CV_CLOCKWISE, storage);
                                //    var ch1=contour.ApproxPoly(0.001);
                                contourPoints = Array.ConvertAll(ch.ToArray(), input => new Point(input.X, input.Y));
                            //}
                            /*
                             * else
                                contourPoints = Array.ConvertAll(contour.ToArray(), input => new Point(input.X, input.Y));
                            */
                            huMoments = momentsOfContour.GetHuMoment();
                            contourFound++;
                            AddContourPoints(ref allContours, contourPoints);
                            AddCenterPoint(ref centerPoints, centerPoint);
                            resultValues += "relative Area=" + contour.Area.ToString() + "  Xc= " + centerPoint.X.ToString() + "  Yc= " + centerPoint.Y.ToString() + "Gravity Center= ( " + momentsOfContour.GravityCenter.x.ToString() + " | " + momentsOfContour.GravityCenter.y.ToString() + " )\n";
                        }
                    }
                }
        }

        protected void AddContourPoints(ref Point[][] allContours, Point[] points)
        {
            Point[][] tmp = null;
            Int32 elements = 0;

            if (allContours != null)
                elements = allContours.Length;
            tmp = new Point[elements + 1][];

            for (Int32 E = 0; E < elements; E++)
                tmp[E] = allContours[E];

            tmp[elements] = points;
            allContours = tmp;
        }

        protected void AddCenterPoint(ref Point[] centerPoints, Point centerPoint)
        {
            Point[] tmp = null;
            Int32 elements = 0;
            if (centerPoints != null)
                elements = centerPoints.Length;

            tmp = new Point[elements + 1];
            for (Int32 E = 0; E < elements; E++)
                tmp[E] = centerPoints[E];


            tmp[elements].X = centerPoint.X;
            tmp[elements].Y = centerPoint.Y;
            centerPoints = tmp;
        }


        /*
         * Code compiliert zwar aber es werden byte-Bilder behandelt --> muss aber eigentlich mit 16Bit Bildern passieren
         */

        /*
        public Image<Gray, byte> FindingContours(Image<Gray, UInt16> NucleiImageNormalized)
        {

            //Canny Edge Detection
            Image<Gray, byte> destImageCanny = new Image<Gray, byte>(NucleiImageNormalized.Width, NucleiImageNormalized.Height);    
            //destImageCanny = NucleiImageNormalized.Convert<Gray, byte>().Canny(50,20);
            destImageCanny = NucleiImageNormalized.Convert<Gray, byte>().Canny(80, 20);
            return destImageCanny;

            /*
            //Sobel Edge Detection
            Image<Gray, float> destImageSobel = new Image<Gray, float>(NucleiImageNormalized.Width, NucleiImageNormalized.Height);
            destImageSobel = NucleiImageNormalized.Convert<Gray, byte>().Sobel(1, 1, 3);
            return destImageSobel.Convert<Gray, byte>();
            */

            /*
            //Laplacian Edge Detection
            Image<Gray, float> destImageSobel = new Image<Gray, float>(NucleiImageNormalized.Width, NucleiImageNormalized.Height);
            destImageSobel = NucleiImageNormalized.Convert<Gray, byte>().Laplace(3);
            return destImageSobel.Convert<Gray, byte>();
            */

        //}
        
    }
}
