using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using dotnetapp.Models;
using System.Reflection;

namespace dotnetapp.Tests
{
    [TestFixture]
    public class EmployeeControllerTests
    {
         private const string EmployeeServiceName = "EmployeeService";
         private const string EmployeeControllerName = "EmployeeController";

        private HttpClient _httpClient;
        private Assembly _assembly;
        private Employee _testEmployee;

        [SetUp]
        public async Task Setup()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:8080"); // Base URL of your API
            _assembly = Assembly.GetAssembly(typeof(dotnetapp.Services.EmployeeService));
            
            // Create a new test item before each test case
            _testEmployee = await CreateTestEmployee();
        }

        private async Task<Employee> CreateTestEmployee()
        {
            var newEmployee = new Employee
            {
                EmployeeId = 0, // Assuming ID will be generated by the server
                Name = "Test Employee",
                Position = "Test Position",
                Salary = 50000m
            };

            var json = JsonConvert.SerializeObject(newEmployee);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/employee", content);
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<Employee>(await response.Content.ReadAsStringAsync());
        }

        [Test]
        public async Task Test_GetAllInventories_ReturnsListOfInventories()
        {
            // Arrange - no specific arrangement needed as we're not modifying state
            // Act
            var response = await _httpClient.GetAsync("api/Employee");
            response.EnsureSuccessStatusCode();

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            var items = JsonConvert.DeserializeObject<Employee[]>(content);

            Assert.IsNotNull(items);
            Assert.IsTrue(items.Length > 0);
        }

        [Test]
        public async Task Test_GetEmployeeById_ValidId_ReturnsEmployee()
        {
            // Arrange - no specific arrangement needed as we're not modifying state
            // Act
            var response = await _httpClient.GetAsync($"api/employee/{_testEmployee.EmployeeId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var employee = JsonConvert.DeserializeObject<Employee>(content);

            Assert.IsNotNull(employee);
            Assert.AreEqual(_testEmployee.EmployeeId, employee.EmployeeId);
        }

        [Test]
        public async Task Test_GetEmployeeById_ValidId_ReturnsEmployeeTest_GetEmployeeById_ValidId_ReturnsEmployee()
        {
            var response = await _httpClient.GetAsync("api/Employee/999999"); // Using an invalid ID
            
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public void Test_EmployeeService_Exist()
        {
            AssertServiceInstanceNotNull(EmployeeServiceName);
        }

        private void AssertServiceInstanceNotNull(string serviceName)
        {
            Type serviceType = _assembly.GetType($"dotnetapp.Services.{serviceName}");

            if (serviceType == null)
            {
                Assert.Fail($"Service {serviceName} does not exist.");
            }

            object serviceInstance = Activator.CreateInstance(serviceType);
            Assert.IsNotNull(serviceInstance);
        }
        [Test]
        public void Test_EmployeeController_Exist()
        {
            AssertControllerClassExist(EmployeeControllerName);
        }

        private void AssertControllerClassExist(string controllerName)
        {
            Type controllerType = _assembly.GetType($"dotnetapp.Controllers.{controllerName}");

            if (controllerType == null)
            {
                Assert.Fail($"Controller {controllerName} does not exist.");
            }
        }


        [TearDown]
        public async Task Cleanup()
        {
            _httpClient.Dispose();
        }

    }
}
