namespace CodeWarsTasks
{
    public class BattleshipField
    {
        private const int TRUE_NUMBERS_CELLS_SHIPS = 20;
        private const int SIZE_FIELD = 10;
        private static readonly Dictionary<int, int> _trueCountsShips;
        private static readonly (int, int)[] _shiftCellForVerticalAndHorizontal;
        private static readonly (int, int)[] _shiftCellForHorizontal;
        private static readonly (int, int)[] _shiftCellForVertical;
        private static readonly (int, int)[] _shiftCellForDiagonal;
        private static Dictionary<int, int> _countsShips;
        private static int[,] _field;
        private static bool[,] _checkField;
        private static int _fieldSizeX;
        private static int _fieldSizeY;

        static BattleshipField()
        {
            _field = new int[,] { };
            _checkField = new bool[SIZE_FIELD, SIZE_FIELD];
            _shiftCellForHorizontal = new (int, int)[] { (1, 0), (-1, 0) };
            _shiftCellForVertical = new (int, int)[] { (0, 1), (0, -1) };
            _shiftCellForVerticalAndHorizontal = new (int, int)[] { (0, 1), (0, -1), (1, 0), (-1, 0) };
            _shiftCellForDiagonal = new (int, int)[] { (1, 1), (-1, -1), (1, -1), (-1, 1) };
            _trueCountsShips = new Dictionary<int, int>() { { 4, 1 }, { 3, 2 }, { 2, 3 }, { 1, 4 } };
            _countsShips = new Dictionary<int, int>();
        }

        public static bool ValidateBattlefield(int[,] field)
        {
            _field = field;
            _fieldSizeX = field.GetLength(0);
            _fieldSizeY = field.GetLength(1);
            Reset();
            if (!CheckCorrectPositionNeighbors()) return false;
            if (!CheckCorrectSizeShips()) return false;
            return true;
        }

        private static bool CheckCorrectSizeShips()
        {
            for (int y = 0; y < _fieldSizeY; y++)
                for (int x = 0; x < _fieldSizeX; x++)
                    if (_field[x, y] == 1 && !_checkField[x, y])
                    {
                        int sizeShip = GetSizeShip(x, y);
                        AddNewShip(sizeShip);
                    }
            return CheckCountsShips();
        }

        private static bool CheckCountsShips() => _trueCountsShips.EqualsWithValue(_countsShips);

        private static void AddNewShip(int sizeShip)
        {
            if (_countsShips.ContainsKey(sizeShip)) _countsShips[sizeShip]++;
            else _countsShips.Add(sizeShip, 1);
        }

        private static void Reset()
        {
            _countsShips.Clear();
            _checkField = new bool[SIZE_FIELD, SIZE_FIELD];
        }   

        private static int GetSizeShip(int x, int y)
        {
            int sizeShip = 0; 
            SearchCellShipRecurs(x, y, ref sizeShip);
            return sizeShip;
        }

        private static void SearchCellShipRecurs(int x, int y, ref int countCellShip)
        {
            if (CheckCellInField(x, y) && _field[x, y] == 1 && !_checkField[x, y]) 
            {
                _checkField[x, y] = true;
                countCellShip++;
                foreach (var direction in _shiftCellForVerticalAndHorizontal)
                    SearchCellShipRecurs(x + direction.Item1, y + direction.Item2, ref countCellShip);
            }
        }

        private static bool CheckCorrectPositionNeighbors()
        {
            int countShipCells = 0;
            for (int x = 0; x < _fieldSizeX; x++)
                for (int y = 0; y < _fieldSizeY; y++)
                    if (_field[x, y] == 1 )
                    {
                        countShipCells++;
                        if (CheckNotCorrectNeighbors(x, y)) 
                            return false;
                    }
            if (countShipCells != TRUE_NUMBERS_CELLS_SHIPS) return false;
            return true;
        }

        private static bool CheckNotCorrectNeighbors(int x, int y)
        {
            var thereShipsDiagonal = CheckForShip(x, y, _shiftCellForDiagonal);
            if (thereShipsDiagonal) return true;
            var thereShipsVertical = CheckForShip(x, y, _shiftCellForVertical);
            var thereShipsHorizontal = CheckForShip(x, y, _shiftCellForHorizontal);
            if (thereShipsVertical && thereShipsHorizontal) return true;
            return false;
        }

        private static bool CheckForShip(int x, int y, (int, int)[] directionNeighbors)
        {
            int nextX = 0;
            int nextY = 0; 
            foreach (var direction in directionNeighbors)
            {
                nextX = x + direction.Item1;
                nextY = y + direction.Item2;
                if (CheckCellInField(nextX, nextY) && _field[nextX, nextY] == 1)
                    return true;
            }
            return false;
        }

        private static bool CheckCellInField(int x , int y) => (x >= 0 && y >= 0 && x < _fieldSizeX && y < _fieldSizeY);
    }

    public static class ExpansionDictionary
    {
        public static bool EqualsWithValue<TKey, TValue>(this Dictionary<TKey, TValue> currentDictionary, Dictionary<TKey, TValue> otherDictionary)
        {
            if (currentDictionary.Count != otherDictionary.Count) return false;
            foreach (var elementCurrentDictionary in currentDictionary)
                if (!(otherDictionary.ContainsKey(elementCurrentDictionary.Key) && otherDictionary[elementCurrentDictionary.Key].Equals(elementCurrentDictionary.Value)))
                    return false;
            return true;
        }
    }
}
