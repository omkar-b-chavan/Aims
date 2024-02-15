

using AimsManagement1.Models;
using Microsoft.AspNetCore.Mvc;
using AimsManagement1.ViewModels;
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;





namespace AimsManagement1.Controllers
{
    public class AdminController : Controller
    {
        public readonly DataBaseContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;
        public AdminController(DataBaseContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this.webHostEnvironment = webHostEnvironment;
        }


        public IActionResult UserProfile()
        {
            return View(new UserProfileModel()
            {
                Name = HttpContext.Session.GetString("UserName"),
                LName = HttpContext.Session.GetString("LName"),
                PNumber = HttpContext.Session.GetString("PNumber"),
                dist = HttpContext.Session.GetString("dist"),
                Address = HttpContext.Session.GetString("Address"),
                Email1 = HttpContext.Session.GetString("Email1")


            });


        }




        public IActionResult Home()
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
        public IActionResult Update(int? Id)
        {
            var data = _context.StudTrainRegModels.Find(Id);
            return View(data);
        }
        [HttpPost]
        public IActionResult Update(int? id, StudTrainRegModel obj)
        {
            var data = _context.Update(obj);
            int x = _context.SaveChanges();
            if (x > 0)
            {
                return RedirectToAction("Display", "Admin");
            }

            return View();
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
