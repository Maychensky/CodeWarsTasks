using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;


// ката не завершена https://www.codewars.com/kata/55dcdd2c5a73bdddcb000044/train/csharp

namespace CodeWarsTasks
{   
    internal class Kata
    {
        private enum Axis { X = 0, Y = 1 };
        public static long Calculate(IEnumerable<int[]> rectangles) // [x0, y0, x1, y1]
        {
            var sumSquareRectangles = GetSumSquareRectangles(rectangles);
            var areaIntersectionRectangles = GetAreaIntersectionRectangles(rectangles);
            return sumSquareRectangles - areaIntersectionRectangles;
        }

        private static int GetSumSquareRectangles(IEnumerable<int[]> rectangles)
        {
            var sumSquareRectangles = 0;
            foreach (var rectangle in rectangles)
                sumSquareRectangles += (rectangle[2] - rectangle[0]) * (rectangle[3] - rectangle[1]);
            return sumSquareRectangles;
        }

        private static int GetAreaIntersectionRectangles(IEnumerable<int[]> rectangles)
        {
            var pointsAxisX = GetPointsAxis(rectangles, Axis.X);
            var pointsAxisY = GetPointsAxis(rectangles, Axis.Y);
            return FindingIntersectionRectanglesByPoints(pointsAxisX, pointsAxisY);
        }

        private static PointAxis[] GetPointsAxis(IEnumerable<int[]> rectangles, Axis x)
        {
            List<PointAxis> pointsAxis = new List<PointAxis>();
            int indexPoint = 0;
            foreach (var rectangle in rectangles)
            {
                pointsAxis.Add(new (rectangle[(int)x], indexPoint, PointAxis.PointPosition.Smaller));
                pointsAxis.Add(new(rectangle[(int)x + 2], indexPoint++, PointAxis.PointPosition.Bigger));
            }
            pointsAxis.Sort();
            return pointsAxis.ToArray(); 
        }

        private static int FindingIntersectionRectanglesByPoints(PointAxis[] pointsAxisX, PointAxis[] pointsAxisY)
        {
            HashSet<PointAxis> currentPoints  = new HashSet<PointAxis>();
            foreach (var currentPoint in pointsAxisX)
            {
                if (currentPoint.pointPosition == PointAxis.PointPosition.Smaller) // добавляем
                {
                    currentPoints .Add(currentPoint);
                    CheckIntersection(currentPoints, pointsAxisY);
                } 
                else currentPoints .Remove(currentPoint);
            }
            return 0;
            throw new NotImplementedException();
        }

        private static void CheckIntersection(HashSet<PointAxis> otherPoints, PointAxis[] pointsAxisY)
        {
            HashSet<PointAxis> currentPoints = new HashSet<PointAxis>();
            foreach (var currentPoint in pointsAxisY)
            {
                if (currentPoint.pointPosition == PointAxis.PointPosition.Smaller) // добавляем
                {
                    currentPoints.Add(currentPoint);
                    CheckForIdenticalPoints(currentPoints, pointsAxisY);
                }
                else currentPoints.Remove(currentPoint);
            }
            return;
            throw new NotImplementedException();

        }

        private static void CheckForIdenticalPoints(HashSet<PointAxis> currentPoints, PointAxis[] pointsAxisY)
        {
            int[] points1 = GetArrayIndexPoints(currentPoints);
            int[] points2 = GetArrayIndexPoints(pointsAxisY);
            var countIntersectIndexPoints = points1.Intersect(points2);
            foreach (var e in points1)
                Console.Write(e + " ");
            Console.WriteLine();
            foreach (var e in points2)
                Console.Write(e + " ");
            Console.WriteLine(countIntersectIndexPoints.Count());
        }

        private static int[] GetArrayIndexPoints(IEnumerable<PointAxis> pointAxes)
        {
            var listIndexesPoints = new List<int>();
            foreach (var e in pointAxes)
                listIndexesPoints.Add(e.indexPoint);
            return listIndexesPoints.ToArray();
        }
    }

    public struct PointAxis : IComparable
    {
        public enum PointPosition { Smaller, Bigger }
        public int pointValue { get; private set; }
        public int indexPoint { get; private set; }
        public PointPosition pointPosition { get; private set; }
        public PointAxis(int pointValue, int indexPoint, PointPosition pointPosition)
        {
            this.pointValue = pointValue;
            this.indexPoint = indexPoint;
            this.pointPosition = pointPosition;
        }

        public int CompareTo(object? obj)
        {
            return this.pointValue - ((PointAxis)obj).pointValue;
        }
    }
}
