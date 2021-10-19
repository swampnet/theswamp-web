using System;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace Agent
{
    public class SendMessageThing : IThing
    {
        public TimeSpan PollInterval => TimeSpan.FromMinutes(1);

        public async Task PollAsync(Monitor monitor)
        {
            if(DateTime.Now.Minute == 0)
            {
                Console.WriteLine($"{this.GetType()} Ding Dong!");

                await API.PostMessageAsync(new AgentMessage()
                {
                    Type = "led-matrix",
                    Properties = new System.Collections.Generic.List<Property>() {
                        new Property("content", $"Ding Dong! It's {DateTime.Now.ToShortTimeString()}!")
                    }
                });
            }
        }
    }
}
