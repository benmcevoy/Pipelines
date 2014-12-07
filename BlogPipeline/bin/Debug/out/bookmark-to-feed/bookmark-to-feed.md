I read [this article][1] about the state of bookmarking and thought "I never use bookmarks because it's just a stupid list that you have to maintain".  

I do most of my online reading via RSS and Google Reader. So I though it would be nice to have a javascript bookmarklet to POST a link somewhere for consumption via RSS later.

We need to POST a bookmark or GET the RSS feed.  This is quick and dirty - a handler:

<pre><code>
 public class Bookmarks : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.HttpMethod == "GET")
            {
                WriteRss(context);
                return;
            }

            if (context.Request.HttpMethod == "POST")
            {
                AddBookmark(context);
                Write(context, "OK");
                return;
            }

            Write(context, "UNKNOWN");
        }

        private void Write(HttpContext context, string message)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(message);
        }

        private void AddBookmark(HttpContext context)
        {
            var bm = context.Request.Form["bm"];
            var t = context.Request.Form["t"];

            if (string.IsNullOrEmpty(bm))
            {
                Write(context, "UNKNOWN");
                return;
            }

            var repository = new Repository();

            repository.Add(new Bookmark()
            {
                CreatedDateTime = DateTime.Now,
                Title = string.IsNullOrEmpty(t) ? bm : t,
                Url = bm
            });
        }

        private void WriteRss(HttpContext context)
        {
            context.Response.Clear();
            context.Response.ContentType = "text/xml";
            XmlTextWriter feedWriter = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8);

            feedWriter.WriteStartDocument();

            // These are RSS Tags
            feedWriter.WriteStartElement("rss");
            feedWriter.WriteAttributeString("version", "2.0");

            feedWriter.WriteStartElement("channel");
            feedWriter.WriteElementString("title", "bookmarks");
            feedWriter.WriteElementString("link", "http://benmcevoy.com.au");
            feedWriter.WriteElementString("description", "Bookmarks");

            var bookmarks = new Repository().Get();

            // Write all Posts in the rss feed
            foreach (var post in bookmarks)
            {
                feedWriter.WriteStartElement("item");
                feedWriter.WriteElementString("title", post.Title);
                feedWriter.WriteElementString("link", post.Url);
                feedWriter.WriteElementString("pubDate", post.CreatedDateTime.ToString());
                feedWriter.WriteEndElement();
            }

            // Close all open tags tags
            feedWriter.WriteEndElement();
            feedWriter.WriteEndElement();
            feedWriter.WriteEndDocument();
            feedWriter.Flush();
            feedWriter.Close();

            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
</code></pre>

[RSS creation taken from here][2]

The repository just serialize to and from disk. The list is ordered by date and filtered to the last 100 entries.  You can work that out yourself.  The Bookmark class is just a simple POCO with CreateDateTime, Url and Title.

The javascriptlet just POSTs the current location.href and document.title, something like this, but minified:

<pre><code>
var http = new XMLHttpRequest();
var url = "http://www.theinternets.com/bookmarks.ashx";
var params = "bm=" + location.href + "&t=" + document.title;
http.open("POST", url, true);

//Send the proper header information along with the request
http.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
http.setRequestHeader("Content-length", params.length);
http.setRequestHeader("Connection", "close");
http.send(params);
</code></pre>

[Mostly nicked from here][3]

And now I can hit the bookmark to add to an RSS feed for reading later.  Sweet.


  [1]: https://blog.mozilla.org/ux/2012/10/save-for-later/
  [2]: http://www.dailycoding.com/Posts/create_rss_feed_programatically_from_data_in_c.aspx
  [3]: http://www.openjs.com/articles/ajax_xmlhttp_using_post.php