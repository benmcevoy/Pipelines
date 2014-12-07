I have been cranking out a few WP7 apps of late, [one of which](http://www.windowsphone.com/en-US/apps/d1af821d-291c-46ba-b82e-12dd768784a9) has met with moderate success (over 1700 downloads at this time).

Basically it's just a little physics simulation where you can trigger a shower of falling stars and they "fall" under gravity. (Acutally... it's just adding a "gravity" vector, I originally wrote a more complete simulation, where each bodies gravity effected every other, but I gutted that code to make this toy app for the phone).

I have roughly ported that over to javascript.  By using prototype I found that it is very easy indeed to convert that code.

For instance, we have a Vector struct in C#

<pre><code>
    public struct Vector
    {
        public Vector(double x, double y)
        {
            _x = x;
            _y = y;
        }

        private double _x;
        public double X { get { return _x; } set { _x = value; } }

        private double _y;
        public double Y { get { return _y; } set { _y = value; } }

        public double Length
        {
            get { return Math.Sqrt(LengthSquared); }
        }

        public double LengthSquared
        {
            get { return (_x * _x) + (_y * _y); }
        }

        public double Angle
        {
            get 
            {
                return Math.Atan2(_y, _x);
            }
        }

        public static Vector Add(Vector v1, Vector v2)
        {
            return new Vector(v1.X + v2.X, v1.Y + v2.Y);
        }
    }
</code></pre>

And now in Javascript

<pre><code>
var Vector = function (x, y) {
    this.x = x;
    this.y = y;
};

Vector.prototype.length = function () {
    return Math.sqrt(this.lengthSquared());
};

Vector.prototype.lengthSquared = function () {
    return (this.x * this.x) + (this.y * this.y);
};

Vector.prototype.angle = function () {
    return Math.atan2(this.y, this.x);
};

Vector.prototype.add = function(value){
    return new Vector(this.x + value.x, this.y + value.y);
};
</code></pre>

EventHandlers are replaced with callbacks - 

<pre>
<code>
 public event EventHandler UniverseCleared;

...

            if (UniverseCleared != null)
            {
                UniverseCleared(this, new EventArgs());
            }

...
</code>
</pre>

Becomes...

<pre>
<code>
// inject the callback
var Universe = function (onUniverseCleared, onBodyRemoved) {
    this.gravity = new Vector(0, 0);
    this.bodies = [];
    this.onUniverseCleared = onUniverseCleared;
    this.onBodyRemoved = onBodyRemoved;
};

...
    if (this.onUniverseCleared && typeof (this.onUniverseCleared) === 'function') {
        this.onUniverseCleared();
    }
...
</code>
</pre>

And so on - the only part that had to be rewritten was the rendering code.

[Check it out](http://benmcevoy.com.au/projects/stars/default.aspx) 