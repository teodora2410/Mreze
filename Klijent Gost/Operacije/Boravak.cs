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

        private void AktivirajAlarm()
        {
            Console.WriteLine("AKTIVACIJA ALARMA");
            Console.WriteLine("Razlozi za alarm:");
            Console.WriteLine("  - Problem sa klimom/grejanjem");
            Console.WriteLine("  - Kvar u kupatilu");
            Console.WriteLine("  - Problem sa TV/internetom");
            Console.WriteLine("  - Ostalo");

            ZahtevKlijenta zahtev = new ZahtevKlijenta
            {
                Tip = TipZahteva.Alarm,
                Payload = StanjeGosta.RezervisanApartmanId
            };

            tcpSocket.Send(MemorySerializer.Serialize(zahtev));
            Console.WriteLine("\nALARM AKTIVIRAN za Vas apartman!");
            Console.WriteLine("Osoblje dolazi!");
            Thread.Sleep(2000);
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

        private void PrikaziRacun(string racunStr)
        {
            string[] delovi = racunStr.Split('|');
            if (delovi.Length < 4)
            {
                Console.WriteLine("Greska pri obradi racuna.");
                return;
            }

            double osnovnaCena = double.Parse(delovi[0]);
            double troskoviNarudzbina = double.Parse(delovi[1]);
            double ukupno = double.Parse(delovi[2]);
            string[] narudzbine = delovi[3].Split(',').Where(n => !string.IsNullOrEmpty(n)).ToArray();

            Console.WriteLine("RACUN");
            Console.WriteLine("------------------------------------");
            Console.WriteLine($"Smestaj:              {osnovnaCena,10:F2} KM");

            if (narudzbine.Length > 0)
            {
                Console.WriteLine("\nNarudzbine:");
                foreach (var narudzbina in narudzbine)
                {
                    Console.WriteLine($"  - {narudzbina}");
                }
                Console.WriteLine($"Troskovi narudzbina:  {troskoviNarudzbina,10:F2} KM");
            }

            Console.WriteLine("------------------------------------");
            Console.WriteLine($"UKUPNO:               {ukupno,10:F2} KM");
            Console.WriteLine("------------------------------------");

            PlatiRacun(ukupno);
        }

        private void PlatiRacun(double iznos)
        {
            Console.Write("\nUnesite broj kreditne kartice za placanje: ");
            string kartica = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(kartica) && kartica.Length >= 10)
            {
                Console.WriteLine("\nPlacanje u toku...");
                Thread.Sleep(1500);
                Console.WriteLine("Placanje potvrdeno!");
                Console.WriteLine("Hvala na poverenju! Dobrodosli opet!");
            }
            else
            {
                Console.WriteLine("\nNevalidan broj kartice - placanje otkazano.");
            }
        }
    }
}