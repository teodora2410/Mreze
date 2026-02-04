using Common;
using Common.Enumeracije;
using Common.Modeli;
using Server_Aplikacija.Operacije;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Server_Aplikacija
{
    public class Program
    {
        static void Main(string[] args)
        {
            Socket tcpListener = null, udpSocket = null;
            List<Socket> tcpClients = new List<Socket>();
            List<EndPoint> osobljeUDPKlijenti = new List<EndPoint>();
            List<Apartman> apartmani = new List<Apartman>();
            Dictionary<Socket, RezervacijaInfo> aktivneRezervacije = new Dictionary<Socket, RezervacijaInfo>();
            PollingTCP tcp;
            PollingUDP udp;

            for (int i = 1; i <= 10; i++)
                apartmani.Add(new Apartman(i, 100 + i, i, i % 3 + 1, i, 0, StanjeApartmana.Prazan, StanjeAlarma.Normalno, new List<int>(), false));

            // TCP
            tcpListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpListener.Bind(new IPEndPoint(IPAddress.Any, 9000));
            tcpListener.Listen(10);
            tcpListener.Blocking = false;

            // UDP
            udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpSocket.Bind(new IPEndPoint(IPAddress.Any, 9001));
            udpSocket.Blocking = false;

            Console.WriteLine($"[Server]: Inicijalizovano {apartmani.Count} apartmana. Port TCP = 9000, Port UDP = 9001");

            ObradaZahteva obradaZahteva = new ObradaZahteva(tcpListener, udpSocket, tcpClients, osobljeUDPKlijenti, apartmani, aktivneRezervacije);

            tcp = new PollingTCP(tcpListener, tcpClients, aktivneRezervacije, obradaZahteva, udpSocket, osobljeUDPKlijenti);
            udp = new PollingUDP(udpSocket, osobljeUDPKlijenti, apartmani);

            while (true)
            {
                tcp.RunPollingTCP();
                udp.RunPollingUDP();
            }
        }
    }
}