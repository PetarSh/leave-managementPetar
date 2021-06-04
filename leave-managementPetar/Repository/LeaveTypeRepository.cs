using leave_managementPetar.Contracts;
using leave_managementPetar.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managementPetar.Repository
{
    
    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        private readonly ApplicationDbContext dbContext;

        public LeaveTypeRepository(ApplicationDbContext db)
        {
            dbContext = db;
        }
        public bool Create(LeaveType entity)
        {
            dbContext.LeaveTypes.Add(entity);
            return Save();
        }

        public bool Delete(LeaveType entity)
        {
            dbContext.LeaveTypes.Remove(entity);
            return Save();
        }

        public ICollection<LeaveType> FindAll()
        {
            var leaveTypes= dbContext.LeaveTypes.ToList();
            return leaveTypes;
        }

        public LeaveType FindById(int id)
        {
            var leaveType = dbContext.LeaveTypes.Find(id);
            return leaveType;
        }

        public ICollection<LeaveType> GetEmployeesByLeaveType(int id)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            var changes= dbContext.SaveChanges();
            return changes > 0;
        }

        public bool Update(LeaveType entity)
        {
            dbContext.LeaveTypes.Update(entity);
            return Save();
        }
    }
}
