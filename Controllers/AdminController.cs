

using AimsManagement1.Models;
using Microsoft.AspNetCore.Mvc;
using AimsManagement1.ViewModels;
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Mono.TextTemplating;
using Syncfusion.HtmlConverter;
using System.Data;
using DinkToPdf;
using DinkToPdf.Contracts;





namespace AimsManagement1.Controllers
{
    public class AdminController : Controller
    {
        public readonly DataBaseContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IConverter _pdfConverter;
        public AdminController(DataBaseContext context, IWebHostEnvironment webHostEnvironment,IConverter pdfConverter)
        {
            _context = context;
            this.webHostEnvironment = webHostEnvironment;
            _pdfConverter = pdfConverter;
        }

        //public IActionResult MainLayout()
        //{
        //    return View(new UserProfileModel()
        //    {
        //        Name = HttpContext.Session.GetString("UserName"),
        //        LName = HttpContext.Session.GetString("LName"),
        //        PNumber = HttpContext.Session.GetString("PNumber"),
        //        dist = HttpContext.Session.GetString("dist"),
        //        Address = HttpContext.Session.GetString("Address"),
        //        Email1 = HttpContext.Session.GetString("Email1"),
        //        passport = HttpContext.Session.GetString("passport"),

        //    });


        //}
        public IActionResult UserProfile()
        {
            return View(new UserProfileModel()
            {
                Name = HttpContext.Session.GetString("UserName"),
                LName = HttpContext.Session.GetString("LName"),
                PNumber = HttpContext.Session.GetString("PNumber"),
                dist = HttpContext.Session.GetString("dist"),
                Address = HttpContext.Session.GetString("Address"),
                Email1 = HttpContext.Session.GetString("Email1"),
                passport = HttpContext.Session.GetString("passport"),

            });


        }
        public StudTrainRegModel Hola(int id)
        {
            StudTrainRegModel? res = (from s in _context.StudTrainRegModels where s.Id == id select s).FirstOrDefault();
            //ViewBag.Source = "data:image/png;base64," + Convert.ToBase64String(res.Photo, 0, (res.Photo).Length);
            return res;
        }

        public IActionResult GeneratePdf(int id)
        {
            StudTrainRegModel model = Hola(id);
            // Read the HTML template file
            var htmlContent = System.IO.File.ReadAllText("template.html");

            // Get your model data
            // Replace this with your method to get model data

            // Replace placeholders in the HTML with actual model data
            htmlContent = htmlContent.Replace("{{img}}", @ViewBag.Source);
            htmlContent = htmlContent.Replace("{{Name}}", model.FirstName +" "+model.LastName);
            //htmlContent = htmlContent.Replace("{{Student ID}}", model.StudentId);
            htmlContent = htmlContent.Replace("{{Email}}", model.Email);
            //htmlContent = htmlContent.Replace("{{Profile}}", model.Profile);
            //htmlContent = htmlContent.Replace("{{Father's Name}}", model.FName);
            //htmlContent = htmlContent.Replace("{{Mother's Name}}", model.MName);
            //htmlContent = htmlContent.Replace("{{Gender}}", model.Gender);
            htmlContent = htmlContent.Replace("{{DOB}}", model.Birthdate.ToShortDateString());
            htmlContent = htmlContent.Replace("{{Mobile No}}", model.PhoneNumber);
            htmlContent = htmlContent.Replace("{{Address}}", model.Address);
            htmlContent = htmlContent.Replace("{{Qualifications}}", model.Education);
            htmlContent = htmlContent.Replace("{{Batch No}}", model.Course);
            //htmlContent = htmlContent.Replace("{{Mode}}", model.Mode);
            //htmlContent = htmlContent.Replace("{{DOJ}}", model.Doj.ToShortDateString());
            //htmlContent = htmlContent.Replace("{{Bank}}", model.BankName);
            //htmlContent = htmlContent.Replace("{{Account No}}", model.AccountNo.ToString());
            //htmlContent = htmlContent.Replace("{{Branch}}", model.BranchName);
            //htmlContent = htmlContent.Replace("{{IFSC Code}}", model.Ifsc);
            // Add more replacements for other properties as needed

            // Convert HTML to PDF
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = {
             PaperSize = PaperKind.A4,
             Orientation = Orientation.Portrait
         },
                Objects = {
             new ObjectSettings() {
                 HtmlContent = htmlContent
             }
         }
            };

