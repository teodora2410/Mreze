using Common;
using Common.Enumeracije;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Klijent_Osoblje
{
    public class Program
    {
        static void Main(string[] args)
        {
            Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpSocket.Blocking = false;

            EndPoint serverEP = new IPEndPoint(IPAddress.Loopback, 9001);
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                Console.WriteLine("Dobrodosli u hotelski kompleks!");

                ZahtevKlijenta registracija = new ZahtevKlijenta
                {
                    Tip = TipZahteva.StatusSobe,
                    Payload = "OSOBLJE_REGISTRACIJA"
                };
                byte[] data = MemorySerializer.Serialize(registracija);
                udpSocket.SendTo(data, serverEP);

                Console.WriteLine("Povezano sa serverom.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greska pri registraciji: {ex.Message}");
            }

            byte[] buffer = new byte[4096];
            int zadatakBroj = 0;

            while (true)
            {
                if (udpSocket.Poll(100_000, SelectMode.SelectRead))
                {
                    try
                    {
                        int bytes = udpSocket.ReceiveFrom(buffer, ref remoteEP);
                        byte[] data = new byte[bytes];
                        Array.Copy(buffer, data, bytes);

                        Zadatak zadatak = MemorySerializer.Deserialize<Zadatak>(data);
                        zadatakBroj++;

                        Console.WriteLine($"{zadatak.Tip} za Apartman {zadatak.ApartmanId}");

                        Console.Write("Uradjeno: ");
                        switch (zadatak.Tip)
                        {
                            case TipZadatka.Ciscenje:
                                for (int i = 0; i < 3; i++)
                                {
                                    Thread.Sleep(800);
                                    Console.Write($"{(i + 1) * 33}%  ");
                                }
                                break;

                            case TipZadatka.Minibar:
                                for (int i = 0; i < 3; i++)
                                {
                                    Thread.Sleep(600);
                                    Console.Write($"{(i + 1) * 33}%  ");
                                }
                                break;

                            case TipZadatka.Alarm:
                                for (int i = 0; i < 2; i++)
                                {
                                    Thread.Sleep(1000);
                                    Console.Write($"{(i + 1) * 50}%   ");
                                }
                                break;
                        }

                        Console.WriteLine("\nZadatak zavrsen\n");

                        PotvrdaOUradjenomZadatku potvrda = new PotvrdaOUradjenomZadatku
                        {
                            ApartmanId = zadatak.ApartmanId,
                            Tip = zadatak.Tip,
                            Uspesno = true
                        };

                        byte[] odgovor = MemorySerializer.Serialize(potvrda);
                        udpSocket.SendTo(odgovor, serverEP);
                    }
                    catch (SocketException)
                    {
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Greska: {ex.Message}");
                    }
                }
            }
        }
    }
}