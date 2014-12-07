using System.Collections.Generic;
using Pipes;

namespace BlogPipeline.Extract
{
    class ExtractFromFunnelwebPipeline :IFilter
    {
        private readonly ILog _log;

        public ExtractFromFunnelwebPipeline(ILog log)
        {
            _log = log;
        }

         public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            var pipeline = new Pipeline();

            pipeline.Create(new IFilter[]
            {
                new GetPostsFilter(_log), 
            });

            return pipeline.Run(context);
        }
    }
}
