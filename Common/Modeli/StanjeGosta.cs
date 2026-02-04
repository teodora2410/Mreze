using System;

namespace Common.Modeli
{
    public static class StanjeGosta
    {
        public static bool ImaRezervaciju { get; set; } = false;
        public static int RezervisanApartmanId { get; set; } = 0;
        public static int PreostaloNocenja { get; set; } = 0;
        public static DateTime VremeKoriscenaUslugaHotela { get; set; } = DateTime.Now;
    }
}
