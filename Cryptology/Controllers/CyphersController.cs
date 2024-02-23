using Cryptology.BLL;
using Cryptology.Domain.Entity;
using Cryptology.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Cryptology.Controllers
{
    public class CyphersController : Controller
    {
        private readonly CaesarService _caesarService;

        public CyphersController (CaesarService caesarService)
        {
            _caesarService = caesarService;
        }

        public async Task<IActionResult> CaesarCypher()
        {
            return View(new CaesarViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CaesarCypher(CaesarViewModel caesar, string action)
        {
            if (action == "Encrypt")
            {
                var encryptResponse = await _caesarService.Encrypt(caesar);

                if (encryptResponse.StatusCode == Domain.Enum.StatusCode.OK)
                {
                    TempData["AlertMessage"] = encryptResponse.Description;
                    TempData["ResponseStatus"] = encryptResponse.StatusCode.ToString();
                    return View("CaesarCypher", caesar);
                }
                else
                {
                    TempData["AlertMessage"] = encryptResponse.Description;
                    TempData["ResponseStatus"] = "Error";
                }
            }
            else if (action == "Decrypt")
            {
                var decryptResponse = await _caesarService.Decrypt(caesar);

                if (decryptResponse.StatusCode == Domain.Enum.StatusCode.OK)
                {
                    TempData["AlertMessage"] = decryptResponse.Description;
                    TempData["ResponseStatus"] = decryptResponse.StatusCode.ToString();
                    return View("CaesarCypher", caesar);
                }
                else
                {
                    TempData["AlertMessage"] = decryptResponse.Description;
                    TempData["ResponseStatus"] = "Error";
                }
            }
            else if (action == "BruteForce")
            {
                var bruteForceResponse = await _caesarService.BruteForce(caesar);

                if (bruteForceResponse.StatusCode == Domain.Enum.StatusCode.OK)
                {
                    TempData["AlertMessage"] = bruteForceResponse.Description;
                    TempData["ResponseStatus"] = bruteForceResponse.StatusCode.ToString();
                    return View("CaesarCypher", caesar);
                }
                else
                {
                    TempData["AlertMessage"] = bruteForceResponse.Description;
                    TempData["ResponseStatus"] = "Error";
                }
            }
            else if (action == "OpenFile")
            {
                var bruteForceResponse = await _caesarService.OpenFromFile(caesar.FileToEncrypt);

                if (bruteForceResponse.StatusCode == Domain.Enum.StatusCode.OK)
                {
                    TempData["AlertMessage"] = bruteForceResponse.Description;
                    TempData["ResponseStatus"] = bruteForceResponse.StatusCode.ToString();
                    return View("CaesarCypher", caesar);
                }
                else
                {
                    TempData["AlertMessage"] = bruteForceResponse.Description;
                    TempData["ResponseStatus"] = "Error";
                }
            }

            return View(caesar);
        }
    }
}
