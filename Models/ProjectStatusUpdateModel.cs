using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerServiceApp1.Models
{
    [Keyless]
    public class ProjectStatusUpdateModel
    {
        public string project_code { get; set; }
        public decimal project_status { get; set; }

    }
}
