using Common;
using Common.Enumeracije;
using Common.Modeli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Server_Aplikacija.Operacije
{
    public class ObradaZahteva
    {
        Socket tcpListener, udpSocket;
        List<Socket> tcpClients;
        List<EndPoint> osobljeUDPKlijenti;
        List<Apartman> apartmani;
        Dictionary<Socket, RezervacijaInfo> aktivneRezervacije;

        public ObradaZahteva(Socket tcpListener, Socket udpSocket, List<Socket> tcpClients, List<EndPoint> osobljeUDPKlijenti, List<Apartman> apartmani, Dictionary<Socket, RezervacijaInfo> aktivneRezervacije)
        {
            this.tcpListener = tcpListener;
            this.udpSocket = udpSocket;
            this.tcpClients = tcpClients;
            this.osobljeUDPKlijenti = osobljeUDPKlijenti;
            this.apartmani = apartmani;
            this.aktivneRezervacije = aktivneRezervacije;
        }

        public void ObradaZahtevaKlijenta(Socket klijent, ZahtevKlijenta zahtev)
        {
            switch (zahtev.Tip)
            {
                case TipZahteva.StatusSobe:
                    List<Apartman> slobodni = apartmani.Where(a => a.Stanje == StanjeApartmana.Prazan).ToList();
                    byte[] slobodniApartmani = MemorySerializer.Serialize(slobodni);
                    klijent.Send(slobodniApartmani);
                    Console.WriteLine($"[Server] Poslat status soba klijentu ({slobodni.Count} slobodnih)");
                    break;

                default:
                    byte[] data = MemorySerializer.Serialize("Nepoznat tip zahteva");
                    klijent.Send(data);
                    break;
            }
        }
}
