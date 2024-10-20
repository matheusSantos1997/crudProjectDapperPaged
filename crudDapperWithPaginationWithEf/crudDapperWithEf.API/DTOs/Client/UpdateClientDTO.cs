using crudDapperWithEf.API.DTOs.Product;

namespace crudDapperWithEf.API.DTOs.Client
{
    public class UpdateClientDTO
    {
        public long Id { get; set; }

        public string? ClientName { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public IList<UpdateProductDTO>? Products { get; set; }
    }
}