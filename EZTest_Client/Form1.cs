using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace EZTest_Client
{
    public partial class Form1 : Form
    {
        public static NetworkStream stream { get; set; }
        public TcpClient closeClient { get; set; }

        public Point mouseLocation;

        public Form1()
        {
            InitializeComponent();
        }

        private void Connect(string ip)
        {
            try
            {
                TcpClient client = new TcpClient(ip, 3559);
                stream = client.GetStream();
                closeClient = client;
                TCPConnection tcp = new TCPConnection(closeClient, stream);
                Main main = new Main(tcp);
                this.Invoke(new Action(() => main.Show()));
                this.Invoke(new Action(() => this.Hide()));
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.StackTrace);
                MessageBox.Show(e1.Message);
            }
        }

        public void hide()
        {
            TCPConnection tcp = new TCPConnection(closeClient, stream);
            Main main = new Main(tcp);
            main.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ip = textBox1.Text;

            Task.Run(() =>
            {
                if (ip.Length > 5)
                {
                    showBalloon("Připojování", $"Počkejte chvíli, připojujeme vás na server {ip}");
                    Connect(ip);
                }
                else MessageBox.Show("Špatná IP adresa");
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                showBalloon("Připojování", "Počkejte chvíli, připojujeme vás na server");
                Connect("ip");
            });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseLocation = new Point(-e.X, -e.Y);
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePose = Control.MousePosition;
                mousePose.Offset(mouseLocation.X, mouseLocation.Y);
                Location = mousePose;
            }
        }

        private void showBalloon(string title, string body)
        {
            NotifyIcon notifyIcon1 = new NotifyIcon();
            notifyIcon1.Visible = true;
            notifyIcon1.Icon = SystemIcons.Information;

            if (title != null)
            {
                notifyIcon1.BalloonTipTitle = title;
            }

            if (body != null)
            {
                notifyIcon1.BalloonTipText = body;
            }

            notifyIcon1.ShowBalloonTip(2000);
        }
    }
}
