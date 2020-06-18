using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace IDontKonwHowToDoIt
{
    public partial class Form1 : Form
    {
        private DateTime current_time = new DateTime();
        public Series S;
        public DateTime base_time = new DateTime();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 检测串口
            comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            // 初始化
            comboBox1.Text = "COM20";
            comboBox2.Text = "115200";
            comboBox3.Text = "8";
            comboBox4.Text = "无校验";
            comboBox5.Text = "1";
            // 表格初始化
            chart1.Series.Clear();

            S = new Series();
            base_time = System.DateTime.Now;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                //根据当前串口属性来判断是否打开
                if (serialPort1.IsOpen)
                {
                    //串口已经处于打开状态
                    serialPort1.Close();    //关闭串口
                    button1.Text = "打开串口";
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox4.Enabled = true;
                    comboBox5.Enabled = true;
                }
                else
                {
                    //串口已经处于关闭状态，则设置好串口属性后打开
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = false;
                    comboBox4.Enabled = false;
                    comboBox5.Enabled = false;
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                    serialPort1.DataBits = Convert.ToInt16(comboBox3.Text);

                    if (comboBox4.Text.Equals("无校验"))
                        serialPort1.Parity = System.IO.Ports.Parity.None;
                    else if (comboBox4.Text.Equals("奇校验"))
                        serialPort1.Parity = System.IO.Ports.Parity.Odd;
                    else if (comboBox4.Text.Equals("偶校验"))
                        serialPort1.Parity = System.IO.Ports.Parity.Even;

                    if (comboBox5.Text.Equals("0"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.None;
                    else if (comboBox5.Text.Equals("1"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.One;
                    else if (comboBox5.Text.Equals("1.5"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.OnePointFive;
                    else if (comboBox5.Text.Equals("2"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.Two;

                    serialPort1.Open();     //打开串口
                    button1.Text = "关闭串口";
                }
            }
            catch (Exception ex)
            {
                //捕获到异常，创建一个新的对象
                serialPort1 = new System.IO.Ports.SerialPort();
                //刷新COM口选项
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                button1.Text = "打开串口";
                MessageBox.Show(ex.Message);
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                //串口已经处于打开状态
                textBox_receive.Text = "";  //清空接收区
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {//如果串口开启
                if (textBox_send.Text.Trim() != "")//如果框内不为空
                {
                    serialPort1.Write(textBox_send.Text.Trim());//写数据
                }
                else
                {
                    MessageBox.Show("发送框没有数据");
                }
            }
            else
            {
                MessageBox.Show("串口未打开");
            }
        }

        private void SerialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                //invoke同步ui
                this.Invoke((EventHandler)(delegate
                {
                    current_time = System.DateTime.Now;
                    if (checkBox3.Checked)
                    {
                        textBox_receive.AppendText(current_time.ToString("HH:mm:ss") + " ");
                    }
                    textBox_receive.AppendText(serialPort1.ReadExisting());
                    var time = current_time.ToOADate();
                    var base_t = base_time.ToOADate();
                    int x = (int)time - (int)base_t;
                    S.Points.AddXY(x, serialPort1.ReadExisting());
                }
                   )
                );

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // 表格初始化
            chart1.Series.Clear();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            chart1.Series.Add(S);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            chart1.Series.Remove(S);
        }
    }
}
