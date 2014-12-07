using System.Collections.Generic;
using System.IO;
using Pipes;

namespace BlogPipeline.Publish
{
    class EnsurePublishedFolder : IFilter
    {
        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            var post = (PostToProcess)context["currentpost"];

            var path = Path.Combine("published", post.RelativePath);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            };

            return context;
        }
    }
}
