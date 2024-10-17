using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERS16.DB;
using ERS16.Src;
using Moq;
using NUnit.Framework;

namespace ERS16.Tests
{
    

    [TestFixture]
    public class TestBlockChain
    {

        [Test]
        public void Blockchain_AddBlock_BlockIsAdded()
        {
            // Arrange
            var blockchain = new BlockChain();
            var block = new Block("0", "some data");

            // Act
            blockchain.AddBlock(block);

            // Assert
            Assert.AreEqual(1, blockchain.Chain.Count);
            Assert.AreEqual(block, blockchain.Chain[0]);
        }

        [Test]
        public void Blockchain_AddMultipleBlocks_ChainIsCorrect()
        {
            // Arrange
            var blockchain = new BlockChain();
            var block1 = new Block("0", "some data");
            var block2 = new Block(block1.ID, "some other data");

            // Act
            blockchain.AddBlock(block1);
            blockchain.AddBlock(block2);

            // Assert
            Assert.AreEqual(2, blockchain.Chain.Count);
            Assert.AreEqual(block1, blockchain.Chain[0]);
            Assert.AreEqual(block2, blockchain.Chain[1]);
        }


        [Test]
        public void LoadBlockChain_LoadSavedBlockChain()
        {
            var blockchain = new BlockChain();
            blockchain.LoadBlockChain();

            Assert.Less(0,blockchain.Chain.Count);

        }


        [Test]
        public void SaveBlockChainToDB()
        {
            IDbConnection connection = DBConnection.GetConnection();

            var blockchain = new BlockChain();
            blockchain.AddBlock(new Block("0", "Some Data"));
            blockchain.SaveBlockChain();

            string sql = "SELECT COUNT(*) FROM BlockChain";
            int count;
            connection.Open();
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;                
                count = Convert.ToInt32(command.ExecuteScalar());        
            }
            connection.Close();
            Assert.Greater(count, 0);

        }

        [Test]
        public void TestExistsById_Exist()
        {
            var blockchain = new BlockChain();
            Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
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
            var result = blockchain.ExistsById("block1", mockConnection.Object);
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void TestExistsById_NotExist()
        {
            // Arrange
            var blockchain = new BlockChain();
            Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand())
                .Returns(() =>
                {
                    var mockCommand = new Mock<IDbCommand>();
                    mockCommand.Setup(x => x.ExecuteScalar())
                        .Returns(null);
                    return mockCommand.Object;
                });
            // Act
            var result = blockchain.ExistsById("block", mockConnection.Object);
            // Assert
            Assert.IsFalse(result);
        }

    }
}
