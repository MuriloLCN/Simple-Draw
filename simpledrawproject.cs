using System;
using System.Windows.Forms;
using System.Drawing;

namespace SimpleDrawProject
{
    public class SimpleDraw
    {
        // PictureBox element to be drawn on
        public PictureBox canvas;

        public int frameCount = 0;
        public int deltaTime = 16;

        // Translation coefficients
        public int dx = 0;
        public int dy = 0;

        public Bitmap currentFrame;
        public Graphics graphics;

        public Color currentStrokeColor = Color.Black;
        public Color currentFillColor = Color.Black;

        public Color backgroundColor = Color.White;

        public bool fillState = true;
        public bool strokeState = true;

        public bool antiAlias = false;

        public int width;
        public int height;

        public bool resetAfterLoop = true;

        public Font textFont = new Font("Times New Roman", 12.0f);

        public SolidBrush BGbrush = new SolidBrush(Color.White);
        public SolidBrush fillBrush = new SolidBrush(Color.Black);
        public Pen strokePen = new Pen(new SolidBrush(Color.Black), 1);

        public int currentStrokeWeight = 1;

        // Temporary drawing state for between Push() and Pop()
        public bool isTempState = false;
        public Color tempStrokeColor = Color.Black;
        public Color tempFillColor = Color.Black;
        public Color tempBGColor = Color.White;
        public SolidBrush tempBGbrush = new SolidBrush(Color.White);
        public SolidBrush tempFillBrush = new SolidBrush(Color.Black);
        public Pen tempStrokePen = new Pen(new SolidBrush(Color.Black), 1);

        public int tempStrokeWeight = 1;

        private void getPenAndBrush(out Pen p, out SolidBrush b)
        {
            // Decides which pen and brush to use depending on isTempState
            if (!isTempState)
            {
                p = strokePen;
                b = fillBrush;
            }
            else
            {
                p = tempStrokePen;
                b = tempFillBrush;
            }
        }

        private void resetVariables()
        {
            // Resets all drawing variables to default state
            dx = 0;
            dy = 0;

            currentStrokeColor = Color.Black;
            currentFillColor = Color.Black;

            backgroundColor = Color.White;

            fillState = true;
            strokeState = true;

            textFont = new Font("Times New Roman", 12.0f);

            BGbrush = new SolidBrush(Color.White);
            fillBrush = new SolidBrush(Color.Black);
            strokePen = new Pen(new SolidBrush(Color.Black), 1);

            currentStrokeWeight = 1;

            isTempState = false;
            tempStrokeColor = Color.Black;
            tempFillColor = Color.Black;
            tempBGColor = Color.White;
            tempBGbrush = new SolidBrush(Color.White);
            tempFillBrush = new SolidBrush(Color.Black);
            tempStrokePen = new Pen(new SolidBrush(Color.Black), 1);
        }

        public void point(int x, int y)
        {
            // Draws a point at coordinates (x,y) with the current stroke color
            Pen st;
            if (!isTempState)
            {
                st = strokePen;
            }
            else
            {
                st = tempStrokePen;
            }
            graphics.DrawEllipse(st, x - dx, y - dy, 1, 1);
        }

        public void text(string s, int x, int y)
        {
            // Writes a string of text at the coordinates (x,y) with the current fill color

            SolidBrush b;
            if (!isTempState)
            {
                b = fillBrush;
            }
            else {
                b = tempFillBrush;  
            }

            graphics.DrawString(s, textFont, b, new Point(x - dx, y - dy));
        }

        public void clear()
        {
            // Clears the canvas to just the background color
            graphics.Clear(backgroundColor);
        }

        public void background(Color c)
        {
            // Changes the background color and draws it (i.e, clears the screen)
            if (!isTempState)
            {
                backgroundColor = c;
                BGbrush = new SolidBrush(c);
                
            }
            else
            {
                tempBGColor = c;
                tempBGbrush = new SolidBrush(c);
            }
            clear();
        }

        public void fill(Color c)
        {
            // Changes the fill state to true and sets the fill color
            // While the fill state is true, all closed figures (squares, circles, etc) will have their areas filled in with this color
            fillState = true;
            if (!isTempState)
            {
                currentFillColor = c;
                fillBrush = new SolidBrush(c);
            }
            else
            {
                tempFillColor = c;
                tempFillBrush = new SolidBrush(c);
            }
        }

        public void changeFont(string name, float size)
        {
            // Changes the font used for text
            textFont = new Font(name, size);
        }

        public void noFill()
        {
            // Sets the fill state to false
            // All figures drawn while fill state is false will only have their perimeters drawn
            fillState = false;
        }

        public void translate(int x, int y)
        {
            // Changes the origin point of the coordinate system
            dx = x;
            dy = y;
        }

        public void strokeWeight(int s)
        {
            // Changes the stroke weight of the pen
            // Bigger stroke weight means bigger edges on figures drawn
            if (!isTempState)
            {
                strokePen = new Pen(currentStrokeColor, s);
                currentStrokeWeight = s;
            }
            else
            {
                tempStrokePen = new Pen(tempStrokeColor, s);
                tempStrokeWeight = s;
            }
        }

        public void stroke(Color c)
        {
            // sets the stroke state to true and sets the stroking color
            // Figures drawn while stroke state is true will have their perimeters drawn to the screen
            strokeState = true;
            if (!isTempState)
            {
                currentStrokeColor = c;
                strokePen = new Pen(new SolidBrush(c), currentStrokeWeight);
            }
            else
            {
                tempStrokeColor = c;
                tempStrokePen = new Pen(new SolidBrush(c), tempStrokeWeight);
            }
        }

