using APIKeyValidatorMiddelware.Midlewares.ApiKey;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace TestEquipment_Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[ApiKeyValidation]
    public class ApiKeyTestController : ControllerBase
    {
        [HttpGet]
        [ApiKeyValidation]
        public async Task<IActionResult> TestApiKey()
        {
            return Ok();
        }

        [HttpGet("publico")]
        public async Task<IActionResult> PublicMetodo()
        {
            return Ok();
        }
    }
}
