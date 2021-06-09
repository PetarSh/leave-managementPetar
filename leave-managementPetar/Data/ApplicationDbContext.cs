using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using leave_managementPetar.Models;

namespace leave_managementPetar.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employess { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<LeaveAllocation> LeaveAllocations { get; set; }
        public DbSet<leave_managementPetar.Models.LeaveRequestVM> LeaveRequestVM { get; set; }
        //public DbSet<LeaveTypeVM> DetailsLeaveTypeVM { get; set; }
        //public DbSet<EmployeeVM> EmployeeVM { get; set; }
        ////public DbSet<LeaveTypeVM> DetailsLeaveTypeVM { get; set; }
        //public DbSet<leave_managementPetar.Models.LeaveRequestVM> LeaveRequestVM { get; set; }
        //public DbSet<leave_managementPetar.Models.LeaveAllocationVM> LeaveAllocationVM { get; set; }
        //public DbSet<leave_managementPetar.Models.EditLeaveAllocationVM> EditLeaveAllocationVM { get; set; }

    }
}
