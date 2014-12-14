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
                new CreateNavigationPartial(), 
                new ProcessFolders(),  
                

            });

            return postProcessor.Run(context);
        }
    }
}
