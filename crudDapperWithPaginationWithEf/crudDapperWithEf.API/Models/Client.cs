using System.ComponentModel.DataAnnotations;

namespace crudDapperWithEf.API.Models
{
    public class Client
    {
        [Key]
        public long Id { get; set; }

        public string? ClientName { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public IList<Product>? Products { get; set; }
    }
}