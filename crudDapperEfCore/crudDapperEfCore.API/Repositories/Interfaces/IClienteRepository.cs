using crudDapperEfCore.API.models;
using crudDapperEfCore.API.Pagination;

namespace crudDapperEfCore.API.Repositories.Interfaces
{
    public interface IClienteRepository
    {
        public Task<(IEnumerable<Cliente> Clientes, int TotalCount)> GetAllClientes(int pageNumber, int pageSize);

        public Task<List<Cliente>> GetAllClientes();

        public Task<Cliente> GetClienteById(long id);

        public Task<(IEnumerable<Cliente> Clientes, int TotalCount)> GetClienteByNome(string? nomeCliente, string? nomeProduto, int pageNumber, int pageSize);
    }
}