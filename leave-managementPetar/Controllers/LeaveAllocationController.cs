using AutoMapper;
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
        private readonly ILeaveTypeRepository typo;
        private readonly ILeaveAllocationRepository repo;
        private readonly IMapper mapo;
        private readonly UserManager<Employee> _userManager;

        public LeaveAllocationController(ILeaveAllocationRepository repos, IMapper mapos,
            ILeaveTypeRepository typos, UserManager<Employee> userManager)
        {
            repo = repos;
            mapo = mapos;
            typo = typos;
            _userManager = userManager;
        }


        // GET: LeaveAllocationController
        public async Task<ActionResult> Index()
        {
            var levetyps =await typo.FindAll();
            var mappedLeaveTypes = mapo.Map<List<LeaveType>, List<LeaveTypeVM>>(levetyps.ToList());
            var model = new CreateLeaveAllocationVM
            {
                LeaveTypes = mappedLeaveTypes,
                NumberUpdated = 0
            };

            return View(model);
        }

        public async Task<ActionResult> SetLeave(int id)
        {
            var leavetype =await typo.FindById(id);
            var employees =await _userManager.GetUsersInRoleAsync("Employee");
            foreach (var emp in employees)
            {
                if (await repo.CheckAllocation(id, emp.Id))
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
                await repo.Create(leaveallocation);
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
            var allocations = mapo.Map<List<LeaveAllocationVM>>( await repo.GetLeaveAllocationsByEmployee(id));
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
            var leaveallocation =await repo.FindById(id);
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
                var record =await repo.FindById(model.Id);
                record.NumberOfDays = model.NumberOfDays;
                var isSuccess =await repo.Update(record);
                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Error while saving");
                    return View(model);
                }
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
