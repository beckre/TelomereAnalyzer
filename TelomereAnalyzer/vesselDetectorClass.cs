using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;


namespace TelomereAnalyzer
{
    public class vesselDetectorClass : IDisposable
    {
        Detection _parentControl = null;
        struct Settings
        {
            public Double imgBrightness;   //--------------------------------------------------- 
            public Double imgContrast;     // Global image settings for grayscale interpretation
            public Double imgGamma;        //---------------------------------------------------

            //---------------------------------------------------
            public Double minPixelEnergy;  // Define span for pixel values,
            public Double maxPixelEnergy;  // needed for cv thresholding 
                                           //---------------------------------------------------

            public Double minAreaDetectedVessel;
        }

        Settings _settingsVesselDetector;

        VesselClass[] _vessels = null;
        StochasticsClass _mathStochastics = null;
        Image<Hsv, Byte> _resultVesselImg = null;

        public vesselDetectorClass(Detection parentControl)
        {
            //parentControl is an Object of the Detection Class
            _parentControl = parentControl;
            InitializeGlobalParameter();
            //_settingsVesselDetector.imgGamma = 1.43;
            _mathStochastics = new StochasticsClass();
        }

        protected void InitializeGlobalParameter()
        {
            _settingsVesselDetector.imgBrightness = 0.0;
            _settingsVesselDetector.imgContrast = 1.0;
            _settingsVesselDetector.imgGamma = 1.43;

            _settingsVesselDetector.minPixelEnergy = 28.0;
            _settingsVesselDetector.maxPixelEnergy = 255.0;

            _settingsVesselDetector.minAreaDetectedVessel = 0.0;   // Zero means: Take all objects.
        }



        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (_vessels != null)
            {
                foreach (VesselClass vessel in _vessels)
                    vessel.Dispose();
                _vessels = null;
            }
            if (_mathStochastics != null)
                _mathStochastics.Dispose();
            _mathStochastics = null;
            if (_resultVesselImg != null)
                _resultVesselImg.Dispose();
            _resultVesselImg = null;
        }

