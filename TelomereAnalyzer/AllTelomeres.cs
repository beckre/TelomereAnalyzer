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
    public class AllTelomeres
    {
        public List<Telomere> _lstAllTelomeres = null;
        public Image<Bgr, byte> _imageToDrawOn = null;
        public Bitmap _btmImageToDrawOn = null;

        String _allTelomeresResultValues = null;
        Int32 _allTelomeresContourFound = 0;
        PointF[] _allTelomeresCenterPoints = null;
        PointF[][] _allTelomeresAllContours = null;

        public AllTelomeres()
        {
            _lstAllTelomeres = new List<Telomere>();
        }

        public void SetAttributes(String resultValues, Int32 contourFound, PointF[] centerPoints, PointF[][] allContours)
        {
            this._allTelomeresResultValues = resultValues;
            this._allTelomeresContourFound = contourFound;
            this._allTelomeresCenterPoints = centerPoints;
            this._allTelomeresAllContours = allContours;
        }

        //Im Moment werden absolut alle validen Telomere hier eingespeichert, auch die, die außerhabl von Nuclei-Bereichen sind
        public void AddTelomereToAllTelomeresList(Telomere telomere)
        {
            if (_lstAllTelomeres != null)
                _lstAllTelomeres.Add(telomere);
        }

        public void PrepareDrawingCenterPoints()
        {
            if (_lstAllTelomeres != null)
                for (Int32 E = 0; E < _lstAllTelomeres.Count; E++)
                {
                    if (_lstAllTelomeres.ElementAt(E) != null)
                        DrawPoint(_lstAllTelomeres.ElementAt(E)._telomereCenterPoint);
                }


        }

        public void PrepareDrawingContoursByTelomere()
        {
            if (_lstAllTelomeres != null)
            {
                for (Int32 E = 0; E < _lstAllTelomeres.Count; E++)
                {
                    if (_lstAllTelomeres.ElementAt(E) != null)
                    {
                        if (_lstAllTelomeres.ElementAt(E)._telomereContourPoints != null)
                        {
                            DrawContour(_lstAllTelomeres.ElementAt(E)._telomereContourPoints);
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
            /*
            Bgr color = new Bgr(Color.Yellow);

            _imageToDrawOn.DrawPolyline(contour, true, color, 1);
            */

            Bgr color = new Bgr(Color.DarkViolet);
            _btmImageToDrawOn = _imageToDrawOn.ToBitmap();

            //_imageToDrawOn.DrawPolyline(contour, true, color, 1);
            Graphics graphics = Graphics.FromImage(_btmImageToDrawOn);
            graphics.DrawPolygon(Pens.Blue, contour);
            _imageToDrawOn = new Image<Bgr, byte>(_btmImageToDrawOn);
        }

        public void PrintResultValues()
        {
            Console.WriteLine(_allTelomeresContourFound.ToString() + " Objekte gefunden\n\n" + _allTelomeresResultValues);
        }

        public Boolean IsImageOkay(Image<Bgr, byte> image)
        {
            if (image == null)
                return false;
            return true;
        }
    }
}
