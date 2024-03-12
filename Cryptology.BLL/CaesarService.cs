using Cryptology.Domain.Entity;
using Cryptology.Domain.Enum;
using Cryptology.Domain.Response;
using Cryptology.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Cryptology.BLL
{
    public class CaesarService
    {
        private readonly GlobalService _globalService;

        public CaesarService (GlobalService globalService)
        {
            _globalService = globalService;
        }

        public async Task<BaseResponse<CaesarViewModel>> Encrypt(CaesarViewModel caesar)
        {
            try
            {
                if (caesar == null)
                {
                    return new BaseResponse<CaesarViewModel>
                    {
                        Description = "Model is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                if (caesar.Key == null)
                {
                    return new BaseResponse<CaesarViewModel>
                    {
                        Description = "Key is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                if (caesar.Text == null)
                {
                    return new BaseResponse<CaesarViewModel>
                    {
                        Description = "Text is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                var result = new StringBuilder();

                if (_globalService.IsEnglish(caesar.Text))
                {
                    foreach (char c in caesar.Text)
                    {
                        if (char.IsLetter(c))
                        {
                            if (char.IsUpper(c))
                            {
                                char ch = (char)(((int)c + caesar.Key - 65) % 26 + 65);
                                result.Append(ch);
                            }
                            else
                            {
                                char ch = (char)(((int)c + caesar.Key - 97) % 26 + 97);
                                result.Append(ch);
                            }
                        }
                        else
                        {
                            result.Append(c);
                        }
                    }
                }
                else if (_globalService.IsUkrainian(caesar.Text))
                {
                    foreach (char c in caesar.Text)
                    {
                        if (char.IsLetter(c))
                        {
                            if (char.IsUpper(c))
                            {
                                char ch = (char)(((int)c + caesar.Key - 1040) % 33 + 1040);
                                result.Append(ch);
                            }
                            else
                            {
                                char ch = (char)(((int)c + caesar.Key - 1072) % 33 + 1072);
                                result.Append(ch);
                            }
                        }
                        else
                        {
                            result.Append(c);
                        }
                    }
                }
                else
                {
                        return new BaseResponse<CaesarViewModel>
                        {
                            Description = "Language does not support",
                            StatusCode = Domain.Enum.StatusCode.NotFound
                        };
                }

                caesar.Encrypted = result.ToString();

                return new BaseResponse<CaesarViewModel>
                {
                    Data = caesar,
                    Description = "Encryption successful",
                    StatusCode = Domain.Enum.StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<CaesarViewModel>()
                {
                    Description = $"[Encrypt] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<CaesarViewModel>> Decrypt(CaesarViewModel caesar)
        {
            try
            {
                if (caesar == null)
                {
                    return new BaseResponse<CaesarViewModel>
                    {
                        Description = "Model is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                if (caesar.Key == null)
                {
                    return new BaseResponse<CaesarViewModel>
                    {
                        Description = "Key is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                if (caesar.Encrypted == null)
                {
                    return new BaseResponse<CaesarViewModel>
                    {
                        Description = "Encrypted is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                var result = new StringBuilder();

                if (_globalService.IsEnglish(caesar.Encrypted))
                {
                    foreach (char c in caesar.Encrypted)
                    {
                        if (char.IsLetter(c))
                        {
                            if (char.IsUpper(c))
                            {
                                char ch = (char)(((int)c - caesar.Key - 65 + 26) % 26 + 65);
                                result.Append(ch);
                            }
                            else
                            {
                                char ch = (char)(((int)c - caesar.Key - 97 + 26) % 26 + 97);
                                result.Append(ch);
                            }
                        }
                        else
                        {
                            result.Append(c);
                        }
                    }
                }
                else if (_globalService.IsUkrainian(caesar.Encrypted))
                {
                    foreach (char c in caesar.Encrypted)
                    {
                        if (char.IsLetter(c))
                        {
                            if (char.IsUpper(c))
                            {
                                char ch = (char)(((int)c - caesar.Key - 1040 + 33) % 33 + 1040);
                                result.Append(ch);
                            }
                            else
                            {
                                char ch = (char)(((int)c - caesar.Key - 1072 + 33) % 33 + 1072);
                                result.Append(ch);
                            }
                        }
                        else
                        {
                            result.Append(c);
                        }
                    }
                }
                else
                {
                    return new BaseResponse<CaesarViewModel>
                    {
                        Description = "Language does not support",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                caesar.Decrypted = result.ToString();

                return new BaseResponse<CaesarViewModel>
                {
                    Data = caesar,
                    Description = "Decryption successful",
                    StatusCode = Domain.Enum.StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<CaesarViewModel>()
                {
                    Description = $"[Decrypt] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<CaesarViewModel>> BruteForce(CaesarViewModel caesar)
        {
            try
            {
                if (caesar == null)
                {
                    return new BaseResponse<CaesarViewModel>
                    {
                        Description = "Model is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                if (caesar.Encrypted == null)
                {
                    return new BaseResponse<CaesarViewModel>
                    {
                        Description = "Encrypted is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                if (_globalService.IsEnglish(caesar.Encrypted))
                {
                    for (int step = 1; step <= 26; step++)
                    {
                        var result = new StringBuilder();

                        foreach (char c in caesar.Encrypted)
                        {
                            if (char.IsLetter(c))
                            {
                                if (char.IsUpper(c))
                                {
                                    char ch = (char)(((int)c - step - 65 + 26) % 26 + 65);
                                    result.Append(ch);
                                }
                                else
                                {
                                    char ch = (char)(((int)c - step - 97 + 26) % 26 + 97);
                                    result.Append(ch);
                                }
                            }
                            else
                            {
                                result.Append(c);
                            }
                        }

                        caesar.BruteForced.Add(result.ToString());
                    }
                }
                else if (_globalService.IsUkrainian(caesar.Encrypted))
                {
                    for (int step = 1; step <= 33; step++)
                    {
                        var result = new StringBuilder();

                        foreach (char c in caesar.Encrypted)
                        {
                            if (char.IsLetter(c))
                            {
                                if (char.IsUpper(c))
                                {
                                    char ch = (char)(((int)c - step - 1040 + 33) % 33 + 1040);
                                    result.Append(ch);
                                }
                                else
                                {
                                    char ch = (char)(((int)c - step - 1072 + 33) % 33 + 1072);
                                    result.Append(ch);
                                }
                            }
                            else
                            {
                                result.Append(c);
                            }
                        }

                        caesar.BruteForced.Add(result.ToString());
                    }
                }
                else
                {
                    return new BaseResponse<CaesarViewModel>
                    {
                        Description = "Language does not support",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                if (caesar.BruteForced == null)
                {
                    return new BaseResponse<CaesarViewModel>
                    {
                        Description = "BruteForced is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                return new BaseResponse<CaesarViewModel>
                {
                    Data = caesar,
                    Description = "Brute force attack successful",
                    StatusCode = Domain.Enum.StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<CaesarViewModel>()
                {
                    Description = $"[BruteForce] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<CaesarViewModel>> OpenFromFile(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    return new BaseResponse<CaesarViewModel>()
                    {
                        Description = "File is null",
                        StatusCode = StatusCode.NotFound
                    };
                }

                if (!file.ContentType.Equals("text/plain", StringComparison.OrdinalIgnoreCase))
                {
                    return new BaseResponse<CaesarViewModel>()
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

                    if (lines.Count >= 2)
                    {
                        string numericPart = new string(lines[0].Where(char.IsDigit).ToArray());

                        if (int.TryParse(numericPart, out int key))
                        {
                            CaesarViewModel caesar = new CaesarViewModel
                            {
                                Key = key,
                                Text = lines[1]
                            };

                            return new BaseResponse<CaesarViewModel>()
                            {
                                Data = caesar,
                                StatusCode = StatusCode.OK
                            };
                        }
                        else
                        {
                            return new BaseResponse<CaesarViewModel>()
                            {
                                Description = "Unable to parse Key from the file.",
                                StatusCode = StatusCode.NotFound
                            };
                        }
                    }
                    else
                    {
                        return new BaseResponse<CaesarViewModel>()
                        {
                            Description = "Too few lines in the file.",
                            StatusCode = StatusCode.NotFound
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse<CaesarViewModel>()
                {
                    Description = $"[OpenFromFile] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<CaesarViewModel>> FrequencyTable(CaesarViewModel caesar)
        {
            try
            {
                if (caesar == null)
                {
                    return new BaseResponse<CaesarViewModel>
                    {
                        Description = "Model is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                if (caesar.Text == null)
                {
                    return new BaseResponse<CaesarViewModel>
                    {
                        Description = "Text is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                var frequencies = new Dictionary<char, int>();

                foreach (char c in caesar.Text.ToLower())
                {
                    if (char.IsLetter(c))
                    {
                        if (frequencies.ContainsKey(c))
                            frequencies[c]++;
                        else
                            frequencies[c] = 1;
                    }
                }

                caesar.FrequencyTable = frequencies.ToDictionary(
                    table => table.Key,
                    table => table.Value
                );

                return new BaseResponse<CaesarViewModel>()
                {
                    Data = caesar,
                    Description = "Text Analyzing successfully",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<CaesarViewModel>()
                {
                    Description = $"[AnalyzeText] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<CaesarViewModel>> SaveToFile(CaesarViewModel caesar)
        {
            try
            {
                if (caesar == null)
                {
                    return new BaseResponse<CaesarViewModel>()
                    {
                        Description = "CaesarViewModel is null",
                        StatusCode = StatusCode.NotFound
                    };
                }

                StringBuilder contentBuilder = new StringBuilder();

                contentBuilder.AppendLine($"Key - {caesar.Key};");
                contentBuilder.AppendLine($"Text - {caesar.Text}\n");
                contentBuilder.AppendLine($"Encrypted - {caesar.Encrypted}");
                contentBuilder.AppendLine($"Decrypted - {caesar.Decrypted}\n");

                var bruteForceResponse = await BruteForce(caesar);
                var frequencyTableResponse = await FrequencyTable(caesar);

                if (caesar.BruteForced != null && caesar.BruteForced.Count > 0)
                {
                    contentBuilder.AppendLine("Brute Force Results:");
                    foreach (var result in caesar.BruteForced)
                    {
                        if (result == caesar.Text)
                        {
                            contentBuilder.AppendLine($"{result} - Answer");
                        }
                        contentBuilder.AppendLine(result);
                    }
                }

                if (caesar.FrequencyTable != null && caesar.FrequencyTable.Count > 0)
                {
                    contentBuilder.AppendLine("\nFrequency Table Results:");

                    foreach (var res in caesar.FrequencyTable)
                    {
                        contentBuilder.AppendLine($"{res.Key}: {res.Value}");
                    }
                }

                string filePath = "path_to_save_file.txt";

                await File.WriteAllTextAsync(filePath, contentBuilder.ToString());

                return new BaseResponse<CaesarViewModel>()
                {
                    Data = caesar,
                    Description = "File saved successfully",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<CaesarViewModel>()
                {
                    Description = $"[SaveToFile] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<CaesarViewModel>> EncryptImage(CaesarViewModel caesar)
        {
            try
            {
                if (caesar == null)
                {
                    return new BaseResponse<CaesarViewModel>()
                    {
                        Description = "CaesarViewModel is null",
                        StatusCode = StatusCode.NotFound
                    };
                }

                if (caesar.InputImage == null)
                {
                    return new BaseResponse<CaesarViewModel>()
                    {
                        Description = "Image is null",
                        StatusCode = StatusCode.NotFound
                    };
                }

                byte[] imageBytes = await _globalService.ReadImageBytes(caesar.InputImage);
                for (int i = 0; i < imageBytes.Length; i++)
                {
                    imageBytes[i] = (byte)(imageBytes[i] + caesar.Key);
                }

                string outputPath = "D:\\МОК\\1\\encrypted.jpg";
                await _globalService.SaveImage(imageBytes, outputPath);

                return new BaseResponse<CaesarViewModel>()
                {
                    Data = caesar,
                    Description = $"File encrypted to {outputPath}",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<CaesarViewModel>()
                {
                    Description = $"[EncryptImage] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<CaesarViewModel>> DecryptImage(CaesarViewModel caesar)
        {
            try
            {
                if (caesar == null)
                {
                    return new BaseResponse<CaesarViewModel>()
                    {
                        Description = "CaesarViewModel is null",
                        StatusCode = StatusCode.NotFound
                    };
                }

                if (caesar.EncryptedImage == null)
                {
                    return new BaseResponse<CaesarViewModel>()
                    {
                        Description = "Encrypted Image is null",
                        StatusCode = StatusCode.NotFound
                    };
                }

                byte[] encryptedImageBytes = await _globalService.ReadImageBytes(caesar.EncryptedImage);

                for (int i = 0; i < encryptedImageBytes.Length; i++)
                {
                    encryptedImageBytes[i] = (byte)(encryptedImageBytes[i] - caesar.Key);
                }

                string outputPath = "D:\\МОК\\1\\decrypted.jpg";
                await _globalService.SaveImage(encryptedImageBytes, outputPath);

                return new BaseResponse<CaesarViewModel>()
                {
                    Data = caesar,
                    Description = $"File decrypted to {outputPath}",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<CaesarViewModel>()
                {
                    Description = $"[DecryptImage] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }       
    }
}