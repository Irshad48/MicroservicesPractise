using AutoMapper;
using DepartmentMicroservice.DTOs.Requests;
using DepartmentMicroservice.DTOs.Responses;
using DepartmentMicroservice.Interfaces;
using DepartmentMicroservice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DepartmentMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(ILogger<DepartmentController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var depts = await _unitOfWork.Departments.GetAllActiveAsync();
            return Ok(_mapper.Map<System.Collections.Generic.IEnumerable<DepartmentResponseDto>>(depts));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var dept = await _unitOfWork.Departments.GetByIdActiveAsync(id);
            if (dept == null) return NotFound();
            return Ok(_mapper.Map<DepartmentResponseDto>(dept));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentDto dto)
        {
            if (await _unitOfWork.Departments.NameExistsAsync(dto.Name))
                return BadRequest($"Department name '{dto.Name}' already exists.");

            var dept = _mapper.Map<Department>(dto);
            await _unitOfWork.Departments.CreateAsync(dept);
            var saved = await _unitOfWork.SaveChangesAsync();
            if (!saved) return BadRequest("Failed to create department.");

            var response = _mapper.Map<DepartmentResponseDto>(dept);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentDto dto)
        {
            var existing = await _unitOfWork.Departments.GetByIdActiveAsync(id);
            if (existing == null) return NotFound();

            if (!string.IsNullOrEmpty(dto.Name) && await _unitOfWork.Departments.NameExistsAsync(dto.Name, id))
                return BadRequest($"Department name '{dto.Name}' is already in use.");

            _mapper.Map(dto, existing);
            var updated = await _unitOfWork.Departments.UpdateAsync(id, existing);
            if (updated == null) return BadRequest("Update failed");

            var saved = await _unitOfWork.SaveChangesAsync();
            if (!saved) return BadRequest("Failed to save department update.");

            return Ok(_mapper.Map<DepartmentResponseDto>(updated));
        }

        // PATCH: api/department/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] UpdateDepartmentDto patchDto)
        {
            try
            {
                _logger.LogInformation("Patching department with ID: {Id}", id);

                var existing = await _unitOfWork.Departments.GetByIdActiveAsync(id);
                if (existing == null) return NotFound();

                if (!string.IsNullOrEmpty(patchDto.Name) && patchDto.Name != existing.Name)
                {
                    if (await _unitOfWork.Departments.NameExistsAsync(patchDto.Name, id))
                    {
                        return BadRequest($"Department name '{patchDto.Name}' is already in use.");
                    }
                }

                if (patchDto.Name != null) existing.Name = patchDto.Name;
                if (patchDto.Description != null) existing.Description = patchDto.Description;

                existing.UpdatedAt = DateTime.UtcNow;

                var updated = await _unitOfWork.Departments.UpdateAsync(id, existing);
                if (updated == null) return BadRequest("Update failed");

                var saved = await _unitOfWork.SaveChangesAsync();
                if (!saved) return BadRequest("Failed to save department update.");

                return Ok(_mapper.Map<DepartmentResponseDto>(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error patching department {Id}", id);
                return Problem($"Error patching department {id}");
            }
        }

        // DELETE: api/department/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting department with ID: {Id}", id);

                var dept = await _unitOfWork.Departments.GetByIdActiveAsync(id);
                if (dept == null) return NotFound();

                // Idempotent: if already soft-deleted, return 204 No Content
                if (!dept.IsActive)
                {
                    return NoContent();
                }

                // Soft delete
                dept.IsActive = false;
                dept.UpdatedAt = DateTime.UtcNow;

                var updated = await _unitOfWork.Departments.UpdateAsync(id, dept);
                if (updated == null) return BadRequest("Failed to delete department.");

                var saved = await _unitOfWork.SaveChangesAsync();
                if (!saved) return BadRequest("Failed to delete department.");

                var dto = _mapper.Map<DepartmentResponseDto>(updated);

                _logger.LogInformation("Department with ID: {Id} deleted successfully", id);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department {Id}", id);
                return Problem($"Error deleting department {id}");
            }
        }
    }
}
