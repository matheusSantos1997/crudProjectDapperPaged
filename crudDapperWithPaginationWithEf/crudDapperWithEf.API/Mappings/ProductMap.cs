using crudDapperWithEf.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace crudDapperWithEf.API.Mappings
{
    public class ProductMap : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.ProductName).IsRequired().HasMaxLength(50);

            builder.Property(p => p.ProductType).IsRequired().HasMaxLength(50);

            builder.Property(p => p.Price).IsRequired().HasColumnType("decimal");

            builder.HasOne(p => p.Client)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.ClientId);

            builder.ToTable("Products");
        }
    }
}