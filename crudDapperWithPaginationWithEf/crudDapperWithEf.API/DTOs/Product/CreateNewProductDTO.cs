namespace crudDapperWithEf.API.DTOs.Product
{
    public class CreateNewProductDTO
    {
        public string? ProductName { get; set; }

        public string? ProductType { get; set; }

        public decimal? Price { get; set; }
    }
}