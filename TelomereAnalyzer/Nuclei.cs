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
        public List<Nucleus> _lstAllNuclei = null;
        public Image<Bgr, byte> _imageToDrawOn = null;
        public Bitmap _btmImageToDrawOn = null;

        String _nucleiResultValues = null;
        Int32 _nucleiContourFound = 0;
        PointF[] _nucleiCenterPoints = null;
        PointF[][] _nucleiAllContours = null;
        /*----------------------------------------------------------------------------------------*\
        |* This Class is for managing and storing all the Nucleus-Objects.                        *|
        \*----------------------------------------------------------------------------------------*/
        public Nuclei()
        {
            _lstAllNuclei = new List<Nucleus>();
        }

        public void SetAttributes(String resultValues, Int32 contourFound, PointF[] centerPoints, PointF[][] allContours)
        {
            this._nucleiResultValues = resultValues;
            this._nucleiContourFound = contourFound;
            this._nucleiCenterPoints = centerPoints;
            this._nucleiAllContours = allContours;
        }
        /*----------------------------------------------------------------------------------------*\
        |* This Method adds every Nucleus Object to a List.                                       *|
        \*----------------------------------------------------------------------------------------*/
        public void AddNucleusToNucleiList(Nucleus nucleus)
        {
            if (_lstAllNuclei != null)
                _lstAllNuclei.Add(nucleus);
        }
        #region Drawing Nucleus-Coordinates-----------------------------------------------------------------------------------
        /*----------------------------------------------------------------------------------------*\
        |* Methods for drawing the Nuclues-Borders and Center Point.                              *|
        \*----------------------------------------------------------------------------------------*/
        public void PrepareDrawingCenterPoints()
        {
            if (_lstAllNuclei != null)
                for (Int32 E = 0; E < _lstAllNuclei.Count; E++)
                {
                    if (_lstAllNuclei.ElementAt(E) != null)
                        DrawPoint(_imageToDrawOn, _lstAllNuclei.ElementAt(E)._nucleusCenterPoint);
                }
        }
        public void PrepareDrawingContoursByNucleus(Bgr color)
        {
            if (_lstAllNuclei != null)
            {
                for (Int32 E = 0; E < _lstAllNuclei.Count; E++)
                {
                    if (_lstAllNuclei.ElementAt(E) != null)
                    {
                        if (_lstAllNuclei.ElementAt(E)._nucleusContourPoints != null)
                        {
                            _imageToDrawOn = DrawContour(_imageToDrawOn, _lstAllNuclei.ElementAt(E)._nucleusContourPoints, color);
                            _btmImageToDrawOn = _imageToDrawOn.ToBitmap();
                        }
                    }
                }
            }
        }
        private void DrawPoint(Image<Bgr, byte> imageToDrawOn, PointF point)
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
        public Image<Bgr, byte> DrawContour(Image<Bgr, byte> imageToDrawOn, PointF[] contour, Bgr color)
        {
            Bitmap btmp = imageToDrawOn.ToBitmap();
            Graphics graphics = Graphics.FromImage(btmp);
            graphics.DrawPolygon(Pens.Green, contour);
            imageToDrawOn = new Image<Bgr, byte>(btmp);
            Image<Bgr, byte> imageForDrawing = imageToDrawOn;
            return imageForDrawing;
        }
        #endregion
        public void PrintResultValues()
        {
            Console.WriteLine(_nucleiContourFound.ToString() + " Objekte gefunden\n\n" + _nucleiResultValues);
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
