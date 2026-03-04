using AutoMapper;
using EmployeeMicroservice.DTOs.Requests.Employee;
using EmployeeMicroservice.DTOs.Responses.Employee;
using EmployeeMicroservice.Interfaces;
using EmployeeMicroservice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;

namespace EmployeeMicroservice.Controllers
{
    public class EmployeeController : BaseApiController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public EmployeeController(
            ILogger<EmployeeController> logger,
            IEmployeeService employeeService,
            IMapper mapper) : base(logger)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        // GET: api/employee
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                _logger.LogInformation("Getting all employees");
                var employees = await _employeeService.GetAllAsync();
                var employeeDtos = _mapper.Map<IEnumerable<EmployeeResponseDto>>(employees);

                _logger.LogInformation("Retrieved {Count} employees", employeeDtos.Count());
                return Ok(employeeDtos);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "retrieving all employees");
            }
        }

        // GET: api/employee/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting employee with ID: {Id}", id);
                var employee = await _employeeService.GetByIdAsync(id);

                if (employee == null)
                {
                    return NotFoundResponse("Employee", id);
                }

                var employeeDto = _mapper.Map<EmployeeResponseDto>(employee);
                return Ok(employeeDto);
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"retrieving employee with ID {id}");
            }
        }

        // POST: api/employee
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto createEmployeeDto)
        {
            try
            {

                // Map DTO to Employee entity
                var employee = _mapper.Map<Employee>(createEmployeeDto);

                var createdEmployee = await _employeeService.CreateAsync(employee);

                var employeeDto = _mapper.Map<EmployeeResponseDto>(createdEmployee);

                return CreatedAtAction(nameof(GetEmployeeById),
                    new { id = employeeDto.Id },
                    employeeDto);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "creating employee");
            }
        }

        // PUT: api/employee/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            try
            {
                var employeeToUpdate = _mapper.Map<Employee>(updateEmployeeDto);

                var updatedEmployee = await _employeeService.UpdateAsync(id, employeeToUpdate);

                if (updatedEmployee == null)
                {
                    return NotFoundResponse("Employee", id);
                }

                var employeeDto = _mapper.Map<EmployeeResponseDto>(updatedEmployee);

                return Ok(employeeDto);
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"updating employee with ID {id}");
            }
        }
        // PATCH: api/employee/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchEmployee(Guid id, [FromBody] UpdateEmployeeDto patchDto)
        {
            try
            {
                var existingEmployee = await _employeeService.GetByIdAsync(id);

                if (existingEmployee == null)
                {
                    return NotFoundResponse("Employee", id);
                }

                // Apply patch manually
                if (patchDto.FirstName != null) existingEmployee.FirstName = patchDto.FirstName;
                if (patchDto.LastName != null) existingEmployee.LastName = patchDto.LastName;
                if (patchDto.Email != null) existingEmployee.Email = patchDto.Email;
                if (patchDto.DateOfBirth.HasValue) existingEmployee.DateOfBirth = patchDto.DateOfBirth.Value;
                if (patchDto.Position != null) existingEmployee.Position = patchDto.Position;
                if (patchDto.Salary.HasValue) existingEmployee.Salary = patchDto.Salary.Value;
                if (patchDto.DepartmentId.HasValue) existingEmployee.DepartmentId = patchDto.DepartmentId.Value;
                if (patchDto.IsActive.HasValue) existingEmployee.IsActive = patchDto.IsActive.Value;

                var updated = await _employeeService.UpdateAsync(id, existingEmployee);

                var dto = _mapper.Map<EmployeeResponseDto>(updated);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"patching employee with ID {id}");
            }
        }

        // DELETE: api/employee/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            try
            {
                var deleted = await _employeeService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFoundResponse("Employee", id);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"deleting employee with ID {id}");
            }
        }

        // GET: api/employee/department/{departmentId}
        [HttpGet("department/{departmentId}")]
        public async Task<IActionResult> GetEmployeesByDepartment(Guid departmentId)
        {
            try
            {
                _logger.LogInformation("Getting employees for department ID: {DepartmentId}", departmentId);
                var employees = await _employeeService.GetByDepartmentIdAsync(departmentId);
                var employeeDtos = _mapper.Map<IEnumerable<EmployeeResponseDto>>(employees);

                _logger.LogInformation("Retrieved {Count} employees for department {DepartmentId}",
                    employeeDtos.Count(), departmentId);
                return Ok(employeeDtos);
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"retrieving employees for department {departmentId}");
            }
        }
    }
}