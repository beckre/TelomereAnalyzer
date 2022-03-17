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
    class Nuclei
    {
        List<Nucleus> _allNucleiCoordinates = null;
        public Image<Bgr, byte> _imageToDrawOn = null;

        String _resultValues = null;
        Int32 _contourFound = 0;
        Point[] _centerPoints = null;
        Point[][] _allContours = null;

        public Nuclei( )
        {
            _allNucleiCoordinates = new List<Nucleus>();
        }

        public void SetAttributes(String resultValues, Int32 contourFound, Point[] centerPoints, Point[][] allContours)
        {
            this._resultValues = resultValues;
            this._contourFound = contourFound;
            this._centerPoints = centerPoints;
            this._allContours = allContours;
        }

        public void AddNucleusToNucleiList(Nucleus nucleus)
        {
            if (_allNucleiCoordinates != null)
                _allNucleiCoordinates.Add(nucleus);
        }

        /*
         * Testing drawing the contours here to check if the values that are stored inside of the Nucleus and Nuclei class are correct
         */

        public void PrepareDrawingCenterPoints()
        {
            if (_allNucleiCoordinates != null)
                for (Int32 E = 0; E < _allNucleiCoordinates.Count; E++)
                {
                    if(_allNucleiCoordinates.ElementAt(E) != null)
                        DrawPoint(_allNucleiCoordinates.ElementAt(E)._centerPoint);
                }
                    
            
        }

        public void PrepareDrawingContoursByNucleus()
        {
            if(_allNucleiCoordinates != null)
            {
                for(Int32 E = 0; E < _allNucleiCoordinates.Count; E++)
                {
                    if(_allNucleiCoordinates.ElementAt(E) != null)
                    {
                        if(_allNucleiCoordinates.ElementAt(E)._contourPoints != null)
                        {
                            DrawContour(_allNucleiCoordinates.ElementAt(E)._contourPoints);
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
            Console.WriteLine(_contourFound.ToString() + " Objekte gefunden\n\n" + _resultValues);
        }

        public Boolean IsImageOkay(Image<Bgr, byte> image)
        {
            if (image == null)
                return false;
            return true;
        }
    }
}
