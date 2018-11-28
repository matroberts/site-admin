using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                var description = doc.DocumentNode.SelectSingleNode("//head/meta[@name='description']")?.Attributes["content"].Value ?? "";
                postLinks.AppendLine($"<dt><a href=\"{f}\">{title}</a></dt><dd>{description}</dd>");
            }

            string menu = $@"
<menu>
<li><a href=""{filenames.First()}"">Previous</a></li>
<li><a href=""{filenames.Last()}"">Next</a></li>
</menu>
";

            string content = $@"
<dl>
{postLinks}
</dl>

<h3>Little Projects</h3>
<dl>
<dt><a href=""code/RsVsShrtCt/Resharper-VisualStudio-Shortcuts.html"">Resharper Shortcuts</a></dt><dd>Selected Resharper and Visual Studio Keyboad Shortcuts (IntelliJ Keybindings).</dd>
<dt><a href=""code/Typing/Lesson.htm"">Learn to touchtype</a></dt><dd>Online tool for learning to touch type.</dd>
<dt><a href=""code/Pomodoro/Timer.htm"">Pomodoro Timer</a></dt><dd>Online timer for the Pomodoro technique.</dd>
</dl>

<h3>Credits</h3>
<dl>
<dt><a href=""https://github.com/"">Hosting by Github</a></dt>
<dt><a href=""https://prismjs.com/"">Syntax Highlighting by Prism</a></dt>
<dt><a href=""https://github.com/csstools/sanitize.css"">Css Reset by sanitize.css</a></dt>
<dt><a href=""https://html-agility-pack.net/"">Mass edits by Html Agility Pack</a></dt>
</dl>
";
            var newpost = template.Replace("TODO-TITLE", "Articles")
                .Replace("TODO-DESCRIPTION", "Articles on Programming, Test Driven Development, C#, Javascript, ASP.NET MVC and SQL Server, with a bit of maths and science thrown in too. ")
                .Replace("TODO-CANONICALURL", canonicalurl)
                .Replace("TODO-POSTDATE", postdate)
                .Replace("TODO-POSTDATE", postdate)
                .Replace("TODO-NAV", menu)
                .Replace("TODO-CONTENT", content);

            File.WriteAllText(Path.Combine(SiteRootPath, filename), newpost, new UTF8Encoding(false));
        }

        [Test]
        public void MakeLinks()
        {
            var filenames = Directory.GetFiles(SiteRootPath)
                .Where(f => PostNamePattern.IsMatch(Path.GetFileName(f)))
                .Select(f => Path.GetFileName(f))
                .OrderBy(f => f)
                .ToList();

            for (int i = 0; i < filenames.Count; i++)
            {
                var filename = filenames[i];
                var previous = i-1 < 0 ? "index.html" : filenames[i-1];
                var next = i+1 >= filenames.Count ? "index.html" : filenames[i+1];
                string menu = $@"
<menu>
<li><a href=""{previous}"">Previous</a></li>
<li><a href=""{next}"">Next</a></li>
</menu>
";

                var doc = new HtmlDocument();
                doc.Load(Path.Combine(SiteRootPath, filename), new UTF8Encoding(false));
                doc.DocumentNode.SelectSingleNode("//nav").InnerHtml = menu;
                doc.Save(Path.Combine(SiteRootPath, filename), new UTF8Encoding(false));
            }
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

                // seo
                var headNode = doc.DocumentNode.SelectSingleNode("//head/title");
                var headText = headNode?.InnerText ?? "";
                if (headText == "" || headText.StartsWith("TODO"))
                    Console.WriteLine($"{file}({headNode?.Line},{headNode?.LinePosition}): M0010: title not set");

                var descriptionNode = doc.DocumentNode.SelectSingleNode("//head/meta[@name='description']");
                var descriptionText = descriptionNode?.Attributes["content"].Value ?? "";
                if (descriptionText == "" || descriptionText.StartsWith("TODO"))
                    Console.WriteLine($"{file}({descriptionNode?.Line},{descriptionNode?.LinePosition}): M0011: meta description not set");

                var canonicalNode = doc.DocumentNode.SelectSingleNode("//head/link[@rel='canonical']");
                var canonicalText = canonicalNode?.Attributes["href"].Value ?? "";
                if (canonicalText == "" || canonicalText.StartsWith("TODO") || canonicalText.EndsWith(file) == false)
                    Console.WriteLine($"{file}({canonicalNode?.Line},{canonicalNode?.LinePosition}): M0012: canonical url not set, or does not match filename");

                // <img> in a figure, alt and title tags set, src tag to the images folder
                foreach (var img in doc.DocumentNode.SelectNodes("//img"))
                {
                    if(img.ParentNode.Name != "figure" && img.ParentNode.ParentNode.Name != "figure")
                        Console.WriteLine($"{file}({img.Line},{img.LinePosition}): M0001: img not in a figure");

                    var alt = img.Attributes["alt"]?.Value;
                    if (alt == null || string.IsNullOrWhiteSpace(alt) || alt.StartsWith("TODO"))
                        Console.WriteLine($"{file}({img.Line},{img.LinePosition}): M0002: img doens't have a valid alt attribute");

                    var title = img.Attributes["title"]?.Value;
                    if (title == null || string.IsNullOrWhiteSpace(title) || title.StartsWith("TODO"))
                        Console.WriteLine($"{file}({img.Line},{img.LinePosition}): M0003: img doens't have a valid title attribute");

                    var src = img.Attributes["src"]?.Value;
                    if (src == null || src.StartsWith("images/") == false)
                        Console.WriteLine($"{file}({img.Line},{img.LinePosition}): M0004: img doens't have a valid src attribute");

                    if(img.Attributes.Count != 3)
                        Console.WriteLine($"{file}({img.Line},{img.LinePosition}): M0005: img has unwanted attributes");
                }

                // figcaption - nested inside two figures
                foreach (var figcaption in doc.DocumentNode.SelectNodes("//figcaption"))
                {
                    if(figcaption.Ancestors().Count(a => a.Name == "figure") != 2)
                        Console.WriteLine($"{file}({figcaption.Line},{figcaption.LinePosition}): M0005: figcaption needs to be in a figure which is itself nested in a figure");
                }

                // table - in a figure
                foreach (var table in doc.DocumentNode.SelectNodes("//table"))
                {
                    if (table.ParentNode.Name != "figure" && table.ParentNode.ParentNode.Name != "figure")
                        Console.WriteLine($"{file}({table.Line},{table.LinePosition}): M0031: table not in a figure");
                }

                foreach (var ul in doc.DocumentNode.SelectNodes("//ul"))
                {
                    if (ul.ParentNode.Name == "p")
                        Console.WriteLine($"{file}({ul.Line},{ul.LinePosition}): M0032: ul should not be in a p tag");
                }
            }
        }

        [Test, Ignore("")]
        public void ManipulateAllDocs()
        {
            var filenames = Directory.GetFiles(SiteRootPath)
                .Where(f => PostNamePattern.IsMatch(Path.GetFileName(f)))
                .Select(f => Path.GetFileName(f))
                .OrderByDescending(f => f)
                .ToList();

            foreach (var filename in filenames)
            {
                var doc = new HtmlDocument();
                doc.Load(Path.Combine(SiteRootPath, filename), new UTF8Encoding(false));

                var head = doc.DocumentNode.SelectSingleNode("//head");
                var refChild = head.ChildNodes.Last();
                var newChild = HtmlNode.CreateNode("<link href=\"https://fonts.googleapis.com/css?family=Nunito+Sans\" rel =\"stylesheet\">");
                head.InsertAfter(HtmlNode.CreateNode("\r\n"), refChild);
                head.InsertAfter(newChild, refChild);
                head.InsertAfter(doc.CreateTextNode("    "), refChild);

                doc.Save(Path.Combine(SiteRootPath, filename), new UTF8Encoding(false));
            }
        }



        //var head = doc.DocumentNode.SelectSingleNode("//head");
        //var fontlink = doc.DocumentNode.SelectNodes("//head/link").SingleOrDefault(n => n.Attributes["href"]?.Value.StartsWith("https://fonts.googleapis.com") ?? false);
        //head.RemoveChild(fontlink);

    }
}
