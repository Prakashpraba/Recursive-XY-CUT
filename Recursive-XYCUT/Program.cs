using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

public class RecursiveXycut
{
    public static int[] Xprojection_by_bboxes(int[][] boxes, int axis)
    {
        if (axis != 0)
        {
            throw new ArgumentException("axis must be 0 for Xprojection");
        }
        //Projection for Bounding Boxes along the X-Axis

        int length = boxes.Select(b => b[axis * 2 + 2]).Max();

        int[] res = new int[length];

        res = res.Select((_, i) => boxes.Count(b => b[axis * 2] < i && i <= b[axis * 2 + 2])).ToArray();

        return res;
    }

    public static int[] Yprojection_by_bboxes(int[][] boxes, int axis)
    {
        if (axis != 1)
        {
            throw new ArgumentException("axis must be 1 for Yprojection");
        }
        //Projection for Bounding Boxes along the Y-Axis
        if (boxes.Length == 0)
        {
            return new int[0]; 
        }
        int length = boxes.Select(b => b[axis * 2 + 1]).Max();

        int[] res = new int[length];

        res = res.Select((_, i) => boxes.Count(b => b[axis + axis * 2] > i && i >= b[axis])).ToArray();

        return res;
    }

    static Tuple<int[], int[]> split_projection_profile(int[] arrValues, float minValue, float minGap)
    {
        int[] arr_index = Recursive_XYCUT.Support.WhereGreaterThan(arrValues, minValue);
        
        //Trial
        Console.WriteLine("arrValues :");
        foreach (int i in arrValues)
        {
            Console.Write(i + " ");
        }
        Console.WriteLine();


        if (arr_index.Length == 0)
        {
            return Tuple.Create(arr_index, arr_index);
        }
        
        //Trial
        Console.WriteLine("arr_index :");
        foreach (int i in arr_index)
        {
            Console.Write(i + " ");
        }
        Console.WriteLine();

        int[] arr_diff = new int[arr_index.Length - 1];
        for (int i = 0; i < arr_index.Length - 1; i++)
        {
            arr_diff[i] = arr_index[i + 1] - arr_index[i];
        }
        
        //Trial
        Console.WriteLine("arr_diff :");
        foreach (int i in arr_diff)
        {
            Console.Write(i + " ");
        }
        Console.WriteLine();

        int[] arr_diff_index = Recursive_XYCUT.Support.WhereGreaterThan(arr_diff, minGap);
        
        //Trial
        Console.WriteLine("arr_diff_index :");
        foreach (int i in arr_diff_index)
        {
            Console.Write(i + " ");
        }
        Console.WriteLine();

        int[] arrZeroIntvlStart = arr_diff_index.Select(i => arr_index[i]).ToArray();
        
        //Trial
        Console.WriteLine("arrZeroIntvlStart :");
        foreach (int i in arrZeroIntvlStart)
        {
            Console.Write(i + " ");
        }
        Console.WriteLine();

        int[] arrZeroIntvlEnd = arr_diff_index.Select(i => arr_index[i + 1]).ToArray();
        
        //Trial
        Console.WriteLine("arrZeroIntvlEnd :");
        foreach (int i in arrZeroIntvlEnd)
        {
            Console.Write(i + " ");
        }
        Console.WriteLine();

        List<int> arrZeroIntvlEndList = new List<int>(arrZeroIntvlEnd);
        arrZeroIntvlEndList.Insert(0, arr_index[0]);
        int[] arr_start = arrZeroIntvlEndList.ToArray();
        
        //Trial
        Console.WriteLine("arr_start :");
        foreach (int i in arr_start)
        {
            Console.Write(i + " ");
        }
        Console.WriteLine();

        List<int> arrEndList = new List<int>(arrZeroIntvlStart);
        arrEndList.Add(arr_index[arr_index.Length - 1]);
        int[] arr_end = arrEndList.ToArray();
        for (int i = 0; i < arr_end.Length; i++)
        {
            arr_end[i] += 1;
        }
        
        //Trial
        Console.WriteLine("arr_end :");
        foreach (int i in arr_end)
        {
            Console.Write(i + " ");
        }
        Console.WriteLine();

        return Tuple.Create(arr_start, arr_end);
    }

