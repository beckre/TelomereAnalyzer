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

        vesselDetectorClass _imgProcessor = null;

        public String _resultFile = null;

        public bool _mergeToOneResultfile = true;

        #region stuff for vessel analysis
        //public Emgu.CV.UI.ImageBox ImageBoxTesting;
        //public Label lblTesting;
        #endregion(stuff for vessel analysis)

        public Detection(FormOne formOne)
        {
            _formOne = formOne;
        }
        public bool DoAnalyze(Image<Gray, byte> imageToAnalyze)
        {
            _oriImage = imageToAnalyze.ToBitmap();
            _grayImage = imageToAnalyze.Convert<Gray, byte>();

            if (_imgProcessor == null)
                _imgProcessor = new vesselDetectorClass(this);
            _imgProcessor.DoThresholding();
            return true;
        }
    }
}


