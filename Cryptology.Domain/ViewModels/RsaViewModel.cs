using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptology.Domain.ViewModels
{
    public class RsaViewModel
    {
        public string? PublicKey { get; set; } = string.Empty;
        public string? PrivateKey { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public byte[]? Encrypted { get; set; } = null;

        public string EncryptedString
        {
            get => Convert.ToBase64String(Encrypted ?? Array.Empty<byte>());
            set
            {
                if (value != null)
                    Encrypted = Convert.FromBase64String(value);
            }
        }

        public string? Decrypted { get; set; }
        public IFormFile? OpenFile { get; set; }
        public IFormFile? SaveFile { get; set; }
        public Dictionary<char, int>? FrequencyTable { get; set; } = new Dictionary<char, int>();
    }
}
