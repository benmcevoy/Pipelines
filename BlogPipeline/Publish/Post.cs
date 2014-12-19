using System;

namespace BlogPipeline.Publish
{
    class PostToProcess
    {
        public string SourcePath { get; set; }

        public string RelativePath { get; set; }

        public Meta Meta { get; set; }

        public string BodyHtml { get; set; }
    }

    class Meta
    {
        public DateTime Published { get; set; }

        public string Slug { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Keywords { get; set; }
    }
}
