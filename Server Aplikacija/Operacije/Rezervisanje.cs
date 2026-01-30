using Common;
using Common.Enumeracije;
using Common.Modeli;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server_Aplikacija.Operacije
{
    public class Rezervisanje
    {
        List<Apartman> apartmani;
        Dictionary<Socket, RezervacijaInfo> aktivneRezervacije;

        public Rezervisanje(List<Apartman> apartmani, Dictionary<Socket, RezervacijaInfo> aktivneRezervacije)
        {
            this.apartmani = apartmani;
            this.aktivneRezervacije = aktivneRezervacije;
        }

        public void ObradaRezervacije(Socket klijent, object payload)
        {
            int brojGostiju = 0;
            int klasa = 0;
            int brojNoci = 1;

            if (payload is ZahtevZaRezervacijuApartmana zahtev)
            {
                brojGostiju = zahtev.BrojGostiju;
                klasa = zahtev.Klasa;
                brojNoci = zahtev.BrojNoci > 0 ? zahtev.BrojNoci : 1;
            }
            else if (payload is int broj)
            {
                brojGostiju = broj;
                klasa = 0;
                brojNoci = 1;
            }

            Apartman slobodan = null;
            if (klasa > 0)
                slobodan = apartmani.Find(a => a.Stanje == StanjeApartmana.Prazan && a.MaksimalanBrojGostiju >= brojGostiju && a.Klasa == klasa);
            else
                slobodan = apartmani.Find(a => a.Stanje == StanjeApartmana.Prazan && a.MaksimalanBrojGostiju >= brojGostiju);

            string odgovor;
            if (slobodan != null)
            {
                slobodan.TrenutniBrojGostiju = brojGostiju;
                slobodan.Stanje = StanjeApartmana.Zauzet;

                if (!aktivneRezervacije.ContainsKey(klijent))
                {
                    aktivneRezervacije[klijent] = new RezervacijaInfo();
                }

                aktivneRezervacije[klijent].Apartman = slobodan;
                aktivneRezervacije[klijent].ApartmanId = slobodan.Id;
                aktivneRezervacije[klijent].BrojNoci = brojNoci;

                odgovor = $"Rezervacija potvrđena: Apartman {slobodan.BrojApartmana} {slobodan.Id}, Klasa {slobodan.Klasa}, Broj noći: {brojNoci}";
                Console.WriteLine($"[Server] {odgovor}");
            }
            else
            {
                odgovor = "Nema slobodnih apartmana za zadate uslove";
                Console.WriteLine($"[Server] {odgovor}");
            }

            byte[] data = MemorySerializer.Serialize(odgovor);
            klijent.Send(data);
        }
    }
}