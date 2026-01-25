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

        }
    }
}