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
        //Nucleus _nucleusOfTelomere = null;
        public PointF _telomereCenterPoint = new PointF();
        public PointF[] _telomereContourPoints = null;
        //Stuff for Calculations
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
        //Must be different when 16Bit or 8Bit Image --> First only 16 Bit is handled
        Image<Bgr, byte> _ROIimageForFilledPolygon;
        Image<Gray, byte> _ROIimageForTelomerePolygon;
        Point _location;
        public List<Point> _allTelomerePoints;

        public Telomere(String telomereName, PointF centerPoint, PointF[] contourPoints)
        {
            this._telomereName = telomereName;
            this._telomereCenterPoint = centerPoint;
            this._telomereContourPoints = contourPoints;
            _allTelomerePoints = new List<Point>();
            //this._contour = contourPoints;
            // Nun müsste in der Contour<PointF> Liste das gleiche drin sein wie in dem telomer-Contour Array 
            /*
            for(Int32 i = 0; i < _telomereContourPoints.Length; i++)
            {
                _contour.Insert(i, _telomereContourPoints[i]);

            }
            */
        }

        #region Calculations--------------------------------------------------------------------------------------------------------
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

        public void getAmountOfPixelsInTelomereArea(Image<Gray, byte> imageForReference)
        {
            //imageForReference.Save(@"D:\Hochschule Emden Leer - Bachelor Bioinformatik\Praxisphase Bachelorarbeit Vorbereitungen\Praktikumsstelle\MHH Hannover Telomere\WE Transfer\Bilder filled Polygon\Referenz-Bild.jpg");
            Rectangle rec = Rectangle.Empty;
            rec.Width = (int)(_highestX - _lowestX);
            rec.Height = (int)(_highestY - _lowestY);
            rec.Location = _location;
            //imageForReference.ROI = rec;
            //Copies the segment of the roi into this image
            _ROIimageForTelomerePolygon = imageForReference.Copy(rec);
            //Bitmap btmReference = new Bitmap(imageForReference.ToBitmap());
            Bitmap btmReference = new Bitmap(imageForReference.ToBitmap());
            Graphics graphics = Graphics.FromImage(btmReference);

            //First the Polygon is filled in red in the reference Bitmap
            SolidBrush redBrush = new SolidBrush(Color.Red);
            graphics.FillPolygon(redBrush, _telomereContourPoints);
            //imageForReference = new Image<Bgr, UInt16>(btmReference);
            Image<Bgr, byte> tempRedFilledPolygonImg = new Image<Bgr, byte>(btmReference);
            //tempRedFilledPolygonImg.Save(@"D:\Hochschule Emden Leer - Bachelor Bioinformatik\Praxisphase Bachelorarbeit Vorbereitungen\Praktikumsstelle\MHH Hannover Telomere\WE Transfer\Bilder filled Polygon\Reference Telomer " + this._telomereName + ".jpg");
            //tempRedFilledPolygonImg.ROI = rec;
            _ROIimageForFilledPolygon = tempRedFilledPolygonImg.Copy(rec);

            //_ROIimageForFilledPolygon.Save(@"D:\Hochschule Emden Leer - Bachelor Bioinformatik\Praxisphase Bachelorarbeit Vorbereitungen\Praktikumsstelle\MHH Hannover Telomere\WE Transfer\Bilder filled Polygon\ROI Red Telomer " + this._telomereName + ".jpg");
            //_ROIimageForTelomerePolygon.Save(@"D:\Hochschule Emden Leer - Bachelor Bioinformatik\Praxisphase Bachelorarbeit Vorbereitungen\Praktikumsstelle\MHH Hannover Telomere\WE Transfer\Bilder filled Polygon\ROI Telomer " + this._telomereName + ".jpg");

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

        public void getSumMinMaxMeanStddevOfTelomere(Image<Gray, byte> imageForReference)
        {
            Bitmap btmp = null;
            if (_telomereContourPoints.Length < 8)
            {
                //temporäre Lösung für sehr kleine Telomere unter 8 Pixel --> muss noch geändert werden
                btmp = imageForReference.ToBitmap();
            }
            else
            {
                btmp = _ROIimageForTelomerePolygon.ToBitmap();
            }
            List<double> redOfPixel = new List<double>();
            //Bitmap btmp = new Bitmap(image.ToBitmap());

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
            if(_area <= 0)
            {
                _min = 0;
                _max = 0;
            }
            if (_allTelomerePoints.Count > 0)
                _mean = _sum / _allTelomerePoints.Count;
            else
                _mean = 0;
            //Calculate the standard deviation
            double ret = 0;
            //Perform the Sum of (value-avg)^2
            double sum = redOfPixel.Sum(d => (d - _mean) * (d - _mean));
            //Put it all together
            ret = Math.Sqrt(sum / _allTelomerePoints.Count);
            _stdDev = ret;
        }
        #endregion
    }
}
