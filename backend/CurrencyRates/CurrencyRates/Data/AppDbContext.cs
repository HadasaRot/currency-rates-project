using Microsoft.EntityFrameworkCore;
using CurrencyRates.Models;
using System.Collections.Generic;

namespace CurrencyRates.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<CurrencyRate> CurrencyRates { get; set; }
    }
}