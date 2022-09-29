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
using System.Timers;

namespace EZTest_Client
{
    public partial class Main : Form
    {
        CreateTest createTestForm;
        static TCPConnection tcpConnection;
        private static System.Timers.Timer timer;
        private static string changingText;
        public Point mouseLocation;
        TestManager testManager;

        public Main()
        {
            InitializeComponent();
            testManager = new TestManager(this);
        }

        public Main(TCPConnection tcp)
            : this()
        {
            tcpConnection = tcp;
            Task.Run(() => {
                StartHandler(tcp.networkStream);
                });
        }

        private void StartHandler(NetworkStream stream)
        {
            getAllTests();
            while (true)
            {
                if (stream != null)
                {
                    if (stream.DataAvailable)
                    {
                        byte[] data = ReadToEnd(stream);
                        string dataString = Encoding.UTF8.GetString(data);

                        if (dataString.Contains("createTextBoxes"))
                        {
                            int count = Int32.Parse(dataString.Split('/')[1]);
                            asyncCreateQuestions(count);
                        }

                        if (dataString.Contains("changeText") && !dataString.Contains("testQuestions"))
                        {
                            //MessageBox.Show("changeText ");
                            string test = dataString.Split('/')[1];
                            string id = dataString.Split('/')[2];
                            string text = dataString.Split('/')[3];
                            //Console.WriteLine($"Change txt {id} {text}");
                            //MessageBox.Show($"Change test {test}, id {id}, text {text}");

                            //SendMessage($"changeText/{testName}/{id}/{text}", stream);
                            //updateTextBox(id, text);
                            updateTextBox(test, id, text);
                        }

                        if (dataString.Contains("checkBox"))
                        {
                            //MessageBox.Show("check");
                            string id = dataString.Split('/')[1];
                            string check = dataString.Split('/')[2];
                            updateCheckBox(id, check);
                        }

                        if (dataString.Contains("addTest"))
                        {
                            string name = dataString.Split('/')[1];
                            int quest = Int32.Parse(dataString.Split('/')[2]);
                            Console.WriteLine("Create test " + dataString);
                            Test test = new Test(name, quest);
                            testManager.addTest(test);
                            this.Invoke(new Action(() => testManager.renderTests(panel3)));
                        }

                        if (dataString.Contains("addAllTest"))
                        {
                            string[] tests = dataString.Split(':');

                            for (int i = 0; i < tests.Length - 1; i++)
                            {
                                string name = tests[i].Split('/')[1];
                                int quest = Int32.Parse(tests[i].Split('/')[2]);
                                Console.WriteLine("Create test " + dataString);
                                Test test = new Test(name, quest);
                                testManager.addTest(test);
                                this.Invoke(new Action(() => testManager.renderTests(panel3)));
                            }
                        }

                        if (dataString.Contains("testQuestions"))
                        {
                            //MessageBox.Show("here");
                            //MessageBox.Show("testQuestions " + dataString);
                            string[] answers = dataString.Split(':');
                            Test test = null;

                            for (int i = 0; i < answers.Length - 1; i++)
                            {
                                string name = answers[i].Split('/')[1];
                                string question = answers[i].Split('/')[3];
                                string text = answers[i].Split('/')[4];
                                //MessageBox.Show(name);

                                test = testManager.getTest(name);
                                if (test == null)
                                    MessageBox.Show("NULL");
                                testManager.updateTestText2(test, $"changeText/{question}/{text}");
                                //if (testManager.currentTest.name == test.name)
                                //{
                                //    updateTextBox(question, text);
                                //    MessageBox.Show("Here");
                                //}
                                    
                                //else MessageBox.Show("Not same test");
                                //foreach (var item2 in test.textBoxes)
                                //{
                                //    //MessageBox.Show("item " + item2);
                                //    //updateTextBox(item2.Split('/')[1], item2.Split('/')[2]);
                                //    updateTextBox(test.name, item2.Split('/')[1], item2.Split('/')[2]);
                                //}
                            }



                            //testQuestions/test/changeText/1/aaaa

                            //MessageBox.Show(question);

                            //testManager.getTest(name).textBoxes.
                            updateTextBoxes(test);
                        }
                        //Console.WriteLine(dataString);
                    }
                    else Thread.Sleep(1);
                }
            }
        }

