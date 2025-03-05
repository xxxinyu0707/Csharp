using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homework4
{
    class Solution
    {
        public bool IsToeplitzMatrix(int[][] matrix)
        {
            int m = matrix.Length;
            if (m == 0) return false; // 空矩阵不是托普利茨矩阵
            int n = matrix[0].Length;
            if (n == 0) return false;

            // 遍历所有元素，检查它与它的右下角元素是否一致
            for (int i = 0; i < m - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    if (matrix[i][j] != matrix[i + 1][j + 1])
                    {
                        return false; // 如果不一致，说明不是托普利茨矩阵
                    }
                }
            }

            return true; // 如果所有相邻元素都满足托普利茨矩阵的条件，则是托普利茨矩阵
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            int[][] matrix = new int[3][];
            matrix[0] = new int[] { 1, 2, 3, 4 };
            matrix[1] = new int[] { 5, 1, 2, 3 };
            matrix[2] = new int[] { 9, 5, 1, 2 };
            Solution s = new Solution();
            Console.WriteLine(s.IsToeplitzMatrix(matrix));
        }
    }
}
