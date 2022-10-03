using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using System;
using System.Threading.Tasks;
using TheSwamp.Api.DAL.LWIN;
using TheSwamp.Shared;
using System.Linq;

namespace TheSwamp.Api
{
    public class WineFunction
    {
        private readonly IConfiguration _cfg;
        private readonly LWINContext _context;
        private readonly IOpenAIService _openAI;

        public WineFunction(IConfiguration cfg, LWINContext context, IOpenAIService openAI)
        {
            _cfg = cfg;
            _context = context;
            _openAI = openAI;
        }


        [FunctionName("random-review")]
        public async Task<ActionResult<string>> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wine/random-review")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("random-review");

            var lwin = await _context.GetRandomWineAsync();

            
            var completionResult = await _openAI.Completions.CreateCompletion(new CompletionCreateRequest()
            {
                Prompt = $"Write a long pretentious wine review for {lwin.DISPLAY_NAME}",
                MaxTokens = 512
            }, Models.TextCurieV1);

            var review = new Review()
            {
                Id = lwin.LWIN,
                Name = lwin.DISPLAY_NAME
            };

            if (completionResult.Successful)
            {
                review.Blurb = completionResult.Choices.FirstOrDefault().Text;
            }
            else
            {
                if (completionResult.Error == null)
                {
                    throw new Exception("Unknown Error");
                }
                Console.WriteLine($"{completionResult.Error.Code}: {completionResult.Error.Message}");
            }


            return new OkObjectResult(review);
        }
    }
}
