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
    class AllTelomeres
    {
        List<Telomere> _allTelomeres = null;

        String _allTelomeresResultValues = null;
        Int32 _allTelomeresContourFound = 0;
        Point[] _allTelomeresCenterPoints = null;
        Point[][] _allTelomeresAllContours = null;

        public AllTelomeres()
        {
            _allTelomeres = new List<Telomere>();
        }

        public void SetAttributes(String resultValues, Int32 contourFound, Point[] centerPoints, Point[][] allContours)
        {
            this._allTelomeresResultValues = resultValues;
            this._allTelomeresContourFound = contourFound;
            this._allTelomeresCenterPoints = centerPoints;
            this._allTelomeresAllContours = allContours;
        }

        //Im Moment werden absolut alle validen Telomere hier eingespeichert, auch die, die außerhabl von Nuclei-Bereichen sind
        public void AddTelomereToAllTelomeresList(Telomere telomere)
        {
            if (_allTelomeres != null)
                _allTelomeres.Add(telomere);
        }
    }
}
