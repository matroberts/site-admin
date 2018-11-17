using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NUnit.Framework;

namespace siteadmin
{
    [TestFixture]
    public class Admin
    {
        public string SiteRootPath { get; } = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\matroberts.github.io");
        public string TemplatePath { get; } = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\BlogTemplate.html");

        public Regex PostNamePattern = new Regex(@"\d\d\d\d-\d\d-\d\d-.*\.html");

        [Test]
        public void MakeIndex()
        {
            var template = File.ReadAllText(TemplatePath);
            var postdate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var filename = "index.html";
            var canonicalurl = $"https://moleseyhill.com/{filename}";

            var filenames = Directory.GetFiles(SiteRootPath)
                .Where(f => PostNamePattern.IsMatch(Path.GetFileName(f)))
                .Select(f => Path.GetFileName(f))
                .OrderByDescending(f => f)
                .ToList();

            var postLinks = new StringBuilder();
            foreach (var f in filenames)
            {
                var doc = new HtmlDocument();
                doc.Load(Path.Combine(SiteRootPath, f));
                var title = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;
                postLinks.AppendLine($"<li><a href=\"{f}\">{title}</a></li>");
            }

            string content = $@"
<h3>Posts</h3>
<ul>
{postLinks}
</ul>

<h3>Little Projects</h3>
<ul>
<li><a href=""code/RsVsShrtCt/Resharper-VisualStudio-Shortcuts.html"">Resharper Shortcuts</a></li>
<li><a href=""code/Typing/Lesson.htm"">Learn to touchtype</a></li>
<li><a href=""code/Pomodoro/Timer.htm"">Pomodoro Timer</a></li>
</ul>
";
            var newpost = template.Replace("TODO-TITLE", "Struggling for Competence")
                .Replace("TODO-DESCRIPTION", "Mat Roberts website.  Programming, Development, TDD, Test Driven Design, C#, ASP.NET MVC, SQL Server, Windows")
                .Replace("TODO-CANONICALURL", canonicalurl)
                .Replace("TODO-POSTDATE", postdate)
                .Replace("TODO-POSTDATE", postdate)
                .Replace("TODO-CONTENT", content);

            File.WriteAllText(Path.Combine(SiteRootPath, filename), newpost, new UTF8Encoding(false));
        }

        [Test]
        public void ValidateHtml()
        {
            foreach (var path in Directory.GetFiles(SiteRootPath).Where(f => f.EndsWith(".html")))
            {
                var doc = new HtmlDocument();
                doc.Load(Path.Combine(SiteRootPath, path), new UTF8Encoding(false));
                doc.OptionEmptyCollection = true;  // allows select nodes to return empty collection instead of null, when no matches - WTF were they thinking
                var file = Path.GetFileName(path);

                // parse errors
                foreach (var error in doc.ParseErrors)
                {
                    Console.WriteLine($"{file}({error.Line},{error.LinePosition}): {error.Code}: {error.Reason}");
                }

                // <img> tags with a title attribute
                foreach (var img in doc.DocumentNode.SelectNodes("//img"))
                {
                    if(img.ParentNode.Name != "figure")
                        Console.WriteLine($"{file}({img.Line},{img.LinePosition}): M1: img not in a figure");
                }

                // <p> tag containing <ul>
                // <img> tag with empty alt
                // <title> empty
                // <meta description> empty
                // <time> empty
                //
            }
        }
    }
}
