using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace TelomereAnalyzer
{
    public partial class FormOne : Form
    {
        ElmiWood _elmiWood = null;


        //Image<Bgr, byte> Main8BitImage = null;       // Das Hauptbild

        Boolean _nucleiImageUploaded = false;
        Boolean _telomereImageUploaded = false;

        EdgeDetection _EdgeDetection = null;
        DisplayingFinalResults _DisplayingFinalResults = null;

        Nuclei _allNuclei = null;
        AllTelomeres _allTelomeres = null;

        /*
         * Die Bilder werden alle seperat gespeichert, da es die Option geben soll
         * die unterschiedlichen Stadien der Bilder später zu speichern.
         */
        //Image und Bitmap vom originalen hochgeladenen Nuclei Bild in 16 Bit
        Image<Gray, UInt16> _uploadedRawNucleiImage16Bit = null;
        Bitmap _btmUploadedRawNucleiImage16Bit = null;
        //Image und Bitmap vom originalen hochgeladenen Telomere Bild in 16 Bit
        public Image<Gray, UInt16> _uploadedRawTelomereImage16Bit = null;
        Bitmap _btmUploadedRawTelomereImage16Bit = null;

        //Image und Bitmap vom originalen hochgeladenen Nuclei Bild
        Image<Gray, byte> _uploadedRawNucleiImage8Bit = null;
        Bitmap _btmUploadedRawNucleiImage8Bit = null;

        //Image und Bitmap vom originalen hochgeladenen Telomer Bild
        Image<Gray, byte> _uploadedRawTelomereImage8Bit = null;
        Bitmap _btmUploadedRawTelomereImage8Bit = null;

        //Image und Bitmap vom normalisierten Nuclei Bild
        public Image<Gray, byte> _NucleiImageNormalized = null;
        Bitmap _btmNucleiImageNormalized = null;

        //Image und Bitmap vom normalisierten Telomer Bild
        Image<Gray, byte> _TelomereImageNormalized = null;
        Bitmap _btmTelomereImageNormalized = null;

        //Bitmap vom normalisierten Bild, wo die Threshold Methode angewandt wurde
        public Bitmap _btmTelomereImageThreshold = null;

        //Bitmap vom normalisierten Bild, wo die Threshold Methode angewandt wurde und die Transparenz auf die Hälfte gesetzt wurde
        public Bitmap _btmTelomereImageHalfTransparent = null;

        // Image und Bitmap vom Nuclei Bild, welches von Elmi Wood bearbeitet und hieran übergeben wurde
        Image<Gray, byte> _nucleiBitonalForEdgeDetection = null;

        //Image und Bitmap vom Nuclei Bild mit Anwendung von Edge Detection
        public Image<Bgr, byte> _NucleiImageEdgesDetected = null;

        //Nochmal zum Testen der Nucleus und Nuclei Klasse
        public Image<Bgr, byte> _TestingNucleiImageEdgesDetected = null;
        Bitmap _btmNucleiImageEdgesDetected = null;

        //Image vom Nuclei Bild mit Anwendung von Edge Detection + selbst gemalten Nuclei
        public Image<Bgr, byte> _NucleiImageEdgesDetectedAndDrawn = null;

        //Bitmap vom Nuclei Bild mit erkannten und gemalten Nuclei + Threshold Telomere Bild mit Transparenz übereinander
        public Bitmap _btmNucleiImageMergedWithTresholdImage;

        //Image von entdeckten Nuclei Bild mit Anwendung von Edge Detection
        public Image<Bgr, byte> _TelomereImageTelomeresDetected = null;
        //Nochmal zum Testen der Telomere und AllTelomeres Klasse
        public Image<Bgr, byte> _TestingTelomereImageTelomeresDetected = null;

        //zum testen, ob telomere dem zugehörigem Nucleus zugeórdnet wurden
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
         *  Enables the user to choose and load any file for now --> only .tiff should be         *|
         *  acceptible.
        \*----------------------------------------------------------------------------------------*/

        private void OnUploadNucleiImage(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new
            System.Windows.Forms.OpenFileDialog();
            //dialog.Filter = "Image Files|*.tif;*.tiff";
            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                //erstmal mit der 16 Bit Version des Bildes ohne es in 8 Bit zu konvertieren
                _uploadedRawNucleiImage16Bit = new Image<Gray, UInt16>(dialog.FileName);
                _uploadedRawNucleiImage8Bit = new Image<Gray, byte>(dialog.FileName);
                //Main8BitImage = MainImage.Convert<Bgr, byte>();
                //Bitmap tiffImageConvertedTo8Bit = Main8BitImage.ToBitmap();
                //ImageBox.BackgroundImage = tiffImageConvertedTo8Bit;
                _btmUploadedRawNucleiImage16Bit = _uploadedRawNucleiImage16Bit.ToBitmap();
                _btmUploadedRawNucleiImage8Bit = _uploadedRawNucleiImage8Bit.ToBitmap();
                ShowBitmapOnForm(ImageBoxOne, _btmUploadedRawNucleiImage8Bit);
                //btnGenerateThreshold.Hide();
                //btnMergeImages.Hide();
                //btnDetectingNuclei.Hide();
                _nucleiImageUploaded = true;
                if (_nucleiImageUploaded && _telomereImageUploaded)
                {
                    lblPleaseSelectPic.Text = "Please click on Normalize to normalize both images";
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
            dialog.Filter = "Image Files|*.tif;*.tiff";
            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                //erstmal mit der 16 Bit Version des Bildes ohne es in 8 Bit zu konvertieren
                _uploadedRawTelomereImage16Bit = new Image<Gray, UInt16>(dialog.FileName);
                _uploadedRawTelomereImage8Bit = new Image<Gray, byte>(dialog.FileName);
                //Main8BitImage = MainImage.Convert<Bgr, byte>();
                //Bitmap tiffImageConvertedTo8Bit = Main8BitImage.ToBitmap();
                //ImageBox.BackgroundImage = tiffImageConvertedTo8Bit;
                _btmUploadedRawTelomereImage16Bit = _uploadedRawTelomereImage16Bit.ToBitmap();
                _btmUploadedRawTelomereImage8Bit = _uploadedRawTelomereImage8Bit.ToBitmap();
                ShowBitmapOnForm(ImageBoxTwo, _btmUploadedRawTelomereImage8Bit);
                //btnGenerateThreshold.Hide();
                _telomereImageUploaded = true;
                if (_nucleiImageUploaded && _telomereImageUploaded)
                {
                    lblPleaseSelectPic.Text = "Please click on Normalize to normalize both images";
                    grpBoxSelectOptions.Show();
                }
                if (!_nucleiImageUploaded)
                    lblPleaseSelectPic.Text = "Please upload a Nuclei .TIFF file";
            }
        }

        /*
         * Normalizes the nuclei and telomer Image at once
         */
        private void OnStart(object sender, EventArgs e)
        {
            if (IsImageOkay(_uploadedRawNucleiImage8Bit) && IsImageOkay(_uploadedRawTelomereImage8Bit))
                Normalize();
        }

        private void Normalize()
        {
            /*
            //Normalizing the Nuclei Image
            Image<Gray, UInt16> destNucleiImage = new Image<Gray, UInt16>(_uploadedRawNucleiImage.Width, _uploadedRawNucleiImage.Height, new Gray(0));
            //CvInvoke.Normalize(_uploadedRawNucleiImage, destNucleiImage, 0, 65535, Emgu.CV.CvEnum.NormType.MinMax);
            CvInvoke.cvNormalize(_uploadedRawNucleiImage, destNucleiImage, 0, 65535, Emgu.CV.CvEnum.NORM_TYPE.CV_MINMAX, _uploadedRawNucleiImage);
            //CvInvoke.cvEqualizeHist(_uploadedRawNucleiImage, destNucleiImage);
            _NucleiImageNormalized = destNucleiImage;
            _btmNucleiImageNormalized = destNucleiImage.ToBitmap();

            //Normalizing the Telomere Image --> doesn't really make a visible difference
            Image<Gray, UInt16> destTelomereImage = new Image<Gray, UInt16>(_uploadedRawTelomereImage.Width, _uploadedRawTelomereImage.Height, new Gray(0));
            //CvInvoke.Normalize(_uploadedRawTelomereImage, destTelomereImage, 0, 65535, Emgu.CV.CvEnum.NormType.MinMax);
            CvInvoke.cvNormalize(_uploadedRawTelomereImage, destNucleiImage, 0, 65535, Emgu.CV.CvEnum.NORM_TYPE.CV_MINMAX, _uploadedRawTelomereImage);
            //CvInvoke.cvEqualizeHist(_uploadedRawTelomereImage, destTelomereImage);
            _TelomereImageNormalized = destTelomereImage;
            _btmTelomereImageNormalized = destTelomereImage.ToBitmap();

            //shows both of the images normalized
            ShowBitmapOnForm(ImageBoxOne, _btmNucleiImageNormalized);
            ShowBitmapOnForm(ImageBoxTwo, _btmTelomereImageNormalized);
            */
            lblPleaseSelectPic.Text = "Please click on Threshold to automatically generate a Threshold for the Telomere Image";
            //btnGenerateThreshold.Show();

            


            Threshold();
        }
        /*----------------------------------------------------------------------------------------*\
         *  Generates the threshold of the uploaded image using the otsu's method.                *|
         *  Converts the choosen image to grayscale and byte before thresholding                  *|
         *  otherwise an exception is thrown.
        \*----------------------------------------------------------------------------------------*/
        private void Threshold()
        {
            if (IsImageOkay(_TelomereImageNormalized))
            {
                Thresholding();
                _btmTelomereImageHalfTransparent = MergeImages(_btmNucleiImageNormalized, _btmTelomereImageThreshold);
                BtnDetectNuclei();
            }
            else
            {
                _TelomereImageNormalized = _uploadedRawTelomereImage8Bit;
                _btmTelomereImageNormalized = _uploadedRawTelomereImage8Bit.ToBitmap();
                _NucleiImageNormalized = _uploadedRawNucleiImage8Bit;
                _btmNucleiImageNormalized = _uploadedRawNucleiImage8Bit.ToBitmap();
                Thresholding();
                _btmTelomereImageHalfTransparent = MergeImages(_btmNucleiImageNormalized, _btmTelomereImageThreshold);
                BtnDetectNuclei();
            }

        }

        private Bitmap MergeImages(Bitmap primaryImage, Bitmap secondaryImage) {
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
            //savingImage = transparentImage;
            graphics.DrawImage(primaryImage, 0, 0);
            graphics.DrawImage(transparentImage, 0, 0);
            ShowBitmapOnForm(ImageBoxTwo, finalImage);
            //savingImage = finalImage;
            return finalImage;
            //btnDetectingNuclei.Show();
            //BtnDetectNuclei();
        }

        private void BtnDetectNuclei()
        {
            _elmiWood = new ElmiWood(this);
            _elmiWood.DoAnalyze(_NucleiImageNormalized);
            _nucleiBitonalForEdgeDetection = _elmiWood._nucleiBitonalForEdgeDetection;

            if (IsImageOkay(_nucleiBitonalForEdgeDetection))
            {
                _EdgeDetection = new EdgeDetection(this);
                _EdgeDetection.FindingContoursNuclei(_nucleiBitonalForEdgeDetection, _NucleiImageNormalized);
            }

            _btmNucleiImageEdgesDetected = _NucleiImageEdgesDetected.ToBitmap();
            ShowBitmapOnForm(ImageBoxOne, _btmNucleiImageEdgesDetected);
            //_btmNucleiImageEdgesDetected.Save("D:\\Hochschule Emden Leer - Bachelor Bioinformatik\\Praxisphase Bachelorarbeit Vorbereitungen\\Praktikumsstelle\\MHH Hannover Telomere\\Programm Bilder\\5_NucleiDetected.tiff");
            //Nuclei selber umranden können


            //Testing the Nucleus and Nuclei Classes
            //ShowBitmapOnForm(ImageBoxTwo, _TestingNucleiImageEdgesDetected.ToBitmap());

            /* Nun wurden die Nuclei automatisch umrandet.
             * Der User soll daraufhin die Nuclei selber einzeichnen können. 
             * Hierfür soll sich eine neue Form öffnen, in der dies komplett gehandelt wird.
             * Übergeben wird die Nuclei-Liste und diese kann modifiziert werden.
             */
            _allNuclei = _EdgeDetection._allNuclei;
            FormTwo formTwo = new FormTwo(_allNuclei, _NucleiImageNormalized.Convert<Bgr, byte>());
            formTwo.ShowDialog(); //.ShowDialog anstatt .Show, da es so nicht möglich ist auf das 1. Fenster zuzugreifen, bis das 2. Fenster geschlossen wurde

            _NucleiImageEdgesDetectedAndDrawn = formTwo._NucleiImageWithAutomaticEdgesToDrawOn;
            _btmNucleiImageMergedWithTresholdImage = MergeImages(_NucleiImageEdgesDetectedAndDrawn.ToBitmap(), _TelomereImageNormalized.ToBitmap());

            //Detecting Telomeres
            Image<Gray, byte> telomereImageToDrawOn = new Image<Gray, byte>(_btmTelomereImageThreshold);
            DetectingTelomeres(telomereImageToDrawOn);
        }

        #endregion

        #region Thresholding---------------------------------------------------------------------------------
        private void Thresholding()
        {
            Image<Gray, byte> image = _TelomereImageNormalized.Convert<Gray, byte>();
            Image<Gray, byte> destImage = new Image<Gray, byte>(image.Width, image.Height, new Gray(0));
            try
            {
                //double threshold = CvInvoke.Threshold(image.Convert<Gray, byte>(), destImage, 50, 255, Emgu.CV.CvEnum.ThresholdType.Otsu);
                double threshold = CvInvoke.cvThreshold(image.Convert<Gray, byte>(), destImage, 0.0, 255.0, THRESH.CV_THRESH_OTSU);
                lblThreshold.Text = "Calculated Threshold: " + threshold;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            _btmTelomereImageThreshold = ChangingColourOfBitonalImage(destImage);
            ShowBitmapOnForm(ImageBoxTwo, _btmTelomereImageThreshold);
            //_btmTelomereImageThreshold.Save("D:\\Hochschule Emden Leer - Bachelor Bioinformatik\\Praxisphase Bachelorarbeit Vorbereitungen\\Praktikumsstelle\\MHH Hannover Telomere\\Programm Bilder\\0_Threshold_Done_OnTelomeres.tiff");
            lblPleaseSelectPic.Text = "Please click on Merge Images to overlay both of the Images on top of each other";
            //btnMergeImages.Show();

        }

        //funktioniert, ist wahrscheinlich nicht die effizienteste Lösung
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
                     * Das Bild ist schwarzweiß, also 2 farbig. Wenn ein Rotanteil in dem Pixel drin ist, ist dieser Pixel weiß.
                     * Der weiße Pixel wird in rot umgewandelt und die restlichen Pixel werden schwarz gesetzt.
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
            //_TelomereImageTelomeresDetected.Save("D:\\Hochschule Emden Leer - Bachelor Bioinformatik\\Praxisphase Bachelorarbeit Vorbereitungen\\Praktikumsstelle\\MHH Hannover Telomere\\Programm Bilder\\6_TelomeresDetected.tiff");
            //Testing the Telomere and AllTelomere Classes
            //ShowBitmapOnForm(ImageBoxTwo, _TestingTelomereImageTelomeresDetected.ToBitmap());

            AllocateTelomeresToNucleus();
        }

        public void AllocateTelomeresToNucleus()
        {
            _allTelomeres = _EdgeDetection._allTelomeres;
            for (Int32 n = 0; n < _allNuclei._LstAllNuclei.Count; n++)
            {
                _allNuclei._LstAllNuclei[n].getAmountOfPixelsInNucleusArea(_uploadedRawNucleiImage16Bit);
                //Goes through every existing telomere per Nucleus --> checks if any contour-Point of the Telomere is in the Nucleus-Area
                for(Int32 t = 0; t < _allTelomeres._LstAllTelomeres.Count; t++)
                {
                    if(_allNuclei._LstAllNuclei[n].isTelomereContourInThisNucleus(_allTelomeres._LstAllTelomeres[t]._telomereContourPoints))
                            _allNuclei._LstAllNuclei[n].AddTelomereToTelomereList(_allTelomeres._LstAllTelomeres[t]);
                }
            }


            /*
            //hier muss ungebdingt nochmal überprüft werden, ob dies hier so richtig abläuft!
            //es wird immer noch geprüft, ob die Center-Points des Telomers innerhalb des Punkte-Arrays der Nucleus-Kontur vorhanden ist und NICHT, ob der Center-Point innerhalb dieses Nucleus-Kontur-Polygons enthalten ist!
            _allTelomeres = _EdgeDetection._allTelomeres;
            Boolean telomereIsInNucleus = false;
            //Geht alle Nuclei durch und geht dann alle Telomere pro Nucleus durch und überprüft ob diese in der Nucleus Kontur enthalten sind
            for (Int32 n = 0; n < _allNuclei._LstAllNuclei.Count; n++)
            {
                //Geht alle Telomere pro Nucleus durch --> es wird überprüft, ob der center point des Telomers in der Nucleus-Kontur enthalten ist
                //Theoretisch müssen nicht nochmal absolut alle Telomere überprüft werden, da ja manche schon Nuclei zugeordnet sind --> kann effizienter sein!
                for (Int32 t = 0; t < _allTelomeres._LstAllTelomeres.Count; t++)
                {
                    telomereIsInNucleus = _allNuclei.IsCenterPointOfTelomereInNucleus(_allNuclei._LstAllNuclei[n]._nucleusContourPoints, _allTelomeres._LstAllTelomeres[t]._telomereCenterPoint);
                    if (telomereIsInNucleus)
                    {
                        _allNuclei._LstAllNuclei[n].AddTelomereToTelomereList(_allTelomeres._LstAllTelomeres[t]);
                    }
                }

            }
            */

            //Alles zum Testen
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
            

            DisplayEndResults();
        }

        public void DisplayEndResults()
        {
            FormThree formThree = new FormThree(this, _allNuclei, _allTelomeres);
            formThree.Show();
            _DisplayingFinalResults = new DisplayingFinalResults(_allNuclei, _allTelomeres);
            _DisplayingFinalResults.PrintResultsOnConsole();
        }



        public void ShowBitmapOnForm(ImageBox imageBox, Bitmap bitmap)
        {
            if (imageBox.BackgroundImage == null)
            {
                imageBox.BackgroundImage = bitmap;
                imageBox.Width = bitmap.Width;
                imageBox.Height = bitmap.Height;
                imageBox.MaximumSize = bitmap.Size;
                //imageBox.BackgroundImageLayout
                imageBox.BackgroundImageLayout = ImageLayout.Stretch;
                imageBox.Refresh();
            }
            else
                imageBox.BackgroundImage = bitmap;
        }


        private void DisplaySelectPicForTreshhold()
        {
            grpBoxSelectDialog.Show();
        }

        private void OnClickNext(object sender, EventArgs e)
        {
            /*
            if(!nucleiImageUploaded && !telomereImageUploaded)
            {
                lblPleaseSelectPic.Text = "Please upload a Nuclei .TIFF file";
            }
            if (nucleiImageUploaded && !telomereImageUploaded)
            {
                lblPleaseSelectPic.Text = "Please upload a Telomere .TIFF file";
            }
            if(nucleiImageUploaded && telomereImageUploaded)
            {
                //lblPleaseSelectPic.Text
            }
            */
        }
        // checks if image is null --> needs to be extended probably
        private Boolean IsImageOkay(Image<Gray, byte> image)
        {
            if (image == null)
                return false;
            return true;
        }

        private void FormOne_Load(object sender, EventArgs e)
        {

        }
    }
}

