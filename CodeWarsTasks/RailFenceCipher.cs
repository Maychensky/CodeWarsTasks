using System.Text;

namespace CodeWarsTasks
{
    public class RailFenceCipher
    {
        public static string Encode(string s, int n)
        {
            Console.WriteLine("Encode s = '{0}', n = '{1}'", s, n);
            return GetEncode(s, n);
        }

        private static string GetEncode(string s, int n)
        {
            List<string> strList = new List<string>();
            StringBuilder strSB = new StringBuilder();
            int maxInterval = (n - 1) * 2;
            int minus = -2;
            int leftIntervalRange = 0;
            int rightIntervalRange = 0;
            for (int startIndexNextLint = 0; startIndexNextLint < n; startIndexNextLint++)
            {
                minus += 2;
                leftIntervalRange = (maxInterval - minus == 0) ? maxInterval : maxInterval - minus;
                rightIntervalRange = (minus == 0 || minus == maxInterval) ? maxInterval : maxInterval - leftIntervalRange;
                var rangeForIndex = (leftIntervalRange, rightIntervalRange);
                strList.Add(GetNextLineEncode(s, strSB, startIndexNextLint, rangeForIndex));
            }
            strSB.Clear();
            foreach (var e in strList)
                strSB.Append(e);
            return strSB.ToString();
        }

        private static string GetNextLineEncode(string word, StringBuilder stringBuilder, int startIndexNextLint, (int, int) rangeForIndex)
        {
            stringBuilder.Clear();
            bool isLeftItem = false;
            for (int i = startIndexNextLint; i < word.Length; i += (isLeftItem) ? rangeForIndex.Item1 : rangeForIndex.Item2)
            {
                stringBuilder.Append(word[i]);
                isLeftItem = !isLeftItem;
            }
            return stringBuilder.ToString();
        }

        public static string Decode(string s, int n)
        {
            Console.WriteLine("Decode s = '{0}', n = '{1}'", s, n);
            return GetDecode(s, n);
        }

        public static string GetDecode(string s, int n)
        {
            List<string> strList = new List<string>();
            List<int> rangeIndex = new List<int>();
            StringBuilder strSB = new StringBuilder();
            int maxInteval = (n - 1) * 2;
            int minus = -2;
            int leftIntervalRange = 0;
            int rightIntervalRange = 0;
            for (int startIndexNextLint = 0; startIndexNextLint < n; startIndexNextLint++)
            {
                minus += 2;
                leftIntervalRange = (maxInteval - minus == 0) ? maxInteval : maxInteval - minus;
                rightIntervalRange = (minus == 0 || minus == maxInteval) ? maxInteval : maxInteval - leftIntervalRange;
                var rangeForIndex = (leftIntervalRange, rightIntervalRange);
                rangeIndex.Add(GetNextLineNumber(s, startIndexNextLint, rangeForIndex));
            }
            return GetSumLinesForDecode(s,strSB, rangeIndex);
        }

        private static int GetNextLineNumber(string word, int startIndexNextLint, (int, int) rangeForIndex)
        {
            var countElementLine = 0;
            bool isLeftItem = false;
            for (int i = startIndexNextLint; i < word.Length; i += (isLeftItem) ? rangeForIndex.Item1 : rangeForIndex.Item2)
            {
                countElementLine++;
                isLeftItem = !isLeftItem;
            }
            return countElementLine;
        }

        private static string GetSumLinesForDecode(string s, StringBuilder stringBuilder, List<int> sizeIntevals)
        {
            List<int> startIndexIntervals = new List<int>();
            List<(int, int)> indexIntervals = new List<(int, int)>();
            for (int i = 0; i < sizeIntevals.Count + 1; i++)
                 startIndexIntervals.Add((startIndexIntervals.Count == 0) ? 0 : startIndexIntervals.Last() + sizeIntevals[i - 1]);
            for (int i = 0; i < sizeIntevals.Count; i++)
                indexIntervals.Add((startIndexIntervals[i], startIndexIntervals[i + 1] - 1));
            return GetResultLine(s, stringBuilder, indexIntervals);
        }

        private static string GetResultLine(string s, StringBuilder stringBuilder, List<(int, int)> indexValues)
        {
            int currentIndex = -1;
            bool directionIndexLeft = true;
            int countFalse = 0;
            while (true)
            {
                if (currentIndex == 0) directionIndexLeft = true;
                if (currentIndex == indexValues.Count - 1) directionIndexLeft = false;
                currentIndex += (directionIndexLeft) ? 1 : -1;
                var nextValue = GetNextValue(s, indexValues[currentIndex]);
                if (nextValue.Item1)
                {
                    indexValues[currentIndex] = (indexValues[currentIndex].Item1 + 1, indexValues[currentIndex].Item2);
                    countFalse = 0;
                    stringBuilder.Append(nextValue.Item2);
                }
                else countFalse++;
                if (countFalse == indexValues.Count) break;
            }
            return stringBuilder.ToString();
        }

        private static (bool, char) GetNextValue(string s, (int, int) value)
        {
            if (value.Item1 > value.Item2) return (false, ' ');
            else return (true, s[value.Item1]);
        }
    }

}
