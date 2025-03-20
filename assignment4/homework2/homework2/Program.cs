using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace homework2
{
    public class Clock
    {
        // 定义事件
        public event EventHandler Tick; // 嘀嗒事件
        public event EventHandler Alarm; // 响铃事件


        private int alarmHour;
        private int alarmMinute;


        public Clock(int alarmHour, int alarmMinute)
        {
            this.alarmHour = alarmHour;
            this.alarmMinute = alarmMinute;
        }


        // 触发嘀嗒事件的方法
        protected virtual void OnTick(EventArgs e)
        {
            Tick?.Invoke(this, e); // 如果有订阅者，则触发 Tick 事件
        }


        // 触发响铃事件的方法
        protected virtual void OnAlarm(EventArgs e)
        {
            Alarm?.Invoke(this, e); // 如果有订阅者，则触发 Alarm 事件
        }


        // 运行闹钟
        public void Run()
        {
            while (true)
            {
                DateTime now = DateTime.Now;


                // 触发嘀嗒事件
                OnTick(EventArgs.Empty);


                // 检查是否到达闹钟时间
                if (now.Hour == alarmHour && now.Minute == alarmMinute && now.Second == 0)
                {
                    OnAlarm(EventArgs.Empty);
                    break; // 闹钟响铃后停止
                }


                Thread.Sleep(1000); // 每秒嘀嗒一次
            }
        }
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            // 创建一个闹钟，设定闹钟时间
            Console.WriteLine("设置闹钟时间(小时):");
            int alarmHour = int.Parse(Console.ReadLine());
            Console.WriteLine("设置闹钟时间(分钟):");
            int alarmMinute = int.Parse(Console.ReadLine());


            Clock clock = new Clock(alarmHour, alarmMinute);


            // 订阅嘀嗒事件
            clock.Tick += (sender, e) =>
            {
                Console.WriteLine("Tick: " + DateTime.Now.ToString("HH:mm:ss"));
            };


            // 订阅响铃事件
            clock.Alarm += (sender, e) =>
            {
                Console.WriteLine("Alarm! Alarm!");
            };


            // 运行闹钟
            clock.Run();
        }
    }
}
