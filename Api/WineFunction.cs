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

namespace TheSwamp.Api
{
    public class WineFunction
    {
        private readonly ILogger _log;
        private readonly IReviewWine _review;

        public WineFunction(
            ILogger<WineFunction> log,
            IReviewWine review)
        {
            _log = log;
            _review = review;
        }


        [FunctionName("random-review")]
        public async Task<ActionResult<string>> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wine/random-review")] HttpRequest req)
        {
            _log.LogDebug("generate random wine review");

            var review = await _review.ReviewAsync();

            return new OkObjectResult(review);
        }
    }
}
