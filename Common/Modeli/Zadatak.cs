using Common.Enumeracije;
using System;
namespace Common
{
    [Serializable]
    public class Zadatak
    {
        public TipZadatka Tip { get; set; }
        public int ApartmanId { get; set; }

        public Zadatak() { }

        public Zadatak(TipZadatka tip, int apartmanId)
        {
            Tip = tip;
            ApartmanId = apartmanId;
        }
    }

    [Serializable]
    public class PotvrdaOUradjenomZadatku
    {
        public int ApartmanId { get; set; }
        public TipZadatka Tip { get; set; }
        public bool Uspesno { get; set; }

        public PotvrdaOUradjenomZadatku() { }

        public PotvrdaOUradjenomZadatku(int apartmanId, TipZadatka tip, bool uspesno)
        {
            ApartmanId = apartmanId;
            Tip = tip;
            Uspesno = uspesno;
        }
    }
}

