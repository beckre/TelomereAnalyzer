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
using Emgu.CV.Structure; // Gray etc.
using System.IO;
using System.Drawing.Drawing2D;
using System.Reflection;

namespace TelomereAnalyzer
{
    public partial class ElmiWood : Form
    {
        FormOne _formOne = null;
        vesselDetectorClass _imgProcessor = null;
        StreamWriter txtStream = null;

        public Bitmap _oriImage = null;
        public Image<Gray, byte> _grayImage = null;

        public String _resultFile = null;
        String[] _filesToProcess = null;
        String _targetDir = null;
        String _singleFileAnalysisImageToSave = null;

        public bool _mergeToOneResultfile = true;
        bool _singleFileAnalysis = true;
        bool _scrollerInAction = false;
        bool _settingsInAction = false;
        bool _vesselAnalysisDone = false;

        VesselClass vesselToMerge = null;
        Int32 vesselToMergeNumber;
        Int32 vesselToMergeClusterNumber;
        Int32 newClusterNumber;

        bool hitFirstVessel = false;
        bool hitSecondVessel = false;

        #region stuff for vessel analysis

        VesselClass[] _vesselsFound = null;         // Result of thresholding procedure
        VesselAnalysisClass _vesselAnalysis = null;  // Cluster analysis
        VesselAnalysisClass.StochasticalVesselGeometry _normalizedVesselGeometry;
        Double _neighborSearchHor = 0.0;
        Double _neighborSearchVer = 0.0;

        #endregion(stuff for vessel analysis)
        
        public ElmiWood(FormOne formOne)
        {
            //SetLocalPathDescriptions(Environment.CurrentDirectory);
            //InitializeComponent();
            //InitButtons();
            //ReadConfigurationFile();
            _formOne = formOne;
            DoAnalyze(_formOne._NucleiImageNormalized);
        }
        /*
        protected void SetLocalPathDescriptions(String baseDir)
        {
            var dllDirectory = baseDir + @"\Dlls";
            String myEnvironment = Environment.GetEnvironmentVariable("PATH");
            String myNewEnvironment = dllDirectory + ";" + myEnvironment;

            Environment.SetEnvironmentVariable("PATH", myNewEnvironment);
            myEnvironment = Environment.GetEnvironmentVariable("PATH");
            var emguCvDllDirectory = baseDir + @"\Dlls\OpenCv";
            myNewEnvironment = emguCvDllDirectory + ";" + myEnvironment;

            Environment.SetEnvironmentVariable("PATH", myNewEnvironment);
            myEnvironment = Environment.GetEnvironmentVariable("PATH");
            var emguCvDllSubDirectoryx86 = baseDir + @"\Dlls\EmguCv\x86";
            myNewEnvironment = emguCvDllSubDirectoryx86 + ";" + myEnvironment;

            Environment.SetEnvironmentVariable("PATH", myNewEnvironment);
            myEnvironment = Environment.GetEnvironmentVariable("PATH");
            var emguCvDllSubDirectoryx64 = baseDir + @"\Dlls\EmguCv\x64";
            myNewEnvironment = emguCvDllSubDirectoryx64 + ";" + myEnvironment;

            Environment.SetEnvironmentVariable("PATH", myNewEnvironment);
            myEnvironment = Environment.GetEnvironmentVariable("PATH");
        }

        private void InitButtons()
        {
            btnBackground.Hide();
        }

        private void OnOpenImageFile(Object sender, EventArgs e)
        {
            String fileName = null;
            String pathAlone = null;
            String fileTargetName = null;

            if (dlgOpenFile.ShowDialog() != DialogResult.OK)
                return;

            if (dlgOpenFile.FileNames != null)
            {
                _filesToProcess = new String[dlgOpenFile.FileNames.Length];

                for (Int32 F = 0; F < dlgOpenFile.FileNames.Length; F++)
                    _filesToProcess[F] = dlgOpenFile.FileNames[F];
            }

            if (_filesToProcess.Length == 1)  // Single file analysis
            {
                _singleFileAnalysis = true;
                btnBackground.Show();

                if (GetFileNameFromPath(ref _filesToProcess[0], ref fileName, ref pathAlone, true) == false)
                    return;

                _singleFileAnalysisImageToSave = pathAlone + fileName + "_processed.png";
                LoadImageFile(_filesToProcess[0]);
                return;
            }

            if (_targetDir == null)
            {
                OnSelTargetDir(null, null);

                if (_targetDir == null)
                {
                    MessageBox.Show("I don't know where to save the result images to", "Info needed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }     // Stack analysis

            if (_mergeToOneResultfile == true && _resultFile == null)
            {
                MessageBox.Show("You want me to save all results to one single file, but I miss the filename.\n\nPlease define the merged file name at applications menu <Configuration> and then <Settings>, first.", "Info needed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            btnBackground.Hide();
            Application.DoEvents();
            this.Cursor = Cursors.AppStarting;
            _singleFileAnalysis = false;

            for (Int32 F = 0; F < _filesToProcess.Length; F++)
            {
                if (File.Exists(_filesToProcess[F]) == true)
                {
                    if (GetFileNameFromPath(ref _filesToProcess[F], ref fileName, ref pathAlone, true) == false)
                        continue;

                    labFile.Text = "(" + (F + 1).ToString() + "|" + _filesToProcess.Length.ToString() + ")  " + fileName;

                    if (LoadImageFile(_filesToProcess[F]) == true)
                    {
                        fileTargetName = _targetDir + "\\processed_" + (F + 1).ToString() + fileName + ".png";

                        if (DoAnalyze(ref fileTargetName) == false)
                            continue;
                    }
                }
            }
            btnBackground.Show();
            _filesToProcess = null;
            this.Cursor = Cursors.Arrow;

            if (txtStream != null)
            {
                txtStream.Flush();
                txtStream.Close();
                txtStream = null;
            }
        }

        private bool LoadImageFile(String imageFile)
        {
            try
            {
                if (_oriImage != null)
                    _oriImage.Dispose();

                _oriImage = null;
                _oriImage = new Bitmap(imageFile);

                if (_vesselAnalysis != null)
                    _vesselAnalysis.Dispose();

                _vesselAnalysis = null;
                picBox.Image = _oriImage;
                Application.DoEvents();
                return true;
            }
            catch
            {
                return false;
            }
        }
        */
        /*
         * Hier wird zuerst geschaut, ob schon eine Analyse gemacht wurde.
         * Der Path von dem Bild + Bildname wird der Methode DoAnalyze als Referenz übergeben.
         */
        /*
        private void OnDoEarlyWoodVessels(Object sender, EventArgs e)
        {
            String fileToSave = null;
            if (_singleFileAnalysis == true)
            {
                fileToSave = _singleFileAnalysisImageToSave;

                if (File.Exists(fileToSave) == true)
                    File.Delete(fileToSave);
            }
            //picAbout.Hide();
            //btnBackground.Hide();
            Application.DoEvents();
            DoAnalyze(ref fileToSave);
            //picAbout.Show();
            //btnBackground.Show();
            _vesselAnalysisDone = true;
            Application.DoEvents();
        }
        */

