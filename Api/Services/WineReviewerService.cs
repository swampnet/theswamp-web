using Microsoft.EntityFrameworkCore;
using OpenAI.GPT3.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Api.DAL.LWIN;
using TheSwamp.Api.DAL.LWIN.Entities;
using TheSwamp.Api.Interfaces;
using TheSwamp.Shared;

namespace TheSwamp.Api.Services
{
    internal class WineReviewerService : IReviewWine
    {
        private readonly LWINContext _context;
        private readonly IOpenAIService _openAI;

        public WineReviewerService(LWINContext context, IOpenAIService openAI)
        {
            _context = context;
            _openAI = openAI;
        }


        public async Task<LWINRaw> LoadWine(string id = null)
        {
            var wine = await _context.GetRandomWineAsync();

            return wine;
        }

        public Task ReviewAsync(LWINRaw wine)
        {
            return Task.CompletedTask;
        }
    }
}
