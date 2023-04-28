using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using WorkerService.Models;
using WorkerServiceApp1.Models;
using WorkerServiceApp1.Services;

namespace WorkerServiceApp1
{

    public interface IAPIEventHandler
    {
        public string DeleteChangeRequestType(string token, string apiUrl, string object_code);
        public  string CreateCredit(string token, string apiUrl, ProjectDetails projectDetails, int pElementCPMT);
        public string DeleteProject( string token,string apiUrl, string objectCode);
        public string CreateBudgetVersion(string token, string apiUrl, CPMTBudgetVersionModel data, string project_code);
        public string UpdateProjectStatus(string token,string apiUrl, ProjectStatusUpdateModel projectDetails);
        public string GetAPIToken( string apiUrl, Login body);
        public string GetSPMAPIToken(string apiUrl, Login body);
        public string UpdateProjectFormData(string token, string apiUrl, UpdateProjectFormDataModel data);
        public void UpdateCostPhasing(string token, string apiUrl, List<CPMTUpdateCostPhasingModel> data);
        public string CloseProjectCPMT(string token, string apiUrl, string project_code);
        public string CreateChangeRequestType(string token, string apiUrl, ChangeRequestTypeModel crsDetails);
        public void UpdateProjectSchedule(string token, string apiUrl, List<UpdateProjectScheduleModel> data);
    }

    public class APIEventHandler : IAPIEventHandler
    {
        public string DeleteChangeRequestType(string token, string apiUrl, string object_code)
        {
            var client = new RestClient(apiUrl + "/api/CPMTIntegration/DeleteChangeRequestType");
            client.Timeout = -1;
            var request = new RestRequest(Method.DELETE);
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("object_code", object_code, ParameterType.QueryString);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            var jsonResponse = JsonConvert.DeserializeObject<Response>(response.Content);
            return jsonResponse.message;
        }
        public string CloseProjectCPMT(string token, string apiUrl, string project_code)
        {
            var client = new RestClient(apiUrl + "/api/CPMTIntegration/CloseProject");
            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("project_code", project_code, ParameterType.QueryString);
            IRestResponse response = client.Execute(request);
            var jsonResponse = JsonConvert.DeserializeObject<Response>(response.Content);
            Console.WriteLine(response.Content);
            return jsonResponse.message;
        }

