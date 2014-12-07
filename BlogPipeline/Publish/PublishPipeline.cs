using System.Collections.Generic;
using Pipes;

namespace BlogPipeline.Publish
{
    class PublishPipeline  : IFilter
    {
        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            var postProcessor = new Pipeline();

            postProcessor.Create(new IFilter[]
            {
                new ProcessFolders(),  
                new CreateNavigationPages(), 

            });

            return postProcessor.Run(context);
        }
    }
}
