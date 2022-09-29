using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EZTest_Server
{
    class Test
    {
        public string name;
        public NetworkStream owner;
        public List<string> textBoxes;
        public int answerSize;

        public Test(string name, NetworkStream owner, int answerSize)
        {
            this.name = name;
            this.owner = owner;
            textBoxes = new List<string>();
            this.answerSize = answerSize;
        }

        public Test(string name, int answerSize)
        {
            this.name = name;
            textBoxes = new List<string>();
            this.answerSize = answerSize;
        }
    }
}
