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
using Emgu.CV.Structure;
using Emgu.CV.UI;



namespace TelomereAnalyzer
{
    public partial class FormOne : Form
    {
        Image<Gray, UInt16> MainImage = null;       // Das Hauptbild
        //Image<Bgr, byte> Main8BitImage = null;       // Das Hauptbild
        public FormOne()
        {
            InitializeComponent();
            var dllDirectory = @"./OpenCv";
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable
                                               ("PATH") + ";" + dllDirectory);

            grpBoxSelectDialog.Hide();
        }

        /*----------------------------------------------------------------------------------------*\
         *  is called when clicking on the uplad --> .tiff button.                                *|
         *  Enabled the user to choose and load any file for now --> only .tiff should be         *|
         *  acceptible.
        \*----------------------------------------------------------------------------------------*/

        private void OnUploadTIFF(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new
                System.Windows.Forms.OpenFileDialog();

            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                //erstmal mit der 16 Bit Version des Bildes versuchen ohne es in 8 Bit zu konvertieren

                MainImage = new Image<Gray, UInt16>(dialog.FileName);
                //Main8BitImage = MainImage.Convert<Bgr, byte>();
                //Bitmap tiffImageConvertedTo8Bit = Main8BitImage.ToBitmap();
                Bitmap tiffImage = MainImage.ToBitmap();
                //ImageBox.BackgroundImage = tiffImageConvertedTo8Bit;
                ImageBox.BackgroundImage = tiffImage;
                Thresholding(MainImage);

            }
        }
        /*----------------------------------------------------------------------------------------*\
         *  Generates the threshold of the uploaded image using the otsu's method.                *|
         *  Converts the choosen image to grayscale and byte before thresholding                  *|
         *  otherwise an exception is thrown.
        \*----------------------------------------------------------------------------------------*/

        private void Thresholding(Image<Gray, UInt16> image)
        {
            Image<Gray, UInt16> destImage = new Image<Gray, UInt16>(image.Width, image.Height, new Gray(0));
            try
            { 
                double threshold = CvInvoke.Threshold(image.Convert<Gray, byte>(), destImage, 50, 255, Emgu.CV.CvEnum.ThresholdType.Otsu);
                lblThreshold.Text = "Calculated Threshold: "+threshold;
                // wäre schön das Threshold Bild in rot schwarz darzustellen anstatt von weiß schwarz

                //destImage.Convert<Hsv, byte>();
                Bitmap destImageBitmap = destImage.ToBitmap();
                ImageBoxTwo.BackgroundImage = destImageBitmap;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
           
        }
        private void displaySelectPicForTreshhold()
        {
            grpBoxSelectDialog.Show();

        }

        private void OnClickNext(object sender, EventArgs e)
        {
            lblPleaseSelectPic.Text = "The Treshhold was succesfully generated";
        }
    }
}
