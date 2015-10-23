# Pipelines

A most simple implementation of pipes and filters

Pipes

```
    public class Pipeline : IFilter
    {
        private readonly IEnumerable<IFilter> _filters;

        protected Pipeline(IEnumerable<IFilter> filters)
        {
            _filters = filters;
        }

        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            return _filters.Aggregate(context, (current, filter) => filter.Run(current));
        }
    }
```

and Filters

```
    public interface IFilter
    {
        IDictionary<string, object> Run(IDictionary<string, object> context);
    }
```

and a bunch of other code as i explore some options
