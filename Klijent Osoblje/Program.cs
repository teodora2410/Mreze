using Common;
using Common.Enumeracije;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Klijent_Osoblje
{
    public class Program
    {
        static void Main(string[] args)
        {
            Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpSocket.Blocking = false;

            EndPoint serverEP = new IPEndPoint(IPAddress.Loopback, 9001);
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

         
        }
    }
}