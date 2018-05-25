using RabbitMQService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        ServerRabbit serverRabbit = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            serverRabbit = new ServerRabbit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            serverRabbit.Dispose();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //pause?
            //serverRabbit.Cancel();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //we cant do resume (continue) ping because clients(peers)  become older than 5 second(and automaticly delete)
            //and than is raight way. Client must to try reconnect to server. That move 
            //serverRabbit.tokenSource = new CancellationTokenSource();
            //serverRabbit.PingToAll();
        }

        private void restart_Click(object sender, EventArgs e)
        {
            if(!serverRabbit.connection.IsOpen)
            {
                serverRabbit = new ServerRabbit();
            }
        }
    }
}
