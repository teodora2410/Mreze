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

    }
}