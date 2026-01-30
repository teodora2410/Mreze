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

       
    }
}