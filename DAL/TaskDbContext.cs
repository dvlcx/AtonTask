using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace AtonTask.DAL
{
    public class TaskDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = default!;

        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
        {
        }
    }
}