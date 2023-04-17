namespace CodeWarsTasks
{
    // kata: https://www.codewars.com/kata/52b7ed099cdc285c300001cd/train/csharp
    public class Intervals
    { 
        public static int SumIntervals((int, int)[] intervals)
        {
            int sumAllIntervals = SumAllIntervals(intervals);
            var pointsAndItsDirectionBrackets = CreateArrayIntervalsPoint(intervals);
            int sumOverlappingIntervals = SumOverlappingIntervals(pointsAndItsDirectionBrackets);
            return sumAllIntervals - sumOverlappingIntervals;
        }

        private static int SumOverlappingIntervals((int, bool)[] pointsAndItsDirectionBrackets)
        {
            int currentCountBrackets = -1;
            int sumOverlappingIntervals = 0;
            (int, bool) prevPoint = (0, false);
            foreach (var currentPoint in pointsAndItsDirectionBrackets)
            {
                sumOverlappingIntervals += (currentCountBrackets > 0) ? (currentPoint.Item1 - prevPoint.Item1) * currentCountBrackets : 0;
                currentCountBrackets += (currentPoint.Item2) ? 1 : -1;
                prevPoint = currentPoint;
            }
            return sumOverlappingIntervals;
        }

        private static (int, bool)[] CreateArrayIntervalsPoint((int, int)[] intervals)
        {
            (int, bool)[] pointsAndItsDirectionBrackets = new (int, bool)[intervals.Length * 2];
            for (int i = 0; i < intervals.Length; i ++)
            {
                pointsAndItsDirectionBrackets[i] = (intervals[i].Item1, true);
                pointsAndItsDirectionBrackets[intervals.Length + i] = (intervals[i].Item2, false);
            }
            Array.Sort(pointsAndItsDirectionBrackets);
            return pointsAndItsDirectionBrackets;
        }

        private static int SumAllIntervals((int, int)[] intervals)
        {
            int sumAllIntervals = 0;
            foreach (var interval in intervals)
                sumAllIntervals += interval.Item2 - interval.Item1;
            return sumAllIntervals;
        }
    }
}
