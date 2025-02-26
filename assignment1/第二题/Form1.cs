using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1234
{
    public partial class Form1 : Form
    {
        private double num1 = 0;
        private double num2 = 0;
        private string operation = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void operatorButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string currentOperation = button.Text; // 获取当前点击的运算符

            // 输入验证
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("请输入两个数字。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!double.TryParse(textBox1.Text, out num1) || !double.TryParse(textBox2.Text, out num2))
            {
                MessageBox.Show("无效的数字格式。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double result = 0;

            // 执行计算
            switch (currentOperation) // 使用当前运算符
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
                        textBoxResult.Text = "Error: Division by zero";
                        return;
                    }
                    result = num1 / num2;
                    break;
                default:
                    MessageBox.Show("无效的运算符。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
            }

            operation = currentOperation; // 保存当前运算符，以便连续计算
            textBoxResult.Text = result.ToString(); // 显示结果
        }

    }
}

