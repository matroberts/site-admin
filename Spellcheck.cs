using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using NHunspell;
using NUnit.Framework;

namespace siteadmin
{
    public class Spellcheck : IDisposable
    {
        private Hunspell Hunspell = null;
        private char[] Punctuation = new char[] { ',', '.', ';', '?', '-', '(', ')', '"', ':', '/', '[', ']' };

        public Spellcheck(string dictionaryPath)
        {
            var affix = Path.Combine(dictionaryPath, "en-GB.aff");
            var dict = Path.Combine(dictionaryPath, "en-GB.dic");
            var specials = Path.Combine(dictionaryPath, "specials.txt");

            Hunspell = new Hunspell(affix, dict);
            foreach (var line in File.ReadAllLines(specials))
            {
                Hunspell.Add(line);
            }
        }
        public List<string> Spell(string words)
        {
            var mistakes = new List<string>();
            foreach (var word in words.Replace("&nbsp;", " ").Replace("&gt;", " ").Replace("...", " ").Split().Select(word => word.Trim(Punctuation)))
            {
                if (Hunspell.Spell(word) == false)
                {
                    mistakes.Add(word);
                }
            }

            return mistakes;
        }
        public void Dispose()
        {
            Hunspell?.Dispose();
            Hunspell = null;
        }
    }
}