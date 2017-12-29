using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fractal
{
    public partial class Form1 : Form
    {
        class HSB
        {//djm added, it makes it simpler to have this code in here than in the C#
            public float rChan, gChan, bChan;
            public HSB()
            {
                rChan = gChan = bChan = 0;
            }
            public void fromHSB(float h, float s, float b)
            {
                float red = b;
                float green = b;
                float blue = b;
                if (s != 0)
                {
                    float max = b;
                    float dif = b * s / 255f;
                    float min = b - dif;

                    float h2 = h * 360f / 255f;

                    if (h2 < 60f)
                    {
                        red = max;
                        green = h2 * dif / 60f + min;
                        blue = min;
                    }
                    else if (h2 < 120f)
                    {
                        red = -(h2 - 120f) * dif / 60f + min;
                        green = max;
                        blue = min;
                    }
                    else if (h2 < 180f)
                    {
                        red = min;
                        green = max;
                        blue = (h2 - 120f) * dif / 60f + min;
                    }
                    else if (h2 < 240f)
                    {
                        red = min;
                        green = -(h2 - 240f) * dif / 60f + min;
                        blue = max;
                    }
                    else if (h2 < 300f)
                    {
                        red = (h2 - 240f) * dif / 60f + min;
                        green = min;
                        blue = max;
                    }
                    else if (h2 <= 360f)
                    {
                        red = max;
                        green = min;
                        blue = -(h2 - 360f) * dif / 60 + min;
                    }
                    else
                    {
                        red = 0;
                        green = 0;
                        blue = 0;
                    }
                }

                rChan = (float) Math.Round(Math.Min(Math.Max(red, 0f), 255));
                gChan = (float) Math.Round(Math.Min(Math.Max(green, 0), 255));
                bChan = (float) Math.Round(Math.Min(Math.Max(blue, 0), 255));

            }
        }

        private const int MAX = 256;      // max iterations
        private const double SX = -2.025; // start value real
        private const double SY = -1.125; // start value imaginary
        private const double EX = 0.6;    // end value real
        private const double EY = 1.125;  // end value imaginary
        private static int x1, y1, xs, ys, xe, ye;
        private static double xstart, ystart, xende, yende, xzoom, yzoom;
        private static bool action, rectangle, finished;
        private static float xy;
        private Bitmap picture;
        private Graphics g1;
        private Cursor c1, c2;
        private HSB HSBcol = new HSB();

        public Form1()
        {
            InitializeComponent();
            init();
            start();
            this.Show();
            Refresh();
        }

        public void init() // all instances will be prepared
        {
            HSBcol = new HSB();
            //setSize(640, 480);
            finished = false;
            //addMouseListener(this);
            //addMouseMotionListener(this);
            //c1 = new Cursor(Cursor.WAIT_CURSOR);
            //c2 = new Cursor(Cursor.CROSSHAIR_CURSOR);
            int x1 = 640;//x1 = getSize().width;
            int y1 = 480;//y1 = getSize().height;
            xy = (float)x1 / (float)y1;
            picture = new Bitmap(640, 480); // picture = createImage(x1, y1);
            g1 = Graphics.FromImage(picture);//g1 = picture.getGraphics();
            finished = true;
        }

        public void start()
        {
            action = false;
            rectangle = false;
            initvalues();
            xzoom = (xende - xstart) / (double)x1;
            yzoom = (yende - ystart) / (double)y1;
            mandelbrot();
        }

        public void paint(Graphics g)
        {
            update(g);
        }

        public void update(Graphics g)
        {
            g.DrawImage(picture, 0, 0);
            if (rectangle)
            {
                Pen myPen = new Pen(Color.White, 5); //create a pen object

                if (xs < xe)
                {
                    if (ys < ye) g.DrawRectangle(myPen, xs, ys, (xe - xs), (ye - ys));
                    else g.DrawRectangle(myPen, xs, ye, (xe - xs), (ys - ye));
                }
                else
                {
                    if (ys < ye) g.DrawRectangle(myPen, xe, ys, (xs - xe), (ye - ys));
                    else g.DrawRectangle(myPen, xe, ye, (xs - xe), (ys - ye));
                }
            }
        }

        private void mandelbrot() // calculate all points
        {
            int x, y;
            float h, b, alt = 0.0f;
            Pen myPen = new Pen(Color.Black);//(h, 0.8f, b)
            action = false;
            //setCursor(c1);
            //showStatus("Mandelbrot-Set will be produced - please wait...");
            for (x = 0; x < x1; x += 2)
                for (y = 0; y < y1; y++)
                {
                    h = pointcolour(xstart + xzoom * (double)x, ystart + yzoom * (double)y); // color value
                    if (h != alt)
                    {
                        b = 1.0f - h * h; // brightnes
                                          ///djm added
                                          ///HSBcol.fromHSB(h,0.8f,b); //convert hsb to rgb then make a Java Color
                                          ///Color col = new Color(0,HSBcol.rChan,HSBcol.gChan,HSBcol.bChan);
                                          ///g1.setColor(col);
                        //djm end
                        //djm added to convert to RGB from HSB

                        Pen myPen = new Pen(Color.Black);//(h, 0.8f, b)

                        g1.setColor(Color.getHSBColor(h, 0.8f, b));
                        //djm test
                        Color col = Color.getHSBColor(h, 0.8f, b);
                        Color red = col.getRed();
                        Color green = col.getGreen();
                        Color blue = col.getBlue();
                        //djm 
                        alt = h;
                    }
                    g1.DrawLine(myPen, x, y, x + 1, y);
                }
            showStatus("Mandelbrot-Set ready - please select zoom area with pressed mouse.");
            setCursor(c2);
            action = true;
        }

        private float pointcolour(double xwert, double ywert) // color value from 0.0 to 1.0 by iterations
        {
            double r = 0.0, i = 0.0, m = 0.0;
            int j = 0;

            while ((j < MAX) && (m < 4.0))
            {
                j++;
                m = r * r - i * i;
                i = 2.0 * r * i + ywert;
                r = m + xwert;
            }
            return (float)j / (float)MAX;
        }
    }
}
