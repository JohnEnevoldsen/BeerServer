using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BeerServer
{
    class Program
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("server starting listener!");

            // start listener , get tcp client, read write
            //Server.ReadWriteStream(Server.GetTcpClient(Server.StartListener()));

            // concurrent server
            TcpListener socket = Server.StartListener();
            while (true)
            {
                Task.Run((() => Server.ReadWriteStream(Server.GetTcpClient(socket))));

            }
        }
    }
}
