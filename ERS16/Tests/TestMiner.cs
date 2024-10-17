using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using ERS16.Src;
using NUnit.Framework.Internal;
using System.Runtime.CompilerServices;
using System.Security.Policy;

namespace ERS16.Tests
{
    [TestFixture]
    public class TestMiner
    {

        private Mock<BlockChain> mockChain;
        private Miner miner;

        [SetUp]
        public void Setup()
        {
            mockChain = new Mock<BlockChain>();
            miner = new Miner("123") { Chain = mockChain.Object };
        }

        [Test]
        public void Solve_Returns_Hash_String()
        {
            var blockData = "Test data";
            var result = miner.Solve(blockData);

            Assert.IsTrue(result is string);
        }

        [Test]
        public void Validate_Returns_True_For_Valid_Hash()
        {
            var validHash = "000000validhash";
            var result = miner.Validate(validHash);

            Assert.IsTrue(result);
        }

        [Test]
        public void Validate_Returns_False_For_Invalid_Hash()
        {
            var invalidHash = "invalidhash";
            var result = miner.Validate(invalidHash);

            Assert.IsFalse(result);
        }

        [Test]
        public void UpdateBalance_Increments_Balance()
        {
            var initialBalance = miner.Balance;
            miner.UpdateBalance();

            Assert.AreEqual(initialBalance + 1, miner.Balance);
        }

        [Test]
        public void AddBlock_Adds_Block_To_Chain()
        {
            var data = "Test data";
            miner = new Miner("123");
            miner.Addblock(data);
            var block = miner.Chain.Chain.Last();
            miner.Addblock("some other data");
            

            Assert.AreEqual(data, block.Data);
            Assert.AreEqual(2, miner.Chain.Chain.Count);
        }

        [Test]
        public void AddBlock_Calls_AddBlock_On_Chain()
        {
            var data = "Test data";

            miner.Addblock(data);

            mockChain.Verify(c => c.AddBlock(It.IsAny<Block>()), Times.Once());
        }

        [Test]
        public void Miner_CreateNewMinerInstance()
        {

            Miner m = new Miner("123", 4);

            Assert.AreEqual("123", m.ID);
            Assert.AreEqual(4, m.Balance);

        }

        [Test]
        public void TestParallelValidation()
        {
            // Arrange
            Miner miner1 = new Miner("miner1", 10);
            Miner miner2 = new Miner("miner2", 10);
            Miner miner3 = new Miner("miner3", 10);

            List<Miner> miners = new List<Miner>() { miner1, miner2, miner3 };
            string validHash = "000hesh";
            string invalidHesh = "hesh";
            // Act
            bool result = miner1.ParallelValidation(validHash, miners);
            bool result2 = miner2.ParallelValidation(invalidHesh, miners);
            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(result2);
        }
   

    }
}
