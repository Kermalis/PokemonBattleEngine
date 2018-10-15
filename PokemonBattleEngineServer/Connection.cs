using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PokemonBattleEngineServer
{
    class Connection
    {
        static Socket server;
        static List<Socket> ConnectedClients;

        public Connection(IPAddress ip)
        {
            ConnectedClients = new List<Socket>();

            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { ReceiveTimeout = -1 };
            server.Bind(new IPEndPoint(ip, 8888));
            server.Listen(-1);

            Console.WriteLine("Server online");
            while (true)
            {
                Socket client = server.Accept();
                new Thread(() =>
                {
                    ConnectClient(client);
                }).Start();
            }
        }

        void DisconnectClient(Socket client)
        {
            ConnectedClients.Remove(client);
            client.Disconnect(false);
        }
        void ConnectClient(Socket client)
        {
            Console.WriteLine("Incoming connection from " + client.RemoteEndPoint);
            ConnectedClients.Add(client);

            const int maxMessageSize = 1024;
            byte[] response;
            int received;

            while (true)
            {
                try
                {
                    Console.Write("Server: ");
                    client.Send(Encoding.ASCII.GetBytes(Console.ReadLine()));
                    Console.WriteLine();

                    response = new byte[maxMessageSize];
                    received = client.Receive(response);

                    List<byte> respBytesList = new List<byte>(response);
                    respBytesList.RemoveRange(received, maxMessageSize - received); // truncate zero end
                    Console.WriteLine("Client (" + client.RemoteEndPoint + "): " + Encoding.ASCII.GetString(respBytesList.ToArray()));
                }
                catch (SocketException e)
                {
                    DisconnectClient(client);
                    return;
                }
            }
        }
    }
}
