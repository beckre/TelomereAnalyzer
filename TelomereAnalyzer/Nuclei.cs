using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Drawing;

namespace TelomereAnalyzer
{
    public class Nuclei
    {
        public List<Nucleus> _allNuclei = null;
        public Image<Bgr, byte> _imageToDrawOn = null;

        String _nucleiResultValues = null;
        Int32 _nucleiContourFound = 0;
        Point[] _nucleiCenterPoints = null;
        Point[][] _nucleiAllContours = null;

        public Nuclei( )
        {
            _allNuclei = new List<Nucleus>();
        }

        public void SetAttributes(String resultValues, Int32 contourFound, Point[] centerPoints, Point[][] allContours)
        {
            this._nucleiResultValues = resultValues;
            this._nucleiContourFound = contourFound;
            this._nucleiCenterPoints = centerPoints;
            this._nucleiAllContours = allContours;
        }

        public void AddNucleusToNucleiList(Nucleus nucleus)
        {
            if (_allNuclei != null)
                _allNuclei.Add(nucleus);
        }

        /*
         * Testing drawing the contours here to check if the values that are stored inside of the Nucleus and Nuclei class are correct
         */

        public void PrepareDrawingCenterPoints()
        {
            if (_allNuclei != null)
                for (Int32 E = 0; E < _allNuclei.Count; E++)
                {
                    if(_allNuclei.ElementAt(E) != null)
                        DrawPoint(_allNuclei.ElementAt(E)._nucleusCenterPoint);
                }
                    
            
        }

        public void PrepareDrawingContoursByNucleus()
        {
            if(_allNuclei != null)
            {
                for(Int32 E = 0; E < _allNuclei.Count; E++)
                {
                    if(_allNuclei.ElementAt(E) != null)
                    {
                        if(_allNuclei.ElementAt(E)._nucleusContourPoints != null)
                        {
                            DrawContour(_allNuclei.ElementAt(E)._nucleusContourPoints);
                            /*
                            Point[] points = _allNucleiCoordinates.ElementAt(E)._contourPoints;
                            for (Int32 J = 0; J < points.Length; J++)
                            {
                                DrawContour(points[J]);
                            }
                            */
                        }
                    }

                }
            }
        }

        private void DrawPoint(Point point)
        {
            Int32 sizeCross = 5;
            Point[] halfLRCross = new Point[2];
            Point[] halfTBCross = new Point[2];

            halfLRCross[0] = new Point(point.X - sizeCross, point.Y);
            halfLRCross[1] = new Point(point.X + sizeCross, point.Y);

            halfTBCross[0] = new Point(point.X, point.Y - sizeCross);
            halfTBCross[1] = new Point(point.X, point.Y + sizeCross);

            Bgr colorBlue = new Bgr(Color.Blue);

            if (IsImageOkay(_imageToDrawOn))
            {
                _imageToDrawOn.DrawPolyline(halfLRCross, false, colorBlue, 1);
                _imageToDrawOn.DrawPolyline(halfTBCross, false, colorBlue, 1);
            }

        }

        private void DrawContour(Point[] contour)
        {
            Bgr color = new Bgr(Color.DarkViolet);

            _imageToDrawOn.DrawPolyline(contour, true, color, 1);
        }

        public void PrintResultValues()
        {
            Console.WriteLine(_nucleiContourFound.ToString() + " Objekte gefunden\n\n" + _nucleiResultValues);
        }

        public Boolean IsImageOkay(Image<Bgr, byte> image)
        {
            if (image == null)
                return false;
            return true;
        }

        public Boolean IsCenterPointOfTelomereInNucleus(Point[] contour, Point centerPointOfTelomere)
        {
            bool result = false;
            int j = contour.Count() - 1;
            for (int i = 0; i < contour.Count(); i++)
            {
                if (contour[i].Y < centerPointOfTelomere.Y && contour[j].Y >= centerPointOfTelomere.Y || contour[j].Y < centerPointOfTelomere.Y && contour[i].Y >= centerPointOfTelomere.Y)
                {
                    if (contour[i].X + (centerPointOfTelomere.Y - contour[i].Y) / (contour[j].Y - contour[i].Y) * (contour[j].X - contour[i].X) < centerPointOfTelomere.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }
    }
}
