using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;



namespace TelomereAnalyzer
{
    public partial class FormOne : Form
    {
        //Image<Bgr, byte> Main8BitImage = null;       // Das Hauptbild
        
        Boolean _nucleiImageUploaded = false;
        Boolean _telomereImageUploaded = false;

        NucleiEdgeDetection _nucleiEdgeDetection = null;

        /*
         * Die Bilder werden alle seperat gespeichert, da es die Option geben soll
         * die unterschiedlichen Stadien der Bilder später zu speichern.
         */

        //Image und Bitmap vom originalen hochgeladenen Nuclei Bild
        Image<Gray, UInt16> _uploadedRawNucleiImage = null;       
        Bitmap _btmUploadedRawNucleiImage = null;
        //Image und Bitmap vom originalen hochgeladenen Telomer Bild
        Image<Gray, UInt16> _uploadedRawTelomereImage = null;
        Bitmap _btmUploadedRawTelomereImage = null;

        //Image und Bitmap vom normalisierten Nuclei Bild
        public Image<Gray, UInt16> _NucleiImageNormalized = null;
        Bitmap _btmNucleiImageNormalized = null;

        //Image und Bitmap vom normalisierten Telomer Bild
        Image<Gray, UInt16> _TelomereImageNormalized = null;
        Bitmap _btmTelomereImageNormalized = null;

        //Bitmap vom normalisierten Bild, wo die Threshold Methode angewandt wurde
        Bitmap _btmTelomereImageThreshold = null;

        //Bitmap vom normalisierten Bild, wo die Threshold Methode angewandt wurde und die Transparenz auf die Hälfte gesetzt wurde
        Bitmap _btmTelomereImageHalfTransparent = null;

        //Image und Bitmap vom Nuclei Bild mit Anwendung von Edge Detection
        public Image<Bgr, UInt16> _NucleiImageEdgesDetected = null;
        Bitmap _btmNucleiImageEdgesDetected = null;

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

                _uploadedRawNucleiImage = new Image<Gray, UInt16>(dialog.FileName);
                //Main8BitImage = MainImage.Convert<Bgr, byte>();
                //Bitmap tiffImageConvertedTo8Bit = Main8BitImage.ToBitmap();
                //ImageBox.BackgroundImage = tiffImageConvertedTo8Bit;
                _btmUploadedRawNucleiImage = _uploadedRawNucleiImage.ToBitmap();
                ShowBitmapOnForm(ImageBoxOne, _btmUploadedRawNucleiImage);
                btnGenerateThreshold.Hide();
                btnMergeImages.Hide();
                btnFindNucleiContours.Hide();
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

                _uploadedRawTelomereImage = new Image<Gray, UInt16>(dialog.FileName);
                //Main8BitImage = MainImage.Convert<Bgr, byte>();
                //Bitmap tiffImageConvertedTo8Bit = Main8BitImage.ToBitmap();
                //ImageBox.BackgroundImage = tiffImageConvertedTo8Bit;
                _btmUploadedRawTelomereImage = _uploadedRawTelomereImage.ToBitmap();
                ShowBitmapOnForm(ImageBoxTwo, _btmUploadedRawTelomereImage);
                btnGenerateThreshold.Hide();
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
        private void OnNormalize(object sender, EventArgs e)
        {
            if (IsImageOkay(_uploadedRawNucleiImage) && IsImageOkay(_uploadedRawTelomereImage))
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
            btnGenerateThreshold.Show();
            
        }
            /*----------------------------------------------------------------------------------------*\
             *  Generates the threshold of the uploaded image using the otsu's method.                *|
             *  Converts the choosen image to grayscale and byte before thresholding                  *|
             *  otherwise an exception is thrown.
            \*----------------------------------------------------------------------------------------*/
            private void OnThreshold(object sender, EventArgs e)
        {
            if (IsImageOkay(_TelomereImageNormalized))
            {
                Thresholding();
            }
            else
            {
                _TelomereImageNormalized = _uploadedRawTelomereImage;
                _btmTelomereImageNormalized = _uploadedRawTelomereImage.ToBitmap();
                _NucleiImageNormalized = _uploadedRawNucleiImage;
                _btmNucleiImageNormalized = _uploadedRawNucleiImage.ToBitmap();
                Thresholding();
            }
                
        }

