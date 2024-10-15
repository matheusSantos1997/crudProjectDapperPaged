using System.Text.Json;
using crudDapperEfCore.API.DTOs.Cliente;
using crudDapperEfCore.API.Extensions;
using crudDapperEfCore.API.Middlewares;
using crudDapperEfCore.API.Pagination;
using crudDapperEfCore.API.Services.Interfaces;
using crudDapperEfCore.API.Shared;
using Microsoft.AspNetCore.Mvc;

namespace crudDapperEfCore.API.Controllers
{
    [Route("api/[controller]")]
    public class ClientesController : ApiController
    {
        private readonly IClienteService _clienteService;

        private readonly IExcelExportService _excelExportService;

        public ClientesController(IClienteService clienteService, IExcelExportService excelExportService)
        {
            _clienteService = clienteService;
            _excelExportService = excelExportService;
        }

        [CustomResponse(StatusCodes.Status200OK)]
        [Route("GetAllClientes")]
        [HttpGet]
        public async Task<IActionResult> GetAllClientes([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var (clientes, totalCount) = await _clienteService.ListarTodosClientes(pageNumber, pageSize);

                if (!clientes.Any()) return ResponseNoContent();

                //Response.AddPagination(clientes.CurrentPage, clientes.PageSize, clientes.TotalCount, clientes.TotalPages);
                // Calcula o total de páginas com base na contagem total e no tamanho da página
                int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                // Adiciona cabeçalhos de paginação
                // Cria um objeto de paginação
                var paginationHeader = new
                {
                    TotalCount = totalCount,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    HasNext = pageNumber < totalPages,
                    HasPrevious = pageNumber > 1
                };

                // Serializa o objeto para JSON
                var paginationJson = JsonSerializer.Serialize(paginationHeader);

                // Adiciona o cabeçalho de paginação como JSON
                Response.Headers.Add("X-Pagination", paginationJson);

                return ResponseOk(clientes, "Sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex.Message);
            }
        }

        [CustomResponse(StatusCodes.Status200OK)]
        [Route("GetAllClientesByNome/GetByNome/{nome}/{nomeProduto}")]
        [HttpGet]
        public async Task<IActionResult> GetAllClientesByNome(string? nome, string? nomeProduto, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var (clientes, totalCount) = await _clienteService.ListarTodosClientesPorNome(nome, nomeProduto, pageNumber, pageSize);

                //Response.AddPagination(clientes.CurrentPage, clientes.PageSize, clientes.TotalCount, clientes.TotalPages);
                // Calcula o total de páginas com base na contagem total e no tamanho da página
                int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                // Adiciona cabeçalhos de paginação
                // Cria um objeto de paginação
                var paginationHeader = new
                {
                    TotalCount = totalCount,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    HasNext = pageNumber < totalPages,
                    HasPrevious = pageNumber > 1
                };

                // Serializa o objeto para JSON
                var paginationJson = JsonSerializer.Serialize(paginationHeader);

                // Adiciona o cabeçalho de paginação como JSON
                Response.Headers.Add("X-Pagination", paginationJson);

                return ResponseOk(clientes, "Sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex.Message);
            }
        }

        [CustomResponse(StatusCodes.Status200OK)]
        [Route("ExportToExcelFile")]
        [HttpGet]
        public async Task<IActionResult> ExportToExcelFile([FromQuery] bool asBase64)
        {
            try
            {
                var clientes = await _clienteService.ListarTodosClientes();

                if (clientes == null || !clientes.Any())
                {
                    return ResponseNotFound();
                }

                var propertiesToExport = GetColumsCells();

                if (asBase64)
                {
                    // Retornar como Base64
                    var base64Content = await _excelExportService.ExportToExcelBase64Async(clientes, "Clientes", propertiesToExport);

                    if (string.IsNullOrEmpty(base64Content))
                    {
                        return ResponseInternalServerError("Falha ao exportar um arquivo.");
                    }

                    return ResponseOk(new { fileName = "Clientes.xlsx", content = base64Content });
                }
                else
                {
                    // Retornar como array de bytes
                    var content = await _excelExportService.ExportToExcelAsync(clientes, "Clientes", propertiesToExport);

                    if (content == null || content.Length == 0)
                    {
                        return ResponseInternalServerError("Falha ao exportar um arquivo.");
                    }

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Clientes.xlsx");
                }
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex.Message);
            }
        }

        [CustomResponse(StatusCodes.Status200OK)]
        [Route("GetClienteById/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetClienteById(long id)
        {
            try
            {
                var cliente = await _clienteService.ListarClientePorId(id);

                if (cliente == null) return ResponseNotFound();

                return ResponseOk(cliente, "Sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex.Message);
            }
        }

        [CustomResponse(StatusCodes.Status201Created)]
        [Route("InsertNewCliente")]
        [HttpPost]
        public async Task<IActionResult> InsertNewCliente(CreateNewClienteDTO clienteDto)
        {
            try
            {
                var cliente = await _clienteService.AdicionarNovoCliente(clienteDto);

                if (!ModelState.IsValid) return ResponseBadRequest();

                return ResponseCreated(cliente, "Cliente cadastrado com sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex.Message);
            }
        }

        [CustomResponse(StatusCodes.Status200OK)]
        [Route("UpdateClientes/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateClientes(long id, UpdateClienteDTO clienteDto)
        {
            try
            {
                var cliente = await _clienteService.AtualizarCliente(id, clienteDto);

                if (cliente == null) return ResponseBadRequest();

                return ResponseOk(cliente, "Cliente atualizado com sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex.Message);
            }
        }

        [CustomResponse(StatusCodes.Status200OK)]
        [Route("DeleteCliente/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteCliente(long id)
        {
            try
            {
                var deleted = await _clienteService.DeletarCliente(id);

                if (deleted == false) return ResponseBadRequest();

                return ResponseOk("Cliente Deletado com sucesso.");
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex.Message);
            }
        }

        [NonAction]
        private static Dictionary<string, string> GetColumsCells()
        {
            var propertiesToExport = new Dictionary<string, string>
            {
                  { "Id", "IdCliente" },
                  { "NomeCliente", "NomeCliente" },
                  { "Email", "Email" },
                  { "Endereco", "Endereço" },
                  { "NomesProdutos", "Produtos" }
            };

            return propertiesToExport;
        }
    }
}