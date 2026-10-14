using Microsoft.EntityFrameworkCore;

namespace DotnetShop.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
}