        private void getAllTests()
        {
            Thread.Sleep(100);
            //this.Invoke(new Action(() => MessageBox.Show("Here")));
            Write(tcpConnection.networkStream, Encoding.UTF8.GetBytes("getAllTests"));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            createTestForm = new CreateTest(this);
            createTestForm.Show();
        }

        private void createTest(int count, string name)
        {
            Write(tcpConnection.networkStream, Encoding.UTF8.GetBytes($"createTest/{count}/{name}"));
            string testIcon = name.Substring(0,3);

            var test = new Test(name, tcpConnection.networkStream, count);
            testManager.addTest(test);
            testManager.renderTests(panel3);
            testManager.setCurrentTest(test);
        }

        private void createQuestionsFromClick(int count)
        {
            panel1.Controls.Clear();
            int controlCount = 1;
            int checkCount = 1;

            int pointY = 30;
            for (int i = 0; i < count; i++)
            {
                TextBox ot = new TextBox();
                ot.Location = new Point(80, pointY);
                ot.Width = 465;
                ot.Font = new Font("Gravity", 11, FontStyle.Regular);
                ot.Name = $"changeText/{controlCount}";
                ot.MaxLength = 300;

                Label lab = new Label();
                lab.Location = new Point(550, pointY);
                lab.Text = "Zabráno";
                lab.Font = new Font("Gravity", 11, FontStyle.Regular);
                lab.ForeColor = Color.White;

                Label lab2 = new Label();
                lab2.Location = new Point(20, pointY);
                lab2.Text = "Otázka:";
                lab2.Font = new Font("Gravity", 11, FontStyle.Regular);
                lab2.ForeColor = Color.White;

                CheckBox check = new CheckBox();
                check.Location = new Point(620, pointY);
                check.Name = $"checkBox/{checkCount}";

                pointY += 35;
                controlCount++;
                checkCount++;

                TextBox od = new TextBox();
                od.Location = new Point(110, pointY);
                od.Width = 465;
                od.Font = new Font("Gravity", 11, FontStyle.Regular);
                od.Name = $"changeText/{controlCount}";
                od.MaxLength = 300;

                controlCount++;

                Label lab3 = new Label();
                lab3.Location = new Point(35, pointY);
                lab3.Text = "Odpověd:";
                lab3.Font = new Font("Gravity", 11, FontStyle.Regular);
                lab3.ForeColor = Color.White;

                panel1.Controls.Add(ot);
                panel1.Controls.Add(od);
                panel1.Controls.Add(check);
                panel1.Controls.Add(lab);
                panel1.Controls.Add(lab2);
                panel1.Controls.Add(lab3);
                pointY += 60;
            }


            loadEventHandlers(panel1.Controls);
            loadCheckBoxEventHandler(panel1.Controls);
        }

