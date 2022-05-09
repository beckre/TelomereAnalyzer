using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using ImageMagick;

namespace TelomereAnalyzer
{
    public partial class FormOne : Form
    {
        ElmiWood _elmiWood;
        EdgeDetection _EdgeDetection;
        Nuclei _allNuclei;
        AllTelomeres _allTelomeres;

        Boolean _nucleiImageUploaded = false;
        Boolean _telomereImageUploaded = false;

        String _nucleiFilePathName;
        String _telomereFilePathName;
        public String _nucleiFileName;
        public String _telomereFileName;

        /*
         * All Images are stored individually because the User should have
         * the Option to store the Images at different stages at the End.
         */
        //Image and Bitmap of the original loaded Nuclei Image
        Image<Gray, byte> _uploadedRawNucleiImage = null;
        Bitmap _btmUploadedRawNucleiImage = null;

        //Image and Bitmap of the original loaded Telomere Image
        Image<Gray, byte> _uploadedRawTelomereImage = null;
        Bitmap _btmUploadedRawTelomereImage = null;

        //Image and Bitmap of the auto-leveled Nuclei Image
        public Image<Gray, byte> _NucleiImageAutoLevel = null;
        public Bitmap _btmNucleiImageAutoLevel = null;

        //Image and Bitmap of the auto-leveled Telomere Image
        public Image<Gray, byte> _TelomereImageAutoLevel = null;
        public Bitmap _btmTelomereImageAutoLevel = null;

        //Bitmap of the auto-leveld Telomere Image with applied Threshold
        public Bitmap _btmTelomereImageThreshold = null;

        //Bitmap of the auto-leveled merged Nuclei and Telomere Image with applied Threshold
        public Bitmap _btmTelomereImageHalfTransparent = null;

        // Image and Bitmap of the Nuclei Image which is to be altered for the Edge Detection % by Elmi Wood %
        Image<Gray, byte> _nucleiBitonalForEdgeDetection = null;

        //Image and Bitmap of the Nuclei Image and the automatically detected Nuclei Borders % by Elmi Wood %
        public Image<Bgr, byte> _NucleiImageEdgesDetected = null;
        Bitmap _btmNucleiImageEdgesDetected = null;

        //Image for Testing the Nuclei Edge Detection
        public Image<Bgr, byte> _TestingNucleiImageEdgesDetected = null;

        //Image of the automatically detected Nuclei % by Elmi Wood % + Nuclei drawn by User
        public Image<Bgr, byte> _NucleiImageEdgesDetectedAndDrawn = null;

        //Bitmap of Nuclei Image with automatically detected and drawn Nuclei merged with Telomere Image with applied Threshold
        public Bitmap _btmNucleiImageMergedWithTresholdImage;

        //Image of the detected Telomeres Borders
        public Image<Bgr, byte> _TelomereImageTelomeresDetected = null;

        //Image for Testing the Telomere Edge Detection
        public Image<Bgr, byte> _TestingTelomereImageTelomeresDetected = null;

        //Image for Testing the Allocation of the Telomeres to their belonging Nucleus
        public Image<Bgr, byte> _TestingAllocatingTelomeresToNucleus = null;


