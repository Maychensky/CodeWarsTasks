using CodeWarsTasks;



int[,] field = new int[10, 10]
{
{1, 0, 0, 0, 0, 1, 1, 0, 0, 0} ,
{1, 0, 1, 0, 0, 0, 0, 0, 1, 0} ,
{1, 0, 1, 0, 1, 1, 1, 0, 1, 0} ,
{1, 0, 0, 0, 0, 0, 0, 0, 0, 0} ,
{0, 0, 0, 0, 0, 0, 0, 0, 1, 0} ,
{0, 0, 0, 0, 1, 1, 1, 0, 0, 0} ,
{0, 0, 0, 0, 0, 0, 0, 0, 1, 0} ,
{0, 0, 0, 1, 0, 0, 0, 0, 0, 0} ,
{0, 0, 0, 0, 0, 0, 0, 1, 0, 0} ,
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0} ,
};

var test = BattleshipField.ValidateBattlefield(field);
Console.WriteLine(test);

//Dictionary<int, int> dic1 = new Dictionary<int, int>
//{
//    {1, 2}
//};
//Dictionary<int, int> dic2 = new Dictionary<int, int>
//{
//    {1, 1 }
//};

//Console.WriteLine(Eqlips(dic1, dic2));

//bool Eqlips(Dictionary<int, int> dic1, Dictionary<int, int> dic2)
//{
//    if (dic1.Count != dic2.Count) return false;
//    foreach (var elementDic1 in dic1)
//        if (!(dic2.ContainsKey(elementDic1.Key) && dic2[elementDic1.Key] == elementDic1.Value))
//            return false;
//    return true;
//}

