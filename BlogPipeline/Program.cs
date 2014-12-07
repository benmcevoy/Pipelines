using System;
using System.Collections.Generic;
using BlogPipeline.Extract;
using BlogPipeline.Publish;

namespace BlogPipeline
{
    class Program
    {
        static void Main(string[] args)
        {
            new ExtractFromFunnelwebPipeline(new ConsoleLog()).Run( CreateContext());

            new PublishPipeline().Run(CreateContext());

            Console.ReadKey();
        }

        private static IDictionary<string, object> CreateContext()
        {
            return new Dictionary<string, object>(32);
        }
    }
}
