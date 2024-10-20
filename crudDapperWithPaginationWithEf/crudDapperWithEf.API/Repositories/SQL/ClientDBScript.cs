using System.Text;
using Dapper;

namespace crudDapperWithEf.API.Repositories.SQL
{
    public class ClientDBScript
    {
        public static string SelectAllClientsPaged()
        {
            const string sql = @"SELECT DISTINCT c.* FROM Clients c ORDER BY c.Id
                                 OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            return sql;
        }

        public static (string Sql, object Parameters) SelectAllproductsByClientsIdsPaged(IEnumerable<long> clientIds)
        {
            const string sql = @"SELECT * FROM Products WHERE ClientId IN @ClientIds;";

            return (sql, new { ClientIds = clientIds });
        }

        public static string SelectTotalCountClients()
        {
            const string sql = @"SELECT COUNT(*) FROM Clients;";

            return sql;
        }

        public static string SelectAllClients()
        {
            const string sql = @"SELECT * FROM Clients AS c LEFT JOIN Products AS p ON p.ClientId = c.Id;";

            return sql;
        }

        public static Dictionary<string, object> SelectClientById(long id)
        {
            const string sql = @"SELECT * FROM Clients AS c LEFT JOIN Products AS p ON p.ClientId = c.Id WHERE c.Id = @Id";

            return new Dictionary<string, object> { { sql, new { Id = id } } };

        }

        public static (string Sql, object Parameters) FilterPaged(string? clientName, string? productName, int pageNumber, int pageSize)
        {
            var sql = new StringBuilder(@"SELECT DISTINCT c.* FROM Clients c ");
            var parameters = new DynamicParameters();
            int offset = (pageNumber - 1) * pageSize;

            // Se o nome do produto não for vazio, faz a junção com a tabela de produtos
            if (!string.IsNullOrEmpty(productName))
            {
                sql.Append("LEFT JOIN Products p ON c.Id = p.ClientId ");
                sql.Append("WHERE p.ProductName COLLATE Latin1_General_CI_AI LIKE @ProductName ");
                parameters.Add("ProductName", productName + "%");
            }

            // Se o nome do cliente não for vazio, adiciona a condição de filtro
            if (!string.IsNullOrEmpty(clientName))
            {
                if (sql.ToString().Contains("WHERE"))
                {
                    sql.Append("AND ");
                }
                else
                {
                    sql.Append("WHERE ");
                }
                sql.Append("c.ClientName COLLATE Latin1_General_CI_AI LIKE @ClientName ");
                parameters.Add("ClientName", clientName + "%");
            }

            // Adiciona a cláusula de paginação
            sql.Append(@"ORDER BY c.Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;");
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);

            return (sql.ToString(), parameters);
        }
    }
}