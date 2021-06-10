using leave_managementPetar.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managementPetar.Contracts
{
    public interface ILeaveAllocationRepository:IRepositoryBase<LeaveAllocation>
    {
        Task<bool> CheckAllocation(int leavetypeid, string employeeid);
        Task<ICollection<LeaveAllocation>> GetLeaveAllocationsByEmployee(string id);
        Task<LeaveAllocation> GetLeaveAllocationsByEmployeeAndType(string id, int leavetypeid);
    }
}
