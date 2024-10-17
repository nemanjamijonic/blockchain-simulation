using ERS16.Src;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using System.Data;
using ERS16.DB;
using NUnit.Framework.Internal;

namespace ERS16.Tests
{
    [TestFixture]
    public class TestSmartContract
    {
        SmartContract contract;
        Mock<IDbConnection> mockConnection;
        [SetUp]
        public void Setup()
        {
            contract = new SmartContract();
            mockConnection = new Mock<IDbConnection>();
        }

        [Test]
        public void SmartContract_RegisterMiner_MinerIsRegistered()
        {
            // Arrange
            
            var miner = new Miner("0");

            // Act
            contract.RegisterMiner(miner);

            // Assert
            Assert.AreEqual(1, contract.Miners.Count);
            Assert.AreEqual(miner, contract.Miners[miner.ID]);
        }

        [Test]
        public void SmartContract_RegisterClient_ClientIsRegistered()
        {
            // Arrange
           
            var client = new Client("0", "data");

            // Act
            contract.RegisterClients(client);

            // Assert
            Assert.AreEqual(1, contract.Clients.Count);
            Assert.AreEqual(client, contract.Clients[client.ID]);
        }


        [Test]
        public void ReceiveData_ValidData_DataAddedToBlockchain()
        {
            // Arrange
            
            var clientId = "client1";
            var data = "test data";
            var miner = new Miner("miner1");
            var miner2 = new Miner("miner2");
            contract.RegisterMiner(miner);
            contract.RegisterMiner(miner2); 
            contract.RegisterClients(new Client(clientId, data));
           

            // Act
            contract.ReceiveData(clientId, data);

            // Assert
            Assert.AreEqual(1, miner.Chain.Chain.Count);
            Assert.AreEqual(data, miner.Chain.Chain.Last().Data);
            Assert.AreEqual(1, miner2.Chain.Chain.Count);
            Assert.AreEqual(data, miner2.Chain.Chain.Last().Data);
        }


        [Test]
        public void ReceiveData_ValidData_BtcAddedToBalance()
        {
            // Arrange
            
            var clientId = "client1";
            var data = "test data";
            Mock<Miner> minerMock = new Mock<Miner>("1");
            minerMock.Setup(m => m.Solve(It.IsAny<string>())).Returns("task");
            minerMock.Setup(m => m.Validate(It.IsAny<string>())).Returns(true);
            minerMock.Setup(m => m.Addblock(It.IsAny<string>()));
            contract.RegisterMiner(minerMock.Object);
            contract.RegisterClients(new Client(clientId, data));


            // Act
            contract.ReceiveData(clientId, data);

            // Assert
            minerMock.Verify(m => m.UpdateBalance(), Times.Once);
        }

        [Test]
        public void ReceiveData_Throws_Exception_For_Unregistered_Client()
        {
            var clientId = "456";
            var data = "Test data";

            var ex = Assert.Throws<ArgumentException>(() => contract.ReceiveData(clientId, data));

            Assert.AreEqual($"Client with id {clientId} is not registered.", ex.Message);
        }

        [Test]
        public void ShowMinerBalance_Returns_Correct_String()
        {
            ;
            Mock<Miner> minerMock = new Mock<Miner>("123");
            minerMock.Setup(m => m.ToString()).Returns("Miner: ID: 123 Balance: 10 BTC");
            contract.RegisterMiner(minerMock.Object);
            var result = contract.ShowMinerBalance("123");

            Assert.AreEqual("Miner: ID: 123 Balance: 10 BTC", result);
        }
        [Test]
        public void ShowMinerBalance_Returns_DontExist()
        {
            
            Miner miner = new Miner("123");
            
            contract.RegisterMiner(miner);
            var result = contract.ShowMinerBalance("213");

            Assert.AreEqual("Miner dont exist", result);
        }

        [Test]
        public void LoadMiners_AddSavedMiners()
        {

            contract.LoadMiners();

            Assert.AreNotEqual(0, contract.Miners.Count);

        }


        [Test]
        public void SaveMiners_SaveMinersToDB()
        {
            IDbConnection connection = DBConnection.GetConnection();

            contract.LoadMiners();
            contract.SaveMiners();

            string sql = "SELECT COUNT(*) FROM Miner";
            int count;
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                connection.Open();
                count = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
            Assert.Greater(count, 0);

        }

        [Test]
        public void TestExistsById_Exist()
        {
            // Arrange
            mockConnection.Setup(x => x.CreateCommand())
                .Returns(() =>
                {
                    var mockCommand = new Mock<IDbCommand>();
                    mockCommand.Setup(x => x.ExecuteScalar())
                        .Returns(1);
                    return mockCommand.Object;
                });
            // Act
            var result = contract.ExistsById("miner1", mockConnection.Object);
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void TestExistsById_NotExist()
        {
            // Arrange
            mockConnection.Setup(x => x.CreateCommand())
                .Returns(() =>
                {
                    var mockCommand = new Mock<IDbCommand>();
                    mockCommand.Setup(x => x.ExecuteScalar())
                        .Returns(null);
                    return mockCommand.Object;
                });
            // Act
            var result = contract.ExistsById("miner1", mockConnection.Object);
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void TestUpdateBlockChain_UpdateChainCopyOfMiners()
        {
            Mock<Miner> m = new Mock<Miner>("123");
            contract.RegisterMiner(m.Object);
            BlockChain bc = new BlockChain();
            bc.AddBlock(new Block("1", "data"));
            m.Setup(x => x.Chain).Returns(bc);

            contract.UpdateBlockChain(bc);

            m.VerifySet(x => x.Chain = bc, Times.Once);


        }


    }
}
