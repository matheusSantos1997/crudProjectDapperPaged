using crudDapperEfCore.API.DTOs.Cliente;
using crudDapperEfCore.API.Pagination;

namespace crudDapperEfCore.API.Services.Interfaces
{
    public interface IClienteService
    {
       Task<(IEnumerable<ClienteDTO> Clientes, int TotalCount)> ListarTodosClientes(int pageNumber, int pageSize);

        Task<List<ClienteDTO>> ListarTodosClientes();

        Task<ClienteDTO> ListarClientePorId(long id);

        Task<(IEnumerable<ClienteDTO> Clientes, int TotalCount)> ListarTodosClientesPorNome(string? nome, string? nomeProduto, int pageNumber, int pageSize);

        Task<CreateNewClienteDTO> AdicionarNovoCliente(CreateNewClienteDTO clienteDto);

        Task<UpdateClienteDTO> AtualizarCliente(long id, UpdateClienteDTO clienteDto);

        Task<bool> DeletarCliente(long id);
    }
}