        //protected bool DoAnalyze(ref String bmpToSave)
        protected bool DoAnalyze(Image<Gray, UInt16> imageToAnalyze)
        {
            if (_vesselsFound != null)
            {
                for (Int32 V = 0; V < _vesselsFound.Length; V++)
                    _vesselsFound[V].Dispose();
            }
            _vesselsFound = null;

            if (_vesselAnalysis != null)
                _vesselAnalysis.Dispose();

            _vesselAnalysis = null;

            //this.Text = "ElmiWood-> " + bmpToSave;
            /*
            //labOutPut.Text = "Start investigation: Generate gray scaled image...";
            Console.WriteLine("Start investigation: Generate gray scaled image...");
            Application.DoEvents();
            _grayImage = new Image<Gray, byte>(_oriImage);
            picBox.Image = _grayImage.ToBitmap();

            picBox.Image = _grayImage.ToBitmap();
            picBox.Width = _grayImage.Width;
            picBox.Height = _grayImage.Height;
            */
            //labOutPut.Text = "Start investigation: Generate gray scaled image...done";
            Application.DoEvents();

            if (_imgProcessor == null)
                _imgProcessor = new vesselDetectorClass(this);

            if (_imgProcessor.DoThresholding(ref _vesselsFound) == false)
            {
                this.Text = "ElmiWood";
                return false;
            }

            //labOutPut.Text = "Year ring detection: Cluster analysis running...";
            Console.WriteLine("Year ring detection: Cluster analysis running...");
            Application.DoEvents();

            _vesselAnalysis = new VesselAnalysisClass(this, ref _vesselsFound);
            _vesselAnalysis.CalculateStandardValues();
            Double dHorMultiplicator = (Double)numHorDist.Value;
            Double dVerMultiplicator = (Double)numVerDist.Value;
            _vesselAnalysis.ClusterVessels(dHorMultiplicator, dVerMultiplicator);
            _vesselAnalysis.GetResult(ref _normalizedVesselGeometry);

            if (_vesselAnalysis._vesselCluster == null)
            {
                //labOutPut.Text = "No clusters found.";
                Console.WriteLine("No clusters found.");
                this.Text = "ElmiWood";
                return false;
            }
            else
            {
                Int32 safeClusters = 0;
                Int32 unsafeClusters = 0;
                _vesselAnalysis.GetAmountClustersFound(ref safeClusters, ref unsafeClusters);

                if (unsafeClusters != 0)
                {
                    //labOutPut.Text = safeClusters.ToString() + " safe clusters and " + unsafeClusters.ToString() + " unsafe items found.";
                    Console.WriteLine(safeClusters.ToString() + " safe clusters and " + unsafeClusters.ToString() + " unsafe items found.");
                }    
                else {
                    //labOutPut.Text = safeClusters.ToString() + " safe clusters found";
                    Console.WriteLine(safeClusters.ToString() + " safe clusters found");
                }
            }

            if (bmpToSave != null && _singleFileAnalysis != true)  // Batch processing, only.
            {
                CompileResultImage(bmpToSave);
                //labOutPut.Text = "Saving result image...done";
                Console.WriteLine("Saving result image...done");
                Application.DoEvents();
            }

            picBox.Image = _oriImage;
     

            if (_imgProcessor != null)
                _imgProcessor.Dispose();

            _imgProcessor = null;

            if (_vesselsFound == null)
            {
                //trkArea.Hide();
                //  labAreaThreshold.Hide();
                //panClouding.Hide();
                //btnSaveImage.Hide();
                this.Text = "ElmiWood";
                return false;
            }

            if (_singleFileAnalysis == true)
            {
                // trkArea.Show();
                //panClouding.Show();
                // labAreaThreshold.Show();
                //btnSaveImage.Show();
                Int32 minimumValue = Convert.ToInt32(_normalizedVesselGeometry.meanArea - _normalizedVesselGeometry.stdArea);
                //trkArea.Minimum = minimumValue;
                //trkArea.Maximum = Convert.ToInt32(_normalizedVesselGeometry.meanArea + _normalizedVesselGeometry.stdArea);
                //trkArea.Value = minimumValue;
                // labAreaThreshold.Text="100%";
                this.Refresh();
            }
            SaveNumericalResults(bmpToSave);
            return true;
        }

        protected void CompileResultImage(String bmpFileToSave)
        {
            if (File.Exists(bmpFileToSave) == true)
                File.Delete(bmpFileToSave);

            Bitmap bmpDisplay = new Bitmap(_oriImage);
            Graphics gr = Graphics.FromImage(bmpDisplay);

            gr.SmoothingMode = SmoothingMode.AntiAlias;
            DrawVesselInformation(gr);
            gr.Dispose();
            bmpDisplay.Save(bmpFileToSave, System.Drawing.Imaging.ImageFormat.Png);
            bmpDisplay.Dispose();
        }

