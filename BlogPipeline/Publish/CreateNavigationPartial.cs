using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Pipes;

namespace BlogPipeline.Publish
{
    class CreateNavigationPartial : IFilter
    {
        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            var posts = new List<PostToProcess>(32);

            foreach (var fileToProcess in GetFilesToProcess("out"))
            {
                var meta = JsonConvert.DeserializeObject<Meta>(File.ReadAllText(fileToProcess + "\\meta.json"));

                var post = new PostToProcess
                {
                    BodyHtml = File.ReadAllText(fileToProcess + "\\" + meta.Slug + ".html"),
                    Meta = meta,
                    RelativePath = string.Format("{0:yyyy}\\{0:MM}\\{1}\\", meta.Published, meta.Slug)
                };

                posts.Add(post);
            }

            var years =
                posts.OrderByDescending(process => process.Meta.Published)
                    .GroupBy(process => process.Meta.Published.Year)
                    .Select(ToNav);

            var html = new StringBuilder(1024);

            foreach (var year in years)
            {
                html.AppendFormat(Outer, year.Item1, year.Item2);
            }

            context["navigation"] = html.ToString();

            return context;
        }


        private static Tuple<int, string> ToNav(IEnumerable<PostToProcess> postsForTheYear)
        {
            var inner = new StringBuilder();

            var postToProcesses = postsForTheYear as PostToProcess[] ?? postsForTheYear.ToArray();

            foreach (var post in postToProcesses)
            {
                inner.AppendFormat(Inner, string.Format("/{0:yyyy}/{0:MM}/{1}", post.Meta.Published, post.Meta.Slug), post.Meta.Title);
            }

            return new Tuple<int, string>(postToProcesses.First().Meta.Published.Year, inner.ToString());
        }

        private static IEnumerable<string> GetFilesToProcess(string path)
        {
            return Directory.GetDirectories(path);
        }

        private const string Inner = @"<li><a href=""{0}"">{1}</a></li>";

        private const string Outer = @"<div class=""col-md-4"">
                <div class=""well"">
                    <h4>{0}</h4>
                    <div class=""row"">
                        <div class=""col-lg-12"">
                            <ul class=""list-unstyled"">
                                {1}
                            </ul>
                        </div>
                    </div>
                </div>
            </div>";
    }
}
