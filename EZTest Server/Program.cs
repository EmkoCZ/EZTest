using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EZTest_Server
{
    class Program
    {
        private static List<User> onlineUsers = new List<User>();
        private static TestManager testManager = new TestManager();
        private static NetworkStream netStream;

        static void Main(string[] args)
        {
            Console.Title = "Server";
            TcpListener listener = new TcpListener(IPAddress.Any, 3559);
            listener.Start();

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Task.Run(() => AddUser(client.GetStream()));

                NetworkStream stream = client.GetStream();
                netStream = stream;
                Task.Run(() =>
                {
                    while (true)
                    {
                        if (stream.DataAvailable)
                        {
                            byte[] receivedBytes = ReadToEnd(stream);
                            string streamData = Encoding.UTF8.GetString(receivedBytes);

                            // change?/name/object
                            // changeText/LolTest/5/LOL

                            if (streamData.Contains("createTextBoxes"))
                            {
                                int count = Int32.Parse(streamData.Split('/')[1]);
                                SendMessage($"createTextBoxes/{count}", stream);
                                Console.WriteLine($"Incoming Data: createTextBoxes/{count}");
                            }
                            if (streamData.Contains("changeText"))
                            {
                                string testName = streamData.Split('/')[0];
                                string id = streamData.Split('/')[2];
                                string text = streamData.Split('/')[3];
                                Console.WriteLine($"Incoming Data: changeText/{testName}/{id}/{text}");
                                SendMessage($"changeText/{testName}/{id}/{text}", stream);

                                Test test = testManager.getTest(testName);
                                //testManager.updateTestText(test, streamData);
                                testManager.updateTestText2(test, $"changeText/{id}/{text}");
                            }
                            if (streamData.Contains("disconect"))
                            {
                                RemUser(stream);
                                Console.WriteLine("Client Disconnected");
                            }
                            if (streamData.Contains("checkBox"))
                            {
                                string id = streamData.Split('/')[1];
                                string check = streamData.Split('/')[2];
                                Console.WriteLine($"checkBox/{id}/{check}");
                                SendMessage($"checkBox/{id}/{check}", stream);
                            }
                            if (streamData.Contains("createTest"))
                            {
                                // createData/questions/name
                                string name = streamData.Split('/')[2];
                                string questions = streamData.Split('/')[1];

                                var test = new Test(name, stream, Int32.Parse(questions));
                                testManager.addTest(test);
                                SendMessage($"addTest/{name}/{questions}", stream);
                                Console.WriteLine("Incoming Data: create test");
                            }
                            if (streamData.Contains("getTest"))
                            {
                                string questions = streamData.Split('/')[2];
                                string name = streamData.Split('/')[1];
                                //Console.WriteLine("get test: " + streamData);

                                Test test = testManager.getTest(name, questions);
                                string dataToSend = "";
                                if(test.textBoxes.Count > 0)
                                {
                                    foreach (var item in test.textBoxes)
                                    {
                                        dataToSend += $"testQuestions/{test.name}/{item}:";
                                        Console.WriteLine("Sending answer " + $"testQuestions/{test.name}/{item}");
                                    }
                                    double b = dataToSend.Length * 2;
                                    double kb = Math.Round((b / 1024), 2);
                                    //Console.WriteLine($"{kb}KB");
                                    SendMessage(dataToSend, stream);
                                }
                            }
                            if (streamData.Contains("getAllTests"))
                            {
                                if(testManager.tests.Count > 0)
                                {
                                    string dataToSend = "";
                                    foreach (var item in testManager.tests)
                                    {
                                        dataToSend += $"addAllTest/{item.name}/{item.answerSize}:";
                                       
                                    }
                                    SendMessage($"{dataToSend}");
                                }
                            }
                        }
                        else Thread.Sleep(1);
                    }
                });


            }
        }

        private static void AddUser(NetworkStream stream)
        {
            User newUser = new User();
            newUser.stream = stream;

            onlineUsers.Add(newUser);
            Console.WriteLine("Add user");
        }

        private static void RemUser(NetworkStream stream)
        {
            try
            {
                foreach (User user in onlineUsers)
                {
                    if (user.stream == stream)
                    {
                        onlineUsers.Remove(user);
                    }
                }
            }
            catch (Exception)
            {
            }

        }

        public static void SendMessage(string data, NetworkStream stream)
        {
            int clientID = 0;
            try
            {
                //Console.WriteLine("online " + onlineUsers.Count);
                foreach (User user in onlineUsers)
                {
                    if (user.stream != null && user.stream != stream && !data.Contains("addAllTest") && !data.Contains("testQuestions"))
                    {
                        //Console.WriteLine("send 1 " + data);
                        Write(user.stream, Encoding.UTF8.GetBytes(data));
                        clientID++;
                        //if(user.stream != stream)
                        //    Console.WriteLine("Not same user");

                    } else if(user.stream != null && user.stream == stream && data.Contains("addAllTest"))
                    {
                        //Console.WriteLine("send 2 " + data);
                        Write(user.stream, Encoding.UTF8.GetBytes(data));

                        clientID++;
                    }
                    else if (user.stream != null && user.stream == stream && data.Contains("testQuestions"))
                    {
                        //Console.WriteLine("send 3 " + data);
                        Write(user.stream, Encoding.UTF8.GetBytes(data));

                        clientID++;
                    }// else if(data.Contains("testQuestions"))
                    //{
                    //    Console.WriteLine($"Else null: {user.stream!=null}| same user: {user.stream == stream}");
                    //}
                    //clientID++;
                }
            }
            catch (Exception)
            {
                onlineUsers.RemoveAt(clientID);
                //Console.WriteLine("Exception");
            }
        }

        public static void SendMessage(string data)
        {
            int clientID = 0;
            NetworkStream stream = netStream;
            try
            {
                foreach (User user in onlineUsers)
                {
                    if (user.stream != null && user.stream != stream && !data.Contains("addAllTest") && !data.Contains("testQuestions"))
                    {
                        Write(user.stream, Encoding.UTF8.GetBytes(data));
                        clientID++;
                        //if(user.stream != stream)
                        //    Console.WriteLine("Not same user");
                    }
                    else if (user.stream != null && user.stream == stream && data.Contains("addAllTest") || user.stream != null && user.stream == stream &&  data.Contains("testQuestions"))
                    {
                        Write(user.stream, Encoding.UTF8.GetBytes(data));
                        clientID++;
                    }
                }
            }
            catch (Exception)
            {
                onlineUsers.RemoveAt(clientID);
            }
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

    }
}
