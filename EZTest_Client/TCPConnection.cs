using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace EZTest_Client
{
    public class TCPConnection
    {
        public TcpClient client { get; set; }
        public NetworkStream networkStream { get; set; }

        public TCPConnection(TcpClient client, NetworkStream networkStream)
        {
            this.client = client;
            this.networkStream = networkStream;
        }
    }
}
