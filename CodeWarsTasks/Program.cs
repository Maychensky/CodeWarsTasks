using CodeWarsTasks;
using System.Data;

var testStr = "WECRLTEERDSOEEFEAOCAIVDEN";
var n = 5;

var result = RailFenceCipher.Decode(testStr, n);
var tryStr = "WLSADOOTEEECEAEECRFINVEDR";


Console.WriteLine(result);
Console.WriteLine(result.Equals(tryStr));


