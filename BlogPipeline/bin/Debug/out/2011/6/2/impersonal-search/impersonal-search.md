If you are logged into Google your search results may not be what you are expecting.

Google will personalize your search results, which these days means anyone you're linked to on twitter, facebook etc is obviously more relevant than say, some subject matter expert.

In a nutshell - add &pws=0 to the query string.

I was so irked I went to the trouble of:

+ Open C:\Program Files (x86)\Mozilla Firefox\searchplugins\
+ Copy pasta the google.xml file, renamed to impersonal-google.xml
+ Updated the xml as below

It's also interesting to see the [suggest query](http://suggestqueries.google.com/complete/search?q=test) link, which returns JSONP.  Could be some fun to be had there...

If you wanted more control I'm sure there is a plugin or a [greasemonkey](http://googlesystem.googlepages.com/nogoogpers.user.js) script around.

<pre><code>
&lt;SearchPlugin xmlns="http://www.mozilla.org/2006/browser/search/"&gt;
  &lt;ShortName&gt;Unpersonal Google&lt;/ShortName&gt;
  &lt;Description&gt;Google Search without personal search results&lt;/Description&gt;
  &lt;InputEncoding&gt;UTF-8&lt;/InputEncoding&gt;
  &lt;Image width="16" height="16"&gt;data:image/png;base64,AAABAAEAEBAAAAEAGABoAwAAFgAAACgAAAAQAAAAIAAAAAEAGAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADs9Pt8xetPtu9FsfFNtu%2BTzvb2%2B%2Fne4dFJeBw0egA%2FfAJAfAA8ewBBegAAAAD%2B%2FPtft98Mp%2BwWsfAVsvEbs%2FQeqvF8xO7%2F%2F%2F63yqkxdgM7gwE%2FggM%2BfQA%2BegBDeQDe7PIbotgQufcMufEPtfIPsvAbs%2FQvq%2Bfz%2Bf%2F%2B%2B%2FZKhR05hgBBhQI8hgBAgAI9ewD0%2B%2Fg3pswAtO8Cxf4Kw%2FsJvvYAqupKsNv%2B%2Fv7%2F%2FP5VkSU0iQA7jQA9hgBDgQU%2BfQH%2F%2Ff%2FQ6fM4sM4KsN8AteMCruIqqdbZ7PH8%2Fv%2Fg6Nc%2Fhg05kAA8jAM9iQI%2BhQA%2BgQDQu6b97uv%2F%2F%2F7V8Pqw3eiWz97q8%2Ff%2F%2F%2F%2F7%2FPptpkkqjQE4kwA7kAA5iwI8iAA8hQCOSSKdXjiyflbAkG7u2s%2F%2B%2F%2F39%2F%2F7r8utrqEYtjQE8lgA7kwA7kwA9jwA9igA9hACiWSekVRyeSgiYSBHx6N%2F%2B%2Fv7k7OFRmiYtlAA5lwI7lwI4lAA7kgI9jwE9iwI4iQCoVhWcTxCmb0K%2BooT8%2Fv%2F7%2F%2F%2FJ2r8fdwI1mwA3mQA3mgA8lAE8lAE4jwA9iwE%2BhwGfXifWvqz%2B%2Ff%2F58u%2Fev6Dt4tr%2B%2F%2F2ZuIUsggA7mgM6mAM3lgA5lgA6kQE%2FkwBChwHt4dv%2F%2F%2F728ei1bCi7VAC5XQ7kz7n%2F%2F%2F6bsZkgcB03lQA9lgM7kwA2iQktZToPK4r9%2F%2F%2F9%2F%2F%2FSqYK5UwDKZAS9WALIkFn%2B%2F%2F3%2F%2BP8oKccGGcIRJrERILYFEMwAAuEAAdX%2F%2Ff7%2F%2FP%2B%2BfDvGXQLIZgLEWgLOjlf7%2F%2F%2F%2F%2F%2F9QU90EAPQAAf8DAP0AAfMAAOUDAtr%2F%2F%2F%2F7%2B%2Fu2bCTIYwDPZgDBWQDSr4P%2F%2Fv%2F%2F%2FP5GRuABAPkAA%2FwBAfkDAPAAAesAAN%2F%2F%2B%2Fz%2F%2F%2F64g1C5VwDMYwK8Yg7y5tz8%2Fv%2FV1PYKDOcAAP0DAf4AAf0AAfYEAOwAAuAAAAD%2F%2FPvi28ymXyChTATRrIb8%2F%2F3v8fk6P8MAAdUCAvoAAP0CAP0AAfYAAO4AAACAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAAQAA&lt;/Image&gt;
  &lt;Url type="application/x-suggestions+json" method="GET" template="http://suggestqueries.google.com/complete/search?output=firefox&amp;client=firefox&amp;hl={moz:locale}&amp;q={searchTerms}"/&gt;
  &lt;Url type="text/html" method="GET" template="http://www.google.com/search"&gt;
    &lt;Param name="q" value="{searchTerms}"/&gt;
    &lt;Param name="ie" value="utf-8"/&gt;
    &lt;Param name="oe" value="utf-8"/&gt;
    &lt;Param name="aq" value="t"/&gt;
    &lt;Param name="pws" value="0"/&gt;
    &lt;!-- Dynamic parameters --&gt;
    &lt;Param name="rls" value="{moz:distributionID}:{moz:locale}:{moz:official}"/&gt;
    &lt;MozParam name="client" condition="defaultEngine" trueValue="firefox-a" falseValue="firefox"/&gt;
  &lt;/Url&gt;
  &lt;SearchForm&gt;http://www.google.com/&lt;/SearchForm&gt;
&lt;/SearchPlugin&gt;
</code></pre>
