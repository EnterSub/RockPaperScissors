using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using RockPaperScissors.Properties;

namespace RockPaperScissors
{
    public partial class Form1 : Form
    {
        int port = 10000;
        StreamReader reader; //Threads
        StreamWriter writer;
        bool sent_hand = false; //Send
        bool read_hand = false; //Receive
        string my_hand = "";
        string op_hand = "";
        public Form1()
        {
            InitializeComponent();
        }
        private void setButtons(bool enable)
        {
            button1.Enabled = enable;
            button2.Enabled = enable;
            button3.Enabled = enable;
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            if (radio_server.Checked == true && radio_client.Checked == false)
            {
                radio_server.Text += " (You)";
                text_ip.Enabled = false;
                radio_server.Enabled = false;
                radio_client.Enabled = false;
                button_start.Enabled = false;
                StartServer();
            }

            if (radio_client.Checked == true && radio_server.Checked == false)
            {
                radio_client.Text += " (You)";
                text_ip.Enabled = false;
                radio_client.Enabled = false;
                radio_server.Enabled = false;
                button_start.Enabled = false;
                StartClient();
            }
            label7.Visible = true;
            button1.Visible = true;
            button1.BringToFront();
            button2.Visible = true;
            button3.Visible = true;
            setButtons(true);
            timer1.Enabled = true;
        }
        private void StartServer()
        {
            TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Parse(text_ip.Text), port)); //Client
            listener.Start(); //запуск
            TcpClient server = listener.AcceptTcpClient();
            server.ReceiveTimeout = 50; //Check signal
                                        //Thread
            reader = new StreamReader(server.GetStream()); //Listen
            writer = new StreamWriter(server.GetStream()); //Send
            writer.AutoFlush = true;
        }
        private void StartClient()
        {
            TcpClient client = new TcpClient(); //Client
            client.Connect(text_ip.Text, port); //Where connect
            client.ReceiveTimeout = 50; //Read
            //Threads
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());
            writer.AutoFlush = true;
        }

        private void button1_Click(object sender, EventArgs e) //Rock
        {
            send("K");
        }

        private void button2_Click(object sender, EventArgs e) //Scissors
        {
            send("B");
        }

        private void button3_Click(object sender, EventArgs e) //Paper
        {
            send("N");
        }

        private void send(string text)
        {
            if (sent_hand) return; //If already have been read, then exit
            writer.WriteLine(text);
            sent_hand = true; //Send (if true, then do)
            my_hand = text; //Hand name
            setButtons(false); //Prevent double click
        }

        private string read()
        {
            if (read_hand) return ""; //If already have been read, then exit
            try //если <50 мс
            {
                string text;
                text = reader.ReadLine();
                read_hand = true; //Read (if true, then do)
                op_hand = text; //Opponent
                return text;
            }
            catch
            {
                return "";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string hand = read();
            if (sent_hand && read_hand)
            {
                label8.Visible = true;
                label_stat.Visible = true;
                FinishGame();
            }
        }

        private async void FinishGame()
        {
            int ch = CompareHands(my_hand, op_hand);
            string res = "";
            if (ch == 1) //KN, NB, BK = win 1
            {
                if (radio_server.Checked == true && radio_client.Checked == false)
                {
                    if (progressBar1.Value + progressBar1.Step / 2 < progressBar1.Maximum && progressBar2.Value + progressBar2.Step / 2 < progressBar2.Maximum)
                    {
                        progressBar1.Value += progressBar1.Step;
                    }
                }
                else
                {
                    if (progressBar1.Value + progressBar1.Step / 2 < progressBar1.Maximum && progressBar2.Value + progressBar2.Step / 2 < progressBar2.Maximum)
                    {
                        progressBar2.Value += progressBar2.Step;
                    }
                }
                res = "Win";
                label9.Visible = true;
                label_stat.Text = res + " (" + my_hand + " & " + op_hand + ")"; //Статистика
                pictureBox1.Visible = true;
                pictureBox1.BringToFront();
                setButtons(false);
                await Task.Delay(1500);
                pictureBox1.Visible = false;
                label9.Visible = false;
                setButtons(true);
            }

            if (ch == 2) //KB, NK, BN = win 2
            {
                if (radio_server.Checked == true && radio_client.Checked == false)
                {
                    if (progressBar1.Value + progressBar1.Step / 2 < progressBar1.Maximum && progressBar2.Value + progressBar2.Step / 2 < progressBar2.Maximum)
                    {
                        progressBar2.Value += progressBar2.Step;
                    }
                }
                else
                {
                    if (progressBar1.Value + progressBar1.Step / 2 < progressBar1.Maximum && progressBar2.Value + progressBar2.Step / 2 < progressBar2.Maximum)
                    {
                        progressBar1.Value += progressBar1.Step;
                    }
                }
                res = "Lose";
                label9.Visible = true;
                label_stat.Text = res + " (" + my_hand + " & " + op_hand + ")"; //Stats
                pictureBox2.Visible = true;
                pictureBox2.BringToFront();
                setButtons(false);
                await Task.Delay(1500);
                pictureBox2.Visible = false;
                label9.Visible = false;
                setButtons(true);
            }

            if (ch == 0)
            {
                res = "Draw";
                label9.Visible = true;
                label_stat.Text = res + " (" + my_hand + " & " + op_hand + ")"; //Stats
                pictureBox3.Visible = true;
                pictureBox3.BringToFront();
                setButtons(false);
                await Task.Delay(1500);
                pictureBox3.Visible = false;
                label9.Visible = false;
                setButtons(true);
            }
            label_stat.Text = "";
            label8.Text = "Round: " + (((progressBar1.Value + progressBar2.Value)/4)+1);
            sent_hand = false;
            read_hand = false;
            setButtons(true);

                if ((radio_server.Checked == true && radio_client.Checked == false) && progressBar1.Value > progressBar2.Value && (progressBar1.Value >= progressBar1.Maximum || progressBar2.Value >= progressBar2.Maximum)) //3 победы
            {
                label10.Text = "The game was finished with the score: " + progressBar1.Value / 4 + ":" + progressBar2.Value / 4;
                label10.Visible = true;
                label10.BringToFront();
                pictureBox4.Visible = true;
                pictureBox4.BringToFront();
                await Task.Delay(1500);
                await Task.Delay(1500);
                this.Close();
            }
            if ((radio_server.Checked == true && radio_client.Checked == false) && progressBar1.Value < progressBar2.Value && (progressBar1.Value >= progressBar1.Maximum || progressBar2.Value >= progressBar2.Maximum)) //3 победы
            {
                label10.Text = "The game was finished with the score: " + progressBar1.Value / 4 + ":" + progressBar2.Value / 4;
                label10.Visible = true;
                label10.BringToFront();
                pictureBox5.Visible = true;
                pictureBox5.BringToFront();
                await Task.Delay(1500);
                await Task.Delay(1500);
                this.Close();
            }

            if ((radio_client.Checked == true && radio_server.Checked == false) && progressBar1.Value > progressBar2.Value && (progressBar1.Value >= progressBar1.Maximum || progressBar2.Value >= progressBar2.Maximum)) //3 победы
            {
                label10.Text = "The game was finished with the score: " + progressBar1.Value / 4 + ":" + progressBar2.Value / 4;
                label10.Visible = true;
                label10.BringToFront();
                pictureBox5.Visible = true;
                pictureBox5.BringToFront();
                await Task.Delay(1500);
                await Task.Delay(1500);
                this.Close();
            }

            if ((radio_client.Checked == true && radio_server.Checked == false) && progressBar1.Value < progressBar2.Value && (progressBar1.Value >= progressBar1.Maximum || progressBar2.Value >= progressBar2.Maximum)) //3 победы
            {
                label10.Text = "The game was finished with the score: " + progressBar1.Value / 4 + ":" + progressBar2.Value / 4;
                label10.Visible = true;
                label10.BringToFront();
                pictureBox4.Visible = true;
                pictureBox4.BringToFront();
                await Task.Delay(1500);
                await Task.Delay(1500);
                this.Close();
            }
        }

        private int CompareHands(string hand1, string hand2) //Rules
        {
            if (hand1 == hand2) return 0;
            if (hand1 == "K")
                if (hand2 == "N")
                {
                    return 1;
                }
                else
                    return 2;
            if (hand1 == "N")
                if (hand2 == "B")
                {
                    return 1;
                }
                else
                    return 2;
            if (hand1 == "B")
                if (hand2 == "K")
                {
                    return 1;
                }
                else
                    return 2;
            return 0;
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click_1(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }
    }
}
