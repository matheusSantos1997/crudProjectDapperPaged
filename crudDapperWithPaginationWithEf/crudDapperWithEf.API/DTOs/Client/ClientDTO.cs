using crudDapperWithEf.API.DTOs.Product;
using Newtonsoft.Json;

namespace crudDapperWithEf.API.DTOs.Client
{
    public class ClientDTO
    {
        public long Id { get; set; }

        public string? ClientName { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public IList<ProductDTO>? Products { get; set; }

        [JsonIgnore]
        public string ProductsNames => string.Join(",", Products!.Select(p => p.ProductName ?? string.Empty));
    }
}