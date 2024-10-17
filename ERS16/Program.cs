using ERS16.DB;
using ERS16.Src;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ERS16
{
    [ExcludeFromCodeCoverage]
    internal class Program
    {
        
        public static SmartContract contract = new SmartContract();
        private static TcpListener listener;

        static void Main(string[] args)
        {
            contract.LoadMiners();
            BlockChain bc = new BlockChain();
            bc.LoadBlockChain();
            contract.UpdateBlockChain(bc);


            Console.WriteLine("Wellcome to BlockChain Simulation");
            while (true)
            {
                Console.WriteLine("\n------------------------------");
                Console.WriteLine("1. Register Miner");
                Console.WriteLine("2. Register Client");
                Console.WriteLine("3. Recieve Data");
                Console.WriteLine("4. Check Miner Balance");
                Console.WriteLine("5. Exit");
                Console.WriteLine("------------------------------\n");


                var option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        RegisterMiner();
                        break;
                    case "2":
                        RegisterClient();
                        break;
                    case "3":
                        RecieveData();
                        break;
                    case "4":
                        CheckMinerBalance();
                        break;
                    case "5":
                        contract.SaveMiners();
                        bc.Chain = contract.Miners.ElementAt(0).Value.Chain.Chain;
                        bc.SaveBlockChain();
                        return;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }

           
        }

        public static void CheckMinerBalance()
        {
            Console.WriteLine("Enter miner id:");
            var id = Console.ReadLine();
            Console.WriteLine(contract.ShowMinerBalance(id));
            
        }

        public static void RegisterMiner()
        {
            Console.WriteLine("Enter miner id:");
            var id = Console.ReadLine();
            contract.RegisterMiner(new Miner(id));
            Console.WriteLine("Miner registered successfully!");
        }

        public static void RegisterClient()
        {
            Console.WriteLine("Enter client id:");
            var id = Console.ReadLine();
            Console.WriteLine("Enter data:");
            var data = Console.ReadLine();
            contract.RegisterClients(new Client(id, data));
            contract.ReceiveData(id, data);
            Console.WriteLine("Client registered successfully!");
        }

        public static void RecieveData()
        {
            listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start();

            Console.WriteLine("Listening for incoming connections...");

            // Start a new task to handle incoming connections
            Task.Factory.StartNew(() =>
            {
                // Continuously listen for incoming connections
                while (true)
                {
                    // Accept an incoming connection
                    TcpClient client = listener.AcceptTcpClient();

                    // Start a new task to handle the connection
                    Task.Factory.StartNew(() =>
                    {
                        // Get the client's stream
                        NetworkStream stream = client.GetStream();

                        // Read the message from the client
                        byte[] buffer = new byte[1024];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        string[] parts = message.Split(':');
                        string clientId = parts[0];
                        string data = parts[1];
                        if(!contract.Clients.ContainsKey(clientId))
                            contract.RegisterClients(new Client(clientId, data));
                        // Call the RecieveData method
                        contract.ReceiveData(clientId, data);
                        Console.WriteLine("Data received from client '{0}': {1}", clientId, data);

                        // Close the connection
                        client.Close();
                    });
                }
            });
        }


    }
}