using Microsoft.EntityFrameworkCore;
using TaskMasterAPI.Models;

namespace TaskMasterAPI.Data
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected AppDbContext()
        {
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<TaskItemModel> TaskItems { get; set; }

    }
}
