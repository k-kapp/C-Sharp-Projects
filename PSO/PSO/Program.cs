using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace PSO
{

    public delegate string ParamInfo<T>(PSO<T> pso_obj);

    public class PointND : IEquatable<PointND>
    {
        public PointND(int [] init, float thresh=0)
        {
            Length = init.Length;
            intern_repr = new float[Length];
            Array.Copy(init, intern_repr, init.Length);
            this.thresh = thresh;
        }

        public PointND(float [] init, float thresh=0)
        {
            Length = init.Length;
            intern_repr = new float[Length];
            Array.Copy(init, intern_repr, init.Length);
            this.thresh = thresh;
        }

        private void make_str_intern_repr()
        {
            str_repr = "";
            foreach(var val in intern_repr)
            {
                str_repr += val.ToString() + ",";
            }
        }

        float [] intern_repr;
        float thresh;
        string str_repr;
        public int Length;

        public float this[int idx]
        {
            get
            {
                return intern_repr[idx];
            }

            set
            {
                intern_repr[idx] = value;
            }
        }

        public float [] get_internal_repr()
        {
            return intern_repr;
        }

        public bool Equals(PointND other)
        {
            if (other.Length != Length)
                return false;
            for (int i = 0; i < Length; i++)
                if (Math.Abs(other[i] - this[i]) > thresh)
                {
                    Console.WriteLine("equality: false with thresh = " + thresh.ToString());
                    Console.WriteLine("positions: " + other[i].ToString() + " and " + this[i].ToString());
                    return false;
                }
            return true;
        }

        public override bool Equals(object obj)
        {
            PointND temp = obj as PointND;

            return Equals(temp);
        }

        public override int GetHashCode()
        {
            //return str_repr.GetHashCode();
            return "".GetHashCode();
        }

        public static bool operator ==(PointND first, PointND second)
        {
            if ((object)first == null || (object)second == null)
            {
                return object.Equals(first, second);
            }
            else
            {
                return first.Equals(second);
            }
        }

        public static bool operator !=(PointND first, PointND second)
        {
            if ((object)first == null || (object)second == null)
            {
                return !object.Equals(first, second);
            }
            else
            {
                return !first.Equals(second);
            }
        }

        public static bool approxEqual (PointND first, PointND second, float thresh)
        {
            if (first.Length != second.Length)
                return false;
            for (int i = 0; i < first.Length; i++)
            {
                if (Math.Abs(first[i] - second[i]) > thresh)
                    return false;
            }
            return true;
        }
    }


    class PSOLabel : Label
    {
        protected PSO<PointND> pso_obj;

        public PSOLabel(PSO<PointND> pso_obj, int locX, int locY, int fontSize = 16) : base()
        {
            this.pso_obj = pso_obj;
            ForeColor = Color.Black;
            BackColor = Color.White;
            Location = new Point(locX, locY);
            Font = new Font("Arial", fontSize);
        }

        public SizeF getSize()
        {
            return TextRenderer.MeasureText(Text, Font);
        }

    }

    class InfoLabel<T> : Label
    {
        ParamInfo<T> _stringMaker;
        PSO<T> _psoObj;

        public InfoLabel(int locX, int locY, int fontSize, ParamInfo<T> stringMaker, PSO<T> psoObj, string initText="") : base()
        {
            _psoObj = psoObj;
            _stringMaker = stringMaker;

            ForeColor = Color.Black;
            BackColor = Color.White;
            Location = new Point(locX, locY);
            Font = new Font("Arial", fontSize);

            Text = initText;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Text = _stringMaker(_psoObj);
            Size = new Size(PreferredWidth, PreferredHeight);
        }

        public SizeF GetSize()
        {
            return TextRenderer.MeasureText(Text, Font);
        }
    }

    class BestPosLabel : PSOLabel
    {
        public BestPosLabel(PSO<PointND> pso_obj, int locX, int locY) : base(pso_obj, locX, locY)
        {
            Text = "Global best position: 0\nGlobal best fitness: 0"; 
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            double[] globBest = pso_obj.getBest().Item1;
            double globBestValue = pso_obj.getBest().Item2;
            string globBestString = "Global best position: " + Math.Round(globBest[0], 4).ToString() + ", " + Math.Round(globBest[1], 4).ToString() + "\n"
                + "Global best fitness: " + Math.Round(globBestValue, 4).ToString();

            Text = globBestString;
            Size = new Size(PreferredWidth, PreferredHeight);
        }
    }

    class CogScaleLabel : PSOLabel
    {
        public CogScaleLabel(PSO<PointND> pso_obj, int locX, int locY) : base(pso_obj, locX, locY)
        {
            Text = "Cognitive scale";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            float cogScale = pso_obj.getCogScale();
            string cogScaleString = "Cognitive scale: " + Math.Round(cogScale*pso_obj.getSpeed(), 4).ToString();
            Text = cogScaleString;
            Size = new Size(PreferredWidth, PreferredHeight);
        }
    }

    class SocScaleLabel : PSOLabel
    {
        public SocScaleLabel(PSO<PointND> pso_obj, int locX, int locY) : base(pso_obj, locX, locY)
        {
            Text = "Social scale";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            float socScale = pso_obj.getSocScale();

            string socScaleString = "Social scale: " + (socScale*pso_obj.getSpeed()).ToString();
            Text = socScaleString;
            Size = new Size(PreferredWidth, PreferredHeight);
        }
    }

    public static class StringMaker
    {
        public static string CogScaleString<T>(PSO<T> psoObj)
        {
            float cogScale = psoObj.getCogScale();
            return "Cognitive scale: " + Math.Round(cogScale*psoObj.getSpeed(), 4).ToString();
        }

        public static string SocScaleString<T>(PSO<T> psoObj)
        {
            float socScale = psoObj.getSocScale();
            return "Social scale: " + (socScale*psoObj.getSpeed()).ToString();
        }

        public static string BestPosString<T>(PSO<T> psoObj)
        {
            double[] globBest = psoObj.getBest().Item1;
            double globBestValue = psoObj.getBest().Item2;
            return "Global best position: " + Math.Round(globBest[0], 4).ToString() + ", " + Math.Round(globBest[1], 4).ToString() + "\n"
                + "Global best fitness: " + Math.Round(globBestValue, 4).ToString();
        }
    }


    public class Form1 : Form
    {
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Timer timer1;
        private float angle = 0;
        private PSO<PointND> pso_obj;
        private double[,] allPosses;
        System.Drawing.Graphics formGraphics;
        System.Drawing.Font drawFont;
        System.Drawing.SolidBrush drawBrush;
        System.Drawing.StringFormat drawFormat;
        float textX;
        float textY;
        List<PointND> refPoints;
        Func<double[], double> objectiveFunc;

        InfoLabel<PointND> bestPosLbl;
        InfoLabel<PointND> cogScaleLbl;
        InfoLabel<PointND> socScaleLbl;

        TrackBar socTb, cogTb, speedTb, inertiaTb;

        Button resetButton;

        float maxCogScale = 2.5f;
        float minCogScale = 0.0f;
        float maxSocScale = 2.5f;
        float minSocScale = 0.0f;
        float psoScaleSpeed = 2e-3f;

        float initCogScale = 2.5f;
        float initSocScale = 0.0f;

        static float[,] optimalLocs = { { 0, 0 } };

        int startX = 400;
        int startY = 500;
        int scale = 10;

        int windowSizeX = 950;
        int windowSizeY = 1000;

        bool leftDown = false;
        PointND currHeldPos = null;

        const int circleRad = 5;

        List<float[]> psoRefs;

        int sliderSpace = 75;

        public Form1(Func<double[], List<PointND>, double> objectiveFunc, List<float[]> refPoints)
        {

            initializeComponent();
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);

            initRefPoints(refPoints);
            initPSO(objectiveFunc);
            initColors();
            initLabels();
            initSliders();
            initButtons();

            Size = new Size(windowSizeX, windowSizeY);
            Location = new Point(500, 20);

            this.KeyUp += key_pressed;
            this.MouseDown += mouseButtonDown;
            this.MouseUp += mouseButtonUp;
            this.MouseMove += updateHeldPos;

            List<float[]> myList = new List<float[]>();
            List<float[]> myRefList = new List<float[]>();

            myList.Add(new float[2] { 4, 6 });
            myList.Add(new float[2] { 10, 10 });

            myRefList.Add(myList[0]);
            myRefList.Add(myList[1]);

            myList.RemoveAt(0);
            
            if (myRefList[0] == null)
            {
                Console.WriteLine("Removed reference is now null");
            }
            else
            {
                Console.WriteLine(myRefList[0][0]);
                Console.WriteLine("Removed reference is not null");
                Console.WriteLine(myList[0][0]);
            }


        }
        
        private void fillCircle(PaintEventArgs e, Brush color, int x, int y, int radius)
        {
            e.Graphics.FillEllipse(color, x - radius, y - radius, radius * 2, radius * 2);
        }

        private void fillCircle(PaintEventArgs e, Brush color, float x, float y, int radius)
        {
            e.Graphics.FillEllipse(color, x - radius, y - radius, radius * 2, radius * 2);
        }

        private void selectPoint(MouseEventArgs e)
        {
            currHeldPos = refPoints.Find(x => x == new PointND(toOrigSpace(new float[2] { e.X, e.Y }), circleRad/(float)scale));
            if (currHeldPos == null)
            {
                Console.WriteLine("Point not selected");
            }
            else
            {
                Console.WriteLine("Point was selected");
            }
        }

        private float [] toOrigSpace(float [] point)
        {
            return new float[2] { (point[0] - startX) / scale, (point[1] - startY) / scale };
        }

        private float[] toOrigSpace(MouseEventArgs e)
        {
            return new float[2] { (e.X - startX) / (float)scale, (e.Y - startY) / (float)scale };
        }

        private bool removeRefPoint(float [] point)
        {
            PointND temp = new PointND(toOrigSpace(point), circleRad/(float)scale);
            return removeRefPoint(temp);
        }

        private bool removeRefPoint(PointND point)
        {
            if (refPoints.Remove(point))
            {
                pso_obj.resetBests();
                return true;
            }
            else
                return false;
        }

        private bool removeRefPoint(MouseEventArgs e)
        {
            //return removeRefPoint(toOrigSpace(new float[2] { e.X, e.Y }));
            return removeRefPoint(new float[2] { e.X, e.Y });
        }

        private void addRefPoint(float [] point)
        {
            addRefPoint(new PointND(toOrigSpace(point), circleRad/(float)scale));
        }

        private void addRefPoint(PointND point)
        {
            refPoints.Add(point);
            pso_obj.resetBests();
        }

        private void addRefPoint(MouseEventArgs e)
        {
            addRefPoint(new float[2] { e.X, e.Y });
        }

        private void mouseButtonDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectPoint(e);
            }
        }

        private void mouseButtonUp(object sender, MouseEventArgs e)
        {
            PointF nums1 = new PointF ( 5, 9 );
            PointF nums2 = new PointF ( 5, 9 );
            Console.WriteLine(nums1.Equals(nums2));
            if (e.Button == MouseButtons.Left)
            {
                currHeldPos = null;
            }
            else if (e.Button == MouseButtons.Right)
            {
                /* TODO: let removeRefPoint return false if refPoint not found, which
                 * will be used in deciding whether to call addRefPoint
                 */
                if (!removeRefPoint(e))
                {
                    addRefPoint(e);
                }
            }
        }

        private void updateHeldPos(object sender, MouseEventArgs e)
        {
            if (currHeldPos != null)
            {

                float newX = (e.X - startX) / (float)scale;
                float newY = (e.Y - startY) / (float)scale;
                if (newX != currHeldPos[0] || newY != currHeldPos[1])
                {
                    currHeldPos[0] = newX;
                    currHeldPos[1] = newY;
                    pso_obj.resetBests();
                }
            }
        }

        private void initLabels()
        {
            bestPosLbl = new InfoLabel<PointND>(10, 10, 16, StringMaker.BestPosString, pso_obj, 
                "Global best position: 0\nGlobal best fitness: 0");
            Controls.Add(bestPosLbl);

            cogScaleLbl = new InfoLabel<PointND>(10, bestPosLbl.Location.Y + (int)bestPosLbl.GetSize().Height + 5, 16, StringMaker.CogScaleString, pso_obj);
            Controls.Add(cogScaleLbl);

            socScaleLbl = new InfoLabel<PointND>(10, cogScaleLbl.Location.Y + cogScaleLbl.Height + 5, 16, StringMaker.SocScaleString, pso_obj);
            //socScaleLbl = new SocScaleLabel(pso_obj, 10, cogScaleLbl.Location.Y + cogScaleLbl.Height + 5);
            Controls.Add(socScaleLbl);
        }

        private void initButtons()
        {
            resetButton = new Button();

            resetButton.Text = "Reset";
            resetButton.Location = new Point(50, 170);
            resetButton.Size = new Size(120, 30);
            Controls.Add(resetButton);

            resetButton.Click += resetButtonPressed;
        }

        private void initSliders()
        {
            int topY = 20;

            cogTb = new TrackBar();
            cogTb.Minimum = 0;
            cogTb.Maximum = 100;
            cogTb.Value = (int)((initCogScale - minCogScale)/(maxCogScale - minCogScale)*100.0f);
            cogTb.SmallChange = 1;
            cogTb.LargeChange = 10;
            cogTb.Location = new Point(500, topY);
            cogTb.Size = new Size(200, 60);
            //cogTb.Dock = DockStyle.Bottom;
            Controls.Add(cogTb);

            socTb = new TrackBar();
            socTb.Minimum = 0;
            socTb.Maximum = 100;
            socTb.Value = (int)((initSocScale - minSocScale)/(maxSocScale - minSocScale)*100.0f);
            socTb.SmallChange = 1;
            socTb.LargeChange = 10;
            socTb.Location = new Point(720, topY);
            socTb.Size = new Size(200, 60);
            //socTb.Dock = DockStyle.Bottom;
            Controls.Add(socTb);

            speedTb = new TrackBar();
            speedTb.Minimum = 0;
            speedTb.Maximum = 100;
            speedTb.Value = 10;
            speedTb.SmallChange = 1;
            speedTb.LargeChange = 10;
            speedTb.Location = new Point(610, topY + sliderSpace);
            speedTb.Size = new Size(200, 60);
            Controls.Add(speedTb);

            inertiaTb = new TrackBar();
            inertiaTb.Minimum = 0;
            inertiaTb.Maximum = 110;
            inertiaTb.Value = 99;
            inertiaTb.SmallChange = 1;
            inertiaTb.LargeChange = 10;
            inertiaTb.Location = new Point(610, topY + 2*sliderSpace);
            inertiaTb.Size = new Size(200, 60);
            Controls.Add(inertiaTb);

            cogTb.Scroll += cognitiveScroll;
            socTb.Scroll += socialScroll;
            speedTb.Scroll += speedScroll;
            inertiaTb.Scroll += inertiaScroll;
        }

        private void initSliderLabels()
        {
            
        }

        private void resetButtonPressed(object sender, EventArgs e)
        {
            pso_obj.initPop();
        }

        private void inertiaScroll(object sender, EventArgs e)
        {
            pso_obj.setInertia(inertiaTb.Value / 100.0f);
        }

        private void speedScroll(object sender, EventArgs e)
        {
            updateSpeed();
        }

        private void updateSpeed()
        {
            pso_obj.setSpeed(speedTb.Value * psoScaleSpeed);
        }

        private void updateScales()
        {
            pso_obj.setSocScale(socTb.Value/100.0f*(maxSocScale - minSocScale) + minSocScale);
            pso_obj.setCogScale(cogTb.Value/100.0f*(maxCogScale - minCogScale) + minCogScale);
        }

        private void cognitiveScroll(object sender, EventArgs e)
        {
            socTb.Value = 100 - cogTb.Value;
            updateScales();
        }

        private void socialScroll(object sender, EventArgs e)
        {
            cogTb.Value = 100 - socTb.Value;
            updateScales();
        }

        private void updateLabels()
        {
            bestPosLbl.Refresh();
            cogScaleLbl.Refresh();
            socScaleLbl.Refresh();
        }

        private void updateSliders()
        {
            cogTb.Update();
            socTb.Update();
        }

        private void updateButtons()
        {
            resetButton.Update();
        }

        private void initPSO(Func<double [], List<PointND>, double> objectiveFunc)
        {
            /*
            List<float[]> psoRefs = new List<float[]>();
            foreach(var refPt in refPoints)
            {
                psoRefs.Add(refPt.get_internal_repr());
            }
            */
            int dim = 2;
            Tuple<double, double>[] ranges = new Tuple<double, double>[dim];
            ranges[0] = new Tuple<double, double>(-30.0, 30.0);
            ranges[1] = new Tuple<double, double>(-30.0, 30.0);
            pso_obj = new PSO<PointND>(2, 30, 0.999, new LinearScale(minCogScale, maxCogScale, initCogScale, -0.0000f), 
                new LinearScale(minSocScale, maxSocScale, initSocScale, 0.0000f), psoScaleSpeed, objectiveFunc, refPoints, 1, ranges);
        }

        private void initRefPoints(List<float []> refPoints)
        {
            this.refPoints = new List<PointND>();
            foreach (var refpt in refPoints)
            {
                this.refPoints.Add(new PointND(refpt, circleRad/(float)scale));


            }
        }

        /*
        private float [] findRefPoint(MouseEventArgs e)
        {
            return findRefPoint(toOrigSpace(e));
        }
        
        private float [] findRefPoint(float [] point)
        {
            for (int refIdx = 0; refIdx < refPoints.Count; refIdx++)
            {
                var refPt = refPoints[refIdx];
                bool match = true;
                for (int i = 0; i < refPt.Length; i++)
                {
                    Console.WriteLine("refPt at " + i.ToString() + " :" + refPt[i].ToString());
                    Console.WriteLine("pt at " + i.ToString() + " :" + point[i].ToString());
                    if (Math.Abs(refPt[i] - point[i]) > circleRad/(float)scale)
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    return refPoints[refIdx];
                }
            }
            return null;
        }
        */
        
        private void initColors()
        {
            this.BackColor = Color.FromArgb(255, 255, 255, 255);
            this.ForeColor = Color.FromArgb(0, 0, 0, 255);
        }

        #region Windows Form designer generated code
        private void initializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.timer1 = new System.Windows.Forms.Timer(this.components);

            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1Tick);
            this.timer1.Interval = 15;

            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.ClientSize = new System.Drawing.Size(800, 600);

            this.Name = "Form1";
            this.Text = "LinearGradientBrush Demo";

            this.Load += new System.EventHandler(this.form1Load);

            initDebugText();
        }
        #endregion

        static double objectiveFunction(double[] pos, List<PointND> optimPoints)
        {
            double totalSum = 0.0;
            for (int j = 0; j < optimPoints.Count; j++)
            {
                double optimPointSum = 0.0;
                for (int i = 0; i < pos.Length; i++)
                {
                    optimPointSum += Math.Abs(pos[i] - optimPoints[j][i]);
                }
                totalSum += Math.Exp(-optimPointSum);
            }
            return totalSum;
        }

        static void Main()
        {
            List<float[]> refPoints = new List<float []>();
            for(int i = 0; i < optimalLocs.GetLength(0); i++)
            {
                float[] destArr = new float[2];
                Buffer.BlockCopy(optimalLocs, sizeof(float) * 2 * i, destArr, 0, sizeof(float) * 2);
                refPoints.Add(destArr);
            }

            Application.Run(new Form1(objectiveFunction, refPoints));
        }

        private void form1Load(object sender, System.EventArgs e)
        {
            allPosses = pso_obj.yieldPosses();
        }

        private LinearGradientBrush getBrush()
        {
            return new LinearGradientBrush(
                new Rectangle(20, 20, 200, 100),
                Color.Orange,
                Color.Yellow,
                0.0F,
                true);
        }

        private void Rotate(Graphics graphics, LinearGradientBrush brush)
        {
            brush.RotateTransform(angle);
            brush.SetBlendTriangularShape(0.5F);
            graphics.FillRectangle(brush, brush.Rectangle);
        }

        private void Rotate(Graphics graphics)
        {
            angle += 5 % 360;
            Rotate(graphics, getBrush());
        }

        private void timer1Tick(object sender, System.EventArgs e)
        {
            pso_obj.nextIteration();
            allPosses = pso_obj.yieldPosses();

            this.Invalidate();
            this.Update();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //drawDebugText(e);

            /*
            circlePosses[0] = new Tuple<int, int>(circlePosses[0].Item1 + 2, circlePosses[0].Item2);
            circlePosses[1] = new Tuple<int, int>(circlePosses[1].Item1, circlePosses[1].Item2 + 2);
            */
            for (int i = 0; i < allPosses.GetLength(0); i++)
            {
                int x = (int)(allPosses[i, 0]*scale) + startX;
                int y = (int)(allPosses[i, 1]*scale) + startY;
                e.Graphics.FillEllipse(Brushes.Black, x, y, 10, 10);
            }

            paintRefPoints(e);

            drawUpperRect(e);

            updateLabels();
            updateSliders();
            updateButtons();
        }

        private void drawUpperRect(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, 0, 0, windowSizeX, 200);
        }

        private void initDebugText()
        {
            formGraphics = this.CreateGraphics();
            drawFont = new System.Drawing.Font("Arial", 16);
            drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            textX = 10.0F;
            textY = 10.0F;
            drawFormat = new System.Drawing.StringFormat();
            /*
            drawFont.Dispose();
            drawBrush.Dispose();
            formGraphics.Dispose();
            */
        }

        private void drawDebugText(PaintEventArgs e)
        {
            double[] globBest = pso_obj.getBest().Item1;
            double globBestValue = pso_obj.getBest().Item2;
            string globBestString = "Global best position: " + globBest[0].ToString() + ", " + globBest[1].ToString() + "\n";
           // string globBestString = "Global best position: " + globBest[0].ToString() + ", " + globBest[1].ToString() + "\n";

            e.Graphics.DrawString(globBestString, drawFont, drawBrush, textX, textY, drawFormat);
        }

        private void cleanupDebugText()
        {
            drawFont.Dispose();
            drawBrush.Dispose();
            formGraphics.Dispose();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            cleanupDebugText();
        }

        public void paintRefPoints(PaintEventArgs e)
        {
            foreach (var refArr in refPoints)
            {
                //e.Graphics.FillEllipse(Brushes.Red, refArr[0]*scale + startX, refArr[1]*scale + startY, circleRad*2, circleRad*2);
                fillCircle(e, Brushes.Red, refArr[0] * scale + startX, refArr[1] * scale + startY, circleRad);
            }
        }

        void key_pressed(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }


}
