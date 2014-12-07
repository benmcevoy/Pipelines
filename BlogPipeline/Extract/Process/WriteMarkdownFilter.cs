using System.Collections.Generic;
using System.IO;
using Pipes;

namespace BlogPipeline.Extract.Process
{
    class WriteMarkdownFilter : IFilter
    {
        private readonly ILog _log;

        public WriteMarkdownFilter(ILog log)
        {
            _log = log;
        }

        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            var post = context.GetPost();

            _log.Log("WriteMarkdownFilter: " + post.Path);

            var md = new Markdown("");
            var article = md.Render(post.Body);

            var mdPath = string.Format("{0}\\{1}.md", post.Path, post.Slug);
            var htmlPath = string.Format("{0}\\{1}.html", post.Path, post.Slug);

            File.WriteAllText(mdPath, post.Body);
            File.WriteAllText(htmlPath, article);

            post.HtmlPath = htmlPath;
            post.MarkdownPath = mdPath;

            context.SetPost(post);

            return context;
        }
    }
}
