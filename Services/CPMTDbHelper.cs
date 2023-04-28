using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using WorkerService.Models;
using WorkerServiceApp1.Models;
using WorkerServiceApp1.Utility;

namespace WorkerServiceApp1.Services
{
    public class CPMTDbHelper
    {
        private CPMTAppDbContext dbContext;


        private DbContextOptions<CPMTAppDbContext> GetAllOptions()
        {
            var optionBuilder = new DbContextOptionsBuilder<CPMTAppDbContext>();
            optionBuilder.UseSqlServer(AppSettings.CPMConnectionString);
            return optionBuilder.Options;
        }

        public DataTable GetProjectDetails(string currentDate)
        {
            var context = new CPMTAppDbContext(GetAllOptions());
            string Sql = "select C_PROG, OBJ_CODE  from BPM_OBJECTS where f_id=10 and obj_Date>=" + DbUtility.QS(currentDate) + "";
            var dt = ExecuteQueryForDT(context, Sql);
            return dt;
        }
        public CPMTBudgetVersionModel GetProjectBudgetInfo(string project_code)
        {
            var context = new CPMTAppDbContext(GetAllOptions());
            string sql = "select ISNULL(IS_BUDGETPROCESS, 0) IS_BUDGETPROCESS,isnull(BUDGET_PHASE, '') BUDGET_PHASE, WF_STEP as step_id,bpm.WF_ID as workflow_id,OBJ_CODE as project_code, F_TYPE as f_type, F_ID as form_id from BPM_OBJECTS bpm left join ASCN_WF_ADVANCE_OPTIONS aa on aa.WF_ID = bpm.WF_ID and aa.STEP_ID = bpm.WF_STEP where OBJ_CODE =" + DbUtility.QS(project_code) + "";
            var data = context.BudgetVersion.FromSqlRaw(sql).SingleOrDefault();
            return data;
        }
        public DataTable GetChangeRequestDetails(string currentDate)
        {
            var context = new CPMTAppDbContext(GetAllOptions());
            string Sql = "select OBJ_CODE from BPM_OBJECTS where f_id=5 and obj_Date>=" + DbUtility.QS(currentDate) + "";
            var dt = ExecuteQueryForDT(context, Sql);
            return dt;
        }
        public List<Product_Category> GetProductCategory(decimal c_prog)
        {
            var context = new CPMTAppDbContext(GetAllOptions());
            string sql = "select t3.C_CODE ,t3.CATEG_VALUE , t3.c_prog from ascn_project_product_category t3 where c_prog = " + c_prog + "";
            var data = context.ProductCategory.FromSqlRaw(sql).ToList();

            return data;
        }


        public int GetpElementCPMT(string s_element)
        {
            var context = new CPMTAppDbContext(GetAllOptions());
            string query1 = "select P_ELMNT from STRU_SIST_T094 where S_ELMNT=" + DbUtility.QS(s_element) + "";
            var data = ExecuteQueryWithStringType(context, query1);
            if (data.Count() != 0)
            {
                int res = Convert.ToInt32(data[0]["P_ELMNT"]);
                return res;
            }
            else
            {
                return 0;
            }
        }
        public List<UpdateProjectFormDataModel> GetProjectFormData(string currentDate)
        {
            var context = new CPMTAppDbContext(GetAllOptions());
            string sql = "select t1.c_prog,t1.obj_date,t2.F_STA as Status, S_PROG_AZD_NOM as  project_code,t2.D_COR as Currenttime from BPM_OBJECTS t1 inner join  PROG_T056 t2 on t2.C_PROG=t1.C_PROG where t1.OBJ_DATE>=" + DbUtility.QS(currentDate);/* + " and t2.LAST_UPDATED_DATE>= " + DbUtility.QS(currentDate) + "";*/
            var data = context.ProjectData.FromSqlRaw(sql).ToList();
            return data;
        }


        public void DeleteProjectCost()
        {
            var context = new CPMTAppDbContext(GetAllOptions());
            string sql = "select c_prog , project_code, last_update_date from CPMT_COST_PROJECT_UPDATE";
            var deleteProject = context.ProjectCostUpdate.FromSqlRaw(sql).ToList();
            context.ProjectCostUpdate.RemoveRange(deleteProject);
            context.SaveChanges();
        }

