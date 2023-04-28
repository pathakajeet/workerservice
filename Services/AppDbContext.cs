using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WorkerServiceApp1.Models;

namespace WorkerServiceApp1.Services
{
    public class AppDbContext : DbContext
    {
      
        public DbSet<CPMTIntegrationEvent> CPMTIntegrationEvent { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }
        public DbSet<ProjectStatusUpdateModel> ProjectStatus { get; set; }
        public DbSet<UpdateProjectFormDataModel> ProjectData { get; set; }
        public DbSet<Product_Category> ProductCategory { get; set; }
        public DbSet<MappingUpdateProjectSchedule> ProjectSchedules { get; set; }

    }
}