        public void createQuestions(int count, string name)
        {

            panel1.Controls.Clear();
            int controlCount = 1;
            int checkCount = 1;

            int pointY = 30;
            for (int i = 0; i < count; i++)
            {
                TextBox ot = new TextBox();
                ot.Location = new Point(80, pointY);
                ot.Width = 465;
                ot.Font = new Font("Gravity", 11, FontStyle.Regular);
                ot.Name = $"changeText/{controlCount}";
                ot.MaxLength = 300;

                Label lab = new Label();
                lab.Location = new Point(550, pointY);
                lab.Text = "Zabráno";
                lab.Font = new Font("Gravity", 11, FontStyle.Regular);
                lab.ForeColor = Color.White;

                Label lab2 = new Label();
                lab2.Location = new Point(20, pointY);
                lab2.Text = "Otázka:";
                lab2.Font = new Font("Gravity", 11, FontStyle.Regular);
                lab2.ForeColor = Color.White;

                CheckBox check = new CheckBox();
                check.Location = new Point(620, pointY);
                check.Name = $"checkBox/{checkCount}";

                pointY += 35;
                controlCount++;
                checkCount++;

                TextBox od = new TextBox();
                od.Location = new Point(110, pointY);
                od.Width = 465;
                od.Font = new Font("Gravity", 11, FontStyle.Regular);
                od.Name = $"changeText/{controlCount}";
                od.MaxLength = 300;

                controlCount++;

                Label lab3 = new Label();
                lab3.Location = new Point(35, pointY);
                lab3.Text = "Odpověd:";
                lab3.Font = new Font("Gravity", 11, FontStyle.Regular);
                lab3.ForeColor = Color.White;

                panel1.Controls.Add(ot);
                panel1.Controls.Add(od);
                panel1.Controls.Add(check);
                panel1.Controls.Add(lab);
                panel1.Controls.Add(lab2);
                panel1.Controls.Add(lab3);
                pointY += 60;
            }

            
            loadEventHandlers(panel1.Controls);
            loadCheckBoxEventHandler(panel1.Controls);
            createTest(count, name);
        }

        public void asyncCreateQuestions(int count)
        {
            this.Invoke(new Action(() => panel1.Controls.Clear()));

            int pointY = 30;
            int controlCount = 1;
            int checkCount = 1;

            for (int i = 0; i < count; i++)
            {
                TextBox ot = new TextBox();
                ot.Location = new Point(80, pointY);
                ot.Width = 465;
                ot.Font = new Font("Gravity", 11, FontStyle.Regular);
                ot.Name = $"changeText/{controlCount}";
                ot.MaxLength = 300;

                Label lab = new Label();
                lab.Location = new Point(550, pointY);
                lab.Text = "Zabráno";
                lab.Font = new Font("Gravity", 11, FontStyle.Regular);
                lab.ForeColor = Color.White;

                Label lab2 = new Label();
                lab2.Location = new Point(20, pointY);
                lab2.Text = "Otázka:";
                lab2.Font = new Font("Gravity", 11, FontStyle.Regular);
                lab2.ForeColor = Color.White;

                CheckBox check = new CheckBox();
                check.Location = new Point(620, pointY);
                check.Name = $"checkBox/{checkCount}";

                pointY += 35;
                controlCount++;
                checkCount++;

                TextBox od = new TextBox();
                od.Location = new Point(110, pointY);
                od.Width = 465;
                od.Font = new Font("Gravity", 11, FontStyle.Regular);
                od.Name = $"changeText/{controlCount}";
                od.MaxLength = 300;

                controlCount++;

                Label lab3 = new Label();
                lab3.Location = new Point(35, pointY);
                lab3.Text = "Odpověd:";
                lab3.Font = new Font("Gravity", 11, FontStyle.Regular);
                lab3.ForeColor = Color.White;

                this.Invoke(new Action(() => panel1.Controls.Add(ot)));
                this.Invoke(new Action(() => panel1.Controls.Add(od)));
                this.Invoke(new Action(() => panel1.Controls.Add(check)));
                this.Invoke(new Action(() => panel1.Controls.Add(lab)));
                this.Invoke(new Action(() => panel1.Controls.Add(lab2)));
                this.Invoke(new Action(() => panel1.Controls.Add(lab3)));
                pointY += 60;
            }
            loadEventHandlers(panel1.Controls);
            loadCheckBoxEventHandler(panel1.Controls);
        }

        [Obsolete("Pre-Test update")]
        public void updateTextBox(string num, string text)
        {
            int id = Int32.Parse(num);
            Console.WriteLine(id);

            Control ctrl = getTextBox(id, panel1.Controls);
            (ctrl as TextBox).TextChanged -= Form_TextChangedEvent;
            this.Invoke(new Action(() => ctrl.Text = text));
            (ctrl as TextBox).TextChanged += Form_TextChangedEvent;
            
        }

