using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;

namespace TelomereAnalyzer
{
    public class VesselClass : IDisposable
    {
        StochasticsClass _objStochastics = null;
        public Double _area;
        public Contour<Point> _contour = null;
        public Int32 _iD;               // Identification number for this vessel within the tissue
        Ellipse _ellipseModel;
        public Rectangle _boundingBox;
        public Point _gravCenter;
        public bool _belongsToCluster = false;

        MCvMoments _momentsOfContour;   //  Hu's moments are defined with  _contour.GetMoments().GetHuMoment();
        Double _density = 0.0;
        Double _excentricity = 0.0;
        Double _circularity = 0.0;
        Double _orientation = 0.0;
        Double _isoperimetricQuotient = 0.0;
        Double _normalizedShapeVectorIndicator = 0.0;

        public bool _vesselLikeObject = false;

        #region construction, disposal, destruction
        public VesselClass(ref StochasticsClass objStochastics, Contour<Point> contour)
        {
            _objStochastics = objStochastics;
            _contour = contour;
            _momentsOfContour = _contour.GetMoments();
            InitializeGlobals();
            GeometricalDescription();
            _vesselLikeObject = CorrelatesToVesselModel();
        }
        protected void InitializeGlobals()
        {
            _gravCenter = new Point(-1, -1);
            _iD = -1; // Signal for unknown
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        ~VesselClass()
        {
            Dispose();
        }
        #endregion

        #region Calculate geometrical description
        protected void GeometricalDescription()
        {
            GravityCenter();
            CalculateDensity();
            CalculateCircularity();
            CalculateExcentricity();
            CalculateEllipseModel();
            CalculateBoundingBox();
            CalculateIsoperimetricQuotient();
            CalculateNormalizedShapeVectorIndicator();
            CalculateArea();
        }
        protected void CalculateArea()
        {
            try
            {
                _area = _contour.Area;
            }
            catch
            {
            }
        }
        protected void GravityCenter()
        {
            _gravCenter = new Point((Int32)_momentsOfContour.GravityCenter.x, (Int32)_momentsOfContour.GravityCenter.y);
        }
        protected bool CalculateEllipseModel()
        {
            Point[] pts = _contour.ToArray<Point>();
            PointF[] fpts = new PointF[pts.Length];
            if (pts.Length < 5)
                return false;

            for (int i = 0; i < pts.Length; i++)
                fpts[i] = new PointF(Convert.ToSingle(pts[i].X), Convert.ToSingle(pts[i].Y));

            _ellipseModel = PointCollection.EllipseLeastSquareFitting(fpts);
            return true;
        }
        protected void CalculateCircularity()
        {
            _circularity = _objStochastics.Circularity(_contour.Perimeter, _contour.Area);
        }
        protected void CalculateBoundingBox()
        {
            Int32 x;
            Int32 y;
            Int32 xMin = Int32.MaxValue;
            Int32 yMin = Int32.MaxValue;
            Int32 xMax = Int32.MinValue;
            Int32 yMax = Int32.MinValue;

            for (Int32 c = 0; c < _contour.Count(); c++)
            {
                x = _contour[c].X;
                y = _contour[c].Y;
                xMin = (xMin < _contour[c].X) ? xMin : _contour[c].X;
                yMin = (yMin < _contour[c].Y) ? yMin : _contour[c].Y;
                xMax = (xMax > _contour[c].X) ? xMax : _contour[c].X;
                yMax = (yMax > _contour[c].Y) ? yMax : _contour[c].Y;
            }
            Int32 width = Math.Abs(xMax - xMin);
            Int32 height = Math.Abs(yMax - yMin);
            _boundingBox = new Rectangle(new Point(xMin, yMin), new Size(width, height));
        }
        protected void CalculateDensity()
        {
            _density = _contour.Area / _contour.GetConvexHull(Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE).Area;
        }
        protected void CalculateOrientation()
        {
            Double u20 = _momentsOfContour.GetCentralMoment(2, 0);
            Double u02 = _momentsOfContour.GetCentralMoment(0, 2);
            Double u11 = _momentsOfContour.GetCentralMoment(1, 1);
            _orientation = 0.5 * Math.Atan2(u20 - u02, 2 * u11);
        }
        protected void CalculateExcentricity()
        {
            Double u20 = _momentsOfContour.GetCentralMoment(2, 0);
            Double u02 = _momentsOfContour.GetCentralMoment(0, 2);
            Double u11 = _momentsOfContour.GetCentralMoment(1, 1);

            Double i = Math.Sqrt(((u20 - u02) * (u20 - u02)) + 4 * (u11 * u11));

            Double a1 = u20 + u02 + i;
            Double a2 = u20 + u02 - i;

            _excentricity = a1 / a2;
        }
        protected void CalculateIsoperimetricQuotient()
        {
            _isoperimetricQuotient = _objStochastics.IsoPerimetricQuotient(_contour.Perimeter, _contour.Area);
        }
        protected void CalculateNormalizedShapeVectorIndicator()
        {
            double[] vectorLength = new Double[_contour.LongCount()];
            int i = 0;
            foreach (Point Pt in _contour)
            {
                vectorLength[i] = Math.Sqrt(((Pt.X - _gravCenter.X) * (Pt.X - _gravCenter.X)) + ((Pt.Y - _gravCenter.Y) * (Pt.Y - _gravCenter.Y)));
                i++;
            }
            double mittel = vectorLength.Average();

            for (int v = 0; v < vectorLength.Length; v++)
                vectorLength[v] = vectorLength[v] / mittel;

            _normalizedShapeVectorIndicator = vectorLength.Max() - vectorLength.Min();
        }
        protected bool CorrelatesToVesselModel()
        {
            return _normalizedShapeVectorIndicator < 0.55 || _isoperimetricQuotient > 0.7;
        }
        #endregion (calculate geometrical description)
    }
}
