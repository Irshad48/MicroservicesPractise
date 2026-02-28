using AutoMapper;
using EmployeeMicroservice.DTOs.Requests.Employee;
using EmployeeMicroservice.DTOs.Requests.EmployeeSkill;
using EmployeeMicroservice.DTOs.Requests.Skill;
using EmployeeMicroservice.DTOs.Responses.Employee;
using EmployeeMicroservice.DTOs.Responses.EmployeeSkill;
using EmployeeMicroservice.DTOs.Responses.Skill;
using EmployeeMicroservice.Models;
using EmployeeMicroservice.Models.Entities;
using System;

namespace EmployeeMicroservice.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Employee mappings - map EmployeeSkills -> Skills property on response
            CreateMap<Employee, EmployeeResponseDto>()
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.EmployeeSkills));

            CreateMap<CreateEmployeeDto, Employee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeSkills, opt => opt.Ignore());

            CreateMap<UpdateEmployeeDto, Employee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.EmployeeSkills, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Skill mappings
            CreateMap<Skill, SkillResponseDto>();
            CreateMap<Skill, SkillDetailResponseDto>()
                .ForMember(dest => dest.Employees, opt => opt.MapFrom(src => src.EmployeeSkills));

            CreateMap<CreateSkillDto, Skill>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeSkills, opt => opt.Ignore());

            CreateMap<UpdateSkillDto, Skill>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeSkills, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // EmployeeSkill mappings - do NOT include employee fields in the compact skill response
            CreateMap<EmployeeSkill, EmployeeSkillResponseDto>()
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.SkillName, opt => opt.MapFrom(src => src.Skill != null ? src.Skill.Name : null))
                .ForMember(dest => dest.SkillCategory, opt => opt.MapFrom(src => src.Skill != null ? src.Skill.Category : null))
                .ForMember(dest => dest.ProficiencyLevel, opt => opt.MapFrom(src => src.ProficiencyLevel.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<AssignSkillDto, EmployeeSkill>()
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.Skill, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<UpdateEmployeeSkillDto, EmployeeSkill>()
                .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
                .ForMember(dest => dest.SkillId, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.Skill, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())             
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // For SkillDetailResponseDto's Employee details (keep full info here)
            CreateMap<EmployeeSkill, EmployeeSkillDetailDto>()
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Employee.Id))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => $"{src.Employee.FirstName} {src.Employee.LastName}"))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Employee.Email))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Employee.Position))
                .ForMember(dest => dest.ProficiencyLevel, opt => opt.MapFrom(src => src.ProficiencyLevel.ToString()))
                .ForMember(dest => dest.YearsOfExperience, opt => opt.MapFrom(src => src.YearsOfExperience))
                .ForMember(dest => dest.IsPrimarySkill, opt => opt.MapFrom(src => src.IsPrimarySkill));
        }
    }
}