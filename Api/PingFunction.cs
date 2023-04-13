using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TheSwamp.Api.Interfaces;
using TheSwamp.Shared;
using OpenAI.GPT3.Interfaces;
using System.Linq;

namespace TheSwamp.Api
{
    public class PingFunction
    {
        private readonly IConfiguration _cfg;
        private readonly IOpenAIService _openAI;

        public PingFunction(IConfiguration cfg, IOpenAIService openAI)
        {
            _cfg = cfg;
            _openAI = openAI;
        }


        [FunctionName("ping")]
        public async Task<ActionResult<string>> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            await Task.CompletedTask;

            log.LogInformation("pong");

            return new OkObjectResult($"pong @ {DateTime.Now}");
        }


        [FunctionName("story")]
        public async Task<ActionResult<string>> OnceUponATime(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var response = req.HttpContext.Response;

            response.StatusCode = 200;

            await using var sw = new StreamWriter(response.Body);

            var completionResult = _openAI.Completions.CreateCompletionAsStream(new CompletionCreateRequest()
            {
                Prompt = "Once upon a time",
                MaxTokens = 1000
            }, Models.TextDavinciV3);


            await foreach (var completion in completionResult)
            {
                if (completion.Successful)
                {
                    var msg = completion.Choices.FirstOrDefault()?.Text;
                    await sw.WriteAsync(msg);
                    await sw.FlushAsync();
                    Console.Write(msg);
                }
                else
                {
                    if (completion.Error == null)
                    {
                        throw new Exception("Unknown Error");
                    }

                    Console.WriteLine($"{completion.Error.Code}: {completion.Error.Message}");
                }
            }

            Console.WriteLine("Complete");
            return new EmptyResult();
        }
    }
}
