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

                if (IsEnglish(caesar.Text))
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
                else if (IsUkrainian(caesar.Text))
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

                if (IsEnglish(caesar.Encrypted))
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
                else if (IsUkrainian(caesar.Encrypted))
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

                if (IsEnglish(caesar.Encrypted))
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
                else if (IsUkrainian(caesar.Encrypted))
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
                        int key;
                        if (int.TryParse(lines[0], out key))
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
                            Description = "Insufficient lines in the file.",
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

        public bool IsEnglish(string text)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(text, @"^[a-zA-Z\s.,-]+$");
        }

        public bool IsUkrainian(string text)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(text, @"^[а-яА-ЯіІїЇєЄґҐ\s.,-]+$");
        }
    }
}