It has obviously been a while since I’ve used the Google map API, as we no longer require a developer key to access the services (bonus!).  This means I can freely go about the following:

 - Do  some reverse geocoding (address lookup)
 - Find ourselves. On a map.
 - And get the local weather forecast

[Check out the demo](/../projects/google-weather/default2.aspx)

Let’s prove our Google fu is cool first.  Let’s go and grab a [static map](https://code.google.com/apis/maps/documentation/staticmaps/) and return that.  

<iframe style="width: 100%; height: 300px" src="http://jsfiddle.net/ben_mcevoy/xau8w/embedded/"></iframe>

The codes;)

<pre><code>
$(function() {
    navigator.geolocation.getCurrentPosition(function(pos) {
        var lat = pos.coords.latitude;
        var lon = pos.coords.longitude;

        $('#mapImage').attr('src', 'http://maps.google.com/maps/api/staticmap?center=' + lat + ',' + lon + '&z=16&zoom=14&size=512x512&maptype=roadmap&sensor=false');

    });
});
</code></pre>
We can set the latitude and longitude using your modern browsers [geolocation](http://dev.w3.org/geo/api/spec-source.html#geolocation_interface) [support]( http://dev.w3.org/geo/api/spec-source.html#position).

Asking for a location should prompt with a security check.

That is all just great.  Meanwhile back on the server we want to be passed in a latitude and longitude so we can find out where the sun is shining.

The server will expect latitude and longitude on the query string of the handler request.

The server code in its entirety is below.  To be honest I am not proud of this code.  It’s brittle. It’s ugly. It assumes waaaay too much about what Google might return.  If I wrote this in production I would kick my arse.
If this was production code I would:

 - Use a third party Google Map library to give me some strong types.
 - Have our own model of the weather response.
 - Return JSONP, perhaps in a more semantic format (i.e. use our own model).
 - Use WCF, possibly allow content negotiation, REST, and so on.  Or maybe not. It depends.
 - Put some better validation around the query string parameters.
 - Use reasonable defaults (don’t know where you are?  Maybe I’ll just give you the weather in Brisbane.  It’s always sunny there:).
 - Handle exceptions!

Note also that Google allows content negotiation. We could have called that geo code url and got JSON back instead.  It looks like this API does not support JSONP, [but this one here might](https://code.google.com/apis/maps/documentation/javascript/).

<pre><code>
    public class WeatherProxy : IHttpHandler
    {
        private HttpContext _context;
        private readonly string _googleWeatherUrl = "http://www.google.com/ig/api?weather={0}";
        private readonly string _googleGeoCodeUrl = "http://maps.googleapis.com/maps/api/geocode/xml?latlng={0}&sensor=false";
       
        public void ProcessRequest(HttpContext context)
        {
            _context = context;

            var latlon = _context.Request.QueryString["ll"];
            var location = LookupCity(latlon);
            var weatherXML = GetWeather(location);
            var weatherJSON = ToJson(weatherXML);

            WriteJsonResponse(weatherJSON);
        }

        private string LookupCity(string latlon)
        {
            // Ensure that no space exists between the latitude and longitude values when passed in the latlng parameter.
            latlon = latlon.Replace(" ", "");

            var xml = "";

            using (var wc = new WebClient())
            {
                xml = wc.DownloadString(string.Format(_googleGeoCodeUrl, latlon));
            }

            // if we were good there would be an object model we could deserialize into
            // or we would use some existing google geo code library.
            // I'm sure there are many
            XmlDocument doc = new XmlDocument();
            
            doc.LoadXml(xml);

            // can you say nasty?
            return doc.ChildNodes[1].ChildNodes[1].ChildNodes[4].FirstChild.InnerText;
        }

        private string GetWeather(string location)
        {
            using (var wc = new WebClient())
            {
                return wc.DownloadString(string.Format(_googleWeatherUrl, location));
            }
        }

        private void WriteJsonResponse(string result)
        {
            _context.Response.Clear();
            _context.Response.ContentType = "application/json";
            _context.Response.AddHeader("content-disposition", "attachment;filename=weather.json");
            _context.Response.Write(result);
            _context.Response.Flush();
            _context.Response.Close();
        }

        private string ToJson(string xml)
        {
            // Thank's NewtonSoft! You rock!
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return JsonConvert.SerializeXmlNode(doc);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
</code></pre>

Client side is not looking too crash hot either, to be honest.  Some tidy up around the string concatenation would be nice.
<pre><code>
    &lt;h2&gt;How's the weather?&lt;/h2&gt;
    &lt;div id="weather"&gt; &lt;/div&gt;
    &lt;div&gt;
        You're about here:
        &lt;image id="mapImage"&gt;&lt;/image&gt;
    &lt;/div&gt;

        $().ready(function () {

            navigator.geolocation.getCurrentPosition(function (pos) {
                var lat = pos.coords.latitude;
                var lon = pos.coords.longitude;

                $('#mapImage').attr('src', 'http://maps.google.com/maps/api/staticmap?center=' + lat + ',' + lon + '&z=16&zoom=14&size=512x512&maptype=roadmap&sensor=false');

                $.getJSON('weatherproxy.ashx?ll=' + lat +',' + lon, function (data) {
                    $('<p>' + data.xml_api_reply.weather.current_conditions.condition["@data"] + '</p>').appendTo('#weather');
                    $('<p>The temperature is ' + data.xml_api_reply.weather.current_conditions.temp_c["@data"] + '&deg;C</p>').appendTo('#weather');
                    $('<span>Look&rsquo;s a bit like this: <image src="http://www.google.com/' + data.xml_api_reply.weather.current_conditions.icon["@data"] + '"></image></span>').appendTo('#weather');
                });
            });
        });

</code></pre>

[See it in all its glory](/../projects/google-weather/default2.aspx)

And I lied about JSONP.  It's _context.Response.Write(string.Format("{0}({1});", callback, json));

[You figure it out](http://stackoverflow.com/questions/3702959/asp-net-generic-http-handler-ashx-supporting-jsonp)