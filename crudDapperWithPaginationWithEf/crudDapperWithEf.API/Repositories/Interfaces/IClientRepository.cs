using crudDapperWithEf.API.Models;

namespace crudDapperWithEf.API.Repositories.Interfaces
{
    public interface IClientRepository
    {
        Task<(IEnumerable<Client> Clients, int TotalCount)> GetAllClients(int pageNumber, int pageSize);

        Task<List<Client>> GetAllClients();

        Task<Client> GetClientById(long id);

        Task<(IEnumerable<Client> Clients, int TotalCount)> GetClientsWithFilters(string? clientName, string? productName, int pageNumber, int pageSize);
    }
}