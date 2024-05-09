using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TestEquipment_Test.Models.Data;

namespace TestEquipment_Test.Controllers
{
    [ApiController]
    [Route("stations")]
    public class StationsController : ControllerBase
    {

        private readonly BDContext _context;
        public StationsController(BDContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stations>>> GetStationsByLineID(string lineID)
        {
            try
            {
                var _params = new List<SqlParameter>();
                _params.Add(new SqlParameter("@Option", "GetStations"));
                _params.Add(new SqlParameter("@Value", lineID));
                var areas = await _context.Stations
                    //.FromSqlRaw("SELECT StationId, Station FROM Station WHERE LineId = @lineID", new SqlParameter("@lineID", lineID))
                    .FromSqlRaw(@"exec dbo.SP_FWDAPI @Option, @Value", _params.ToArray())
                    .ToListAsync();

                return Ok(areas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class Stations
    {
        public string? StationId { get; set; }
        public string? Station { get; set; }
    }

}