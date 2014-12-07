#Clustering Results on the Windows Phone Bing Map Control#

I have been having fun today with the Bing Map Control for Silverlight/Windows Phone 7.

Having knocked up a couple of apps that use the map control, I sat down and refactored some common functionality out of them, and implemented a nice Clusterer, that will group points on the map together if they are within certain thresholds.

There are a few implementations of clustering around, mainly for the AJAX control, and therefore in JavaScript. 

[Get the all the code and more][1] 

##Clustering Location Items##

![alt text][2] ![alt text][3] ![alt text][4] ![alt text][5]

For each point on the map I am interested in I have a model item that implements ILocationItem

<pre><code>
    public interface ILocationItem
    {
        int Id { get;  }

        Microsoft.Phone.Controls.Maps.Platform.Location Location { get; }
        
        LocationRect LocationRect { get; }
        
        string Name { get;  }
    }
</code></pre>

In order to cluster these items I provide the set of location items, the geographic boundary we are interested in (the map boundary) and some threshold to control the level of clustering:

<pre><code>
    public interface ILocationItemClusterer
    {
        IEnumerable&lt;ClusteredLocationItem&gt; Cluster(IEnumerable&lt;ILocationItem&gt; items, 
			LocationRect boundingRectangle, double threshold);
    }
</code></pre>

In any algorithm the first thing I'd want to do is discard points that are outside that boundary.  Other implementations might retain all points, some might calcuate the cluster for a zoom level and cache it and so on.  My implementation reclusters each time, so I guess there is some room for improvement there.

I have a nice way to filter the items, taking advantage of yield:

<pre>
<code>
        public static IEnumerable&lt;ILocationItem&gt; IsInBoundary(this IEnumerable&lt;ILocationItem&gt; items, LocationRect boundary)
        {
            foreach (ILocationItem item in items)
            {
                if (boundary.Intersects(item.LocationRect))
                {
                    yield return item;
                }
            }
        }
</code>
</pre>

So only those items that intersect with the map boundary will be processed.

Incidently, each item has an arbitrary LocationRect, or area. As we are essentially dealing with points on a map I have just given them a small area, of maybe a couple of square meters.  It's just to ensure they have a non-zero area.

We can group or cluster these location items together inside a ClusteredLocationItem

<pre><code>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using map = Microsoft.Phone.Controls.Maps;

namespace Radio7.Phone.Common.Location
{
    public class ClusteredLocationItem : ILocationItem
    {
        public ClusteredLocationItem(map.LocationRect sector)
        {
            SectorLocationRect = sector;
            _locationItems = new List&lt;ILocationItem&gt;();
        }

        public void AddLocationItem(ILocationItem item)
        {
            _locationItems.Add(item);

            if (_firstItem == null)
            {
                _firstItem = item;
            }
        }

        public bool IsInSector(map.LocationRect sectorRect)
        {
            return SectorLocationRect.Intersects(sectorRect);
        }

        private ILocationItem _firstItem;
        private ILocationItem FirstItem { get { return _firstItem; } }

        public map.LocationRect SectorLocationRect { get; private set; }

        private List&lt;ILocationItem&gt; _locationItems;
        public IEnumerable&lt;ILocationItem&gt; LocationItems { get { return _locationItems; } }

        public int Count { get { return LocationItems.Count(); } }

        public int Id { get { return FirstItem.Id; } }

        public map.Platform.Location Location
        {
            get
            {
                if (Count > 1)
                    return SectorLocationRect.Center;

                return FirstItem.Location;
            }
        }

        public map.LocationRect LocationRect
        {
            get
            {
                if (Count > 1)
                    return SectorLocationRect;

                return FirstItem.LocationRect;
            }
        }

        public string Name
        {
            get
            {
                if (Count > 1)
                    return Count.ToString();

                return FirstItem.Name;
            }
        }
    }
}
</code></pre>

A ClusteredLocationItem is basically another ILocationItem, representing where on the map the cluster is.  It also contains the collection of items that comprise the cluster. If the cluster only has a single item then it will behave like it is that single item.

##Grid Clustering##

The most common, and most obvious algorithm is to use a regular grid over the map.  If one or more points are in a common cell of the grid then they will be clustered.

As the map pans and zooms however, the number of cells in the cluster will be more or less, that is the cell will contain different sets of points.  This causes the rendered clusters to jump around as they regroup within the ever shifting boundaries of the cells.  It's quite distracting.

Firstly we split our bounding rectangle up into a grid of cells, or sectors.

<pre><code>
        private IEnumerable&lt;LocationRect&gt; GetSectorRects(LocationRect boundingRectangle, double sectorOrdinal)
        {
            var sectors = new List&lt;LocationRect&gt;();

            var strideX = (boundingRectangle.West - boundingRectangle.East) / sectorOrdinal;
            var strideY = (boundingRectangle.North - boundingRectangle.South) / sectorOrdinal;

            for (int x = 0; x &lt; sectorOrdinal; x++)
            {
                for (int y = 0; y &lt; sectorOrdinal; y++)
                {
                    sectors.Add(new LocationRect(
                            boundingRectangle.North - (y * strideY),
                            boundingRectangle.West - (x * strideX),
                            boundingRectangle.North - ((y + 1) * strideY),
                            boundingRectangle.West - ((x + 1) * strideX)));
                }
            }

            return sectors;
        }
</code></pre>

Here the sectorOrdinal indicates the number of cells to create - e.g. 10 by 10.  This splits the map up in to sectorOrdinal^2 cells.

