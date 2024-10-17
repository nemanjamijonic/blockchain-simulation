using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    [ExcludeFromCodeCoverage]
    internal class Program
    {
        static void Main(string[] args)
        {

            // Create a TcpClient object
            int port = 5000;
            string server = "127.0.0.1";
           

            // Prompt the user for their ID
            Console.Write("Enter your ID: ");
            string clientId = Console.ReadLine();

            // Enter the sending loop
            while (true)
            {
                try
                {
                    TcpClient client = new TcpClient(server, port);

                    // Get a client stream for reading and writing
                    NetworkStream stream = client.GetStream();
                    Console.Write("Enter a message: ");
                    string message = Console.ReadLine();

                    // Add the client's ID to the message
                    message = clientId + ":" + message;

                    // Translate the message into a byte array
                    byte[] data = Encoding.ASCII.GetBytes(message);

                    // Send the message
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("Sent: {0}", message);
                }
                catch (Exception)
                {
                    Console.WriteLine("Sesion finished");
                    Console.ReadLine();
                    return;
                }
            

            }
        }
    }
}
