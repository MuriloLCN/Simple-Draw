# Simple-Draw

  This interface is aimed to provide a simplified experience for using the Graphic library in C# (.Net) to modify and update a picture box in a Windows Forms app.
  
  The general design of the library structure is heavily (if not all) inspired by the [P5.js](https://p5js.org/) library and the Processing library, and is intended to make simple graphical programs easy to do in the shape of a Windows Forms app using the C# language and all it offers, alongside all of the flexibility of the .NET environment.
  
 ## Installation
 
  In order to use this interface, you can either download the ```simpledrawproject.cs``` file and include it in your code or, alternatively, you can copy the ```SimpleDraw``` class anywhere into your app's namespace to use it.
  
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
    s.circleMode(s.mode_top_left);
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
    s.circleMode(s.mode_top_left);
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
    s.circleMode(s.mode_top_left);
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

(Note: This is a compressed GIF loop, the quality in the real sketch is far better)

And just like that, we've got a ball that bounces around like a DVD screensaver (I wonder when it'll hit the corners...), you should try for yourself. 

This example was just to give you an idea of how you can work with this interface, and there is so much you can do with this, it's all up to you.

# Documentation
Here are the components that make up the SimpleDraw class and how you can use them.

---
## State methods

These methods don't directly draw onto the screen, but rather control aspects of the simulation.

---
### SimpleDraw.changeFont(string name, float size)
Changes the font used in the canvas. If temp state is on, it will change the temporary font only.

Parameters:
string name: The name of the font (e.g: "Time New Roman")

float size: The size of the font (e.g: 15.0f)

Example:
```
void draw()
{
    s.changeFont("Arial", 25);
    s.text("Hello, ", 50, 50);
    s.changeFont("Times New Roman", 38);
    s.text("World!", 50, 100);
}
```

