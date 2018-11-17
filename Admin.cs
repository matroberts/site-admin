using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

            string posts = $@"
<h3>Posts</h3>
<ul>
{string.Join("\r\n", filenames.Select(f => $"<li><a href=\"{f}\">{f}</a></li>"))}
</ul>";

            var littleprojects = $@"
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
                .Replace("TODO-CONTENT", posts + littleprojects);

            File.WriteAllText(Path.Combine(SiteRootPath, filename), newpost, new UTF8Encoding(false));

        }
    }
}
