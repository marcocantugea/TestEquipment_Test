using APIKeyValidatorMiddelware.Midlewares.ApiKey;
using Microsoft.AspNetCore.Mvc;

namespace APIKeyValidatorMiddelware.Midlewares.ApiKey
{
    public class ApiKeyValidationAttribute : TypeFilterAttribute
    {
        public ApiKeyValidationAttribute() : base(typeof(ApiKeyFilter))
        {

        }
    }
}
