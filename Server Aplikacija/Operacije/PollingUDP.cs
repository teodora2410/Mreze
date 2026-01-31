using Common;
using Common.Enumeracije;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Server_Aplikacija.Operacije
{
    public class PollingUDP
    {
        Socket udpSocket;
        List<EndPoint> osobljeUDPKlijenti;
        List<Apartman> apartmani;

        public PollingUDP(Socket udpSocket, List<EndPoint> osobljeUDPKlijenti, List<Apartman> apartmani)
        {
            this.udpSocket = udpSocket;
            this.osobljeUDPKlijenti = osobljeUDPKlijenti;
            this.apartmani = apartmani;
        }

     
    }
}