        public string CreateBudgetVersion(string token, string apiUrl, CPMTBudgetVersionModel data, string project_code)
        {
            CPMTBudgetVersionModelCall model = new CPMTBudgetVersionModelCall()
            {
                step_id = data.step_id,
                workflow_id = data.workflow_id,
                project_code = project_code,
                f_type = data.f_type,
                form_id = data.form_id,
                version = "Initial Budget Version",
            };
            var client = new RestClient(apiUrl + "/api/CPMTIntegration/CreateBudgetVersion");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", JsonConvert.SerializeObject(model), ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            var jsonResponse = JsonConvert.DeserializeObject<Response>(response.Content);
            Console.WriteLine(response.Content);
            return jsonResponse.message;
        }
        public void UpdateCostPhasing(string token,string apiUrl, List<CPMTUpdateCostPhasingModel> data)
        { 
            foreach (var cost in data)
            {
                var client = new RestClient(apiUrl + "/api/CPMTIntegration/UpdateCostPhasing");
                client.Timeout = -1;
                var request = new RestRequest(Method.PUT);
                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(cost), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
            }



        }
        public string CreateCredit( string token,string apiUrl, ProjectDetails projectDetails, int pElementCPMT)
        {
            CreateProjectModel creatProject = new CreateProjectModel()
            {
                ActivityBreakdownStructure = "NSG01",
                f_type = "CS",

                request_type = 1,

                P_Elmt = pElementCPMT,  /*projectDetails.P_Elmt,*/

                Code = projectDetails.ProjectCode,

                Title = projectDetails.ProjectTitle,

                InvestmentYear = 2022,

                StartDate =projectDetails.StartDate,

                FinishDate = projectDetails.FinishDate,

                Currency = projectDetails.Currency

            };
            var client = new RestClient( apiUrl + "/api/CPMTIntegration/CreateCredit");

            client.Timeout = -1;

            var request = new RestRequest(Method.POST);

            request.AddHeader("Authorization", "Bearer " + token);

            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("application/json", JsonConvert.SerializeObject(creatProject), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            Console.WriteLine(response.Content);

            var jsonResponse = JsonConvert.DeserializeObject<Response>(response.Content);
            return jsonResponse.message; 
        }


        public string DeleteProject(string token,string apiUrl, string object_code)
        {
         

            var client = new RestClient(apiUrl + "/api/CPMTIntegration/CPMTDeleteProject");

            client.Timeout = -1;

            var request = new RestRequest(Method.DELETE);

            request.AddHeader("Authorization", "Bearer " + token);

            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("object_code", object_code, ParameterType.QueryString);

            IRestResponse response = client.Execute(request);

            Console.WriteLine(response.Content);

            var jsonResponse = JsonConvert.DeserializeObject<Response>(response.Content);
            return jsonResponse.message;

        }

        public string UpdateProjectStatus(string token,string apiUrl, ProjectStatusUpdateModel data)
        { 
            MappingProjectStatusModel updateStatus = new MappingProjectStatusModel()
            {
                State = data.project_status,
                Currenttime = DateTime.Now,
                Comment = "",
                Currenttimeptn = DateTime.Now,
                CheckStatus = "",
                CheckValue = ""
            };

            var client = new RestClient(apiUrl + "/api/CPMTIntegration/ChangeProjectStatus");

            client.Timeout = -1;

            var request = new RestRequest(Method.PUT);

            request.AddHeader("Authorization", "Bearer " + token);

            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("application/json", JsonConvert.SerializeObject(updateStatus), ParameterType.RequestBody);
            request.AddParameter("object_code", data.project_code, ParameterType.QueryString);

            IRestResponse response = client.Execute(request);

            Console.WriteLine(response.Content);
            var jsonResponse = JsonConvert.DeserializeObject<Response>(response.Content);
            return jsonResponse.message;


        }

        public string GetSPMAPIToken(string apiUrl, Login body)
        {

            string _apiUrl = apiUrl;
            var client = new RestClient(_apiUrl + "/api/UppwiseAD/GetToken");
            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            var Token = JsonConvert.DeserializeObject<TokenGen>(response.Content);
            return Token.data.token;


        }

        public string GetAPIToken( string apiUrl,Login body)
        {

            string _apiUrl = apiUrl;
            var client = new RestClient(_apiUrl + "/api/UppwiseAD/GetToken");
            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            var Token = JsonConvert.DeserializeObject<TokenGen>(response.Content);
            
            return Token.data.token;


        }
        public string UpdateProjectFormData(string token, string apiUrl, UpdateProjectFormDataModel data)
        {
           MappingProjectFormDataModel  updateStatus = new MappingProjectFormDataModel()
            {
                State = data.Status,
                Currenttime = data.Currenttime,
                prod_category = data.Prod_category
            };

            var client = new RestClient(apiUrl + "/api/CPMTIntegration/UpdateProjectFormData");

            client.Timeout = -1;

            var request = new RestRequest(Method.PUT);

            request.AddHeader("Authorization", "Bearer " + token);

            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("application/json", JsonConvert.SerializeObject(updateStatus), ParameterType.RequestBody);
            request.AddParameter("object_code", data.project_code, ParameterType.QueryString);

            IRestResponse response = client.Execute(request);

            Console.WriteLine(response.Content);

            var jsonResponse = JsonConvert.DeserializeObject<Response>(response.Content);
            return jsonResponse.message;
        }
        public string CreateChangeRequestType(string token, string apiUrl, ChangeRequestTypeModel crsDetails)
        {
            crsDetails.f_type = "CRS";

            var client = new RestClient(apiUrl + "/api/CPMTIntegration/CreateChangeRequestType");

            client.Timeout = -1;

            var request = new RestRequest(Method.POST);

            request.AddHeader("Authorization", "Bearer " + token);

            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("application/json", JsonConvert.SerializeObject(crsDetails), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            Console.WriteLine(response.Content);

            var jsonResponse = JsonConvert.DeserializeObject<Response>(response.Content);
            return jsonResponse.message;
        }

        public void UpdateProjectSchedule(string token, string apiUrl, List<UpdateProjectScheduleModel> data)
        {
            foreach (var scheduleupdate in data)
            {
                var client = new RestClient(apiUrl + "/api/CPMTIntegration/UpdateProjectSchedule");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(scheduleupdate), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
            }
        }


        public void DeleteCostUpdatedProject()
        {
            throw new NotImplementedException();
        }
    }
}
