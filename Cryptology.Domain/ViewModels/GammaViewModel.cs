using Microsoft.AspNetCore.Http;

namespace Cryptology.Domain.ViewModels;

public class GammaViewModel
{
    public byte[]? Gamma { get; set; }
    
    public string GammaString
    {
        get => Convert.ToBase64String(Gamma ?? Array.Empty<byte>());
        set => Gamma = Convert.FromBase64String(value);
    }
    public string? Text { get; set; }
    public byte[]? Encrypted { get; set; }
    public string EncryptedString
    {
        get => Convert.ToBase64String(Encrypted ?? Array.Empty<byte>());
        set => Encrypted = Convert.FromBase64String(value);
    }
    public string? Decrypted { get; set; }
    public IFormFile? OpenFile { get; set; }
    public IFormFile? SaveFile { get; set; }
    public List<string>? BruteForced { get; set; } = new List<string>();
    public Dictionary<char, int>? FrequencyTable { get; set; } = new Dictionary<char, int>();
}