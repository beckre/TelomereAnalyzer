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
        /*----------------------------------------------------------------------------------------*\
        |* This Class is for managing and storing all the Telomere-Objects.                       *|
        \*----------------------------------------------------------------------------------------*/
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
        /*----------------------------------------------------------------------------------------*\
        |* This Method adds EVERY Telomere-Contour that is found to a List.                       *|
        |* Also contains Telomeres that are outside of a Nucleus. The Allocation of the Telomers  *|
        |* to their Nucleus happens in FormOne and the Nucleus Class.                             *|
        \*----------------------------------------------------------------------------------------*/
        public void AddTelomereToAllTelomeresList(Telomere telomere)
        {
            if (_lstAllTelomeres != null)
                _lstAllTelomeres.Add(telomere);
        }
        #region Drawing of Telomere-Coordinates-------------------------------------------------------------------
        /*----------------------------------------------------------------------------------------*\
        |* Methods for drawing the Telomer-Borders and Center Point.                              *|
        \*----------------------------------------------------------------------------------------*/
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
                /*Drawing Center Points
                _imageToDrawOn.DrawPolyline(halfLRCross, false, colorBlue, 1);
                _imageToDrawOn.DrawPolyline(halfTBCross, false, colorBlue, 1);
                */
            }
        }
        private void DrawContour(PointF[] contour)
        {
            Bgr color = new Bgr(Color.DarkViolet);
            _btmImageToDrawOn = _imageToDrawOn.ToBitmap();
            Graphics graphics = Graphics.FromImage(_btmImageToDrawOn);
            graphics.DrawPolygon(Pens.Blue, contour);
            _imageToDrawOn = new Image<Bgr, byte>(_btmImageToDrawOn);
        }
        #endregion
        public void PrintResultValues()
        {
            Console.WriteLine(_allTelomeresContourFound.ToString() + " Objekte gefunden\n\n" + _allTelomeresResultValues);
        }
        /*---------------------------------------------------------------------------------------*\
        |* Checks if an Image is null                                                            *|
        \*---------------------------------------------------------------------------------------*/
        public Boolean IsImageOkay(Image<Bgr, byte> image)
        {
            if (image == null)
                return false;
            return true;
        }
    }
}
