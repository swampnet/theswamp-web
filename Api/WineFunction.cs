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
using TheSwamp.Api.DAL.LWIN.Entities;
using System.Reflection.Metadata;
using System.Diagnostics;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Azure;
using Microsoft.Azure.SignalR.Protocol;
using TheSwamp.Api.Interfaces;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace TheSwamp.Api
{
    public class WineFunction
    {
        private readonly ILogger _log;
        private readonly IOpenAIService _ai;
        private readonly LWINContext _lwin;
        private readonly IReviewWine _review;

        public WineFunction(
            ILogger<WineFunction> log,
            IOpenAIService openAI,
            LWINContext lwin,
            IReviewWine review)
        {
            _log = log;
            _ai = openAI;
            _lwin = lwin;
            _review = review;
        }

        [FunctionName("wine-random")]
        public async Task<ActionResult<Wine>> GetRandom([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wine/random")] HttpRequest req)
        {
            _log.LogDebug("load random wine");
            var w = await _lwin.GetRandomWineAsync();
            var wine = new Wine()
            {
                Id = w.LWIN,
                Colour = w.COLOUR,
                Country = w.COUNTRY,
                Name = w.DISPLAY_NAME,
                ProducerName = w.PRODUCER_NAME,
                Region = w.REGION,
                SubType = w.SUB_TYPE,
                Vintage = w.FINAL_VINTAGE
            };

            return new OkObjectResult(wine);
        }


        [FunctionName("wine")]
        public async Task<ActionResult<Wine>> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wine/{id:long}")] HttpRequest req, long id)
        {
            _log.LogDebug($"load wine {id}");
            var w = await _lwin.Raw.SingleOrDefaultAsync(x => x.LWIN == id);
            var wine = new Wine()
            {
                Id = w.LWIN,
                Colour = w.COLOUR,
                Country = w.COUNTRY,
                Name = w.DISPLAY_NAME,
                ProducerName = w.PRODUCER_NAME,
                Region = w.REGION,
                SubType = w.SUB_TYPE,
                Vintage = w.FINAL_VINTAGE
            };

            return new OkObjectResult(wine);
        }




        [FunctionName("random-review")]
        public async Task<ActionResult<string>> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wine/random-review")] HttpRequest req)
        {
            _log.LogDebug("generate random wine review");

            var review = await _review.ReviewAsync();

            return new OkObjectResult(review);
        }


        [FunctionName("review")]
        public async Task<ActionResult<string>> Review(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wine/{id:long}/review")] HttpRequest req, long id)
        {
            var response = req.HttpContext.Response;

            var w = await _lwin.Raw.SingleOrDefaultAsync(x => x.LWIN == id);

            response.StatusCode = 200;

            await using var sw = new StreamWriter(response.Body);

            var completionResult = _ai.ChatCompletion.CreateCompletionAsStream(new ChatCompletionCreateRequest
            {
                Messages = new[] { 
                    new ChatMessage(StaticValues.ChatMessageRoles.System, "You are a snooty wine reviewer"),
                    new ChatMessage(StaticValues.ChatMessageRoles.User, $"Review a {w.FINAL_VINTAGE} {w.DISPLAY_NAME} wine")
                },
                Model = Models.ChatGpt3_5Turbo
            });

            await foreach (var completion in completionResult)
            {
                if (completion.Successful)
                {
                    var msg = completion.Choices.FirstOrDefault()?.Message.Content;
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

            Console.WriteLine();
            Console.WriteLine("Complete");
            return new EmptyResult();
        }
    }
}
