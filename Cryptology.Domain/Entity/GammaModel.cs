using Microsoft.AspNetCore.Http;

namespace Cryptology.Domain.Entity;

public class GammaModel
{
    public byte[]? Gamma { get; set; }
    public string? Text { get; set; }
    public byte[]? Encrypted { get; set; }
    public string? Decrypted { get; set; }
    public IFormFile? OpenFile { get; set; }
    public IFormFile? SaveFile { get; set; }
    public List<string>? BruteForced { get; set; }
    public Dictionary<char, int>? FrequencyTable { get; set; }
}