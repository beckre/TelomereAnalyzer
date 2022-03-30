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

        public Telomere(String telomereName, PointF centerPoint, PointF[] contourPoints)
        {
            this._telomereName = telomereName;
            this._telomereCenterPoint = centerPoint;
            this._telomereContourPoints = contourPoints;
            //nucleusOfTelomere = new Nucleus();
        }
    }
}
