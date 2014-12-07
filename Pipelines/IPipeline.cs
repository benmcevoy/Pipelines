using System;
using System.Collections.Generic;

namespace Pipelines
{
    // we initally thought "I want a task, with a bunch of subtasks"
    public interface IPipelineV1
    {
        IDictionary<string, object> Run(IDictionary<string, object> context);

        IEnumerable<IPipelineV1> Pipes { get; set; }
    }

    // but then we realised that when it ran it was flattened into a list (and we took a snkeaky peek at OWIN)
    public interface IPipelineV2
    {
        IDictionary<string, object> Run(IDictionary<string, object> context);

        IPipelineV2 Next { get; set; }
    }

    // then we took a bigger peek at OWIN
    public interface IPipelineV3
    {
        IPipelineV3 Run(IDictionary<string, object> context);

        Func<IDictionary<string, object>, IPipelineV3> Next { get; set; }
    } 

    // then we thought of the simplest thing we could come up with
    // maybe just Run(object state) is simpler, but a pain to use.
    public interface IPipeline
    {
        IDictionary<string, object> Run(IDictionary<string, object> context);
    }
}
