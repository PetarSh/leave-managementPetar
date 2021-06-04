using leave_managementPetar.Contracts;
using leave_managementPetar.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managementPetar.Repository
{
    
    public class LeaveHistoryRepository : ILeaveHistoryRepository
    {
        private readonly ApplicationDbContext dbContext;

        public LeaveHistoryRepository(ApplicationDbContext db)
        {
            dbContext = db;
        }

        public bool Create(LeaveHistory entity)
        {
            dbContext.LeaveHistorys.Add(entity);
            return Save();
        }

        public bool Delete(LeaveHistory entity)
        {
            dbContext.LeaveHistorys.Remove(entity);
            return Save();
        }

        public ICollection<LeaveHistory> FindAll()
        {
            var histories = dbContext.LeaveHistorys.ToList();
            return histories;
        }

        public LeaveHistory FindById(int id)
        {
            var history = dbContext.LeaveHistorys.Find(id);
            return history;
        }

        public bool isExists(int id)
        {
            var exist = dbContext.LeaveHistorys.Any(q => q.Id == id);
            return exist;
        }

        public bool Save()
        {
            var changes = dbContext.SaveChanges();
            return changes > 0;
        }

        public bool Update(LeaveHistory entity)
        {
            dbContext.LeaveHistorys.Update(entity);
            return Save();
        }
    }
}
