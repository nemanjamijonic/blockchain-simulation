using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ERS16.Src
{
    internal class Block
    {
        public string ID { get; set; }
        public string PreviousBlockID { get; set; }
        public string Data { get; set; }
        public DateTime Time { get; set; }  

        
        public Block(string previousBlockID, string data)
        {
            PreviousBlockID = previousBlockID;
            Data = data;
            Time = DateTime.Now;
            GenerateHash();
        }
        [ExcludeFromCodeCoverage]
        private void GenerateHash()
        {
            using (var sha256 = SHA256.Create())
            {
                var data = $"{Data}-{PreviousBlockID}";
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                ID = Convert.ToBase64String(bytes);
            }
        }


    }
}
