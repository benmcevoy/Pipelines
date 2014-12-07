#Less Worse Webforms#

##Routing##

I seem to be unable to escape the horrific pit of despair that is ASP.NET WebForms, it's really starting to look like the Classic ASP of the um... noughties? Millennial something something? At any rate, there have at least been a couple of nice(r) features introduced to ease our suffering.

Routing is one of these, but to get the best out of it you'll also want to nuget in some Friendly Urls:

<pre><code>
Install-Package Microsoft.AspNet.FriendlyUrls
</code></pre>

A vanilla asp.net web application (in VS2013 at least) is kind enough to include this by default.

The Friendly Urls package [includes a few things](http://www.hanselman.com/blog/IntroducingASPNETFriendlyUrlsCleanerURLsEasierRoutingAndMobileViewsForASPNETWebForms.aspx), and supports .NET 4, but for my purposes I care only for *EnableFriendlyUrls()* which will effectively turn on extensionless urls for a webforms project.  Probably in your RouteConfig class:

<pre><code>
    var settings = new FriendlyUrlSettings();
    routes.EnableFriendlyUrls(settings);
</code></pre>

Armed with this and System.Web.Routing we can start to do less sucky things.

From .NET 4 System.Web.Routing is in the System.Web assembly, and so should be available to most new Web Forms applications.  The [MapPageRoute](http://msdn.microsoft.com/en-us/library/system.web.routing.routecollection.mappageroute(v=vs.100).aspx) method lets us start to do nice(r) things.

For instance, we can map request values (query string or form) to placeholders in the url:

<pre><code>
    routes.MapPageRoute("AboutRoute", "About/{param1}/{*param2}", "~/about.aspx");
</code></pre>

And we can map arbitrary friendly urls to existing pages:

<pre><code>
    routes.MapPageRoute("AnotherRoute", "TotallyFriendlyUrl/{param1}/{*param2}", "~/about.aspx");
</code></pre>

In the second case there is no underlying page at *TotallyFriendlyUrl.aspx* so if the route is not matched correctly, i.e. */TotallyFriendlyUrl/this_bit_is_mandatory/this_bit_is_optional* then a 404 will result.

Calls to */About/* will succeed for pretty much any parameters as 1) there is a page at about.aspx and 2) the *param2* is a catch all.  

##Other bits##

Quick, my beer is getting warm, we have an expression builder so we can make friendly urls declaratively in an aspx page:

<pre><code>
&lt;asp:HyperLink ID="HyperLink5" runat="server" 
    NavigateUrl="&lt;%$RouteUrl:locale=CA,year=2009,routename=salesroute%&gt;"&gt;
    Sales Report - CA, 2009
&lt;/asp:HyperLink&gt;
</code></pre>

And [FriendlyUrlSegments] parameter attribute for um.. doing things like [model binding to a method and junk](http://www.asp.net/web-forms/tutorials/data-access/model-binding/retrieving-data). And also [Form] and [QueryString] to boot.

The friendly url package also allows to do something that MVC 4 (or was it from an earlier version?) can do out of the box, namely chuck a device specific view in, e.g.  

- about.aspx
- about.mobile.aspx

Unfortunately this does not seem to play well with the other children.  So adding an *about.mobile.aspx* you *might* expect the uh... attractive */totallyfriendlyurl/oh/hai/* route to dish up a mobile specific view.  It did not.  Given we stated that the route mapped to *about.aspx* it's not that surprising, but it would have been nice if it was magic and just worked.


To be fair I'm not sure where I can be using this stuff.  WebForms work is either some legacy hell that would break as soon as you started doing this (and mobile support? Ha!) or is piggy backing on top of Umbraco or SiteCore which takes care of (some) of this stuff for you anyway.  No-one in their right mind would start a green fields webforms project today? Right? Amiright?


