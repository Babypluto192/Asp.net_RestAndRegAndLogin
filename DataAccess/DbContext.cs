using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class DbContext:Microsoft.EntityFrameworkCore.DbContext
{
    public DbContext(DbContextOptions<DbContext> options) : base(options)
    {
    }
    public DbSet<BookEntity> Books { get; set; }
    
    
    public DbSet<UserEntity> Users { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>()
            .Property<string>("Password")
            .IsRequired()
            .HasMaxLength(int.MaxValue);
    }
}