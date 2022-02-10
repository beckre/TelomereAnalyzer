using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;



namespace TelomereAnalyzer
{
    public partial class FormOne : Form
    {
        //Image<Bgr, byte> Main8BitImage = null;       // Das Hauptbild

        //Image und Bitmap vom originalen hochgeladenen Bild
        Image<Gray, UInt16> _uploadedRawImage = null;       
        Bitmap _btmUploadedRawImage = null;
        //Image und Bitmap vom normalisierten Bild
        Image<Gray, UInt16> _resultImageNormalized = null;
        Bitmap _btmResultImageNormalized = null;
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

        private void OnUploadTIFF(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new
                System.Windows.Forms.OpenFileDialog();

            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                //erstmal mit der 16 Bit Version des Bildes versuchen ohne es in 8 Bit zu konvertieren

                _uploadedRawImage = new Image<Gray, UInt16>(dialog.FileName);
                //Main8BitImage = MainImage.Convert<Bgr, byte>();
                //Bitmap tiffImageConvertedTo8Bit = Main8BitImage.ToBitmap();
                //ImageBox.BackgroundImage = tiffImageConvertedTo8Bit;
                _btmUploadedRawImage = _uploadedRawImage.ToBitmap();
                ImageBoxOne.BackgroundImage = _btmUploadedRawImage;
                //Thresholding(_uploadedRawImage);
                grpBoxSelectOptions.Show();
                btnGenerateThreshold.Hide();

            }
        }
        
        private void OnNormalize(object sender, EventArgs e)
        {
            Image<Gray, UInt16> destImage = new Image<Gray, UInt16>(_uploadedRawImage.Width, _uploadedRawImage.Height, new Gray(0));
            CvInvoke.Normalize(_uploadedRawImage, destImage, 0, 65535, Emgu.CV.CvEnum.NormType.MinMax);
            _resultImageNormalized = destImage;
            _btmResultImageNormalized = destImage.ToBitmap();
            ShowBitmapOnForm(ImageBoxOne, _btmResultImageNormalized);
            btnGenerateThreshold.Show();
        }
            /*----------------------------------------------------------------------------------------*\
             *  Generates the threshold of the uploaded image using the otsu's method.                *|
             *  Converts the choosen image to grayscale and byte before thresholding                  *|
             *  otherwise an exception is thrown.
            \*----------------------------------------------------------------------------------------*/
            private void OnThreshold(object sender, EventArgs e)
        {
            //Thresholding(_uploadedRawImage);
            Thresholding(_resultImageNormalized);
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
            lblPleaseSelectPic.Text = "The Treshhold was succesfully generated";
        }
    }
}
