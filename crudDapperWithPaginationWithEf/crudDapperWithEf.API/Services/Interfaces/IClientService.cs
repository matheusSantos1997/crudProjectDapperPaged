using crudDapperWithEf.API.DTOs.Client;

namespace crudDapperWithEf.API.Services.Interfaces
{
    public interface IClientService
    {
        Task<(IEnumerable<ClientDTO> Clients, int TotalCount)> ListAllClients(int pageNumber, int pageSize);

        Task<List<ClientDTO>> ListAllClients();

        Task<ClientDTO> ListClientById(long id);

        Task<(IEnumerable<ClientDTO> Clients, int TotalCount)> ListClientsFilter(string? nameClient, string? nameProduct, int pageNumber, int pageSize);

        Task<CreateNewClientDTO> AddNewClient(CreateNewClientDTO clientDto);

        Task<UpdateClientDTO> UpdateClient(long id, UpdateClientDTO clientDto);

        Task<bool> DeleteClient(long id);
    }
}