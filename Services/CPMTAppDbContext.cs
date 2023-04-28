using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WorkerService.Models;
using WorkerServiceApp1.Models;

namespace WorkerServiceApp1.Services
{
    public class CPMTAppDbContext : DbContext
    {
         public DbSet<CPMTIntegrationEvent> CPMTIntegrationEvent { get; set; }
        public CPMTAppDbContext(DbContextOptions<CPMTAppDbContext> options):base(options)
        {

        }

        public DbSet<CPMTBudgetVersionModel> BudgetVersion { get; set; }
        public DbSet<ProjectStatusUpdateModel> ProjectStatus { get; set; }
        public DbSet<UpdateProjectFormDataModel> ProjectData { get; set; }
        public DbSet<MappingUpdateCostPhasingModel> ProjectCostUpdate { get; set; }
        public DbSet<Product_Category> ProductCategory { get; set; }
        public DbSet<ExpenditureValue> ExpenditureValues { get; set; }
        public DbSet<ActualValue> ActualValues { get; set; }
        public DbSet<ForecastValue> ForecastValue { get; set; }
        public DbSet<BudgetValue> BudgetValue { get; set; }
    }
}
