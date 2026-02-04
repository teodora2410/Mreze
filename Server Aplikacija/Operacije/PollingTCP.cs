using Common;
using Common.Enumeracije;
using Common.Modeli;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Server_Aplikacija.Operacije
{
    public class PollingTCP
    {
        Socket tcpListener;
        List<Socket> tcpClients;
        Dictionary<Socket, RezervacijaInfo> aktivneRezervacije;
        ObradaZahteva obradaZahteva;
        Socket udpSocket;
        List<EndPoint> osobljeUDPKlijenti;

        public PollingTCP(Socket tcpListener, List<Socket> tcpClients, Dictionary<Socket, RezervacijaInfo> aktivneRezervacije, ObradaZahteva obradaZahteva, Socket udpSocket, List<EndPoint> osobljeUDPKlijenti)
        {
            this.tcpListener = tcpListener;
            this.tcpClients = tcpClients;
            this.aktivneRezervacije = aktivneRezervacije;
            this.obradaZahteva = obradaZahteva;
            this.udpSocket = udpSocket;
            this.osobljeUDPKlijenti = osobljeUDPKlijenti;
        }

        public void RunPollingTCP()
        {
            if (tcpListener.Poll(0, SelectMode.SelectRead))
            {
                Socket klijent = tcpListener.Accept();
                klijent.Blocking = false;
                tcpClients.Add(klijent);
                Console.WriteLine($"[TCP] Klijent povezan: {klijent.RemoteEndPoint}");
            }

            List<Socket> disconnected = new List<Socket>();
            foreach (var client in tcpClients)
            {
                if (!client.Poll(0, SelectMode.SelectRead))
                    continue;

                try
                {
                    byte[] buffer = new byte[8192];
                    int bytes = client.Receive(buffer);
                    if (bytes == 0)
                    {
                        disconnected.Add(client);
                        continue;
                    }

                    byte[] data = new byte[bytes];
                    Array.Copy(buffer, data, bytes);
                    ZahtevKlijenta zahtev = MemorySerializer.Deserialize<ZahtevKlijenta>(data);
                    obradaZahteva.ObradaZahtevaKlijenta(client, zahtev);
                }
                catch (SocketException)
                {
                    disconnected.Add(client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Greška TCP] {ex.Message}");
                    disconnected.Add(client);
                }
            }

            foreach (var d in disconnected)
            {
                Console.WriteLine($"[TCP] Klijent diskonektovan: {d.RemoteEndPoint}");
                if (aktivneRezervacije.ContainsKey(d))
                {
                    var info = aktivneRezervacije[d];
                    if (info.Apartman != null)
                    {
                        PosaljiRacun(d, info);

                        info.Apartman.TrenutniBrojGostiju = 0;
                        info.Apartman.Stanje = StanjeApartmana.PotrebnoCiscenje;
                        info.Apartman.Gosti.Clear();
                        Console.WriteLine($"[Server] Apartman {info.Apartman.BrojApartmana} oslobođen");

                        PosaljiZadatakCiscenja(info.Apartman);
                    }
                    aktivneRezervacije.Remove(d);
                }
                tcpClients.Remove(d);
                d.Close();
            }
        }

        private void PosaljiRacun(Socket klijent, RezervacijaInfo info)
        {
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
            }
        }

        private void PosaljiZadatakCiscenja(Apartman apartman)
        {
            if (osobljeUDPKlijenti.Count == 0)
            {
                Console.WriteLine("[Server] Nema dostupnog osoblja za čišćenje");
                return;
            }

            Zadatak zadatak = new Zadatak(TipZadatka.Ciscenje, apartman.Id);
            byte[] data = MemorySerializer.Serialize(zadatak);

            foreach (var osoblje in osobljeUDPKlijenti)
            {
                udpSocket.SendTo(data, osoblje);
            }

            Console.WriteLine($"[Server] Zadatak čišćenja poslat osoblju za apartman {apartman.BrojApartmana}");
        }
    }
}