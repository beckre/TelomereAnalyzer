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
    class Telomere
    {
        public String _telomereName = "";
        //Nucleus _nucleusOfTelomere = null;
        public Point _telomereCenterPoint = new Point();
        public Point[] _telomereContourPoints = null;

        public Telomere(String telomereName, Point centerPoint, Point[] contourPoints)
        {
            this._telomereName = telomereName;
            this._telomereCenterPoint = centerPoint;
            this._telomereContourPoints = contourPoints;
            //nucleusOfTelomere = new Nucleus();
        }
    }
}
