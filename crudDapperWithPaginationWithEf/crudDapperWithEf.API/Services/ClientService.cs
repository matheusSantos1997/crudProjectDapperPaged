using crudDapperWithEf.API.DTOs.Client;
using crudDapperWithEf.API.DTOs.Product;
using crudDapperWithEf.API.Models;
using crudDapperWithEf.API.Repositories.Interfaces;
using crudDapperWithEf.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace crudDapperWithEf.API.Services
{
    public class ClientService : IClientService
    {
        private readonly IGenericRepository _genericRepository;

        private readonly IClientRepository _clientRepository;

        public ClientService(IGenericRepository genericRepository, IClientRepository clientRepository)
        {
            _genericRepository = genericRepository;
            _clientRepository = clientRepository;
        }

        public async Task<(IEnumerable<ClientDTO> Clients, int TotalCount)> ListAllClients(int pageNumber, int pageSize)
        {
            try
            {
                var (clients, totalCount) = await _clientRepository.GetAllClients(pageNumber, pageSize);

                if (clients == null || !clients.Any()) return (Enumerable.Empty<ClientDTO>(), 0);

                var clientsDto = new List<ClientDTO>();

                foreach (var client in clients)
                {
                    // Mapeia manualmente os Produtos para ProdutoDTO
                    var productsDto = new List<ProductDTO>();

                    if (client.Products != null)
                    {
                        foreach (var product in client.Products)
                        {
                            var productDto = new ProductDTO
                            {
                                Id = product.Id,
                                ProductName = product.ProductName!,
                                ProductType = product.ProductType!,
                                Price = (decimal)product.Price!,
                                ClientId = product.ClientId
                            };

                            productsDto.Add(productDto);
                        }
                    }

                    var clientDto = new ClientDTO
                    {
                        Id = client.Id,
                        ClientName = client.ClientName!,
                        Email = client.Email!,
                        Address = client.Address!,
                        Products = productsDto
                    };

                    clientsDto.Add(clientDto);
                }

                return (clientsDto, totalCount); // Retorna a lista de clientes e a contagem total
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ClientDTO>> ListAllClients()
        {
            try
            {
                var clients = await _clientRepository.GetAllClients();

                if (clients == null) return null!;

                var clientsDto = new List<ClientDTO>();

                // Mapeia cada cliente para o DTO correspondente
                foreach (var client in clients)
                {
                    // Mapeia manualmente os Produtos para ProdutoDTO
                    var productsDto = client.Products?.Select(product => new ProductDTO
                    {
                        Id = product.Id,
                        ProductName = product.ProductName!,
                        ProductType = product.ProductType!,
                        Price = (decimal)product.Price!,
                        ClientId = product.ClientId

                        // Adicione outras propriedades conforme necessário
                    }).ToList();

                    var clientDto = new ClientDTO()
                    {
                        Id = client.Id,
                        ClientName = client.ClientName!,
                        Email = client.Email!,
                        Address = client.Address!,
                        Products = productsDto!
                    };

                    clientsDto.Add(clientDto);
                }

                return clientsDto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(IEnumerable<ClientDTO> Clients, int TotalCount)> ListClientsFilter(string? nameClient, string? nameProduct, int pageNumber, int pageSize)
        {
            try
            {
                var (clients, totalCount) = await _clientRepository.GetClientsWithFilters(nameClient, nameProduct, pageNumber, pageSize);

                if (clients == null || !clients.Any()) return (Enumerable.Empty<ClientDTO>(), 0);

                var clientsDto = new List<ClientDTO>();

                foreach (var client in clients)
                {
                    // Mapeia manualmente os Produtos para ProdutoDTO
                    var productsDto = new List<ProductDTO>();

                    if (client.Products != null)
                    {
                        foreach (var product in client.Products)
                        {
                            var productDto = new ProductDTO
                            {
                                Id = product.Id,
                                ProductName = product.ProductName!,
                                ProductType = product.ProductType!,
                                Price = product.Price ?? 0,
                                ClientId = product.ClientId
                            };

                            productsDto.Add(productDto);
                        }
                    }

                    var clientDto = new ClientDTO
                    {
                        Id = client.Id,
                        ClientName = client.ClientName!,
                        Email = client.Email!,
                        Address = client.Address!,
                        Products = productsDto
                    };

                    clientsDto.Add(clientDto);
                }

                return (clientsDto, totalCount); // Retorna a lista de clientes e a contagem total

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ClientDTO> ListClientById(long id)
        {
            var client = await _clientRepository.GetClientById(id);

                if (client == null) return null!;

                // Mapeia manualmente os Produtos para ProdutoDTO
                var productsDto = client.Products?.Select(product => new ProductDTO
                {
                    Id = product.Id,
                    ProductName = product.ProductName!,
                    ProductType = product.ProductType!,
                    Price = (decimal)product.Price!,
                    ClientId = product.ClientId

                    // Adicione outras propriedades conforme necessário
                }).ToList();

                var clientDto = new ClientDTO()
                {
                    Id = client.Id,
                    ClientName = client.ClientName!,
                    Email = client.Email!,
                    Address = client.Address!,
                    Products = productsDto!
                };

                return clientDto;
        }

        public async Task<CreateNewClientDTO> AddNewClient(CreateNewClientDTO clientDto)
        {
            try
            {
                Client client = new()
                {
                    ClientName = clientDto.ClientName,
                    Email = clientDto.Email,
                    Address = clientDto.Address,
                    Products = clientDto.Products?.Select(productDto => new Product
                    {
                        ProductName = productDto.ProductName,
                        ProductType = productDto.ProductType,
                        Price = productDto.Price
                    }).ToList()
                };

                _genericRepository.Add(client);

                bool save = await _genericRepository.CommitAsync();

                var clientEntity = await _clientRepository.GetClientById(client.Id);

                CreateNewClientDTO resultClientDto = new()
                {
                    ClientName = clientEntity.ClientName!,
                    Email = clientEntity.Email!,
                    Address = clientEntity.Address!,
                    Products = clientEntity.Products?.Select(productDto => new CreateNewProductDTO
                    {
                        ProductName = productDto.ProductName!,
                        ProductType = productDto.ProductType!,
                        Price = (decimal)productDto.Price!
                    }).ToList()
                };

                if (save)
                {
                    return resultClientDto;
                }

                return null!;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UpdateClientDTO> UpdateClient(long id, UpdateClientDTO clientDto)
        {
            try
            {
                // Usar Dapper para recuperar o cliente existente e seus produtos
                var clientExisting = await _clientRepository.GetClientById(id);

                if (clientExisting == null) return null!;

                var client = new Client
                {
                    Id = clientDto.Id,
                    ClientName = clientDto.ClientName,
                    Email = clientDto.Email,
                    Address = clientDto.Address,
                    Products = clientDto.Products?.Select(p => new Product
                    {
                        Id = p.Id,
                        ProductName = p.ProductName,
                        ProductType = p.ProductType,
                        Price = p.Price,
                        ClientId = p.ClientId
                    }).ToList()
                };

                // Atualiza as propriedades do cliente existente com os dados do cliente atualizado
                clientExisting.ClientName = client.ClientName;
                clientExisting.Email = client.Email;
                clientExisting.Address = client.Address;

                // Recupera os produtos existentes
                var productsExistings = clientExisting.Products?.ToList();

                // Exclui produtos que não estão na nova lista
                foreach (var productExisting in productsExistings!)
                {
                    if (!client.Products!.Any(p => p.Id == productExisting.Id))
                    {
                        _genericRepository.Delete(productExisting);
                    }
                }

                // Atualiza ou adiciona novos produtos
                foreach (var productUpdated in client.Products!)
                {
                    var productExisting = productsExistings.FirstOrDefault(p => p.Id == productUpdated.Id);

                    if (productExisting != null)
                    {
                        // Atualiza o produto existente
                        productExisting.ProductName = productUpdated.ProductName;
                        productExisting.Price = productUpdated.Price;
                        // Atualize outras propriedades conforme necessário

                        // Marca o produto existente como modificado
                        _genericRepository.Attach(productExisting, EntityState.Modified);
                    }
                    else
                    {
                        // Adiciona o novo produto
                        productUpdated.ClientId = client.Id;
                        _genericRepository.Add(productUpdated);
                    }
                }

                // Marca o cliente existente como modificado
                //_genericRepository.Attach(clienteExistente, EntityState.Modified);
                _genericRepository.Update(clientExisting);

                bool save = await _genericRepository.CommitAsync();

                var clientEntity = await _clientRepository.GetClientById(client.Id);

                // mapeamento de retorno da entidade cliente para updateClienteDTO
                UpdateClientDTO resultClientDto = new()
                {
                    Id = clientEntity.Id,
                    ClientName = clientEntity.ClientName!,
                    Email = clientEntity.Email!,
                    Address = clientEntity.Address!,
                    Products = clientEntity.Products?.Select(productDto => new UpdateProductDTO
                    {
                        Id = productDto.Id,
                        ProductName = productDto.ProductName!,
                        ProductType = productDto.ProductType!,
                        Price = (decimal)productDto.Price!,
                        ClientId = productDto.ClientId
                    }).ToList()
                };

                if (save)
                {
                    // Usa Dapper para recuperar o cliente atualizado
                    return resultClientDto;
                }

                return null!;
            }
            catch(DbUpdateConcurrencyException ex)
            {
                throw new DbUpdateConcurrencyException(ex.Message);
            }
        }

        public async Task<bool> DeleteClient(long id)
        {
            try
            {
                var client = await _clientRepository.GetClientById(id);

                if (client == null) return false;

                _genericRepository.Delete(client);

                await _genericRepository.CommitAsync();

                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}