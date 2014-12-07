using System.Collections.Generic;
using Pipes;

namespace BlogPipeline.Publish
{
    class CreateNavigationPages : IFilter
    {
        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            return context;
        }
    }
}
