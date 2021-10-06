using System;
using System.Collections.Generic;
using System.Text;

namespace TheSwamp.Api.DAL.API.Entities
{
    public class KeyEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
