using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeWarsTasks
{
    internal class RangeExtraction
    {
        public static string Extract(int[] args) => JoinIntervals(args); 
        private static string JoinIntervals(int[] args)
        {
            int lengthTrueInterval = 0;
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < args.Length; i ++)
            {
                if (args[i-1] + 1 == args[i])
                    lengthTrueInterval++;
                else
                {
                    var isLast = i == args.Length - 1;
                    if (lengthTrueInterval > 0)
                    {
                        var intervalFormat = GetIntervalFormat(args[i - lengthTrueInterval- 1 ], args[i - 1]);
                        AddElement(ref sb, intervalFormat, isLast);
                    }
                    else
                        AddElement(ref sb, args[i-1].ToString(), isLast);
                    if (isLast)
                        AddElement(ref sb, args[i].ToString(), isLast);
                    lengthTrueInterval = 0;
                }
            }
            if (lengthTrueInterval > 0)
            {
                var intervalFormat = GetIntervalFormat(args[args.Length - lengthTrueInterval - 1], args[args.Length - 1]);
                AddElement(ref sb, intervalFormat, true);
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        private static string GetIntervalFormat(int v1, int v2) => (v1 + 1 == v2 ) ? string.Format("{0},{1}", v1, v2) : string.Format("{0}-{1}", v1, v2);
        private static void AddElement(ref StringBuilder sb ,string element, bool isLast = false) => sb.Append(string.Format("{0},", element));
    }
}