        private void updateTextBoxes(Test test)
        {
            //MessageBox.Show("size " + test.answerSize);
            MessageBox.Show("size2 " + test.textBoxes.Count);
            int curAns = 0;
            //Console.WriteLine(test.textBoxes[1]);
            for (int i = 1; i < test.textBoxes.Count + 1; i++)
            {
                Console.WriteLine(test.textBoxes[curAns]);
                MessageBox.Show(test.textBoxes[i - 1]);
                Control ctrl = getTextBox(i, panel1.Controls);
                (ctrl as TextBox).TextChanged -= Form_TextChangedEvent;
                Invoke(new Action(() => ctrl.Text = test.textBoxes[curAns].Split('/')[2]));
                (ctrl as TextBox).TextChanged += Form_TextChangedEvent;
                curAns++;
            }
        }

        public void updateTextBox(string test, string num, string text)
        {
            //MessageBox.Show("here");
            int id = Int32.Parse(num);
            Console.WriteLine(id);

            var testIns = testManager.getTest(test);
            if (testManager.currentTest != null)
            {
                testManager.updateTestText(testIns, $"test/{test}/{num}/{text}");

                if (testManager.currentTest.name == test)
                {
                    //MessageBox.Show("cur");
                    Console.WriteLine("CUR");
                    Control ctrl = getTextBox(id, panel1.Controls);
                    (ctrl as TextBox).TextChanged -= Form_TextChangedEvent;
                    this.Invoke(new Action(() => ctrl.Text = text));
                    (ctrl as TextBox).TextChanged += Form_TextChangedEvent;
                }
                //else MessageBox.Show("Not cur");
            }

        }

        public void updateCheckBox(string num, string check)
        {
            int id = Int32.Parse(num);
            bool checkState = Boolean.Parse(check);
            //MessageBox.Show(checkState.ToString());

            Control ctrl = getCheckBox(id, panel1.Controls);
            (ctrl as CheckBox).CheckedChanged -= checkBox1_CheckedChanged;
            this.Invoke(new Action(() => (ctrl as CheckBox).Checked = checkState));
            (ctrl as CheckBox).CheckedChanged += checkBox1_CheckedChanged;

        }

        private void startTimer()
        {
            timer = new System.Timers.Timer(100);
            timer.Elapsed += setChangeTextData;
            timer.AutoReset = false;
            timer.Enabled = true;
        }

        private static void setChangeTextData(Object source, ElapsedEventArgs e)
        {
            Write(tcpConnection.networkStream, Encoding.UTF8.GetBytes(changingText));
        }

        public Control getTextBox(int id, Control.ControlCollection controls)
        {
            int loop = 0;
            for (int i = 0; i < controls.Count; i++)
            {
                Control ctrl = controls[i];
                if ((ctrl as TextBox) != null)
                {
                    loop++;
                    if (id == loop) return ctrl;
                }
            }

            return null;
        }

        public Control getCheckBox(int id, Control.ControlCollection controls)
        {
            int loop = 0;
            for (int i = 0; i < controls.Count; i++)
            {
                Control ctrl = controls[i];
                if ((ctrl as CheckBox) != null)
                {
                    loop++;
                    if (id == loop) return ctrl;
                }
            }

            return null;
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Write(tcpConnection.networkStream, Encoding.UTF8.GetBytes("disconect"));
            Application.Exit();
        }

        private static byte[] ReadToEnd(NetworkStream stream)
        {
            List<byte> receivedBytes = new List<byte>();

            while (stream.DataAvailable)
            {
                byte[] buffer = new byte[1024];
                stream.Read(buffer, 0, buffer.Length);
                receivedBytes.AddRange(buffer);
            }

            receivedBytes.RemoveAll(b => b == 0);
            return receivedBytes.ToArray();
        }

