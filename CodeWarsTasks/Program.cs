using CodeWarsTasks;




Test('A', 0, 0);
Test('A', 10, 0);
Test('B', 1, 1);
Test('C', 2, 5);
Test('D', 3, 37);
Test('E', 4, 256);
Test('E', 8, 23281);



void Test(char letter, int legth, int trueResult)
{
    var result = Kata002.CountPatternsFrom(letter, legth);
    Console.WriteLine((result == trueResult) ? true : false);

}


