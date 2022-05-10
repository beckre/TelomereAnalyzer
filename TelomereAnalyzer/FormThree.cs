using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using mso = Microsoft.Office.Interop.Excel;

namespace TelomereAnalyzer
{
    public partial class FormThree : Form
    {
        FormOne _formOne;
        Nuclei _allNuclei = null;
        List<Nucleus> _Lstnuclei = null;
        AllTelomeres _allTelomeres = null;

        mso.Application excel = new mso.Application();
        mso.Workbook wb;
        mso.Worksheet ws;


        public FormThree(FormOne formOne, Nuclei nuclei, AllTelomeres allTelomeres)
        {
            this._formOne = formOne;
            this._allNuclei = nuclei;
            _Lstnuclei = _allNuclei._LstAllNuclei;
            this._allTelomeres = allTelomeres;

            //Speichert die hier erzeugte Excel-Datei erstmal in einem bestimmten Pfad
            //var file = new FileInfo(@"D:\Hochschule Emden Leer - Bachelor Bioinformatik\Praxisphase Bachelorarbeit Vorbereitungen\Praktikumsstelle\MHH Hannover Telomere\Erzeugte Excel-Datei\excel.xls");
            
            //object misValue = System.Reflection.Missing.Value;
            excel = new mso.Application();
            //wb = excel.Workbooks.Add(misValue);
            wb = excel.Workbooks.Add();
            ws = wb.Worksheets[1];
            ws.Name = "Telomere";

            CreateExcelFile();
            InitializeComponent();
            
        }

        private void CreateExcelFile()
        {
            //here all the headings of the excel file are specified
            ws.Cells[1, 1] = "Nucleus";
            ws.Cells[1, 2] = "Telomere Number";
            ws.Cells[1, 3] = "Telomere ID";
            ws.Cells[1, 4] = "Telomere Name";
            ws.Cells[1, 5] = "X";
            ws.Cells[1, 6] = "Y";
            ws.Cells[1, 7] = "Area";
            ws.Cells[1, 8] = "Sum";
            ws.Cells[1, 9] = "Min";
            ws.Cells[1, 10] = "Max";
            ws.Cells[1, 11] = "Stddev";
            ws.Cells[1, 12] = "Mean";
            ws.Cells[1, 13] = "Average of Means";

            FillExcelFile();
        }

