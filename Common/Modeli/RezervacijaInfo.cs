using System;
using System.Collections.Generic;

namespace Common.Modeli
{
    [Serializable]
    public class RezervacijaInfo
    {
        public Apartman Apartman { get; set; }
        public int ApartmanId { get; set; }
        public List<Gost> Gosti { get; set; } = new List<Gost>();
        public List<string> Narudzbine { get; set; } = new List<string>();
        public int BrojNoci { get; set; }

        public RezervacijaInfo() { }

        public RezervacijaInfo(Apartman apartman, int apartmanId, List<Gost> gosti, List<string> narudzbine, int brojNoci)
        {
            Apartman = apartman;
            ApartmanId = apartmanId;
            Gosti = gosti;
            Narudzbine = narudzbine;
            BrojNoci = brojNoci;
        }
    }
}
