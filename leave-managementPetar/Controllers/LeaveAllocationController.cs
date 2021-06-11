using AutoMapper;
using leave_management.Contracts;
using leave_managementPetar.Contracts;
using leave_managementPetar.Data;
using leave_managementPetar.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managementPetar.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveAllocationController : Controller
    {
        private readonly IUnitOfWork _unitOfwork;
        private readonly IMapper mapo;
        private readonly UserManager<Employee> _userManager;

        public LeaveAllocationController( IMapper mapos,
           IUnitOfWork unitOfwork, UserManager<Employee> userManager)
        {
            
            mapo = mapos;
            _unitOfwork = unitOfwork;
            _userManager = userManager;
        }


        // GET: LeaveAllocationController
        public async Task<ActionResult> Index()
        {
            //var levetyps =await typo.FindAll();
            var leavetypes = await _unitOfwork.LeaveTypes.FindAll();
            var mappedLeaveTypes = mapo.Map<List<LeaveType>, List<LeaveTypeVM>>(leavetypes.ToList());
            var model = new CreateLeaveAllocationVM
            {
                LeaveTypes = mappedLeaveTypes,
                NumberUpdated = 0
            };

            return View(model);
        }

        public async Task<ActionResult> SetLeave(int id)
        {
            //var leavetype =await typo.FindById(id);
            var leavetype = await _unitOfwork.LeaveTypes.Find(q => q.Id == id);
            var employees =await _userManager.GetUsersInRoleAsync("Employee");
            var period = DateTime.Now.Year;
            foreach (var emp in employees)
            {
                //if (await _leaveallocationrepo.CheckAllocation(id, emp.Id))
                if (await _unitOfwork.LeaveAllocations.isExists(q => q.EmployeeId == emp.Id
                                        && q.LeaveTypeId == id
                                        && q.Period == period))
                    continue;
                var allocation = new LeaveAllocationVM
                {
                    DateCreated = DateTime.Now,
                    EmployeeId = emp.Id,
                    LeaveTypeId = id,
                    NumberOfDays = leavetype.DefaultDays,
                    Period = DateTime.Now.Year
                };
                var leaveallocation = mapo.Map<LeaveAllocation>(allocation);
                //await _leaveallocationrepo.Create(leaveallocation);
                await _unitOfwork.LeaveAllocations.Create(leaveallocation);
                await _unitOfwork.Save();
            }
            return RedirectToAction(nameof(Index));
        }


        public async Task<ActionResult> ListEmployees()
        {
            var employees =await _userManager.GetUsersInRoleAsync("Employee");
            var model = mapo.Map<List<EmployeeVM>>(employees);

            return View(model);
        }




        // GET: LeaveAllocationController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var employee = mapo.Map<EmployeeVM>(await _userManager.FindByIdAsync(id));
            //var allocations = mapo.Map<List<LeaveAllocationVM>>( await repo.GetLeaveAllocationsByEmployee(id));
            var period = DateTime.Now.Year;
            var records = await _unitOfwork.LeaveAllocations.FindAll(
               expression: q => q.EmployeeId == id && q.Period == period,
               includes: new List<string> { "LeaveType" }
           );
            var allocations = mapo.Map<List<LeaveAllocationVM>>(records);
            var model = new ViewAllocationsVM
            {
                Employee = employee,
                LeaveAllocations = allocations
            };
            return View(model);
        }

        // GET: LeaveAllocationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveAllocationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAllocationController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            //var leaveallocation =await repo.FindById(id);
            var leaveallocation = await _unitOfwork.LeaveAllocations.Find(q => q.Id == id,
                includes: new List<string> { "Employee", "LeaveType" });
            var model = mapo.Map<EditLeaveAllocationVM>(leaveallocation);
            return View(model);
        }

        // POST: LeaveAllocationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditLeaveAllocationVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                //var record =await repo.FindById(model.Id);
                var record = await _unitOfwork.LeaveAllocations.Find(q => q.Id == model.Id);
                record.NumberOfDays = model.NumberOfDays;
                _unitOfwork.LeaveAllocations.Update(record);
                await _unitOfwork.Save();
                return RedirectToAction(nameof(Details), new { id = model.EmployeeId });
            }
            catch
            {
                return View(model);
            }
        }

        // GET: LeaveAllocationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveAllocationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
