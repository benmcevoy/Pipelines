using System.Collections.Generic;

namespace Pipes
{
    public interface IFilter
    {
        IDictionary<string, object> Run(IDictionary<string, object> context);
    }
}