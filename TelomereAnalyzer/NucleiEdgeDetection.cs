using System;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Drawing;




namespace TelomereAnalyzer

{
    class NucleiEdgeDetection
    {
        FormOne _formOne = null;
        Image<Bgr, byte> _ProcessedImage = null;
        Nuclei _allNuclei = null;

        public NucleiEdgeDetection(FormOne formOne)
        {
            this._formOne = formOne;
            _allNuclei = new Nuclei();
        }

        public void FindingContours(Image<Gray, byte> imageForEdgeDetection, Image<Gray, byte> imageNormalizedToDrawOn)
        {
            //rtfResultBox.Text = "";
            Image<Gray, byte> grayImage = imageForEdgeDetection;
            _ProcessedImage = imageNormalizedToDrawOn.Convert<Bgr, byte>();

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
                    //if (contour.Area > 50) //hier könnte man testen, was passiert wenn die Zahl höher (niedriger) ist
                    //if(contour.Area > 200)
                    if (contour.Area > 1500)
                    {
                            //if (chbConvexHull.Checked == true)
                            //{
                                var ch = contour.GetConvexHull(ORIENTATION.CV_CLOCKWISE, storage);
                                //    var ch1=contour.ApproxPoly(0.001);
                                contourPoints = Array.ConvertAll(ch.ToArray(), input => new Point(input.X, input.Y));
                        //Jede Kontur könnte hier als eigene einzelne Einheit in ein neues Klassenobjekt (neue Klasse Nucleus) gesteckt werden --> Alle Konturinformationen hätte man somit an einem Ort
                        //}
                        /*
                         * else
                            contourPoints = Array.ConvertAll(contour.ToArray(), input => new Point(input.X, input.Y));
                        */
                        huMoments = momentsOfContour.GetHuMoment();
                            contourFound++;
                            AddContourPoints(ref allContours, contourPoints);
                            AddCenterPoint(ref centerPoints, centerPoint);

                        Nucleus nucleus = new Nucleus(centerPoint, contourPoints);
                        _allNuclei.AddNucleusToNucleiList(nucleus);


                        //Jede Kontur könnte hier als eigene einzelne Einheit in ein neues Klassenobjekt (neue Klasse Nucleus) gesteckt werden --> Alle Konturinformationen hätte man somit an einem Ort
                        resultValues += "relative Area=" + contour.Area.ToString() + "  Xc= " + centerPoint.X.ToString() + "  Yc= " + centerPoint.Y.ToString() + "Gravity Center= ( " + momentsOfContour.GravityCenter.x.ToString() + " | " + momentsOfContour.GravityCenter.y.ToString() + " )\n";
                        }
                    }
                
                }
            _allNuclei.SetAttributes(resultValues, contourFound, centerPoints, allContours);
            
           
            if (centerPoints != null)
                for (Int32 E = 0; E < centerPoints.Length; E++)
                    DrawPoint(centerPoints[E]);



            if (allContours != null)
                for (Int32 E = 0; E < allContours.Length; E++)
                    DrawContour(allContours[E]);


            _formOne._NucleiImageEdgesDetected = _ProcessedImage;
            //rtfResultBox.Text = contourFound.ToString() + " Objekte gefunden\n\n" + resultValues;
            //   MessageBox.Show(contourFound.ToString() + " Objekte gefunden\n\n" + resultValues);
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

        protected void DrawPoint(Point cP)
        {
            Int32 sizeCross = 5;
            Point[] halfLRCross = new Point[2];
            Point[] halfTBCross = new Point[2];

            halfLRCross[0] = new Point(cP.X - sizeCross, cP.Y);
            halfLRCross[1] = new Point(cP.X + sizeCross, cP.Y);

            halfTBCross[0] = new Point(cP.X, cP.Y - sizeCross);
            halfTBCross[1] = new Point(cP.X, cP.Y + sizeCross);

            Bgr colorBlue = new Bgr(Color.Blue);

            _ProcessedImage.DrawPolyline(halfLRCross, false, colorBlue, 1);
            _ProcessedImage.DrawPolyline(halfTBCross, false, colorBlue, 1);
        }
        protected void DrawContour(Point[] contour)
        {
            Bgr colorYellow = new Bgr(Color.Red);

            _ProcessedImage.DrawPolyline(contour, true, colorYellow, 1);
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
