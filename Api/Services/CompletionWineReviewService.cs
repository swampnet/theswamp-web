using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.ObjectModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Api.DAL.LWIN;
using TheSwamp.Api.DAL.LWIN.Entities;
using TheSwamp.Api.Interfaces;
using TheSwamp.Shared;

namespace TheSwamp.Api.Services
{
    internal class CompletionWineReviewService : IReviewWine
    {
        private readonly LWINContext _context;
        private readonly IOpenAIService _openAI;
        private readonly string _prompt = "Write long and {{TONE}} tasting notes for a {{VINTAGE}} fine wine called {{DISPLAY_NAME}}. It's a {{SUB_TYPE}} {{COLOUR}} wine from {{REGION}} in {{COUNTRY}}. It's produced by {{PRODUCER_NAME}}.";
        private readonly ILogger _log;
        private readonly IConfiguration _cfg;
        private readonly Random _rng = new Random();

        public CompletionWineReviewService(
            ILogger<WineFunction> log,
            IConfiguration cfg,
            LWINContext context, 
            IOpenAIService openAI)
        {
            _log = log;
            _cfg = cfg;
            _context = context;
            _openAI = openAI;
        }


        public async Task<Review> ReviewAsync(string id = null)
        {
            var sw = Stopwatch.StartNew();
            Review review = null;

            try
            {
                var wine = await _context.GetRandomWineAsync();
                review = new Review()
                {
                    Id = wine.LWIN,
                    Name = wine.DISPLAY_NAME,
                    Vintage = wine.FINAL_VINTAGE,
                    SubType = wine.SUB_TYPE,
                    Colour = wine.COLOUR,
                    ProducerName = wine.PRODUCER_NAME,
                    Country = wine.COUNTRY,
                    Region = wine.REGION,
                    Tone = RollTone()
                };

                review.Benchmarks.Add(new Benchmark("Get random wine from LWIN data", sw.Elapsed));

                await ReviewAsync(review);
                await ModerateAsync(review);
            }
            catch (Exception ex)
            {
                review.Error = ex.Message;
                _log.LogError(ex, ex.Message);
            }
            finally
            {
                review.Benchmarks.Add(new Benchmark("Total elapsed", sw.Elapsed));
                _log.LogDebug("generate random wine review - complete in {elapsed:0.00}s", sw.Elapsed.TotalSeconds);
            }

            return review;
        }

        /// <summary>
        /// pick a random tone.
        /// </summary>
        private string RollTone()
        {
            var v = _rng.Next(0, 100);
            var tone = v switch
            {
                _ when v < 20 => "angry",
                _ when v < 30 => "sarcastic",
                _ when v < 40 => "rhyming",
                //_ when v < 80 => "funny",
                _ => "pretentious"
            };

            return tone;
        }

        private async Task ReviewAsync(Review review)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                review.Model = _cfg["openai.model"];

                var p = _prompt
                    .Replace("{{TONE}}", review.Tone)
                    .Replace("{{VINTAGE}}", review.Vintage.EqualsNoCase("na") ? "" : $"{review.Vintage} vintage")
                    .Replace("{{DISPLAY_NAME}}", review.Name)
                    .Replace("{{SUB_TYPE}}", review.SubType)
                    .Replace("{{COLOUR}}", review.Colour)
                    .Replace("{{COUNTRY}}", review.Country)
                    .Replace("{{REGION}}", review.Region)
                    .Replace("{{PRODUCER_NAME}}", review.ProducerName);

                var completionResult = await _openAI.Completions.CreateCompletion(new CompletionCreateRequest()
                {
                    Prompt = p,
                    MaxTokens = 512
                }, review.Model);

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
                    else
                    {
                        throw new Exception($"{completionResult.Error.Code}: {completionResult.Error.Message}");
                    }
                }
            }
            finally
            {
                review.Benchmarks.Add(new Benchmark($"Generate review ({review.Model})", sw.Elapsed));
            }
        }

        /// <summary>
        /// Moderate the review.
        /// </summary>
        private async Task ModerateAsync(Review review)
        {
            var sw = Stopwatch.StartNew();

            var mod = await _openAI.Moderation.CreateModeration(new CreateModerationRequest()
            {
                Input = review.Blurb
            });

            if (!mod.Successful || mod.Results.Any(r => r.Flagged))
            {
                review.Error = "Moderation failed";
                review.Blurb = "[censored]";
            }

            review.Benchmarks.Add(new Benchmark("Moderation", sw.Elapsed));
        }
    }
}
