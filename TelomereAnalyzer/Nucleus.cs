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
        //public Point _nucleusCenterPoint = new Point();
        public PointF _nucleusCenterPoint = new Point();
        //public Point[] _nucleusContourPoints = null;
        public PointF[] _nucleusContourPoints = null;
        public List<Telomere> _LstnucleusTelomeres = null;

        public Nucleus(String nucleusName, PointF centerPoint, PointF[] contourPoints)
        {
            this._nucleusName = nucleusName;
            this._nucleusCenterPoint = centerPoint;
            this._nucleusContourPoints = contourPoints;
            _LstnucleusTelomeres = new List<Telomere>();
        }

        //Selbst gemalte und hinzugefügte Nuclei haben noch keinen Center Point!!
        public Nucleus(String nucleusName, PointF[] contourPoints)
        {
            this._nucleusName = nucleusName;
            this._nucleusContourPoints = contourPoints;
            _LstnucleusTelomeres = new List<Telomere>();
        }

        public void AddTelomereToTelomereList(Telomere telomere)
        {
            _LstnucleusTelomeres.Add(telomere);
        }
    }
}
