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
    class Nucleus
    {
        
        public Point _centerPoint = new Point();
        public Point[] _contourPoints = null;

        public Nucleus( Point centerPoint, Point[] contourPoints)
        {
            this._centerPoint = centerPoint;
            this._contourPoints = contourPoints;
        }

    }
}
