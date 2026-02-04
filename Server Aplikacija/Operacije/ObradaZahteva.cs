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

        Alarmiranje alarm;
        Narucivanje narucivanje;
        Rezervisanje rezervisanje;

        public ObradaZahteva(Socket tcpListener, Socket udpSocket, List<Socket> tcpClients, List<EndPoint> osobljeUDPKlijenti, List<Apartman> apartmani, Dictionary<Socket, RezervacijaInfo> aktivneRezervacije)
        {
            this.tcpListener = tcpListener;
            this.udpSocket = udpSocket;
            this.tcpClients = tcpClients;
            this.osobljeUDPKlijenti = osobljeUDPKlijenti;
            this.apartmani = apartmani;
            this.aktivneRezervacije = aktivneRezervacije;

            alarm = new Alarmiranje(apartmani, osobljeUDPKlijenti, udpSocket);
            narucivanje = new Narucivanje(aktivneRezervacije);
            rezervisanje = new Rezervisanje(apartmani, aktivneRezervacije);
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

                case TipZahteva.Rezervacija:
                    rezervisanje.ObradaRezervacije(klijent, zahtev.Payload);
                    break;

                case TipZahteva.Alarm:
                    alarm.ObradaAlarma((int)zahtev.Payload);
                    break;

                case TipZahteva.Narudzbina:
                    narucivanje.ObradaNarudzbine(klijent, (string)zahtev.Payload);
                    break;

                case TipZahteva.KrajBoravka:
                    PosaljiRacun(klijent);
                    break;

                default:
                    byte[] data = MemorySerializer.Serialize("Nepoznat tip zahteva");
                    klijent.Send(data);
                    break;
            }
        }

        private void PosaljiRacun(Socket klijent)
        {
            if (!aktivneRezervacije.ContainsKey(klijent))
            {
                Console.WriteLine("[Greška] Klijent nema aktivnu rezervaciju");
                return;
            }

            var info = aktivneRezervacije[klijent];

            double osnovnaCena = info.BrojNoci * 100.0;
            double troskoviNarudzbina = info.Narudzbine.Count * 50.0;
            double ukupno = osnovnaCena + troskoviNarudzbina;

            string racun = $"{osnovnaCena}|{troskoviNarudzbina}|{ukupno}|{string.Join(",", info.Narudzbine)}";

            try
            {
                byte[] data = MemorySerializer.Serialize(racun);
                klijent.Send(data);
                Console.WriteLine($"[Server] Račun poslat gostu - Ukupno: {ukupno:F2} KM");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Greška] Slanje računa: {ex.Message}");
                return;
            }

            if (info.Apartman != null)
            {
                info.Apartman.TrenutniBrojGostiju = 0;
                info.Apartman.Stanje = StanjeApartmana.PotrebnoCiscenje;
                info.Apartman.Gosti.Clear();
                Console.WriteLine($"[Server] Apartman {info.Apartman.BrojApartmana} oslobođen");

              
            }

            aktivneRezervacije.Remove(klijent);
        }
    }
}
