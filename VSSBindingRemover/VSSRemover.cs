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
        }

        private void RemoveReadOnlyAttributes()
        {
            var allFiles = slnFiles.Union(projFiles).Union(vssFiles).ToList();

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
                                     string regex = "\tGlobalSection\\(SourceCodeControl\\) [\\s\\S]*? EndGlobalSection\r\n";
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

                string regex = "\"*<*Scc.*(>|\\W=\\W\").*?(>|%\")";
                RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
                Regex reg = new Regex(regex, options);

                text = reg.Replace(text, string.Empty);
                StreamWriter writer = new StreamWriter(fullName);
                writer.Write(text);
                writer.Flush();
                writer.Close();
            });
        }
    }
}
