using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WorkerServiceApp1.Models
{
    [Table("CHANGE_IN_PROJ_DATES")]
    public class MappingUpdateProjectSchedule
    {
        [Key]
        public decimal c_prog { get; set; }
        public string project_code { get; set; }
        public DateTime new_start_Date { get; set; }
    }
}