        private void FillExcelFile()
        {
            Int32 counter = 2;
            
            for (Int32 n = 0; n < _Lstnuclei.Count; n++)
            {
                Int32 telomereNumber = 1;
                List<Telomere> lsTelomeres = new List<Telomere>();
                lsTelomeres = _Lstnuclei[n]._LstNucleusTelomeres;
                //If the Nuclei does not contain a Telomere at all, then this Nuclei should not show up in the Excel-Sheet at all
                if (lsTelomeres.Count <= 0)
                    continue;
                //are needed for calculating the average of the means
                double sum = 0;
                double averageOfMeans = 0;
                /*
                for (Int32 t = 0; t < lsTelomeres.Count; t++)
                {
                    
                    sum += lsTelomeres[t]._mean;
                }
                averageOfMeans = sum / lsTelomeres.Count;
                */

                for (Int32 t = 0; t < lsTelomeres.Count; t++)
                {
                    //here the corresponding Nuclei is written in the 1. column of the Excel file
                    String strNuclei = _Lstnuclei[n]._nucleusName;
                    ws.Cells[counter, 1] = "N " + strNuclei.Substring(strNuclei.LastIndexOf(" ") + 1);

                    //here the Telomere Numbers are written in the 2. column of the Excel file
                    ws.Cells[counter, 2] = telomereNumber;

                    //here the Telomere ID is written in the 3. column of the Excel file --> takes the last characters of the Telomere Name
                    String strTID = lsTelomeres[t]._telomereName;
                    ws.Cells[counter, 3] = strTID.Substring(strTID.LastIndexOf(" ") + 1);

                    //here the Telomere Names are written in the 4. column of the Excel file
                    ws.Cells[counter, 4] = lsTelomeres[t]._telomereName;

                    //here the lowest X- and Y-Values of the Telomere-Contour is found and written in the 5. and 6. column of the Excel file
                    lsTelomeres[t].getLowestAndHighestXY();
                    ws.Cells[counter, 5] = lsTelomeres[t]._lowestX;
                    ws.Cells[counter, 6] = lsTelomeres[t]._lowestY;

                    //here the area of the telomere is calculated and written in the 7. column of the Excel file
                    //the area is the amount of pixels inside the telomere

                    lsTelomeres[t].getAmountOfPixelsInTelomereArea(_formOne._TelomereImageAutoLevel);
                    ws.Cells[counter, 7] = lsTelomeres[t]._area;

                    //here the sum and the min and max values of the telomere are calculated and written in the 8. column of the Excel file
                    //the sum is the sum of all pixel values in the telomere
                    // the min/max are the min/max values of the pixel values in the telomere
                    //lsTelomeres[t].getSumMinMaxMeanStddevOfTelomere(_formOne._uploadedRawTelomereImage16Bit);
                    lsTelomeres[t].getSumMinMaxMeanStddevOfTelomere(_formOne._TelomereImageAutoLevel);
                    ws.Cells[counter, 8] = lsTelomeres[t]._sum;
                    ws.Cells[counter, 9] = lsTelomeres[t]._min;
                    ws.Cells[counter, 10] = lsTelomeres[t]._max;

                    //here the mean and stdv is written in the 11. and 12. column of the Excel file
                    ws.Cells[counter, 11] = lsTelomeres[t]._stdDev;
                    ws.Cells[counter, 12] = lsTelomeres[t]._mean;

                    //here the average of means is written in the 13. column of the Excel file
                    sum += lsTelomeres[t]._mean;
                    counter++;
                    telomereNumber++;
                }
                //Kann natürlich nicht durch 0 teilen --> Polygon konnte nicht rot gefärbt werden aufgrund von zu wenig Kontur-Punkten oder Standort der Punkte
                //Ist dies der Fall gibt es natürlich auch nur eine Area = 0 --> Alles andere = 0 --> muss gefixt werden
                if (lsTelomeres.Count > 0)
                    averageOfMeans = sum / lsTelomeres.Count;
                else
                    averageOfMeans = 0;
                ws.Cells[counter-1, 13] = averageOfMeans;
            }
        }

        private void OnSaveAutoLevelNucleiImage(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.Filter = "tiff files (*.tiff)|*.tiff|All files (*.*|*.*";
            saveFileDlg.FileName = _formOne._nucleiFileName +"_Auto-Level Nuclei";
            if (saveFileDlg.FileName.Length >= 248)
                saveFileDlg.FileName = "_Auto-Level Nuclei";

            if (saveFileDlg.ShowDialog() == DialogResult.OK)
            {
                _formOne._btmNucleiImageAutoLevel.Save(saveFileDlg.FileName);
                //_formOne._btmTelomereImageThreshold.Dispose();
            }
            else if (saveFileDlg.ShowDialog() == DialogResult.Cancel)
                return;
        }
        private void OnSaveAutoLevelTelomereImage(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.Filter = "tiff files (*.tiff)|*.tiff|All files (*.*|*.*";
            saveFileDlg.FileName = _formOne._telomereFileName+"_Auto-Level Telomere";
            if (saveFileDlg.FileName.Length >= 248)
                saveFileDlg.FileName = "_Auto-Level Telomere";

            if (saveFileDlg.ShowDialog() == DialogResult.OK)
            {
                _formOne._btmTelomereImageAutoLevel.Save(saveFileDlg.FileName);
                //_formOne._btmTelomereImageThreshold.Dispose();
            }
            else if (saveFileDlg.ShowDialog() == DialogResult.Cancel)
                return;
        }

