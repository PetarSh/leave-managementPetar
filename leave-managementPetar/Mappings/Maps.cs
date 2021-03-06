using AutoMapper;
using leave_managementPetar.Data;
using leave_managementPetar.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managementPetar.Mappings
{
    public class Maps:Profile
    {
        public Maps()
        {
            CreateMap<LeaveType, LeaveTypeVM>().ReverseMap();
            CreateMap<LeaveRequest, LeaveRequestVM>().ReverseMap();
            CreateMap<LeaveAllocation, LeaveAllocationVM>().ReverseMap();
            CreateMap<LeaveAllocation, EditLeaveAllocationVM>().ReverseMap();
            CreateMap<Employee, EmployeeVM>().ReverseMap();
            
        }
    }
}
