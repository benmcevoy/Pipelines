﻿using System.Collections.Generic;
using System.IO;
using Pipes;

namespace BlogPipeline.Publish
{
    class CreatePostPage : IFilter
    {
        private static readonly string PageTemplate;

        static CreatePostPage()
        {
            PageTemplate = File.ReadAllText("Publish\\Templates\\page.html");
        }

        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            var post = (PostToProcess)context["currentpost"];

            var html = PageTemplate.Replace("{{renderbody}}", post.BodyHtml);

            File.WriteAllText(string.Format("{0}{1}.html", Path.Combine("published", post.RelativePath), post.Meta.Slug), html);

            return context;
        }
    }
}