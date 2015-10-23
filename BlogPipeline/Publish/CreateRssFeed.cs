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
            var feed = new SyndicationFeed("Ben McEvoy", "Just stuff for fun", new Uri("http://benmcevoy.com.au/blog/feed"), "http://benmcevoy.com.au/blog/feed", DateTime.Now);
            var items = new List<SyndicationItem>();

            foreach (var post in entries)
            {
                var entry = new SyndicationItem(
                    post.Meta.Title,
                    post.BodyHtml,
                    new Uri("http://benmcevoy.com.au/blog/" + post.RelativePath.Replace(@"\\", "/"), UriKind.Absolute),
                    post.RelativePath,
                    post.Meta.Published
                    );

                entry.PublishDate = post.Meta.Published;

                items.Add(entry);
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
