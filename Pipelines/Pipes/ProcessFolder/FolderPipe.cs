//using System;
//using System.Collections.Generic;
//using nextFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, Pipelines.IPipeline>;

//namespace Pipelines.Pipes.ProcessFolder
//{
//    class FolderPipe : IPipeline
//    {
//        //public FolderPipe(nextFunc next)
//        //{
//        //    if (next == null) throw new ArgumentNullException("next");
//        //    Next = next;
//        //}

//        public IPipeline Run(IDictionary<string, object> context)
//        {
//            Console.WriteLine("FolderPipe");
//            context["FolderPipe"] = 1;

//            return Next(context);
//        }

//        public nextFunc Next { get; set; }
//    }
//}
