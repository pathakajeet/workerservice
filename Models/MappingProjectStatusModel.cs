using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerServiceApp1.Models
{
    public class MappingProjectStatusModel
    {
        [JsonProperty("state")]
        public decimal State { get; set; }

        [JsonProperty("current_time")]
        public DateTime Currenttime { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("current_time_ptn")]
        public DateTime Currenttimeptn { get; set; }

        [JsonProperty("check_status")]
        public string CheckStatus { get; set; }

        [JsonProperty("check_value")]
        public string CheckValue { get; set; }
    }
}
