using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TestEquipment_Test.Models.Data;

namespace TestEquipment_Test.Controllers
{
    [ApiController]
    [Route("saporder")]
    public class SapOrderController : ControllerBase
    {
        private readonly string _connectionString;

        public SapOrderController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //[HttpPost]
        //public IActionResult CheckOrderNumber(string order)
        //{
        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        using (SqlCommand command = new SqlCommand("SP_FWDAPI", connection))
        //        {
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.Parameters.AddWithValue("@Option", "CheckOrder");
        //            command.Parameters.AddWithValue("@Value", order);

        //            connection.Open();
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                if (reader.Read())
        //                {
        //                    string message = "";
        //                    string select = reader.GetString(0); // Mensaje recibido del select
        //                    bool ordenExists = false;
        //                    if (select != "NOEXISTS") // Revisar si existe la orden
        //                    {
        //                        ordenExists = true;
        //                        message = "ORDER_EXISTS";
        //                        connection.Close();
        //                    }                            
        //                    else
        //                    {
        //                        //9005774610
        //                        connection.Close();
        //                        try
        //                        {
        //                            var errorMessage = string.Empty;
        //                            if (!(SapDataHelper.SapOrders.GetOrder(order, true, false, ref errorMessage)))
        //                            {
        //                                connection.Open();
        //                                using (SqlDataReader reader2 = command.ExecuteReader())
        //                                {
        //                                    if (reader2.Read())
        //                                    {
        //                                        string select2 = reader2.GetString(0); // Mensaje recibido del select
        //                                        if (select2 == "" || select2 == "NOEXISTS") // Revisar si existe la orden
        //                                        {
        //                                            ordenExists = false;
        //                                            message = "DONT_RECEIVED";
        //                                            connection.Close();
        //                                        }
        //                                        else
        //                                        {
        //                                            ordenExists = true;
        //                                            message = "ORDER_RECEIVED";
        //                                            connection.Close();                                                    
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        return BadRequest("No se pudo obtener el resultado del procedimiento almacenado.2");
        //                                    }
        //                                }
        //                            }
        //                            else { 
        //                                ordenExists = false;
        //                                message = "SAP_FAILED";
        //                                connection.Close();
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            connection.Close();                                    
        //                            ordenExists = false;
        //                            message = "FAILED: " + ex;
        //                            using (SqlCommand command2 = new SqlCommand("SP_SystemErrorsLog", connection))
        //                            {
        //                                command.CommandType = CommandType.StoredProcedure;
        //                                command.Parameters.AddWithValue("@Option", "RecordSystemError");
        //                                command.Parameters.AddWithValue("@ErrorMessage", message);
        //                                command.Parameters.AddWithValue("@StationID", "");
        //                                command.Parameters.AddWithValue("@StationID", "");
        //                                command.Parameters.AddWithValue("@EmployeeID", "");
        //                                command.Parameters.AddWithValue("@Component", "CheckOrder_FWDAPI");
        //                                command.Parameters.AddWithValue("@SourceFunction", "CheckOrderNumber");

        //                                connection.Open();
        //                                using (SqlDataReader reader3 = command.ExecuteReader())
        //                                {
        //                                    if (reader.Read())
        //                                    {
        //                                        connection.Close();
        //                                    }
        //                                    else
        //                                    {
        //                                        connection.Close();
        //                                        return BadRequest("No se pudo obtener el resultado del procedimiento almacenado.");
        //                                    }
        //                                }
        //                            }

        //                        }


        //                    }
        //                    var response = new CheckOrder_
        //                    {
        //                        success = ordenExists.ToString(),
        //                        message = message
        //                    };

        //                    return Ok(response);
        //                }
        //                else
        //                {
        //                    return BadRequest("No se pudo obtener el resultado del procedimiento almacenado.1");
        //                }
        //            }
        //        }
        //    }
        //}

        
    }
}
