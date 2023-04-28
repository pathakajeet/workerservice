using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WorkerService.Models
{
    [Table("CPMT_COST_PROJECT_UPDATE")]
    public class MappingUpdateCostPhasingModel
    {
        [Key]
        public decimal c_prog { get; set; }
        public string project_code { get; set; }
        public DateTime last_update_date { get; set; }



    }
}
