using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptology.Domain.ViewModels
{
    public class KnapsackViewModel
    {
        public List<int> CloseKey { get; set; } = new List<int>();
        public List<int> OpenKey { get; set; } = new List<int>();
        public int M { get; set; }
        public int T { get; set; }
        public string Text { get; set; }
        public int?[] CypherText { get; set; }
        public string? DecryptedText { get; set; }
        public IFormFile? OpenFile { get; set; }
        public IFormFile? SaveFile { get; set; }
        public Dictionary<char, int>? FrequencyTable { get; set; } = new Dictionary<char, int>();
    }
}
