using leave_managementPetar.Contracts;
using leave_managementPetar.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace leave_managementPetar.Repository
{
    
    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        private readonly ApplicationDbContext dbContext;

        public LeaveTypeRepository(ApplicationDbContext db)
        {
            dbContext = db;
        }
        public async Task<bool> Create(LeaveType entity)
        {
            await dbContext.LeaveTypes.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveType entity)
        {
            dbContext.LeaveTypes.Remove(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveType>> FindAll()
        {
            var leaveTypes= await dbContext.LeaveTypes.ToListAsync();
            return leaveTypes;
        }

        public async Task<LeaveType> FindById(int id)
        {
            var leaveType =await dbContext.LeaveTypes.FindAsync(id);
            return leaveType;
        }

        public ICollection<LeaveType> GetEmployeesByLeaveType(int id)
        {
             throw new NotImplementedException();
        }

        public async Task<bool> isExists(int id)
        {
            var exist = await dbContext.LeaveTypes.AnyAsync(q => q.Id == id);
            return exist;
        }

        public async Task<bool> Save()
        {
            var changes=await dbContext.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(LeaveType entity)
        {
           dbContext.LeaveTypes.Update(entity);
            return await Save();
        }
    }
}
