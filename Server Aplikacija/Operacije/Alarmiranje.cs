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

     
    }
}