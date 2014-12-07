//using System;
//using System.Collections.Generic;

//namespace Pipelines
//{
//    public class PipelineRunner : IPipeline
//    {
//        private readonly IPipeline _head;

//        public PipelineRunner(IPipeline head)
//        {
//            if (head == null) throw new NullReferenceException("head");

//            _head = head;

//            Next = (_ => head);
//        }

//        public IPipeline Run(IDictionary<string, object> context)
//        {
//            var next = Next(context);

//            return next == null
//                ? _head
//                : RunImpl(next, context);
//        }

//        private static IPipeline RunImpl(IPipeline pipeline, IDictionary<string, object> context)
//        {
//            while (true)
//            {
//                if (pipeline == null) return null;

//                if (pipeline.Next == null) pipeline.Next = _ => new TerminalPipeline();

//                var nextPipe = pipeline.Run(context);

//                if (nextPipe == null) return pipeline;

//                pipeline = nextPipe.Run(context);
//            }
//        }

//        public Func<IDictionary<string, object>, IPipeline> Next { get; set; }
//    }
//}
