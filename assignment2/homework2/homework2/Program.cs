using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homework2
{
    // 定义一个类来保存计算结果
    class ArrayStats
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public double Average { get; set; }
        public int Sum { get; set; }
    }

    class Solution3
    {
        public ArrayStats ProcessArray(int[] array)
        {
            ArrayStats stats = new ArrayStats();

            if (array == null || array.Length == 0)
            {
                Console.WriteLine("Empty or null array.");
                stats.Min = 0;
                stats.Max = 0;
                stats.Average = 0;
                stats.Sum = 0;
                return stats; // 返回默认值，或者可以抛出异常
            }

            int min = array[0];  // 初始化 min 为数组的第一个元素
            int max = array[0];  // 初始化 max 为数组的第一个元素
            int sum = 0;

            foreach (int num in array)
            {
                if (num < min)
                    min = num;

                if (num > max)
                    max = num;

                sum += num;
            }

            double average = (double)sum / array.Length;

            stats.Min = min;
            stats.Max = max;
            stats.Sum = sum;
            stats.Average = average;

            return stats;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            int[] array = { 1, 3, 5, 8, 4, 2 };
            Solution3 s = new Solution3();

            ArrayStats result = s.ProcessArray(array); // 调用方法并接收 ArrayStats 对象

            double roundedAverage = Math.Round(result.Average, 3); // 对平均值进行四舍五入

            Console.WriteLine($"{result.Min} {result.Max} {roundedAverage} {result.Sum}"); // 输出结果
        }
    }
}
