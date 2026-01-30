using Common.Modeli;
using System;
using System.Net.Sockets;

namespace Klijent_Gost
{
    public class Prijavljivanje
    {
        Socket tcpSocket;
        Rezervacija rezervacija;
        Boravak boravak;

        public Prijavljivanje(Socket socket)
        {
            tcpSocket = socket;
            rezervacija = new Rezervacija(tcpSocket);
            boravak = new Boravak(tcpSocket);
        }

        public void Pokreni()
        {
            bool running = true;

            while (running)
            {
                if (StanjeGosta.ImaRezervaciju && StanjeGosta.PreostaloNocenja > 0)
                {
                    boravak.PrikaziMeniTokomBoravka();
                }
                else if (StanjeGosta.ImaRezervaciju && StanjeGosta.PreostaloNocenja == 0)
                {
                    boravak.ZavrsetakBoravka();
                    StanjeGosta.ImaRezervaciju = false;
                }
                else
                {
                    running = PrikaziGlavniMeni(rezervacija);
                }
            }
        }

        public bool PrikaziGlavniMeni(Rezervacija rezervacijaManager)
        {
            Console.WriteLine("\n1. Rezervacija sobe");
            Console.WriteLine("2. Odjava");
            Console.Write("Izaberite opciju: ");
            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    Console.Clear();
                    rezervacijaManager.IzvrsiRezervaciju();
                    return true;

                case "2":
                    Console.WriteLine("\nOdjava...");
                    return false;

                default:
                    Console.Clear();
                    Console.WriteLine("\nNepoznata opcija.");
                    return true;
            }
        }
    }
}