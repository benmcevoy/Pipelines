# You Never Get Anything Done #

## Or How It Takes 8 Hours To Write One Line of Code ##

I recently wrote some code to [cluster points on a map](http://benmcevoy.com.au/blog/clustering-results-with-windows-phone-bing-map-control).  This was a fun little problem, but I was nagged by the knowledge that my algorithm for working out distance for any scale was... flawed, to say the least.

## Guesstimating ##

While writing it I assumed that there must be some exponential relationship happening, for each level of zoom an order of magnitude more or less of map was being displayed.  So I chucked a few numbers in and quickly came to:

<pre><code>
    var threshold = 8192 / Math.Exp(zoomLevel); 
</code></pre>

The 8192 term seemed to fix things up a bit, and what the hey it's a nice round number. In binary. So that was as good a reason to use it as any :)

Unfortunately at low zoom levels the radius of the cluster would be quite large.  Several planets large.  

## Numerical Techniques ##

So I took the raw numbers it was generating at each zoom level, from 1-21 and wrote a little function to return the now hard coded values.  Then I tweaked those numbers till it looked better.

Then the clever bit - I pasted those numbers into Excel and plotted them against the zoom level.

![alt text][1]

Excel will let you add a trend line:

![alt text][2]

I don't have that tweaked data no more, but you can see what's up by using the original formula.

Excel ends up being able to figure out the formula - 8192e^-x which is the same as 8192/e^x

![alt text][3]

My new and improved tweaked formula:

<pre><code>
    var threshold = 560 * Math.Exp(-0.854 * zoomLevel); 
</code></pre>

And life was good for the rest of the afternoon.

It's a nice technique actually, and I'll keep that in my bag of tricks for another day - being able to take sampled data, or measured data and then pull out a relationship for it is very useful.

## Realisation and Theory ##

But still... this was crap.  There's no basis in reality for using that formula, except for the exponential part.  So I thunk some more on it and realised I should just state the problem I was really trying to solve:

What's the size of the cluster *in degrees* for each zoom level?

And then things start to make sense.  What's the length of a degree?  It depends on where you are, and it depends on which direction you’re measuring in.

At the equator, or Latitude zero, [the circumference of the earth](https://en.wikipedia.org/wiki/Equator#Exact_length_of_the_equator) is about 40,075.035535 kilometres.

There are 360 degrees in a circle, so a degree is about 111320 meters, or 111.32 km.

At differing latitudes the length of a degree will vary slightly due to the earth bulging, but the difference between the equator (Latitude 0) and poles (Latitude 90) is small, a bit over a km, so we can make an approximation here and just use value for any latitude.

It is worth noting, particularly as I found it quite confusing, that the length of the degree is measured perpendicular to the line of latitude - so the line of latitude runs east-west, but it's value indicates the north-south position (distance from the equator). The length of the degree is measured north-south.

The length of a degree longitudinally will be much more influenced by where you are on the globe, nearer the poles the length will be shorter than at the equator.  This will obviously be [some function of latitude and is in fact](https://en.wikipedia.org/wiki/Longitude#Length_of_a_degree_of_longitude):

pi/180 * radius * Cosine(degree of latitude)

Which can be simplified to (as circumference = 2 * pi * r, or radius = c/2 * pi)

circumference/360 * Cosine(latitude)

or 111320m * Cosine(latitude)


## The Perfect(ish) Line ##

Given all that I can now write, with some confidence:

<pre><code>
    var threshold = Math.Pow(2f, 16f - zoomLevel) / 1113.2;
</code></pre>

Which lets me define that at zoom level 16 my area will be 1/1113.2 degrees long or

1/1113.2 * length of a degree = about 100 meters

Which is about what I was after.  

At zoom level 16 the map shows about [1km across the screen](http://msdn.microsoft.com/en-us/library/aa940990.aspx), the screen is 480 pixels wide, 100 meters is about 50 pixels on the screen and all is good.

At zoom level 12, for instance, the map should be showing about 18km across and our function gives us a length of 2^4/1113.2 degrees or about 1600 meters.

## Finally ##

As there is a lot of logic embedded in those formulas I refactored them to be a bit more meaningful, as the next time I look at this I will NOT remember a damn thing.  They have also been tweaked further to give me more control over the size of the cluster, and the cluster size has been increased slightly to 150m at zoom level 16:

<pre><code>

     private const double EquatorialLatitudeMetersPerDegree = 111319.5431527;
    
    ...

        var thresholdLat = GetLatititudeThresholdDistanceInDegrees(zoomLevel);
        var thresholdLon = GetLongitudeThresholdDistanceInDegrees(zoomLevel, boundingRectangle.Center.Latitude); 

    ...

    private double GetLatititudeThresholdDistanceInDegrees(double zoomLevel)
    {
        var metersAtZoomLevel = GetMetersAtZoomLevel(zoomLevel);
        return metersAtZoomLevel / EquatorialLatitudeMetersPerDegree;
    }

    private double GetLongitudeThresholdDistanceInDegrees(double zoomLevel, double latitude)
    {
        var metersAtZoomLevel = GetMetersAtZoomLevel(zoomLevel);
        return metersAtZoomLevel / (Math.Cos(latitude) * EquatorialLatitudeMetersPerDegree);
    }

    private static double GetMetersAtZoomLevel(double zoomLevel)
    {
        return 150f * Math.Pow(2f, 16f - zoomLevel);
    }

</code></pre>


  [1]: http://blog.benmcevoy.com.au/get/cluster/RawDistance.PNG
  [2]: http://blog.benmcevoy.com.au/get/cluster/trendy.png
  [3]: http://blog.benmcevoy.com.au/get/cluster/formula.PNG