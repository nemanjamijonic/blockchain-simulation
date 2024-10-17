using ERS16.DB;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERS16.Src
{
    internal class SmartContract
    {
        public Dictionary<String, Miner> Miners { get; set; }
        public Dictionary<String, Client> Clients { get; set; }

        public SmartContract()
        {
            Miners = new Dictionary<String, Miner>();
            Clients = new Dictionary<String, Client>();
        }

        public void RegisterMiner(Miner miner)
        {
            Miners.Add(miner.ID, miner);
        }

        public void RegisterClients(Client client)
        {
            Clients.Add(client.ID, client);
           
        }

        public void ReceiveData(string clientId, string data)
        {

            if (!Clients.ContainsKey(clientId))
            {
                throw new ArgumentException($"Client with id {clientId} is not registered.");
            }

           

            bool solved = false;
            object lockObject = new object();

                Parallel.ForEach(Miners.Values, miner => {
                    while (!solved)
                    {
                        string hash = miner.Solve(data);
                        if (miner.ParallelValidation(hash, Miners.Values.ToList()))
                        {
                            lock (lockObject)
                            {
                                if (!solved)
                                {
                                    solved = true;
                                    UpdateIf(miner, data);
                                }
                            }
                        }
                    }
                });
        }

        private void UpdateIf(Miner chosenMiner, string data) {
            
                chosenMiner.UpdateBalance();
                foreach (Miner m in Miners.Values)
                {
                    m.Addblock(data);
                }
            
        }

        public string ShowMinerBalance(string minerId)
        {
            if (Miners.ContainsKey(minerId))
                return Miners[minerId].ToString();
            else
                return "Miner dont exist";
        }

        public void LoadMiners()
        {

            using (IDbConnection connection = DBConnection.GetConnection())
            {
                connection.Open();
                string sql = "SELECT ID, Balance FROM Miner";
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RegisterMiner(new Miner(reader.GetString(0), reader.GetDouble(1)));
                        }
                    }
                }
                connection.Close();
            }
        }


        public void SaveMiners()
        {
            using (IDbConnection connection = DBConnection.GetConnection())
            {
                connection.Open();
                foreach (Miner miner in Miners.Values)
                {
                    string sql = $"INSERT INTO Miner (ID, Balance) VALUES ('{miner.ID}', {miner.Balance})";
                    string sql2 = $"UPDATE Miner SET Balance = {miner.Balance} WHERE ID = '{miner.ID}'";
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        command.CommandText = ExistsById(miner.ID, connection) ? sql2 : sql;

                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
        }

        public bool ExistsById(string id, IDbConnection connection)
        {
            string query = $"select * from Miner where ID={id}";

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                return command.ExecuteScalar() != null;
            }
        }

        public void UpdateBlockChain(BlockChain bc)
        {
            foreach (Miner m in Miners.Values)
            {
                m.Chain = bc;
            }
        }
    }
}
