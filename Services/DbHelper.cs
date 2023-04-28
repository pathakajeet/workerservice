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
    public class DbHelper
    {
        private AppDbContext dbContext;

        private DbContextOptions<AppDbContext> GetAllOptions()
        {
            var optionBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionBuilder.UseSqlServer(AppSettings.ConnectionString);
            return optionBuilder.Options;
        }

    
        public DataTable GetProjectDetails(string currentDate)
        {
            var context = new AppDbContext(GetAllOptions());
            string Sql = "select OBJ_CODE from BPM_OBJECTS where f_id=10 and obj_Date>=" + DbUtility.QS(currentDate) + "";
            var dt = ExecuteQueryForDT(context, Sql);
            return dt;
        }

        //added by ajeet
        public DataTable GetupdatedProjectlist(string currentDate)
        {
            var context = new AppDbContext(GetAllOptions());
            string Sql = "SELECT * FROM PROG_T056 PRJ INNER JOIN BPM_OBJECTS BO ON BO.OBJ_CODE=PRJ.S_PROG_AZD_NOM WHERE BO.F_ID=10 AND BO.OBJ_DATE>=" + DbUtility.QS(currentDate) + "";
            var dt = ExecuteQueryForDT(context, Sql);
            return dt;
        }
        //end
        public DataTable GetChangeRequestDetails(string currentDate)
        {
            var context = new AppDbContext(GetAllOptions());
            string Sql = "select OBJ_CODE from BPM_OBJECTS where f_id=5 and obj_Date>=" + DbUtility.QS(currentDate) + "";
            var dt = ExecuteQueryForDT(context, Sql);
            return dt;
        }

        public List<ProjectStatusUpdateModel> GetProjectStateDetail()
        {
            var context = new AppDbContext(GetAllOptions());
            string sql = "select F_STA as project_status, S_PROG_AZD_NOM as  project_code from PROG_T056";
            var data = context.ProjectStatus.FromSqlRaw(sql).ToList();
            return data;
        }


        public string GetpElementSPM(int p_element)
        {
            var context = new AppDbContext(GetAllOptions());
            string sqlQuery = "select S_ELMNT from STRU_SIST_T094 where P_ELMNT=" + p_element + "";
            var data = ExecuteQueryWithStringType(context, sqlQuery);
            if (data.Count() != 0)
            {
                var res = data[0]["S_ELMNT"].ToString();
                return res.ToString();
            }
            else
            {
                return null;
            }
        }

        public ChangeRequestTypeModel ChangeRequestTypeDetails(string obj_code)
        {
            var context = new AppDbContext(GetAllOptions());
            string sql = "select prj.S_PROG_AZD_NOM as projectCode,bpm.* from bpm_objects bpm inner join PROG_T056 prj on prj.C_PROG = bpm.C_PROG where OBJ_CODE = '" + obj_code + "'  AND F_ID = 5";
            List<Dictionary<string, string>> dt = ExecuteQueryWithStringType(context, sql);
            if (dt.Any())
            {
                var data = new ChangeRequestTypeModel()
                {
                    OBJ_CODE = obj_code,
                    Title = dt[0]["OBJ_TITLE"].ToString(),
                    projectCode = dt[0]["projectCode"].ToString(),
                    WF_ID = Convert.ToInt32(dt[0]["WF_ID"]),
                    BDG_CHANGE_TYPE = dt[0]["BDG_CHANGE_TYPE"].ToString(),
                    OBJ_CREATOR = Convert.ToDecimal(dt[0]["OBJ_CREATOR"]),
                    WF_STEP = Convert.ToInt64(dt[0]["WF_STEP"]),
                    FORM_VERSION = Convert.ToInt32(dt[0]["FORM_VERSION"]),
                    IS_DELETE = Convert.ToBoolean(dt[0]["IS_DELETE"]),
                    IS_PR = Convert.ToBoolean(dt[0]["IS_PR"])
                };
                if (dt[0]["OBJ_DUE_DATE"].ToString() != "")
                {
                    data.OBJ_DUE_DATE = Convert.ToDateTime(dt[0]["OBJ_DUE_DATE"].ToString());
                }
                else
                {
                    data.OBJ_DUE_DATE = null;
                }
                if (dt[0]["OBJ_DATE"].ToString() != "")
                {
                    data.OBJ_DATE = Convert.ToDateTime(dt[0]["OBJ_DATE"].ToString());
                }
                else
                {

                    data.OBJ_DATE = null;
                }
                return data;
            }
            else
            {
                return new ChangeRequestTypeModel();
            }
        }

        public List<Product_Category> GetProductCategory(decimal c_prog)
        {
            var context = new AppDbContext(GetAllOptions());
            string sql = "select t3.C_CODE ,t3.CATEG_VALUE , t3.c_prog from ascn_project_product_category t3 where c_prog = " + c_prog+"";
            var data = context.ProductCategory.FromSqlRaw(sql).ToList();

            return data;
        }
        public List<UpdateProjectFormDataModel> GetProjectFormData(string currentDate)
        {
            var context = new AppDbContext(GetAllOptions());
            string sql = "";
            sql = sql + "SELECT y.c_prog, " + "\n";
            sql = sql + "       y.obj_date, " + "\n";
            sql = sql + "       y.f_sta       AS Status, " + "\n";
            sql = sql + "       s_prog_azd_nom AS project_code, " + "\n";
            sql = sql + "       c.Currenttime  AS Currenttime " + "\n";
            sql = sql + "FROM  (select t1.C_PROG,t1.OBJ_DATE,t2.F_STA,t2.s_prog_azd_nom from bpm_objects t1 " + "\n";
            sql = sql + "       INNER JOIN prog_t056 t2 " + "\n";
            sql = sql + "               ON t2.c_prog = t1.c_prog " + "\n";
            sql = sql + "                  AND t1.f_id = 10) y " + "\n";
            sql = sql + "       left JOIN (SELECT max( [reporting period]) AS Currenttime, " + "\n";
            sql = sql + "                               x.c_prog " + "\n";
            sql = sql + "                  FROM   (SELECT DISTINCT tab.c_prog, " + "\n";
            sql = sql + "                                          dbo.Last_day(Datefromparts(ref_year, " + "\n";
            sql = sql + "                                                       ref_month, 1)) " + "\n";
            sql = sql + "                                                  [Reporting Period] " + "\n";
            sql = sql + "                          FROM   tab_prj_detail_report tab " + "\n";
            sql = sql + "                                 INNER JOIN(SELECT main.msu_id, " + "\n";
            sql = sql + "                                                   ref_year, " + "\n";
            sql = sql + "                                                   ref_month, " + "\n";
            sql = sql + "                                                   submain.c_prog, " + "\n";
            sql = sql + "                                                   mr_obj_id " + "\n";
            sql = sql + "                                            FROM   pf_tab_monthlystatusupdate " + "\n";
            sql = sql + "                                                   main " + "\n";
            sql = sql + "                                 LEFT JOIN pf_tab_monthlystatusobjlink submain " + "\n";
            sql = sql + "                                        ON main.msu_id = submain.msu_id " + "\n";
            sql = sql + "                                           AND main.portfolio_code = " + "\n";
            sql = sql + "                                               submain.portfolio_code) " + "\n";
            sql = sql + "                                                           tab_pf " + "\n";
            sql = sql + "                                         ON tab.c_prog = tab_pf.c_prog " + "\n";
            sql = sql + "                                            AND tab.obj_id = tab_pf.mr_obj_id " + "\n";
            sql = sql + "                                 INNER JOIN bpm_objects O " + "\n";
            sql = sql + "                                         ON TAB.c_prog = O.c_prog " + "\n";
            sql = sql + "                                            AND tab.obj_id = o.obj_id) x group by x.C_PROG) c " + "\n";
            sql = sql + "              ON c.c_prog = y.c_prog where y.OBJ_DATE >=" + DbUtility.QS(currentDate);
            var data = context.ProjectData.FromSqlRaw(sql).ToList();
            return data;
        }

        public string GetCurrentimeNow(decimal c_prog)
        {
            var context = new AppDbContext(GetAllOptions());
            string sql1 = "select top 1 [Reporting Period] as Currenttime from (select tab.C_PROG,dbo.last_day(DATEFROMPARTS(ref_year, ref_month, 1)) [Reporting Period] FROM tab_prj_detail_report tab INNER JOIN( SELECT main.MSU_ID, REF_YEAR, REF_MONTH, submain.C_PROG, MR_OBJ_ID FROM PF_TAB_MonthlyStatusUpdate main LEFT JOIN PF_TAB_MonthlyStatusOBJLink submain ON main.MSU_ID = submain.MSU_ID AND main.Portfolio_Code = submain.Portfolio_Code) tab_pf ON tab.C_PROG = tab_pf.C_PROG AND tab.OBJ_ID = tab_pf.MR_OBJ_ID INNER JOIN BPM_OBJECTS O ON TAB.C_PROG = O.C_PROG AND tab.OBJ_ID = o.OBJ_ID ) x where C_PROG = "+c_prog+" order by x.[Reporting Period] desc";
            var data = ExecuteQueryWithStringType(context,sql1);
            if(data.Count()!=0)
            {
                var res = data[0]["Currenttime"].ToString();
                return res.ToString();

            }
            else
            {
                return null;
            }
        }

        public virtual DataTable ExecuteQueryForDT(AppDbContext poDbContext, string stmt)
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



        public ProjectDetails ProjectDetails(string obj_Code)
        {
            var context = new AppDbContext(GetAllOptions());
            string Sql = "select t1.C_PROG, t1.OBJ_TITLE ,t1.OBJ_DATE, T2.D_INI_FRC, T2.D_FIN_FRC,t3.P_ELMNT,t2.C_Vl from BPM_OBJECTS t1  inner join  PROG_T056  T2 on T2.C_PROG = t1.C_PROG and t1.F_ID = 10  inner join ASCN_OBS_PROG t3 on t3.C_PROG=t1.C_PROG where t1.OBJ_CODE = '" + obj_Code + "'AND F_ID = 10";
            List<Dictionary<string, string>> dt = ExecuteQueryWithStringType(context, Sql);
            if (dt.Any())
            {
                var data = new ProjectDetails() { ProjectCode = obj_Code, ProjectTitle = dt[0]["OBJ_TITLE"].ToString(), ProjId = Convert.ToDecimal(dt[0]["C_PROG"]), FinishDate = Convert.ToDateTime(dt[0]["D_FIN_FRC"]), StartDate = Convert.ToDateTime(dt[0]["D_INI_FRC"]), P_Elmt = Convert.ToInt32(dt[0]["P_ELMNT"]), obj_Date = Convert.ToDateTime(dt[0]["OBJ_DATE"]), Currency = dt[0]["C_Vl"] };
                return data;
            }
            else
            {
                return new ProjectDetails();
            }
        }


        public virtual List<Dictionary<string, string>> ExecuteQueryWithStringType(AppDbContext poDbContext, string stmt)
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

       // GetAllEvents
        public List<CPMTIntegrationEvent> GetAllEvents()
        {
            using (dbContext = new AppDbContext(GetAllOptions()))
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


        public List<MappingUpdateProjectSchedule> DbGetUpdatedProjectSchedules()
        {
            var context = new AppDbContext(GetAllOptions());
            string sql = "select c_prog , project_code, new_start_Date from CHANGE_IN_PROJ_DATES";
            var data = context.ProjectSchedules.FromSqlRaw(sql).ToList();
            return data;
        }
        public List<UpdateProjectScheduleModel> GetUpdatedProjectSchedules()
        {
            var data = new List<UpdateProjectScheduleModel>();
           
            var spmProjectDateUpdates = DbGetUpdatedProjectSchedules();
            var obj = new List<UpdateProjectSchedule>();
            var mainObj = new UpdateProjectScheduleModel();
            if (spmProjectDateUpdates != null)
            {
                foreach (var cpm in spmProjectDateUpdates)
                {
                    var costUpdateModel = new UpdateProjectSchedule();
                    costUpdateModel.project_code = cpm.project_code;
                    costUpdateModel.start_date = cpm.new_start_Date;

                    obj.Add(costUpdateModel);
                }
                mainObj.ProjectSchedules = obj;

                data.Add(mainObj);
            }

            return data;
        }


    }
}
