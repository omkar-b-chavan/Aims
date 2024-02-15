using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AimsManagement1.Models
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext() { }
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {

        }

        public virtual DbSet<StudTrainRegModel> StudTrainRegModels { get; set; }
        public virtual DbSet<StudTrainRegModel> BankDetailsModels { get; set; }

    }
}