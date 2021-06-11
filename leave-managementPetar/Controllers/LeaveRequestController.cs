using AutoMapper;
using leave_management.Contracts;
using leave_managementPetar.Contracts;
using leave_managementPetar.Data;
using leave_managementPetar.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managementPetar.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly IUnitOfWork _unitOfwork;
        private readonly IMapper mapo;
        private readonly UserManager<Employee> _userManager;

        public LeaveRequestController( 
            IMapper mapos,
            UserManager<Employee> userManager,
            IUnitOfWork unitOfwork)
        {
            _userManager = userManager;
            mapo = mapos;
            _unitOfwork = unitOfwork;
        }

        [Authorize(Roles = "Administrator")]
        // GET: LeaveRequestController
        public async Task<ActionResult> Index()
        {
            var leaveRequests = await _unitOfwork.LeaveRequests.FindAll(
                includes: new List<string> { "RequestingEmployee", "LeaveType" });
            var leaveRequstsModel = mapo.Map<List<LeaveRequestVM>>(leaveRequests);
            var model = new AdminLeaveRequestViewVM
            {
                TotalRequests = leaveRequstsModel.Count,
                ApprovedRequests = leaveRequstsModel.Count(q => q.Approved == true),
                PendingRequests = leaveRequstsModel.Count(q => q.Approved == null),
                RejectedRequests = leaveRequstsModel.Count(q => q.Approved == false),
                LeaveRequests = leaveRequstsModel
            };
            return View(model);
        }

        public async Task<ActionResult> MyLeave()
        {
            var employee =await _userManager.GetUserAsync(User);
            var employeeid = employee.Id;
            //var employeeAllocations =await _leaveAllocRepo.GetLeaveAllocationsByEmployee(employeeid);
            //var employeeRequests =await repo.GetLeaveRequestsByEmployee(employeeid);

            var employeeAllocations = await _unitOfwork.LeaveAllocations.FindAll(q => q.EmployeeId == employeeid,
               includes: new List<string> { "LeaveType" });

            var employeeRequests = await _unitOfwork.LeaveRequests
                .FindAll(q => q.RequestingEmployeeId == employeeid);

            var employeeAllocationsModel = mapo.Map<List<LeaveAllocationVM>>(employeeAllocations);
            var employeeRequestsModel = mapo.Map<List<LeaveRequestVM>>(employeeRequests);

            var model = new EmployeeLeaveRequestViewVM
            {
                LeaveAllocations = employeeAllocationsModel,
                LeaveRequests = employeeRequestsModel
            };

            return View(model);

        }

        // GET: LeaveRequestController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            //var leaveRequest =await repo.FindById(id);
            var leaveRequest = await _unitOfwork.LeaveRequests.Find(q => q.Id == id,
               includes: new List<string> { "ApprovedBy", "RequestingEmployee", "LeaveType" });
            var model = mapo.Map<LeaveRequestVM>(leaveRequest);
            return View(model);
        }

        public async Task<ActionResult> ApproveRequest(int id)
        {
            try
            {
                var user =await _userManager.GetUserAsync(User);
                //var leaveRequest =await repo.FindById(id);
                var leaveRequest = await _unitOfwork.LeaveRequests.Find(q => q.Id == id);
                var employeeid = leaveRequest.RequestingEmployeeId;
                var leaveTypeId = leaveRequest.LeaveTypeId;
                var period = DateTime.Now.Year;
                //var allocation =await _leaveAllocRepo.GetLeaveAllocationsByEmployeeAndType(employeeid, leaveTypeId);
                var allocation = await _unitOfwork.LeaveAllocations.Find(q => q.EmployeeId == employeeid
                                                    && q.Period == period
                                                    && q.LeaveTypeId == leaveTypeId);

                int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
                
                allocation.NumberOfDays = allocation.NumberOfDays - daysRequested;

                leaveRequest.Approved = true;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.DateActioned = DateTime.Now;

                //await repo.Update(leaveRequest);
                //await _leaveAllocRepo.Update(allocation);
                _unitOfwork.LeaveRequests.Update(leaveRequest);
                _unitOfwork.LeaveAllocations.Update(allocation);
                await _unitOfwork.Save();

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<ActionResult> RejectRequest(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                //var leaveRequest = await repo.FindById(id);
                var leaveRequest = await _unitOfwork.LeaveRequests.Find(q => q.Id == id);
                leaveRequest.Approved = false;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.DateActioned = DateTime.Now;

                //await repo.Update(leaveRequest);
                _unitOfwork.LeaveRequests.Update(leaveRequest);
                await _unitOfwork.Save();
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

            // GET: LeaveRequestController/Create
        public async Task<ActionResult> Create()
        {
            //var leaveTypes =await _leaveTypeRepo.FindAll();
            var leaveTypes = await _unitOfwork.LeaveTypes.FindAll();
            var leaveTypeItems = leaveTypes.Select(q => new SelectListItem
            {
                Text = q.Name,
                Value = q.Id.ToString()
            });
            var model = new CreateLeaveRequestVM
            {
                LeaveTypes = leaveTypeItems
            };
            return View(model);
        }

        // POST: LeaveRequestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateLeaveRequestVM model)
        {
            try
            {
                var startDate = Convert.ToDateTime(model.StartDate);
                var endDate = Convert.ToDateTime(model.EndDate);
                var leaveTypes = await _unitOfwork.LeaveTypes.FindAll();

                var employee = await _userManager.GetUserAsync(User);
                var period = DateTime.Now.Year;
                var allocation = await _unitOfwork.LeaveAllocations.Find(q => q.EmployeeId == employee.Id
                                                    && q.Period == period
                                                    && q.LeaveTypeId == model.LeaveTypeId);
                int daysRequested = (int)(endDate - startDate).TotalDays;
                var leaveTypeItems = leaveTypes.Select(q => new SelectListItem
                {
                    Text = q.Name,
                    Value = q.Id.ToString()
                });
                model.LeaveTypes = leaveTypeItems;


                if (allocation == null)
                {
                    ModelState.AddModelError("", "You Have No Days Left");
                }
                if (DateTime.Compare(startDate, endDate) > 1)
                {
                    ModelState.AddModelError("", "Start Date cannot be further in the future than the End Date");
                }
                if (daysRequested > allocation.NumberOfDays)
                {
                    ModelState.AddModelError("", "You Do Not Sufficient Days For This Request");
                }
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var leaveRequestModel = new LeaveRequestVM
                {
                    RequestingEmployeeId = employee.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    Approved = null,
                    DateRequested = DateTime.Now,
                    DateActioned = DateTime.Now,
                    LeaveTypeId = model.LeaveTypeId,
                    RequestComments = model.RequestComments
                };

                var leaveRequest = mapo.Map<LeaveRequest>(leaveRequestModel);
                await _unitOfwork.LeaveRequests.Create(leaveRequest);
                await _unitOfwork.Save();


                return RedirectToAction("MyLeave");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong with insert");
                return View(model);
            }
        }

        public async Task< ActionResult> CancelRequest(int id)
        {
            //var leaveRequest =await repo.FindById(id);
            var leaveRequest = await _unitOfwork.LeaveRequests.Find(q => q.Id == id);
            leaveRequest.Cancelled = true;
            _unitOfwork.LeaveRequests.Update(leaveRequest);
            await _unitOfwork.Save();
            return RedirectToAction("MyLeave");
        }

        // GET: LeaveRequestController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: LeaveRequestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Delete/5
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
