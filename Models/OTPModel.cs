using MessagePack;
using System.ComponentModel.DataAnnotations;

namespace AimsManagement1.Models
{
    public class OTPModel
    {

        [Required(ErrorMessage = "please Enter Otp ")]
        public string Otp { get; set; }
    }
}
