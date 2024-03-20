
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using AimsManagement1.Models;

namespace AimsManagement1.Controllers
{
    public class OTPController : Controller
    {
        public readonly DataBaseContext _context;
        public OTPController(DataBaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult SendOtp()

        {
            var otpStartTime = HttpContext.Session.GetString("OTPSessionStartTime");
            if (string.IsNullOrEmpty(otpStartTime))
            {
                // If the session start time is not set, set it to the current time
                otpStartTime = DateTime.UtcNow.ToString();
                HttpContext.Session.SetString("OTPSessionStartTime", otpStartTime);
            }

            // Calculate the remaining time for OTP verification
            var currentTime = DateTime.UtcNow;
            var otpSessionTimeout = TimeSpan.FromSeconds(180);
            var elapsedTime = currentTime - DateTime.Parse(otpStartTime);
           
            return View();
        }


        [HttpPost]
        public IActionResult SendOtp(string Email, StudTrainRegModel obj)
        {

            var ex = (from object1 in _context.StudTrainRegModels where object1.Email == obj.Email select object1).Any();

            





            if (ex)
            {
                HttpContext.Session.SetString("Email", obj.Email);
                Random rand = new Random();
                HttpContext.Session.SetString("Otp", rand.Next(111111, 999999).ToString());


                SendEmail(obj.Email);
                return RedirectToAction("VerifyOtp", "Otp");

            }
            else
            {
                TempData["msg"] = "Email not Matches to any Account";
                return View();
            }
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
                mail.Subject = "Verify Otp";
                mail.Body = "Otp is :" + HttpContext.Session.GetString("Otp");
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



        [HttpGet]

        public IActionResult VerifyOtp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyOtp(OTPModel obj)
        {
            if (obj == null)
            {
                ViewData["msg"] = "Plz Enter OTP";
            }
            else
            {
                if (obj.Otp == HttpContext.Session.GetString("Otp"))
                {
                    return RedirectToAction("ChangePass", "OTP");
                }
                else
                {
                    ViewData["msg"] = "OTp is not correct";
                }

            }
            return View();

        }

        [HttpGet]
        public IActionResult ChangePass()
        {


            return View();
        }

        [HttpPost]

        public IActionResult ChangePass(StudTrainRegModel obj)
        {
            try
            {

                var user = _context.StudTrainRegModels.FirstOrDefault(s => s.Email == HttpContext.Session.GetString("Email"));


                if (user != null)
                {

                    user.Password = obj.Password;
                    user.ConfirmPassword = obj.ConfirmPassword;


                    int affectedRows = _context.SaveChanges();


                    if (affectedRows > 0)
                    {

                        return RedirectToAction("Home", "Admin");
                    }
                }



                return View();
            }
            catch (Exception ex)
            {

                return View();
            }
        }


    }

}
