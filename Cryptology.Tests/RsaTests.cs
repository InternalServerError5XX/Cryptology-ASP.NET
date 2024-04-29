using Cryptology.BLL;
using Cryptology.Domain.Enum;
using Cryptology.Domain.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cryptology.Tests
{
    [TestClass]
    public class RsaTests
    {
        private RsaService _rsaService;
        private RsaViewModel rsaViewModel;

        [TestInitialize]
        public void Setup()
        {
            _rsaService = new RsaService();

            rsaViewModel = new RsaViewModel
            {
                Text = "abbccc"
            };
        }

        [TestMethod]
        public async Task TestEncryption()
        {
            var encryptResponse = await _rsaService.Encrypt(rsaViewModel);

            Assert.AreEqual(StatusCode.OK, encryptResponse.StatusCode);
            Assert.IsNotNull(encryptResponse.Data.Encrypted);
        }

        [TestMethod]
        public async Task TestDecryption()
        {
            var encryptResponse = await _rsaService.Encrypt(rsaViewModel);
            var decryptResponse = await _rsaService.Decrypt(encryptResponse.Data);

            Assert.AreEqual(StatusCode.OK, decryptResponse.StatusCode);
            Assert.AreEqual(rsaViewModel.Text, decryptResponse.Data.Decrypted);
        }

        [TestMethod]
        public async Task TestGenerateKeys()
        {
            var generateKeysResponse = await _rsaService.GenerateKeys(rsaViewModel);

            Assert.AreEqual(StatusCode.OK, generateKeysResponse.StatusCode);
            Assert.IsNotNull(generateKeysResponse.Data.PublicKey);
            Assert.IsNotNull(generateKeysResponse.Data.PrivateKey);
            Assert.IsTrue(generateKeysResponse.Data.PublicKey.Length > 0);
            Assert.IsTrue(generateKeysResponse.Data.PrivateKey.Length > 0);
        }

        [TestMethod]
        public async Task FrequencyTableTest()
        {
            var table = await _rsaService.FrequencyTable(rsaViewModel);

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
            var saved = await _rsaService.SaveToFile(rsaViewModel);

            Assert.IsNotNull(saved);
            Assert.AreEqual(saved.Data.Text, rsaViewModel.Text);
        }
    }
}
