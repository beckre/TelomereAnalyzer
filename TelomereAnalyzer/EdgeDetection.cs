using System;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Drawing;




namespace TelomereAnalyzer

{
    class EdgeDetection
    {
        FormOne _formOne = null;
        Image<Bgr, byte> _ProcessedImage = null;
        Bitmap _btmProcessedImage = null;
       public Nuclei _allNuclei = null;
        public AllTelomeres _allTelomeres = null;

        public EdgeDetection(FormOne formOne)
        {
            this._formOne = formOne;
            _allNuclei = new Nuclei();
            _allTelomeres = new AllTelomeres();
        }

        public void FindingContoursNuclei(Image<Gray, byte> imageForEdgeDetection, Image<Gray, byte> imageNormalizedToDrawOn)
        {
            //rtfResultBox.Text = "";
            Image<Gray, byte> grayImage = imageForEdgeDetection;
            _ProcessedImage = imageNormalizedToDrawOn.Convert<Bgr, byte>();
            _btmProcessedImage = _ProcessedImage.ToBitmap();
            _allNuclei._imageToDrawOn = imageNormalizedToDrawOn.Convert<Bgr, byte>();

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
                PointF[] centerPoints = null;
                PointF centerPoint = new PointF();
                PointF[] contourPoints = null;
                PointF[][] allContours = null;


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
                                contourPoints = Array.ConvertAll(ch.ToArray(), input => new PointF(input.X, input.Y));
                        //}
                        /*
                         * else
                            contourPoints = Array.ConvertAll(contour.ToArray(), input => new Point(input.X, input.Y));
                        */
                        huMoments = momentsOfContour.GetHuMoment();
                            contourFound++;
                            AddContourPoints(ref allContours, contourPoints);
                            AddCenterPoint(ref centerPoints, centerPoint);

                        Nucleus nucleus = new Nucleus("Nucleus " + contourFound, centerPoint, contourPoints);
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
            
             //For Testing Nucleus and Nuclei Classes
            _allNuclei.PrepareDrawingCenterPoints();
            _allNuclei.PrepareDrawingContoursByNucleus(new Bgr(Color.DarkViolet));
            _allNuclei.PrintResultValues();
           

            _formOne._NucleiImageEdgesDetected = _ProcessedImage;
            _formOne._TestingNucleiImageEdgesDetected = _allNuclei._imageToDrawOn;
            //rtfResultBox.Text = contourFound.ToString() + " Objekte gefunden\n\n" + resultValues;
            //   MessageBox.Show(contourFound.ToString() + " Objekte gefunden\n\n" + resultValues);
        }

        public void FindingContoursTelomeres(Image<Gray, byte> imageForTelomereDetection, Image<Gray, byte> imageNormalizedToDrawOn)
        {
            //rtfResultBox.Text = "";
            Image<Gray, byte> grayImage = imageForTelomereDetection;
            _ProcessedImage = imageNormalizedToDrawOn.Convert<Bgr, byte>();
            _allTelomeres._imageToDrawOn = imageNormalizedToDrawOn.Convert<Bgr, byte>();

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
            PointF[] centerPoints = null;
            PointF centerPoint = new Point();
            PointF[] contourPoints = null;
            PointF[][] allContours = null;
            Boolean contourTooSmall = false;

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
                    if (contour.Area >= 8)
                    {
                        contourPoints = Array.ConvertAll(contour.ToArray(), input => new PointF(input.X, input.Y));

                        huMoments = momentsOfContour.GetHuMoment();
                    //Contours smaller than 8 Pixels should not be counted as a Telomere
                    //if (contourPoints.Length >= 8)
                    //{
                        contourFound++;
                        AddContourPoints(ref allContours, contourPoints);
                        AddCenterPoint(ref centerPoints, centerPoint);

                        Telomere telomere = new Telomere("Telomere " + contourFound, centerPoint, contourPoints);
                        _allTelomeres.AddTelomereToAllTelomeresList(telomere);
                        resultValues += "relative Area=" + contour.Area.ToString() + "  Xc= " + centerPoint.X.ToString() + "  Yc= " + centerPoint.Y.ToString() + "Gravity Center= ( " + momentsOfContour.GravityCenter.x.ToString() + " | " + momentsOfContour.GravityCenter.y.ToString() + " )\n";
                        contourTooSmall = true;
                    }
                    //}
                }

            }
            _allTelomeres.SetAttributes(resultValues, contourFound, centerPoints, allContours);


