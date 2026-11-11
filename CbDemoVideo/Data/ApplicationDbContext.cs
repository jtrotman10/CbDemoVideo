using CbDemoVideo.Models;
using Microsoft.EntityFrameworkCore;

namespace CbDemoVideo.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Customer> Customers => Set<Customer>();
    }
}
