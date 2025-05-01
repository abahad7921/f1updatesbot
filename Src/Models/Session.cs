using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1UpdatesBot.Src.Models
{
    public class Session
    {
        [JsonProperty("session_key")]
        public int SessionKey { get; set; }

        [JsonProperty("session_type")]
        public string SessionType { get; set; }

        [JsonProperty("date_start")]
        public DateTime DateStart { get; set; }

        [JsonProperty("date_end")]
        public DateTime DateEnd { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }
    }
}