        public bool GetFileNameFromPath(ref String FilePath, ref String FileName, ref String pathOfFile, bool CutExtensionFromFileName)
        {
            String[] FileElements = null;
            try
            {
                String[] Splitter = { "\\", "/" };
                String[] PathElements = FilePath.Split(Splitter, StringSplitOptions.None);

                //---- Im letzten Element befindet sich der Dateiname mit Extension. Nun entweder mit oder ohne Extension zurückliefern
                if (CutExtensionFromFileName == false)
                    FileName = PathElements[PathElements.Length - 1];
                else
                {
                    String[] FileSplitter = { "." };
                    FileElements = PathElements[PathElements.Length - 1].Split(FileSplitter, StringSplitOptions.None);
                    if (FileElements != null)
                    {
                        if (FileElements.Length > 0)
                            FileName = FileElements[0];
                        else
                            FileName = PathElements[PathElements.Length - 1];
                    }
                    else
                        FileName = PathElements[PathElements.Length - 1];
                }
                for (Int32 P = 0; P < PathElements.Length - 1; P++)
                    pathOfFile += PathElements[P] + "\\";
            }
            catch
            {
                return false;
            }
            return true;
        }
        /*
        private void OnSettings(Object sender, EventArgs e)
        {
            DlgSettings dlgSettings = new DlgSettings(_resultFile, _mergeToOneResultfile, _neighborSearchHor, _neighborSearchVer);

            if (dlgSettings.ShowDialog() != DialogResult.OK)
                return;

            _resultFile = dlgSettings._resultFile;
            _mergeToOneResultfile = dlgSettings._mergeToOneResultfile;
            _neighborSearchHor = dlgSettings._neighborSearchHor;
            _neighborSearchVer = dlgSettings._neighborSearchVer;
            _settingsInAction = true;
            numHorDist.Value = (Decimal)_neighborSearchHor;
            numVerDist.Value = (Decimal)_neighborSearchVer;
            Application.DoEvents();
            _settingsInAction = false;
            OnCloudingHorDist(null, null);
        }

        private void OnSelTargetDir(Object sender, EventArgs e)
        {
            if (dlgFolder.ShowDialog() != DialogResult.OK)
                return;

            _targetDir = dlgFolder.SelectedPath;
        }

        private void OnShowStochastics(Object sender, EventArgs e)
        {
            DlgShowStochastics oDlg = new DlgShowStochastics();
            oDlg.AddData(_normalizedVesselGeometry);
            oDlg.ShowDialog();
        }
        */
        protected void DrawClusterVessel(Graphics oDc, Int32 vesselCluster, Int32 vesselId, Color colVessel, Color textCol, StringFormat sf, Font textFont)
        {
            String outText = null;
            Int32 X, Y, W, H;
            SolidBrush brushText = null;
            Double OrbitalWidth = 0.0;
            Double OrbitalHeight = 0.0;

            DrawContour(oDc, _vesselAnalysis._vesselCluster[vesselCluster].vessels[vesselId], colVessel);

            _vesselAnalysis.GetOrbital(vesselCluster, vesselId, ref OrbitalWidth, ref OrbitalHeight);
            X = (Int32)(_vesselAnalysis._vesselCluster[vesselCluster].vessels[vesselId]._gravCenter.X - (OrbitalWidth / 2.0));
            Y = (Int32)(_vesselAnalysis._vesselCluster[vesselCluster].vessels[vesselId]._gravCenter.Y - (OrbitalHeight / 2.0));
            W = (Int32)(OrbitalWidth);
            H = (Int32)(OrbitalHeight);

            try
            {
                if (chbShowOrbitals.Checked == true)
                    oDc.DrawRectangle(Pens.Red, X, Y, W, H);

                brushText = new SolidBrush(textCol);
                outText = "Sc:" + vesselCluster.ToString() + " /" + vesselId.ToString();
                oDc.DrawString(outText, textFont, brushText, _vesselAnalysis._vesselCluster[vesselCluster].vessels[vesselId]._gravCenter.X, _vesselAnalysis._vesselCluster[vesselCluster].vessels[vesselId]._gravCenter.Y, sf);
                brushText.Dispose();
            }
            catch
            {
            }
        }

        private void DrawVesselInformation1(Graphics oDc)
        {
            Color clusCol = Color.Black;
            Color textCol = Color.Black;
            String outText = null;
            Font oFont = new Font("Arial", 10);
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            SolidBrush brushText = new SolidBrush(textCol);

            if (_vesselAnalysis == null)
                return;

            if (_vesselAnalysis._vesselCluster == null)
                return;

            for (Int32 V = 0; V < _vesselAnalysis._vesselInTissue.Length; V++)    // All vessels
            {
                for (Int32 vC = 0; vC < _vesselAnalysis._vesselCluster.Length; vC++)      // Something from interest?
                {
                    for (Int32 vt = 0; vt < _vesselAnalysis._vesselCluster[vC].vessels.Length; vt++)   // Have a look to the clustered vessels
                    {
                        if (_vesselAnalysis._vesselInTissue[V]._gravCenter.X == _vesselAnalysis._vesselCluster[vC].vessels[vt]._gravCenter.X && _vesselAnalysis._vesselInTissue[V]._gravCenter.Y == _vesselAnalysis._vesselCluster[vC].vessels[vt]._gravCenter.Y)    // Thats the one we look at in tissue
                        {
                            DrawContour(oDc, _vesselAnalysis._vesselCluster[vC].vessels[vt], Color.Blue);
                            outText = "Sc:" + vC.ToString() + " /" + V.ToString();
                            oDc.DrawString(outText, oFont, brushText, _vesselAnalysis._vesselInTissue[V]._gravCenter.X, _vesselAnalysis._vesselInTissue[V]._gravCenter.Y, sf);
                        }
                    }
                }
            }
        }

