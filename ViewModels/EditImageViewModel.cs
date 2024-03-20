namespace AimsManagement1.ViewModels
{
    public class EditImageViewModel: UploadImageViewModel
    {
        public int Id { get; set; }
        public string ExistingPassport { get; set; }
        public string ExistingIdentity { get; set; }
    }
}