        public bool DoThresholding(ref VesselClass[] vesselsFound)
        {
            bool success = true;
            Gray colorWhite = new Gray(255);
            MemStorage mem = new MemStorage();
            //Kopiert das normalisierte Bild in ElmiWood.shadowImage hinein
            Image<Gray, byte> shadowImage = _parentControl._grayImage.CopyBlank();

            //Image equilization...
            Console.WriteLine("Image equilization...");
            Application.DoEvents();

            #region--1. Grayscale image erzeugen
            //Bild ist zwar schon grau, wird aber hier nach bestimmten Vorgaben zum grayscale Image gemacht
            //AutoContrastBrightness(_parentControl._grayImage, ref _settingsVesselDetector.imgBrightness, ref _settingsVesselDetector.imgContrast); //kann vielleicht rausgenommen werden
            //_parentControl.labOutPut.Text = "Image equilization...done. Adapt image energy for optimized vessels display...";
            Application.DoEvents();

            //_parentControl._grayImage.Save("D:\\Hochschule Emden Leer - Bachelor Bioinformatik\\Praxisphase Bachelorarbeit Vorbereitungen\\Praktikumsstelle\\MHH Hannover Telomere\\Programm Bilder\\1_ImageEqualization.tiff");
            //_settingsVesselDetector.imgBrightness = OptimizeBrightnessForVessels(_settingsVesselDetector.imgBrightness); //macht absolut keinen unterschied
            _parentControl._grayImage.Save("D:\\Hochschule Emden Leer - Bachelor Bioinformatik\\Praxisphase Bachelorarbeit Vorbereitungen\\Praktikumsstelle\\MHH Hannover Telomere\\Programm Bilder\\2_ImageEqualization_Done.tiff");
            Console.WriteLine("Image equilization...done. Adapt image energy for optimized vessels display... done");
            //Image equilization...done. Adapt image energy for optimized vessels display... done
            #endregion

            #region--2. Binary image erzeugen

            shadowImage = _parentControl._grayImage.ConvertScale<Byte>(_settingsVesselDetector.imgContrast, _settingsVesselDetector.imgBrightness);
            shadowImage._GammaCorrect(_settingsVesselDetector.imgGamma);
            // shadowImage.Save("E:\\3.jpg");
            shadowImage.Save("D:\\Hochschule Emden Leer - Bachelor Bioinformatik\\Praxisphase Bachelorarbeit Vorbereitungen\\Praktikumsstelle\\MHH Hannover Telomere\\Programm Bilder\\3_ConvertScale.tiff");
            //_parentControl.picBox.Image = shadowImage.ToBitmap();
            /*
             * Übergangsweise wird das generierte Bild von einem Extra Fenster von ElmiWood dargestellt
             */
            //_parentControl.labOutPut.Text = "Calculate threshold...";
            //_parentControl.ImageBoxElmiTesting.BackgroundImage = shadowImage.ToBitmap();
            Console.WriteLine("Calculate threshold...");
            //_parentControl.lblElmiTesting.Text = "Calculate threshold...";
            Application.DoEvents();

            CvInvoke.cvThreshold(shadowImage, shadowImage, _settingsVesselDetector.minPixelEnergy, 255, THRESH.CV_THRESH_TOZERO);
            CvInvoke.cvThreshold(shadowImage, shadowImage, _settingsVesselDetector.maxPixelEnergy, 255, THRESH.CV_THRESH_TRUNC);

            Image<Gray, byte> Bin = shadowImage.CopyBlank();
            CvInvoke.cvThreshold(shadowImage, Bin, 0.0, 255.0, THRESH.CV_THRESH_OTSU);
            //--2.1. Fill holes
            Int32 testCounter = 0;
            for (var contour = Bin.FindContours(CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, RETR_TYPE.CV_RETR_CCOMP, mem); contour != null; contour = contour.HNext)
            {
                Bin.Draw(contour, colorWhite, -1);
                testCounter++;
            }
            shadowImage = Bin.Copy();
            //  shadowImage.Save("E:\\4.jpg");
            shadowImage.Save("D:\\Hochschule Emden Leer - Bachelor Bioinformatik\\Praxisphase Bachelorarbeit Vorbereitungen\\Praktikumsstelle\\MHH Hannover Telomere\\Programm Bilder\\4_Threshold_Done.tiff");
            Bin.Dispose();
            //Versuch
            Image<Bgr, Byte> oriColorImage = new Image<Bgr, Byte>(_parentControl._oriImage); //hier übergeben
            _parentControl._nucleiBitonalForEdgeDetection = shadowImage;
            //return hier nur, damit hier der Prozess abbricht
            return true;
        }
        #region Global operators 

        #region 1. Autocontrast
        private void AutoContrastBrightness(Image<Gray, byte> grayImage, ref Double brightness, ref Double contrast)
        {
            /*---------------------------------------------------------*\
            |* See:                                                    *|
            |*     Burger&Burger: Digitale Bildverarbeitung,           *|
            |*                     Springerverlag, 3. Auflage          *|
            \*---------------------------------------------------------*/

            Emgu.CV.DenseHistogram hist = new Emgu.CV.DenseHistogram(256, new Emgu.CV.Structure.RangeF(0.0f, 255.0f));

            // Gett the histogram for evaluation     
            hist.Calculate<Byte>(new Image<Gray, byte>[] { grayImage }, true, null);
            var mat = hist.MatND;
            float[] sum = new float[256];
            var myArr = mat.ManagedArray as float[];
            sum[0] = myArr[0];
            for (int i = 1; i < 256; ++i)
                sum[i] = sum[i - 1] + myArr[i];


            Double darknessCutoff = 0.3;      // 30% => 0.3
            Double brightnessCutoff = 0.005;  // 0.5% => 0.005

            Double percentileDarkness = sum.Select((f, Index) => new { f = f, Index = Index }).Where(fi => fi.f > sum[255] * darknessCutoff).First().Index;
            Double percentileBrightness = sum.Select((f, Index) => new { f = f, Index = Index }).Where(fi => fi.f > sum[255] * (1 - brightnessCutoff)).First().Index;

            contrast = 255.0 / (percentileBrightness - percentileDarkness);
            if (contrast == 0)
                contrast = 1;

            brightness = (255.0 - contrast * (percentileBrightness + percentileDarkness)) / 2.0;
        }
        #endregion (autocontrast)
        #endregion
        #endregion
    }
}
