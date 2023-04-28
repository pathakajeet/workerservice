using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerService.Models
{
    public class CPMTUpdateCostPhasingModel
    {
        public string project_code { get; set; }
        public List<ActualValue> actual { get; set; }
        public List<ForecastValue> forecast { get; set; }
        public List<BudgetValue> budget { get; set; }
        public List<ExpenditureValue> expenditure { get; set; }
    }
    [Keyless]
    public class ActualValue
    {
        public string s_prog_azd_nom { get; set; }
        public string element_code { get; set; }
        public DateTime start_date { get; set; }
        public DateTime finish_date { get; set; }
        public double value { get; set; }
    }
    [Keyless]
    public class ForecastValue
    {
        public string s_prog_azd_nom { get; set; }
        public string element_code { get; set; }
        public DateTime start_date { get; set; }
        public DateTime finish_date { get; set; }
        public double value { get; set; }
    }
    [Keyless]
    public class BudgetValue
    {
        public string s_prog_azd_nom { get; set; }
        public string element_code { get; set; }
        public DateTime start_date { get; set; }
        public DateTime finish_date { get; set; }
        public double value { get; set; }
    }
    [Keyless]
    public class ExpenditureValue
    {
        public string element_code { get; set; }
        public DateTime start_date { get; set; }
        public DateTime finish_date { get; set; }
        public double value { get; set; }
    }
}
