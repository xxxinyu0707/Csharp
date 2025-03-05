using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homework3
{
    class MySolution // 定义一个名为 MySolution 的类
    {
        public int[] AllSunNum(int r) // 定义一个名为 AllSunNum 的方法，用于找出给定范围内（从 2 到 r）的所有素数
        {
            bool[] issu = new bool[r + 1]; // 创建一个布尔数组，用于标记每个数是否为素数
            Array.Fill(issu, true); // 将数组中的所有元素初始化为 true，表示初始状态下所有数都认为是素数
            int size = 0; // 初始化 size 为 0，用于记录素数的个数

            for (int i = 2; i <= r; i++) // 从 2 循环到 r
            {
                if (!issu[i]) continue; // 如果 i 已经被标记为非素数，则跳过本次循环

                size++; // 如果 i 是素数，则素数个数加 1

                for (int k = i * 2; k <= r; k += i) // 将 i 的所有倍数标记为非素数
                {
                    issu[k] = false;
                }
            }

            int[] allSu = new int[size]; // 创建一个整数数组，用于存储所有素数
            int j = 0; // 初始化 j 为 0，用于记录数组的索引

            for (int i = 2; i <= r; i++) // 从 2 循环到 r
            {
                if (issu[i]) // 如果 i 是素数
                {
                    allSu[j] = i; // 将 i 存入数组
                    j++; // 索引加 1
                }
            }

            return allSu; // 返回存储所有素数的数组
        }
    };

    internal class Program // 定义一个名为 Program 的类
    {
        static void Main(string[] args) // 定义一个名为 Main 的方法，是程序的入口点
        {
            MySolution solution = new MySolution(); // 创建一个 MySolution 类的实例
            int[] allsun = solution.AllSunNum(100); // 调用 AllSunNum 方法找出 100 以内的所有素数
            foreach (int i in allsun) // 循环遍历数组中的每一个元素
            {
                Console.Write(i + " "); // 打印素数，并在数字后加一个空格
            }
        }
    }
}
