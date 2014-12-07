using System;

namespace BlogPipeline.Extract
{
    class Post
    {
        public string Body { get; set; }

        public DateTime Published { get; set; }

        public string Slug { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Path { get; set; }
        
        public string Keywords { get; set; }

        public string MarkdownPath { get; set; }

        public string HtmlPath { get; set; }
    }
}
