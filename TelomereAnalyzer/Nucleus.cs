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
    public class Nucleus
    {
        public String _nucleusName = "";
        public PointF _nucleusCenterPoint = new Point();
        public PointF[] _nucleusContourPoints = null;
        public List<Telomere> _lstNucleusTelomeres = null;

        //Images for getting all the Pixels inside the Telomere-Contour
        //Must be different when 16Bit or 8Bit Image --> First only 16 Bit is handled
        Image<Bgr, byte> _imageForFilledPolygon;
        List<PointF> _allNucleusPoints;

        public Nucleus(String nucleusName, PointF centerPoint, PointF[] contourPoints)
        {
            this._nucleusName = nucleusName;
            this._nucleusCenterPoint = centerPoint;
            this._nucleusContourPoints = contourPoints;
            _lstNucleusTelomeres = new List<Telomere>();
            _allNucleusPoints = new List<PointF>();
        }

        //Selbst gemalte und hinzugefügte Nuclei haben noch keinen Center Point --> nicht schlimm, wird nicht gebraucht
        public Nucleus(String nucleusName, PointF[] contourPoints)
        {
            this._nucleusName = nucleusName;
            this._nucleusContourPoints = contourPoints;
            _lstNucleusTelomeres = new List<Telomere>();
            _allNucleusPoints = new List<PointF>();
        }

        public void AddTelomereToTelomereList(Telomere telomere)
        {
            _lstNucleusTelomeres.Add(telomere);
        }

        public void getAmountOfPixelsInNucleusArea(Image<Gray, byte> imageForReference)
        {   //No ROI here

            Bitmap btmReference = new Bitmap(imageForReference.ToBitmap());
            Graphics graphics = Graphics.FromImage(btmReference);
            //First the Polygon is filled in red in the reference Bitmap
            SolidBrush redBrush = new SolidBrush(Color.Red);
            graphics.FillPolygon(redBrush, _nucleusContourPoints);
            _imageForFilledPolygon = new Image<Bgr, byte>(btmReference);
            _imageForFilledPolygon.Save(@"D:\Hochschule Emden Leer - Bachelor Bioinformatik\Praxisphase Bachelorarbeit Vorbereitungen\Praktikumsstelle\MHH Hannover Telomere\WE Transfer\Bilder filled Polygon\ROI Red Nucleus " + this._nucleusName + ".jpg");
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
                        _allNucleusPoints.Add(p); // Liste war null
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
