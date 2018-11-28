using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;

namespace siteadmin
{
    [TestFixture]
    public class GenerateRedirects
    {
        public string SiteRootPath { get; } = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\matroberts.github.io");
        public string TemplatePath { get; } = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\RedirectTemplate.html");

        List<string> ExtraLinks = new List<string>
        {
            "http://moleseyhill.com/code/Pomodoro/Timer.htm",
            "http://moleseyhill.com/code/RsVsShrtCt/Resharper-VisualStudio-Shortcuts.html",
            "http://moleseyhill.com/code/Typing/Lesson.htm",
        };

        public Dictionary<string, string> Redirects = new Dictionary<string, string>
            {
                {"http://moleseyhill.com/blog/"                                                                  , "index.html" },

                {"http://moleseyhill.com/blog/2009/02/21/struggling-for-competence/"                             , "2009-02-21-struggling-for-competence.html" },
                {"http://moleseyhill.com/blog/2009/03/03/source-control-build-test/"                             , "2009-03-03-source-control-build-test.html" },
                {"http://moleseyhill.com/blog/2009/03/09/create-alter-table/"                                    , "2009-03-09-create-alter-table.html" },
                {"http://moleseyhill.com/blog/2009/03/16/csharp-custom-attributes/"                              , "2009-03-16-csharp-custom-attributes.html" },
                {"http://moleseyhill.com/blog/2009/03/24/aspnet-session-state/"                                  , "2009-03-24-aspnet-session-state.html" },
                {"http://moleseyhill.com/blog/2009/03/29/picks-theorem/"                                         , "2009-03-29-picks-theorem.html" },
                {"http://moleseyhill.com/blog/2009/04/06/code-kata-atari/"                                       , "2009-04-06-code-kata-atari.html" },
                {"http://moleseyhill.com/blog/2009/04/13/csharp-ienumerable-yield/"                              , "2009-04-13-csharp-ienumerable-yield.html" },
                {"http://moleseyhill.com/blog/2009/04/20/csharp-events/"                                         , "2009-04-20-csharp-events.html" },
                {"http://moleseyhill.com/blog/2009/04/27/first-steps-javascript-jquery/"                         , "2009-04-27-first-steps-javascript-jquery.html" },
                {"http://moleseyhill.com/blog/2009/05/06/css-selectors/"                                         , "2009-05-06-css-selectors.html" },
                {"http://moleseyhill.com/blog/2009/05/11/css-enlightenment/"                                     , "2009-05-11-css-enlightenment.html" },
                {"http://moleseyhill.com/blog/2009/05/18/kent-becks-four-hats/"                                  , "2009-05-18-kent-becks-four-hats.html" },
                {"http://moleseyhill.com/blog/2009/05/25/how-many-bugs/"                                         , "2009-05-25-how-many-bugs.html" },
                {"http://moleseyhill.com/blog/2009/06/02/http-basics/"                                           , "2009-06-02-http-basics.html" },
                {"http://moleseyhill.com/blog/2009/06/08/test-msbuild-from-mstest/"                              , "2009-06-08-test-msbuild-from-mstest.html" },
                {"http://moleseyhill.com/blog/2009/06/15/how-far-is-the-horizon/"                                , "2009-06-15-how-far-is-the-horizon.html" },
                {"http://moleseyhill.com/blog/2009/06/22/how-to-stop-fogging/"                                   , "2009-06-22-how-to-stop-fogging.html" },
                {"http://moleseyhill.com/blog/2009/06/29/simple-repository-pattern/"                             , "2009-06-29-simple-repository-pattern.html" },
                {"http://moleseyhill.com/blog/2009/07/06/unit-testing-with-repository-pattern/"                  , "2009-07-06-unit-testing-with-repository-pattern.html" },
                {"http://moleseyhill.com/blog/2009/07/13/active-record-verses-repository/"                       , "2009-07-13-active-record-verses-repository.html" },
                {"http://moleseyhill.com/blog/2009/07/20/unit-testing-with-email/"                               , "2009-07-20-unit-testing-with-email.html" },
                {"http://moleseyhill.com/blog/2009/07/27/the-evolving-definition-of-unit-testing/"               , "2009-07-27-the-evolving-definition-of-unit-testing.html" },
                {"http://moleseyhill.com/blog/2009/08/04/castell-de-montgri/"                                    , "2009-08-04-castell-de-montgri.html" },
                {"http://moleseyhill.com/blog/2009/08/11/dependency-inversion-repository-pattern/"               , "2009-08-11-dependency-inversion-repository-pattern.html" },
                {"http://moleseyhill.com/blog/2009/08/19/how-safe-is-flying/"                                    , "2009-08-19-how-safe-is-flying.html" },
                {"http://moleseyhill.com/blog/2009/08/27/dreyfus-model/"                                         , "2009-08-27-dreyfus-model.html" },
                {"http://moleseyhill.com/blog/2009/09/01/sql-server-bits-and-bats/"                              , "2009-09-01-sql-server-bits-and-bats.html" },
                {"http://moleseyhill.com/blog/2009/09/07/xp-engineering-practices/"                              , "2009-09-07-xp-engineering-practices.html" },
                {"http://moleseyhill.com/blog/2009/09/14/extreme-quotes/"                                        , "2009-09-14-extreme-quotes.html" },
                {"http://moleseyhill.com/blog/2009/09/22/bayesian-probability-of-success/"                       , "2009-09-22-bayesian-probability-of-success.html" },
                {"http://moleseyhill.com/blog/2009/09/28/css-block-verses-inline/"                               , "2009-09-28-css-block-verses-inline.html" },
                {"http://moleseyhill.com/blog/2009/10/07/css-simple-form/"                                       , "2009-10-07-css-simple-form.html" },
                {"http://moleseyhill.com/blog/2009/10/18/xhtml-whats-that-all-about/"                            , "2009-10-18-xhtml-whats-that-all-about.html" },
                {"http://moleseyhill.com/blog/2009/10/24/whats-the-chance-of-winning-the-national-lottery/"      , "2009-10-24-whats-the-chance-of-winning-the-national-lottery.html" },
                {"http://moleseyhill.com/blog/2009/10/27/browser-differences/"                                   , "2009-10-27-browser-differences.html" },
                {"http://moleseyhill.com/blog/2009/11/03/chance-of-winning-any-prize-on-the-national-lottery/"   , "2009-11-03-chance-of-winning-any-prize-on-the-national-lottery.html" },
                {"http://moleseyhill.com/blog/2009/11/13/guesstimation/"                                         , "2009-11-13-guesstimation.html" },
                {"http://moleseyhill.com/blog/2009/11/17/first-steps-with-fluent-nhibernate/"                    , "2009-11-17-first-steps-with-fluent-nhibernate.html" },
                {"http://moleseyhill.com/blog/2009/11/23/optional-filtering-in-stored-procedures/"               , "2009-11-23-optional-filtering-in-stored-procedures.html" },
                {"http://moleseyhill.com/blog/2009/12/02/fluent-nhibernate-unit-of-work-pattern/"                , "2009-12-02-fluent-nhibernate-unit-of-work-pattern.html" },
                {"http://moleseyhill.com/blog/2009/12/11/test-data-setup-with-fluent-builder-pattern/"           , "2009-12-11-test-data-setup-with-fluent-builder-pattern.html" },
                {"http://moleseyhill.com/blog/2009/12/18/pimp-my-visual-studio/"                                 , "2009-12-18-pimp-my-visual-studio.html" },
                {"http://moleseyhill.com/blog/2010/01/11/engine-efficiency/"                                     , "2010-01-11-engine-efficiency.html" },
                {"http://moleseyhill.com/blog/2010/01/24/css-shorthand-properties/"                              , "2010-01-24-css-shorthand-properties.html" },
                {"http://moleseyhill.com/blog/2010/01/31/unit-test-msbuild-custom-task/"                         , "2010-01-31-unit-test-msbuild-custom-task.html" },
                {"http://moleseyhill.com/blog/2010/02/08/prime-numbers/"                                         , "2010-02-08-prime-numbers.html" },
                {"http://moleseyhill.com/blog/2010/02/14/unit-test-logging-on-a-custom-msbuild-task/"            , "2010-02-14-unit-test-logging-on-a-custom-msbuild-task.html" },
                {"http://moleseyhill.com/blog/2010/02/22/javascript-objects/"                                    , "2010-02-22-javascript-objects.html" },
                {"http://moleseyhill.com/blog/2010/03/01/this-javascript/"                                       , "2010-03-01-this-javascript.html" },
                {"http://moleseyhill.com/blog/2010/03/07/simple-logic/"                                          , "2010-03-07-simple-logic.html" },
                {"http://moleseyhill.com/blog/2010/03/15/the-state-of-the-stack/"                                , "2010-03-15-the-state-of-the-stack.html" },
                {"http://moleseyhill.com/blog/2010/03/22/hardy-littlewood-rules/"                                , "2010-03-22-hardy-littlewood-rules.html" },
                {"http://moleseyhill.com/blog/2010/03/31/learning-scheme/"                                       , "2010-03-31-learning-scheme.html" },
                {"http://moleseyhill.com/blog/2010/04/13/prefix-infix-and-postfix/"                              , "2010-04-13-prefix-infix-and-postfix.html" },
                {"http://moleseyhill.com/blog/2010/04/21/functions-as-first-class-objects/"                      , "2010-04-21-functions-as-first-class-objects.html" },
                {"http://moleseyhill.com/blog/2010/05/05/backus-naur-form/"                                      , "2010-05-05-backus-naur-form.html" },
                {"http://moleseyhill.com/blog/2010/05/14/simple-recursion/"                                      , "2010-05-14-simple-recursion.html" },
                {"http://moleseyhill.com/blog/2010/08/11/oxford-cambridge-evidence-controversy/"                 , "2010-08-11-oxford-cambridge-evidence-controversy.html" },
                {"http://moleseyhill.com/blog/2010/08/22/the-englishano/"                                        , "2010-08-22-the-englishano.html" },
                {"http://moleseyhill.com/blog/2010/08/29/randomizing-output/"                                    , "2010-08-29-randomizing-output.html" },
                {"http://moleseyhill.com/blog/2010/08/30/hot-numbers/"                                           , "2010-08-30-hot-numbers.html" },
                {"http://moleseyhill.com/blog/2011/10/12/mass-delete-all-wordpress-comments/"                    , "2011-10-12-mass-delete-all-wordpress-comments.html" },
                {"http://moleseyhill.com/blog/2011/10/18/checklist/"                                             , "2011-10-18-checklist.html" },
                {"http://moleseyhill.com/blog/2011/10/20/why-you-are-wrong/"                                     , "2011-10-20-why-you-are-wrong.html" },
                {"http://moleseyhill.com/blog/2011/10/31/unemployment/"                                          , "2011-10-31-unemployment.html" },
                {"http://moleseyhill.com/blog/2011/11/03/bargain-hunting/"                                       , "2011-11-03-bargain-hunting.html" },
                {"http://moleseyhill.com/blog/2011/12/05/lines-of-code/"                                         , "2011-12-05-lines-of-code.html" },
                {"http://moleseyhill.com/blog/2012/05/29/mstest-original-file-location/"                         , "2012-05-29-mstest-original-file-location.html" },
                {"http://moleseyhill.com/blog/2012/05/30/maximum-contiguous-sum/"                                , "2012-05-30-maximum-contiguous-sum.html" },

                {"http://moleseyhill.com/blog/quotes/"                                                           , "index.html" },
                {"http://moleseyhill.com/blog/the-stack/"                                                        , "2010-03-15-the-state-of-the-stack.html" },
                {"http://moleseyhill.com/blog/books/"                                                            , "index.html" },
                {"http://moleseyhill.com/blog/credits/"                                                          , "index.html" },
                {"http://moleseyhill.com/blog/about/"                                                            , "index.html" },
                {"http://moleseyhill.com/blog/archives/"                                                         , "index.html" },

                // Only tag based link i could find
                {"http://moleseyhill.com/blog/tag/create-login/"                                                 , "2009-06-08-test-msbuild-from-mstest.html" }
            };

        [Test]
        public void Generate()
        {
            var template = File.ReadAllText(TemplatePath);

            foreach (var oldUrl in Redirects.Keys)
            {
                // generate folders for each of the part of the path of the old url
                var urlpath = oldUrl.Substring("http://moleseyhill.com/".Length).TrimEnd('/').Split('/');
                var folder = Path.Combine(SiteRootPath, Path.Combine(urlpath));
                Directory.CreateDirectory(folder);

                // fill in the template with the redirect
                var redirect = template.Replace("TODO-REDIRECT", Redirects[oldUrl]);

                // write the redirect into the index file for the folder
                var filename = Path.Combine(folder, "index.html");
                File.WriteAllText(filename, redirect, new UTF8Encoding(false));
            }
        }



        [Test]
        public void MakeTestLinks()
        {
            var allUrls = Redirects.Keys.Concat(ExtraLinks);
            foreach (var testurl in allUrls.Select(oldUrl => "https://matroberts.github.io/" + oldUrl.Substring("http://moleseyhill.com/".Length)))
            {
                Console.WriteLine(testurl);
            }
            
        }
    }



}