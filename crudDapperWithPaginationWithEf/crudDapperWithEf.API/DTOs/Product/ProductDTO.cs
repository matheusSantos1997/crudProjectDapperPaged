namespace crudDapperWithEf.API.DTOs.Product
{
    public class ProductDTO
    {
        public long Id { get; set; }

        public string? ProductName { get; set; }

        public string? ProductType { get; set; }

        public decimal? Price { get; set; }

        public long? ClientId { get; set; }
    }
}