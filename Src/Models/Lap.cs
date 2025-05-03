using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1UpdatesBot.Src.Models
{
    public class Lap
    {
        [JsonProperty("driver_number")]
        public int DriverNumber { get; set; }

        [JsonProperty("date_start")]
        public DateTime? DateStart { get; set; }

        [JsonProperty("lap_number")]
        public int LapNumber { get; set; }

        [JsonProperty("lap_duration")]
        public double? LapDuration { get; set; }

        [JsonProperty("lap_time")]
        public string LapTime { get; set; }
    }
}
