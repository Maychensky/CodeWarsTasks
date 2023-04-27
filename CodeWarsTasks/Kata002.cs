namespace CodeWarsTasks
{
    internal class Kata002
    {
        private readonly static Dictionary<char, Dictionary<char, char>> _directionLetter;
        private readonly static HashSet<char> _letters;
        private static Dictionary<char, bool> _useLetterCheck;
        private static int _countPatternsFrom;
        static Kata002 ()
        {
            _letters = new HashSet<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', };
            _directionLetter = new Dictionary<char, Dictionary<char,char>>
            {
                { 'A', new Dictionary<char, char> { {'C', 'B'},{'I','E'},{ 'G','D'} } },
                { 'B', new Dictionary<char, char> { {'H', 'E'} } },
                { 'C', new Dictionary<char, char> { {'A', 'B'},{'G','E'},{ 'I','F'} } },
                { 'D', new Dictionary<char, char> { {'F', 'E'} } },
                { 'E', new Dictionary<char, char> { } },
                { 'F', new Dictionary<char, char> { {'D', 'E'} } },
                { 'G', new Dictionary<char, char> { {'A', 'D'},{'C','E'},{ 'I','H'} } },
                { 'H', new Dictionary<char, char> { {'B', 'E'} } },
                { 'I', new Dictionary<char, char> { {'G', 'H'},{'A','E'},{ 'C','F'} } },
            };
            _useLetterCheck = new Dictionary<char, bool>
            {
                { 'A', false },
                { 'B', false },
                { 'C', false },
                { 'D', false },
                { 'E', false },
                { 'F', false },
                { 'G', false },
                { 'H', false },
                { 'I', false },
            };
        }

        public static int CountPatternsFrom(char firstDot, int length)
        {
            ResetParams();
            RecursiveSearchMoves(firstDot, length, 0);
            return _countPatternsFrom;
        }

        private static void ResetParams()
        {
            foreach (var letter in _letters)
                _useLetterCheck[letter] = false;
            _countPatternsFrom = 0;
        }

        private static void RecursiveSearchMoves(char nextLetter, int length, int lengthPathTraveled)
        {
            lengthPathTraveled++;
            if (lengthPathTraveled == length)
            {
                _countPatternsFrom++;
                return;
            }
            _useLetterCheck[nextLetter] = true; 
            foreach (var letter in _letters)
                if (_useLetterCheck[letter] == false && IsLetterAvailable(nextLetter,  letter)) 
                    RecursiveSearchMoves(letter, length, lengthPathTraveled);
            _useLetterCheck[nextLetter] = false; 
        }

        private static bool IsLetterAvailable(char currentLetter, char nextLetter)
        {
            if (_directionLetter[currentLetter].ContainsKey(nextLetter))
                if (_useLetterCheck[_directionLetter[currentLetter][nextLetter]] == false)
                    return false;
            return true;    
        }
    }
}
