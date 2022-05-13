using System;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;


namespace TelomereAnalyzer
{
    public class vesselDetectorClass //: IDisposable
    {
        Detection _parentControl = null;
        struct Settings
        {
            /*
             * Global image settings for grayscale interpretation
             */
            public Double imgBrightness;
            public Double imgContrast;
            public Double imgGamma;

            /*
             * Define span for pixel values
             * Is needed for cv thresholding
             */
            public Double minPixelEnergy;
            public Double maxPixelEnergy;
        }

        Settings _settingsVesselDetector;

        //StochasticsClass _mathStochastics = null;
        //Image<Hsv, Byte> _resultVesselImg = null;

        public vesselDetectorClass(Detection parentControl)
        {
            //parentControl is an Object of the Detection Class
            _parentControl = parentControl;
            InitializeGlobalParameter();
            //_mathStochastics = new StochasticsClass();
        }
        //Values defined through testing
        protected void InitializeGlobalParameter()
        {
            _settingsVesselDetector.imgBrightness = 0.0;
            _settingsVesselDetector.imgContrast = 1.0;
            _settingsVesselDetector.imgGamma = 1.5;
            _settingsVesselDetector.minPixelEnergy = 25.0;
            _settingsVesselDetector.maxPixelEnergy = 255.0;
        }
        public bool DoThresholding()
        {
            Gray colorWhite = new Gray(255);
            MemStorage mem = new MemStorage();
            //copies the auto-leveled Image from FormOne which was given to Detection into the shadowImage
            Image<Gray, byte> shadowImage = _parentControl._grayImage.CopyBlank();

            //_parentControl._grayImage.Save("1_OriImage.tiff");

            #region--2. Binary image erzeugen
            //this is necessary for the further Image editing
            shadowImage = _parentControl._grayImage.ConvertScale<Byte>(_settingsVesselDetector.imgContrast, _settingsVesselDetector.imgBrightness);
            //gamma Correction
            shadowImage._GammaCorrect(_settingsVesselDetector.imgGamma);

            //Threshold is calculated
            CvInvoke.cvThreshold(shadowImage, shadowImage, _settingsVesselDetector.minPixelEnergy, 255, THRESH.CV_THRESH_TOZERO);
            CvInvoke.cvThreshold(shadowImage, shadowImage, _settingsVesselDetector.maxPixelEnergy, 255, THRESH.CV_THRESH_TRUNC);

            Image<Gray, byte> Bin = shadowImage.CopyBlank();
            CvInvoke.cvThreshold(shadowImage, Bin, 0.0, 255.0, THRESH.CV_THRESH_OTSU);

            //fills the holes in the edited Image
            Int32 testCounter = 0;
            for (var contour = Bin.FindContours(CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, RETR_TYPE.CV_RETR_CCOMP, mem); contour != null; contour = contour.HNext)
            {
                Bin.Draw(contour, colorWhite, -1);
                testCounter++;
            }
            shadowImage = Bin.Copy();
            //shadowImage.Save("3_Threshold_Done.tiff");
            Bin.Dispose();

            //gives the edited Image to it's parent (the Detection Object)
            _parentControl._nucleiBitonalForEdgeDetection = shadowImage;
            return true;
            #endregion
        }
    }
}
