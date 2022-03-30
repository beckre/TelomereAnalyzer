using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TelomereAnalyzer
{
    public class Nuclei
    {
        public List<Nucleus> _LstAllNuclei = null;
        public Image<Bgr, byte> _imageToDrawOn = null;
        public Bitmap _btmImageToDrawOn = null;

        String _nucleiResultValues = null;
        Int32 _nucleiContourFound = 0;
        //Point[] _nucleiCenterPoints = null;
        PointF[] _nucleiCenterPoints = null;
        //Point[][] _nucleiAllContours = null;
        PointF[][] _nucleiAllContours = null;

        public Nuclei( )
        {
            _LstAllNuclei = new List<Nucleus>();
        }

        public void SetAttributes(String resultValues, Int32 contourFound, PointF[] centerPoints, PointF[][] allContours)
        {
            this._nucleiResultValues = resultValues;
            this._nucleiContourFound = contourFound;
            this._nucleiCenterPoints = centerPoints;
            this._nucleiAllContours = allContours;
        }

        public void AddNucleusToNucleiList(Nucleus nucleus)
        {
            if (_LstAllNuclei != null)
                _LstAllNuclei.Add(nucleus);
        }

        /*
         * Testing drawing the contours here to check if the values that are stored inside of the Nucleus and Nuclei class are correct
         */

        public void PrepareDrawingCenterPoints()
        {
            if (_LstAllNuclei != null)
                for (Int32 E = 0; E < _LstAllNuclei.Count; E++)
                {
                    if(_LstAllNuclei.ElementAt(E) != null)
                        DrawPoint(_LstAllNuclei.ElementAt(E)._nucleusCenterPoint);
                }
                    
            
        }

        public void PrepareDrawingContoursByNucleus()
        {
            if(_LstAllNuclei != null)
            {
                for(Int32 E = 0; E < _LstAllNuclei.Count; E++)
                {
                    if(_LstAllNuclei.ElementAt(E) != null)
                    {
                        if(_LstAllNuclei.ElementAt(E)._nucleusContourPoints != null)
                        {
                            DrawContour(_LstAllNuclei.ElementAt(E)._nucleusContourPoints);
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

        private void DrawPoint(PointF point)
        {
            Int32 sizeCross = 5;
            PointF[] halfLRCross = new PointF[2];
            PointF[] halfTBCross = new PointF[2];

            halfLRCross[0] = new PointF(point.X - sizeCross, point.Y);
            halfLRCross[1] = new PointF(point.X + sizeCross, point.Y);

            halfTBCross[0] = new PointF(point.X, point.Y - sizeCross);
            halfTBCross[1] = new PointF(point.X, point.Y + sizeCross);

            Bgr colorBlue = new Bgr(Color.Blue);

            if (IsImageOkay(_imageToDrawOn))
            {
                /*Hier Center Points malen
                _imageToDrawOn.DrawPolyline(halfLRCross, false, colorBlue, 1);
                _imageToDrawOn.DrawPolyline(halfTBCross, false, colorBlue, 1);
                */
            }

        }

        private void DrawContour(PointF[] contour)
        {
            Bgr color = new Bgr(Color.DarkViolet);
            _btmImageToDrawOn = _imageToDrawOn.ToBitmap();

            //_imageToDrawOn.DrawPolyline(contour, true, color, 1);
            Graphics graphics = Graphics.FromImage(_btmImageToDrawOn);
            graphics.DrawPolygon(Pens.Blue, contour);
            _imageToDrawOn = new Image<Bgr, byte>(_btmImageToDrawOn);
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

        public Boolean IsCenterPointOfTelomereInNucleus(PointF[] contour, PointF centerPointOfTelomere)
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
