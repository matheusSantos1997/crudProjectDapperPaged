using System.Data;
using System.Data.SqlClient;
using crudDapperWithEf.API.Models;
using Microsoft.EntityFrameworkCore;

namespace crudDapperWithEf.API.Context
{
    public class DataContext : DbContext
    {
        #nullable disable
        public DataContext(DbContextOptions<DataContext> options): base(options){}

        public DbSet<Client> Clients { get; set; }

        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                   .AddJsonFile("appsettings.json")
                   .Build();
            
            string conn = configuration.GetConnectionString("localConnection");

            optionsBuilder.UseSqlServer(conn);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
        }
    }
}