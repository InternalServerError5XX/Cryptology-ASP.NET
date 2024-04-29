using Cryptology.Domain.Enum;
using Cryptology.Domain.Response;
using Cryptology.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Cryptology.BLL
{
    public class RsaService
    {
        private RSACryptoServiceProvider? _rsaProvider = new RSACryptoServiceProvider();

        public async Task<BaseResponse<RsaViewModel>> GenerateKeys(RsaViewModel rsa)
        {
            try
            {
                rsa.PublicKey = _rsaProvider.ToXmlString(false);
                rsa.PrivateKey = _rsaProvider.ToXmlString(true);

                return await Task.Run(() => new BaseResponse<RsaViewModel>()
                {
                    Data = rsa,
                    StatusCode = StatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return new BaseResponse<RsaViewModel>()
                {
                    Description = $"[GenerateKeys] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<RsaViewModel>> Encrypt(RsaViewModel rsa)
        {
            try
            {
                if (rsa.Text == null)
                {
                    return new BaseResponse<RsaViewModel>
                    {
                        Description = "Text is null",
                        StatusCode = StatusCode.NotFound
                    };
                }

                await GenerateKeys(rsa);
                byte[] dataToEncrypt = Encoding.UTF8.GetBytes(rsa.Text);

                _rsaProvider.FromXmlString(rsa.PublicKey);
                rsa.Encrypted = _rsaProvider.Encrypt(dataToEncrypt, false);

                return await Task.Run(() => new BaseResponse<RsaViewModel>()
                {
                    Data = rsa,
                    StatusCode = StatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return new BaseResponse<RsaViewModel>()
                {
                    Description = $"[Encrypt] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<RsaViewModel>> Decrypt(RsaViewModel rsa)
        {
            try
            {
                if (rsa.Encrypted == null)
                {
                    return new BaseResponse<RsaViewModel>
                    {
                        Description = "Encrypted is null",
                        StatusCode = StatusCode.NotFound
                    };
                }

                _rsaProvider.FromXmlString(rsa.PrivateKey);
                var decryptedData = _rsaProvider.Decrypt(rsa.Encrypted, false);
                rsa.Decrypted = Encoding.UTF8.GetString(decryptedData);

                return await Task.Run(() => new BaseResponse<RsaViewModel>()
                {
                    Data = rsa,
                    StatusCode = StatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return new BaseResponse<RsaViewModel>()
                {
                    Description = $"[Decrypt] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<RsaViewModel>> OpenFromFile(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    return new BaseResponse<RsaViewModel>()
                    {
                        Description = "File is null",
                        StatusCode = StatusCode.NotFound
                    };
                }

                if (!file.ContentType.Equals("text/plain", StringComparison.OrdinalIgnoreCase))
                {
                    return new BaseResponse<RsaViewModel>()
                    {
                        Description = "Invalid file type.",
                        StatusCode = StatusCode.NotFound
                    };
                }

                using StreamReader reader = new StreamReader(file.OpenReadStream());
                List<string> lines = new List<string>();

                while (!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();
                    lines.Add(line);
                }

                if (lines.Count == 1)
                {
                    RsaViewModel rsaViewModel = new RsaViewModel
                    {
                        Text = lines[0]
                    };

                    return await Task.Run(() => new BaseResponse<RsaViewModel>()
                    {
                        Data = rsaViewModel,
                        StatusCode = StatusCode.OK
                    });
                }
                else
                {
                    return new BaseResponse<RsaViewModel>()
                    {
                        Description = "Invalid lines count in the file.",
                        StatusCode = StatusCode.NotFound
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse<RsaViewModel>()
                {
                    Description = $"[OpenFromFile] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<RsaViewModel>> FrequencyTable(RsaViewModel rsaViewModel)
        {
            try
            {
                if (rsaViewModel == null)
                {
                    return new BaseResponse<RsaViewModel>
                    {
                        Description = "Model is null",
                        StatusCode = StatusCode.NotFound
                    };
                }

                if (rsaViewModel.Text == null)
                {
                    return new BaseResponse<RsaViewModel>
                    {
                        Description = "Text is null",
                        StatusCode = StatusCode.NotFound
                    };
                }

                var frequencies = new Dictionary<char, int>();

                foreach (char c in rsaViewModel.Text.ToLower())
                {
                    if (char.IsLetter(c))
                    {
                        if (frequencies.ContainsKey(c))
                            frequencies[c]++;
                        else
                            frequencies[c] = 1;
                    }
                }

                rsaViewModel.FrequencyTable = frequencies.ToDictionary(
                    table => table.Key,
                    table => table.Value
                );

                return await Task.Run(() => new BaseResponse<RsaViewModel>()
                {
                    Data = rsaViewModel,
                    Description = "Text Analyzing successfully",
                    StatusCode = StatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return new BaseResponse<RsaViewModel>()
                {
                    Description = $"[FrequencyTable] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<RsaViewModel>> SaveToFile(RsaViewModel rsaViewModel)
        {
            try
            {
                if (rsaViewModel == null)
                {
                    return new BaseResponse<RsaViewModel>()
                    {
                        Description = "KnapsackViewModel is null",
                        StatusCode = StatusCode.NotFound
                    };
                }

                StringBuilder contentBuilder = new StringBuilder();

                contentBuilder.AppendLine($"OpenKey - {rsaViewModel.PublicKey};");
                contentBuilder.AppendLine($"CloseKey - {rsaViewModel.PrivateKey};");
                contentBuilder.AppendLine($"Text - {rsaViewModel.Text}\n");
                
                if (rsaViewModel.Encrypted != null)
                {
                    contentBuilder.AppendLine($"Encrypted - {BitConverter.ToString(rsaViewModel.Encrypted)}");
                }

                contentBuilder.AppendLine($"Decrypted - {rsaViewModel.Decrypted}\n");

                await FrequencyTable(rsaViewModel);

                if (rsaViewModel.FrequencyTable != null && rsaViewModel.FrequencyTable.Count > 0)
                {
                    contentBuilder.AppendLine("Frequency Table Results:");

                    foreach (var res in rsaViewModel.FrequencyTable.OrderByDescending(s => s.Value))
                    {
                        contentBuilder.AppendLine($"{res.Key}: {res.Value}");
                    }
                }

                string filePath = "path_to_save_file.txt";

                await File.WriteAllTextAsync(filePath, contentBuilder.ToString());

                return await Task.Run(() => new BaseResponse<RsaViewModel>()
                {
                    Data = rsaViewModel,
                    Description = "File saved successfully",
                    StatusCode = StatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return new BaseResponse<RsaViewModel>()
                {
                    Description = $"[SaveToFile] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
