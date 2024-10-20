using crudDapperWithEf.API.DTOs.Product;

namespace crudDapperWithEf.API.DTOs.Client
{
    public class CreateNewClientDTO
    {
        public string? ClientName { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public IList<CreateNewProductDTO>? Products { get; set; }
    }
}