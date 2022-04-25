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
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using System.Drawing.Drawing2D;


namespace TelomereAnalyzer
{
    //Handling Drawing Nuclei by User
    public partial class FormTwo : Form
    {
        public Nuclei _allNuclei = null;
        //Final Images
        public Image<Bgr, byte> _NucleiImageWithAutomaticEdgesToDrawOn = null;
        public Bitmap _btmNucleiImageWithAutomaticEdges = null;
        //RawImages
        public Image<Bgr, byte> _RawNucleiImage = null;
     
        public Boolean _finishedDrawingOfOneNucleus = false;
        public Boolean _pressedBtnApply = false;
        ImageBox _imgBx;

        public FormTwo(Nuclei allNuclei, Image<Bgr, byte> rawNucleiImageNormalized)
        {
            //this.FormClosing += FormTwo_OnClosing;
            InitializeComponent();
            this._allNuclei = allNuclei;
            /*
            this._NucleiImageWithAutomaticEdgesToDrawOn = nucleiImageEdgesDetected;
            this._btmNucleiImageWithAutomaticEdges = nucleiImageEdgesDetected.ToBitmap();
            */
            this._RawNucleiImage = rawNucleiImageNormalized;
            _allNuclei.PrepareDrawingCenterPoints();
            _allNuclei.PrepareDrawingContoursByNucleus(new Bgr(Color.DarkSeaGreen));
            this._NucleiImageWithAutomaticEdgesToDrawOn = _allNuclei._imageToDrawOn;
            this._btmNucleiImageWithAutomaticEdges = _NucleiImageWithAutomaticEdgesToDrawOn.ToBitmap();

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            ShowImageOnForm(pcBxOriImage, _NucleiImageWithAutomaticEdgesToDrawOn);
            _imgBx = new ImageBox();
            pcBxOriImage.Controls.Add(_imgBx);
            //Displaying the Image in the PictureBox and aligning the ImageBox with the PictureBox on top of it
            try {
                //pcBxOriImage.BackgroundImage = image.ToBitmap();
                _imgBx.Width = _NucleiImageWithAutomaticEdgesToDrawOn.Width;
                _imgBx.Height = _NucleiImageWithAutomaticEdgesToDrawOn.Height;
                _imgBx.MaximumSize = _NucleiImageWithAutomaticEdgesToDrawOn.Size;
                _imgBx.Location = pcBxOriImage.Location;
                _imgBx.Refresh();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            _imgBx.BackColor = Color.Transparent;
            _imgBx.MouseUp += new MouseEventHandler(OnMouseUp);
            _imgBx.MouseDown += new MouseEventHandler(OnMouseDown);
            _imgBx.MouseMove += new MouseEventHandler(OnMouseMove);
            _imgBx.Paint += new PaintEventHandler(OnPaint);
            DisplayAllNucleiAsCheckboxesBeginning();
        }

        private void DisplayAllNucleiAsCheckboxesBeginning()
        {
            pnlSelectNuclei.Controls.Clear();
            for(Int32 n = 0; n < _allNuclei._LstAllNuclei.Count; n++)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Name = "chkBx" + _allNuclei._LstAllNuclei[n]._nucleusName;
                checkBox.Text = _allNuclei._LstAllNuclei[n]._nucleusName;
                checkBox.Width = (TextRenderer.MeasureText(checkBox.Text, checkBox.Font)).Width + 50;
                checkBox.Location = new Point(5, pnlSelectNuclei.Controls.Count * 20);
                //checkBox.Checked = true;
                CreateNucleiLabelOnImageBox(_allNuclei._LstAllNuclei[n]._nucleusContourPoints, _allNuclei._LstAllNuclei[n]._nucleusName);
                //dynamically adds EventHandler for each Checkbox that is dynamically created
                checkBox.CheckedChanged += new EventHandler(CheckBoxChanged);
                pnlSelectNuclei.Controls.Add(checkBox);
            }            
            foreach (Label label in _imgBx.Controls.OfType<Label>()) 
            {
                foreach(CheckBox checkBx in pnlSelectNuclei.Controls.OfType<CheckBox>())
                    if (label.Text.Equals(checkBx.Text))
                    {
                        if (label.ForeColor.ToArgb().Equals(Color.White.ToArgb()))
                            checkBx.Checked = true;
                        else
                            checkBx.Checked = false;
                    }
            }
            Refresh();
        }

