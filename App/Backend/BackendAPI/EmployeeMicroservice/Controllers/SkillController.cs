using AutoMapper;
using EmployeeMicroservice.DTOs.Requests.Skill;
using EmployeeMicroservice.DTOs.Responses.Skill;  // Make sure this using is present
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
    public class SkillController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SkillController(
            ILogger<SkillController> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper) : base(logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/skill
        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            try
            {
                _logger.LogInformation("Getting all skills");
                var skills = await _unitOfWork.Skills.GetAllAsync();
                var skillDtos = _mapper.Map<IEnumerable<SkillResponseDto>>(skills);
                return Ok(skillDtos);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "retrieving all skills");
            }
        }

        // GET: api/skill/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSkillById(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting skill with ID: {Id}", id);
                var skill = await _unitOfWork.Skills.GetByIdAsync(id);

                if (skill == null)
                    return NotFoundResponse("Skill", id);

                var skillDto = _mapper.Map<SkillResponseDto>(skill);
                return Ok(skillDto);
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"retrieving skill with ID {id}");
            }
        }

        // GET: api/skill/with-employees/{id}
        [HttpGet("with-employees/{id}")]
        public async Task<IActionResult> GetSkillWithEmployees(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting skill with employees for ID: {Id}", id);
                var skill = await _unitOfWork.Skills.GetSkillWithEmployeesAsync(id);

                if (skill == null)
                    return NotFoundResponse("Skill", id);

                // Use SkillDetailResponseDto instead of SkillWithEmployeesDto
                var skillDto = _mapper.Map<SkillDetailResponseDto>(skill);
                return Ok(skillDto);
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"retrieving skill with employees for ID {id}");
            }
        }

        // GET: api/skill/category/{category}
        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetSkillsByCategory(string category)
        {
            try
            {
                _logger.LogInformation("Getting skills for category: {Category}", category);
                var skills = await _unitOfWork.Skills.GetSkillsByCategoryAsync(category);
                var skillDtos = _mapper.Map<IEnumerable<SkillResponseDto>>(skills);
                return Ok(skillDtos);
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"retrieving skills for category {category}");
            }
        }

        // GET: api/skill/categories
        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                _logger.LogInformation("Getting all skill categories");
                var categories = await _unitOfWork.Skills.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "retrieving skill categories");
            }
        }

        // POST: api/skill
        [HttpPost]
        public async Task<IActionResult> CreateSkill([FromBody] CreateSkillDto createSkillDto)
        {
            try
            {
                _logger.LogInformation("Creating new skill with name: {Name}", createSkillDto.Name);

                // Check if skill name is unique
                if (!await _unitOfWork.Skills.IsSkillNameUniqueAsync(createSkillDto.Name))
                {
                    return BadRequestResponse($"Skill name '{createSkillDto.Name}' already exists.");
                }

                var skill = _mapper.Map<Skill>(createSkillDto);
                skill.Id = Guid.NewGuid(); // Ensure Id is set

                var createdSkill = await _unitOfWork.Skills.AddAsync(skill);

                var saved = await _unitOfWork.SaveChangesAsync();
                if (!saved)
                    return BadRequestResponse("Failed to save skill.");

                var skillDto = _mapper.Map<SkillResponseDto>(createdSkill);
                return CreatedAtAction(nameof(GetSkillById), new { id = skillDto.Id }, skillDto);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "creating skill");
            }
        }

        // PUT: api/skill/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSkill(Guid id, [FromBody] UpdateSkillDto updateSkillDto)
        {
            try
            {
                _logger.LogInformation("Updating skill with ID: {Id}", id);

                var existingSkill = await _unitOfWork.Skills.GetByIdAsync(id);
                if (existingSkill == null)
                    return NotFoundResponse("Skill", id);

                // Check if name is being changed and if it's unique
                if (!string.IsNullOrEmpty(updateSkillDto.Name) &&
                    updateSkillDto.Name != existingSkill.Name)
                {
                    if (!await _unitOfWork.Skills.IsSkillNameUniqueAsync(updateSkillDto.Name, id))
                    {
                        return BadRequestResponse($"Skill name '{updateSkillDto.Name}' already exists.");
                    }
                }

                _mapper.Map(updateSkillDto, existingSkill);
                existingSkill.UpdatedAt = DateTime.UtcNow;

                var updatedSkill = await _unitOfWork.Skills.UpdateAsync(existingSkill);

                var saved = await _unitOfWork.SaveChangesAsync();
                if (!saved)
                    return BadRequestResponse("Failed to update skill.");

                var skillDto = _mapper.Map<SkillResponseDto>(updatedSkill);
                return Ok(skillDto);
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"updating skill with ID {id}");
            }
        }

        // DELETE: api/skill/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkill(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting skill with ID: {Id}", id);

                // Check if skill is being used by any employees
                var skillWithEmployees = await _unitOfWork.Skills.GetSkillWithEmployeesAsync(id);
                if (skillWithEmployees?.EmployeeSkills != null && skillWithEmployees.EmployeeSkills.Any())
                {
                    return BadRequestResponse("Cannot delete skill that is assigned to employees.");
                }

                var deleted = await _unitOfWork.Skills.DeleteAsync(id);
                if (!deleted)
                    return NotFoundResponse("Skill", id);

                var saved = await _unitOfWork.SaveChangesAsync();
                if (!saved)
                    return BadRequestResponse("Failed to delete skill.");

                _logger.LogInformation("Skill with ID: {Id} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"deleting skill with ID {id}");
            }
        }
    }
}