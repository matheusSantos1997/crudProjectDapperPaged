namespace crudDapperEfCore.API.Repositories.SQL
{
    public class ClienteDBScript
    {
        public static string SelectAllClientesPaged()
        {
            const string sql = @"SELECT DISTINCT c.* FROM Clientes c ORDER BY c.Id
                                 OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            return sql;
        }

        public static (string Sql, object Parameters) SelectAllprodutosByClientesIdsPaged(IEnumerable<long> clienteIds)
        {
            const string sql = "SELECT * FROM Produtos WHERE ClienteId IN @ClienteIds;";

            return (sql, new { ClienteIds = clienteIds });
        }

        public static string SelectAllClientes()
        {
            const string sql = @"SELECT * FROM Clientes as c LEFT JOIN 
                                               Produtos as p ON p.ClienteId = c.Id;";

            return sql;
        }

        public static Dictionary<string, object> SelectClientePeloId(long id)
        {
            const string sql = @"SELECT * FROM Clientes as c LEFT JOIN 
                                               Produtos as p ON p.ClienteId = c.Id WHERE c.Id = @Id";

            return new Dictionary<string, object> { { sql, new { Id = id } } };

        }

        public static Dictionary<string, object> FiltrarPaged(string? nomeCliente, string? nomeProduto, int pageNumber, int pageSize)
        {
            const string sql = @"SELECT DISTINCT c.* FROM Clientes c 
                               LEFT JOIN Produtos p ON c.Id = p.ClienteId
                               WHERE NomeCliente COLLATE Latin1_General_CI_AI LIKE @NomeCliente
                               OR (p.NomeProduto COLLATE Latin1_General_CI_AI LIKE @NomeProduto OR p.NomeProduto IS NULL)
                               ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            int offset = (pageNumber - 1) * pageSize;

            return new Dictionary<string, object>
            {
                { "Sql", sql },
                { "Parameters", new { NomeCliente = nomeCliente + "%", NomeProduto = nomeProduto + "%", Offset = offset, PageSize = pageSize } }
            };
        }
    }
}