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
    public class Telomere
    {
        public String _telomereName = "";
        public PointF _telomereCenterPoint = new PointF();
        public PointF[] _telomereContourPoints = null;
        //Vairables for Calculations
        public double _area;
        public float _lowestX;
        public float _highestX;
        public float _lowestY;
        public float _highestY;
        public long _sum;
        public int _min;
        public int _max;
        public double _stdDev;
        public double _mean;

        //Images for getting all the Pixels inside the Telomere-Contour
        Image<Bgr, byte> _ROIimageForFilledPolygon;
        Image<Gray, byte> _ROIimageForTelomerePolygon;
        Point _location;
        public List<Point> _allTelomerePoints;
        /*----------------------------------------------------------------------------------------*\
        |* This Class is for managing and storing single Telomere Objects                         *|
        \*----------------------------------------------------------------------------------------*/
        public Telomere(String telomereName, PointF centerPoint, PointF[] contourPoints)
        {
            this._telomereName = telomereName;
            this._telomereCenterPoint = centerPoint;
            this._telomereContourPoints = contourPoints;
            _allTelomerePoints = new List<Point>();
        }
        #region Calculations--------------------------------------------------------------------------------------------------------
        /*----------------------------------------------------------------------------------------*\
        |* This region handles all the Calculations that should be done.                          *|
        \*----------------------------------------------------------------------------------------*/
        /*----------------------------------------------------------------------------------------*\
        |* This Methods checks what the lowest X and Y and the highest X and Y values out of the  *|
        |* Telomere-Contour-Coordinates are.                                                      *|
        \*----------------------------------------------------------------------------------------*/
        public void getLowestAndHighestXY()
        {
            _lowestX = float.PositiveInfinity;
            _lowestY = float.PositiveInfinity;
            _highestX = float.NegativeInfinity;
            _highestY = float.NegativeInfinity;

            for (int p = 0; p < _telomereContourPoints.Length; p++)
            {
                if (_telomereContourPoints[p].X < _lowestX)
                    _lowestX = _telomereContourPoints[p].X;
                if (_telomereContourPoints[p].Y < _lowestY)
                    _lowestY = _telomereContourPoints[p].Y;
                if (_telomereContourPoints[p].X > _highestX)
                    _highestX = _telomereContourPoints[p].X;
                if (_telomereContourPoints[p].Y > _highestY)
                    _highestY = _telomereContourPoints[p].Y;
            }
            _location = new Point();
            _location.X = (int)_lowestX;
            _location.Y = (int)_lowestY;
        }
        /*----------------------------------------------------------------------------------------*\
        |* This Methods gets the amount of Pixels that are in a Telomere Spot.                    *|
        |* In order to do that two other Image is created out                                     *|
        |* of the Telomere Image that is significantly smaller. It only has the size of the       *|
        |* smallest rectangle (region of interest) that aligns over the Telomere Spot.            *|
        |* The Telomere Spot in one of the roi Images is firstly filled in red. Then every Pixel  *|
        |* in the roi Image is checked if it's red. If red, then the Coordinate to that Pixel is  *|
        |* stored inside a list. This List contains every Pixel of the Telomere at the end.       *|
        |* To access the original Pixel values the uncoloured roi Image is used.                  *|
        \*----------------------------------------------------------------------------------------*/
        public void getAmountOfPixelsInTelomereArea(Image<Gray, byte> imageForReference)
        {
            Rectangle rec = Rectangle.Empty;
            rec.Width = (int)(_highestX - _lowestX);
            rec.Height = (int)(_highestY - _lowestY);
            rec.Location = _location;
            _ROIimageForTelomerePolygon = imageForReference.Copy(rec);
            Bitmap btmReference = new Bitmap(imageForReference.ToBitmap());
            Graphics graphics = Graphics.FromImage(btmReference);

            //First the Polygon is filled in red in the reference Bitmap
            SolidBrush redBrush = new SolidBrush(Color.Red);
            graphics.FillPolygon(redBrush, _telomereContourPoints);

            Image<Bgr, byte> tempRedFilledPolygonImg = new Image<Bgr, byte>(btmReference);
            _ROIimageForFilledPolygon = tempRedFilledPolygonImg.Copy(rec);

            Bitmap btmp = _ROIimageForFilledPolygon.ToBitmap();
            //Then every Pixel in the ROI filled Polygon Image is checked if it's red --> if yes, then the Pixel is added to the List of every Pixel of the entirety of the Telomere
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
                        _allTelomerePoints.Add(p);
                    }
                }
            }
            //Very small Areas of Telomere Contours cannot be filled with .fillPolygon()
            if (_telomereContourPoints.Length < 8)
            {
                for (Int32 i = 0; i < _telomereContourPoints.Length; i++)
                {
                    Point point = Point.Round(_telomereContourPoints[i]);
                    _allTelomerePoints.Add(point);
                }
            }
            _area = _allTelomerePoints.Count;
        }
        /*----------------------------------------------------------------------------------------*\
        |* Calculates the other needed values for the Excel-Sheet.                                *|
        |* Sum: This is the sum of all the intensities of all Pixels inside the Telomere          *|
        |* Min: This is the lowest Pixel-intensity value of all Pixels in the Telomere            *|
        |* Man: This is the highest Pixel-intensity value of all Pixels in the Telomere           *|
        |* Mean: This is the mean of the Pixel-Intensities of all Pixels in the Telomere          *|
        |* stdDev: This is the standard deviation belonging to the mean                           *|
        \*----------------------------------------------------------------------------------------*/
        public void getSumMinMaxMeanStddevOfTelomere(Image<Gray, byte> imageForReference)
        {
            Bitmap btmp = null;
            btmp = _ROIimageForTelomerePolygon.ToBitmap();
            List<double> redOfPixel = new List<double>();
            _sum = 0;
            int tempMin = int.MaxValue;
            int tempMax = int.MinValue;
            for (Int32 p = 0; p < _allTelomerePoints.Count; p++)
            {
                Color tempColor = btmp.GetPixel(_allTelomerePoints[p].X, _allTelomerePoints[p].Y);
                //Sum is calulated
                _sum += tempColor.R;
                redOfPixel.Add(tempColor.R);
                //In a grayscale Image all RGB Values are equal, so it is only necessary to check one of them
                if (tempColor.R < tempMin)
                    tempMin = tempColor.R;
                if (tempColor.R > tempMax)
                    tempMax = tempColor.R;
            }
            _min = tempMin;
            _max = tempMax;
            if (_area <= 0)
            {
                _min = 0;
                _max = 0;
            }
            if (_allTelomerePoints.Count > 0)
                _mean = _sum / _allTelomerePoints.Count;
            else
                _mean = 0;
            //Calculates the standard deviation
            double ret = 0;
            //Perform the Sum of (value-avg)^2
            double sum = redOfPixel.Sum(d => (d - _mean) * (d - _mean));
            ret = Math.Sqrt(sum / _allTelomerePoints.Count);
            _stdDev = ret;
        }
        #endregion
    }
}
