using System;
using System.Collections.Generic;

namespace BlogPipeline.Extract.Process
{
    static class ContextExtensions
    {
        public static bool IsPostContextRequest(this IDictionary<string, object> context)
        {
            return context.ContainsKey(Constants.CurrentPostContextKey);
        }

        public static Post GetPost(this IDictionary<string, object> context)
        {
            if (context.IsPostContextRequest())
            {
                return (Post) context[Constants.CurrentPostContextKey];
            }

            throw new InvalidOperationException("context is not a post context");
        }

        public static void SetPost(this IDictionary<string, object> context, Post post)
        {
            context[Constants.CurrentPostContextKey] = post;
        }
    }
}
