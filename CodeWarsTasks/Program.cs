using CodeWarsTasks;
using System.Data.Common;

var test = Kata003.TreeByLevels(new Node(new Node(null, new Node(null, null, 4), 2), new Node(new Node(null, null, 5), new Node(null, null, 6), 3), 1));
foreach (var e in test)
    Console.WriteLine(e);



