using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EZTest_Server
{
    class Uzivatel
    {
        public string getJmeno()
        {
            return "Pavel";
        }
    }


    class User
    {
        //public NetworkStream stream { get; set; }

        public User()
        {
            Uzivatel uziv = new Uzivatel();
            Console.WriteLine(uziv.getJmeno());
        }

    }
}
