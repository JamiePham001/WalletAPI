using Microsoft.EntityFrameworkCore;
using WalletAPI.models;

namespace WalletAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {}

    public DbSet<Wallet> Wallet { get; set; }
}