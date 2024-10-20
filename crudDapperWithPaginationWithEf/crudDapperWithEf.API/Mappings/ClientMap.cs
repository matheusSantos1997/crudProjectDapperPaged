using crudDapperWithEf.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace crudDapperWithEf.API.Mappings
{
    public class ClientMap : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.ClientName).IsRequired().HasMaxLength(50);

            builder.Property(c => c.Email).IsRequired().HasMaxLength(90);

            builder.Property(c => c.Address).IsRequired().HasMaxLength(40);

            builder.HasMany(c => c.Products)
                   .WithOne(P => P.Client)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Clients");
        }
    }
}