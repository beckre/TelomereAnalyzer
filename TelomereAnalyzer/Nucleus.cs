using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace TelomereAnalyzer
{
    public class Nucleus
    {
        public String _nucleusName = "";
        public PointF _nucleusCenterPoint = new Point();
        public PointF[] _nucleusContourPoints = null;
        public List<Telomere> _lstNucleusTelomeres = null;

        //Images for getting all the Pixels inside the Telomere-Contour
        Image<Bgr, byte> _imageForFilledPolygon;
        List<PointF> _allNucleusPoints;
        /*----------------------------------------------------------------------------------------*\
        |* This Class is for managing and storing single Nucleus  Objects                         *|
        \*----------------------------------------------------------------------------------------*/
        public Nucleus(String nucleusName, PointF centerPoint, PointF[] contourPoints)
        {
            this._nucleusName = nucleusName;
            this._nucleusCenterPoint = centerPoint;
            this._nucleusContourPoints = contourPoints;
            _lstNucleusTelomeres = new List<Telomere>();
            _allNucleusPoints = new List<PointF>();
        }

        //Nuclei drawn by the User do not have a center point --> does not have consequences for the rest of the analysis
        public Nucleus(String nucleusName, PointF[] contourPoints)
        {
            this._nucleusName = nucleusName;
            this._nucleusContourPoints = contourPoints;
            _lstNucleusTelomeres = new List<Telomere>();
            _allNucleusPoints = new List<PointF>();
        }
        /*----------------------------------------------------------------------------------------*\
        |* This Method adds every Telomere that is inside of this Nucleus to it's Telomere-List   *|
        |* The examination if a Telomere belongs to a Nucleus is handled in FormOne and in        *|
        |* Nucleus.isTelomereContourInThisNucleus()                                               *|
        \*----------------------------------------------------------------------------------------*/
        public void AddTelomereToTelomereList(Telomere telomere)
        {
            _lstNucleusTelomeres.Add(telomere);
        }
        /*----------------------------------------------------------------------------------------*\
        |* This Methods gets the amount of Pixels that are in a Nucleus Spot.                     *|
        |* In order to do that another Image is created out of the Nuclei Image.                  *|
        |* The Nucleus in the other Images is firstly filled in red. Then every Pixel             *|
        |* in the roi Image is checked if it's red. If red, then the Coordinate to that Pixel is  *|
        |* stored inside a list. This List contains every Pixel of the Nucleus at the end.        *|
        |* To access the original Pixel values the reference Image is used.                       *|
        \*----------------------------------------------------------------------------------------*/
        public void getAmountOfPixelsInNucleusArea(Image<Gray, byte> imageForReference)
        {
            Bitmap btmReference = new Bitmap(imageForReference.ToBitmap());
            Graphics graphics = Graphics.FromImage(btmReference);

            //First the Polygon is filled in red in the reference Bitmap
            SolidBrush redBrush = new SolidBrush(Color.Red);
            graphics.FillPolygon(redBrush, _nucleusContourPoints);

            _imageForFilledPolygon = new Image<Bgr, byte>(btmReference);
            Bitmap btmp = _imageForFilledPolygon.ToBitmap();
            //Then every Pixel in the filled Polygon Image is checked if it's red --> if yes, then the Pixel is added to the List of every Pixel of the entirety of the Telomere
            int width = btmp.Width;
            int height = btmp.Height;
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    Color checkedColor = btmp.GetPixel(w, h);
                    if (Color.Red.ToArgb().Equals(checkedColor.ToArgb()))
                    {
                        Point p = new Point();
                        p.X = w;
                        p.Y = h;
                        _allNucleusPoints.Add(p);
                    }
                }
            }
            //Very small Areas of Telomere Contours cannot be filled with .fillPolygon()
            if (_nucleusContourPoints.Length < 8)
            {
                for (Int32 i = 0; i < _nucleusContourPoints.Length; i++)
                {
                    Point point = Point.Round(_nucleusContourPoints[i]);
                    _allNucleusPoints.Add(point);
                }
            }
        }
        /*----------------------------------------------------------------------------------------*\
        |* Checks if any point of a Telomere-Contour is inside of a Nucleus.                      *|
        |* If yes, then return true, if yes return false.                                         *|
        \*----------------------------------------------------------------------------------------*/
        public Boolean isTelomereContourInThisNucleus(PointF[] telomereContour)
        {
            if (_allNucleusPoints.Count <= 0)
                return false;

            Boolean isPointInNucleusArea = false;
            for (Int32 p = 0; p < telomereContour.Length; p++)
            {
                if (_allNucleusPoints.Contains(telomereContour[p]))
                    isPointInNucleusArea = true;
                else
                    return false;
            }
            return isPointInNucleusArea;
        }
    }
}
