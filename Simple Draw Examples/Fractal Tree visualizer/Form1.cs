using SimpleDrawProject;

namespace FractalTree
{
    public partial class Form1 : Form
    {
        SimpleDraw s = new SimpleDraw();
        public Form1()
        {
            InitializeComponent();
            s.start(setup, draw, myCanvas);
        }

        void draw()
        {
            s.translate(currentTranslate.X,currentTranslate.Y);
            s.zoom(zoomBar.Value * 0.1);
            getData();
            s.background(Color.White);
            drawTree(s.width / 2, s.height, size, Math.PI / 2);
            s.text("angle = " + (angle*360/(Math.PI * 2)).ToString(), 0, 0);
            s.text("size = " + size.ToString(), 0, 15);
            s.text("n branches = " + numberOfBranches.ToString(), 0, 30);
            s.text("resize factor = " + resizeFactor.ToString(), 0, 45);
        }

        void getData()
        {
            numberOfBranches = branchBar.Value;
            size = (int)(sizeBar.Value * 2);
            angle = (angleBar.Value * 2 * Math.PI) / 360;
            minSize = minSizeBar.Value;
            resizeFactor = (double)resizeFactorBar.Value * 0.1;
        }

        int numberOfBranches = 2;
        int size = 200;
        double angle = Math.PI / 2;
        int minSize = 4;
        double resizeFactor = 0.5;

        Point currentTranslate = new Point(0,0);
        void drawTree(double x, double y, double currentSize, double currentAngle)
        {
            if (currentSize < minSize)
            {
                return;
            }

            double newX, newY;

            newX = x + currentSize * Math.Cos(currentAngle);
            newY = y - currentSize * Math.Sin(currentAngle);


            s.line((int)x, (int)y, (int)newX, (int)newY);

            double deltaAngle = (2 * angle) / (numberOfBranches - 1);
            double minAngle = currentAngle - angle;

            for (int i = 0; i < numberOfBranches; i++)
            {
                drawTree(newX, newY, currentSize * resizeFactor, minAngle + i * deltaAngle);
            }

        }

        void setup()
        {
            s.frameRate(15);
            s.toggleAntiAlias();
        }

        private void myCanvas_Click(object sender, EventArgs e)
        {
            Point pos = s.mousePos(this);
            currentTranslate.X += (int)((pos.X - s.width / 2) * 0.1);
            currentTranslate.Y += (int)((pos.Y - s.height / 2) * 0.1);
        }
    }
}