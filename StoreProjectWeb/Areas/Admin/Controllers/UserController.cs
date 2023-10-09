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
using Stripe;

namespace StoreProjectWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(IUnitOfWork unitOfWork, ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string? id)
        {
            UserVM userVM = new()
            {
                ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == id, includeProperties: "Company"),
                CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.CompanyId.ToString(),
                }),
                RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i,
                })
            };

            userVM.ApplicationUser.Role = _userManager.GetRolesAsync(userVM.ApplicationUser).GetAwaiter().GetResult().FirstOrDefault();
            return View(userVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(UserVM userVM)
        {
            string oldRole = _userManager.GetRolesAsync(userVM.ApplicationUser).GetAwaiter().GetResult().FirstOrDefault();
            
            ApplicationUser userFromDb = _unitOfWork.ApplicationUser.Get(u => u.Id == userVM.ApplicationUser.Id);

            if(userVM.ApplicationUser.Role != oldRole)
            {

                if (userVM.ApplicationUser.Role == SD.Role_Company)
                    userFromDb.CompanyId = userVM.ApplicationUser.CompanyId;
                else
                    userFromDb.CompanyId = null;
                

                _unitOfWork.ApplicationUser.Update(userFromDb);
                _unitOfWork.Save();

                _userManager.RemoveFromRoleAsync(userFromDb, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(userFromDb, userVM.ApplicationUser.Role).GetAwaiter().GetResult();

            }
            else
            {
                if(oldRole == SD.Role_Company && userFromDb.CompanyId != userVM.ApplicationUser.CompanyId)
                {
                    userFromDb.CompanyId=userVM.ApplicationUser.CompanyId;
                    _unitOfWork.ApplicationUser.Update(userFromDb);
                    _unitOfWork.Save();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _unitOfWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();

            foreach(var user in objUserList)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

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
            var objFromDb = _unitOfWork.ApplicationUser.Get(u => u.Id == id);

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

            _unitOfWork.ApplicationUser.Update(objFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "User updated successfully" });
        }
        #endregion
    }
}
