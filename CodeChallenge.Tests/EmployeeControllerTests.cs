
using System;
using System.Net;
using System.Net.Http;
using System.Text;

using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void GetReportingStructure_Manager()
        {
            // Arrange
            string employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f"; // John Lennon

            // Execute
			var getRequestTask = _httpClient.GetAsync($"api/employee/reportingStructure/{employeeId}");
			var response = getRequestTask.Result;

			// Assert
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			var reports = response.DeserializeContent<ReportingStructure>();
            Assert.AreEqual(reports.NumberOfReports, 4);
		}

		[TestMethod]
		public void GetReportingStructure_DeveloperV()
		{
			// Arrange
			string employeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f"; // Ringo Starr

			// Execute
			var getRequestTask = _httpClient.GetAsync($"api/employee/reportingStructure/{employeeId}");
			var response = getRequestTask.Result;

			// Assert
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			var reports = response.DeserializeContent<ReportingStructure>();
			Assert.AreEqual(reports.NumberOfReports, 2);
		}

		[TestMethod]
		public void GetReportingStructure_NoReports()
		{
			// Arrange
			string employeeId = "b7839309-3348-463b-a7e3-5de1c168beb3"; // Paul McCartney

			// Execute
			var getRequestTask = _httpClient.GetAsync($"api/employee/reportingStructure/{employeeId}");
			var response = getRequestTask.Result;

			// Assert
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			var reports = response.DeserializeContent<ReportingStructure>();
			Assert.AreEqual(reports.NumberOfReports, 0);
		}

        [TestMethod]
		public void AddCompensation()
		{
			// Arrange/Execute
            Compensation compensation;
			HttpResponseMessage response;
			AddEmployeeCompensation(out compensation, out response);

			// Assert
			Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

			var addedCompensation = response.DeserializeContent<Compensation>();
			Assert.AreEqual(addedCompensation.Salary, compensation.Salary);
			Assert.AreEqual(addedCompensation.EmployeeId, compensation.EmployeeId);
			Assert.AreEqual(addedCompensation.EffectiveDate, compensation.EffectiveDate);
		}

		private static void AddEmployeeCompensation(out Compensation compensation, out HttpResponseMessage response)
		{
			// Arrange
            compensation = new Compensation()
			{
				EmployeeId = "b7839309-3348-463b-a7e3-5de1c168beb3", // Paul McCartney
				EffectiveDate = DateTime.Today,
				Salary = 100000.00M,
			};
			var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
			var postRequestTask = _httpClient.PostAsync("api/employee/compensation",
			   new StringContent(requestContent, Encoding.UTF8, "application/json"));
			response = postRequestTask.Result;
		}

		[TestMethod]
        public void GetCompensation() 
        {
			// Arrange
			Compensation createCompensation;
			AddEmployeeCompensation(out createCompensation, out _);

			// Execute
			var getRequestTask = _httpClient.GetAsync($"api/employee/compensation/{createCompensation.EmployeeId}");
			var response = getRequestTask.Result;

			// Assert
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			var compensation = response.DeserializeContent<Compensation>();
			Assert.AreEqual(createCompensation.Salary, compensation.Salary);
            Assert.AreEqual(createCompensation.EmployeeId, compensation.EmployeeId);
            Assert.AreEqual(createCompensation.EffectiveDate, compensation.EffectiveDate);
		}
	}
}
