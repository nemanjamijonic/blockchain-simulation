using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERS16.Src
{
    internal class Client
    {
        public string ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Data { get; set; }

        
        public Client(string id, string data)
        {
            ID = id;
            Data = data;
            CreatedAt = DateTime.Now;
        }
    }
}
