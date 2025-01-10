using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        public ReportingStructure GetReportingStructure(Employee employee)
        {
            var reportingStructure = new ReportingStructure()
            {
                Employee = employee,
                NumberOfReports = CountReports(employee),
            };
            return reportingStructure;
        }

        private int CountReports(Employee employee, int totalReports = 0)
        {
            foreach (var report in employee.DirectReports)
            {
				totalReports++;

                // Copy the foreach iterator variable to a new variable
				Employee currentReport = report;

                // Refetch employee with direct reports
				if (currentReport.DirectReports is null)
					currentReport = _employeeRepository.GetById(currentReport.EmployeeId);

                // Use recursion to count sub-reports
				if (currentReport.DirectReports.Count > 0) 
                    totalReports = CountReports(currentReport, totalReports);
            }
            return totalReports;
        }

        public Compensation AddCompensation(Compensation compensation)
        {
            if(compensation != null)
            {
                _employeeRepository.AddCompensation(compensation);
                _employeeRepository.SaveAsync().Wait();
            }
            return compensation;
        }

        public Compensation GetCompensation(string id)
        {
            if(!string.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetCompensation(id);
            }

            return null;
        }
    }
}