![changefont](https://user-images.githubusercontent.com/88753590/200180481-7ba46dbd-f2b5-4b8a-a343-c3689c48dd91.PNG)

---
### SimpleDraw.translate(int x, int y)
Changes the position of the origin.

Parameters:

int x; int y: The new x,y coordinates of the origin point.

Example:
```
void draw()
{
    s.translate(0, 0);
    s.background(Color.White);
    s.fill(Color.Gray);
    s.square(50, 50, 50);

    s.translate(-50, 50);
    s.fill(Color.Gold);
    s.square(50, 50, 50);
}
```

![translate](https://user-images.githubusercontent.com/88753590/200180664-236ccddb-01bf-4bef-a36f-2b3cea8bce0b.PNG)


---
### SimpleDraw.noFill()
Makes all shapes drawn after this call to not be filled in.

### SimpleDraw.fill(Color c)
Makes all shapes drawn after this call to be filled in with a given color.

Parameters:

Color c: The color to fill the subsequent shapes.

Example:
```
void draw()
{
    s.strokeWeight(2);
    s.background(Color.White);
    s.noFill();
    s.stroke(Color.Green);
    s.square(50, 50, 50);

    s.fill(Color.Gold);
    s.circle(100, 150, 25);
}
```

![fill](https://user-images.githubusercontent.com/88753590/200180924-67dc4dec-41f6-4372-b8b3-6bc450c4db81.PNG)

---
### SimpleDraw.strokeWeight(int s)
Changes how thick or thin the stroke lines are, smaller numbers means thinner outlines, bigger numbers mean thicker outlines.

Parameters:

Int s: The size (in px) of the stroking line.

Example:
```
void draw()
{
    s.strokeWeight(1);
    s.line(0, 20, s.width, 20);

    s.strokeWeight(2);
    s.line(0, 40, s.width, 40);

    s.strokeWeight(4);
    s.line(0, 60, s.width, 60);

    s.strokeWeight(8);
    s.line(0, 80, s.width, 80);
}
```

![strokeweight](https://user-images.githubusercontent.com/88753590/200181101-81967879-15f3-4912-86b4-ee4bb0f83fea.PNG)


---
### SimpleDraw.noStroke()
Makes all shapes drawn after this call to not have their outlines drawn.

### SimpleDraw.stroke(Color c)
Makes all shapes drawn after this call to have their outlines drawn with a given color.

Parameters:

Color c: The color to draw the outlines of the subsequent shapes.

Example:
```
void draw()
{
    s.strokeWeight(2);

    s.fill(Color.Red);
    s.noStroke();

    s.square(20, 30, 60);

    s.fill(Color.Silver);
    s.stroke(Color.Blue);

    s.triangle(200, 200, 300, 300, 350, 150);

}
```

![strokenostroke](https://user-images.githubusercontent.com/88753590/200181365-bbd8184a-2839-4d8c-bb91-e6932493a39c.PNG)

---
### SimpleDraw.frameRate(int frames)
Changes the framerate of the simulation. Must be called in ```setup()```.
Note that this has a limit as the deltaTime between frames must be at least 1ms.

Parameters:

int frames: The number of FPS to set the simulation to.

---
### SimpleDraw.toggleAntiAlias()
Toggles anti-aliasing on and off. It is off by default.

---
### SimpleDraw.push()
Toggles the temporary state on. All color changes made when temporary state is on can be undone by leaving this state.

### SimpleDraw.pop()
Toggles the temporary state off.

Example:
```
void draw()
{
    s.strokeWeight(2);

    s.fill(Color.Red);
    s.stroke(Color.Blue);

    s.square(50, 50, 50);
    s.circle(100, 50, 25);

    s.push();
    s.fill(Color.Gold);
    s.square(100, 100, 50);
    s.pop();

    s.circle(50, 100, 25);
}
```

![pushpop](https://user-images.githubusercontent.com/88753590/200181536-7f694d7d-4737-4874-a551-53449d176d48.PNG)


---
### SimpleDraw.start(Action setup, Action draw, PictureBox img)
Starts the simulation.

Parameters:

Action setup: The setup() method written by the user.

Action draw: The draw() method written by the user.

PictureBox img: The PictureBox element to which the drawing will occur.

### SimpleDraw.pause()
Pauses the simulation.

### SimpleDraw.unpause()
Unpauses the simulation. Needs to be called somewhere other than ```draw()```, as it will be halted.

### SimpleDraw.quit()
Quits the simulation.

---
### SimpleDraw.zoom(Int zoomFactor)
Zooms in or out depending on the factor. Multiplies the width and height by it.
Beware: Very zoomed out pictures can use a lot of memory.

Examples:

```s.zoom(2);```
Makes the image zoomed out by a factor of 2x (2x for each side, 4x the area)

```s.zoom(0.5);```
Makes the image zoomed in by a factor of 2x (0.5x for each side, 0.25x the area)

---
### SimpleDraw.mousePos(Form f) -> Point()
Gets the current mouse position (relative to the top left corner of the canvas).
Returns a Point with the X,Y position of the cursor.

Parameters:
Form f : The current form to get relative mouse position from

Returns:
Point p : A point with the relative coordinates of the mouse relative to the canvas

Example:
```
void draw()
{
    s.background(Color.Silver);

    Point pos = s.mousePos(this); // Form where we're drawing this passed in

    s.noFill();
    if (pos.X >= 75 && pos.X <= 150)
    {
        if (pos.Y >= 75 && pos.Y <= 150)
        {
            s.fill(Color.Gold);
        }
    }

    s.push();
    s.strokeWeight(3);
    s.point(pos.X, pos.Y);
    s.pop();

    s.square(75, 75, 75);
    s.fill(Color.Green);
    s.text(pos.X.ToString(), 10, 10);
    s.fill(Color.Red);
    s.text(pos.Y.ToString(), 10, 30);
}
```

![mouseposgif](https://user-images.githubusercontent.com/88753590/200938260-1b20c3b1-adb7-4002-ad5d-b1b9425716f6.gif)


---
### SimpleDraw.rectMode(DRAW_MODE r)
Changes the current mode for drawing squares, rectangles and images.

If ```r``` is ```TOP_LEFT``` (which is the default), the (X,Y) coordinates passed in for the ```rect()```,```square()``` and ```image()``` methods will represent the top left coordinate of the rectangle. ```TOP_LEFT``` can be passed in with ```SimpleDraw.mode_top_left```.

On the other hand, if ```r``` is ```CENTER```, the (X,Y) coordinates passed in for the methods above will represent the position of the center of the rectangle and the resulting position will be calculated based on it. ```CENTER``` can be passed in with ```SimpleDraw.mode_center```.

Example:
```
void draw()
{
    s.background(Color.Silver);
    s.noFill();

    s.stroke(Color.Red);
    s.rect(50, 25, 100, 50); // Rectangle with RECT_MODE as TOP_LEFT

    s.stroke(Color.Blue);
    s.rectMode(s.mode_center);
    s.rect(50, 25, 100, 50); // Same rectangle with RECT_MODE as CENTER

    s.stroke(Color.Black);
    s.strokeWeight(3);
    s.point(50, 25); // Point at the (x,y) coordinates passed in for the rectangles above
}
```

![rectmode](https://user-images.githubusercontent.com/88753590/200935303-f2766482-e3ed-4aa6-a3bd-9a073e4f88f4.PNG)

---
### SimpleDraw.circleMode(DRAW_MODE c)
Changes the current mode for drawing circles and ellipses.

If ```c``` is ```TOP_LEFT```, the (X,Y) coordinates passed in for the ```circle()``` and ```ellipse()``` methods will represent the top left coordinate of the rectangle which contains the ellipse. ```TOP_LEFT``` can be passed in with ```SimpleDraw.mode_top_left```.

On the other hand, if ```c``` is ```CENTER``` (which is the default), the (X,Y) coordinates passed in for the methods above will represent the position of the center of the ellipse and the resulting position will be calculated based on it. ```CENTER``` can be passed in with ```SimpleDraw.mode_center```.

```
void draw()
{
    s.background(Color.Silver);
    s.noFill();
    s.strokeWeight(3);

    // Three circles at the same starting position with different sizes with CIRCLE_MODE as TOP_LEFT
    s.stroke(Color.Red);
    s.circle(50, 50, 25);
    s.stroke(Color.Green);
    s.circle(50, 50, 18);
    s.stroke(Color.Blue);
    s.circle(50, 50, 8);

    // Three circles at the same starting position with different sizes with CIRCLE_MODE as CENTER
    s.circleMode(s.mode_center);
    s.circle(150, 50, 25);
    s.stroke(Color.Red);
    s.circle(150, 50, 18);
    s.stroke(Color.Green);
    s.circle(150, 50, 8);

    s.stroke(Color.Black);
    s.point(150, 50);
    s.point(50, 50);
}
```

![circlemode](https://user-images.githubusercontent.com/88753590/200936464-4b944ae9-c36a-4209-9954-31ef12691092.PNG)

---
## Drawing methods

These methods draw onto the canvas.

---
### SimpleDraw.point(int x, int y)
Draws a point at coordinates (x,y)

Parameters:

Int x, y: The x,y coordinates to draw the point to.

Example:
```
void draw()
{
    s.strokeWeight(2);

    s.point(50, 50);
    s.point(60, 60);
    s.point(80, 80);
}
```

![point](https://user-images.githubusercontent.com/88753590/200181835-f8a0649e-16cd-4253-b9a4-2099c79be317.PNG)

---
### SimpleDraw.text(string s, int x, int y)
Writes a string of text onto the screen.

Parameters:

String s: The text to be written

Int x,y: The coordinates of the top-left corner of the text if RECT_MODE is TOP_LEFT or the middle of the text is RECT_MODE is CENTER.

---
### SimpleDraw.clear()
Clears the entire canvas and just leaves it's background.

Example:
```
void draw()
{
    s.fill(Color.Silver);
    s.stroke(Color.Blue);

    s.square(50, 50, 50); // This should not appear if we call clear()

    s.clear();

    s.circle(100, 100, 25); // This should appear
}
```

![clear](https://user-images.githubusercontent.com/88753590/200182146-059dac63-46cb-41a7-9df7-28cbf1804060.PNG)

---
### SimpleDraw.background(Color c)
Sets a new background color and clears the canvas to it.

Parameters:

Color c: The new background color.

Example:
```
void draw()
{
    s.background(Color.Black);
    s.noFill();
    s.stroke(Color.Red);
    s.strokeWeight(3);

    s.circle(30, 30, 30);
}
```

![bgactual](https://user-images.githubusercontent.com/88753590/200182316-f87b285d-f803-4b08-8cae-3c352de2122d.PNG)


---
### SimpleDraw.circle(int x, int y, int r)
Draws a circle onto the screen.

Parameters:

Int x,y: The coordinates of the top-left part of the rectangle that contains the circle if CIRCLE_MODE is TOP_LEFT, or the center of the circle if CIRCLE_MODE is CENTER.

Int r: The radius of the circle.

Example:
```
void draw()
{
    s.noFill();
    s.stroke(Color.Red);
    s.strokeWeight(3);

    s.circle(30, 30, 30);
    s.stroke(Color.Black);
    s.circle(30, 30, 25);
    s.stroke(Color.Yellow);
    s.circle(30, 30, 15);
    s.stroke(Color.Blue);
    s.circle(30, 30, 7);
}
```

![circle](https://user-images.githubusercontent.com/88753590/200190224-902b2b8b-241f-44f8-b891-ff1562c755a5.PNG)

---
### SimpleDraw.ellipse(int x, int y, int w, int h)
Draws an ellipse onto the screen.

Parameters:

Int x,y: The coordinates of the top-left part of the rectange that contains the ellipse if CIRCLE_MODE is TOP_LEFT, or the center of the ellipse if CIRCLE_MODE is CENTER.

Int w,h: The width and height of the rectangle that contains the ellipse.

Example:
```
void draw()
{
    s.strokeWeight(3);

    s.fill(Color.Silver);
    s.stroke(Color.Blue);
    s.ellipse(62, 40, 45, 70);

    s.noFill();
    s.stroke(Color.Red);
    s.ellipse(50, 50, 70, 30);
}
```

![ellipse](https://user-images.githubusercontent.com/88753590/200190559-d751d4d4-f6c7-41b2-8119-d994e044e988.PNG)

---
### SimpleDraw.line(int x1, int y1, int x2, int y2)
Draws a line between two points.

Parameters:

Int x1,y1: The coordinates of the first point.

Int x2,y2: The coordinates of the second point.

Example:
```
void draw()
{
    s.strokeWeight(3);

    s.line(0, 0, s.width, s.height);
    s.stroke(Color.Blue);
    s.line(0, s.height/2, s.width, s.height/2);
    s.stroke(Color.Red);
    s.line(200, 300, 400, 250);
}
```
![lines](https://user-images.githubusercontent.com/88753590/200190969-754cf660-f443-4afd-a040-a0d3a9b64336.PNG)

---
### SimpleDraw.rect(int x, int y, int w, int h)
Draws a rectangle onto the screen.

Parameters:

Int x, y: The coordinates of the top-left corner of the rectangle if RECT_MODE is TOP_LEFT, or the center of the rectangle if RECT_MODE is CENTER.

Int w,h: The width and height of the rectangle.

Example:
```
void draw()
{
    s.strokeWeight(3);

    int x = 50;
    int y = 60;
    int dx = 5;
    int dy = 5;

    s.noFill();

    s.rect(x, y, 50, 90);

    s.stroke(Color.Red);
    s.rect(y, x, 90, 50);

    s.stroke(Color.Blue);
    s.rect(x+dx, y+dy, 50-dx*2, 90-dy*2);
}
```

![rect](https://user-images.githubusercontent.com/88753590/200190928-c1e02c29-c48d-42b1-8d88-c7f8e59c6d1a.PNG)


---
### SimpleDraw.square(int x, int y, int s)
Draws a square onto the screen.

Parameters:

Int x, y: The coordinates of the top-left corner of the square if RECT_MODE is TOP_LEFT, or the center of the square if RECT_MODE is CENTER.

Int s: The size of the sides of the square.

Example:
```
void draw()
{
    s.noStroke();

    s.fill(Color.Black);
    for (int i = 0; i < s.width; i += 25)
    {
        for (int j = 0; j < s.height; j += 25)
        {
            if ((i+j)%2 == 0)
            {
                s.square(i, j, 25);
            }
        }
    }
}
```
![square](https://user-images.githubusercontent.com/88753590/200191124-c2af008c-e575-429f-8aba-be3b253b3bf7.PNG)

---
### SimpleDraw.triangle(int x1, int y1, int x2, int y2, int x3, int y3)
Draws a triangle onto the screen.

Parameters:

Int x1,y1: The coordinates of the first vertex of the triangle.

Int x2,y2: The coordinates of the second vertex of the triangle.

Int x3,y3: The coordinates of the third vertex of the triangle.

Example:
```
void draw()
{
    s.background(Color.Silver);
    s.fill(Color.Gold);

    s.triangle(40, 40, 120, 40, 80, 120);
    s.triangle(120, 40, 200, 40, 160, 120);
    s.triangle(80, 120, 160, 120, 120, 200);
}
```

![triangle](https://user-images.githubusercontent.com/88753590/200191477-0bd81153-71ec-4bad-9982-3a37595925ad.PNG)

---
### SimpleDraw.polygon(Point[] points)
Draws any convex polygon onto the screen.

Parameters:

Point[] points: An array containing all of the vertices of the polygon.

Example:
```
void draw()
{
    s.background(Color.Silver);
    s.fill(Color.Gold);
    Point[] pts = { new Point(50,50), new Point(100, 50), new Point(120, 100), new Point(75, 135), new Point(30, 100) };
    s.polygon(pts);
}
```

![polygon](https://user-images.githubusercontent.com/88753590/200191606-e1730252-b389-40d8-b1f9-5edcee397622.PNG)

---

### SimpleDraw.image(Image img, int x, int y)
### SimpleDraw.image(Image img, int x, int y, int w, int h)

Draws an image onto the screen at the given position with a determined size. If no size is passed in, the image will have it's original size.
Notice that using very high resolution images (expectedly) makes the program use more memory.

Parameters:

Int x, y : The coordinates of the top left corner of the image if RECT_MODE is TOP_LEFT, or the center of the image if RECT_MODE is CENTER

(Optional) Int w, h: The width and height to contain the image in a well defined space. If not passed in, the original image dimensions will be used.

Examples:
```
Image myImage = Image.FromFile("Image\\path\\goes\\here.png");
void draw()
{
    s.background(Color.Silver);

    s.image(myImage, 20, 20, 300, 350);
}
```

![image](https://user-images.githubusercontent.com/88753590/200208717-adc548f4-4ec6-4225-b723-61786af20390.PNG)




