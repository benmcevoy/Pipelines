Following on from my handy [javascript module template for visual studio](http://benmcevoy.com.au/blog/javascript-module-template) I have added a jQuery plugin template, taking the bits I can never remember and wrapping them up in a usable way.

As usual and as per the last time - creating a VS template is easy - just hit "Export Template" under the file menu in Visual Studio 2010. You can crack that zip file open and tweak stuff up or make new templates as you see fit.

You can copy this into C:\Users*yourname*\Documents\Visual Studio 2010\Templates\ItemTemplates\

<pre><code>
(function ($) {
    "use strict";

    $.fn.myPlugin = function (options) {
        var _options = {};

        if (options) {
            $.extend(_options, options);
        };

        // we expect to work on an array of elements
        return this.each(function () {
            // implement plugin logic
        });
    };
})(jQuery);
</pre></code>

Happily my understanding has improved since the last time I wrote a jQuery plugin. What was once mysterious invocation is now clear - we got ourselves a self executing anonymous function that imports the jQuery namespace and extends it.  What could be clearer?  Returning "this" makes it fluent or chainable and we are good.

The [jQuery documentation](http://docs.jquery.com/Plugins/Authoring) explains better than I.  Enjoy.
