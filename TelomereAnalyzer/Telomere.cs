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
        public PointF _telomereCenterPoint = new Point();
        public PointF[] _telomereContourPoints = null;
        //Stuff for Calculations
        //public Contour<PointF> _contour;
        public List<PointF> _allTelomerePoints;
        public double _area;
        public float _lowestX;
        public float _lowestY;
        public long _sum;
        public int _min;
        public int _max;
        public int _stdDev;
        public int _mean;

        //Images for getting all the Pixels inside the Telomere-Contour
        //Must be different when 16Bit or 8Bit Image --> First only 16 BIt is handled
        Image<Bgr, UInt16> _imageForFilledPolygon;
        Image<Gray, UInt16> _imageForTelomerePolygon;

        public Telomere(String telomereName, PointF centerPoint, PointF[] contourPoints)
        {
            this._telomereName = telomereName;
            this._telomereCenterPoint = centerPoint;
            this._telomereContourPoints = contourPoints;
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
        public void getLowestXY()
        {
            _lowestX = float.PositiveInfinity;
            _lowestY = float.PositiveInfinity;
            for(int p = 0; p < _telomereContourPoints.Length; p++)
            {
                if (_telomereContourPoints[p].X < _lowestX)
                    _lowestX = _telomereContourPoints[p].X;
                if (_telomereContourPoints[p].Y < _lowestY)
                    _lowestY = _telomereContourPoints[p].Y;
            }
        }

        public void getAmountOfPixelsInTelomereArea(Image<Gray, UInt16> imageForReference)
        {
            imageForReference.Save(@"D:\Hochschule Emden Leer - Bachelor Bioinformatik\Praxisphase Bachelorarbeit Vorbereitungen\Praktikumsstelle\MHH Hannover Telomere\WE Transfer\Bilder filled Polygon\Referenz-Bild.jpg");
            Bitmap btmReference = new Bitmap(imageForReference.ToBitmap());
            Graphics graphics = Graphics.FromImage(btmReference);

            //First the Polygon is filled in red on an empty image
            SolidBrush redBrush = new SolidBrush(Color.Red);
            graphics.FillPolygon(redBrush, _telomereContourPoints);
            _imageForFilledPolygon = new Image<Bgr, UInt16>(btmReference);
            _imageForFilledPolygon.Save(@"D:\Hochschule Emden Leer - Bachelor Bioinformatik\Praxisphase Bachelorarbeit Vorbereitungen\Praktikumsstelle\MHH Hannover Telomere\WE Transfer\Bilder filled Polygon\TelomerInRot.jpg");
            Bitmap btmp = _imageForFilledPolygon.ToBitmap();

            //Then every Pixel in the filled Polygon Image is checked if it's red --> if yes, then the Pixel is added to the List of every Pixel of the entirety of the Telomere
            _allTelomerePoints = new List<PointF>();

            int width = btmp.Width;
            int height = btmp.Height;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color checkedColor = btmp.GetPixel(i, j);
                    //if (checkedColor.Equals(Color.Red))
                    if(Color.Red.ToArgb().Equals(checkedColor.ToArgb()))
                    {
                        //Is that really the correct way to get the coordinates?
                        PointF p = new PointF();
                        p.X = i;
                        p.Y = j;
                        _allTelomerePoints.Add(p);
                    }
                }
            }
            _area = _allTelomerePoints.Count;
        }
        #endregion
    }
}
