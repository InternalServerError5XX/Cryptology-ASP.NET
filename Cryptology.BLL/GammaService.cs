using System.Security.Cryptography;
using System.Text;
using Cryptology.Domain.Entity;
using Cryptology.Domain.Enum;
using Cryptology.Domain.Response;
using Cryptology.Domain.ViewModels;
using Microsoft.AspNetCore.Http;

namespace Cryptology.BLL;

public class GammaService
{
    private readonly GlobalService _globalService;

    public GammaService(GlobalService globalService)
    {
        _globalService = globalService;
    }
    
    public async Task<BaseResponse<GammaViewModel>> Encrypt(GammaViewModel model)
    {
        try
        {
            if (model == null)
            {
                return new BaseResponse<GammaViewModel>
                {
                    Description = "Model is null",
                    StatusCode = StatusCode.NotFound
                };
            }

            if (model.Text == null)
            {
                return new BaseResponse<GammaViewModel>
                {
                    Description = "Text is null",
                    StatusCode = StatusCode.NotFound
                };
            }

            var gamma = GenerateGamma(model.Text);
            model.Gamma = gamma;
            
            if (model.Gamma == null)
            {
                return new BaseResponse<GammaViewModel>
                {
                    Description = "Gamma is invalid",
                    StatusCode = StatusCode.NotFound
                };
            }
            
            var input = Encoding.UTF8.GetBytes(model.Text);
            var output = new byte[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                output[i] = (byte)(input[i] ^ model.Gamma[i % model.Gamma.Length]);
            }

            model.Encrypted = output;

            return new BaseResponse<GammaViewModel>
            {
                Data = model,
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<GammaViewModel>()
            {
                Description = $"[Encrypt] : {ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }
    
    public async Task<BaseResponse<GammaViewModel>> Decrypt(GammaViewModel model)
    {
        try
        {
            if (model == null)
            {
                return new BaseResponse<GammaViewModel>
                {
                    Description = "Model is null",
                    StatusCode = StatusCode.NotFound
                };
            }

            if (model.Encrypted == null)
            {
                return new BaseResponse<GammaViewModel>
                {
                    Description = "Encrypted is null",
                    StatusCode = StatusCode.NotFound
                };
            }
            
            if (model.Gamma == null)
            {
                return new BaseResponse<GammaViewModel>
                {
                    Description = "Gamma is invalid",
                    StatusCode = StatusCode.NotFound
                };
            }

            var input = model.Encrypted;
            var output = new byte[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                output[i] = (byte)(input[i] ^ model.Gamma[i % model.Gamma.Length]);
            }

            model.Decrypted = Encoding.UTF8.GetString(output);

            return new BaseResponse<GammaViewModel>
            {
                Data = model,
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<GammaViewModel>()
            {
                Description = $"[Decrypt] : {ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<BaseResponse<GammaViewModel>> OpenFromFile(IFormFile file)
    {
        try
        {
            if (file == null)
            {
                return new BaseResponse<GammaViewModel>()
                {
                    Description = "File is null",
                    StatusCode = StatusCode.NotFound
                };
            }

            if (!file.ContentType.Equals("text/plain", StringComparison.OrdinalIgnoreCase))
            {
                return new BaseResponse<GammaViewModel>()
                {
                    Description = "Invalid file type.",
                    StatusCode = StatusCode.NotFound
                };
            }

            using (StreamReader reader = new StreamReader(file.OpenReadStream()))
            {
                List<string> lines = new List<string>();

                while (!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();
                    lines.Add(line);
                }

                if (lines.Count == 1)
                {
                    GammaViewModel gammaViewModel = new GammaViewModel
                    {
                        Text = lines[0]
                    };

                    return new BaseResponse<GammaViewModel>()
                    {
                        Data = gammaViewModel,
                        StatusCode = StatusCode.OK
                    };
                }
                else
                {
                    return new BaseResponse<GammaViewModel>()
                    {
                        Description = "Invalid lines count in the file.",
                        StatusCode = StatusCode.NotFound
                    };
                }
            }
        }
        catch (Exception ex)
        {
            return new BaseResponse<GammaViewModel>()
            {
                Description = $"[OpenFromFile] : {ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }
    
    public async Task<BaseResponse<GammaViewModel>> FrequencyTable(GammaViewModel gammaViewModel)
    {
        try
        {
            if (gammaViewModel == null)
            {
                return new BaseResponse<GammaViewModel>
                {
                    Description = "Model is null",
                    StatusCode = Domain.Enum.StatusCode.NotFound
                };
            }

            if (gammaViewModel.Text == null)
            {
                return new BaseResponse<GammaViewModel>
                {
                    Description = "Text is null",
                    StatusCode = Domain.Enum.StatusCode.NotFound
                };
            }

            var frequencies = new Dictionary<char, int>();

            foreach (char c in gammaViewModel.Text.ToLower())
            {
                if (char.IsLetter(c))
                {
                    if (frequencies.ContainsKey(c))
                        frequencies[c]++;
                    else
                        frequencies[c] = 1;
                }
            }

            gammaViewModel.FrequencyTable = frequencies.ToDictionary(
                table => table.Key,
                table => table.Value
            );

            return new BaseResponse<GammaViewModel>()
            {
                Data = gammaViewModel,
                Description = "Text Analyzing successfully",
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<GammaViewModel>()
            {
                Description = $"[FrequencyTable] : {ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }
    
    public async Task<BaseResponse<GammaViewModel>> SaveToFile(GammaViewModel gammaViewModel)
    {
        try
        {
            if (gammaViewModel == null)
            {
                return new BaseResponse<GammaViewModel>()
                {
                    Description = "CaesarViewModel is null",
                    StatusCode = StatusCode.NotFound
                };
            }

            StringBuilder contentBuilder = new StringBuilder();

            contentBuilder.AppendLine($"Key - {gammaViewModel.GammaString};");
            contentBuilder.AppendLine($"Text - {gammaViewModel.Text}\n");
            contentBuilder.AppendLine($"Encrypted - {gammaViewModel.EncryptedString}");
            contentBuilder.AppendLine($"Decrypted - {gammaViewModel.Decrypted}\n");
            
            await FrequencyTable(gammaViewModel);

            if (gammaViewModel.FrequencyTable != null && gammaViewModel.FrequencyTable.Count > 0)
            {
                contentBuilder.AppendLine("\nFrequency Table Results:");

                foreach (var res in gammaViewModel.FrequencyTable)
                {
                    contentBuilder.AppendLine($"{res.Key}: {res.Value}");
                }
            }

            string filePath = "path_to_save_file.txt";

            await File.WriteAllTextAsync(filePath, contentBuilder.ToString());

            return new BaseResponse<GammaViewModel>()
            {
                Data = gammaViewModel,
                Description = "File saved successfully",
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<GammaViewModel>()
            {
                Description = $"[SaveToFile] : {ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }
    
    private byte[] GenerateGamma(string text)
    {
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            var length = text.Length;
            var randomByte = new byte[length];
            rng.GetBytes(randomByte);
            
            return randomByte;
        }
    }
}