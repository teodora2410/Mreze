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