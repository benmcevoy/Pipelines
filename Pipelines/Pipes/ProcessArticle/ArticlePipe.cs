//using System;
//using System.Collections.Generic;
//using nextFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, Pipelines.IPipeline>;

//namespace Pipelines.Pipes.ProcessArticle
//{
//    class ArticlePipe : IPipeline
//    {
//        //public ArticlePipe(nextFunc next)
//        //{
//        //    if (next == null) throw new ArgumentNullException("next");
//        //    Next = next;
//        //}

//        public IPipeline Run(IDictionary<string, object> context)
//        {
//            Console.WriteLine("ArticlePipe");
//            context["ArticlePipe"] = 2;

//            return Next(context);
//        }

//        public Func<IDictionary<string, object>, IPipeline> Next { get; set; }
//    }
//}
