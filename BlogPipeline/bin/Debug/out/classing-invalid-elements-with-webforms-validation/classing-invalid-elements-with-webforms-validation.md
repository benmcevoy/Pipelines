# Enhancing Web Forms Validation #

*Oh woe. I am working (at work at least) pretty much exclusively in the land of legacy .NET WebForms. This has lead me to build a kind of ["remedial framework"][1]. I haven't had the courage to inflict that on any production system, but writing it has helped me deepen my understanding, particularly around validation and reflection. The following is a little bit more of that.*

I have been working with the standard WebForms validation of .NET 2.0-3.5 and had forgotten how "old school" it is all is.

I **moderately** like the validator controls.  They are crap compared to a more unobtrusive approach, such as using data- attributes, and they leave your markup looking very crufty, but at least they are declarative and simple to use.

One thing that particularly irks, however, is the lack of classing on an invalid element.

The display of validation elements works by evaluating each "**evaluationfunction**" and then showing or hiding the corresponding span that contains the validation errormessage or text.

What it doesn't do is look at all the validation rules for the "**controltovalidate**" as a set.

The JavaScript emitted by WebForms is in webuivalidation.js which you can inspect either in the browser (it's one of those WebResource.axd requests) or you can see it using reflector as a resource in the System.Web assembly.

Two things:

1. JavaScript is dynamic, so we can add and delete functions to an object and reassign those functions as we wish.

2. The WebForms validation script leaks all of its methods to window.  

After a bit of *noseying* around I *picked* on window.ValidatorValidate as a likely culprit to override.

My first attempt went something like:

<pre><code>
  var original = window.ValidatorValidate;

  window.ValidatorValidate = function(){

	original();

	// do more stuff like apply a class
	alert('hey, this seems to work');
  };
</code></pre>

This seems dodgy... Some googling "override javascript functions" later yielded a [nicer proxy pattern, courtesy of 
the jQuery API docs][2].

In the end I've settled on the following.  It adds an "error" class to invalid elements. I took a dependency on jQuery as I couldn't be bothered writing addClass/removeClass functions. This function should be run onready, or after webuivalidation.js has loaded.  It's self executing so just drop it in and see what breaks!

<pre><code>
  (function ($) {
    if (window.ValidatorUpdateDisplay) {
        var proxied = window.ValidatorUpdateDisplay;

        window.ValidatorUpdateDisplay = function () {
            onBefore(arguments);

            var result = proxied.apply(this, arguments);

            onAfter(arguments);

            return result;
        };

        var onBefore = function (arguments) {
        };

        var onAfter = function (arguments) {
            var control = document.getElementById(arguments[0].controltovalidate);
            var validators = control.Validators;
            var isValid = true;

            for (var i = 0; i &lt; validators.length; i++) {
                if (!validators[i].isvalid) {
                    isValid = false;
                    break;
                }
            }

            if (isValid) {
                $(control).removeClass('error');
            } else {
                $(control).addClass('error');
            }
        };
    }
  })(jQuery);
</code></pre>


  [1]: https://bitbucket.org/benmcevoy/webforms.framework
  [2]: http://api.jquery.com/Types/#Proxy_Pattern