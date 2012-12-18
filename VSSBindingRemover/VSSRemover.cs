using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace VSSBindingRemover
{
    public class VSSRemover
    {
        private readonly DirectoryInfo selectedPath;
        private List<FileInfo> slnFiles;
        private List<FileInfo> projFiles;
        private List<FileInfo> vssFiles;

        public VSSRemover(string selectedPath)
        {
            var path = new DirectoryInfo(selectedPath);
            this.selectedPath = path;
        }

        public void RemoveBindings()
        {
            this.GetFiles();
            this.RemoveReadOnlyAttributes();
            this.DeleteVssFiles();
            this.EditSolutionFiles();
            this.EditProjectFiles();
        }

        private void GetFiles()
        {
            slnFiles = new List<FileInfo>(selectedPath.GetFiles("*.sln", SearchOption.AllDirectories));
            projFiles = new List<FileInfo>(selectedPath.GetFiles("*.*proj", SearchOption.AllDirectories));
            vssFiles = new List<FileInfo>(selectedPath.GetFiles("*.vssscc", SearchOption.AllDirectories));
            vssFiles.AddRange(selectedPath.GetFiles("*.vsscc", SearchOption.AllDirectories));
            vssFiles.AddRange(selectedPath.GetFiles("*.vssscc", SearchOption.AllDirectories));
            vssFiles.AddRange(selectedPath.GetFiles("*.scc", SearchOption.AllDirectories));
            vssFiles.AddRange(selectedPath.GetFiles("*.vspscc", SearchOption.AllDirectories));
            vssFiles.AddRange(selectedPath.GetFiles("*.suo", SearchOption.AllDirectories));
        }

        private void RemoveReadOnlyAttributes()
        {
            // Remove Read-Only attributes from all files
            var allFiles = new List<FileInfo>(selectedPath.GetFiles("*", SearchOption.AllDirectories));

            allFiles.ForEach(f => f.IsReadOnly = false);
        }

        private void DeleteVssFiles()
        {
            vssFiles.ForEach(file => file.Delete());
        }

        private void EditSolutionFiles()
        {
            slnFiles.ForEach(sln =>
            {
                string fullName = sln.FullName;
                string relPath = sln.Directory.FullName.Replace(selectedPath.FullName, string.Empty);

                StreamReader reader = sln.OpenText();
                String text = reader.ReadToEnd();
                reader.Close();
                string regex = @"\tGlobalSection\(SourceCodeControl\) [\s\S]*? EndGlobalSection\r\n";
                RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
                Regex reg = new Regex(regex, options);

                text = reg.Replace(text, string.Empty);
                if (text.StartsWith(Environment.NewLine))
                    text = text.Remove(0, 2);
                StreamWriter writer = new StreamWriter(fullName);
                writer.Write(text);
                writer.Flush();
                writer.Close();
            });
        }

        private void EditProjectFiles()
        {
            projFiles.ForEach(proj =>
            {
                string fullName = proj.FullName;
                string relPath = proj.Directory.FullName.Replace(selectedPath.FullName, string.Empty);

                StreamReader reader = proj.OpenText();
                String text = reader.ReadToEnd();
                reader.Close();

                string regex = "";
                string replacementString = "";

                if (proj.Extension.Equals(".dtproj"))
                {
                    regex = @"^.*(State).*\n?";
                    replacementString = "<State>$base64$PFNvdXJjZUNvbnRyb2xJbmZvIHhtbG5zOnhzZD0iaHR0cDovL3d3dy53My5vcmcvMjAwMS9YTUxTY2hlbWEiIHhtbG5zOnhzaT0iaHR0cDovL3d3dy53My5vcmcvMjAwMS9YTUxTY2hlbWEtaW5zdGFuY2UiIHhtbG5zOmRkbDI9Imh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vYW5hbHlzaXNzZXJ2aWNlcy8yMDAzL2VuZ2luZS8yIiB4bWxuczpkZGwyXzI9Imh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vYW5hbHlzaXNzZXJ2aWNlcy8yMDAzL2VuZ2luZS8yLzIiIHhtbG5zOmRkbDEwMF8xMDA9Imh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vYW5hbHlzaXNzZXJ2aWNlcy8yMDA4L2VuZ2luZS8xMDAvMTAwIiB4bWxuczpkZGwyMDA9Imh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vYW5hbHlzaXNzZXJ2aWNlcy8yMDEwL2VuZ2luZS8yMDAiIHhtbG5zOmRkbDIwMF8yMDA9Imh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vYW5hbHlzaXNzZXJ2aWNlcy8yMDEwL2VuZ2luZS8yMDAvMjAwIiB4bWxuczpkd2Q9Imh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vRGF0YVdhcmVob3VzZS9EZXNpZ25lci8xLjAiPg0KICA8RW5hYmxlZD5mYWxzZTwvRW5hYmxlZD4NCiAgPFByb2plY3ROYW1lPjwvUHJvamVjdE5hbWU+DQogIDxBdXhQYXRoPjwvQXV4UGF0aD4NCiAgPExvY2FsUGF0aD48L0xvY2FsUGF0aD4NCiAgPFByb3ZpZGVyPjwvUHJvdmlkZXI+DQo8L1NvdXJjZUNvbnRyb2xJbmZvPg==</State>";
                }
                else
                {
                    regex = @"^.*(SccProjectName|SccLocalPath|SccAuxPath|SccProvider).*\n?";
                    replacementString = string.Empty;
                }

                RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
                Regex reg = new Regex(regex, options);

                text = reg.Replace(text, replacementString);

                StreamWriter writer = new StreamWriter(fullName);
                writer.Write(text);
                writer.Flush();
                writer.Close();
            });
        }
    }
}
