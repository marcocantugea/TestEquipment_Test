using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TestEquipment_Test.Models.Data;

namespace TestEquipment_Test.Controllers
{
    [ApiController]
    [Route("areas")]
    public class AreasController : ControllerBase
    {
        private readonly BDContext _context;
        public AreasController(BDContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Areas>>> GetAreasByPlantID(string plantID)
        {
            try
            {
                var _params = new List<SqlParameter>();
                _params.Add(new SqlParameter("@Option", "GetAreas"));
                _params.Add(new SqlParameter("@Value", plantID));
                var areas = await _context.Areas
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


    public class Areas
    {
        public string? AreaID { get; set; }
        public string? Area { get; set; }
    }


}
