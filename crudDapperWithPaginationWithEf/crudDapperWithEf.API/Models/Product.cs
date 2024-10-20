using System.ComponentModel.DataAnnotations;

namespace crudDapperWithEf.API.Models
{
    public class Product
    {
        [Key]
        public long Id { get; set; }

        public string? ProductName { get; set; }

        public string? ProductType { get; set; }

        public decimal? Price { get; set; }

        public long? ClientId { get; set; }

        public Client? Client { get; set; }
    }
}