        private static void Write(NetworkStream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.O)
            {
                tcpConnection.networkStream.Write(Encoding.UTF8.GetBytes("leng"), 0, Encoding.UTF8.GetBytes("leng").Length);
            }
        }

        private void loadCheckBoxEventHandler(System.Windows.Forms.Control.ControlCollection controls)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                Control ctrl = controls[i];

                if((ctrl as CheckBox) != null)
                {
                    (ctrl as CheckBox).CheckedChanged += checkBox1_CheckedChanged;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //MessageBox.Show((sender as CheckBox).Checked.ToString());
            Write(tcpConnection.networkStream, Encoding.UTF8.GetBytes($"{(sender as CheckBox).Name}/{(sender as CheckBox).Checked}"));
        }

        private void loadEventHandlers(System.Windows.Forms.Control.ControlCollection controls)
        {
            List<int> ids = new List<int>();

            for (int i = 0; i < controls.Count; i++)
            {
                Control ctrl = controls[i];

                if ((ctrl as TextBox) != null)
                {
                    (ctrl as TextBox).TextChanged += Form_TextChangedEvent;
                }
            }
        }

        public void addClickEvent(System.Windows.Forms.Control.ControlCollection controls)
        {
            List<int> ids = new List<int>();

            for (int i = 0; i < controls.Count; i++)
            {
                Control ctrl = controls[i];

                if ((ctrl as Panel) != null)
                {
                    (ctrl as Panel).Click += Form_PanelClick;
                }
            }
        }

        public void addClickEvent2(System.Windows.Forms.Control.ControlCollection controls)
        {
            List<int> ids = new List<int>();

            for (int i = 0; i < controls.Count; i++)
            {
                Control ctrl = controls[i];

                if ((ctrl as Label) != null)
                {
                    (ctrl as Label).Click += Form_LabelClick;
                }
            }
        }

        public void Form_PanelClick(object sender, EventArgs e)
        {
            string panel = (sender as Panel).Name;
            Test test = testManager.getTest(panel.Split('/')[1]);
            if (test == null)
            {
                MessageBox.Show("NULL");
                MessageBox.Show("count " + testManager.tests.Count);
            }
            testManager.setCurrentTest(test);
            Write(tcpConnection.networkStream, Encoding.UTF8.GetBytes($"getTest/{panel.Split('/')[1]}/{panel.Split('/')[2]}"));
            createQuestionsFromClick(Int32.Parse(panel.Split('/')[2]));
            if(testManager.currentTest == null)
            {
                MessageBox.Show("NULL TEST");
            }
        }

        public void Form_LabelClick(object sender, EventArgs e)
        {
            string label = (sender as Label).Name;
            Test test = testManager.getTest(label.Split('/')[1]);
            if(test == null)
            {
                MessageBox.Show("NULL");
                MessageBox.Show("count " + testManager.tests.Count);
            }
            testManager.setCurrentTest(test);
            Write(tcpConnection.networkStream, Encoding.UTF8.GetBytes($"getTest/{label.Split('/')[1]}/{label.Split('/')[2]}"));
            createQuestionsFromClick(Int32.Parse(label.Split('/')[2]));
            if (testManager.currentTest == null)
            {
                MessageBox.Show("NULL TEST");
            }
        }

        public void Form_TextChangedEvent(object sender, EventArgs e)
        {
            //MessageBox.Show($"{(sender as TextBox).Name}/{(sender as TextBox).Text}");
            //Console.WriteLine(testManager.currentTest.name);
            changingText = $"{testManager.currentTest.name}/{(sender as TextBox).Name}/{(sender as TextBox).Text}";
            if (timer != null)
                timer.Stop();
            startTimer();
        }


        private void Main_Resize(object sender, EventArgs e)
        {
            //panel1.Size = this.Size;
            //panel1.AutoScroll = true;
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
    }
}
