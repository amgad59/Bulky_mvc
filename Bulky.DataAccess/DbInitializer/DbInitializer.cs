using Empire.DataAccess.Data;
using Empire.Models;
using Empire.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task Initialize()
        {
            try
            {
                if(_db.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count() > 0)
                {
                    await _db.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {

            }

            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();

                //if roles are not created, then we will create admin user as well
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@gmail.com",
                    Name = "Bhrugen Patel",
                    PhoneNumber = "1112223333",
                    StreetAddress = "test 123 Ave",
                    City = "Chicago"
                }, "admin").GetAwaiter().GetResult();

                ApplicationUser user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == "admin@gmail.com");
                await _userManager.AddToRoleAsync(user, SD.Role_Admin);

            }
            return;
        }
    }
}
