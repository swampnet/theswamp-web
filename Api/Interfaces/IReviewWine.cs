using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Api.DAL.LWIN.Entities;

namespace TheSwamp.Api.Interfaces
{
    public interface IReviewWine
    {
        Task<LWINRaw> LoadWine(string id = null);
        Task ReviewAsync(LWINRaw wine);
    }
}
