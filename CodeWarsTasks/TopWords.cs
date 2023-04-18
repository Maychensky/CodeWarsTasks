using System.ComponentModel;
using System.Diagnostics.SymbolStore;
using System.Dynamic;
using System.Linq;

namespace CodeWarsTasks
{
    // kata https://www.codewars.com/kata/51e056fe544cf36c410000fb/train/csharp
    public class TopWords
    {
        public static List<string> Top3(string s)
        {
            s = s.ToLower();
            Console.Write(">>>>");
            Console.Write(s);
            Console.WriteLine("<<<<");
            Dictionary<string, int> countWords = new Dictionary<string, int>();
            CountNumberRepetitionsWords(ref countWords, s);
            List<string> topWords = new List<string>();
            int countTopWord = (countWords.Count < 3) ? countWords.Count : 3;
            GetTopMostRepetitiveWord(ref topWords, countWords, countTopWord);
            return topWords;
        }

        private static void GetTopMostRepetitiveWord(ref List<string> topWords, Dictionary<string, int> countWords, int countTopWords = 3)
        {
            if (countWords.Count == 0) return;
            string mostRepeatedWord = " ";
            for (int i = 0; i < countTopWords; i ++)
            {
                foreach (var currentWord in countWords.Keys)
                    if (!topWords.Contains(currentWord)) mostRepeatedWord = currentWord;
                foreach (var currentWord in countWords)
                    if (currentWord.Value > countWords[mostRepeatedWord] && !topWords.Contains(currentWord.Key)) mostRepeatedWord = currentWord.Key;
                topWords.Add(mostRepeatedWord);
            }
        }

        private static void CountNumberRepetitionsWords(ref Dictionary<string, int> countWords, string s)
        {
            char currentLetter = ' ';
            int startIndexWord = 0;
            int endIndexWord = 0;
            bool startedNewWord = false;
            for (int i = 0; i < s.Length; i++)
            {
                currentLetter = s[i];
                if (char.IsLetter(currentLetter) || currentLetter.Equals('\''))
                {
                    if (!startedNewWord) startIndexWord = i;
                    startedNewWord = true;
                }
                else
                {
                    endIndexWord = i - 1;
                    if (startedNewWord) AddNewWord(ref countWords, s.Substring(startIndexWord, endIndexWord - startIndexWord + 1));
                    startedNewWord = false;
                }
            }
        }

        private static void AddNewWord(ref Dictionary<string, int> countWords, string newWord)
        {
            if (countWords.ContainsKey(newWord)) countWords[newWord]++;
            else countWords.Add(newWord, 1);
        }
    }
}
