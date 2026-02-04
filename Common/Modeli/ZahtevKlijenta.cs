using Common.Enumeracije;
using System;
namespace Common
{
    [Serializable]
    public class ZahtevKlijenta
    {
        public TipZahteva Tip { get; set; }
        public object Payload { get; set; } = new object();

        public ZahtevKlijenta() { }

        public ZahtevKlijenta(TipZahteva type, object payload)
        {
            Tip = type;
            Payload = payload;
        }
    }
}

