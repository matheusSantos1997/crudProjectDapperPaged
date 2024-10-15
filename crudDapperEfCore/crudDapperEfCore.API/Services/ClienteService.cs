using crudDapperEfCore.API.DTOs.Cliente;
using crudDapperEfCore.API.DTOs.Produto;
using crudDapperEfCore.API.models;
using crudDapperEfCore.API.Pagination;
using crudDapperEfCore.API.Repositories.Interfaces;
using crudDapperEfCore.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace crudDapperEfCore.API.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IGenericRepository _genericRepository;

        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IGenericRepository genericRepository, IClienteRepository clienteRepository)
        {
            _genericRepository = genericRepository;
            _clienteRepository = clienteRepository;
        }

        public async Task<(IEnumerable<ClienteDTO> Clientes, int TotalCount)> ListarTodosClientes(int pageNumber, int pageSize)
        {
            try
            {
                var (clientes, totalCount) = await _clienteRepository.GetAllClientes(pageNumber, pageSize);

                if (clientes == null || !clientes.Any()) return (Enumerable.Empty<ClienteDTO>(), 0);

                var clientesDto = new List<ClienteDTO>();

                foreach (var cliente in clientes)
                {
                    // Mapeia manualmente os Produtos para ProdutoDTO
                    var produtosDto = new List<ProdutoDTO>();

                    if (cliente.Produtos != null)
                    {
                        foreach (var produto in cliente.Produtos)
                        {
                            var produtoDto = new ProdutoDTO
                            {
                                Id = produto.Id,
                                NomeProduto = produto.NomeProduto!,
                                TipoProduto = produto.TipoProduto!,
                                Preco = (decimal)produto.Preco!,
                                ClienteId = produto.ClienteId
                            };
                            produtosDto.Add(produtoDto);
                        }
                    }

                    var clienteDto = new ClienteDTO
                    {
                        Id = cliente.Id,
                        NomeCliente = cliente.NomeCliente!,
                        Email = cliente.Email!,
                        Endereco = cliente.Endereco!,
                        Produtos = produtosDto
                    };

                    clientesDto.Add(clienteDto);
                }

                return (clientesDto, totalCount); // Retorna a lista de clientes e a contagem total
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ClienteDTO>> ListarTodosClientes()
        {
            try
            {
                var clientes = await _clienteRepository.GetAllClientes();

                if (clientes == null) return null!;

                var clientesDto = new List<ClienteDTO>();

                // Mapeia cada cliente para o DTO correspondente
                foreach (var cliente in clientes)
                {
                    // Mapeia manualmente os Produtos para ProdutoDTO
                    var produtosDto = cliente.Produtos?.Select(produto => new ProdutoDTO
                    {
                        Id = produto.Id,
                        NomeProduto = produto.NomeProduto!,
                        TipoProduto = produto.TipoProduto!,
                        Preco = (decimal)produto.Preco!,
                        ClienteId = produto.ClienteId

                        // Adicione outras propriedades conforme necessário
                    }).ToList();

                    var clienteDto = new ClienteDTO()
                    {
                        Id = cliente.Id,
                        NomeCliente = cliente.NomeCliente!,
                        Email = cliente.Email!,
                        Endereco = cliente.Endereco!,
                        Produtos = produtosDto!
                    };

                    clientesDto.Add(clienteDto);
                }

                return clientesDto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ClienteDTO> ListarClientePorId(long id)
        {
            try
            {
                var cliente = await _clienteRepository.GetClienteById(id);

                if (cliente == null) return null!;

                // Mapeia manualmente os Produtos para ProdutoDTO
                var produtosDto = cliente.Produtos?.Select(produto => new ProdutoDTO
                {
                    Id = produto.Id,
                    NomeProduto = produto.NomeProduto!,
                    TipoProduto = produto.TipoProduto!,
                    Preco = (decimal)produto.Preco!,
                    ClienteId = produto.ClienteId

                    // Adicione outras propriedades conforme necessário
                }).ToList();

                var clienteDto = new ClienteDTO()
                {
                    Id = cliente.Id,
                    NomeCliente = cliente.NomeCliente!,
                    Email = cliente.Email!,
                    Endereco = cliente.Endereco!,
                    Produtos = produtosDto!
                };

                return clienteDto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(IEnumerable<ClienteDTO> Clientes, int TotalCount)> ListarTodosClientesPorNome(string? nome, string? nomeProduto, int pageNumber, int pageSize)
        {
            try
            {
                var (clientes, totalCount) = await _clienteRepository.GetClienteByNome(nome, nomeProduto, pageNumber, pageSize);

                if (clientes == null || !clientes.Any()) return (Enumerable.Empty<ClienteDTO>(), 0);

                var clientesDto = new List<ClienteDTO>();

                // Mapeia cada cliente para o DTO correspondente
                foreach (var cliente in clientes)
                {
                    // Mapeia manualmente os Produtos para ProdutoDTO
                    var produtosDto = new List<ProdutoDTO>();
                    
                    if (cliente.Produtos != null)
                    {
                        foreach (var produto in cliente.Produtos)
                        {
                            var produtoDto = new ProdutoDTO
                            {
                                Id = produto.Id,
                                NomeProduto = produto.NomeProduto!,
                                TipoProduto = produto.TipoProduto!,
                                Preco = (decimal)produto.Preco!,
                                ClienteId = produto.ClienteId
                            };
                            produtosDto.Add(produtoDto);
                        }
                    }

                    var clienteDto = new ClienteDTO
                    {
                        Id = cliente.Id,
                        NomeCliente = cliente.NomeCliente!,
                        Email = cliente.Email!,
                        Endereco = cliente.Endereco!,
                        Produtos = produtosDto
                    };

                    clientesDto.Add(clienteDto);
                }

                return (clientesDto, totalCount); // Retorna a lista de clientes e a contagem total
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CreateNewClienteDTO> AdicionarNovoCliente(CreateNewClienteDTO clienteDto)
        {
            try
            {
                Cliente cliente = new()
                {
                    NomeCliente = clienteDto.NomeCliente,
                    Email = clienteDto.Email,
                    Endereco = clienteDto.Endereco,
                    Produtos = clienteDto.Produtos.Select(produtoDto => new Produto
                    {
                        NomeProduto = produtoDto.NomeProduto,
                        TipoProduto = produtoDto.TipoProduto,
                        Preco = produtoDto.Preco
                    }).ToList()
                };

                _genericRepository.Adicionar(cliente);

                bool save = await _genericRepository.CommitAsync();

                var clienteEntity = await _clienteRepository.GetClienteById(cliente.Id);

                CreateNewClienteDTO resultClienteDto = new()
                {
                    NomeCliente = clienteEntity.NomeCliente!,
                    Email = clienteEntity.Email!,
                    Endereco = clienteEntity.Endereco!,
                    Produtos = clienteEntity.Produtos!.Select(produtoDto => new CreateNewProdutoDTO
                    {
                        NomeProduto = produtoDto.NomeProduto!,
                        TipoProduto = produtoDto.TipoProduto!,
                        Preco = (decimal)produtoDto.Preco!
                    }).ToList()
                };

                if (save)
                {
                    return resultClienteDto;
                }

                return null!;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UpdateClienteDTO> AtualizarCliente(long id, UpdateClienteDTO clienteDto)
        {
            try
            {
                // Usar Dapper para recuperar o cliente existente e seus produtos
                var clienteExistente = await _clienteRepository.GetClienteById(id);

                if (clienteExistente == null) return null!;

                var cliente = new Cliente
                {
                    Id = clienteDto.Id,
                    NomeCliente = clienteDto.NomeCliente,
                    Email = clienteDto.Email,
                    Endereco = clienteDto.Endereco,
                    Produtos = clienteDto.Produtos.Select(p => new Produto
                    {
                        Id = p.Id,
                        NomeProduto = p.NomeProduto,
                        TipoProduto = p.TipoProduto,
                        Preco = p.Preco,
                        ClienteId = p.ClienteId
                    }).ToList()
                };

                // Atualiza as propriedades do cliente existente com os dados do cliente atualizado
                clienteExistente.NomeCliente = cliente.NomeCliente;
                clienteExistente.Email = cliente.Email;
                clienteExistente.Endereco = cliente.Endereco;

                // Recupera os produtos existentes
                var produtosExistentes = clienteExistente.Produtos?.ToList();

                // Exclui produtos que não estão na nova lista
                foreach (var produtoExistente in produtosExistentes!)
                {
                    if (!cliente.Produtos.Any(p => p.Id == produtoExistente.Id))
                    {
                        _genericRepository.Excluir(produtoExistente);
                    }
                }

                // Atualiza ou adiciona novos produtos
                foreach (var produtoAtualizado in cliente.Produtos)
                {
                    var produtoExistente = produtosExistentes.FirstOrDefault(p => p.Id == produtoAtualizado.Id);

                    if (produtoExistente != null)
                    {
                        // Atualiza o produto existente
                        produtoExistente.NomeProduto = produtoAtualizado.NomeProduto;
                        produtoExistente.Preco = produtoAtualizado.Preco;
                        // Atualize outras propriedades conforme necessário

                        // Marca o produto existente como modificado
                        _genericRepository.Attach(produtoExistente, EntityState.Modified);
                    }
                    else
                    {
                        // Adiciona o novo produto
                        produtoAtualizado.ClienteId = cliente.Id;
                        _genericRepository.Adicionar(produtoAtualizado);
                    }
                }

                // Marca o cliente existente como modificado
                //_genericRepository.Attach(clienteExistente, EntityState.Modified);
                _genericRepository.Atualizar(clienteExistente);

                bool save = await _genericRepository.CommitAsync();

                var clienteEntity = await _clienteRepository.GetClienteById(cliente.Id);

                // mapeamento de retorno da entidade cliente para updateClienteDTO
                UpdateClienteDTO resultClienteDto = new()
                {
                    Id = clienteEntity.Id,
                    NomeCliente = clienteEntity.NomeCliente!,
                    Email = clienteEntity.Email!,
                    Endereco = clienteEntity.Endereco!,
                    Produtos = clienteEntity.Produtos!.Select(produtoDto => new UpdateProdutoDTO
                    {
                        Id = produtoDto.Id,
                        NomeProduto = produtoDto.NomeProduto!,
                        TipoProduto = produtoDto.TipoProduto!,
                        Preco = (decimal)produtoDto.Preco!,
                        ClienteId = produtoDto.ClienteId
                    }).ToList()
                };

                if (save)
                {
                    // Usa Dapper para recuperar o cliente atualizado
                    return resultClienteDto;
                }

                return null!;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new DbUpdateConcurrencyException(ex.Message);
            }
        }

        public async Task<bool> DeletarCliente(long id)
        {
            try
            {
                var cliente = await _clienteRepository.GetClienteById(id);

                if (cliente == null) return false;

                _genericRepository.Excluir(cliente);

                await _genericRepository.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}