using System.Collections.Generic;
using System.IO;
using Pipes;

namespace BlogPipeline.Publish
{
    class CopyAssetsFolder :IFilter
    {
        public IDictionary<string, object> Run(IDictionary<string, object> context)
        {
            var post = (PostToProcess)context["currentpost"];

            DirectoryCopy(Path.Combine(post.SourcePath, "assets"), Path.Combine("published", post.RelativePath, "assets"), true); 

            return context;
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists) return;

            var dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(destDirName, file.Name);

                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (!copySubDirs) return;

            foreach (var subdir in dirs)
            {
                var temppath = Path.Combine(destDirName, subdir.Name);

                DirectoryCopy(subdir.FullName, temppath, copySubDirs);
            }
        }
    }
}