        public List<MappingUpdateCostPhasingModel> GetProjectCostUpdateDetail()
        {
            var context = new CPMTAppDbContext(GetAllOptions());
            string sql = "select c_prog , project_code, last_update_date from CPMT_COST_PROJECT_UPDATE";
            var data = context.ProjectCostUpdate.FromSqlRaw(sql).ToList();
            return data;
        }
        public List<CPMTUpdateCostPhasingModel> GetCostElementDetails()
        {
            var context = new CPMTAppDbContext(GetAllOptions());
            var cpmtCostUpdateList = GetProjectCostUpdateDetail();
            var cost = new List<CPMTUpdateCostPhasingModel>();
            if (cpmtCostUpdateList != null)
            {
                foreach (var cpm in cpmtCostUpdateList)
                {
                    var costUpdateModel = new CPMTUpdateCostPhasingModel();
                    costUpdateModel.project_code = cpm.project_code;
                    costUpdateModel.actual = context.ActualValues.FromSqlRaw("Select * from GetAdvanceCostPhasedDataForBasicCost(" + cpm.c_prog + ", 16)").ToList();
                    costUpdateModel.forecast = context.ForecastValue.FromSqlRaw("Select * from GetAdvanceCostPhasedDataForBasicCost(" + cpm.c_prog + ", 5)").ToList();
                    costUpdateModel.budget = context.BudgetValue.FromSqlRaw("Select * from GetAdvanceCostPhasedDataForBasicCost(" + cpm.c_prog + ", 3)").ToList();
                    costUpdateModel.expenditure = context.ExpenditureValues.FromSqlRaw("Select * from GetAdvanceCostPhasedDataForBasicCost(" + cpm.c_prog + ", 11)").ToList();
                    cost.Add(costUpdateModel);
                }
            }
            return cost;
        }

        public List<Dictionary<string, string>> GetCrsToDelete(string cpmCrsLists, string spmCrsLists)
        {
            var context = new CPMTAppDbContext(GetAllOptions());
            const string stmt = "[sp_DeleteChangeRequestType]";
            SqlParameter[] @params =
            {
               new SqlParameter("@spmCrsList", SqlDbType.VarChar) { Value = spmCrsLists  },
                new SqlParameter("@cpmtCrsList", SqlDbType.VarChar) { Value = cpmCrsLists }
            };
            var dataList = ExecuteStoredProcedureWithStringType(context, stmt, @params);
            return dataList;
        }

        public List<Dictionary<string, string>> GetCrsToCreate(string cpmCrsLists, string spmCrsLists)
        {
            var context = new CPMTAppDbContext(GetAllOptions());

            const string stmt = "[sp_CreateChangeRequestTypeCPMTList]";
            SqlParameter[] @params =
            {
               new SqlParameter("@spmCrsList", SqlDbType.VarChar) { Value = spmCrsLists  },
                new SqlParameter("@cpmtCrsList", SqlDbType.VarChar) { Value = cpmCrsLists }

            };
            var dataList = ExecuteStoredProcedureWithStringType(context, stmt, @params);
            return dataList;
        }

        public ProjectDetails ProjectDetails(string obj_Code)
        {
            var context = new CPMTAppDbContext(GetAllOptions());
            string Sql = "select t1.C_PROG, t1.OBJ_TITLE ,t1.OBJ_DATE, T2.D_INI_FRC, T2.D_FIN_FRC,t3.P_ELMNT from BPM_OBJECTS t1  inner join  PROG_T056  T2 on T2.C_PROG = t1.C_PROG and t1.F_ID = 10  inner join ASCN_OBS_PROG t3 on t3.C_PROG=t1.C_PROG where t1.OBJ_CODE = '" + obj_Code + "'AND F_ID = 10";
            List<Dictionary<string, string>> dt = ExecuteQueryWithStringType(context, Sql);
            if (dt.Any())
            {
                var data = new ProjectDetails() { ProjectCode = obj_Code, ProjectTitle = dt[0]["OBJ_TITLE"].ToString(), ProjId = Convert.ToDecimal(dt[0]["C_PROG"]), FinishDate = Convert.ToDateTime(dt[0]["D_FIN_FRC"]), StartDate = Convert.ToDateTime(dt[0]["D_INI_FRC"]), P_Elmt = Convert.ToInt32(dt[0]["P_ELMNT"]), obj_Date = Convert.ToDateTime(dt[0]["OBJ_DATE"]) };
                return data;
            }
            else
            {
                return new ProjectDetails();
            }
}

