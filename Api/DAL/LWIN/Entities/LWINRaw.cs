using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheSwamp.Api.DAL.LWIN.Entities
{
    public class LWINRaw
    {
        public long LWIN { get; set; }
        public string STATUS { get; set; }
        public string DISPLAY_NAME { get; set; }
        public string PRODUCER_TITLE { get; set; }
        public string PRODUCER_NAME { get; set; }
        public string WINE { get; set; }
        public string COUNTRY { get; set; }
        public string REGION { get; set; }
        public string SUB_REGION { get; set; }
        public string SITE { get; set; }
        public string PARCEL { get; set; }
        public string COLOUR { get; set; }
        public string TYPE { get; set; }
        public string SUB_TYPE { get; set; }
        public string DESIGNATION { get; set; }
        public string CLASSIFICATION { get; set; }
        public string VINTAGE_CONFIG { get; set; }
        public string FIRST_VINTAGE { get; set; }
        public string FINAL_VINTAGE { get; set; }
        public DateTime DATE_ADDED { get; set; }
        public DateTime DATE_UPDATED { get; set; }
        public long? REFERENCE { get; set; }
    }
}
