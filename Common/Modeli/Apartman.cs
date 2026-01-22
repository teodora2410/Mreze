using Common.Enumeracije;
using System;
using System.Collections.Generic;

namespace Common
{
    [Serializable]
    public class Apartman
    {
        public int Id { get; set; } = 0;
        public int BrojApartmana { get; set; } = 0;
        public int Sprat { get; set; } = 0;
        public int Klasa { get; set; } = 0;
        public int MaksimalanBrojGostiju { get; set; } = 0;
        public int TrenutniBrojGostiju { get; set; } = 0;
        public StanjeApartmana Stanje { get; set; }
        public StanjeAlarma Alarm { get; set; }
        public bool MinibarPun { get; set; } = true;

        public List<int> Gosti { get; set; } = new List<int>();

        public Apartman() { }

        public Apartman(int id, int brojApartmana, int sprat, int klasa, int maksimalanBrojGostiju, int trenutniBrojGostiju, StanjeApartmana stanje, StanjeAlarma alarm, List<int> gosti, bool minibar)
        {
            Id = id;
            BrojApartmana = brojApartmana;
            Sprat = sprat;
            Klasa = klasa;
            MaksimalanBrojGostiju = maksimalanBrojGostiju;
            TrenutniBrojGostiju = trenutniBrojGostiju;
            Stanje = stanje;
            Alarm = alarm;
            Gosti = gosti;
            MinibarPun = minibar;
        }
    }
}
