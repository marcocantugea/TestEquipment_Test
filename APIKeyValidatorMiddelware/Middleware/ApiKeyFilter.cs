using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Filters;
using APIKeyValidatorMiddelware.Services;

namespace APIKeyValidatorMiddelware.Midlewares.ApiKey
{
    public class ApiKeyFilter : IAsyncActionFilter
    {
        private readonly ApiKeyService _apiKeyService;
        private readonly IConfiguration _configuration;
        private const string API_KEY = "x-api-key";

        public ApiKeyFilter(ApiKeyService apiKeyService,IConfiguration configuration)
        {
            _apiKeyService = apiKeyService;
            _configuration = configuration;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            bool success = context.HttpContext.Request.Headers.TryGetValue(API_KEY, out var apiKeyFromHttpHeader);
            if (!success)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "The Api Key for accessing this endpoint is not available"
                };
                return;
            }
            string appService = _configuration["APP_SERVICE"];
            bool isValidKey = await _apiKeyService.IsAPIKeyValid(appService, apiKeyFromHttpHeader);
            if (!isValidKey)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "The Api key is incorrect : Unauthorized access"
                };
                return;
            }
            await next();
        }

    }
}