        private void DrawVesselInformation(Graphics oDc)
        {
            Color clusCol = Color.Black;
            Color textCol = Color.Black;

            if (_vesselAnalysis == null)
                return;

            if (_vesselAnalysis._vesselCluster == null)
                return;

            Font oFont = new Font("Arial", 10);
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            Int32 colCount = 0;

            for (Int32 vC = 0; vC < _vesselAnalysis._vesselCluster.Length; vC++)
            {
                #region trusted clusters
                if (_vesselAnalysis._vesselCluster[vC].saveCluster == true)
                {
                    GetClusterColor(colCount, ref clusCol, ref textCol);

                    for (Int32 V = 0; V < _vesselAnalysis._vesselCluster[vC].vessels.Length; V++)
                        DrawClusterVessel(oDc, vC, V, clusCol, textCol, sf, oFont);

                    colCount++;

                    if (colCount > 7)
                        colCount = 0;
                }
                #endregion trusted clusters

                #region mistrusted clusters
                if (_vesselAnalysis._vesselCluster[vC].mistrustedCluster == true && chmMistrustedClusters.Checked == true)
                {
                    GetMistrustedClusterColor(colCount, ref clusCol, ref textCol);
                    for (Int32 V = 0; V < _vesselAnalysis._vesselCluster[vC].vessels.Length; V++)
                        DrawClusterVessel(oDc, vC, V, clusCol, textCol, sf, oFont);
                }
                #endregion

                #region unsafe clusters
                if (_vesselAnalysis._vesselCluster[vC].saveCluster == false && chbShowUnsafeClusters.Checked == true)
                {
                    GetUnsafeClusterColor(colCount, ref clusCol, ref textCol);

                    for (Int32 V = 0; V < _vesselAnalysis._vesselCluster[vC].vessels.Length; V++)
                        DrawClusterVessel(oDc, vC, V, clusCol, textCol, sf, oFont);
                }
                #endregion
            }
            oFont.Dispose();
        }

        private void DrawSearchPattern(Graphics oDc, VesselAnalysisClass.spotField[] testSpots)
        {
            SolidBrush spotBrush = new SolidBrush(Color.FromArgb(30, Color.Green));
            SolidBrush targetingBrush = new SolidBrush(Color.FromArgb(30, Color.Red));

            if (testSpots == null)
                return;

            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            String outText = null;
            Font oFont = new Font("Arial", 10);
            Int32 width;
            Int32 height;

            for (Int32 S = 1; S < testSpots.Length; S++)
            {
                width = testSpots[S].X1 - testSpots[S].Xo;
                height = testSpots[S].Y1 - testSpots[S].Yo;
                oDc.DrawRectangle(Pens.Black, testSpots[S].Xo, testSpots[S].Yo, width, height);
                oDc.FillRectangle(spotBrush, testSpots[S].Xo, testSpots[S].Yo, width, height);
                outText = S.ToString();
                oDc.DrawString(outText, oFont, Brushes.Black, testSpots[S].Xc, testSpots[S].Yc, sf);
            }
            Int32 widthEllipse = testSpots[0].X1 - testSpots[0].Xo;
            Int32 heightEllipse = testSpots[0].Y1 - testSpots[0].Yo;
            Int32 halfWidthEllipse = widthEllipse / 2;
            Int32 halfHeightEllipse = heightEllipse / 2;

            oDc.FillEllipse(targetingBrush, testSpots[0].Xc - halfWidthEllipse, testSpots[0].Yc - halfHeightEllipse, widthEllipse, heightEllipse);
            oDc.DrawLine(Pens.Black, testSpots[0].Xc, testSpots[0].Yc - halfHeightEllipse, testSpots[0].Xc, testSpots[0].Yc + halfHeightEllipse);
            oDc.DrawLine(Pens.Black, testSpots[0].Xc - halfWidthEllipse, testSpots[0].Yc, testSpots[0].Xc + halfWidthEllipse, testSpots[0].Yc);
            spotBrush.Dispose();
        }

        private void OnPaint(Object sender, PaintEventArgs e)
        {
            DrawVesselInformation(e.Graphics);
            Graphics picGraph = picBox.CreateGraphics();
            DrawVesselInformation(picGraph);
            picGraph.Dispose();
        }
        private void DrawContour(Graphics dc, VesselClass vessel, Color clusCol)
        {
            if (vessel._area <= 5)
                return;

            Int32 elements = vessel._contour.Count();
            Int32 X = vessel._contour[0].X;
            Int32 Y = vessel._contour[0].Y;
            GraphicsPath NewPth = new GraphicsPath();
            Point[] pth = new Point[elements];
            Color brushColor = Color.Aquamarine;
            Int32[] x = new Int32[4];
            brushColor = clusCol;

            Int32 halfWidth = vessel._boundingBox.Width / 2;
            Int32 halfHeight = vessel._boundingBox.Height / 2;
            Int32 xC = vessel._boundingBox.X + halfWidth;
            Int32 yC = vessel._boundingBox.Y + halfHeight;

            SolidBrush vesselFill = new SolidBrush(brushColor);

            pth[0].X = X;
            pth[0].Y = Y;

            //dc.DrawLine(Pens.White, xC-10, yC-10, xC+10, yC+10);
            //dc.DrawLine(Pens.Black, xC-10, yC+10, xC+10, yC-10);


            for (Int32 P = 1; P < elements; P++)
            {
                dc.DrawLine(Pens.Blue, X, Y, vessel._contour[P].X, vessel._contour[P].Y);
                X = vessel._contour[P].X;
                Y = vessel._contour[P].Y;
                pth[P].X = X;
                pth[P].Y = Y;
            }
            dc.DrawLine(Pens.Blue, X, Y, vessel._contour[0].X, vessel._contour[0].Y);
            NewPth.AddClosedCurve(pth);
            dc.FillPath(vesselFill, NewPth);
            vesselFill.Dispose();
        }

        private void OnScrollArea(Object sender, EventArgs e)
        {
            Double dMax = (Double)(trkArea.Maximum);
            Double dMin = (Double)(trkArea.Minimum);

            Double val = (1.0 - ((trkArea.Value) - dMin) / (dMax - dMin)) * 100.0;

            //labAreaThreshold.Text= val.ToString("##0.0") +  "%";
        }

        private void OnScrollerReleased(Object sender, MouseEventArgs e)
        {
            if (_scrollerInAction == true)
                this.Refresh();

            _scrollerInAction = false;
        }

        private void OnScrollerPressedx(Object sender, MouseEventArgs e)
        {
            _scrollerInAction = true;
        }

