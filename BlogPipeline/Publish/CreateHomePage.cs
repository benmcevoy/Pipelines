﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pipes;

namespace BlogPipeline.Publish
{
    class CreateHomePage : IFilter
    {
        private static readonly string PageTemplate;
        private static readonly string NavPartial;

        static CreateHomePage()
        {
            PageTemplate = File.ReadAllText("Publish\\Templates\\page.html");
            NavPartial = File.ReadAllText("Publish\\Templates\\_nav.html");
        }

        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            var posts = (List<PostToProcess>)context["posts"];
            var post = posts.OrderByDescending(process => process.Meta.Published).First();
            var html = PageTemplate.Replace("{{renderbody}}", post.BodyHtml);

            html = html.Replace("{{title}}", post.Meta.Title);
            html = html.Replace("{{keywords}}", post.Meta.Keywords);
            html = html.Replace("{{summary}}", post.Meta.Summary);
            html = html.Replace("{{published}}", post.Meta.Published.ToString("dddd MMMM yyyy"));
            html = html.Replace("{{nav}}", NavPartial);
            html = html.Replace("{{navigation}}", (string)context["navigation"]);

            File.WriteAllText(string.Format("{0}\\{1}.html", "published", "index"), html);

            return context;
        }
    }
}
