using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EZTest_Server
{
    class DataManager
    {
        public DataManager()
        {
            //CheckFiles();
        }

        public void DataManage(NetworkStream stream, string streamData)
        {
            string[] data = streamData.Split(':');

            string msg = data[2];
            // Data Structure: Name:Password:Register

            if (msg.Contains("register"))
            {
                string name = data[0];
                string pass = data[1];

                if (name != null || name != "" && pass != null || pass != "")
                {
                    CreateUser(name, pass);
                }
            }
            else if (msg.Contains("login"))
            {
                string name = data[0];
                string pass = data[1];

                if (name != null || name != "" && pass != null || pass != "")
                {
                    CheckUserLogin(name, pass);
                }
            }
        }

        public bool IsCommand(string msg)
        {
            if (msg.Contains("register"))
            {
                return true;
            }
            else
                return false;
        }

        private void CheckFiles()
        {
            if (!Directory.Exists("./data"))
            {
                Directory.CreateDirectory("./data");
            }

            if (!Directory.Exists("./data/users"))
            {
                Directory.CreateDirectory("./data/users");
            }

            //if (!File.Exists("./data/users.txt"))
            //{
            //    var file = File.Create("./data/users.txt");
            //    file.Close();
            //}
        }

        private void CreateUser(string name, string pass)
        {
            if (!File.Exists($"./data/users/{name}.txt"))
            {
                var file = File.Create($"./data/users/{name}.txt");
                file.Close();

                var path = $"./data/users/{name}.txt";

                string data = $"{name}:{pass}";
                File.WriteAllText(path, data);
            }
        }

        private void CheckUserLogin(string name, string pass)
        {
            if (!File.Exists($"./data/users/{name}.txt"))
            {

            }
        }
    }
}
