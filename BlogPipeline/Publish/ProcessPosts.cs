using System.Collections.Generic;
using Pipes;

namespace BlogPipeline.Publish
{
    class ProcessPosts : IFilter
    {
        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            var postProcessor = Pipeline.Create(new IFilter[]
            {
                new EnsurePublishedFolder(), 
                new EnsureNavigationPage(), 
                new CreatePostPage(),
                new CopyAssetsFolder(), 
            });

            var posts = (List<PostToProcess>)context["posts"];

            foreach (var postToProcess in posts)
            {
                context["currentpost"] = postToProcess;
                context = postProcessor.Run(context);
            }

            return context;
        }
    }
}
