
using AimsManagement1.Models;
using Microsoft.AspNetCore.Mvc;

namespace AimsManagement1.Controllers
{
    public class BankController : Controller
    {
        public readonly DataBaseContext _context;
        public BankController(DataBaseContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult BankDetailsReg()
        {

            return View();
        }

        // POST: BankDetails/Create
        [HttpPost]

        public async Task<IActionResult> BankDetailsReg(BankDetailsModel bankDetails, IFormFile proofImage)
        {
            if (ModelState.IsValid)
            {
                // Save proof image
                if (proofImage != null && proofImage.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(proofImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "proofImages", fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await proofImage.CopyToAsync(fileStream);
                    }

                    bankDetails.ProofImageFileName = fileName;
                }

                _context.Add(bankDetails);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "User", new { id = bankDetails.BankDetailsId });
            }

            return View(bankDetails);
        }
    }
}
