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
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult RoleManagement(string userId)
        {
            var roleId = _db.UserRoles.FirstOrDefault(u=>u.UserId == userId).RoleId;
            RoleManagementVM roleManagementVM = new RoleManagementVM()
            {
                ApplicationUser = _db.ApplicationUsers.FirstOrDefault(u=>u.Id == userId),
                RoleList = _db.Roles.Select(i=>new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                })
            };
            roleManagementVM.ApplicationUser.role = _db.Roles.FirstOrDefault(u => u.Id == roleId).Name;
            return View(roleManagementVM);
        }
        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
		{
			var roleId = _db.UserRoles.FirstOrDefault(u => u.UserId == roleManagementVM.ApplicationUser.Id).RoleId;
            var oldRole = _db.Roles.FirstOrDefault(u => u.Id == roleId).Name;
            if(!(roleManagementVM.ApplicationUser.role == oldRole))
            {
                ApplicationUser applicationUser = _db.ApplicationUsers
                    .FirstOrDefault(u => u.Id == roleManagementVM.ApplicationUser.Id);
                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser,roleManagementVM.ApplicationUser.role)
                    .GetAwaiter().GetResult();
            }
            return RedirectToAction("Index");
        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<ApplicationUser> users = await _db.ApplicationUsers.ToListAsync();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach(var user in users)
            {
                var roleId = userRoles.FirstOrDefault(u=>u.UserId == user.Id).RoleId;
                user.role = roles.FirstOrDefault(u=>u.Id == roleId).Name;

            }

            return Json(new { data = users });
        }
        [HttpPost]
        public async Task<IActionResult> LockUnlock([FromBody]string id)
        {
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == id);
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
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Operation Successful" });
        }
        #endregion
    }
}
