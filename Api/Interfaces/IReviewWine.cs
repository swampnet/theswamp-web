using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Api.DAL.LWIN.Entities;
using TheSwamp.Shared;

namespace TheSwamp.Api.Interfaces
{
    public interface IReviewWine
    {
        Task<Review> ReviewAsync(string id = null);
    }
}
