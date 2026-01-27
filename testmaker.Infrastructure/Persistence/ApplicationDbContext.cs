using Microsoft.EntityFrameworkCore;
using testmaker.Domain.Entities;

namespace testmaker.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<School> School => Set<School>();
}