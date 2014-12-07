using System;
using System.Collections.Generic;

namespace Pipelines
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new Dictionary<string, object>();

            //new FolderPipe(new ArticlePipe(new GenerateTitlePipe().Run).Run).Run(ctx);


            // the simplest pipeline acceptx a context and returns a mutated context
            // the builder is just a FIFO thing
            // the runner pops one off and runs it till there is no more

            // in reality you want
            // Task continuation ala owin
            // a builder that accepts typeof(pipeline) so you can use DI to construct it
            // a little ambiance so you can abort a pipeline, although throw will do it
            // so basically OWIN


            //new PipelineBuilder(new FolderPipe())
            //    .Then(new FolderPipe())
            //    .Then(new GenerateTitlePipe())
            //    .Then(new GenerateTitlePipe())
            //    .Then(new FolderPipe())
            //    .Then(new GenerateTitlePipe())
            //    .Then(new FolderPipe())
            //    .Run(ctx);

            //Console.ReadKey();
        }
    }
}
