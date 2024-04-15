using Cryptology.BLL;
using Cryptology.Domain.Enum;
using Cryptology.Domain.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cryptology.Tests
{
    [TestClass]
    public class KnapsackTests
    {
        private KnapsackService _knapsackService;
        private KnapsackViewModel knapsackViewModel;

        [TestInitialize]
        public void Setup()
        {
            _knapsackService = new KnapsackService(new GlobalService());

            knapsackViewModel = new KnapsackViewModel
            {
                Text = "abbccc"
            };
        }

        [TestMethod]
        public async Task TestEncryption()
        {
            var encryptResponse = await _knapsackService.Encrypt(knapsackViewModel);

            Assert.AreEqual(StatusCode.OK, encryptResponse.StatusCode);
            Assert.IsNotNull(encryptResponse.Data.CypherText);
        }

        [TestMethod]
        public async Task TestDecryption()
        {
            var encryptResponse = await _knapsackService.Encrypt(knapsackViewModel);
            var decryptResponse = await _knapsackService.Decrypt(encryptResponse.Data);

            Assert.AreEqual(StatusCode.OK, decryptResponse.StatusCode);
            Assert.AreEqual(knapsackViewModel.Text, decryptResponse.Data.DecryptedText);
        }

        [TestMethod]
        public async Task TestGenerateKeys()
        {
            var generateKeysResponse = await _knapsackService.GenerateKeys(8, knapsackViewModel);

            Assert.AreEqual(StatusCode.OK, generateKeysResponse.StatusCode);
            Assert.IsNotNull(generateKeysResponse.Data.OpenKey);
            Assert.IsNotNull(generateKeysResponse.Data.CloseKey);
            Assert.IsTrue(generateKeysResponse.Data.OpenKey.Count > 0);
            Assert.IsTrue(generateKeysResponse.Data.CloseKey.Count > 0);
        }

        [TestMethod]
        public async Task FrequencyTableTest()
        {
            var table = await _knapsackService.FrequencyTable(knapsackViewModel);

            var frequencyDictionary = new Dictionary<char, int>()
            {
                { 'a', 1 },
                { 'b', 2 },
                { 'c', 3 }
            };

            CollectionAssert.AreEqual(table.Data.FrequencyTable, frequencyDictionary);
        }

        [TestMethod]
        public async Task SaveToFileTest()
        {
            var saved = await _knapsackService.SaveToFile(knapsackViewModel);

            Assert.IsNotNull(saved);
            Assert.AreEqual(saved.Data.Text, knapsackViewModel.Text);
        }
    }
}
