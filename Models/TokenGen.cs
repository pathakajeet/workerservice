using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerServiceApp1.Models
{
    public class Data
    {
        [JsonProperty("$id")]
        public string Id { get; set; }
        public string token { get; set; }
    }

    public class TokenGen
    {
        [JsonProperty("$id")]
        public string Id { get; set; }
        public bool success { get; set; }
        public int error_code { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }
    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public decimal CustomerId { get; set; } = 0;
        public string PinPass { get; set; } = "";
    }
}
