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

        vesselDetectorClass _imgProcessor = null;

        public Bitmap _oriImage = null;
        public Image<Gray, byte> _grayImage = null;

        public String _resultFile = null;

        public bool _mergeToOneResultfile = true;

        #region stuff for vessel analysis
        // Result of thresholding procedure
    
        // Cluster analysis
        VesselAnalysisClass _vesselAnalysis = null;  
        public Emgu.CV.UI.ImageBox ImageBoxTesting;
        public Label lblTesting;

        #endregion(stuff for vessel analysis)

        public Detection(FormOne formOne)
        {
            _formOne = formOne;
        }
        public bool DoAnalyze(Image<Gray, byte> imageToAnalyze)
        {
            /*
            if (_vesselsFound != null)
            {
                for (Int32 V = 0; V < _vesselsFound.Length; V++)
                    _vesselsFound[V].Dispose();
            }
            _vesselsFound = null;
            */
            if (_vesselAnalysis != null)
                _vesselAnalysis.Dispose();

            _vesselAnalysis = null;

            _oriImage = imageToAnalyze.ToBitmap();
            _grayImage = imageToAnalyze.Convert<Gray, byte>();
            //Application.DoEvents();
            if (_imgProcessor == null)
                _imgProcessor = new vesselDetectorClass(this);
            /*
            if (_imgProcessor.DoThresholding(ref _vesselsFound) == false)
            {
                return false;
            }
            */
            _imgProcessor.DoThresholding();
            return true;
        }
    }
}


