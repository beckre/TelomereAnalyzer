﻿using System;
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
        Image<Bgr, byte> Main8BitImage = null;       // Das Hauptbild
        public FormOne()
        {
            InitializeComponent();
            var dllDirectory = @"./OpenCv";
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable
                                               ("PATH") + ";" + dllDirectory);

            grpBoxSelectDialog.Hide();
            Console.WriteLine("Henloooo Uwu");
        }

        /*
         *  erstmal mit der 16 Bit Version des Bildes versuchen ohne es in 8 Bit zu konvertieren
         */

        private void OnUploadTIFF(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new
                System.Windows.Forms.OpenFileDialog();

            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {

                MainImage = new Image<Gray, UInt16>(dialog.FileName);
                //Main8BitImage = MainImage.Convert<Bgr, byte>();
                //Bitmap tiffImageConvertedTo8Bit = Main8BitImage.ToBitmap();
                Bitmap tiffImage = MainImage.ToBitmap();
                //ImageBox.BackgroundImage = tiffImageConvertedTo8Bit;
                ImageBox.BackgroundImage = tiffImage;
                displaySelectPicForTreshhold();

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
