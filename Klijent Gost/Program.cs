using System;
using System.Net;
using System.Net.Sockets;

namespace Klijent_Gost
{
    public class Program
    {
        static void Main()
        {
            Console.WriteLine("Dobrodosli u hotelski kompleks!");

            Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                tcpSocket.Connect(new IPEndPoint(IPAddress.Loopback, 9000));
                Console.WriteLine("Povezano sa serverom.");
            }
            catch
            {
                Console.WriteLine("Server nije dostupan!");
                Console.ReadKey();
                return;
            }

            Prijavljivanje kontroler = new Prijavljivanje(tcpSocket);
            kontroler.Pokreni();

            tcpSocket.Close();
            Console.WriteLine("\nPritisnite bilo koji taster za izlaz...");
            Console.ReadKey();
        }
    }
}