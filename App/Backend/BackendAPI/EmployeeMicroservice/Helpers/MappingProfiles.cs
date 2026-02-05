using AutoMapper;
using EmployeeMicroservice.DTOs.Requests;
using EmployeeMicroservice.DTOs.Responses;
using EmployeeMicroservice.Models;

namespace EmployeeMicroservice.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // CreateEmployeeDto -> Employee
            CreateMap<CreateEmployeeDto, Employee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId));

            // UpdateEmployeeDto -> Employee
            CreateMap<UpdateEmployeeDto, Employee>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null)); // Only map if source property is not null

            // Employee -> EmployeeResponseDto
            CreateMap<Employee, EmployeeResponseDto>();
        }
    }
}