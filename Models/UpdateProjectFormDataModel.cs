using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerServiceApp1.Models
{
    [Keyless]
    public class UpdateProjectFormDataModel
    {
        public decimal c_prog { get; set; }
        public decimal Status { get; set; }
        public string project_code { get; set; }
        public DateTime? Currenttime { get; set; }

        public List<Product_Category> Prod_category { get; set; } 

        
    }
    public class MappingProjectFormDataModel
    {
        [JsonProperty("state")]
        public decimal State { get; set; }
        [JsonProperty("current_time_ptn")]
        public DateTime? Currenttime { get; set; }

        public List<Product_Category> prod_category { get; set; } = new List<Product_Category>();


    }
    [Keyless]
    public class Product_Category
    {
        public decimal Categ_Value { get; set; }
        public string C_CODE { get; set; }

        public decimal c_prog { get; set; }
    }
    
}