        private void DisplayAddedNucleusAsCheckboxes(Nucleus nucleus)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.Name = "chkBx" + nucleus._nucleusName;
            checkBox.Text = nucleus._nucleusName;
            checkBox.Width = (TextRenderer.MeasureText(checkBox.Text, checkBox.Font)).Width + 50;
            checkBox.Location = new Point(5, pnlSelectNuclei.Controls.Count * 20);
            checkBox.Checked = true;
            CreateNucleiLabelOnImageBox(nucleus._nucleusContourPoints, nucleus._nucleusName);
            //dynamically adds EventHandler for each Checkbox that is dynamically created
            checkBox.CheckedChanged += new EventHandler(CheckBoxChanged);
            pnlSelectNuclei.Controls.Add(checkBox);
        }

        private void CreateNucleiLabelOnImageBox(PointF[] contour, String nucleusName)
        {
            //fügt jedem Nucleus ein Label hinzu --> funktioniert aber irgendwie werden die ersten beiden Nuclei übersprungen --> sie haben kein Label
            //Speichert den letzten Punkt der Konturpunkt-Liste
            PointF lastPoint = contour[contour.Length-1];
            Label label = new Label();
            label.Name = "lbl" + nucleusName;
            label.BackColor = Color.Transparent;
            label.ForeColor = Color.White;
            label.Text = nucleusName;
            label.Location = Point.Round(lastPoint);
            //panel1.Controls.Add(label);
            //pcBxOriImage.Controls.Add(label);
            _imgBx.Controls.Add(label);
            label.Show();
        }

        private void CheckBoxChanged(object sender, EventArgs e)
        {
            //Label hiding/showing if Checkbox not checked/checked
            CheckBox checkBox = (sender as CheckBox);
            foreach (Label label in _imgBx.Controls.OfType<Label>())
            {
                if (label.Text.Equals(checkBox.Text))
                {
                    if (!checkBox.Checked)
                    {
                        label.Hide();
                        label.ForeColor = Color.Red;
                    }
                    else if (checkBox.Checked)
                    {
                        label.ForeColor = Color.White;
                        label.Show();
                    }
                }
            }
            //attempt to redraw the shown Image with deselected Nuclei without their borders and redraw them when selected again
            _NucleiImageWithAutomaticEdgesToDrawOn = _RawNucleiImage;
            if (_allNuclei._LstAllNuclei != null)
            {
                for (Int32 E = 0; E < _allNuclei._LstAllNuclei.Count; E++)
                {
                    if (_allNuclei._LstAllNuclei.ElementAt(E) != null)
                    {
                        if (_allNuclei._LstAllNuclei.ElementAt(E)._nucleusContourPoints != null)
                        {      
                            foreach(CheckBox checkBx in pnlSelectNuclei.Controls.OfType<CheckBox>())
                            {
                                //If the Nuclei Name equals an checked Checkbox the Nucleus should be redrawn
                                if ( checkBx.Text.Equals(_allNuclei._LstAllNuclei.ElementAt(E)._nucleusName))
                                    {
                                        if (checkBx.Checked)
                                        {
                                            _NucleiImageWithAutomaticEdgesToDrawOn = _allNuclei.DrawContour(_NucleiImageWithAutomaticEdgesToDrawOn, _allNuclei._LstAllNuclei.ElementAt(E)._nucleusContourPoints, new Bgr(Color.DarkSeaGreen));
                                            _btmNucleiImageWithAutomaticEdges = _NucleiImageWithAutomaticEdgesToDrawOn.ToBitmap();
                                        }
                                    }   
                            }
                        }
                    }
                }
            }
            ShowImageOnForm(pcBxOriImage, _NucleiImageWithAutomaticEdgesToDrawOn);
        }

        #region Nuclei Borders drawn by User--------------------------------------------------------
        public enum MouseButtonStatus : int
        {
            UNDEFINED = 0,
            DOWN = 1,
            UP = 2
        }
        public enum MouseButton : int
        {
            UNDEFINED = 0,
            LEFT = 1,
            MIDDLE = 2,
            RIGHT = 3
        }

        public struct MouseCoordinate
        {
            public float X;
            public float Y;
        }

        MouseCoordinate[] _mouseCoordinates = null;
        PointF[] _malKurve = null;

        public struct MouseStatus
        {
            public MouseButtonStatus status;
            public MouseButton button;

