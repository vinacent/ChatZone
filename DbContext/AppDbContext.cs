using ChatZone.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatZone.DbContext;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, long>
{
    public DbSet<Comment> Comments { get; set; }

    // dotnet ef migrations add "Update.5"
    // dotnet ef database update
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}