        public void noStroke()
        {
            // Sets the stroke state to false
            // Figures drawn while stroke state is false will not have their perimeters drawn to the screen
            strokeState = false;
        }

        public void frameRate(int frames)
        {
            // Sets the frame rate for the draw() function
            // Note: This must be done in setup()
            deltaTime = (int)Math.Floor((double)1000 / frames);
            if (deltaTime <= 0)
            {
                deltaTime = 1;
            }
        }

        public void circle(int x, int y, int r)
        {
            // Draws a circle at position (x,y) with radius r
            Pen p;
            SolidBrush sb;
            getPenAndBrush(out p, out sb);

            if (fillState)
            {
                graphics.FillEllipse(sb, x - dx, y - dy, 2 * r, 2 * r);
            }
            if (strokeState)
            {
                graphics.DrawEllipse(p, x - dx, y - dy, 2 * r, 2 * r);
            }
        }

        public void ellipse(int x, int y, int w, int h)
        {
            // Draws an ellipse at (x,y) delimited by a rect with sides (w,h)
            Pen p;
            SolidBrush sb;

            getPenAndBrush(out p, out sb);

            if (fillState)
            {
                graphics.FillEllipse(sb, x - dx, y - dy, w, h);
            }
            if (strokeState)
            {
                graphics.DrawEllipse(p, x - dx, y - dy, w, h);
            }
        }

        public void line(int x1, int y1, int x2, int y2)
        {
            // Draws a line between two points (x1,y1) and (x2,y2)
            Pen p;
            if (!isTempState)
            {
                p = strokePen;
            }
            else
            {
                p = tempStrokePen;
            }

            if (strokeState)
                graphics.DrawLine(p, x1-dx, y1-dy, x2-dx, y2-dy);
        }

        public void rect(int x, int y, int w, int h)
        {
            // Draws a rectangle at the position (x,y) with sides (w,h)
            Pen p;
            SolidBrush sb;
            getPenAndBrush(out p, out sb);

            if (fillState)
            {
                graphics.FillRectangle(sb, x-dx, y-dy, w, h);
            }
            if (strokeState)
            {
                graphics.DrawRectangle(p, x-dx, y-dy, w, h);
            }
        }

        public void square(int x, int y, int s)
        {
            // Draws a square at position (x,y) with sides s
            Pen p;
            SolidBrush sb;
            getPenAndBrush(out p, out sb);

            if (fillState)
            {
                graphics.FillRectangle(sb, x-dx, y-dy, s, s);
            }
            if (strokeState)
            {
                graphics.DrawRectangle(p, x-dx, y-dy, s, s);
            }
        }

        public void triangle(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            // Draws a triangle from three vertices (x1,y1),(x2,y2) and (x3,y3)
            Pen p;
            SolidBrush sb;
            getPenAndBrush(out p, out sb);

            Point[] matrix = { new Point(x1-dx, y1-dy), new Point(x2-dx, y2-dy), new Point(x3-dx, y3-dy) };
            if (fillState)
            {
                graphics.FillPolygon(sb, matrix);
            }
            if (strokeState)
            {
                graphics.DrawPolygon(p, matrix);
            }
        }

        public void polygon(Point[] points)
        {
            // Draws a polygon from a points matrix
            Pen p;
            SolidBrush sb;
            getPenAndBrush(out p, out sb);

            for (int i = 0; i < points.Length; i++)
            {
                points[i].X -= dx;
                points[i].Y -= dy;
            }

            if (fillState)
            {
                graphics.FillPolygon(sb, points);
            }
            if (strokeState)
            {
                graphics.DrawPolygon(p, points);
            }
        }

        public void toggleAntiAlias()
        {
            // Toggles anti aliasing on or off
            antiAlias = !antiAlias;
        }

        public void push()
        {
            // Sets the temporary state to true
            // All changes to colors done while temp state is on will be reverted back upon leaving
            isTempState = true;
            tempBGbrush = BGbrush;
            tempBGColor = backgroundColor;
            tempFillBrush = fillBrush;
            tempFillColor = currentFillColor;
            tempStrokeColor = currentStrokeColor;
            tempStrokePen = strokePen;
        }

        public void pop()
        {
            // Leaves temp state and reverts back color changes made while it was on
            isTempState = false;
        }

        private Action drawAct;

        private void TimerTick(object sender, EventArgs e)
        {
            currentFrame = new Bitmap(width, height);
            graphics = Graphics.FromImage(currentFrame);
            
            if (antiAlias)
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            }
            else
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            }

            frameCount++;
 
            drawAct();
            canvas.Image = (Image)currentFrame;
            GC.Collect();
            if (resetAfterLoop)
            {
                resetVariables();
            }
        }
        void protoDraw(Action draw)
        {
            Timer drawEvent = new System.Windows.Forms.Timer();
            drawAct = draw;
            drawEvent.Tick += new EventHandler(TimerTick);
            drawEvent.Interval = deltaTime;
            drawEvent.Start();
        }

        void protoSetup(Action setup)
        {
            setup();
        }

        public void start(Action setup, Action draw, PictureBox img)
        {
            canvas = img;
            width = img.Width;
            height = img.Height;
            protoSetup(setup);
            protoDraw(draw);
        }
    }
}
