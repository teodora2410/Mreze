using System;

namespace Common
{
    [Serializable]
    public class Osoblje
    {
        public int Id { get; set; } = 0;
        public string Ime { get; set; } = "";
        public string Prezime { get; set; } = "";
        public string Pol { get; set; } = "";
        public string Funkcija { get; set; } = "";

        public Osoblje() { }

        public Osoblje(int id, string ime, string prezime, string pol, string funkcija)
        {
            Id = id;
            Ime = ime;
            Prezime = prezime;
            Pol = pol;
            Funkcija = funkcija;
        }
    }
}
