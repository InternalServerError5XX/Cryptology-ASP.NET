using Cryptology.Domain.Enum;
using Cryptology.Domain.Response;
using Cryptology.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptology.BLL
{
    public class TrithemiusService
    {
        private readonly GlobalService _globalService;

        public TrithemiusService(GlobalService globalService)
        {
            _globalService = globalService;
        }

        public async Task<BaseResponse<TrithemiusViewModel>> Encrypt(TrithemiusViewModel trithemius)
        {
            try
            {
                if (trithemius == null)
                {
                    return new BaseResponse<TrithemiusViewModel>
                    {
                        Description = "Model is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                if (!Enum.IsDefined(typeof(KeyType), trithemius.KeyType))
                {
                    return new BaseResponse<TrithemiusViewModel>
                    {
                        Description = "Invalid key type",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                if (trithemius.Text == null)
                {
                    return new BaseResponse<TrithemiusViewModel>
                    {
                        Description = "Text is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                var result = new StringBuilder();
                switch (trithemius.KeyType)
                {
                    case KeyType.LinearEquation:
                        if (trithemius.LinearCoefficientA == null ||
                            trithemius.LinearCoefficientB == null)
                        {
                            return new BaseResponse<TrithemiusViewModel>
                            {
                                Description = "Coefficients invlad",
                                StatusCode = Domain.Enum.StatusCode.NotFound
                            };
                        }

                        result.Append(EncryptWithLinear
                            (trithemius.LinearCoefficientA, trithemius.LinearCoefficientB, trithemius.Text));
                        break;

                    case KeyType.NonlinearEquation:
                        if (trithemius.NonlinearCoefficientA == null ||
                            trithemius.NonlinearCoefficientB == null ||
                            trithemius.NonlinearCoefficientC == null)
                        {
                            return new BaseResponse<TrithemiusViewModel>
                            {
                                Description = "Coefficients invlad",
                                StatusCode = Domain.Enum.StatusCode.NotFound
                            };
                        }

                        result.Append(EncryptWithNonlinear
                            (trithemius.Text, trithemius.Position, trithemius.NonlinearCoefficientA, 
                            trithemius.NonlinearCoefficientB, trithemius.NonlinearCoefficientC));
                        break;

                    case KeyType.Password:
                        if (trithemius.Password == null)
                        {
                            return new BaseResponse<TrithemiusViewModel>
                            {
                                Description = "Password is null",
                                StatusCode = Domain.Enum.StatusCode.NotFound
                            };
                        }

                        result.Append(EncryptWithPassword(trithemius.Password, trithemius.Text));
                        break;

                    default:
                        return new BaseResponse<TrithemiusViewModel>
                        {
                            Description = "Invalid key type",
                            StatusCode = Domain.Enum.StatusCode.NotFound
                        };
                }

                trithemius.Encrypted = result.ToString();

                return new BaseResponse<TrithemiusViewModel>
                {
                    Data = trithemius,
                    Description = "Encryption successful",
                    StatusCode = Domain.Enum.StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<TrithemiusViewModel>()
                {
                    Description = $"[Encrypt] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<TrithemiusViewModel>> Decrypt(TrithemiusViewModel trithemius)
        {
            try
            {
                if (trithemius == null)
                {
                    return new BaseResponse<TrithemiusViewModel>
                    {
                        Description = "Model is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                if (!Enum.IsDefined(typeof(KeyType), trithemius.KeyType))
                {
                    return new BaseResponse<TrithemiusViewModel>
                    {
                        Description = "Invalid key type",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                if (trithemius.Text == null)
                {
                    return new BaseResponse<TrithemiusViewModel>
                    {
                        Description = "Text is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                var result = new StringBuilder();
                switch (trithemius.KeyType)
                {
                    case KeyType.LinearEquation:
                        if (trithemius.LinearCoefficientA == null ||
                            trithemius.LinearCoefficientB == null)
                        {
                            return new BaseResponse<TrithemiusViewModel>
                            {
                                Description = "Coefficients invalid",
                                StatusCode = Domain.Enum.StatusCode.NotFound
                            };
                        }

                        result.Append(DecryptWithLinear
                            (trithemius.LinearCoefficientA, trithemius.LinearCoefficientB, trithemius.Encrypted));
                        break;

                    case KeyType.NonlinearEquation:
                        if (trithemius.NonlinearCoefficientA == null ||
                            trithemius.NonlinearCoefficientB == null ||
                            trithemius.NonlinearCoefficientC == null)
                        {
                            return new BaseResponse<TrithemiusViewModel>
                            {
                                Description = "Coefficients invalid",
                                StatusCode = Domain.Enum.StatusCode.NotFound
                            };
                        }

                        result.Append(DecryptWithNonlinear
                            (trithemius.Encrypted, trithemius.Position, trithemius.NonlinearCoefficientA,
                            trithemius.NonlinearCoefficientB, trithemius.NonlinearCoefficientC));
                        break;

                    case KeyType.Password:
                        if (trithemius.Password == null)
                        {
                            return new BaseResponse<TrithemiusViewModel>
                            {
                                Description = "Password is null",
                                StatusCode = Domain.Enum.StatusCode.NotFound
                            };
                        }

                        result.Append(DecryptWithPassword(trithemius.Password, trithemius.Encrypted));
                        break;

                    default:
                        return new BaseResponse<TrithemiusViewModel>
                        {
                            Description = "Invalid key type",
                            StatusCode = Domain.Enum.StatusCode.NotFound
                        };
                }

                trithemius.Decrypted = result.ToString();

                return new BaseResponse<TrithemiusViewModel>
                {
                    Data = trithemius,
                    Description = "Decryption successful",
                    StatusCode = Domain.Enum.StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<TrithemiusViewModel>()
                {
                    Description = $"[Decrypt] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<TrithemiusViewModel>> FrequencyTable(TrithemiusViewModel trithemius)
        {
            try
            {
                if (trithemius == null)
                {
                    return new BaseResponse<TrithemiusViewModel>
                    {
                        Description = "Model is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                if (trithemius.Text == null)
                {
                    return new BaseResponse<TrithemiusViewModel>
                    {
                        Description = "Text is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                var frequencies = new Dictionary<char, int>();

                foreach (char c in trithemius.Text.ToLower())
                {
                    if (char.IsLetter(c))
                    {
                        if (frequencies.ContainsKey(c))
                            frequencies[c]++;
                        else
                            frequencies[c] = 1;
                    }
                }

                trithemius.FrequencyTable = frequencies.ToDictionary(
                    table => table.Key,
                    table => table.Value
                );

                return new BaseResponse<TrithemiusViewModel>()
                {
                    Data = trithemius,
                    Description = "Text Analyzing successfully",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<TrithemiusViewModel>()
                {
                    Description = $"[AnalyzeText] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<TrithemiusViewModel>> OpenFromFile(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    return new BaseResponse<TrithemiusViewModel>()
                    {
                        Description = "File is null",
                        StatusCode = StatusCode.NotFound
                    };
                }

                if (!file.ContentType.Equals("text/plain", StringComparison.OrdinalIgnoreCase))
                {
                    return new BaseResponse<TrithemiusViewModel>()
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
                        var line = await reader.ReadLineAsync();
                        lines.Add(line);
                    }

                    if (lines.Count >= 3)
                    {
                        string numericPart = new string(lines[0].Where(char.IsDigit).ToArray());

                        if (int.TryParse(numericPart, out int key))
                        {
                            var trithemius = new TrithemiusViewModel
                            {
                                KeyType = (KeyType)key
                            };

                            switch (trithemius.KeyType)
                            {
                                case KeyType.Password:
                                    trithemius.Password = lines[1];
                                    trithemius.Text = lines[2];
                                    break;
                                case KeyType.LinearEquation:
                                    trithemius.LinearCoefficientA = int.TryParse(lines[1], out int linearA) ? (int?)linearA : null;
                                    trithemius.LinearCoefficientB = int.TryParse(lines[2], out int linearB) ? (int?)linearB : null;
                                    trithemius.Text = lines[3];
                                    break;
                                case KeyType.NonlinearEquation:
                                    trithemius.NonlinearCoefficientA = int.TryParse(lines[1], out int nonlinearA) ? (int?)nonlinearA : null;
                                    trithemius.NonlinearCoefficientB = int.TryParse(lines[2], out int nonlinearB) ? (int?)nonlinearB : null;
                                    trithemius.NonlinearCoefficientC = int.TryParse(lines[3], out int nonlinearC) ? (int?)nonlinearC : null;
                                    trithemius.Position = int.TryParse(lines[4], out int position) ? (int?)nonlinearC : null;
                                    trithemius.Text = lines[5];
                                    break;
                                default:
                                    return new BaseResponse<TrithemiusViewModel>()
                                    {
                                        Description = "Invalid file.",
                                        StatusCode = StatusCode.NotFound
                                    };

                            }

                            return new BaseResponse<TrithemiusViewModel>()
                            {
                                Data = trithemius,
                                StatusCode = StatusCode.OK
                            };
                        }
                        else
                        {
                            return new BaseResponse<TrithemiusViewModel>()
                            {
                                Description = "Unable to parse Key from the file.",
                                StatusCode = StatusCode.NotFound
                            };
                        }
                    }
                    else
                    {
                        return new BaseResponse<TrithemiusViewModel>()
                        {
                            Description = "Too few lines in the file.",
                            StatusCode = StatusCode.NotFound
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse<TrithemiusViewModel>()
                {
                    Description = $"[OpenFromFile] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<TrithemiusViewModel>> SaveToFile(TrithemiusViewModel trithemius)
        {
            try
            {
                if (trithemius == null)
                {
                    return new BaseResponse<TrithemiusViewModel>()
                    {
                        Description = "TrithemiusViewModel is null",
                        StatusCode = StatusCode.NotFound
                    };
                }

                StringBuilder contentBuilder = new StringBuilder();

                contentBuilder.AppendLine($"Text - {trithemius.Text}\n");
                contentBuilder.AppendLine($"Encrypted - {trithemius.Encrypted}");
                contentBuilder.AppendLine($"Decrypted - {trithemius.Decrypted}\n");

                var bruteForceResponse = await BruteForce(trithemius);
                var frequencyTableResponse = await FrequencyTable(trithemius);

                if (trithemius.BruteForced != null && trithemius.BruteForced.Count > 0)
                {
                    contentBuilder.AppendLine("Brute Force Results:");
                    foreach (var result in trithemius.BruteForced)
                    {
                        if (result == trithemius.Text)
                        {
                            contentBuilder.AppendLine($"{result} - Answer");
                        }
                        contentBuilder.AppendLine(result);
                    }
                }

                if (trithemius.FrequencyTable != null && trithemius.FrequencyTable.Count > 0)
                {
                    contentBuilder.AppendLine("\nFrequency Table Results:");

                    foreach (var res in trithemius.FrequencyTable)
                    {
                        contentBuilder.AppendLine($"{res.Key}: {res.Value}");
                    }
                }

                string filePath = "path_to_save_file.txt";

                await File.WriteAllTextAsync(filePath, contentBuilder.ToString());

                return new BaseResponse<TrithemiusViewModel>()
                {
                    Data = trithemius,
                    Description = "File saved successfully",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<TrithemiusViewModel>()
                {
                    Description = $"[SaveToFile] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public string EncryptWithLinear(int? a, int? b, string text)
        {
            StringBuilder encryptedText = new StringBuilder();

            foreach (char character in text)
            {
                if (char.IsLetter(character))
                {
                    if (_globalService.IsEnglish(text))
                    {
                        if (char.IsUpper(character))
                        {
                            int x = character - 'A';
                            int? y = (a * x + b) % 26;
                            char encryptedChar = (char)(y + 'A');
                            encryptedText.Append(encryptedChar);
                        }
                        if (char.IsLower(character))
                        {
                            int x = character - 'a';
                            int? y = (a * x + b) % 26;
                            char encryptedChar = (char)(y + 'a');
                            encryptedText.Append(encryptedChar);
                        }
                    }
                    else if (_globalService.IsUkrainian(text))
                    {
                        if (char.IsUpper(character))
                        {
                            int x = character - 'А';
                            int? y = (a * x + b) % 32;
                            char encryptedChar = (char)(y + 'А');
                            encryptedText.Append(encryptedChar);
                        }
                        if (char.IsLower(character))
                        {
                            int x = character - 'а';
                            int? y = (a * x + b) % 32;
                            char encryptedChar = (char)(y + 'а');
                            encryptedText.Append(encryptedChar);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Language does not support");
                    }
                }
                else
                {
                    encryptedText.Append(character);
                }
            }
            return encryptedText.ToString();
        }


        public string EncryptWithNonlinear(string text, int? p, int? a, int? b, int? c)
        {
            StringBuilder encryptedText = new StringBuilder();

            foreach (char character in text)
            {
                if (char.IsLetter(character))
                {
                    int? k = NonlinearShift(p, a, b, c);
                    if (_globalService.IsEnglish(text))
                    {
                        if (char.IsUpper(character))
                        {
                            int x = character - 'A';
                            int? y = (x + k) % 26;
                            char encryptedChar = (char)(y + 'A');
                            encryptedText.Append(encryptedChar);
                        }
                        if (char.IsLower(character))
                        {
                            int x = character - 'a';
                            int? y = (x + k) % 26;
                            char encryptedChar = (char)(y + 'a');
                            encryptedText.Append(encryptedChar);
                        }
                    }
                    else if (_globalService.IsUkrainian(text))
                    {
                        if (char.IsUpper(character))
                        {
                            int x = character - 'А';
                            int? y = (x + k) % 32;
                            char encryptedChar = (char)(y + 'А');
                            encryptedText.Append(encryptedChar);
                        }
                        if (char.IsLower(character))
                        {
                            int x = character - 'а';
                            int? y = (x + k) % 32;
                            char encryptedChar = (char)(y + 'а');
                            encryptedText.Append(encryptedChar);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Language does not support");
                    }
                }
                else
                {
                    encryptedText.Append(character);
                }
            }

            return encryptedText.ToString();
        }

        public string EncryptWithPassword(string? password, string text)
        {
            StringBuilder encryptedText = new StringBuilder();

            int passwordLength = password.Length;
            int passwordIndex = 0;

            foreach (char character in text)
            {
                if (char.IsLetter(character))
                {
                    int shift, alphabetSize, baseChar;
                    if (_globalService.IsEnglish(text))
                    {
                        shift = (int)char.ToUpperInvariant(password[passwordIndex]) - 'A';
                        alphabetSize = 26;
                        baseChar = 'A';
                    }
                    else if (_globalService.IsUkrainian(text))
                    {
                        shift = (int)char.ToUpperInvariant(password[passwordIndex]) - 'А';
                        alphabetSize = 32;
                        baseChar = 'А';
                    }
                    else
                    {
                        throw new InvalidOperationException("Language does not support");
                    }

                    int charShift = (int)char.ToUpperInvariant(character) - baseChar;
                    int encryptedChar = (charShift + shift) % alphabetSize;
                    if (encryptedChar < 0)
                    {
                        encryptedChar += alphabetSize;
                    }

                    if (char.IsUpper(character))
                    {
                        encryptedText.Append((char)(encryptedChar + baseChar));
                    }
                    else
                    {
                        encryptedText.Append((char)(encryptedChar + baseChar + 32));
                    }

                    passwordIndex = (passwordIndex + 1) % passwordLength;
                }
                else
                {
                    encryptedText.Append(character);
                }
            }

            return encryptedText.ToString();
        }

        public string DecryptWithLinear(int? a, int? b, string text)
        {
            StringBuilder decryptedText = new StringBuilder();

            foreach (char character in text)
            {
                if (char.IsLetter(character))
                {
                    if (_globalService.IsEnglish(text))
                    {
                        if (char.IsUpper(character))
                        {
                            int y = character - 'A';
                            int? x = (ModInverse(a, 26) * (y - b + 26)) % 26;
                            char decryptedChar = (char)(x + 'A');
                            decryptedText.Append(decryptedChar);
                        }
                        if (char.IsLower(character))
                        {
                            int y = character - 'a';
                            int? x = (ModInverse(a, 26) * (y - b + 26)) % 26;
                            char decryptedChar = (char)(x + 'a');
                            decryptedText.Append(decryptedChar);
                        }
                    }
                    else if (_globalService.IsUkrainian(text))
                    {
                        if (char.IsUpper(character))
                        {
                            int y = character - 'А';
                            int? x = (ModInverse(a, 32) * (y - b + 32)) % 32;
                            char decryptedChar = (char)(x + 'А');
                            decryptedText.Append(decryptedChar);
                        }
                        if (char.IsLower(character))
                        {
                            int y = character - 'а';
                            int? x = (ModInverse(a, 32) * (y - b + 32)) % 32;
                            char decryptedChar = (char)(x + 'а');
                            decryptedText.Append(decryptedChar);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Language does not support");
                    }
                }
                else
                {
                    decryptedText.Append(character);
                }
            }

            return decryptedText.ToString();
        }

        public string DecryptWithNonlinear(string text, int? p, int? a, int? b, int? c)
        {
            StringBuilder decryptedText = new StringBuilder();

            foreach (char character in text)
            {
                if (char.IsLetter(character))
                {
                    int? k = NonlinearShift(p, a, b, c);
                    if (_globalService.IsEnglish(text))
                    {
                        if (char.IsUpper(character))
                        {
                            int y = character - 'A';
                            int? x = (y + 26 - (k % 26)) % 26;
                            char decryptedChar = (char)(x + 'A');
                            decryptedText.Append(decryptedChar);
                        }
                        if (char.IsLower(character))
                        {
                            int y = character - 'a';
                            int? x = (y + 26 - (k % 26)) % 26;
                            char decryptedChar = (char)(x + 'a');
                            decryptedText.Append(decryptedChar);
                        }
                    }
                    else if (_globalService.IsUkrainian(text))
                    {
                        if (char.IsUpper(character))
                        {
                            int y = character - 'А';
                            int? x = (y + 32 - (k % 32)) % 32;
                            char decryptedChar = (char)(x + 'А');
                            decryptedText.Append(decryptedChar);
                        }
                        if (char.IsLower(character))
                        {
                            int y = character - 'а';
                            int? x = (y + 32 - (k % 32)) % 32;
                            char decryptedChar = (char)(x + 'а');
                            decryptedText.Append(decryptedChar);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Language does not support");
                    }
                }
                else
                {
                    decryptedText.Append(character);
                }
            }

            return decryptedText.ToString();
        }

        public string DecryptWithPassword(string? password, string text)
        {
            StringBuilder decryptedText = new StringBuilder();

            int passwordLength = password.Length;
            int passwordIndex = 0;

            foreach (char character in text)
            {
                if (char.IsLetter(character))
                {
                    int shift, alphabetSize, baseChar;
                    if (_globalService.IsEnglish(text))
                    {
                        shift = (int)char.ToUpperInvariant(password[passwordIndex]) - 'A';
                        alphabetSize = 26;
                        baseChar = 'A';
                    }
                    else if (_globalService.IsUkrainian(text))
                    {
                        shift = (int)char.ToUpperInvariant(password[passwordIndex]) - 'А';
                        alphabetSize = 32;
                        baseChar = 'А';
                    }
                    else
                    {
                        throw new InvalidOperationException("Language does not support");
                    }

                    int charShift = (int)char.ToUpperInvariant(character) - baseChar;
                    int decryptedChar = (charShift - shift) % alphabetSize;

                    if (decryptedChar < 0)
                    {
                        decryptedChar += alphabetSize;
                    }

                    if (char.IsUpper(character))
                    {
                        decryptedText.Append((char)(decryptedChar + baseChar));
                    }
                    else
                    {
                        decryptedText.Append((char)(decryptedChar + baseChar + 32));
                    }

                    passwordIndex = (passwordIndex + 1) % passwordLength;
                }
                else
                {
                    decryptedText.Append(character);
                }
            }

            return decryptedText.ToString();
        }

        public async Task<BaseResponse<TrithemiusViewModel>> BruteForce(TrithemiusViewModel trithemius)
        {
            try
            {
                if (trithemius == null)
                {
                    return new BaseResponse<TrithemiusViewModel>
                    {
                        Description = "Model is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                if (trithemius.Encrypted == null)
                {
                    return new BaseResponse<TrithemiusViewModel>
                    {
                        Description = "Encrypted is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                var result = new StringBuilder();
                switch (trithemius.KeyType)
                {
                    case KeyType.LinearEquation:
                        await BruteForceLinear(trithemius);
                        break;

                    case KeyType.NonlinearEquation:
                        await BruteForceNonlinear(trithemius);
                        break;

                    case KeyType.Password:
                        await BruteForcePassword(trithemius);
                        break;

                    default:
                        return new BaseResponse<TrithemiusViewModel>
                        {
                            Description = "Invalid key type",
                            StatusCode = Domain.Enum.StatusCode.NotFound
                        };
                }

                if (trithemius.BruteForced == null)
                {
                    return new BaseResponse<TrithemiusViewModel>
                    {
                        Description = "BruteForced is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                return new BaseResponse<TrithemiusViewModel>
                {
                    Data = trithemius,
                    Description = "Brute force attack successful",
                    StatusCode = Domain.Enum.StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<TrithemiusViewModel>()
                {
                    Description = $"[BruteForce] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        private static int? ModInverse(int? a, int n)
        {
            int? t = 0, newT = 1, r = n, newR = a;

            while (newR != 0)
            {
                int? quotient = r / newR;

                int? tempT = t;
                t = newT;
                newT = tempT - quotient * newT;

                int? tempR = r;
                r = newR;
                newR = tempR - quotient * newR;
            }

            if (r > 1)
            {
                return -1;
            }

            if (t < 0)
            {
                t += n;
            }

            return t;
        }

        private static int? NonlinearShift(int? position, int? A, int? B, int? C)
        {
            return A * position * position + B * position + C;
        }

        private async Task BruteForceLinear(TrithemiusViewModel trithemius)
        {
            if (_globalService.IsEnglish(trithemius.Text))
            {
                for (int a = 1; a < 26; a++)
                {
                    for (int b = 0; b < 26; b++)
                    {
                        var result = new StringBuilder();

                        string decryptedText = DecryptWithLinear(a, b, trithemius.Encrypted);                        

                        if (decryptedText == trithemius.Text)
                        {
                            trithemius.BruteForced.Add($"Linear: a={a}, b={b}, Answer - {decryptedText}");
                            return;
                        }
                        trithemius.BruteForced.Add($"Linear: a={a}, b={b}, {result}");
                    }
                }
            }
            if (_globalService.IsUkrainian(trithemius.Text))
            {
                for (int a = 1; a < 32; a++)
                {
                    for (int b = 0; b < 32; b++)
                    {
                        var result = new StringBuilder();

                        string decryptedText = DecryptWithLinear(a, b, trithemius.Encrypted);                        

                        if (decryptedText == trithemius.Text)
                        {
                            trithemius.BruteForced.Add($"Linear: a={a}, b={b}, Answer - {decryptedText}");
                            return;
                        }
                        trithemius.BruteForced.Add($"Linear: a={a}, b={b}, {result}");
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Language does not support");
            }
        }

        private async Task BruteForceNonlinear(TrithemiusViewModel trithemius)
        {
            if (_globalService.IsEnglish(trithemius.Text))
            {
                for (int a = 1; a < 10; a++)
                {
                    for (int b = 0; b < 10; b++)
                    {
                        for (int c = 0; c < 10; c++)
                        {
                            for (int position = 0; position < 10; position++)
                            {
                                var result = new StringBuilder();

                                string decryptedText = DecryptWithNonlinear(trithemius.Encrypted, position, a, b, c);                              

                                if (decryptedText == trithemius.Text)
                                {
                                    trithemius.BruteForced.Add($"Nonlinear: a={a}, b={b}, c={c}, " +
                                        $"position={position}, Answer - {decryptedText}");
                                    return;
                                }
                                trithemius.BruteForced.Add($"Nonlinear: a={a}, b={b}, c={c}, " +
                                    $"position={position}, {result}");
                            }
                        }
                    }
                }
            }
            else if (_globalService.IsUkrainian(trithemius.Text))
            {
                for (int a = 1; a < 10; a++)
                {
                    for (int b = 0; b < 10; b++)
                    {
                        for (int c = 0; c < 10; c++)
                        {
                            for (int position = 0; position < 10; position++)
                            {
                                var result = new StringBuilder();

                                string decryptedText = DecryptWithNonlinear(trithemius.Encrypted, position, a, b, c);                               

                                if (decryptedText == trithemius.Text)
                                {
                                    trithemius.BruteForced.Add($"Nonlinear: a={a}, b={b}, c={c}, " +
                                        $"position={position},Answer - {decryptedText}");
                                    return;
                                }
                                trithemius.BruteForced.Add($"Nonlinear: a={a}, b={b}, c={c}, " +
                                    $"position={position}, {result}");
                            }
                        }
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Language does not support");
            }
        }

        private async Task BruteForcePassword(TrithemiusViewModel trithemius)
        {
            if (_globalService.IsEnglish(trithemius.Text))
            {
                for (int length = 1; length <= 26; length++)
                {
                    var permutations = GetPasswordPermutations(length, "abcdefghijklmnopqrstuvwxyz");

                    foreach (var passwordAttempt in permutations)
                    {
                        var result = new StringBuilder();

                        string decryptedText = DecryptWithPassword(passwordAttempt, trithemius.Encrypted);                     

                        if (decryptedText == trithemius.Text)
                        {
                            trithemius.BruteForced.Add($"Password: {passwordAttempt}, Answer - {decryptedText}");
                            return;
                        }
                        trithemius.BruteForced.Add($"Password: {passwordAttempt}, {result}");
                    }
                }
            }
            else if (_globalService.IsUkrainian(trithemius.Text))
            {
                for (int length = 1; length <= 32; length++)
                {
                    var permutations = GetPasswordPermutations(length, "абвгґдеєжзиіїйклмнопрстуфхцчшщьюя");

                    foreach (var passwordAttempt in permutations)
                    {
                        var result = new StringBuilder();

                        string decryptedText = DecryptWithPassword(passwordAttempt, trithemius.Encrypted);                        

                        if (decryptedText == trithemius.Text)
                        {
                            trithemius.BruteForced.Add($"Password: {passwordAttempt}, Answer - {decryptedText}");
                            return;
                        }
                        trithemius.BruteForced.Add($"Password: {passwordAttempt}, {result}");
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Language does not support");
            }
        }

        private IEnumerable<string> GetPasswordPermutations(int length, string characters)
        {
            var permutations = from c in characters
                               from p in GetPasswordPermutations(length - 1, characters)
                               select c + p;

            return length == 1 ? characters.Select(c => c.ToString()) : permutations;
        }

    }
}