        private void GetClusterColor(Int32 id, ref Color clusCol, ref Color textCol)
        {
            /*Random rnd = new Random();

            if(id == 0 )
              clusCol = Color.Blue;

            Int32 RedChannel = rnd.Next(255);
            Int32 GreenChannel = rnd.Next(255);
            Int32 BlueChannel = rnd.Next(255);*/

            Int32 RedChannel = 0;
            Int32 GreenChannel = 0;
            Int32 BlueChannel = 0;

            switch (id)
            {
                case 0: RedChannel = 0; GreenChannel = 0; BlueChannel = 255; textCol = Color.White; break;
                case 1: RedChannel = 0; GreenChannel = 255; BlueChannel = 0; textCol = Color.Black; break;
                case 2: RedChannel = 0; GreenChannel = 255; BlueChannel = 255; textCol = Color.Black; break;
                case 3: RedChannel = 255; GreenChannel = 255; BlueChannel = 0; textCol = Color.Black; break;
                case 4: RedChannel = 0; GreenChannel = 0; BlueChannel = 128; textCol = Color.White; break;
                case 5: RedChannel = 0; GreenChannel = 128; BlueChannel = 0; textCol = Color.White; break;
                case 6: RedChannel = 0; GreenChannel = 128; BlueChannel = 128; textCol = Color.White; break;
                case 7: RedChannel = 128; GreenChannel = 128; BlueChannel = 0; textCol = Color.White; break;
                default: RedChannel = 255; GreenChannel = 0; BlueChannel = 0; textCol = Color.White; break;
            }
            clusCol = Color.FromArgb(RedChannel, GreenChannel, BlueChannel);
        }

        private void GetMistrustedClusterColor(Int32 id, ref Color clusCol, ref Color textCol)
        {
            clusCol = Color.Pink;
            textCol = Color.Green;
        }

        private void GetUnsafeClusterColor(Int32 id, ref Color clusCol, ref Color textCol)
        {
            clusCol = Color.Red;
            textCol = Color.Yellow;
        }

        private void OnCloudingVerDist(Object sender, EventArgs e)
        {
            if (_vesselAnalysis == null)
                return;

            if (_settingsInAction == true)
                return;

            Double dHorMultiplicator = (Double)numHorDist.Value;
            Double dVerMultiplicator = (Double)numVerDist.Value;
            _vesselAnalysis.ClusterVessels(dHorMultiplicator, dVerMultiplicator);

            if (_vesselAnalysis._vesselCluster == null)
            {
                labOutPut.Text = "No clusters found.";
                return;
            }
            labOutPut.Text = _vesselAnalysis._vesselCluster.Length.ToString() + " clusters found.";
            this.Refresh();
        }

        private void OnCloudingHorDist(Object sender, EventArgs e)
        {
            if (_vesselAnalysis == null)
                return;

            if (_settingsInAction == true)
                return;

            _neighborSearchHor = (Double)numHorDist.Value;
            _neighborSearchVer = (Double)numVerDist.Value;
            _vesselAnalysis.ClusterVessels(_neighborSearchHor, _neighborSearchVer);
            if (_vesselAnalysis._vesselCluster == null)
            {
                labOutPut.Text = "No clusters found.";
                return;
            }

            labOutPut.Text = _vesselAnalysis._vesselCluster.Length.ToString() + " clusters found.";
            this.Refresh();
        }

        private void OnSaveResultImage(Object sender, EventArgs e)
        {
            labOutPut.Text = "Saving image...";
            Application.DoEvents();
            CompileResultImage(_singleFileAnalysisImageToSave);
            labOutPut.Text = "Saving image...done";
        }

        private void OnAboutDlg(Object sender, EventArgs e)
        {
            DlgAbout aboutDlg = new DlgAbout();
            aboutDlg.Show();
        }

        private void OnShowUnsafeClusters(Object sender, EventArgs e)
        {
            picBox.Refresh();
        }

        private void SaveNumericalResults(String bmpToSave)
        {
            String pathOfFile = null;
            String fileName = null;
            String resultFile = null;
            GetFileNameFromPath(ref bmpToSave, ref fileName, ref pathOfFile, true);
            fileName += ".txt";

            if (_mergeToOneResultfile == false || _singleFileAnalysis == true)  // merging single files is nonsense
            {
                resultFile = pathOfFile + fileName;

                if (File.Exists(resultFile) == true)
                    File.Delete(resultFile);
            }
            else
                resultFile = _resultFile;

            if (txtStream == null)
                txtStream = File.CreateText(resultFile);

            WriteResults(bmpToSave, txtStream);

            if (_mergeToOneResultfile == false || _singleFileAnalysis == true)  // merging single files is nonsense) 
            {
                txtStream.Flush();
                txtStream.Close();
                txtStream = null;
            }
        }

        String SEPARATOR = "\t";

        protected void WriteResults(String bmpToSave, StreamWriter txtStream)
        {
            StochasticsClass stoch = new StochasticsClass();
            ResultLine(txtStream, "Wood sample:" + SEPARATOR + bmpToSave);

            if (_vesselAnalysis._vesselCluster == null)
            {
                ResultLine(txtStream, "Year rings detected:" + SEPARATOR + "0");
                return;
            }

            if (_vesselAnalysis._vesselInTissue == null)
            {
                ResultLine(txtStream, "Vessels detected  (total):" + SEPARATOR + "0");
                return;
            }
            Int32 yearRingsDetected = 0;

            for (Int32 cV = 0; cV < _vesselAnalysis._vesselCluster.Length; cV++)
            {
                if (_vesselAnalysis._vesselCluster[cV].saveCluster == true && _vesselAnalysis._vesselCluster[cV].mistrustedCluster == false)
                    yearRingsDetected++;
            }

            ResultLine(txtStream, "Year rings detected:" + SEPARATOR + yearRingsDetected);
            ResultLine(txtStream, "Vessels detected (total):" + SEPARATOR + _vesselAnalysis._vesselInTissue.Length);
            ResultLine(txtStream, "Mean area (total):" + SEPARATOR + _vesselAnalysis._normalizedVesselGeometry.meanArea.ToString() + SEPARATOR + "+-" + SEPARATOR + _vesselAnalysis._normalizedVesselGeometry.stdArea.ToString());

            Int32 yearRingCounter = 1;
            Double meanArea = 0;
            Double stdArea = 0;
            ResultLine(txtStream, "Tree ring" + SEPARATOR + "No. of vessels" + SEPARATOR + "Mean vessel area" + SEPARATOR + "Stdv mean vessel area");
            String outLine = null;

            for (Int32 cV = 0; cV < _vesselAnalysis._vesselCluster.Length; cV++)
            {
                if (_vesselAnalysis._vesselCluster[cV].saveCluster == true && _vesselAnalysis._vesselCluster[cV].mistrustedCluster == false)
                {
                    outLine = yearRingCounter.ToString() + SEPARATOR + _vesselAnalysis._vesselCluster[cV].vessels.Length.ToString() + SEPARATOR;

                    if (stoch.StdevValue(_vesselAnalysis._vesselCluster[cV].vessels, ref meanArea, ref stdArea) == true)
                        outLine += meanArea.ToString() + SEPARATOR + stdArea.ToString();

                    ResultLine(txtStream, outLine);
                    yearRingCounter++;
                }
            }
            ResultLine(txtStream, " ");
        }

