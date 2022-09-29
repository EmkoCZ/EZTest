using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EZTest_Client
{
    class TestManager
    {
        public List<Test> tests;
        private Main main;
        public Test currentTest;

        public TestManager(Main main)
        {
            tests = new List<Test>();
            this.main = main;
        }

        public void addTest(Test test)
        {
            tests.Add(test);
        }

        public Test getTest(string name, string quest)
        {
            //Console.WriteLine(quest);
            //MessageBox.Show($"get test name {name}, get test size {quest}");
            Test test = new Test(name, Int32.Parse(quest));
            foreach (var item in tests)
            {
                if (item.name == test.name && item.answerSize == test.answerSize)
                {
                    return item;
                }

            }
            return null;
        }
        public Test getTest(string name)
        {
            foreach (var item in tests)
            {
                if (item.name == name)
                {
                    return item;
                }
                    
            }
            return null;
        }


        public void setCurrentTest(Test test)
        {
            this.currentTest = test;
        }

        public void updateTestText(Test test, string text)
        {
            //MessageBox.Show("updateTestText " + text);
            string testName = text.Split('/')[1];
            string id = text.Split('/')[2];
            string changedText = text.Split('/')[3];
            //MessageBox.Show($"updateTestText test info {testName} {id} {changedText}");

            // textbox structure: changeText/ID/text

            if (test.textBoxes.Count > 0)
            {
                for (int i = 0; i < test.textBoxes.Count; i++)
                {
                    if (test.textBoxes[i].Split('/')[1] == id)
                    {
                        test.textBoxes[i] = $"changeText/{id}/{changedText}";
                    }
                }
            }

            if (!test.textBoxes.Contains($"changeText/{id}/"))
            {
                test.textBoxes.Add($"changeText/{id}/{changedText}");
            }
        }

        public void updateTestText2(Test test, string text)
        {
            string id = text.Split('/')[1];
            string changedText = text.Split('/')[2];

            bool found = false;

            // textbox structure: changeText/ID/text

            //Console.WriteLine("Test id " + id);

            if (test.textBoxes.Count > 0)
            {
                for (int i = 0; i < test.textBoxes.Count; i++)
                {
                    if (test.textBoxes[i].Split('/')[1] == id)
                    {
                        test.textBoxes[i] = $"changeText/{id}/{changedText}";
                        found = true;
                    }
                }

                if (!found)
                    test.textBoxes.Add($"changeText/{id}/{changedText}");
                //Console.WriteLine($"size {test.textBoxes.Count}");
            }
            else
            {
                test.textBoxes.Add($"changeText/{id}/{changedText}");
                //Console.WriteLine("Adding");
            }
        }

        public void updateTextBoxes(Test test, Panel panel)
        {
            //MessageBox.Show("size " + test.answerSize);
            //MessageBox.Show("size2 " + test.textBoxes.Count);
            int curAns = 0;
            //Console.WriteLine(test.textBoxes[1]);
            for (int i = 1; i < test.textBoxes.Count + 1; i++)
            {
                Console.WriteLine(test.textBoxes[curAns]);
                //MessageBox.Show(test.textBoxes[i - 1]);
                Control ctrl = main.getTextBox(i, panel.Controls);
                (ctrl as TextBox).TextChanged -= main.Form_TextChangedEvent;
                main.Invoke(new Action(() => ctrl.Text = test.textBoxes[curAns].Split('/')[2]));
                (ctrl as TextBox).TextChanged += main.Form_TextChangedEvent;
                curAns++;
            }  
        }

        private static void Write(NetworkStream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }

        public int getTestsSize()
        {
            return tests.Count;
        }

        public void renderTests(Panel panel)
        {
            int location = 62;
            int s = 0;
            if(tests.Count > 0)
            {
                foreach (var test in tests)
                {
                    Panel testPanel = new Panel();
                    testPanel.Location = new Point(0, location);
                    testPanel.Size = new Size(81, 45);
                    testPanel.BackColor = Color.FromArgb(121, 121, 121);
                    testPanel.Name = $"test{s}/{test.name}/{test.answerSize}";
                    

                    Label name = new Label();
                    if(test.name.Length > 3)
                        name.Text = test.name.Substring(0,4);
                    else
                        name.Text = test.name.Substring(0, test.name.Length);
                    name.Font = new Font("Microsoft Sans Serif", 20.25f, FontStyle.Regular);
                    name.Location = new Point(5, 5);
                    name.Size = new Size(78, 33);
                    name.ForeColor = Color.White;
                    name.Name = $"test{s}/{test.name}/{test.answerSize}";

                    testPanel.Controls.Add(name);
                    panel.Controls.Add(testPanel);
                    location += 50;
                    s++;
                    main.addClickEvent2(testPanel.Controls);
                }
                main.addClickEvent(panel.Controls);
            }
        }
    }
}
