using ERS16.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ERS16.Src
{
    internal class BlockChain
    {
        public List<Block> Chain { get; set; }

        public BlockChain()
        {
            Chain = new List<Block>();
        }

        public virtual void AddBlock(Block block)
        {
            Chain.Add(block);
        }


        public void LoadBlockChain()
        {

            using (IDbConnection connection = DBConnection.GetConnection())
            {
                connection.Open();
                string sql = "SELECT ID, PreviousBlockID, Data, vreme  FROM BlockChain";
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(1) && !reader.IsDBNull(2) ) 
                                AddBlock(new Block(reader.GetString(1), reader.GetString(2)));
                        }
                    }
                }
                connection.Close();
            }
        }


        public void SaveBlockChain()
        {
            using (IDbConnection connection = DBConnection.GetConnection())
            {
                connection.Open();
                foreach (Block block in Chain)
                {
                    string sql = $"INSERT INTO BlockChain (ID, PreviousBlockID, Data) VALUES ('{block.ID}', '{block.PreviousBlockID}','{block.Data}' )";
                    string sql2 = $"UPDATE BlockChain SET Data = '{block.Data}' WHERE ID = '{block.ID}'";
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        command.CommandText = ExistsById(block.ID, connection) ? sql2 : sql;

                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
        }

        public bool ExistsById(string id, IDbConnection connection)
        {
            string query = $"select * from BlockChain where ID='{id}'";

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                return command.ExecuteScalar() != null;
            }
        }



    }
}
