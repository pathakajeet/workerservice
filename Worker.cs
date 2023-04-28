using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using WorkerServiceApp1.Models;
using WorkerServiceApp1.Services;

namespace WorkerServiceApp1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker>  _logger;
        private readonly DbHelper dbHelper;
        private readonly CPMTDbHelper _cpmtdbHelper;
       

        public IAPIEventHandler _apiEventHandler;


        private readonly IConfiguration _configuration;
        public Worker(ILogger<Worker> logger, IConfiguration configuration, IAPIEventHandler apiEventHandler)
        
        {
             _logger = logger;
            dbHelper = new DbHelper();
            _apiEventHandler = apiEventHandler;
             _configuration = configuration;
            _cpmtdbHelper =  new CPMTDbHelper();
            
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("This service has been stopped.......");
            return base.StopAsync(cancellationToken);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                 _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                string apiUrl = _configuration.GetConnectionString("CPMTApiUrl");
                string spmApiURl = _configuration.GetConnectionString("SPMApiUrl");
                var userName = _configuration.GetSection("UserCred:Username").Value;
                var passWord = _configuration.GetSection("UserCred:Password").Value;
                string spmUser = _configuration.GetSection("UserCredSPM:Username").Value;
                string spmPassword = _configuration.GetSection("UserCredSPM:Password").Value;
                 var currentDate= _configuration.GetSection("CurrentDate:DateTimeNow").Value;

                    Login body = new Login()
                    {
                        Username = userName,
                        Password = passWord,
                        PinPass = "",
                        CustomerId = 0,
                    };

                    Login spmbody = new Login()
                    {
                        Username = spmUser,
                        Password = spmPassword,
                        PinPass = "",
                        CustomerId = 0,
                    };
                    string token = _apiEventHandler.GetAPIToken(apiUrl, body);
                    string spmToken = _apiEventHandler.GetSPMAPIToken(spmApiURl, spmbody);
                    _logger.LogInformation("Token working", token.ToString());

                    var dtCPMT = _cpmtdbHelper.GetProjectDetails(currentDate);
                    var dtSPM = dbHelper.GetupdatedProjectlist(currentDate);


                    var cpmtProjectList = from project in dtCPMT.AsEnumerable() select (Convert.ToString(project.Field<string>("Obj_Code")));
                    var spmProjectList = from project in dtSPM.AsEnumerable() select (Convert.ToString(project.Field<string>("Obj_Code")));

                    string cpmtProjectLists = String.Join(",", cpmtProjectList.ToList());
                    string spmProjectLists = String.Join(",", spmProjectList.ToList());

                    var result = _cpmtdbHelper.GetProjectsToCreate(cpmtProjectLists, spmProjectLists);
                    if (result.Count > 0)
                    {
                        foreach (var prj in result)
                        {
                            var projDetails = dbHelper.ProjectDetails(Convert.ToString(prj["ProjectCode"]));
                            int pElement = projDetails.P_Elmt;
                            string sElement = dbHelper.GetpElementSPM(pElement);
                            int pElementCPMT = _cpmtdbHelper.GetpElementCPMT(sElement);
                            var response = _apiEventHandler.CreateCredit(token, apiUrl, projDetails, pElementCPMT);
                            _logger.LogInformation(response + " for " + Convert.ToString(prj["ProjectCode"]));

                        }


                    }
                    var dtCrSPM = dbHelper.GetChangeRequestDetails(currentDate);
                    var dtCrCPMT = _cpmtdbHelper.GetChangeRequestDetails(currentDate);

                    var cpmtCrList = from changeRequest in dtCrCPMT.AsEnumerable() select (Convert.ToString(changeRequest.Field<string>("Obj_Code")));
                    var spmCrList = from changeRequest in dtCrSPM.AsEnumerable() select (Convert.ToString(changeRequest.Field<string>("Obj_Code")));

                    string cpmtCrLists = String.Join(",", cpmtCrList.ToList());
                    string spmCrLists = String.Join(",", spmCrList.ToList());
                    var crResult = _cpmtdbHelper.GetCrsToCreate(cpmtCrLists, spmCrLists);
                    if (crResult.Count > 0)
                    {
                        foreach (var cr in crResult)
                        {
                            var crDetails = dbHelper.ChangeRequestTypeDetails(Convert.ToString(cr["ProjectCode"]));
                            var response = _apiEventHandler.CreateChangeRequestType(token, apiUrl, crDetails);
                            _logger.LogInformation(response + " for " + Convert.ToString(cr["ProjectCode"]));


                        }
                    }

                    var res = _cpmtdbHelper.GetProjectsToDelete(cpmtProjectLists, spmProjectLists);

                    if (res.Count >= 1)
                    {
                        foreach (var prj in res)
                        {
                            var projDetails = _cpmtdbHelper.ProjectDetails(Convert.ToString(prj["ProjectCode"]));

                            string objectCode = Convert.ToString(prj["ProjectCode"]);
                            var response = _apiEventHandler.DeleteProject(token, apiUrl, objectCode);
                            _logger.LogInformation(response + " for " + Convert.ToString(objectCode));

                        }
                    }

                    var spmProjectStatusList = dbHelper.GetProjectStateDetail();
                    var cpmtProjectStatusList = _cpmtdbHelper.GetProjectStateDetail();

                    var spmFormDt = dbHelper.GetProjectFormData(currentDate);
                    var cpmtFormDt = _cpmtdbHelper.GetProjectFormData(currentDate);


                    var prjUpdateList = from spm in spmProjectStatusList
                                        join cpmt in cpmtProjectStatusList on spm.project_code equals cpmt.project_code
                                        where spm.project_status != cpmt.project_status &&
                                        spm.project_status == 2 && cpmt.project_status == 0
                                        select new
                                        {
                                            project_code = spm.project_code,
                                            project_status = spm.project_status
                                        };
                    if (prjUpdateList.Count() >= 1)
                    {
                        foreach (var prj in prjUpdateList)
                        {
                            ProjectStatusUpdateModel check = new ProjectStatusUpdateModel()
                            {
                                project_code = prj.project_code,
                                project_status = prj.project_status
                            };
                            var response = _apiEventHandler.UpdateProjectStatus(token, apiUrl, check);
                            var info = _cpmtdbHelper.GetProjectBudgetInfo(check.project_code);
                            var responseInfo = _apiEventHandler.CreateBudgetVersion(token, apiUrl, info, check.project_code);

                            _logger.LogInformation(response + " for " + Convert.ToString(check.project_code));
                        }
                    }


                    // Map category 

                    var prod_catSPMData = new List<UpdateProjectFormDataModel>();
                    foreach (var spm in spmFormDt)
                    {
                        var spm_update = new UpdateProjectFormDataModel() { Status = spm.Status, project_code = spm.project_code, c_prog = spm.c_prog, Currenttime = spm.Currenttime };
                        spm_update.Prod_category = (dbHelper.GetProductCategory(spm.c_prog));
                        prod_catSPMData.Add(spm_update);
                    }
                    var updatePrjData = from spm in prod_catSPMData
                                        join cpmt in cpmtFormDt on spm.project_code equals cpmt.project_code
                                        where spm.Prod_category.Any(m => cpmt.Prod_category.Any(n => n.C_CODE != m.C_CODE)) || spm.Status != cpmt.Status || spm.Currenttime > cpmt.Currenttime
                                        select new
                                        {
                                            project_code = spm.project_code,
                                            status = spm.Status,
                                            Currenttimeptn = spm.Currenttime,
                                            Product_Category = spm.Prod_category,
                                        };
                    if (updatePrjData.Count() >= 1)
                    {
                        foreach (var prj in updatePrjData)
                        {
                            UpdateProjectFormDataModel check = new UpdateProjectFormDataModel()
                            {

                                project_code = prj.project_code,
                                Status = prj.status,
                                Currenttime = prj.Currenttimeptn,
                                Prod_category = prj.Product_Category

                            };
                            var response = _apiEventHandler.UpdateProjectFormData(token, apiUrl, check);
                            _logger.LogInformation(response + " for " + Convert.ToString(check.project_code));
                        }
                    }

                    var close_PrjList = from spm in spmProjectStatusList
                                        join cpmt in cpmtProjectStatusList on spm.project_code equals cpmt.project_code
                                        where spm.project_status == 3 && cpmt.project_status != 3
                                        select new
                                        {
                                            project_code = spm.project_code,
                                            project_status = spm.project_status
                                        };
                    if (close_PrjList.Count() >= 1)
                    {
                        foreach (var prj in close_PrjList)
                        {
                            ProjectStatusUpdateModel check = new ProjectStatusUpdateModel()
                            {
                                project_code = prj.project_code,
                                project_status = prj.project_status
                            };
                            var response = _apiEventHandler.CloseProjectCPMT(token, apiUrl, check.project_code);
                            _logger.LogInformation(response + " for " + Convert.ToString(check.project_code));
                        }
                    }

                    var updatePrjStatusList = from spm in spmProjectStatusList
                                              join cpmt in cpmtProjectStatusList on spm.project_code equals cpmt.project_code
                                              where spm.project_status != cpmt.project_status
                                              select new
                                              {

                                                  project_code = spm.project_code,
                                                  project_status = spm.project_status

                                              };

                    if (updatePrjStatusList.Count() >= 1)
                    {
                        foreach (var prj in updatePrjStatusList)
                        {
                            ProjectStatusUpdateModel check = new ProjectStatusUpdateModel()
                            {

                                project_code = prj.project_code,
                                project_status = prj.project_status
                            };
                            var response = _apiEventHandler.UpdateProjectStatus(token, apiUrl, check);
                            _logger.LogInformation(response + " for " + Convert.ToString(check.project_code));
                        }
                    }
                    var cpmtCostElementListForProject = _cpmtdbHelper.GetCostElementDetails();
                    _apiEventHandler.UpdateCostPhasing(spmToken, spmApiURl, cpmtCostElementListForProject);
                    //  _cpmtdbHelper.DeleteProjectCost();

                    var crRes = _cpmtdbHelper.GetCrsToDelete(cpmtCrLists, spmCrLists);
                    if (crRes.Count > 0)
                    {
                        foreach (var cr in crRes)
                        {
                            string objectCode = Convert.ToString(cr["Obj_Code"]);
                            var response = _apiEventHandler.DeleteChangeRequestType(token, apiUrl, objectCode);
                            _logger.LogInformation(response + " for " + Convert.ToString(objectCode)); ;
                        }
                    }

                    var spmProjectDatesToBeUpdated = dbHelper.GetUpdatedProjectSchedules();
                    _apiEventHandler.UpdateProjectSchedule(spmToken, apiUrl, spmProjectDatesToBeUpdated);


                }
                catch (Exception ex )
                {

                     _logger.LogInformation("Error logged  running at: {time}", ex.Message);
                }
                await Task.Delay(3000, stoppingToken);
            }


        }

        
    }
}
