using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptology.Domain.Entity
{
    public class Rsa
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string Text { get; set; }
        public byte[]? Encrypted { get; set; }
        public string? Decrypted { get; set; }
        public IFormFile? OpenFile { get; set; }
        public IFormFile? SaveFile { get; set; }
        public Dictionary<char, int>? FrequencyTable { get; set; }
    }
}
