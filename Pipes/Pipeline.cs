using System.Collections.Generic;
using System.Linq;

namespace Pipes
{
    public class Pipeline : IFilter
    {
        private IEnumerable<IFilter> _filters;

        public Pipeline()
        {
            _filters = new List<IFilter>(32);
        }

        public void Create(IEnumerable<IFilter> filters)
        {
            _filters = filters;
        }

        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            return _filters.Aggregate(context, (current, filter) => filter.Run(current));
        }
    }
}
