using leave_managementPetar.Contracts;
using leave_managementPetar.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace leave_managementPetar.Repository
{
    
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext dbContext;

        public LeaveRequestRepository(ApplicationDbContext db)
        {
            dbContext = db;
        }

        public async Task<bool> Create(LeaveRequest entity)
        {
            await dbContext.LeaveRequests.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveRequest entity)
        {
           dbContext.LeaveRequests.Remove(entity);
            return await Save();
        }

        public async Task< ICollection<LeaveRequest>> FindAll()
        {
            var leaveRequests = await dbContext.LeaveRequests
                .Include(q => q.RequestingEmployee)
                .Include(q => q.ApprovedBy)
                .Include(q => q.LeaveType)
                .ToListAsync();
            return leaveRequests;
        }

        public async Task<LeaveRequest> FindById(int id)
        {
            var leaveRequests = await dbContext.LeaveRequests
                .Include(q => q.RequestingEmployee)
                .Include(q => q.ApprovedBy)
                .Include(q => q.LeaveType).
                FirstOrDefaultAsync(q => q.Id==id);
            return leaveRequests;
        }

        public async Task< ICollection<LeaveRequest>> GetLeaveRequestsByEmployee(string employeeid)
        {
            var leaveRequests = await FindAll();
            return leaveRequests.Where(q => q.RequestingEmployeeId == employeeid)
            .ToList();
        }

        public async Task<bool> isExists(int id)
        {
            var exist =await dbContext.LeaveRequests.AnyAsync(q => q.Id == id);
            return exist;
        }

        public async Task<bool> Save()
        {
            var changes = await dbContext.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(LeaveRequest entity)
        {
            dbContext.LeaveRequests.Update(entity);
            return await Save();
        }
    }
}
