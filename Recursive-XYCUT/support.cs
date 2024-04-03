using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recursive_XYCUT
{
    internal class Support
    {
        public static int GetMaxCoordinate(int[,] boxes, int axis)
        {
            int maxCoordinate = int.MinValue;

            int step = (axis == 0) ? 2 : 2;

            for (int i = 0; i < boxes.GetLength(0); i++)
            {
                int coordinate = boxes[i, axis];

                if (coordinate > maxCoordinate)
                {
                    maxCoordinate = coordinate;
                }
            }

            return maxCoordinate;
        }

        public static int[] WhereGreaterThan(int[] arrValues, float minValue)
        {
            var indices = Enumerable.Range(0, arrValues.Length)
                                    .Where(i => arrValues[i] > minValue)
                                    .ToArray();

            return indices;
        }

        public static int[] GetSortedIndices(int[][] array, int columnIndex)
        {
            int[] column = array.Select(row => row[columnIndex]).ToArray();
            Console.WriteLine("column:");
            foreach (var item in column)
            {
                Console.WriteLine(item);
            }

            int[] indices = Enumerable.Range(0, column.Length)
                                       .OrderBy(i => column[i])
                                       .ToArray();

            return indices;
        }
    }
}
