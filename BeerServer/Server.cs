using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using BeerLib;

namespace BeerServer
{
    class Server
    {
        static List<Beer> beerlist = new List<Beer>
        {
            new Beer("Turbog", 74, 4.6) {ID = 1},
            new Beer("Gobrut", 47, 6.4) {ID = 2},
            new Beer("Carlsberg", 73, 5) {ID = 3}
        };
        private static int clientNr = 0;
        public static TcpListener StartListener()
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Loopback, 4646);
            serverSocket.Start();
            Console.WriteLine("server started waiting for client connection");
            return serverSocket;
        }

        public static TcpClient GetTcpClient(TcpListener socket)
        {
            TcpClient clientConnection = socket.AcceptTcpClient();
            clientNr++;
            Console.WriteLine("Client " + clientNr + "connected");
            return clientConnection;
        }

        public static void ReadWriteStream(TcpClient client)
        {
            try
            {
                Stream ns = client.GetStream();
                StreamReader sr = new StreamReader(ns);
                StreamWriter sw = new StreamWriter(ns);
                sw.AutoFlush = true;

                string message = sr.ReadLine();
                string answer = "";

                while (message != " " && message != "stop")
                {
                    Console.WriteLine("Client: " + message);
                    message = message.ToLower();
                    switch (message)
                    {
                        case "hentalle":
                            Console.WriteLine(JsonConvert.SerializeObject(beerlist));
                            sw.WriteLine(JsonConvert.SerializeObject(beerlist));
                            break;
                        case "hent":
                            answer = "Vær sød at skrive et ID-nummer nu. Det skal være et heltal.";
                            Console.WriteLine(answer);
                            sw.WriteLine(answer);
                            message = sr.ReadLine();
                            int nummer;
                            bool lykkedesDet = Int32.TryParse(message, out nummer);
                            if (!lykkedesDet)
                            {
                                answer = "Det var ikke et heltal";
                                Console.WriteLine(answer);
                                sw.WriteLine(answer);
                                break;
                            }
                            Console.WriteLine(JsonConvert.SerializeObject(beerlist.Find(x => x.ID == nummer)));
                            sw.WriteLine(JsonConvert.SerializeObject(beerlist.Find(x => x.ID == nummer)));
                            break;
                        case "gem":
                            answer = "Vær sød at skrive en json string med ID, Name, Price og Abv.";
                            Console.WriteLine(answer);
                            sw.WriteLine(answer);
                            string jsonString = sr.ReadLine();
                            Beer beer = JsonConvert.DeserializeObject<Beer>(jsonString);
                            beerlist.Add(beer);
                            break;
                        default:
                            answer = "Det kan jeg ikke forstå. Vær sød at skrive HentAlle, Hent, eller Gem.";
                            Console.WriteLine(answer);
                            sw.WriteLine(answer);
                            break;
                    }
                    message = sr.ReadLine();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
