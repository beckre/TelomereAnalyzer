using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;


namespace TelomereAnalyzer
{
    public partial class Detection : Form
    {
        public FormOne _formOne = null;
        public Image<Gray, byte> _nucleiBitonalForEdgeDetection = null;

        public Bitmap _oriImage = null;
        public Image<Gray, byte> _grayImage = null;

        vesselDetection _imgProcessor = null;

        /*----------------------------------------------------------------------------------------*\
        |* This Class starts the preparations for the Nuclei Image so that the Nuclei can         *|
        |* be detected and borders can be drawn around them                                       *|
        \*----------------------------------------------------------------------------------------*/
        public Detection(FormOne formOne)
        {
            _formOne = formOne;
        }
        /*----------------------------------------------------------------------------------------*\
        |* This Method is called in FormOne.DetectNuclei()                                        *|
        |* Stores the auto-leveld Nuclei Image FormOne._NucleiImageAutoLevel into Image and       *|
        |* Bitmap Objects. Creates a new vesselDetection Object                                   *|
        |* and calls vesselDetection.DoThresholding()                                             *|
        \*----------------------------------------------------------------------------------------*/
        public bool DoAnalyze(Image<Gray, byte> imageToAnalyze)
        {
            _oriImage = imageToAnalyze.ToBitmap();
            _grayImage = imageToAnalyze;

            if (_imgProcessor == null)
                _imgProcessor = new vesselDetection(this);
            _imgProcessor.DoThresholding();
            return true;
        }
    }
}


