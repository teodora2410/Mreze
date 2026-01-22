using System;

namespace Common.Modeli
{
    [Serializable]
    public class ZahtevZaRezervacijuApartmana
    {
        public int BrojGostiju { get; set; } = 0;
        public int Klasa { get; set; } = 0;
        public int BrojNoci { get; set; } = 0;

        public ZahtevZaRezervacijuApartmana() { }

        public ZahtevZaRezervacijuApartmana(int brojGostiju, int klasa, int brojNoci)
        {
            BrojGostiju = brojGostiju;
            Klasa = klasa;
            BrojNoci = brojNoci;
        }
    }
}
