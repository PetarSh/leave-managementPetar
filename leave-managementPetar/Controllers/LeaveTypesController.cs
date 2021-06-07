using AutoMapper;
using leave_managementPetar.Contracts;
using leave_managementPetar.Data;
using leave_managementPetar.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managementPetar.Controllers
{
    [Authorize(Roles ="Administrator")]
    public class LeaveTypesController : Controller
    {
        private readonly ILeaveTypeRepository repo;
        private readonly IMapper mapo;

        public LeaveTypesController(ILeaveTypeRepository repos, IMapper mapos)
        {
            repo = repos;
            mapo = mapos;
        }
        
        // GET: LeaveTypesController
        
        public ActionResult Index()
        {
            var levetyps = repo.FindAll().ToList();
            var model = mapo.Map < List<LeaveType>, List<LeaveTypeVM>>(levetyps);

            return View(model);
        }

        // GET: LeaveTypesController/Details/5
        public ActionResult Details(int id)
        {
            if (!repo.isExists(id))
            {
                return NotFound();
            }
            var leavetype = repo.FindById(id);
            var model = mapo.Map<LeaveTypeVM>(leavetype);

            return View(model);
        }

        // GET: LeaveTypesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveTypesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LeaveTypeVM model)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return View(model);
                }
                var leavetype = mapo.Map<LeaveType>(model);
                leavetype.DateCreated = DateTime.Now;
                var isok=repo.Create(leavetype);
                if(!isok)
                {
                    ModelState.AddModelError("", "something went wrong with insert");
                    return View(model);
                }


                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveTypesController/Edit/5
        public ActionResult Edit(int id)
        {
            if(!repo.isExists(id))
            {
               return NotFound();
            }

            var leavetype = repo.FindById(id);
            var model = mapo.Map<LeaveTypeVM>(leavetype);

            return View(model);
        }

        // POST: LeaveTypesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LeaveTypeVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var leavetype = mapo.Map<LeaveType>(model);
                leavetype.DateCreated = DateTime.Now;
                var isok = repo.Update(leavetype);
                if (!isok)
                {
                    ModelState.AddModelError("", "something went wrong with update");
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "something went wrong with update");
                return View(model);
            }
        }

        // GET: LeaveTypesController/Delete/5
        public ActionResult Delete(int id)
        {
            var leavetype = repo.FindById(id);
            if (leavetype == null)
            {
                return NotFound();
            }
            var isok = repo.Delete(leavetype);
            if (!isok)
            {

                return BadRequest();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: LeaveTypesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id,LeaveTypeVM model)
        {
            try
            {
               
                var leavetype = repo.FindById(id);
                if (leavetype==null)
                {
                    return NotFound();
                }
                var isok = repo.Delete(leavetype);
                if (!isok)
                {
                    
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(model);
            }
        }
    }
}