            var pdfBytes = _pdfConverter.Convert(pdf);

            // Return PDF file as IActionResult
            return File(pdfBytes, "application/pdf", "ModelData.pdf");
        }


        public IActionResult Home()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login2()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(StudTrainRegModel obj)
        {
            try
            {

                var user = _context.StudTrainRegModels.FirstOrDefault(s => s.Email == obj.Email && s.Password == obj.Password);
                if (user != null)
                {
                    HttpContext.Session.SetString("UserName", user.FirstName);
                    HttpContext.Session.SetString("LName", user.LastName);
                    HttpContext.Session.SetString("PNumber", user.PhoneNumber);
                    HttpContext.Session.SetString("Address", user.Address);
                    HttpContext.Session.SetString("dist", user.District);
                    HttpContext.Session.SetString("passport", user.PassportPhoto);

                    HttpContext.Session.SetString("Email1", user.Email);
                    HttpContext.Session.SetString("LoginTime", System.DateTime.Now.ToShortTimeString());
                    return RedirectToAction("Home", "Admin");
                }
                else
                {
                    return View();
                }

            }
            catch (Exception)
            {

                throw;
            }



        }


        [HttpGet]
        public IActionResult StdRegister()
        {

            return View();
        }

        [HttpPost]
        
        public async Task<IActionResult> StdRegister(StudTrainViewModel model)
        {
            

                    string Identityphoto1 = ProcessUploadedFile1(model);
                    string uniqueFileName = ProcessUploadedFile(model);
                    StudTrainRegModel StudTrainRegModels = new StudTrainRegModel
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Address = model.Address,
                        District = model.District,
                        State = model.State,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        Password = model.Password,
                        ConfirmPassword = model.ConfirmPassword,
                        Education = model.Education,
                        Birthdate = model.Birthdate,
                        Age = model.Age,
                        Course = model.Course,
                        PassportPhoto = uniqueFileName,
                        IdentityProof = Identityphoto1,
                    };


                    _context.Add(StudTrainRegModels);
                    var savedChanges = await _context.SaveChangesAsync();

            HttpContext.Session.SetString("FirstName", model.FirstName);
            HttpContext.Session.SetString("LastName", model.LastName);
            HttpContext.Session.SetString("Email", model.Email);
            HttpContext.Session.SetString("Password", model.Password);

