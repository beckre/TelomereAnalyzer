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
    //Hanlding Drawing Nuclei by User
    public partial class FormTwo : Form
    {
        public List<Nucleus> _allNuclei = null;
        public Image<Bgr, byte> _NucleiImageEdgesDetected = null;
        public Bitmap _btmNucleiImageEdgesDetected = null;
        public FormTwo(List<Nucleus> allNuclei, Image<Bgr, byte> nucleiImageEdgesDetected)
        {
            this._allNuclei = allNuclei;
            this._NucleiImageEdgesDetected = nucleiImageEdgesDetected;
            this._btmNucleiImageEdgesDetected = nucleiImageEdgesDetected.ToBitmap();
            InitializeComponent();
            ShowBitmapOnForm(ImageBoxOneFormTwo, _NucleiImageEdgesDetected.ToBitmap());
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
            Invalidate();
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
            //Draw();
            //this.Invalidate();
            //Invalidate();
            //Update(); //Invalidate alleine called nicht unbedingt OnPaint, Update forciert den Call von OnPaint --> funktioniert nicht
            this.Refresh(); //Forciert Invalidate und ein redraw und called OnPaint
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseStatus.status != MouseButtonStatus.DOWN)
                return;

            AddMouseCoordinate(e.X, e.Y);
            Invalidate();
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
            PointF[] malKurve = new PointF[_mouseCoordinates.Length];
            for (Int32 P = 0; P < _mouseCoordinates.Length; P++)
                malKurve[P] = new PointF(_mouseCoordinates[P].X, _mouseCoordinates[P].Y);

            e.Graphics.DrawLines(Pens.Blue, malKurve);
            //g.DrawLines(Pens.Blue, malKurve); //Wird zwar gemalt aber verschoben vom Klick-Point
            ShowBitmapOnForm(ImageBoxOneFormTwo, _btmNucleiImageEdgesDetected);
            _mouseCoordinates = null;
        }
        #endregion


        public void ShowBitmapOnForm(ImageBox imageBox, Bitmap bitmap)
        {
            if (imageBox.BackgroundImage == null)
            {
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
