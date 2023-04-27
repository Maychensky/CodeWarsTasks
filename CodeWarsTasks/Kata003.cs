namespace CodeWarsTasks
{
    public class Node
    {
        public Node Left;
        public Node Right;
        public int Value;

        public Node(Node l, Node r, int v)
        {
            Left = l;
            Right = r;
            Value = v;
        }
    }

    internal class Kata003
    {
        private static List<List<int>> _sortLevelsNodes;
        public static List<int> TreeByLevels(Node node)
        {
            ResetParams();
            RecursivePassageInDepthLeft(node, 0);
            return JoinLevelsNodes();
        }

        private static List<int> JoinLevelsNodes()
        {
            var countJoinList = 0;
            foreach (var listLevel in _sortLevelsNodes)
                countJoinList += listLevel.Count;
            var result = new List<int>(countJoinList);
            foreach (var listLevel in _sortLevelsNodes)
                foreach (var elementLevel in listLevel)
                    result.Add(elementLevel);
            return result;
        }

        private static void RecursivePassageInDepthLeft(Node node, int level)
        {
            if (node == null) return;
            AddNodeToSortList(node.Value, level);
            RecursivePassageInDepthLeft(node.Left, ++level);
            RecursivePassageInDepthLeft(node.Right, level);
        }

        private static void AddNodeToSortList(int valueNode, int levelInTree, int numberOfLine = 0)
        {
            while (_sortLevelsNodes.Count <= levelInTree)
                _sortLevelsNodes.Add(new List<int>());
            _sortLevelsNodes[levelInTree].Add(valueNode);
        }

        private static void ResetParams() => _sortLevelsNodes = new List<List<int>>();
    }
}
