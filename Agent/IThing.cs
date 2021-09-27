using System;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent
{
    interface IThing
    {
        TimeSpan PollInterval { get; }
        Task PollAsync(Monitor monitor);
    }
}