            if (savedChanges > 0)
                    {

                        SendEmail(model.Email);

                        return RedirectToAction("Home", "Admin");
                    }
                    return View(model); 
           
        }

        public bool SendEmail(string Email)
        {

            bool chk = false;
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("ochavan4444@gmail.com");
                mail.To.Add(Email);
                mail.IsBodyHtml = true;
                mail.Subject = "Registration Successful";
                mail.Body = "Hi " + HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName") +
                    " Welcome to AarohiInfo Institute of Management and Studies You have been successfully registered to our Institue Your  Provided Email  Is: "
                    + HttpContext.Session.GetString("Email") + " and Password is:" + HttpContext.Session.GetString("Password") + ". Change the Password after login. Thank You";
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("ochavan4444@gmail.com", "tgtl rvwm fyjq ogwt");
                smtp.EnableSsl = true;
                smtp.Send(mail);
                chk = true;

            }
            catch (Exception)
            {

                throw;
            }
            return chk;
        }

      

        public async Task<IActionResult> Display()
        {

            var res = _context.StudTrainRegModels.ToListAsync();
            return View(await res);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var model = await _context.StudTrainRegModels.FindAsync(Id);
            var StudTrainViewModel = new StudTrainViewModel()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                District = model.District,
                State = model.State,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                Education = model.Education,
                Birthdate = model.Birthdate,
                Age = model.Age,
                Course = model.Course,
                ExistingPassport = model.PassportPhoto,
                ExistingIdentity = model.IdentityProof,
            };

            if (model == null)
            {
                return NotFound();
            }
            return View(StudTrainViewModel);
           
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, StudTrainViewModel model)
        {

            var Student = await _context.StudTrainRegModels.FindAsync(model.Id);

            Student.FirstName = model.FirstName;
            Student.LastName = model.LastName;
            Student.Address = model.Address;
            Student.District = model.District;
            Student.State = model.State;
            Student.Email = model.Email;
            Student.PhoneNumber = model.PhoneNumber;
            Student.Password = model.Password;
            Student.ConfirmPassword = model.ConfirmPassword;
            Student.Education = model.Education;
            Student.Birthdate = model.Birthdate;
            Student.Age = model.Age;
            Student.Course = model.Course;

            if (model.PassportPhoto != null && model.IdentityProof != null)
            {
                if (model.ExistingPassport != null && model.ExistingPassport != null)
                {
                    string filePath = Path.Combine(webHostEnvironment.WebRootPath, "Uploads", model.ExistingPassport);
                    string filePath1 = Path.Combine(webHostEnvironment.WebRootPath, "Uploads", model.ExistingIdentity);
                    System.IO.File.Delete(filePath);
                    System.IO.File.Delete(filePath1);
                }

                Student.PassportPhoto = ProcessUploadedFile(model);
                Student.IdentityProof = ProcessUploadedFile1(model);
            }
            _context.Update(Student);
            await _context.SaveChangesAsync();
            return RedirectToAction("Display", "Admin");


        }
        public IActionResult Delete(int Id)
        {
            var res = _context.StudTrainRegModels.Find(Id);
            if (res != null)
            {
                _context.Remove(res);
            }

            int x = _context.SaveChanges();
            if (x > 0)
            {
                return RedirectToAction("Display", "Admin");
            }

            return View();
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _context.StudTrainRegModels
                .FirstOrDefaultAsync(m => m.Id == id);

            var StudTrainViewModel = new StudTrainViewModel()
            {

                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                District = model.District,
                State = model.State,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                Education = model.Education,
                Birthdate = model.Birthdate,
                Age = model.Age,
                Course = model.Course,
                ExistingPassport = model.PassportPhoto,
                ExistingIdentity = model.IdentityProof,
            };

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }


		/*public IActionResult ExportToPDF()
		{
			// Initialize the HTML to PDF converter
			HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();

			// Convert a specific URL (your details page) to a PDF document
			PdfDocument document = htmlConverter.Convert("C:\\Users\\Omkar Chavan\\source\\repos\\AimsManagement1\\Views\\Admin");

			// Create a memory stream to save the document
			MemoryStream stream = new MemoryStream();

			// Save the document to the memory stream
			document.Save(stream);

			// Return the PDF file for download
			return File(stream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf, "DetailsPage.pdf");
		}
*/

		public IActionResult TrainerRegister()
        {
            return View();
        }

        private string ProcessUploadedFile(StudTrainViewModel model)
        {
            string uniqueFileName = null;
            

            if (model.PassportPhoto != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PassportPhoto.FileName;
                
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.PassportPhoto.CopyTo(fileStream);
                }
            }

            return uniqueFileName ;
        }

        private string ProcessUploadedFile1(StudTrainViewModel model)
        {
            string Identityphoto1 = null;


          if (model.IdentityProof != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
                Identityphoto1 = Guid.NewGuid().ToString() + "_" + model.IdentityProof.FileName;

                string filePath = Path.Combine(uploadsFolder, Identityphoto1);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.IdentityProof.CopyTo(fileStream);
                }
            }

            return Identityphoto1;
        }


    }
}