    public static void RecursiveXYCUT(int[][] boxes, int[] indices, List<int> res)
    {
        if (boxes.Length != indices.Length)
        {
            throw new ArgumentException("indices and boxes are not equal");
        }

        var _indices = Enumerable.Range(0, boxes.Length).OrderBy(i => boxes[i][1]).ToArray();
        //var _indices = Recursive_XYCUT.Support.GetSortedIndices(boxes, 1).ToArray();
        
        //Trial
        Console.WriteLine("_indices :" );
        foreach (int i in _indices)
        {
            Console.Write(i + " ");
        }
        Console.WriteLine();
        
        
        var ySortedBoxes = new int[boxes.Length][];
        var ySortedIndices = new int[indices.Length];

        for (int i = 0; i < _indices.Length; i++)
        {
            ySortedBoxes[i] = new int[boxes[i].Length];
            for (int j = 0; j < boxes[i].Length; j++)
            {
                ySortedBoxes[i][j] = boxes[_indices[i]][j];
            }
            ySortedIndices[i] = indices[_indices[i]];
        }

        //Trial
        Console.WriteLine("ySortedBoxes :");
        foreach (var innerArray in ySortedBoxes)
        {
            foreach (var element in innerArray)
            {
                Console.Write(element + " ");
            }
            Console.WriteLine();
        }

        //Trial
        Console.WriteLine("ySortedindices :");
        foreach (int i in ySortedIndices)
        {
            Console.Write(i + " ");
        }
        Console.WriteLine();

        int[] y_projection = Yprojection_by_bboxes(ySortedBoxes, 1);

        //Trial
        Console.WriteLine("y_projection :");
        foreach (int i in y_projection)
        {
            Console.Write(i + " ");
        }
        Console.WriteLine();

        Tuple<int[], int[]> pos_y = split_projection_profile(y_projection, 0, 1);

        int[] arr_y0 = pos_y.Item1;
        int[] arr_y1 = pos_y.Item2;

        if (arr_y0 == null || arr_y1 == null)
        {
            return;
        }

        for (int k = 0; k < arr_y0.Length && k < arr_y1.Length; k++)
        {
            int r0 = arr_y0[k];
            int r1 = arr_y1[k];

            List<int> indices1 = new List<int>();

            for (int i = 0; i < ySortedBoxes.GetLength(0); i++)
            {
                if (r0 <= ySortedBoxes[i][1] && ySortedBoxes[i][1] < r1)
                {
                    indices1.Add(i);
                }
            }

            int[] _indices1 = indices1.ToArray();

            //Trial
            Console.WriteLine("_indices1 :");
            foreach (int i in _indices1)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine();

            List<int[]> ySortedBoxesChunkList = _indices1.Select(i => ySortedBoxes[i]).ToList();

            List<int> ySortedIndicesChunkList = _indices1.Select(i => ySortedIndices[i]).ToList();

            int[][] ySortedBoxesChunk = ySortedBoxesChunkList.ToArray();

            int[] ySortedIndicesChunk = ySortedIndicesChunkList.ToArray();

            //Trial
            Console.WriteLine("ySortedBoxesChunk :");
            foreach (var innerArray in ySortedBoxesChunk)
            {
                foreach (var element in innerArray)
                {
                    Console.Write(element + " ");
                }
                Console.WriteLine();
            }

            //Trial
            Console.WriteLine("ySortedIndicesChunk :");
            foreach (int i in ySortedIndicesChunk)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine();

            //var sortedIndices = Enumerable.Range(0, ySortedBoxes.Length).OrderBy(i => ySortedBoxesChunk[i][0]).ToArray();
            var sortedIndices = Recursive_XYCUT.Support.GetSortedIndices(ySortedBoxesChunk, 0).ToArray();
            //Trial
            Console.WriteLine("sortedIndices :");
            foreach (int i in sortedIndices)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine();

            int[][] xSortedBoxesChunk = _indices1.Select(i => ySortedBoxesChunk[i - _indices1[0]]).ToArray();
            int[] xSortedIndicesChunk = _indices1.Select(i => ySortedIndicesChunk[i - _indices1[0]]).ToArray();

            //Trial
            Console.WriteLine("xSortedBoxesChunk :");
            foreach (var innerArray in xSortedBoxesChunk)
            {
                foreach (var element in innerArray)
                {
                    Console.Write(element + " ");
                }
                Console.WriteLine();
            }

            //Trial
            Console.WriteLine("xSortedIndicesChunk :");
            foreach (int i in xSortedIndicesChunk)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine();

            int[] x_projection = Xprojection_by_bboxes(xSortedBoxesChunk, 0);

            //Trial
            Console.WriteLine("x_projection :");
            foreach (int i in x_projection)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine();

            Tuple<int[], int[]> pos_x = split_projection_profile(x_projection, 0, 1);

            if (pos_x == null || pos_x.Item1 == null || pos_x.Item2 == null || pos_x.Item1.Length == 0 || pos_x.Item2.Length == 0)
            {
                continue;
            }

            int[] arr_x0 = pos_x.Item1;
            int[] arr_x1 = pos_x.Item2;

            if (arr_x0.Length == 1)
            {
                // x direction cannot be split
                res.AddRange(xSortedIndicesChunk);
                Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
                Console.WriteLine("lalalalalala");
                Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
                continue;
            }
            Console.WriteLine("the result is:");
            foreach (int i in res)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine();

            for (int m = 0; m < Math.Min(arr_x0.Length, arr_x1.Length); m++)
            {
                Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
                Console.WriteLine("THE FOR LOOP IS CALLED!!!!!");
                Console.WriteLine("xSortedIndicesChunk :");
                foreach (int i in xSortedIndicesChunk)
                {
                    Console.Write(i + " ");
                }
                Console.WriteLine();

                int c0 = arr_x0[m] - 1;
                int c1 = arr_x1[m] - 1;

                Console.WriteLine("c0: " + c0 + ", c1: " + c1);
                Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");

                List<int> indices2 = new List<int>();

                for (int n = 0; n < xSortedBoxesChunk.GetLength(0); n++)
                {
                    if (c0 <= xSortedBoxesChunk[n][0] && xSortedBoxesChunk[n][0] < c1)
                    {
                        indices2.Add(n);
                    }

                    int[] _indices2 = indices2.ToArray();
                    Console.WriteLine("_indices2: ");
                    foreach (int i in _indices2)
                    {
                        Console.Write(i + " ");
                    }
                    Console.WriteLine();
                    var filteredXSortedBoxesChunk = _indices2.Select(i => xSortedBoxesChunk[i]).ToArray();
                    var filteredXSortedIndicesChunk = _indices2.Select(i => xSortedIndicesChunk[i]).ToArray();

                    //Trial
                    Console.WriteLine("filteredXSortedBoxesChunk :");
                    foreach (var innerArray in filteredXSortedBoxesChunk)
                    {
                        foreach (var element in innerArray)
                        {
                            Console.Write(element + " ");
                        }
                        Console.WriteLine();
                    }

                    //Trial
                    Console.WriteLine("filteredXSortedIndicesChunk  :");
                    foreach (int i in filteredXSortedIndicesChunk)
                    {
                        Console.Write(i + " ");
                    }
                    Console.WriteLine();

                    bool alreadyCalled = false; 

                    
                    if (!alreadyCalled)
                    {
                        RecursiveXYCUT(filteredXSortedBoxesChunk, filteredXSortedIndicesChunk, res);
                        alreadyCalled = true; 
                    }
                }
                Console.WriteLine("the result is:");
                foreach (int i in res)
                {
                    Console.Write(i + " ");
                }
                Console.WriteLine();
                List<int> resu = new List<int>();
                foreach (int item in res)
                {
                    if (!resu.Contains(item)) 
                    {
                        resu.Add(item); 
                    }
                }
                res.Clear(); 
                res.AddRange(resu); 
                Console.WriteLine("THE RESULT IS:");
                foreach (int i in res)
                {
                    Console.Write(i + " ");
                }
                Console.WriteLine();
            }
        }
    }

