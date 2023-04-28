using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerService.Models
{
   public class UpdateProjectScheduleModel
    {    
       public List<UpdateProjectSchedule> ProjectSchedules { get; set; }
    }

    public class UpdateProjectSchedule
    {   public string project_code { get; set; }
        public DateTime start_date { get; set; }
    }
}
