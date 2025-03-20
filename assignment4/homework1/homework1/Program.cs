using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homework1
{
    public class GenericList<T>
    {
        private Node<T> head;
        private Node<T> tail;


        public GenericList()
        {
            tail = head = null;
        }


        public Node<T> Head
        {
            get { return head; }
        }


        public void Add(T t)
        {
            Node<T> n = new Node<T>(t);
            if (tail == null)
            {
                head = tail = n;
            }
            else
            {
                tail.Next = n;
                tail = n;
            }
        }
        // Node 类 (内部类)
        public class Node<T>
        {
            public T Data { get; set; }
            public Node<T> Next { get; set; }


            public Node(T data)
            {
                Data = data;
                Next = null;
            }

        }
        public void ForEach(Action<T> action)
        {
            Node<T> current = head;
            while (current != null)
            {
                action(current.Data);
                current = current.Next;
            }
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            GenericList<int> numbers = new GenericList<int>();
            numbers.Add(5);
            numbers.Add(2);
            numbers.Add(8);
            numbers.Add(1);


            // 打印链表元素
            Console.WriteLine("链表元素:");
            numbers.ForEach(num => Console.WriteLine(num));


            // 求最大值
            int max = int.MinValue;
            numbers.ForEach(num => max = Math.Max(max, num));
            Console.WriteLine("最大值: " + max);


            // 求最小值
            int min = int.MaxValue;
            numbers.ForEach(num => min = Math.Min(min, num));
            Console.WriteLine("最小值: " + min);


            // 求和
            int sum = 0;
            numbers.ForEach(num => sum += num);
            Console.WriteLine("总和: " + sum);
        }
    }
}
