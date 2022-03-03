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
        ElmiWood _parentControl = null;
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

        public vesselDetectorClass(ElmiWood parentControl)
        {
            _parentControl = parentControl;
            InitializeGlobalParameter();
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
            Image<Gray, byte> shadowImage = _parentControl._grayImage.CopyBlank();

            _parentControl.labOutPut.Text = "Image equilization...";
            Application.DoEvents();

            #region--1. Grayscale image erzeugen
            AutoContrastBrightness(_parentControl._grayImage, ref _settingsVesselDetector.imgBrightness, ref _settingsVesselDetector.imgContrast);
            _parentControl.labOutPut.Text = "Image equilization...done. Adapt image energy for optimized vessels display...";
            Application.DoEvents();

            // _parentControl._grayImage.Save("E:\\1.jpg");
            _settingsVesselDetector.imgBrightness = OptimizeBrightnessForVessels(_settingsVesselDetector.imgBrightness);
            // _parentControl._grayImage.Save("E:\\2.jpg");
            _parentControl.labOutPut.Text = "Image equilization...done. Adapt image energy for optimized vessels display... done";
            #endregion

            #region--2. Binary image erzeugen

            shadowImage = _parentControl._grayImage.ConvertScale<Byte>(_settingsVesselDetector.imgContrast, _settingsVesselDetector.imgBrightness);
            shadowImage._GammaCorrect(_settingsVesselDetector.imgGamma);
            // shadowImage.Save("E:\\3.jpg");
            _parentControl.picBox.Image = shadowImage.ToBitmap();
            _parentControl.labOutPut.Text = "Calculate threshold...";
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
            Bin.Dispose();
            _parentControl.labOutPut.Text = "Calculate threshold... done";
            _parentControl.picBox.Image = shadowImage.ToBitmap();
            Application.DoEvents();
            // MessageBox.Show("Stop");
            #endregion

            #region Operation using the structural element
            Int32 radius = 12;
            _parentControl.labOutPut.Text = "Detecting vessels...";
            Application.DoEvents();
            circularMorphologicalOperator ellipseObject = new circularMorphologicalOperator(radius);
            Image<Gray, Byte> MaskImage = shadowImage.CopyBlank();
            //--1. Opening
            StructuringElementEx structuralElement = ellipseObject.getElement();
            MaskImage = shadowImage.MorphologyEx(structuralElement, CV_MORPH_OP.CV_MOP_OPEN, 1);
            shadowImage = MaskImage.Copy();
            MaskImage.Dispose();
            #endregion

            //  shadowImage.Save("E:\\5.jpg");

            #region Dilation using Watershed
            List<VesselClass> listOfVessels = null;
            Image<Bgr, Byte> oriColorImage = new Image<Bgr, Byte>(_parentControl._oriImage);

            DoWatershedDilation(ref oriColorImage, ref listOfVessels, shadowImage);
            //  shadowImage.Save("E:\\6.jpg");
            #endregion

            //-- Save result and cleanup
            _parentControl._grayImage = shadowImage.Copy();  // We need that for contour finding             
                                                             //

            // processedBinary= oriColorImage.ToBitmap();      
            // processedBinary= shadowImage.ToBitmap();
            // processedBinary= _resultVesselImg.ToBitmap();
            shadowImage.Dispose();
            _parentControl.labOutPut.Text = "Detecting vessels...done";
            Application.DoEvents();
            if (listOfVessels != null)
            {
                if (listOfVessels.Count > 0)
                {
                    Int32 vesselsToStore = TestValidVessels(listOfVessels);
                    if (vesselsToStore <= 0)
                        return false;
                    vesselsFound = new VesselClass[vesselsToStore];
                    Int32 saveVessel = 0;
                    for (Int32 V = 0; V < listOfVessels.Count; V++)
                    {
                        if (VesselDefinitionValid(listOfVessels[V]) == true)
                            vesselsFound[saveVessel++] = listOfVessels[V];
                    }
                }
                else
                    success = false;
            }
            else
                success = false;

            _parentControl.labOutPut.Text = "Sorting vessels...";
            Application.DoEvents();
            if (success == true)
            {
                Array.Sort(vesselsFound, (x, y) => x._gravCenter.X.CompareTo(y._gravCenter.X));   //  Sort from left to right
                _parentControl.labOutPut.Text = "Sorting vessels...done";
                Application.DoEvents();
            }
            return success;
        }

        protected bool VesselDefinitionValid(VesselClass vessel)
        {
            if (vessel._gravCenter.X < 0)
                return false;
            if (vessel._gravCenter.Y < 0)
                return false;
            return true;
        }
        protected Int32 TestValidVessels(List<VesselClass> listOfVessels)
        {
            Int32 Cnt = 0;

            if (listOfVessels == null)
                return 0;
            if (listOfVessels.Count == 0)
                return 0;
            for (Int32 V = 0; V < listOfVessels.Count; V++)
            {
                if (VesselDefinitionValid(listOfVessels[V]) == false)
                    continue;
                Cnt++;
            }
            return Cnt;
        }

        #region Contours
        public void IdentifyContours(int thresholdValue, bool invert, out Bitmap processedGray, out Bitmap processedColor)
        {
            // Extract  contours from threshold.
            Image<Bgr, byte> color = new Image<Bgr, byte>(_parentControl._oriImage);
            Image<Bgr, byte> shadowImage = _parentControl._grayImage.Convert<Bgr, byte>();

            #region Extracting the Contours
            MemStorage storage = new MemStorage();

            for (Contour<Point> contours = _parentControl._grayImage.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_TREE, storage); contours != null; contours = contours.HNext)
            {
                Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.015, storage);
                if (currentContour.BoundingRectangle.Width > 20)
                {
                    CvInvoke.cvDrawContours(color, contours, new MCvScalar(255), new MCvScalar(255), -1, 2, Emgu.CV.CvEnum.LINE_TYPE.EIGHT_CONNECTED, new Point(0, 0));
                    color.Draw(currentContour.BoundingRectangle, new Bgr(0, 255, 0), 1);
                    CvInvoke.cvDrawContours(shadowImage, contours, new MCvScalar(255), new MCvScalar(255), -1, 2, Emgu.CV.CvEnum.LINE_TYPE.EIGHT_CONNECTED, new Point(0, 0));
                    if (currentContour.Area > _settingsVesselDetector.minAreaDetectedVessel)  // minimum Area
                        AddVesselObject(currentContour);
                }
            }

            if (_vessels != null)
                DoFilterVessels();

            #endregion

            //4. Setting the results to the output variables.

            #region Asigning output
            processedColor = color.ToBitmap();
            processedGray = shadowImage.ToBitmap();
            #endregion
        }

        protected void DoFilterVessels()
        {
            bool eliminateVessel = false;
            Int32 V = 0;
            do
            {
                eliminateVessel = false;
                if (_vessels[V]._vesselLikeObject == false)
                    EliminateVesselObject(V);
                V++;
                if (V >= _vessels.Length)
                    return;
            } while (eliminateVessel == true);
        }

        #endregion (contours)


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

        #region Watershed dilation
        protected void DoWatershedDilation(ref Image<Bgr, Byte> oriColorImage, ref List<VesselClass> listOfVessels, Image<Gray, Byte> shadowImage)
        {
            listOfVessels = null;
            List<VesselClass> regions = GetVesselList(shadowImage);
            Image<Gray, Byte> Img = shadowImage.CopyBlank();
            _resultVesselImg = new Image<Hsv, Byte>(_parentControl._oriImage);
            _resultVesselImg[1] = _resultVesselImg[0].CopyBlank();

            listOfVessels = new List<VesselClass>();

            Image<Gray, Byte> imageNotAcceptedVessels = GenerateNotAcceptedVesselImage(shadowImage, regions);

            Image<Gray, Byte> DilateImage = GetDilatedImage(imageNotAcceptedVessels, 1, 19, CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
            Image<Gray, Byte> erodedImage = null;
            List<VesselClass> additionallyDetectedVessels = null;

            for (int i = 1; i < 6; i++)
            {
                if (erodedImage != null)
                    erodedImage.Dispose();

                erodedImage = GetErodedImage(imageNotAcceptedVessels, i, 11, CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);

                additionallyDetectedVessels = WatershededContours(oriColorImage, erodedImage, DilateImage);
                if (additionallyDetectedVessels != null)
                {
                    if (additionallyDetectedVessels.Count > 0)
                        regions.AddRange(additionallyDetectedVessels);
                }

                foreach (VesselClass vesselDetected in additionallyDetectedVessels)
                {
                    if (vesselDetected._vesselLikeObject != true)
                        imageNotAcceptedVessels.Draw(vesselDetected._contour, new Gray(0), -1);
                }
            }

            int c = 0;
            if (regions != null)
            {
                foreach (VesselClass ves in regions)
                    if (ves._vesselLikeObject == true)
                    {
                        if (c >= 255)
                            c = 0;
                        _resultVesselImg.Draw(ves._contour, new Hsv(180, 255, 255), -1);
                        listOfVessels.Add(ves);
                    }
            }

            oriColorImage = _resultVesselImg.Convert<Bgr, Byte>();
        }
        private Image<Gray, Byte> GenerateNotAcceptedVesselImage(Image<Gray, Byte> shadowImage, List<VesselClass> liste)
        {
            Image<Gray, Byte> Img = shadowImage.CopyBlank();
            if (liste == null)
                return Img;
            foreach (VesselClass vesselObject in liste)
                if (vesselObject._vesselLikeObject == false)
                    Img.Draw(vesselObject._contour, new Gray(255), -1);

            return Img;
        }
        private List<VesselClass> GetVesselList(Emgu.CV.Image<Gray, Byte> bin)
        {
            if (bin == null)
                return null;
            List<VesselClass> detectedVessels = new List<VesselClass>();
            VesselClass investigatedVessel = null;
            int iIdentifier = 0;

            using (var contourimg = bin.Copy())
                for (var cont = contourimg.FindContours(CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_NONE, RETR_TYPE.CV_RETR_CCOMP); cont != null; cont = cont.HNext)
                {
                    if (cont.Area > 0)//Set.minVesselPixelArea())  // Min Vessel Area!
                    {
                        investigatedVessel = new VesselClass(ref _mathStochastics, cont);
                        investigatedVessel._contour = cont;
                        investigatedVessel._iD = iIdentifier;
                        iIdentifier++;
                        detectedVessels.Add(investigatedVessel);
                    }
                }
            return detectedVessels;
        }
        #endregion

        #endregion (global operators)

        #region Tools
        protected void AddVesselObject(Contour<Point> currentContour)
        {
            VesselClass[] tmp = null;
            Int32 currentObjects = 0;
            if (_vessels != null)
                currentObjects = _vessels.Length;

            tmp = new VesselClass[currentObjects + 1];
            for (Int32 V = 0; V < currentObjects; V++)
                tmp[V] = _vessels[V];

            tmp[currentObjects] = new VesselClass(ref _mathStochastics, currentContour);
            tmp[currentObjects]._area = currentContour.Area;
            _vessels = null;
            _vessels = tmp;
        }
        protected void EliminateVesselObject(Int32 vesselToDelete)
        {
            VesselClass[] tmp = null;
            Int32 currentObjects = 0;
            if (_vessels != null)
                currentObjects = _vessels.Length;

            if (currentObjects == 0)  // last object to be deleted
            {
                _vessels[0].Dispose();
                _vessels = null;
                return;
            }

            tmp = new VesselClass[currentObjects - 1];

            // copy all until the one to be deleted
            Int32 V = 0;
            Int32 N = 0;
            for (; V < vesselToDelete; V++)
                tmp[V] = _vessels[V];

            N = V + 1;

            for (; V < vesselToDelete; V++, N++)
                tmp[V] = _vessels[N];

            _vessels = null;
            _vessels = tmp;
        }
        public Image<Gray, Byte> GetDilatedImage(Image<Gray, Byte> sourceImage, int iterations, int structureSize, object shapingElement)
        {
            if (sourceImage == null)
                return null;
            if (structureSize % 2 != 1)
                return null;

            Image<Gray, Byte> dilatedImage = sourceImage.CopyBlank();

            int Anchor = (structureSize - 1) / 2;

            StructuringElementEx element = new StructuringElementEx((Int32)(structureSize), (Int32)(structureSize), Anchor, Anchor, (CV_ELEMENT_SHAPE)shapingElement);

            CvInvoke.cvDilate(sourceImage, dilatedImage, element, iterations);

            return dilatedImage;
        }
        public Image<Gray, Byte> GetErodedImage(Image<Gray, Byte> sourceImage, int iterations, int structureSize, object shapingElement)
        {
            if (sourceImage == null)
                return null;
            if (structureSize % 2 != 1)
                return null;

            Image<Gray, Byte> erodedImage = sourceImage.CopyBlank();

            int Anchor = (structureSize - 1) / 2;

            StructuringElementEx element = new StructuringElementEx((Int32)(structureSize), (Int32)(structureSize), Anchor, Anchor, (CV_ELEMENT_SHAPE)shapingElement);

            CvInvoke.cvErode(sourceImage, erodedImage, element, iterations);

            return erodedImage;
        }
        private List<VesselClass> WatershededContours(Image<Bgr, Byte> oriColorImage, Image<Gray, Byte> sourceImage, Image<Gray, Byte> DilSource)
        {
            List<VesselClass> additionallyFoundVessels = new List<VesselClass>();
            int Counter = 0; // _GefäßListe.Count;
            Image<Gray, int> watershedImage = GetMarkerImage(sourceImage, DilSource);

            CvInvoke.cvWatershed(oriColorImage.Convert<Bgr, Byte>(), watershedImage);

            for (var contour = watershedImage.FindContours(CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_NONE, RETR_TYPE.CV_RETR_CCOMP); contour != null; contour = contour.HNext)
            {
                if (!(contour.BoundingRectangle.X == 1) && !(contour.BoundingRectangle.Y == 1) && !(contour.BoundingRectangle.Bottom == watershedImage.Height - 1) && !(contour.BoundingRectangle.Left == watershedImage.Width - 1))
                {
                    VesselClass mehrGefäße = new VesselClass(ref _mathStochastics, contour);
                    mehrGefäße._iD = Counter++;
                    additionallyFoundVessels.Add(mehrGefäße);
                }
            }
            watershedImage.Dispose();
            watershedImage = null;
            return additionallyFoundVessels;
        }
        private Image<Gray, int> GetMarkerImage(Image<Gray, Byte> erodedImage, Image<Gray, Byte> dilatedImage)
        {
            /*----------------------------------------------------------*\
            |*  Setup routine for watershed: Generate marker image for  *|
            |*  seeds.                                                  *|
            \*----------------------------------------------------------*/
            Image<Gray, int> markerImage = new Image<Gray, int>(erodedImage.Width, erodedImage.Height, new Gray(1));
            markerImage -= dilatedImage.Convert<Gray, int>();

            int grau = 2;
            for (var contour = erodedImage.FindContours(CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, RETR_TYPE.CV_RETR_CCOMP); contour != null; contour = contour.HNext)
                markerImage.Draw(contour, new Gray(grau++), -1);

            return markerImage;
        }
        protected Double OptimizeBrightnessForVessels(Double suggestedBrightness)
        {
            // Postprocessing for automated brightness: Find optimum of brightness within histogram
            Double corrBright = suggestedBrightness;
            Int32 minCount = Int32.MaxValue;
            Int32 testCounter = 0;
            Double testBrightness;
            Image<Gray, byte> shadowImage = null;
            Image<Gray, Byte> Bin = null;
            Gray colorWhite = new Gray(255);
            MemStorage mem = new MemStorage();

            //-- Starting from rough to fine
            Double windowWidth = 2 * 35.0;    // -35  -> +35
            Double testStep = 5.0;
            Double divisorIt = 1.0;
            Int32 iterations = 3;
            Int32 range = 8;
            Int32 countDown = iterations * range;

            for (Int32 I = 0; I < iterations; I++)
            {
                testStep = ((windowWidth / (Double)(range)) / divisorIt);
                testBrightness = suggestedBrightness + ((range * testStep) / 2.0);      // Symmetrically to the left and right of the brightness of interest
                divisorIt += 2.0; // half, quarter, etc.        
                for (Int32 A = 0; A < range; A++)
                {
                    countDown--;
                    _parentControl.labOutPut.Text = "Image equilization...done. Adapt image energy for optimized vessels display...(" + countDown.ToString() + ")";
                    Application.DoEvents();
                    shadowImage = _parentControl._grayImage.ConvertScale<Byte>(_settingsVesselDetector.imgContrast, testBrightness);
                    shadowImage._GammaCorrect(_settingsVesselDetector.imgGamma);
                    CvInvoke.cvThreshold(shadowImage, shadowImage, _settingsVesselDetector.minPixelEnergy, 255, THRESH.CV_THRESH_TOZERO);
                    CvInvoke.cvThreshold(shadowImage, shadowImage, _settingsVesselDetector.maxPixelEnergy, 255, THRESH.CV_THRESH_TRUNC);
                    Bin = shadowImage.CopyBlank();
                    CvInvoke.cvThreshold(shadowImage, Bin, 0.0, 255.0, THRESH.CV_THRESH_OTSU);
                    //--2.1. Fill holes
                    // Rebecca hier wird's heiß!!! :-)
                    testCounter = 0;
                    for (var contour = Bin.FindContours(CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, RETR_TYPE.CV_RETR_CCOMP, mem); contour != null; contour = contour.HNext)
                    {
                        Bin.Draw(contour, colorWhite, -1);
                        testCounter++;
                    }

                    if (testCounter < minCount)
                    {
                        suggestedBrightness = testBrightness;
                        minCount = testCounter;
                    }
                    shadowImage.Dispose();
                    Bin.Dispose();
                    testBrightness -= testStep;
                }
                corrBright = suggestedBrightness;
            }
            return corrBright;
        }

        #endregion (tools)
    }
}
