using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1UpdatesBot.Src.Models
{
    public class Driver
    {
        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("team_name")]
        public string TeamName { get; set; }
    }
}
