using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreProject.DataAccess.Data;
using StoreProject.DataAccess.Repository;
using StoreProject.DataAccess.Repository.IRepository;
using StoreProject.Models;
using StoreProject.Models.ViewModels;
using StoreProject.Utility;

namespace StoreProjectWeb.Areas.Admin.Controllers
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

        public IActionResult RoleManagement(string? id)
        {
            var roleId = _db.UserRoles.FirstOrDefault(u => u.UserId == id).RoleId;

            UserVM productVM = new()
            {
                ApplicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id),
                CompanyList = _db.Companies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.CompanyId.ToString(),
                }),
                RoleList = _db.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name,
                })
            };

            productVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == roleId).Name;
            return View(productVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(UserVM userVM)
        {
            string oldRoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userVM.ApplicationUser.Id).RoleId;
            string oldRole = _db.Roles.FirstOrDefault(u => u.Id == oldRoleId).Name;

            if(userVM.ApplicationUser.Role != oldRole)
            {
                ApplicationUser userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userVM.ApplicationUser.Id);

                if (userVM.ApplicationUser.Role == SD.Role_Company)
                {
                    userFromDb.CompanyId = userVM.ApplicationUser.CompanyId;
                }
                else
                {
                    userFromDb.CompanyId = null;
                }

                _db.SaveChanges();

                _userManager.RemoveFromRoleAsync(userFromDb, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(userFromDb, userVM.ApplicationUser.Role).GetAwaiter().GetResult();

            }

            return RedirectToAction(nameof(Index));
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _db.ApplicationUsers.Include(u => u.Company).ToList();
            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            

            foreach(var user in objUserList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

                if(user.Company == null)
                {
                    user.Company = new() { Name = "" };
                }
            }

            return Json(new {data = objUserList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            if(objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            _db.SaveChanges();

            return Json(new { success = true, message = "User updated successfully" });
        }
        #endregion
    }
}
