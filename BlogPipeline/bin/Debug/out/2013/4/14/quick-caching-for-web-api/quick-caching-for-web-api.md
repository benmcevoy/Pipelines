## Web Api Cache Filter ##

Having recently been playing with the ASP.NET Web API bits lately I found I wanted to be able to cache some of the actions response.

The following will insert cache control headers into the response.  Perfect for responses that are the same for every user.

<pre><code>

    // in the ApiController
    [ApiCache(600)]  // cache for 10 minutes
    public IEnumerable&lt;MyViewModel&gt; Get()
    {
        ....
    }


    public class ApiCacheAttribute : ActionFilterAttribute
    {
        private readonly int _cacheDurationSeconds;

        public ApiCacheAttribute(int seconds = 0)
        {
            _cacheDurationSeconds = seconds;
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            // TODO: would need vary-by-param or something
            // all users are getting the same cached response
            actionExecutedContext
                  .Response
                  .Headers
                  .Add("Cache-Control", "public, max-age=" + _cacheDurationSeconds );

            base.OnActionExecuted(actionExecutedContext);
        }
    }
</code>
</pre>