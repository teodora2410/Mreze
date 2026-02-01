using Common;
using Common.Enumeracije;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Server_Aplikacija.Operacije
{
    public class Alarmiranje
    {
        List<Apartman> apartmani;
        List<EndPoint> osobljeUDPKlijenti;
        Socket udpSocket;

        public Alarmiranje(List<Apartman> apartmani, List<EndPoint> osobljeUDPKlijenti, Socket udpSocket)
        {
            this.apartmani = apartmani;
            this.osobljeUDPKlijenti = osobljeUDPKlijenti;
            this.udpSocket = udpSocket;
        }

        public void ObradaAlarma(int apartmanId)
        {
            Apartman apartman = apartmani.Find(x => x.Id == apartmanId);
            if (apartman != null)
            {
                apartman.Alarm = StanjeAlarma.Aktivirano;
                Console.WriteLine($"[ALARM] Aktiviran u apartmanu {apartman.BrojApartmana}");
                PosaljiOsoblje(apartman, TipZadatka.Alarm);
            }
            else
            {
                Console.WriteLine($"[Greška] Apartman ID {apartmanId} ne postoji");
            }
        }

        public void PosaljiOsoblje(Apartman apartman, TipZadatka tipZadatka)
        {
            if (osobljeUDPKlijenti.Count == 0)
            {
                Console.WriteLine("[Server] Nema dostupnog osoblja!");
                return;
            }

            Zadatak zadatak = new Zadatak(tipZadatka, apartman.Id);
            byte[] data = MemorySerializer.Serialize(zadatak);

            foreach (var osoblje in osobljeUDPKlijenti)
                udpSocket.SendTo(data, osoblje);

            Console.WriteLine($"[Server] Zadatak {tipZadatka} poslat osoblju za apartman {apartman.BrojApartmana}");
        }

        public void ObradiPotvrdu(PotvrdaOUradjenomZadatku potvrda)
        {
            Apartman apartman = apartmani.Find(x => x.Id == potvrda.ApartmanId);

            if (apartman == null)
            {
                Console.WriteLine($"[Greška] Apartman ID {potvrda.ApartmanId} ne postoji");
                return;
            }

            if (!potvrda.Uspesno)
            {
                Console.WriteLine($"[Server] Zadatak za apartman {apartman.BrojApartmana} NIJE uspešno izvršen");
                return;
            }

            switch (potvrda.Tip)
            {
                case TipZadatka.Ciscenje:
                    apartman.Stanje = StanjeApartmana.Zauzet;
                    Console.WriteLine($"[Server] Apartman {apartman.BrojApartmana} - Čišćenje završeno, status: SPREMAN");
                    break;

                case TipZadatka.Minibar:
                    Console.WriteLine($"[Server] Apartman {apartman.BrojApartmana} - Minibar popunjen");
                    break;

                case TipZadatka.Alarm:
                    apartman.Alarm = StanjeAlarma.Normalno;
                    Console.WriteLine($"[Server] Apartman {apartman.BrojApartmana} - Alarm saniran i isključen");
                    break;
            }
        }
    }
}