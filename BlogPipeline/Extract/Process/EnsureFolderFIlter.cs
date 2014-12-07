using System.Collections.Generic;
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

            var slug = Path.Combine(basePath, post.Slug);

            EnsureFolder(basePath);
            EnsureFolder(slug);

            post.Path = slug;

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
