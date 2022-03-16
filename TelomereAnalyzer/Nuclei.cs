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
    class Nuclei
    {
        List<Nucleus> _allNucleiCoordinates = null;

        String _resultValues = null;
        Int32 _contourFound = 0;
        Point[] _centerPoints = null;
        Point[][] _allContours = null;

        public Nuclei( )
        {
            _allNucleiCoordinates = new List<Nucleus>();
        }

        public void SetAttributes(String resultValues, Int32 contourFound, Point[] centerPoints, Point[][] allContours)
        {
            this._resultValues = resultValues;
            this._contourFound = contourFound;
            this._centerPoints = centerPoints;
            this._allContours = allContours;
        }

        public void AddNucleusToNucleiList(Nucleus nucleus)
        {
            if (_allNucleiCoordinates != null)
                _allNucleiCoordinates.Add(nucleus);
        }

    }
}