            public Int32 firstX;
            public Int32 firstY;
            /* public Int32 lastX;
             public Int32 lastY;       So könnte man das für bestimmte "Interpolationen" z.B. von Spline Zwischenabschnitten machen, insbesondere Chaincode basiert. 
             public Int32 currentX;    Ich denke, wir brauchen das zur Zeit nicht. Aber schon mal zur Erinnerung hier "merken". Zur Zeit ist es ja eher einfach, weil wir nur kurze miteinander verbundene Strecken um den Kern malen
             public Int32 currentY; */
        }

        MouseStatus _mouseStatus = new MouseStatus();
        /*
        public Form1()
        {
            InitializeComponent();
        }
        */

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            _mouseCoordinates = null;
            if (e.Button == MouseButtons.Left)
                _mouseStatus.button = MouseButton.LEFT;
            if (e.Button == MouseButtons.Middle)
                _mouseStatus.button = MouseButton.MIDDLE;
            if (e.Button == MouseButtons.Right)
                _mouseStatus.button = MouseButton.RIGHT;

            _mouseStatus.status = MouseButtonStatus.DOWN;
            _mouseStatus.firstX = e.X;
            _mouseStatus.firstY = e.Y;

            /* _mouseStatus.currentX= e.X;
             _mouseStatus.currentY= e.Y;
             _mouseStatus.lastX= e.X;
             _mouseStatus.lastY= e.Y; */

            AddMouseCoordinate(e.X, e.Y);
            //Invalidate();
            Refresh();
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            _mouseStatus.status = MouseButtonStatus.UP;
            /* _mouseStatus.currentX= e.X;
             _mouseStatus.currentY= e.Y;
             _mouseStatus.lastX= e.X;
             _mouseStatus.lastY= e.Y;*/

