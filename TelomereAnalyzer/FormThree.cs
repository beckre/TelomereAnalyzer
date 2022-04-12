using OfficeOpenXml;
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

namespace TelomereAnalyzer
{
    public partial class FormThree : Form
    {
        Nuclei _allNuclei = null;
        List<Nucleus> _nuclei = null;
        AllTelomeres _allTelomeres = null;


        public FormThree(Nuclei nuclei, AllTelomeres allTelomeres)
        {
            /* The libary EPPlus is used in a non-commercial context as 
             * this application is also non-commercial*/
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            this._allNuclei = nuclei;
            _nuclei = _allNuclei._LstAllNuclei;
            this._allTelomeres = allTelomeres;
            InitializeComponent();

            //Speichert die hier erzeugte Excel-Datei erstmal in einem bestimmten Pfad
            var file = new FileInfo(@"D:\Hochschule Emden Leer - Bachelor Bioinformatik\Praxisphase Bachelorarbeit Vorbereitungen\Praktikumsstelle\MHH Hannover Telomere\Erzeugte Excel-Datei");
            CreateExcelSheet(file);
        }

        private void CreateExcelSheet(FileInfo fileInfo)
        {
            using (var package = new ExcelPackage(fileInfo))
            {
                
            }

        }
    }
}
