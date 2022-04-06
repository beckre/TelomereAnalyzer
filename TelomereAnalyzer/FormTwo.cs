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
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using System.Drawing.Drawing2D;


namespace TelomereAnalyzer
{
    //Handling Drawing Nuclei by User
    public partial class FormTwo : Form
    {
        public Nuclei _allNuclei = null;
        public Image<Bgr, byte> _NucleiImageEdited = null;
        public Bitmap _btmNucleiImageEdited = null;
        public Boolean _finishedDrawingOfOneNucleus = false;

        public FormTwo(Nuclei allNuclei, Image<Bgr, byte> nucleiImageEdgesDetected)
        {
            this._allNuclei = allNuclei;
            this._NucleiImageEdited = nucleiImageEdgesDetected;
            this._btmNucleiImageEdited = nucleiImageEdgesDetected.ToBitmap();
            InitializeComponent();
            _allNuclei.PrepareDrawingCenterPoints();
            _allNuclei.PrepareDrawingContoursByNucleus(new Bgr(Color.Green));
            this._NucleiImageEdited = _allNuclei._imageToDrawOn;
            this._btmNucleiImageEdited = _NucleiImageEdited.ToBitmap();
            //ShowBitmapOnForm(ImageBoxOneFormTwo, _btmNucleiImageEdited);
            //panel1.BackgroundImageLayout = ImageLayout.Stretch;
            panel1.BackgroundImage = _btmNucleiImageEdited;
            
            //pictureBox1.BackgroundImage = _btmNucleiImageEdited;
            DisplayAllNucleiOnPanel();
        }

        private void DisplayAllNucleiOnPanel()
        {
            pnlSelectNuclei.Controls.Clear();

            for(Int32 i = 0; i < _allNuclei._LstAllNuclei.Count; i++)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Name = "chkBx" + _allNuclei._LstAllNuclei[i]._nucleusName;
                checkBox.Text = _allNuclei._LstAllNuclei[i]._nucleusName;
                checkBox.Width = (TextRenderer.MeasureText(checkBox.Text, checkBox.Font)).Width + 50;
                checkBox.Location = new Point(5, pnlSelectNuclei.Controls.Count * 20);
                checkBox.Checked = true;
                pnlSelectNuclei.Controls.Add(checkBox);
            }
            Refresh();

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

            //g.DrawLines(Pens.Blue, malKurve); //Wird zwar gemalt aber verschoben vom Klick-Point
            ShowBitmapOnForm(ImageBoxOneFormTwo, _btmNucleiImageEdited);
            //_mouseCoordinates = null;
        }
        #endregion

        private void OnAddNucleus(object sender, EventArgs e)
        {
            if (!_finishedDrawingOfOneNucleus)
                return;

            Graphics graphics = Graphics.FromImage(_btmNucleiImageEdited);
            /*
            PointF[] contour = new PointF[_mouseCoordinates.Length];
            for (Int32 P = 0; P < _mouseCoordinates.Length; P++)
                contour[P] = new PointF(_mouseCoordinates[P].X, _mouseCoordinates[P].Y);
            */

            //graphics.DrawLines(Pens.Blue, contour);
            //graphics.DrawPolygon(Pens.Blue, _malKurve);
            graphics.DrawLines(Pens.Green, _malKurve);
            ShowBitmapOnForm(ImageBoxOneFormTwo, _btmNucleiImageEdited);
            _NucleiImageEdited = new Image<Bgr, byte>(_btmNucleiImageEdited);

            //hier erstellte Nuclei haben noch keinen Center Point!! Der Center Point ist hier also null!!
            Int32 nucleiNumber = _allNuclei._LstAllNuclei.Count + 1;
            Nucleus nucleus = new Nucleus("Nucleus " + nucleiNumber, _malKurve);
            _allNuclei.AddNucleusToNucleiList(nucleus);
            _finishedDrawingOfOneNucleus = false;
            _mouseCoordinates = null;
            _malKurve = null;
            DisplayAllNucleiOnPanel();
        }
        /*
         * Hier werden alle Checkboxen im Panel durchgegangen.
         * Wenn die Checkbox nicht ausgewählt wurde, wird das Nucleus-Objekt in der Nucleus Liste, des Nuclei-Objekts
         * gelöscht. Dies muss dann noch an das Nuclei-Objekt in FormOne übergeben werden, damit dann dort die Telomer-Spots
         * zu den einzelnen Nuclei zugeordnet werden.
         */
        private void OnApply(object sender, EventArgs e)
        {
            btnAddNucleus.Hide();
            Graphics graphics = Graphics.FromImage(_btmNucleiImageEdited);
            //try
            //{
                foreach (CheckBox checkBox in pnlSelectNuclei.Controls)
                {
                    if (!checkBox.Checked)
                    {
                        for(Int32 n = 0; n < _allNuclei._LstAllNuclei.Count; n++)
                        {
                                if(_allNuclei._LstAllNuclei[n] == null)
                                continue;
                            if (checkBox.Name.Equals("chkBx" + _allNuclei._LstAllNuclei[n]._nucleusName))
                            {
                                _allNuclei.DrawContour(_NucleiImageEdited, _allNuclei._LstAllNuclei[n]._nucleusContourPoints, new Bgr(Color.Red));
                                //_NucleiImageEdited.Draw(new Rectangle(150, 80, 300, 95), new Bgr(Color.Blue), 90);
                                SolidBrush redBrush = new SolidBrush(Color.Red);
                                graphics.FillPolygon(redBrush, _allNuclei._LstAllNuclei[n]._nucleusContourPoints);
                                _NucleiImageEdited = new Image<Bgr, byte>(_btmNucleiImageEdited);
                                ShowBitmapOnForm(ImageBoxOneFormTwo, _btmNucleiImageEdited);
                                //ShowBitmapOnForm(ImageBoxOneFormTwo, _NucleiImageEdited.ToBitmap());
                                Refresh();
                                _allNuclei._LstAllNuclei.Remove(_allNuclei._LstAllNuclei[n]);
                            }
                        }
                    }
                }
            /*}

           catch(Exception ex)
           {
               Console.WriteLine(ex.ToString());
           }
       */
        }

        public void ShowBitmapOnForm(ImageBox imageBox, Bitmap bitmap)
        {
            if (imageBox.BackgroundImage == null)
            {
                //imageBox.
                imageBox.BackgroundImage = bitmap;
                imageBox.Width = bitmap.Width;
                imageBox.Height = bitmap.Height;
                imageBox.MaximumSize = bitmap.Size;
                imageBox.Refresh();
            }
            else
                imageBox.BackgroundImage = bitmap;
        }
    }
}
