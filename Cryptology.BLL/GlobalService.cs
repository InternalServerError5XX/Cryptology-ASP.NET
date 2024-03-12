using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptology.BLL
{
    public class GlobalService
    {
        public bool IsEnglish(string text)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(text, @"^[a-zA-Z\s.,-?]+$");
        }

        public bool IsUkrainian(string text)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(text, @"^[а-яА-ЯіІїЇєЄґҐ\s.,-?]+$");
        }

        public async Task<byte[]> ReadImageBytes(IFormFile imageFile)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public async Task SaveImage(byte[] encryptedImageBytes, string outputPath)
        {
            using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
            {
                await fileStream.WriteAsync(encryptedImageBytes, 0, encryptedImageBytes.Length);
            }
        }
    }
}