        public FormOne()
        {
            InitializeComponent();
            var dllDirectory = @"./OpenCv";
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable
                                               ("PATH") + ";" + dllDirectory);
            grpBoxSelectOptions.Hide();
        }
        #region Callbacks--------------------------------------------------------------------------------------------
        /*----------------------------------------------------------------------------------------*\
         *  is called when clicking on the upload --> .tiff button.                               *|
         *  
        \*----------------------------------------------------------------------------------------*/
        private void OnUploadNucleiImage(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new
            System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "Image Files|*.tif;*.tiff;*.jpg;*.jpeg;*.png;*";
            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                _nucleiFilePathName = dialog.FileName;
                //String[] tempFileNameArr = dialog.SafeFileName.Split('.');
                //_nucleiFileName = tempFileNameArr[0];
                _nucleiFileName = dialog.SafeFileName;
                _uploadedRawNucleiImage = new Image<Gray, byte>(dialog.FileName);
                _btmUploadedRawNucleiImage = _uploadedRawNucleiImage.ToBitmap();
                ShowBitmapOnForm(ImageBoxOne, _btmUploadedRawNucleiImage);
                _nucleiImageUploaded = true;
                if (_nucleiImageUploaded && _telomereImageUploaded)
                {
                    lblPleaseSelectPic.Text = "Please click on Start to launch the Analysis";
                    grpBoxSelectOptions.Show();
                }
                if (!_telomereImageUploaded)
                    lblPleaseSelectPic.Text = "Please upload a Telomere .TIFF file";
            }
        }

        private void OnUploadTelomereImage(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new
            System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "Image Files|*.tif;*.tiff;*.jpg;*.jpeg;*.png;*";
            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                _telomereFilePathName = dialog.FileName;
                //String[] tempFileNameArr = dialog.SafeFileName.Split('.');
                //_telomereFileName = tempFileNameArr[0];
                _telomereFileName = dialog.SafeFileName;
                _uploadedRawTelomereImage = new Image<Gray, byte>(dialog.FileName);
                _btmUploadedRawTelomereImage = _uploadedRawTelomereImage.ToBitmap();
                ShowBitmapOnForm(ImageBoxTwo, _btmUploadedRawTelomereImage);
                _telomereImageUploaded = true;
                if (_nucleiImageUploaded && _telomereImageUploaded)
                {
                    lblPleaseSelectPic.Text = "Please click on Start to launch the Analysis";
                    grpBoxSelectOptions.Show();
                }
                if (!_nucleiImageUploaded)
                    lblPleaseSelectPic.Text = "Please upload a Nuclei .TIFF file";
            }
        }

        private void OnStart(object sender, EventArgs e)
        {
            if (IsImageOkay(_uploadedRawNucleiImage) && IsImageOkay(_uploadedRawTelomereImage))
                AutoLevel();
        }

        /*
         * Auto-Levels the nuclei and telomer Image at once
         */
        private void AutoLevel()
        {
            //lblPleaseSelectPic.Text = "Please click on Threshold to automatically generate a Threshold for the Telomere Image";
            MagickImage magickImageNuclei = new MagickImage(_nucleiFilePathName);
            magickImageNuclei.AutoLevel();
            using (var memStream = new MemoryStream())
            {
                magickImageNuclei.Write(memStream);
                Bitmap btm = new System.Drawing.Bitmap(memStream);
                _NucleiImageAutoLevel = new Image<Gray, byte>(btm);
            }
            _btmNucleiImageAutoLevel = _NucleiImageAutoLevel.ToBitmap();
            ImageBoxOne.BackgroundImage = _btmNucleiImageAutoLevel;
            MagickImage magickImageTelomere = new MagickImage(_telomereFilePathName);
            magickImageTelomere.AutoLevel();
            using (var memStream = new MemoryStream())
            {
                magickImageTelomere.Write(memStream);
                Bitmap btm = new System.Drawing.Bitmap(memStream);
                _TelomereImageAutoLevel = new Image<Gray, byte>(btm);
            }
            _btmTelomereImageAutoLevel = _TelomereImageAutoLevel.ToBitmap();
            ImageBoxTwo.BackgroundImage = _btmNucleiImageAutoLevel;
            Threshold();
        }
        /*----------------------------------------------------------------------------------------*\
         *  Generates the Threshold of the uploaded image using the otsu's method.                *|
         *  Converts the choosen image to grayscale and byte before thresholding                  *|
         *  otherwise an exception is thrown.
        \*----------------------------------------------------------------------------------------*/
        private void Threshold()
        {
            if (IsImageOkay(_TelomereImageAutoLevel))
            {
                Thresholding();
                _btmTelomereImageHalfTransparent = MergeImages(_btmNucleiImageAutoLevel, _btmTelomereImageThreshold);
                DetectNuclei();
            }
            else
            {
                _TelomereImageAutoLevel = _uploadedRawTelomereImage;
                _btmTelomereImageAutoLevel = _uploadedRawTelomereImage.ToBitmap();
                _NucleiImageAutoLevel = _uploadedRawNucleiImage;
                _btmNucleiImageAutoLevel = _uploadedRawNucleiImage.ToBitmap();
                Thresholding();
                _btmTelomereImageHalfTransparent = MergeImages(_btmNucleiImageAutoLevel, _btmTelomereImageThreshold);
                DetectNuclei();
            }
        }

        private Bitmap MergeImages(Bitmap primaryImage, Bitmap secondaryImage)
        {
            var finalImage = new Bitmap(secondaryImage.Width, secondaryImage.Height);
            var graphics = Graphics.FromImage(finalImage);
            graphics.CompositingMode = CompositingMode.SourceOver;
            double alphaTransparent = 0.0;
            var transparentImage = new Bitmap(secondaryImage.Width, secondaryImage.Height);
            for (int x = 0; x < secondaryImage.Width; ++x)
            {
                for (int y = 0; y < secondaryImage.Height; ++y)
                {
                    Color pixel = _btmTelomereImageThreshold.GetPixel(x, y);
                    alphaTransparent = pixel.A / 2.0;
                    transparentImage.SetPixel(x, y, Color.FromArgb(Convert.ToInt32(alphaTransparent), pixel.R, pixel.G, pixel.B));
                }
            }
            graphics.DrawImage(primaryImage, 0, 0);
            graphics.DrawImage(transparentImage, 0, 0);
            ShowBitmapOnForm(ImageBoxTwo, finalImage);
            return finalImage;
        }

        private void DetectNuclei()
        {
            _elmiWood = new ElmiWood(this);
            _elmiWood.DoAnalyze(_NucleiImageAutoLevel);
            _nucleiBitonalForEdgeDetection = _elmiWood._nucleiBitonalForEdgeDetection;
            if (IsImageOkay(_nucleiBitonalForEdgeDetection))
            {
                _EdgeDetection = new EdgeDetection(this);
                _EdgeDetection.FindingContoursNuclei(_nucleiBitonalForEdgeDetection, _NucleiImageAutoLevel);
            }
            _btmNucleiImageEdgesDetected = _NucleiImageEdgesDetected.ToBitmap();
            ShowBitmapOnForm(ImageBoxOne, _btmNucleiImageEdgesDetected);
            //_btmNucleiImageEdgesDetected.Save("D:\\Hochschule Emden Leer - Bachelor Bioinformatik\\Praxisphase Bachelorarbeit Vorbereitungen\\Praktikumsstelle\\MHH Hannover Telomere\\Programm Bilder\\5_NucleiDetected.tiff");

            /* Nun wurden die Nuclei automatisch umrandet.
             * Der User soll daraufhin die Nuclei selber einzeichnen können. 
             * Hierfür soll sich eine neue Form öffnen, in der dies komplett gehandelt wird.
             * Übergeben wird die Nuclei-Liste und diese kann modifiziert werden.
             */
            _allNuclei = _EdgeDetection._allNuclei;
            FormTwo formTwo = new FormTwo(_allNuclei, _NucleiImageAutoLevel.Convert<Bgr, byte>());
            formTwo.ShowDialog(); //.ShowDialog anstatt .Show, da es so nicht möglich ist auf das 1. Fenster zuzugreifen, bis das 2. Fenster geschlossen wurde

            _NucleiImageEdgesDetectedAndDrawn = formTwo._NucleiImageWithAutomaticEdgesToDrawOn;
            _btmNucleiImageMergedWithTresholdImage = MergeImages(_NucleiImageEdgesDetectedAndDrawn.ToBitmap(), _TelomereImageAutoLevel.ToBitmap());
            Image<Gray, byte> telomereImageToDrawOn = new Image<Gray, byte>(_btmTelomereImageThreshold);
            DetectingTelomeres(telomereImageToDrawOn);
        }

        #endregion

        #region Thresholding---------------------------------------------------------------------------------
        private void Thresholding()
        {
            Image<Gray, byte> image = _TelomereImageAutoLevel.Convert<Gray, byte>();
            Image<Gray, byte> destImage = new Image<Gray, byte>(image.Width, image.Height, new Gray(0));
            try
            {
                double threshold = CvInvoke.cvThreshold(image.Convert<Gray, byte>(), destImage, 0.0, 255.0, THRESH.CV_THRESH_OTSU);
                lblThreshold.Text = "Calculated Threshold: " + threshold;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            _btmTelomereImageThreshold = ChangingColourOfBitonalImage(destImage);
            ShowBitmapOnForm(ImageBoxTwo, _btmTelomereImageThreshold);
            lblPleaseSelectPic.Text = "Please click on Merge Images to overlay both of the Images on top of each other";
        }

        private Bitmap ChangingColourOfBitonalImage(Image<Gray, byte> image)
        {
            Bitmap destImageBitmap = image.ToBitmap();
            //Ziel Bitmap muss diese Art von Bitmap sein, weil sonst die Methode .SetPixel() nicht aufrufbar ist
            Bitmap resultBmpToBeColoured = new Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int width = destImageBitmap.Width;
            int height = destImageBitmap.Height;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color checkedColor = destImageBitmap.GetPixel(i, j);
                    /*
                     * The Image is black and white (bitonal). If there is a red component in a Pixel than this Pixel is white.
                     * The white Pixels are converted to yellow and the rest is set to black.
                     */
                    if (checkedColor.R == 255)
                        resultBmpToBeColoured.SetPixel(i, j, Color.FromArgb(255, 255, 255, 0));
                    else
                        resultBmpToBeColoured.SetPixel(i, j, Color.FromArgb(255, 0, 0, 0));
                }
            }
            return resultBmpToBeColoured;
            
        }
        #endregion
        public void DetectingTelomeres(Image<Gray, byte> telomereImage)
        {
            _EdgeDetection.FindingContoursTelomeres(telomereImage, telomereImage);
            ShowBitmapOnForm(ImageBoxOne, _TelomereImageTelomeresDetected.ToBitmap());
            //Testing the Telomere and AllTelomere Classes
            //ShowBitmapOnForm(ImageBoxTwo, _TestingTelomereImageTelomeresDetected.ToBitmap());
            AllocateTelomeresToNucleus();
        }

        public void AllocateTelomeresToNucleus()
        {
            _allTelomeres = _EdgeDetection._allTelomeres;
            for (Int32 n = 0; n < _allNuclei._LstAllNuclei.Count; n++)
            {
                _allNuclei._LstAllNuclei[n].getAmountOfPixelsInNucleusArea(_uploadedRawNucleiImage);
                //Goes through every existing telomere per Nucleus --> checks if any contour-Point of the Telomere is in the Nucleus-Area
                for (Int32 t = 0; t < _allTelomeres._LstAllTelomeres.Count; t++)
                {
                    if (_allNuclei._LstAllNuclei[n].isTelomereContourInThisNucleus(_allTelomeres._LstAllTelomeres[t]._telomereContourPoints))
                        _allNuclei._LstAllNuclei[n].AddTelomereToTelomereList(_allTelomeres._LstAllTelomeres[t]);
                }
            }
            //Alles zum Testen
            /*
            _TestingAllocatingTelomeresToNucleus = new Image<Bgr, byte>(_btmTelomereImageThreshold);
            //malt Kontur von 1 Nuclei
            PointF[] contour = _allNuclei._LstAllNuclei[0]._nucleusContourPoints;
            Bgr color = new Bgr(Color.HotPink);
            Point[] nucleiContourTesting = new Point[contour.Length];
            for (Int32 p = 0; p < contour.Length; p++)
            {
                nucleiContourTesting[p] = Point.Round(contour[p]);
            }

            _TestingAllocatingTelomeresToNucleus.DrawPolyline(nucleiContourTesting, true, color, 1);
            //List<Telomere> telomeres = _allNuclei._allNuclei[0]._nucleusTelomeres;
            //malt Konturen zu zugehörigen Telomeren
            for (Int32 i = 0; i < _allNuclei._LstAllNuclei[0]._LstNucleusTelomeres.Count; i++)
            {
                PointF[] contourTelomeres = _allNuclei._LstAllNuclei[0]._LstNucleusTelomeres[i]._telomereContourPoints;
                Point[] contourTesting = new Point[contourTelomeres.Length];
                Bgr colorTelomeres = new Bgr(Color.Red);
                for(Int32 p = 0; p < contourTelomeres.Length; p++)
                {
                    contourTesting[p] = Point.Round(contourTelomeres[p]);
                }
                _TestingAllocatingTelomeresToNucleus.DrawPolyline(contourTesting, true, colorTelomeres, 1);
            }
            ShowBitmapOnForm(ImageBoxOne, _TestingAllocatingTelomeresToNucleus.ToBitmap());
            _TestingAllocatingTelomeresToNucleus.Save("D:\\Hochschule Emden Leer - Bachelor Bioinformatik\\Praxisphase Bachelorarbeit Vorbereitungen\\Praktikumsstelle\\MHH Hannover Telomere\\Programm Bilder\\6_TestingTelomeresDetected.tiff");
            // Ende Testen
            */
            DisplayEndResults();
        }

        public void DisplayEndResults()
        {
            FormThree formThree = new FormThree(this, _allNuclei, _allTelomeres);
            formThree.Show();
        }

        public void ShowBitmapOnForm(ImageBox imageBox, Bitmap bitmap)
        {
            if (imageBox.BackgroundImage == null)
            {
                imageBox.BackgroundImage = bitmap;
                imageBox.Width = bitmap.Width;
                imageBox.Height = bitmap.Height;
                imageBox.MaximumSize = bitmap.Size;
                imageBox.Refresh();
            }
            else
                imageBox.BackgroundImage = bitmap;
        }

        // checks if image is null --> needs to be extended probably
        private Boolean IsImageOkay(Image<Gray, byte> image)
        {
            if (image == null)
                return false;
            return true;
        }
    }
}

