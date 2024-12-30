using UserManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.Data
{
    public class UserManagementContext: DbContext
    {
        public UserManagementContext(DbContextOptions<UserManagementContext> options) : base(options) { 
        }
        public DbSet<User> Users { get; set; }
    }
}