I am unsure about my stride logic here.  I'm not sure what happens in the northern hemisphere, or if we cross a meridan or equator.  I believe it should be using some kind of sine function, but I haven't really looked into it.

I guess that's another item on the TODO list :)

Now that we have our sectors we need to check if a point belongs to one and start actually clustering results.

<pre><code>
        public IEnumerable&lt;ClusteredLocationItem&gt; Cluster(IEnumerable&lt;ILocationItem&gt; items, LocationRect boundingRectangle, double sectorOrdinal)
        {
            var clusters = new List&lt;ClusteredLocationItem&gt;();
            var sectorRects = GetSectorRects(boundingRectangle, sectorOrdinal);
            
            foreach (var item in items.IsInBoundary(boundingRectangle))
            {
                foreach (var sector in sectorRects)
                {
                    if (sector.Intersects(item.LocationRect))
                    {
                        var cluster = GetClusterForThisSector(clusters, sector);
                        cluster.AddLocationItem(item);
                        break;
                    }
                }
            }

            return clusters;
        }

        private ClusteredLocationItem GetClusterForThisSector(List&lt;ClusteredLocationItem&gt; clusters, LocationRect sector)
        {
            foreach (var cluster in clusters)
            {
                if (cluster.IsInSector(sector))
                {
                    return cluster;
                }
            }

            var newCluster = new ClusteredLocationItem(sector);

            clusters.Add(newCluster);

            return newCluster;
        }
</code></pre>

Once we find the sector the item belongs to we add it to a cluster, creating one if need be, then move on to the next item.

##Distance Clustering##

As clustering on a grid produced some unpleasant effects when paning and zooming I looked for alteratives.

We can cluster on distance instead.  For location item we are interested in we find or create a cluster for it.  To determine if a point belongs to a cluster we create an area (LocationRect) around the cluster center.  If a point intersects with this area then it is included in the cluster.

This algorithm turns out be simpler, faster and produces more aesthetically pleasing results.

<pre><code>
using System;
using System.Collections.Generic;
using Microsoft.Phone.Controls.Maps;

namespace Radio7.Phone.Common.Location
{
    public class LocationItemDistanceClusterer : ILocationItemClusterer
    {
        public IEnumerable&lt;ClusteredLocationItem&gt; Cluster(IEnumerable&lt;ILocationItem&gt; items, LocationRect boundingRectangle, double zoomLevel)
        {
            var threshold = 8192 / Math.Exp(zoomLevel);
            var clusters = new List&lt;ClusteredLocationItem&gt;();

            foreach (var item in items.IsInBoundary(boundingRectangle))
            {
                var cluster = GetClusterForThisPoint(clusters, item.LocationRect, threshold);
                cluster.AddLocationItem(item);
            }

            return clusters;
        }

        private ClusteredLocationItem GetClusterForThisPoint(List&lt;ClusteredLocationItem&gt; clusters, LocationRect locationRect, double threshold)
        {
            foreach (var cluster in clusters)
            {
                var thresholdRect = new LocationRect(cluster.Location, threshold, threshold);

                if (thresholdRect.Intersects(locationRect))
                {
                    return cluster;
                }
            }

            var newCluster = new ClusteredLocationItem(new LocationRect(locationRect.Center, threshold, threshold));

            clusters.Add(newCluster);

            return newCluster;
        }
    }
}
</code></pre>

The threshold value of var threshold = 8192 / Math.Exp(zoomLevel) is something I arrived at by trial and error.  It works pretty good on the phone in portrait mode, I would suggest you test it with different resolutions and layouts.


##More Betterer Grid Clusterer##

After building these two implementations it occoured to me that a better grid clusterer is most likely.  By aligning the grid to the underlying co-ordinate system - the latitude and longitude - the cells would move in sync with the map.  Changing the zoom level would change the size of the cells, but panning should look good.  To be fair I'm pretty happy with the distance Clusterer for now, so I'll leave this for another day.

##Using the Clusterer##

<pre><code>
       private void UpdateMap()
        {
            var clusters = _clusterer.Cluster(_repository.Get().Cast&lt;ILocationItem&gt;(), MapControl.BoundingRectangle, MapControl.ZoomLevel);

            _pinLayer.Children.Clear();

            foreach (var cluster in clusters)
            {
                if (cluster.Count == 1)
                {
                    var p = new Image();
                    p.Source = new BitmapImage(new Uri("pushpin.png", UriKind.Relative));
                    p.Stretch = System.Windows.Media.Stretch.None;
                    p.Name = string.Format("pin_{0}", cluster.Id);

                    p.Tag = cluster.LocationItems.First() as Shop;
                    p.Tap += new EventHandler&lt;GestureEventArgs&gt;(PinTap);

                    _pinLayer.AddChild(p, cluster.Location, PositionOrigin.Center);
                }
                else
                {
                    var p = new Pushpin()
                    {
                        Location = cluster.Location,
                        Content = cluster.Name,
                    };

                    _pinLayer.AddChild(p, cluster.Location, PositionOrigin.Center);
                }
            }

            UpdateLabel();
        }
</code></pre>


  [1]: https://bitbucket.org/benmcevoy/radio7.phone.common
  [2]: http://benmcevoy.com.au/blog/get/cluster/zoom12.png
  [3]: http://benmcevoy.com.au/blog/get/cluster/zoom16.png
  [4]: http://benmcevoy.com.au/blog/get/cluster/zoom18.png
  [5]: http://benmcevoy.com.au/blog/get/cluster/detail.png