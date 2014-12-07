I have been meaning to try this out for a while – draw the [Mandelbrot set]( https://secure.wikimedia.org/wikipedia/en/wiki/Mandelbrot_set) on an html canvas.
And I have to say that I was very very pleased with the results :)  Not that it drew the set, or the speed of it or anything like that, but the technique was just fantastic.

I am not a particular strong javascript developer, but I feel like I just ascended a level.  I’m a little bit embarrassed to say I’m actually feeling very excited, almost a bit too excited by this.

[tldr;](http://benmcevoy.com.au/projects/mandelbrot/default.htm)

Firstly, the Mandelbrot set is basically a set of numbers on the [complex]( https://secure.wikimedia.org/wikipedia/en/wiki/Complex_number#Elementary_operations) plane (e.g. real+imaginary = complex number).  You are either in the set or out depending on whether you exceed a certain threshold after having a function applied several times.

Hard to explain, but basically for a complex number pretend the real part is on the x axis and imaginary is y.

<pre><code>
var cn = new complexNumber(1, 1);  // real is 1, imaginary part is 1i
</code></pre>

The function that is applied is given on Wikipedia, but for those who don’t speak mathematics it’s basically 

z(n+1) = z(n)^2 + c

so the next value of z or z(n+1)  is the square of z plus c

z and c are both complex numbers.  z starts at zero or z = 0 + 0i

c is the point on the plane we are testing

if we write that as code it’s a bit easier to see:

<pre><code>
        var z = new complexNumber(0, 0);
        var c = new complexNumber(x, y);

        for (var n = 0; n < 100; n++) {
            z = z.multiply(z).add(c);
        }
</code></pre>

After 100 iterations of applying this function we test the value of z and see if it has exceeded some threshold.  If it is still within the threshold then it’s part of the Mandelbrot set.

This is fine and dandy.

The first thing that floated my boat was making use of javascript prototype to define a “class”.  I was watching some video from the Windows 8 BUILD and they advocated the use of this pattern over say the javascript module pattern.  Either way I wanted to try this out.

A complex number class.

<pre><code>
    var complexNumber = function (real, imaginary) {
        this.r = real;
        this.i = imaginary;
    };

    complexNumber.prototype.toString = function () {
        return '{' + this.r + ',' + this.i + '}';
    }

    complexNumber.prototype.add = function (value) {
        return new complexNumber(this.r + value.r, this.i + value.i);
    };

    complexNumber.prototype.subtract = function (value) {
        return new complexNumber(this.r - value.r, this.i - value.i);
    };

    complexNumber.prototype.multiply = function (value) {
        return new complexNumber(this.r * value.r - this.i * value.i, this.i * value.r + this.r * value.i);
    };

    complexNumber.prototype.conjugate = function () {
        return new complexNumber(this.r, -this.i);
    };

</code></pre>
This is sweet.  With the prototype pattern we can declared class level members using the this keyword, so this.r and this.i are now available throughout the class. They are also available as properties or fields on the class as well.
The other very nice thing is we can now say 

<pre><code>
    var cn = new complexNumber(1, 1);  // real is 1, imaginary part is 1i
</code></pre>

And new it up.  Sweet.

The next little hurdle was getting to grips with the HTML canvas.  After some trial and error I realised that it is basically like playing with a bitmap object in c#.  I want to be able to set pixels and draw the set.  There is a bit of jiggery pokery to scale the complex plane to fit the canvas as I want the origin (0,0) to be in the center of the canvas, not at top,left.

When we wish to set a pixel we first need to grab the canvas data, essentially a big array.  We then index into the array and can then set the RGBA values and “turn the pixel on”. Or set it black.

The code to calculate the Mandelbrot set and render it on the canvas I wrote using the module pattern.  This pattern I also really like! Mainly because it feels a lot like writing c#

<pre><code>
var mandelbrot = function () {

    var _options;
    var _canvas;
    var _context;
    var _canvasData;
    var _scaleX;
    var _scaleY;
    var _offsetX;
    var _offsetY;

    initialize = function (options) {
        _options = options;

        _canvas = document.getElementById(_options.canvasId);
        _context = _canvas.getContext('2d');
        _context.fillStyle = 'rgb(0,0,0)'; // initialize the canvas to be black
        _context.fillRect(0, 0, _canvas.width, _canvas.height);

        // canvasData is a bit like a writeable bitmap in WPF
        // or a handle on that chunk of memory
        _canvasData = _context.getImageData(0, 0, _canvas.width, _canvas.height);

        // cartesian system is -2 to 2, or 4 units each axis
        // 2x2 as that's where the interesting stuff is in mandelbrot set
        _scaleX = _canvas.width / 4; // this many pixels per unit
        _scaleY = _canvas.width / 4;
        _offsetX = _scaleX * 2;
        _offsetY = _scaleY * 2;
    };

    // main render loop
    render = function () {
        for (var x = -2; x < 2; x = x + 0.01) {
            for (var y = -2; y < 2; y = y + 0.01) {
                if (isInSet(x, y)) {
                    setPoint(x, y);
                }
            }
        }
        // then we update it in one hit like bitblit?
        _context.putImageData(_canvasData, 0, 0);
    };

    // determine if this point is in the set
    isInSet = function (x, y) {
        // series is z(n+1) => z(n)^2 + c
        // where z starts at zero
        // and c is a complex number
        // and n goes from 0 to say 100
        // if the number exceeds some threshold (4) then we say it out of the set

        var z = new complexNumber(0, 0);
        var c = new complexNumber(x, y);

        for (var n = 0; n < 100; n++) {
            z = z.multiply(z).add(c);
        }

        return (z.conjugate().r < 4);
    }

    setPoint = function (x, y) {
        var index = cartesianToPixelIndex(x, y);
        setCanvasIndex(index);
    };

    cartesianToPixelIndex = function (x, y) {
        x = Math.floor(x * _scaleX + _offsetX);
        y = Math.floor(y * _scaleY + _offsetY);

        // the bitmap is like a vector in memory, each point takes 32 bits (RGBA) or 4 bytes
        // to represent the colour depth and alpha
        // _canvas.width is like stride
        // multiply by four to account for the colour depth
        // a four pixel canvas might look like
        // [rgbargbargbargba]
        return (x + (y * _canvas.width)) * 4;
    };

    setCanvasIndex = function (index) {
        // set this pixel white
        _canvasData.data[index + 0] = 255; //r
        _canvasData.data[index + 1] = 255; //g
        _canvasData.data[index + 2] = 255; //b
        _canvasData.data[index + 3] = 255; //a
    };

    return {
        init: initialize,
        render: render
    };
} ();

// on ready fire her up!
$(function () {
    mandelbrot.init({ canvasId: "canvas1" });
    mandelbrot.render();
});

</code></pre>

Now this code is pretty slow.  That’s not the point.  The point is that I could write this code and on the first time I ran it it worked. Perfectly.  I think that’s proof positive for me that following these patterns is an absolute must.  My old style crappy global function javascript is dead.

Oh and check out [the demo](http://benmcevoy.com.au/projects/mandelbrot/default.htm)

Cheers!