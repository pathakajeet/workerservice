using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerService.Models
{
    [Keyless]
    public class CPMTBudgetVersionModel
    {
        public long step_id { get; set; }
        public decimal workflow_id { get; set; }
        public string project_code { get; set; }
        public string f_type { get; set; }
        public int form_id { get; set; }
        public bool IS_BUDGETPROCESS { get; set; }
        public string BUDGET_PHASE { get; set; }
    }
    [Keyless]
    public class CPMTBudgetVersionModelCall
    {
        public long step_id { get; set; }
        public decimal workflow_id { get; set; }
        public string project_code { get; set; }
        public string f_type { get; set; }
        public int form_id { get; set; }
        public string version { get; set; }
        //public bool IS_BUDGETPROCESS { get; set; }
        //public string BUDGET_PHASE { get; set; }
    }
}
