<p>
I recently noticed google suggest is a JSONP source, so lets quickly try and consume it. JQuery can handle JSONP easily by appending ?callback=? to the request URL.
</p>

<iframe style="width: 100%; height: 300px" src="http://jsfiddle.net/ben_mcevoy/MtqLw/5/embedded/"></iframe>

<p>From a very brief search it looks like this is an undocumented feature of Google. A bit like the <a href="http://www.google.com/ig/api?weather=melbourne">Google weather API</a>.</p>
<p>The codes...</p>

<pre><code>
&lt;input type="text" id="suggestText" /&gt;
&lt;ul id="suggestions"&gt;&lt;/ul&gt;

$('#suggestText').change(function() {
    $.getJSON('http://suggestqueries.google.com/complete/search?q={0}&callback=?'
         .replace('{0}', $(this).val()), 
         function(data) {
             $('#suggestions li').remove();
             $.each(data[1], function(key, val) {
                 $('#suggestions').append('&lt;li&gt;' + val[0] + '&lt;/li&gt;');
         });
    });
});
</code></pre>

The JSON is similiar to:

<pre><code>
["test",
[
	["test","","0"],
	["test internet speed","","1"],
	["testosterone","","2"],
	["test my speed","","3"],
	["testicular cancer","","4"],
	["testament","","5"],
	["test flash","","6"],
	["testudo","","7"],
	["test drive unlimited 2","","8"],
	["testing","","9"]
],{"k":1}]
</code></pre>