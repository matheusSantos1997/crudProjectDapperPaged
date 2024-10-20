using System.Text.Json;
using crudDapperWithEf.API.Middlewares;
using crudDapperWithEf.API.Extensions;
using crudDapperWithEf.API.Services.Interfaces;
using crudDapperWithEf.API.Shared;
using Microsoft.AspNetCore.Mvc;
using crudDapperWithEf.API.DTOs.Client;

namespace crudDapperWithEf.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ApiController
    {
        private readonly IClientService _clientService;

        private readonly IExcelExportService _excelExportService;

        public ClientsController(IClientService clientService, IExcelExportService excelExportService)
        {
            _clientService = clientService;
            _excelExportService = excelExportService;
        }

        [CustomResponse(StatusCodes.Status200OK)]
        [Route("GetAllClients")]
        [HttpGet]
        public async Task<IActionResult> GetAllClients([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var (clients, totalCount) = await _clientService.ListAllClients(pageNumber, pageSize);

                if (!clients.Any()) return ResponseNoContent();

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

                return ResponseOk(clients, "Sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex.Message);
            }
        }

        [CustomResponse(StatusCodes.Status200OK)]
        [Route("GetClientsFilter/GetByFilter")]
        [HttpGet]
        public async Task<IActionResult> GetClientsFilter([FromQuery] string? clientName = "", [FromQuery] string? productName = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 3)
        {
            try
            {
                // Se ambos os parâmetros forem nulos ou vazios, retornar uma lista vazia
                if (string.IsNullOrWhiteSpace(clientName) && string.IsNullOrWhiteSpace(productName))
                {
                    return ResponseOk(new List<ClientDTO>(), "Nenhum cliente encontrado.");
                }

                var (clients, totalCount) = await _clientService.ListClientsFilter(
            string.IsNullOrWhiteSpace(clientName) ? null : clientName,
            productName,
            pageNumber,
            pageSize);

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

                return ResponseOk(clients, "Sucesso!");
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
                var clients = await _clientService.ListAllClients();

                if (clients == null || !clients.Any())
                {
                    return ResponseNotFound();
                }

                var propertiesToExport = GetColumsCells();

                if (asBase64)
                {
                    // Retornar como Base64
                    var base64Content = await _excelExportService.ExportToExcelBase64Async(clients, "Clientes", propertiesToExport);

                    if (string.IsNullOrEmpty(base64Content))
                    {
                        return ResponseInternalServerError("Falha ao exportar um arquivo.");
                    }

                    return ResponseOk(new { fileName = "Clientes.xlsx", content = base64Content });
                }
                else
                {
                    // Retornar como array de bytes
                    var content = await _excelExportService.ExportToExcelAsync(clients, "Clientes", propertiesToExport);

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
        [Route("GetClientById/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetClientById(long id)
        {
            try
            {
                var client = await _clientService.ListClientById(id);

                if (client == null) return ResponseNotFound();

                return ResponseOk(client, "Sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex.Message);
            }
        }

        [CustomResponse(StatusCodes.Status201Created)]
        [Route("InsertNewCliente")]
        [HttpPost]
        public async Task<IActionResult> InsertNewCliente(CreateNewClientDTO clientDto)
        {
            try
            {
                var client = await _clientService.AddNewClient(clientDto);

                if (!ModelState.IsValid) return ResponseBadRequest();

                return ResponseCreated(client, "Cliente cadastrado com sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex.Message);
            }
        }

        [CustomResponse(StatusCodes.Status200OK)]
        [Route("UpdateCliente/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateCliente(long id, UpdateClientDTO clientDto)
        {
            try
            {
                var client = await _clientService.UpdateClient(id, clientDto);

                if (client == null) return ResponseBadRequest();

                return ResponseOk(client, "Cliente atualizado com sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex.Message);
            }
        }

        [CustomResponse(StatusCodes.Status200OK)]
        [Route("DeleteClient/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeletCliente(long id)
        {
            try
            {
                var deleted = await _clientService.DeleteClient(id);

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