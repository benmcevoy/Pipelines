using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Pipes;

namespace BlogPipeline.Extract.Process
{
    class EnsureFolderFilter : IFilter
    {
        private readonly ILog _log;

        public EnsureFolderFilter(ILog log)
        {
            _log = log;
        }

        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            var post = context.GetPost();

            _log.Log("EnsureFolderFilter:" + post.Published);

            var basePath = "out";
            var year = post.Published.Year.ToString(CultureInfo.InvariantCulture);
            var month = post.Published.Month.ToString(CultureInfo.InvariantCulture);
            var day = post.Published.Day.ToString(CultureInfo.InvariantCulture);
            var slug = post.Slug;

            var final = Path.Combine(basePath, year, month, day, slug);

            EnsureFolder(basePath);
            EnsureFolder(Path.Combine(basePath, year));
            EnsureFolder(Path.Combine(basePath, year, month));
            EnsureFolder(Path.Combine(basePath, year, month, day));

            EnsureFolder(final);

            post.Path = final;

            context.SetPost(post);

            return context;
        }

        private static void EnsureFolder(string path)
        {
            if (Directory.Exists(path)) return;

            Directory.CreateDirectory(path);
        }
    }
}
