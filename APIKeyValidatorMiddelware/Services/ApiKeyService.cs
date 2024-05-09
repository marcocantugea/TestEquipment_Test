using APIKeyValidatorMiddelware.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace APIKeyValidatorMiddelware.Services
{
    public class ApiKeyService : IDisposable
    {
        private SqlConnection _connection;

        public ApiKeyService(string connectionString)
        {
            _connection=new SqlConnection(connectionString);
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State==ConnectionState.Open) { 
                _connection.Close();
            }
        }

        public async Task<bool> IsAPIKeyValid(string service,string apikey) {
            try
            {
                if(String.IsNullOrEmpty(service)) return false;

                var isValid = false;

                _connection.Open();

                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@Option", "Validate_APIKEY"));
                parameters.Add(new SqlParameter("@APIService", service));

                var spSystemApiServices = new SqlCommand("SP_System_ApiServices", _connection);
                spSystemApiServices.CommandType = System.Data.CommandType.StoredProcedure;
                spSystemApiServices.Parameters.AddRange(parameters.ToArray());

                

                var records=spSystemApiServices.ExecuteReader();
                var models=new List<SystemApiServiceRow>();
                while(records.Read())
                {
                    models.Add(MapReader((IDataRecord)records));
                }

                if (models.Where(x => x.API_Key == apikey).Any())
                {
                    isValid = true;
                }


                return isValid;
            }
            catch (Exception)
            {
                throw;
            }finally { 
                _connection.Close(); 
            }
        }

        private SystemApiServiceRow MapReader(IDataRecord record)
        {
            return new SystemApiServiceRow()
            {
                Id = record.GetInt32(0),
                API_Service=record.GetString(1),
                API_Key=record.GetString(2),
            };
        } 
    }
}
