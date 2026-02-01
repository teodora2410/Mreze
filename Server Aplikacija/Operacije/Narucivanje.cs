using Common.Modeli;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server_Aplikacija.Operacije
{
    public class Narucivanje
    {
        Dictionary<Socket, RezervacijaInfo> aktivneRezervacije;

        public Narucivanje(Dictionary<Socket, RezervacijaInfo> aktivneRezervacije)
        {
            this.aktivneRezervacije = aktivneRezervacije;
        }

        public void ObradaNarudzbine(Socket klijent, string tip)
        {
            if (!aktivneRezervacije.ContainsKey(klijent))
            {
                Console.WriteLine("[Greška] Klijent nema aktivnu rezervaciju");
                return;
            }

            var info = aktivneRezervacije[klijent];
            info.Narudzbine.Add(tip);

            Console.WriteLine($"[Server] Narudzbina '{tip}' zabelezena za apartman {info.Apartman.BrojApartmana}");
        }
    }
}