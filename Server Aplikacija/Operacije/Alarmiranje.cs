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

 
       
}