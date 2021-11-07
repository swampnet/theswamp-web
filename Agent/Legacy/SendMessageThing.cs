//using System;
//using System.Threading.Tasks;
//using TheSwamp.Shared;
//using Microsoft.Extensions.Configuration;

//namespace Agent
//{
//    public class SendMessageThing : ISampleProvider
//    {
//        public TimeSpan PollInterval => TimeSpan.FromMinutes(1);
//        private readonly Random _rnd = new Random();

//        public async Task PollAsync(Monitor monitor)
//        {
//            if(DateTime.Now.Minute == 0)
//            {
//                var x = Program.Cfg.GetSection("time-quotes").Get<string[]>();

//                var msg = x[_rnd.Next(0, x.Length - 1)];
//                msg = msg.Replace("{{now}}", DateTime.Now.ToShortTimeString());

//                Console.WriteLine($"{this.GetType()} {msg}");

//                await API.PostMessageAsync(new AgentMessage()
//                {
//                    Type = "led-matrix",
//                    Properties = new System.Collections.Generic.List<Property>() {
//                        new Property("content", msg)
//                    }
//                });
//            }
//        }
//    }
//}