        protected void ResultLine(StreamWriter txtStream, String lineToFile)
        {
            txtStream.WriteLine(lineToFile);
        }

        private void OnShown(Object sender, EventArgs e)
        {
            numHorDist.Value = (Decimal)_neighborSearchHor;
            numVerDist.Value = (Decimal)_neighborSearchVer;
        }

        #region Configuration file ------------------------------------------------------------------------------------------------

        private void ReadConfigurationFile()
        {
            String configFile = Environment.CurrentDirectory + "\\cawa.config";

            if (File.Exists(configFile) == false)
            {
                _mergeToOneResultfile = false;
                _neighborSearchHor = 1.7;
                _neighborSearchVer = 1.7;
                return;
            }
            StreamReader objReader = new StreamReader(configFile);
            String InfoLine = null;

            for (; ; )
            {
                InfoLine = objReader.ReadLine();

                if (InfoLine == null)
                    break;

                //-------------------------------------------------------------------------------------------------------------

                if (InfoLine.StartsWith("CAWA->NEAR_NEIGHBOR_ORBITAL->WIDTH->"))
                    GetCharacteristicsInformation("CAWA->NEAR_NEIGHBOR_ORBITAL->WIDTH->", ref InfoLine, ref _neighborSearchHor);
                if (InfoLine.StartsWith("CAWA->NEAR_NEIGHBOR_ORBITAL->HEIGHT->"))
                    GetCharacteristicsInformation("CAWA->NEAR_NEIGHBOR_ORBITAL->HEIGHT->", ref InfoLine, ref _neighborSearchVer);
                if (InfoLine.StartsWith("CAWA->MERGED_RESULTFILE->"))
                    GetCharacteristicsInformation("CAWA->MERGED_RESULTFILE->", ref InfoLine, ref _mergeToOneResultfile);
                if (InfoLine.StartsWith("CAWA->MERGED_RESULTFILE_NAME->"))
                    GetCharacteristicsInformation("CAWA->MERGED_RESULTFILE_NAME->", ref InfoLine, ref _resultFile);
            }
            objReader.Close();
            objReader.Dispose();
            objReader = null;
        }

        private void WriteConfigurationFile()
        {
            String configFile = Environment.CurrentDirectory + "\\cawa.config";
            if (File.Exists(configFile) == true)
                File.Delete(configFile);

            StreamWriter objWriter = File.CreateText(configFile);
            WriteGlobalConfigurationHeader(objWriter);
            WriteConfigurationParameters(objWriter);

            objWriter.Flush();
            objWriter.Close();
            objWriter.Dispose();
            objWriter = null;
        }

