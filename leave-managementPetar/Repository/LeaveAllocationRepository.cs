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

        public bool CheckAllocation(int leavetypeid, string employeeid)
        {
            var period = DateTime.Now.Year;
            return FindAll()
                .Where(q => q.EmployeeId == employeeid && q.LeaveTypeId == leavetypeid && q.Period == period)
                .Any();
        }

        public bool Create(LeaveAllocation entity)
        {
            dbContext.LeaveAllocations.Add(entity);
            return Save();
        }

        public bool Delete(LeaveAllocation entity)
        {
            dbContext.LeaveAllocations.Remove(entity);
            return Save();
        }

        public ICollection<LeaveAllocation> FindAll()
        {
            //var alocations = dbContext.LeaveAllocations.ToList();
            //return alocations;
            var alocations = dbContext.LeaveAllocations
               .Include(q => q.LeaveType)
               .Include(q => q.Employee)
               .ToList();
            return alocations;
        }

        public LeaveAllocation FindById(int id)
        {
            //var alocation = dbContext.LeaveAllocations.Find(id);
            //return alocation;

            var alocation = dbContext.LeaveAllocations
                .Include(q => q.LeaveType)
                .Include(q => q.Employee)
                .FirstOrDefault(q => q.Id == id);
            return alocation;
        }

        public ICollection<LeaveAllocation> GetLeaveAllocationsByEmployee(string id)
        {
            var period = DateTime.Now.Year;
            return FindAll()
                    .Where(q => q.EmployeeId == id && q.Period == period)
                    .ToList();
        }

        public LeaveAllocation GetLeaveAllocationsByEmployeeAndType(string id, int leavetypeid)
        {
            var period = DateTime.Now.Year;
            return FindAll()
                    .FirstOrDefault(q => q.EmployeeId == id && q.Period == period && q.LeaveTypeId == leavetypeid);
        }

        public bool isExists(int id)
        {
            var exist = dbContext.LeaveAllocations.Any(q => q.Id == id);
            return exist;
        }

        public bool Save()
        {
            var changes = dbContext.SaveChanges();
            return changes > 0;
        }

        public bool Update(LeaveAllocation entity)
        {
            dbContext.LeaveAllocations.Update(entity);
            return Save();
        }
    }
}
