using Common;
using Common.Enumeracije;
using Common.Modeli;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Klijent_Gost
{
    public class Rezervacija
    {
        Socket tcpSocket;

        public Rezervacija(Socket socket)
        {
            tcpSocket = socket;
        }

        public void IzvrsiRezervaciju()
        {
            ZahtevKlijenta reqStatus = new ZahtevKlijenta
            {
                Tip = TipZahteva.StatusSobe,
                Payload = null
            };

            tcpSocket.Send(MemorySerializer.Serialize(reqStatus));

            byte[] buffer = new byte[8192];
            int bytes = tcpSocket.Receive(buffer);
            byte[] data = new byte[bytes];
            Array.Copy(buffer, data, bytes);

            List<Apartman> apartmani = MemorySerializer.Deserialize<List<Apartman>>(data);

            if (apartmani.Count == 0)
            {
                Console.WriteLine("\nNema dostupnih apartmana.");
                return;
            }

            PrikaziApartmane(apartmani);

            Console.Write("\nREZERVACIJA SOBE");
            Console.Write("\nUnesite zeljenu klasu sobe (1-3): ");
            if (!int.TryParse(Console.ReadLine(), out int klasa) || klasa < 1 || klasa > 3)
            {
                Console.WriteLine("Nevalidna klasa.");
                return;
            }

            Console.Write("Unesite broj gostiju: ");
            if (!int.TryParse(Console.ReadLine(), out int brojGostiju) || brojGostiju < 1)
            {
                Console.WriteLine("Nevalidan broj gostiju.");
                return;
            }

            Console.Write("Unesite broj noci boravka: ");
            if (!int.TryParse(Console.ReadLine(), out int brojNoci) || brojNoci < 1)
            {
                Console.WriteLine("Nevalidan broj noci.");
                return;
            }

            ZahtevZaRezervacijuApartmana rezervacija = new ZahtevZaRezervacijuApartmana
            {
                BrojGostiju = brojGostiju,
                Klasa = klasa,
                BrojNoci = brojNoci
            };

            ZahtevKlijenta zahtev = new ZahtevKlijenta
            {
                Tip = TipZahteva.Rezervacija,
                Payload = rezervacija
            };

            tcpSocket.Send(MemorySerializer.Serialize(zahtev));

            bytes = tcpSocket.Receive(buffer);
            data = new byte[bytes];
            Array.Copy(buffer, data, bytes);

            string odgovor = MemorySerializer.Deserialize<string>(data);
            Console.WriteLine($"\n{odgovor}");

            if (odgovor == null || !odgovor.StartsWith("Rezervacija potvrđena"))
            {
                Console.WriteLine("Rezervacija nije uspela.");
                return;
            }

            IDApartmana(odgovor);
            UnosDetaljaBoravka(brojGostiju, brojNoci);
        }

        private void PrikaziApartmane(List<Apartman> apartmani)
        {
            Console.WriteLine("\t\tDOSTUPNI APARTMANI");
            Console.WriteLine("|---------------------------------------------|");
            Console.WriteLine("| ID  | Broj  | Klasa | Kapacitet | Status    |");
            Console.WriteLine("|---------------------------------------------|");

            foreach (var a in apartmani)
            {
                Console.WriteLine($"| {a.Id,-3} | {a.BrojApartmana,-5} | {a.Klasa,-5} | {a.MaksimalanBrojGostiju,-9} | {a.Stanje,-9} |");
            }
        }

        private void IDApartmana(string odgovor)
        {
            string[] delovi = odgovor.Split(' ');
            for (int i = 0; i < delovi.Length; i++)
            {
                if (delovi[i] == "Apartman" && i + 2 < delovi.Length)
                {
                    string idStr = delovi[i + 2].TrimEnd(',');
                    if (int.TryParse(idStr, out int id))
                    {
                        StanjeGosta.RezervisanApartmanId = id;
                    }
                    break;
                }
            }
        }

        private void UnosDetaljaBoravka(int brojGostiju, int brojNoci)
        {
            StanjeGosta.PreostaloNocenja = brojNoci;

            List<Gost> gosti = new List<Gost>();
            for (int i = 0; i < brojGostiju; i++)
            {
                Console.WriteLine($"\nPodaci za gosta #{i + 1}");

                string ime;
                do
                {
                    Console.Write("Ime: ");
                    ime = Console.ReadLine()?.Trim() ?? "";
                    if (ime.Length < 2)
                        Console.WriteLine("Ime mora imati bar 2 slova.");
                }
                while (ime.Length < 2);

                string prezime;
                do
                {
                    Console.Write("Prezime: ");
                    prezime = Console.ReadLine()?.Trim() ?? "";
                    if (prezime.Length < 2)
                        Console.WriteLine("Prezime mora imati bar 2 slova.");
                }
                while (prezime.Length < 2);

                string pol;
                do
                {
                    Console.Write("Pol (M/Z): ");
                    pol = (Console.ReadLine() ?? "").Trim().ToUpper();
                    if (pol != "M" && pol != "Z")
                        Console.WriteLine("Pol mora biti 'M' ili 'Z'.");
                }
                while (pol != "M" && pol != "Z");

                DateTime datum;
                do
                {
                    Console.Write("Datum rodjenja (yyyy-MM-dd): ");
                    if (!DateTime.TryParse(Console.ReadLine(), out datum))
                    {
                        Console.WriteLine("Neispravan format datuma.");
                        continue;
                    }

                    if (datum > DateTime.Now.AddYears(-18))
                    {
                        Console.WriteLine("Gost mora imati najmanje 18 godina.");
                        datum = DateTime.MinValue;
                    }

                } while (datum == DateTime.MinValue);

                string pas;
                do
                {
                    Console.Write("Broj pasosa: ");
                    pas = Console.ReadLine()?.Trim() ?? "";
                    if (pas.Length < 6)
                        Console.WriteLine("Broj pasosa mora imati bar 6 karaktera.");
                }
                while (pas.Length < 6);

                gosti.Add(new Gost(0, ime, prezime, pol, datum, pas));
            }

            StanjeGosta.ImaRezervaciju = true;
            StanjeGosta.VremeKoriscenaUslugaHotela = DateTime.Now;

            Console.WriteLine("\nRezervacija uspesno kreirana!");
            Console.WriteLine($"Vas boravak pocinje! Preostalo nocenja: {StanjeGosta.PreostaloNocenja}");
            Thread.Sleep(2000);
        }
    }
}