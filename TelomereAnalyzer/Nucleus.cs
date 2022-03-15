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
        Int32 contourFound = 0;
        String resultValues = null;
        Point[] centerPoints = null;
        Point centerPoint = new Point();
        Point[] contourPoints = null;
        Point[][] allContours = null;


    }
}
