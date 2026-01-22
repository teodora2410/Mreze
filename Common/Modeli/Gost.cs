using System;

namespace Common
{
    [Serializable]
    public class Gost
    {
        public int Id { get; set; } = 0;
        public string Ime { get; set; } = "";
        public string Prezime { get; set; } = "";
        public string Pol { get; set; } = "";
        public DateTime DatumRodjenja { get; set; }
        public string BrojPasosa { get; set; } = "";

        public Gost() { }

        public Gost(int id, string ime, string prezime, string pol, DateTime datumRodjenja, string brojPasosa)
        {
            Id = id;
            Ime = ime;
            Prezime = prezime;
            Pol = pol;
            DatumRodjenja = datumRodjenja;
            BrojPasosa = brojPasosa;
        }
    }
}
