using System.Collections.Generic;
using System.IO;
using Pipes;

namespace BlogPipeline.Publish
{
    class CreateNavigationPages : IFilter
    {
        private static readonly string PageTemplate;

        static CreateNavigationPages()
        {
            PageTemplate = File.ReadAllText("Publish\\Templates\\navigation.html");
        }

        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            return context;

            //var post = (PostToProcess)context["currentpost"];

            //var html = PageTemplate.Replace("{{renderbody}}", post.BodyHtml);
            //html = html.Replace("{{title}}", post.Meta.Title);
            //html = html.Replace("{{keywords}}", post.Meta.Keywords);
            //html = html.Replace("{{summary}}", post.Meta.Summary);
            //html = html.Replace("{{published}}", post.Meta.Published.ToString("dddd MMMM yyyy"));

            //File.WriteAllText(string.Format("{0}{1}.html", Path.Combine("published", post.RelativePath), "index"), html);

            return context;
        }
    }
}
