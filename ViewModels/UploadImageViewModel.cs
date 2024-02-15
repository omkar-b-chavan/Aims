using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace AimsManagement1.ViewModels
{
    public class UploadImageViewModel
    {

        [Display(Name = "Pass Image")]

        public IFormFile PassportPhoto { get; set; }


        [Display(Name = "Iden Image")]
        public IFormFile IdentityProof { get; set; }
    }

}