            AddMouseCoordinate(e.X, e.Y);
            //if (chbCloseContour.Checked == true)
            AddMouseCoordinate(_mouseStatus.firstX, _mouseStatus.firstY);
            //Invalidate();
            //Update(); //Invalidate alleine called nicht unbedingt OnPaint, Update forciert den Call von OnPaint --> funktioniert nicht
            _finishedDrawingOfOneNucleus = true;
            this.Refresh(); //Forciert Invalidate und ein redraw und called OnPaint         
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseStatus.status != MouseButtonStatus.DOWN)
                return;

            AddMouseCoordinate(e.X, e.Y);
            //Invalidate();
            Refresh();
        }

        protected void AddMouseCoordinate(Int32 X, Int32 Y)
        {
            MouseCoordinate[] tmpMouseCoordinates = null;
            Int32 savedCoordinates = 0;

            if (_mouseCoordinates != null)
                savedCoordinates = _mouseCoordinates.Length;

            tmpMouseCoordinates = new MouseCoordinate[savedCoordinates + 1];
            for (Int32 C = 0; C < savedCoordinates; C++)
                tmpMouseCoordinates[C] = _mouseCoordinates[C];
            tmpMouseCoordinates[savedCoordinates].X = (float)X;
            tmpMouseCoordinates[savedCoordinates].Y = (float)Y;
            _mouseCoordinates = tmpMouseCoordinates;
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            // Nur mal so zum Ausprobieren.
            if (_mouseCoordinates == null)
                return;
            if (_mouseCoordinates.Length < 2)
                return;

            //Graphics g = Graphics.FromImage(_btmNucleiImageEdgesDetected);
            //hier wird ein PointF-Array erstellt nach den Koordinaten des _mouseCoordinates-Array --> brauch ich noch später für die Speicherung der selbst gezeichneten Nuclei-Umrandungen
            /*
            PointF[] malKurve = new PointF[_mouseCoordinates.Length];
            for (Int32 P = 0; P < _mouseCoordinates.Length; P++)
                malKurve[P] = new PointF(_mouseCoordinates[P].X, _mouseCoordinates[P].Y);
            */

            _malKurve = new PointF[_mouseCoordinates.Length];
            for (Int32 P = 0; P < _mouseCoordinates.Length; P++)
                _malKurve[P] = new PointF(_mouseCoordinates[P].X, _mouseCoordinates[P].Y);

            e.Graphics.DrawLines(Pens.Green, _malKurve);
            _NucleiImageWithAutomaticEdgesToDrawOn = new Image<Bgr, byte>(_btmNucleiImageWithAutomaticEdges);
            //g.DrawLines(Pens.Blue, malKurve); //Wird zwar gemalt aber verschoben vom Klick-Point
            ShowImageOnForm(pcBxOriImage, _NucleiImageWithAutomaticEdgesToDrawOn);
            //_mouseCoordinates = null;
        }
        #endregion

        private void OnAddNucleus(object sender, EventArgs e)
        {
            if (!_finishedDrawingOfOneNucleus)
                return;

            Graphics graphics = Graphics.FromImage(_btmNucleiImageWithAutomaticEdges);
            graphics.DrawLines(Pens.Green, _malKurve);       
            _NucleiImageWithAutomaticEdgesToDrawOn = new Image<Bgr, byte>(_btmNucleiImageWithAutomaticEdges);
            ShowImageOnForm(pcBxOriImage, _NucleiImageWithAutomaticEdgesToDrawOn);
            //hier erstellte Nuclei haben noch keinen Center Point!! Der Center Point ist hier also null!!
            Int32 nucleiNumber = _allNuclei._LstAllNuclei.Count + 1;
            Nucleus nucleus = new Nucleus("Nucleus " + nucleiNumber, _malKurve);
            _allNuclei.AddNucleusToNucleiList(nucleus);
            _finishedDrawingOfOneNucleus = false;
            _mouseCoordinates = null;
            _malKurve = null;
            DisplayAddedNucleusAsCheckboxes(nucleus);
        }
        /*
         * Hier werden alle Checkboxen im Panel durchgegangen.
         * Wenn die Checkbox nicht ausgewählt wurde, wird das Nucleus-Objekt in der Nucleus Liste, des Nuclei-Objekts
         * gelöscht. Dies muss dann noch an das Nuclei-Objekt in FormOne übergeben werden, damit dann dort die Telomer-Spots
         * zu den einzelnen Nuclei zugeordnet werden.
         */
        private void OnApply(object sender, EventArgs e)
        {
            _pressedBtnApply = true;
            btnAddNucleus.Hide();
            Graphics graphics = Graphics.FromImage(_btmNucleiImageWithAutomaticEdges);
                foreach (CheckBox checkBox in pnlSelectNuclei.Controls.OfType<CheckBox>())
                {
                    if (!checkBox.Checked)
                    {
                        for(Int32 n = 0; n < _allNuclei._LstAllNuclei.Count; n++)
                        {
                                if(_allNuclei._LstAllNuclei[n] == null)
                                continue;
                            if (checkBox.Name.Equals("chkBx" + _allNuclei._LstAllNuclei[n]._nucleusName))
                            {
                                _allNuclei.DrawContour(_NucleiImageWithAutomaticEdgesToDrawOn, _allNuclei._LstAllNuclei[n]._nucleusContourPoints, new Bgr(Color.Red)); //funktioniert nicht bzw. ist nicht sichtbar
                            /*
                                SolidBrush redBrush = new SolidBrush(Color.Red);
                                graphics.FillPolygon(redBrush, _allNuclei._LstAllNuclei[n]._nucleusContourPoints);
                                Hier sollen einfach nur die Namen der Nuclei auf dem Bild angezeigt werden bzw. verschwinden, wenn sie abgewählt sind
                            */
                                _NucleiImageWithAutomaticEdgesToDrawOn = new Image<Bgr, byte>(_btmNucleiImageWithAutomaticEdges);
                                ShowImageOnForm(pcBxOriImage, _NucleiImageWithAutomaticEdgesToDrawOn);
                                Refresh();
                                _allNuclei._LstAllNuclei.Remove(_allNuclei._LstAllNuclei[n]);
                            }
                        }
                    }
                }
                
        }

        public void ShowImageOnForm(PictureBox picBox, Image<Bgr, byte> image)
        {
            try
            {
                if (picBox.BackgroundImage == null)
                {
                    picBox.BackgroundImage = image.ToBitmap();
                    picBox.Width = image.Width;
                    picBox.Height = image.Height;
                    picBox.MaximumSize = image.Size;
                    //picBox.Refresh();
                    Refresh();
                }
                else
                    picBox.BackgroundImage = image.ToBitmap();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /*
        private void FormTwo_OnClosing(object sender, FormClosingEventArgs e)
        {
            if (!_pressedBtnApply)
            {
                DialogResult result = MessageBox.Show("You should press on Apply first to save your Changes. Do you really want to close this Window?", string.Empty, MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                    e.Cancel = true;
            }
            else
            {
                Invalidate();
                //Environment.Exit(0);
                System.Windows.Forms.Application.Exit();
                //this.Close();
            }
        }
        */
    }
}
