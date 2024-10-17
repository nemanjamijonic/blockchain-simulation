using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ERS16.Src;
using NUnit.Framework;

namespace ERS16.Tests
{
    [TestFixture]
    public class TestBlock
    {
        [Test]
        public void Block_GenerateHash_HashIsCorrect()
        {

            var block = new Block("0","some data");

            using (var sha256 = SHA256.Create())
            {
                var data = $"{block.Data}-{block.PreviousBlockID}";
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                var expectedHash = Convert.ToBase64String(bytes);
                Assert.AreEqual(expectedHash, block.ID);
            }
        }

        [Test]
        public void Constructor_Sets_Properties()
        {
            var previousBlockID = "123";
            var data = "Test data";


            var block = new Block(previousBlockID, data);

            Assert.AreEqual(previousBlockID, block.PreviousBlockID);
            Assert.AreEqual(data, block.Data);
            Assert.AreEqual(DateTime.Now, block.Time);
        }
    }
}
