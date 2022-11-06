# Simple-Draw

  This library is aimed to provide a simplified experience for using the Graphic library in C# (.Net) to modify and update a picture box in a Windows Forms app.
  
  The general design of the library structure is heavily (if not all) inspired by the [P5.js](https://p5js.org/) library and the Processing library, and is intended to make simple graphical programs easy to do in the shape of a Windows Forms app using the C# language and all it offers, alongside all of the flexibility of the .NET environment.
  
 ## Installation
 
  In order to use this library, you can either download the ```simpledrawproject.cs``` file and include it in your code or, alternatively, you can copy the ```SimpleDraw``` class anywhere into your app's namespace to use it.
  
 ## Setting up
 
  In order to begin using the SimpleDraw class to make your drawings, you need to do a few steps first:
  
  #### 0. Make sure you've got System.Windows.Forms and System.Drawing in your project
  
  These are the prerequisites for running this, and should be the first thing you check for before moving forward.
  
  #### 1. Create an instance of the SimpleDraw class
  
  This can be done by simply creating a new SimpleDraw variable like this:
    
    SimpleDraw s = new SimpleDraw();
    
  #### 2. Create a PictureBox element in your form
  
On Visual Studio, this can be done in the designer tab of your form by simply selecting a PictureBox element in the Toolbox tab and dropping it into your form, then all you need is to shape it anyway you want in your window and give it a name. You should have something like this:
  
  ![picboxpic1](https://user-images.githubusercontent.com/88753590/200151960-d3f8d32d-5e62-4a9f-9493-0c61a2cadb20.PNG)
  
  #### 3. Create a ```setup()``` and a ```draw()``` function

These functions are the core of your drawing. ```setup()``` runs once and before ```draw()```, and it is used to set up the environment for the drawings that you want to do. Some things (like the frame rate) must be set in here, before the drawing actually begins. 
  
```draw()```, on the other hand, runs every frame (by default, every 16ms), and is where you will give life to your graphics. Here is where every shape and element will get drawn to the screen.

  #### 4. Call ```s.start(setup, draw, pictureBox)```
 
Once you run this method on the instance you made for the SimpleDraw class, the simulation will start and keep running until you give it a halt command.
You need to pass the ```setup()``` and ```draw()``` functions you made, and also the ```pictureBox``` element to which the drawing will occur.

The general structure of your form code may look something like this:

```
namespace YourNameSpace
{
    public partial class FormName : Form
    {
        SimpleDraw s = new SimpleDraw();
        
        public FormName()
        {
            InitializeComponent();
            s.start(setup, draw, pictureBox1);
            // Other stuff also goes here
        }

        void setup()
        {
            // Setting up goes here
        }
        void draw()
        {
            // Drawing goes here
        }

        // Other stuff
    }
}
```

## Getting started (with a simple example)

Now that you have your canvas ready for the show, it's time to actually draw something on the screen.
To begin with, let's turn on anti-aliasing so our canvas looks smoother, and let's draw a circle on the screen with a beige background.
Our code might look something like this:

```
void setup()
{
    s.toggleAntiAlias();
}
void draw()
{
    s.background(Color.Beige);
    s.circle(s.width / 2, s.height / 2, 24);
}
```

And our canvas looks like this:

![pic1](https://user-images.githubusercontent.com/88753590/200152462-ced934a0-1193-495c-a6a6-8a9619c607fa.PNG)

That's our circle! But this is not very interesting, so let's give it a red filling color, a blue outline and make it move around.

```
void setup()
{
    s.toggleAntiAlias();
}

int velocity = 5;
Point ballPosition = new Point(50,50);
void draw()
{
    s.background(Color.Beige);
    s.fill(Color.Red);
    s.stroke(Color.Blue);
    ballPosition.X += velocity;
    ballPosition.Y += velocity;
    s.circle(ballPosition.X, ballPosition.Y, 24);
}
```

![pic2](https://user-images.githubusercontent.com/88753590/200152681-30018274-a818-41f3-af71-a988966d3e28.PNG)

Now we have a moving red ball :D, but it leaves the screen fairly quickly, so let's make it bounce around.

```
void setup()
{
    width = s.width;
    height = s.height;
    s.toggleAntiAlias();
}

int x_velocity = 5;
int y_velocity = 5;
Point ballPosition = new Point(50,50);
int width;
int height;
void draw()
{
    s.background(Color.Beige);
    s.fill(Color.Red);
    s.stroke(Color.Blue);

    if (ballPosition.Y < 0 || ballPosition.Y + 42 > height)
    {
        y_velocity *= -1;
    }
    if (ballPosition.X < 0 || ballPosition.X + 42 > width)
    {
        x_velocity *= -1;
    }

    ballPosition.X += x_velocity;
    ballPosition.Y += y_velocity;

    s.circle(ballPosition.X, ballPosition.Y, 24);
}
```
![gif](https://user-images.githubusercontent.com/88753590/200155629-56506076-f47e-4f27-8389-0178632b08f2.gif)

(Note: This is a compressed GIF loop, the quality is far worse from the real sketch)

And just like that, we've got a ball that bounces around like a DVD screensaver (I wonder when it'll hit the corners...), you should try for yourself. 

This example was just to give you an idea of how you can work with this library, and there is so much you can do with this, it's all up to you.

## Documentation
Here are the components that make up the SimpleDraw class and how you can use them.

## Variables

Here are the variables that compose the SimpleDraw class, they are public and can be retrieved and changed directly, although it is not recommended for most of them that you the latter, and instead call the appropriate method to do so (unless explicitly dictated), as there are behaviors that need to happen during those calls.

## Simulation variables

#### SimpleDraw.canvas : PictureBox
The PictureBox element in which the final generated frame will be displayed at. Initially given by the user with the ```start()``` method.

#### SimpleDraw.frameCount : Int
The count of how many frames have passed since the start of the simulation.

#### SimpleDraw.deltaTime : Int 
The time interval between frames (in ms). Changing this mid-run will not change the speed of the simulation.

#### SimpleDraw.currentFrame : Bitmap
The current frame image being generated.

#### SimpleDraw.graphics : Graphics
The current graphics object used to draw onto the currentFrame.

#### SimpleDraw.fillState : Bool
Whether the shapes that are being drawn should or not be filled in. Default: true

#### SimpleDraw.strokeState : Bool 
Whether the shapes that are being drawn should or not have their outlines (perimeters) drawn. Default: true

#### SimpleDraw.antiAlias : Bool
Whether anti-aliasing is on or off. Default: false

#### SimpleDraw.width : Int
The width of the canvas. Default: The same width of the PictureBox passed in.
#### SimpleDraw.height : Int
The height of the canvas. Default: The same height of the PictureBox passed in.

## Drawing variables

#### SimpleDraw.dx : Int 
The translation in the x axis for the origin. Default: 0
#### SimpleDraw.dy : Int 
The translation in the y axis for the origin. Default: 0

#### SimpleDraw.currentStrokeColor : Color
The current color used for drawing points, lines and the edges of shapes. Default: Color.Black
#### SimpleDraw.currentFillColor : Color
The current color used for filling in shapes. Default: Color.Black

#### SimpleDraw.backgroundColor : Color 
The current background color. Default: Color.White

#### SimpleDraw.textFont : Font
The current font used for text. Default: Times New Roman, 12

#### SimpleDraw.BGbrush : SolidBrush
The current brush used for the background. Same color as backgroundColor

#### SimpleDraw.fillBrush : SolidBrush
The current brush used for filling shapes. Same color as currentFillColor

#### SimpleDraw.strokePen : Pen 
The current pen used to draw points, lines and edges of shapes. Same color as currentStrokeColor

## Temporary state variables

#### SimpleDraw.isTempState : Bool
Whether or not the drawing is in temporary state (i.e, when push() is called). Default: false

#### SimpleDraw.tempStrokeColor : Color
The current temporary stroke color. Default: currentStrokeColor

#### SimpleDraw.tempFillColor : Color
The current temporary fill color. Default: currentFillColor

#### SimpleDraw.tempBGColor : Color
The current temporary background color. Default: backgroundColor

#### SimpleDraw.tempBGbrush : SolidBrush
The current temporary background brush. Default: BGbrush

#### SimpleDraw.tempFillBrush : SolidBrush
The current temporary filling brush. Default: fillBrush

#### SimpleDraw.tempStrokePen : Pen
The current temporary stroking pen. Default: strokePen

### Methods

The methods used to control and draw stuff to the screen.

### State methods

These methods don't directly draw onto the screen, but rather control aspects of the simulation.

#### void changeFont(string name, float size)
Changes the font used in the canvas. 

Parameters:
string name: The name of the font (e.g: "Time New Roman")

float size: The size of the font (e.g: 15.0f)

#### void translate(int x, int y)
Changes the position of the origin.

Parameters:

int x; int y: The new x,y coordinates of the origin point.

#### void noFill()
Makes all shapes drawn after this call to not be filled in.

#### void fill(Color c)
Makes all shapes drawn after this call to be filled in with a given color.

Parameters:

Color c: The color to fill the subsequent shapes.

#### void strokeWeight(int s)
Changes how thick or thin the stroke lines are, smaller numbers means thinner outlines, bigger numbers mean thicker outlines.

Parameters:

Int s: The size (in px) of the stroking line.

#### void noStroke()
Makes all shapes drawn after this call to not have their outlines drawn.

#### void stroke(Color c)
Makes all shapes drawn after this call to have their outlines drawn with a given color.

Parameters:

Color c: The color to draw the outlines of the subsequent shapes.

#### void frameRate(int frames)
Changes the framerate of the simulation. Must be called in ```setup()```.
Note that this has a limit as the deltaTime between frames must be at least 1ms.

Parameters:

int frames: The number of FPS to set the simulation to.

#### void toggleAntiAlias()
Toggles anti-aliasing on and off.

#### void push()
Toggles the temporary state on. All color changes made when temporary state is on can be undone by leaving this state.

#### void pop()
Toggles the temporary state off.

#### void start(Action setup, Action draw, PictureBox img)
Starts the simulation.

Parameters:

Action setup: The setup() method written by the user.

Action draw: The draw() method written by the user.

PictureBox img: The PictureBox element to which the drawing will occur.

### Drawing methods

These methods draw onto the canvas.

#### void point(int x, int y)
Draws a point at coordinates (x,y)

Parameters:

Int x, y: The x,y coordinates to draw the point to.

#### void text(string s, int x, int y)
Writes a string of text onto the screen.

Parameters:

String s: The text to be written

Int x,y: The coordinates of the top-left corner of the text.

#### void clear()
Clears the entire canvas and just leaves it's background.

#### void background(Color c)
Sets a new background color and clears the canvas to it.

Parameters:

Color c: The new background color.

#### void circle(int x, int y, int r)
Draws a circle onto the screen.

Parameters:

Int x,y: The coordinates of the top-left part of the rectangle that contains the circle.

Int r: The radius of the circle.

#### void ellipse(int x, int y, int w, int h)
Draws an ellipse onto the screen.

Parameters:

Int x,y: The coordinates of the top-left part of the rectange that contains the ellipse.

Int w,h: The width and height of the rectangle that contains the ellipse.

#### void line(int x1, int y1, int x2, int y2)
Draws a line between two points.

Parameters:

Int x1,y1: The coordinates of the first point.

Int x2,y2: The coordinates of the second point.

#### void rect(int x, int y, int w, int h)
Draws a rectangle onto the screen.

Parameters:

Int x, y: The coordinates of the top-left corner of the rectangle.

Int w,h: The width and height of the rectangle.

#### void square(int x, int y, int s)
Draws a square onto the screen.

Parameters:

Int x, y: The coordinates of the top-left corner of the square.

Int s: The size of the sides of the square.

#### void triangle(int x1, int y1, int x2, int y2, int x3, int y3)
Draws a triangle onto the screen.

Parameters:

Int x1,y1: The coordinates of the first vertex of the triangle.

Int x2,y2: The coordinates of the second vertex of the triangle.

Int x3,y3: The coordinates of the third vertex of the triangle.

#### void polygon(Point[] points)
Draws any convex polygon onto the screen.

Parameters:

Point[] points: An array containing all of the vertices of the polygon.

// Todo: Add examples here

