using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Pipes;
using Radio7.Portable.OpenTextSummarizer;

namespace BlogPipeline.Extract.Process
{
    class WriteMetaFilter : IFilter
    {
        private readonly ILog _log;

        public WriteMetaFilter(ILog log)
        {
            _log = log;
        }

        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            var post = context.GetPost();

            _log.Log("WriteMetaFilter");


            var summary = GetSummary(post);

            post.Summary = string.Join(" ", summary.Sentences);
            post.Keywords = string.Join(", ", summary.Concepts);

            _log.Log(post.Summary);

            context.SetPost(post);

            var meta = new
            {
                published = post.Published,
                summary = post.Summary,
                keywords = post.Keywords,
                slug = post.Slug,
                title = post.Title
            };

            File.WriteAllText(post.Path + "\\meta.json", JsonConvert.SerializeObject(meta));

            return context;
        }

        private SummarizedDocument GetSummary(Post post)
        {
            var summary = Summarizer.Summarize(new SummarizerArguments
            {
                // ah... markdown is clean text
                DisplayLines = 1,
                InputString = post.Body.RemoveTags().RemoveWhitespace().RemoveMdCrap()
            });

            return summary;
        }
    }
}
