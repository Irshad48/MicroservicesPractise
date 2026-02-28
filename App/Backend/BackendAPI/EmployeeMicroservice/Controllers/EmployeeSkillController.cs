using AutoMapper;
using EmployeeMicroservice.DTOs.Requests.EmployeeSkill;
using EmployeeMicroservice.DTOs.Responses.EmployeeSkill;
using EmployeeMicroservice.Interfaces;
using EmployeeMicroservice.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeMicroservice.Controllers
{
    public class EmployeeSkillController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeSkillController(
            ILogger<EmployeeSkillController> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper) : base(logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/employeeskill/employee/{employeeId}
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetEmployeeSkills(Guid employeeId)
        {
            try
            {
                _logger.LogInformation("Getting skills for employee ID: {EmployeeId}", employeeId);

                var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
                if (employee == null)
                    return NotFoundResponse("Employee", employeeId);

                var employeeSkills = await _unitOfWork.EmployeeSkills
                    .GetEmployeeSkillsWithDetailsAsync(employeeId);

                var skillDtos = _mapper.Map<IEnumerable<EmployeeSkillResponseDto>>(employeeSkills);
                return Ok(skillDtos);
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"retrieving skills for employee {employeeId}");
            }
        }

        // GET: api/employeeskill/skill/{skillId}
        [HttpGet("skill/{skillId}")]
        public async Task<IActionResult> GetSkillEmployees(Guid skillId)
        {
            try
            {
                _logger.LogInformation("Getting employees for skill ID: {SkillId}", skillId);

                var skill = await _unitOfWork.Skills.GetByIdAsync(skillId);
                if (skill == null)
                    return NotFoundResponse("Skill", skillId);

                var skillEmployees = await _unitOfWork.EmployeeSkills
                    .GetSkillEmployeesWithDetailsAsync(skillId);

                // Use EmployeeSkillResponseDto instead of the missing EmployeeSkillEmployeeResponseDto
                var employeeDtos = _mapper.Map<IEnumerable<EmployeeSkillResponseDto>>(skillEmployees);
                return Ok(employeeDtos);
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"retrieving employees for skill {skillId}");
            }
        }

        // POST: api/employeeskill
        [HttpPost]
        public async Task<IActionResult> AssignSkillToEmployee([FromBody] AssignSkillDto assignSkillDto)
        {
            try
            {
                _logger.LogInformation("Assigning skill {SkillId} to employee {EmployeeId}",
                    assignSkillDto.SkillId, assignSkillDto.EmployeeId);

                // Check if employee exists
                var employee = await _unitOfWork.Employees.GetByIdAsync(assignSkillDto.EmployeeId);
                if (employee == null)
                    return NotFoundResponse("Employee", assignSkillDto.EmployeeId);

                // Check if skill exists
                var skill = await _unitOfWork.Skills.GetByIdAsync(assignSkillDto.SkillId);
                if (skill == null)
                    return NotFoundResponse("Skill", assignSkillDto.SkillId);

                // Check if already assigned
                if (await _unitOfWork.EmployeeSkills.ExistsAsync(assignSkillDto.EmployeeId, assignSkillDto.SkillId))
                {
                    return BadRequestResponse("Skill already assigned to this employee.");
                }

                var employeeSkill = _mapper.Map<EmployeeSkill>(assignSkillDto);
                employeeSkill.CreatedAt = DateTime.UtcNow;

                var created = await _unitOfWork.EmployeeSkills.AddAsync(employeeSkill);
                var saved = await _unitOfWork.SaveChangesAsync();

                if (!saved)
                    return BadRequestResponse("Failed to assign skill.");

                // If this is set as primary, update other skills
                if (assignSkillDto.IsPrimarySkill)
                {
                    await _unitOfWork.EmployeeSkills.SetPrimarySkillAsync(
                        assignSkillDto.EmployeeId, assignSkillDto.SkillId);
                    await _unitOfWork.SaveChangesAsync();
                }

                var result = _mapper.Map<EmployeeSkillResponseDto>(created);
                return CreatedAtAction(nameof(GetEmployeeSkills), new { employeeId = result.EmployeeId }, result);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "assigning skill to employee");
            }
        }

        // PUT: api/employeeskill/{employeeId}/{skillId}
        [HttpPut("{employeeId}/{skillId}")]
        public async Task<IActionResult> UpdateEmployeeSkill(Guid employeeId, Guid skillId, [FromBody] UpdateEmployeeSkillDto updateDto)
        {
            try
            {
                _logger.LogInformation("Updating skill assignment for employee {EmployeeId}, skill {SkillId}",
                    employeeId, skillId);

                var employeeSkill = await _unitOfWork.EmployeeSkills
                    .GetEmployeeSkillAsync(employeeId, skillId);

                if (employeeSkill == null)
                    return NotFoundResponse("EmployeeSkill", $"{employeeId}-{skillId}");

                // Update properties if they are provided in the DTO
                if (updateDto.ProficiencyLevel.HasValue)
                    employeeSkill.ProficiencyLevel = updateDto.ProficiencyLevel.Value;

                if (updateDto.YearsOfExperience.HasValue)
                    employeeSkill.YearsOfExperience = updateDto.YearsOfExperience.Value;

                if (updateDto.AcquiredDate.HasValue)
                    employeeSkill.AcquiredDate = updateDto.AcquiredDate.Value;

                if (updateDto.IsPrimarySkill.HasValue)
                    employeeSkill.IsPrimarySkill = updateDto.IsPrimarySkill.Value;

                employeeSkill.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.EmployeeSkills.UpdateAsync(employeeSkill);
                var saved = await _unitOfWork.SaveChangesAsync();

                if (!saved)
                    return BadRequestResponse("Failed to update skill assignment.");

                // Handle primary skill changes
                if (updateDto.IsPrimarySkill.HasValue && updateDto.IsPrimarySkill.Value == true)
                {
                    await _unitOfWork.EmployeeSkills.SetPrimarySkillAsync(employeeId, skillId);
                    await _unitOfWork.SaveChangesAsync();
                }

                var result = _mapper.Map<EmployeeSkillResponseDto>(employeeSkill);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "updating employee skill");
            }
        }

        // DELETE: api/employeeskill/{employeeId}/{skillId}
        [HttpDelete("{employeeId}/{skillId}")]
        public async Task<IActionResult> RemoveSkillFromEmployee(Guid employeeId, Guid skillId)
        {
            try
            {
                _logger.LogInformation("Removing skill {SkillId} from employee {EmployeeId}",
                    skillId, employeeId);

                var employeeSkill = await _unitOfWork.EmployeeSkills
                    .GetEmployeeSkillAsync(employeeId, skillId);

                if (employeeSkill == null)
                    return NotFoundResponse("EmployeeSkill", $"{employeeId}-{skillId}");

                // For composite key, we need to remove using the entity
                // Since we're using UnitOfWork pattern, we need to add a specific delete method
                // For now, let's assume we have access to context or add a method

                // Option 1: If your repository has a Delete method that takes the entity
                var deleted = await _unitOfWork.EmployeeSkills.DeleteEmployeeSkillAsync(employeeId, skillId);

                if (!deleted)
                    return BadRequestResponse("Failed to remove skill.");

                var saved = await _unitOfWork.SaveChangesAsync();
                if (!saved)
                    return BadRequestResponse("Failed to save changes.");

                _logger.LogInformation("Skill removed successfully from employee {EmployeeId}", employeeId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleError(ex, "removing skill from employee");
            }
        }

        // GET: api/employeeskill/skill/{skillId}/level/{level}
        [HttpGet("skill/{skillId}/level/{level}")]
        public async Task<IActionResult> GetEmployeesBySkillAndLevel(Guid skillId, int level)
        {
            try
            {
                _logger.LogInformation("Getting employees for skill {SkillId} with minimum level {Level}",
                    skillId, level);

                var skillEmployees = await _unitOfWork.EmployeeSkills
                    .GetEmployeesBySkillAndLevelAsync(skillId, (Models.Enums.SkillLevel)level);

                var result = _mapper.Map<IEnumerable<EmployeeSkillResponseDto>>(skillEmployees);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"retrieving employees for skill {skillId} with level {level}");
            }
        }
    }
}