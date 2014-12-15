using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pipes;

namespace BlogPipeline.Publish
{
    class EnsureNavigationPage : IFilter
    {
        private static readonly string PageTemplate;
        private static readonly string NavPartial;

        static EnsureNavigationPage()
        {
            PageTemplate = File.ReadAllText("Publish\\Templates\\navigation.html");
            NavPartial = File.ReadAllText("Publish\\Templates\\_nav.html");
        }

        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            var post = (PostToProcess)context["currentpost"];
            var yearPath = Path.Combine("published", string.Format("{0:yyyy}\\index.html", post.Meta.Published));
            var monthPath = Path.Combine("published", string.Format("{0:yyyy}\\{0:MM}\\index.html", post.Meta.Published));
            var posts = (List<PostToProcess>)context["posts"];

            if (!Directory.Exists(yearPath))
            {
                var body = posts
                    .Where(process => process.Meta.Published.Year == post.Meta.Published.Year)
                    .Select(ToBody);

                WriteNavigationIndex( string.Join("", body), post.Meta.Published.ToString("yyyy"), yearPath);
            }

            if (!Directory.Exists(monthPath))
            {
                var body = posts
                    .Where(process => process.Meta.Published.Year == post.Meta.Published.Year 
                    && process.Meta.Published.Month == post.Meta.Published.Month )
                    .Select(ToBody);

                WriteNavigationIndex( string.Join("", body), post.Meta.Published.ToString("MMMM yyyy"), monthPath);
            };

            return context;
        }

        private static string ToBody(PostToProcess post)
        {
            return string.Format(Inner, string.Format("/{0:yyyy}/{0:MM}/{1}", post.Meta.Published, post.Meta.Slug), post.Meta.Title );
        }

        private static void WriteNavigationIndex( string body, string title, string path)
        {
            body = string.Format(@"<ul class=""list-unstyled"">{0}</ul>", body);

            var html = PageTemplate.Replace("{{renderbody}}", body);

            html = html.Replace("{{title}}", title);
            html = html.Replace("{{nav}}", NavPartial);

            File.WriteAllText(path, html);
        }

        private const string Inner = @"<li><a href=""{0}/"">{1}</a></li>";
    }
}
