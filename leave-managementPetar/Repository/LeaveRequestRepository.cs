﻿using leave_managementPetar.Contracts;
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

        public bool Create(LeaveRequest entity)
        {
            dbContext.LeaveRequests.Add(entity);
            return Save();
        }

        public bool Delete(LeaveRequest entity)
        {
            dbContext.LeaveRequests.Remove(entity);
            return Save();
        }

        public ICollection<LeaveRequest> FindAll()
        {
            var leaveRequests = dbContext.LeaveRequests
                .Include(q => q.RequestingEmployee)
                .Include(q => q.ApprovedBy)
                .Include(q => q.LeaveType)
                .ToList();
            return leaveRequests;
        }

        public LeaveRequest FindById(int id)
        {
            var leaveRequests = dbContext.LeaveRequests
                .Include(q => q.RequestingEmployee)
                .Include(q => q.ApprovedBy)
                .Include(q => q.LeaveType).
                FirstOrDefault(q => q.Id==id);
            return leaveRequests;
        }

        public ICollection<LeaveRequest> GetLeaveRequestsByEmployee(string employeeid)
        {
            var leaveRequests = FindAll()
                 .Where(q => q.RequestingEmployeeId == employeeid)
                 .ToList();
            return leaveRequests;
        }

        public bool isExists(int id)
        {
            var exist = dbContext.LeaveRequests.Any(q => q.Id == id);
            return exist;
        }

        public bool Save()
        {
            var changes = dbContext.SaveChanges();
            return changes > 0;
        }

        public bool Update(LeaveRequest entity)
        {
            dbContext.LeaveRequests.Update(entity);
            return Save();
        }
    }
}