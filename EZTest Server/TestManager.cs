using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EZTest_Server
{
    class TestManager
    {
        public List<Test> tests;

        public TestManager()
        {
            tests = new List<Test>();
        }

        public void addTest(Test test)
        {
            tests.Add(test);
            Console.WriteLine(tests.Count);
        }

        public void updateTestText(Test test, string text)
        {
            string testName = text.Split('/')[1];
            string id = text.Split('/')[2];
            string changedText = text.Split('/')[3];
            bool found = false;

            // textbox structure: changeText/ID/text

            //Console.WriteLine("Test id " + id);

            if(test.textBoxes.Count > 0)
            {
                for (int i = 0; i < test.textBoxes.Count; i++)
                {
                    if (test.textBoxes[i].Split('/')[1] == id)
                    {
                        test.textBoxes[i] = $"changeText/{id}/{changedText}";
                        found = true;
                    }
                }

                if(!found)
                    test.textBoxes.Add($"changeText/{id}/{changedText}");
                //Console.WriteLine($"size {test.textBoxes.Count}");
            } else
            {
                test.textBoxes.Add($"changeText/{id}/{changedText}");
                //Console.WriteLine("Adding");
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


        public Test getTest(string name, string quest)
        {
            Console.WriteLine(quest);
            Test test = new Test(name, Int32.Parse(quest));
            foreach (var item in tests)
            {
                if (item.name == test.name && item.answerSize == test.answerSize)
                {
                    //Console.WriteLine("Yes Contains");
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
                    return item;
            }
            return null;
        }

        public void sendAllTests()
        {
            if (tests.Count > 0)
            {
                foreach (var test in tests)
                {
                    Program.SendMessage($"addTest/{test.name}");
                }
            }
        }
    }
}
