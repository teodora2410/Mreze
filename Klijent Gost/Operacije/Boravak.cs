using Common;
using Common.Enumeracije;
using Common.Modeli;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Klijent_Gost
{
    public class Boravak
    {
        Socket tcpSocket;
        Narucivanje narudzbina;

        public Boravak(Socket socket)
        {
            tcpSocket = socket;
            narudzbina = new Narucivanje(tcpSocket);
        }

        public void PrikaziMeniTokomBoravka()
        {
            TimeSpan protekloVreme = DateTime.Now - StanjeGosta.VremeKoriscenaUslugaHotela;
            if (protekloVreme.TotalSeconds >= 10)
            {
                StanjeGosta.PreostaloNocenja--;
                StanjeGosta.VremeKoriscenaUslugaHotela = DateTime.Now;

                if (StanjeGosta.PreostaloNocenja > 0)
                {
                    Console.WriteLine($"\nProsao je dan! Preostalo nocenja: {StanjeGosta.PreostaloNocenja}");
                }
                else
                {
                    Console.WriteLine($"\nVas boravak je zavrsen!");
                    return;
                }
            }

            Console.WriteLine("\nINFORMACIJE O BORAVKU");
            Console.WriteLine($"Preostalo nocenja: {StanjeGosta.PreostaloNocenja,-14}");
            Console.WriteLine("");
            Console.WriteLine("1. Narudzbina (hrana/pice)");
            Console.WriteLine("2. Rekreacija");
            Console.WriteLine("3. Aktivacija alarma");
            Console.Write("Izaberite opciju: ");
            string izbor = Console.ReadLine() ?? "";

            switch (izbor)
            {
                case "1":
                    narudzbina.PosaljiNarudzbinu("hrana/pice");
                    break;

                case "2":
                    narudzbina.PosaljiNarudzbinu("rekreacija");
                    break;

                case "3":
                    AktivirajAlarm();
                    break;

                default:
                    Console.WriteLine("\nNepoznata opcija.");
                    break;
            }
        }

      

        public void ZavrsetakBoravka()
        {
            Console.WriteLine("\nZAVRSETAK BORAVKA");
            Console.WriteLine("Cekanje racuna od servera...\n");

            try
            {
                ZahtevKlijenta zahtev = new ZahtevKlijenta
                {
                    Tip = TipZahteva.KrajBoravka,
                    Payload = null
                };
                tcpSocket.Send(MemorySerializer.Serialize(zahtev));

                byte[] buffer = new byte[8192];
                tcpSocket.ReceiveTimeout = 5000;

                int bytes = tcpSocket.Receive(buffer);
                if (bytes > 0)
                {
                    byte[] data = new byte[bytes];
                    Array.Copy(buffer, data, bytes);
                    string racunStr = MemorySerializer.Deserialize<string>(data);
                    Console.WriteLine(racunStr);
                    PrikaziRacun(racunStr);
                }
                else
                {
                    Console.WriteLine("Greska: Racun nije primljen.");
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Greska pri primanju racuna: {ex.Message}");
            }

            Thread.Sleep(2000);
        }

 
    }
}