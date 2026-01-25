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

      
}