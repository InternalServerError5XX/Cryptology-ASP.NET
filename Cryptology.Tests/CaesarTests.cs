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
        private CaesarService _caesarService;
        private CaesarViewModel caesar1;
        private CaesarViewModel caesar2;
        private CaesarViewModel caesar3;
        private CaesarViewModel caesar4;

        [TestInitialize]
        public void Setup()
        {
            _caesarService = new CaesarService();

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
            caesar3 = new CaesarViewModel
            {
                Key = 0,
                Text = "‡·‚"
            };
            caesar4 = new CaesarViewModel
            {
                Key = 10,
                Text = "¿·¬, - “ÂÒÚ."
            };

            var content = Encoding.UTF8.GetBytes("Mock image content");
            var ms = new MemoryStream(content);
            IFormFile imageFile = new FormFile(ms, 0, content.Length, "mockImageFile", "test.jpg");
        }

        [TestMethod]
        public void LanguageTest()
        {
            Assert.IsTrue(_caesarService.IsEnglish(caesar2.Text));
            Assert.IsFalse(_caesarService.IsEnglish(caesar3.Text));
            Assert.IsTrue(_caesarService.IsUkrainian(caesar4.Text));
            Assert.IsFalse(_caesarService.IsUkrainian(caesar1.Text));
        }

        [TestMethod]
        public async Task EncryptionTest()
        {
            var encrypted1 = await _caesarService.Encrypt(caesar1);
            var encrypted2 = await _caesarService.Encrypt(caesar2);
            var encrypted3 = await _caesarService.Encrypt(caesar3);
            var encrypted4 = await _caesarService.Encrypt(caesar4);

            Assert.AreEqual(encrypted1.Data.Encrypted, "abc");
            Assert.AreEqual(encrypted2.Data.Encrypted, "KlM, - Docd.");
            Assert.AreEqual(encrypted3.Data.Encrypted, "‡·‚");
            Assert.AreEqual(encrypted4.Data.Encrypted, " ÎÃ, - ‹Ô˚¸.");
        }

        [TestMethod]
        public async Task DecryptionTest()
        {
            caesar1.Encrypted = caesar1.Text;
            caesar2.Encrypted = caesar2.Text;
            caesar3.Encrypted = caesar3.Text;
            caesar4.Encrypted = caesar4.Text;

            var decrypted1 = await _caesarService.Decrypt(caesar1);
            var decrypted2 = await _caesarService.Decrypt(caesar2);
            var decrypted3 = await _caesarService.Decrypt(caesar3);
            var decrypted4 = await _caesarService.Decrypt(caesar4);            

            Assert.AreEqual(decrypted1.Data.Decrypted, "abc");
            Assert.AreEqual(decrypted2.Data.Decrypted, "QrS, - Juij.");
            Assert.AreEqual(decrypted3.Data.Decrypted, "‡·‚");
            Assert.AreEqual(decrypted4.Data.Decrypted, "◊¯Ÿ, - »¸ÁË.");
        }

        [TestMethod]
        public async Task EncryptionDecryptionTest()
        {
            var encrypted1 = await _caesarService.Encrypt(caesar1);
            var encrypted2 = await _caesarService.Encrypt(caesar2);
            var encrypted3 = await _caesarService.Encrypt(caesar3);
            var encrypted4 = await _caesarService.Encrypt(caesar4);

            var decrypted1 = await _caesarService.Decrypt(encrypted1.Data);
            var decrypted2 = await _caesarService.Decrypt(encrypted2.Data);
            var decrypted3 = await _caesarService.Decrypt(encrypted3.Data);
            var decrypted4 = await _caesarService.Decrypt(encrypted4.Data);

            Assert.AreEqual(decrypted1.Data.Decrypted, caesar1.Text);
            Assert.AreEqual(decrypted2.Data.Decrypted, caesar2.Text);
            Assert.AreEqual(decrypted3.Data.Decrypted, caesar3.Text);
            Assert.AreEqual(decrypted4.Data.Decrypted, caesar4.Text);
        }

        [TestMethod]
        public async Task BruteForceTest()
        {
            var encrypted1 = await _caesarService.Encrypt(caesar1);
            var encrypted2 = await _caesarService.Encrypt(caesar2);
            var encrypted3 = await _caesarService.Encrypt(caesar3);
            var encrypted4 = await _caesarService.Encrypt(caesar4);

            var decrypted1 = await _caesarService.Decrypt(encrypted1.Data);
            var decrypted2 = await _caesarService.Decrypt(encrypted2.Data);
            var decrypted3 = await _caesarService.Decrypt(encrypted3.Data);
            var decrypted4 = await _caesarService.Decrypt(encrypted4.Data);

            var bruteForce1 = await _caesarService.BruteForce(caesar1);
            var bruteForce2 = await _caesarService.BruteForce(caesar2);
            var bruteForce3 = await _caesarService.BruteForce(caesar3);
            var bruteForce4 = await _caesarService.BruteForce(caesar4);

            Assert.IsTrue(bruteForce1.Data.BruteForced.Contains(decrypted1.Data.Decrypted));
            Assert.IsTrue(bruteForce2.Data.BruteForced.Contains(decrypted2.Data.Decrypted));
            Assert.IsTrue(bruteForce3.Data.BruteForced.Contains(decrypted3.Data.Decrypted));
            Assert.IsTrue(bruteForce4.Data.BruteForced.Contains(decrypted4.Data.Decrypted));
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
            var nullCaesarService = new CaesarService();
            BaseResponse<CaesarViewModel> nullResponse = await nullCaesarService.OpenFromFile(nullFile);

            var caesarService = new CaesarService();
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
            var content = Encoding.UTF8.GetBytes("Mock image content");
            var ms = new MemoryStream(content);
            IFormFile imageFile = new FormFile(ms, 0, content.Length, "mockImageFile", "test.jpg");

            byte[] result = await _caesarService.ReadImageBytes(imageFile);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
        }

        [TestMethod]
        public async Task SaveImageTest()
        {
            string outputPath = "D:\\ÃŒ \\1\\test-output.jpg";
            byte[] testData = Encoding.UTF8.GetBytes("Test data");

            await _caesarService.SaveImage(testData, outputPath);

            Assert.IsTrue(File.Exists(outputPath));
            File.Delete(outputPath);
        }

        [TestMethod]
        public async Task EncryptImageTest()
        {
            var content = Encoding.UTF8.GetBytes("Mock image content");
            var ms = new MemoryStream(content);
            IFormFile imageFile = new FormFile(ms, 0, content.Length, "mockImageFile", "test.jpg");

            CaesarViewModel caesar = new CaesarViewModel
            {
                InputImage = imageFile,
                Key = 3
            };

            var response = await _caesarService.EncryptImage(caesar);

            Assert.AreEqual(StatusCode.OK, response.StatusCode);
            Assert.IsNotNull(response.Data);
            Assert.IsTrue(File.Exists("D:\\ÃŒ \\1\\encrypted.jpg"));

            File.Delete("D:\\ÃŒ \\1\\encrypted.jpg");
        }

        [TestMethod]
        public async Task DecryptImageTest()
        {
            var content = Encoding.UTF8.GetBytes("Mock image content");
            var ms = new MemoryStream(content);
            IFormFile imageFile = new FormFile(ms, 0, content.Length, "mockImageFile", "test.jpg");

            CaesarViewModel caesar = new CaesarViewModel
            {
                EncryptedImage = imageFile,
                Key = 3
            };

            var decryptResponse = await _caesarService.DecryptImage(caesar);  
            caesar.DecryptedImage = decryptResponse.Data.EncryptedImage;

            Assert.AreEqual(StatusCode.OK, decryptResponse.StatusCode);
            Assert.IsNotNull(caesar.DecryptedImage);
            Assert.IsTrue(File.Exists("D:\\ÃŒ \\1\\decrypted.jpg"));

            File.Delete("D:\\ÃŒ \\1\\decrypted.jpg");
        }
    }
}
