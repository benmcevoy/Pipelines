using System.Collections.Generic;
using System.Data.SqlClient;
using BlogPipeline.Extract.Process;
using Pipes;

namespace BlogPipeline.Extract
{
    class GetPostsFilter : IFilter
    {
        private readonly ILog _log;

        public GetPostsFilter(ILog log)
        {
            _log = log;
        }

        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            var processor = CreatePostPipeline();

            foreach (var post in GetPosts())
            {
                context[Constants.CurrentPostContextKey] = post;
                context = processor.Run(context);
            }

            return context;
        }

        private static IEnumerable<Post> GetPosts()
        {
            using (var cn = new SqlConnection(@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=funnelweb;Data Source=TOWER\SQLEXPRESS;"))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = cn;

                cmd.CommandText = @"SELECT [Name], [Title], [Published], [Body] FROM [dbo].[Entry]";

                cn.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    yield return new Post
                    {
                        Title = (string)reader.GetValue(1),
                        Body = (string)reader.GetValue(3),
                        Published = reader.GetDateTime(2),
                        Slug = (string)reader.GetValue(0),
                    };
                }
            }
        }

        private Pipeline CreatePostPipeline()
        {
            return Pipeline.Create(new IFilter[]
            {
                new EnsureFolderFilter(_log), 
                new WriteMetaFilter(_log), 
                new WriteMarkdownFilter(_log)
            });
        }
    }
}
