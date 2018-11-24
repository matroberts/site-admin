using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace siteadmin
{
    [TestFixture]
    public class Redirects
    {
        public string SiteRootPath { get; } = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\matroberts.github.io");
        public string TemplatePath { get; } = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\RedirectTemplate.html");



        [Test]
        public void Generate()
        {
            var redirects = new Dictionary<string, string>
            {
                {"http://moleseyhill.com/blog/"                                                                  , "" },
                {"http://moleseyhill.com/blog/2009/11/17/first-steps-with-fluent-nhibernate/"                    , "" },
                {"http://moleseyhill.com/blog/2009/12/02/fluent-nhibernate-unit-of-work-pattern/"                , "" },
                {"http://moleseyhill.com/blog/2009/12/11/test-data-setup-with-fluent-builder-pattern/"           , "" },
                {"http://moleseyhill.com/blog/2010/01/31/unit-test-msbuild-custom-task/"                         , "" },
                {"http://moleseyhill.com/blog/2010/02/14/unit-test-logging-on-a-custom-msbuild-task/"            , "" },
                {"http://moleseyhill.com/blog/2012/05/29/mstest-original-file-location/"                         , "" },
                {"http://moleseyhill.com/blog/2012/05/30/maximum-contiguous-sum/"                                , "" },
                {"http://moleseyhill.com/blog/2011/12/05/lines-of-code/"                                         , "" },
                {"http://moleseyhill.com/blog/2011/11/03/bargain-hunting/"                                       , "" },
                {"http://moleseyhill.com/blog/2011/10/31/unemployment/"                                          , "" },
                {"http://moleseyhill.com/blog/2011/10/20/why-you-are-wrong/"                                     , "" },
                {"http://moleseyhill.com/blog/2011/10/18/checklist/"                                             , "" },
                {"http://moleseyhill.com/blog/2009/09/28/css-block-verses-inline/"                               , "" },
                {"http://moleseyhill.com/blog/2009/02/21/struggling-for-competence/"                             , "" },
                {"http://moleseyhill.com/blog/2009/03/03/source-control-build-test/"                             , "" },
                {"http://moleseyhill.com/blog/2009/03/09/create-alter-table/"                                    , "" },
                {"http://moleseyhill.com/blog/2009/03/16/csharp-custom-attributes/"                              , "" },
                {"http://moleseyhill.com/blog/2009/03/24/aspnet-session-state/"                                  , "" },
                {"http://moleseyhill.com/blog/2009/03/29/picks-theorem/"                                         , "" },
                {"http://moleseyhill.com/blog/2009/04/06/code-kata-atari/"                                       , "" },
                {"http://moleseyhill.com/blog/2009/04/13/csharp-ienumerable-yield/"                              , "" },
                {"http://moleseyhill.com/blog/2009/04/20/csharp-events/"                                         , "" },
                {"http://moleseyhill.com/blog/2009/04/27/first-steps-javascript-jquery/"                         , "" },
                {"http://moleseyhill.com/blog/2009/05/06/css-selectors/"                                         , "" },
                {"http://moleseyhill.com/blog/2009/05/11/css-enlightenment/"                                     , "" },
                {"http://moleseyhill.com/blog/2009/05/18/kent-becks-four-hats/"                                  , "" },
                {"http://moleseyhill.com/blog/2009/05/25/how-many-bugs/"                                         , "" },
                {"http://moleseyhill.com/blog/2009/06/02/http-basics/"                                           , "" },
                {"http://moleseyhill.com/blog/2009/06/08/test-msbuild-from-mstest/"                              , "" },
                {"http://moleseyhill.com/blog/2009/06/15/how-far-is-the-horizon/"                                , "" },
                {"http://moleseyhill.com/blog/2009/06/22/how-to-stop-fogging/"                                   , "" },
                {"http://moleseyhill.com/blog/2009/06/29/simple-repository-pattern/"                             , "" },
                {"http://moleseyhill.com/blog/2009/07/06/unit-testing-with-repository-pattern/"                  , "" },
                {"http://moleseyhill.com/blog/2009/07/13/active-record-verses-repository/"                       , "" },
                {"http://moleseyhill.com/blog/2009/07/20/unit-testing-with-email/"                               , "" },
                {"http://moleseyhill.com/blog/2009/07/27/the-evolving-definition-of-unit-testing/"               , "" },
                {"http://moleseyhill.com/blog/2009/08/04/castell-de-montgri/"                                    , "" },
                {"http://moleseyhill.com/blog/2009/08/11/dependency-inversion-repository-pattern/"               , "" },
                {"http://moleseyhill.com/blog/2009/08/19/how-safe-is-flying/"                                    , "" },
                {"http://moleseyhill.com/blog/2009/08/27/dreyfus-model/"                                         , "" },
                {"http://moleseyhill.com/blog/2009/09/01/sql-server-bits-and-bats/"                              , "" },
                {"http://moleseyhill.com/blog/2009/09/07/xp-engineering-practices/"                              , "" },
                {"http://moleseyhill.com/blog/2009/09/14/extreme-quotes/"                                        , "" },
                {"http://moleseyhill.com/blog/2009/09/22/bayesian-probability-of-success/"                       , "" },
                {"http://moleseyhill.com/blog/2009/10/07/css-simple-form/"                                       , "" },
                {"http://moleseyhill.com/blog/2009/10/18/xhtml-whats-that-all-about/"                            , "" },
                {"http://moleseyhill.com/blog/2009/10/24/whats-the-chance-of-winning-the-national-lottery/"      , "" },
                {"http://moleseyhill.com/blog/2009/10/27/browser-differences/"                                   , "" },
                {"http://moleseyhill.com/blog/2009/11/03/chance-of-winning-any-prize-on-the-national-lottery/"   , "" },
                {"http://moleseyhill.com/blog/2009/11/13/guesstimation/"                                         , "" },
                {"http://moleseyhill.com/blog/2009/11/23/optional-filtering-in-stored-procedures/"               , "" },
                {"http://moleseyhill.com/blog/2009/12/18/pimp-my-visual-studio/"                                 , "" },
                {"http://moleseyhill.com/blog/2010/01/11/engine-efficiency/"                                     , "" },
                {"http://moleseyhill.com/blog/2010/01/24/css-shorthand-properties/"                              , "" },
                {"http://moleseyhill.com/blog/2010/02/08/prime-numbers/"                                         , "" },
                {"http://moleseyhill.com/blog/2010/02/22/javascript-objects/"                                    , "" },
                {"http://moleseyhill.com/blog/2010/03/01/this-javascript/"                                       , "" },
                {"http://moleseyhill.com/blog/2010/03/07/simple-logic/"                                          , "" },
                {"http://moleseyhill.com/blog/2010/03/15/the-state-of-the-stack/"                                , "" },
                {"http://moleseyhill.com/blog/2010/03/22/hardy-littlewood-rules/"                                , "" },
                {"http://moleseyhill.com/blog/2010/03/31/learning-scheme/"                                       , "" },
                {"http://moleseyhill.com/blog/2010/04/13/prefix-infix-and-postfix/"                              , "" },
                {"http://moleseyhill.com/blog/2010/04/21/functions-as-first-class-objects/"                      , "" },
                {"http://moleseyhill.com/blog/2010/05/05/backus-naur-form/"                                      , "" },
                {"http://moleseyhill.com/blog/2010/05/14/simple-recursion/"                                      , "" },
                {"http://moleseyhill.com/blog/2010/08/11/oxford-cambridge-evidence-controversy/"                 , "" },
                {"http://moleseyhill.com/blog/2010/08/22/the-englishano/"                                        , "" },
                {"http://moleseyhill.com/blog/2010/08/30/hot-numbers/"                                           , "" },
                {"http://moleseyhill.com/blog/2010/08/29/randomizing-output/"                                    , "" },
                {"http://moleseyhill.com/blog/2011/10/12/mass-delete-all-wordpress-comments/"                    , "" },
                {"http://moleseyhill.com/blog/quotes/"                                                           , "" },
                {"http://moleseyhill.com/blog/the-stack/"                                                        , "" },
                {"http://moleseyhill.com/blog/books/"                                                            , "" },
                {"http://moleseyhill.com/blog/credits/"                                                          , "" },
                {"http://moleseyhill.com/blog/about/"                                                            , "" },
                {"http://moleseyhill.com/blog/archives/"                                                         , "" },

            };
        }
    }



}