            if (centerPoints != null)
                for (Int32 E = 0; E < centerPoints.Length; E++)
                    DrawPoint(centerPoints[E]);



            if (allContours != null)
                for (Int32 E = 0; E < allContours.Length; E++)
                    DrawContour(allContours[E]);

            //For Testing Nucleus and Nuclei Classes
            /*
            _allNuclei.PrepareDrawingCenterPoints();
            _allNuclei.PrepareDrawingContoursByNucleus();
            _allNuclei.PrintResultValues();
            */
            _allTelomeres.PrepareDrawingCenterPoints();
            _allTelomeres.PrepareDrawingContoursByTelomere();
            _allTelomeres.PrintResultValues();

            _formOne._TelomereImageTelomeresDetected = _ProcessedImage;

            //For Testing Nucleus and Nuclei Classes
            //_formOne._TestingNucleiImageEdgesDetected = _allNuclei._imageToDrawOn;
            _formOne._TestingTelomereImageTelomeresDetected = _allTelomeres._imageToDrawOn;

            //rtfResultBox.Text = contourFound.ToString() + " Objekte gefunden\n\n" + resultValues;
            //   MessageBox.Show(contourFound.ToString() + " Objekte gefunden\n\n" + resultValues);
        }

        protected void AddContourPoints(ref PointF[][] allContours, PointF[] points)
        {
            PointF[][] tmp = null;
            Int32 elements = 0;

            if (allContours != null)
                elements = allContours.Length;
            tmp = new PointF[elements + 1][];

            for (Int32 E = 0; E < elements; E++)
                tmp[E] = allContours[E];

            tmp[elements] = points;
            allContours = tmp;
        }

        protected void AddCenterPoint(ref PointF[] centerPoints, PointF centerPoint)
        {
            PointF[] tmp = null;
            Int32 elements = 0;
            if (centerPoints != null)
                elements = centerPoints.Length;

            tmp = new PointF[elements + 1];
            for (Int32 E = 0; E < elements; E++)
                tmp[E] = centerPoints[E];


            tmp[elements].X = centerPoint.X;
            tmp[elements].Y = centerPoint.Y;
            centerPoints = tmp;
        }

        //Malen ist hier eventuell nicht notwendig. Die Methoden befinden sich auch in der Nuclei bzw. AllTelomeres Klasse
        //Kann hier später weggenommen werden.
        protected void DrawPoint(PointF cP)
        {
            Int32 sizeCross = 5;
            PointF[] halfLRCross = new PointF[2];
            PointF[] halfTBCross = new PointF[2];

            halfLRCross[0] = new PointF(cP.X - sizeCross, cP.Y);
            halfLRCross[1] = new PointF(cP.X + sizeCross, cP.Y);

            halfTBCross[0] = new PointF(cP.X, cP.Y - sizeCross);
            halfTBCross[1] = new PointF(cP.X, cP.Y + sizeCross);

            Bgr colorBlue = new Bgr(Color.Blue);
            /* Hier Center Point malen
            _ProcessedImage.DrawPolyline(halfLRCross, false, colorBlue, 1);
            _ProcessedImage.DrawPolyline(halfTBCross, false, colorBlue, 1);
            */
        }
        protected void DrawContour(PointF[] contour)
        {
            /*
            Bgr colorYellow = new Bgr(Color.Red);

            _ProcessedImage.DrawPolyline(contour, true, colorYellow, 1);
            */

            Bgr color = new Bgr(Color.DarkViolet);
            _btmProcessedImage = _ProcessedImage.ToBitmap();

            //_imageToDrawOn.DrawPolyline(contour, true, color, 1);
            Graphics graphics = Graphics.FromImage(_btmProcessedImage);
            graphics.DrawPolygon(Pens.DarkViolet, contour);
            _ProcessedImage = new Image<Bgr, byte>(_btmProcessedImage);
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
