using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerServiceApp1.Models
{
    public class Response
    {
        public bool success { get; set; }
        public string message { get; set; }
        public dynamic data { get; set; }
    }
}
