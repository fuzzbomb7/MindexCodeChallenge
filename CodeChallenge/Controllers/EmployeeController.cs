﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        [HttpGet("reportingStructure/{id}")]
        public IActionResult GetReportingStructure(String id) 
        {
            _logger.LogDebug($"Received reporting structure request for '{id}'");

			var employee = _employeeService.GetById(id);

			if (employee == null)
				return NotFound();

            var reportingStructure = _employeeService.GetReportingStructure(employee);

            return Ok(reportingStructure);
		}

        [HttpGet("compensation/{id}", Name = "getCompensation")]
        public IActionResult GetCompensation(String id)
        {
            _logger.LogDebug($"Received compensation request for '{id}");

            var compensation = _employeeService.GetCompensation(id);

            if(compensation == null)
                return NotFound();

            return Ok(compensation);
        }

        [HttpPost("compensation")]
        public IActionResult AddCompensation(Compensation compensation)
        {
            _logger.LogDebug($"Received compensation add request for '{compensation.EmployeeId}'");

             var addCompensation = _employeeService.AddCompensation(compensation);

            return CreatedAtRoute("getCompensation", new { id = addCompensation.EmployeeId }, addCompensation);
        }
    }
}
