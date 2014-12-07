I got me a Windows phone 7 recently or WinMoPho as we like to say.  Having never had an iPhone I have to say, "hey! this phone is sweet!". My iPhone brandishing buddies, however, say "hey! you're on crack!"

The mango update has made up a fair bit of ground, so instead of the old and insane IE7 based browser we now have a quite tolerable IE9 based browser.

But I wanted more.

There's been some nice weather recently, we hit 40 degrees the other week, so I like to keep an eye on the weather.

I wrote a little [metro styled javascript weather app](http://benmcevoy.com.au/projects/metro-weather/default.htm) for Melbourne awhiles back, never did write a blog post on it, but it's just pulling some XML and JSON from the [BOM](http://www.bom.gov.au/) and reexposing it as a JSON service in a format I liked.

## Target that WinMoPho ##

The layout is reasonably fluid, CSS ain't really my bag, I just aim for "less is more" and try and write as little as possible of it.  I wanted to view it on the phone and see it in a sensible format, that is each day tile is laid out vertically.

So google some "target windows phone 7 css" and get [this on conditional comments](https://blogs.msdn.com/b/iemobile/archive/2010/12/08/targeting-mobile-optimized-css-at-windows-phone-7.aspx?Redirected=true). Quite an amusing read, particularly the nearly 100% negative comments, (although [this was even funnier](https://blogs.msdn.com/b/iemobile/archive/2010/05/10/javascript-and-css-changes-in-ie-mobile-for-windows-phone-7.aspx?Redirected=true), honestly WTF are Microsoft thinking? "We've also added support for -webkit-..."!?!??! O rly?

The first suggestion is an IE conditional comment:

<pre><code>&lt;!--[if IEMobile]&gt;
&lt;p>Welcome to Internet Explorer Mobile.&lt;/p&gt;
&lt;![endif]--&gt;
</code></pre>

(Un)fortunately this has no effect.  Much googling later and we discover WinMoPho 7.5, with its IE9 based browser, does not support this.  But it does support media queries:

<pre><code>&lt;link rel="stylesheet" type="text/css"
  media="screen and (max-device-width: 480px)"
  href="mobile.css" /&gt;
</code></pre>

And my mobile.css styles suddenly kick in.  Or at least, some of them do.  WinMoPho is still scaling the page and pretending it's 1024 by something.

[Another MSDN blog post](https://blogs.msdn.com/b/iemobile/archive/2010/11/22/the-ie-mobile-viewport-on-windows-phone-7.aspx?Redirected=true) comes up with the viewport meta tag. Thankfully this is a fairly standard tag (not sure if it is a "standard" standard?) and is supported by iOS and others.  So we set:

<pre><code>&lt;meta content="width=device-width" name="viewport"&gt;
&lt;link rel="stylesheet" type="text/css" 
    media="screen and (device-width: 480px)" 
    href="mobile.css" /&gt;
</code></pre>

And lo! it is working as expected.  And quite probably on an iPhone too!

## Making the pretty ##

So now I want to pin that URL to my start page.  iPhone, iPad lets you[ have a pretty icon](http://www.jamiebutler.com/tutorials/iphone_web.php#5) for a "web app", (and a splash screen).  

Not so WinMoPho. Not even the favicon.  Windows Phone will use what is on the screen as the tile image, which is a bit crap.

You can use [this to kinda fake](http://www.web2tile.com/) it (it requests the apple-touch-icon via a bookmarklet). That is actually rather tricky.  The bookmarklet requests the icon and replaces the document of the page your on with it.  You then pin the page.  I settled on just putting an image at the bottom of the page, scrolling down and then pinning it.  Crappy!  And then I totally stole that dudes idea! Slightly less crappy.  

Poor WinMoPho users, still not upto par with your privileged iPhone elite.
