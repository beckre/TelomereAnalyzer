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
        /*----------------------------------------------------------------------------------------*\
        |* This Class is for detecting the Edges/Contours of the Nuclei that were detected with   *|
        |* the Detection Class.                                                                   *|
        \*----------------------------------------------------------------------------------------*/
        public EdgeDetection(FormOne formOne)
        {
            this._formOne = formOne;
            _allNuclei = new Nuclei();
            _allTelomeres = new AllTelomeres();
        }
        /*----------------------------------------------------------------------------------------*\
        |* This Method finds the Contours of the Nuclei that are found through the Detection      *|
        |* Class. The Contour-Coordinates that are found are used to create new Nucleus Objects.  *|
        |* They are added to a List which contains all the Nucleus Objects. This makes it easy to *|
        |* access all Nuclei and iterate over them at any Point.                                  *|
        |* Draws Borders around the Nuclei.                                                       *|
        \*----------------------------------------------------------------------------------------*/
        public void FindingContoursNuclei(Image<Gray, byte> imageForEdgeDetection, Image<Gray, byte> imageNormalizedToDrawOn)
        {
            Image<Gray, byte> grayImage = imageForEdgeDetection;
            _ProcessedImage = imageNormalizedToDrawOn.Convert<Bgr, byte>();
            _btmProcessedImage = _ProcessedImage.ToBitmap();
            _allNuclei._imageToDrawOn = imageNormalizedToDrawOn.Convert<Bgr, byte>();

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
                    //1500 was chosen through testing
                    if (contour.Area > 1500)
                    {
                        var ch = contour.GetConvexHull(ORIENTATION.CV_CLOCKWISE, storage);
                        contourPoints = Array.ConvertAll(ch.ToArray(), input => new PointF(input.X, input.Y));
                        huMoments = momentsOfContour.GetHuMoment();
                        contourFound++;
                        AddContourPoints(ref allContours, contourPoints);
                        AddCenterPoint(ref centerPoints, centerPoint);
                        Nucleus nucleus = new Nucleus("Nucleus " + contourFound, centerPoint, contourPoints);
                        _allNuclei.AddNucleusToNucleiList(nucleus);

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
            /*For Testing Nucleus and Nuclei Classes
            _allNuclei.PrepareDrawingCenterPoints();
            _allNuclei.PrepareDrawingContoursByNucleus(new Bgr(Color.DarkViolet));
            _allNuclei.PrintResultValues();
            */
            _formOne._NucleiImageEdgesDetected = _ProcessedImage;
            _formOne._TestingNucleiImageEdgesDetected = _allNuclei._imageToDrawOn;
        }
        /*----------------------------------------------------------------------------------------*\
        |* This Method finds the Contours of the Telomeres. Similar Function to                   *|
        |* FindingContoursNuclei(). The Contour-Coordinates that are found are used to create     *|
        |* new Telomere Objects. They are added to a List which contains all the Telomere Objects.*|
        |* This makes it easy to access all Telomeres and iterate over them at any Point.         *|
        |* Draws Borders around the Telomeres.                                                    *|
        \*----------------------------------------------------------------------------------------*/
        public void FindingContoursTelomeres(Image<Gray, byte> imageForTelomereDetection, Image<Gray, byte> imageNormalizedToDrawOn)
        {
            Image<Gray, byte> grayImage = imageForTelomereDetection;
            _ProcessedImage = imageNormalizedToDrawOn.Convert<Bgr, byte>();
            _allTelomeres._imageToDrawOn = imageNormalizedToDrawOn.Convert<Bgr, byte>();

            MemStorage storage = new MemStorage();
            Bgr color = new Bgr(Color.Red);

            Int32 contourFound = 0;
            String resultValues = null;
            PointF[] centerPoints = null;
            PointF centerPoint = new Point();
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
                    //Contours that have a very small Area should not be counted as a Telomere
                    if (contour.Area >= 8)
                    {
                        contourPoints = Array.ConvertAll(contour.ToArray(), input => new PointF(input.X, input.Y));
                        huMoments = momentsOfContour.GetHuMoment();
                        contourFound++;
                        AddContourPoints(ref allContours, contourPoints);
                        AddCenterPoint(ref centerPoints, centerPoint);
                        Telomere telomere = new Telomere("Telomere " + contourFound, centerPoint, contourPoints);
                        _allTelomeres.AddTelomereToAllTelomeresList(telomere);
                        resultValues += "relative Area=" + contour.Area.ToString() + "  Xc= " + centerPoint.X.ToString() + "  Yc= " + centerPoint.Y.ToString() + "Gravity Center= ( " + momentsOfContour.GravityCenter.x.ToString() + " | " + momentsOfContour.GravityCenter.y.ToString() + " )\n";
                    }
                }
            }
            _allTelomeres.SetAttributes(resultValues, contourFound, centerPoints, allContours);
            if (centerPoints != null)
                for (Int32 E = 0; E < centerPoints.Length; E++)
                    DrawPoint(centerPoints[E]);

            if (allContours != null)
                for (Int32 E = 0; E < allContours.Length; E++)
                    DrawContour(allContours[E]);

            _allTelomeres.PrepareDrawingCenterPoints();
            _allTelomeres.PrepareDrawingContoursByTelomere();
            _allTelomeres.PrintResultValues();

            _formOne._TelomereImageTelomeresDetected = _ProcessedImage;
            _formOne._TestingTelomereImageTelomeresDetected = _allTelomeres._imageToDrawOn;
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
        #region Drawing Coordinates-----------------------------------------------------------------------------------------
        /*----------------------------------------------------------------------------------------*\
        |* These Methods are also contained in the Nuclei/AllTelomeres-Classes.                   *|
        |* Here they can be used for Testing the detection.                                       *|
        \*----------------------------------------------------------------------------------------*/
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
            /* Drawing Center Point
            _ProcessedImage.DrawPolyline(halfLRCross, false, colorBlue, 1);
            _ProcessedImage.DrawPolyline(halfTBCross, false, colorBlue, 1);
            */
        }
        protected void DrawContour(PointF[] contour)
        {
            Bgr color = new Bgr(Color.DarkViolet);
            _btmProcessedImage = _ProcessedImage.ToBitmap();
            Graphics graphics = Graphics.FromImage(_btmProcessedImage);
            graphics.DrawPolygon(Pens.DarkViolet, contour);
            _ProcessedImage = new Image<Bgr, byte>(_btmProcessedImage);
        }
        #endregion
    }
}
