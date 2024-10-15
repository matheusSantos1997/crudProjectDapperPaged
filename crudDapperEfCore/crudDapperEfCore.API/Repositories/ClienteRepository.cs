using System.Data;
using crudDapperEfCore.API.Context;
using crudDapperEfCore.API.models;
using crudDapperEfCore.API.Pagination;
using crudDapperEfCore.API.Repositories.Interfaces;
using crudDapperEfCore.API.Repositories.SQL;
using Dapper;

namespace crudDapperEfCore.API.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly IDbConnection _connection;

        public ClienteRepository()
        {
            _connection = DataContext.GetConnection();
        }

        public async Task<(IEnumerable<Cliente> Clientes, int TotalCount)> GetAllClientes(int pageNumber, int pageSize)
        {
            try
            {
                var clientes = new Dictionary<long, Cliente>();

                // Calcula o deslocamento com base no número da página e no tamanho da página
                int offset = (pageNumber - 1) * pageSize;

                // Primeiro, conta o total de clientes para a paginação
                string countQuery = "SELECT COUNT(*) FROM Clientes;";
                int totalCount = await _connection.ExecuteScalarAsync<int>(countQuery);

                // Primeiro, busca os clientes
                string queryClientes = ClienteDBScript.SelectAllClientesPaged();
                var parameters = new { Offset = offset, PageSize = pageSize };

                var clientesResult = await _connection.QueryAsync<Cliente>(queryClientes, parameters);

                // Armazena os clientes retornados
                foreach (var cliente in clientesResult)
                {
                    clientes[cliente.Id] = cliente;
                }

                // Agora, busca todos os produtos associados aos clientes retornados
                if (clientes.Any())
                {
                    // Constrói a consulta para buscar produtos pelos IDs dos clientes
                    var clienteIds = clientes.Keys.ToList();
                    var (queryProdutos, produtoParams) = ClienteDBScript.SelectAllprodutosByClientesIdsPaged(clienteIds);

                    var produtosResult = await _connection.QueryAsync<Produto>(queryProdutos, produtoParams);

                    // Associa os produtos aos clientes
                    foreach (var produto in produtosResult)
                    {
                        if (clientes.TryGetValue(produto.ClienteId!.Value, out var cliente))
                        {
                            if(cliente.Produtos == null)
                            {
                                cliente.Produtos = new List<Produto>();
                            }
                            cliente.Produtos.Add(produto);
                        }
                    }
                }

                return (clientes.Values.ToList(), totalCount); // Retorna a lista de clientes
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Cliente>> GetAllClientes()
        {
            try
            {
                Dictionary<long, Cliente> clientes = new();

                string query = ClienteDBScript.SelectAllClientes();

                var result = await _connection.QueryAsync<Cliente, Produto, Cliente>(
                        query,
                        (cliente, produto) =>
                        {
                            if (!clientes.TryGetValue(cliente.Id, out var currentCliente))
                            {
                                currentCliente = cliente;
                                currentCliente.Produtos = new List<Produto>();
                                clientes[cliente.Id] = currentCliente;
                            }

                            if (produto != null)
                            {
                                currentCliente.Produtos?.Add(produto);
                            }

                            return currentCliente;
                        });

                result = clientes.Values;

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Cliente> GetClienteById(long id)
        {
            try
            {
                Dictionary<long, Cliente> clientes = new();

                var query = ClienteDBScript.SelectClientePeloId(id);

                foreach (KeyValuePair<string, object> item in query)
                {
                    var result = await _connection.QueryAsync<Cliente, Produto, Cliente>(item.Key,
                    map: (cliente, produto) =>
                    {
                        if (!clientes.TryGetValue(cliente.Id, out var currentCliente))
                        {
                            currentCliente = cliente;
                            currentCliente.Produtos = new List<Produto>();
                            clientes[cliente.Id] = currentCliente;
                        }
                        if (produto != null)
                        {
                            currentCliente.Produtos?.Add(produto);
                        }

                        return currentCliente;
                    }, item.Value);

                    return result.FirstOrDefault()!;
                }

                return null!;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(IEnumerable<Cliente> Clientes, int TotalCount)> GetClienteByNome(string? nomeCliente, string? nomeProduto, int pageNumber, int pageSize)
        {
            try
            {
                Dictionary<long, Cliente> clientes = new();

                // Primeiro, conta o total de clientes para a paginação
                string countQuery = "SELECT COUNT(*) FROM Clientes;";
                int totalCount = await _connection.ExecuteScalarAsync<int>(countQuery);

                // Chama a consulta de filtragem
                var query = ClienteDBScript.FiltrarPaged(nomeCliente, nomeProduto, pageNumber, pageSize);

                var sql = query["Sql"].ToString();
                var parameters = query["Parameters"];

                // Executa a consulta
                var result = await _connection.QueryAsync<Cliente>(sql!, parameters);

                foreach (var cliente in result)
                {
                    clientes[cliente.Id] = cliente;
                }

                // Agora, busca todos os produtos associados aos clientes retornados
                if (clientes.Any())
                {
                    var clienteIds = clientes.Keys.ToList();
                    var (queryProdutos, produtoParams) = ClienteDBScript.SelectAllprodutosByClientesIdsPaged(clienteIds);
                    var produtosResult = await _connection.QueryAsync<Produto>(queryProdutos, produtoParams);

                    foreach (var produto in produtosResult)
                    {
                        if (clientes.TryGetValue(produto.ClienteId!.Value, out var cliente))
                        {
                            if(cliente.Produtos == null)
                            {
                                cliente.Produtos = new List<Produto>();
                            }
                            cliente.Produtos.Add(produto);
                        }
                    }
                }

                return (clientes.Values.ToList(), totalCount); // Retorna a lista de clientes
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}