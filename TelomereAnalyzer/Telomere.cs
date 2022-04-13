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
        public float _lowestX;
        public float _lowestY;
        public long _sum;
        public int _area;
        public int _min;
        public int _max;
        public int _stdDev;
        public int _mean;

        public Telomere(String telomereName, PointF centerPoint, PointF[] contourPoints)
        {
            this._telomereName = telomereName;
            this._telomereCenterPoint = centerPoint;
            this._telomereContourPoints = contourPoints;
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

        public void getAmountOfPixelsInTelomereArea()
        {

        }
        #endregion
    }
}
