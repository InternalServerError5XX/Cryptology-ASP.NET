using Cryptology.Domain.Entity;
using Cryptology.Domain.Enum;
using Cryptology.Domain.Response;
using Cryptology.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptology.BLL
{
    public class KnapsackService
    {
        private readonly GlobalService _globalService;
        private readonly Random _random = new Random();

        public KnapsackService(GlobalService globalService)
        {
            _globalService = globalService;
        }

        public async Task<BaseResponse<KnapsackViewModel>> GenerateKeys(int n, KnapsackViewModel knapsack)
        {
            try
            {
                knapsack.CloseKey = GenerateSuperIncreasingSequence(n);
                int sum = knapsack.CloseKey.Sum();
                knapsack.M = sum + _random.Next(10, 100);
                knapsack.T = knapsack.M * _random.Next(2, 10) + 1;
                knapsack.OpenKey = GenerateOpenKey(knapsack.CloseKey, knapsack.M, knapsack.T);

                return new BaseResponse<KnapsackViewModel>
                {
                    Data = knapsack,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<KnapsackViewModel>()
                {
                    Description = $"[GenerateKeys] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<KnapsackViewModel>> Encrypt(KnapsackViewModel knapsack)
        {
            try
            {
                await GenerateKeys(8, knapsack);
                int[] openKey = knapsack.OpenKey.ToArray();
                string text = knapsack.Text;
                int n = openKey.Length;
                var binaryBlockText = ToBinBlocks(knapsack.Text, n);
                int?[] ciphertext = binaryBlockText
                    .Select(block => (int?)block.Select((bit, i) => (bit - '0') * openKey[i]).Sum())
                    .ToArray();

                knapsack.CypherText = ciphertext;

                return new BaseResponse<KnapsackViewModel>
                {
                    Data = knapsack,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<KnapsackViewModel>()
                {
                    Description = $"[Encrypt] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<KnapsackViewModel>> Decrypt(KnapsackViewModel knapsack)
        {
            try
            {
                int[] supIncrSeq = knapsack.CloseKey.ToArray();
                int m = knapsack.M;
                int t = knapsack.T;
                int?[] ciphertext = knapsack.CypherText;
                int inverse = FindInverse(m, t);
                if (inverse == -1)
                {
                    return new BaseResponse<KnapsackViewModel>()
                    {
                        Description = "Modular inverse does not exist. Decryption cannot proceed.",
                        StatusCode = StatusCode.InternalServerError
                    };
                }

                int n = supIncrSeq.Length;
                var decryptedText = new StringBuilder();
                foreach (var block in ciphertext)
                {
                    string decryptedBlock = "";
                    int? product = (block * inverse) % m;
                    for (int i = n - 1; i >= 0; i--)
                    {
                        if (product >= supIncrSeq[i])
                        {
                            decryptedBlock = "1" + decryptedBlock;
                            product -= supIncrSeq[i];
                        }
                        else
                        {
                            decryptedBlock = "0" + decryptedBlock;
                        }
                    }
                    decryptedText.Append(Convert.ToChar(Convert.ToInt32(decryptedBlock, 2)));
                }

                knapsack.DecryptedText = decryptedText.ToString();

                return new BaseResponse<KnapsackViewModel>
                {
                    Data = knapsack,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<KnapsackViewModel>()
                {
                    Description = $"[Decrypt] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<KnapsackViewModel>> OpenFromFile(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    return new BaseResponse<KnapsackViewModel>()
                    {
                        Description = "File is null",
                        StatusCode = StatusCode.NotFound
                    };
                }

                if (!file.ContentType.Equals("text/plain", StringComparison.OrdinalIgnoreCase))
                {
                    return new BaseResponse<KnapsackViewModel>()
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
                        KnapsackViewModel knapsackViewModel = new KnapsackViewModel
                        {
                            Text = lines[0]
                        };

                        return new BaseResponse<KnapsackViewModel>()
                        {
                            Data = knapsackViewModel,
                            StatusCode = StatusCode.OK
                        };
                    }
                    else
                    {
                        return new BaseResponse<KnapsackViewModel>()
                        {
                            Description = "Invalid lines count in the file.",
                            StatusCode = StatusCode.NotFound
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse<KnapsackViewModel>()
                {
                    Description = $"[OpenFromFile] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<KnapsackViewModel>> FrequencyTable(KnapsackViewModel knapsackViewModel)
        {
            try
            {
                if (knapsackViewModel == null)
                {
                    return new BaseResponse<KnapsackViewModel>
                    {
                        Description = "Model is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                if (knapsackViewModel.Text == null)
                {
                    return new BaseResponse<KnapsackViewModel>
                    {
                        Description = "Text is null",
                        StatusCode = Domain.Enum.StatusCode.NotFound
                    };
                }

                var frequencies = new Dictionary<char, int>();

                foreach (char c in knapsackViewModel.Text.ToLower())
                {
                    if (char.IsLetter(c))
                    {
                        if (frequencies.ContainsKey(c))
                            frequencies[c]++;
                        else
                            frequencies[c] = 1;
                    }
                }

                knapsackViewModel.FrequencyTable = frequencies.ToDictionary(
                    table => table.Key,
                    table => table.Value
                );

                return new BaseResponse<KnapsackViewModel>()
                {
                    Data = knapsackViewModel,
                    Description = "Text Analyzing successfully",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<KnapsackViewModel>()
                {
                    Description = $"[FrequencyTable] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<KnapsackViewModel>> SaveToFile(KnapsackViewModel knapsackViewModel)
        {
            try
            {
                if (knapsackViewModel == null)
                {
                    return new BaseResponse<KnapsackViewModel>()
                    {
                        Description = "KnapsackViewModel is null",
                        StatusCode = StatusCode.NotFound
                    };
                }

                StringBuilder contentBuilder = new StringBuilder();

                contentBuilder.AppendLine($"OpenKey - {knapsackViewModel.OpenKey};");
                contentBuilder.AppendLine($"CloseKey - {knapsackViewModel.CloseKey};");
                contentBuilder.AppendLine($"Text - {knapsackViewModel.Text}\n");
                contentBuilder.AppendLine($"Encrypted - {knapsackViewModel.CypherText}");
                contentBuilder.AppendLine($"Decrypted - {knapsackViewModel.DecryptedText}\n");

                await FrequencyTable(knapsackViewModel);

                if (knapsackViewModel.FrequencyTable != null && knapsackViewModel.FrequencyTable.Count > 0)
                {
                    contentBuilder.AppendLine("\nFrequency Table Results:");

                    foreach (var res in knapsackViewModel.FrequencyTable)
                    {
                        contentBuilder.AppendLine($"{res.Key}: {res.Value}");
                    }
                }

                string filePath = "path_to_save_file.txt";

                await File.WriteAllTextAsync(filePath, contentBuilder.ToString());

                return new BaseResponse<KnapsackViewModel>()
                {
                    Data = knapsackViewModel,
                    Description = "File saved successfully",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<KnapsackViewModel>()
                {
                    Description = $"[SaveToFile] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        private List<int> GenerateSuperIncreasingSequence(int n)
        {
            var res = new List<int>();
            int currentSum = 0;
            for (int i = 0; i < n; i++)
            {
                int toAdd = _random.Next(currentSum + 1, currentSum + 10);
                res.Add(toAdd);
                currentSum += toAdd;
            }
            return res;
        }

        private bool CheckSuperIncreasingSequence(List<int> seq)
        {
            int curSum = seq[0];
            for (int i = 1; i < seq.Count; i++)
            {
                if (seq[i] <= curSum)
                    return false;
                curSum += seq[i];
            }
            return true;
        }

        private bool CheckCondition(List<int> closeKey, int m, int t)
        {
            return CheckSuperIncreasingSequence(closeKey) && m > closeKey.Sum() && (t - 1) % m == 0;
        }

        private List<int> GenerateOpenKey(List<int> closeKey, int m, int t)
        {
            if (!CheckCondition(closeKey, m, t))
                return null;

            return closeKey.Select(x => (x * t) % m).ToList();
        }

        private string[] ToBinBlocks(string text, int n)
        {
            return text.Select(ch => Convert.ToString(ch, 2).PadLeft(n, '0')).ToArray();
        }

        private int ExtendedGcd(int a, int b, out int x, out int y)
        {
            if (a == 0)
            {
                x = 0;
                y = 1;
                return b;
            }
            int gcd = ExtendedGcd(b % a, a, out int x1, out int y1);
            x = y1 - (b / a) * x1;
            y = x1;
            return gcd;
        }

        private int FindInverse(int m, int t)
        {
            int gcd = ExtendedGcd(t, m, out int x, out int y);
            if (gcd != 1)
                return -1;
            return (x % m + m) % m;
        }
    }
}