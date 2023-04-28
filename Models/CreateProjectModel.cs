using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WorkerServiceApp1.Models
{
    public class CreateProjectModel
    {
        [Required]
        [JsonProperty("initiative_type")]
        public string f_type { get; set; }



        //we get the value from session using fillreqtype method an its bydefault 1.
        [JsonProperty("request_type")]
        public int request_type { get; set; }



        [Required]
        [JsonProperty("cos_id")]
        public int P_Elmt { get; set; }




        [JsonProperty("code")]
        [StringLength(20)]
        public string Code { get; set; }



        [Required]
        [JsonProperty("title")]
        [StringLength(50)]
        public string Title { get; set; }




        [JsonProperty("currency")]
        [StringLength(3)]
        public string Currency { get; set; } = "";



        [JsonProperty("investment_year")]
        public int InvestmentYear { get; set; }



        [Required]
        [JsonProperty("start_date")]
        public DateTime StartDate { get; set; }



        [Required]
        [JsonProperty("finish_date")]
        public DateTime FinishDate { get; set; }




        [JsonProperty("activity_breakdown_structure")]
        public string ActivityBreakdownStructure { get; set; }
    }
}
