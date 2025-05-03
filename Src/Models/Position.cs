using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1UpdatesBot.Src.Models
{
    public class Position
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("driver_number")]
        public int DriverNumber { get; set; }

        [JsonProperty("meeting_key")]
        public string MeetingKey { get; set; }

        [JsonProperty("position")]
        public int CurrentPosition { get; set; }

        [JsonProperty("session_key")]
        public int SessionKey { get; set; }

        [JsonProperty("lap_number")]
        public int LapNumber { get; set; }
    }
}
