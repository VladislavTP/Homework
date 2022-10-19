using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DictionaryCreator
{
    public class DicCreator
    {

        private Dictionary<string, int> CreateDic(string text)
        {
            string[] correctedWords = CorrectPrimaryText(text);
            return GetSortedWordCountDic(correctedWords);
        }

        private string[] CorrectPrimaryText(string text)
        {
            text.ToLower();
            text = Regex.Replace(text, "[\\p{P}[0-9\\]]", "");
            var splittedWords = text.Split(' ', '\n', '\t');
            return splittedWords;
        }

        private Dictionary<string, int> GetSortedWordCountDic(string[] allWords)
        {
            var uniqueWords = allWords.Distinct();
            Dictionary<string, int> wordCountDic = new Dictionary<string, int>();
            foreach (string word in uniqueWords)
            {
                wordCountDic.Add(word, allWords.Count(w => w.Equals(word)));
            }

            return wordCountDic.OrderByDescending(w => w.Value).ToDictionary(k => k.Key, v => v.Value);
        }

        public Dictionary<string, int> ParallelCreateDic(string text)
        {
            string[] correctedWords = CorrectPrimaryText(text);

            ConcurrentDictionary<string, int> concWordDictionary = new ConcurrentDictionary<string, int>();
            try
            {
                Parallel.ForEach(correctedWords, w =>
                {
                    concWordDictionary.AddOrUpdate(w, 1, (k, oldV) => oldV + 1);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            Dictionary<string, int> wordCountDic = new Dictionary<string, int>(concWordDictionary);
            return wordCountDic.OrderByDescending(w => w.Value).ToDictionary(k => k.Key, v => v.Value);

        }

    }
}
