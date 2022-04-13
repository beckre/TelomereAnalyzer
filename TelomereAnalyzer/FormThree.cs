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
        Nuclei _allNuclei = null;
        List<Nucleus> _Lstnuclei = null;
        AllTelomeres _allTelomeres = null;
        mso.Application excel = new mso.Application();
        mso.Workbook wb;
        mso.Worksheet ws;


        public FormThree(Nuclei nuclei, AllTelomeres allTelomeres)
        {
            this._allNuclei = nuclei;
            _Lstnuclei = _allNuclei._LstAllNuclei;
            this._allTelomeres = allTelomeres;

            //Speichert die hier erzeugte Excel-Datei erstmal in einem bestimmten Pfad
            var file = new FileInfo(@"D:\Hochschule Emden Leer - Bachelor Bioinformatik\Praxisphase Bachelorarbeit Vorbereitungen\Praktikumsstelle\MHH Hannover Telomere\Erzeugte Excel-Datei\excel.xls");
            
            
            //object misValue = System.Reflection.Missing.Value;
            excel = new mso.Application();
            //wb = excel.Workbooks.Add(misValue);
            wb = excel.Workbooks.Add();
            ws = wb.Worksheets[1];
            ws.Name = "Telomere";

            CreateExcelFile(file);
            InitializeComponent();
            
        }

        private void CreateExcelFile(FileInfo filePath)
        {
            //here all the headings of the excel file are specified
            ws.Cells[1, 1] = "Nucleus";
            ws.Cells[1, 2] = "Telomere Number";
            ws.Cells[1, 3] = "Telomere ID";
            ws.Cells[1, 4] = "Telomere Name";
            ws.Cells[1, 5] = "X";
            ws.Cells[1, 6] = "Y";
            //
            //
            FillExcelFile();
            wb.SaveAs(filePath);
            wb.Close();
        }

        private void FillExcelFile()
        {
            Int32 counter = 2;
            for (Int32 n = 0; n < _Lstnuclei.Count; n++)
            {
                List<Telomere> lsTelomeres = _Lstnuclei[n]._LstNucleusTelomeres;
                for (Int32 t = 0; t < lsTelomeres.Count; t++)
                {
                    //here the corresponding Nuclei is written in the 1. column of the Excel file
                    String strNuclei = _Lstnuclei[n]._nucleusName;
                    ws.Cells[counter, 1] = "N " + strNuclei.Substring(strNuclei.LastIndexOf(" ") + 1);

                    //here the Telomere Numbers are written in the 2. column of the Excel file
                    ws.Cells[counter, 2] = counter-1;

                    //here the Telomere ID is written in the 3. column of the Excel file --> takes the last characters of the Telomere Name
                    String strTID = lsTelomeres[t]._telomereName;
                    ws.Cells[counter, 3] = strTID.Substring(strTID.LastIndexOf(" ") + 1);

                    //here the Telomere Names are written in the 4. column of the Excel file
                    ws.Cells[counter, 4] = lsTelomeres[t]._telomereName;
                    


                    counter++;
                }
            }
        }
    }
}
