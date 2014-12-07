I find myself starting a lot of things these days and never “finishing” them.

After my little play with [javascript mandlebrot generation]( http://benmcevoy.com.au/blog/html5-canvas-mandelbrot) I really, really intended to continue with it and have a crack at using workers.

Instead, a few weeks pass, and I have started three other little play projects and finished none of them either :)

So today I cranked it out for a couple of hours and got workers working.

[demo](http://benmcevoy.com.au/projects/mandelbrotworker/default.htm)

##Messaging

Javascript is single threaded. You can fake a bit of multithreaded action by using the setInterval and setTimeout, but this is really a bit like the old interrupt programming we did way back in the day.

Workers bring real multi-threading to JavaScript, albeit with a few caveats.  They cannot work directly on the DOM.  This is similar to the issues you might experience in win forms or WFP – you have to dispatch the work back to the UI thread for updates to happen.

Threads therefore need a way of communicating:

postMessage – send a message to a worker thread or back to the UI thread
onMessage – listen for message events

Both the worker thread and the UI thread use these mechanisms, a sort of duplex messaging.

In order to make use of workers I pulled the render code out into a separate file, the worker.js

We set up a worker in the UI:

<pre><code>
    _worker1 = new Worker('/js/mandelbrot.worker.js');
    // listen for messages coming back
    _worker1.addEventListener('message', function (event) {
            var message = event.data;
            _context.putImageData(event.data.canvasData, 0, 0);
        }, false);
</code></pre>

Then we must pass some data to operate on.  The canvas data is really just a big array of data, so I grab out a chunk, work out where we are in the set and then fire it off:

<pre><code>
        _worker1.postMessage({ canvasData: _context.getImageData(0, 0, width, height),
            options: { width: width, height: height, origin: origin1, zoom: _options.zoom }
        });
</code></pre>

The _canvas.getImageData just grabs out the subset of the array for this quadrant.  We post this message to the worker who immediately starts calculating.

<pre><code>
    // this event is raised when a message is received on the worker
onmessage = function (event) {
        _canvasData = event.data.canvasData;
        _options = event.data.options;

        render(_options.width, _options.height);
    };
</code></pre>

When the work is complete the worker posts a message back using [“self”]( http://www.alistapart.com/articles/getoutbindingsituations) - self.postMessage({ canvasData: _canvasData, x: 0, y: 0 });

I can see you might want a message class to be passed in as the event data, with message name, payload etc.  Then you can send a bunch of different messages and get different responses back, maybe ask for progress, send a cancel and so on.

My very rough, dodgy profiling indicates (on a quad core I7 with 18GB RAM):

**Single threaded:**   673 milliseconds to render the default view

**Four workers in parallel:**   average 365 milliseconds each (239, 247, 436, 539), but all happen in parallel so elapsed about 400 milliseconds or so I guess. Or maybe the longest time should be used. I don’t know.

Not massive speed up BUT by operating on a thread other than the UI thread the UI is now no longer blocked – while the calculation happens you can click around, enter values and carry on.

My quadrant calculation is a bit dodgy, the code sometimes does not render but what the hell – I learnt something and now I am finished with this, for now.

![mandelbrot][1]


  [1]: http://benmcevoy.com.au/blog/get/mandelbrot.JPG