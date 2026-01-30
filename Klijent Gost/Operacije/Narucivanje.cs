using Common;
using Common.Enumeracije;
using System;
using System.Net.Sockets;
using System.Threading;

namespace Klijent_Gost
{
    public class Narucivanje
    {
        Socket tcpSocket;

        public Narucivanje(Socket socket)
        {
            tcpSocket = socket;
        }

        public void PosaljiNarudzbinu(string tip)
        {
            string narudzbina = "";

            if (tip == "hrana/pice")
            {
                Console.WriteLine("\nNARUDZBINA");
                Console.WriteLine("1. Dorucak");
                Console.WriteLine("2. Rucak");
                Console.WriteLine("3. Vecera");
                Console.WriteLine("4. Pice");
                Console.Write("Izaberite: ");
                string izbor = Console.ReadLine() ?? "";

                switch (izbor)
                {
                    case "1":
                        narudzbina = "Hrana - Dorucak";
                        break;

                    case "2":
                        narudzbina = "Hrana - Rucak";
                        break;

                    case "3":
                        narudzbina = "Hrana - Vecera";
                        break;

                    case "4":
                        narudzbina = "Pice";
                        break;

                    default:
                        narudzbina = "Hrana - Mesovita narudzbina";
                        break;
                }
            }
            else if (tip == "rekreacija")
            {
                Console.WriteLine("\nREKREACIJA");
                Console.WriteLine("1. Bazen");
                Console.WriteLine("2. Fitnes centar");
                Console.WriteLine("3. SPA & Wellness");
                Console.WriteLine("4. Tenis teren");
                Console.Write("Izaberite: ");
                string izbor = Console.ReadLine() ?? "";

                switch (izbor)
                {
                    case "1":
                        narudzbina = "Rekreacija - Bazen (2h)";
                        break;

                    case "2":
                        narudzbina = "Rekreacija - Fitnes (1h)";
                        break;

                    case "3":
                        narudzbina = "Rekreacija - SPA & Wellness (masaza)";
                        break;

                    case "4":
                        narudzbina = "Rekreacija - Tenis teren (1h)";
                        break;

                    default:
                        narudzbina = "Rekreacija - Opsta";
                        break;
                }
            }

            ZahtevKlijenta zahtev = new ZahtevKlijenta
            {
                Tip = TipZahteva.Narudzbina,
                Payload = narudzbina
            };

            tcpSocket.Send(MemorySerializer.Serialize(zahtev));
            Console.WriteLine($"\nNarudzbina '{narudzbina}' poslata serveru.");
            Console.WriteLine("Narudzbina ce biti dostavljena uskoro!");
            Thread.Sleep(1500);
        }
    }
}