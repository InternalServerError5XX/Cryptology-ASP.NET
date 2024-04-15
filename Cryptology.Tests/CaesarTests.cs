using Cryptology.BLL;
using Cryptology.Domain.Enum;
using Cryptology.Domain.Response;
using Cryptology.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System.Text;

namespace Cryptology.Tests
{
    [TestClass]
    public class CaesarTests
    {
        private GlobalService _globalService;
        private CaesarService _caesarService;       
        private CaesarViewModel caesar1;
        private CaesarViewModel caesar2;
        private CaesarViewModel caesar3;
        private CaesarViewModel caesar4;

        [TestInitialize]
        public void Setup()
        {
            _globalService = new GlobalService();
            _caesarService = new CaesarService(_globalService);          

            caesar1 = new CaesarViewModel
            {
                Key = 0,
                Text = "abc"
            };
            caesar2 = new CaesarViewModel
            {
                Key = 10,
                Text = "AbC, - Test."
            };

            var content = Encoding.UTF8.GetBytes("Mock image content");
            var ms = new MemoryStream(content);
            IFormFile imageFile = new FormFile(ms, 0, content.Length, "mockImageFile", "test.jpg");
        }

        [TestMethod]
        public void LanguageTest()
        {
            Assert.IsTrue(_globalService.IsEnglish(caesar1.Text));
            Assert.IsTrue(_globalService.IsEnglish(caesar1.Text));
        }

        [TestMethod]
        public async Task EncryptionTest()
        {
            var encrypted1 = await _caesarService.Encrypt(caesar1);
            var encrypted2 = await _caesarService.Encrypt(caesar2);

            Assert.AreEqual(encrypted1.Data.Encrypted, "abc");
            Assert.AreEqual(encrypted2.Data.Encrypted, "KlM, - Docd.");
        }

        [TestMethod]
        public async Task DecryptionTest()
        {
            caesar1.Encrypted = caesar1.Text;
            caesar2.Encrypted = caesar2.Text;

            var decrypted1 = await _caesarService.Decrypt(caesar1);
            var decrypted2 = await _caesarService.Decrypt(caesar2);

            Assert.AreEqual(decrypted1.Data.Decrypted, "abc");
            Assert.AreEqual(decrypted2.Data.Decrypted, "QrS, - Juij.");
        }

        [TestMethod]
        public async Task EncryptionDecryptionTest()
        {
            var encrypted1 = await _caesarService.Encrypt(caesar1);
            var encrypted2 = await _caesarService.Encrypt(caesar2);

            var decrypted1 = await _caesarService.Decrypt(encrypted1.Data);
            var decrypted2 = await _caesarService.Decrypt(encrypted2.Data);

            Assert.AreEqual(decrypted1.Data.Decrypted, caesar1.Text);
            Assert.AreEqual(decrypted2.Data.Decrypted, caesar2.Text);
        }

        [TestMethod]
        public async Task BruteForceTest()
        {
            var encrypted1 = await _caesarService.Encrypt(caesar1);
            var encrypted2 = await _caesarService.Encrypt(caesar2);

            var decrypted1 = await _caesarService.Decrypt(encrypted1.Data);
            var decrypted2 = await _caesarService.Decrypt(encrypted2.Data);

            var bruteForce1 = await _caesarService.BruteForce(caesar1);
            var bruteForce2 = await _caesarService.BruteForce(caesar2);

            Assert.IsTrue(bruteForce1.Data.BruteForced.Contains(decrypted1.Data.Decrypted));
            Assert.IsTrue(bruteForce2.Data.BruteForced.Contains(decrypted2.Data.Decrypted));
        }

        [TestMethod]
        public async Task OpenFromFileTest()
        {
            string fileContent = "1;\nabcd";
            byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);

            IFormFile file = new FormFile(new MemoryStream(fileBytes), 0, fileBytes.Length, "file", "file.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            IFormFile nullFile = null;
            var nullCaesarService = new CaesarService(_globalService);
            BaseResponse<CaesarViewModel> nullResponse = await nullCaesarService.OpenFromFile(nullFile);

            var caesarService = new CaesarService(_globalService);
            BaseResponse<CaesarViewModel> result = await caesarService.OpenFromFile(file);

            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Key);
            Assert.AreEqual("abcd", result.Data.Text);            

            Assert.AreEqual(StatusCode.NotFound, nullResponse.StatusCode);
            Assert.IsNull(nullResponse.Data);
        }


        [TestMethod]
        public async Task FrequencyTableTest()
        {
            var response = await _caesarService.FrequencyTable(caesar1);
            var frequencyDictionary = new Dictionary<char, int>()
            {
                { 'a', 1 },
                { 'b', 1 },
                { 'c', 1 }
            };

            CollectionAssert.AreEqual(response.Data.FrequencyTable, frequencyDictionary);
        }

        [TestMethod]
        public async Task SaveToFileTest()
        {
            var saved = await _caesarService.SaveToFile(caesar1);
            
            Assert.IsNotNull(saved);
            Assert.AreEqual(saved.Data.Key, caesar1.Key);
            Assert.AreEqual(saved.Data.Text, caesar1.Text);
        }

        [TestMethod]
        public async Task ReadImageTest()
        {
            var content = Encoding.UTF8.GetBytes("Image content");
            var ms = new MemoryStream(content);
            IFormFile imageFile = new FormFile(ms, 0, content.Length, "mockImageFile", "test.jpg");

            byte[] result = await _globalService.ReadImageBytes(imageFile);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
        }
    }
}
