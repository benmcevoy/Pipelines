# Pipelines

Pipes

```
    public class Pipeline : IFilter
    {
        private readonly IEnumerable<IFilter> _filters;

        protected Pipeline()
        {
            _filters = new List<IFilter>(32);
        }

        protected Pipeline(IEnumerable<IFilter> filters)
        {
            _filters = filters;
        }

        public static Pipeline Create(IEnumerable<IFilter> filters)
        {
            return new Pipeline(filters);
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
