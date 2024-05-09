using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net;
using TestEquipment_Test.Models.Data;

namespace TestEquipment_Test.Controllers
{
    [Route("plants")]
    [ApiController]
    public class PlantsController : ControllerBase
    {

        private readonly BDContext _context;
        public PlantsController(BDContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plants>>> GetPlants()
        {
            try
            {
                var plants = await _context.Plants
                    .FromSqlRaw(@"exec dbo.SP_FWDAPI @Option", new SqlParameter("@Option", "GetPlants"))
                    .ToListAsync();

                return Ok(plants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class Plants
    {
        public string? name { get; set; }
        public string? value { get; set; }
    }

}
