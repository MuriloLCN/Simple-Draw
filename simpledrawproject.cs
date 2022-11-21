using System;
using System.Windows.Forms;
using System.Drawing;

namespace SimpleDrawProject
{
    public class SimpleDraw
    { 
        public enum DRAW_MODE {TOP_LEFT, CENTER};

        // Used to access the enum from outside of this class
        public DRAW_MODE mode_center = DRAW_MODE.CENTER;
        public DRAW_MODE mode_top_left = DRAW_MODE.TOP_LEFT;

        // PictureBox element to be drawn on
        public PictureBox canvas;

        public DRAW_MODE currentRectMode = DRAW_MODE.TOP_LEFT;
        public DRAW_MODE currentCircleMode = DRAW_MODE.CENTER;

        public int frameCount = 0;
        public int deltaTime = 16;

        // Translation coefficients
        public int dx = 0;
        public int dy = 0;

        public Bitmap currentFrame;
        public Graphics graphics;

        public Color currentStrokeColor = Color.Black;
        public Color currentFillColor = Color.Black;

        public Color? backgroundColor = Color.White;

        public bool fillState = true;
        public bool strokeState = true;

        public bool antiAlias = false;

        public int width;
        public int height;

        private bool isPaused = false;
        private bool quitted = false;

        private int originalWidth;
        private int originalHeight;

        public double zoomFactor = 1;

        public bool resetAfterLoop = true;

        public Font textFont = new Font("Times New Roman", 12.0f);

        public SolidBrush fillBrush = new SolidBrush(Color.Black);
        public Pen strokePen = new Pen(new SolidBrush(Color.Black), 1);

        public int currentStrokeWeight = 1;

        // Temporary drawing state for between Push() and Pop()
        public bool isTempState = false;
        public Color tempStrokeColor = Color.Black;
        public Color tempFillColor = Color.Black;
        public Color? tempBGColor = Color.White;
        public SolidBrush tempFillBrush = new SolidBrush(Color.Black);
        public Pen tempStrokePen = new Pen(new SolidBrush(Color.Black), 1);
        public DRAW_MODE tempRectMode;
        public DRAW_MODE tempCircleMode;
        public Font tempTextFont;

        public int tempStrokeWeight = 1;

        public void rectMode(DRAW_MODE r)
        {
            currentRectMode = r;
        }

        public void circleMode(DRAW_MODE c)
        {
            currentCircleMode = c;
        }

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

        private void getCircleOffset(out int kx, out int ky, int w, int h)
        {
            // Gets extra offset for x,y coordinates if circle is in CENTER mode
            if (isTempState)
            {
                if (tempCircleMode == DRAW_MODE.CENTER)
                {
                    kx = -w / 2;
                    ky = -h / 2;
                    return;
                }
            }
            else
            {
                if (currentCircleMode == DRAW_MODE.CENTER)
                {
                    kx = -w / 2;
                    ky = -h / 2;
                    return;
                }
            }
            kx = 0;
            ky = 0;
        }

        private void getRectOffset(out int kx, out int ky, int w, int h)
        {
            // gets extra offset for rects if current rect mode is CENTER
            if (isTempState)
            {
                if (tempRectMode == DRAW_MODE.CENTER)
                {
                    kx = -w / 2;
                    ky = -h / 2;
                    return;
                }
            }
            else
            {
                if (currentRectMode == DRAW_MODE.CENTER)
                {
                    kx = -w / 2;
                    ky = -h / 2;
                    return;
                }
            }
            kx = 0;
            ky = 0;
        }

        private void resetVariables()
        {
            // Resets all drawing variables to default state

            currentCircleMode = DRAW_MODE.CENTER;
            currentRectMode = DRAW_MODE.TOP_LEFT;
            
            dx = 0;
            dy = 0;

            currentStrokeColor = Color.Black;
            currentFillColor = Color.Black;

            backgroundColor = Color.White;

            fillState = true;
            strokeState = true;

            textFont = new Font("Times New Roman", 12.0f);

            fillBrush = new SolidBrush(Color.Black);
            strokePen = new Pen(new SolidBrush(Color.Black), 1);

            currentStrokeWeight = 1;

            isTempState = false;
            tempStrokeColor = Color.Black;
            tempFillColor = Color.Black;
            tempBGColor = Color.White;
            tempFillBrush = new SolidBrush(Color.Black);
            tempStrokePen = new Pen(new SolidBrush(Color.Black), 1);
            tempStrokeWeight = 1;
            tempRectMode = DRAW_MODE.TOP_LEFT;
            tempCircleMode = DRAW_MODE.CENTER;
            tempTextFont = textFont;
        }

        public void zoom(double zoomFactorCurrent)
        {
            width = (int)Math.Floor(originalWidth * zoomFactorCurrent);
            height = (int)Math.Floor(originalHeight * zoomFactorCurrent);

            zoomFactor = zoomFactorCurrent;
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

            Font usedFont;

            if (isTempState)
            {
                usedFont = tempTextFont;
            }
            else
            {
                usedFont = textFont;
            }

            int kx = 0;
            int ky = 0;
            SizeF stringSize = graphics.MeasureString(s, usedFont);

            getRectOffset(out kx, out ky, (int)stringSize.Width, (int)stringSize.Height);

            SolidBrush b;
            if (!isTempState)
            {
                b = fillBrush;
            }
            else {
                b = tempFillBrush;  
            }

            graphics.DrawString(s, textFont, b, new Point(x - dx + kx, y - dy + ky));
        }

