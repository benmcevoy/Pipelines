Ooo I thought I was being clever.  I was looking for an easy way to get the maximum value out of an array of numbers.  [John Resig had an answer]( http://ejohn.org/blog/fast-javascript-maxmin/), making clever use of the **apply** feature of javascript.

And that sparked me to thinkings: "If I can apply a function to every value in an eumerable... can I write a kind of LINQy select?"

And the answer is yes, yes you can:

<pre><code>
  var select = function (property, array) {
       var result = { p: property, output: [] };
       __select.apply(result, array);
       return result.output;
   };

   var __select = function () {
       for (var i = 0; i < arguments.length; i++) {
           if (arguments[i][this.p]) {
               this.output.push(arguments[i][this.p]);
           }
       }
   };
</code></pre>

And I was very chuffed and turned round to my co-worker David to show him.

"Uh-huh... so why don't you just call the select thingy function?"

Oh. Yeah.  No need to complicate things.  Damn.  Not so chuffed anymore.  Still this is a handy little function.   The poor mans LINQ select, allowing you to "project" a property out of an array of objects.

<pre><code>
   var select = function (property, array) {
       var result [];

       for (var i = 1; i < arguments.length; i++) {
           if (arguments[i][arguments[0]]) {
               result.push(arguments[i][arguments[0]]);
           }
       }

       return result;
   };
</code></pre>

And to solve my original problem of finding the max value:

<pre><code>
    var orders = [
	    { orderNo: 1, orderValue:  123 },
    	{ orderNo: 1, orderValue:  321 },
	    { orderNo: 1, orderValue:  456 }
     ];
    
    var orderValues = select(‘orderValue’, orders);
    // returns [123, 321, 456]
    Math.max.apply(Math, orderValues);
</code></pre>

There are of course plenty of "real" [LINQ](https://jslinq.codeplex.com/) for [JavaScript](https://linqjs.codeplex.com/) and [variations](https://code.google.com/p/arrayzing/) about. Cheers.

