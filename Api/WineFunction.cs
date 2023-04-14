using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using System;
using System.Threading.Tasks;
using TheSwamp.Api.DAL.LWIN;
using TheSwamp.Shared;
using System.Linq;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using TheSwamp.Api.DAL.LWIN.Entities;
using Microsoft.Identity.Client;

namespace TheSwamp.Api
{
    public class WineFunction
    {
        private readonly ILogger _log;
        private readonly IOpenAIService _ai;
        private readonly LWINContext _lwin;

        public WineFunction(
            ILogger<WineFunction> log,
            IOpenAIService openAI,
            LWINContext lwin)
        {
            _log = log;
            _ai = openAI;
            _lwin = lwin;
        }

        [FunctionName("wine-random")]
        public async Task<ActionResult<Wine>> GetRandom(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wine/random")] HttpRequest req)
        {
            _log.LogInformation("load random wine");
            var lwin = await _lwin.GetRandomWineAsync();
            var wine = lwin.Convert();

            return new OkObjectResult(wine);
        }


        [FunctionName("wine")]
        public async Task<ActionResult<Wine>> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wine/{id:long}")] HttpRequest req, 
            long id)
        {
            _log.LogInformation($"load wine {id}");
            var lwin = await _lwin.Raw.SingleOrDefaultAsync(x => x.LWIN == id);
            var wine = lwin.Convert();

            return new OkObjectResult(wine);
        }


        [FunctionName("review")]
        public async Task<ActionResult<string>> Review(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wine/{id:long}/review")] HttpRequest req, long id)
        {
            _log.LogInformation($"reviewing wine {id}");

            var response = req.HttpContext.Response;

            var w = await _lwin.Raw.SingleOrDefaultAsync(x => x.LWIN == id);

            response.StatusCode = 200;

            var personality = ReviewPersonality.Pretentious;
            if(Enum.TryParse(req.Query["p"], out ReviewPersonality p))
            {
                personality = p;
            }

            await using var sw = new StreamWriter(response.Body);

            var completionResult = _ai.ChatCompletion.CreateCompletionAsStream(new ChatCompletionCreateRequest
            {
                Messages = BuildPrompt(personality, w).ToList(),
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


        private IEnumerable<ChatMessage> BuildPrompt(ReviewPersonality personality, LWINRaw wine)
        {
            var seed = new List<ChatMessage>();

            seed.Add(new ChatMessage(StaticValues.ChatMessageRoles.System, "I want you to act as an expert reviewer of fine wine. I will give you some information on a wine or spirit and you will reply with a review. You should only reply with your review, and nothing else. Do not write explanations. The review must be at least 2 paragraphs long."));
            
            seed.Add(new ChatMessage(StaticValues.ChatMessageRoles.User, $"Review {wine.DISPLAY_NAME}."));
            seed.Add(new ChatMessage(StaticValues.ChatMessageRoles.User, $"It's a {wine.COLOUR.Value()} {wine.SUB_TYPE.Value()} {wine.TYPE.Value()}."));

            if (!string.IsNullOrEmpty(wine.FINAL_VINTAGE) && !wine.FINAL_VINTAGE.EqualsNoCase("na"))
            {
                seed.Add(new ChatMessage(StaticValues.ChatMessageRoles.User, $"It is a {wine.FINAL_VINTAGE} vintage."));
            }

            if (!wine.PRODUCER_NAME.EqualsNoCase("na"))
            {
                var prompt = $"It is produced by {wine.PRODUCER_NAME}";
                if(!wine.REGION.EqualsNoCase("na") && !wine.SUB_REGION.EqualsNoCase("na"))
                {
                    prompt += $" in {wine.REGION}, {wine.SUB_REGION} in {wine.COUNTRY}";
                }
                seed.Add(new ChatMessage(StaticValues.ChatMessageRoles.User, prompt));
            }

            // GPT3.5 is very reluctant to output anything negative, so any modifiers that ask it to be angry or negative about anything don't really work.
            switch (personality)
            {
                case ReviewPersonality.Rhyme:
                    seed.Add(new ChatMessage(StaticValues.ChatMessageRoles.System, "I also want your reviews to rhyme."));
                    break;

                case ReviewPersonality.BadFrench:
                    seed.Add(new ChatMessage(StaticValues.ChatMessageRoles.System, "I also want your reviews to be in a bad french accent."));
                    break;

                case ReviewPersonality.Gangster:
                    seed.Add(new ChatMessage(StaticValues.ChatMessageRoles.System, "I also want your reviews to be in the style of a 1920 gangstar."));
                    break;

                case ReviewPersonality.Pretentious:
                    seed.Add(new ChatMessage(StaticValues.ChatMessageRoles.System, "I also want you to act as if you have a very pretentious demeanor."));
                    break;

                case ReviewPersonality.Rap:
                    seed.Add(new ChatMessage(StaticValues.ChatMessageRoles.System, "I also want you to act as a rapper. You will come up with powerful and meaningful lyrics related to the wine or spirit you are reviewing, beats and rhythm that can ‘wow’ the audience. Your lyrics should have an intriguing meaning and message which people can relate too. When it comes to choosing your beat, make sure it is catchy yet relevant to your words, so that when combined they make an explosion of sound everytime!"));
                    break;

                case ReviewPersonality.Drunk:
                    seed.Add(new ChatMessage(StaticValues.ChatMessageRoles.System, "I also want you to act as a drunk person. You will only answer like a very drunk person texting and nothing else. Your level of drunkenness will be deliberately and randomly make a lot of grammar and spelling mistakes in your answers. You will also randomly ignore what I said and say something random with the same level of drunkeness I mentioned."));
                    break;

                case ReviewPersonality.Emoji:
                    seed.Add(new ChatMessage(StaticValues.ChatMessageRoles.System, "I also want you to just use emojis in all responses."));
                    break;

                case ReviewPersonality.Apathetic:
                    seed.Add(new ChatMessage(StaticValues.ChatMessageRoles.System, "I also want you to be completely apathetic and uninterested in your response, you don't reallyh care for wine and you're reluctantly being forced to drink and review this muck."));
                    break;
            }

            return seed;
        }
    }
}
