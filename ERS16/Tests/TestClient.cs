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
    class TestClient
    {
        private const string _clientId = "client123";
        private const string _data = "test data";

        [Test]
        public void Constructor_ValidInput_PropertiesAreSet()
        {
            // Arrange
            var client = new Client(_clientId, _data);

            // Assert
            Assert.AreEqual(_clientId, client.ID);
            Assert.AreEqual(_data, client.Data);
            Assert.IsTrue((DateTime.Now - client.CreatedAt).TotalSeconds < 1);
        }
    }
}
