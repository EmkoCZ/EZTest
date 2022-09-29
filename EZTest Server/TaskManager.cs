using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace EZTest_Server
{
    class TaskManager
    {
        List<User> clients;
        public TaskManager(List<User> clients)
        {
            this.clients = clients;
        }

        public void AddQuestions(int count)
        {
            //foreach (TcpClient cl in clients)
            //{
            //    NetworkStream clientsStream = cl.GetStream();
            //    clientsStream.Write(Encoding.UTF8.GetBytes($"createTextBoxes/{count}"), 0, Encoding.UTF8.GetBytes($"createTextBoxes/{count}").Length);
            //}
        }
    }
}
