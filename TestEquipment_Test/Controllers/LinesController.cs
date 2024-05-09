using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TestEquipment_Test.Models.Data;

namespace TestEquipment_Test.Controllers
{
    [ApiController]
    [Route("lines")]
    public class LinesController : ControllerBase
    {

        private readonly BDContext _context;
        public LinesController(BDContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lines>>> GetLinesByAreaID(string areaID)
        {
            try
            {
                var _params = new List<SqlParameter>();
                _params.Add(new SqlParameter("@Option", "GetLines"));
                _params.Add(new SqlParameter("@Value", areaID));
                var areas = await _context.Lines
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

    public class Lines
    {
        public string? LineID { get; set; }
        public string? Line { get; set; }
    }

}

