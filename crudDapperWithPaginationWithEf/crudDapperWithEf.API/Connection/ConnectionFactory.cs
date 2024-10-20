using System.Data;
using System.Data.SqlClient;

namespace crudDapperWithEf.API.Connection
{
    public class ConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public ConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            string conn = _configuration.GetConnectionString("localConnection");
            return new SqlConnection(conn);
        }
    }
}