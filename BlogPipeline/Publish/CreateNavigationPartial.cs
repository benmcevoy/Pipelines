using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pipes;

namespace BlogPipeline.Publish
{
    class CreateNavigationPartial : IFilter
    {
        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            var posts = (List<PostToProcess>)context["posts"];

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
                inner.AppendFormat(
                    Inner, 
                    string.Format("/{0:yyyy}/{0:MM}/{1}", post.Meta.Published, post.Meta.Slug), post.Meta.Title);
            }

            return new Tuple<int, string>(postToProcesses.First().Meta.Published.Year, inner.ToString());
        }

        private const string Inner = @"<li><a href=""{0}/"">{1}</a></li>";

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