        public List<ProjectStatusUpdateModel> GetProjectStateDetail()
        {
            var context = new CPMTAppDbContext(GetAllOptions());
            string sql = "select F_STA as project_status, S_PROG_AZD_NOM as  project_code from PROG_T056";
            var data = context.ProjectStatus.FromSqlRaw(sql).ToList();

            return data;
        }


        public virtual DataTable ExecuteQueryForDT(CPMTAppDbContext poDbContext, string stmt)
        {
            DataTable dt = new DataTable();
            using (var cmd = poDbContext.Database.GetDbConnection().CreateCommand())
            {

                cmd.Connection.Open();
                cmd.CommandText = stmt;
                cmd.CommandTimeout = 2000;
                using (var reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                    
                }


                cmd.Connection.Close();
            }
            return dt;
        }

        public List<Dictionary<string, string>> GetProjectsToCreate(string dtCPMT, string dtSPM)
        {
            var context = new CPMTAppDbContext(GetAllOptions());

            const string stmt = "[sp_CreateProjectCPMTList]";
            SqlParameter[] @params =
            {
               new SqlParameter("@spmProjList", SqlDbType.VarChar) { Value = dtSPM  },
                new SqlParameter("@cpmtProjList", SqlDbType.VarChar) { Value = dtCPMT }

            };
            var dataList = ExecuteStoredProcedureWithStringType(context, stmt, @params);
            return dataList;
        }

        public List<Dictionary<string, string>> GetProjectsToDelete(string dtCPMT, string dtSPM)
        {
            var context = new CPMTAppDbContext(GetAllOptions());
            const string stmt = "[sp_DeleteProjectCPMTList]";
            SqlParameter[] @params =
            {
               new SqlParameter("@spmProjList", SqlDbType.VarChar) { Value = dtSPM  },
                new SqlParameter("@cpmtProjList", SqlDbType.VarChar) { Value = dtCPMT }
           };
            var dataList = ExecuteStoredProcedureWithStringType(context, stmt, @params);
            return dataList;
        }
        public virtual List<Dictionary<string, string>> ExecuteStoredProcedureWithStringType(CPMTAppDbContext poDbContext, string stmt, SqlParameter[] @params)
        {
            List<Dictionary<string, string>> dt = new List<Dictionary<string, string>>();
            using (var cmd = poDbContext.Database.GetDbConnection().CreateCommand())
            {

                cmd.Connection.Open();
                cmd.CommandText = stmt;
                cmd.Parameters.AddRange(@params);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var reader = cmd.ExecuteReader())
                {
                    dt = DbUtility.Read(reader, "").ToList();

                }


                cmd.Connection.Close();
            }
            return dt;
        }

        public ProjectDetails ProjectDetails(decimal projId)
        {


            var context = new CPMTAppDbContext(GetAllOptions());
            string Sql = "select OBJ_CODE, OBJ_TITLE from BPM_OBJECTS where C_PROG = " + projId + " AND F_ID = 10";
            List<Dictionary<string, string>> dt = ExecuteQueryWithStringType(context, Sql);

            if (dt.Any())
            {

                var data = new ProjectDetails() { ProjectCode = dt[0]["OBJ_CODE"].ToString(), ProjectTitle = dt[0]["OBJ_TITLE"].ToString(), ProjId = projId };

                return data;
            }
            else
            {
                return new ProjectDetails();
            }



        }

        public virtual List<Dictionary<string, string>> ExecuteQueryWithStringType(CPMTAppDbContext poDbContext, string stmt)
        {
            List<Dictionary<string, string>> dt = new List<Dictionary<string, string>>();
            using (var cmd = poDbContext.Database.GetDbConnection().CreateCommand())
            {
                if (cmd.Connection.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                cmd.CommandText = stmt;

                using (var reader = cmd.ExecuteReader())
                {
                    dt = DbUtility.Read(reader, "").ToList();

                }

                if (cmd.Connection.State == ConnectionState.Open)
                    cmd.Connection.Close();
            }
            return dt;
        }

        //GetAllEvents
        public List<CPMTIntegrationEvent> GetAllEvents()
        {
            using (dbContext = new CPMTAppDbContext(GetAllOptions()))
            {
                try
                {
                    var events = dbContext.CPMTIntegrationEvent.ToList();
                    if (events != null)
                        return events;
                    else
                        return new List<CPMTIntegrationEvent>();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


    }
}
