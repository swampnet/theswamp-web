using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TheSwamp.Api.DAL.LWIN;
using TheSwamp.Shared;

namespace TheSwamp.Api
{
    public class WineFunction
    {
        private readonly IConfiguration _cfg;
        private readonly LWINContext _context;

        public WineFunction(IConfiguration cfg, LWINContext context)
        {
            _cfg = cfg;
            _context = context;
        }


        [FunctionName("random-review")]
        public async Task<ActionResult<string>> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wine/random-review")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("random-review");

            var lwin = await _context.GetRandomWineAsync();
            
            var review = new Review()
            {
                Id = lwin.LWIN,
                Name = lwin.DISPLAY_NAME
            };

            return new OkObjectResult(review);
        }
    }
}
