//using System;
//using System.Collections.Generic;

//namespace Pipelines
//{
//    /// <summary>
//    /// Encapsulate a pipeline whose Next property returns null
//    /// </summary>
//    internal sealed class TerminalPipeline : IPipeline
//    {
//        public TerminalPipeline()
//        {
//            Next = (_ => null);
//        }

//        public IPipeline Run(IDictionary<string, object> context)
//        {
//            return Next(context);
//        }

//        public Func<IDictionary<string, object>, IPipeline> Next { get; set; }
//    }
//}