    public static void DrawBoundingBoxes(int[][] boxes, List<int> indices, string imagePath)
    {
        using (var bmp = new Bitmap(imagePath))
        using (var graphics = Graphics.FromImage(bmp))
        using (var pen = new Pen(Color.Red, 2))
        using (var font = new System.Drawing.Font("Arial", 12))
        using (var brush = new SolidBrush(Color.Red))
        {
            for (int i = 0; i < indices.Count; i++)
            {
                int index = indices[i];
                int x = boxes[index][0];
                int y = boxes[index][1];
                int width = boxes[index][2] - x;
                int height = boxes[index][3] - y;
                graphics.DrawRectangle(pen, x, y, width, height);
                graphics.DrawString($"{i + 1}", font, brush, x, y);
            }

            bmp.Save("C:\\Users\\rr422792\\Documents\\models\\Sample_Output.png");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //note-to-self:  the input order is x1, y1, x2, y3 not x1, x2, y1, y2 like in DLA
            int[][] boxes =
            {
              new [] {390,22,564,45},
new [] {189,108,754,163},
new [] {647,191,902,336},
new [] {347,192,592,356},
new [] {51,191,300,406}
            };

            int[] indices = { 0, 1, 2, 3, 4};

            List<int> res = new List<int>();
           

            RecursiveXYCUT(boxes, indices, res);

            Console.WriteLine("Sorted order of bounding boxes:");
            for (int i = 0; i < res.Count; i++)
            {
                int index = res[i];
                Console.WriteLine($"Order No: {i + 1}, Bounding Box {index + 1}: ({boxes[index][0]}, {boxes[index][1]}, {boxes[index][2]}, {boxes[index][3]})");
                
            }

            DrawBoundingBoxes(boxes, res, "C:\\Users\\rr422792\\Documents\\models\\sample0.jpg");


        }
    }
}
