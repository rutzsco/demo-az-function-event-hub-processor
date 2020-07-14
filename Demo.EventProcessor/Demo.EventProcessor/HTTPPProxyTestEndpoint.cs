using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace Demo.EventProcessor
{
    public static class HTTPPProxyTestEndpoint
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        [FunctionName("HTTPPProxyTestEndpoint")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ExecutionContext context, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var config = new ConfigurationBuilder()
                                .SetBasePath(context.FunctionAppDirectory)
                                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                                .AddEnvironmentVariables()
                                .Build();

            var backendServiceUrl = config["BackendServiceUrl"];
            var response = await _httpClient.GetAsync(backendServiceUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("An error occured invoking measurement service.");
            }

            return new OkObjectResult(response.Content);
        }
    }
}
