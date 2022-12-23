using System;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleDrawProject
{
    public class SimpleDraw
    {
        public enum DRAW_MODE { TOP_LEFT, CENTER };

        // Used to access the enum from outside of this class
        public DRAW_MODE mode_center = DRAW_MODE.CENTER;
        public DRAW_MODE mode_top_left = DRAW_MODE.TOP_LEFT;

        // PictureBox element to be drawn on
        public PictureBox canvas;

        public DRAW_MODE currentRectMode = DRAW_MODE.TOP_LEFT;
        public DRAW_MODE currentCircleMode = DRAW_MODE.CENTER;

        public int frameCount = 0;
        public int deltaTime = 16;

        public Point translationCoefficients = new Point(0, 0);
        public float[] scalingCoefficients = {1,1};

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

        public float rotationAngle = 0.0F;

        public bool resetAfterLoop = true;
        public bool mousePosUseRealPosition = false;

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

        public float tempRotationAngle = 0.0F;
        public float[] tempScalingCoefficients = { 1, 1 };
        public Point tempTranslationCoefficients = new Point(0, 0);


        public int tempStrokeWeight = 1;

        public void rectMode(DRAW_MODE r)
        {
            // Changes rect mode
            currentRectMode = r;
        }

        public void circleMode(DRAW_MODE c)
        {
            // Change circle mode
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

            translationCoefficients = new Point(0, 0);
            rotationAngle = 0;
            scalingCoefficients[0] = 1;
            scalingCoefficients[1] = 1;

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

            tempRotationAngle = 0;
            tempScalingCoefficients[0] = 1;
            tempScalingCoefficients[1] = 1;
            tempTranslationCoefficients = new Point(0, 0);

        }

        public void resetTransformations()
        {
            // Resets all matrix transformations in current state
            if (!isTempState)
            {
                translationCoefficients = new Point(0, 0);
                rotationAngle = 0;
                scalingCoefficients[0] = 1;
                scalingCoefficients[1] = 1;
            }
            else
            {
                tempTranslationCoefficients = new Point(0, 0);
                tempRotationAngle = 0;
                tempScalingCoefficients[0] = 1;
                tempScalingCoefficients[1] = 1;
            }

            graphics.ResetTransform();
        }

        public void scale(float scaleX, float scaleY)
        {
            // Sets the scaling of the screen to a desired size
            if (!isTempState)
            {
                graphics.ScaleTransform(1 / scalingCoefficients[0], 1 / scalingCoefficients[1]);
                scalingCoefficients[0] = scaleX;
                scalingCoefficients[1] = scaleY;
            }
            else
            {
                graphics.ScaleTransform(1 / tempScalingCoefficients[0], 1 / tempScalingCoefficients[1]);
                tempScalingCoefficients[0] = scaleX;
                tempScalingCoefficients[1] = scaleY;
            }
            graphics.ScaleTransform(scaleX, scaleY);

        }

        public void cumulativeScale(float scaleX, float scaleY)
        {
            // Scales the screen relative to the previous size
            if (!isTempState)
            {
                scale(scalingCoefficients[0] * scaleX, scalingCoefficients[1] * scaleY);
            }
            else
            {
                scale(tempScalingCoefficients[0] * scaleX, tempScalingCoefficients[1] * scaleY);
            }
        }

        public void zoom(float zoomFactorCurrent)
        {
            // Sets scale for both axis to a desired value
            scale(zoomFactorCurrent, zoomFactorCurrent);
        }

        public void cumulativeZoom(float zoomFactor)
        {
            // Multiplies the scale for both axis by a given value
            cumulativeScale(zoomFactor, zoomFactor);
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
            graphics.DrawEllipse(st, x, y, 1, 1);
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

            // Offsets change based on drawMode
            int kx = 0;
            int ky = 0;
            SizeF stringSize = graphics.MeasureString(s, usedFont);

            getRectOffset(out kx, out ky, (int)stringSize.Width, (int)stringSize.Height);

            SolidBrush b;
            if (!isTempState)
            {
                b = fillBrush;
            }
            else
            {
                b = tempFillBrush;
            }

            graphics.DrawString(s, textFont, b, new Point(x + kx, y + ky));
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

        public void rotate(float angle)
        {
            // Sets the rotation to a given angle
            if (!isTempState)
            {
                graphics.RotateTransform(-rotationAngle);
                graphics.RotateTransform(angle);
                rotationAngle = angle;
            }
            else
            {
                graphics.RotateTransform(-tempRotationAngle);
                graphics.RotateTransform(angle);
                tempRotationAngle = angle;
            }
        }

        public void cumulativeRotate(float angle)
        {
            // Adds the given angle to previously added angles and rotates by their total
            if (!isTempState)
                rotate(rotationAngle + angle);
            else
                rotate(tempRotationAngle + angle);
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
            if (!isTempState)
            {
                graphics.TranslateTransform(-translationCoefficients.X, -translationCoefficients.Y);
                translationCoefficients = new Point(x, y);
            }
            else
            {
                graphics.TranslateTransform(-tempTranslationCoefficients.X, -tempTranslationCoefficients.Y);
                tempTranslationCoefficients = new Point(x, y);
            }

            graphics.TranslateTransform(x, y);
        }

        public void cumulativeTranslate(int x, int y)
        {
            // Adds to the origin point of the coordinate system
            if (!isTempState)
            {
                translate(translationCoefficients.X + x, translationCoefficients.Y + y);
            }
            else
            {
                translate(tempTranslationCoefficients.X + x, tempTranslationCoefficients.Y + y);
            }

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
            getPenAndBrush(out Pen p, out SolidBrush sb);
            getCircleOffset(out int kx, out int ky, 2 * r, 2 * r);

            if (fillState)
            {
                graphics.FillEllipse(sb, x + kx, y + ky, 2 * r, 2 * r);
            }
            if (strokeState)
            {
                graphics.DrawEllipse(p, x + kx, y + ky, 2 * r, 2 * r);
            }
        }

        public void ellipse(int x, int y, int w, int h)
        {
            // Draws an ellipse at (x,y) delimited by a rect with sides (w,h)
            getPenAndBrush(out Pen p, out SolidBrush sb);
            getCircleOffset(out int kx, out int ky, w, h);

            if (fillState)
            {
                graphics.FillEllipse(sb, x + kx, y + ky, w, h);
            }
            if (strokeState)
            {
                graphics.DrawEllipse(p, x + kx, y + ky, w, h);
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
                graphics.DrawLine(p, x1, y1, x2, y2);
        }

        public void rect(int x, int y, int w, int h)
        {
            // Draws a rectangle at the position (x,y) with sides (w,h)
            getPenAndBrush(out Pen p, out SolidBrush sb);
            getRectOffset(out int kx, out int ky, w, h);

            if (fillState)
            {
                graphics.FillRectangle(sb, x + kx, y + ky, w, h);
            }
            if (strokeState)
            {
                graphics.DrawRectangle(p, x + kx, y + ky, w, h);
            }
        }

        public void square(int x, int y, int s)
        {
            // Draws a square at position (x,y) with sides s
            getPenAndBrush(out Pen p, out SolidBrush sb);

            getRectOffset(out int kx, out int ky, s, s);

            if (fillState)
            {
                graphics.FillRectangle(sb, x + kx, y + ky, s, s);
            }
            if (strokeState)
            {
                graphics.DrawRectangle(p, x + kx, y + ky, s, s);
            }
        }

        public void triangle(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            // Draws a triangle from three vertices (x1,y1),(x2,y2) and (x3,y3)
            getPenAndBrush(out Pen p, out SolidBrush sb);

            Point[] matrix = { new Point(x1, y1), new Point(x2, y2), new Point(x3, y3) };
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
            getPenAndBrush(out Pen p, out SolidBrush sb);

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
            getRectOffset(out int kx, out int ky, img.Width, img.Height);

            graphics.DrawImage(img, new Point(x + kx, y + ky));
        }

        public void image(Image img, int x, int y, int w, int h)
        {
            // Draws an image to the screen at position x,y with sizes w,h
            getRectOffset(out int kx, out int ky, w, h);

            graphics.DrawImage(img, new Rectangle(x + kx, y + ky, w, h));
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

            tempRotationAngle = rotationAngle;
            tempScalingCoefficients = scalingCoefficients;
            tempTranslationCoefficients = translationCoefficients;
        }

        public void pop()
        {
            // Leaves temp state and reverts back color changes made while it was on
            isTempState = false;
            cumulativeTranslate(-tempTranslationCoefficients.X, -tempTranslationCoefficients.Y);
            rotate(rotationAngle);
            scale(scalingCoefficients[0], scalingCoefficients[1]);
        }

        public void pause()
        {
            // Pauses the simulation
            isPaused = true;
        }

        public void unpause()
        {
            // Resumes the simulation
            isPaused = false;
        }

        public void quit()
        {
            // Quits the simulation
            quitted = true;
        }
        public Point mousePos(Form f)
        {
            // Gets mouse position in the screen
            // NOTE: Rotation still skews the mouse position, could not get over this yet

            Point absolutePos = System.Windows.Forms.Cursor.Position;
            Point relativeForm = f.PointToClient(absolutePos);

            relativeForm.X -= canvas.Location.X;
            relativeForm.Y -= canvas.Location.Y;

            relativeForm.X = (int)(relativeForm.X * (1/scalingCoefficients[0]));
            relativeForm.Y = (int)(relativeForm.Y * (1/scalingCoefficients[1]));

            return relativeForm;
        }

        private Action drawAct;

        private void TimerTick(object sender, EventArgs e)
        {
            // Behind the scenes of draw()
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
            canvas.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
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
