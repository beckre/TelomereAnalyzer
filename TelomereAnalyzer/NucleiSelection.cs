using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;


namespace TelomereAnalyzer
{
    public partial class NucleiSelection : Form
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
        /*----------------------------------------------------------------------------------------*\
        |* Initializes the 2. Form and positions all its controls. Displays the Image with the    *|
        |* automatically detected Nucleiborders in a Picture Box. Creates an Image Box that       *|
        |* alignes with the Picture Box and is on top of the Picture Box. The User has the        *|
        |* ability to draw on the Image Box so that it gives the Illusion that they draw on the   *|
        |* displayed Image itself.                                                                *|
        |* Calls DisplayAllNucleiAsCheckboxesBeginning()                                          *|
        \*----------------------------------------------------------------------------------------*/
        public NucleiSelection(Nuclei allNuclei, Image<Bgr, byte> rawNucleiImageNormalized)
        {
            InitializeComponent();
            this._allNuclei = allNuclei;
            this._RawNucleiImage = rawNucleiImageNormalized;
            _allNuclei.PrepareDrawingCenterPoints();
            _allNuclei.PrepareDrawingContoursByNucleus(new Bgr(Color.DarkSeaGreen));
            this._NucleiImageWithAutomaticEdgesToDrawOn = _allNuclei._imageToDrawOn;
            this._btmNucleiImageWithAutomaticEdges = _NucleiImageWithAutomaticEdgesToDrawOn.ToBitmap();

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            ShowImageOnForm(pcBxOriImage, _NucleiImageWithAutomaticEdgesToDrawOn);
            _imgBx = new ImageBox();
            pcBxOriImage.Controls.Add(_imgBx);
            //Aligning the PictureBox with and on top of the ImageBox
            try
            {
                _imgBx.Width = _NucleiImageWithAutomaticEdgesToDrawOn.Width;
                _imgBx.Height = _NucleiImageWithAutomaticEdgesToDrawOn.Height;
                _imgBx.MaximumSize = _NucleiImageWithAutomaticEdgesToDrawOn.Size;
                _imgBx.Location = pcBxOriImage.Location;
                _imgBx.Refresh();
            }
            catch (Exception ex)
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
        /*----------------------------------------------------------------------------------------*\
        |* Automatically creates a CheckBox for each Nucleus and adds it to a Panel at            *|
        |* the Beginning. The User has the Ability to select/deselect the Nuclei that should      *|
        |* go into the analysis through clicking on the Checkboxes.                               *|
        \*----------------------------------------------------------------------------------------*/
        private void DisplayAllNucleiAsCheckboxesBeginning()
        {
            pnlSelectNuclei.Controls.Clear();
            for (Int32 n = 0; n < _allNuclei._lstAllNuclei.Count; n++)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Name = "chkBx" + _allNuclei._lstAllNuclei[n]._nucleusName;
                checkBox.Text = _allNuclei._lstAllNuclei[n]._nucleusName;
                checkBox.Width = (TextRenderer.MeasureText(checkBox.Text, checkBox.Font)).Width + 50;
                checkBox.Location = new Point(5, pnlSelectNuclei.Controls.Count * 20);
                CreateNucleiLabelOnImageBox(_allNuclei._lstAllNuclei[n]._nucleusContourPoints, _allNuclei._lstAllNuclei[n]._nucleusName);
                //dynamically adds EventHandler for each Checkbox that is dynamically created
                checkBox.CheckedChanged += new EventHandler(CheckBoxChanged);
                pnlSelectNuclei.Controls.Add(checkBox);
            }
            foreach (Label label in _imgBx.Controls.OfType<Label>())
            {
                foreach (CheckBox checkBx in pnlSelectNuclei.Controls.OfType<CheckBox>())
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
        /*----------------------------------------------------------------------------------------*\
        |* Automatically creates a CheckBox for each Nucleus and adds it to a Panel every         *|
        |* time a new Nucleus is drawn by the User and added through clicking on the Button       *|
        |* Add-Nucleus. Calls CreateNucleiLabelOnImageBox                                         *|
        \*----------------------------------------------------------------------------------------*/
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
        /*----------------------------------------------------------------------------------------*\
        |* Creates a Label with the Nucleus Name for each selected Nucleus.                       *|
        |* The Location of the Label is the last Point in the Nucleus Contour-Array.              *|
        \*----------------------------------------------------------------------------------------*/
        private void CreateNucleiLabelOnImageBox(PointF[] contour, String nucleusName)
        {
            PointF lastPoint = contour[contour.Length - 1];
            Label label = new Label();
            label.Name = "lbl" + nucleusName;
            label.BackColor = Color.Transparent;
            label.ForeColor = Color.White;
            label.Text = nucleusName;
            label.Location = Point.Round(lastPoint);
            _imgBx.Controls.Add(label);
            label.Show();
        }
        /*----------------------------------------------------------------------------------------*\
        |* Is called everytime a Checkbox is checked or unchecked through clicking on it.         *|
        |* Changes the ForeColor of a Label from white to red in order to identify checked or     *|
        |* Redraws the whole Image with all selected Nuclei-Borders and Labels.                   *|
        \*----------------------------------------------------------------------------------------*/
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
            //Redraing of Image when selecting/deselecting a Nucleus
            _NucleiImageWithAutomaticEdgesToDrawOn = _RawNucleiImage;
            if (_allNuclei._lstAllNuclei != null)
            {
                for (Int32 E = 0; E < _allNuclei._lstAllNuclei.Count; E++)
                {
                    if (_allNuclei._lstAllNuclei.ElementAt(E) != null)
                    {
                        if (_allNuclei._lstAllNuclei.ElementAt(E)._nucleusContourPoints != null)
                        {
                            foreach (CheckBox checkBx in pnlSelectNuclei.Controls.OfType<CheckBox>())
                            {
                                //If the Nuclei Name equals a checked Checkbox the Nucleus should be redrawn
                                if (checkBx.Text.Equals(_allNuclei._lstAllNuclei.ElementAt(E)._nucleusName))
                                {
                                    if (checkBx.Checked)
                                    {
                                        _NucleiImageWithAutomaticEdgesToDrawOn = _allNuclei.DrawContour(_NucleiImageWithAutomaticEdgesToDrawOn, _allNuclei._lstAllNuclei.ElementAt(E)._nucleusContourPoints, new Bgr(Color.DarkSeaGreen));
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
        /*----------------------------------------------------------------------------------------*\
        |* This region handles the Function of drawing the Nuclei Borders by using the Mouse.     *|
        |* The User draws on an Imagebox that is created during Runtime and not on the Image      *|
        |* itself. This is necessary so that the User can draw freely.                            *|
        \*----------------------------------------------------------------------------------------*/
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
        }
        MouseStatus _mouseStatus = new MouseStatus();
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

            AddMouseCoordinate(e.X, e.Y);
            Refresh();
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            _mouseStatus.status = MouseButtonStatus.UP;
            AddMouseCoordinate(e.X, e.Y);
            AddMouseCoordinate(_mouseStatus.firstX, _mouseStatus.firstY);
            _finishedDrawingOfOneNucleus = true;
            //.Refresh forces the Calling of Invalidate()
            this.Refresh();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseStatus.status != MouseButtonStatus.DOWN)
                return;
            AddMouseCoordinate(e.X, e.Y);
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
            if (_mouseCoordinates == null)
                return;
            if (_mouseCoordinates.Length < 2)
                return;

            _malKurve = new PointF[_mouseCoordinates.Length];
            for (Int32 P = 0; P < _mouseCoordinates.Length; P++)
                _malKurve[P] = new PointF(_mouseCoordinates[P].X, _mouseCoordinates[P].Y);

            e.Graphics.DrawLines(Pens.Green, _malKurve);
            _NucleiImageWithAutomaticEdgesToDrawOn = new Image<Bgr, byte>(_btmNucleiImageWithAutomaticEdges);
            ShowImageOnForm(pcBxOriImage, _NucleiImageWithAutomaticEdgesToDrawOn);
        }
        #endregion
        /*----------------------------------------------------------------------------------------*\
        |* After drawing a Nucleus the User can add it by clicking on the Add Nucleus-Button.     *|
        |* Draws the Nuclei Border on the Image permanently (Can be reverted by deselecting the   *|
        |* corresponding Checkbox). Also creates a new Nucleus-Object and adds it to the List     *|
        |* where all Nuclei that should be analysed are in.                                       *|
        \*----------------------------------------------------------------------------------------*/
        private void OnAddNucleus(object sender, EventArgs e)
        {
            if (!_finishedDrawingOfOneNucleus)
                return;

            Graphics graphics = Graphics.FromImage(_btmNucleiImageWithAutomaticEdges);
            graphics.DrawLines(Pens.Green, _malKurve);
            _NucleiImageWithAutomaticEdgesToDrawOn = new Image<Bgr, byte>(_btmNucleiImageWithAutomaticEdges);
            ShowImageOnForm(pcBxOriImage, _NucleiImageWithAutomaticEdgesToDrawOn);
            //Nuclei that are created here do not have a center point
            Int32 nucleiNumber = _allNuclei._lstAllNuclei.Count + 1;
            Nucleus nucleus = new Nucleus("Nucleus " + nucleiNumber, _malKurve);
            _allNuclei.AddNucleusToNucleiList(nucleus);
            _finishedDrawingOfOneNucleus = false;
            _mouseCoordinates = null;
            _malKurve = null;
            DisplayAddedNucleusAsCheckboxes(nucleus);
        }
        /*----------------------------------------------------------------------------------------*\
        |* If the User is content with their choice of Nuclei, they can click on the Apply-Button.*|
        |* All Checkboxes in the panel are reviewed if their checked or not.                      *|
        |* If a Checkbox is not checked then their Nucleus-Object information is removed out of   *|
        |* the Nuclei-List that contains all Nuclei that should be analysed.                      *|
        \*----------------------------------------------------------------------------------------*/
        private void OnApply(object sender, EventArgs e)
        {
            _pressedBtnApply = true;
            btnAddNucleus.Hide();
            Graphics graphics = Graphics.FromImage(_btmNucleiImageWithAutomaticEdges);
            foreach (CheckBox checkBox in pnlSelectNuclei.Controls.OfType<CheckBox>())
            {
                if (!checkBox.Checked)
                {
                    for (Int32 n = 0; n < _allNuclei._lstAllNuclei.Count; n++)
                    {
                        if (_allNuclei._lstAllNuclei[n] == null)
                            continue;
                        if (checkBox.Name.Equals("chkBx" + _allNuclei._lstAllNuclei[n]._nucleusName))
                        {
                            _allNuclei._lstAllNuclei.Remove(_allNuclei._lstAllNuclei[n]);
                        }
                    }
                }
            }
        }
        /*---------------------------------------------------------------------------------------*\
        |* Displays a Bitmap in a Picture Box.                                                   *|
        |* If it is the first time that an Image is displayed in this Picture Box than the       *|
        |* attributes of the Picture Box are altered to match the Bitmap.                        *|
        |* If not the Bitmap is simply shown in the Picture Box                                  *|
        \*---------------------------------------------------------------------------------------*/
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
                    Refresh();
                }
                else
                    picBox.BackgroundImage = image.ToBitmap();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
