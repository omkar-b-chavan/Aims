using System.ComponentModel.DataAnnotations.Schema;

namespace AimsManagement1.Models
{
    [Table("BankDetails")]
    public class BankDetailsModel
    {
        public int BankDetailsId { get; set; }
        // Foreign key linking to the StudTrainReg table
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string ProofImageFileName { get; set; }

        public byte[] ProofImage { get; set; }
        // Other bank details...


    }
}
