using AutoMapper;
using EmployeeMicroservice.DTOs.Requests.Employee;
using EmployeeMicroservice.DTOs.Responses.Employee;
using EmployeeMicroservice.Interfaces;
using EmployeeMicroservice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; 

namespace EmployeeMicroservice.Controllers
{
    public class EmployeeController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeController(
            ILogger<EmployeeController> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper) : base(logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/employee
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                _logger.LogInformation("Getting all employees");
                var employees = await _unitOfWork.Employees.GetAllAsync();
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
                var employee = await _unitOfWork.Employees.GetByIdAsync(id);

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
                _logger.LogInformation("Creating new employee with email: {Email}", createEmployeeDto.Email);

                // Check if email already exists
                if (await _unitOfWork.Employees.EmailExistsAsync(createEmployeeDto.Email))
                {
                    return BadRequestResponse($"Email '{createEmployeeDto.Email}' is already in use.");
                }

                // Map DTO to Employee entity
                var employee = _mapper.Map<Employee>(createEmployeeDto);

                // Create employee
                var createdEmployee = await _unitOfWork.Employees.CreateAsync(employee);

                // Save changes
                var saved = await _unitOfWork.SaveChangesAsync();
                if (!saved)
                {
                    return BadRequestResponse("Failed to save employee to database.");
                }

                // Map to response DTO
                var employeeDto = _mapper.Map<EmployeeResponseDto>(createdEmployee);

                _logger.LogInformation("Employee created with ID: {Id}", employeeDto.Id);
                return CreatedAtAction(nameof(GetEmployeeById), new { id = employeeDto.Id }, employeeDto);
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
                _logger.LogInformation("Updating employee with ID: {Id}", id);

                // Check if employee exists
                var existingEmployee = await _unitOfWork.Employees.GetByIdAsync(id);
                if (existingEmployee == null)
                {
                    return NotFoundResponse("Employee", id);
                }

                // Check if email is being changed and if it already exists
                if (!string.IsNullOrEmpty(updateEmployeeDto.Email) &&
                    updateEmployeeDto.Email != existingEmployee.Email)
                {
                    if (await _unitOfWork.Employees.EmailExistsAsync(updateEmployeeDto.Email, id))
                    {
                        return BadRequestResponse($"Email '{updateEmployeeDto.Email}' is already in use.");
                    }
                }

                // Map update DTO to existing employee
                _mapper.Map(updateEmployeeDto, existingEmployee);

                // Update employee
                var updatedEmployee = await _unitOfWork.Employees.UpdateAsync(id, existingEmployee);
                if (updatedEmployee == null)
                {
                    return BadRequestResponse("Failed to update employee.");
                }

                // Save changes
                var saved = await _unitOfWork.SaveChangesAsync();
                if (!saved)
                {
                    return BadRequestResponse("Failed to save updated employee to database.");
                }

                // Map to response DTO
                var employeeDto = _mapper.Map<EmployeeResponseDto>(updatedEmployee);

                _logger.LogInformation("Employee with ID: {Id} updated successfully", id);
                return Ok(employeeDto);
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"updating employee with ID {id}");
            }
        }

        // DELETE: api/employee/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting employee with ID: {Id}", id);

                // Check if employee exists
                var employee = await _unitOfWork.Employees.GetByIdAsync(id);
                if (employee == null)
                {
                    return NotFoundResponse("Employee", id);
                }

                // Soft delete (set IsActive to false) instead of hard delete
                employee.IsActive = false;
                var updated = await _unitOfWork.Employees.UpdateAsync(id, employee);

                if (updated == null)
                {
                    return BadRequestResponse("Failed to delete employee.");
                }

                // Save changes
                var saved = await _unitOfWork.SaveChangesAsync();
                if (!saved)
                {
                    return BadRequestResponse("Failed to save deletion to database.");
                }

                _logger.LogInformation("Employee with ID: {Id} deleted successfully", id);
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
                var employees = await _unitOfWork.Employees.GetByDepartmentIdAsync(departmentId);
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