        public void clear()
        {
            // Clears the canvas to just the background color
            Color? current;
            if (isTempState)
            {
                current = tempBGColor;
            }
            else
            {
                current = backgroundColor;
            }

            if (current != null)
            {
                graphics.Clear((Color)current);
            }
            else
            {
                currentFrame.Dispose();
                graphics.Dispose();
                currentFrame = new Bitmap(width, height);
                graphics = Graphics.FromImage(currentFrame);
                GC.Collect();
            }
        }

        public void background(Color? c)
        {
            // Changes the background color and draws it (i.e, clears the screen)
            if (isTempState)
            {
                tempBGColor = c;
            }
            else
            {
                backgroundColor = c;
            }
            
            clear();
            GC.Collect();
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
            if (isTempState)
            {
                tempTextFont = new Font(name, size);
                return;
            }
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
            int kx = 0;
            int ky = 0;
            getCircleOffset(out kx, out ky, 2 * r, 2 * r);

            if (fillState)
            {
                graphics.FillEllipse(sb, x - dx + kx, y - dy + ky, 2 * r, 2 * r);
            }
            if (strokeState)
            {
                graphics.DrawEllipse(p, x - dx + kx, y - dy + ky, 2 * r, 2 * r);
            }
        }

        public void ellipse(int x, int y, int w, int h)
        {
            // Draws an ellipse at (x,y) delimited by a rect with sides (w,h)
            Pen p;
            SolidBrush sb;

            int kx = 0;
            int ky = 0;
            getCircleOffset(out kx, out ky, w, h);

            getPenAndBrush(out p, out sb);

            if (fillState)
            {
                graphics.FillEllipse(sb, x - dx + kx, y - dy + ky, w, h);
            }
            if (strokeState)
            {
                graphics.DrawEllipse(p, x - dx + kx, y - dy + ky, w, h);
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

            int kx = 0;
            int ky = 0;
            getRectOffset(out kx, out ky, w, h);

            if (fillState)
            {
                graphics.FillRectangle(sb, x-dx+kx, y-dy+ky, w, h);
            }
            if (strokeState)
            {
                graphics.DrawRectangle(p, x-dx+kx, y-dy+ky, w, h);
            }
        }

        public void square(int x, int y, int s)
        {
            // Draws a square at position (x,y) with sides s
            Pen p;
            SolidBrush sb;
            getPenAndBrush(out p, out sb);

            int kx = 0;
            int ky = 0;
            getRectOffset(out kx, out ky, s, s);

            if (fillState)
            {
                graphics.FillRectangle(sb, x-dx+kx, y-dy+ky, s, s);
            }
            if (strokeState)
            {
                graphics.DrawRectangle(p, x-dx+kx, y-dy+ky, s, s);
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

        public void image(Image img, int x, int y)
        {
            // Draws an image to the screen at position x,y
            int kx = 0;
            int ky = 0;
            getRectOffset(out kx, out ky, img.Width, img.Height);
            
            graphics.DrawImage(img, new Point(x+kx-dx,y+ky-dy));
        }
        
        public void image(Image img, int x, int y, int w, int h)
        {
            // Draws an image to the screen at position x,y with sizes w,h

            int kx = 0;
            int ky = 0;
            getRectOffset(out kx, out ky, w, h);

            graphics.DrawImage(img, new Rectangle(x+kx-dx, y+ky-dy, w, h));
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
            tempBGColor = backgroundColor;
            tempFillBrush = fillBrush;
            tempFillColor = currentFillColor;
            tempStrokeColor = currentStrokeColor;
            tempStrokePen = strokePen;
            tempCircleMode = currentCircleMode;
            tempRectMode = currentRectMode;
            tempTextFont = textFont;
        }

        public void pop()
        {
            // Leaves temp state and reverts back color changes made while it was on
            isTempState = false;
        }

        public void pause()
        {
            isPaused = true;
        }

        public void unpause()
        {
            isPaused = false;
        }

        public void quit()
        {
            quitted = true;
        }

        public Point mousePos(Form f)
        {
            // Gets mouse position in the screen

            Point absolutePos = System.Windows.Forms.Cursor.Position;
            Point relativeForm = f.PointToClient(absolutePos);
            relativeForm.X -= canvas.Location.X;
            relativeForm.Y -= canvas.Location.Y;
            relativeForm.X += dx;
            relativeForm.Y += dy;
            relativeForm.X = (int)(relativeForm.X * zoomFactor);
            relativeForm.Y = (int)(relativeForm.Y * zoomFactor);

            return relativeForm;
        }

        private Action drawAct;

        private void TimerTick(object sender, EventArgs e)
        {
            if (isPaused)
            {
                return;
            }
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
            if (quitted)
            {
                drawEvent.Stop();
                GC.Collect();
            }
        }

        Timer drawEvent = new System.Windows.Forms.Timer();

        void protoDraw(Action draw)
        {
            
            drawAct = draw;
            drawEvent.Tick += new EventHandler(TimerTick);
            drawEvent.Interval = deltaTime;
            drawEvent.Start();
        }

        void protoSetup(Action setup)
        {
            originalHeight = height;
            canvas.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            originalWidth = width;
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
