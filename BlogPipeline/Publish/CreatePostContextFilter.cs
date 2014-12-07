using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Pipes;

namespace BlogPipeline.Publish
{
    class CreatePostContextFilter: IFilter
    {
        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            var currentPath = context["currentpath"];

            var meta = JsonConvert.DeserializeObject<Meta>(File.ReadAllText(currentPath + "\\meta.json"));

            var post = new PostToProcess
            {
                BodyHtml =  File.ReadAllText(currentPath + "\\" + meta.Slug + ".html"),
                Meta = meta,
                RelativePath = string.Format("{0:yyyy}\\{0:MM}\\{1}\\", meta.Published, meta.Slug)
            };

            context["currentpost"] = post;

            return context;

        }
    }
}
