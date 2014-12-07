using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pipes;

namespace BlogPipeline.Publish
{
    class ProcessFolders : IFilter
    {
        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            var postProcessor = new Pipeline();

            postProcessor.Create(new IFilter[]
            {
                new CreatePostContextFilter(), 
                new CreatePostPage(),

            });


            //find all files for processing
            // and run the pipeline for each


            return postProcessor.Run(context);
        }
    }
}
