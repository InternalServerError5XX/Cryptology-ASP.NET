using Cryptology.BLL;
using Cryptology.Domain.ViewModels;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptology.Tests
{
    [TestClass]
    public class TritemiusTests
    {
        private GlobalService _globalService;
        private TrithemiusService _trithemiusService;
        private TrithemiusViewModel trithemius1, trithemius2, trithemius3;

        [TestInitialize]
        public void Setup()
        {
            _globalService = new GlobalService();
            _trithemiusService = new TrithemiusService(_globalService);

            trithemius1 = new TrithemiusViewModel
            {
                KeyType = Domain.Enum.KeyType.Password,
                Password = "asd",
                Text = "test"
            };
            trithemius2 = new TrithemiusViewModel
            {
                KeyType = Domain.Enum.KeyType.LinearEquation,
                LinearCoefficientA = 5,
                LinearCoefficientB = 7,
                Text = "test"
            };
            trithemius3 = new TrithemiusViewModel
            {
                KeyType = Domain.Enum.KeyType.NonlinearEquation,
                NonlinearCoefficientA = 2,
                NonlinearCoefficientB = 5,
                NonlinearCoefficientC = 8,
                Position = 3,
                Text = "test"
            };
        }

        [TestMethod]
        public async Task PasswordEncryptionTest()
        {
            var response = await _trithemiusService.Encrypt(trithemius1);

            Assert.IsNotNull(response);
            Assert.AreEqual("twvt", response.Data.Encrypted);
        }

        [TestMethod]
        public async Task LinearEncryptionTest()
        {
            var response = await _trithemiusService.Encrypt(trithemius2);

            Assert.IsNotNull(response);
            Assert.AreEqual("ybty", response.Data.Encrypted);
        }

        [TestMethod]
        public async Task NonlinearEncryptionTest()
        {
            var response = await _trithemiusService.Encrypt(trithemius3);

            Assert.IsNotNull(response);
            Assert.AreEqual("ithi", response.Data.Encrypted);
        }

        [TestMethod]
        public async Task PasswordDecryptionTest()
        {
            var encription = await _trithemiusService.Encrypt(trithemius1);
            var decription = await _trithemiusService.Decrypt(trithemius1);

            Assert.IsNotNull(decription);
            Assert.AreEqual(trithemius1.Text, decription.Data.Decrypted);
        }

        [TestMethod]
        public async Task LinearDecryptionTest()
        {
            var encription = await _trithemiusService.Encrypt(trithemius2);
            var decription = await _trithemiusService.Decrypt(trithemius2);

            Assert.IsNotNull(decription);
            Assert.AreEqual(trithemius2.Text, decription.Data.Decrypted);
        }

        [TestMethod]
        public async Task NonlinearDecryptionTest()
        {
            var encription = await _trithemiusService.Encrypt(trithemius3);
            var decription = await _trithemiusService.Decrypt(trithemius3);

            Assert.IsNotNull(decription);
            Assert.AreEqual(trithemius3.Text, decription.Data.Decrypted);
        }

        [TestMethod]
        public async Task BruteForceTestTest()
        {
            var trithemiusModels = new List<TrithemiusViewModel> { trithemius1, trithemius2, trithemius3 };

            foreach (var trithemius in trithemiusModels)
            {
                var encrypted = await _trithemiusService.Encrypt(trithemius);
                trithemius.Encrypted = encrypted.Data.Encrypted;

                var bruteForce = await _trithemiusService.BruteForce(trithemius);
                trithemius.BruteForced = bruteForce.Data.BruteForced;

                var containsText = trithemius.BruteForced.Any(bruteForceElement => bruteForceElement.Contains(trithemius.Text));
                Assert.IsTrue(containsText);
            }
        }

        [TestMethod]
        public async Task FrequencyTableTest()
        {
            var response = await _trithemiusService.FrequencyTable(trithemius1);
            var frequencyDictionary = new Dictionary<char, int>()
            {
                { 't', 2 },
                { 'e', 1 },
                { 's', 1 }
            };

            CollectionAssert.AreEqual(response.Data.FrequencyTable, frequencyDictionary);
        }

        [TestMethod]
        public async Task SaveToFileTest()
        {
            var saved = await _trithemiusService.SaveToFile(trithemius1);

            Assert.IsNotNull(saved);
            Assert.AreEqual(saved.Data.KeyType, trithemius1.KeyType);
            Assert.AreEqual(saved.Data.Text, trithemius1.Text);
        }
    }
}