        private void OnSaveThresholdTelomereImage(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.Filter = "tiff files (*.tiff)|*.tiff|All files (*.*|*.*";
            saveFileDlg.FileName = _formOne._telomereFileName+"_Threshold Telomere";
            if (saveFileDlg.FileName.Length >= 248)
                saveFileDlg.FileName = "_Threshold Telomere";

            if (saveFileDlg.ShowDialog() == DialogResult.OK)
            {
                _formOne._btmTelomereImageThreshold.Save(saveFileDlg.FileName);
                //_formOne._btmTelomereImageThreshold.Dispose();
            }
            else if (saveFileDlg.ShowDialog() == DialogResult.Cancel)
                return;
        }

        private void OnSaveThresholdTelomereOverlayNucleiImage(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.Filter = "tiff files (*.tiff)|*.tiff|All files (*.*|*.*";
            saveFileDlg.FileName = _formOne._nucleiFileName+"_Threshold Telomere+Nuclei";
            if (saveFileDlg.FileName.Length >= 248)
                saveFileDlg.FileName = "_Threshold Telomere Overlay Nuclei";

            if (saveFileDlg.ShowDialog() == DialogResult.OK)
            {
                _formOne._btmTelomereImageHalfTransparent.Save(saveFileDlg.FileName);
                //disposen
            }
            else if (saveFileDlg.ShowDialog() == DialogResult.Cancel)
                return;
        }

        private void OnSaveDetectedNucleiImage(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.Filter = "tiff files (*.tiff)|*.tiff|All files (*.*|*.*";
            saveFileDlg.FileName = _formOne._nucleiFileName+"_Detected Nuclei";
            if (saveFileDlg.FileName.Length >= 248)
                saveFileDlg.FileName = "_Detected Nuclei";

            if (saveFileDlg.ShowDialog() == DialogResult.OK)
            {
                _formOne._NucleiImageEdgesDetected.Save(saveFileDlg.FileName);
                //disposen
            }
            else if (saveFileDlg.ShowDialog() == DialogResult.Cancel)
                return;
        }

        private void OnSaveDetectedAndDrawnNucleiImage(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.Filter = "tiff files (*.tiff)|*.tiff|All files (*.*|*.*";
            saveFileDlg.FileName = _formOne._nucleiFileName+"_Detected+drawn Nuclei";

            if (saveFileDlg.FileName.Length >= 248)
                saveFileDlg.FileName = "_Detected and drawn Nucle";

            if (saveFileDlg.ShowDialog() == DialogResult.OK)
            {
                _formOne._NucleiImageEdgesDetectedAndDrawn.Save(saveFileDlg.FileName);
                //disposen
            }
            else if (saveFileDlg.ShowDialog() == DialogResult.Cancel)
                return;
        }
        private void OnSaveMergedDetectedAndDrawnNucleiImage(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.Filter = "tiff files (*.tiff)|*.tiff|All files (*.*|*.*";
            saveFileDlg.FileName = _formOne._nucleiFileName+"_Detected+drawn Nuclei+Threhold Telomere";
            if (saveFileDlg.FileName.Length >= 248)
                saveFileDlg.FileName = "_Detected+drawn Nuclei+Threhold Telomere";

            if (saveFileDlg.ShowDialog() == DialogResult.OK)
            {
                _formOne._btmNucleiImageMergedWithTresholdImage.Save(saveFileDlg.FileName);
                //disposen
            }
            else if (saveFileDlg.ShowDialog() == DialogResult.Cancel)
                return;
        }

        private void OnExportExcel(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.Filter = "Excel files (*.xlsx)|*.xlsx|Excel files (*.xls)|*.xls|All files (*.*)|*.*";
            saveFileDlg.FileName = _formOne._nucleiFileName+"_Telomere Analysis";
            if (saveFileDlg.FileName.Length >= 248)
                saveFileDlg.FileName = "_Telomere Analysis";
            if (saveFileDlg.ShowDialog() == DialogResult.OK)
            {
                wb.SaveAs(saveFileDlg.FileName);
                wb.Close();
            }
            else if (saveFileDlg.ShowDialog() == DialogResult.Cancel)
                return;

        }


    }
}
