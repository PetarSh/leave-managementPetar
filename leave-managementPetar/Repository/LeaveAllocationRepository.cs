using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leave_managementPetar.Contracts;
using leave_managementPetar.Data;
using Microsoft.EntityFrameworkCore;

namespace leave_managementPetar.Repository
{
    public class LeaveAllocationRepository : ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext dbContext;

        public LeaveAllocationRepository (ApplicationDbContext db)
        {
            dbContext = db;
        }

        public async Task<bool> CheckAllocation(int leavetypeid, string employeeid)
        {
            var period = DateTime.Now.Year;
            var allocations = await FindAll();
            return allocations.Where(q => q.EmployeeId == employeeid
                                        && q.LeaveTypeId == leavetypeid
                                        && q.Period == period)
                .Any();
        }

        public async Task<bool> Create(LeaveAllocation entity)
        {
             await dbContext.LeaveAllocations.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveAllocation entity)
        {
            dbContext.LeaveAllocations.Remove(entity);
            return await Save();
        }

        public async Task< ICollection<LeaveAllocation>> FindAll()
        {
            //var alocations = dbContext.LeaveAllocations.ToList();
            //return alocations;
            var alocations = await dbContext.LeaveAllocations
               .Include(q => q.LeaveType)
               .Include(q => q.Employee)
               .ToListAsync();
            return alocations;
        }

        public async Task<LeaveAllocation> FindById(int id)
        {
            //var alocation = dbContext.LeaveAllocations.Find(id);
            //return alocation;

            var alocation =await dbContext.LeaveAllocations
                .Include(q => q.LeaveType)
                .Include(q => q.Employee)
                .FirstOrDefaultAsync(q => q.Id == id);
            return alocation;
        }

        public async Task<ICollection<LeaveAllocation>> GetLeaveAllocationsByEmployee(string employeeid)
        {
            var period = DateTime.Now.Year;
            var allocations = await FindAll();
            return allocations.Where(q => q.EmployeeId == employeeid && q.Period == period)
                    .ToList();
        }

        public async Task<LeaveAllocation> GetLeaveAllocationsByEmployeeAndType(string employeeid, int leavetypeid)
        {
            var period = DateTime.Now.Year;
            var allocations = await FindAll();
            return allocations.FirstOrDefault(q => q.EmployeeId == employeeid
                                                    && q.Period == period
                                                    && q.LeaveTypeId == leavetypeid);
        }

        public async Task<bool> isExists(int id)
        {
            var exist = await dbContext.LeaveAllocations.AnyAsync(q => q.Id == id);
            return exist;
            
        }

        public async Task<bool> Save()
        {
            var changes = await dbContext.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(LeaveAllocation entity)
        {
            dbContext.LeaveAllocations.Update(entity);
            return await Save();
        }
    }
}
