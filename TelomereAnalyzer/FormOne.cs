using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Drawing;
using System.Windows.Forms;



namespace TelomereAnalyzer
{
    public partial class FormOne : Form
    {
        //Image<Bgr, byte> Main8BitImage = null;       // Das Hauptbild
        
        Boolean _nucleiImageUploaded = false;
        Boolean _telomereImageUploaded = false;

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
        Image<Gray, UInt16> _resultNucleiImageNormalized = null;
        Bitmap _btmResultNucleiImageNormalized = null;

        //Image und Bitmap vom normalisierten Telomer Bild
        Image<Gray, UInt16> _resultTelomereImageNormalized = null;
        Bitmap _btmResultTelomereImageNormalized = null;

        //Bitmap vom normalisierten Bild, wo die Threshold Methode angewandt wurde
        Bitmap _btmResultImageThreshold = null;

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
                _nucleiImageUploaded = true;
            }
        }

        private void OnUploadTelomereImage(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new
            System.Windows.Forms.OpenFileDialog();
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
                    grpBoxSelectOptions.Show();
                }
            }
        }

        private void OnNormalize(object sender, EventArgs e)
        {
            //Normalizing the Nuclei Image
            Image<Gray, UInt16> destNucleiImage = new Image<Gray, UInt16>(_uploadedRawNucleiImage.Width, _uploadedRawNucleiImage.Height, new Gray(0));
            CvInvoke.Normalize(_uploadedRawNucleiImage, destNucleiImage, 0, 65535, Emgu.CV.CvEnum.NormType.MinMax);
            _resultNucleiImageNormalized = destNucleiImage;
            _btmResultNucleiImageNormalized = destNucleiImage.ToBitmap();

            //Normalizing the Telomere Image
            Image<Gray, UInt16> destTelomereImage = new Image<Gray, UInt16>(_uploadedRawTelomereImage.Width, _uploadedRawTelomereImage.Height, new Gray(0));
            CvInvoke.Normalize(_uploadedRawTelomereImage, destTelomereImage, 0, 65535, Emgu.CV.CvEnum.NormType.MinMax);
            _resultTelomereImageNormalized = destTelomereImage;
            _btmResultTelomereImageNormalized = destTelomereImage.ToBitmap();

            //only shows the normaized nuclei image for now
            ShowBitmapOnForm(ImageBoxOne, _btmResultNucleiImageNormalized);
            btnGenerateThreshold.Show();
        }
            /*----------------------------------------------------------------------------------------*\
             *  Generates the threshold of the uploaded image using the otsu's method.                *|
             *  Converts the choosen image to grayscale and byte before thresholding                  *|
             *  otherwise an exception is thrown.
            \*----------------------------------------------------------------------------------------*/
            private void OnThreshold(object sender, EventArgs e)
        {
            Thresholding(_resultTelomereImageNormalized);
        }

        #endregion

        #region Thresholding---------------------------------------------------------------------------------
        private void Thresholding(Image<Gray, UInt16> image)
        {
            Image<Gray, UInt16> destImage = new Image<Gray, UInt16>(image.Width, image.Height, new Gray(0));
            try
            { 
                double threshold = CvInvoke.Threshold(image.Convert<Gray, byte>(), destImage, 50, 255, Emgu.CV.CvEnum.ThresholdType.Otsu);
                lblThreshold.Text = "Calculated Threshold: "+threshold;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            _btmResultImageThreshold = ChangingColourOfBitonalImage(destImage);
            ShowBitmapOnForm(ImageBoxTwo, _btmResultImageThreshold);

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
                     * Das Bild ist schwarzweiß, also 2 farbig. Weenn ein Rotanteil in dem Pixel drin ist, ist dieser Pixel weiß.
                     * Der weiße Pixel wird in rot umgewandelt und die restlichen Pixel werden schwarz gesetzt.
                     */
                    if (checkedColor.R == 255)
                    {
                        resultBmpToBeColoured.SetPixel(i, j, Color.FromArgb(255,255, 0, 0));
                    }
                    else
                    {
                        resultBmpToBeColoured.SetPixel(i, j, Color.FromArgb(255, 0, 0, 0));
                    }
                }
            }
            return resultBmpToBeColoured;
        }
        #endregion

        private void ShowBitmapOnForm(ImageBox imageBox, Bitmap bitmap)
        {
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


    }
}
