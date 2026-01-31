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

        public void RunPollingUDP()
        {
            if (!udpSocket.Poll(0, SelectMode.SelectRead))
                return;

            EndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            byte[] buffer = new byte[2048];

            try
            {
                int bytes = udpSocket.ReceiveFrom(buffer, ref ep);
                if (bytes == 0)
                    return;

                byte[] data = new byte[bytes];
                Array.Copy(buffer, data, bytes);

                try
                {
                    PotvrdaOUradjenomZadatku potvrda = MemorySerializer.Deserialize<PotvrdaOUradjenomZadatku>(data);
                    ObradaPotvrdeOUradjenomZadatku(potvrda);
                    return;
                }
                catch { }

                try
                {
                    ZahtevKlijenta zahtev = MemorySerializer.Deserialize<ZahtevKlijenta>(data);
                    if (!osobljeUDPKlijenti.Any(c => c.ToString() == ep.ToString()))
                    {
                        osobljeUDPKlijenti.Add(ep);
                        Console.WriteLine($"[UDP] Osoblje registrovano: {ep}");
                    }
                }
                catch
                {
                    Console.WriteLine($"[UDP] Primljen nepoznat paket od {ep}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Greška UDP] {ex.Message}");
            }
        }

        public void ObradaPotvrdeOUradjenomZadatku(PotvrdaOUradjenomZadatku potvrda)
        {
            Apartman a = apartmani.Find(x => x.Id == potvrda.ApartmanId);
            if (a == null)
            {
                Console.WriteLine($"[Greška] Apartman ID {potvrda.ApartmanId} ne postoji");
                return;
            }

            if (!potvrda.Uspesno)
            {
                Console.WriteLine($"[Potvrda] Zadatak {potvrda.Tip} NIJE uspešno završen za apartman {a.BrojApartmana}");
                return;
            }

            Console.WriteLine($"[Potvrda] Zadatak {potvrda.Tip} završen za apartman {a.BrojApartmana}");

        }
    }
}