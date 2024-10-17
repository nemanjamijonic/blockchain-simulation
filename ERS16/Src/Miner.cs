using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ERS16.Src
{
    internal class Miner
    {
        public string ID { get; set; }
        public double Balance { get; set; }
        public virtual BlockChain Chain { get; set; }
        public Miner(string id)
        {            
            ID = id;
            Balance = 0;
            Chain = new BlockChain();
        }

        public Miner(string id, double balance)
        {
            ID = id;
            Balance = balance;
            Chain = new BlockChain();
        }

        public virtual string Solve(string blockData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                var hash = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(blockData + Guid.NewGuid().ToString()))).Replace("-", "");
                return hash.ToString();
            }
        }


        public virtual bool Validate(string hash) {
            var leadingZeroes = "000";

            if (hash.StartsWith(leadingZeroes))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ParallelValidation(string hash, List<Miner> miners)
        {
            bool allValidated = true;
            Parallel.ForEach(miners, miner => {
                if (!miner.Validate(hash))
                {
                    allValidated = false;
                }
            });
            return allValidated;
        }

        public virtual void UpdateBalance()
        {
            Balance++;
        }

        public virtual void Addblock(string data)
        {
            if (Chain.Chain.Count == 0)
               Chain.AddBlock(new Block("0", data));
            else
               Chain.AddBlock(new Block(Chain.Chain.Last().ID, data));
           
        }
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return "Miner: ID: "+ ID + " Balance: " + Balance +" BTC";
        }
    }
}
