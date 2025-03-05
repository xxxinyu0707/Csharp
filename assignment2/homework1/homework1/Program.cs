using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homework1
{
    class MySolutionSimple
    {
        // 判断是否为素数
        bool IsPrime(int num)
        {
            if (num <= 1) return false; // 1 和小于 1 的数不是素数

            // 从 2 循环到 num - 1
            for (int i = 2; i < num; i++)
            {
                if (num % i == 0)
                {
                    return false; // 如果可以被整除，就不是素数
                }
            }
            return true; // 循环结束，没有被整除，是素数
        }

        // 分解质因数
        public List<int> PrimeFactors(int num)
        {
            List<int> factors = new List<int>(); // 用 List 存储质因数

            if (num <= 1)
            {
                Console.WriteLine("Number " + num + " has no prime factors.");
                return factors;
            }

            int i = 2; // 从 2 开始找质因数
            while (num > 1) // 当 num 大于 1 时循环
            {
                if (IsPrime(i)) // 如果 i 是素数
                {
                    while (num % i == 0) // 如果 num 可以被 i 整除
                    {
                        factors.Add(i); // 将 i 加入 List
                        num /= i; // num 除以 i，更新 num
                    }
                }
                i++; // i 加 1，继续查找
            }

            return factors; // 返回质因数列表
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please input a number to show its prime factors:");

            if (int.TryParse(Console.ReadLine(), out int num))
            {
                MySolutionSimple s = new MySolutionSimple();
                List<int> primeFactors = s.PrimeFactors(num);

                if (primeFactors.Count > 0)
                {
                    Console.WriteLine("Prime factors are: " + string.Join(" ", primeFactors));
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter an integer.");
            }
        }
    }
}
