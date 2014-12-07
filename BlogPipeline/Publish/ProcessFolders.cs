using System.Collections.Generic;
using System.IO;
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
                new EnsurePublishedFolder(), 
                new CreatePostPage(),
            });

            foreach (var fileToProcess in GetFilesToProcess("out"))
            {
                context["currentpath"] = fileToProcess;

                context = postProcessor.Run(context);
            }

            return context;
        }

        private IEnumerable<string> GetFilesToProcess(string path)
        {
            return Directory.GetDirectories(path);
        }
    }
}
