//using System;
//using System.Collections.Generic;
//using nextFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, Pipelines.IPipeline>;

//namespace Pipelines.Pipes.ProcessArticle
//{
//    class GenerateTitlePipe : IPipeline
//    {
//        //public GenerateTitlePipe(nextFunc next)
//        //{
//        //    if (next == null) throw new ArgumentNullException("next");
//        //    Next = next;
//        //}

//        public IPipeline Run(IDictionary<string, object> context)
//        {
//            Console.WriteLine("GenerateTitlePipe");
//            context["GenerateTitlePipe"] = "my title";

//            return Next(context);
//        }

//        public Func<IDictionary<string, object>, IPipeline> Next { get; set; }
//    }
//}