        private void OnMergeImages(object sender, EventArgs e)
        {
            var finalImage = new Bitmap(_btmTelomereImageThreshold.Width, _btmTelomereImageThreshold.Height);
            var graphics = Graphics.FromImage(finalImage);
            graphics.CompositingMode = CompositingMode.SourceOver;

            double alphaTransparent = 0.0;
            var transparentImage = new Bitmap(_btmTelomereImageThreshold.Width, _btmTelomereImageThreshold.Height);
            for (int x = 0; x < _btmTelomereImageThreshold.Width; ++x)
            {
                for (int y = 0; y < _btmTelomereImageThreshold.Height; ++y)
                {
                    Color pixel = _btmTelomereImageThreshold.GetPixel(x, y);
                    alphaTransparent = pixel.A / 2.0;
                    transparentImage.SetPixel(x, y, Color.FromArgb(Convert.ToInt32(alphaTransparent), pixel.R, pixel.G, pixel.B));
                }
            }
            _btmTelomereImageHalfTransparent = transparentImage;
            graphics.DrawImage(_btmNucleiImageNormalized, 0, 0);
            graphics.DrawImage(_btmTelomereImageHalfTransparent, 0, 0);
            ShowBitmapOnForm(ImageBoxTwo, finalImage);
            btnFindNucleiContours.Show();
        }

        private void OnFindNucleiContours(object sender, EventArgs e)
        {
            _nucleiEdgeDetection = new NucleiEdgeDetection(this);
            if (IsImageOkay(_NucleiImageNormalized))
            {
                _nucleiEdgeDetection.FindingContours();
                /*
                if (IsImageOkay(_nucleiImageEdgesDetected))
                {
                    _btmNucleiImageEdgesDetected = _nucleiImageEdgesDetected.ToBitmap();
                    ShowBitmapOnForm(ImageBoxTwo, _btmNucleiImageEdgesDetected);
                }
                */
            }
            _btmNucleiImageEdgesDetected = _NucleiImageEdgesDetected.ToBitmap();
            ShowBitmapOnForm(ImageBoxOne, _btmNucleiImageEdgesDetected);
        }

        #endregion

        #region Thresholding---------------------------------------------------------------------------------
        private void Thresholding()
        {
            //hat vorher mit 16 Bit Bildern funktioniert! aber auch nur mit der Emgu.Cv.World.dll
            Image<Gray, byte> image = _TelomereImageNormalized.Convert<Gray, byte>();
            Image<Gray, byte> destImage = new Image<Gray, byte>(image.Width, image.Height, new Gray(0));
            try
            {
                //double threshold = CvInvoke.Threshold(image.Convert<Gray, byte>(), destImage, 50, 255, Emgu.CV.CvEnum.ThresholdType.Otsu);
                double threshold = CvInvoke.cvThreshold(image.Convert<Gray, byte>(), destImage, 0.0, 255.0, THRESH.CV_THRESH_OTSU);
                lblThreshold.Text = "Calculated Threshold: "+threshold;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            _btmTelomereImageThreshold = ChangingColourOfBitonalImage(destImage.Convert<Gray, UInt16>());
            ShowBitmapOnForm(ImageBoxTwo, _btmTelomereImageThreshold);
            lblPleaseSelectPic.Text = "Please click on Merge Images to overlay both of the Images on top of each other";
            btnMergeImages.Show();

        }

        //funktioniert, ist wahrscheinlich nicht die effizienteste Lösung
        private Bitmap ChangingColourOfBitonalImage(Image<Gray, UInt16> image)
        {
            Bitmap destImageBitmap = image.ToBitmap();
            //Ziel Bitmap muss diese Art von Bitmap sein, weil sonst die Methode .SetPixel() nicht aufrufbar ist
            Bitmap resultBmpToBeColoured = new Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int width = destImageBitmap.Width;
            int height = destImageBitmap.Height;

            for(int i = 0; i< width; i++)
            {
                for(int j = 0; j< height; j++)
                {
                    Color checkedColor = destImageBitmap.GetPixel(i, j);
                    /*
                     * Das Bild ist schwarzweiß, also 2 farbig. Wenn ein Rotanteil in dem Pixel drin ist, ist dieser Pixel weiß.
                     * Der weiße Pixel wird in rot umgewandelt und die restlichen Pixel werden schwarz gesetzt.
                     */
                    if (checkedColor.R == 255)
                        resultBmpToBeColoured.SetPixel(i, j, Color.FromArgb(255,255, 0, 0));
                    else
                        resultBmpToBeColoured.SetPixel(i, j, Color.FromArgb(255, 0, 0, 0));
                }
            }
            return resultBmpToBeColoured;
        }
        #endregion

        public void ShowBitmapOnForm(ImageBox imageBox, Bitmap bitmap)
        {
            if(imageBox.BackgroundImage == null)
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
        private Boolean IsImageOkay(Image<Gray, UInt16> image)
        {
            if (image == null)
                return false;
            return true;
        }
    }
}
