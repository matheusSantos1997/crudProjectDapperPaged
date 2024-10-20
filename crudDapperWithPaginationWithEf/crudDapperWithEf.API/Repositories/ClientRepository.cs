using System.Data;
using System.Data.SqlClient;
using crudDapperWithEf.API.Connection;
using crudDapperWithEf.API.Models;
using crudDapperWithEf.API.Repositories.Interfaces;
using crudDapperWithEf.API.Repositories.SQL;
using Dapper;

namespace crudDapperWithEf.API.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ConnectionFactory _connectionFactory;

        public ClientRepository(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<(IEnumerable<Client> Clients, int TotalCount)> GetAllClients(int pageNumber, int pageSize)
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    var clients = new Dictionary<long, Client>();

                    // Calcula o deslocamento com base no número da página e no tamanho da página
                    int offset = (pageNumber - 1) * pageSize;

                    // Conta o totalde clientes para paginacao
                    string countQuery = ClientDBScript.SelectTotalCountClients();
                    int totalCount = await connection.ExecuteScalarAsync<int>(countQuery);

                    // busca os clientes
                    string clientQuery = ClientDBScript.SelectAllClientsPaged();
                    var parameters = new { Offset = offset, PageSize = pageSize };

                    var clientsResults = await connection.QueryAsync<Client>(clientQuery, parameters);

                    // Armazena os clientes retornados
                    foreach (var client in clientsResults)
                    {
                        clients[client.Id] = client;
                    }

                    // Busca todos os produtos associados aos clientes retornados
                    if (clients.Any())
                    {
                        // Constrói a consulta para buscar produtos pelos IDs dos clientes
                        var clientIds = clients.Keys.ToList();
                        var (productsQuery, productsParams) = ClientDBScript.SelectAllproductsByClientsIdsPaged(clientIds);

                        var productsResult = await connection.QueryAsync<Product>(productsQuery, productsParams);

                        // Associa os produtos aos clientes
                        foreach (var product in productsResult)
                        {
                            if (clients.TryGetValue(product.ClientId!.Value, out var client))
                            {
                                if (client.Products == null)
                                {
                                    client.Products = new List<Product>();
                                }

                                client.Products.Add(product);
                            }
                        }
                    }

                    return (clients.Values.ToList(), totalCount);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Client>> GetAllClients()
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    var clients = new Dictionary<long, Client>();

                    string clientQuery = ClientDBScript.SelectAllClients();

                    var result = await connection.QueryAsync<Client, Product, Client>(
                             clientQuery,
                             (client, product) =>
                             {
                                 if (!clients.TryGetValue(client.Id, out var currentClient))
                                 {
                                     currentClient = client;
                                     currentClient.Products = new List<Product>();
                                     clients[client.Id] = currentClient;
                                 }

                                 if (product != null)
                                 {
                                     currentClient.Products?.Add(product);
                                 }

                                 return currentClient;
                             });

                    result = clients.Values;

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(IEnumerable<Client> Clients, int TotalCount)> GetClientsWithFilters(string? clientName, string? productName, int pageNumber, int pageSize)
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    var clients = new Dictionary<long, Client>();

                    // Chama a consulta de filtragem com paginação
                    var (sql, parameters) = ClientDBScript.FilterPaged(clientName, productName, pageNumber, pageSize);

                    // Executa a consulta para buscar os clientes
                    var clientsResult = await connection.QueryAsync<Client>(sql, parameters);

                    // Armazena os clientes retornados
                    foreach (var client in clientsResult)
                    {
                        clients[client.Id] = client;
                    }

                    // Busca todos os produtos associados aos clientes retornados
                    if (clients.Any())
                    {
                        var clientIds = clients.Keys.ToList();
                        var (productsQuery, productsParams) = ClientDBScript.SelectAllproductsByClientsIdsPaged(clientIds);
                        var productsResult = await connection.QueryAsync<Product>(productsQuery, productsParams);

                        foreach (var product in productsResult)
                        {
                            if (clients.TryGetValue(product.ClientId!.Value, out var client))
                            {
                                if (client.Products == null)
                                {
                                    client.Products = new List<Product>();
                                }
                                client.Products.Add(product);
                            }
                        }
                    }

                    // Contagem total de clientes para paginação
                    string countQuery = ClientDBScript.SelectTotalCountClients();
                    int totalCount = await connection.ExecuteScalarAsync<int>(countQuery);

                    return (clients.Values.ToList(), totalCount); // Retorna a lista de clientes e o total para paginação
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Client> GetClientById(long id)
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    var clients = new Dictionary<long, Client>();

                    var query = ClientDBScript.SelectClientById(id);

                    foreach (KeyValuePair<string, object> item in query)
                    {
                        var result = await connection.QueryAsync<Client, Product, Client>(item.Key,
                    map: (client, product) =>
                    {
                        if (!clients.TryGetValue(client.Id, out var currentClient))
                        {
                            currentClient = client;
                            currentClient.Products = new List<Product>();
                            clients[client.Id] = currentClient;
                        }
                        if (product != null)
                        {
                            currentClient.Products?.Add(product);
                        }

                        return currentClient;
                    }, item.Value);

                        return result.FirstOrDefault()!;
                    }

                    return null!;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}