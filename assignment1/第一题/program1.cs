using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _123
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("请输入第一个数字: ");
            double num1 = double.Parse(Console.ReadLine());

            Console.Write("请输入第二个数字: ");
            double num2 = double.Parse(Console.ReadLine());

            Console.Write("请输入运算符 (+, -, *, /): ");
            string operation = Console.ReadLine();

            double result = 0;

            switch (operation)
            {
                case "+":
                    result = num1 + num2;
                    break;
                case "-":
                    result = num1 - num2;
                    break;
                case "*":
                    result = num1 * num2;
                    break;
                case "/":
                    if (num2 == 0)
                    {
                        Console.WriteLine("错误：除数不能为0！");
                        return; // Exit the program
                    }
                    result = num1 / num2;
                    break;
                default:
                    Console.WriteLine("错误：无效的运算符！");
                    return; // Exit the program
            }

            Console.WriteLine($"计算结果是: {result}");
        }
    }
}
