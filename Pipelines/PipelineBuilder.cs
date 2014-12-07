//using System;
//using System.Collections.Generic;

//namespace Pipelines
//{
//    public class PipelineBuilder : IPipeline
//    {
//        private readonly IPipeline _head;
//        private IPipeline _prev;

//        public PipelineBuilder(IPipeline head)
//        {
//            if (head == null) throw new ArgumentNullException("head");

//            _head = _prev = head;

//            Next = (_ => _head);
//        }

//        public PipelineBuilder Then(IPipeline next)
//        {
//            if (next.Next == null) next.Next = (_ => new TerminalPipeline());

//            _prev.Next = next.Run;
//            _prev = next;

//            return this;
//        }

//        public IPipeline Run(IDictionary<string, object> context)
//        {
//            context = context ?? new Dictionary<string, object>(16);
//            return new PipelineRunner(_head).Run(context);
//        }

//        public Func<IDictionary<string, object>, IPipeline> Next { get; set; }
//    }
//}
