using Empire.DataAccess.Data;
using Empire.DataAccess.Repository;
using Empire.DataAccess.Repository.IRepository;
using Empire.Models;
using Empire.Models.ViewModels;
using Empire.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EmpireApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(UserManager<IdentityUser> userManager
            , RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> RoleManagement(string userId)
        {
            var user = await _unitOfWork.ApplicationUser.GetEntity(u=>u.Id == userId);
            RoleManagementVM roleManagementVM = new RoleManagementVM()
            {
                ApplicationUser = await _unitOfWork.ApplicationUser.GetEntity(u => u.Id == userId),
                RoleList = _roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                })
            };
            roleManagementVM.ApplicationUser.role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault()!;
            return View(roleManagementVM);
        }
        [HttpPost]
        public async Task<IActionResult> RoleManagement(RoleManagementVM roleManagementVM)
        {
            var oldRole = _userManager.GetRolesAsync(roleManagementVM.ApplicationUser).GetAwaiter().GetResult().FirstOrDefault();
            if(oldRole == null)
            {
                return BadRequest();
            }
            ApplicationUser applicationUser = await _unitOfWork.ApplicationUser
                .GetEntity(u=>u.Id == roleManagementVM.ApplicationUser.Id,isTracked:true);

            if(!(roleManagementVM.ApplicationUser.role == oldRole))
            {
                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagementVM.ApplicationUser.role)
                    .GetAwaiter().GetResult();
            }
            return RedirectToAction("Index");
        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<ApplicationUser> users = _unitOfWork.ApplicationUser.GetAllEntities().GetAwaiter().GetResult().ToList();

            foreach(var user in users)
            {
                var u = await _userManager.GetRolesAsync(user);
                if(u != null)
                    user.role = u.FirstOrDefault()!;
            }

            return Json(new { data = users });
        }
        [HttpPost]
        public async Task<IActionResult> LockUnlock([FromBody]string id)
        {
            var user = await _unitOfWork.ApplicationUser.GetEntity(u => u.Id == id);
            if(user == null)
            {
                return Json(new { success = false, message = "Error while unlocking" });
            }

            if(user.LockoutEnd != null &&  user.LockoutEnd > DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now;
            }
            else
            {
                user.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _unitOfWork.ApplicationUser.Update(user);
            await _unitOfWork.Save();
            return Json(new { success = true, message = "Operation Successful" });
        }
        #endregion
    }
}
