using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TestEquipment_Test.Models.Data;

namespace TestEquipment_Test.Controllers
{
    [ApiController]
    [Route("checkins")]
    public class CheckInController : ControllerBase
    {
        private readonly string _connectionString;

        public CheckInController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection"); 
        }

        [HttpPost]
        public IActionResult Checkin(string employeeId, string stationId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_FWDAPI", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Option", "GetCheckIn");
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);
                    command.Parameters.AddWithValue("@StationID", stationId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string message = "";
                            string select = reader.GetString(0); // Mensaje recibido del select
                            bool isLoggedIn = false;
                            if (select == "CHECK IN") // Si el select es checkin entonces acaba iniciar sesion, si no, es un checkout
                            {
                                isLoggedIn = true;
                            }
                            else if (select == "CHECK OUT") {
                                isLoggedIn = false;
                                message = "CHECKOUT";
                            }
                            else if (select == "NOT ALLOWED") {
                                isLoggedIn = false;
                                message = "NOTALLOWED";
                            }
                            else if (select == "FAILED") {
                                isLoggedIn = false;
                                message = select;
                            }
                            else if (select == "NO SHIFT")
                            {
                                isLoggedIn = false;
                                message = "NOSHIFT";
                            }
                            else if (select == "EMPLOYEE_NOT_FOUND")
                            {
                                isLoggedIn = false;
                                message = select;
                            }
                            else {
                                isLoggedIn = false;
                                message = "ERROR";
                            }
                            var response = new CheckIn_
                            {
                                success = isLoggedIn.ToString(),
                                message = message
                            };

                            return Ok(response);
                        }
                        else
                        {
                            return BadRequest("No se pudo obtener el resultado del procedimiento almacenado.");
                        }
                    }
                }
            }
        }

    }

    public class CheckIns
    {
        public string? OperationID { get; set; }
    }

}