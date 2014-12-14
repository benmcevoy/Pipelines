using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using Pipes;

namespace BlogPipeline.Publish
{
    class CreateRssFeed : IFilter
    {
        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            var posts = (List<PostToProcess>)context["posts"];

            const string path = "published//feed//";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var entries = posts.OrderByDescending(process => process.Meta.Published).Take(10);

            CreateFeed(entries);

            return context;
        }

        private void CreateFeed(IEnumerable<PostToProcess> entries)
        {
            var feed = new SyndicationFeed("blog.benmcevoy.com.au", "blog.benmcevoy.com.au", new Uri("http://blog.benmcevoy.com.au/feed"), "FeedID", DateTime.Now);
            var items = new List<SyndicationItem>();

            foreach (var post in entries)
            {
                items.Add(new SyndicationItem(
                    post.Meta.Title,
                    post.BodyHtml,
                    new Uri("http://blog.benmcevoy.com.au/" + post.RelativePath.Replace(@"\\", "/"), UriKind.Absolute),
                    post.RelativePath,
                    post.Meta.Published
                    ));
            }

            feed.Items = items;
            feed.Language = "en-us";
            feed.LastUpdatedTime = DateTime.Now;

            var atomWriter = XmlWriter.Create("published//feed//atom.xml");
            var atomFormatter = new Atom10FeedFormatter(feed);
            atomFormatter.WriteTo(atomWriter);
            atomWriter.Close();
        }
    }
}
