using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leave_managementPetar.Data;

namespace leave_managementPetar.Contracts
{
    public interface ILeaveRequestRepository : IRepositoryBase<LeaveRequest>
    {
        Task<ICollection<LeaveRequest>> GetLeaveRequestsByEmployee(string employeeid);
    }
}