        protected bool WriteGlobalConfigurationHeader(StreamWriter objWriter)
        {
            objWriter.WriteLine(@"/----------------------------------------------------------------------------\");
            objWriter.WriteLine(@"|  CAWA  - Sibling of Elmi5                                                  |");
            objWriter.WriteLine(@"|  Expert system for Light Microscopy Generation 5                           |");
            objWriter.WriteLine(@"|                                                                            |");
            objWriter.WriteLine(@"|  Main configuration file.                                                  |");
            objWriter.WriteLine(@"|                  Major configuration settings for the main program         |");
            objWriter.WriteLine(@"|                                                                            |");
            objWriter.WriteLine(@"|  Please do not edit following lines without knowing what you do.           |");
            objWriter.WriteLine(@"\----------------------------------------------------------------------------/");
            return true;
        }

        protected bool WriteConfigurationParameters(StreamWriter objWriter)
        {
            objWriter.WriteLine(@"CAWA->NEAR_NEIGHBOR_ORBITAL->WIDTH->" + _neighborSearchHor.ToString());
            objWriter.WriteLine(@"CAWA->NEAR_NEIGHBOR_ORBITAL->HEIGHT->" + _neighborSearchVer.ToString());
            objWriter.WriteLine(@"CAWA->MERGED_RESULTFILE->" + _mergeToOneResultfile.ToString());

            if (_mergeToOneResultfile == true)
                objWriter.WriteLine(@"CAWA->MERGED_RESULTFILE_NAME->" + _resultFile);

            return true;
        }

        #region GetCharacteristics (borrowed from Elmi5)

        public bool GetCharacteristicsInformation(String Detector, ref String InfoLine, ref bool Result)
        {
            try
            {
                String SubInfo = InfoLine.Remove(0, Detector.Length);

                if (SubInfo == "")
                    return false;

                Result = Convert.ToBoolean(SubInfo);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool GetCharacteristicsInformation(String Detector, ref String InfoLine, ref Double Result)
        {
            try
            {
                String SubInfo = InfoLine.Remove(0, Detector.Length);

                if (SubInfo == "")
                    return false;

                Result = Convert.ToDouble(SubInfo);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool GetCharacteristicsInformation(String Detector, ref String InfoLine, ref String Result)
        {
            try
            {
                String SubInfo = InfoLine.Remove(0, Detector.Length);

                if (SubInfo == "")
                    return false;

                Result = SubInfo;
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion GetCharacteristics (borrowed from Elmi5)

        #endregion (configuration file)

        private void OnCLosing(Object sender, FormClosingEventArgs e)
        {
            WriteConfigurationFile();
        }


        /*
                private void OnImageBoxClick(object sender, EventArgs e)
                {
                    if (_vesselAnalysisDone == false)
                        return;

                    MouseEventArgs mouseArgs = (MouseEventArgs)e;

                    Int32 cursorLocationX;
                    Int32 cursorLocationY;
                    Double gravCenterLocationX;
                    Double gravCenterLocationY;
                    Double zoomFactor = picBox.ZoomScale;

                    cursorLocationX = mouseArgs.X;
                    cursorLocationY = mouseArgs.Y;

                    if (hitFirstVessel == false)
                    {
                        for (Int32 vC = 0; vC < _vesselAnalysis._vesselCluster.Length; vC++)
                        {
                            for (Int32 vT = 0; vT < _vesselAnalysis._vesselCluster[vC].vessels.Length; vT++)
                            {
                                gravCenterLocationX = _vesselAnalysis._vesselCluster[vC].vessels[vT]._gravCenter.X * zoomFactor;
                                gravCenterLocationY = _vesselAnalysis._vesselCluster[vC].vessels[vT]._gravCenter.Y * zoomFactor;

                                if (cursorLocationX >= gravCenterLocationX - 15 && cursorLocationX <= gravCenterLocationX + 15 &&
                                    cursorLocationY <= gravCenterLocationY + 15 && cursorLocationY >= gravCenterLocationY - 15)
                                {
                                    vesselToMerge = _vesselAnalysis._vesselCluster[vC].vessels[vT];
                                    vesselToMergeNumber = vT;
                                    vesselToMergeClusterNumber = vC;
                                    hitFirstVessel = true;
                                    labOutPut.Text = "Vessel no. " + vT + " of Cluster no. " + vC + " selected.";
                                    Application.DoEvents();
                                    return;
                                }
                            }
                        }
                    }
                    if (hitFirstVessel == true && hitSecondVessel == false)
                    {
                        for (int vC = 0; vC < _vesselAnalysis._vesselCluster.Length; vC++)
                        {                
                            for (int vT = 0; vT < _vesselAnalysis._vesselCluster[vC].vessels.Length; vT++)
                            {
                                gravCenterLocationX = _vesselAnalysis._vesselCluster[vC].vessels[vT]._gravCenter.X * zoomFactor;
                                gravCenterLocationY = _vesselAnalysis._vesselCluster[vC].vessels[vT]._gravCenter.Y * zoomFactor;

                                if (cursorLocationX >= gravCenterLocationX - 15 && cursorLocationX <= gravCenterLocationX + 15 &&
                                    cursorLocationY <= gravCenterLocationY + 15 && cursorLocationY >= gravCenterLocationY - 15)
                                {
                                    if (vesselToMergeClusterNumber == vC)
                                    {
                                        vesselToMerge = null;
                                        hitFirstVessel = false;

                                        Int32 safeClusters = 0;
                                        Int32 unsafeClusters = 0;
                                        _vesselAnalysis.GetAmountClustersFound(ref safeClusters, ref unsafeClusters);

                                        if (unsafeClusters != 0)
                                            labOutPut.Text = "Operation canceled! Please select a vessel from a different cluster than the first one! " + safeClusters.ToString() + " safe clusters and " + unsafeClusters.ToString() + " unsafe items found.";
                                        else
                                            labOutPut.Text = "Operation canceled! Please select a different vessel than the first one! " + safeClusters.ToString() + " safe clusters found";
                                        Application.DoEvents();
                                        return;
                                    }
                                    else
                                    {
                                        MergeButton.Show();
                                        newClusterNumber = vC;
                                        hitSecondVessel = true;
                                        labOutPut.Text = "Vessel no. " + vesselToMergeNumber + " of Cluster no. " + vesselToMergeClusterNumber + " and Cluster no. " + newClusterNumber + " selected. For adding this vessel to the new cluster, click the Button 'Merge'.";
                                        Application.DoEvents();
                                    }
                                }
                            }
                        }
                    }
                }

                private void OnImageBoxDoubleClick(object sender, EventArgs e)
                {
                    if (_vesselAnalysisDone == false)
                        return;

                    MergeButton.Hide();
                    vesselToMerge = null;
                    hitFirstVessel = false;
                    hitSecondVessel = false;

                    Int32 safeClusters = 0;
                    Int32 unsafeClusters = 0;
                    _vesselAnalysis.GetAmountClustersFound(ref safeClusters, ref unsafeClusters);

                    if (unsafeClusters != 0)
                        labOutPut.Text = "Selection canceled! " + safeClusters.ToString() + " safe clusters and " + unsafeClusters.ToString() + " unsafe items found.";
                    else
                        labOutPut.Text = "Selection canceled! " + safeClusters.ToString() + " safe clusters found";
                    Application.DoEvents();
                }
        */

        private void OnClickMergeButton(object sender, EventArgs e)
        {
            if (hitFirstVessel == true && hitSecondVessel == true)
            {
                AddVesselToNewCluster();
                DeleteVesselFromOldCluster();

                picBox.Refresh();

                Int32 safeClusters = 0;
                Int32 unsafeClusters = 0;
                _vesselAnalysis.GetAmountClustersFound(ref safeClusters, ref unsafeClusters);

                if (unsafeClusters != 0)
                    labOutPut.Text = "Merging successfull! " + safeClusters.ToString() + " safe clusters and " + unsafeClusters.ToString() + " unsafe items found.";
                else
                    labOutPut.Text = "Merging successfull! " + safeClusters.ToString() + " safe clusters found.";
                Application.DoEvents();

                hitFirstVessel = false;
                hitSecondVessel = false;
                MergeButton.Hide();
            }
        }

        protected void AddVesselToNewCluster()
        {
            VesselClass[] tmp = null;
            Int32 elements = _vesselAnalysis._vesselCluster[newClusterNumber].vessels.Length;

            tmp = new VesselClass[elements + 1];

            for (Int32 V = 0; V < elements; V++)
                tmp[V] = _vesselAnalysis._vesselCluster[newClusterNumber].vessels[V];

            tmp[elements] = vesselToMerge;
            _vesselAnalysis._vesselCluster[newClusterNumber].vessels = tmp;
        }

        protected void DeleteVesselFromOldCluster()
        {
            VesselClass[] tmp = null;
            Int32 counter = 0;
            Int32 elements = _vesselAnalysis._vesselCluster[vesselToMergeClusterNumber].vessels.Length;

            tmp = new VesselClass[elements - 1];

            for (Int32 V = 0; V < elements; V++)
                if (V != vesselToMergeNumber)
                {
                    tmp[counter] = _vesselAnalysis._vesselCluster[vesselToMergeClusterNumber].vessels[V];
                    counter++;
                }

            if (tmp.Length == 0)
            {
                DeleteCluster();
            }
            else
                _vesselAnalysis._vesselCluster[vesselToMergeClusterNumber].vessels = tmp;
        }

        protected void DeleteCluster()
        {
            VesselAnalysisClass.VesselCluster[] tmp = null;
            Int32 counter = 0;
            Int32 TotalClusterNumber = _vesselAnalysis._vesselCluster.Length;

            tmp = new VesselAnalysisClass.VesselCluster[TotalClusterNumber - 1];

            for (int vC = 0; vC < TotalClusterNumber; vC++)
                if (vC != vesselToMergeClusterNumber)
                {
                    tmp[counter] = _vesselAnalysis._vesselCluster[vC];
                    counter++;
                }
            _vesselAnalysis._vesselCluster = tmp;
        }

        private void OnScroll(object sender, ScrollEventArgs e)
        {
            picBox.Invalidate();
        }

        private void OnMouseClickInPicWood(object sender, MouseEventArgs e)
        {
            if (_vesselAnalysisDone == false)
                return;

            MouseEventArgs mouseArgs = (MouseEventArgs)e;

            Int32 cursorLocationX;
            Int32 cursorLocationY;
            Double gravCenterLocationX;
            Double gravCenterLocationY;
            Double zoomFactor = 1.0; //picBox.ZoomScale;

            cursorLocationX = mouseArgs.X;
            cursorLocationY = mouseArgs.Y;

            if (hitFirstVessel == false)
            {
                for (Int32 vC = 0; vC < _vesselAnalysis._vesselCluster.Length; vC++)
                {
                    for (Int32 vT = 0; vT < _vesselAnalysis._vesselCluster[vC].vessels.Length; vT++)
                    {
                        gravCenterLocationX = _vesselAnalysis._vesselCluster[vC].vessels[vT]._gravCenter.X * zoomFactor;
                        gravCenterLocationY = _vesselAnalysis._vesselCluster[vC].vessels[vT]._gravCenter.Y * zoomFactor;

                        if (cursorLocationX >= gravCenterLocationX - 15 && cursorLocationX <= gravCenterLocationX + 15 &&
                            cursorLocationY <= gravCenterLocationY + 15 && cursorLocationY >= gravCenterLocationY - 15)
                        {
                            vesselToMerge = _vesselAnalysis._vesselCluster[vC].vessels[vT];
                            vesselToMergeNumber = vT;
                            vesselToMergeClusterNumber = vC;
                            hitFirstVessel = true;
                            labOutPut.Text = "Vessel no. " + vT + " of Cluster no. " + vC + " selected.";
                            Application.DoEvents();
                            return;
                        }
                    }
                }
            }
            if (hitFirstVessel == true && hitSecondVessel == false)
            {
                for (int vC = 0; vC < _vesselAnalysis._vesselCluster.Length; vC++)
                {
                    for (int vT = 0; vT < _vesselAnalysis._vesselCluster[vC].vessels.Length; vT++)
                    {
                        gravCenterLocationX = _vesselAnalysis._vesselCluster[vC].vessels[vT]._gravCenter.X * zoomFactor;
                        gravCenterLocationY = _vesselAnalysis._vesselCluster[vC].vessels[vT]._gravCenter.Y * zoomFactor;

                        if (cursorLocationX >= gravCenterLocationX - 15 && cursorLocationX <= gravCenterLocationX + 15 &&
                            cursorLocationY <= gravCenterLocationY + 15 && cursorLocationY >= gravCenterLocationY - 15)
                        {
                            if (vesselToMergeClusterNumber == vC)
                            {
                                vesselToMerge = null;
                                hitFirstVessel = false;

                                Int32 safeClusters = 0;
                                Int32 unsafeClusters = 0;
                                _vesselAnalysis.GetAmountClustersFound(ref safeClusters, ref unsafeClusters);

                                if (unsafeClusters != 0)
                                    labOutPut.Text = "Operation canceled! Please select a vessel from a different cluster than the first one! " + safeClusters.ToString() + " safe clusters and " + unsafeClusters.ToString() + " unsafe items found.";
                                else
                                    labOutPut.Text = "Operation canceled! Please select a different vessel than the first one! " + safeClusters.ToString() + " safe clusters found";
                                Application.DoEvents();
                                return;
                            }
                            else
                            {
                                MergeButton.Show();
                                newClusterNumber = vC;
                                hitSecondVessel = true;
                                labOutPut.Text = "Vessel no. " + vesselToMergeNumber + " of Cluster no. " + vesselToMergeClusterNumber + " and Cluster no. " + newClusterNumber + " selected. For adding this vessel to the new cluster, click the Button 'Merge'.";
                                Application.DoEvents();
                            }
                        }
                    }
                }
            }
        }

        private void OnImageBoxDoubleClick(Object sender, MouseEventArgs e)
        {
            if (_vesselAnalysisDone == false)
                return;

            MergeButton.Hide();
            vesselToMerge = null;
            hitFirstVessel = false;
            hitSecondVessel = false;

            Int32 safeClusters = 0;
            Int32 unsafeClusters = 0;
            _vesselAnalysis.GetAmountClustersFound(ref safeClusters, ref unsafeClusters);

            if (unsafeClusters != 0)
                labOutPut.Text = "Selection canceled! " + safeClusters.ToString() + " safe clusters and " + unsafeClusters.ToString() + " unsafe items found.";
            else
                labOutPut.Text = "Selection canceled! " + safeClusters.ToString() + " safe clusters found";
            Application.DoEvents();
        }
    }
}


